namespace DSharpPlus.Commands.Converters;

using System.Globalization;
using System.Threading.Tasks;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Commands.Processors.TextCommands;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

public class Int32Converter : ISlashArgumentConverter<int>, ITextArgumentConverter<int>
{
    public ApplicationCommandOptionType ParameterType { get; init; } = ApplicationCommandOptionType.Integer;
    public bool RequiresText { get; init; } = true;

    public Task<Optional<int>> ConvertAsync(ConverterContext context, MessageCreateEventArgs eventArgs) => int.TryParse(context.As<TextConverterContext>().Argument, CultureInfo.InvariantCulture, out int result)
        ? Task.FromResult(Optional.FromValue(result))
        : Task.FromResult(Optional.FromNoValue<int>());

    public Task<Optional<int>> ConvertAsync(ConverterContext context, InteractionCreateEventArgs eventArgs) => int.TryParse(context.As<InteractionConverterContext>().Argument.RawValue, CultureInfo.InvariantCulture, out int result)
        ? Task.FromResult(Optional.FromValue(result))
        : Task.FromResult(Optional.FromNoValue<int>());
}
