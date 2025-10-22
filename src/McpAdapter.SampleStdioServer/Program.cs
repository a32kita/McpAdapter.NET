/*
 * SimpleMcpServer - A Sample MCP Server Implementation
 * 
 * This is a sample implementation of a Model Context Protocol (MCP) server, demonstrating how to create a server with stateful tools and parameterized functions. It serves as an example for building MCP servers with the ModelContextProtocol library. The server provides three tools:
 * 
 * - StartTimer: Records the current UTC time and returns it in ISO 8601 format.
 * - GetCurrentElapsedSeconds: Returns the elapsed time in seconds since the timer was started.
 * - GetStartTime: Returns the recorded start time in a user-specified format, compatible with DateTime.ToString.
 */

using ModelContextProtocol.Server;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.ComponentModel;

namespace McpAdapter.SampleStdioServer
{
    [McpServerToolType]
    static class Program
    {
        private static DateTime? _startTime;

        static async Task Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);

            builder.Services.AddMcpServer()
                .WithStdioServerTransport()
                .WithToolsFromAssembly();

            var host = builder.Build();

            await host.RunAsync();
        }

        [McpServerTool, Description("Starts a timer by recording the current date and time. This can be used by agents to initiate timing measurements for processes or workflows.")]
        public static string StartTimer()
        {
            _startTime = DateTime.UtcNow;
            return _startTime.Value.ToString("O");
        }

        [McpServerTool, Description("Retrieves the elapsed seconds since the timer was started. Useful for agents to monitor duration of ongoing tasks or performance metrics. Requires the timer to have been started previously.")]
        public static double GetCurrentElapsedSeconds()
        {
            if (_startTime == null)
            {
                throw new InvalidOperationException("Timer has not been started. Call StartTimer first.");
            }
            return DateTime.UtcNow.Subtract(_startTime.Value).TotalSeconds;
        }

        [McpServerTool, Description("Returns the start time of the timer formatted according to the provided format string. This helps agents in logging or reporting timestamps in custom formats. Requires the timer to have been started previously.")]
        public static string GetStartTime(
            [Description("The format string for DateTime.ToString(). Examples: 'yyyy-MM-dd HH:mm:ss' for detailed date/time, 'HH:mm:ss' for time only, or 'yyyy-MM-dd' for date only. Defaults to 'O' (ISO 8601) if empty.")]
            string format = "O")
        {
            if (_startTime == null)
            {
                throw new InvalidOperationException("Timer has not been started. Call StartTimer first.");
            }
            try
            {
                return _startTime.Value.ToString(format);
            }
            catch (FormatException)
            {
                throw new ArgumentException($"Invalid format string: {format}. Refer to DateTime.ToString documentation for valid formats.");
            }
        }
    }
}
