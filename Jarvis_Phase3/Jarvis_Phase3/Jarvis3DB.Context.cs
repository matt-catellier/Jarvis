﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Jarvis_Phase3
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class JarvisEntities : DbContext
    {
        public JarvisEntities()
            : base("name=JarvisEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<C__MigrationHistory> C__MigrationHistory { get; set; }
        public virtual DbSet<AspNetRole> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUser> AspNetUsers { get; set; }
        public virtual DbSet<Account> Accounts { get; set; }
        public virtual DbSet<Detail> Details { get; set; }
        public virtual DbSet<Device> Devices { get; set; }
        public virtual DbSet<DeviceData> DeviceDatas { get; set; }
        public virtual DbSet<ProviderAccount> ProviderAccounts { get; set; }
        public virtual DbSet<StoredData> StoredDatas { get; set; }
    }
}
