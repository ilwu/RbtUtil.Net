using System;
using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace rbt.DataAnnotations.Validation
{
    /// <summary>
    /// 檢核 List 不能為空
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class ListNotEmptyAttribute : ValidationAttribute
    {
        private const string defaultError = "'{0}' 至少需要有一筆資料";

        public override bool IsValid(object value)
        {
            IList list = value as IList;
            return (list != null && list.Count > 0);
        }

        public override string FormatErrorMessage(string name)
        {
            return String.Format(defaultError, name);
        }
    }
}