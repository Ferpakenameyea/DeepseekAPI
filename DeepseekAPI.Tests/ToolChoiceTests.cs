using DeepseekAPI.Tools;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DeepseekAPI.Tests;

public class ToolChoiceTests
{
    private static readonly JsonSerializerSettings s_settings = new()
    {
        NullValueHandling = NullValueHandling.Ignore,
        Formatting = Formatting.None
    };

    private static void AssertJsonMatches(object typeSafe, object anonymous)
    {
        var typeSafeJson = JsonConvert.SerializeObject(typeSafe, s_settings);
        var anonymousJson = JsonConvert.SerializeObject(anonymous, s_settings);

        var typeSafeToken = JToken.Parse(typeSafeJson);
        var anonymousToken = JToken.Parse(anonymousJson);

        Assert.That(
            JToken.DeepEquals(typeSafeToken, anonymousToken),
            Is.True,
            $"JSON structures differ.\nType-safe:\n{typeSafeToken}\n\nAnonymous:\n{anonymousToken}");
    }

    [Test]
    public void None_SerializesAsString()
    {
        AssertJsonMatches(ToolChoice.None, "none");
    }

    [Test]
    public void Auto_SerializesAsString()
    {
        AssertJsonMatches(ToolChoice.Auto, "auto");
    }

    [Test]
    public void Required_SerializesAsString()
    {
        AssertJsonMatches(ToolChoice.Required, "required");
    }

    [Test]
    public void CustomStringChoice_SerializesAsString()
    {
        var typeSafe = new ChatCompletionToolChoice("custom_value");
        AssertJsonMatches(typeSafe, "custom_value");
    }

    [Test]
    public void NamedToolChoice_SerializesAsObject()
    {
        var typeSafe = new ChatCompletionNamedToolChoice("get_weather");

        var anonymous = new
        {
            type = "function",
            function = new { name = "get_weather" }
        };

        AssertJsonMatches(typeSafe, anonymous);
    }
}
