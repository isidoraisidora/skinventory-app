using Domain.Config;
using Domain.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Repository.Implementation;
using Repository.Interface;
using Service.Implementation;
using Service.Interface;
using Repository;
using Service.BackgroundJobs;
using Web.Mappers;

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
builder.Services.AddSingleton<IExpirationCalculator, ExpirationCalculator>();
builder.Services.AddHostedService<BackgroundEtlSyncJob>();



builder.Services.Configure<ProductEtlOptions>(builder.Configuration.GetSection("ProductEtl"));
builder.Services.AddHttpClient<IExternalProductApi, ExternalProductApi>(client =>
{
    client.BaseAddress = new Uri("https://world.openbeautyfacts.org/");
    client.DefaultRequestHeaders.Add("User-Agent", "SkincareInventory-StudentProject/1.0");
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                       throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages()
    .WithStaticAssets();

app.Run();
