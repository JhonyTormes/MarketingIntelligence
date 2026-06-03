using MarketingIntelligence.Modules.Identity.Infrastructure;
using MarketingIntelligence.Modules.LinkShortener.Infrastructure;
using MarketingIntelligence.Modules.Notification.Infrastructure;
using MassTransit;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]!))
        };
    });

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Define the Bearer security scheme
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Please enter your JWT token into the field below."
    });

    // Apply the security requirement globally to all endpoints
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});
builder.Services.AddControllers();

// Register Modules
builder.Services.AddLinkShortenerModule(builder.Configuration);
builder.Services.AddIdentityModule(builder.Configuration);
builder.Services.AddNotificationModule(builder.Configuration);

var moduleAssemblies = new[]
{
    typeof(IdentityModuleServiceCollectionExtension).Assembly,
    typeof(LinkShortenerModuleServiceCollectionExtensions).Assembly,
    typeof(NotificationModuleServiceCollectionExtension).Assembly
};

builder.Services.AddMassTransit(x =>
{
    x.SetKebabCaseEndpointNameFormatter();


    x.AddConsumers(moduleAssemblies);

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.ConfigureEndpoints(context);
    });
});

// Configure Forwarded Headers for Linux/Container hosting
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor | 
                               Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto;
});

builder.Services.AddControllers()
    .AddApplicationPart(typeof(LinkShortenerModuleServiceCollectionExtensions).Assembly);

builder.Services.AddControllers()
    .AddApplicationPart(typeof(IdentityModuleServiceCollectionExtension).Assembly);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontEnd", policy =>
    {
        policy.WithOrigins("http://127.0.0.1:5501", "http://localhost:5501")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseForwardedHeaders(); // Must be before other middleware

app.UseHttpsRedirection();

app.UseCors("AllowFrontEnd");

app.MapControllers();

app.Run();
