using System;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.UM
{
	internal class DiagnosticsItemUnknown : DiagnosticsItemBase
	{
		internal static bool IsExpectedErrorId(int errorid)
		{
			return false;
		}
	}
}
