using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct Cluster_DagMgmtTags
	{
		public const int Service = 0;

		public const int DatabaseHealthTracker = 1;

		public const int MonitoringWcfService = 2;

		public const int MonitoringWcfClient = 3;

		public static Guid guid = new Guid("3ce77de7-c264-4d85-96f6-d0c3b66d4a4b");
	}
}
