# 🧠 DecisionEngine
**Enterprise-grade Decision Optimization Framework (.NET)**

![.NET](https://img.shields.io/badge/.NET-6.0%2B-blue)
![Build](https://img.shields.io/badge/build-passing-brightgreen)
![Tests](https://img.shields.io/badge/tests-MSTest%20%2B%20Moq-success)
![Architecture](https://img.shields.io/badge/architecture-clean-blueviolet)

---

## 📌 Overview
**DecisionEngine** is a modular, extensible **decision-making framework** built on  
**Dynamic Programming (DP)** and **Policy Iteration**.

It supports:
- **Offline strategy learning**
- **Online optimal decision execution**
- **Multi-tenant, versioned policy persistence**
- **Pluggable storage (JSON / MongoDB) via DI**

Designed with **Clean Architecture**, **testability**, and **enterprise readiness** in mind.

---

## 🚀 Key Features

- ✅ Dynamic Programming Engine (finite horizon, constraint-aware)
- ✅ Policy Iteration Engine (infinite horizon MDPs)
- ✅ Offline training → Online execution integration
- ✅ Policy persistence abstraction (`IPolicyStore`)
- ✅ JSON (dev) & MongoDB (prod) implementations
- ✅ Multi-tenant policy keys
- ✅ Policy versioning
- ✅ Policy TTL / expiry
- ✅ Dependency Injection throughout
- ✅ MSTest + Moq test suite
- ✅ DI container validation tests

---

## 🏗 Architecture Overview

```
+----------------------+
| Policy Iteration     |
| (Offline Training)   |
+----------+-----------+
           |
           | Versioned Policy
           v
+----------------------+
| Policy Store         |
| (JSON / MongoDB)     |
+----------+-----------+
           |
           v
+----------------------+
| Dynamic Programming  |
| (Runtime Execution)  |
+----------------------+
```

**Key idea**  
> Learn long-term strategy offline, execute optimal short-term decisions online.

---

## ▶ Getting Started

### Prerequisites
- .NET 6.0 or later
- (Optional) MongoDB for production policy storage

---

## 🧠 Offline Training (Policy Iteration)

```bash
dotnet run --project src/Simple.DecisionEngine.App train
```

- Learns optimal **stationary policy**
- Saves policy with:
  - tenant
  - policy key
  - version
  - TTL

---

## ⚙ Runtime Decision Execution (DP)

```bash
dotnet run --project src/Simple.DecisionEngine.App run
```

- Loads **latest policy**
- Injects it as a **bias constraint**
- Performs **finite-horizon optimization**
- Executes optimal action at `t = 0`

---

## 🗄 Policy Storage

All policies are persisted via:

```csharp
IPolicyStore
```

### Supported Backends
- **JSON** (local development)
- **MongoDB** (production)

### Supported Capabilities
- Multi-tenant isolation
- Immutable versioning
- Latest-version auto selection
- TTL / expiry (MongoDB TTL index)

---

## 🧪 Testing

Run all tests:

```bash
dotnet test
```

Skip performance tests:

```bash
dotnet test --filter "TestCategory!=Performance"
```

Includes:
- Algorithm correctness tests
- DI container validation tests
- Policy store mock tests
- Performance / stress tests

---

## 🔒 Design Principles

- Clean Architecture
- Dependency Inversion
- SOLID principles
- Storage-agnostic persistence
- Deterministic unit tests
- Enterprise scalability

---

## 🛣 Roadmap

- [ ] Policy promotion (staging → production)
- [ ] Policy rollback
- [ ] Policy audit logging
- [ ] Policy encryption at rest
- [ ] ASP.NET Core API wrapper
- [ ] Observability & metrics
- [ ] Kubernetes-ready deployment

---

## 📄 License

MIT License  
Free to use, modify, and distribute.
