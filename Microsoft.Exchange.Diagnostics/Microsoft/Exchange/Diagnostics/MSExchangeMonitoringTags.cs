using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct MSExchangeMonitoringTags
	{
		public const int MonitoringService = 0;

		public const int MonitoringRpcServer = 1;

		public const int MonitoringTasks = 2;

		public const int MonitoringHelper = 3;

		public const int MonitoringData = 4;

		public const int CorrelationEngine = 5;

		public const int FaultInjection = 6;

		public static Guid guid = new Guid("170506F7-64BA-4C74-A2A3-0A5CC247DB58");
	}
}
