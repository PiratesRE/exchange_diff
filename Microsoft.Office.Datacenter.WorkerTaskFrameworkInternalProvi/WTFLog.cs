using System;

namespace Microsoft.Office.Datacenter.WorkerTaskFramework
{
	public static class WTFLog
	{
		public static WTFLogComponent Core
		{
			get
			{
				return WTFLog.coreLogger;
			}
		}

		public static WTFLogComponent DataAccess
		{
			get
			{
				return WTFLog.dataAccessLogger;
			}
		}

		public static WTFLogComponent WorkItem
		{
			get
			{
				return WTFLog.workItemLogger;
			}
		}

		public static WTFLogComponent ManagedAvailability
		{
			get
			{
				return WTFLog.managedAvailabilityLogger;
			}
		}

		private static readonly Guid componentGuid = new Guid("EAF36C57-87B9-4D84-B551-3537A14A62B8");

		private static readonly WTFLogComponent coreLogger = new WTFLogComponent(WTFLog.componentGuid, 1, "Core", true);

		private static readonly WTFLogComponent dataAccessLogger = new WTFLogComponent(WTFLog.componentGuid, 2, "DataAccess", true);

		private static readonly WTFLogComponent workItemLogger = new WTFLogComponent(WTFLog.componentGuid, 3, "WorkItem", true);

		private static readonly WTFLogComponent managedAvailabilityLogger = new WTFLogComponent(WTFLog.componentGuid, 4, "ManagedAvailability", true);
	}
}
