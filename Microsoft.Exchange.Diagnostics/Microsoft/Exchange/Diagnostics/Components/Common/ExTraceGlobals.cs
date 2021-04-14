using System;
using Microsoft.Exchange.Diagnostics.FaultInjection;

namespace Microsoft.Exchange.Diagnostics.Components.Common
{
	public static class ExTraceGlobals
	{
		public static Trace CommonTracer
		{
			get
			{
				if (ExTraceGlobals.commonTracer == null)
				{
					ExTraceGlobals.commonTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.commonTracer;
			}
		}

		public static Trace EventLogTracer
		{
			get
			{
				if (ExTraceGlobals.eventLogTracer == null)
				{
					ExTraceGlobals.eventLogTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.eventLogTracer;
			}
		}

		public static Trace ScheduleIntervalTracer
		{
			get
			{
				if (ExTraceGlobals.scheduleIntervalTracer == null)
				{
					ExTraceGlobals.scheduleIntervalTracer = new Trace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.scheduleIntervalTracer;
			}
		}

		public static Trace CertificateValidationTracer
		{
			get
			{
				if (ExTraceGlobals.certificateValidationTracer == null)
				{
					ExTraceGlobals.certificateValidationTracer = new Trace(ExTraceGlobals.componentGuid, 3);
				}
				return ExTraceGlobals.certificateValidationTracer;
			}
		}

		public static Trace AuthorizationTracer
		{
			get
			{
				if (ExTraceGlobals.authorizationTracer == null)
				{
					ExTraceGlobals.authorizationTracer = new Trace(ExTraceGlobals.componentGuid, 4);
				}
				return ExTraceGlobals.authorizationTracer;
			}
		}

		public static Trace TracingTracer
		{
			get
			{
				if (ExTraceGlobals.tracingTracer == null)
				{
					ExTraceGlobals.tracingTracer = new Trace(ExTraceGlobals.componentGuid, 5);
				}
				return ExTraceGlobals.tracingTracer;
			}
		}

		public static FaultInjectionTrace FaultInjectionTracer
		{
			get
			{
				if (ExTraceGlobals.faultInjectionTracer == null)
				{
					ExTraceGlobals.faultInjectionTracer = new FaultInjectionTrace(ExTraceGlobals.componentGuid, 6);
				}
				return ExTraceGlobals.faultInjectionTracer;
			}
		}

		public static Trace RpcTracer
		{
			get
			{
				if (ExTraceGlobals.rpcTracer == null)
				{
					ExTraceGlobals.rpcTracer = new Trace(ExTraceGlobals.componentGuid, 7);
				}
				return ExTraceGlobals.rpcTracer;
			}
		}

		public static Trace SqmTracer
		{
			get
			{
				if (ExTraceGlobals.sqmTracer == null)
				{
					ExTraceGlobals.sqmTracer = new Trace(ExTraceGlobals.componentGuid, 8);
				}
				return ExTraceGlobals.sqmTracer;
			}
		}

		public static Trace TracingConfigurationTracer
		{
			get
			{
				if (ExTraceGlobals.tracingConfigurationTracer == null)
				{
					ExTraceGlobals.tracingConfigurationTracer = new Trace(ExTraceGlobals.componentGuid, 9);
				}
				return ExTraceGlobals.tracingConfigurationTracer;
			}
		}

		public static Trace FaultInjectionConfigurationTracer
		{
			get
			{
				if (ExTraceGlobals.faultInjectionConfigurationTracer == null)
				{
					ExTraceGlobals.faultInjectionConfigurationTracer = new Trace(ExTraceGlobals.componentGuid, 10);
				}
				return ExTraceGlobals.faultInjectionConfigurationTracer;
			}
		}

		public static Trace AppConfigLoaderTracer
		{
			get
			{
				if (ExTraceGlobals.appConfigLoaderTracer == null)
				{
					ExTraceGlobals.appConfigLoaderTracer = new Trace(ExTraceGlobals.componentGuid, 11);
				}
				return ExTraceGlobals.appConfigLoaderTracer;
			}
		}

		public static Trace WebHealthTracer
		{
			get
			{
				if (ExTraceGlobals.webHealthTracer == null)
				{
					ExTraceGlobals.webHealthTracer = new Trace(ExTraceGlobals.componentGuid, 12);
				}
				return ExTraceGlobals.webHealthTracer;
			}
		}

		public static Trace VariantConfigurationTracer
		{
			get
			{
				if (ExTraceGlobals.variantConfigurationTracer == null)
				{
					ExTraceGlobals.variantConfigurationTracer = new Trace(ExTraceGlobals.componentGuid, 13);
				}
				return ExTraceGlobals.variantConfigurationTracer;
			}
		}

		public static Trace ClientAccessRulesTracer
		{
			get
			{
				if (ExTraceGlobals.clientAccessRulesTracer == null)
				{
					ExTraceGlobals.clientAccessRulesTracer = new Trace(ExTraceGlobals.componentGuid, 14);
				}
				return ExTraceGlobals.clientAccessRulesTracer;
			}
		}

		public static Trace ConcurrencyGuardTracer
		{
			get
			{
				if (ExTraceGlobals.concurrencyGuardTracer == null)
				{
					ExTraceGlobals.concurrencyGuardTracer = new Trace(ExTraceGlobals.componentGuid, 15);
				}
				return ExTraceGlobals.concurrencyGuardTracer;
			}
		}

		private static Guid componentGuid = new Guid("5948f08f-9d8f-11da-9575-00e08161165f");

		private static Trace commonTracer = null;

		private static Trace eventLogTracer = null;

		private static Trace scheduleIntervalTracer = null;

		private static Trace certificateValidationTracer = null;

		private static Trace authorizationTracer = null;

		private static Trace tracingTracer = null;

		private static FaultInjectionTrace faultInjectionTracer = null;

		private static Trace rpcTracer = null;

		private static Trace sqmTracer = null;

		private static Trace tracingConfigurationTracer = null;

		private static Trace faultInjectionConfigurationTracer = null;

		private static Trace appConfigLoaderTracer = null;

		private static Trace webHealthTracer = null;

		private static Trace variantConfigurationTracer = null;

		private static Trace clientAccessRulesTracer = null;

		private static Trace concurrencyGuardTracer = null;
	}
}
