using System;
using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace CloudMicroServices.Web
{
	public class Program
	{
		public static IConfiguration Configuration { get; } = new ConfigurationBuilder()
																   .SetBasePath(Directory.GetCurrentDirectory())
																   .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
																   .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: false, reloadOnChange: true)
																   .Build();

		public static void Main(string[] args)
		{
			BuildWebHost(args).Run();
		}

		public static IWebHost BuildWebHost(string[] args)
		{
			return WebHost.CreateDefaultBuilder(args)
							.UseKestrel()
							.UseStartup<Startup>()
							.UseConfiguration(Configuration)
							.Build();
		}
	}
}
