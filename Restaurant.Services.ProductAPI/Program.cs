using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Restaurant.Services.ProductAPI;
using Restaurant.Services.ProductAPI.DbContexts;
using Restaurant.Services.ProductAPI.Repository;
using Restaurant.Services.ProductAPI.Repository.Contracts;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1",new OpenApiInfo { Title = "Restaurant.Services.ProductAPI", Description = "ProductAPI" });
    options.EnableAnnotations();
    options.AddSecurityDefinition("Bearer",new OpenApiSecurityScheme
    {
        Description = @"Enter 'Bearer' [space] and your token",
        Name ="Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
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
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

IMapper mapper = MappingConfig.RegisterMap().CreateMapper();

builder.Services.AddSingleton(mapper);
builder.Services.AddScoped<IProductRepository,ProductRepository>();

//Authen
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer",options =>
    {
        options.Authority = "https://localhost:7255/";
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ApiScope", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("scope", "RestaurantApp");
    });
});

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
