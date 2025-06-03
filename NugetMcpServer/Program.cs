﻿using System;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using NuGetMcpServer.Services;
using NuGetMcpServer.Tools;

internal class Program
{
    static async Task<int> Main(string[] args)
    {
        if (args.Any(a => a is "--version" or "-v"))
        {
            var asm = Assembly.GetExecutingAssembly();
            var version =
                asm.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion
                ?? asm.GetName().Version?.ToString()
                ?? "unknown";

            Console.WriteLine($"NugetMcpServer {version}");
            return 0;
        }

        var builder = Host.CreateApplicationBuilder(args);

        // Console logging (stderr)
        builder.Logging.AddConsole(options =>
        {
            options.LogToStandardErrorThreshold = LogLevel.Trace;
        });

        // Register common services
        builder.Services.AddSingleton<HttpClient>();
        builder.Services.AddSingleton<NuGetPackageService>();
        builder.Services.AddSingleton<InterfaceFormattingService>();
        builder.Services.AddSingleton<EnumFormattingService>();

        // Register MCP server and STDIO transport
        builder.Services
            .AddMcpServer()
            .WithStdioServerTransport()
            .WithToolsFromAssembly(typeof(ListInterfacesTool).Assembly);

        await builder.Build().RunAsync();
        return 0;
    }
}
