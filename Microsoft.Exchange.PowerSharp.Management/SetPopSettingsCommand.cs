using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class SetPopSettingsCommand : SyntheticCommandWithPipelineInputNoOutput<Pop3AdConfiguration>
	{
		private SetPopSettingsCommand() : base("Set-PopSettings")
		{
		}

		public SetPopSettingsCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual SetPopSettingsCommand SetParameters(SetPopSettingsCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual ServerIdParameter Server
			{
				set
				{
					base.PowerSharpParameters["Server"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual int MaxCommandSize
			{
				set
				{
					base.PowerSharpParameters["MaxCommandSize"] = value;
				}
			}

			public virtual SortOrder MessageRetrievalSortOrder
			{
				set
				{
					base.PowerSharpParameters["MessageRetrievalSortOrder"] = value;
				}
			}

			public virtual MultiValuedProperty<IPBinding> UnencryptedOrTLSBindings
			{
				set
				{
					base.PowerSharpParameters["UnencryptedOrTLSBindings"] = value;
				}
			}

			public virtual MultiValuedProperty<IPBinding> SSLBindings
			{
				set
				{
					base.PowerSharpParameters["SSLBindings"] = value;
				}
			}

			public virtual MultiValuedProperty<ProtocolConnectionSettings> InternalConnectionSettings
			{
				set
				{
					base.PowerSharpParameters["InternalConnectionSettings"] = value;
				}
			}

			public virtual MultiValuedProperty<ProtocolConnectionSettings> ExternalConnectionSettings
			{
				set
				{
					base.PowerSharpParameters["ExternalConnectionSettings"] = value;
				}
			}

			public virtual string X509CertificateName
			{
				set
				{
					base.PowerSharpParameters["X509CertificateName"] = value;
				}
			}

			public virtual string Banner
			{
				set
				{
					base.PowerSharpParameters["Banner"] = value;
				}
			}

			public virtual LoginOptions LoginType
			{
				set
				{
					base.PowerSharpParameters["LoginType"] = value;
				}
			}

			public virtual EnhancedTimeSpan AuthenticatedConnectionTimeout
			{
				set
				{
					base.PowerSharpParameters["AuthenticatedConnectionTimeout"] = value;
				}
			}

			public virtual EnhancedTimeSpan PreAuthenticatedConnectionTimeout
			{
				set
				{
					base.PowerSharpParameters["PreAuthenticatedConnectionTimeout"] = value;
				}
			}

			public virtual int MaxConnections
			{
				set
				{
					base.PowerSharpParameters["MaxConnections"] = value;
				}
			}

			public virtual int MaxConnectionFromSingleIP
			{
				set
				{
					base.PowerSharpParameters["MaxConnectionFromSingleIP"] = value;
				}
			}

			public virtual int MaxConnectionsPerUser
			{
				set
				{
					base.PowerSharpParameters["MaxConnectionsPerUser"] = value;
				}
			}

			public virtual MimeTextFormat MessageRetrievalMimeFormat
			{
				set
				{
					base.PowerSharpParameters["MessageRetrievalMimeFormat"] = value;
				}
			}

			public virtual int ProxyTargetPort
			{
				set
				{
					base.PowerSharpParameters["ProxyTargetPort"] = value;
				}
			}

			public virtual CalendarItemRetrievalOptions CalendarItemRetrievalOption
			{
				set
				{
					base.PowerSharpParameters["CalendarItemRetrievalOption"] = value;
				}
			}

			public virtual Uri OwaServerUrl
			{
				set
				{
					base.PowerSharpParameters["OwaServerUrl"] = value;
				}
			}

			public virtual bool EnableExactRFC822Size
			{
				set
				{
					base.PowerSharpParameters["EnableExactRFC822Size"] = value;
				}
			}

			public virtual bool LiveIdBasicAuthReplacement
			{
				set
				{
					base.PowerSharpParameters["LiveIdBasicAuthReplacement"] = value;
				}
			}

			public virtual bool SuppressReadReceipt
			{
				set
				{
					base.PowerSharpParameters["SuppressReadReceipt"] = value;
				}
			}

			public virtual bool ProtocolLogEnabled
			{
				set
				{
					base.PowerSharpParameters["ProtocolLogEnabled"] = value;
				}
			}

			public virtual bool EnforceCertificateErrors
			{
				set
				{
					base.PowerSharpParameters["EnforceCertificateErrors"] = value;
				}
			}

			public virtual string LogFileLocation
			{
				set
				{
					base.PowerSharpParameters["LogFileLocation"] = value;
				}
			}

			public virtual LogFileRollOver LogFileRollOverSettings
			{
				set
				{
					base.PowerSharpParameters["LogFileRollOverSettings"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> LogPerFileSizeQuota
			{
				set
				{
					base.PowerSharpParameters["LogPerFileSizeQuota"] = value;
				}
			}

			public virtual ExtendedProtectionTokenCheckingMode ExtendedProtectionPolicy
			{
				set
				{
					base.PowerSharpParameters["ExtendedProtectionPolicy"] = value;
				}
			}

			public virtual bool EnableGSSAPIAndNTLMAuth
			{
				set
				{
					base.PowerSharpParameters["EnableGSSAPIAndNTLMAuth"] = value;
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
