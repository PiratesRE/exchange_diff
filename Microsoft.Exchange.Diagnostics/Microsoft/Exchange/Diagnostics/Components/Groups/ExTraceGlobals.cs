using System;

namespace Microsoft.Exchange.Diagnostics.Components.Groups
{
	public static class ExTraceGlobals
	{
		public static Trace GroupNotificationStorageTracer
		{
			get
			{
				if (ExTraceGlobals.groupNotificationStorageTracer == null)
				{
					ExTraceGlobals.groupNotificationStorageTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.groupNotificationStorageTracer;
			}
		}

		public static Trace UnseenItemsReaderTracer
		{
			get
			{
				if (ExTraceGlobals.unseenItemsReaderTracer == null)
				{
					ExTraceGlobals.unseenItemsReaderTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.unseenItemsReaderTracer;
			}
		}

		public static Trace COWGroupMessageEscalationTracer
		{
			get
			{
				if (ExTraceGlobals.cOWGroupMessageEscalationTracer == null)
				{
					ExTraceGlobals.cOWGroupMessageEscalationTracer = new Trace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.cOWGroupMessageEscalationTracer;
			}
		}

		public static Trace COWGroupMessageWSPublishingTracer
		{
			get
			{
				if (ExTraceGlobals.cOWGroupMessageWSPublishingTracer == null)
				{
					ExTraceGlobals.cOWGroupMessageWSPublishingTracer = new Trace(ExTraceGlobals.componentGuid, 3);
				}
				return ExTraceGlobals.cOWGroupMessageWSPublishingTracer;
			}
		}

		private static Guid componentGuid = new Guid("1E4EC963-CD8B-4D26-A28B-832E3EA645CA");

		private static Trace groupNotificationStorageTracer = null;

		private static Trace unseenItemsReaderTracer = null;

		private static Trace cOWGroupMessageEscalationTracer = null;

		private static Trace cOWGroupMessageWSPublishingTracer = null;
	}
}
