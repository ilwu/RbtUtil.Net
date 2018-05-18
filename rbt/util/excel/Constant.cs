using CSharpJExcel.Jxl.Format;
using CSharpJExcel.Jxl.Write;

namespace rbt.util.excel
{
    public class Constant
    {
        // =========================================================
        // ELEMENT
        // =========================================================

        public static readonly string ELEMENT_FUNCTION = "function";
        public static readonly string ELEMENT_FORMAT = "format";

        public static readonly string ELEMENT_EXCEL = "excel";
        public static readonly string ELEMENT_STYLE = "style";
        public static readonly string ELEMENT_SHEET = "sheet";
        public static readonly string ELEMENT_CONTEXT = "context";
        public static readonly string ELEMENT_TR = "tr";
        public static readonly string ELEMENT_TD = "td";
        public static readonly string ELEMENT_DETAIL = "detail";
        public static readonly string ELEMENT_COLUMN = "column";
        public static readonly string ELEMENT_ARRAY = "array";
        public static readonly string ELEMENT_SINGLE = "single";
        public static readonly string ELEMENT_READ = "read";
        public static readonly string ELEMENT_PARAMS = "params";
        public static readonly string ELEMENT_PARAM = "param";
        public static readonly string ELEMENT_DEFAULT_VALUE = "defaultValue";
        public static readonly string ELEMENT_FUNC_PARAM = "funcParam";

        // =========================================================
        // ATTRIBUTE
        // =========================================================

        public static readonly string ATTRIBUTE_FUNCID = "funcId";
        public static readonly string ATTRIBUTE_FORMATID = "formatId";
        public static readonly string ATTRIBUTE_CLASSNAME = "className";
        public static readonly string ATTRIBUTE_METHOD = "method";
        public static readonly string ATTRIBUTE_ID = "id";
        public static readonly string ATTRIBUTE_FILENAME = "fileName";
        public static readonly string ATTRIBUTE_SHEETNAME = "sheetName";
        public static readonly string ATTRIBUTE_PAPERSIZE = "paperSize";
        public static readonly string ATTRIBUTE_DATAID = "dataId";
        public static readonly string ATTRIBUTE_KEY = "key";
        public static readonly string ATTRIBUTE_FUNC_PARAM = "funcParam";
        public static readonly string ATTRIBUTE_ROWSPAN = "rowspan";
        public static readonly string ATTRIBUTE_COLSPAN = "colspan";
        public static readonly string ATTRIBUTE_WIDTH = "width";
        public static readonly string ATTRIBUTE_HEIGHT = "height";
        public static readonly string ATTRIBUTE_FONT = "font";
        public static readonly string ATTRIBUTE_SIZE = "size";
        public static readonly string ATTRIBUTE_BOLD = "bold";
        public static readonly string ATTRIBUTE_ITALIC = "italic";
        public static readonly string ATTRIBUTE_UNDERLINE = "underline";
        public static readonly string ATTRIBUTE_COLOR = "color";
        public static readonly string ATTRIBUTE_ALIGN = "align";
        public static readonly string ATTRIBUTE_VALIGN = "valign";
        public static readonly string ATTRIBUTE_WRAP = "wrap";
        public static readonly string ATTRIBUTE_BACKGROUND = "background";
        public static readonly string ATTRIBUTE_BORDERSIDE = "borderSide";
        public static readonly string ATTRIBUTE_BORDERSTYLE = "borderStyle";
        public static readonly string ATTRIBUTE_SHEETNUM = "sheetNum";
        public static readonly string ATTRIBUTE_STARTROW = "startRow";
        public static readonly string ATTRIBUTE_CHECK_EMPTY_ROW = "checkEmptyRow";
        public static readonly string ATTRIBUTE_CHECK_DUPLICATE = "checkDuplicate";
        public static readonly string ATTRIBUTE_DESC = "desc";
        public static readonly string ATTRIBUTE_REGEX = "regex";
        public static readonly string ATTRIBUTE_REGEX_ERROR_MSG = "regexErrorMsg";
        public static readonly string ATTRIBUTE_CHECK_NULL = "checkNull";
        public static readonly string ATTRIBUTE_PASS = "pass";
        public static readonly string ATTRIBUTE_INDEX = "index";
        public static readonly string ATTRIBUTE_DEFAULT_VALUE = "defaultValue";

        // ===========================================================================
        // EXPORT 參數預設值區
        // ===========================================================================
        /// <summary>
        /// 預設文字大小
        /// </summary>
        public static readonly int DEFAULT_FONT_SIZE = 13;

        /// <summary>
        /// 預設字體設定
        /// </summary>
        public static readonly WritableFont.FontName DEFAULT_FONT = WritableFont.createFont("標楷體");

        /// <summary>
        /// 預設粗體設定
        /// </summary>
        public static readonly string DEFAULT_BOLD = "false";

        /// <summary>
        /// 預設斜體設定
        /// </summary>
        public static readonly string DEFAULT_ITALIC = "false";

        /// <summary>
        /// 預設字底底線設定
        /// </summary>
        public static readonly UnderlineStyle DEFAULT_UNDERLINE_STYLE = UnderlineStyle.NO_UNDERLINE;

        /// <summary>
        /// 預設字體顏色設定
        /// </summary>
        public static readonly Colour DEFAULT_COLOR = Colour.BLACK;

        /// <summary>
        /// 預設欄寬
        /// </summary>
        public static readonly string DEFAULT_WIDTH = "16";

        /// <summary>
        /// 預設水平置中設定
        /// </summary>
        public static readonly Alignment DEFAULT_ALIGN = Alignment.CENTRE;

        /// <summary>
        /// 預設垂直置中設定
        /// </summary>
        public static readonly VerticalAlignment DEFAULT_VALIGN = VerticalAlignment.CENTRE;

        /// <summary>
        /// 預設文字換行設定
        /// </summary>
        public static readonly string DEFAULT_WRAP = "true";

        /// <summary>
        /// 預設邊線位置設定
        /// </summary>
        public static readonly Border DEFAULT_BORDER_SIDE = Border.ALL;

        /// <summary>
        /// 預設邊線樣式設定
        /// </summary>
        public static readonly BorderLineStyle DEFAULT_BORDER_STYLE = BorderLineStyle.THIN;

        /// <summary>
        /// 預設背景顏色設定
        /// </summary>
        public static readonly Colour DEFAULT_BACKGROUND = Colour.WHITE;
    }
}