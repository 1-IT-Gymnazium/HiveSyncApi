using HiveSync.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Text.RegularExpressions;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System;

namespace HiveSyncApi;

public class Startup
{
    public const string APPLICATION_NAME = "HiveSync";
    private static readonly Regex ANGULAR_BUNDLE_RX = new(".*\\.[0-9a-f]{20}\\..*", RegexOptions.Compiled);
    private const string CorsPolicy = "_corsPolicy";

    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _hostEnvironment;

    public Startup(IConfiguration configuration, IWebHostEnvironment hostEnvironment)
    {
        _configuration = configuration;
        _hostEnvironment = hostEnvironment;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            options.UseNpgsql(connectionString, builder =>
            {
                builder.UseNodaTime();
            });
        });

        services.AddControllers();
            //.AddNewtonsoftJson();

        services.AddSwaggerGen(config =>
        {
            config.SwaggerDoc("v1", new OpenApiInfo { Title = "DynamicForms API ", Version = "v1" });
            //config.DocumentFilter<JsonPatchDocumentFilter>();
            //config.RequestBodyFilter<JsonPatchDocumentFilter>();

            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            config.IncludeXmlComments(xmlPath);
        });

        services.Scan(scan =>
                scan.FromEntryAssembly()
                    .FromApplicationDependencies()
                    .AddClasses(c => c.AssignableTo<ITransientService>())
                        .AsImplementedInterfaces()
                        .WithTransientLifetime()
                    .AddClasses(c => c.AssignableTo<IScopedService>())
                        .AsImplementedInterfaces()
                        .WithScopedLifetime()
                    .AddClasses(c => c.AssignableTo<ISingletonService>())
                        .AsImplementedInterfaces()
                        .WithSingletonLifetime()
            )
            ;

        var serviceProvider = services.BuildServiceProvider();
        foreach (var serviceType in serviceProvider.GetRequiredServices().Sele)
        {
            // Process each service type
        }
    }

    public void Configure(IApplicationBuilder app)
    {
        // Configure the HTTP request pipeline.
        if (_hostEnvironment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();

            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "DynamicForms API v1");
            });
        }

        //app.UseHttpsRedirection();

        app.UseRouting();

        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}
