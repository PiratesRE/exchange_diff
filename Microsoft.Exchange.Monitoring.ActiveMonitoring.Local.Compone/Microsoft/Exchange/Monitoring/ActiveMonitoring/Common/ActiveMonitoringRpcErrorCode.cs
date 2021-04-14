using System;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Common
{
	internal static class ActiveMonitoringRpcErrorCode
	{
		public const int Success = 0;

		public const int CafeServerOverloaded = -2147417343;

		public const int CafeServerNotOwner = -2147417342;

		public const int DatabaseCopyNotActive = -2147417341;

		public const int ServerVersionNotMatch = -2147417340;

		public const int DatabaseBlacklisted = -2147417339;

		public const int MonitoringStateOff = -2147417338;

		public const int WLIDException = -2147417337;

		public const int OtherManagedException = -2147417336;

		public const int InvalidArgument = -2147024809;
	}
}
