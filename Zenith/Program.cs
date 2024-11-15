using Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Zenith.BLL.Interface;
using Zenith.BLL.Logic;
using Zenith.Repository.Data;
using Zenith.Repository.DomainModels;
using Zenith.Repository.RepositoryFiles;
var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("ZenithDbContextConnection")
    ?? throw new InvalidOperationException("Connection string 'ZenithDbContextConnection' not found.");

builder.Services.AddDbContext<ZenithDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(opt =>
{
    opt.SignIn.RequireConfirmedAccount = true;
    opt.Lockout.AllowedForNewUsers = true;
    opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(2);
    opt.Lockout.MaxFailedAccessAttempts = 3;
})
.AddEntityFrameworkStores<ZenithDbContext>()
.AddDefaultUI()
.AddRoles<IdentityRole>()
.AddDefaultTokenProviders();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(8);
});
builder.Services.AddRazorPages();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

//Tables repo scoped

builder.Services.AddScoped<IRepository<Manufacturer>, Repository<Manufacturer>>();
builder.Services.AddScoped<IRepository<VendorsInitializationForm>, Repository<VendorsInitializationForm>>();
builder.Services.AddScoped<IRepository<Registrations>, Repository<Registrations>>();
builder.Services.AddScoped<IRepository<QualityCertification>, Repository<QualityCertification>>();
builder.Services.AddScoped<IRepository<Products>, Repository<Products>>();
builder.Services.AddScoped<IRepository<PaymentTerms>, Repository<PaymentTerms>>();
builder.Services.AddScoped<IRepository<AccountDetails>, Repository<AccountDetails>>();
builder.Services.AddScoped<IRepository<OtherDocuments>, Repository<OtherDocuments>>();
builder.Services.AddScoped<IRepository<DropdownValues>, Repository<DropdownValues>>();
builder.Services.AddScoped<IRepository<DropdownLists>, Repository<DropdownLists>>();
builder.Services.AddScoped<IRepository<Contacts>, Repository<Contacts>>();
builder.Services.AddScoped<IRepository<Brands>, Repository<Brands>>();
builder.Services.AddScoped<IRepository<Attachments>, Repository<Attachments>>();
builder.Services.AddScoped<IRepository<Address>, Repository<Address>>();
builder.Services.AddScoped<IRepository<VacationRequests>, Repository<VacationRequests>>();
builder.Services.AddScoped<IRepository<DelegationRequests>, Repository<DelegationRequests>>();

// Register your repositories and logic services with scoped lifetime
builder.Services.AddScoped<IRepository<ApplicationUser>, Repository<ApplicationUser>>();
builder.Services.AddScoped<EmailUtils>();
builder.Services.AddScoped<IUser, UserLogic>();
builder.Services.AddScoped<IManufacturer, ManufacturerLogic>();
builder.Services.AddScoped<IVendors, VendorsLogic>();
builder.Services.AddScoped<IDropdownList, DropdownListLogic>();
builder.Services.AddScoped<ISetting, SettingLogic>();
builder.Services.AddScoped<IVacationRequests, VacationRequestsLogic>();
builder.Services.AddScoped<IDelegationRequests, DelegationRequestsLogic>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts(); 
}

app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();
app.MapControllerRoute(  name: "default",  pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();
app.Run();
