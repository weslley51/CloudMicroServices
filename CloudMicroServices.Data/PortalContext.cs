using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Linq;

namespace CloudMicroServices.Data
{
	public class PortalContext : DbContext
	{
		protected override void OnConfiguring(DbContextOptionsBuilder OptionsBuilder)
		{
			IConfigurationRoot Configuration = new ConfigurationBuilder()
															.SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
															.AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json")
															.Build();

			OptionsBuilder.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
		}

		protected override void OnModelCreating(ModelBuilder ModelBuilder)
		{			
			//ModelBuilder.AddConfiguration(new UsuarioMap());

			ModelBuilder
				.Model
				.GetEntityTypes()
				.SelectMany(x => x.GetProperties())
				.Where
				(
					p => p.ClrType == typeof(string) ||
						 p.ClrType == typeof(DateTime) || p.ClrType == typeof(DateTime?) ||
						 p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)
				)
				.ToList()
				.ForEach(x =>
				{
					var Type = string.Empty;

					if (x.ClrType == typeof(string))
					{
						if (!x.GetMaxLength().HasValue)
							ModelBuilder.Entity(x.DeclaringType.ClrType).Property(x.Name).HasMaxLength(50);
					}
					else
					{
						if (x.ClrType == typeof(DateTime) || x.ClrType == typeof(DateTime?))
							Type = "DATETIME2(0)";
						else
							Type = "DECIMAL(18,3)";

						ModelBuilder.Entity(x.DeclaringType.ClrType).Property(x.Name).HasColumnType(Type);
					}
				});
		}
	}
}
