using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace rbt.util
{
    public class ClassUtil
    {
        /// <summary>
        /// 取得 Property 名稱
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="property"></param>
        /// <returns></returns>
        public static string PropertyName<T>(Expression<Func<T, object>> property) where T : class
        {
            MemberExpression body = (MemberExpression)property.Body;
            return body.Member.Name;
        }

        public static IDictionary<Type, Func<object, object>> GetConvertFuncMap()
        {
            var ConvertFuncMap = new Dictionary<Type, Func<object, object>>();

            ConvertFuncMap.Add(typeof(Int16?), (object x) =>
            {
                return x != null ? Convert.ToInt16(x) : x;
            });
            ConvertFuncMap.Add(typeof(Int16), (object x) =>
            {
                return x != null ? Convert.ToInt16(x) : x;
            });
            ConvertFuncMap.Add(typeof(Int32?), (object x) =>
            {
                return x != null ? Convert.ToInt32(x) : x;
            });
            ConvertFuncMap.Add(typeof(Int32), (object x) =>
            {
                return x != null ? Convert.ToInt32(x) : x;
            });
            ConvertFuncMap.Add(typeof(Int64?), (object x) =>
            {
                return x != null ? Convert.ToInt64(x) : x;
            });
            ConvertFuncMap.Add(typeof(Int64), (object x) =>
            {
                return x != null ? Convert.ToInt64(x) : x;
            });
            ConvertFuncMap.Add(typeof(decimal?), (object x) =>
            {
                return x != null ? Convert.ToDecimal(x) : x;
            });
            ConvertFuncMap.Add(typeof(decimal), (object x) =>
            {
                return x != null ? Convert.ToDecimal(x) : x;
            });
            ConvertFuncMap.Add(typeof(double?), (object x) =>
            {
                return x != null ? Convert.ToDouble(x) : x;
            });
            ConvertFuncMap.Add(typeof(double), (object x) =>
            {
                return x != null ? Convert.ToDouble(x) : x;
            });
            ConvertFuncMap.Add(typeof(DateTime?), (object x) =>
            {
                if (Convert.IsDBNull(x))
                {
                    return null;
                }
                return x;
            });

            return ConvertFuncMap;
        }
    }
}