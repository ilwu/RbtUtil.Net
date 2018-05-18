using CSharpJExcel.Jxl.Format;
using CSharpJExcel.Jxl.Write;
using rbt.util.excel.util;
using System;
using System.Xml;

namespace rbt.util.excel.bean.expt.config
{
    /// <summary>
    /// 和 Style 有關的屬性設定資訊
    /// </summary>
    public abstract class AbstractStyleInfo
    {
        // =====================================================
        // 字體
        // =====================================================

        /// <summary>
        /// 字型
        /// </summary>
        public WritableFont.FontName Font { get; set; }

        /// <summary>
        /// 字體大小
        /// </summary>
        public string Size { get; set; }

        /// <summary>
        /// 字體顏色
        /// </summary>
        public Colour Color { get; set; }

        /// <summary>
        /// 背景顏色
        /// </summary>
        public Colour Background { get; set; }

        /// <summary>
        /// 是否加粗
        /// </summary>
        public string Bold { get; set; }

        /// <summary>
        ///是否斜體
        /// </summary>
        public string Italic { get; set; }

        /// <summary>
        /// 底線
        /// </summary>
        public UnderlineStyle Underline { get; set; }

        /// <summary>
        /// 垂直位置
        /// </summary>
        public VerticalAlignment Valign { get; set; }

        /// <summary>
        /// 欄位寬度
        /// </summary>
        public string Width { get; set; }

        /// <summary>
        /// 欄位高度
        /// </summary>
        public string Height { get; set; }

        /// <summary>
        /// 自動換行
        /// </summary>
        public string Wrap { get; set; }

        /// <summary>
        /// 水平對齊
        /// </summary>
        public Alignment Align { get; set; }

        /// <summary>
        /// 邊線樣式
        /// </summary>
        public BorderLineStyle BorderStyle { get; set; }

        /// <summary>
        /// 邊線位置
        /// </summary>
        public Border BorderSide { get; set; }

        // =====================================================
        // 公用程式
        // =====================================================
        /// <summary>
        /// 讀取 Node 中,與 style 類型元素 相關的屬性
        /// </summary>
        /// <param name="node"></param>
        public void readStyleAttr(XmlNode node)
        {
            if (node == null)
            {
                return;
            }

            if (node.Attributes == null)
            {
                return;
            }
            // 字型
            Font = getFont(ExcelStringUtil.GetNodeAttr(node, Constant.ATTRIBUTE_FONT));
            // 字體大小
            Size = ExcelStringUtil.GetNodeAttr(node, Constant.ATTRIBUTE_SIZE);
            // 粗體
            Bold = ExcelStringUtil.GetNodeAttr(node, Constant.ATTRIBUTE_BOLD);
            // 斜體
            Italic = ExcelStringUtil.GetNodeAttr(node, Constant.ATTRIBUTE_ITALIC);
            // 底線
            Underline = getUnderlineStyle(ExcelStringUtil.GetNodeAttr(node, Constant.ATTRIBUTE_UNDERLINE));
            // 列高
            Height = ExcelStringUtil.GetNodeAttr(node, Constant.ATTRIBUTE_HEIGHT);
            // 欄寬
            Width = ExcelStringUtil.GetNodeAttr(node, Constant.ATTRIBUTE_WIDTH);
            // 文字顏色
            Color = getColour(ExcelStringUtil.GetNodeAttr(node, Constant.ATTRIBUTE_COLOR));
            // 水平位置
            Align = getAlign(ExcelStringUtil.GetNodeAttr(node, Constant.ATTRIBUTE_ALIGN));
            // 垂直位置
            Valign = getValign(ExcelStringUtil.GetNodeAttr(node, Constant.ATTRIBUTE_VALIGN));
            // 自動換行
            Wrap = ExcelStringUtil.GetNodeAttr(node, Constant.ATTRIBUTE_WRAP);
            // 背景顏色
            Background = getColour(ExcelStringUtil.GetNodeAttr(node, Constant.ATTRIBUTE_BACKGROUND));
            // 邊線位置
            BorderSide = getBorderSide(ExcelStringUtil.GetNodeAttr(node, Constant.ATTRIBUTE_BORDERSIDE));
            // 邊線樣式
            BorderStyle = getBorderLineStyle(ExcelStringUtil.GetNodeAttr(node, Constant.ATTRIBUTE_BORDERSTYLE));
        }

        // =====================================================
        // 私有 function
        // =====================================================
        /// <summary>
        /// 取得水平位置設定
        /// </summary>
        /// <param name="align"></param>
        private Alignment getAlign(string align)
        {
            if (string.Equals(align, "center", StringComparison.OrdinalIgnoreCase))
            {
                return Alignment.CENTRE;
            }
            if (string.Equals(align, "right", StringComparison.OrdinalIgnoreCase))
            {
                return Alignment.RIGHT;
            }
            if (string.Equals(align, "left", StringComparison.OrdinalIgnoreCase))
            {
                return Alignment.LEFT;
            }
            return null;
        }

        /// <summary>
        /// 取得垂直位置設定
        /// </summary>
        /// <param name="valign"></param>
        private VerticalAlignment getValign(string valign)
        {
            if (string.Equals(valign, "center", StringComparison.OrdinalIgnoreCase))
            {
                return VerticalAlignment.CENTRE;
            }
            if (string.Equals(valign, "top", StringComparison.OrdinalIgnoreCase))
            {
                return VerticalAlignment.TOP;
            }
            if (string.Equals(valign, "botton", StringComparison.OrdinalIgnoreCase))
            {
                return VerticalAlignment.BOTTOM;
            }
            return null;
        }

        /**
         * 取得粗體設定
         * @param fontName
         * @return
         */

        private WritableFont.FontName getFont(string fontName)
        {
            if (StringUtil.NotEmpty(fontName))
            {
                return WritableFont.createFont(fontName);
            }
            return null;
        }

        /**
         * 取得底線設定
         * @param underLine
         * @return
         */

        private UnderlineStyle getUnderlineStyle(string underLine)
        {
            if (StringUtil.NotEmpty(underLine))
            {
                if (string.Equals(underLine, "true", StringComparison.OrdinalIgnoreCase))
                {
                    return UnderlineStyle.SINGLE;
                }
                return UnderlineStyle.NO_UNDERLINE;
            }
            return null;
        }

        /**
	     * 取得字體顏色
	     * @param fontColor
	     * @return
	     */

        private Colour getColour(string fontColor)
        {
            if (string.Equals(fontColor, "AQUA", StringComparison.OrdinalIgnoreCase))
            {
                return Colour.AQUA;
            }
            if (string.Equals(fontColor, "AUTOMATIC", StringComparison.OrdinalIgnoreCase))
            {
                return Colour.AUTOMATIC;
            }
            if (string.Equals(fontColor, "BLACK", StringComparison.OrdinalIgnoreCase))
            {
                return Colour.BLACK;
            }
            if (string.Equals(fontColor, "BLUE_GREY", StringComparison.OrdinalIgnoreCase))
            {
                return Colour.BLUE_GREY;
            }
            if (string.Equals(fontColor, "BLUE", StringComparison.OrdinalIgnoreCase))
            {
                return Colour.BLUE;
            }
            if (string.Equals(fontColor, "BLUE2", StringComparison.OrdinalIgnoreCase))
            {
                return Colour.BLUE2;
            }
            if (string.Equals(fontColor, "BRIGHT_GREEN", StringComparison.OrdinalIgnoreCase))
            {
                return Colour.BRIGHT_GREEN;
            }
            if (string.Equals(fontColor, "BROWN", StringComparison.OrdinalIgnoreCase))
            {
                return Colour.BROWN;
            }
            if (string.Equals(fontColor, "CORAL", StringComparison.OrdinalIgnoreCase))
            {
                return Colour.CORAL;
            }
            if (string.Equals(fontColor, "DARK_BLUE", StringComparison.OrdinalIgnoreCase))
            {
                return Colour.DARK_BLUE;
            }
            if (string.Equals(fontColor, "DARK_BLUE2", StringComparison.OrdinalIgnoreCase))
            {
                return Colour.DARK_BLUE2;
            }
            if (string.Equals(fontColor, "DARK_GREEN", StringComparison.OrdinalIgnoreCase))
            {
                return Colour.DARK_GREEN;
            }
            if (string.Equals(fontColor, "DARK_PURPLE", StringComparison.OrdinalIgnoreCase))
            {
                return Colour.DARK_PURPLE;
            }
            if (string.Equals(fontColor, "DARK_RED", StringComparison.OrdinalIgnoreCase))
            {
                return Colour.DARK_RED;
            }
            if (string.Equals(fontColor, "DARK_RED2", StringComparison.OrdinalIgnoreCase))
            {
                return Colour.DARK_RED2;
            }
            if (string.Equals(fontColor, "DARK_TEAL", StringComparison.OrdinalIgnoreCase))
            {
                return Colour.DARK_TEAL;
            }
            if (string.Equals(fontColor, "DARK_YELLOW", StringComparison.OrdinalIgnoreCase))
            {
                return Colour.DARK_YELLOW;
            }
            if (string.Equals(fontColor, "GOLD", StringComparison.OrdinalIgnoreCase))
            {
                return Colour.GOLD;
            }
            if (string.Equals(fontColor, "GRAY_25", StringComparison.OrdinalIgnoreCase))
            {
                return Colour.GRAY_25;
            }
            if (string.Equals(fontColor, "GRAY_50", StringComparison.OrdinalIgnoreCase))
            {
                return Colour.GRAY_50;
            }
            if (string.Equals(fontColor, "GRAY_80", StringComparison.OrdinalIgnoreCase))
            {
                return Colour.GRAY_80;
            }
            if (string.Equals(fontColor, "GREEN", StringComparison.OrdinalIgnoreCase))
            {
                return Colour.GREEN;
            }
            if (string.Equals(fontColor, "GREY_25_PERCENT", StringComparison.OrdinalIgnoreCase))
            {
                return Colour.GREY_25_PERCENT;
            }
            if (string.Equals(fontColor, "GREY_40_PERCENT", StringComparison.OrdinalIgnoreCase))
            {
                return Colour.GREY_40_PERCENT;
            }
            if (string.Equals(fontColor, "GREY_50_PERCENT", StringComparison.OrdinalIgnoreCase))
            {
                return Colour.GREY_50_PERCENT;
            }
            if (string.Equals(fontColor, "GREY_80_PERCENT", StringComparison.OrdinalIgnoreCase))
            {
                return Colour.GREY_80_PERCENT;
            }
            if (string.Equals(fontColor, "ICE_BLUE", StringComparison.OrdinalIgnoreCase))
            {
                return Colour.ICE_BLUE;
            }
            if (string.Equals(fontColor, "INDIGO", StringComparison.OrdinalIgnoreCase))
            {
                return Colour.INDIGO;
            }
            if (string.Equals(fontColor, "IVORY", StringComparison.OrdinalIgnoreCase))
            {
                return Colour.IVORY;
            }
            if (string.Equals(fontColor, "LAVENDER", StringComparison.OrdinalIgnoreCase))
            {
                return Colour.LAVENDER;
            }
            if (string.Equals(fontColor, "LIGHT_BLUE", StringComparison.OrdinalIgnoreCase))
            {
                return Colour.LIGHT_BLUE;
            }
            if (string.Equals(fontColor, "LIGHT_GREEN", StringComparison.OrdinalIgnoreCase))
            {
                return Colour.LIGHT_GREEN;
            }
            if (string.Equals(fontColor, "LIGHT_ORANGE", StringComparison.OrdinalIgnoreCase))
            {
                return Colour.LIGHT_ORANGE;
            }
            if (string.Equals(fontColor, "LIGHT_TURQUOISE", StringComparison.OrdinalIgnoreCase))
            {
                return Colour.LIGHT_TURQUOISE;
            }
            if (string.Equals(fontColor, "LIGHT_TURQUOISE2", StringComparison.OrdinalIgnoreCase))
            {
                return Colour.LIGHT_TURQUOISE2;
            }
            if (string.Equals(fontColor, "LIME", StringComparison.OrdinalIgnoreCase))
            {
                return Colour.LIME;
            }
            if (string.Equals(fontColor, "OCEAN_BLUE", StringComparison.OrdinalIgnoreCase))
            {
                return Colour.OCEAN_BLUE;
            }
            if (string.Equals(fontColor, "OLIVE_GREEN", StringComparison.OrdinalIgnoreCase))
            {
                return Colour.OLIVE_GREEN;
            }
            if (string.Equals(fontColor, "ORANGE", StringComparison.OrdinalIgnoreCase))
            {
                return Colour.ORANGE;
            }
            if (string.Equals(fontColor, "PALE_BLUE", StringComparison.OrdinalIgnoreCase))
            {
                return Colour.PALE_BLUE;
            }
            if (string.Equals(fontColor, "PERIWINKLE", StringComparison.OrdinalIgnoreCase))
            {
                return Colour.PERIWINKLE;
            }
            if (string.Equals(fontColor, "PINK", StringComparison.OrdinalIgnoreCase))
            {
                return Colour.PINK;
            }
            if (string.Equals(fontColor, "PINK2", StringComparison.OrdinalIgnoreCase))
            {
                return Colour.PINK2;
            }
            if (string.Equals(fontColor, "PLUM", StringComparison.OrdinalIgnoreCase))
            {
                return Colour.PLUM;
            }
            if (string.Equals(fontColor, "PLUM2", StringComparison.OrdinalIgnoreCase))
            {
                return Colour.PLUM2;
            }
            if (string.Equals(fontColor, "RED", StringComparison.OrdinalIgnoreCase))
            {
                return Colour.RED;
            }
            if (string.Equals(fontColor, "ROSE", StringComparison.OrdinalIgnoreCase))
            {
                return Colour.ROSE;
            }
            if (string.Equals(fontColor, "SEA_GREEN", StringComparison.OrdinalIgnoreCase))
            {
                return Colour.SEA_GREEN;
            }
            if (string.Equals(fontColor, "SKY_BLUE", StringComparison.OrdinalIgnoreCase))
            {
                return Colour.SKY_BLUE;
            }
            if (string.Equals(fontColor, "TAN", StringComparison.OrdinalIgnoreCase))
            {
                return Colour.TAN;
            }
            if (string.Equals(fontColor, "TEAL", StringComparison.OrdinalIgnoreCase))
            {
                return Colour.TEAL;
            }
            if (string.Equals(fontColor, "TEAL2", StringComparison.OrdinalIgnoreCase))
            {
                return Colour.TEAL2;
            }
            if (string.Equals(fontColor, "TURQOISE2", StringComparison.OrdinalIgnoreCase))
            {
                return Colour.TURQOISE2;
            }
            if (string.Equals(fontColor, "TURQUOISE", StringComparison.OrdinalIgnoreCase))
            {
                return Colour.TURQUOISE;
            }
            if (string.Equals(fontColor, "VERY_LIGHT_YELLOW", StringComparison.OrdinalIgnoreCase))
            {
                return Colour.VERY_LIGHT_YELLOW;
            }
            if (string.Equals(fontColor, "VIOLET", StringComparison.OrdinalIgnoreCase))
            {
                return Colour.VIOLET;
            }
            if (string.Equals(fontColor, "VIOLET2", StringComparison.OrdinalIgnoreCase))
            {
                return Colour.VIOLET2;
            }
            if (string.Equals(fontColor, "WHITE", StringComparison.OrdinalIgnoreCase))
            {
                return Colour.WHITE;
            }
            if (string.Equals(fontColor, "YELLOW", StringComparison.OrdinalIgnoreCase))
            {
                return Colour.YELLOW;
            }
            if (string.Equals(fontColor, "YELLOW2", StringComparison.OrdinalIgnoreCase))
            {
                return Colour.YELLOW2;
            }

            return null;
        }

        /**
	     * 取得邊線設定
	     * @param borderSide
	     * @return
	     */

        private Border getBorderSide(string borderSide)
        {
            // NONE|ALL|TOP|BOTTOM|LEFT|RIGHT

            if (string.Equals(borderSide, "NONE", StringComparison.OrdinalIgnoreCase))
            {
                return Border.NONE;
            }
            if (string.Equals(borderSide, "ALL", StringComparison.OrdinalIgnoreCase))
            {
                return Border.ALL;
            }
            if (string.Equals(borderSide, "TOP", StringComparison.OrdinalIgnoreCase))
            {
                return Border.TOP;
            }
            if (string.Equals(borderSide, "BOTTOM", StringComparison.OrdinalIgnoreCase))
            {
                return Border.BOTTOM;
            }
            if (string.Equals(borderSide, "LEFT", StringComparison.OrdinalIgnoreCase))
            {
                return Border.LEFT;
            }
            if (string.Equals(borderSide, "RIGHT", StringComparison.OrdinalIgnoreCase))
            {
                return Border.RIGHT;
            }
            return null;
        }

        /**
	     * 取得邊線樣式設定 BorderLineStyle
	     * NONE 無 <br/>
	     * THIN 薄 <br/>
	     * MEDIUM 中等的 <br/>
	     * DASHED 虛線 <br/>
	     * DOTTED 點綴 <br/>
	     * THICK 厚<br/>
	     * DOUBLE 雙 <br/>
	     * HAIR 毛髮<br/>
	     * MEDIUM_DASHED 中等虛線<br/>
	     * DASH_DOT 點劃線<br/>
	     * MEDIUM_DASH_DOT 中等點劃線<br/>
	     * DASH_DOT_DOT 點點劃線<br/>
	     * MEDIUM_DASH_DOT_DOT 中等點點劃線<br/>
	     * SLANTED_DASH_DOT 斜沖點<br/>
	     * @param borderStyle
	     * @return
	     */

        private BorderLineStyle getBorderLineStyle(string borderStyle)
        {
            if (string.Equals(borderStyle, "NONE", StringComparison.OrdinalIgnoreCase))
            {
                return BorderLineStyle.NONE;
            }
            if (string.Equals(borderStyle, "THIN", StringComparison.OrdinalIgnoreCase))
            {
                return BorderLineStyle.THIN;
            }
            if (string.Equals(borderStyle, "MEDIUM", StringComparison.OrdinalIgnoreCase))
            {
                return BorderLineStyle.MEDIUM;
            }
            if (string.Equals(borderStyle, "DASHED", StringComparison.OrdinalIgnoreCase))
            {
                return BorderLineStyle.DASHED;
            }
            if (string.Equals(borderStyle, "THICK", StringComparison.OrdinalIgnoreCase))
            {
                return BorderLineStyle.THICK;
            }
            if (string.Equals(borderStyle, "DOUBLE", StringComparison.OrdinalIgnoreCase))
            {
                return BorderLineStyle.DOUBLE;
            }
            if (string.Equals(borderStyle, "HAIR", StringComparison.OrdinalIgnoreCase))
            {
                return BorderLineStyle.HAIR;
            }
            if (string.Equals(borderStyle, "MEDIUM_DASHED", StringComparison.OrdinalIgnoreCase))
            {
                return BorderLineStyle.MEDIUM_DASHED;
            }
            if (string.Equals(borderStyle, "DASH_DOT", StringComparison.OrdinalIgnoreCase))
            {
                return BorderLineStyle.DASH_DOT;
            }
            if (string.Equals(borderStyle, "MEDIUM_DASH_DOT", StringComparison.OrdinalIgnoreCase))
            {
                return BorderLineStyle.MEDIUM_DASH_DOT;
            }
            if (string.Equals(borderStyle, "DASH_DOT_DOT", StringComparison.OrdinalIgnoreCase))
            {
                return BorderLineStyle.DASH_DOT_DOT;
            }
            if (string.Equals(borderStyle, "MEDIUM_DASH_DOT_DOT", StringComparison.OrdinalIgnoreCase))
            {
                return BorderLineStyle.MEDIUM_DASH_DOT_DOT;
            }
            if (string.Equals(borderStyle, "DASH_DOT_DOT", StringComparison.OrdinalIgnoreCase))
            {
                return BorderLineStyle.DASH_DOT_DOT;
            }
            if (string.Equals(borderStyle, "MEDIUM_DASH_DOT_DOT", StringComparison.OrdinalIgnoreCase))
            {
                return BorderLineStyle.MEDIUM_DASH_DOT_DOT;
            }
            if (string.Equals(borderStyle, "SLANTED_DASH_DOT", StringComparison.OrdinalIgnoreCase))
            {
                return BorderLineStyle.SLANTED_DASH_DOT;
            }
            return null;
        }
    }
}