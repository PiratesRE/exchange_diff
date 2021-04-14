using System;

namespace Microsoft.Exchange.Diagnostics.Components.ProtocolAnalysis
{
	public static class ExTraceGlobals
	{
		public static Trace FactoryTracer
		{
			get
			{
				if (ExTraceGlobals.factoryTracer == null)
				{
					ExTraceGlobals.factoryTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.factoryTracer;
			}
		}

		public static Trace DatabaseTracer
		{
			get
			{
				if (ExTraceGlobals.databaseTracer == null)
				{
					ExTraceGlobals.databaseTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.databaseTracer;
			}
		}

		public static Trace CalculateSrlTracer
		{
			get
			{
				if (ExTraceGlobals.calculateSrlTracer == null)
				{
					ExTraceGlobals.calculateSrlTracer = new Trace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.calculateSrlTracer;
			}
		}

		public static Trace OnMailFromTracer
		{
			get
			{
				if (ExTraceGlobals.onMailFromTracer == null)
				{
					ExTraceGlobals.onMailFromTracer = new Trace(ExTraceGlobals.componentGuid, 3);
				}
				return ExTraceGlobals.onMailFromTracer;
			}
		}

		public static Trace OnRcptToTracer
		{
			get
			{
				if (ExTraceGlobals.onRcptToTracer == null)
				{
					ExTraceGlobals.onRcptToTracer = new Trace(ExTraceGlobals.componentGuid, 4);
				}
				return ExTraceGlobals.onRcptToTracer;
			}
		}

		public static Trace OnEODTracer
		{
			get
			{
				if (ExTraceGlobals.onEODTracer == null)
				{
					ExTraceGlobals.onEODTracer = new Trace(ExTraceGlobals.componentGuid, 5);
				}
				return ExTraceGlobals.onEODTracer;
			}
		}

		public static Trace RejectTracer
		{
			get
			{
				if (ExTraceGlobals.rejectTracer == null)
				{
					ExTraceGlobals.rejectTracer = new Trace(ExTraceGlobals.componentGuid, 6);
				}
				return ExTraceGlobals.rejectTracer;
			}
		}

		public static Trace DisconnectTracer
		{
			get
			{
				if (ExTraceGlobals.disconnectTracer == null)
				{
					ExTraceGlobals.disconnectTracer = new Trace(ExTraceGlobals.componentGuid, 7);
				}
				return ExTraceGlobals.disconnectTracer;
			}
		}

		private static Guid componentGuid = new Guid("A0F3DC2A-7FD4-491E-C176-4857EAF2D7EF");

		private static Trace factoryTracer = null;

		private static Trace databaseTracer = null;

		private static Trace calculateSrlTracer = null;

		private static Trace onMailFromTracer = null;

		private static Trace onRcptToTracer = null;

		private static Trace onEODTracer = null;

		private static Trace rejectTracer = null;

		private static Trace disconnectTracer = null;
	}
}
