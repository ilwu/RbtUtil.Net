using CSharpJExcel.Jxl.Format;
using Newtonsoft.Json;
using rbt.util.excel;
using rbt.util.excel.bean.expt;
using rbt.util.excel.bean.expt.config;
using rbt.util.excel.config;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace rbt.Excel.Test
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            MyTest();
        }

        public void MyTest()
        {
            Type myType = typeof(Colour);
            PropertyInfo myPropInfo = myType.GetProperty("AQUA");
        }

        protected object BaseUtil_GetObjectFromTemp(string fileName, Type type)
        {
            //讀取 Server 中的 Json檔案
            if (!System.IO.File.Exists(fileName))
            {
                return null;
            }
            //讀取
            StringBuilder jsonText = new StringBuilder();
            using (System.IO.StreamReader sr = System.IO.File.OpenText(fileName))
            {
                string input = null;
                input = sr.ReadLine();
                jsonText.Append(input);
                while ((input != null))
                {
                    input = sr.ReadLine();
                    jsonText.Append(input);
                }
                sr.Close();
            }

            return JsonConvert.DeserializeObject(jsonText.ToString(), type);
        }

        private void exportFile_Load(object sender, EventArgs e)
        {
            txt_configFile.Text = @"q:/Tesa/Reports/config/TESD104R.xml";
            txt_configID.Text = @"2";
            txt_exportPath.Text = @"c:/logs/";
        }

        private void process_Click(object sender, EventArgs e)
        {
            var excelExporter = new ExcelExporter();
            FileStream fileStream = null;
            try
            {
                console.Text = @"開始執行";
                //
                console.Text += "\r\n讀取設定檔";
                ExportConfigInfo exportConfigInfo = (
                    new ExportConfigReader()).read(txt_configFile.Text, txt_configID.Text);

                DataTable newDataTable = (DataTable)BaseUtil_GetObjectFromTemp("c:\\Logs\\6.txt", new DataTable().GetType());

                //=================================================
                //準備匯出資料的容器
                //=================================================

                ExportDataSet exportDataSet = new ExportDataSet();

                //=================================================
                //兜組單筆型態欄位資料 (dataId = header)
                //=================================================
                Dictionary<string, object> headerData = new Dictionary<string, object>();

                //放入資料容器 (設定檔 context id 為  title)
                exportDataSet.setContext("title", headerData);

                //=================================================
                //兜組多筆型態欄位資料 (dataId = detail)
                //=================================================
                exportDataSet.setDetailDataTable("detail", newDataTable);

                console.Text += "\r\n輸出";
                fileStream = new FileStream(txt_exportPath.Text + exportConfigInfo.FileName, FileMode.Create);
                excelExporter.export(exportConfigInfo, perpareTestData(), fileStream);

                console.Text += "\r\n執行完成";
            }
            catch (Exception ex)
            {
                console.Text += "\r\n發生錯誤";
                console.Text += ex.StackTrace;
            }
            finally
            {
                if (fileStream != null)
                {
                    fileStream.Dispose();
                }
            }
        }

        // ===========================================================================
        // 參數預設值區
        // ===========================================================================
        /// <summary>
        ///     產生測試用的資料
        ///     @return
        /// </summary>
        public virtual ExportDataSet perpareTestData()
        {
            // ExportDataSet
            var exportDataSet = new ExportDataSet();
            // Detail DataSet List
            var detailDataSetList = new List<ColumnDataSet>();
            exportDataSet.setDetailDataSet("detail", detailDataSetList);

            // ==============================================
            // 外框資料
            // ==============================================
            string[] schoolNames = { "莊敬高職", "亞東", "東南", "城市科大", "致理", "耕莘", "康寧" };
            foreach (string schoolName in schoolNames)
            {
                var columnData = new Dictionary<string, object>();
                columnData["SCHOOL_CHN_NAME"] = schoolName;
                columnData["EMPLOYMENT_RATE"] = "100.00%";
                detailDataSetList.Add(perpareTesDataDetail(columnData));
            }
            return exportDataSet;
        }

        private ColumnDataSet perpareTesDataDetail(Dictionary<string, object> columnData)
        {
            // ColumnDataSet
            var columnDataSet = new ColumnDataSet();
            columnDataSet.ColumnDataMap = columnData;

            // ==============================================
            // 年度資料
            // ==============================================
            var yearDataList = new List<Dictionary<string, object>>();

            var yearDataMap98 = new Dictionary<string, object>();
            yearDataList.Add(yearDataMap98);
            yearDataMap98["YEAR"] = 98;
            yearDataMap98["1"] = 4;
            yearDataMap98["2"] = 1;
            yearDataMap98["3-1"] = 6;
            yearDataMap98["3-2"] = 0;
            yearDataMap98["3-3"] = 12;
            yearDataMap98["3-4"] = 0;
            yearDataMap98["3-5"] = 0;
            yearDataMap98["4"] = 1;
            yearDataMap98["5"] = 0;
            yearDataMap98["6"] = 0;
            yearDataMap98["7-1"] = 0;
            yearDataMap98["7-2"] = 0;
            yearDataMap98["7-3"] = 0;
            yearDataMap98["8"] = 24;

            var yearDataMap99 = new Dictionary<string, object>();
            yearDataList.Add(yearDataMap99);
            yearDataMap99["YEAR"] = 99;
            yearDataMap99["1"] = 5;
            yearDataMap99["2"] = 0;
            yearDataMap99["3-1"] = 0;
            yearDataMap99["3-2"] = 6;
            yearDataMap99["3-3"] = 0;
            yearDataMap99["3-4"] = 12;
            yearDataMap99["3-5"] = 0;
            yearDataMap99["4"] = 0;
            yearDataMap99["5"] = 0;
            yearDataMap99["6"] = 1;
            yearDataMap99["7-1"] = 0;
            yearDataMap99["7-2"] = 0;
            yearDataMap99["7-3"] = 0;
            yearDataMap99["8"] = 24;

            columnDataSet.setArray("yearDataList", yearDataList);

            // ==============================================
            // 小計
            // ==============================================
            var singleData = new Dictionary<string, object>();
            singleData["1"] = 9;
            singleData["2"] = 1;
            singleData["3"] = 36;
            singleData["4"] = 1;
            singleData["5"] = 0;
            singleData["6"] = 1;
            singleData["7"] = 0;
            singleData["8"] = 48;
            columnDataSet.setSingle("countMap", singleData);

            return columnDataSet;
        }
    }
}