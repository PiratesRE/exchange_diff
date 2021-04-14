using System;

namespace Microsoft.Exchange.Diagnostics.Components.Messenger
{
	public static class ExTraceGlobals
	{
		public static Trace CoreTracer
		{
			get
			{
				if (ExTraceGlobals.coreTracer == null)
				{
					ExTraceGlobals.coreTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.coreTracer;
			}
		}

		public static Trace MSNPTracer
		{
			get
			{
				if (ExTraceGlobals.mSNPTracer == null)
				{
					ExTraceGlobals.mSNPTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.mSNPTracer;
			}
		}

		public static Trace ABCHTracer
		{
			get
			{
				if (ExTraceGlobals.aBCHTracer == null)
				{
					ExTraceGlobals.aBCHTracer = new Trace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.aBCHTracer;
			}
		}

		public static Trace SharingTracer
		{
			get
			{
				if (ExTraceGlobals.sharingTracer == null)
				{
					ExTraceGlobals.sharingTracer = new Trace(ExTraceGlobals.componentGuid, 3);
				}
				return ExTraceGlobals.sharingTracer;
			}
		}

		public static Trace StorageTracer
		{
			get
			{
				if (ExTraceGlobals.storageTracer == null)
				{
					ExTraceGlobals.storageTracer = new Trace(ExTraceGlobals.componentGuid, 4);
				}
				return ExTraceGlobals.storageTracer;
			}
		}

		private static Guid componentGuid = new Guid("5099defc-8a21-405a-ba04-e0857dd8d94e");

		private static Trace coreTracer = null;

		private static Trace mSNPTracer = null;

		private static Trace aBCHTracer = null;

		private static Trace sharingTracer = null;

		private static Trace storageTracer = null;
	}
}
