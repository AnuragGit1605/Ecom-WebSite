using EcomWeb1.Data;
using EcomWeb1.DataAccess.Repo.IRepo;
using EcomWeb1.DataAccess.Repo;
using EcomWeb1.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Stripe;
using PayPal;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("con");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddRazorPages();
//builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
//    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddDefaultTokenProviders().AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();

builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("StripeSettings"));
builder.Services.Configure<PayPalSettings>(builder.Configuration.GetSection("PayPalSettings"));

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IEmailSender, EmailSender>();
builder.Services.AddScoped<ISMSSender, SMSSender>();
builder.Services.AddScoped<IPaypal, Paypal>();

builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.Configure<TwilioSettings>(builder.Configuration.GetSection("TwilioSettings"));


builder.Services.ConfigureApplicationCookie(option =>
{
    option.LoginPath = $"/Identity/Account/Login";
    option.AccessDeniedPath = $"/Identity/Account/AccessDenied";
    option.LogoutPath = $"/Identity/Account/Logout";
});
builder.Services.AddAuthentication().AddFacebook(options =>
{
    options.AppId = "2144443389269020";
    options.AppSecret = "a795c4c69eece142e0f08949bd33a973";
});
builder.Services.AddAuthentication().AddGoogle(options =>
{
    options.ClientId = "939539726818-gh4ckhve3iuuiagskg4jcvsrg0br5qja.apps.googleusercontent.com";
    options.ClientSecret = "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";
});
builder.Services.AddAuthentication().AddTwitter(options =>
{
    options.ConsumerKey = "ZlgET1rlYxCllS91i2ZvCc47l";
    options.ConsumerSecret = "LToyrBTLDSyJumEAHdROCcDY0rXei8YoNvNTwY2ZX2TdKPU3TF";
});
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

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
app.UseStaticFiles();

app.UseRouting();
StripeConfiguration.ApiKey = builder.Configuration.GetSection("StripeSettings")["SecretKey"];
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{area=Customers}/{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
