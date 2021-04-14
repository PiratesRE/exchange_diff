using System;
using System.Management.Automation;
using System.Net;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Management;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class SetTransportServiceCommand : SyntheticCommandWithPipelineInputNoOutput<TransportServer>
	{
		private SetTransportServiceCommand() : base("Set-TransportService")
		{
		}

		public SetTransportServiceCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual SetTransportServiceCommand SetParameters(SetTransportServiceCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SetTransportServiceCommand SetParameters(SetTransportServiceCommand.IdentityParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual EnhancedTimeSpan QueueLogMaxAge
			{
				set
				{
					base.PowerSharpParameters["QueueLogMaxAge"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> QueueLogMaxDirectorySize
			{
				set
				{
					base.PowerSharpParameters["QueueLogMaxDirectorySize"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> QueueLogMaxFileSize
			{
				set
				{
					base.PowerSharpParameters["QueueLogMaxFileSize"] = value;
				}
			}

			public virtual LocalLongFullPath QueueLogPath
			{
				set
				{
					base.PowerSharpParameters["QueueLogPath"] = value;
				}
			}

			public virtual EnhancedTimeSpan WlmLogMaxAge
			{
				set
				{
					base.PowerSharpParameters["WlmLogMaxAge"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> WlmLogMaxDirectorySize
			{
				set
				{
					base.PowerSharpParameters["WlmLogMaxDirectorySize"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> WlmLogMaxFileSize
			{
				set
				{
					base.PowerSharpParameters["WlmLogMaxFileSize"] = value;
				}
			}

			public virtual LocalLongFullPath WlmLogPath
			{
				set
				{
					base.PowerSharpParameters["WlmLogPath"] = value;
				}
			}

			public virtual EnhancedTimeSpan AgentLogMaxAge
			{
				set
				{
					base.PowerSharpParameters["AgentLogMaxAge"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> AgentLogMaxDirectorySize
			{
				set
				{
					base.PowerSharpParameters["AgentLogMaxDirectorySize"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> AgentLogMaxFileSize
			{
				set
				{
					base.PowerSharpParameters["AgentLogMaxFileSize"] = value;
				}
			}

			public virtual LocalLongFullPath AgentLogPath
			{
				set
				{
					base.PowerSharpParameters["AgentLogPath"] = value;
				}
			}

			public virtual bool AgentLogEnabled
			{
				set
				{
					base.PowerSharpParameters["AgentLogEnabled"] = value;
				}
			}

			public virtual EnhancedTimeSpan FlowControlLogMaxAge
			{
				set
				{
					base.PowerSharpParameters["FlowControlLogMaxAge"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> FlowControlLogMaxDirectorySize
			{
				set
				{
					base.PowerSharpParameters["FlowControlLogMaxDirectorySize"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> FlowControlLogMaxFileSize
			{
				set
				{
					base.PowerSharpParameters["FlowControlLogMaxFileSize"] = value;
				}
			}

			public virtual LocalLongFullPath FlowControlLogPath
			{
				set
				{
					base.PowerSharpParameters["FlowControlLogPath"] = value;
				}
			}

			public virtual bool FlowControlLogEnabled
			{
				set
				{
					base.PowerSharpParameters["FlowControlLogEnabled"] = value;
				}
			}

			public virtual EnhancedTimeSpan ProcessingSchedulerLogMaxAge
			{
				set
				{
					base.PowerSharpParameters["ProcessingSchedulerLogMaxAge"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> ProcessingSchedulerLogMaxDirectorySize
			{
				set
				{
					base.PowerSharpParameters["ProcessingSchedulerLogMaxDirectorySize"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> ProcessingSchedulerLogMaxFileSize
			{
				set
				{
					base.PowerSharpParameters["ProcessingSchedulerLogMaxFileSize"] = value;
				}
			}

			public virtual LocalLongFullPath ProcessingSchedulerLogPath
			{
				set
				{
					base.PowerSharpParameters["ProcessingSchedulerLogPath"] = value;
				}
			}

			public virtual bool ProcessingSchedulerLogEnabled
			{
				set
				{
					base.PowerSharpParameters["ProcessingSchedulerLogEnabled"] = value;
				}
			}

			public virtual EnhancedTimeSpan ResourceLogMaxAge
			{
				set
				{
					base.PowerSharpParameters["ResourceLogMaxAge"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> ResourceLogMaxDirectorySize
			{
				set
				{
					base.PowerSharpParameters["ResourceLogMaxDirectorySize"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> ResourceLogMaxFileSize
			{
				set
				{
					base.PowerSharpParameters["ResourceLogMaxFileSize"] = value;
				}
			}

			public virtual LocalLongFullPath ResourceLogPath
			{
				set
				{
					base.PowerSharpParameters["ResourceLogPath"] = value;
				}
			}

			public virtual bool ResourceLogEnabled
			{
				set
				{
					base.PowerSharpParameters["ResourceLogEnabled"] = value;
				}
			}

			public virtual EnhancedTimeSpan DnsLogMaxAge
			{
				set
				{
					base.PowerSharpParameters["DnsLogMaxAge"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> DnsLogMaxDirectorySize
			{
				set
				{
					base.PowerSharpParameters["DnsLogMaxDirectorySize"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> DnsLogMaxFileSize
			{
				set
				{
					base.PowerSharpParameters["DnsLogMaxFileSize"] = value;
				}
			}

			public virtual LocalLongFullPath DnsLogPath
			{
				set
				{
					base.PowerSharpParameters["DnsLogPath"] = value;
				}
			}

			public virtual bool DnsLogEnabled
			{
				set
				{
					base.PowerSharpParameters["DnsLogEnabled"] = value;
				}
			}

			public virtual EnhancedTimeSpan JournalLogMaxAge
			{
				set
				{
					base.PowerSharpParameters["JournalLogMaxAge"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> JournalLogMaxDirectorySize
			{
				set
				{
					base.PowerSharpParameters["JournalLogMaxDirectorySize"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> JournalLogMaxFileSize
			{
				set
				{
					base.PowerSharpParameters["JournalLogMaxFileSize"] = value;
				}
			}

			public virtual LocalLongFullPath JournalLogPath
			{
				set
				{
					base.PowerSharpParameters["JournalLogPath"] = value;
				}
			}

			public virtual bool JournalLogEnabled
			{
				set
				{
					base.PowerSharpParameters["JournalLogEnabled"] = value;
				}
			}

			public virtual EnhancedTimeSpan TransportMaintenanceLogMaxAge
			{
				set
				{
					base.PowerSharpParameters["TransportMaintenanceLogMaxAge"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> TransportMaintenanceLogMaxDirectorySize
			{
				set
				{
					base.PowerSharpParameters["TransportMaintenanceLogMaxDirectorySize"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> TransportMaintenanceLogMaxFileSize
			{
				set
				{
					base.PowerSharpParameters["TransportMaintenanceLogMaxFileSize"] = value;
				}
			}

			public virtual LocalLongFullPath TransportMaintenanceLogPath
			{
				set
				{
					base.PowerSharpParameters["TransportMaintenanceLogPath"] = value;
				}
			}

			public virtual bool TransportMaintenanceLogEnabled
			{
				set
				{
					base.PowerSharpParameters["TransportMaintenanceLogEnabled"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual bool AntispamAgentsEnabled
			{
				set
				{
					base.PowerSharpParameters["AntispamAgentsEnabled"] = value;
				}
			}

			public virtual bool ConnectivityLogEnabled
			{
				set
				{
					base.PowerSharpParameters["ConnectivityLogEnabled"] = value;
				}
			}

			public virtual EnhancedTimeSpan ConnectivityLogMaxAge
			{
				set
				{
					base.PowerSharpParameters["ConnectivityLogMaxAge"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> ConnectivityLogMaxDirectorySize
			{
				set
				{
					base.PowerSharpParameters["ConnectivityLogMaxDirectorySize"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> ConnectivityLogMaxFileSize
			{
				set
				{
					base.PowerSharpParameters["ConnectivityLogMaxFileSize"] = value;
				}
			}

			public virtual LocalLongFullPath ConnectivityLogPath
			{
				set
				{
					base.PowerSharpParameters["ConnectivityLogPath"] = value;
				}
			}

			public virtual EnhancedTimeSpan DelayNotificationTimeout
			{
				set
				{
					base.PowerSharpParameters["DelayNotificationTimeout"] = value;
				}
			}

			public virtual bool ExternalDNSAdapterEnabled
			{
				set
				{
					base.PowerSharpParameters["ExternalDNSAdapterEnabled"] = value;
				}
			}

			public virtual Guid ExternalDNSAdapterGuid
			{
				set
				{
					base.PowerSharpParameters["ExternalDNSAdapterGuid"] = value;
				}
			}

			public virtual ProtocolOption ExternalDNSProtocolOption
			{
				set
				{
					base.PowerSharpParameters["ExternalDNSProtocolOption"] = value;
				}
			}

			public virtual MultiValuedProperty<IPAddress> ExternalDNSServers
			{
				set
				{
					base.PowerSharpParameters["ExternalDNSServers"] = value;
				}
			}

			public virtual IPAddress ExternalIPAddress
			{
				set
				{
					base.PowerSharpParameters["ExternalIPAddress"] = value;
				}
			}

			public virtual bool InternalDNSAdapterEnabled
			{
				set
				{
					base.PowerSharpParameters["InternalDNSAdapterEnabled"] = value;
				}
			}

			public virtual Guid InternalDNSAdapterGuid
			{
				set
				{
					base.PowerSharpParameters["InternalDNSAdapterGuid"] = value;
				}
			}

			public virtual ProtocolOption InternalDNSProtocolOption
			{
				set
				{
					base.PowerSharpParameters["InternalDNSProtocolOption"] = value;
				}
			}

			public virtual MultiValuedProperty<IPAddress> InternalDNSServers
			{
				set
				{
					base.PowerSharpParameters["InternalDNSServers"] = value;
				}
			}

			public virtual int MaxConcurrentMailboxDeliveries
			{
				set
				{
					base.PowerSharpParameters["MaxConcurrentMailboxDeliveries"] = value;
				}
			}

			public virtual int MaxConcurrentMailboxSubmissions
			{
				set
				{
					base.PowerSharpParameters["MaxConcurrentMailboxSubmissions"] = value;
				}
			}

			public virtual int MaxConnectionRatePerMinute
			{
				set
				{
					base.PowerSharpParameters["MaxConnectionRatePerMinute"] = value;
				}
			}

			public virtual Unlimited<int> MaxOutboundConnections
			{
				set
				{
					base.PowerSharpParameters["MaxOutboundConnections"] = value;
				}
			}

			public virtual Unlimited<int> MaxPerDomainOutboundConnections
			{
				set
				{
					base.PowerSharpParameters["MaxPerDomainOutboundConnections"] = value;
				}
			}

			public virtual EnhancedTimeSpan MessageExpirationTimeout
			{
				set
				{
					base.PowerSharpParameters["MessageExpirationTimeout"] = value;
				}
			}

			public virtual EnhancedTimeSpan MessageRetryInterval
			{
				set
				{
					base.PowerSharpParameters["MessageRetryInterval"] = value;
				}
			}

			public virtual bool MessageTrackingLogEnabled
			{
				set
				{
					base.PowerSharpParameters["MessageTrackingLogEnabled"] = value;
				}
			}

			public virtual EnhancedTimeSpan MessageTrackingLogMaxAge
			{
				set
				{
					base.PowerSharpParameters["MessageTrackingLogMaxAge"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> MessageTrackingLogMaxDirectorySize
			{
				set
				{
					base.PowerSharpParameters["MessageTrackingLogMaxDirectorySize"] = value;
				}
			}

			public virtual ByteQuantifiedSize MessageTrackingLogMaxFileSize
			{
				set
				{
					base.PowerSharpParameters["MessageTrackingLogMaxFileSize"] = value;
				}
			}

			public virtual LocalLongFullPath MessageTrackingLogPath
			{
				set
				{
					base.PowerSharpParameters["MessageTrackingLogPath"] = value;
				}
			}

			public virtual bool IrmLogEnabled
			{
				set
				{
					base.PowerSharpParameters["IrmLogEnabled"] = value;
				}
			}

			public virtual EnhancedTimeSpan IrmLogMaxAge
			{
				set
				{
					base.PowerSharpParameters["IrmLogMaxAge"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> IrmLogMaxDirectorySize
			{
				set
				{
					base.PowerSharpParameters["IrmLogMaxDirectorySize"] = value;
				}
			}

			public virtual ByteQuantifiedSize IrmLogMaxFileSize
			{
				set
				{
					base.PowerSharpParameters["IrmLogMaxFileSize"] = value;
				}
			}

			public virtual LocalLongFullPath IrmLogPath
			{
				set
				{
					base.PowerSharpParameters["IrmLogPath"] = value;
				}
			}

			public virtual EnhancedTimeSpan ActiveUserStatisticsLogMaxAge
			{
				set
				{
					base.PowerSharpParameters["ActiveUserStatisticsLogMaxAge"] = value;
				}
			}

			public virtual ByteQuantifiedSize ActiveUserStatisticsLogMaxDirectorySize
			{
				set
				{
					base.PowerSharpParameters["ActiveUserStatisticsLogMaxDirectorySize"] = value;
				}
			}

			public virtual ByteQuantifiedSize ActiveUserStatisticsLogMaxFileSize
			{
				set
				{
					base.PowerSharpParameters["ActiveUserStatisticsLogMaxFileSize"] = value;
				}
			}

			public virtual LocalLongFullPath ActiveUserStatisticsLogPath
			{
				set
				{
					base.PowerSharpParameters["ActiveUserStatisticsLogPath"] = value;
				}
			}

			public virtual EnhancedTimeSpan ServerStatisticsLogMaxAge
			{
				set
				{
					base.PowerSharpParameters["ServerStatisticsLogMaxAge"] = value;
				}
			}

			public virtual ByteQuantifiedSize ServerStatisticsLogMaxDirectorySize
			{
				set
				{
					base.PowerSharpParameters["ServerStatisticsLogMaxDirectorySize"] = value;
				}
			}

			public virtual ByteQuantifiedSize ServerStatisticsLogMaxFileSize
			{
				set
				{
					base.PowerSharpParameters["ServerStatisticsLogMaxFileSize"] = value;
				}
			}

			public virtual LocalLongFullPath ServerStatisticsLogPath
			{
				set
				{
					base.PowerSharpParameters["ServerStatisticsLogPath"] = value;
				}
			}

			public virtual bool MessageTrackingLogSubjectLoggingEnabled
			{
				set
				{
					base.PowerSharpParameters["MessageTrackingLogSubjectLoggingEnabled"] = value;
				}
			}

			public virtual EnhancedTimeSpan OutboundConnectionFailureRetryInterval
			{
				set
				{
					base.PowerSharpParameters["OutboundConnectionFailureRetryInterval"] = value;
				}
			}

			public virtual ProtocolLoggingLevel IntraOrgConnectorProtocolLoggingLevel
			{
				set
				{
					base.PowerSharpParameters["IntraOrgConnectorProtocolLoggingLevel"] = value;
				}
			}

			public virtual ByteQuantifiedSize PickupDirectoryMaxHeaderSize
			{
				set
				{
					base.PowerSharpParameters["PickupDirectoryMaxHeaderSize"] = value;
				}
			}

			public virtual int PickupDirectoryMaxMessagesPerMinute
			{
				set
				{
					base.PowerSharpParameters["PickupDirectoryMaxMessagesPerMinute"] = value;
				}
			}

			public virtual int PickupDirectoryMaxRecipientsPerMessage
			{
				set
				{
					base.PowerSharpParameters["PickupDirectoryMaxRecipientsPerMessage"] = value;
				}
			}

			public virtual LocalLongFullPath PickupDirectoryPath
			{
				set
				{
					base.PowerSharpParameters["PickupDirectoryPath"] = value;
				}
			}

			public virtual bool PipelineTracingEnabled
			{
				set
				{
					base.PowerSharpParameters["PipelineTracingEnabled"] = value;
				}
			}

			public virtual bool ContentConversionTracingEnabled
			{
				set
				{
					base.PowerSharpParameters["ContentConversionTracingEnabled"] = value;
				}
			}

			public virtual LocalLongFullPath PipelineTracingPath
			{
				set
				{
					base.PowerSharpParameters["PipelineTracingPath"] = value;
				}
			}

			public virtual SmtpAddress? PipelineTracingSenderAddress
			{
				set
				{
					base.PowerSharpParameters["PipelineTracingSenderAddress"] = value;
				}
			}

			public virtual bool PoisonMessageDetectionEnabled
			{
				set
				{
					base.PowerSharpParameters["PoisonMessageDetectionEnabled"] = value;
				}
			}

			public virtual int PoisonThreshold
			{
				set
				{
					base.PowerSharpParameters["PoisonThreshold"] = value;
				}
			}

			public virtual EnhancedTimeSpan QueueMaxIdleTime
			{
				set
				{
					base.PowerSharpParameters["QueueMaxIdleTime"] = value;
				}
			}

			public virtual EnhancedTimeSpan ReceiveProtocolLogMaxAge
			{
				set
				{
					base.PowerSharpParameters["ReceiveProtocolLogMaxAge"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> ReceiveProtocolLogMaxDirectorySize
			{
				set
				{
					base.PowerSharpParameters["ReceiveProtocolLogMaxDirectorySize"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> ReceiveProtocolLogMaxFileSize
			{
				set
				{
					base.PowerSharpParameters["ReceiveProtocolLogMaxFileSize"] = value;
				}
			}

			public virtual LocalLongFullPath ReceiveProtocolLogPath
			{
				set
				{
					base.PowerSharpParameters["ReceiveProtocolLogPath"] = value;
				}
			}

			public virtual bool RecipientValidationCacheEnabled
			{
				set
				{
					base.PowerSharpParameters["RecipientValidationCacheEnabled"] = value;
				}
			}

			public virtual LocalLongFullPath ReplayDirectoryPath
			{
				set
				{
					base.PowerSharpParameters["ReplayDirectoryPath"] = value;
				}
			}

			public virtual string RootDropDirectoryPath
			{
				set
				{
					base.PowerSharpParameters["RootDropDirectoryPath"] = value;
				}
			}

			public virtual EnhancedTimeSpan RoutingTableLogMaxAge
			{
				set
				{
					base.PowerSharpParameters["RoutingTableLogMaxAge"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> RoutingTableLogMaxDirectorySize
			{
				set
				{
					base.PowerSharpParameters["RoutingTableLogMaxDirectorySize"] = value;
				}
			}

			public virtual LocalLongFullPath RoutingTableLogPath
			{
				set
				{
					base.PowerSharpParameters["RoutingTableLogPath"] = value;
				}
			}

			public virtual EnhancedTimeSpan SendProtocolLogMaxAge
			{
				set
				{
					base.PowerSharpParameters["SendProtocolLogMaxAge"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> SendProtocolLogMaxDirectorySize
			{
				set
				{
					base.PowerSharpParameters["SendProtocolLogMaxDirectorySize"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> SendProtocolLogMaxFileSize
			{
				set
				{
					base.PowerSharpParameters["SendProtocolLogMaxFileSize"] = value;
				}
			}

			public virtual LocalLongFullPath SendProtocolLogPath
			{
				set
				{
					base.PowerSharpParameters["SendProtocolLogPath"] = value;
				}
			}

			public virtual int TransientFailureRetryCount
			{
				set
				{
					base.PowerSharpParameters["TransientFailureRetryCount"] = value;
				}
			}

			public virtual EnhancedTimeSpan TransientFailureRetryInterval
			{
				set
				{
					base.PowerSharpParameters["TransientFailureRetryInterval"] = value;
				}
			}

			public virtual bool TransportSyncEnabled
			{
				set
				{
					base.PowerSharpParameters["TransportSyncEnabled"] = value;
				}
			}

			public virtual bool TransportSyncPopEnabled
			{
				set
				{
					base.PowerSharpParameters["TransportSyncPopEnabled"] = value;
				}
			}

			public virtual bool WindowsLiveHotmailTransportSyncEnabled
			{
				set
				{
					base.PowerSharpParameters["WindowsLiveHotmailTransportSyncEnabled"] = value;
				}
			}

			public virtual bool TransportSyncExchangeEnabled
			{
				set
				{
					base.PowerSharpParameters["TransportSyncExchangeEnabled"] = value;
				}
			}

			public virtual bool TransportSyncImapEnabled
			{
				set
				{
					base.PowerSharpParameters["TransportSyncImapEnabled"] = value;
				}
			}

			public virtual int MaxNumberOfTransportSyncAttempts
			{
				set
				{
					base.PowerSharpParameters["MaxNumberOfTransportSyncAttempts"] = value;
				}
			}

			public virtual int MaxActiveTransportSyncJobsPerProcessor
			{
				set
				{
					base.PowerSharpParameters["MaxActiveTransportSyncJobsPerProcessor"] = value;
				}
			}

			public virtual string HttpTransportSyncProxyServer
			{
				set
				{
					base.PowerSharpParameters["HttpTransportSyncProxyServer"] = value;
				}
			}

			public virtual bool HttpProtocolLogEnabled
			{
				set
				{
					base.PowerSharpParameters["HttpProtocolLogEnabled"] = value;
				}
			}

			public virtual LocalLongFullPath HttpProtocolLogFilePath
			{
				set
				{
					base.PowerSharpParameters["HttpProtocolLogFilePath"] = value;
				}
			}

			public virtual EnhancedTimeSpan HttpProtocolLogMaxAge
			{
				set
				{
					base.PowerSharpParameters["HttpProtocolLogMaxAge"] = value;
				}
			}

			public virtual ByteQuantifiedSize HttpProtocolLogMaxDirectorySize
			{
				set
				{
					base.PowerSharpParameters["HttpProtocolLogMaxDirectorySize"] = value;
				}
			}

			public virtual ByteQuantifiedSize HttpProtocolLogMaxFileSize
			{
				set
				{
					base.PowerSharpParameters["HttpProtocolLogMaxFileSize"] = value;
				}
			}

			public virtual ProtocolLoggingLevel HttpProtocolLogLoggingLevel
			{
				set
				{
					base.PowerSharpParameters["HttpProtocolLogLoggingLevel"] = value;
				}
			}

			public virtual bool TransportSyncLogEnabled
			{
				set
				{
					base.PowerSharpParameters["TransportSyncLogEnabled"] = value;
				}
			}

			public virtual LocalLongFullPath TransportSyncLogFilePath
			{
				set
				{
					base.PowerSharpParameters["TransportSyncLogFilePath"] = value;
				}
			}

			public virtual SyncLoggingLevel TransportSyncLogLoggingLevel
			{
				set
				{
					base.PowerSharpParameters["TransportSyncLogLoggingLevel"] = value;
				}
			}

			public virtual EnhancedTimeSpan TransportSyncLogMaxAge
			{
				set
				{
					base.PowerSharpParameters["TransportSyncLogMaxAge"] = value;
				}
			}

			public virtual ByteQuantifiedSize TransportSyncLogMaxDirectorySize
			{
				set
				{
					base.PowerSharpParameters["TransportSyncLogMaxDirectorySize"] = value;
				}
			}

			public virtual ByteQuantifiedSize TransportSyncLogMaxFileSize
			{
				set
				{
					base.PowerSharpParameters["TransportSyncLogMaxFileSize"] = value;
				}
			}

			public virtual bool TransportSyncHubHealthLogEnabled
			{
				set
				{
					base.PowerSharpParameters["TransportSyncHubHealthLogEnabled"] = value;
				}
			}

			public virtual LocalLongFullPath TransportSyncHubHealthLogFilePath
			{
				set
				{
					base.PowerSharpParameters["TransportSyncHubHealthLogFilePath"] = value;
				}
			}

			public virtual EnhancedTimeSpan TransportSyncHubHealthLogMaxAge
			{
				set
				{
					base.PowerSharpParameters["TransportSyncHubHealthLogMaxAge"] = value;
				}
			}

			public virtual ByteQuantifiedSize TransportSyncHubHealthLogMaxDirectorySize
			{
				set
				{
					base.PowerSharpParameters["TransportSyncHubHealthLogMaxDirectorySize"] = value;
				}
			}

			public virtual ByteQuantifiedSize TransportSyncHubHealthLogMaxFileSize
			{
				set
				{
					base.PowerSharpParameters["TransportSyncHubHealthLogMaxFileSize"] = value;
				}
			}

			public virtual bool TransportSyncAccountsPoisonDetectionEnabled
			{
				set
				{
					base.PowerSharpParameters["TransportSyncAccountsPoisonDetectionEnabled"] = value;
				}
			}

			public virtual int TransportSyncAccountsPoisonAccountThreshold
			{
				set
				{
					base.PowerSharpParameters["TransportSyncAccountsPoisonAccountThreshold"] = value;
				}
			}

			public virtual int TransportSyncAccountsPoisonItemThreshold
			{
				set
				{
					base.PowerSharpParameters["TransportSyncAccountsPoisonItemThreshold"] = value;
				}
			}

			public virtual int TransportSyncAccountsSuccessivePoisonItemThreshold
			{
				set
				{
					base.PowerSharpParameters["TransportSyncAccountsSuccessivePoisonItemThreshold"] = value;
				}
			}

			public virtual EnhancedTimeSpan TransportSyncRemoteConnectionTimeout
			{
				set
				{
					base.PowerSharpParameters["TransportSyncRemoteConnectionTimeout"] = value;
				}
			}

			public virtual ByteQuantifiedSize TransportSyncMaxDownloadSizePerItem
			{
				set
				{
					base.PowerSharpParameters["TransportSyncMaxDownloadSizePerItem"] = value;
				}
			}

			public virtual ByteQuantifiedSize TransportSyncMaxDownloadSizePerConnection
			{
				set
				{
					base.PowerSharpParameters["TransportSyncMaxDownloadSizePerConnection"] = value;
				}
			}

			public virtual int TransportSyncMaxDownloadItemsPerConnection
			{
				set
				{
					base.PowerSharpParameters["TransportSyncMaxDownloadItemsPerConnection"] = value;
				}
			}

			public virtual string DeltaSyncClientCertificateThumbprint
			{
				set
				{
					base.PowerSharpParameters["DeltaSyncClientCertificateThumbprint"] = value;
				}
			}

			public virtual bool UseDowngradedExchangeServerAuth
			{
				set
				{
					base.PowerSharpParameters["UseDowngradedExchangeServerAuth"] = value;
				}
			}

			public virtual int IntraOrgConnectorSmtpMaxMessagesPerConnection
			{
				set
				{
					base.PowerSharpParameters["IntraOrgConnectorSmtpMaxMessagesPerConnection"] = value;
				}
			}

			public virtual bool TransportSyncLinkedInEnabled
			{
				set
				{
					base.PowerSharpParameters["TransportSyncLinkedInEnabled"] = value;
				}
			}

			public virtual bool TransportSyncFacebookEnabled
			{
				set
				{
					base.PowerSharpParameters["TransportSyncFacebookEnabled"] = value;
				}
			}

			public virtual SwitchParameter Verbose
			{
				set
				{
					base.PowerSharpParameters["Verbose"] = value;
				}
			}

			public virtual SwitchParameter Debug
			{
				set
				{
					base.PowerSharpParameters["Debug"] = value;
				}
			}

			public virtual ActionPreference ErrorAction
			{
				set
				{
					base.PowerSharpParameters["ErrorAction"] = value;
				}
			}

			public virtual ActionPreference WarningAction
			{
				set
				{
					base.PowerSharpParameters["WarningAction"] = value;
				}
			}

			public virtual SwitchParameter WhatIf
			{
				set
				{
					base.PowerSharpParameters["WhatIf"] = value;
				}
			}
		}

		public class IdentityParameters : ParametersBase
		{
			public virtual ServerIdParameter Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = value;
				}
			}

			public virtual EnhancedTimeSpan QueueLogMaxAge
			{
				set
				{
					base.PowerSharpParameters["QueueLogMaxAge"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> QueueLogMaxDirectorySize
			{
				set
				{
					base.PowerSharpParameters["QueueLogMaxDirectorySize"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> QueueLogMaxFileSize
			{
				set
				{
					base.PowerSharpParameters["QueueLogMaxFileSize"] = value;
				}
			}

			public virtual LocalLongFullPath QueueLogPath
			{
				set
				{
					base.PowerSharpParameters["QueueLogPath"] = value;
				}
			}

			public virtual EnhancedTimeSpan WlmLogMaxAge
			{
				set
				{
					base.PowerSharpParameters["WlmLogMaxAge"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> WlmLogMaxDirectorySize
			{
				set
				{
					base.PowerSharpParameters["WlmLogMaxDirectorySize"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> WlmLogMaxFileSize
			{
				set
				{
					base.PowerSharpParameters["WlmLogMaxFileSize"] = value;
				}
			}

			public virtual LocalLongFullPath WlmLogPath
			{
				set
				{
					base.PowerSharpParameters["WlmLogPath"] = value;
				}
			}

			public virtual EnhancedTimeSpan AgentLogMaxAge
			{
				set
				{
					base.PowerSharpParameters["AgentLogMaxAge"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> AgentLogMaxDirectorySize
			{
				set
				{
					base.PowerSharpParameters["AgentLogMaxDirectorySize"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> AgentLogMaxFileSize
			{
				set
				{
					base.PowerSharpParameters["AgentLogMaxFileSize"] = value;
				}
			}

			public virtual LocalLongFullPath AgentLogPath
			{
				set
				{
					base.PowerSharpParameters["AgentLogPath"] = value;
				}
			}

			public virtual bool AgentLogEnabled
			{
				set
				{
					base.PowerSharpParameters["AgentLogEnabled"] = value;
				}
			}

			public virtual EnhancedTimeSpan FlowControlLogMaxAge
			{
				set
				{
					base.PowerSharpParameters["FlowControlLogMaxAge"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> FlowControlLogMaxDirectorySize
			{
				set
				{
					base.PowerSharpParameters["FlowControlLogMaxDirectorySize"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> FlowControlLogMaxFileSize
			{
				set
				{
					base.PowerSharpParameters["FlowControlLogMaxFileSize"] = value;
				}
			}

			public virtual LocalLongFullPath FlowControlLogPath
			{
				set
				{
					base.PowerSharpParameters["FlowControlLogPath"] = value;
				}
			}

			public virtual bool FlowControlLogEnabled
			{
				set
				{
					base.PowerSharpParameters["FlowControlLogEnabled"] = value;
				}
			}

			public virtual EnhancedTimeSpan ProcessingSchedulerLogMaxAge
			{
				set
				{
					base.PowerSharpParameters["ProcessingSchedulerLogMaxAge"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> ProcessingSchedulerLogMaxDirectorySize
			{
				set
				{
					base.PowerSharpParameters["ProcessingSchedulerLogMaxDirectorySize"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> ProcessingSchedulerLogMaxFileSize
			{
				set
				{
					base.PowerSharpParameters["ProcessingSchedulerLogMaxFileSize"] = value;
				}
			}

			public virtual LocalLongFullPath ProcessingSchedulerLogPath
			{
				set
				{
					base.PowerSharpParameters["ProcessingSchedulerLogPath"] = value;
				}
			}

			public virtual bool ProcessingSchedulerLogEnabled
			{
				set
				{
					base.PowerSharpParameters["ProcessingSchedulerLogEnabled"] = value;
				}
			}

			public virtual EnhancedTimeSpan ResourceLogMaxAge
			{
				set
				{
					base.PowerSharpParameters["ResourceLogMaxAge"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> ResourceLogMaxDirectorySize
			{
				set
				{
					base.PowerSharpParameters["ResourceLogMaxDirectorySize"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> ResourceLogMaxFileSize
			{
				set
				{
					base.PowerSharpParameters["ResourceLogMaxFileSize"] = value;
				}
			}

			public virtual LocalLongFullPath ResourceLogPath
			{
				set
				{
					base.PowerSharpParameters["ResourceLogPath"] = value;
				}
			}

			public virtual bool ResourceLogEnabled
			{
				set
				{
					base.PowerSharpParameters["ResourceLogEnabled"] = value;
				}
			}

			public virtual EnhancedTimeSpan DnsLogMaxAge
			{
				set
				{
					base.PowerSharpParameters["DnsLogMaxAge"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> DnsLogMaxDirectorySize
			{
				set
				{
					base.PowerSharpParameters["DnsLogMaxDirectorySize"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> DnsLogMaxFileSize
			{
				set
				{
					base.PowerSharpParameters["DnsLogMaxFileSize"] = value;
				}
			}

			public virtual LocalLongFullPath DnsLogPath
			{
				set
				{
					base.PowerSharpParameters["DnsLogPath"] = value;
				}
			}

			public virtual bool DnsLogEnabled
			{
				set
				{
					base.PowerSharpParameters["DnsLogEnabled"] = value;
				}
			}

			public virtual EnhancedTimeSpan JournalLogMaxAge
			{
				set
				{
					base.PowerSharpParameters["JournalLogMaxAge"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> JournalLogMaxDirectorySize
			{
				set
				{
					base.PowerSharpParameters["JournalLogMaxDirectorySize"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> JournalLogMaxFileSize
			{
				set
				{
					base.PowerSharpParameters["JournalLogMaxFileSize"] = value;
				}
			}

			public virtual LocalLongFullPath JournalLogPath
			{
				set
				{
					base.PowerSharpParameters["JournalLogPath"] = value;
				}
			}

			public virtual bool JournalLogEnabled
			{
				set
				{
					base.PowerSharpParameters["JournalLogEnabled"] = value;
				}
			}

			public virtual EnhancedTimeSpan TransportMaintenanceLogMaxAge
			{
				set
				{
					base.PowerSharpParameters["TransportMaintenanceLogMaxAge"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> TransportMaintenanceLogMaxDirectorySize
			{
				set
				{
					base.PowerSharpParameters["TransportMaintenanceLogMaxDirectorySize"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> TransportMaintenanceLogMaxFileSize
			{
				set
				{
					base.PowerSharpParameters["TransportMaintenanceLogMaxFileSize"] = value;
				}
			}

			public virtual LocalLongFullPath TransportMaintenanceLogPath
			{
				set
				{
					base.PowerSharpParameters["TransportMaintenanceLogPath"] = value;
				}
			}

			public virtual bool TransportMaintenanceLogEnabled
			{
				set
				{
					base.PowerSharpParameters["TransportMaintenanceLogEnabled"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual bool AntispamAgentsEnabled
			{
				set
				{
					base.PowerSharpParameters["AntispamAgentsEnabled"] = value;
				}
			}

			public virtual bool ConnectivityLogEnabled
			{
				set
				{
					base.PowerSharpParameters["ConnectivityLogEnabled"] = value;
				}
			}

			public virtual EnhancedTimeSpan ConnectivityLogMaxAge
			{
				set
				{
					base.PowerSharpParameters["ConnectivityLogMaxAge"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> ConnectivityLogMaxDirectorySize
			{
				set
				{
					base.PowerSharpParameters["ConnectivityLogMaxDirectorySize"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> ConnectivityLogMaxFileSize
			{
				set
				{
					base.PowerSharpParameters["ConnectivityLogMaxFileSize"] = value;
				}
			}

			public virtual LocalLongFullPath ConnectivityLogPath
			{
				set
				{
					base.PowerSharpParameters["ConnectivityLogPath"] = value;
				}
			}

			public virtual EnhancedTimeSpan DelayNotificationTimeout
			{
				set
				{
					base.PowerSharpParameters["DelayNotificationTimeout"] = value;
				}
			}

			public virtual bool ExternalDNSAdapterEnabled
			{
				set
				{
					base.PowerSharpParameters["ExternalDNSAdapterEnabled"] = value;
				}
			}

			public virtual Guid ExternalDNSAdapterGuid
			{
				set
				{
					base.PowerSharpParameters["ExternalDNSAdapterGuid"] = value;
				}
			}

			public virtual ProtocolOption ExternalDNSProtocolOption
			{
				set
				{
					base.PowerSharpParameters["ExternalDNSProtocolOption"] = value;
				}
			}

			public virtual MultiValuedProperty<IPAddress> ExternalDNSServers
			{
				set
				{
					base.PowerSharpParameters["ExternalDNSServers"] = value;
				}
			}

			public virtual IPAddress ExternalIPAddress
			{
				set
				{
					base.PowerSharpParameters["ExternalIPAddress"] = value;
				}
			}

			public virtual bool InternalDNSAdapterEnabled
			{
				set
				{
					base.PowerSharpParameters["InternalDNSAdapterEnabled"] = value;
				}
			}

			public virtual Guid InternalDNSAdapterGuid
			{
				set
				{
					base.PowerSharpParameters["InternalDNSAdapterGuid"] = value;
				}
			}

			public virtual ProtocolOption InternalDNSProtocolOption
			{
				set
				{
					base.PowerSharpParameters["InternalDNSProtocolOption"] = value;
				}
			}

			public virtual MultiValuedProperty<IPAddress> InternalDNSServers
			{
				set
				{
					base.PowerSharpParameters["InternalDNSServers"] = value;
				}
			}

			public virtual int MaxConcurrentMailboxDeliveries
			{
				set
				{
					base.PowerSharpParameters["MaxConcurrentMailboxDeliveries"] = value;
				}
			}

			public virtual int MaxConcurrentMailboxSubmissions
			{
				set
				{
					base.PowerSharpParameters["MaxConcurrentMailboxSubmissions"] = value;
				}
			}

			public virtual int MaxConnectionRatePerMinute
			{
				set
				{
					base.PowerSharpParameters["MaxConnectionRatePerMinute"] = value;
				}
			}

			public virtual Unlimited<int> MaxOutboundConnections
			{
				set
				{
					base.PowerSharpParameters["MaxOutboundConnections"] = value;
				}
			}

			public virtual Unlimited<int> MaxPerDomainOutboundConnections
			{
				set
				{
					base.PowerSharpParameters["MaxPerDomainOutboundConnections"] = value;
				}
			}

			public virtual EnhancedTimeSpan MessageExpirationTimeout
			{
				set
				{
					base.PowerSharpParameters["MessageExpirationTimeout"] = value;
				}
			}

			public virtual EnhancedTimeSpan MessageRetryInterval
			{
				set
				{
					base.PowerSharpParameters["MessageRetryInterval"] = value;
				}
			}

			public virtual bool MessageTrackingLogEnabled
			{
				set
				{
					base.PowerSharpParameters["MessageTrackingLogEnabled"] = value;
				}
			}

			public virtual EnhancedTimeSpan MessageTrackingLogMaxAge
			{
				set
				{
					base.PowerSharpParameters["MessageTrackingLogMaxAge"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> MessageTrackingLogMaxDirectorySize
			{
				set
				{
					base.PowerSharpParameters["MessageTrackingLogMaxDirectorySize"] = value;
				}
			}

			public virtual ByteQuantifiedSize MessageTrackingLogMaxFileSize
			{
				set
				{
					base.PowerSharpParameters["MessageTrackingLogMaxFileSize"] = value;
				}
			}

			public virtual LocalLongFullPath MessageTrackingLogPath
			{
				set
				{
					base.PowerSharpParameters["MessageTrackingLogPath"] = value;
				}
			}

			public virtual bool IrmLogEnabled
			{
				set
				{
					base.PowerSharpParameters["IrmLogEnabled"] = value;
				}
			}

			public virtual EnhancedTimeSpan IrmLogMaxAge
			{
				set
				{
					base.PowerSharpParameters["IrmLogMaxAge"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> IrmLogMaxDirectorySize
			{
				set
				{
					base.PowerSharpParameters["IrmLogMaxDirectorySize"] = value;
				}
			}

			public virtual ByteQuantifiedSize IrmLogMaxFileSize
			{
				set
				{
					base.PowerSharpParameters["IrmLogMaxFileSize"] = value;
				}
			}

			public virtual LocalLongFullPath IrmLogPath
			{
				set
				{
					base.PowerSharpParameters["IrmLogPath"] = value;
				}
			}

			public virtual EnhancedTimeSpan ActiveUserStatisticsLogMaxAge
			{
				set
				{
					base.PowerSharpParameters["ActiveUserStatisticsLogMaxAge"] = value;
				}
			}

			public virtual ByteQuantifiedSize ActiveUserStatisticsLogMaxDirectorySize
			{
				set
				{
					base.PowerSharpParameters["ActiveUserStatisticsLogMaxDirectorySize"] = value;
				}
			}

			public virtual ByteQuantifiedSize ActiveUserStatisticsLogMaxFileSize
			{
				set
				{
					base.PowerSharpParameters["ActiveUserStatisticsLogMaxFileSize"] = value;
				}
			}

			public virtual LocalLongFullPath ActiveUserStatisticsLogPath
			{
				set
				{
					base.PowerSharpParameters["ActiveUserStatisticsLogPath"] = value;
				}
			}

			public virtual EnhancedTimeSpan ServerStatisticsLogMaxAge
			{
				set
				{
					base.PowerSharpParameters["ServerStatisticsLogMaxAge"] = value;
				}
			}

			public virtual ByteQuantifiedSize ServerStatisticsLogMaxDirectorySize
			{
				set
				{
					base.PowerSharpParameters["ServerStatisticsLogMaxDirectorySize"] = value;
				}
			}

			public virtual ByteQuantifiedSize ServerStatisticsLogMaxFileSize
			{
				set
				{
					base.PowerSharpParameters["ServerStatisticsLogMaxFileSize"] = value;
				}
			}

			public virtual LocalLongFullPath ServerStatisticsLogPath
			{
				set
				{
					base.PowerSharpParameters["ServerStatisticsLogPath"] = value;
				}
			}

			public virtual bool MessageTrackingLogSubjectLoggingEnabled
			{
				set
				{
					base.PowerSharpParameters["MessageTrackingLogSubjectLoggingEnabled"] = value;
				}
			}

			public virtual EnhancedTimeSpan OutboundConnectionFailureRetryInterval
			{
				set
				{
					base.PowerSharpParameters["OutboundConnectionFailureRetryInterval"] = value;
				}
			}

			public virtual ProtocolLoggingLevel IntraOrgConnectorProtocolLoggingLevel
			{
				set
				{
					base.PowerSharpParameters["IntraOrgConnectorProtocolLoggingLevel"] = value;
				}
			}

			public virtual ByteQuantifiedSize PickupDirectoryMaxHeaderSize
			{
				set
				{
					base.PowerSharpParameters["PickupDirectoryMaxHeaderSize"] = value;
				}
			}

			public virtual int PickupDirectoryMaxMessagesPerMinute
			{
				set
				{
					base.PowerSharpParameters["PickupDirectoryMaxMessagesPerMinute"] = value;
				}
			}

			public virtual int PickupDirectoryMaxRecipientsPerMessage
			{
				set
				{
					base.PowerSharpParameters["PickupDirectoryMaxRecipientsPerMessage"] = value;
				}
			}

			public virtual LocalLongFullPath PickupDirectoryPath
			{
				set
				{
					base.PowerSharpParameters["PickupDirectoryPath"] = value;
				}
			}

			public virtual bool PipelineTracingEnabled
			{
				set
				{
					base.PowerSharpParameters["PipelineTracingEnabled"] = value;
				}
			}

			public virtual bool ContentConversionTracingEnabled
			{
				set
				{
					base.PowerSharpParameters["ContentConversionTracingEnabled"] = value;
				}
			}

			public virtual LocalLongFullPath PipelineTracingPath
			{
				set
				{
					base.PowerSharpParameters["PipelineTracingPath"] = value;
				}
			}

			public virtual SmtpAddress? PipelineTracingSenderAddress
			{
				set
				{
					base.PowerSharpParameters["PipelineTracingSenderAddress"] = value;
				}
			}

			public virtual bool PoisonMessageDetectionEnabled
			{
				set
				{
					base.PowerSharpParameters["PoisonMessageDetectionEnabled"] = value;
				}
			}

			public virtual int PoisonThreshold
			{
				set
				{
					base.PowerSharpParameters["PoisonThreshold"] = value;
				}
			}

			public virtual EnhancedTimeSpan QueueMaxIdleTime
			{
				set
				{
					base.PowerSharpParameters["QueueMaxIdleTime"] = value;
				}
			}

			public virtual EnhancedTimeSpan ReceiveProtocolLogMaxAge
			{
				set
				{
					base.PowerSharpParameters["ReceiveProtocolLogMaxAge"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> ReceiveProtocolLogMaxDirectorySize
			{
				set
				{
					base.PowerSharpParameters["ReceiveProtocolLogMaxDirectorySize"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> ReceiveProtocolLogMaxFileSize
			{
				set
				{
					base.PowerSharpParameters["ReceiveProtocolLogMaxFileSize"] = value;
				}
			}

			public virtual LocalLongFullPath ReceiveProtocolLogPath
			{
				set
				{
					base.PowerSharpParameters["ReceiveProtocolLogPath"] = value;
				}
			}

			public virtual bool RecipientValidationCacheEnabled
			{
				set
				{
					base.PowerSharpParameters["RecipientValidationCacheEnabled"] = value;
				}
			}

			public virtual LocalLongFullPath ReplayDirectoryPath
			{
				set
				{
					base.PowerSharpParameters["ReplayDirectoryPath"] = value;
				}
			}

			public virtual string RootDropDirectoryPath
			{
				set
				{
					base.PowerSharpParameters["RootDropDirectoryPath"] = value;
				}
			}

			public virtual EnhancedTimeSpan RoutingTableLogMaxAge
			{
				set
				{
					base.PowerSharpParameters["RoutingTableLogMaxAge"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> RoutingTableLogMaxDirectorySize
			{
				set
				{
					base.PowerSharpParameters["RoutingTableLogMaxDirectorySize"] = value;
				}
			}

			public virtual LocalLongFullPath RoutingTableLogPath
			{
				set
				{
					base.PowerSharpParameters["RoutingTableLogPath"] = value;
				}
			}

			public virtual EnhancedTimeSpan SendProtocolLogMaxAge
			{
				set
				{
					base.PowerSharpParameters["SendProtocolLogMaxAge"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> SendProtocolLogMaxDirectorySize
			{
				set
				{
					base.PowerSharpParameters["SendProtocolLogMaxDirectorySize"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> SendProtocolLogMaxFileSize
			{
				set
				{
					base.PowerSharpParameters["SendProtocolLogMaxFileSize"] = value;
				}
			}

			public virtual LocalLongFullPath SendProtocolLogPath
			{
				set
				{
					base.PowerSharpParameters["SendProtocolLogPath"] = value;
				}
			}

			public virtual int TransientFailureRetryCount
			{
				set
				{
					base.PowerSharpParameters["TransientFailureRetryCount"] = value;
				}
			}

			public virtual EnhancedTimeSpan TransientFailureRetryInterval
			{
				set
				{
					base.PowerSharpParameters["TransientFailureRetryInterval"] = value;
				}
			}

			public virtual bool TransportSyncEnabled
			{
				set
				{
					base.PowerSharpParameters["TransportSyncEnabled"] = value;
				}
			}

			public virtual bool TransportSyncPopEnabled
			{
				set
				{
					base.PowerSharpParameters["TransportSyncPopEnabled"] = value;
				}
			}

			public virtual bool WindowsLiveHotmailTransportSyncEnabled
			{
				set
				{
					base.PowerSharpParameters["WindowsLiveHotmailTransportSyncEnabled"] = value;
				}
			}

			public virtual bool TransportSyncExchangeEnabled
			{
				set
				{
					base.PowerSharpParameters["TransportSyncExchangeEnabled"] = value;
				}
			}

			public virtual bool TransportSyncImapEnabled
			{
				set
				{
					base.PowerSharpParameters["TransportSyncImapEnabled"] = value;
				}
			}

			public virtual int MaxNumberOfTransportSyncAttempts
			{
				set
				{
					base.PowerSharpParameters["MaxNumberOfTransportSyncAttempts"] = value;
				}
			}

			public virtual int MaxActiveTransportSyncJobsPerProcessor
			{
				set
				{
					base.PowerSharpParameters["MaxActiveTransportSyncJobsPerProcessor"] = value;
				}
			}

			public virtual string HttpTransportSyncProxyServer
			{
				set
				{
					base.PowerSharpParameters["HttpTransportSyncProxyServer"] = value;
				}
			}

			public virtual bool HttpProtocolLogEnabled
			{
				set
				{
					base.PowerSharpParameters["HttpProtocolLogEnabled"] = value;
				}
			}

			public virtual LocalLongFullPath HttpProtocolLogFilePath
			{
				set
				{
					base.PowerSharpParameters["HttpProtocolLogFilePath"] = value;
				}
			}

			public virtual EnhancedTimeSpan HttpProtocolLogMaxAge
			{
				set
				{
					base.PowerSharpParameters["HttpProtocolLogMaxAge"] = value;
				}
			}

			public virtual ByteQuantifiedSize HttpProtocolLogMaxDirectorySize
			{
				set
				{
					base.PowerSharpParameters["HttpProtocolLogMaxDirectorySize"] = value;
				}
			}

			public virtual ByteQuantifiedSize HttpProtocolLogMaxFileSize
			{
				set
				{
					base.PowerSharpParameters["HttpProtocolLogMaxFileSize"] = value;
				}
			}

			public virtual ProtocolLoggingLevel HttpProtocolLogLoggingLevel
			{
				set
				{
					base.PowerSharpParameters["HttpProtocolLogLoggingLevel"] = value;
				}
			}

			public virtual bool TransportSyncLogEnabled
			{
				set
				{
					base.PowerSharpParameters["TransportSyncLogEnabled"] = value;
				}
			}

			public virtual LocalLongFullPath TransportSyncLogFilePath
			{
				set
				{
					base.PowerSharpParameters["TransportSyncLogFilePath"] = value;
				}
			}

			public virtual SyncLoggingLevel TransportSyncLogLoggingLevel
			{
				set
				{
					base.PowerSharpParameters["TransportSyncLogLoggingLevel"] = value;
				}
			}

			public virtual EnhancedTimeSpan TransportSyncLogMaxAge
			{
				set
				{
					base.PowerSharpParameters["TransportSyncLogMaxAge"] = value;
				}
			}

			public virtual ByteQuantifiedSize TransportSyncLogMaxDirectorySize
			{
				set
				{
					base.PowerSharpParameters["TransportSyncLogMaxDirectorySize"] = value;
				}
			}

			public virtual ByteQuantifiedSize TransportSyncLogMaxFileSize
			{
				set
				{
					base.PowerSharpParameters["TransportSyncLogMaxFileSize"] = value;
				}
			}

			public virtual bool TransportSyncHubHealthLogEnabled
			{
				set
				{
					base.PowerSharpParameters["TransportSyncHubHealthLogEnabled"] = value;
				}
			}

			public virtual LocalLongFullPath TransportSyncHubHealthLogFilePath
			{
				set
				{
					base.PowerSharpParameters["TransportSyncHubHealthLogFilePath"] = value;
				}
			}

			public virtual EnhancedTimeSpan TransportSyncHubHealthLogMaxAge
			{
				set
				{
					base.PowerSharpParameters["TransportSyncHubHealthLogMaxAge"] = value;
				}
			}

			public virtual ByteQuantifiedSize TransportSyncHubHealthLogMaxDirectorySize
			{
				set
				{
					base.PowerSharpParameters["TransportSyncHubHealthLogMaxDirectorySize"] = value;
				}
			}

			public virtual ByteQuantifiedSize TransportSyncHubHealthLogMaxFileSize
			{
				set
				{
					base.PowerSharpParameters["TransportSyncHubHealthLogMaxFileSize"] = value;
				}
			}

			public virtual bool TransportSyncAccountsPoisonDetectionEnabled
			{
				set
				{
					base.PowerSharpParameters["TransportSyncAccountsPoisonDetectionEnabled"] = value;
				}
			}

			public virtual int TransportSyncAccountsPoisonAccountThreshold
			{
				set
				{
					base.PowerSharpParameters["TransportSyncAccountsPoisonAccountThreshold"] = value;
				}
			}

			public virtual int TransportSyncAccountsPoisonItemThreshold
			{
				set
				{
					base.PowerSharpParameters["TransportSyncAccountsPoisonItemThreshold"] = value;
				}
			}

			public virtual int TransportSyncAccountsSuccessivePoisonItemThreshold
			{
				set
				{
					base.PowerSharpParameters["TransportSyncAccountsSuccessivePoisonItemThreshold"] = value;
				}
			}

			public virtual EnhancedTimeSpan TransportSyncRemoteConnectionTimeout
			{
				set
				{
					base.PowerSharpParameters["TransportSyncRemoteConnectionTimeout"] = value;
				}
			}

			public virtual ByteQuantifiedSize TransportSyncMaxDownloadSizePerItem
			{
				set
				{
					base.PowerSharpParameters["TransportSyncMaxDownloadSizePerItem"] = value;
				}
			}

			public virtual ByteQuantifiedSize TransportSyncMaxDownloadSizePerConnection
			{
				set
				{
					base.PowerSharpParameters["TransportSyncMaxDownloadSizePerConnection"] = value;
				}
			}

			public virtual int TransportSyncMaxDownloadItemsPerConnection
			{
				set
				{
					base.PowerSharpParameters["TransportSyncMaxDownloadItemsPerConnection"] = value;
				}
			}

			public virtual string DeltaSyncClientCertificateThumbprint
			{
				set
				{
					base.PowerSharpParameters["DeltaSyncClientCertificateThumbprint"] = value;
				}
			}

			public virtual bool UseDowngradedExchangeServerAuth
			{
				set
				{
					base.PowerSharpParameters["UseDowngradedExchangeServerAuth"] = value;
				}
			}

			public virtual int IntraOrgConnectorSmtpMaxMessagesPerConnection
			{
				set
				{
					base.PowerSharpParameters["IntraOrgConnectorSmtpMaxMessagesPerConnection"] = value;
				}
			}

			public virtual bool TransportSyncLinkedInEnabled
			{
				set
				{
					base.PowerSharpParameters["TransportSyncLinkedInEnabled"] = value;
				}
			}

			public virtual bool TransportSyncFacebookEnabled
			{
				set
				{
					base.PowerSharpParameters["TransportSyncFacebookEnabled"] = value;
				}
			}

			public virtual SwitchParameter Verbose
			{
				set
				{
					base.PowerSharpParameters["Verbose"] = value;
				}
			}

			public virtual SwitchParameter Debug
			{
				set
				{
					base.PowerSharpParameters["Debug"] = value;
				}
			}

			public virtual ActionPreference ErrorAction
			{
				set
				{
					base.PowerSharpParameters["ErrorAction"] = value;
				}
			}

			public virtual ActionPreference WarningAction
			{
				set
				{
					base.PowerSharpParameters["WarningAction"] = value;
				}
			}

			public virtual SwitchParameter WhatIf
			{
				set
				{
					base.PowerSharpParameters["WhatIf"] = value;
				}
			}
		}
	}
}
