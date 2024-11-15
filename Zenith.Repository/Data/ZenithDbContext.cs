using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Zenith.Repository.DomainModels;

namespace Zenith.Repository.Data;

public class ZenithDbContext : IdentityDbContext<ApplicationUser>
{
    public ZenithDbContext(DbContextOptions<ZenithDbContext> options)
        : base(options)
    {
    }

    //protected override void OnModelCreating(ModelBuilder builder)
    //{
    //    base.OnModelCreating(builder);
    //    // Customize the ASP.NET Identity model and override the defaults if needed.
    //    // For example, you can rename the ASP.NET Identity table names and more.
    //    // Add your customizations after calling base.OnModelCreating(builder);
    //}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        base.OnModelCreating(modelBuilder);

        // Disable cascading deletes globally
        foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
        {
            relationship.DeleteBehavior = DeleteBehavior.Restrict;
        }
    }

    public virtual DbSet<Manufacturer> Manufacturer { get; set; }
    public virtual DbSet<DropdownLists> DropdownLists { get; set; }
    public virtual DbSet<DropdownValues> DropdownValues { get; set; }
    public virtual DbSet<VendorsInitializationForm> VendorsInitializationForm { get; set; }
    public virtual DbSet<Address> Address { get; set; }
    public virtual DbSet<Contacts> Contacts { get; set; }
    public virtual DbSet<Products> Products { get; set; }
    public virtual DbSet<Registrations> Registrations { get; set; }
    public virtual DbSet<QualityCertification> QualityCertification { get; set; }
    public virtual DbSet<AccountDetails> AccountDetails { get; set; }
    public virtual DbSet<PaymentTerms> PaymentTerms { get; set; }
    public virtual DbSet<OtherDocuments> OtherDocuments { get; set; }
    public virtual DbSet<Attachments> Attachments { get; set; }
    public virtual DbSet<ApplicationUser> ApplicationUser { get; set; }
    public virtual DbSet<ApplicationRoles> ApplicationRoles { get; set; }
    public virtual DbSet<SecurityGroups> SecurityGroups { get; set; }
    public virtual DbSet<SecurityGroupsRoles> SecurityGroupsRoles { get; set; }
    public virtual DbSet<VendorQualificationWorkFlow> VendorQualificationWorkFlow { get; set; }
    public virtual DbSet<VendorQualificationWorkFlowExecution> VendorQualificationWorkFlowExecution { get; set; }
    public virtual DbSet<VacationRequests> VacationRequests { get; set; }
    public virtual DbSet<DelegationRequests> DelegationRequests { get; set; }
}
