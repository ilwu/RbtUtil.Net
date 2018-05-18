using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace rbt.Extension
{
    public static class ModelExtension
    {
        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="Model"></param>
        /// <returns></returns>
        public static Hashtable ToMap<TModel>(this TModel Model)
        {
            var resultMap = new Hashtable();

            if (Model != null)
            {
                TModel rtn = (TModel)Activator.CreateInstance(typeof(TModel));
                foreach (var propertie in typeof(TModel).GetProperties())
                {
                    // 只處理資料庫欄位有對應到的 Property
                    MethodInfo getMethod = propertie.GetGetMethod();

                    if (getMethod != null)
                    {
                        resultMap.Add(propertie.Name, getMethod.Invoke(Model, new object[0]));
                    }
                }
            }
            return resultMap;
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="modelList"></param>
        /// <returns></returns>
        public static IList<Hashtable> ToMapList<TModel>(this IList<TModel> modelList)
        {
            var mapList = new List<Hashtable>();
            if (modelList != null)
            {
                foreach (var model in modelList)
                {
                    mapList.Add(model.ToMap());
                }
            }
            return mapList;
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static T GetAttributeFromProperty<T>(this object instance, string propertyName) where T : Attribute
        {
            var attrType = typeof(T);
            var property = instance.GetType().GetProperty(propertyName);
            var customAttributes = (T[])property.GetCustomAttributes(attrType, true);
            if (customAttributes != null && customAttributes.Length > 0)
            {
                return customAttributes.First();
            }
            return null;
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static T GetAttributeFromType<T>(this object instance) where T : Attribute
        {
            return instance.GetType().GetAttributeFromType<T>();
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <returns></returns>
        public static T GetAttributeFromType<T>(this Type type) where T : Attribute
        {
            var attrType = typeof(T);
            var customAttributes = (T[])type.GetCustomAttributes(attrType, true);
            if (customAttributes != null && customAttributes.Length > 0)
            {
                return customAttributes.First();
            }
            return null;
        }
    }
}