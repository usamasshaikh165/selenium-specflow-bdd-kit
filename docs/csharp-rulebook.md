# C# Coding Standards (v1.0)

**Instruction for Copilot:**
When reviewing pull requests, always **cite the specific rule ID** (e.g., CS2 or CS18) and **quote the corresponding rule text** when reporting violations.

---

## 1. Naming Conventions

*These rules define how all identifiers must be named in C#.*

* **R1:** Classes, Structs → `PascalCase`.	Example: `OrderProcessor`, `UserProfile`
* **R2:** Interfaces → `IPascalCase` (prefix with `I`).	Example: `IOrderService`
* **R3:** Public methods and properties → `PascalCase`.	Example: `CalculateTotal()`
* **R4:** Local variables and method parameters → `camelCase`.	Example: `orderId`, `totalAmount`
* **R5:** Constants → `PascalCase`.	Example: `MaxRetryCount`
* **R6:** Async methods → Must end with `Async`.	Example: `GetOrdersAsync`
* **R7:** Avoid unclear or abbreviated names unless industry-standard.	`tmp`, `obj`

---

## 2. File & Class Structure

*These rules define how code files and types must be organized.*

* **R8:** One public class or struct per file.
* **R9:** File name must match the public type name.
* **R10:** Classes exceeding ~300 lines must be reviewed for refactoring.
* **R11:** Avoid deeply nested classes unless strongly justified.

---

## 3. Nullability & Defensive Coding

*These rules prevent null reference issues and runtime failures.*

* **R12:** External inputs (API, DB, config) must be validated at boundaries.
* **R13:** Nullable reference types must be enabled (`<Nullable>enable</Nullable>`).
* **R14:** Do not suppress null warnings using `!` unless safety is guaranteed and documented.
* **R15:** Public methods must validate input arguments.
* **R16:** Methods returning collections must never return `null`; return empty collections instead.

---

## 4. Exception Handling

*These rules ensure predictable and traceable error handling.*

* **R17:** Exceptions must represent exceptional scenarios, not control flow.
* **R18:** Do not catch `Exception` unless absolutely required.
* **R19:** Never swallow exceptions without logging.
* **R20:** Use specific exception types instead of generic `Exception`.
* **R21:** Preserve stack trace by using `throw;` instead of `throw ex;`.
* **R22:** Empty `catch` blocks are strictly prohibited.

---

## 5. Async & Await Usage

*These rules prevent deadlocks and async misuse.*

* **R23:** Avoid `.Result` and `.Wait()` on async code paths.
* **R24:** Avoid `async void` except for event handlers.
* **R25:** Async methods performing I/O must be awaited properly.
* **R26:** `Task.Run` must not be used in server-side code unless explicitly justified.
* **R27:** Long-running async public methods must accept `CancellationToken`.
* **R28:** `CancellationToken` must be passed to downstream calls when available.

---

## 6. Logging Standards

*These rules ensure consistent and secure logging.*

* **R29:** Use structured logging instead of string interpolation.
* **R30:** Log levels must reflect severity accurately.
* **R31:** Errors must always be logged with sufficient context.
* **R32:** Never log secrets (passwords, tokens, connection strings).
* **R33:** Correlation IDs must be logged when available.

---

## 7. API & DTO Design

*These rules govern API consistency and maintainability.*

* **R34:** Controllers must remain thin; business logic belongs in services.
* **R35:** Public APIs must validate incoming DTOs.
* **R36:** Breaking API changes must be explicitly documented.
* **R37:** Error responses must follow a consistent format (e.g., `ProblemDetails`).
* **R38:** DTOs must not expose internal domain entities.

---

## 8. Date, Time & Culture

*These rules prevent timezone and localization bugs.*

* **R39:** Use `DateTimeOffset` for cross-system timestamps.
* **R40:** Use UTC for persistence and comparisons.
* **R41:** Avoid `DateTime.Now` in server-side code.
* **R42:** Culture-sensitive parsing must explicitly specify culture info.

---

## 9. LINQ, Collections & Performance

*These rules ensure efficient and readable data processing.*

* **R43:** Avoid multiple enumerations of the same collection.
* **R44:** Avoid unnecessary `.ToList()` or `.ToArray()` calls.
* **R45:** Prefer `Any()` over `Count() > 0`.
* **R46:** Avoid overly complex LINQ expressions when loops improve readability.
* **R47:** Performance-critical paths must avoid excessive allocations.

---

## 10. Security Rules

*These rules protect against common security vulnerabilities.*

* **R48:** SQL queries must always be parameterized.
* **R49:** Never construct SQL using string concatenation.
* **R50:** Secrets must not be hardcoded.
* **R51:** Validate and sanitize all external inputs.
* **R52:** Authorization checks must exist at API and service boundaries.
* **R53:** Sensitive data must not be exposed in logs or error messages.

---

## 11. Dependency Injection

*These rules ensure correct and testable DI usage.*

* **R54:** Constructor injection is mandatory.
* **R55:** Service locator pattern is prohibited.
* **R56:** DI lifetimes must be correctly chosen and justified.
* **R57:** Singleton services must not depend on scoped services.
* **R58:** Constructors with excessive dependencies must be refactored.

---

## 12. Configuration Management

*These rules enforce secure and maintainable configuration.*

* **R59:** Configuration must use the Options pattern (`IOptions<T>`).
* **R60:** Secrets must never be committed to configuration files.
* **R61:** Configuration values must be validated at startup when possible.

---

## 13. Data Access Rules (EF / SQL)

*These rules ensure efficient and safe data access.*

* **R62:** Async database calls must use async EF methods.
* **R63:** Avoid N+1 query patterns.
* **R64:** Fetch only required columns.
* **R65:** Transactions must be used intentionally, not by default.
* **R66:** Database access inside loops must be reviewed carefully.

---

## 14. Testing Standards

*These rules enforce reliable and meaningful tests.*

* **R67:** New features must include automated tests.
* **R68:** Tests must be deterministic and repeatable.
* **R69:** Tests must follow Arrange-Act-Assert pattern.
* **R70:** External dependencies must be mocked or isolated.
* **R71:** Time-dependent logic must use an abstraction.

---

## 15. PR Hygiene & Maintainability

*These rules keep PRs reviewable and maintainable.*

* **R72:** PRs must address a single concern.
* **R73:** Dead code and commented-out code must be removed.
* **R74:** Large methods (>40 lines) must be refactored.
* **R75:** Formatting-only changes must not be mixed with logic changes.
* **R76:** Public methods must be documented where required by repo standards.

---

## 16. Mandatory Copilot PR Blockers

*Any violation of these rules must block the PR.*

* **R77:** Sync-over-async usage (`.Result`, `.Wait()`).
* **R78:** `throw ex;` usage.
* **R79:** Empty or silent exception handling.
* **R80:** Hardcoded secrets.
* **R81:** SQL injection patterns.
* **R82:** Unsafe null suppression using `!`.

---

By following this rulebook, developers ensure C# code that is consistent, maintainable, and AI-auditable.