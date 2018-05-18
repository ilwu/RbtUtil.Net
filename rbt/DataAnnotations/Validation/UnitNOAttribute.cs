using rbt.util;
using System.ComponentModel.DataAnnotations;

namespace rbt.DataAnnotations.Validation
{
    /// <summary>
    /// 檢核是否為統一編號
    /// </summary>
    public class UnitNOAttribute : ValidationAttribute
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="value"></param>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            //======================================
            //傳入為空時不在此檢核 (由必填觸發)
            //======================================
            if (StringUtil.IsEmpty(value))
            {
                return ValidationResult.Success;
            }

            //======================================
            //檢核
            //======================================
            if (StringUtil.IsUniformNo(value))
            {
                return ValidationResult.Success;
            }

            return new ValidationResult(validationContext.DisplayName + "輸入格式錯誤！");
        }
    }
}