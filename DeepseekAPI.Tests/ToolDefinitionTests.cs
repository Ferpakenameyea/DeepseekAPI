using DeepseekAPI.Tools;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DeepseekAPI.Tests;

public class ToolDefinitionTests
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
    public void BasicTool_MatchesAnonymousObject()
    {
        var typeSafe = new ToolDefinition(new Function(
            name: "get_weather",
            description: "Get the current weather for a city",
            parameters: new FunctionParameters(new Dictionary<string, ParameterDefinition>
            {
                ["city"] = new StringParameter(description: "The city name")
            }, required: new[] { "city" })
        ));

        var anonymous = new
        {
            type = "function",
            function = new
            {
                name = "get_weather",
                description = "Get the current weather for a city",
                parameters = new
                {
                    type = "object",
                    properties = new
                    {
                        city = new { type = "string", description = "The city name" }
                    },
                    required = new[] { "city" }
                }
            }
        };

        AssertJsonMatches(typeSafe, anonymous);
    }

    [Test]
    public void AllParameterTypes_MatchesAnonymousObject()
    {
        var typeSafe = new ToolDefinition(new Function(
            name: "all_types",
            description: "A function with every parameter type",
            parameters: new FunctionParameters(new Dictionary<string, ParameterDefinition>
            {
                ["str"] = new StringParameter(description: "A string param"),
                ["num"] = new NumberParameter(description: "A number param"),
                ["int"] = new IntegerParameter(description: "An integer param"),
                ["bool"] = new BooleanParameter(description: "A boolean param"),
                ["arr"] = new ArrayParameter(
                    items: new StringParameter(),
                    description: "An array of strings"
                ),
                ["obj"] = new ObjectParameter(
                    properties: new Dictionary<string, ParameterDefinition>
                    {
                        ["nested"] = new StringParameter()
                    },
                    required: new[] { "nested" },
                    description: "A nested object"
                ),
                ["color"] = new EnumParameter(
                    values: new List<string> { "red", "green", "blue" },
                    description: "A color enum"
                ),
                ["union"] = new AnyOfParameter(
                    schemas: new List<ParameterDefinition>
                    {
                        new StringParameter(),
                        new NumberParameter()
                    },
                    description: "String or number"
                )
            }, required: new[] { "str", "num" })
        ));

        var anonymous = new
        {
            type = "function",
            function = new
            {
                name = "all_types",
                description = "A function with every parameter type",
                parameters = new
                {
                    type = "object",
                    properties = new
                    {
                        str = new { type = "string", description = "A string param" },
                        num = new { type = "number", description = "A number param" },
                        @int = new { type = "integer", description = "An integer param" },
                        @bool = new { type = "boolean", description = "A boolean param" },
                        arr = new
                        {
                            type = "array",
                            description = "An array of strings",
                            items = new { type = "string" }
                        },
                        obj = new
                        {
                            type = "object",
                            description = "A nested object",
                            properties = new
                            {
                                nested = new { type = "string" }
                            },
                            required = new[] { "nested" },
                            additionalProperties = false
                        },
                        color = new
                        {
                            type = "string",
                            description = "A color enum",
                            @enum = new[] { "red", "green", "blue" }
                        },
                        union = new
                        {
                            type = "anyOf",
                            description = "String or number",
                            anyOf = new object[]
                            {
                                new { type = "string" },
                                new { type = "number" }
                            }
                        }
                    },
                    required = new[] { "str", "num" }
                }
            }
        };

        AssertJsonMatches(typeSafe, anonymous);
    }

    [Test]
    public void StringParameter_WithFormatCheck_MatchesAnonymousObject()
    {
        var typeSafe = new ToolDefinition(new Function(
            name: "validate_email",
            description: "Check email format",
            parameters: new FunctionParameters(new Dictionary<string, ParameterDefinition>
            {
                ["email"] = new StringParameter(
                    description: "Email address",
                    formatCheck: StringParameter.PredefinedFormatCheck.Email
                )
            })
        ));

        var anonymous = new
        {
            type = "function",
            function = new
            {
                name = "validate_email",
                description = "Check email format",
                parameters = new
                {
                    type = "object",
                    properties = new
                    {
                        email = new
                        {
                            type = "string",
                            description = "Email address",
                            format = "email"
                        }
                    }
                }
            }
        };

        AssertJsonMatches(typeSafe, anonymous);
    }

    [Test]
    public void StringParameter_WithPattern_MatchesAnonymousObject()
    {
        var typeSafe = new ToolDefinition(new Function(
            name: "validate_code",
            description: "Check code format",
            parameters: new FunctionParameters(new Dictionary<string, ParameterDefinition>
            {
                ["code"] = new StringParameter(
                    description: "Product code",
                    pattern: "^[A-Z]{3}-\\d{4}$"
                )
            })
        ));

        var anonymous = new
        {
            type = "function",
            function = new
            {
                name = "validate_code",
                description = "Check code format",
                parameters = new
                {
                    type = "object",
                    properties = new
                    {
                        code = new
                        {
                            type = "string",
                            description = "Product code",
                            pattern = "^[A-Z]{3}-\\d{4}$"
                        }
                    }
                }
            }
        };

        AssertJsonMatches(typeSafe, anonymous);
    }

    [Test]
    public void NumberParameter_WithAllConstraints_MatchesAnonymousObject()
    {
        var typeSafe = new ToolDefinition(new Function(
            name: "filter_scores",
            description: "Filter by numeric constraints",
            parameters: new FunctionParameters(new Dictionary<string, ParameterDefinition>
            {
                ["score"] = new NumberParameter(
                    description: "Score value",
                    @const: 100,
                    @default: 50,
                    minimum: 0,
                    maximum: 200,
                    exclusiveMinimum: -1,
                    exclusiveMaximum: 201,
                    multipleOf: 5
                )
            })
        ));

        var anonymous = new
        {
            type = "function",
            function = new
            {
                name = "filter_scores",
                description = "Filter by numeric constraints",
                parameters = new
                {
                    type = "object",
                    properties = new
                    {
                        score = new
                        {
                            type = "number",
                            description = "Score value",
                            @const = 100L,
                            @default = 50L,
                            minimum = 0L,
                            maximum = 200L,
                            exclusiveMinimum = -1L,
                            exclusiveMaximum = 201L,
                            multipleOf = 5L
                        }
                    }
                }
            }
        };

        AssertJsonMatches(typeSafe, anonymous);
    }

    [Test]
    public void ObjectParameter_AdditionalPropertiesAlwaysFalse_MatchesAnonymousObject()
    {
        var typeSafe = new ToolDefinition(new Function(
            name: "nested_obj",
            description: "A deeply nested object",
            parameters: new FunctionParameters(new Dictionary<string, ParameterDefinition>
            {
                ["config"] = new ObjectParameter(
                    properties: new Dictionary<string, ParameterDefinition>
                    {
                        ["timeout"] = new IntegerParameter(description: "Timeout in seconds")
                    },
                    required: new[] { "timeout" }
                )
            })
        ));

        var anonymous = new
        {
            type = "function",
            function = new
            {
                name = "nested_obj",
                description = "A deeply nested object",
                parameters = new
                {
                    type = "object",
                    properties = new
                    {
                        config = new
                        {
                            type = "object",
                            properties = new
                            {
                                timeout = new
                                {
                                    type = "integer",
                                    description = "Timeout in seconds"
                                }
                            },
                            required = new[] { "timeout" },
                            additionalProperties = false
                        }
                    }
                }
            }
        };

        AssertJsonMatches(typeSafe, anonymous);
    }

    [Test]
    public void Function_WithStrict_MatchesAnonymousObject()
    {
        var typeSafe = new ToolDefinition(new Function(
            name: "strict_func",
            description: "Has strict mode",
            parameters: new FunctionParameters(new Dictionary<string, ParameterDefinition>
            {
                ["input"] = new StringParameter()
            }),
            strict: true
        ));

        var anonymous = new
        {
            type = "function",
            function = new
            {
                name = "strict_func",
                description = "Has strict mode",
                strict = true,
                parameters = new
                {
                    type = "object",
                    properties = new
                    {
                        input = new { type = "string" }
                    }
                }
            }
        };

        AssertJsonMatches(typeSafe, anonymous);
    }

    [Test]
    public void Function_WithoutParameters_MatchesAnonymousObject()
    {
        var typeSafe = new ToolDefinition(new Function(
            name: "no_params",
            description: "No parameters needed"
        ));

        var anonymous = new
        {
            type = "function",
            function = new
            {
                name = "no_params",
                description = "No parameters needed"
            }
        };

        AssertJsonMatches(typeSafe, anonymous);
    }

    [Test]
    public void TypeRef_SerializesAsRef()
    {
        var typeSafe = new TypeRef("#/$def/Address");

        var expected = JToken.Parse(@"{""$ref"":""#/$def/Address""}");

        var typeSafeJson = JsonConvert.SerializeObject(typeSafe, s_settings);
        var typeSafeToken = JToken.Parse(typeSafeJson);

        Assert.That(JToken.DeepEquals(typeSafeToken, expected), Is.True,
            $"JSON structures differ.\nType-safe:\n{typeSafeToken}\n\nExpected:\n{expected}");
    }

    [Test]
    public void ArrayParameter_WithTypeRef_MatchesExpected()
    {
        var typeSafe = new ArrayParameter(
            items: new TypeRef("#/$def/Address"),
            description: "An array of addresses"
        );

        var expected = JToken.Parse(
            @"{""type"":""array"",""description"":""An array of addresses"",""items"":{""$ref"":""#/$def/Address""}}");

        var typeSafeJson = JsonConvert.SerializeObject(typeSafe, s_settings);
        var typeSafeToken = JToken.Parse(typeSafeJson);

        Assert.That(JToken.DeepEquals(typeSafeToken, expected), Is.True,
            $"JSON structures differ.\nType-safe:\n{typeSafeToken}\n\nExpected:\n{expected}");
    }
}
