# Implementation Instructions

Before implementation:

1. Analyze impacted layers:
   - API
   - Application
   - Domain
   - Infrastructure
   - Database
   - Tests

2. Reuse existing:
   - Abstractions
   - Base classes
   - DTO patterns
   - Extension methods
   - Middleware
   - Response contracts

3. Create implementation plan including:
   - Files changing
   - New services/interfaces
   - DB impact
   - Validation strategy
   - Testing strategy
   - Risks

---

# Layered Architecture Rules

## API Layer

Can depend on:
- Application layer

Must NOT depend on:
- Infrastructure implementations
- Database logic

---

# API Rules

Controllers must remain thin.

Controllers should:
- Receive request
- Call application layer
- Return response

Controllers must NOT:
- Contain business logic
- Access DbContext directly
- Perform heavy mapping

---

# DTO Rules

- Never expose entities directly
- Use request/response DTOs
- Prefer immutable DTOs

---

# Mapping Rules

- Use existing mapping strategy
- Prefer explicit mapping
- Avoid hidden magic mappings
