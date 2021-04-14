using System;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.UM
{
	internal class DiagnosticsItemCallReceived : DiagnosticsItemWithServiceBase
	{
		public DateTime ReceivedTime
		{
			get
			{
				DateTime minValue = DateTime.MinValue;
				DateTime.TryParse(base.GetValue("time"), out minValue);
				return minValue.ToUniversalTime();
			}
		}

		internal static bool IsExpectedErrorId(int errorid)
		{
			return errorid == 15638 || errorid == 15644;
		}
	}
}
