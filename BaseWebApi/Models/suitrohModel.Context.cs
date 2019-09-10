﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BaseWebApi.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class SuitrohDBEntities : DbContext
    {
        public SuitrohDBEntities()
            : base("name=SuitrohDBEntities")
        {
            Configuration.ProxyCreationEnabled = false;
            Configuration.LazyLoadingEnabled = false;
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<sysdiagram> sysdiagrams { get; set; }
        public virtual DbSet<ActivityTag> ActivityTags { get; set; }
        public virtual DbSet<Audit_CompanyStockTag> Audit_CompanyStockTag { get; set; }
        public virtual DbSet<Audit_StockIn> Audit_StockIn { get; set; }
        public virtual DbSet<Audit_Supplier> Audit_Supplier { get; set; }
        public virtual DbSet<CompanyStockTag> CompanyStockTags { get; set; }
        public virtual DbSet<Country> Countries { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<FieldAgent> FieldAgents { get; set; }
        public virtual DbSet<OrderItem> OrderItems { get; set; }
        public virtual DbSet<Sale> Sales { get; set; }
        public virtual DbSet<SaleReport> SaleReports { get; set; }
        public virtual DbSet<State> States { get; set; }
        public virtual DbSet<StockIn> StockIns { get; set; }
        public virtual DbSet<Supplier> Suppliers { get; set; }
        public virtual DbSet<Audit_CurrentStock> Audit_CurrentStock { get; set; }
        public virtual DbSet<Gender> Genders { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<CurrentStock> CurrentStocks { get; set; }
        public virtual DbSet<Order_Audit_CurrentStock> Order_Audit_CurrentStock { get; set; }
        public virtual DbSet<Order_CurrentStock> Order_CurrentStock { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
    }
}
