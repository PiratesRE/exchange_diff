using System;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.UM
{
	internal class DiagnosticsItemLyncServer : DiagnosticsItemBase
	{
		internal static bool IsExpectedErrorId(int errorid)
		{
			return errorid >= 15000 && errorid <= 15499;
		}
	}
}
