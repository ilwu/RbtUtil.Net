using rbt.Extension;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;

namespace rbt.util.excel.bean.expt
{
    /// <summary>
    ///     匯出的資料
    /// </summary>
    public class ExportDataSet
    {
        /// <summary>
        ///     建構子
        /// </summary>
        public ExportDataSet()
        {
            ContextDataMap = new Dictionary<string, Dictionary<string, Object>>();
            DetailDataMap = new Dictionary<string, IList<ColumnDataSet>>();
        }

        // =====================================================
        // 資料儲存區
        // =====================================================
        /// <summary>
        ///     context data
        /// </summary>
        public Dictionary<string, Dictionary<string, Object>> ContextDataMap { get; set; }

        /// <summary>
        ///     detail Data
        /// </summary>
        public Dictionary<string, IList<ColumnDataSet>> DetailDataMap { get; set; }

        // =====================================================
        // contextDataMap
        // =====================================================
        /// <summary>
        ///     放入 context data
        /// </summary>
        /// <param name="dataId">dataId , 對應 xml 設定</param>
        /// <param name="dataMap">context data map </param>
        public void setContext(string dataId, Dictionary<string, Object> dataMap)
        {
            ContextDataMap.Add(dataId, dataMap);
        }

        /// <summary>
        ///     放入 context data
        /// </summary>
        /// <param name="dataId">dataId , 對應 xml 設定</param>
        /// <param name="dataMap">context data map </param>
        public void setContext(string dataId, Hashtable dataMap)
        {
            ContextDataMap.Add(dataId, map2Dic(dataMap));
        }

        /// <summary>
        ///     取得 context data
        /// </summary>
        /// <param name="dataId"></param>
        /// <returns></returns>
        public Dictionary<string, Object> getContext(string dataId)
        {
            if (ContextDataMap.ContainsKey(dataId))
            {
                return ContextDataMap[dataId];
            }
            throw new Exception("getContext : 索引 [" + dataId + "]未設定");
        }

        // =====================================================
        // detailDataMap
        // =====================================================
        /// <summary>
        ///     放入 ColumnDataSet List
        /// </summary>
        /// <param name="dataId"></param>
        /// <param name="detailDataSetList"></param>
        public void setDetailDataSet(string dataId, IList<ColumnDataSet> detailDataSetList)
        {
            DetailDataMap.Add(dataId, detailDataSetList);
        }

        /// <summary>
        ///     無子欄位特殊設定時, 可直接將 List＜Map＜String,Object＞＞ 型態資料放入, 會自動轉為 List[ColumnDataSet]
        /// </summary>
        /// <param name="dataId"></param>
        /// <param name="dataMapList"></param>
        public void setDetail(string dataId, IList<Hashtable> dataMapList)
        {
            var newList = new List<Dictionary<string, object>>();
            foreach (Hashtable item in dataMapList)
            {
                newList.Add(map2Dic(item));
            }

            setDetail(dataId, newList);
        }

        public void setDetail<T>(string dataId, IList<T> modelList)
        {
            setDetail(dataId, modelList.ToMapList());
        }

        //IList<Hashtable>

        public void setDetail(string dataId, List<Dictionary<string, object>> dataMapList)
        {
            var detailDataSetList = new List<ColumnDataSet>();

            foreach (var columnData in dataMapList)
            {
                var columnDataSet = new ColumnDataSet { ColumnDataMap = columnData };

                foreach (var item in columnData)
                {
                    var value = item.Value;

                    if (value != null)
                    {
                        if (value.GetType() == typeof(List<Hashtable>))
                        {
                            columnDataSet.setArray(item.Key, mapList2DicList((List<Hashtable>)value));
                        }

                        if (value.GetType() == typeof(List<Dictionary<string, object>>))
                        {
                            columnDataSet.setArray(
                                item.Key,
                                (List<Dictionary<string, object>>)value
                                );
                        }
                    }
                }

                detailDataSetList.Add(columnDataSet);
            }
            DetailDataMap.Add(dataId, detailDataSetList);
        }

        /// <summary>
        ///     無子欄位特殊設定時, 可直接將 ListMap＜String,Object＞＞ 型態資料放入, 會自動轉為 List[ColumnDataSet]
        /// </summary>
        /// <param name="dataId"></param>
        /// <param name="values"></param>
        public void setDetail(string dataId, Collection<Dictionary<string, object>> values)
        {
            var detailDataSetList = new List<ColumnDataSet>();

            foreach (var columnData in values)
            {
                var columnDataSet = new ColumnDataSet { ColumnDataMap = columnData };
                detailDataSetList.Add(columnDataSet);
            }
            DetailDataMap.Add(dataId, detailDataSetList);
        }

        /// <summary>
        ///     取得 ColumnDataSet List
        /// </summary>
        /// <param name="dataId"></param>
        /// <returns></returns>
        public IList<ColumnDataSet> getDetail(string dataId)
        {
            if (DetailDataMap.ContainsKey(dataId))
            {
                return DetailDataMap[dataId];
            }
            throw new Exception("getDetail : 索引 [" + dataId + "]未設定");
        }

        /// <summary>
        /// </summary>
        /// <param name="dataId"></param>
        /// <param name="dataTable"></param>
        public void setDetailDataTable(string dataId, DataTable dataTable)
        {
            var detailDataSetList = new List<ColumnDataSet>();

            foreach (DataRow row in dataTable.Rows)
            {
                var columnDataSet = new ColumnDataSet();
                foreach (DataColumn column in dataTable.Columns)
                {
                    columnDataSet.setColumn(column.ColumnName, row[column.ColumnName]);
                }

                detailDataSetList.Add(columnDataSet);
            }
            DetailDataMap.Add(dataId, detailDataSetList);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="dataMap"></param>
        /// <returns></returns>
        private static Dictionary<string, object> map2Dic(Hashtable dataMap)
        {
            var dataDic = new Dictionary<string, Object>();

            foreach (DictionaryEntry item in dataMap)
            {
                dataDic.Add(item.Key.ToString(), item.Value);
            }

            return dataDic;
        }

        public static List<Dictionary<string, Object>> mapList2DicList(IList<Hashtable> dataMapList)
        {
            var dicList = new List<Dictionary<string, Object>>();

            foreach (var item in dataMapList)
            {
                dicList.Add(map2Dic(item));
            }
            return dicList;
        }
    }
}