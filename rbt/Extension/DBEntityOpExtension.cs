using rbt.util;
using rbt.util.db;
using rbt.util.db.model;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq.Expressions;

namespace rbt.Extension
{
    public static class DBEntityOpExtension
    {
        #region "條件式"

        //====================================================================
        //條件式
        //====================================================================
        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="exp"></param>
        /// <param name="OPERATOR"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static TSource AddWhereOperating<TSource, T>(
            this TSource model, Expression<Func<TSource, T>> exp, string OPERATOR, T value)
            where TSource : IDBEntity
        {
            OperatingField field = new OperatingField()
            {
                COLUMN_NAME = ((MemberExpression)exp.Body).Member.Name,
                OPERATOR = OPERATOR,
                VALUE = value
            };

            model.GetOperating().WHERE_LIST.Add(field);

            return model;
        }

        /// <summary>
        /// 等於
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="exp"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static TSource IsEqual<TSource, T>(this TSource model, Expression<Func<TSource, T>> exp, T value)
            where TSource : IDBEntity
        {
            return AddWhereOperating(model, exp, "=", value);
        }

        /// <summary>
        /// 不等於
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="exp"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static TSource IsNotEqual<TSource, T>(this TSource model, Expression<Func<TSource, T>> exp, T value)
            where TSource : IDBEntity
        {
            return AddWhereOperating(model, exp, "<>", value);
        }

        /// <summary>
        /// 大於
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="exp"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static TSource GreaterThen<TSource, T>(this TSource model, Expression<Func<TSource, T>> exp, T value)
            where TSource : IDBEntity
        {
            return AddWhereOperating(model, exp, ">", value);
        }

        /// <summary>
        /// 大於等於
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="exp"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static TSource GreaterEqual<TSource, T>(this TSource model, Expression<Func<TSource, T>> exp, T value)
            where TSource : IDBEntity
        {
            return AddWhereOperating(model, exp, ">=", value);
        }

        /// <summary>
        /// 小於
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="exp"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static TSource LessThen<TSource, T>(this TSource model, Expression<Func<TSource, T>> exp, T value)
            where TSource : IDBEntity
        {
            return AddWhereOperating(model, exp, "<", value);
        }

        /// <summary>
        /// 小於等於
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="exp"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static TSource LessEqual<TSource, T>(this TSource model, Expression<Func<TSource, T>> exp, T value)
            where TSource : IDBEntity
        {
            return AddWhereOperating(model, exp, "<=", value);
        }

        /// <summary>
        /// 範圍： 大於等於 start. 小於等於 End
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="exp"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static TSource InRange<TSource, T>(this TSource model, Expression<Func<TSource, T>> exp, T start, T end)
            where TSource : IDBEntity
        {
            if (start != null)
            {
                AddWhereOperating(model, exp, ">=", start);
            }

            if (end != null)
            {
                AddWhereOperating(model, exp, "<=", end);
            }

            return model;
        }

        /// <summary>
        /// 欄位值為 NULL
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="exp"></param>
        /// <returns></returns>
        public static TSource IsNull<TSource, T>(this TSource model, Expression<Func<TSource, T>> exp)
            where TSource : IDBEntity
        {
            OperatingField field = new OperatingField()
            {
                COLUMN_NAME = ((MemberExpression)exp.Body).Member.Name,
                OPERATOR = "IS",
                VALUE = "NULL"
            };

            model.GetOperating().WHERE_LIST.Add(field);

            return model;
        }

        /// <summary>
        /// 欄位值不為 NULL
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="exp"></param>
        /// <returns></returns>
        public static TSource IsNotNull<TSource, T>(this TSource model, Expression<Func<TSource, T>> exp)
            where TSource : IDBEntity
        {
            model.GetOperating().WHERE_LIST.Add(new OperatingField()
            {
                COLUMN_NAME = ((MemberExpression)exp.Body).Member.Name,
                OPERATOR = "IS",
                VALUE = "NOT NULL"
            });

            return model;
        }

        /// <summary>
        /// 待測試
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="exp"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public static TSource IsLike<TSource, T>(this TSource model, Expression<Func<TSource, T>> exp, string pattern) where TSource : IDBEntity
        {
            model.GetOperating().WHERE_LIST.Add(new OperatingField()
            {
                COLUMN_NAME = ((MemberExpression)exp.Body).Member.Name,
                OPERATOR = "LIKE",
                VALUE = pattern
            });

            return model;
        }

        /// <summary>
        /// SQL查詢語句中 'IN (...)'查詢條件。
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="exp"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public static TSource IsInRAW<TSource, T>(this TSource model, Expression<Func<TSource, T>> exp, IList<string> list) where TSource : IDBEntity
        {
            model.GetOperating().WHERE_LIST.Add(new OperatingField()
            {
                COLUMN_NAME = ((MemberExpression)exp.Body).Member.Name,
                OPERATOR = "IN",
                VALUE = "(" + BaseSqlUtil.GenInSql((List<string>)list) + ")"
            });

            return model;
        }

        /// <summary>
        /// 正向排序
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="exp"></param>
        /// <returns></returns>
        public static TSource OrderByAsc<TSource, T>(this TSource model, Expression<Func<TSource, T>> exp)
            where TSource : IDBEntity
        {
            model.GetOperating().ORDER_LIST.Add(new OperatingField()
            {
                COLUMN_NAME = ((MemberExpression)exp.Body).Member.Name,
                VALUE = "ASC"
            });

            return model;
        }

        /// <summary>
        /// 反向排序
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="exp"></param>
        /// <returns></returns>
        public static TSource OrderByDesc<TSource, T>(this TSource model, Expression<Func<TSource, T>> exp)
            where TSource : IDBEntity
        {
            model.GetOperating().ORDER_LIST.Add(new OperatingField()
            {
                COLUMN_NAME = ((MemberExpression)exp.Body).Member.Name,
                VALUE = "DESC"
            });

            return model;
        }

        #endregion "條件式"

        #region "查詢"

        //====================================================================
        //查詢
        //====================================================================
        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="dao"></param>
        /// <returns></returns>
        public static IList<T> QueryList<T>(this T model, IDBUtil dao,
            DbConnection conn = null, DbTransaction trns = null) where T : IDBEntity
        {
            //處理 where 參數
            var whereParams = prepareWhereParams(model.GetOperating().WHERE_LIST);

            //處理 order by 參數
            string[] orderByFileds = prepareOrderBy(model.GetOperating().ORDER_LIST);

            //查詢
            return dao.QueryByTableAndOrderBy<T>(
                model.GetTableName(), whereParams, orderByFileds, conn, trns);
        }

        /// <summary>
        /// 取回第一筆 (查不到時, 回傳 null)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="dao"></param>
        /// <returns></returns>
        public static T QueryFirst<T>(this T model, IDBUtil dao,
            DbConnection conn = null, DbTransaction trns = null) where T : IDBEntity
        {
            //處理 where 參數
            var whereParams = prepareWhereParams(model.GetOperating().WHERE_LIST);

            //處理 order by 參數
            string[] orderByFileds = prepareOrderBy(model.GetOperating().ORDER_LIST);

            //查詢
            return dao.QueryByTableAndOrderBy<T>(
                model.GetTableName(), whereParams, orderByFileds, conn, trns).SafeFirst();
        }

        /// <summary>
        /// 取得筆數
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static int QueryCount<T>(this T model, IDBUtil dao,
            DbConnection conn = null, DbTransaction trns = null) where T : IDBEntity
        {
            //處理 where 參數
            var whereParams = prepareWhereParams(model.GetOperating().WHERE_LIST);
            //查詢
            return dao.QueryCount(model.GetTableName(), whereParams, conn, trns);
        }

        /// <summary>
        /// 查詢欄位最大值
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="exp"></param>
        /// <param name="dao"></param>
        /// <param name="conn"></param>
        /// <param name="trns"></param>
        /// <returns></returns>
        public static T QueryMax<TSource, T>(this TSource model, Expression<Func<TSource, T>> exp, IDBUtil dao,
            DbConnection conn = null, DbTransaction trns = null) where TSource : IDBEntity
        {
            return QueryAggregate(model, exp, "MAX", dao, conn, trns);
        }

        /// <summary>
        /// 查詢欄位最小值
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="exp"></param>
        /// <param name="dao"></param>
        /// <param name="conn"></param>
        /// <param name="trns"></param>
        /// <returns></returns>
        public static T QueryMin<TSource, T>(this TSource model, Expression<Func<TSource, T>> exp, IDBUtil dao,
            DbConnection conn = null, DbTransaction trns = null) where TSource : IDBEntity
        {
            return QueryAggregate(model, exp, "MIN", dao, conn, trns);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="exp"></param>
        /// <param name="optor"></param>
        /// <param name="dao"></param>
        /// <param name="conn"></param>
        /// <param name="trns"></param>
        /// <returns></returns>
        private static T QueryAggregate<TSource, T>(
            TSource model,
            Expression<Func<TSource, T>> exp,
            string optor,
            IDBUtil dao,
            DbConnection conn = null, DbTransaction trns = null) where TSource : IDBEntity
        {
            //要查詢的欄位
            string propName = ((MemberExpression)exp.Body).Member.Name;

            //處理 where 參數
            var whereParams = prepareWhereParams(model.GetOperating().WHERE_LIST);

            var selectColumns = new List<string>();
            selectColumns.Add(optor + "(" + propName + ") AS result");

            //查詢
            var resultList = dao.QueryByTable(model.GetTableName(), selectColumns, whereParams, conn, trns);

            if (resultList != null && resultList.Count > 0 && !DBNull.Value.Equals(resultList[0]["result"]))
            {
                Func<object, object> converter = ClassUtil.GetConvertFuncMap()[typeof(T)];
                return (T)converter(resultList[0].SafeGetValue("result"));
            }
            return default(T);
        }

        /// <summary>
        /// 回傳依條件判斷是否存在筆數
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static bool IsExist<T>(this T model, IDBUtil dao,
            DbConnection conn = null, DbTransaction trns = null) where T : IDBEntity
        {
            //處理 where 參數
            var whereParams = prepareWhereParams(model.GetOperating().WHERE_LIST);
            //查詢
            return dao.IsExist(model.GetTableName(), whereParams, conn, trns);
        }

        #endregion "查詢"

        #region "新增 INSERT"

        //====================================================================
        //新增 INSERT
        //====================================================================

        #endregion "新增 INSERT"

        /// <summary>
        /// 新增 (條件式結束語句)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="dao"></param>
        /// <param name="conn"></param>
        /// <param name="trns"></param>
        /// <returns></returns>
        public static int Insert<T>(this T model, IDBUtil dao,
            DbConnection conn = null, DbTransaction trns = null) where T : IDBEntity
        {
            //Insert
            return dao.Insert(model, model.GetTableName(), conn, trns);
        }

        #region "修改 UPDATE"

        //====================================================================
        //修改 UPDATE
        //====================================================================
        /// <summary>
        /// 修改 UPDATE
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="dao"></param>
        /// <param name="conn"></param>
        /// <param name="trns"></param>
        /// <returns>異動筆數</returns>
        //public static int Update<T>(this T model, IDBUtil dao,
        //    DbConnection conn = null, DbTransaction trns = null) where T : IDBEntity
        //{
        //    //處理 where 參數
        //    var whereParams = prepareWhereParams(model.GetOperating().WHERE_LIST);

        //    //處理 set 參數 (僅抓 model 中有異動到的參數)
        //    var setColumnsInfo = new Dictionary<string, object>();

        //    foreach (var pi in model.GetType().GetProperties())
        //    {
        //        //var pName = "";
        //        ////忽略 db 欄位大小寫和 model 的不同
        //        //if (dic.ContainsKey(p.Name.ToUpper())) pName = p.Name.ToUpper();
        //        //if (dic.ContainsKey(p.Name.ToLower())) pName = p.Name.ToLower();

        //        if (model.GetModeifyField().Contains(pi.Name))
        //        {
        //            setColumnsInfo.Add(pi.Name, pi.GetValue(model));
        //        }
        //    }

        //    return dao.Update(model.GetTableName(), setColumnsInfo, whereParams, conn, trns);
        //}

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="updateModel"></param>
        /// <param name="dao"></param>
        /// <param name="conn"></param>
        /// <param name="trns"></param>
        /// <returns></returns>
        public static int Update<T>(this T model,
            IDBUtil dao,
            T updateModel,
            DbConnection conn = null, DbTransaction trns = null) where T : IDBEntity
        {
            //處理 where 參數
            var whereParams = prepareWhereParams(model.GetOperating().WHERE_LIST);

            //處理 set 參數 (僅抓 model 中有異動到的參數)
            var setColumnsInfo = new Dictionary<string, object>();

            foreach (var pi in updateModel.GetType().GetProperties())
            {
                //var pName = "";
                ////忽略 db 欄位大小寫和 model 的不同
                //if (dic.ContainsKey(p.Name.ToUpper())) pName = p.Name.ToUpper();
                //if (dic.ContainsKey(p.Name.ToLower())) pName = p.Name.ToLower();

                if (updateModel.GetModeifyField().Contains(pi.Name))
                {
                    setColumnsInfo.Add(pi.Name, pi.GetValue(updateModel));
                }
            }

            return dao.Update(model.GetTableName(), setColumnsInfo, whereParams, conn, trns);
        }

        #endregion "修改 UPDATE"

        #region "刪除 DELETE"

        //====================================================================
        //刪除 DELETE
        //====================================================================
        /// <summary>
        /// 刪除 (條件式結束語句)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="dao"></param>
        /// <param name="conn"></param>
        /// <param name="trns"></param>
        /// <returns></returns>
        public static int Delete<T>(this T model, rbt.util.db.IDBUtil dao,
            DbConnection conn = null, DbTransaction trns = null) where T : IDBEntity
        {
            //處理 where 參數
            var whereParams = prepareWhereParams(model.GetOperating().WHERE_LIST);

            //刪除
            return dao.Delete(model.GetTableName(), whereParams, conn, trns);
        }

        #endregion "刪除 DELETE"

        #region "其他"

        //====================================================================
        //其他
        //====================================================================

        /// <summary>
        /// 組排序欄位
        /// </summary>
        /// <param name="ORDER_LIST"></param>
        /// <returns></returns>
        private static string[] prepareOrderBy(IList<OperatingField> ORDER_LIST)
        {
            var orderByFileds = new List<string>();

            foreach (OperatingField field in ORDER_LIST)
            {
                orderByFileds.Add(field.COLUMN_NAME + " " + field.OPERATOR);
            }

            return orderByFileds.ToArray();
        }

        /// <summary>
        /// 組 Where 欄位
        /// </summary>
        /// <param name="WHERE_LIST"></param>
        /// <returns></returns>
        private static Dictionary<string, object> prepareWhereParams(IList<OperatingField> WHERE_LIST)
        {
            var whereParams = new Dictionary<string, object>();

            foreach (OperatingField field in WHERE_LIST)
            {
                switch (field.OPERATOR)
                {
                    case "=":
                        whereParams.Add(field.COLUMN_NAME, field.VALUE);
                        break;

                    case ">":
                    case ">=":
                    case "<":
                    case "<=":
                    case "<>":
                        whereParams.Add(field.OPERATOR + field.COLUMN_NAME, field.VALUE);
                        break;

                    case "IN":
                        whereParams.Add("!@#" + field.COLUMN_NAME + " IN ", field.VALUE);
                        break;

                    case "IS":
                        whereParams.Add("!@#" + field.COLUMN_NAME + " IS ", field.VALUE);
                        break;

                    case "LIKE":
                        whereParams.Add("#@!" + field.COLUMN_NAME, field.VALUE);
                        break;

                    default:
                        throw new Exception("未定義的型態:[" + field.OPERATOR + "]");
                }
            }

            return whereParams;
        }

        #endregion "其他"
    }
}