namespace rbt.util.excel.bean.expt.config
{
    /// <summary>
    ///
    /// </summary>
    public class StyleInfo : AbstractStyleInfo
    {
        /// <summary>
        /// 未設定的元素, 自動帶入系統預設
        /// </summary>
        public void setEmptyAttrToSystemDefault()
        {
            // 字型
            if (Font == null)
            {
                Font = Constant.DEFAULT_FONT;
            }
            // 字體大小
            if (StringUtil.IsEmpty(Size))
            {
                Size = Constant.DEFAULT_FONT_SIZE + "";
            }
            //是否加粗
            if (StringUtil.IsEmpty(Bold))
            {
                Bold = Constant.DEFAULT_BOLD;
            }
            //是否斜體
            if (StringUtil.IsEmpty(Italic))
            {
                Italic = Constant.DEFAULT_ITALIC;
            }
            //底線
            if (StringUtil.IsEmpty(Underline))
            {
                Underline = Constant.DEFAULT_UNDERLINE_STYLE;
            }
            //字體顏色
            if (StringUtil.IsEmpty(Color))
            {
                Color = Constant.DEFAULT_COLOR;
            }

            //欄寬 因此欄位會影響整張表, 故輸出前才作判定
            //if (StringUtil.IsEmpty(this.width) || "0".equals(this.width.trim())) {
            //	this.width = Constant.WIDTH;
            //}

            //水平位置
            if (StringUtil.IsEmpty(Align))
            {
                Align = Constant.DEFAULT_ALIGN;
            }
            //垂直位置
            if (StringUtil.IsEmpty(Valign))
            {
                Valign = Constant.DEFAULT_VALIGN;
            }
            //自動換行
            if (StringUtil.IsEmpty(Wrap))
            {
                Wrap = Constant.DEFAULT_WRAP;
            }
            //背景顏色
            if (StringUtil.IsEmpty(Background))
            {
                Background = Constant.DEFAULT_BACKGROUND;
            }
            //邊線位置
            if (StringUtil.IsEmpty(BorderSide))
            {
                BorderSide = Constant.DEFAULT_BORDER_SIDE;
            }
            //邊線樣式
            if (StringUtil.IsEmpty(BorderStyle))
            {
                BorderStyle = Constant.DEFAULT_BORDER_STYLE;
            }
        }
    }
}