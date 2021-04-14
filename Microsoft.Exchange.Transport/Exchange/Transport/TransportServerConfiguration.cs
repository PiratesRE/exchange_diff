using System;
using System.DirectoryServices;
using System.IO;
using System.Security.AccessControl;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Directory.Transport;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Transport.Configuration;

namespace Microsoft.Exchange.Transport
{
	internal class TransportServerConfiguration
	{
		protected TransportServerConfiguration(Server server, ActiveDirectorySecurity sid)
		{
			this.server = server;
			this.sid = sid;
			this.processorCount = Environment.ProcessorCount;
			if (TransportServerConfiguration.isBridgehead == null)
			{
				TransportServerConfiguration.isBridgehead = new bool?(server.IsHubTransportServer || server.IsFrontendTransportServer);
			}
		}

		public Server TransportServer
		{
			get
			{
				return this.server;
			}
		}

		public string ContentConversionTracingPath
		{
			get
			{
				if (this.TransportServer.ContentConversionTracingEnabled && this.TransportServer.PipelineTracingPath != null)
				{
					return this.TransportServer.PipelineTracingPath.PathName;
				}
				return null;
			}
		}

		public int MaxConcurrentMailboxSubmissions
		{
			get
			{
				return this.processorCount * this.server.MaxConcurrentMailboxSubmissions;
			}
		}

		public int MaxConcurrentMailboxDeliveries
		{
			get
			{
				return this.processorCount * this.server.MaxConcurrentMailboxDeliveries;
			}
		}

		public bool IsBridgehead
		{
			get
			{
				if (TransportServerConfiguration.isBridgehead == null)
				{
					throw new InvalidOperationException("TransportServerConfiguration.isBridgehead should not be null");
				}
				return TransportServerConfiguration.isBridgehead.Value;
			}
		}

		public ActiveDirectorySecurity TransportServerSecurity
		{
			get
			{
				return this.sid;
			}
		}

		private static LocalLongFullPath AddPathSuffix(LocalLongFullPath path, string suffix)
		{
			if (path != null && !string.IsNullOrEmpty(path.PathName))
			{
				return LocalLongFullPath.Parse(Path.Combine(path.PathName, suffix));
			}
			return null;
		}

		private static ActiveDirectorySecurity SetupActiveDirectorySecurity(RawSecurityDescriptor rawSecurityDescriptor)
		{
			ActiveDirectorySecurity result;
			try
			{
				result = TransportADUtils.SetupActiveDirectorySecurity(rawSecurityDescriptor);
			}
			catch (OverflowException ex)
			{
				throw new TransportComponentLoadFailedException(ex.Message, ex);
			}
			return result;
		}

		private static ADObjectId GetNotificationRootId()
		{
			if (TransportServerConfiguration.notificationRootId == null)
			{
				ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(true, ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 201, "GetNotificationRootId", "f:\\15.00.1497\\sources\\dev\\Transport\\src\\Configuration\\TransportServerConfiguration.cs");
				TransportServerConfiguration.notificationRootId = topologyConfigurationSession.FindLocalServer().Id;
			}
			return TransportServerConfiguration.notificationRootId;
		}

		internal void UpdateFrontEndConfiguration(FrontendTransportServer frontendServer, bool cloneAndUpdate)
		{
			lock (TransportServerConfiguration.serverUpdateLock)
			{
				ExTraceGlobals.ConfigurationTracer.TraceDebug(0L, "Frontend server object has been updated. We will reconcile the server object with this change.");
				Server server;
				if (cloneAndUpdate)
				{
					server = (Server)this.server.Clone();
				}
				else
				{
					server = this.server;
				}
				server.SetIsReadOnly(false);
				server.TransientFailureRetryInterval = frontendServer.TransientFailureRetryInterval;
				server.TransientFailureRetryCount = frontendServer.TransientFailureRetryCount;
				server.ReceiveProtocolLogPath = frontendServer.ReceiveProtocolLogPath;
				server.ReceiveProtocolLogMaxAge = frontendServer.ReceiveProtocolLogMaxAge;
				server.ReceiveProtocolLogMaxDirectorySize = frontendServer.ReceiveProtocolLogMaxDirectorySize;
				server.ReceiveProtocolLogMaxFileSize = frontendServer.ReceiveProtocolLogMaxFileSize;
				server.SendProtocolLogPath = frontendServer.SendProtocolLogPath;
				server.SendProtocolLogMaxAge = frontendServer.SendProtocolLogMaxAge;
				server.SendProtocolLogMaxDirectorySize = frontendServer.SendProtocolLogMaxDirectorySize;
				server.SendProtocolLogMaxFileSize = frontendServer.SendProtocolLogMaxFileSize;
				server.InternalDNSAdapterEnabled = frontendServer.InternalDNSAdapterEnabled;
				server.InternalDNSAdapterGuid = frontendServer.InternalDNSAdapterGuid;
				server.InternalDNSServers = frontendServer.InternalDNSServers;
				server.InternalDNSProtocolOption = frontendServer.InternalDNSProtocolOption;
				server.IntraOrgConnectorProtocolLoggingLevel = frontendServer.IntraOrgConnectorProtocolLoggingLevel;
				server.ExternalDNSAdapterEnabled = frontendServer.ExternalDNSAdapterEnabled;
				server.ExternalDNSAdapterGuid = frontendServer.ExternalDNSAdapterGuid;
				server.ExternalDNSServers = frontendServer.ExternalDNSServers;
				server.ExternalDNSProtocolOption = frontendServer.ExternalDNSProtocolOption;
				server.ExternalIPAddress = frontendServer.ExternalIPAddress;
				server.ConnectivityLogEnabled = frontendServer.ConnectivityLogEnabled;
				server.ConnectivityLogPath = frontendServer.ConnectivityLogPath;
				server.ConnectivityLogMaxAge = frontendServer.ConnectivityLogMaxAge;
				server.ConnectivityLogMaxDirectorySize = frontendServer.ConnectivityLogMaxDirectorySize;
				server.ConnectivityLogMaxFileSize = frontendServer.ConnectivityLogMaxFileSize;
				server.AntispamAgentsEnabled = frontendServer.AntispamAgentsEnabled;
				server.MaxConnectionRatePerMinute = frontendServer.MaxConnectionRatePerMinute;
				server.MaxOutboundConnections = frontendServer.MaxOutboundConnections;
				server.MaxPerDomainOutboundConnections = frontendServer.MaxPerDomainOutboundConnections;
				server.IntraOrgConnectorSmtpMaxMessagesPerConnection = frontendServer.IntraOrgConnectorSmtpMaxMessagesPerConnection;
				server.AgentLogEnabled = frontendServer.AgentLogEnabled;
				server.AgentLogMaxAge = frontendServer.AgentLogMaxAge;
				server.AgentLogMaxDirectorySize = frontendServer.AgentLogMaxDirectorySize;
				server.AgentLogMaxFileSize = frontendServer.AgentLogMaxFileSize;
				server.AgentLogPath = frontendServer.AgentLogPath;
				server.DnsLogEnabled = frontendServer.DnsLogEnabled;
				server.DnsLogMaxAge = frontendServer.DnsLogMaxAge;
				server.DnsLogMaxDirectorySize = frontendServer.DnsLogMaxDirectorySize;
				server.DnsLogMaxFileSize = frontendServer.DnsLogMaxFileSize;
				server.DnsLogPath = frontendServer.DnsLogPath;
				server.ResourceLogEnabled = frontendServer.ResourceLogEnabled;
				server.ResourceLogMaxAge = frontendServer.ResourceLogMaxAge;
				server.ResourceLogMaxDirectorySize = frontendServer.ResourceLogMaxDirectorySize;
				server.ResourceLogMaxFileSize = frontendServer.ResourceLogMaxFileSize;
				server.ResourceLogPath = frontendServer.ResourceLogPath;
				server.AttributionLogEnabled = frontendServer.AttributionLogEnabled;
				server.AttributionLogMaxAge = frontendServer.AttributionLogMaxAge;
				server.AttributionLogMaxDirectorySize = frontendServer.AttributionLogMaxDirectorySize;
				server.AttributionLogMaxFileSize = frontendServer.AttributionLogMaxFileSize;
				server.AttributionLogPath = frontendServer.AttributionLogPath;
				server.MaxReceiveTlsRatePerMinute = frontendServer.MaxReceiveTlsRatePerMinute;
				server.SetIsReadOnly(true);
				this.server = server;
			}
		}

		internal void UpdateMailboxConfiguration(MailboxTransportServer mailboxServer, string pathSuffix, bool cloneAndUpdate)
		{
			lock (TransportServerConfiguration.serverUpdateLock)
			{
				ExTraceGlobals.ConfigurationTracer.TraceDebug(0L, "Mailbox server object has been updated. We will reconcile the server object with this change.");
				Server server;
				if (cloneAndUpdate)
				{
					server = (Server)this.server.Clone();
				}
				else
				{
					server = this.server;
				}
				server.SetIsReadOnly(false);
				server.ReceiveProtocolLogPath = TransportServerConfiguration.AddPathSuffix(mailboxServer.ReceiveProtocolLogPath, pathSuffix);
				server.ReceiveProtocolLogMaxAge = mailboxServer.ReceiveProtocolLogMaxAge;
				server.ReceiveProtocolLogMaxDirectorySize = mailboxServer.ReceiveProtocolLogMaxDirectorySize;
				server.ReceiveProtocolLogMaxFileSize = mailboxServer.ReceiveProtocolLogMaxFileSize;
				server.SendProtocolLogPath = TransportServerConfiguration.AddPathSuffix(mailboxServer.SendProtocolLogPath, pathSuffix);
				server.SendProtocolLogMaxAge = mailboxServer.SendProtocolLogMaxAge;
				server.SendProtocolLogMaxDirectorySize = mailboxServer.SendProtocolLogMaxDirectorySize;
				server.SendProtocolLogMaxFileSize = mailboxServer.SendProtocolLogMaxFileSize;
				server.ConnectivityLogEnabled = mailboxServer.ConnectivityLogEnabled;
				server.ConnectivityLogPath = TransportServerConfiguration.AddPathSuffix(mailboxServer.ConnectivityLogPath, pathSuffix);
				server.ConnectivityLogMaxAge = mailboxServer.ConnectivityLogMaxAge;
				server.ConnectivityLogMaxDirectorySize = mailboxServer.ConnectivityLogMaxDirectorySize;
				server.ConnectivityLogMaxFileSize = mailboxServer.ConnectivityLogMaxFileSize;
				server.MaxConcurrentMailboxDeliveries = mailboxServer.MaxConcurrentMailboxDeliveries;
				server.MaxConcurrentMailboxSubmissions = mailboxServer.MaxConcurrentMailboxSubmissions;
				server.ContentConversionTracingEnabled = mailboxServer.ContentConversionTracingEnabled;
				server.PipelineTracingEnabled = mailboxServer.PipelineTracingEnabled;
				server.PipelineTracingPath = TransportServerConfiguration.AddPathSuffix(mailboxServer.PipelineTracingPath, pathSuffix);
				server.PipelineTracingSenderAddress = mailboxServer.PipelineTracingSenderAddress;
				server.InMemoryReceiveConnectorProtocolLoggingLevel = mailboxServer.InMemoryReceiveConnectorProtocolLoggingLevel;
				server.InMemoryReceiveConnectorSmtpUtf8Enabled = mailboxServer.InMemoryReceiveConnectorSmtpUtf8Enabled;
				server.MailboxDeliveryAgentLogEnabled = mailboxServer.MailboxDeliveryAgentLogEnabled;
				server.MailboxDeliveryAgentLogMaxAge = mailboxServer.MailboxDeliveryAgentLogMaxAge;
				server.MailboxDeliveryAgentLogMaxDirectorySize = mailboxServer.MailboxDeliveryAgentLogMaxDirectorySize;
				server.MailboxDeliveryAgentLogMaxFileSize = mailboxServer.MailboxDeliveryAgentLogMaxFileSize;
				server.MailboxDeliveryAgentLogPath = mailboxServer.MailboxDeliveryAgentLogPath;
				server.MailboxDeliveryThrottlingLogEnabled = mailboxServer.MailboxDeliveryThrottlingLogEnabled;
				server.MailboxDeliveryThrottlingLogMaxAge = mailboxServer.MailboxDeliveryThrottlingLogMaxAge;
				server.MailboxDeliveryThrottlingLogMaxDirectorySize = mailboxServer.MailboxDeliveryThrottlingLogMaxDirectorySize;
				server.MailboxDeliveryThrottlingLogMaxFileSize = mailboxServer.MailboxDeliveryThrottlingLogMaxFileSize;
				server.MailboxDeliveryThrottlingLogPath = mailboxServer.MailboxDeliveryThrottlingLogPath;
				server.MailboxSubmissionAgentLogEnabled = mailboxServer.MailboxSubmissionAgentLogEnabled;
				server.MailboxSubmissionAgentLogMaxAge = mailboxServer.MailboxSubmissionAgentLogMaxAge;
				server.MailboxSubmissionAgentLogMaxDirectorySize = mailboxServer.MailboxSubmissionAgentLogMaxDirectorySize;
				server.MailboxSubmissionAgentLogMaxFileSize = mailboxServer.MailboxSubmissionAgentLogMaxFileSize;
				server.MailboxSubmissionAgentLogPath = mailboxServer.MailboxSubmissionAgentLogPath;
				server.SetIsReadOnly(true);
				this.server = server;
			}
		}

		private static bool? isBridgehead;

		private static readonly object serverUpdateLock = new object();

		private static ADObjectId notificationRootId;

		private Server server;

		private readonly ActiveDirectorySecurity sid;

		private readonly int processorCount;

		internal class Builder : ConfigurationLoader<TransportServerConfiguration, TransportServerConfiguration.Builder>.Builder
		{
			public override void Register()
			{
				base.Register<Server>(new Func<ADObjectId>(TransportServerConfiguration.GetNotificationRootId));
			}

			public bool RoleCheck
			{
				set
				{
					this.roleCheck = value;
				}
			}

			public override void LoadData(ITopologyConfigurationSession session, QueryScope scope)
			{
				Server server = session.FindLocalServer();
				if (server != null)
				{
					RawSecurityDescriptor rawSecurityDescriptor = server.ReadSecurityDescriptor();
					if (rawSecurityDescriptor != null)
					{
						ActiveDirectorySecurity sid = TransportServerConfiguration.SetupActiveDirectorySecurity(rawSecurityDescriptor);
						this.config = new TransportServerConfiguration(server, sid);
					}
				}
			}

			public override TransportServerConfiguration BuildCache()
			{
				if (this.config != null)
				{
					if (!this.roleCheck)
					{
						return this.config;
					}
					if (this.config.TransportServer.IsHubTransportServer || this.config.TransportServer.IsEdgeServer)
					{
						if (TransportServerConfiguration.isBridgehead.Value == this.config.TransportServer.IsHubTransportServer)
						{
							return this.config;
						}
						base.FailureMessage = Strings.InvalidRoleChange;
					}
					else
					{
						if (this.config.TransportServer.IsFrontendTransportServer || this.config.TransportServer.IsMailboxServer)
						{
							return this.config;
						}
						base.FailureMessage = Strings.InvalidTransportServerRole;
					}
				}
				return null;
			}

			private TransportServerConfiguration config;

			private bool roleCheck = true;
		}

		internal class FrontendBuilder : ConfigurationLoader<FrontendTransportServer, TransportServerConfiguration.FrontendBuilder>.Builder
		{
			public override void Register()
			{
				base.Register<FrontendTransportServer>(new Func<ADObjectId>(TransportServerConfiguration.GetNotificationRootId));
			}

			public override void LoadData(ITopologyConfigurationSession session, QueryScope scope)
			{
				Server server = session.FindLocalServer();
				if (server != null)
				{
					ADObjectId childId = server.Id.GetChildId("Transport Configuration");
					ADObjectId childId2 = childId.GetChildId("Frontend");
					FrontendTransportServer frontendTransportServer = session.Read<FrontendTransportServer>(childId2);
					if (frontendTransportServer != null)
					{
						RawSecurityDescriptor rawSecurityDescriptor = frontendTransportServer.ReadSecurityDescriptor();
						if (rawSecurityDescriptor != null)
						{
							TransportServerConfiguration.SetupActiveDirectorySecurity(rawSecurityDescriptor);
							this.frontEndServer = frontendTransportServer;
						}
					}
				}
			}

			public override FrontendTransportServer BuildCache()
			{
				if (this.frontEndServer == null)
				{
					return null;
				}
				if (this.frontEndServer.IsFrontendTransportServer)
				{
					return this.frontEndServer;
				}
				throw new InvalidOperationException(string.Format("The Frontend object's server role has been set to an invalid role: {0}", this.frontEndServer.CurrentServerRole));
			}

			private FrontendTransportServer frontEndServer;
		}

		internal class MailboxBuilder : ConfigurationLoader<MailboxTransportServer, TransportServerConfiguration.MailboxBuilder>.Builder
		{
			public override void Register()
			{
				base.Register<MailboxTransportServer>(new Func<ADObjectId>(TransportServerConfiguration.GetNotificationRootId));
			}

			public override void LoadData(ITopologyConfigurationSession session, QueryScope scope)
			{
				Server server = session.FindLocalServer();
				if (server != null)
				{
					ADObjectId childId = server.Id.GetChildId("Transport Configuration");
					ADObjectId childId2 = childId.GetChildId("Mailbox");
					MailboxTransportServer mailboxTransportServer = session.Read<MailboxTransportServer>(childId2);
					if (mailboxTransportServer != null)
					{
						RawSecurityDescriptor rawSecurityDescriptor = mailboxTransportServer.ReadSecurityDescriptor();
						if (rawSecurityDescriptor != null)
						{
							TransportServerConfiguration.SetupActiveDirectorySecurity(rawSecurityDescriptor);
							this.mailboxServer = mailboxTransportServer;
						}
					}
				}
			}

			public override MailboxTransportServer BuildCache()
			{
				if (this.mailboxServer == null)
				{
					return null;
				}
				if (this.mailboxServer.IsMailboxServer)
				{
					return this.mailboxServer;
				}
				throw new InvalidOperationException(string.Format("The Mailbox object's server role has been set to an invalid role: {0}", this.mailboxServer.CurrentServerRole));
			}

			private MailboxTransportServer mailboxServer;
		}
	}
}
