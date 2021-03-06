﻿using AutoTransfer.Utility;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using static AutoTransfer.Enum.Ref;
using static AutoTransfer.Transfer.A53;

namespace AutoTransfer.Transfer
{
    public class CompleteEvent
    {
        private IFRS9Entities db = new IFRS9Entities();
        private Log log = new Log();
        private string A57logPath = string.Empty;
        private string A58logPath = string.Empty;

        /// <summary>
        /// A53 save 後續動作(save A57,A58)
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="version"></param>
        public void saveDb(DateTime dt, int version)
        {
            A57logPath = log.txtLocation(TableType.A57.ToString());
            A58logPath = log.txtLocation(TableType.A58.ToString());

            DateTime startTime = DateTime.Now;

            if (log.checkTransferCheck(TableType.A57.ToString(), TableType.A53.ToString(), dt, version) &&
                log.checkTransferCheck(TableType.A58.ToString(), TableType.A53.ToString(), dt, version))
            {
                var A41Data = db.Bond_Account_Info
                    .Where(x => x.Report_Date == dt &&
                                 x.Version != null &&
                                 x.Version == version);
                if (A41Data.Any())
                {
                    try
                    {
                            string parmID = getParmID(); //選取離今日最近的D60
                            string reportData = dt.ToString("yyyy/MM/dd");
                            string ver = version.ToString();
                            string sql = string.Empty;
                            sql = $@"
Begin Try

WITH T0 AS (
   Select BA_Info.Reference_Nbr AS Reference_Nbr ,
          BA_Info.Bond_Number AS Bond_Number,
		  BA_Info.Lots AS Lots,
		  BA_Info.Portfolio AS Portfolio,
		  BA_Info.Segment_Name AS Segment_Name,
		  BA_Info.Bond_Type AS Bond_Type,
		  BA_Info.Lien_position AS Lien_position,
		  BA_Info.Origination_Date AS Origination_Date,
          BA_Info.Report_Date AS Report_Date,
		  RA_Info.Rating_Date AS Rating_Date,
		  RA_Info.Rating_Object AS Rating_Object,
          RA_Info.Rating_Org AS Rating_Org,
		  RA_Info.Rating AS Rating,
		  (CASE WHEN RA_Info.Rating_Org in ('SP','cnSP','Fitch') THEN '國外'
	            WHEN RA_Info.Rating_Org in ('Fitch(twn)','CW') THEN '國內'
	      END) AS Rating_Org_Area,
		  GMapInfo.PD_Grade AS PD_Grade,
		  GMooInfo.Grade_Adjust AS Grade_Adjust,
		  CASE WHEN RISI.ISSUER_TICKER in ('N.S.', 'N.A.') THEN null Else RISI.ISSUER_TICKER END AS ISSUER_TICKER,
		  CASE WHEN RISI.GUARANTOR_NAME in ('N.S.', 'N.A.') THEN null Else RISI.GUARANTOR_NAME END AS GUARANTOR_NAME,
		  CASE WHEN RISI.GUARANTOR_EQY_TICKER in ('N.S.', 'N.A.') THEN null Else RISI.GUARANTOR_EQY_TICKER END AS GUARANTOR_EQY_TICKER,
		  BA_Info.Portfolio_Name AS Portfolio_Name,
		  RA_Info.RTG_Bloomberg_Field AS RTG_Bloomberg_Field,
		  BA_Info.PRODUCT AS SMF,
		  BA_Info.ISSUER AS ISSUER,
		  BA_Info.Version AS Version,
		  (CASE WHEN BA_Info.PRODUCT like 'A11%' OR BA_Info.PRODUCT like '932%' 
		  THEN BA_Info.Bond_Number + ' Mtge' ELSE
		    BA_Info.Bond_Number + ' Corp' END) AS Security_Ticker
   from  Bond_Account_Info BA_Info --A41
   Join Rating_Info RA_Info --A53
   on BA_Info.Bond_Number = RA_Info.Bond_Number   
   Left Join Grade_Mapping_Info GMapInfo --A52
   on RA_Info.Rating_Org = GMapInfo.Rating_Org
   AND RA_Info.Rating = GMapInfo.Rating
   Left Join Grade_Moody_Info GMooInfo --A51
   on GMapInfo.PD_Grade = GMooInfo.PD_Grade
   Left Join Rating_Info_SampleInfo RISI --A53 Sample
   on BA_Info.Bond_Number = RISI.Bond_Number
   AND RA_Info.Rating_Object = '債項'
   Where BA_Info.Report_Date = '{reportData}'
   And   BA_Info.Version = {ver}
)
Insert into Bond_Rating_Info
           (Reference_Nbr,
           Bond_Number,
           Lots,
           Portfolio,
           Segment_Name,
           Bond_Type,
           Lien_position,
           Origination_Date,
           Report_Date,
           Rating_Date,
           Rating_Type,
           Rating_Object,
           Rating_Org,
           Rating,
           Rating_Org_Area,
           PD_Grade,
           Grade_Adjust,
           ISSUER_TICKER,
           GUARANTOR_NAME,
           GUARANTOR_EQY_TICKER,
           Parm_ID,
           Portfolio_Name,
           RTG_Bloomberg_Field,
           SMF,
           ISSUER,
           Version,
           Security_Ticker)
Select     Reference_Nbr,
		   Bond_Number,
		   Lots,
           Portfolio,
           Segment_Name,
           Bond_Type,
           Lien_position,
           Origination_Date,
           Report_Date,
           Rating_Date,
           '1',
           Rating_Object,
           Rating_Org,
           Rating,
           Rating_Org_Area,
           PD_Grade,
           Grade_Adjust,
           ISSUER_TICKER,
           GUARANTOR_NAME,
           GUARANTOR_EQY_TICKER,
           '{parmID}',
           Portfolio_Name,
           RTG_Bloomberg_Field,
           SMF,
           ISSUER,
           Version,
           Security_Ticker
		   From
		   T0;
WITH T1 AS (
   Select BA_Info.Reference_Nbr AS Reference_Nbr ,
          BA_Info.Bond_Number AS Bond_Number,
		  BA_Info.Lots AS Lots,
		  BA_Info.Portfolio AS Portfolio,
		  BA_Info.Segment_Name AS Segment_Name,
		  BA_Info.Bond_Type AS Bond_Type,
		  BA_Info.Lien_position AS Lien_position,
		  BA_Info.Origination_Date AS Origination_Date,
          BA_Info.Report_Date AS Report_Date,
		  RA_Info.Rating_Date AS Rating_Date,
		  RA_Info.Rating_Object AS Rating_Object,
          RA_Info.Rating_Org AS Rating_Org,
		  oldA57.Rating AS Rating,
		  (CASE WHEN RA_Info.Rating_Org in ('SP','cnSP','Fitch') THEN '國外'
	            WHEN RA_Info.Rating_Org in ('Fitch(twn)','CW') THEN '國內'
	      END) AS Rating_Org_Area,
		  GMapInfo.PD_Grade AS PD_Grade,
		  GMooInfo.Grade_Adjust AS Grade_Adjust,
		  CASE WHEN RISI.ISSUER_TICKER in ('N.S.', 'N.A.') THEN null Else RISI.ISSUER_TICKER END AS ISSUER_TICKER,
		  CASE WHEN RISI.GUARANTOR_NAME in ('N.S.', 'N.A.') THEN null Else RISI.GUARANTOR_NAME END AS GUARANTOR_NAME,
		  CASE WHEN RISI.GUARANTOR_EQY_TICKER in ('N.S.', 'N.A.') THEN null Else RISI.GUARANTOR_EQY_TICKER END AS GUARANTOR_EQY_TICKER,
		  BA_Info.Portfolio_Name AS Portfolio_Name,
		  RA_Info.RTG_Bloomberg_Field AS RTG_Bloomberg_Field,
		  BA_Info.PRODUCT AS SMF,
		  BA_Info.ISSUER AS ISSUER,
		  BA_Info.Version AS Version,
		  (CASE WHEN BA_Info.PRODUCT like 'A11%' OR BA_Info.PRODUCT like '932%' 
		  THEN BA_Info.Bond_Number + ' Mtge' ELSE
		    BA_Info.Bond_Number + ' Corp' END) AS Security_Ticker
   from  Bond_Account_Info BA_Info --A41
   Join Rating_Info RA_Info --A53
   on BA_Info.Bond_Number = RA_Info.Bond_Number   
   Left Join Bond_Rating_Info oldA57 --oldA57
   on BA_Info.Bond_Number = oldA57.Bond_Number 
   AND BA_Info.Lots = oldA57.Lots 
   AND BA_Info.Portfolio_Name = oldA57.Portfolio_Name
   AND BA_Info.Origination_Date = oldA57.Origination_Date 
   AND oldA57.Rating_Type = '2'
   Left Join Grade_Mapping_Info GMapInfo --A52
   on RA_Info.Rating_Org = GMapInfo.Rating_Org
   AND oldA57.Rating = GMapInfo.Rating
   Left Join Grade_Moody_Info GMooInfo --A51
   on GMapInfo.PD_Grade = GMooInfo.PD_Grade
   Left Join Rating_Info_SampleInfo RISI --A53 Sample
   on BA_Info.Bond_Number = RISI.Bond_Number
   AND RA_Info.Rating_Object = '債項'
   Where BA_Info.Report_Date = '{reportData}'
   And   BA_Info.Version = {ver}
)
Insert into Bond_Rating_Info
           (Reference_Nbr,
           Bond_Number,
           Lots,
           Portfolio,
           Segment_Name,
           Bond_Type,
           Lien_position,
           Origination_Date,
           Report_Date,
           Rating_Date,
           Rating_Type,
           Rating_Object,
           Rating_Org,
           Rating,
           Rating_Org_Area,
           PD_Grade,
           Grade_Adjust,
           ISSUER_TICKER,
           GUARANTOR_NAME,
           GUARANTOR_EQY_TICKER,
           Parm_ID,
           Portfolio_Name,
           RTG_Bloomberg_Field,
           SMF,
           ISSUER,
           Version,
           Security_Ticker)
Select     Reference_Nbr,
		   Bond_Number,
		   Lots,
           Portfolio,
           Segment_Name,
           Bond_Type,
           Lien_position,
           Origination_Date,
           Report_Date,
           Rating_Date,
           '2',
           Rating_Object,
           Rating_Org,
           Rating,
           Rating_Org_Area,
           PD_Grade,
           Grade_Adjust,
           ISSUER_TICKER,
           GUARANTOR_NAME,
           GUARANTOR_EQY_TICKER,
           '{parmID}',
           Portfolio_Name,
           RTG_Bloomberg_Field,
           SMF,
           ISSUER,
           Version,
           Security_Ticker
		   From
		   T1;
Insert Into Bond_Rating_Summary
            (
			  Reference_Nbr,
              Report_Date,
              Parm_ID,
              Bond_Type,
              Rating_Type,
              Rating_Object,
              Rating_Org_Area,
              Rating_Selection,
              Grade_Adjust,
              Rating_Priority,
              --Processing_Date,
              Version,
              Bond_Number,
              Lots,
              Portfolio,
              Origination_Date,
              Portfolio_Name,
              SMF,
              ISSUER
			)
Select BR_Info.Reference_Nbr,
       BR_Info.Report_Date,
	   BR_Info.Parm_ID,
	   BR_Info.Bond_Type,
	   BR_Info.Rating_Type,
	   BR_Info.Rating_Object,
	   BR_Info.Rating_Org_Area,	   
	   BR_Parm.Rating_Selection,
	   (CASE WHEN BR_Parm.Rating_Selection = '1' 
	         THEN Min(BR_Info.Grade_Adjust)
			 WHEN BR_Parm.Rating_Selection = '2'
			 THEN Max(BR_Info.Grade_Adjust)
			 ELSE null  END) AS Grade_Adjust,
	   BR_Parm.Rating_Priority,
	   BR_Info.Version,
	   BR_Info.Bond_Number,
	   BR_Info.Lots,
	   BR_Info.Portfolio,
	   BR_Info.Origination_Date,
	   BR_Info.Portfolio_Name,
	   BR_Info.SMF,
	   BR_Info.ISSUER
From   Bond_Rating_Info BR_Info
LEFT JOIN  Bond_Rating_Parm BR_Parm
on         BR_Info.Parm_ID = BR_Parm.Parm_ID
AND        BR_Info.Rating_Object = BR_Parm.Rating_Object
AND        BR_Info.Rating_Org_Area = BR_Parm.Rating_Org_Area
Where  BR_Info.Report_Date = '{reportData}'
AND    BR_Info.Version = {ver}
GROUP BY BR_Info.Reference_Nbr,
         BR_Info.Report_Date,
		 BR_Info.Bond_Type,
	     BR_Info.Rating_Type,
	     BR_Info.Rating_Object,
	     BR_Info.Rating_Org_Area,
		 BR_Info.Parm_ID,
		 BR_Info.Version,
		 BR_Info.Bond_Number,
		 BR_Info.Lots,
	     BR_Info.Portfolio,
	     BR_Info.Origination_Date,
	     BR_Info.Portfolio_Name,
	     BR_Info.SMF,
	     BR_Info.ISSUER,
		 BR_Parm.Rating_Selection,
		 BR_Parm.Rating_Priority;
End Try Begin Catch End Catch
                    ";
                        db.Database.ExecuteSqlCommand(sql);
                        db.Dispose();
                        log.saveTransferCheck(
                            TableType.A57.ToString(),
                            true,
                            dt,
                            version,
                            startTime,
                            DateTime.Now);
                        log.saveTransferCheck(
                            TableType.A58.ToString(),
                            true,
                            dt,
                            version,
                            startTime,
                            DateTime.Now);
                        log.txtLog(
                           TableType.A57.ToString(),
                           true,
                           startTime,
                           A57logPath,
                           MessageType.Success.GetDescription());
                        log.txtLog(
                           TableType.A58.ToString(),
                           true,
                           startTime,
                           A58logPath,
                           MessageType.Success.GetDescription());
                    }
                    catch (Exception ex)
                    {
                        log.saveTransferCheck(
                            TableType.A57.ToString(),
                            false,
                            dt,
                            version,
                            startTime,
                            DateTime.Now);
                        log.saveTransferCheck(
                            TableType.A58.ToString(),
                            false,
                            dt,
                            version,
                            startTime,
                            DateTime.Now);
                        log.txtLog(
                            TableType.A57.ToString(),
                            false,
                            startTime,
                            A57logPath,
                            $"message: {ex.Message}" +
                            $", inner message {ex.InnerException?.InnerException?.Message}");
                        log.txtLog(
                            TableType.A58.ToString(),
                            false,
                            startTime,
                            A58logPath,
                            $"message: {ex.Message}" +
                            $", inner message {ex.InnerException?.InnerException?.Message}");
                    }
                }
                else
                {
                    log.saveTransferCheck(
                        TableType.A57.ToString(),
                        false,
                        dt,
                        version,
                        startTime,
                        DateTime.Now);
                    log.saveTransferCheck(
                        TableType.A58.ToString(),
                        false,
                        dt,
                        version,
                        startTime,
                        DateTime.Now);
                    log.txtLog(
                        TableType.A57.ToString(),
                        false,
                        startTime,
                        A57logPath,
                        MessageType.not_Find_Any.GetDescription("A41"));
                    log.txtLog(
                        TableType.A58.ToString(),
                        false,
                        startTime,
                        A58logPath,
                        MessageType.not_Find_Any.GetDescription("A41"));
                }
            }
            else
            {
                log.saveTransferCheck(
                    TableType.A57.ToString(),
                    false,
                    dt,
                    version,
                    startTime,
                    DateTime.Now);
                log.saveTransferCheck(
                    TableType.A58.ToString(),
                    false,
                    dt,
                    version,
                    startTime,
                    DateTime.Now);
                log.txtLog(
                    TableType.A57.ToString(),
                    false,
                    startTime,
                    A57logPath,
                    MessageType.transferError.GetDescription());
                log.txtLog(
                    TableType.A58.ToString(),
                    false,
                    startTime,
                    A58logPath,
                    MessageType.transferError.GetDescription());
            }
        }

        #region
        /// <summary>
        /// C03 Mortgage save
        /// </summary>
        /// <param name="yearQuartly"></param>
        public void saveC03Mortgage(string yearQuartly)
        {
            string logPath = log.txtLocation(TableType.C03Mortgage.ToString());
            DateTime startTime = DateTime.Now;

            try
            {
                List<Loan_default_Info> A06Data = db.Loan_default_Info
                                                  .Where(x => x.Year_Quartly == yearQuartly).ToList();

                List<Econ_Domestic> A07Data = db.Econ_Domestic
                                              .Where(x => x.Year_Quartly == yearQuartly).ToList();
                if (!A06Data.Any())
                {
                    log.txtLog(
                       TableType.C03Mortgage.ToString(),
                       false,
                       startTime,
                       logPath,
                       "Loan_default_Info 無 " + yearQuartly + " 的資料");
                }
                else if (!A07Data.Any())
                {
                    log.txtLog(
                       TableType.C03Mortgage.ToString(),
                       false,
                       startTime,
                       logPath,
                       "Econ_Domestic 無 " + yearQuartly + " 的資料");
                }
                else
                {
                    string productCode = GroupProductCode.M.GetDescription();
                    Group_Product_Code_Mapping gpcm = db.Group_Product_Code_Mapping.Where(x => x.Group_Product_Code.StartsWith(productCode)).FirstOrDefault();
                    if (gpcm == null)
                    {
                        log.txtLog(
                           TableType.C03Mortgage.ToString(),
                           false,
                           startTime,
                           logPath,
                           "Group_Product_Code_Mapping 無房貸的 Product_Code");
                    }
                    else
                    {
                        productCode = gpcm.Product_Code;

                        var query = db.Econ_D_YYYYMMDD
                                    .Where(x => x.Year_Quartly == yearQuartly);
                        db.Econ_D_YYYYMMDD.RemoveRange(query);

                        db.Econ_D_YYYYMMDD.AddRange(
                            A07Data.Select(x => new Econ_D_YYYYMMDD()
                            {
                                Processing_Date = DateTime.Now.ToString("yyyy/MM/dd"),
                                Product_Code = productCode,
                                Data_ID = "",
                                Year_Quartly = x.Year_Quartly,
                                PD_Quartly = A06Data[0].PD_Quartly,
                                var1 = Extension.doubleNToDouble(x.TWSE_Index),
                                var2 = Extension.doubleNToDouble(x.TWRGSARP_Index),
                                var3 = Extension.doubleNToDouble(x.TWGDPCON_Index),
                                var4 = Extension.doubleNToDouble(x.TWLFADJ_Index),
                                var5 = Extension.doubleNToDouble(x.TWCPI_Index),
                                var6 = Extension.doubleNToDouble(x.TWMSA1A_Index),
                                var7 = Extension.doubleNToDouble(x.TWMSA1B_Index),
                                var8 = Extension.doubleNToDouble(x.TWMSAM2_Index),
                                var9 = Extension.doubleNToDouble(x.GVTW10YR_Index),
                                var10 = Extension.doubleNToDouble(x.TWTRBAL_Index),
                                var11 = Extension.doubleNToDouble(x.TWTREXP_Index),
                                var12 = Extension.doubleNToDouble(x.TWTRIMP_Index),
                                var13 = Extension.doubleNToDouble(x.TAREDSCD_Index),
                                var14 = Extension.doubleNToDouble(x.TWCILI_Index),
                                var15 = Extension.doubleNToDouble(x.TWBOPCUR_Index),
                                var16 = Extension.doubleNToDouble(x.EHCATW_Index),
                                var17 = Extension.doubleNToDouble(x.TWINDPI_Index),
                                var18 = Extension.doubleNToDouble(x.TWWPI_Index),
                                var19 = Extension.doubleNToDouble(x.TARSYOY_Index),
                                var20 = Extension.doubleNToDouble(x.TWEOTTL_Index),
                                var21 = Extension.doubleNToDouble(x.SLDETIGT_Index),
                                var22 = Extension.doubleNToDouble(x.TWIRFE_Index),
                                var23 = Extension.doubleNToDouble(x.SINYI_HOUSE_PRICE_index),
                                var24 = Extension.doubleNToDouble(x.CATHAY_ESTATE_index),
                                var25 = Extension.doubleNToDouble(x.Real_GDP2011),
                                var26 = Extension.doubleNToDouble(x.MCCCTW_Index),
                                var27 = Extension.doubleNToDouble(x.TRDR1T_Index)
                            })
                        );

                        db.SaveChanges();

                        log.txtLog(
                           TableType.C03Mortgage.ToString(),
                           true,
                           startTime,
                           logPath,
                           MessageType.Success.GetDescription());
                    }
                }
            }
            catch (Exception ex)
            {
                log.txtLog(
                   TableType.C03Mortgage.ToString(),
                   false,
                   startTime,
                   logPath,
                   ex.Message);
            }
        }
        #endregion

        #region private function
        /// <summary>
        /// 判斷國內外
        /// </summary>
        /// <param name="ratingOrg"></param>
        /// <returns></returns>
        private string formatOrgArea(string ratingOrg)
        {
            if (ratingOrg.Equals(RatingOrg.SP.GetDescription()) ||
               ratingOrg.Equals(RatingOrg.Moody.GetDescription()) ||
               ratingOrg.Equals(RatingOrg.Fitch.GetDescription()))
                return "國外";
            if (ratingOrg.Equals(RatingOrg.FitchTwn.GetDescription()) ||
                ratingOrg.Equals(RatingOrg.CW.GetDescription()))
                return "國內";
            return string.Empty;
        }

        /// <summary>
        /// get Security_Ticker
        /// </summary>
        /// <param name="SMF"></param>
        /// <param name="bondNumber"></param>
        /// <returns></returns>
        private string getSecurityTicker(string SMF, string bondNumber)
        {
            List<string> Mtges = new List<string>() { "A11", "932" };
            if (!SMF.IsNullOrWhiteSpace() && SMF.Trim().Length > 3)
                if (Mtges.Contains(SMF.Substring(0, 3)))
                    return string.Format("{0} {1}", bondNumber, "Mtge");
            return string.Format("{0} {1}", bondNumber, "Corp");
        }

        /// <summary>
        /// get ParmID
        /// </summary>
        /// <returns></returns>
        private string getParmID()
        {
            string parmID = string.Empty;
            DateTime dt = DateTime.Now;
            var parms = db.Bond_Rating_Parm
            .Where(j => j.Processing_Date != null &&
                   dt > j.Processing_Date);
            Bond_Rating_Parm parmf =
            parms.FirstOrDefault(q => q.Processing_Date == parms.Max(w => w.Processing_Date));
            if (parmf != null) // 參數編號
                parmID = parmf.Parm_ID;
            return parmID;
        }

        /// <summary>
        /// 抓取 sampleInfo
        /// </summary>
        /// <param name="sampleInfos"></param>
        /// <param name="info"></param>
        /// <param name="nullarr"></param>
        /// <returns></returns>
        private sampleInfo formateSampleInfo(
            List<A53.sampleInfo> sampleInfos,
            Rating_Info info,
            List<string> nullarr)
        {
            if (RatingObject.Bonds.GetDescription().Equals(info.Rating_Object))
            {
                sampleInfo s = sampleInfos.FirstOrDefault(j => info.Bond_Number.Equals(j.Bond_Number));
                if (s != null)
                {
                    return new sampleInfo()
                    {
                        Bond_Number = s.Bond_Number,
                        ISSUER_TICKER = sampleInfoValue(s.ISSUER_TICKER, nullarr),
                        GUARANTOR_EQY_TICKER = sampleInfoValue(s.GUARANTOR_EQY_TICKER, nullarr),
                        GUARANTOR_NAME = sampleInfoValue(s.GUARANTOR_NAME, nullarr)
                    };
                }
            }
            return new sampleInfo();
        }

        /// <summary>
        /// 判斷sampleInfo 參數
        /// </summary>
        /// <param name="value"></param>
        /// <param name="nullarr"></param>
        /// <returns></returns>
        private string sampleInfoValue(string value, List<string> nullarr)
        {
            if (value == null)
                return null;
            if (nullarr.Contains(value.Trim()))
                return null;
            return value.Trim();
        }
        #endregion


    }
}