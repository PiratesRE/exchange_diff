using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Routing;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Configuration
{
	internal static class ConnectorConfiguration
	{
		public static void SetRoutingOverride(ResolvedMessageEventSource source, Guid tenantId, TenantOutboundConnector recipientConnector, EnvelopeRecipient recipient, Trace tracer, HeaderList headers)
		{
			if (recipient == null)
			{
				throw new ArgumentNullException("recipient");
			}
			RoutingDomain routingDomain = new RoutingDomain(ConnectorConfiguration.RoutingConfiguration.Instance.DefaultOpportunisticTlsConnectorRoutingDomain);
			List<RoutingHost> list = null;
			DeliveryQueueDomain deliveryQueueDomain = DeliveryQueueDomain.UseRecipientDomain;
			string overrideSource = null;
			if (recipientConnector != null)
			{
				if (!recipientConnector.Enabled)
				{
					throw new InvalidOperationException(string.Format("Connector {0} is not enabled.", recipientConnector.Name));
				}
				overrideSource = string.Format("{0}:{1}\\{2}", "Connector", tenantId, recipientConnector.Name);
				if (recipientConnector.Identity != null)
				{
					recipient.Properties["Microsoft.Exchange.Hygiene.TenantOutboundConnectorId"] = recipientConnector.Identity;
					recipient.Properties["Microsoft.Exchange.Hygiene.TenantOutboundConnectorCustomData"] = string.Format("Name={0};ConnectorType={1};UseMxRecord={2}", recipientConnector.Name, recipientConnector.ConnectorType, recipientConnector.UseMXRecord);
				}
				if (headers != null)
				{
					recipient.Properties["PreserveCrossPremisesHeaders"] = recipientConnector.CloudServicesMailEnabled;
				}
				if (!recipientConnector.UseMXRecord)
				{
					tracer.TraceInformation<MultiValuedProperty<SmartHost>>(0, (long)typeof(ConnectorConfiguration).GetHashCode(), "Matching connector found and {0} will be applied as routing override", recipientConnector.SmartHosts);
					list = ConnectorConfiguration.GetRoutingHostCollection(recipientConnector.SmartHosts);
					deliveryQueueDomain = DeliveryQueueDomain.UseAlternateDeliveryRoutingHosts;
				}
				if (recipientConnector.TlsSettings != null)
				{
					TlsAuthLevel valueOrDefault = recipientConnector.TlsSettings.GetValueOrDefault();
					TlsAuthLevel? tlsAuthLevel;
					if (tlsAuthLevel != null)
					{
						switch (valueOrDefault)
						{
						case TlsAuthLevel.EncryptionOnly:
							routingDomain = new RoutingDomain(ConnectorConfiguration.RoutingConfiguration.Instance.ForcedTlsEncryptionOnlyConnectorRoutingDomain);
							goto IL_1CC;
						case TlsAuthLevel.CertificateValidation:
							routingDomain = new RoutingDomain(ConnectorConfiguration.RoutingConfiguration.Instance.ForcedTlsCertificateAuthConnectorRoutingDomain);
							goto IL_1CC;
						case TlsAuthLevel.DomainValidation:
							routingDomain = new RoutingDomain(ConnectorConfiguration.RoutingConfiguration.Instance.ForcedTlsDomainValidationConnectorRoutingDomain);
							if (recipientConnector.TlsDomain != null && !string.IsNullOrEmpty(recipientConnector.TlsDomain.Address))
							{
								source.SetTlsDomain(recipient, recipientConnector.TlsDomain.Address);
								goto IL_1CC;
							}
							goto IL_1CC;
						}
					}
					throw new InvalidOperationException("Unexpected TlsAuthLevel enumeration value encountered");
				}
			}
			IL_1CC:
			source.SetRoutingOverride(recipient, (list == null) ? new RoutingOverride(routingDomain, deliveryQueueDomain) : new RoutingOverride(routingDomain, list, deliveryQueueDomain), overrideSource);
		}

		private static List<RoutingHost> GetRoutingHostCollection(IList<SmartHost> smartHosts)
		{
			if (smartHosts == null)
			{
				return null;
			}
			List<RoutingHost> list = new List<RoutingHost>(smartHosts.Count);
			foreach (SmartHost smartHost in smartHosts)
			{
				list.Add(smartHost.InnerRoutingHost);
			}
			return list;
		}

		public static ADOperationResult GetOutboundConnectors(OrganizationId orgId, Predicate<TenantOutboundConnector> predicate, out IEnumerable<TenantOutboundConnector> outboundConnectors)
		{
			IEnumerable<TenantOutboundConnector> queryResult = null;
			ADOperationResult result = ADNotificationAdapter.TryRunADOperation(delegate()
			{
				IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(orgId), 167, "GetOutboundConnectors", "f:\\15.00.1497\\sources\\dev\\Transport\\src\\Configuration\\ConnectorConfiguration.cs");
				queryResult = from toc in tenantOrTopologyConfigurationSession.Find<TenantOutboundConnector>(null, QueryScope.SubTree, null, null, 0)
				where predicate(toc)
				select toc;
			});
			outboundConnectors = queryResult;
			return result;
		}

		public static ADOperationResult GetOutboundConnectors(IConfigurationSession tenantConfigurationSession, Predicate<TenantOutboundConnector> predicate, out IEnumerable<TenantOutboundConnector> outboundConnectors)
		{
			IEnumerable<TenantOutboundConnector> queryResult = null;
			ADOperationResult result = ADNotificationAdapter.TryRunADOperation(delegate()
			{
				queryResult = from toc in tenantConfigurationSession.Find<TenantOutboundConnector>(null, QueryScope.SubTree, null, null, 0)
				where predicate(toc)
				select toc;
			});
			outboundConnectors = queryResult;
			return result;
		}

		public const string OutboundConnectorPrefix = "Connector";

		private class RoutingConfiguration
		{
			public static ConnectorConfiguration.RoutingConfiguration Instance
			{
				get
				{
					if (ConnectorConfiguration.RoutingConfiguration.instance == null)
					{
						ConnectorConfiguration.RoutingConfiguration value = ConnectorConfiguration.RoutingConfiguration.Load();
						Interlocked.CompareExchange<ConnectorConfiguration.RoutingConfiguration>(ref ConnectorConfiguration.RoutingConfiguration.instance, value, null);
					}
					return ConnectorConfiguration.RoutingConfiguration.instance;
				}
			}

			public string ForcedTlsEncryptionOnlyConnectorRoutingDomain { get; internal set; }

			public string ForcedTlsCertificateAuthConnectorRoutingDomain { get; internal set; }

			public string ForcedTlsDomainValidationConnectorRoutingDomain { get; internal set; }

			public string DefaultOpportunisticTlsConnectorRoutingDomain { get; internal set; }

			public static ConnectorConfiguration.RoutingConfiguration Load()
			{
				ConnectorConfiguration.RoutingConfiguration routingConfiguration = new ConnectorConfiguration.RoutingConfiguration();
				string configString = TransportAppConfig.GetConfigString("OpportunisticTlsConnectorRoutingDomain", "DefaultTlsOpportunistic");
				routingConfiguration.DefaultOpportunisticTlsConnectorRoutingDomain = ((!string.IsNullOrEmpty(configString)) ? configString : "DefaultTlsOpportunistic");
				string configString2 = TransportAppConfig.GetConfigString("ForcedTlsEncryptionOnlyConnectorRoutingDomain", "ForcedTlsEncryptionOnly");
				routingConfiguration.ForcedTlsEncryptionOnlyConnectorRoutingDomain = ((!string.IsNullOrEmpty(configString2)) ? configString2 : "ForcedTlsEncryptionOnly");
				string configString3 = TransportAppConfig.GetConfigString("ForcedTlsCAValidationConnectorRoutingDomain", "ForcedTlsCertificateAuth");
				routingConfiguration.ForcedTlsCertificateAuthConnectorRoutingDomain = ((!string.IsNullOrEmpty(configString3)) ? configString3 : "ForcedTlsCertificateAuth");
				string configString4 = TransportAppConfig.GetConfigString("ForcedTlsDomainValidationConnectorRoutingDomain", "ForcedTlsDomainValidation");
				routingConfiguration.ForcedTlsDomainValidationConnectorRoutingDomain = ((!string.IsNullOrEmpty(configString4)) ? configString4 : "ForcedTlsDomainValidation");
				return routingConfiguration;
			}

			private const string DefaultTlsOpportunistic = "DefaultTlsOpportunistic";

			private const string ForcedTlsEncryptionOnly = "ForcedTlsEncryptionOnly";

			private const string ForcedTlsCertificateAuth = "ForcedTlsCertificateAuth";

			private const string ForcedTlsDomainValidation = "ForcedTlsDomainValidation";

			private const string ForcedTLSEncryptionOnlyConnectorRoutingDomainParameterName = "ForcedTlsEncryptionOnlyConnectorRoutingDomain";

			private const string ForcedTLSCertificateAuthConnectorRoutingDomainParameterName = "ForcedTlsCAValidationConnectorRoutingDomain";

			private const string ForcedTLSDomainValidationConnectorRoutingDomainParameterName = "ForcedTlsDomainValidationConnectorRoutingDomain";

			private const string DefaultOpportunisticTlsConnectorRoutingDomainParameterName = "OpportunisticTlsConnectorRoutingDomain";

			private static ConnectorConfiguration.RoutingConfiguration instance;
		}
	}
}
