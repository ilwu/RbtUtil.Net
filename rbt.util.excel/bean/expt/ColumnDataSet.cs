using System;
using System.Collections.Generic;

namespace rbt.util.excel.bean.expt
{
    /// <summary>
    /// </summary>
    public class ColumnDataSet
    {
        /// <summary>
        ///     建構子
        /// </summary>
        public ColumnDataSet()
        {
            ColumnDataMap = new Dictionary<string, object>();
            ArrayDataMap = new Dictionary<string, List<ColumnDataSet>>();
            SingleDataMap = new Dictionary<string, ColumnDataSet>();
        }

        // =====================================================
        // 資料儲存區
        // =====================================================
        /// <summary>
        /// </summary>
        public Dictionary<string, object> ColumnDataMap { get; set; }

        /// <summary>
        /// </summary>
        public Dictionary<string, List<ColumnDataSet>> ArrayDataMap { get; set; }

        /// <summary>
        /// </summary>
        public Dictionary<string, ColumnDataSet> SingleDataMap { get; set; }

        // =====================================================
        // columnDataMap
        // =====================================================

        /// <summary>
        /// 依據傳入的Key ,取得欄位
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object getColumn(string key)
        {
            return ColumnDataMap.ContainsKey(key) ? ColumnDataMap[key] : "";
        }

        /// <summary>
        /// 設定欄位值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="Object"></param>
        public void setColumn(string key, object Object)
        {
            ColumnDataMap.Add(key, Object);
        }

        // =====================================================
        // arrayDataMap
        // =====================================================
        /// <summary>
        ///
        /// </summary>
        /// <param name="dataId"></param>
        /// <returns></returns>
        public List<ColumnDataSet> getArray(string dataId)
        {
            if (ArrayDataMap.ContainsKey(dataId))
            {
                return ArrayDataMap[dataId];
            }
            throw new Exception("getArray : 索引 [" + dataId + "]未設定");
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="dataId"></param>
        /// <param name="arrayDataList"></param>
        public void setArrayColumnDataSet(string dataId, List<ColumnDataSet> arrayDataList)
        {
            ArrayDataMap.Add(dataId, arrayDataList);
        }

        /// <summary>
        ///     無子欄位特殊設定時, 可直接將 List＜Map＜String,Object＞＞ 型態資料放入, 會自動轉為 List＜ColumnDataSet＞
        /// </summary>
        /// <param name="dataId">key</param>
        /// <param name="dataMapList"></param>
        public void setArray(string dataId, List<Dictionary<string, object>> dataMapList)
        {
            var detailDataSetList = new List<ColumnDataSet>();
            foreach (var columnData in dataMapList)
            {
                var columnDataSet = new ColumnDataSet { ColumnDataMap = columnData };
                detailDataSetList.Add(columnDataSet);
            }
            ArrayDataMap.Add(dataId, detailDataSetList);
        }

        // =====================================================
        // singleDataMap
        // =====================================================
        /// <summary>
        ///
        /// </summary>
        /// <param name="dataId"></param>
        /// <returns></returns>
        public ColumnDataSet getSingle(string dataId)
        {
            if (SingleDataMap.ContainsKey(dataId))
            {
                return SingleDataMap[dataId];
            }
            throw new Exception("getSingle : 索引 [" + dataId + "]未設定");
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="dataId"></param>
        /// <param name="singleData"></param>
        public void setSingle(string dataId, ColumnDataSet singleData)
        {
            SingleDataMap.Add(dataId, singleData);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="dataId"></param>
        /// <param name="singleData"></param>
        public void setSingle(string dataId, Dictionary<string, object> singleData)
        {
            var columnDataSet = new ColumnDataSet { ColumnDataMap = singleData };
            SingleDataMap.Add(dataId, columnDataSet);
        }
    }
}