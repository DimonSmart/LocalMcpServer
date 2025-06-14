using System;
using System.Text;

namespace NuGetMcpServer.Services;

/// <summary>
/// Service for formatting enum definitions
/// </summary>
public class EnumFormattingService
{    /// <summary>
     /// Builds a string representation of an enum, including its values,
     /// attributes, and underlying type
     /// </summary>
    public string FormatEnumDefinition(Type enumType)
    {
        if (!enumType.IsEnum)
        {
            throw new ArgumentException($"Type {enumType.Name} is not an enum", nameof(enumType));
        }

        var sb = new StringBuilder();
        var underlyingType = Enum.GetUnderlyingType(enumType);
        var underlyingTypeName = TypeFormattingHelpers.FormatTypeName(underlyingType);

        sb.Append($"public enum {enumType.Name}");
        if (underlyingType != typeof(int))
        {
            sb.Append($" : {underlyingTypeName}");
        }

        sb.AppendLine().AppendLine("{");

        var values = Enum.GetValues(enumType);
        var names = Enum.GetNames(enumType);
        var lastIndex = names.Length - 1;

        // Format each enum value
        for (var i = 0; i < names.Length; i++)
        {
            var name = names[i];
            var value = Convert.ChangeType(values.GetValue(i), underlyingType);

            // Check if we need a suffix for the numeric literal based on underlying type
            var valueSuffix = underlyingType == typeof(ulong) ? "UL" :
                                underlyingType == typeof(long) ? "L" : underlyingType == typeof(uint) ? "U" :
                                string.Empty;

            sb.Append($"    {name} = {value}{valueSuffix}");

            if (i < lastIndex)
            {
                sb.AppendLine(",");
            }
            else
            {
                sb.AppendLine();
            }
        }
        sb.AppendLine("}");
        return sb.ToString();
    }
}
