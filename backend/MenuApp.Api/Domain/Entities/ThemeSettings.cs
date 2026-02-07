namespace MenuApp.Api.Domain.Entities;

public record ThemeSettings(
    string? LogoUrl,
    string PrimaryColor,
    string SecondaryColor,
    string Font,
    string Currency,
    string Language
)
{
    public static ThemeSettings Default => new(
        null,
        "#5b21b6",
        "#0f172a",
        "Inter",
        "TRY",
        "tr"
    );
}
