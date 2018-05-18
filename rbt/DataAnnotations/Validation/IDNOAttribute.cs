using rbt.util;
using System.ComponentModel.DataAnnotations;

namespace rbt.DataAnnotations.Validation
{
    /// <summary>
    /// 身分證字號檢核
    /// </summary>
    public class IDNOAttribute : ValidationAttribute
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
            if (StringUtil.IsIDNO(value))
            {
                return ValidationResult.Success;
            }

            return new ValidationResult(validationContext.DisplayName + "輸入格式錯誤！");
        }
    }
}