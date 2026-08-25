# ADR 0001: Modular Monolith over Microservices

## Status
Accepted · 2026-08-25

## Context
Hospital information systems typically grow to 5-20 subsystems (HIS, EMR, LIS, RIS/PACS, Nursing, Pharmacy, Billing, Reporting). Choosing architecture style at the start is critical because migration later is expensive.

## Decision
Adopt a **Modular Monolith** structure: each subsystem is its own csproj set (Domain / Application / Infrastructure / Web), deployed independently (separate IIS Sites or Docker containers) but sharing common libraries.

## Consequences
- ✅ Single repo, single solution at first; deploys independently
- ✅ Shared kernel (`YourApp.*`) enforces architectural boundaries
- ✅ Easier refactor than microservices
- ⚠ Cross-subsystem calls require explicit API boundaries — never direct DB access

## Alternatives considered
- **Microservices from day 1**: too much overhead for a small team; 6-12 month delay on first deliverable
- **Single big project**: every developer breaks everyone else's tests; no architectural guardrails