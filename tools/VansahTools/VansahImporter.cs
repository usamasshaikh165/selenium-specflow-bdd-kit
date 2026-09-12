using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Vansah.Tools
{
    public class ImportResult
    {
        public string                    FeatureFile { get; set; }
        public List<string>              Succeeded   { get; set; } = new List<string>();
        public List<string>              Skipped     { get; set; } = new List<string>();
        public List<string>              Failed      { get; set; } = new List<string>();
        public Dictionary<string,string> KeysByTitle { get; set; } = new Dictionary<string, string>();
    }

    public class VansahImporter
    {
        // Vansah REST API version segment. Vansah migrated testCase endpoints from v1 → v2;
        // calls on /api/v1 stopped working after their platform upgrade.
        private const string ApiVersion = "v2";

        private readonly VansahConfig _config;
        private static readonly HttpClient _httpClient = new HttpClient();

        // Builds a fully-qualified API URL: {base}/api/v2/{relativePath}
        private string ApiUrl(string relativePath) =>
            $"{_config.VansahApiUrl.TrimEnd('/')}/api/{ApiVersion}/{relativePath.TrimStart('/')}";

        // Tags that map to Vansah priority — excluded from labels
        private static readonly Dictionary<string, int> PriorityMap =
            new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
            {
                ["High"]     = 2,
                ["Medium"]   = 3,
                ["Low"]      = 4,
                ["Critical"] = 10000,
                ["Highest"]  = 1,
            };

        public VansahImporter(VansahConfig config)
        {
            _config = config;
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", _config.VansahToken);
        }

        // ── Public entry point ────────────────────────────────────────────────────

        public async Task<ImportResult> ImportFeatureFileAsync(string featureFilePath)
        {
            if (!File.Exists(featureFilePath))
                throw new FileNotFoundException($"Feature file not found: {featureFilePath}");

            var scenarios = ParseScenarios(File.ReadAllText(featureFilePath));
            var result    = new ImportResult { FeatureFile = featureFilePath };

            Console.WriteLine($"\n[Vansah] Parsed {scenarios.Count} scenario(s) from: {Path.GetFileName(featureFilePath)}");
            Console.WriteLine($"[Vansah] Target folder : {_config.FolderIdentifier}");
            Console.WriteLine($"[Vansah] Project key   : {_config.ProjectKey}\n");

            foreach (var scenario in scenarios)
            {
                // Idempotency guard: if this scenario already carries a Vansah case-key tag
                // (e.g. @BO-C8649) written by a previous run, skip it so re-running the
                // importer on the same feature file never creates duplicate test cases.
                var existingKey = FindExistingCaseKey(scenario.Tags);
                if (existingKey != null)
                {
                    Console.WriteLine($"  [SKIP] {scenario.Title}  →  already imported as {existingKey}");
                    result.Skipped.Add(scenario.Title);
                    continue;
                }

                var (success, key) = await CreateTestCaseAsync(scenario);
                if (success)
                {
                    result.Succeeded.Add(scenario.Title);
                    if (!string.IsNullOrEmpty(key) && key != "N/A")
                        result.KeysByTitle[scenario.Title] = key;
                }
                else
                {
                    result.Failed.Add(scenario.Title);
                }
            }

            Console.WriteLine($"\n[Vansah] Done — {result.Succeeded.Count} created, {result.Skipped.Count} skipped, {result.Failed.Count} failed.\n");

            // Stamp the returned Vansah keys back into the feature file so subsequent runs skip them.
            if (result.KeysByTitle.Count > 0)
                PatchFeatureFile(featureFilePath, result.KeysByTitle);

            return result;
        }

        // ── Gherkin parser ────────────────────────────────────────────────────────

        private List<ScenarioData> ParseScenarios(string content)
        {
            var scenarios    = new List<ScenarioData>();
            var lines        = content.Split(new[] { '\r', '\n' }, StringSplitOptions.None);
            ScenarioData current     = null;
            bool         inExamples  = false;
            var          pendingTags = new List<string>();
            var          pendingDesc = new List<string>();
            string       pendingPrec = null;

            foreach (var raw in lines)
            {
                var line = raw.Trim();
                if (string.IsNullOrWhiteSpace(line)) continue;

                // ── Tag lines ─────────────────────────────────────────────────────
                if (line.StartsWith("@"))
                {
                    foreach (var token in line.Split(' '))
                        if (token.StartsWith("@")) pendingTags.Add(token.TrimStart('@'));
                    continue;
                }

                // ── Comment lines ─────────────────────────────────────────────────
                if (line.StartsWith("#"))
                {
                    if (line.Contains("──")) continue;

                    var precMatch = Regex.Match(line, @"^#\s*Precondition:\s*(.+)$", RegexOptions.IgnoreCase);
                    if (precMatch.Success) { pendingPrec = precMatch.Groups[1].Value.Trim(); continue; }

                    var descMatch = Regex.Match(line, @"^#\s*(?:Test Case Summary|Description):\s*(.+)$", RegexOptions.IgnoreCase);
                    if (descMatch.Success) { pendingDesc.Add(descMatch.Groups[1].Value.Trim()); continue; }

                    continue;
                }

                // ── Feature / Background reset ────────────────────────────────────
                if (Regex.IsMatch(line, @"^(Feature:|Background:)", RegexOptions.IgnoreCase))
                {
                    pendingTags.Clear();
                    pendingDesc.Clear();
                    pendingPrec = null;
                    current     = null;
                    continue;
                }

                if (line.StartsWith("Examples:")) { inExamples = true; continue; }

                // ── Scenario / Scenario Outline header ────────────────────────────
                if (Regex.IsMatch(line, @"^Scenario(?: Outline)?:", RegexOptions.IgnoreCase))
                {
                    if (current != null) scenarios.Add(current);
                    inExamples = false;
                    current = new ScenarioData
                    {
                        Title        = Regex.Replace(line, @"^Scenario(?: Outline)?:\s*", "", RegexOptions.IgnoreCase).Trim(),
                        Tags         = new List<string>(pendingTags),
                        Description  = pendingDesc.Count > 0 ? string.Join("\n", pendingDesc) : null,
                        Precondition = pendingPrec
                    };
                    pendingTags.Clear();
                    pendingDesc.Clear();
                    pendingPrec = null;
                    continue;
                }

                if (current == null) continue;

                // ── Example table rows ────────────────────────────────────────────
                if (inExamples)
                {
                    if (line.StartsWith("|")) current.ExampleRows.Add(line);
                    continue;
                }

                // ── Step lines ────────────────────────────────────────────────────
                if (Regex.IsMatch(line, @"^(Given|When|Then|And|But|\*)\s", RegexOptions.IgnoreCase))
                    current.Steps.Add(line);
            }

            if (current != null) scenarios.Add(current);
            return scenarios;
        }

        // ── Idempotency: detect a Vansah key already stamped on a scenario ─────────

        // Returns the Vansah case key if the scenario's tags already contain one written by a
        // previous import (e.g. "@BO-C8649"). Tags are stored with the leading '@' stripped, so
        // we match the bare form "{ProjectKey}-C{digits}", case-insensitively; otherwise null.
        private string FindExistingCaseKey(List<string> tags)
        {
            if (tags == null || string.IsNullOrWhiteSpace(_config.ProjectKey))
                return null;

            var prefix = _config.ProjectKey + "-C"; // the "C" marks a Vansah test case key
            foreach (var tag in tags)
            {
                // The QA Codegen bot stamps case keys with its own prefix convention
                // ("VansaCase-PROJ-C1234") while this importer historically stamped bare
                // "PROJ-C1234" - accept either form. Without this, a bot-tagged feature file
                // looks entirely unimported and every scenario gets duplicated in Vansah
                // (near-miss caught 2026-09-01 on ProxyRecipientAudienceSelector.feature).
                var candidate = tag.StartsWith("VansaCase-", StringComparison.OrdinalIgnoreCase)
                    ? tag.Substring("VansaCase-".Length)
                    : tag;
                if (candidate.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                {
                    var suffix = candidate.Substring(prefix.Length);
                    if (suffix.Length > 0 && int.TryParse(suffix, out _))
                        return candidate;
                }
            }
            return null;
        }

        // ── Step 1: Create test case ──────────────────────────────────────────────

        private async Task<(bool success, string key)> CreateTestCaseAsync(ScenarioData scenario)
        {
            // Resolve priority and labels from tags
            int?         priorityJiraId = null;
            var          labels         = new List<string>();

            foreach (var tag in scenario.Tags)
            {
                if (PriorityMap.TryGetValue(tag, out int jiraId))
                    priorityJiraId = jiraId;
                else
                    labels.Add(tag);
            }

            var body = new JObject
            {
                ["headline"]     = scenario.Title,
                ["precondition"] = !string.IsNullOrWhiteSpace(scenario.Precondition) ? scenario.Precondition : "N/A",
                ["folder"]       = new JArray { new JObject { ["identifier"] = _config.FolderIdentifier } }
            };
            // v2: no top-level "project" — the workspace/project is inferred from the token.

            if (!string.IsNullOrWhiteSpace(_config.TypeIdentifier))
                body["type"] = new JObject { ["identifier"] = _config.TypeIdentifier };

            if (!string.IsNullOrWhiteSpace(scenario.Description))
                body["description"] = scenario.Description;

            if (labels.Count > 0)
                body["label"] = new JArray(labels.ToArray());

            if (priorityJiraId.HasValue)
                body["priority"] = new JObject { ["jiraID"] = priorityJiraId.Value.ToString() };

            Console.WriteLine($"  [DEBUG] {scenario.Title}");
            Console.WriteLine($"    description  : {scenario.Description ?? "(none)"}");
            Console.WriteLine($"    precondition : {scenario.Precondition ?? "(none)"}");
            Console.WriteLine($"    labels       : {(labels.Count > 0 ? string.Join(", ", labels) : "(none)")}");
            Console.WriteLine($"    priority     : {(priorityJiraId.HasValue ? priorityJiraId.Value.ToString() : "(none)")}");

            var httpContent = new StringContent(body.ToString(Formatting.Indented), Encoding.UTF8, "application/json");

            try
            {
                var url      = ApiUrl("testCase");
                var response = await _httpClient.PostAsync(url, httpContent);
                var raw      = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var parsed     = JObject.Parse(raw);
                    var key        = parsed["data"]?["key"]?.ToString() ?? "N/A";
                    var identifier = parsed["data"]?["identifier"]?.ToString();

                    Console.WriteLine($"    caseKey      : {key}");

                    if (!string.IsNullOrEmpty(identifier) && scenario.Steps.Count > 0)
                        await SetBddStepsAsync(key, identifier, scenario);

                    Console.WriteLine($"  [OK]   {scenario.Title}  →  {key}");
                    return (true, key);
                }
                else
                {
                    Console.WriteLine($"  [FAIL] {scenario.Title}");
                    Console.WriteLine($"         HTTP {(int)response.StatusCode}: {raw}");
                    return (false, null);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  [ERROR] {scenario.Title} — {ex.Message}");
                return (false, null);
            }
        }

        // ── Steps 2-3 : Set BDD script type, then create the BDD script with steps ──
        //   v2 collapses the old 3-call flow: the "Add BDD" POST accepts bddData directly,
        //   so the separate empty-create + PUT-the-steps pair is no longer needed.

        private async Task SetBddStepsAsync(string caseKey, string testCaseIdentifier, ScenarioData scenario)
        {
            // Step 2: PUT — mark test case script type as BDD
            var setTypeBody    = new JObject
            {
                ["scriptType"] = "bdd",
                ["project"]    = new JObject { ["key"] = _config.ProjectKey }
            };
            var setTypeContent = new StringContent(setTypeBody.ToString(Formatting.None), Encoding.UTF8, "application/json");
            await _httpClient.PutAsync(ApiUrl($"testCase/{testCaseIdentifier}"), setTypeContent);

            // Step 3: POST "Add BDD" — create the BDD script WITH the Given/When/Then steps inline
            var bddBody = new JObject
            {
                ["bddData"]    = BuildStepsText(scenario),
                ["scriptType"] = "bdd",
                ["project"]    = new JObject { ["key"] = _config.ProjectKey }
            };
            var bddContent = new StringContent(bddBody.ToString(Formatting.None), Encoding.UTF8, "application/json");
            var bddUrl     = ApiUrl($"testCase/{testCaseIdentifier}/testScript");
            var bddResp    = await _httpClient.PostAsync(bddUrl, bddContent);
            var bddRaw     = await bddResp.Content.ReadAsStringAsync();

            if (bddResp.IsSuccessStatusCode)
            {
                var scriptId = JObject.Parse(bddRaw)?["data"]?["identifier"]?.ToString();
                Console.WriteLine($"    testScript   : saved (bdd, scriptId: {scriptId})");
            }
            else
            {
                Console.WriteLine($"  [WARN] BDD steps not set for {caseKey}: HTTP {(int)bddResp.StatusCode}: {bddRaw}");
            }
        }

        // ── Build steps text ──────────────────────────────────────────────────────

        private string BuildStepsText(ScenarioData scenario)
        {
            var sb = new StringBuilder();

            foreach (var step in scenario.Steps)
                sb.AppendLine($"    {step.Replace("<", "&lt;").Replace(">", "&gt;")}");

            if (scenario.ExampleRows.Count > 0)
            {
                sb.AppendLine();
                sb.AppendLine("    Examples:");
                foreach (var row in scenario.ExampleRows)
                    sb.AppendLine($"      {row}");
            }

            return sb.ToString().TrimEnd();
        }

        // ── Patch feature file with returned Vansah keys ─────────────────────────

        // After a successful import, stamp each scenario's tag line with its Vansah key
        // (e.g. appends " @BO-C8649"). On later runs FindExistingCaseKey reads these back
        // and skips the scenario, preventing duplicate test cases. Idempotent.
        private void PatchFeatureFile(string featureFilePath, Dictionary<string, string> keysByTitle)
        {
            var patched    = new List<string>(File.ReadAllLines(featureFilePath));
            int patchCount = 0;

            for (int i = 0; i < patched.Count; i++)
            {
                var trimmed = patched[i].Trim();

                // Match the header exactly as ParseScenarios does (case-insensitive regex).
                if (!Regex.IsMatch(trimmed, @"^Scenario(?: Outline)?:", RegexOptions.IgnoreCase))
                    continue;

                var title = Regex.Replace(trimmed, @"^Scenario(?: Outline)?:\s*", "", RegexOptions.IgnoreCase).Trim();
                if (!keysByTitle.TryGetValue(title, out var key)) continue;

                // Walk backwards past blank lines to the nearest tag line.
                int tagIdx = i - 1;
                while (tagIdx >= 0 && string.IsNullOrWhiteSpace(patched[tagIdx]))
                    tagIdx--;

                if (tagIdx >= 0 && patched[tagIdx].TrimStart().StartsWith("@"))
                {
                    // There is an existing tag line — append the key to it (unless already present).
                    if (patched[tagIdx].Contains("@" + key)) continue;
                    patched[tagIdx] = patched[tagIdx].TrimEnd() + " @" + key;
                    patchCount++;
                }
                else
                {
                    // No tag line above this scenario — insert one, preserving the header's indentation.
                    var indent = patched[i].Substring(0, patched[i].Length - patched[i].TrimStart().Length);
                    patched.Insert(i, indent + "@" + key);
                    patchCount++;
                    i++; // header shifted down by one; keep index aligned
                }
            }

            if (patchCount > 0)
            {
                File.WriteAllLines(featureFilePath, patched);
                Console.WriteLine($"[Vansah] Feature file patched — {patchCount} scenario tag(s) updated.");
                Console.WriteLine($"[Vansah] File: {Path.GetFileName(featureFilePath)}\n");
            }
        }

        // ── Inner types ───────────────────────────────────────────────────────────

        private class ScenarioData
        {
            public string       Title        { get; set; }
            public string       Description  { get; set; }
            public string       Precondition { get; set; }
            public List<string> Steps        { get; set; } = new List<string>();
            public List<string> ExampleRows  { get; set; } = new List<string>();
            public List<string> Tags         { get; set; } = new List<string>();
        }
    }
}
