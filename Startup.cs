using MaliehIran.Extensions;
using MaliehIran.Infrastructure;
using MaliehIran.Services.MessageServices;
using MaliehIran.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Json.Serialization;
using Swashbuckle.AspNetCore.Newtonsoft;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;
using Microsoft.OpenApi.Any;

namespace MaliehIran
{
    public class Startup
    {
        private readonly JwtSetting _jwtSetting;
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            _jwtSetting = configuration.GetSection(nameof(JwtSetting)).Get<JwtSetting>();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSignalR();
            services.AddControllersWithViews()
               .AddNewtonsoftJson(options =>
               options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
               );
            
            services.AddDistributedMemoryCache();
            services.AddDbContext<ProjectDBContext>(o =>
            { o.UseSqlServer(Configuration.GetConnectionString("MaliehIran")); });
            
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSession();
            services.AddDependency();
            services.AddSettings(Configuration);
            services.AddJwtAuthentication(_jwtSetting);
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "MaliehIran", Version = "v1"});
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n 
                      Enter 'Bearer' [space] and then your token in the text input below.
                      \r\n\r\nExample: 'Bearer 12345abcdef'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });
                c.SchemaFilter<EnumSchemaFilter>();
                //c.DescribeAllEnumsAsStrings();
                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,

                        },
                        new List<string>()
                    }
                });
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                //var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                //c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
            });
            //services.AddControllersWithViews()
            //    .AddJsonOptions(options =>
            //    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
            services.AddSwaggerGenNewtonsoftSupport();
            
            services.AddCors();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "MaliehIran v1");
                c.RoutePrefix = string.Empty;
                c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
            });
            app.UseHttpsRedirection();
            app.UseCors(o =>
            {
                o.AllowAnyMethod();
                o.AllowAnyHeader();
                o.SetIsOriginAllowed(origin => true);
                o.AllowCredentials();
            });

            app.UseStaticFiles();

            app.UseRouting();

            app.UseSession();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<MessageService>("/chatHub");
            });
        }
    }
    internal sealed class EnumSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema model, SchemaFilterContext context)
        {
            if (context.Type.IsEnum)
            {
                model.Enum.Clear();
                Enum
                   .GetNames(context.Type)
                   .ToList()
                   .ForEach(name => model.Enum.Add(new OpenApiString($"{name}")));
                model.Type = "string";
                model.Format = string.Empty;
            }
        }
    }
}
