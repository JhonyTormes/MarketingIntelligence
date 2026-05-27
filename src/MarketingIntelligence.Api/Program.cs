using MarketingIntelligence.Modules.LinkShortener.Infrastructure;
using MarketingIntelligence.Modules.Identity.Infrastructure;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

// Register Modules
builder.Services.AddLinkShortenerModule(builder.Configuration);
builder.Services.AddIdentityModule(builder.Configuration);

var moduleAssemblies = new[]
{
    typeof(IdentityModuleServiceCollectionExtension).Assembly,
    typeof(LinkShortenerModuleServiceCollectionExtensions).Assembly
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
        policy.WithOrigins("http://127.0.0.1:5500", "http://localhost:5500")
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
