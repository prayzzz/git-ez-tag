using System.IO;
using System.Threading.Tasks;
using GitEzTag.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace GitEzTag
{
    internal static class Program
    {
        public static async Task<int> Main(string[] args)
        {
            return await new HostBuilder()
                         .ConfigureHostConfiguration(builder =>
                         {
                             builder.SetBasePath(Directory.GetCurrentDirectory());
                             builder.Add<JsonConfigurationSource>(src =>
                             {
                                 src.FileProvider = new EmbeddedFileProvider(typeof(Program).Assembly, "GitEzTag");
                                 src.Path = "appsettings.json";
                             });
                         })
                         .ConfigureServices((context, services) =>
                         {
                             services.AddSingleton<Git>();
                             services.AddSingleton<EzTag>();
                             services.AddSingleton<TagService>();
                             services.AddSingleton<AnnotationService>();
                         })
                         .UseSerilog((context, configuration) =>
                         {
                             configuration.MinimumLevel.Information();
                             configuration.WriteTo.Console(outputTemplate: "[{Level:u4}] {Message:lj}{NewLine}{Exception}");
                         })
                         .RunCommandLineApplicationAsync<EzTag>(args);
        }
    }
}