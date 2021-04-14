using System;
using System.Management.Automation;
using System.Net;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Management;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class SetFrontendTransportServiceCommand : SyntheticCommandWithPipelineInputNoOutput<FrontendTransportServerPresentationObject>
	{
		private SetFrontendTransportServiceCommand() : base("Set-FrontendTransportService")
		{
		}

		public SetFrontendTransportServiceCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual SetFrontendTransportServiceCommand SetParameters(SetFrontendTransportServiceCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SetFrontendTransportServiceCommand SetParameters(SetFrontendTransportServiceCommand.IdentityParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
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

			public virtual EnhancedTimeSpan AttributionLogMaxAge
			{
				set
				{
					base.PowerSharpParameters["AttributionLogMaxAge"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> AttributionLogMaxDirectorySize
			{
				set
				{
					base.PowerSharpParameters["AttributionLogMaxDirectorySize"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> AttributionLogMaxFileSize
			{
				set
				{
					base.PowerSharpParameters["AttributionLogMaxFileSize"] = value;
				}
			}

			public virtual LocalLongFullPath AttributionLogPath
			{
				set
				{
					base.PowerSharpParameters["AttributionLogPath"] = value;
				}
			}

			public virtual bool AttributionLogEnabled
			{
				set
				{
					base.PowerSharpParameters["AttributionLogEnabled"] = value;
				}
			}

			public virtual int MaxReceiveTlsRatePerMinute
			{
				set
				{
					base.PowerSharpParameters["MaxReceiveTlsRatePerMinute"] = value;
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

			public virtual ProtocolLoggingLevel IntraOrgConnectorProtocolLoggingLevel
			{
				set
				{
					base.PowerSharpParameters["IntraOrgConnectorProtocolLoggingLevel"] = value;
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

			public virtual int MaxConnectionRatePerMinute
			{
				set
				{
					base.PowerSharpParameters["MaxConnectionRatePerMinute"] = value;
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
			public virtual FrontendTransportServerIdParameter Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = value;
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

			public virtual EnhancedTimeSpan AttributionLogMaxAge
			{
				set
				{
					base.PowerSharpParameters["AttributionLogMaxAge"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> AttributionLogMaxDirectorySize
			{
				set
				{
					base.PowerSharpParameters["AttributionLogMaxDirectorySize"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> AttributionLogMaxFileSize
			{
				set
				{
					base.PowerSharpParameters["AttributionLogMaxFileSize"] = value;
				}
			}

			public virtual LocalLongFullPath AttributionLogPath
			{
				set
				{
					base.PowerSharpParameters["AttributionLogPath"] = value;
				}
			}

			public virtual bool AttributionLogEnabled
			{
				set
				{
					base.PowerSharpParameters["AttributionLogEnabled"] = value;
				}
			}

			public virtual int MaxReceiveTlsRatePerMinute
			{
				set
				{
					base.PowerSharpParameters["MaxReceiveTlsRatePerMinute"] = value;
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

			public virtual ProtocolLoggingLevel IntraOrgConnectorProtocolLoggingLevel
			{
				set
				{
					base.PowerSharpParameters["IntraOrgConnectorProtocolLoggingLevel"] = value;
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

			public virtual int MaxConnectionRatePerMinute
			{
				set
				{
					base.PowerSharpParameters["MaxConnectionRatePerMinute"] = value;
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
