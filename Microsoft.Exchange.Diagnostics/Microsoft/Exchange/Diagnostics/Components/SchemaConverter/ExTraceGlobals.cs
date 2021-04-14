using System;

namespace Microsoft.Exchange.Diagnostics.Components.SchemaConverter
{
	public static class ExTraceGlobals
	{
		public static Trace SchemaStateTracer
		{
			get
			{
				if (ExTraceGlobals.schemaStateTracer == null)
				{
					ExTraceGlobals.schemaStateTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.schemaStateTracer;
			}
		}

		public static Trace CommonTracer
		{
			get
			{
				if (ExTraceGlobals.commonTracer == null)
				{
					ExTraceGlobals.commonTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.commonTracer;
			}
		}

		public static Trace XsoTracer
		{
			get
			{
				if (ExTraceGlobals.xsoTracer == null)
				{
					ExTraceGlobals.xsoTracer = new Trace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.xsoTracer;
			}
		}

		public static Trace AirSyncTracer
		{
			get
			{
				if (ExTraceGlobals.airSyncTracer == null)
				{
					ExTraceGlobals.airSyncTracer = new Trace(ExTraceGlobals.componentGuid, 3);
				}
				return ExTraceGlobals.airSyncTracer;
			}
		}

		public static Trace ProtocolTracer
		{
			get
			{
				if (ExTraceGlobals.protocolTracer == null)
				{
					ExTraceGlobals.protocolTracer = new Trace(ExTraceGlobals.componentGuid, 4);
				}
				return ExTraceGlobals.protocolTracer;
			}
		}

		public static Trace ConversionTracer
		{
			get
			{
				if (ExTraceGlobals.conversionTracer == null)
				{
					ExTraceGlobals.conversionTracer = new Trace(ExTraceGlobals.componentGuid, 5);
				}
				return ExTraceGlobals.conversionTracer;
			}
		}

		public static Trace MethodEnterExitTracer
		{
			get
			{
				if (ExTraceGlobals.methodEnterExitTracer == null)
				{
					ExTraceGlobals.methodEnterExitTracer = new Trace(ExTraceGlobals.componentGuid, 6);
				}
				return ExTraceGlobals.methodEnterExitTracer;
			}
		}

		private static Guid componentGuid = new Guid("{7569BC27-E1CA-11D9-88B7-000D9DFFC66E}");

		private static Trace schemaStateTracer = null;

		private static Trace commonTracer = null;

		private static Trace xsoTracer = null;

		private static Trace airSyncTracer = null;

		private static Trace protocolTracer = null;

		private static Trace conversionTracer = null;

		private static Trace methodEnterExitTracer = null;
	}
}
