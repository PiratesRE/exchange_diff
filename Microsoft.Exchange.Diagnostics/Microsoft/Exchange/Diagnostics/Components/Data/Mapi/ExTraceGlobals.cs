using System;

namespace Microsoft.Exchange.Diagnostics.Components.Data.Mapi
{
	public static class ExTraceGlobals
	{
		public static Trace MapiSessionTracer
		{
			get
			{
				if (ExTraceGlobals.mapiSessionTracer == null)
				{
					ExTraceGlobals.mapiSessionTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.mapiSessionTracer;
			}
		}

		public static Trace MapiObjectTracer
		{
			get
			{
				if (ExTraceGlobals.mapiObjectTracer == null)
				{
					ExTraceGlobals.mapiObjectTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.mapiObjectTracer;
			}
		}

		public static Trace PropertyBagTracer
		{
			get
			{
				if (ExTraceGlobals.propertyBagTracer == null)
				{
					ExTraceGlobals.propertyBagTracer = new Trace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.propertyBagTracer;
			}
		}

		public static Trace MessageStoreTracer
		{
			get
			{
				if (ExTraceGlobals.messageStoreTracer == null)
				{
					ExTraceGlobals.messageStoreTracer = new Trace(ExTraceGlobals.componentGuid, 3);
				}
				return ExTraceGlobals.messageStoreTracer;
			}
		}

		public static Trace FolderTracer
		{
			get
			{
				if (ExTraceGlobals.folderTracer == null)
				{
					ExTraceGlobals.folderTracer = new Trace(ExTraceGlobals.componentGuid, 4);
				}
				return ExTraceGlobals.folderTracer;
			}
		}

		public static Trace LogonStatisticsTracer
		{
			get
			{
				if (ExTraceGlobals.logonStatisticsTracer == null)
				{
					ExTraceGlobals.logonStatisticsTracer = new Trace(ExTraceGlobals.componentGuid, 5);
				}
				return ExTraceGlobals.logonStatisticsTracer;
			}
		}

		public static Trace ConvertorTracer
		{
			get
			{
				if (ExTraceGlobals.convertorTracer == null)
				{
					ExTraceGlobals.convertorTracer = new Trace(ExTraceGlobals.componentGuid, 6);
				}
				return ExTraceGlobals.convertorTracer;
			}
		}

		private static Guid componentGuid = new Guid("C9AAFFBB-C5D9-4e08-B398-7733BC04D45E");

		private static Trace mapiSessionTracer = null;

		private static Trace mapiObjectTracer = null;

		private static Trace propertyBagTracer = null;

		private static Trace messageStoreTracer = null;

		private static Trace folderTracer = null;

		private static Trace logonStatisticsTracer = null;

		private static Trace convertorTracer = null;
	}
}
