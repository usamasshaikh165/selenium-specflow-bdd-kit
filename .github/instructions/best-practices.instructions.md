# Best Practices Instructions

# General Principles

- Follow SOLID
- Follow DRY
- Follow KISS
- Prefer readability
- Prefer composition over inheritance
- Avoid premature optimization

---

# Naming

- Use meaningful names
- Avoid abbreviations
- Methods use verbs
- Classes use nouns
- Async methods MUST end with Async

---

# Async/Await

- Use async/await correctly
- Never use .Result or .Wait()
- Propagate async all the way

---

# Null Safety

- Enable nullable reference types
- Avoid null-return patterns
- Validate external input

---

# Exceptions

- Never swallow exceptions
- Use centralized exception handling
- Use domain-specific exceptions
- Log exceptions with context

---

# Logging

Use structured logging.

Include:
- Correlation IDs
- Request context
- Business identifiers

Never log:
- Secrets
- Tokens
- Passwords
- Sensitive PII

---

# Validation

- Use FluentValidation if project uses it
- Validate requests/commands
- Keep validation out of controllers
- Fail fast

---

# Security

- Validate all inputs
- Prevent SQL injection
- Prevent overposting
- Enforce authorization
- Never hardcode secrets
- Use least privilege

---

# Performance

- Avoid N+1 queries
- Use AsNoTracking for reads
- Paginate large datasets
- Avoid unnecessary allocations
- Use cancellation tokens
