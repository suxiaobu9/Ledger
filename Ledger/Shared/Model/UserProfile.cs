using System.Text.Json.Serialization;

namespace Ledger.Shared.Model;

public class UserProfile
{
    [JsonPropertyName("sub")]
    public string? Sub { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }
}
