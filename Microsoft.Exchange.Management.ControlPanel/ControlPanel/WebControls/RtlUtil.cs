using System;
using System.Globalization;
using System.Threading;
using System.Web;
using System.Web.UI.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	public class RtlUtil
	{
		public static string DirectionMark
		{
			get
			{
				if (!RtlUtil.IsRtl)
				{
					return "&#x200E;";
				}
				return "&#x200F;";
			}
		}

		public static string SearchDefaultMock
		{
			get
			{
				if (!RtlUtil.IsRtl)
				{
					return "▶";
				}
				return "◀";
			}
		}

		public static string DecodedDirectionMark
		{
			get
			{
				if (!RtlUtil.IsRtl)
				{
					return RtlUtil.DecodedLtrDirectionMark;
				}
				return RtlUtil.DecodedRtlDirectionMark;
			}
		}

		public static bool IsRtl
		{
			get
			{
				CultureInfo currentUICulture = Thread.CurrentThread.CurrentUICulture;
				return currentUICulture != null && currentUICulture.TextInfo.IsRightToLeft;
			}
		}

		public static string ConvertToBidiString(string value, bool isRtl)
		{
			string newValue = isRtl ? RtlUtil.rdmWithLeftParenthesis : RtlUtil.ldmWithLeftParenthesis;
			string newValue2 = isRtl ? RtlUtil.rdmWithRightParenthesis : RtlUtil.ldmwithRightParenthesis;
			return value.Replace("(", newValue).Replace(")", newValue2);
		}

		public static string ConvertToDecodedBidiString(string value, bool isRtl)
		{
			string newValue = isRtl ? RtlUtil.decodedRDMWithLeftParenthesis : RtlUtil.decodedLDMWithLeftParenthesis;
			string newValue2 = isRtl ? RtlUtil.decodedRDMWithRightParenthesis : RtlUtil.decodedLDMwithRightParenthesis;
			return value.Replace("(", newValue).Replace(")", newValue2);
		}

		public static HorizontalAlign LeftAlign
		{
			get
			{
				if (!RtlUtil.IsRtl)
				{
					return HorizontalAlign.Left;
				}
				return HorizontalAlign.Right;
			}
		}

		public static HorizontalAlign RightAlign
		{
			get
			{
				if (!RtlUtil.IsRtl)
				{
					return HorizontalAlign.Right;
				}
				return HorizontalAlign.Left;
			}
		}

		public static HorizontalAlign GetHorizontalAlign(HorizontalAlign value)
		{
			HorizontalAlign result = value;
			switch (value)
			{
			case HorizontalAlign.Left:
				result = (RtlUtil.IsRtl ? HorizontalAlign.Right : HorizontalAlign.Left);
				break;
			case HorizontalAlign.Right:
				result = (RtlUtil.IsRtl ? HorizontalAlign.Left : HorizontalAlign.Right);
				break;
			}
			return result;
		}

		public const string LtrDirectionMark = "&#x200E;";

		public const string RtlDirectionMark = "&#x200F;";

		public static readonly string DecodedLtrDirectionMark = HttpUtility.HtmlDecode("&#x200E;");

		public static readonly string DecodedRtlDirectionMark = HttpUtility.HtmlDecode("&#x200F;");

		private static readonly string ldmWithLeftParenthesis = "&#x200E;(";

		private static readonly string ldmwithRightParenthesis = ")&#x200E;";

		private static readonly string rdmWithLeftParenthesis = "&#x200F;(";

		private static readonly string rdmWithRightParenthesis = ")&#x200F;";

		private static readonly string decodedLDMWithLeftParenthesis = RtlUtil.DecodedLtrDirectionMark + "(";

		private static readonly string decodedLDMwithRightParenthesis = ")" + RtlUtil.DecodedLtrDirectionMark;

		private static readonly string decodedRDMWithLeftParenthesis = RtlUtil.DecodedRtlDirectionMark + "(";

		private static readonly string decodedRDMWithRightParenthesis = ")" + RtlUtil.DecodedRtlDirectionMark;
	}
}
