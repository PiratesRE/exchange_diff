using System;
using Microsoft.Exchange.Diagnostics.FaultInjection;

namespace Microsoft.Exchange.Diagnostics.Components.AirSync
{
	public static class ExTraceGlobals
	{
		public static Trace RequestsTracer
		{
			get
			{
				if (ExTraceGlobals.requestsTracer == null)
				{
					ExTraceGlobals.requestsTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.requestsTracer;
			}
		}

		public static Trace WbxmlTracer
		{
			get
			{
				if (ExTraceGlobals.wbxmlTracer == null)
				{
					ExTraceGlobals.wbxmlTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.wbxmlTracer;
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

		public static Trace AlgorithmTracer
		{
			get
			{
				if (ExTraceGlobals.algorithmTracer == null)
				{
					ExTraceGlobals.algorithmTracer = new Trace(ExTraceGlobals.componentGuid, 3);
				}
				return ExTraceGlobals.algorithmTracer;
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

		public static Trace ThreadPoolTracer
		{
			get
			{
				if (ExTraceGlobals.threadPoolTracer == null)
				{
					ExTraceGlobals.threadPoolTracer = new Trace(ExTraceGlobals.componentGuid, 6);
				}
				return ExTraceGlobals.threadPoolTracer;
			}
		}

		public static Trace RawBodyBytesTracer
		{
			get
			{
				if (ExTraceGlobals.rawBodyBytesTracer == null)
				{
					ExTraceGlobals.rawBodyBytesTracer = new Trace(ExTraceGlobals.componentGuid, 7);
				}
				return ExTraceGlobals.rawBodyBytesTracer;
			}
		}

		public static Trace MethodEnterExitTracer
		{
			get
			{
				if (ExTraceGlobals.methodEnterExitTracer == null)
				{
					ExTraceGlobals.methodEnterExitTracer = new Trace(ExTraceGlobals.componentGuid, 8);
				}
				return ExTraceGlobals.methodEnterExitTracer;
			}
		}

		public static Trace TiUpgradeTracer
		{
			get
			{
				if (ExTraceGlobals.tiUpgradeTracer == null)
				{
					ExTraceGlobals.tiUpgradeTracer = new Trace(ExTraceGlobals.componentGuid, 9);
				}
				return ExTraceGlobals.tiUpgradeTracer;
			}
		}

		public static Trace ValidationTracer
		{
			get
			{
				if (ExTraceGlobals.validationTracer == null)
				{
					ExTraceGlobals.validationTracer = new Trace(ExTraceGlobals.componentGuid, 10);
				}
				return ExTraceGlobals.validationTracer;
			}
		}

		public static Trace PfdInitTraceTracer
		{
			get
			{
				if (ExTraceGlobals.pfdInitTraceTracer == null)
				{
					ExTraceGlobals.pfdInitTraceTracer = new Trace(ExTraceGlobals.componentGuid, 11);
				}
				return ExTraceGlobals.pfdInitTraceTracer;
			}
		}

		public static Trace CorruptItemTracer
		{
			get
			{
				if (ExTraceGlobals.corruptItemTracer == null)
				{
					ExTraceGlobals.corruptItemTracer = new Trace(ExTraceGlobals.componentGuid, 12);
				}
				return ExTraceGlobals.corruptItemTracer;
			}
		}

		public static Trace ThreadingTracer
		{
			get
			{
				if (ExTraceGlobals.threadingTracer == null)
				{
					ExTraceGlobals.threadingTracer = new Trace(ExTraceGlobals.componentGuid, 13);
				}
				return ExTraceGlobals.threadingTracer;
			}
		}

		public static FaultInjectionTrace FaultInjectionTracer
		{
			get
			{
				if (ExTraceGlobals.faultInjectionTracer == null)
				{
					ExTraceGlobals.faultInjectionTracer = new FaultInjectionTrace(ExTraceGlobals.componentGuid, 14);
				}
				return ExTraceGlobals.faultInjectionTracer;
			}
		}

		public static Trace BodyTracer
		{
			get
			{
				if (ExTraceGlobals.bodyTracer == null)
				{
					ExTraceGlobals.bodyTracer = new Trace(ExTraceGlobals.componentGuid, 15);
				}
				return ExTraceGlobals.bodyTracer;
			}
		}

		public static Trace DiagnosticsTracer
		{
			get
			{
				if (ExTraceGlobals.diagnosticsTracer == null)
				{
					ExTraceGlobals.diagnosticsTracer = new Trace(ExTraceGlobals.componentGuid, 16);
				}
				return ExTraceGlobals.diagnosticsTracer;
			}
		}

		private static Guid componentGuid = new Guid("5e88fb2c-0a36-41f2-a710-c911bfe18e44");

		private static Trace requestsTracer = null;

		private static Trace wbxmlTracer = null;

		private static Trace xsoTracer = null;

		private static Trace algorithmTracer = null;

		private static Trace protocolTracer = null;

		private static Trace conversionTracer = null;

		private static Trace threadPoolTracer = null;

		private static Trace rawBodyBytesTracer = null;

		private static Trace methodEnterExitTracer = null;

		private static Trace tiUpgradeTracer = null;

		private static Trace validationTracer = null;

		private static Trace pfdInitTraceTracer = null;

		private static Trace corruptItemTracer = null;

		private static Trace threadingTracer = null;

		private static FaultInjectionTrace faultInjectionTracer = null;

		private static Trace bodyTracer = null;

		private static Trace diagnosticsTracer = null;
	}
}
