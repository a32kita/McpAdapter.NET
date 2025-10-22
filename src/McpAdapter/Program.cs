using ModelContextProtocol.Server;

namespace McpAdapter
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddMcpServer()
                .WithHttpTransport()
                .WithToolsFromAssembly();

            var app = builder.Build();

            app.MapMcp();
            app.Run();
        }
    }
}
