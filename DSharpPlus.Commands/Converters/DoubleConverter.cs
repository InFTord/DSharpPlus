namespace DSharpPlus.Commands.Converters;

using System.Globalization;
using System.Threading.Tasks;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Commands.Processors.TextCommands;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

public class DoubleConverter : ISlashArgumentConverter<double>, ITextArgumentConverter<double>
{
    public ApplicationCommandOptionType ParameterType { get; init; } = ApplicationCommandOptionType.Number;
    public bool RequiresText { get; init; } = true;

    public Task<Optional<double>> ConvertAsync(ConverterContext context, MessageCreateEventArgs eventArgs) => double.TryParse(context.As<TextConverterContext>().Argument, CultureInfo.InvariantCulture, out double result)
        ? Task.FromResult(Optional.FromValue(result))
        : Task.FromResult(Optional.FromNoValue<double>());

    public Task<Optional<double>> ConvertAsync(ConverterContext context, InteractionCreateEventArgs eventArgs) => double.TryParse(context.As<InteractionConverterContext>().Argument.RawValue, CultureInfo.InvariantCulture, out double result)
        ? Task.FromResult(Optional.FromValue(result))
        : Task.FromResult(Optional.FromNoValue<double>());
}
