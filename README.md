Technologies: C#, ASP.NET Core, Redis, HTTP Client, LINQ, Repository Pattern, RESTful API, Distributed Caching, Logging (ILogger)

Description: A .NET Core API for managing geographically restricted content, featuring IP geolocation validation, temporary country blocking with auto-expiry, and real-time rate limiting. Implements Redis for in-memory data persistence, integrates with third-party geolocation APIs (e.g., ipapi.co), and enforces request quotas via configurable middleware. Designed with asynchronous patterns for high throughput, structured error handling for reliability, and a repository abstraction for clean data access. Securely blocks unauthorized regional access while maintaining auditability through request logging.

Key Features:
Temporal Country Blocks: Auto-expiring restrictions via Redis TTL

IP Geolocation: Real-time validation using external APIs

Rate Limiting: Custom middleware for endpoint-specific request quotas

Distributed Caching: Redis-backed storage for cross-instance consistency

Async Operations: Non-blocking HTTP calls and data operations

Security: Input validation, sanitization, and secure API key management

This architecture balances performance (in-memory checks) with durability (Redis persistence), ideal for distributed systems requiring regional access controls.

