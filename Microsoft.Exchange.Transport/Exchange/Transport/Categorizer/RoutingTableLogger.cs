using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.ExchangeTopology;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Extensibility.Internal;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal class RoutingTableLogger : DisposeTrackableBase
	{
		private RoutingTableLogger(string fileName)
		{
			this.xmlWriter = new XmlTextWriter(fileName, Encoding.UTF8);
			this.xmlWriter.Formatting = Formatting.Indented;
			this.xmlWriter.WriteStartDocument();
		}

		public static void LogRoutingTables(RoutingTables routingTables, RoutingTopologyBase topologyConfig, RoutingContextCore context)
		{
			string text = RoutingTableLogFileManager.LogFilePath;
			try
			{
				text = RoutingTableLogFileManager.CleanupLogsAndGetLogFileName(topologyConfig.WhenCreated, context);
				RoutingDiag.Tracer.TraceDebug<string>(0L, "Start logging routing table to file {0}.", text);
				using (RoutingTableLogger routingTableLogger = new RoutingTableLogger(text))
				{
					routingTableLogger.WriteStartElement("RoutingConfiguration");
					routingTableLogger.WriteElement<string>("SchemaVersion", "15.00.0610.000");
					routingTableLogger.WriteElement<ProcessTransportRole>("ProcessRole", context.GetProcessRoleForDiagnostics());
					topologyConfig.LogData(routingTableLogger);
					routingTableLogger.WriteAppConfigSettings(context.Settings);
					routingTableLogger.WriteEndDocument();
				}
				RoutingDiag.Tracer.TraceDebug<string>(0L, "Finished logging routing table to file {0}.", text);
			}
			catch (UnauthorizedAccessException ex)
			{
				RoutingDiag.Tracer.TraceError<string, UnauthorizedAccessException>(0L, "Failed to log routing table to file: {0} Exception: {1}", text, ex);
				RoutingDiag.EventLogger.LogEvent(TransportEventLogConstants.Tuple_RoutingTableLogCreationFailure, null, new object[]
				{
					text,
					ex.ToString(),
					ex
				});
			}
			catch (IOException ex2)
			{
				RoutingDiag.Tracer.TraceError<string, IOException>(0L, "Failed to log routing table to file: {0} Exception: {1}", text, ex2);
				RoutingDiag.EventLogger.LogEvent(TransportEventLogConstants.Tuple_RoutingTableLogCreationFailure, null, new object[]
				{
					text,
					ex2.ToString(),
					ex2
				});
			}
		}

		public void WriteStartElement(string elementName)
		{
			this.xmlWriter.WriteStartElement(elementName);
		}

		public void WriteEndElement()
		{
			this.xmlWriter.WriteEndElement();
		}

		public void WriteElement<T>(string elementName, T elementValue)
		{
			this.xmlWriter.WriteElementString(elementName, (elementValue == null) ? "null" : elementValue.ToString());
		}

		public void WriteElement(string elementName, DateTime elementValue)
		{
			string elementValue2 = elementValue.ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffZ", DateTimeFormatInfo.InvariantInfo);
			this.WriteElement<string>(elementName, elementValue2);
		}

		public void WriteADObjectId(ADObjectId id)
		{
			this.WriteElement<string>("Name", id.Name);
			this.WriteElement<Guid>("ObjectGuid", id.ObjectGuid);
		}

		public void WriteADObjectIdWithDN(ADObjectId id)
		{
			this.WriteADObjectId(id);
			this.WriteElement<string>("DN", id.DistinguishedName ?? "null");
		}

		public void WriteADReference(string elementName, ADObjectId reference)
		{
			this.WriteStartElement(elementName);
			this.WriteADObjectId(reference);
			this.WriteEndElement();
		}

		public void WriteServer(Server server)
		{
			this.WriteStartElement("Server");
			this.WriteCommonADObjectProperties(server);
			this.WriteElement<int>("MajorVersion", server.MajorVersion);
			if (!string.IsNullOrEmpty(server.Fqdn))
			{
				this.WriteElement<string>("FQDN", server.Fqdn);
			}
			if (!string.IsNullOrEmpty(server.ExchangeLegacyDN))
			{
				this.WriteElement<string>("LegacyDN", server.ExchangeLegacyDN);
			}
			if (server.IsExchange2007OrLater)
			{
				this.WriteElement<ServerRole>("ServerRoles", server.CurrentServerRole);
				if (server.ServerSite != null)
				{
					this.WriteADReference("ADSiteId", server.ServerSite);
				}
				if (server.DatabaseAvailabilityGroup != null)
				{
					this.WriteADReference("DagId", server.DatabaseAvailabilityGroup);
				}
				if (server.IsFrontendTransportServer)
				{
					this.WriteElement<bool>("FrontendComponentActive", RoutingServerInfo.IsFrontendComponentActive(server));
				}
				if (server.IsHubTransportServer)
				{
					this.WriteElement<bool>("HubComponentActive", RoutingServerInfo.IsHubComponentActive(server));
				}
			}
			else if (server.HomeRoutingGroup != null)
			{
				this.WriteADReference("HomeRoutingGroupId", server.HomeRoutingGroup);
			}
			this.WriteEndElement();
		}

		public void WriteTopologyServer(TopologyServer server)
		{
			this.WriteStartElement("Server");
			this.WriteCommonADObjectProperties(server);
			this.WriteElement<int>("MajorVersion", server.MajorVersion);
			if (!string.IsNullOrEmpty(server.Fqdn))
			{
				this.WriteElement<string>("FQDN", server.Fqdn);
			}
			if (!string.IsNullOrEmpty(server.ExchangeLegacyDN))
			{
				this.WriteElement<string>("LegacyDN", server.ExchangeLegacyDN);
			}
			if (server.IsExchange2007OrLater)
			{
				this.WriteElement<ServerRole>("ServerRoles", server.CurrentServerRole);
				if (server.ServerSite != null)
				{
					this.WriteADReference("ADSiteId", server.ServerSite);
				}
				if (server.DatabaseAvailabilityGroup != null)
				{
					this.WriteADReference("DagId", server.DatabaseAvailabilityGroup);
				}
				if (server.IsFrontendTransportServer)
				{
					this.WriteElement<bool>("FrontendComponentActive", RoutingServerInfo.IsFrontendComponentActive(server));
				}
				if (server.IsHubTransportServer)
				{
					this.WriteElement<bool>("HubComponentActive", RoutingServerInfo.IsHubComponentActive(server));
				}
			}
			else if (server.HomeRoutingGroup != null)
			{
				this.WriteADReference("HomeRoutingGroupId", server.HomeRoutingGroup);
			}
			this.WriteEndElement();
		}

		public void WriteSendConnector(MailGateway connector)
		{
			this.WriteStartElement("SendConnector");
			this.WriteCommonConnectorProperties(connector);
			this.WriteElement<string>("ConnectorType", connector.GetType().Name);
			this.WriteElement<bool>("Enabled", connector.Enabled);
			this.WriteElement<bool>("IsScopedConnector", connector.IsScopedConnector);
			this.WriteStartElement("AddressSpaces");
			foreach (AddressSpace elementValue in connector.AddressSpaces)
			{
				this.WriteElement<AddressSpace>("AddressSpace", elementValue);
			}
			this.WriteEndElement();
			SmtpSendConnectorConfig smtpSendConnectorConfig = connector as SmtpSendConnectorConfig;
			if (smtpSendConnectorConfig != null)
			{
				this.WriteElement<bool>("DnsRoutingEnabled", smtpSendConnectorConfig.DNSRoutingEnabled);
				if (!smtpSendConnectorConfig.DNSRoutingEnabled)
				{
					this.WriteElement<string>("SmartHosts", smtpSendConnectorConfig.SmartHostsString);
				}
			}
			if (!RoutingUtils.IsNullOrEmpty<ConnectedDomain>(connector.ConnectedDomains))
			{
				this.WriteStartElement("ConnectedDomains");
				foreach (ConnectedDomain elementValue2 in connector.ConnectedDomains)
				{
					this.WriteElement<ConnectedDomain>("ConnectedDomain", elementValue2);
				}
				this.WriteEndElement();
			}
			this.WriteEndElement();
		}

		public void WriteDatabase(MiniDatabase database)
		{
			this.WriteStartElement("Database");
			this.WriteCommonADObjectProperties(database);
			if (database.Server != null)
			{
				this.WriteADReference("OwningServerId", database.Server);
			}
			this.WriteEndElement();
		}

		public void WriteADSite(TopologySite site)
		{
			this.WriteStartElement("ADSite");
			this.WriteCommonADObjectProperties(site);
			this.WriteElement<bool>("HubSiteEnabled", site.HubSiteEnabled);
			this.WriteElement<bool>("InboundMailEnabled", site.InboundMailEnabled);
			this.WriteEndElement();
		}

		public void WriteADSiteLink(TopologySiteLink link)
		{
			this.WriteStartElement("ADSiteLink");
			this.WriteCommonADObjectProperties(link);
			this.WriteElement<int>("ADCost", link.ADCost);
			if (link.ExchangeCost != null)
			{
				this.WriteElement<int?>("ExchangeCost", link.ExchangeCost);
			}
			if (!link.MaxMessageSize.IsUnlimited)
			{
				this.WriteElement<Unlimited<ByteQuantifiedSize>>("MaxMessageSize", link.MaxMessageSize);
			}
			this.WriteStartElement("LinkedADSites");
			foreach (ADObjectId reference in link.Sites)
			{
				this.WriteADReference("ADSiteId", reference);
			}
			this.WriteEndElement();
			this.WriteEndElement();
		}

		public void WriteRoutingGroup(RoutingGroup routingGroup)
		{
			this.WriteStartElement("RoutingGroup");
			this.WriteCommonADObjectProperties(routingGroup);
			this.WriteEndElement();
		}

		public void WriteRoutingGroupConnector(RoutingGroupConnector rgc)
		{
			this.WriteStartElement("RoutingGroupConnector");
			this.WriteCommonConnectorProperties(rgc);
			this.WriteElement<int>("Cost", rgc.Cost);
			if (rgc.TargetRoutingGroup != null)
			{
				this.WriteADReference("TargetRoutingGroupId", rgc.TargetRoutingGroup);
			}
			if (!RoutingUtils.IsNullOrEmpty<ADObjectId>(rgc.TargetTransportServers))
			{
				this.WriteStartElement("TargetServers");
				foreach (ADObjectId reference in rgc.TargetTransportServers)
				{
					this.WriteADReference("TargetServerId", reference);
				}
				this.WriteEndElement();
			}
			this.WriteEndElement();
		}

		public void WritePublicFolderTree(PublicFolderTree publicFolderTree)
		{
			this.WriteStartElement("PublicFolderTree");
			this.WriteCommonADObjectProperties(publicFolderTree);
			if (!RoutingUtils.IsNullOrEmpty<ADObjectId>(publicFolderTree.PublicDatabases))
			{
				this.WriteStartElement("PublicDatabases");
				foreach (ADObjectId reference in publicFolderTree.PublicDatabases)
				{
					this.WriteADReference("PublicDatabaseId", reference);
				}
				this.WriteEndElement();
			}
			this.WriteEndElement();
		}

		protected override void InternalDispose(bool disposing)
		{
			try
			{
				this.Close();
			}
			catch (InvalidOperationException)
			{
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<RoutingTableLogger>(this);
		}

		private void Close()
		{
			if (this.xmlWriter != null)
			{
				this.xmlWriter.Close();
				this.xmlWriter = null;
			}
		}

		private void WriteCommonConnectorProperties(SendConnector connector)
		{
			this.WriteCommonADObjectProperties(connector);
			this.WriteElement<Unlimited<ByteQuantifiedSize>>("MaxMessageSize", connector.MaxMessageSize);
			if (connector.SourceRoutingGroup != null)
			{
				this.WriteADReference("SourceRoutingGroupId", connector.SourceRoutingGroup);
			}
			this.WriteStartElement("SourceServers");
			foreach (ADObjectId reference in connector.SourceTransportServers)
			{
				this.WriteADReference("SourceServerId", reference);
			}
			this.WriteEndElement();
			if (connector.HomeMtaServerId != null)
			{
				this.WriteADReference("HomeMtaServerId", connector.HomeMtaServerId);
			}
		}

		private void WriteCommonADObjectProperties(ADObject obj)
		{
			this.WriteADObjectIdWithDN(obj.Id);
			this.WriteElement<DateTime?>("WhenCreated", obj.WhenCreatedUTC);
		}

		private void WriteAppConfigSettings(TransportAppConfig.RoutingConfig settings)
		{
			this.WriteStartElement("Settings");
			this.WriteElement<TimeSpan>("ConfigReloadInterval", settings.ConfigReloadInterval);
			this.WriteElement<TimeSpan>("DeferredReloadTimeout", settings.DeferredReloadInterval);
			this.WriteElement<int>("MaxDeferredNotifications", settings.MaxDeferredNotifications);
			this.WriteElement<TimeSpan>("MinConfigReloadInterval", settings.MinConfigReloadInterval);
			this.WriteElement<TimeSpan>("PFReplicaAgeThreshold", settings.PfReplicaAgeThreshold);
			this.WriteElement<bool>("DestinationRoutingToRemoteSitesEnabled", settings.DestinationRoutingToRemoteSitesEnabled);
			this.WriteElement<bool>("DagRoutingEnabled", settings.DagRoutingEnabled);
			this.WriteElement<bool>("RoutingToNonActiveServersEnabled", settings.RoutingToNonActiveServersEnabled);
			this.WriteElement<bool>("SmtpDeliveryToMailboxEnabled", settings.SmtpDeliveryToMailboxEnabled);
			this.WriteElement<int>("ProxyRoutingMaxTotalHubCount", settings.ProxyRoutingMaxTotalHubCount);
			this.WriteElement<int>("ProxyRoutingMaxRemoteSiteHubCount", settings.ProxyRoutingMaxRemoteSiteHubCount);
			if (!RoutingUtils.IsNullOrEmpty<int>(settings.ProxyRoutingAllowedTargetVersions))
			{
				this.WriteStartElement("ProxyRoutingAllowedTargetVersions");
				foreach (int elementValue in settings.ProxyRoutingAllowedTargetVersions)
				{
					this.WriteElement<int>("Version", elementValue);
				}
				this.WriteEndElement();
			}
			if (!RoutingUtils.IsNullOrEmpty<RoutingHost>(settings.OutboundFrontendServers))
			{
				this.WriteStartElement("OutboundFrontendServers");
				foreach (RoutingHost elementValue2 in settings.OutboundFrontendServers)
				{
					this.WriteElement<RoutingHost>("OutboundFrontendServer", elementValue2);
				}
				this.WriteEndElement();
				this.WriteElement<bool>("ExternalOutboundFrontendProxyEnabled", settings.ExternalOutboundFrontendProxyEnabled);
			}
			else
			{
				this.WriteElement<bool>("OutboundProxyRoutingXVersionEnabled", settings.OutboundProxyRoutingXVersionEnabled);
			}
			this.WriteEndElement();
		}

		private void WriteEndDocument()
		{
			this.xmlWriter.WriteEndDocument();
			this.xmlWriter.Flush();
		}

		private const string SchemaVersion = "15.00.0610.000";

		private const string DateTimeFormatSpecifier = "yyyy-MM-ddTHH\\:mm\\:ss.fffZ";

		private XmlTextWriter xmlWriter;
	}
}
