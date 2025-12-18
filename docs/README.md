# Documentation

This folder contains technical documentation for the Ecommerce project.

## 📚 Available Documents

### Payment System

#### [Payment Retry Walkthrough](payment-retry-walkthrough.md)
Complete guide to the payment retry system with Hangfire and RabbitMQ.

**Topics covered:**
- Architecture overview
- Database schema
- Message contracts
- Consumer implementation
- Testing instructions
- Deployment notes

**Key Features:**
- ✅ Async payment processing
- ✅ Automatic retry with exponential backoff (5min → 15min → 30min)
- ✅ Max 3 auto retries
- ✅ Real-time status updates
- ✅ Hangfire scheduling

---

#### [Payment Retry Implementation Plan](payment-retry-implementation-plan.md)
Original implementation plan and technical design decisions.

**Contents:**
- Proposed changes
- Architecture diagrams
- Verification plan
- Design decisions
- File structure

---

## 🚀 Quick Start

### Prerequisites
- .NET 8
- PostgreSQL
- RabbitMQ
- Hangfire (included)

### Running the Application
```powershell
# Start RabbitMQ (if not running)
# Windows: rabbitmq-server
# Docker: docker run -d -p 5672:5672 -p 15672:15672 rabbitmq:management

# Run application
dotnet run --project src/Ecommerce.Web

# Access Hangfire dashboard
# http://localhost:5128/hangfire
```

### Testing Payment Retry
1. Add product to cart
2. Checkout with MoMo payment
3. Observe async processing
4. Monitor Hangfire for retry jobs
5. Check RabbitMQ queues

---

## 🏗️ Project Structure

```
Ecommerce/
├── src/
│   ├── Ecommerce.Web/          # Web application
│   ├── Ecommerce.Workers/      # Background workers & consumers
│   ├── Ecommerce.Infrastructure/ # Data access & entities
│   └── Ecommerce.Contracts/    # Message contracts
├── docs/                       # Documentation (you are here)
└── README.md                   # Project README
```

---

## 📖 Additional Resources

### External Documentation
- [Hangfire Documentation](https://docs.hangfire.io/)
- [MassTransit Documentation](https://masstransit.io/)
- [RabbitMQ Documentation](https://www.rabbitmq.com/documentation.html)

### Related Topics
- E-wallet integrations (MoMo, ZaloPay, VNPay, Apple Pay)
- Soft delete pattern with TTL
- Background job processing
- Message queue patterns

---

## 🤝 Contributing

When adding new documentation:
1. Create a new `.md` file in this folder
2. Update this README with a link
3. Follow the existing format
4. Include code examples where relevant

---

## 📝 Document Index

| Document | Description | Last Updated |
|----------|-------------|--------------|
| [payment-retry-walkthrough.md](payment-retry-walkthrough.md) | Payment retry system guide | 2025-12-18 |
| [payment-retry-implementation-plan.md](payment-retry-implementation-plan.md) | Implementation plan | 2025-12-18 |

---

## 💡 Tips

- **For developers**: Start with the walkthrough for understanding the system
- **For testing**: Follow the Quick Start section
- **For deployment**: Check deployment notes in walkthrough
- **For troubleshooting**: See known limitations in walkthrough

---

*Last updated: 2025-12-18*
