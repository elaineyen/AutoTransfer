﻿using AutoTransfer.Sample;
using AutoTransfer.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using static AutoTransfer.Enum.Ref;

namespace AutoTransfer.CreateFile
{
    public class CreateSampleFile
    {
        /// <summary>
        /// create Put Sample File
        /// </summary>
        /// <param name="type"></param>
        /// <param name="dateTime"></param>
        public bool create(TableType type, string dateTime)
        {
            bool flag = false;
            try
            {
                List<string> data = new List<string>();

                SetFile f = new SetFile(type, dateTime);

                //ex: sampleA53_20170803
                string getFileName = f.getSampleFileName();

                #region File

                data.Add("START-OF-FILE");

                #region Title

                data.Add($"REPLYFILENAME={getFileName}");
                data.Add("PROGRAMNAME=getdata");
                data.Add("PROGRAMFLAG=oneshot");
                data.Add("FIRMNAME=dl221"); //確認是否提出來?
                data.Add("SECMASTER=YES");
                data.Add("OUTPUTFORMAT=bulklist");
                data.Add("DELIMITER=,");
                data.Add("FUNDAMENTALS=yes");

                #endregion Title

                //空一行
                data.Add(string.Empty);

                #region START-OF-FIELDS

                data.Add("START-OF-FIELDS");
                object obj = null;
                bool findFlag = false;
                if (TableType.A53.ToString().Equals(type.ToString()))
                {
                    obj = new A53Sample();
                    findFlag = true;
                }
                if (findFlag)
                    obj.GetType()
                       .GetProperties()
                       //.OrderBy(x => x.Name)
                       .ToList()
                       .ForEach(x => data.Add(x.Name));
                data.Add("END-OF-FIELDS");

                #endregion START-OF-FIELDS

                //空一行
                data.Add(string.Empty);

                #region START-OF-DATA

                data.Add("START-OF-DATA");
                int year = Int32.Parse(dateTime.Substring(0, 4));
                int month = Int32.Parse(dateTime.Substring(4, 2));
                int day = Int32.Parse(dateTime.Substring(6, 2));
                DateTime date = new DateTime(year, month, day);
                IFRS9Entities db = new IFRS9Entities();
                db.Bond_Account_Info
                    .Where(x => x.Report_Date.HasValue &&
                    x.Report_Date.Value.Equals(date))
                    .Select(x => x.Bond_Number).Distinct()
                    .OrderBy(x => x)
                    .ToList().ForEach(x =>
                    {
                        if (!x.IsNullOrWhiteSpace())
                            data.Add(string.Format("{0}|ISIN", x));
                    });
                db.Dispose();
                data.Add("END-OF-DATA");

                #endregion START-OF-DATA

                data.Add("END-OF-FILE");

                #endregion File

                //ex: ../samplePut 資料夾
                //f.putCommpanyFilePath();
                //ex: sampleA53_20170803
                //f.putSampleFileName();
                //建立 scv 檔案
                flag = new CreatePutFile().create(
                    f.putSampleFilePath(),
                    f.putSampleFileName(),
                    data);
            }
            catch
            {
                flag = false;
            }
            return flag;
        }
    }
}