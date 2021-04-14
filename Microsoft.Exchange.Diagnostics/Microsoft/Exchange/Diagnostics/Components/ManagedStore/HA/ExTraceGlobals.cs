using System;
using Microsoft.Exchange.Diagnostics.FaultInjection;

namespace Microsoft.Exchange.Diagnostics.Components.ManagedStore.HA
{
	public static class ExTraceGlobals
	{
		public static Trace EsebackTracer
		{
			get
			{
				if (ExTraceGlobals.esebackTracer == null)
				{
					ExTraceGlobals.esebackTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.esebackTracer;
			}
		}

		public static Trace LogReplayStatusTracer
		{
			get
			{
				if (ExTraceGlobals.logReplayStatusTracer == null)
				{
					ExTraceGlobals.logReplayStatusTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.logReplayStatusTracer;
			}
		}

		public static Trace BlockModeCollectorTracer
		{
			get
			{
				if (ExTraceGlobals.blockModeCollectorTracer == null)
				{
					ExTraceGlobals.blockModeCollectorTracer = new Trace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.blockModeCollectorTracer;
			}
		}

		public static Trace BlockModeMessageStreamTracer
		{
			get
			{
				if (ExTraceGlobals.blockModeMessageStreamTracer == null)
				{
					ExTraceGlobals.blockModeMessageStreamTracer = new Trace(ExTraceGlobals.componentGuid, 3);
				}
				return ExTraceGlobals.blockModeMessageStreamTracer;
			}
		}

		public static Trace BlockModeSenderTracer
		{
			get
			{
				if (ExTraceGlobals.blockModeSenderTracer == null)
				{
					ExTraceGlobals.blockModeSenderTracer = new Trace(ExTraceGlobals.componentGuid, 4);
				}
				return ExTraceGlobals.blockModeSenderTracer;
			}
		}

		public static Trace JetHADatabaseTracer
		{
			get
			{
				if (ExTraceGlobals.jetHADatabaseTracer == null)
				{
					ExTraceGlobals.jetHADatabaseTracer = new Trace(ExTraceGlobals.componentGuid, 5);
				}
				return ExTraceGlobals.jetHADatabaseTracer;
			}
		}

		public static Trace LastLogWriterTracer
		{
			get
			{
				if (ExTraceGlobals.lastLogWriterTracer == null)
				{
					ExTraceGlobals.lastLogWriterTracer = new Trace(ExTraceGlobals.componentGuid, 6);
				}
				return ExTraceGlobals.lastLogWriterTracer;
			}
		}

		public static FaultInjectionTrace FaultInjectionTracer
		{
			get
			{
				if (ExTraceGlobals.faultInjectionTracer == null)
				{
					ExTraceGlobals.faultInjectionTracer = new FaultInjectionTrace(ExTraceGlobals.componentGuid, 20);
				}
				return ExTraceGlobals.faultInjectionTracer;
			}
		}

		private static Guid componentGuid = new Guid("0081576A-CB7C-4aba-9BB4-7D0B44290C2C");

		private static Trace esebackTracer = null;

		private static Trace logReplayStatusTracer = null;

		private static Trace blockModeCollectorTracer = null;

		private static Trace blockModeMessageStreamTracer = null;

		private static Trace blockModeSenderTracer = null;

		private static Trace jetHADatabaseTracer = null;

		private static Trace lastLogWriterTracer = null;

		private static FaultInjectionTrace faultInjectionTracer = null;
	}
}
