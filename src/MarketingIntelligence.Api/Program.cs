using MarketingIntelligence.Modules.LinkShortener.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

// Register Modules
builder.Services.AddLinkShortenerModule(builder.Configuration);

// Configure Forwarded Headers for Linux/Container hosting
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor | 
                               Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto;
});

builder.Services.AddControllers()
    .AddApplicationPart(typeof(LinkShortenerModuleServiceCollectionExtensions).Assembly);


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseForwardedHeaders(); // Must be before other middleware

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
