# Lab: REST API with PostgreSQL and N‑Tier Architecture
**Important:** Please submit your work on the branch named with your index numbers i.e. `s_1xxxxx_1yyyyyy` 

## Overview
Build a production‑style REST API that:
- Is implemented in **any language listed on <https://opentelemetry.io/status/>** (i.e., with official OpenTelemetry support).
- Uses **PostgreSQL** with **≥ 5 normalized tables** and meaningful relationships.
- Follows an **N‑tier architecture** (at minimum: API/Presentation → Service/Business → Data Access/Repository → Database). You may also include Domain and Infrastructure layers.

> **Tip**: Choose a domain you like (e‑commerce, library, course management, booking, etc.). You’ll be more productive when models feel intuitive.

---

## Learning Objectives
- Design a relational data model in PostgreSQL with non‑trivial relationships and constraints.
- Implement a clean **N‑tier** API with separation of concerns and clear contracts.
---

## Functional Requirements
1. **Entities & Relations (≥ 5 tables)**  
   - At least **2 one‑to‑many** and **1 many‑to‑many** relationship (via a join table).  
   - Include **constraints** (PKs, FKs with ON DELETE behavior, UNIQUE, CHECK, NOT NULL).  
   - Create **indexes** for frequent lookups and foreign keys.

2. **Endpoints**
   - CRUD for at least **3 entities** and read‑only endpoints for the rest.
   - **Versioned API** (e.g., `/api/v1/...` or header‑based versioning).

3. **Validation & Errors**
   - Input validation with clear error responses (HTTP 4xx with problem details).
   - Proper HTTP semantics (201 on create with `Location`, 404/409/422 as appropriate).
