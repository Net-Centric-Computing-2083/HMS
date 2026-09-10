var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// Register a shared connection-string accessor for the Data Access layer.
builder.Services.AddSingleton<string>(sp =>
    builder.Configuration.GetConnectionString("HospitalDb")
        ?? throw new InvalidOperationException("Connection string 'HospitalDb' not found."));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
