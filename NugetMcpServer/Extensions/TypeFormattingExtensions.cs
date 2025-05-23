using System;
using System.Collections.Generic;
using System.Linq;

namespace NuGetMcpServer.Extensions
{
    /// <summary>
    /// Extension methods for formatting type names
    /// </summary>
    public static class TypeFormattingExtensions
    {
        private static readonly Dictionary<Type, string> PrimitiveTypeMap = new()
        {
            { typeof(int), "int" },
            { typeof(string), "string" },
            { typeof(bool), "bool" },
            { typeof(double), "double" },
            { typeof(float), "float" },
            { typeof(long), "long" },
            { typeof(short), "short" },
            { typeof(byte), "byte" },
            { typeof(char), "char" },
            { typeof(object), "object" },
            { typeof(decimal), "decimal" },
            { typeof(void), "void" },
            { typeof(ulong), "ulong" },
            { typeof(uint), "uint" },
            { typeof(ushort), "ushort" },
            { typeof(sbyte), "sbyte" }
        };

        /// <summary>
        /// Formats a type name to be more C#-like
        /// </summary>
        /// <param name="type">Type to format</param>
        /// <returns>Formatted type name</returns>
        public static string FormatCSharpTypeName(this Type type)
        {
            if (PrimitiveTypeMap.TryGetValue(type, out var mappedName))
            {
                return mappedName;
            }

            // Handle generic types
            if (type.IsGenericType)
            {
                var genericTypeName = type.Name;
                var tickIndex = genericTypeName.IndexOf('`');

                if (tickIndex > 0)
                {
                    genericTypeName = genericTypeName.Substring(0, tickIndex);
                }

                var genericArgs = type.GetGenericArguments();
                return $"{genericTypeName}<{string.Join(", ", genericArgs.Select(FormatCSharpTypeName))}>";
            }

            // Return the regular type name
            return type.Name;
        }

        /// <summary>
        /// Formats a type name string by converting generic notation from `N to &lt;T, U, ...&gt;
        /// </summary>
        /// <param name="typeName">Type name to format</param>
        /// <returns>Formatted type name</returns>
        public static string FormatGenericTypeName(this string typeName)
        {
            var tickIndex = typeName.IndexOf('`');
            if (tickIndex <= 0)
            {
                return typeName;
            }

            var baseName = typeName.Substring(0, tickIndex);
            var numGenericArgs = int.Parse(typeName.Substring(tickIndex + 1));

            var genericArgs = string.Join(", ", Enumerable.Range(0, numGenericArgs).Select(GetGenericParamName));

            return $"{baseName}<{genericArgs}>";
        }

        /// <summary>
        /// Formats a fully qualified type name with namespace by converting generic notation
        /// </summary>
        /// <param name="fullTypeName">Full type name with namespace</param>
        /// <returns>Formatted full type name</returns>
        public static string FormatFullGenericTypeName(this string fullTypeName)
        {
            var lastDot = fullTypeName.LastIndexOf('.');
            if (lastDot <= 0)
            {
                return fullTypeName.FormatGenericTypeName();
            }

            var ns = fullTypeName.Substring(0, lastDot + 1);
            var name = fullTypeName.Substring(lastDot + 1);

            return $"{ns}{name.FormatGenericTypeName()}";
        }

        private static string GetGenericParamName(int index)
        {
            return index < 26
                ? ((char)('T' + index)).ToString()
                : $"T{index}";
        }
    }
}
