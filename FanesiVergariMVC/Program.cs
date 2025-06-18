var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "autovelox",
    pattern: "autovelox/{action=MostraAutovelox}/{id?}",
    defaults: new { controller = "Autovelox" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Autovelox}/{action=MostraAutovelox}/{id?}");

app.Run();