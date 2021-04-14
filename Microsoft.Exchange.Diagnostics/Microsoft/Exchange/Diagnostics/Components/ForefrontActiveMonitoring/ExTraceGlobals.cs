using System;
using Microsoft.Exchange.Diagnostics.FaultInjection;

namespace Microsoft.Exchange.Diagnostics.Components.ForefrontActiveMonitoring
{
	public static class ExTraceGlobals
	{
		public static FaultInjectionTrace FaultInjectionTracer
		{
			get
			{
				if (ExTraceGlobals.faultInjectionTracer == null)
				{
					ExTraceGlobals.faultInjectionTracer = new FaultInjectionTrace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.faultInjectionTracer;
			}
		}

		public static Trace SMTPTracer
		{
			get
			{
				if (ExTraceGlobals.sMTPTracer == null)
				{
					ExTraceGlobals.sMTPTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.sMTPTracer;
			}
		}

		public static Trace SMTPConnectionTracer
		{
			get
			{
				if (ExTraceGlobals.sMTPConnectionTracer == null)
				{
					ExTraceGlobals.sMTPConnectionTracer = new Trace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.sMTPConnectionTracer;
			}
		}

		public static Trace SMTPMonitorTracer
		{
			get
			{
				if (ExTraceGlobals.sMTPMonitorTracer == null)
				{
					ExTraceGlobals.sMTPMonitorTracer = new Trace(ExTraceGlobals.componentGuid, 3);
				}
				return ExTraceGlobals.sMTPMonitorTracer;
			}
		}

		public static Trace WebServiceTracer
		{
			get
			{
				if (ExTraceGlobals.webServiceTracer == null)
				{
					ExTraceGlobals.webServiceTracer = new Trace(ExTraceGlobals.componentGuid, 4);
				}
				return ExTraceGlobals.webServiceTracer;
			}
		}

		public static Trace HTTPTracer
		{
			get
			{
				if (ExTraceGlobals.hTTPTracer == null)
				{
					ExTraceGlobals.hTTPTracer = new Trace(ExTraceGlobals.componentGuid, 5);
				}
				return ExTraceGlobals.hTTPTracer;
			}
		}

		public static Trace ResponderTracer
		{
			get
			{
				if (ExTraceGlobals.responderTracer == null)
				{
					ExTraceGlobals.responderTracer = new Trace(ExTraceGlobals.componentGuid, 6);
				}
				return ExTraceGlobals.responderTracer;
			}
		}

		public static Trace DNSTracer
		{
			get
			{
				if (ExTraceGlobals.dNSTracer == null)
				{
					ExTraceGlobals.dNSTracer = new Trace(ExTraceGlobals.componentGuid, 7);
				}
				return ExTraceGlobals.dNSTracer;
			}
		}

		public static Trace AntiSpamTracer
		{
			get
			{
				if (ExTraceGlobals.antiSpamTracer == null)
				{
					ExTraceGlobals.antiSpamTracer = new Trace(ExTraceGlobals.componentGuid, 8);
				}
				return ExTraceGlobals.antiSpamTracer;
			}
		}

		public static Trace BackgroundTracer
		{
			get
			{
				if (ExTraceGlobals.backgroundTracer == null)
				{
					ExTraceGlobals.backgroundTracer = new Trace(ExTraceGlobals.componentGuid, 10);
				}
				return ExTraceGlobals.backgroundTracer;
			}
		}

		public static Trace DALTracer
		{
			get
			{
				if (ExTraceGlobals.dALTracer == null)
				{
					ExTraceGlobals.dALTracer = new Trace(ExTraceGlobals.componentGuid, 11);
				}
				return ExTraceGlobals.dALTracer;
			}
		}

		public static Trace DeploymentTracer
		{
			get
			{
				if (ExTraceGlobals.deploymentTracer == null)
				{
					ExTraceGlobals.deploymentTracer = new Trace(ExTraceGlobals.componentGuid, 12);
				}
				return ExTraceGlobals.deploymentTracer;
			}
		}

		public static Trace MonitoringTracer
		{
			get
			{
				if (ExTraceGlobals.monitoringTracer == null)
				{
					ExTraceGlobals.monitoringTracer = new Trace(ExTraceGlobals.componentGuid, 14);
				}
				return ExTraceGlobals.monitoringTracer;
			}
		}

		public static Trace ProvisioningTracer
		{
			get
			{
				if (ExTraceGlobals.provisioningTracer == null)
				{
					ExTraceGlobals.provisioningTracer = new Trace(ExTraceGlobals.componentGuid, 15);
				}
				return ExTraceGlobals.provisioningTracer;
			}
		}

		public static Trace TransportTracer
		{
			get
			{
				if (ExTraceGlobals.transportTracer == null)
				{
					ExTraceGlobals.transportTracer = new Trace(ExTraceGlobals.componentGuid, 16);
				}
				return ExTraceGlobals.transportTracer;
			}
		}

		public static Trace WebStoreTracer
		{
			get
			{
				if (ExTraceGlobals.webStoreTracer == null)
				{
					ExTraceGlobals.webStoreTracer = new Trace(ExTraceGlobals.componentGuid, 17);
				}
				return ExTraceGlobals.webStoreTracer;
			}
		}

		public static Trace CmdletTracer
		{
			get
			{
				if (ExTraceGlobals.cmdletTracer == null)
				{
					ExTraceGlobals.cmdletTracer = new Trace(ExTraceGlobals.componentGuid, 18);
				}
				return ExTraceGlobals.cmdletTracer;
			}
		}

		public static Trace GenericHelperTracer
		{
			get
			{
				if (ExTraceGlobals.genericHelperTracer == null)
				{
					ExTraceGlobals.genericHelperTracer = new Trace(ExTraceGlobals.componentGuid, 19);
				}
				return ExTraceGlobals.genericHelperTracer;
			}
		}

		public static Trace DataminingTracer
		{
			get
			{
				if (ExTraceGlobals.dataminingTracer == null)
				{
					ExTraceGlobals.dataminingTracer = new Trace(ExTraceGlobals.componentGuid, 20);
				}
				return ExTraceGlobals.dataminingTracer;
			}
		}

		public static Trace ShadowRedundancyTracer
		{
			get
			{
				if (ExTraceGlobals.shadowRedundancyTracer == null)
				{
					ExTraceGlobals.shadowRedundancyTracer = new Trace(ExTraceGlobals.componentGuid, 21);
				}
				return ExTraceGlobals.shadowRedundancyTracer;
			}
		}

		public static Trace RWSTracer
		{
			get
			{
				if (ExTraceGlobals.rWSTracer == null)
				{
					ExTraceGlobals.rWSTracer = new Trace(ExTraceGlobals.componentGuid, 22);
				}
				return ExTraceGlobals.rWSTracer;
			}
		}

		public static Trace MessageTracingTracer
		{
			get
			{
				if (ExTraceGlobals.messageTracingTracer == null)
				{
					ExTraceGlobals.messageTracingTracer = new Trace(ExTraceGlobals.componentGuid, 23);
				}
				return ExTraceGlobals.messageTracingTracer;
			}
		}

		public static Trace AsyncEngineTracer
		{
			get
			{
				if (ExTraceGlobals.asyncEngineTracer == null)
				{
					ExTraceGlobals.asyncEngineTracer = new Trace(ExTraceGlobals.componentGuid, 24);
				}
				return ExTraceGlobals.asyncEngineTracer;
			}
		}

		public static Trace FFOMigration1415Tracer
		{
			get
			{
				if (ExTraceGlobals.fFOMigration1415Tracer == null)
				{
					ExTraceGlobals.fFOMigration1415Tracer = new Trace(ExTraceGlobals.componentGuid, 25);
				}
				return ExTraceGlobals.fFOMigration1415Tracer;
			}
		}

		public static Trace QueueDigestTracer
		{
			get
			{
				if (ExTraceGlobals.queueDigestTracer == null)
				{
					ExTraceGlobals.queueDigestTracer = new Trace(ExTraceGlobals.componentGuid, 26);
				}
				return ExTraceGlobals.queueDigestTracer;
			}
		}

		public static Trace CentralAdminTracer
		{
			get
			{
				if (ExTraceGlobals.centralAdminTracer == null)
				{
					ExTraceGlobals.centralAdminTracer = new Trace(ExTraceGlobals.componentGuid, 27);
				}
				return ExTraceGlobals.centralAdminTracer;
			}
		}

		public static Trace RPSTracer
		{
			get
			{
				if (ExTraceGlobals.rPSTracer == null)
				{
					ExTraceGlobals.rPSTracer = new Trace(ExTraceGlobals.componentGuid, 28);
				}
				return ExTraceGlobals.rPSTracer;
			}
		}

		private static Guid componentGuid = new Guid("94FBFACE-D4CE-4A9F-B2C6-64646394868F");

		private static FaultInjectionTrace faultInjectionTracer = null;

		private static Trace sMTPTracer = null;

		private static Trace sMTPConnectionTracer = null;

		private static Trace sMTPMonitorTracer = null;

		private static Trace webServiceTracer = null;

		private static Trace hTTPTracer = null;

		private static Trace responderTracer = null;

		private static Trace dNSTracer = null;

		private static Trace antiSpamTracer = null;

		private static Trace backgroundTracer = null;

		private static Trace dALTracer = null;

		private static Trace deploymentTracer = null;

		private static Trace monitoringTracer = null;

		private static Trace provisioningTracer = null;

		private static Trace transportTracer = null;

		private static Trace webStoreTracer = null;

		private static Trace cmdletTracer = null;

		private static Trace genericHelperTracer = null;

		private static Trace dataminingTracer = null;

		private static Trace shadowRedundancyTracer = null;

		private static Trace rWSTracer = null;

		private static Trace messageTracingTracer = null;

		private static Trace asyncEngineTracer = null;

		private static Trace fFOMigration1415Tracer = null;

		private static Trace queueDigestTracer = null;

		private static Trace centralAdminTracer = null;

		private static Trace rPSTracer = null;
	}
}
