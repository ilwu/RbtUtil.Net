using rbt.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace rbt.Extension
{
    public static class RbtExtension
    {
        /// <summary>
        /// Split 字串, 並回傳 List (字串為空防呆)
        /// </summary>
        /// <param name="str"></param>
        /// <param name="separator"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IList<string> SplitToList(this string str, string[] separator, StringSplitOptions options = StringSplitOptions.None)
        {
            if (StringUtil.IsEmpty(str)) return new List<string>();
            return StringUtil.SafeTrim(str).Split(separator, options).ToList();
        }

        /// <summary>
        /// Split 字串, 並回傳 List (字串為空防呆)
        /// </summary>
        /// <param name="str"></param>
        /// <param name="separator"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IList<string> SplitToList(this string str, string separator, StringSplitOptions options = StringSplitOptions.None)
        {
            return StringUtil.SafeTrim(str).SplitToList(new string[] { separator }, options);
        }

        /// <summary>
        /// Array To List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tAry"></param>
        /// <returns></returns>
        public static IList<T> ToList<T>(this T[] tAry)
        {
            if (tAry == null) tAry = new T[] { };
            var tlist = new List<T>();
            foreach (var t in tAry)
            {
                tlist.Add(t);
            }
            return tlist;
        }

        /// <summary>
        /// Distinct by property
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="items"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        public static IEnumerable<T> DistinctBy<T, TKey>(this IEnumerable<T> items, Func<T, TKey> property)
        {
            return items.GroupBy(property).Select(x => x.First());
        }

        /// <summary>
        /// 泛型方法 IEnumerable.IsExist() , 判斷筆數是否大於0 (傳入值為 null 時所產生的錯誤)
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsExist<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null || source.Count() == 0)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 擴充泛型方法 IEnumerable.First() 避免 傳入值為 null 時所產生的錯誤
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static TSource SafeFirst<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null || source.Count() == 0)
            {
                return default(TSource);
            }
            return source.First();
        }

        /// <summary>
        /// 擴充泛型方法 IList.First() 避免 傳入值為 null 時所產生的錯誤
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static TSource SafeFirst<TSource>(this IList<TSource> source)
        {
            if (source == null || source.Count() == 0)
            {
                return default(TSource);
            }
            return source.First();
        }

        /// <summary>
        /// 擴充泛型方法 IEnumerable.ToList() 避免 傳入值為 null 時所產生的錯誤
        /// 並在傳入 null 時, 也回傳空 list
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IList<TSource> SafeToList<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null || source.Count() == 0)
            {
                return new List<TSource>();
            }
            return source.ToList();
        }

        /// <summary>
        /// 擴充泛型方法 IList.ToList() 避免 傳入值為 null 時所產生的錯誤
        /// 並在傳入 null 時, 也回傳空 list
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IList<TSource> SafeToList<TSource>(this IList<TSource> source)
        {
            if (source == null || source.Count() == 0)
            {
                return new List<TSource>();
            }
            return source.ToList();
        }

        /// <summary>
        /// 泛型方法 IList.Move 移動資料於List 的位置
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">List</param>
        /// <param name="oldIndex">舊的index</param>
        /// <param name="newIndex">新的 index</param>
        /// <returns></returns>
        public static IList<T> Move<T>(this IList<T> list, int oldIndex, int newIndex)
        {
            T item = list[oldIndex];
            list.RemoveAt(oldIndex);
            list.Insert(newIndex, item);
            return list;
        }

        /// <summary>
        /// 直接取得 model 中的某個屬性 , 增加判斷傳入的 model 為 null
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="exp"></param>
        /// <returns></returns>
        public static T SafeGetValue<TSource, T>(this TSource model, Expression<Func<TSource, T>> exp)
        {
            if (model == null)
            {
                return default(T);
            }

            string propName = ((MemberExpression)exp.Body).Member.Name;

            object value = model.GetType().GetProperty(propName).GetValue(model);

            if (value == null)
            {
                return default(T);
            }

            return (T)value;
        }

        /// <summary>
        /// 針對 IDictionary 提供的安全取值方法, 可避免 dictionary 為空, 或 key 未包含在 dictionary 時所造成的 exception
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static TValue SafeGetValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            //檢核會出現 exception 的狀況
            if (dictionary == null || key == null || !dictionary.ContainsKey(key))
            {
                return default(TValue);
            }

            return dictionary[key];
        }

        /// <summary>
        /// object 擴充方法 : to string (避免物件為 null 時 to string 造成的錯誤)
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string SafeToString(this object obj)
        {
            //檢核會出現 exception 的狀況
            if (obj != null)
            {
                return obj.ToString();
            }

            return "";
        }

        ///// <summary>
        ///// String 擴充方法 : 比對是否為傳入值其中之一
        ///// </summary>
        ///// <param name="str"></param>
        ///// <param name="compareStrAry"></param>
        ///// <returns></returns>
        //public static bool In(this string str, params string[] compareStrAry)
        //{
        //    if (compareStrAry != null) return compareStrAry.Contains(str);
        //    return false;
        //}

        /// <summary>
        /// 擴充方法 : 比對是否為傳入值其中之一
        /// </summary>
        /// <param name="str"></param>
        /// <param name="compareStrAry"></param>
        /// <returns></returns>
        public static bool In<T>(this T str, params T[] compareStrAry)
        {
            if (compareStrAry != null) return compareStrAry.Contains(str);
            return false;
        }

        /// <summary>
        /// 物件擴充方法 : 判斷為空
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsEmpty<T>(this T obj)
        {
            if (typeof(T) == typeof(string))
            {
                return StringUtil.IsEmpty(obj + "");
            }
            return obj == null;
        }

        /// <summary>
        /// 擴充方法 : 判斷非空
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool NotEmpty<T>(this T obj)
        {
            return !IsEmpty(obj);
        }

        /// <summary>
        /// String 安全性增強擴充方法 : Trim
        /// </summary>
        /// <param name="str"></param>
        /// <param name="defaultStr">為空時預設的文字</param>
        /// <returns></returns>
        public static string SafeTrim(this string str, string defaultStr = "")
        {
            return StringUtil.SafeTrim(str, defaultStr);
        }

        public static string SafeTrim(this object obj, string defaultStr = "")
        {
            return StringUtil.SafeTrim(obj, defaultStr);
        }

        /// <summary>
        /// String 擴充方法 : 左補字
        /// </summary>
        /// <param name="str"></param>
        /// <param name="paddingChar"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string PaddingLeft(this string str, char paddingChar, int length)
        {
            return StringUtil.Padding(str, paddingChar, length, true);
        }

        /// <summary>
        /// String 擴充方法 : 右補字
        /// </summary>
        /// <param name="str"></param>
        /// <param name="paddingChar"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string PaddingRight(this string str, char paddingChar, int length)
        {
            return StringUtil.Padding(str, paddingChar, length, false);
        }

        /// <summary>
        /// String 擴充方法 : 取代第一個
        /// </summary>
        /// <param name="Source"></param>
        /// <param name="Find"></param>
        /// <param name="Replace"></param>
        /// <returns></returns>
        public static string ReplaceFirst(this string Source, string Find, string Replace)
        {
            int Place = Source.SafeToString().IndexOf(Find);
            if (Place < 0) return Source;
            return Source.Remove(Place, Find.Length).Insert(Place, Replace);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="exp"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string JoinStr<TSource, T>(this IEnumerable<TSource> list, Func<TSource, T> exp, string separator)
        {
            //無資料時, 回傳空白
            if (list == null || list.Count() == 0) return "";
            return String.Join(separator, list.Select(exp));
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="prefixMessage"></param>
        /// <returns></returns>
        public static string ToWellMessage(this Exception ex, string prefixMessage)
        {
            return System.Environment.NewLine + prefixMessage +
                   (prefixMessage.NotEmpty() ? ":" : "") + ex.Message + System.Environment.NewLine +
                   ex.StackTrace;
        }
    }
}