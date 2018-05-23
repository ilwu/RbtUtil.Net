using rbt.util;
using System;
using System.ComponentModel.DataAnnotations;

namespace rbt.DataAnnotations.Validation
{
    /// <summary>
    /// 自訂數字格式檢核
    /// </summary>
    public class NumberValidtionAttribute : ValidationAttribute
    {
        private int DataPrecision = 0;
        private int DataScale = 0;

        public NumberValidtionAttribute(int dataPrecision, int dataScale)
        {
            DataPrecision = (int)dataPrecision;
            DataScale = (int)dataScale;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            //======================================
            //未設定長度時不檢核
            //======================================
            if (DataPrecision == 0 && DataScale == 0)
            {
                return ValidationResult.Success;
            }

            var numStr = StringUtil.SafeTrim(value);
            //======================================
            //傳入為空時不在此檢核 (由必填觸發)
            //======================================
            if (StringUtil.IsEmpty(numStr))
            {
                return ValidationResult.Success;
            }

            //======================================
            //整數
            //======================================
            if (DataScale == 0)
            {
                //非數字檢核
                var a = 0;
                if (!int.TryParse(numStr, out a))
                {
                    return new ValidationResult(validationContext.DisplayName + "輸入格式錯誤，請輸入整數！");
                }

                if (numStr.Length > DataPrecision)
                {
                    return new ValidationResult(validationContext.DisplayName + "輸入錯誤，數字長度需小於等於[" + DataPrecision + "]位！");
                }
            }

            //======================================
            //小數
            //======================================
            if (DataScale > 0)
            {
                var b = new Decimal(0);
                //非數字檢核
                if (!Decimal.TryParse(numStr, out b))
                {
                }
                var numAry = numStr.Split('.');

                if (numAry[0].Length > (DataPrecision - DataScale))
                {
                    return new ValidationResult(validationContext.DisplayName + "輸入格式錯誤，整數部分長度需小於[" + (DataPrecision - DataScale) + "]位！");
                }

                if (numAry.Length > 1 && numAry[1].Length > DataScale)
                {
                    return new ValidationResult(validationContext.DisplayName + "輸入格式錯誤，小數部分長度不可超過[" + (DataScale) + "]位！");
                }
            }

            return ValidationResult.Success;
        }
    }
}