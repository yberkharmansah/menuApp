namespace MenuApp.Api.Api.Contracts;

public record UpdateThemeRequest(
    string? LogoUrl,
    string? PrimaryColor,
    string? SecondaryColor,
    string? Font,
    string? Currency,
    string? Language
);
