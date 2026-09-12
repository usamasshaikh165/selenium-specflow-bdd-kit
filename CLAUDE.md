# CLAUDE.md — Web Automation Rulebook (C# · Selenium · SpecFlow · NUnit)
> **Stack:** C# · Selenium WebDriver · SpecFlow · NUnit
> **Scope:** Best practices, language rules, naming conventions, BDD governance, review checklists


## SECTION 1 — General Automation Best Practices


### 1.1 Test Design Principles

Every test must be independent and self-contained — no test should depend on the outcome or state of another test.

Tests must be idempotent — running the same test multiple times must always produce the same result.

Follow the AAA pattern (Arrange → Act → Assert) in every scenario.

Each scenario must validate one user behaviour — avoid bundling multiple unrelated journeys.

Tests must represent real user interactions — avoid asserting DOM structure; assert visible outcomes.

Tests must be resilient to minor UI changes — locators must not break when styling changes.


### 1.2 Web Test Coverage Standards

Every user journey must have a happy path test, validation failure tests, and boundary/edge case tests.

Never assert implementation details (CSS classes, DOM attributes) — assert what the user sees and experiences.

Always assert navigation outcome (URL change, page heading) after major user actions.

Validate both presence (element exists and is visible) and content (text is correct) in UI assertions.


### 1.3 Test Data Management

Never hardcode test data (usernames, passwords, test records) in step definitions or test classes.

Use deserialised JSON files or builder pattern data objects for all test input data.

Dynamic data (emails, timestamps, reference numbers) must be generated at runtime.

Use API calls or direct DB setup (never UI) to seed prerequisite test state.

Tear down created data via API or DB in [AfterScenario] hooks — never rely on UI cleanup.

Never use production data in automated tests.


### 1.4 Test Execution

All tests must be executable from the command line and from within Visual Studio.

Support tag-based execution (@smoke, @regression) for selective CI runs.

All tests must support parallel execution — driver instances must never be shared across threads.

Environment must be selectable at runtime via an environment variable or CLI argument.


## SECTION 2 — C# Language Rules


### 2.1 Code Quality

Use C# 10+ features where applicable — file-scoped namespaces, global usings, record types, pattern matching.

Declare all fields as private readonly unless mutation is explicitly required.

Constants must be private const or public static readonly — never magic strings or numbers inline.

Use string interpolation ($"...") over string.Format() or concatenation.

Use expression-bodied members for single-line properties and simple methods.

Use the null-conditional operator (?.) and null-coalescing operator (??) — never null checks with if (x == null).

Use nameof() instead of hardcoded strings when referring to members (e.g., in logging, exceptions).



// ✅ Modern C# style
private readonly IWebDriver _driver;
private const string LoginPath = "/login";
public string PageTitle => _driver.Title;
public string GetWelcomeMessage()
=> _driver.FindElement(WelcomeHeading)?.Text
?? throw new ElementNotFoundException(nameof(WelcomeHeading));

### 2.2 Async and Threading

Use async/await for any I/O-bound setup helpers (API seeding, DB calls) — never .Result or .Wait().

Use [ThreadStatic] or ThreadLocal<T> for any shared state that must be isolated per thread.

Never use Task.Delay() or Thread.Sleep() as a wait strategy in tests.


### 2.3 Null Safety

Enable <Nullable>enable</Nullable> in the project file — treat all nullable warnings as errors in CI.

Use nullable reference types (string?) explicitly — never suppress warnings without justification.

Always validate method parameters for null and throw ArgumentNullException with nameof().



// ✅ Null guard
public void EnterText(string text)
{
ArgumentNullException.ThrowIfNull(text, nameof(text));
...
}

### 2.4 Exception Handling

Never catch Exception silently in test helpers — always rethrow or fail the test with context.

Create custom exception types for domain-specific failures (ElementNotFoundException, PageLoadException).

Always include the failing element locator or action in exception messages.


### 2.5 Logging

Use Microsoft.Extensions.Logging or Serilog — never Console.WriteLine().

Log at Debug for element interactions, Information for lifecycle events, Error for failures.

Never log passwords, tokens, or PII — mask before logging.


### 2.6 Collections and LINQ

Prefer LINQ over imperative loops for filtering, projecting, and aggregating.

Use IReadOnlyList<T> or IEnumerable<T> as return types from public methods — never concrete List<T>.

Use Any() and All() over .Count > 0 checks.


## SECTION 3 — Selenium WebDriver Best Practices


### 3.1 Driver Lifecycle

Initialise and dispose IWebDriver per scenario — never reuse across scenarios.

Use [ThreadStatic] in the driver factory for parallel-safe driver access.

Always call driver.Quit() — never driver.Close() alone — in teardown.

Never expose the raw IWebDriver instance outside of BasePage or the driver factory.


### 3.2 Locator Strategy

Locators must be declared as private static readonly By fields at the top of each Page class.

Use locators in this priority order:

By.Id — fastest and most stable

By.CssSelector — preferred over XPath for structural selectors

By.Name

By.LinkText / By.PartialLinkText — only for anchor elements

By.XPath — only when CSS cannot express the query; always use relative XPath

Never use positional XPath (/html/body/div[1]/...).

Never use brittle CSS selectors based on generated class names (e.g., .css-abc123).

Advocate for data-testid attributes in the application — use By.CssSelector("[data-testid='submit-btn']").


### 3.3 Waiting Strategy

Never use ImplicitWait — it conflicts with WebDriverWait and produces unreliable results.

Always use explicit waits via WebDriverWait with ExpectedConditions.

Centralise all wait logic in BasePage or a WaitHelper — never inline new WebDriverWait(...) in page methods.

Default wait timeout must be read from config — never a magic number.

Use ExpectedConditions.ElementToBeClickable before clicking — not just ElementIsVisible.



// ✅ Centralised wait in BasePage
protected IWebElement WaitForClickable(By locator)
=> new WebDriverWait(_driver, TimeSpan.FromSeconds(_config.ExplicitWaitSeconds))
.Until(ExpectedConditions.ElementToBeClickable(locator));
protected IWebElement WaitForVisible(By locator)
=> new WebDriverWait(_driver, TimeSpan.FromSeconds(_config.ExplicitWaitSeconds))
.Until(ExpectedConditions.ElementIsVisible(locator));

### 3.4 Page Object Principles

Every Page Object must extend BasePage.

BasePage owns: the driver reference, wait methods, common interaction helpers (Click, Type, GetText).

Page Objects return values and other page objects — they never contain assertions.

Use method chaining (return this) for sequential actions on the same page.

Never put navigation (driver.Navigate().GoToUrl(...)) inside page constructors — use explicit NavigateTo() methods.


### 3.5 JavaScript Executor

Use JavaScript execution (IJavaScriptExecutor) only as a last resort — prefer native Selenium interactions.

Never scroll to an element using JS when WebDriverWait with ElementToBeClickable would suffice.

Always document why JS execution is needed with a comment.


## SECTION 4 — SpecFlow / BDD Best Practices


### 4.1 Feature File Writing

Write all Gherkin from the user's perspective — never from the developer's or tester's perspective.

Background: applies only to steps common to every scenario — use sparingly.

Use Scenario Outline: with Examples: for data-driven coverage — never copy-paste scenarios.

Limit scenarios to 5–7 steps — extract common sequences into helper steps.

Steps must describe intent, not implementation — no CSS selectors, IDs, or method names.

Use the Rule: keyword to group related scenarios within a feature when applicable.



# ✅ Intent-driven Gherkin
Scenario: Logged-in user accesses account settings
Given the user is authenticated with a standard account
When the user navigates to account settings
Then the account settings page should be displayed
And all sections should be accessible
# ❌ Implementation-driven — never do this
Scenario: Click settings link and verify div
Given the driver navigates to "/login"
When I click element with id "settings-link"
Then div with class "settings-panel" should be visible

### 4.2 Step Definitions

Inject ScenarioContext via SpecFlow's built-in dependency injection — never static state.

One binding class per feature domain — do not create one massive bindings file.

Delegate all page interactions to Page Objects — never call Selenium directly from step methods.

Use [Given], [When], [Then] annotations correctly — do not use [When] for assertions.

Define shared/reusable steps in a CommonSteps binding class.


### 4.3 Hooks

Use [BeforeScenario] and [AfterScenario] with explicit Order values.

Use [BeforeFeature]/[AfterFeature] only for feature-wide (not scenario-wide) setup.

Capture screenshots on failure inside [AfterScenario] — check ScenarioContext.TestError.

Attach screenshots to reports — do not only save to disk.


### 4.4 Tags and Organisation

Every scenario must have at minimum: one suite tag (@smoke/@regression) and one work item tag (@PROJ-123).

@ignore disabled scenarios with a reason comment — never silently comment out scenarios.

Use @browser:chrome, @browser:firefox tags to drive cross-browser execution when applicable.


## SECTION 5 — NUnit Best Practices


### 5.1 Assertions

Always use the constraint model: Assert.That(actual, Is.<Constraint>) — never classic Assert.AreEqual.

Use Assert.Multiple() to collect all assertion failures in a single test rather than stopping at the first.

Always provide a failure message as the last argument to Assert.That().

Use StringAssert, CollectionAssert utilities from NUnit for specific type assertions.



// ✅ NUnit constraint model
Assert.That(pageTitle, Is.EqualTo("Dashboard"), "Page title after login was incorrect.");
Assert.That(items, Has.Count.GreaterThan(0), "Items list should not be empty.");
Assert.That(errorMessage, Does.Contain("required"), "Validation message mismatch.");
// ✅ Multiple assertions
Assert.Multiple(() => {
Assert.That(user.Name, Is.EqualTo("John Doe"));
Assert.That(user.Role, Is.EqualTo("Admin"));
Assert.That(user.IsActive, Is.True);
});

### 5.2 Parallelism

Apply [Parallelizable(ParallelScope.Fixtures)] at the assembly level in AssemblyInfo.cs or via [assembly: Parallelizable].

Never share mutable state between fixtures — each fixture must be completely isolated.

Use [Order] only when absolutely necessary — prefer independent tests.


## SECTION 6 — Naming Conventions


### 6.1 C# Class and Type Naming

Type

Convention

Example

Page Object

PascalCase + Page

LoginPage, CheckoutPage

UI Component

PascalCase + Component

NavbarComponent, DatePickerComponent

Step Definitions

PascalCase + Steps

LoginSteps, CheckoutSteps

Driver Factory

DriverFactory

DriverFactory

Test Data Model

PascalCase + Data

UserData, OrderData

Config Class

PascalCase + Config or Settings

TestSettings, BrowserConfig

Custom Exception

PascalCase + Exception

ElementNotFoundException

Enum

PascalCase (singular noun)

Browser, UserRole

Interface

I + PascalCase

IPage, IDriverFactory

Hook Class

PascalCase + Hooks

ScenarioHooks, FeatureHooks


### 6.2 Method Naming

Method Type

Convention

Example

Page action

Verb + Noun (PascalCase)

EnterUsername(), ClickSubmit()

Page query

Get + Noun or Is + Condition

GetPageTitle(), IsErrorDisplayed()

Navigation

NavigateTo + Destination

NavigateToLogin()

Page returns page

Indicates returned page

ClickLogin() returns DashboardPage

Assertion helper

Assert + Condition

AssertWelcomeMessageContains()

Step method

Descriptive past/present action

WhenUserEntersValidCredentials()


### 6.3 Variable and Field Naming

Instance fields: _camelCase with underscore prefix (e.g., _driver, _config).

Constants: PascalCase (e.g., LoginPath = "/login").

Locals and parameters: camelCase (e.g., pageTitle, expectedUrl).

Boolean fields/props: Is, Has, Can prefix (e.g., IsLoggedIn, HasPermission).

Never abbreviate unless universally understood (url, id, btn, ui).


### 6.4 Locator Field Naming

Locator fields: PascalCase noun describing the element's purpose.

Suffix with element type only when disambiguation is needed.



// ✅ Clear locator names
private static readonly By UsernameField   = By.Id("username");
private static readonly By PasswordField   = By.Id("password");
private static readonly By LoginButton     = By.CssSelector("[data-testid='login-btn']");
private static readonly By ValidationError = By.ClassName("error-message");
private static readonly By WelcomeHeading  = By.TagName("h1");

### 6.5 Feature File and Gherkin Naming

Artifact

Convention

Example

Feature file

PascalCase.feature

UserLogin.feature

Feature name

Title Case noun phrase

User Login and Authentication

Scenario name

Sentence case, active voice

User logs in with valid credentials

Step text

Present tense

the user enters valid credentials

Tag

@camelCase or @JIRA-ID

@smoke, @WEB-55

Examples column

camelCase

username, expectedError


### 6.6 Config and Data Files

Artifact

Convention

Example

Config file

appsettings.<env>.json

appsettings.staging.json

Test data file

PascalCase.json

ValidUsers.json

Environment variable

SCREAMING_SNAKE_CASE

BASE_URL, BROWSER


## SECTION 7 — Do's and Don'ts

✅ Do's
Do use WebDriverWait with ExpectedConditions for all element interactions.

Do declare all locators as private static readonly By fields.

Do return page objects from page methods to enable fluent chaining.

Do use ThreadStatic driver instances for parallel-safe execution.

Do inject ScenarioContext via constructor DI in binding classes.

Do write Gherkin in business language that product owners can read and validate.

Do use Assert.That(..., Is.<constraint>) — the constraint model.

Do use Assert.Multiple() when asserting multiple related properties.

Do attach screenshots on failure in [AfterScenario] hooks.

Do seed test state via API or DB — never via UI navigation as a precondition.

Do use <Nullable>enable</Nullable> and treat nullable warnings as errors.

Do read all config (URL, browser, timeout) from environment-appropriate config files.

Do tag every scenario with a suite tag and JIRA ticket.

Do use data-testid attributes in the application for stable automation locators.

❌ Don'ts
Don't use ImplicitWait — it combines badly with explicit waits and hides failures.

Don't use Thread.Sleep() or Task.Delay() as a wait strategy.

Don't store IWebElement as a class field — elements go stale; always find fresh.

Don't write assertions inside Page Object methods.

Don't use absolute or positional XPath selectors.

Don't share IWebDriver between threads.

Don't call Selenium directly from step definition methods — always delegate to page objects.

Don't use static mutable fields for cross-step state — use ScenarioContext.

Don't write Gherkin steps with technical language (IDs, CSS selectors, HTTP terms).

Don't suppress nullable warnings without a documented justification.

Don't skip teardown — always call driver.Quit() even on test failure.

Don't use Console.WriteLine() for logging.

Don't hardcode browser names, URLs, or credentials.

Don't create multiple step definition files without clear domain boundaries.

Don't use Assert.AreEqual() — always the constraint model.


## SECTION 8 — Code Review Checklist


### 8.1 C# Code Quality

[ ] File-scoped namespaces used (C# 10+).

[ ] No Console.WriteLine() — structured logging used throughout.

[ ] No magic strings or numbers — all constants named and readonly.

[ ] Nullable reference types enabled and warnings treated as errors.

[ ] All method parameters validated with ArgumentNullException.ThrowIfNull().

[ ] No Thread.Sleep() or Task.Delay() present anywhere.

[ ] No [ThreadStatic] misuse — driver instances are thread-isolated.

[ ] LINQ used over imperative loops where appropriate.


### 8.2 Selenium Usage

[ ] ImplicitWait is not set — explicit waits only.

[ ] All locators are private static readonly By fields at class level.

[ ] No XPath with absolute paths or positional indices.

[ ] No IWebElement stored as a class field.

[ ] All wait calls delegate to BasePage helpers — no inline new WebDriverWait(...).

[ ] ElementToBeClickable used before all click interactions.

[ ] driver.Quit() is called in teardown — not just driver.Close().

[ ] No JavaScript execution unless absolutely necessary and commented.


### 8.3 Page Object Model

[ ] All Page classes extend BasePage.

[ ] BasePage owns all wait and interaction helpers.

[ ] No assertions written inside Page Object methods.

[ ] Page action methods return this or the next Page Object.

[ ] No direct Selenium calls outside of BasePage and Page Object classes.

[ ] Navigation is done via explicit NavigateTo() methods — not in constructors.


### 8.4 SpecFlow / BDD

[ ] All step definition classes use [Binding] attribute.

[ ] ScenarioContext injected via constructor — no static state.

[ ] All interactions delegated to Page Objects from step definitions.

[ ] Feature files use business language — no technical terms in Gherkin.

[ ] Every scenario tagged with suite tag and JIRA ID.

[ ] No @wip scenarios committed to main branch.

[ ] [BeforeScenario] and [AfterScenario] have Order values.

[ ] Screenshots captured and attached on scenario failure.


### 8.5 NUnit Assertions

[ ] Assert.That(..., Is.<constraint>) used everywhere — no classic assertion methods.

[ ] Assert.Multiple() used when asserting multiple related properties.

[ ] All Assert.That() calls include a meaningful failure message.

[ ] [Parallelizable] configured at the appropriate level.


### 8.6 Test Data and Configuration

[ ] No hardcoded test data in step definitions or page objects.

[ ] All config values loaded from appsettings.<env>.json.

[ ] Dynamic data generated at runtime — no static email addresses or usernames.

[ ] Test data teardown implemented in [AfterScenario] hooks.

[ ] No credentials or tokens in committed config files — use environment variables or secrets manager.


### 8.7 General Quality

[ ] Tests are independent — no ordering dependency between scenarios.

[ ] Tests are parallel-safe — no shared mutable state across threads.

[ ] Each scenario validates a single behaviour.

[ ] All test, class, method, and variable names are descriptive and follow conventions.

[ ] No commented-out code committed.

[ ] All scenarios are passing or explicitly marked @ignore with a reason.



## SECTION 9 — BDD Test Case Governance (Gherkin / Vansah)

Applies to every .feature file in this project. Supplements Section 1.2 (coverage) and Section 4 (BDD structure)


### 8.1 Mandatory Per-Scenario

Every scenario must have this comment block immediately above its tags, in this exact order:

# Test Case Summary: <one sentence — what this scenario verifies>
# Precondition: <specific system state required before the test runs, or N/A>
Rules: - # Test Case Summary: — one sentence, present tense, states the observable outcome being verified. - # Precondition: — sourced directly from the Jira story. Must capture the full criteria before the test can execute: - Story has a Preconditions heading → use that content - Story explicitly uses the word "precondition" anywhere in Description, AC text, or Definition of Done → extract and use it. Sections labeled "Dependencies", "Assumptions", "Non-Functional Requirements", or "Notes" do NOT count — these are not preconditions. - Given clauses inside AC Gherkin steps do NOT count — they are AC test structure, not precondition declarations. The word "precondition" must appear literally in the story text. - Story has neither a Preconditions heading nor the word "precondition" anywhere in its text → prompt the user at runtime: "No precondition was found in the story. Please provide the precondition for this scenario, or type N/A if none applies." — do NOT write N/A automatically without asking - NEVER infer, assume, or invent a precondition not written in the story - These two lines map directly to Vansah's Test Case Summary and Precondition tabs on import.


### 8.2 Tag Format

Tags must appear in this exact order on the line immediately above the Scenario line:

@<SmokeAutomatable|SmokeAutomated|RegressionAutomatable|RegressionAutomated|NonAutomatable> @<High|Medium|Low> @<STORY_ID> [@BusinessCase] @ClaudeGeneratedTest @smokeBDD

CORRECTED 2026-08-27 (was previously a wrong split-tag format that caused a real Vansah auto-import bug — see CLAUDE.md's own Section 9 "Automation Backlog" history and feedback_vansah_label_format): the combined single-label format below is the ONLY correct one for this project. Never use separate @Smoke/@Regression + @Automatable/@Non-Automatable tags.

Important distinction — Labels vs Priority: - Labels (map to the Vansah Labels field): @SmokeAutomatable, @SmokeAutomated, @RegressionAutomatable, @RegressionAutomated, @NonAutomatable, @BusinessCase, @ClaudeGeneratedTest, @smokeBDD - Priority (maps to the Vansah Priority field): @High, @Medium, @Low — this is a separate Vansah concept from Labels and must be treated independently

Label rules:

Label	When to use
@SmokeAutomatable	@High (P1) scenario — can be automated, step defs not yet implemented
@SmokeAutomated	@High (P1) scenario — step definitions are fully implemented
@RegressionAutomatable	@Medium / @Low (P2/P3) scenario — can be automated, step defs not yet implemented
@RegressionAutomated	@Medium / @Low (P2/P3) scenario — step definitions are fully implemented
@NonAutomatable	Scenario requires human judgment or has an uncontrollable dependency — see decision logic below
Exactly one	Every scenario must carry exactly one of the five labels above — never two
@[STORY_ID]	Jira story number for traceability (e.g. @SBK-1234)
@BusinessCase	Optional — add only to scenarios under the "Business Rules & Restrictions" section; omit from Happy Path, Validation, Edge Cases, and E2E scenarios
@ClaudeGeneratedTest	Always present on every Claude-generated scenario — used to distinguish AI-generated test cases from manually written ones in Vansah
@smokeBDD	Always last, on every scenario — used by Vansah import tooling

Label selection — decision logic:

Step 1 — determine priority: Happy path, E2E lifecycle, data-loss risk → @High. Role restriction, validation, secondary paths → @Medium. Edge cases, cosmetic, rarely exercised → @Low.

Step 2 — determine automatable vs automated vs non-automatable (applies to Frontend, Backend, and API products):

Mark @...Automatable (not yet automated) if the outcome is observable via any of these: - UI element state, text, navigation, or visibility (Frontend) - API response body, status code, or header (API / Backend) - Database record, field value, or row existence (Backend) - Log entry, audit trail record, or event emission (Backend / API) - Email or notification content via a test hook or mailbox API (e.g. Mailosaur)

Mark @NonAutomatable if any of these apply: - Requires a human to visually judge correctness (e.g. layout aesthetics, print output) - Depends on a live third-party system with no sandbox or test hook (live payment gateway, live telecom SMS, uncontrolled external API) - Outcome is non-deterministic with no controllable seed or mock (true randomness, real-time market data) - Requires CAPTCHA or anti-bot challenge that blocks automation by design - Requires manual infrastructure access (physical server inspection, data centre check)

Mark @...Automated only when step definitions for that scenario are fully implemented (not TODO).

Default for Claude-generated tests: always use @SmokeAutomatable or @RegressionAutomatable — step defs are generated as TODO stubs. Change to @SmokeAutomated / @RegressionAutomated once implemented. If the scenario does not match any @NonAutomatable condition above, it is automatable regardless of product type.

Priority tag rules:

Priority Tag	Rule
@High	Happy path, E2E lifecycle, data-loss risk
@Medium	Role restriction, validation, secondary feature paths
@Low	Edge cases, cosmetic, rarely exercised paths
Exactly one	Every scenario must carry exactly one priority tag — never zero, never two

### 8.3 Coverage Categories

Section 1.2 requires happy path, validation, and boundary/edge cases. For Gherkin scenarios the full required coverage is:

Category	Minimum	Notes
Happy path	1	Primary success flow — tag @SmokeAutomatable/@SmokeAutomated @High
Per acceptance criterion	1 per AC	Every numbered AC in the Jira story
Per user role	1 per role	Where behaviour differs between roles
Workflow states	1 per state	Pending, Approved, Rejected, etc.
Positive data variations	1+	Valid alternate inputs that should succeed
Negative — required field missing	1 per field	Each mandatory field left blank
Negative — invalid format	1 per field	Wrong type / format input
Boundary values	3	Below limit, at limit, above limit
Role restriction	1 per rule	Action blocked for unauthorised role
Business restriction	1 per rule	Duplicate submission, guardian lock, etc.
Workflow ordering	1	Step N cannot occur before step N−1
Rejection path	1 per approver	Each approver who can reject
Edge cases	1+	Empty state, max-length, special characters
Full E2E lifecycle	1	Submission → all approvals → final state — tag @SmokeAutomatable/@SmokeAutomated @High
Data-driven (Outline)	1 outline	Minimum 3 rows in the Examples: table

### 8.4 Scenario Title Rules

Titles must be business-readable and role-explicit.
Follow the pattern: [Role] [performs action] and [observable outcome]
GOOD: Admin User submits order and receives confirmation
GOOD: Standard User applies filter and list updates immediately
GOOD: Guest User enters invalid credentials and sees error message
BAD: Test approve button / Verify scenario 3 / Check the form

### 8.5 Gherkin Discipline

Given — system state / precondition (never an action). Always required — every scenario must include a Given step that establishes the system state or precondition before the action.
When — the user action or triggering event. Always required — every scenario must have at least one When.
Then — the observable, user-facing outcome (specific — never vague like "it works"). Always required.
And — continuation of the previous keyword type.
Never jump from Given directly to Then — there must always be a When before Then.
No technical language in step text: no CSS selectors, API paths, button IDs, HTTP methods.
Steps must be unambiguous — one interpretation only; avoid words like "appropriate", "correct", "properly", "some".
Steps must be suitable for automation — outcomes must be deterministic and observable in the UI or API; avoid subjective or human-judgment-only assertions.

### 8.6 Vansah Field Mapping

Feature file element	Vansah field	Type
# Test Case Summary: <text>	Test Case Summary tab	Metadata
# Precondition: <text> (or N/A)	Precondition tab	Metadata
@High / @Medium / @Low	Priority field	Priority (not a Label)
@SmokeAutomatable / @SmokeAutomated / @RegressionAutomatable / @RegressionAutomated / @NonAutomatable	Labels field	Label (exactly one per scenario)
@BusinessCase	Labels field	Label (optional)
@ClaudeGeneratedTest	Labels field	Label (always present on Claude-generated scenarios)
@smokeBDD	Labels field	Label (always present)
Given / When / Then / And steps	Test Script — BDD - GHERKIN	Test Steps

### 8.7 Feature File Section Grouping

Use these section comments to group scenarios. Only include groups that apply to the feature:

# ── Happy Path ────────────────────────────────────────────────────
# ── Role-Based Access ─────────────────────────────────────────────
# ── Business Rules & Restrictions ────────────────────────────────
# ── Validation / Negative ────────────────────────────────────────
# ── Edge Cases ───────────────────────────────────────────────────
# ── Workflow States ───────────────────────────────────────────────
# ── End-to-End ───────────────────────────────────────────────────

### 8.8 BDD Governance Review Checklist

Mandatory components - [ ] Every scenario has # Test Case Summary: comment. - [ ] Every scenario has # Precondition: comment; value sourced from story or confirmed with user at runtime — never left blank, never auto-written as N/A without prompting. - [ ] Every scenario has exactly one priority tag (@High / @Medium / @Low) — maps to Vansah Priority field, not Labels. - [ ] Every scenario has the correct label tags (one combined label — @SmokeAutomatable/@SmokeAutomated/@RegressionAutomatable/@RegressionAutomated/@NonAutomatable, never split into separate tags — plus story ID, @ClaudeGeneratedTest, @smokeBDD).

Tag format - [ ] Tags are in the mandatory order: combined label → priority tag → story tag → @BusinessCase (if applicable) → @ClaudeGeneratedTest → @smokeBDD. - [ ] Exactly one combined label per scenario (never @Smoke/@Automatable as two separate tags) — @SmokeAutomatable or @SmokeAutomated for @High (P1); @RegressionAutomatable or @RegressionAutomated for @Medium/@Low (P2/P3); @NonAutomatable for non-automatable scenarios; decision logic per Section 8.2. - [ ] Exactly one of @High / @Medium / @Low per scenario (Priority — separate from Labels). - [ ] @smokeBDD is the last tag on every scenario (Label). - [ ] Jira story tag (@SBK-XXXX) present on every scenario.

Step quality - [ ] Every scenario includes a Given step that establishes the system state or precondition. - [ ] Every scenario has at least one When and one Then. - [ ] No scenario jumps from Given directly to Then without a When. - [ ] Steps follow a logical sequential flow — no missing or out-of-order actions. - [ ] Expected results (Then statements) are clearly and specifically defined — not vague. - [ ] No ambiguity in step language — one interpretation only; no words like "appropriate", "correct", "properly". - [ ] No technical language (selectors, HTTP methods, API paths) in step text. - [ ] Steps are suitable for automation — outcomes are deterministic and observable.

Scenario quality - [ ] Scenario aligns with and is traceable to the Jira story requirement or AC. - [ ] All data-driven cases use Scenario Outline with minimum 3 Examples: rows. - [ ] Scenario titles are business-readable and identify the role performing the action.
