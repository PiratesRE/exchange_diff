using System;
using System.Web.UI.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public static class UnitTypeExtension
	{
		public static string ToJavaScript(this UnitType value)
		{
			if (value != UnitType.Pixel)
			{
				return "%";
			}
			return "px";
		}
	}
}
