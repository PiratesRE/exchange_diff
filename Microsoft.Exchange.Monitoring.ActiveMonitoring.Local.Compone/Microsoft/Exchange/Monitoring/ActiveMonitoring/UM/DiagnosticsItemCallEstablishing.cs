using System;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.UM
{
	internal class DiagnosticsItemCallEstablishing : DiagnosticsItemWithServiceBase
	{
		internal static bool IsExpectedErrorId(int errorid)
		{
			return errorid == 15902;
		}
	}
}
