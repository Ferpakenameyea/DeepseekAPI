using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace DeepseekAPI.Tools
{
    public sealed class ToolDefinition
    {
        [JsonProperty("type")]
        public string Type => "function";

        [JsonProperty("function")]
        public Function Function { get; }

        public ToolDefinition(Function function)
        {
            Function = function;
        }
    }

    public sealed class Function
    {
        [JsonProperty("name")]
        public string Name { get; }

        [JsonProperty("description")]
        public string Description { get; }

        [JsonProperty("parameters")]
        public FunctionParameters? Parameters { get; }

        [JsonProperty("strict")]
        public bool? Strict { get; }

        public Function(
            string name,
            string description,
            FunctionParameters? parameters = null,
            bool? strict = null)
        {
            Name = name;
            Description = description;
            Parameters = parameters;
            Strict = strict;
        }
    }

    public sealed class FunctionParameters
    {
        [JsonProperty("type")]
        public string Type => "object";

        [JsonProperty("properties")]
        public IReadOnlyDictionary<string, ParameterDefinition> Properties { get; }

        [JsonProperty("required")]
        public string[]? Required { get; }

        [JsonProperty("additionalProperties")]
        public bool? AdditionalProperties { get; }

        public FunctionParameters(
            IReadOnlyDictionary<string, ParameterDefinition> properties,
            string[]? required = null,
            bool? additionalProperties = null)
        {
            Properties = properties;
            Required = required;
            AdditionalProperties = additionalProperties;
        }
    }

    public abstract class ParameterDefinition : ReferencableType
    {
        [JsonProperty("type")]
        public string Type { get; }

        [JsonProperty("description")]
        public string? Description { get; }

        protected ParameterDefinition(string type, string? description)
        {
            Type = type;
            Description = description;
        }
    }

    public sealed class StringParameter : ParameterDefinition
    {
        public enum PredefinedFormatCheck
        {
            Email,
            HostName,
            Ipv4,
            Ipv6,
            Uuid
        };
        [JsonProperty("format")]
        public string? Format { get; } = null;
        [JsonProperty("pattern")]
        public string? Pattern { get; } = null;
        public StringParameter(
            string? description = null,
            PredefinedFormatCheck? formatCheck = null,
            string? pattern = null)
            : base("string", description)
        {
            if (formatCheck != null)
            {
                Format = formatCheck switch
                {
                    PredefinedFormatCheck.Email => "email",
                    PredefinedFormatCheck.HostName => "hostname",
                    PredefinedFormatCheck.Ipv4 => "ipv4",
                    PredefinedFormatCheck.Ipv6 => "ipv6",
                    PredefinedFormatCheck.Uuid => "uuid",
                    _ => throw new ArgumentException($"Unknown {nameof(PredefinedFormatCheck)}: {formatCheck}")
                };
            }

            Pattern = pattern;
        }
    }

    public sealed class NumberParameter : ParameterDefinition
    {
        [JsonProperty("const")]
        public long? Const { get; }
        [JsonProperty("default")]
        public long? Default { get; }
        [JsonProperty("minimum")]
        public long? Minimum { get; }
        [JsonProperty("maximum")]
        public long? Maximum { get; }
        [JsonProperty("exclusiveMinimum")]
        public long? ExclusiveMinimum { get; }
        [JsonProperty("exclusiveMaximum")]
        public long? ExclusiveMaximum { get; }
        [JsonProperty("multipleOf")]
        public long? MultipleOf { get; }
        public NumberParameter(
            string? description = null,
            long? @const = null,
            long? @default = null,
            long? minimum = null,
            long? maximum = null,
            long? exclusiveMinimum = null,
            long? exclusiveMaximum = null,
            long? multipleOf = null
        )
            : base("number", description)
        {
            Const = @const;
            Default = @default;
            Minimum = minimum;
            Maximum = maximum;
            ExclusiveMinimum = exclusiveMinimum;
            ExclusiveMaximum = exclusiveMaximum;
            MultipleOf = multipleOf;
        }
    }

    public sealed class IntegerParameter : ParameterDefinition
    {
        public IntegerParameter(string? description = null)
            : base("integer", description) { }
    }

    public sealed class BooleanParameter : ParameterDefinition
    {
        public BooleanParameter(string? description = null)
            : base("boolean", description) { }
    }

    public sealed class ArrayParameter : ParameterDefinition
    {
        [JsonProperty("items")]
        public ReferencableType Items { get; }

        public ArrayParameter(ReferencableType items, string? description = null)
            : base("array", description)
        {
            Items = items;
        }
    }

    public sealed class ObjectParameter : ParameterDefinition
    {
        [JsonProperty("properties")]
        public IReadOnlyDictionary<string, ParameterDefinition> Properties { get; }

        [JsonProperty("required")]
        public string[]? Required { get; }

        [JsonProperty("additionalProperties")]
        public bool AdditionalProperties => false;

        [JsonProperty("$def")]
        public object? DefSource = null;

        public ObjectParameter(
            IReadOnlyDictionary<string, ParameterDefinition> properties,
            string[]? required = null,
            string? description = null,
            object? defSource = null)
            : base("object", description)
        {
            Properties = properties;
            Required = required;
            DefSource = defSource;
        }
    }

    public sealed class EnumParameter : ParameterDefinition
    {
        [JsonProperty("enum")]
        public IReadOnlyList<string> Values { get; }

        public EnumParameter(IReadOnlyList<string> values, string? description = null)
            : base("string", description)
        {
            Values = values;
        }
    }

    public sealed class AnyOfParameter : ParameterDefinition
    {
        [JsonProperty("anyOf")]
        public IReadOnlyList<ParameterDefinition> Schemas { get; }

        public AnyOfParameter(IReadOnlyList<ParameterDefinition> schemas, string? description = null)
            : base("anyOf", description)
        {
            Schemas = schemas;
        }
    }

    public sealed class TypeRef : ReferencableType
    {
        [JsonProperty("$ref")]
        public string ReferencePath { get; }

        public TypeRef(string referencePath)
        {
            ReferencePath = referencePath;
        }
    }

    public abstract class ReferencableType
    {}
}
