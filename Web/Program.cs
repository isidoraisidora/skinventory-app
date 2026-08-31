using System.Text;
using Domain.Config;
using Domain.Services;
using Microsoft.EntityFrameworkCore;
using Repository.Implementation;
using Repository.Interface;
using Service.Implementation;
using Service.Interface;
using Repository;
using Service.BackgroundJobs;
using Web.Mappers;
using System.IdentityModel.Tokens.Jwt;  
using System.Security.Claims;             // Claim, ClaimTypes
using System.Text;                        // Encoding
using Microsoft.IdentityModel.Tokens;  
using Microsoft.AspNetCore.Authentication.JwtBearer;// SymmetricSecurityKey, SigningCredentials, SecurityAlgorithms

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<IIngredientReactionService, IngredientReactionService>();
builder.Services.AddScoped<IIngredientService, IngredientService>();
builder.Services.AddScoped<IInventoryItemService, InventoryItemService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IWishlistItemService, WishlistItemService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IEtlService, EtlSyncService>();
builder.Services.AddScoped<ProductMapper>();
builder.Services.AddScoped<WishlistItemMapper>();
builder.Services.AddScoped<InventoryItemMapper>();
builder.Services.AddScoped<CategoryMapper>();
builder.Services.AddSingleton<IExpirationCalculator, ExpirationCalculator>();
builder.Services.AddHostedService<BackgroundEtlSyncJob>();

builder.Services.AddScoped<IAuthService, AuthService>();

var jwtSettings = builder.Configuration.GetSection("Jwt");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings["Key"]!))
        };
    });

builder.Services.AddAuthorization();



builder.Services.Configure<ProductEtlOptions>(builder.Configuration.GetSection("ProductEtl"));
builder.Services.AddHttpClient<IExternalProductApi, ExternalProductApi>(client =>
{
    client.BaseAddress = new Uri("https://world.openbeautyfacts.org/");
    client.DefaultRequestHeaders.Add("User-Agent", "SkincareInventoryApp/1.0 (isidorakuzmanovska@gmail.com)");
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                       throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();


builder.Services.AddControllersWithViews();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
