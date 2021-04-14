using System;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public static class BoolExtension
	{
		public static string ToJavaScript(this bool value)
		{
			if (!value)
			{
				return "false";
			}
			return "true";
		}
	}
}
