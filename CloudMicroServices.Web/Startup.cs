using System.IO;
using System.Text;
using AutoMapper;
using CloudMicroServices.Web.Core.Configurations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Server.IISIntegration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace CloudMicroServices.Web
{
	public class Startup
	{
		public IConfiguration Configuration { get; }

		public void ConfigureServices(IServiceCollection Services)
		{
			Services.AddAutoMapper(typeof(Startup));
			Services.AddAuthentication(IISDefaults.AuthenticationScheme);

			IoC.RegisterAppServices(Services);
			Services.AddTransient(s => s.GetService<IHttpContextAccessor>().HttpContext.User);

			var AppSettingsSection = Configuration.GetSection("AppSettings");
			Services.Configure<AppSettings>(AppSettingsSection);

			var AppSettings = AppSettingsSection.Get<AppSettings>();
			var Key = Encoding.ASCII.GetBytes(AppSettings.Secret);

			Services.AddAuthentication(x =>
			{
				x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			})
			.AddJwtBearer(x =>
			{
				x.RequireHttpsMetadata = false;
				x.SaveToken = true;
				x.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuerSigningKey = true,
					IssuerSigningKey = new SymmetricSecurityKey(Key),
					ValidateIssuer = false,
					ValidateAudience = false
				};
			});
			
			Services.AddAuthorization(Options =>
			{
				Options.AddPolicy("Authorized", policy => policy.RequireClaim("UserAuthorized"));
				Options.AddPolicy("Unauthorized", policy => policy.RequireClaim("UserUnauthorized"));
			});

			Services.AddMvc(Configurations =>
			{
				var Policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
				Configurations.Filters.Add(new AuthorizeFilter(Policy));
			})
			.AddJsonOptions(Options => {
				Options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
				Options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
				Options.SerializerSettings.PreserveReferencesHandling = PreserveReferencesHandling.None;
				Options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver();
			});
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder App, IHostingEnvironment Environment)
		{
			if (Environment.IsDevelopment())
				App.UseDeveloperExceptionPage();

			App.Use(async (Context, Next) =>
			{
				await Next();

				if (Context.Response.StatusCode == 404 && !Path.HasExtension(Context.Request.Path.Value))
				{
					Context.Request.Path = "/";
					Context.Response.StatusCode = 200;
					await Next();
				}
			});

			App.UseDefaultFiles()
				.UseStaticFiles()
				.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin().AllowCredentials())
				.UseAuthentication()
				.UseMvc();
		}
	}
}
