//------------------------------------------------------------------------------
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
    using System.Collections.Generic;
    
    public partial class Econ_Domestic
    {
        public string Year_Quartly { get; set; }
        public string Date { get; set; }
        public Nullable<double> TWSE_Index { get; set; }
        public Nullable<double> TWRGSARP_Index { get; set; }
        public Nullable<double> TWGDPCON_Index { get; set; }
        public Nullable<double> TWLFADJ_Index { get; set; }
        public Nullable<double> TWCPI_Index { get; set; }
        public Nullable<double> TWMSA1A_Index { get; set; }
        public Nullable<double> TWMSA1B_Index { get; set; }
        public Nullable<double> TWMSAM2_Index { get; set; }
        public Nullable<double> GVTW10YR_Index { get; set; }
        public Nullable<double> TWTRBAL_Index { get; set; }
        public Nullable<double> TWTREXP_Index { get; set; }
        public Nullable<double> TWTRIMP_Index { get; set; }
        public Nullable<double> TAREDSCD_Index { get; set; }
        public Nullable<double> TWCILI_Index { get; set; }
        public Nullable<double> TWBOPCUR_Index { get; set; }
        public Nullable<double> EHCATW_Index { get; set; }
        public Nullable<double> TWINDPI_Index { get; set; }
        public Nullable<double> TWWPI_Index { get; set; }
        public Nullable<double> TARSYOY_Index { get; set; }
        public Nullable<double> TWEOTTL_Index { get; set; }
        public Nullable<double> SLDETIGT_Index { get; set; }
        public Nullable<double> TWIRFE_Index { get; set; }
        public Nullable<double> SINYI_HOUSE_PRICE_index { get; set; }
        public Nullable<double> CATHAY_ESTATE_index { get; set; }
        public Nullable<double> Real_GDP2011 { get; set; }
        public Nullable<double> MCCCTW_Index { get; set; }
        public Nullable<double> TRDR1T_Index { get; set; }
    
        public virtual Econ_D_YYYYMMDD Econ_D_YYYYMMDD { get; set; }
        public virtual Loan_default_Info Loan_default_Info { get; set; }
    }
}
