using System.Text;
using DeepseekAPI.Constants;
using DeepseekAPI.Messages;
using DeepseekAPI.Tools;
using Newtonsoft.Json;

namespace DeepseekAPI.Tests;

public class ChatCompletionTests
{
    private DeepseekAPI _deepseek;

    [SetUp]
    public void Setup()
    {
        string apiKey = Environment.GetEnvironmentVariable("DEEPSEEK_API_KEY")
            ?? throw new ArgumentNullException("You haven't set api key in environtment variable");
        _deepseek = new(apiKey, model: Models.V4ProModel);
    }

    [Test]
    public async Task CompleteChatTest()
    {
        var completion = await _deepseek.CompleteChatAsync([
            new SystemMessage("Please output only 'OK'")
        ]);

        Assert.That(completion.Choices, Is.Not.Empty);
    }

    [Test]
    public async Task CompleteStreamedChatTest()
    {
        var enumerable = _deepseek.CompleteStreamedChatAsync([
            new SystemMessage("Please output only 'OK'")
        ]);

        StringBuilder reasonBuffer = new();
        StringBuilder contentBuffer = new();

        await foreach (var chunk in enumerable)
        {
            if (chunk.Choices[0].Delta.Content != null)
            {
                contentBuffer.Append(chunk.Choices[0].Delta.Content);
            }
            else
            {
                reasonBuffer.Append(chunk.Choices[0].Delta.ReasoningContent);
            }
        }

        Assert.That(contentBuffer.ToString(), Is.Not.Empty);
    }

    [Test]
    public async Task ToolCall_InvokesDelegateAndChangesLocalVariable()
    {
        string? capturedName = null;

        void SetName(string name)
        {
            capturedName = name;
        }
        const string testToolFunctionName = "set_user_name";

        var tool = new ToolDefinition(new Function(
            name: testToolFunctionName,
            description: "Set the user's display name",
            parameters: new FunctionParameters(new Dictionary<string, ParameterDefinition>
            {
                ["name"] = new StringParameter(description: "The display name to set")
            }, required: new[] { "name" })
        ));

        var completion = await _deepseek.CompleteChatAsync(
            messages: [
                new SystemMessage("Call the set_user_name function to set the name to 'Alice'.")
            ],
            tools: [tool],
            toolChoice: new ChatCompletionNamedToolChoice(testToolFunctionName)
        );

        var toolCalls = completion.Choices[0].Message.ToolCalls;
        Assert.That(toolCalls, Is.Not.Null.And.Not.Empty);

        var callArgs = JsonConvert.DeserializeObject<Dictionary<string, string>>(
            toolCalls[0].Function.Arguments
        );
        Assert.That(callArgs, Is.Not.Null);
        Assert.That(callArgs!.ContainsKey("name"), Is.True);

        SetName(callArgs["name"]);

        Assert.That(capturedName, Is.EqualTo("Alice"));
    }
}
