using System;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public static class ObjectExtension
	{
		public static string StringArrayJoin(this object stringsToJoin, string separator)
		{
			if (stringsToJoin is string[])
			{
				return string.Join(separator, (string[])stringsToJoin);
			}
			return null;
		}

		public static string ToStringWithNull(this object toStringObject)
		{
			if (toStringObject != null)
			{
				return toStringObject.ToString();
			}
			return null;
		}
	}
}
