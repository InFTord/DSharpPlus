namespace DSharpPlus.Commands.Converters;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.Commands.ParameterModifiers;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Commands.Processors.TextCommands;
using DSharpPlus.Commands.Processors.TextCommands.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

public class StringConverter : ISlashArgumentConverter<string>, ITextArgumentConverter<string>
{
    public ApplicationCommandOptionType ParameterType { get; init; } = ApplicationCommandOptionType.String;
    public bool RequiresText { get; init; } = true;

    public Task<Optional<string>> ConvertAsync(ConverterContext context, MessageCreateEventArgs eventArgs)
    {
        TextConverterContext textContext = context.As<TextConverterContext>();
        foreach (Attribute attribute in context.Parameter.Attributes)
        {
            if (attribute is RemainingTextAttribute)
            {
                return Task.FromResult(Optional.FromValue(textContext.Argument));
            }
            else if (attribute is FromCodeAttribute codeAttribute)
            {
                return TryGetCodeBlock(textContext.Argument, codeAttribute.CodeType, out string? code)
                    ? Task.FromResult(Optional.FromValue(code))
                    : Task.FromResult(Optional.FromNoValue<string>());
            }
        }

        TextConverterContext textConverterContext = context.As<TextConverterContext>();
        return Task.FromResult(Optional.FromValue(textConverterContext.RawArguments[textConverterContext.CurrentArgumentIndex..]));
    }

    [SuppressMessage("Roslyn", "IDE0046", Justification = "Ternary rabbit hole.")]
    public Task<Optional<string>> ConvertAsync(ConverterContext context, InteractionCreateEventArgs eventArgs)
    {
        string? value = (string)context.As<InteractionConverterContext>().Argument.Value;
        if (context.Parameter.Attributes.FirstOrDefault(x => x is FromCodeAttribute) is not FromCodeAttribute codeAttribute)
        {
            return Task.FromResult(Optional.FromValue(context.As<InteractionConverterContext>().Argument.RawValue));
        }
        else if (TryGetCodeBlock(value, codeAttribute.CodeType, out string? code))
        {
            return Task.FromResult(Optional.FromValue(code));
        }
        else
        {
            return Task.FromResult(Optional.FromNoValue<string>());
        }
    }

    private static bool TryGetCodeBlock(string input, CodeType expectedCodeType, [NotNullWhen(true)] out string? code)
    {
        code = null;
        ReadOnlySpan<char> inputSpan = input.AsSpan();
        if (inputSpan.Length > 6 && inputSpan.StartsWith("```") && inputSpan.EndsWith("```") && expectedCodeType.HasFlag(CodeType.Codeblock))
        {
            int index = inputSpan.IndexOf('\n');
            if (index == -1 || !FromCodeAttribute.CodeBlockLanguages.Contains(inputSpan[3..index].ToString()))
            {
                code = input[3..^3];
                return true;
            }

            code = input[(index + 1)..^3];
            return true;
        }
        else if (inputSpan.Length > 4 && inputSpan.StartsWith("``") && inputSpan.EndsWith("``") && expectedCodeType.HasFlag(CodeType.Inline))
        {
            code = input[2..^2];
        }
        else if (inputSpan.Length > 2 && inputSpan.StartsWith("`") && inputSpan.EndsWith("`") && expectedCodeType.HasFlag(CodeType.Inline))
        {
            code = input[1..^1];
        }

        return code is not null;
    }
}
