using System.Collections.Generic;

namespace rbt.util.db.model
{
    public abstract class BaseDBEntity : IDBEntity
    {
        /// <summary>
        /// 記錄異動過的 Model
        /// </summary>
        protected HashSet<string> modeifyField { get; set; }

        /// <summary>
        /// DBEntityOpExtension 的資訊記錄欄位
        /// </summary>
        protected EntityOperatingModel operatingModel { get; set; }

        /// <summary>
        ///
        /// </summary>
        private IDBUtil _DAO { get; set; }

        public void SetDAO(IDBUtil dao)
        {
            _DAO = dao;
        }

        public IDBUtil GetDAO()
        {
            return _DAO;
        }

        /// <summary>
        /// 記錄異動過的欄位
        /// </summary>
        public HashSet<string> GetModeifyField()
        {
            return modeifyField;
        }

        /// <summary>
        /// DBEntityOpExtension 的操作儲存區
        /// </summary>
        public EntityOperatingModel GetOperating()
        {
            return operatingModel;
        }

        /// <summary>
        /// 建構子
        /// </summary>
        public BaseDBEntity()
        {
            modeifyField = new HashSet<string>();
            operatingModel = new EntityOperatingModel();
        }

        /// <summary>
        /// 取得 Table 名稱 (回傳 Entity Model 對應的 TableName)
        /// </summary>
        /// <returns></returns>
        public abstract string GetTableName();
    }

    /// <summary>
    /// RowDAO Extension
    /// </summary>
    public class OperatingField
    {
        public string COLUMN_NAME { get; set; }
        public string OPERATOR { get; set; }
        public object VALUE { get; set; }
    }

    /// <summary>
    ///
    /// </summary>
    public class EntityOperatingModel
    {
        public EntityOperatingModel()
        {
            this.AGGREGATE_LIST = new List<OperatingField>();
            this.WHERE_LIST = new List<OperatingField>();
            this.RAW_WHERE_LIST = new List<OperatingField>();
            this.ORDER_LIST = new List<OperatingField>();
            this.SET_LIST = new List<OperatingField>();
        }

        public string TABLE_NAME { get; set; }
        public IList<OperatingField> AGGREGATE_LIST { get; set; }
        public IList<OperatingField> WHERE_LIST { get; set; }
        public IList<OperatingField> RAW_WHERE_LIST { get; set; }
        public IList<OperatingField> ORDER_LIST { get; set; }
        public IList<OperatingField> SET_LIST { get; set; }
    }
}