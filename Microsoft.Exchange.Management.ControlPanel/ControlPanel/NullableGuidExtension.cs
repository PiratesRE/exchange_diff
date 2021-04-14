using System;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public static class NullableGuidExtension
	{
		public static string FormatForLog(this Guid? guid)
		{
			if (guid == null)
			{
				return string.Empty;
			}
			return guid.Value.ToString();
		}
	}
}
