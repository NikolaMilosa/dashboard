using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using dashboard.Components;
using dashboard.Components.Account;
using dashboard.Data;
using dashboard.Jobs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdministratorRole",
         policy => policy.RequireRole("admin"));
});

builder.Services.AddHostedService(x => new LogCleaner(x));

var app = builder.Build();

var scope = app.Services.CreateScope();
var context = scope.ServiceProvider.GetService<ApplicationDbContext>()!;
context.Database.Migrate();

var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
var userStore = scope.ServiceProvider.GetRequiredService<IUserStore<ApplicationUser>>();
if (!userManager.Users.Any())
{
    // Create a new admin user
    var email = Environment.GetEnvironmentVariable("ADMIN_EMAIL") ?? "admin@gmail.com";
    var password = Environment.GetEnvironmentVariable("ADMIN_PASS") ?? "adminPass123!!";
    var user = Activator.CreateInstance<ApplicationUser>();

    await userStore.SetUserNameAsync(user, email, CancellationToken.None);
    var emailStore = (IUserEmailStore<ApplicationUser>)userStore;
    await emailStore.SetEmailAsync(user, email, CancellationToken.None);

    user.EmailConfirmed = true;

    var result = await userManager.CreateAsync(user, password);

    if (!result.Succeeded)
    {
        var errors = result.Errors;
        throw new Exception($"Unable to create admin user with email: {email} with error: {errors}");
    }

    await userManager.AddToRoleAsync(user, "admin");
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

app.Run();
