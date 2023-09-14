using SneakerShopMongoDB.Models;
using SneakerShopMongoDB.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.Configure<SneakerShopDatabaseSettings>(
    builder.Configuration.GetSection("SneakerShopDatabase"));

builder.Services.AddSingleton<SneakerShopService>();

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
	options.IdleTimeout = TimeSpan.FromMinutes(20);
	options.Cookie.HttpOnly = true;
	options.Cookie.IsEssential = true;
});

builder.Logging.AddFile("Logs/SneakerShopLogs-{Date}.txt");

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error");
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseSession();

app.MapRazorPages();

app.Run();
