using OwaspTop10Demo.Helpers;

var builder = WebApplication.CreateBuilder(args);

// Controllers + Swagger
builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
     {
         options.JsonSerializerOptions.PropertyNamingPolicy = null;
     });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "OWASP Top 10 Demo API",
        Version = "v1"
    });
});

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("InsecureCors", policy =>
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

    options.AddPolicy("FrontendPolicy", policy =>
        policy.WithOrigins("https://localhost:7243",
                           "http://localhost:5243",
                           "https://localhost:4200",
                           "https://mi-frontend.com")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

// Rate limiter para A04
builder.Services.AddSingleton<SimpleLoginRateLimiter>();

var app = builder.Build();

// Crear base de datos SQLite local (A03)
SqliteDatabaseInitializer.Initialize();

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseCors("FrontendPolicy");
app.UseAuthorization();

app.MapControllers();

app.Run();
