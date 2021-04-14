using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Management;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class SetMailboxTransportServiceCommand : SyntheticCommandWithPipelineInputNoOutput<MailboxTransportServerPresentationObject>
	{
		private SetMailboxTransportServiceCommand() : base("Set-MailboxTransportService")
		{
		}

		public SetMailboxTransportServiceCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual SetMailboxTransportServiceCommand SetParameters(SetMailboxTransportServiceCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SetMailboxTransportServiceCommand SetParameters(SetMailboxTransportServiceCommand.IdentityParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual EnhancedTimeSpan MailboxSubmissionAgentLogMaxAge
			{
				set
				{
					base.PowerSharpParameters["MailboxSubmissionAgentLogMaxAge"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> MailboxSubmissionAgentLogMaxDirectorySize
			{
				set
				{
					base.PowerSharpParameters["MailboxSubmissionAgentLogMaxDirectorySize"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> MailboxSubmissionAgentLogMaxFileSize
			{
				set
				{
					base.PowerSharpParameters["MailboxSubmissionAgentLogMaxFileSize"] = value;
				}
			}

			public virtual LocalLongFullPath MailboxSubmissionAgentLogPath
			{
				set
				{
					base.PowerSharpParameters["MailboxSubmissionAgentLogPath"] = value;
				}
			}

			public virtual bool MailboxSubmissionAgentLogEnabled
			{
				set
				{
					base.PowerSharpParameters["MailboxSubmissionAgentLogEnabled"] = value;
				}
			}

			public virtual EnhancedTimeSpan MailboxDeliveryAgentLogMaxAge
			{
				set
				{
					base.PowerSharpParameters["MailboxDeliveryAgentLogMaxAge"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> MailboxDeliveryAgentLogMaxDirectorySize
			{
				set
				{
					base.PowerSharpParameters["MailboxDeliveryAgentLogMaxDirectorySize"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> MailboxDeliveryAgentLogMaxFileSize
			{
				set
				{
					base.PowerSharpParameters["MailboxDeliveryAgentLogMaxFileSize"] = value;
				}
			}

			public virtual LocalLongFullPath MailboxDeliveryAgentLogPath
			{
				set
				{
					base.PowerSharpParameters["MailboxDeliveryAgentLogPath"] = value;
				}
			}

			public virtual bool MailboxDeliveryAgentLogEnabled
			{
				set
				{
					base.PowerSharpParameters["MailboxDeliveryAgentLogEnabled"] = value;
				}
			}

			public virtual bool MailboxDeliveryThrottlingLogEnabled
			{
				set
				{
					base.PowerSharpParameters["MailboxDeliveryThrottlingLogEnabled"] = value;
				}
			}

			public virtual EnhancedTimeSpan MailboxDeliveryThrottlingLogMaxAge
			{
				set
				{
					base.PowerSharpParameters["MailboxDeliveryThrottlingLogMaxAge"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> MailboxDeliveryThrottlingLogMaxDirectorySize
			{
				set
				{
					base.PowerSharpParameters["MailboxDeliveryThrottlingLogMaxDirectorySize"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> MailboxDeliveryThrottlingLogMaxFileSize
			{
				set
				{
					base.PowerSharpParameters["MailboxDeliveryThrottlingLogMaxFileSize"] = value;
				}
			}

			public virtual LocalLongFullPath MailboxDeliveryThrottlingLogPath
			{
				set
				{
					base.PowerSharpParameters["MailboxDeliveryThrottlingLogPath"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
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

			public virtual ProtocolLoggingLevel MailboxDeliveryConnectorProtocolLoggingLevel
			{
				set
				{
					base.PowerSharpParameters["MailboxDeliveryConnectorProtocolLoggingLevel"] = value;
				}
			}

			public virtual bool MailboxDeliveryConnectorSmtpUtf8Enabled
			{
				set
				{
					base.PowerSharpParameters["MailboxDeliveryConnectorSmtpUtf8Enabled"] = value;
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
			public virtual MailboxTransportServerIdParameter Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = value;
				}
			}

			public virtual EnhancedTimeSpan MailboxSubmissionAgentLogMaxAge
			{
				set
				{
					base.PowerSharpParameters["MailboxSubmissionAgentLogMaxAge"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> MailboxSubmissionAgentLogMaxDirectorySize
			{
				set
				{
					base.PowerSharpParameters["MailboxSubmissionAgentLogMaxDirectorySize"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> MailboxSubmissionAgentLogMaxFileSize
			{
				set
				{
					base.PowerSharpParameters["MailboxSubmissionAgentLogMaxFileSize"] = value;
				}
			}

			public virtual LocalLongFullPath MailboxSubmissionAgentLogPath
			{
				set
				{
					base.PowerSharpParameters["MailboxSubmissionAgentLogPath"] = value;
				}
			}

			public virtual bool MailboxSubmissionAgentLogEnabled
			{
				set
				{
					base.PowerSharpParameters["MailboxSubmissionAgentLogEnabled"] = value;
				}
			}

			public virtual EnhancedTimeSpan MailboxDeliveryAgentLogMaxAge
			{
				set
				{
					base.PowerSharpParameters["MailboxDeliveryAgentLogMaxAge"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> MailboxDeliveryAgentLogMaxDirectorySize
			{
				set
				{
					base.PowerSharpParameters["MailboxDeliveryAgentLogMaxDirectorySize"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> MailboxDeliveryAgentLogMaxFileSize
			{
				set
				{
					base.PowerSharpParameters["MailboxDeliveryAgentLogMaxFileSize"] = value;
				}
			}

			public virtual LocalLongFullPath MailboxDeliveryAgentLogPath
			{
				set
				{
					base.PowerSharpParameters["MailboxDeliveryAgentLogPath"] = value;
				}
			}

			public virtual bool MailboxDeliveryAgentLogEnabled
			{
				set
				{
					base.PowerSharpParameters["MailboxDeliveryAgentLogEnabled"] = value;
				}
			}

			public virtual bool MailboxDeliveryThrottlingLogEnabled
			{
				set
				{
					base.PowerSharpParameters["MailboxDeliveryThrottlingLogEnabled"] = value;
				}
			}

			public virtual EnhancedTimeSpan MailboxDeliveryThrottlingLogMaxAge
			{
				set
				{
					base.PowerSharpParameters["MailboxDeliveryThrottlingLogMaxAge"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> MailboxDeliveryThrottlingLogMaxDirectorySize
			{
				set
				{
					base.PowerSharpParameters["MailboxDeliveryThrottlingLogMaxDirectorySize"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> MailboxDeliveryThrottlingLogMaxFileSize
			{
				set
				{
					base.PowerSharpParameters["MailboxDeliveryThrottlingLogMaxFileSize"] = value;
				}
			}

			public virtual LocalLongFullPath MailboxDeliveryThrottlingLogPath
			{
				set
				{
					base.PowerSharpParameters["MailboxDeliveryThrottlingLogPath"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
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

			public virtual ProtocolLoggingLevel MailboxDeliveryConnectorProtocolLoggingLevel
			{
				set
				{
					base.PowerSharpParameters["MailboxDeliveryConnectorProtocolLoggingLevel"] = value;
				}
			}

			public virtual bool MailboxDeliveryConnectorSmtpUtf8Enabled
			{
				set
				{
					base.PowerSharpParameters["MailboxDeliveryConnectorSmtpUtf8Enabled"] = value;
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
