﻿//------------------------------------------------------------------------------
// <auto-generated>
//     這個程式碼是由範本產生。
//
//     對這個檔案進行手動變更可能導致您的應用程式產生未預期的行為。
//     如果重新產生程式碼，將會覆寫對這個檔案的手動變更。
// </auto-generated>
//------------------------------------------------------------------------------

namespace AutoTransfer
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class IFRS9Entities : DbContext
    {
        public IFRS9Entities()
            : base("name=IFRS9Entities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Bond_Account_Info> Bond_Account_Info { get; set; }
        public virtual DbSet<Rating_SP_Info> Rating_SP_Info { get; set; }
        public virtual DbSet<Rating_CW_Info> Rating_CW_Info { get; set; }
        public virtual DbSet<Rating_Fitch_Info> Rating_Fitch_Info { get; set; }
        public virtual DbSet<Rating_Moody_Info> Rating_Moody_Info { get; set; }
        public virtual DbSet<IFRS9_SFTP_Log> IFRS9_SFTP_Log { get; set; }
    }
}