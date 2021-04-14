using System;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.UM
{
	internal class DiagnosticsItemExchangeServer : DiagnosticsItemBase
	{
		internal static bool IsExpectedErrorId(int errorid)
		{
			return errorid >= 15500 && errorid <= 15899 && errorid != 15638 && errorid != 15644 && errorid != 15637 && errorid != 15643;
		}
	}
}
