using System;
using Microsoft.Exchange.Diagnostics.FaultInjection;

namespace Microsoft.Exchange.Diagnostics.Components.Compliance
{
	public static class ExTraceGlobals
	{
		public static Trace GeneralTracer
		{
			get
			{
				if (ExTraceGlobals.generalTracer == null)
				{
					ExTraceGlobals.generalTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.generalTracer;
			}
		}

		public static Trace ConfigurationTracer
		{
			get
			{
				if (ExTraceGlobals.configurationTracer == null)
				{
					ExTraceGlobals.configurationTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.configurationTracer;
			}
		}

		public static Trace ViewProviderTracer
		{
			get
			{
				if (ExTraceGlobals.viewProviderTracer == null)
				{
					ExTraceGlobals.viewProviderTracer = new Trace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.viewProviderTracer;
			}
		}

		public static Trace DataProviderTracer
		{
			get
			{
				if (ExTraceGlobals.dataProviderTracer == null)
				{
					ExTraceGlobals.dataProviderTracer = new Trace(ExTraceGlobals.componentGuid, 3);
				}
				return ExTraceGlobals.dataProviderTracer;
			}
		}

		public static Trace ViewTracer
		{
			get
			{
				if (ExTraceGlobals.viewTracer == null)
				{
					ExTraceGlobals.viewTracer = new Trace(ExTraceGlobals.componentGuid, 4);
				}
				return ExTraceGlobals.viewTracer;
			}
		}

		public static FaultInjectionTrace FaultInjectionTracer
		{
			get
			{
				if (ExTraceGlobals.faultInjectionTracer == null)
				{
					ExTraceGlobals.faultInjectionTracer = new FaultInjectionTrace(ExTraceGlobals.componentGuid, 5);
				}
				return ExTraceGlobals.faultInjectionTracer;
			}
		}

		public static Trace ComplianceServiceTracer
		{
			get
			{
				if (ExTraceGlobals.complianceServiceTracer == null)
				{
					ExTraceGlobals.complianceServiceTracer = new Trace(ExTraceGlobals.componentGuid, 6);
				}
				return ExTraceGlobals.complianceServiceTracer;
			}
		}

		public static Trace TaskDistributionSystemTracer
		{
			get
			{
				if (ExTraceGlobals.taskDistributionSystemTracer == null)
				{
					ExTraceGlobals.taskDistributionSystemTracer = new Trace(ExTraceGlobals.componentGuid, 7);
				}
				return ExTraceGlobals.taskDistributionSystemTracer;
			}
		}

		private static Guid componentGuid = new Guid("3719A9EF-E0BD-45DF-9B58-B36C0C2ECF0E");

		private static Trace generalTracer = null;

		private static Trace configurationTracer = null;

		private static Trace viewProviderTracer = null;

		private static Trace dataProviderTracer = null;

		private static Trace viewTracer = null;

		private static FaultInjectionTrace faultInjectionTracer = null;

		private static Trace complianceServiceTracer = null;

		private static Trace taskDistributionSystemTracer = null;
	}
}
