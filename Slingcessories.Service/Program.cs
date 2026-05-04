using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Slingcessories.Service.Data;
using Slingcessories.Service.GraphQL;
using Slingcessories.Service.GraphQL.Types;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter JWT Bearer token only"
    });

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
            Array.Empty<string>()
        }
    });
});

// CORS - allow Blazor WASM dev client
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod());
});

// Add DbContext Factory for GraphQL
builder.Services.AddDbContextFactory<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add pooled DbContext for HotChocolate
builder.Services.AddPooledDbContextFactory<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services
    .AddIdentityCore<IdentityUser>(options =>
    {
        options.User.RequireUniqueEmail = true;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

var jwtSection = builder.Configuration.GetSection("Jwt");
var jwtKey = jwtSection["Key"] ?? throw new InvalidOperationException("JWT signing key is missing. Configure Jwt:Key in appsettings.");
var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
var validAudiences = jwtSection.GetSection("ValidAudiences").Get<string[]>()
    ?? ["Slingcessories.Blazor", "Slingcessories.Maui", "Slingcessories.React"];

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSection["Issuer"],
            ValidAudiences = validAudiences,
            IssuerSigningKey = signingKey,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

// GraphQL
builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddType<AccessoryType>()
    .AddType<SlingshotType>()
    .RegisterDbContext<AppDbContext>(DbContextKind.Pooled)
    .AddProjections()
    .AddFiltering()
    .AddSorting();

var app = builder.Build();

// Apply pending migrations (creates DB if it doesn't exist)
using (var scope = app.Services.CreateScope())
{
    var contextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<AppDbContext>>();
    using var db = contextFactory.CreateDbContext();
    db.Database.Migrate();

    db.Database.ExecuteSqlRaw("""
        UPDATE [AspNetUsers]
        SET [SecurityStamp] = COALESCE([SecurityStamp], CONVERT(nvarchar(36), NEWID())),
            [ConcurrencyStamp] = COALESCE([ConcurrencyStamp], CONVERT(nvarchar(36), NEWID()))
        WHERE [SecurityStamp] IS NULL OR [ConcurrencyStamp] IS NULL;
        """);

    // Optional: seed data here
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// GraphQL endpoint
app.MapGraphQL();

app.Run();
