﻿//------------------------------------------------------------------------------
// <auto-generated>
//    此代码是根据模板生成的。
//
//    手动更改此文件可能会导致应用程序中发生异常行为。
//    如果重新生成代码，则将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace TheDataResourceExporter
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class DataSourceEntities : DbContext
    {
        public DataSourceEntities()
            : base("name=DataSourceEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<S_AMERICA_APPLY_BRAND> S_AMERICA_APPLY_BRAND { get; set; }
        public DbSet<S_AMERICA_TRANSFER_BRAND> S_AMERICA_TRANSFER_BRAND { get; set; }
        public DbSet<S_CHINA_BRAND> S_CHINA_BRAND { get; set; }
        public DbSet<S_CHINA_CIRCUITLAYOUT> S_CHINA_CIRCUITLAYOUT { get; set; }
        public DbSet<S_CHINA_COURTCASE_PROCESS> S_CHINA_COURTCASE_PROCESS { get; set; }
        public DbSet<S_CHINA_CUSTOMS_RECORD> S_CHINA_CUSTOMS_RECORD { get; set; }
        public DbSet<S_CHINA_PATENT_INVALID> S_CHINA_PATENT_INVALID { get; set; }
        public DbSet<S_CHINA_PATENT_JUDGMENT> S_CHINA_PATENT_JUDGMENT { get; set; }
        public DbSet<S_CHINA_PATENT_LAWSPROCESS> S_CHINA_PATENT_LAWSPROCESS { get; set; }
        public DbSet<S_CHINA_PATENT_REVIEW> S_CHINA_PATENT_REVIEW { get; set; }
        public DbSet<S_COMMUNITY_INTELLECTUALRECORD> S_COMMUNITY_INTELLECTUALRECORD { get; set; }
        public DbSet<S_MADRID_BRAND_ENTER_CHINA> S_MADRID_BRAND_ENTER_CHINA { get; set; }
        public DbSet<S_MADRID_BRAND_PURCHASE> S_MADRID_BRAND_PURCHASE { get; set; }
        public DbSet<S_T_BIOLOGICAL_CN> S_T_BIOLOGICAL_CN { get; set; }
        public DbSet<S_T_BIOLOGICAL_FY> S_T_BIOLOGICAL_FY { get; set; }
        public DbSet<S_T_MEDICINE_TRANS_T1> S_T_MEDICINE_TRANS_T1 { get; set; }
        public DbSet<S_T_PHARMACEUTICAL_T1> S_T_PHARMACEUTICAL_T1 { get; set; }
        public DbSet<S_DATA_RESOURCE_TYPES_DETAIL> S_DATA_RESOURCE_TYPES_DETAIL { get; set; }
        public DbSet<W_SJZYZTSXXX> W_SJZYZTSXXX { get; set; }
    }
}
