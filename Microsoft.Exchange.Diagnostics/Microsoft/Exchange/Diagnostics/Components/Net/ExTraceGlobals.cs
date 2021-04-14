using System;

namespace Microsoft.Exchange.Diagnostics.Components.Net
{
	public static class ExTraceGlobals
	{
		public static Trace DNSTracer
		{
			get
			{
				if (ExTraceGlobals.dNSTracer == null)
				{
					ExTraceGlobals.dNSTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.dNSTracer;
			}
		}

		public static Trace NetworkTracer
		{
			get
			{
				if (ExTraceGlobals.networkTracer == null)
				{
					ExTraceGlobals.networkTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.networkTracer;
			}
		}

		public static Trace AuthenticationTracer
		{
			get
			{
				if (ExTraceGlobals.authenticationTracer == null)
				{
					ExTraceGlobals.authenticationTracer = new Trace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.authenticationTracer;
			}
		}

		public static Trace CertificateTracer
		{
			get
			{
				if (ExTraceGlobals.certificateTracer == null)
				{
					ExTraceGlobals.certificateTracer = new Trace(ExTraceGlobals.componentGuid, 3);
				}
				return ExTraceGlobals.certificateTracer;
			}
		}

		public static Trace DirectoryServicesTracer
		{
			get
			{
				if (ExTraceGlobals.directoryServicesTracer == null)
				{
					ExTraceGlobals.directoryServicesTracer = new Trace(ExTraceGlobals.componentGuid, 4);
				}
				return ExTraceGlobals.directoryServicesTracer;
			}
		}

		public static Trace ProcessManagerTracer
		{
			get
			{
				if (ExTraceGlobals.processManagerTracer == null)
				{
					ExTraceGlobals.processManagerTracer = new Trace(ExTraceGlobals.componentGuid, 5);
				}
				return ExTraceGlobals.processManagerTracer;
			}
		}

		public static Trace HttpClientTracer
		{
			get
			{
				if (ExTraceGlobals.httpClientTracer == null)
				{
					ExTraceGlobals.httpClientTracer = new Trace(ExTraceGlobals.componentGuid, 6);
				}
				return ExTraceGlobals.httpClientTracer;
			}
		}

		public static Trace ProtocolLogTracer
		{
			get
			{
				if (ExTraceGlobals.protocolLogTracer == null)
				{
					ExTraceGlobals.protocolLogTracer = new Trace(ExTraceGlobals.componentGuid, 7);
				}
				return ExTraceGlobals.protocolLogTracer;
			}
		}

		public static Trace RightsManagementTracer
		{
			get
			{
				if (ExTraceGlobals.rightsManagementTracer == null)
				{
					ExTraceGlobals.rightsManagementTracer = new Trace(ExTraceGlobals.componentGuid, 8);
				}
				return ExTraceGlobals.rightsManagementTracer;
			}
		}

		public static Trace LiveIDAuthenticationClientTracer
		{
			get
			{
				if (ExTraceGlobals.liveIDAuthenticationClientTracer == null)
				{
					ExTraceGlobals.liveIDAuthenticationClientTracer = new Trace(ExTraceGlobals.componentGuid, 9);
				}
				return ExTraceGlobals.liveIDAuthenticationClientTracer;
			}
		}

		public static Trace DeltaSyncClientTracer
		{
			get
			{
				if (ExTraceGlobals.deltaSyncClientTracer == null)
				{
					ExTraceGlobals.deltaSyncClientTracer = new Trace(ExTraceGlobals.componentGuid, 11);
				}
				return ExTraceGlobals.deltaSyncClientTracer;
			}
		}

		public static Trace DeltaSyncResponseHandlerTracer
		{
			get
			{
				if (ExTraceGlobals.deltaSyncResponseHandlerTracer == null)
				{
					ExTraceGlobals.deltaSyncResponseHandlerTracer = new Trace(ExTraceGlobals.componentGuid, 12);
				}
				return ExTraceGlobals.deltaSyncResponseHandlerTracer;
			}
		}

		public static Trace LanguagePackInfoTracer
		{
			get
			{
				if (ExTraceGlobals.languagePackInfoTracer == null)
				{
					ExTraceGlobals.languagePackInfoTracer = new Trace(ExTraceGlobals.componentGuid, 13);
				}
				return ExTraceGlobals.languagePackInfoTracer;
			}
		}

		public static Trace WSTrustTracer
		{
			get
			{
				if (ExTraceGlobals.wSTrustTracer == null)
				{
					ExTraceGlobals.wSTrustTracer = new Trace(ExTraceGlobals.componentGuid, 14);
				}
				return ExTraceGlobals.wSTrustTracer;
			}
		}

		public static Trace EwsClientTracer
		{
			get
			{
				if (ExTraceGlobals.ewsClientTracer == null)
				{
					ExTraceGlobals.ewsClientTracer = new Trace(ExTraceGlobals.componentGuid, 15);
				}
				return ExTraceGlobals.ewsClientTracer;
			}
		}

		public static Trace ConfigurationTracer
		{
			get
			{
				if (ExTraceGlobals.configurationTracer == null)
				{
					ExTraceGlobals.configurationTracer = new Trace(ExTraceGlobals.componentGuid, 16);
				}
				return ExTraceGlobals.configurationTracer;
			}
		}

		public static Trace SmtpClientTracer
		{
			get
			{
				if (ExTraceGlobals.smtpClientTracer == null)
				{
					ExTraceGlobals.smtpClientTracer = new Trace(ExTraceGlobals.componentGuid, 17);
				}
				return ExTraceGlobals.smtpClientTracer;
			}
		}

		public static Trace XropServiceClientTracer
		{
			get
			{
				if (ExTraceGlobals.xropServiceClientTracer == null)
				{
					ExTraceGlobals.xropServiceClientTracer = new Trace(ExTraceGlobals.componentGuid, 18);
				}
				return ExTraceGlobals.xropServiceClientTracer;
			}
		}

		public static Trace XropServiceServerTracer
		{
			get
			{
				if (ExTraceGlobals.xropServiceServerTracer == null)
				{
					ExTraceGlobals.xropServiceServerTracer = new Trace(ExTraceGlobals.componentGuid, 19);
				}
				return ExTraceGlobals.xropServiceServerTracer;
			}
		}

		public static Trace ClaimTracer
		{
			get
			{
				if (ExTraceGlobals.claimTracer == null)
				{
					ExTraceGlobals.claimTracer = new Trace(ExTraceGlobals.componentGuid, 20);
				}
				return ExTraceGlobals.claimTracer;
			}
		}

		public static Trace FacebookTracer
		{
			get
			{
				if (ExTraceGlobals.facebookTracer == null)
				{
					ExTraceGlobals.facebookTracer = new Trace(ExTraceGlobals.componentGuid, 21);
				}
				return ExTraceGlobals.facebookTracer;
			}
		}

		public static Trace LinkedInTracer
		{
			get
			{
				if (ExTraceGlobals.linkedInTracer == null)
				{
					ExTraceGlobals.linkedInTracer = new Trace(ExTraceGlobals.componentGuid, 22);
				}
				return ExTraceGlobals.linkedInTracer;
			}
		}

		public static Trace MonitoringWebClientTracer
		{
			get
			{
				if (ExTraceGlobals.monitoringWebClientTracer == null)
				{
					ExTraceGlobals.monitoringWebClientTracer = new Trace(ExTraceGlobals.componentGuid, 23);
				}
				return ExTraceGlobals.monitoringWebClientTracer;
			}
		}

		public static Trace RulesBasedHttpModuleTracer
		{
			get
			{
				if (ExTraceGlobals.rulesBasedHttpModuleTracer == null)
				{
					ExTraceGlobals.rulesBasedHttpModuleTracer = new Trace(ExTraceGlobals.componentGuid, 24);
				}
				return ExTraceGlobals.rulesBasedHttpModuleTracer;
			}
		}

		public static Trace AADClientTracer
		{
			get
			{
				if (ExTraceGlobals.aADClientTracer == null)
				{
					ExTraceGlobals.aADClientTracer = new Trace(ExTraceGlobals.componentGuid, 25);
				}
				return ExTraceGlobals.aADClientTracer;
			}
		}

		public static Trace AppSettingsTracer
		{
			get
			{
				if (ExTraceGlobals.appSettingsTracer == null)
				{
					ExTraceGlobals.appSettingsTracer = new Trace(ExTraceGlobals.componentGuid, 26);
				}
				return ExTraceGlobals.appSettingsTracer;
			}
		}

		public static Trace CommonTracer
		{
			get
			{
				if (ExTraceGlobals.commonTracer == null)
				{
					ExTraceGlobals.commonTracer = new Trace(ExTraceGlobals.componentGuid, 27);
				}
				return ExTraceGlobals.commonTracer;
			}
		}

		private static Guid componentGuid = new Guid("351632BC-3F4E-4C79-A368-F8E54DCE4A2E");

		private static Trace dNSTracer = null;

		private static Trace networkTracer = null;

		private static Trace authenticationTracer = null;

		private static Trace certificateTracer = null;

		private static Trace directoryServicesTracer = null;

		private static Trace processManagerTracer = null;

		private static Trace httpClientTracer = null;

		private static Trace protocolLogTracer = null;

		private static Trace rightsManagementTracer = null;

		private static Trace liveIDAuthenticationClientTracer = null;

		private static Trace deltaSyncClientTracer = null;

		private static Trace deltaSyncResponseHandlerTracer = null;

		private static Trace languagePackInfoTracer = null;

		private static Trace wSTrustTracer = null;

		private static Trace ewsClientTracer = null;

		private static Trace configurationTracer = null;

		private static Trace smtpClientTracer = null;

		private static Trace xropServiceClientTracer = null;

		private static Trace xropServiceServerTracer = null;

		private static Trace claimTracer = null;

		private static Trace facebookTracer = null;

		private static Trace linkedInTracer = null;

		private static Trace monitoringWebClientTracer = null;

		private static Trace rulesBasedHttpModuleTracer = null;

		private static Trace aADClientTracer = null;

		private static Trace appSettingsTracer = null;

		private static Trace commonTracer = null;
	}
}
