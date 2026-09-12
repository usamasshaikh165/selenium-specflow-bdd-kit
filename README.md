# selenium-specflow-bdd-kit

A starter kit for web test automation in **C#**: **Selenium WebDriver + SpecFlow + NUnit**, a reusable **control library driven by a JSON locator repository**, an **Extent HTML report** with per-step screenshots, a strict **BDD governance rulebook**, **AI-assisted scenario generation** (Claude Code commands), and a **Vansah** importer that pushes scenarios into Jira test management.

The sample suite runs against the public [Sauce Demo](https://www.saucedemo.com) site so the kit works out of the box. Replace the sample features, steps, and locators with your own application's and keep everything else.

## What is in the box

| Path | Purpose |
|---|---|
| `src/WebControls/` | The framework core. `IControl` interfaces and implementations for buttons, text boxes, labels, check boxes, combo boxes, combo lists, grids, grid headers, and calendars. `ControlFactory` builds them from `Locators.json` tokens. `GeneralHelper` holds fluent and explicit waits. `Diagnostics/Logger` is a small console + file logger. |
| `tests/SampleTests/` | SpecFlow + NUnit test project. `Support/Browser` owns one ChromeDriver per scenario and exposes token-based helpers. `Support/TestSettings` reads URLs and accounts from environment variables. `Hooks/` wire browser cleanup and Extent reporting. `StepDefinitions/` and `FeatureFiles/` are the samples. `Locators.json` is the locator repository. |
| `tools/VansahTools/` | NUnit project that parses a feature file and creates BDD test cases in Vansah through its REST API. |
| `CLAUDE.md` | The automation rulebook: C# rules, Selenium practices, SpecFlow and NUnit conventions, naming, review checklists, and Section 9, the Gherkin/Vansah governance every `.feature` file must follow. Claude Code reads it automatically. |
| `.claude/commands/` | Slash commands for Claude Code: generate a fully covered feature file plus step definitions from a Jira story, optionally import it to Vansah. |
| `docs/csharp-rulebook.md` | Numbered C# coding standards the Copilot PR review cites by rule ID. |
| `docs/MCP-SETUP-GUIDE.md` | How each team member connects Claude Code to Jira through the Atlassian MCP server. |
| `.github/` | CI workflow, PR template, and Copilot instruction files for .NET repositories. |

## Quick start

Requirements: .NET 8 SDK and Google Chrome. Selenium Manager downloads a matching chromedriver automatically.

```bash
dotnet restore SeleniumBddKit.sln
dotnet build SeleniumBddKit.sln
dotnet test tests/SampleTests/SampleTests.csproj                                   # everything
dotnet test tests/SampleTests/SampleTests.csproj --filter "TestCategory=smokeBDD"  # by tag
HEADLESS=1 dotnet test tests/SampleTests/SampleTests.csproj                        # no browser window
OPEN_REPORT=1 dotnet test tests/SampleTests/SampleTests.csproj                     # open the Extent report afterwards
```

The Extent report and step screenshots are written to `tests/SampleTests/bin/<Configuration>/net8.0/Reports/`.

## Configuration

Everything environment-specific is read from environment variables by `TestSettings`. Nothing sensitive lives in source.

| Variable | Meaning | Default |
|---|---|---|
| `BASE_URL` | Application under test | `https://www.saucedemo.com/` |
| `DEFAULT_USER_EMAIL`, `DEFAULT_USER_PASSWORD` | Standard test account | Sauce Demo `standard_user` |
| `LOCKED_USER_EMAIL`, `LOCKED_USER_PASSWORD` | Account the app refuses to sign in | Sauce Demo `locked_out_user` |
| `HEADLESS` | `1` runs Chrome headless | `1` under CI, else `0` |
| `OPEN_REPORT` | `1` opens the Extent report when the run ends | `0` |
| `LOCATOR_FILE` | Path to the locator repository | `Locators.json` next to the test binary |
| `LOG_DIR` | If set, the logger also writes a per-run file there | unset |

## How the framework fits together

1. **Locators.json** maps a token name to `ControlType`, `IdentifierType`, and `Identifier`:

   ```json
   "LoginButton": { "ControlType": "Button", "IdentifierType": "Id", "Identifier": "login-button" }
   ```

   Supported control types: `Button`, `Textbox`, `Label`, `CheckBox`, `Combobox`, `ComboList`, `Grid`, `GridHeader`, `Calendar`. Supported identifier types: `Id`, `Name`, `XPath`, `CssSelector`, `ClassName`, `TagName`, `LinkText`. An `Identifier` may contain `{0}` for parameterised lookups such as a row or card matched by its visible text.

2. **Step definitions** never touch `By` or `IWebElement` directly. They ask the injected `Browser` for a control by token and call its typed methods:

   ```csharp
   Assert.AreEqual(Global.SUCCESS, Browser.Control<ITextControl>("LoginUsername").FillText(user));
   Assert.AreEqual(Global.SUCCESS, Browser.Control<IButtonControl>("AddToCartButton", "Sauce Labs Backpack").ClickButton());
   Browser.WaitUntilVisible("ProductsTitle");
   ```

   Control methods return `Global.SUCCESS` or `Global.FAILURE` and log the reason, so a failed step shows what the framework could not find.

3. **Hooks** create one `Browser` per scenario through SpecFlow context injection, quit it after every scenario regardless of outcome, and record each step with a screenshot in the Extent report.

## Writing tests the kit's way

1. **Start from the feature file.** Write or generate `tests/SampleTests/FeatureFiles/<Feature>/<Feature>.feature` following Section 9 of `CLAUDE.md`: a `# Test Case Summary:` and `# Precondition:` line above every scenario, then tags in this exact order:

   ```
   @<SmokeAutomatable|SmokeAutomated|RegressionAutomatable|RegressionAutomated|NonAutomatable> @<High|Medium|Low> @<STORY_ID> [@BusinessCase] @ClaudeGeneratedTest @smokeBDD
   ```

   `Automatable` means the scenario can be automated but the steps are not implemented yet. `Automated` means the step definitions exist. `NonAutomatable` scenarios also get `@ignore` so SpecFlow skips them. Group scenarios under the standard section comments (Happy Path, Role-Based Access, Business Rules & Restrictions, Validation / Negative, Edge Cases, Workflow States, End-to-End).

2. **Add locators** to `Locators.json` for every new element. Capture them from the live application, never from memory.

3. **Write the step class** in `tests/SampleTests/StepDefinitions/`, inheriting `BaseSteps`. One method per Gherkin step, matching the text exactly. Put shared flows in `BaseSteps`, driver-level helpers in `Browser`, and new settings in `TestSettings`.

4. **Run the review checklists** in `CLAUDE.md` Section 8 (code) and Section 9.8 (BDD) before opening a PR.

## Generating scenarios with Claude Code

With [Claude Code](https://claude.com/claude-code) open in this repository and the Atlassian MCP configured (see `docs/MCP-SETUP-GUIDE.md`):

| Command | What it does |
|---|---|
| `/generate-bdd` | Asks for a Jira story and file locations, reads the story, builds the coverage matrix, presents every candidate scenario for you to label, writes the feature file and step definitions, adds locator placeholders, and updates `VansahConfig.json`. Import is left for you to run after review. |
| `/generate-bdd-import` | Same, then runs the Vansah import immediately. |
| `/vansah-import` | Imports an existing feature file using the current `VansahConfig.json`. |

## Importing to Vansah

1. Copy `VansahConfig.example.json` to `VansahConfig.json` at the repository root and fill in the API URL, connect token, project key, folder UUID, and the feature file to import. The real file is gitignored; never commit a token.
2. Run the importer:

   ```bash
   dotnet test tools/VansahTools/VansahTools.csproj --filter "Category=VansahImport" --logger "console;verbosity=detailed"
   ```

The importer creates one Vansah test case per scenario. `# Test Case Summary:` becomes the summary, `# Precondition:` the precondition, `@High/@Medium/@Low` the priority, all other tags become labels, and the Given/When/Then steps plus any `Examples:` table become the BDD script.

## Adapting the kit to your application

- Replace the two sample feature folders, `LoginSteps.cs`, `CartSteps.cs`, and the entries in `Locators.json`. Keep `BaseSteps`, `Browser`, `TestSettings`, and the hooks.
- Point `BASE_URL` and the account variables at your environment, locally through your shell and in CI through repository secrets.
- Change the story tag prefix (`@SBK-…`) in `CLAUDE.md` Section 9 and in the `.claude/commands/` examples to your Jira project key.
- If your application uses a specific UI library, add its selector conventions to `CLAUDE.md` so generated locators follow them.

## Notes on this version

- Targets .NET 8. The original code base targeted .NET Core 3.1, which is end of life.
- The proprietary logging assembly the controls used to depend on is replaced by `WebControls.Diagnostics.Logger`, which keeps the same call shapes.
- The Windows-only input simulator was removed from the combo-list control; scrolling is done with JavaScript instead, so the library builds and runs on macOS and Linux as well as Windows.
