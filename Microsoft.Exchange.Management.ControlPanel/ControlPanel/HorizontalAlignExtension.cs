using System;
using System.Web.UI.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public static class HorizontalAlignExtension
	{
		public static string ToJavaScript(this HorizontalAlign alignment)
		{
			string result = string.Empty;
			switch (alignment)
			{
			case HorizontalAlign.Left:
				result = "left";
				break;
			case HorizontalAlign.Center:
				result = "center";
				break;
			case HorizontalAlign.Right:
				result = "right";
				break;
			case HorizontalAlign.Justify:
				result = "justify";
				break;
			}
			return result;
		}
	}
}
