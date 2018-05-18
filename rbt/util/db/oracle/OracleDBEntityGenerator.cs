using rbt.Extension;
using System;
using System.Collections.Generic;
using System.Text;

namespace rbt.util.db.oracle
{
    public class OracleDBEntityGenerator
    {
        public void proc(
            string OUTPUT_PTAH,
            string schema,
            string classNamespace,
            string classPrefix,
            string nameMapClassName,
            OracleDBUtil dbUtil,
            string testModelTable = ""
            )
        {
            var tb = new OracleTableInfo();

            var fileUtil = new FileUtil();

            Console.WriteLine("讀取 table 資訊");
            var tableInfoList = tb.GetAllTable(dbUtil, schema, testModelTable);
            Console.WriteLine("讀取完成！: " + tableInfoList.Count);

            if (testModelTable.NotEmpty() && tableInfoList.Count == 0)
            {
                throw new Exception("找不到指定的 Table");
            }

            foreach (KeyValuePair<string, OracleTableInfo> table in tableInfoList)
            {
                //=======================================
                //取得 table 資料
                //=======================================
                //table 代號
                var tableName = table.Key;
                //OracleTableInfo
                var tableInfo = table.Value;
                //不存在column資料時跳過
                if (tableInfo.ColumnDataMapList.Count < 1)
                {
                    continue;
                }

                //table 註解
                var tableComments = prepareStr(replaceBreakLine(tableInfo.ColumnDataMapList[0]["TABLE_COMMENTS"] + "")).Replace(System.Environment.NewLine, "");

                //class Name =
                var className = classPrefix + StringUtil.Convert2CamelCase(tableName) + "";

                //=======================================
                //兜組 base class 代碼
                //=======================================
                //TBMODEL
                var tbModelContent = new StringBuilder();
                tbModelContent.AppendLine("using System;");
                tbModelContent.AppendLine("using System.ComponentModel;");
                tbModelContent.AppendLine("using System.ComponentModel.DataAnnotations;");
                tbModelContent.AppendLine("using System.ComponentModel.DataAnnotations.Schema;");
                tbModelContent.AppendLine("using rbt.util.db.model;");
                tbModelContent.AppendLine("using rbt.DataAnnotations.Validation;");

                tbModelContent.AppendLine("");
                tbModelContent.AppendLine("namespace " + classNamespace);
                tbModelContent.AppendLine("{");
                tbModelContent.AppendLine("    /// <summary>");
                tbModelContent.AppendLine("    /// [" + tableName + " " + tableComments + "] table model");
                tbModelContent.AppendLine("    /// <summary>");
                tbModelContent.AppendLine("    [Table(\"" + tableName + "\")]");
                tbModelContent.AppendLine("    [DisplayName(\"" + tableComments + "\")]");
                tbModelContent.AppendLine("    public class " + className + " : BaseDBEntity");
                tbModelContent.AppendLine("    {");

                //validator
                var validatorContent = new StringBuilder();
                validatorContent.AppendLine("using System;");
                validatorContent.AppendLine("using System.ComponentModel;");
                validatorContent.AppendLine("using System.ComponentModel.DataAnnotations;");
                validatorContent.AppendLine("");
                validatorContent.AppendLine("namespace " + classNamespace + ".validator");
                validatorContent.AppendLine("{");
                validatorContent.AppendLine("    /// <summary>");
                validatorContent.AppendLine("    /// [" + tableName + " " + tableComments + "] table model validator");
                validatorContent.AppendLine("    /// <summary>");
                validatorContent.AppendLine("    [DisplayName(\"" + tableComments + " 欄位驗證\")]");
                validatorContent.AppendLine("    public class " + className + "Validator");
                validatorContent.AppendLine("    {");

                var columnInfoContent = new StringBuilder();
                foreach (Dictionary<string, object> item in tableInfo.ColumnDataMapList)
                {
                    //"COLUMN_NAME"
                    string columnName = prepareStr(StringUtil.SafeTrim(item["COLUMN_NAME"]));
                    //"COMMENTS"
                    string orgComments = replaceBreakLine(StringUtil.SafeTrim(item["COMMENTS"]));
                    string comments = prepareStr(orgComments);
                    columnInfoContent.AppendLine("        //" + columnName + "\t" + comments);
                }

                tbModelContent.AppendLine();
                tbModelContent.AppendLine(columnInfoContent.ToString());
                tbModelContent.AppendLine();

                foreach (Dictionary<string, object> item in tableInfo.ColumnDataMapList)
                {
                    //=====================================================
                    // 取得欄位參數
                    //=====================================================
                    //"COLUMN_NAME"
                    string columnName = prepareStr(StringUtil.SafeTrim(item["COLUMN_NAME"]));
                    //"DATA_TYPE"
                    string dataType = StringUtil.SafeTrim(item["DATA_TYPE"]);
                    //"COMMENTS"
                    string orgComments = replaceBreakLine(StringUtil.SafeTrim(item["COMMENTS"]));
                    string comments = prepareStr(orgComments);
                    //DATA_LENGTH
                    int dataLenght = int.Parse(StringUtil.SafeTrim(item["DATA_LENGTH"]));
                    if ("NUMBER".Equals(dataType) && StringUtil.NotEmpty(item["DATA_PRECISION"]))
                    {
                        dataLenght = int.Parse(StringUtil.SafeTrim(item["DATA_PRECISION"]));
                    }

                    //"DATA_SCALE"
                    int dataScale = int.Parse(StringUtil.SafeTrim(item["DATA_SCALE"], "0"));
                    //額外訊息
                    var ExtInfoList = this.parseExtInfo(orgComments);

                    //=====================================================
                    // TBModel
                    //=====================================================
                    tbModelContent.AppendLine("        /// <summary>");
                    tbModelContent.AppendLine("        /// " + columnName + " " + orgComments);
                    tbModelContent.AppendLine("        /// <summary>");
                    //DisplayName
                    tbModelContent.AppendLine("        [DisplayName(\"" + (StringUtil.IsEmpty(comments) ? "未設定" : comments) + "\")]");
                    //NULLABLE
                    //if (!"Y".Equals(StringUtil.SafeTrim(item["NULLABLE"])))
                    //{
                    //    content.AppendLine("        [Required]");
                    //}
                    //StringLength
                    tbModelContent.Append(GetLengthAttr(dataType, StringUtil.SafeTrim(item["CHAR_LENGTH"]), comments));
                    //NumberValidtion
                    tbModelContent.Append(GetNumberValidate(dataType, StringUtil.SafeTrim(item["DATA_PRECISION"]), StringUtil.SafeTrim(item["DATA_SCALE"]), comments));
                    //other Attribute
                    tbModelContent.Append(prepareAttribute(orgComments));
                    //[Key]
                    if (tableInfo.PKeySet.Contains(columnName))
                    {
                        tbModelContent.AppendLine("        [Key]");
                    }

                    tbModelContent.AppendLine("        public " + getType(dataType, dataLenght, dataScale, tableName + "." + columnName) + " " + columnName);
                    tbModelContent.AppendLine("        {");
                    tbModelContent.AppendLine("            get { return _" + columnName + "; }");
                    tbModelContent.AppendLine("            set { if (!this.GetModeifyField().Contains(\"" + columnName + "\")) this.GetModeifyField().Add(\"" + columnName + "\"); _" + columnName + " = value; }");
                    tbModelContent.AppendLine("        }");
                    tbModelContent.AppendLine("        private " + getType(dataType, dataLenght, dataScale, tableName + "." + columnName) + " _" + columnName + " { get; set; }");
                    tbModelContent.AppendLine("");

                    //=====================================================
                    // TBModel validator
                    //=====================================================
                    validatorContent.AppendLine("        /// <summary>");
                    validatorContent.AppendLine("        /// " + columnName + " " + orgComments);
                    validatorContent.AppendLine("        /// <summary>");
                    //[Required]
                    validatorContent.AppendLine("        [Required]");
                    validatorContent.AppendLine("        public " + getType(dataType, dataLenght, dataScale, tableName + "." + columnName) + " " + columnName + " { get; set; }");
                    validatorContent.AppendLine("");

                    //處理日期轉換欄位
                    if ((("varchar2".Equals(dataType) && dataLenght == 7) || ("nvarchar2".Equals(dataType) && dataLenght == 14))
                         && (columnName.ToLower().IndexOf("date") >= 0 || columnName.ToLower().IndexOf("day") >= 0))
                    {
                        validatorContent.AppendLine("        /// <summary>");
                        validatorContent.AppendLine("        /// " + columnName + " " + comments + ": 西元日期轉換擴充欄位");
                        validatorContent.AppendLine("        /// </summary>");
                        validatorContent.AppendLine("        [Required]");
                        validatorContent.AppendLine("        public string " + columnName + "_AD { get; set; }");
                        validatorContent.AppendLine("");
                    }
                }

                tbModelContent.AppendLine("        /// <summary>");
                tbModelContent.AppendLine("        /// 回傳 Table 名稱");
                tbModelContent.AppendLine("        /// <summary>");
                tbModelContent.AppendLine("        public override string GetTableName()");
                tbModelContent.AppendLine("        {");
                tbModelContent.AppendLine("            return \"" + tableName + "\";");
                tbModelContent.AppendLine("        }");
                tbModelContent.AppendLine("        ");

                tbModelContent.AppendLine("    }");
                //content.AppendLine("}");
                tbModelContent.AppendLine("");

                //=======================================
                //擴充 model
                //=======================================
                tbModelContent.AppendLine("    /// <summary>");
                tbModelContent.AppendLine("    /// [" + tableName + " " + tableComments + "] 擴充 model");
                tbModelContent.AppendLine("    /// </summary>");
                tbModelContent.AppendLine("    public class " + className + "Ext : " + className);
                tbModelContent.AppendLine("    {");

                foreach (Dictionary<string, object> item in tableInfo.ColumnDataMapList)
                {
                    //"COLUMN_NAME"
                    string columnName = prepareStr(StringUtil.SafeTrim(item["COLUMN_NAME"]));
                    //"DATA_TYPE"
                    string dataType = StringUtil.SafeTrim(item["DATA_TYPE"]).ToLower();
                    //"COMMENTS"
                    string orgComments = replaceBreakLine(StringUtil.SafeTrim(item["COMMENTS"]));
                    string comments = prepareStr(orgComments);
                    //額外訊息
                    var ExtInfoList = this.parseExtInfo(orgComments);
                    //DATA_LENGTH
                    int dataLenght = int.Parse(StringUtil.SafeTrim(item["DATA_LENGTH"]));

                    //處理日期轉換欄位
                    if ((("varchar2".Equals(dataType) && dataLenght == 7) || ("nvarchar2".Equals(dataType) && dataLenght == 14))
                         && (columnName.ToLower().IndexOf("date") >= 0 || columnName.ToLower().IndexOf("day") >= 0))
                    {
                        tbModelContent.AppendLine("        /// <summary>");
                        tbModelContent.AppendLine("        /// " + columnName + " " + comments + ": 西元日期轉換擴充欄位");
                        tbModelContent.AppendLine("        /// </summary>");
                        //DisplayName
                        tbModelContent.AppendLine("        [DisplayName(\"" + (StringUtil.IsEmpty(comments) ? "未設定" : comments) + "\")]");
                        tbModelContent.AppendLine("        public string " + columnName + "_AD ");
                        tbModelContent.AppendLine("        {");
                        tbModelContent.AppendLine("            get { return HelperUtil.DateTimeToString(TesnUtil.TransTwToDateTime(this." + columnName + ", \"\")); }");
                        tbModelContent.AppendLine("            set { this." + columnName + " = HelperUtil.DateTimeToTwString(HelperUtil.TransToDateTime(value), \"\"); }");
                        tbModelContent.AppendLine("        }");
                        tbModelContent.AppendLine("");
                    }

                    //電話
                    if (ExtInfoList.IndexOf("PHONE") >= 0)
                    {
                        tbModelContent.AppendLine("        /// <summary>");
                        tbModelContent.AppendLine("        /// " + columnName + " " + comments + ": 電話號碼 擴充資料欄位");
                        tbModelContent.AppendLine("        /// </summary>");
                        tbModelContent.AppendLine("        public PhoneNumberModel " + columnName + "_PHONE ");
                        tbModelContent.AppendLine("        {");
                        tbModelContent.AppendLine("            get { return PhoneNumberModel.Parse(this." + columnName + "); }");
                        tbModelContent.AppendLine("            set { this." + columnName + " = value.ToString(); }");
                        tbModelContent.AppendLine("        }");
                        tbModelContent.AppendLine("");
                    }

                    //手機
                    if (ExtInfoList.IndexOf("MOBILE") >= 0)
                    {
                        tbModelContent.AppendLine("        /// <summary>");
                        tbModelContent.AppendLine("        /// " + columnName + " " + comments + ": 手機號碼 擴充資料欄位");
                        tbModelContent.AppendLine("        /// </summary>");
                        tbModelContent.AppendLine("        public MobileNumberModel " + columnName + "_MOBILE ");
                        tbModelContent.AppendLine("        {");
                        tbModelContent.AppendLine("            get { return MobileNumberModel.Parse(this." + columnName + "); }");
                        tbModelContent.AppendLine("            set { this." + columnName + " = value.ToString(); }");
                        tbModelContent.AppendLine("        }");
                        tbModelContent.AppendLine("");
                    }

                    //增加顯示存放欄位
                    if (ExtInfoList.IndexOf("SHOW_TEXT") >= 0)
                    {
                        tbModelContent.AppendLine("        /// <summary>");
                        tbModelContent.AppendLine("        /// " + columnName + " " + comments + ": 轉文字擴充欄位");
                        tbModelContent.AppendLine("        /// </summary>");
                        tbModelContent.AppendLine("        public string " + columnName + "_SHOW_TEXT { get; set; }");
                        tbModelContent.AppendLine("");
                    }

                    //CHECKBOX
                    foreach (string extInfo in ExtInfoList)
                    {
                        var currExtInfo = (extInfo + "").Trim();

                        if (currExtInfo.IndexOf("CHECKBOX:") >= 0)
                        {
                            var currParams = (currExtInfo + "").Trim().Split(':');
                            if (currParams.Length < 2)
                            {
                                throw new Exception(columnName + "欄位的 【CHECKBOX:】設定錯誤");
                            }

                            tbModelContent.AppendLine("        /// <summary>");
                            tbModelContent.AppendLine("        /// " + columnName + " " + comments + ": CHECKBOX 擴充資料欄位");
                            tbModelContent.AppendLine("        /// </summary>");
                            tbModelContent.AppendLine("        public CheckBoxModel " + columnName + "_CHECKBOX ");
                            tbModelContent.AppendLine("        {");
                            tbModelContent.AppendLine("            get { return new CheckBoxModel(base." + columnName + ", \"" + currParams[1] + "\"); }");
                            tbModelContent.AppendLine("            set { this." + columnName + " = value.Value; }");
                            tbModelContent.AppendLine("        }");
                            tbModelContent.AppendLine("");
                        }
                    }

                    //CHECKBOX
                    foreach (string extInfo in ExtInfoList)
                    {
                        var currExtInfo = (extInfo + "").Trim();

                        if (currExtInfo.StartsWith("ENUM:"))
                        {
                            currExtInfo = currExtInfo.Substring(5);

                            var currParams = (currExtInfo + "").Trim().SplitToList(",");

                            tbModelContent.AppendLine("        /// <summary>");
                            tbModelContent.AppendLine("        /// " + columnName + " " + comments + ": 參數值擴充欄位");
                            tbModelContent.AppendLine("        /// </summary>");
                            tbModelContent.AppendLine("        public enum " + columnName + "_ENUM ");
                            tbModelContent.AppendLine("        {");

                            for (int paramIndex = 0; paramIndex < currParams.Count; paramIndex++)
                            {
                                var parma = currParams[paramIndex];
                                //空白時不處理
                                if (string.IsNullOrEmpty(parma)) continue;
                                //分割代號與說明
                                var paramCols = (parma + "").Trim().SplitToList("-");
                                if (paramCols.Count < 2)
                                {
                                    throw new Exception(columnName + "欄位的 【ENUM:】設定錯誤  ex: 【ENUM:A-依選手(個人/組),B-依評審項目】");
                                }

                                for (int colIndex = 0; colIndex < paramCols.Count; colIndex++)
                                {
                                    paramCols[colIndex] = paramCols[colIndex].Trim();
                                    if (paramCols[colIndex].Length == 0)
                                    {
                                        throw new Exception(columnName + "欄位的 【ENUM:】設定錯誤, 項目為空");
                                    }
                                }

                                tbModelContent.AppendLine("            /// <summary>");
                                tbModelContent.AppendLine("            /// " + paramCols[1]);
                                tbModelContent.AppendLine("            /// </summary>");
                                tbModelContent.AppendLine("            " + paramCols[0] + " = " + paramIndex + ((paramIndex < currParams.Count - 1) ? "," : ""));
                                if (paramIndex < currParams.Count - 1)
                                {
                                    tbModelContent.AppendLine("");
                                }
                            }

                            tbModelContent.AppendLine("        }");
                            tbModelContent.AppendLine("");
                        }
                    }
                }
                tbModelContent.AppendLine("    }");
                tbModelContent.AppendLine("}");
                tbModelContent.AppendLine("");

                validatorContent.AppendLine("    }");
                validatorContent.AppendLine("}");
                validatorContent.AppendLine("");

                //輸出 TbModel
                fileUtil.writeToFile(tbModelContent.ToString(), OUTPUT_PTAH + "/Models/Entities/", className + ".cs");

                //輸出 Validator
                fileUtil.writeToFile(validatorContent.ToString(), OUTPUT_PTAH + "/Models/Entities/validator/", className + "Validator.cs");

                //Console.WriteLine(OUTPUT_PTAH + "/" + className + ".cs");

                //StaticCodeMap.TableName
            }
        }

        private string GetLengthAttr(string dataType, string charLength, string comments)
        {
            if ("CHAR".Equals(dataType) ||
                "VARCHAR".Equals(dataType) || "VARCHAR2".Equals(dataType) ||
                "NVARCHAR".Equals(dataType) || "NVARCHAR2".Equals(dataType) ||
                "TEXT".Equals(dataType) || "CLOB".Equals(dataType))
            {
                //return "        [StringLength(" + charLength + ", ErrorMessage = \"" + comments + "最多" + charLength + "個字\")]\r\n";
                return "        [StringLength(" + charLength + ", ErrorMessage = \"{0}最多{1}個字\")]\r\n";
            }
            return "";
        }

        private string GetNumberValidate(string dataType, string dataPrecision, string dataScale, string comments)
        {
            dataType = dataType.ToUpper();
            if ("INT".Equals(dataType) || "INTEGER".Equals(dataType) || "TINYINT".Equals(dataType)
                || "NUMBER".Equals(dataType)
                || "DECIMAL".Equals(dataType) || "NUMBER".Equals(dataType) || "NUMERIC".Equals(dataType))
            {
                return "        [NumberValidtion("
                    + StringUtil.SafeTrim(dataPrecision, "0") + ", "
                    + StringUtil.SafeTrim(dataScale, "0") +
                    ")]\r\n"; ;
            }
            return "";
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="dataType"></param>
        /// <param name="dataLength"></param>
        /// <param name="dataPrecision"></param>
        /// <param name="charLength"></param>
        /// <returns></returns>
        private string getType(string dataType, int dataLenght, int dataScale, string desc)
        {
            dataType = dataType.ToUpper();
            if ("INT".Equals(dataType) || "INTEGER".Equals(dataType) || "TINYINT".Equals(dataType))
            {
                return "int?";
            }

            if ("NUMBER".Equals(dataType))
            {
                if (dataScale == 0)
                {
                    if (dataLenght <= 4)
                    {
                        return "Int16?";
                    }
                    else if (dataLenght <= 9)
                    {
                        return "Int32?";
                    }
                    else if (dataLenght <= 18)
                    {
                        return "Int64?";
                    }
                    else
                    {
                    }
                }

                //return "Decimal?";
                return "double?";
            }

            if ("DECIMAL".Equals(dataType) || "NUMBER".Equals(dataType) || "NUMERIC".Equals(dataType) || "LONG".Equals(dataType))
            {
                return "Int64?";
            }

            if ("CHAR".Equals(dataType) ||
                "VARCHAR".Equals(dataType) || "VARCHAR2".Equals(dataType) ||
                "NVARCHAR".Equals(dataType) || "NVARCHAR2".Equals(dataType) ||
                "TEXT".Equals(dataType) || "CLOB".Equals(dataType) || "NCLOB".Equals(dataType) ||
                dataType.StartsWith("RAW") || "UROWID".Equals(dataType) || "ROWID".Equals(dataType)
                )
            {
                return "string";
            }

            if ("DATETIME".Equals(dataType) || "SMALLDATETIME".Equals(dataType) || "DATE".Equals(dataType) || dataType.StartsWith("TIMESTAMP"))
            {
                return "DateTime?";
            }

            if ("BLOB".Equals(dataType) || "NBLOB".Equals(dataType))
            {
                return "byte[]";
            }

            throw new Exception("沒有對應型態:[" + dataType + "] (" + desc + ")");
        }

        private string prepareAttribute(string str)
        {
            var resultList = this.parseExtInfo(str);
            var content = new StringBuilder();

            foreach (string extInfo in resultList)
            {
                if ("IDNO".Equals(extInfo))
                {
                    content.AppendLine("        [IDNO]");
                }
                else if ("EMAIL".Equals(extInfo))
                {
                    content.AppendLine("        [EmailAddress]");
                }
                else if ("UNITNO".Equals(extInfo))
                {
                    content.AppendLine("        [UnitNO]");
                }
            }
            return content.ToString();
        }

        private IList<string> parseExtInfo(string str)
        {
            str = StringUtil.SafeTrim(str);
            str = str.Replace(@"\", @"\\");
            str = str.Replace("\"", "\\\"");

            var content = new StringBuilder();
            var temp = "";
            var resultList = new List<string>();
            var flag = false;
            foreach (char chr in str.ToCharArray())
            {
                if (chr.Equals(Char.Parse("【")))
                {
                    temp = "";
                    flag = true;
                    continue;
                }
                else if (chr.Equals(Char.Parse("】")))
                {
                    resultList.Add(temp.ToUpper());
                    flag = true;
                    continue;
                }

                if (flag)
                {
                    temp += chr;
                }
            }

            return resultList;
        }

        private string prepareStr(string str)
        {
            str = StringUtil.SafeTrim(str);
            str = str.Replace(@"\", @"\\");
            str = str.Replace("\"", "\\\"");

            var temp = "";
            var flag = true;
            foreach (char chr in str.ToCharArray())
            {
                if (chr.Equals(Char.Parse("【")))
                {
                    flag = false;
                    continue;
                }
                else if (chr.Equals(Char.Parse("】")))
                {
                    flag = true;
                    continue;
                }

                if (flag)
                {
                    temp += chr;
                }
            }

            return temp.Trim();
        }

        /// <summary>
        /// 移除斷行
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private string replaceBreakLine(string str)
        {
            str += "";
            str = str.Replace("\r", "");
            str = str.Replace("\n", "");
            return str;
        }
    }
}