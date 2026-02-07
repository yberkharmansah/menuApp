namespace MenuApp.Api.Api.Contracts;

public record RegisterRequest(string BusinessName);
public record LoginRequest(string BusinessSlug);
public record AuthResponse(Guid BusinessId, string BusinessSlug, string Token);
