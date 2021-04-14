using System;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.UM
{
	internal class DiagnosticsItemCallAudioReceived : DiagnosticsItemWithServiceBase
	{
		internal static bool IsExpectedErrorId(int errorid)
		{
			return errorid == 15906;
		}
	}
}
