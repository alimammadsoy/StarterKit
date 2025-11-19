using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using StarterKit.Application;
using StarterKit.Application.Abstractions.Services;
using StarterKit.Infrastructure;
using StarterKit.Infrastructure.Services;
using StarterKit.Persistence;
using StarterKit.Persistence.Contexts;
using StarterKit.WebApi.Configurations;
using StarterKit.WebApi.Filters;
using StarterKit.WebApi.Middlewares;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Security.Claims;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder =>
        {
            builder     //WithOrigins("http://10.2.23.117:94", "http://10.218.254.67:94")\
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
            //.AllowCredentials();
        });
});


builder.Services.AddPersistenceServices(builder.Configuration);
builder.Services.AddInfrastructureServices();
builder.Services.AddApplicationServices();

builder.Services.AddSingleton<ILocalizationService, LocalizationService>();
builder.Services.AddScoped<LocalizeResponseFilter>();

builder.Services.AddControllers(opt =>
{
    opt.Conventions.Add(new RouteTokenTransformerConvention(new KebabCaseParameterTransformer()));
    opt.Filters.Add<LocalizeResponseFilter>();
})
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new AzerbaijanDateTimeConverter());
    options.JsonSerializerOptions.Converters.Add(new NullableAzerbaijanDateTimeConverter());
});


builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddHttpClient();

builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddScoped<ApplicationSeedDbContext>();

builder.Services.AddHttpLogging(logging =>
{
    logging.LoggingFields = HttpLoggingFields.All;
    logging.RequestHeaders.Add("sec-chu-ua");
    logging.MediaTypeOptions.AddText("application/javascript");
    logging.RequestBodyLogLimit = 4096;
    logging.ResponseBodyLogLimit = 4096;
});

builder.Logging.ClearProviders();

LoggingConfiguration.ConfigureLogger(builder);

builder.Services.AddSwaggerGen(c =>
{
    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "JWT Authentication",
        Description = "Enter your JWT token in this field",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };

    c.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { securityScheme, new string[] { } }
    });
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddHealthChecks();

builder.Services.Configure<KestrelServerOptions>(options =>
{
    options.AddServerHeader = false;
    options.Limits.MaxRequestBodySize = int.MaxValue;
});

builder.Services.Configure<FormOptions>(x =>
{
    x.ValueLengthLimit = int.MaxValue;
    x.MultipartBodyLengthLimit = int.MaxValue;
});

/*builder.Services.Configure<JsonOptions>(options =>
{
    options.JsonSerializerOptions.Converters.Add(new AzerbaijanDateTimeConverter());
});*/


builder.Services.AddAuthentication(opt =>
{
    opt.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;

}).AddJwtBearer(opt =>
{
    opt.RequireHttpsMetadata = false;
    opt.SaveToken = true;

    opt.TokenValidationParameters = new()
    {
        ValidateAudience = true,
        ValidAudience = builder.Configuration.GetSection("JWT:Audience").Value,

        ValidateIssuer = true,
        ValidIssuer = builder.Configuration.GetSection("JWT:Issuer").Value,

        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetSection("JWT:SecretKey").Value)),

        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero,

        NameClaimType = ClaimTypes.Name,
        RoleClaimType = ClaimTypes.Role
    };
    opt.Events = new JwtBearerEvents()
    {
        OnMessageReceived = context =>
        {
            context.Token = context.Request.Cookies["X-Access-Token"];
            return Task.CompletedTask;
        },

        OnTokenValidated = context =>
        {
          /*  var tokenHandler = new JwtSecurityTokenHandler();
            var token = context.SecurityToken as JwtSecurityToken;

            if (token == null || token.ValidTo < DateTime.UtcNow) // Token expired?
            {
                context.Fail("Token has expired.");
            }*/

            return Task.CompletedTask;
        },

        OnAuthenticationFailed = context =>
        {
            return Task.CompletedTask;
        },
        OnForbidden = context =>
        {
            var token = context.Request.Cookies["X-Access-Token"];
            return Task.CompletedTask;
        }
    };
}).AddCookie(CookieAuthenticationDefaults.AuthenticationScheme);

//builder.Services.AddAuthorization(opt =>
//{
//    opt.AddPolicy(PolicyNames.AdminOnly, policy => policy.RequireClaim("userType", "A"));
//    opt.AddPolicy(PolicyNames.ProfessorOnly, policy => policy.RequireClaim("userType", "P"));
//    opt.AddPolicy(PolicyNames.AdminOrProfessor, policy =>
//        policy.RequireAssertion(context =>
//            context.User.HasClaim("userType", "A") ||
//            context.User.HasClaim("userType", "P")));

//    opt.AddPolicy(PolicyNames.EditScore, policy =>
//        policy.RequireAssertion(context =>
//            context.User.HasClaim("userType", "A") &&
//            context.User.HasClaim("canEditScore", "true")));
//});


var env = builder.Environment;

builder.Configuration
    .SetBasePath(env.ContentRootPath)
    .AddJsonFile("appsettings.json", optional: false)
    .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);



var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "StarterKit.API V1");
        c.DocumentTitle = "StarterKit API";
        c.DocExpansion(DocExpansion.List);
    });
    //app.UseDeveloperExceptionPage();
}

app.Use(async (context, next) =>
{
    context.Response.Headers["Cache-Control"] = "no-store, no-cache, must-revalidate";
    context.Response.Headers["Pragma"] = "no-cache";
    context.Response.Headers["Expires"] = "0";
    await next();
});

/*app.UseStaticFiles(new StaticFileOptions
{
    RequestPath = "/files"
});
*/
app.UseSerilogRequestLogging();
app.UseHttpLogging();
app.UseCors("AllowAllOrigins");

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddlewares();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();
    context.Database.Migrate();
    //var logger = services.GetRequiredService<ILogger<ApplicationSeedDbContext>>();
    //var seed = services.GetRequiredService<ApplicationSeedDbContext>();
    //await seed.SeedAsync(context, logger);

    //var logContext = services.GetRequiredService<LogDbContext>();
    //logContext.Database.Migrate();
}

app.MapFallbackToFile("index.html");
app.Run();
