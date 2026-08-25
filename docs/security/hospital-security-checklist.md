# Hospital / Regulated-Environment Security Checklist

This template is designed to support hospital-grade information security. Use this checklist when deploying to a regulated environment (Taiwan 衛福部, HIPAA-equivalent, GDPR, etc.).

## ✅ Already wired by the template

- [x] **Result pattern** replaces exception-driven control flow for expected failures
- [x] **Audit interceptor** records every entity change to structured log
- [x] **Channel-based audit** decouples request latency from audit write latency
- [x] **Cookie auth** with `HttpOnly`, `Secure=Always`, `SameSite=Lax`
- [x] **JWT bearer** for API/mobile clients (15-minute lifetime default)
- [x] **Windows Authentication (Negotiate)** for AD SSO
- [x] **Break-glass service** interface for emergency privilege override
- [x] **Swagger** only exposed in Development environment
- [x] **ProblemDetails (RFC 7807)** for all errors
- [x] **HealthChecks** at `/health/live` and `/health/ready`
- [x] **CPM-pinned versions** prevent supply-chain drift
- [x] **Centralized Serilog** for structured request tracing
- [x] **CORS off by default** — opt in per-environment

## 🔐 You must configure before production

- [ ] **TLS 1.3 only** — terminate at reverse proxy (IIS / nginx / Traefik)
- [ ] **Database TLS** — `Ssl Mode=Require` in connection string
- [ ] **DataProtection keys** — store in shared UNC (`\\fileserver\hospital$\dp-keys\`)
- [ ] **Secret management** — never commit secrets; use environment variables or self-hosted Vault
- [ ] **PHI masking in logs** — add a Serilog enricher that replaces sensitive fields
- [ ] **Audit retention 7+ years** — drain `AuditEvent` channel to append-only table
- [ ] **Rate limiting** — enable `AddRateLimiter()` for sensitive endpoints
- [ ] **Anti-forgery** — enabled by default for Razor/MVC form posts
- [ ] **HSTS** — `UseHsts()` in Production only
- [ ] **Encryption at rest** — TDE on PG side or Always Encrypted columns
- [ ] **Backup encryption** — PITR backups stored on encrypted volumes
- [ ] **Container hardening** — non-root user in Dockerfile (template defaults to `$APP_UID`)
- [ ] **Dependency scanning** — `dotnet list package --vulnerable` in CI
- [ ] **Pen test** — annually + on any major dependency upgrade
- [ ] **Incident response plan** — documented in `docs/runbooks/`

## 🚫 Forbidden patterns

- ❌ Logging PHI (patient names, IDs, diagnoses) to any sink
- ❌ Catching `Exception` and returning 200
- ❌ String concatenation for SQL (always parameterized)
- ❌ Disabling HTTPS in production
- ❌ Hardcoding credentials in `appsettings.json`
- ❌ Anonymous auth in production
- ❌ Self-signed certs in production
- ❌ Reflective serializers on hot paths (use Mapperly source-gen instead)