using System;
using Microsoft.Exchange.Diagnostics.FaultInjection;

namespace Microsoft.Exchange.Diagnostics.Components.Transport
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

		public static Trace SmtpReceiveTracer
		{
			get
			{
				if (ExTraceGlobals.smtpReceiveTracer == null)
				{
					ExTraceGlobals.smtpReceiveTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.smtpReceiveTracer;
			}
		}

		public static Trace SmtpSendTracer
		{
			get
			{
				if (ExTraceGlobals.smtpSendTracer == null)
				{
					ExTraceGlobals.smtpSendTracer = new Trace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.smtpSendTracer;
			}
		}

		public static Trace PickupTracer
		{
			get
			{
				if (ExTraceGlobals.pickupTracer == null)
				{
					ExTraceGlobals.pickupTracer = new Trace(ExTraceGlobals.componentGuid, 3);
				}
				return ExTraceGlobals.pickupTracer;
			}
		}

		public static Trace ServiceTracer
		{
			get
			{
				if (ExTraceGlobals.serviceTracer == null)
				{
					ExTraceGlobals.serviceTracer = new Trace(ExTraceGlobals.componentGuid, 4);
				}
				return ExTraceGlobals.serviceTracer;
			}
		}

		public static Trace QueuingTracer
		{
			get
			{
				if (ExTraceGlobals.queuingTracer == null)
				{
					ExTraceGlobals.queuingTracer = new Trace(ExTraceGlobals.componentGuid, 5);
				}
				return ExTraceGlobals.queuingTracer;
			}
		}

		public static Trace DSNTracer
		{
			get
			{
				if (ExTraceGlobals.dSNTracer == null)
				{
					ExTraceGlobals.dSNTracer = new Trace(ExTraceGlobals.componentGuid, 6);
				}
				return ExTraceGlobals.dSNTracer;
			}
		}

		public static Trace RoutingTracer
		{
			get
			{
				if (ExTraceGlobals.routingTracer == null)
				{
					ExTraceGlobals.routingTracer = new Trace(ExTraceGlobals.componentGuid, 7);
				}
				return ExTraceGlobals.routingTracer;
			}
		}

		public static Trace ResolverTracer
		{
			get
			{
				if (ExTraceGlobals.resolverTracer == null)
				{
					ExTraceGlobals.resolverTracer = new Trace(ExTraceGlobals.componentGuid, 8);
				}
				return ExTraceGlobals.resolverTracer;
			}
		}

		public static Trace ContentConversionTracer
		{
			get
			{
				if (ExTraceGlobals.contentConversionTracer == null)
				{
					ExTraceGlobals.contentConversionTracer = new Trace(ExTraceGlobals.componentGuid, 9);
				}
				return ExTraceGlobals.contentConversionTracer;
			}
		}

		public static Trace ExtensibilityTracer
		{
			get
			{
				if (ExTraceGlobals.extensibilityTracer == null)
				{
					ExTraceGlobals.extensibilityTracer = new Trace(ExTraceGlobals.componentGuid, 10);
				}
				return ExTraceGlobals.extensibilityTracer;
			}
		}

		public static Trace SchedulerTracer
		{
			get
			{
				if (ExTraceGlobals.schedulerTracer == null)
				{
					ExTraceGlobals.schedulerTracer = new Trace(ExTraceGlobals.componentGuid, 11);
				}
				return ExTraceGlobals.schedulerTracer;
			}
		}

		public static Trace SecureMailTracer
		{
			get
			{
				if (ExTraceGlobals.secureMailTracer == null)
				{
					ExTraceGlobals.secureMailTracer = new Trace(ExTraceGlobals.componentGuid, 12);
				}
				return ExTraceGlobals.secureMailTracer;
			}
		}

		public static Trace MessageTrackingTracer
		{
			get
			{
				if (ExTraceGlobals.messageTrackingTracer == null)
				{
					ExTraceGlobals.messageTrackingTracer = new Trace(ExTraceGlobals.componentGuid, 13);
				}
				return ExTraceGlobals.messageTrackingTracer;
			}
		}

		public static Trace ResourceManagerTracer
		{
			get
			{
				if (ExTraceGlobals.resourceManagerTracer == null)
				{
					ExTraceGlobals.resourceManagerTracer = new Trace(ExTraceGlobals.componentGuid, 14);
				}
				return ExTraceGlobals.resourceManagerTracer;
			}
		}

		public static Trace ConfigurationTracer
		{
			get
			{
				if (ExTraceGlobals.configurationTracer == null)
				{
					ExTraceGlobals.configurationTracer = new Trace(ExTraceGlobals.componentGuid, 15);
				}
				return ExTraceGlobals.configurationTracer;
			}
		}

		public static Trace DumpsterTracer
		{
			get
			{
				if (ExTraceGlobals.dumpsterTracer == null)
				{
					ExTraceGlobals.dumpsterTracer = new Trace(ExTraceGlobals.componentGuid, 16);
				}
				return ExTraceGlobals.dumpsterTracer;
			}
		}

		public static Trace ExpoTracer
		{
			get
			{
				if (ExTraceGlobals.expoTracer == null)
				{
					ExTraceGlobals.expoTracer = new Trace(ExTraceGlobals.componentGuid, 17);
				}
				return ExTraceGlobals.expoTracer;
			}
		}

		public static Trace CertificateTracer
		{
			get
			{
				if (ExTraceGlobals.certificateTracer == null)
				{
					ExTraceGlobals.certificateTracer = new Trace(ExTraceGlobals.componentGuid, 18);
				}
				return ExTraceGlobals.certificateTracer;
			}
		}

		public static Trace OrarTracer
		{
			get
			{
				if (ExTraceGlobals.orarTracer == null)
				{
					ExTraceGlobals.orarTracer = new Trace(ExTraceGlobals.componentGuid, 19);
				}
				return ExTraceGlobals.orarTracer;
			}
		}

		public static Trace ShadowRedundancyTracer
		{
			get
			{
				if (ExTraceGlobals.shadowRedundancyTracer == null)
				{
					ExTraceGlobals.shadowRedundancyTracer = new Trace(ExTraceGlobals.componentGuid, 20);
				}
				return ExTraceGlobals.shadowRedundancyTracer;
			}
		}

		public static Trace ApprovalTracer
		{
			get
			{
				if (ExTraceGlobals.approvalTracer == null)
				{
					ExTraceGlobals.approvalTracer = new Trace(ExTraceGlobals.componentGuid, 22);
				}
				return ExTraceGlobals.approvalTracer;
			}
		}

		public static Trace TransportDumpsterTracer
		{
			get
			{
				if (ExTraceGlobals.transportDumpsterTracer == null)
				{
					ExTraceGlobals.transportDumpsterTracer = new Trace(ExTraceGlobals.componentGuid, 23);
				}
				return ExTraceGlobals.transportDumpsterTracer;
			}
		}

		public static Trace TransportSettingsCacheTracer
		{
			get
			{
				if (ExTraceGlobals.transportSettingsCacheTracer == null)
				{
					ExTraceGlobals.transportSettingsCacheTracer = new Trace(ExTraceGlobals.componentGuid, 24);
				}
				return ExTraceGlobals.transportSettingsCacheTracer;
			}
		}

		public static Trace TransportRulesCacheTracer
		{
			get
			{
				if (ExTraceGlobals.transportRulesCacheTracer == null)
				{
					ExTraceGlobals.transportRulesCacheTracer = new Trace(ExTraceGlobals.componentGuid, 25);
				}
				return ExTraceGlobals.transportRulesCacheTracer;
			}
		}

		public static Trace MicrosoftExchangeRecipientCacheTracer
		{
			get
			{
				if (ExTraceGlobals.microsoftExchangeRecipientCacheTracer == null)
				{
					ExTraceGlobals.microsoftExchangeRecipientCacheTracer = new Trace(ExTraceGlobals.componentGuid, 26);
				}
				return ExTraceGlobals.microsoftExchangeRecipientCacheTracer;
			}
		}

		public static Trace RemoteDomainsCacheTracer
		{
			get
			{
				if (ExTraceGlobals.remoteDomainsCacheTracer == null)
				{
					ExTraceGlobals.remoteDomainsCacheTracer = new Trace(ExTraceGlobals.componentGuid, 27);
				}
				return ExTraceGlobals.remoteDomainsCacheTracer;
			}
		}

		public static Trace JournalingRulesCacheTracer
		{
			get
			{
				if (ExTraceGlobals.journalingRulesCacheTracer == null)
				{
					ExTraceGlobals.journalingRulesCacheTracer = new Trace(ExTraceGlobals.componentGuid, 28);
				}
				return ExTraceGlobals.journalingRulesCacheTracer;
			}
		}

		public static Trace ResourcePoolTracer
		{
			get
			{
				if (ExTraceGlobals.resourcePoolTracer == null)
				{
					ExTraceGlobals.resourcePoolTracer = new Trace(ExTraceGlobals.componentGuid, 29);
				}
				return ExTraceGlobals.resourcePoolTracer;
			}
		}

		public static Trace DeliveryAgentsTracer
		{
			get
			{
				if (ExTraceGlobals.deliveryAgentsTracer == null)
				{
					ExTraceGlobals.deliveryAgentsTracer = new Trace(ExTraceGlobals.componentGuid, 30);
				}
				return ExTraceGlobals.deliveryAgentsTracer;
			}
		}

		public static Trace SupervisionTracer
		{
			get
			{
				if (ExTraceGlobals.supervisionTracer == null)
				{
					ExTraceGlobals.supervisionTracer = new Trace(ExTraceGlobals.componentGuid, 31);
				}
				return ExTraceGlobals.supervisionTracer;
			}
		}

		public static Trace RightsManagementTracer
		{
			get
			{
				if (ExTraceGlobals.rightsManagementTracer == null)
				{
					ExTraceGlobals.rightsManagementTracer = new Trace(ExTraceGlobals.componentGuid, 32);
				}
				return ExTraceGlobals.rightsManagementTracer;
			}
		}

		public static Trace PerimeterSettingsCacheTracer
		{
			get
			{
				if (ExTraceGlobals.perimeterSettingsCacheTracer == null)
				{
					ExTraceGlobals.perimeterSettingsCacheTracer = new Trace(ExTraceGlobals.componentGuid, 33);
				}
				return ExTraceGlobals.perimeterSettingsCacheTracer;
			}
		}

		public static Trace PreviousHopLatencyTracer
		{
			get
			{
				if (ExTraceGlobals.previousHopLatencyTracer == null)
				{
					ExTraceGlobals.previousHopLatencyTracer = new Trace(ExTraceGlobals.componentGuid, 34);
				}
				return ExTraceGlobals.previousHopLatencyTracer;
			}
		}

		public static FaultInjectionTrace FaultInjectionTracer
		{
			get
			{
				if (ExTraceGlobals.faultInjectionTracer == null)
				{
					ExTraceGlobals.faultInjectionTracer = new FaultInjectionTrace(ExTraceGlobals.componentGuid, 35);
				}
				return ExTraceGlobals.faultInjectionTracer;
			}
		}

		public static Trace OrganizationSettingsCacheTracer
		{
			get
			{
				if (ExTraceGlobals.organizationSettingsCacheTracer == null)
				{
					ExTraceGlobals.organizationSettingsCacheTracer = new Trace(ExTraceGlobals.componentGuid, 36);
				}
				return ExTraceGlobals.organizationSettingsCacheTracer;
			}
		}

		public static Trace AnonymousCertificateValidationResultCacheTracer
		{
			get
			{
				if (ExTraceGlobals.anonymousCertificateValidationResultCacheTracer == null)
				{
					ExTraceGlobals.anonymousCertificateValidationResultCacheTracer = new Trace(ExTraceGlobals.componentGuid, 37);
				}
				return ExTraceGlobals.anonymousCertificateValidationResultCacheTracer;
			}
		}

		public static Trace AcceptedDomainsCacheTracer
		{
			get
			{
				if (ExTraceGlobals.acceptedDomainsCacheTracer == null)
				{
					ExTraceGlobals.acceptedDomainsCacheTracer = new Trace(ExTraceGlobals.componentGuid, 38);
				}
				return ExTraceGlobals.acceptedDomainsCacheTracer;
			}
		}

		public static Trace ProxyHubSelectorTracer
		{
			get
			{
				if (ExTraceGlobals.proxyHubSelectorTracer == null)
				{
					ExTraceGlobals.proxyHubSelectorTracer = new Trace(ExTraceGlobals.componentGuid, 39);
				}
				return ExTraceGlobals.proxyHubSelectorTracer;
			}
		}

		public static Trace MessageResubmissionTracer
		{
			get
			{
				if (ExTraceGlobals.messageResubmissionTracer == null)
				{
					ExTraceGlobals.messageResubmissionTracer = new Trace(ExTraceGlobals.componentGuid, 40);
				}
				return ExTraceGlobals.messageResubmissionTracer;
			}
		}

		public static Trace StorageTracer
		{
			get
			{
				if (ExTraceGlobals.storageTracer == null)
				{
					ExTraceGlobals.storageTracer = new Trace(ExTraceGlobals.componentGuid, 41);
				}
				return ExTraceGlobals.storageTracer;
			}
		}

		public static Trace PoisonTracer
		{
			get
			{
				if (ExTraceGlobals.poisonTracer == null)
				{
					ExTraceGlobals.poisonTracer = new Trace(ExTraceGlobals.componentGuid, 42);
				}
				return ExTraceGlobals.poisonTracer;
			}
		}

		public static Trace HostedEncryptionTracer
		{
			get
			{
				if (ExTraceGlobals.hostedEncryptionTracer == null)
				{
					ExTraceGlobals.hostedEncryptionTracer = new Trace(ExTraceGlobals.componentGuid, 43);
				}
				return ExTraceGlobals.hostedEncryptionTracer;
			}
		}

		public static Trace OutboundConnectorsCacheTracer
		{
			get
			{
				if (ExTraceGlobals.outboundConnectorsCacheTracer == null)
				{
					ExTraceGlobals.outboundConnectorsCacheTracer = new Trace(ExTraceGlobals.componentGuid, 44);
				}
				return ExTraceGlobals.outboundConnectorsCacheTracer;
			}
		}

		private static Guid componentGuid = new Guid("c3ea5adf-c135-45e7-9dff-e1dc3bd67999");

		private static Trace generalTracer = null;

		private static Trace smtpReceiveTracer = null;

		private static Trace smtpSendTracer = null;

		private static Trace pickupTracer = null;

		private static Trace serviceTracer = null;

		private static Trace queuingTracer = null;

		private static Trace dSNTracer = null;

		private static Trace routingTracer = null;

		private static Trace resolverTracer = null;

		private static Trace contentConversionTracer = null;

		private static Trace extensibilityTracer = null;

		private static Trace schedulerTracer = null;

		private static Trace secureMailTracer = null;

		private static Trace messageTrackingTracer = null;

		private static Trace resourceManagerTracer = null;

		private static Trace configurationTracer = null;

		private static Trace dumpsterTracer = null;

		private static Trace expoTracer = null;

		private static Trace certificateTracer = null;

		private static Trace orarTracer = null;

		private static Trace shadowRedundancyTracer = null;

		private static Trace approvalTracer = null;

		private static Trace transportDumpsterTracer = null;

		private static Trace transportSettingsCacheTracer = null;

		private static Trace transportRulesCacheTracer = null;

		private static Trace microsoftExchangeRecipientCacheTracer = null;

		private static Trace remoteDomainsCacheTracer = null;

		private static Trace journalingRulesCacheTracer = null;

		private static Trace resourcePoolTracer = null;

		private static Trace deliveryAgentsTracer = null;

		private static Trace supervisionTracer = null;

		private static Trace rightsManagementTracer = null;

		private static Trace perimeterSettingsCacheTracer = null;

		private static Trace previousHopLatencyTracer = null;

		private static FaultInjectionTrace faultInjectionTracer = null;

		private static Trace organizationSettingsCacheTracer = null;

		private static Trace anonymousCertificateValidationResultCacheTracer = null;

		private static Trace acceptedDomainsCacheTracer = null;

		private static Trace proxyHubSelectorTracer = null;

		private static Trace messageResubmissionTracer = null;

		private static Trace storageTracer = null;

		private static Trace poisonTracer = null;

		private static Trace hostedEncryptionTracer = null;

		private static Trace outboundConnectorsCacheTracer = null;
	}
}
