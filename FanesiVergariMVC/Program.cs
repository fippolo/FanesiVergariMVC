var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Abilita logging verboso per SOAP
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Debug);

// WCF logging
System.Diagnostics.Trace.Listeners.Add(new System.Diagnostics.ConsoleTraceListener());

// Antiforgery token
builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "RequestVerificationToken";
});

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
app.UseAntiforgery();
app.UseAuthorization();

app.MapControllerRoute(
    name: "debug",
    pattern: "debug",
    defaults: new { controller = "Autovelox", action = "Debug" });

app.MapControllerRoute(
    name: "autovelox",
    pattern: "autovelox/{action=MostraAutovelox}/{id?}",
    defaults: new { controller = "Autovelox" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Autovelox}/{action=MostraAutovelox}/{id?}");

app.Run();