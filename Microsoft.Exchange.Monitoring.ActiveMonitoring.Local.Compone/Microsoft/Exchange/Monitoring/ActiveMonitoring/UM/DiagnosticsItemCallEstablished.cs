using System;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.UM
{
	internal class DiagnosticsItemCallEstablished : DiagnosticsItemWithServiceBase
	{
		internal static bool IsExpectedErrorId(int errorid)
		{
			return errorid == 15903;
		}
	}
}
