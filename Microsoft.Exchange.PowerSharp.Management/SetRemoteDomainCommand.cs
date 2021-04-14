using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class SetRemoteDomainCommand : SyntheticCommandWithPipelineInputNoOutput<DomainContentConfig>
	{
		private SetRemoteDomainCommand() : base("Set-RemoteDomain")
		{
		}

		public SetRemoteDomainCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual SetRemoteDomainCommand SetParameters(SetRemoteDomainCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SetRemoteDomainCommand SetParameters(SetRemoteDomainCommand.IdentityParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual bool TargetDeliveryDomain
			{
				set
				{
					base.PowerSharpParameters["TargetDeliveryDomain"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual bool IsInternal
			{
				set
				{
					base.PowerSharpParameters["IsInternal"] = value;
				}
			}

			public virtual ByteEncoderTypeFor7BitCharsetsEnum ByteEncoderTypeFor7BitCharsets
			{
				set
				{
					base.PowerSharpParameters["ByteEncoderTypeFor7BitCharsets"] = value;
				}
			}

			public virtual string CharacterSet
			{
				set
				{
					base.PowerSharpParameters["CharacterSet"] = value;
				}
			}

			public virtual string NonMimeCharacterSet
			{
				set
				{
					base.PowerSharpParameters["NonMimeCharacterSet"] = value;
				}
			}

			public virtual AllowedOOFType AllowedOOFType
			{
				set
				{
					base.PowerSharpParameters["AllowedOOFType"] = value;
				}
			}

			public virtual bool AutoReplyEnabled
			{
				set
				{
					base.PowerSharpParameters["AutoReplyEnabled"] = value;
				}
			}

			public virtual bool AutoForwardEnabled
			{
				set
				{
					base.PowerSharpParameters["AutoForwardEnabled"] = value;
				}
			}

			public virtual bool DeliveryReportEnabled
			{
				set
				{
					base.PowerSharpParameters["DeliveryReportEnabled"] = value;
				}
			}

			public virtual bool NDREnabled
			{
				set
				{
					base.PowerSharpParameters["NDREnabled"] = value;
				}
			}

			public virtual bool MeetingForwardNotificationEnabled
			{
				set
				{
					base.PowerSharpParameters["MeetingForwardNotificationEnabled"] = value;
				}
			}

			public virtual ContentType ContentType
			{
				set
				{
					base.PowerSharpParameters["ContentType"] = value;
				}
			}

			public virtual bool DisplaySenderName
			{
				set
				{
					base.PowerSharpParameters["DisplaySenderName"] = value;
				}
			}

			public virtual PreferredInternetCodePageForShiftJisEnum PreferredInternetCodePageForShiftJis
			{
				set
				{
					base.PowerSharpParameters["PreferredInternetCodePageForShiftJis"] = value;
				}
			}

			public virtual int? RequiredCharsetCoverage
			{
				set
				{
					base.PowerSharpParameters["RequiredCharsetCoverage"] = value;
				}
			}

			public virtual bool? TNEFEnabled
			{
				set
				{
					base.PowerSharpParameters["TNEFEnabled"] = value;
				}
			}

			public virtual Unlimited<int> LineWrapSize
			{
				set
				{
					base.PowerSharpParameters["LineWrapSize"] = value;
				}
			}

			public virtual bool TrustedMailOutboundEnabled
			{
				set
				{
					base.PowerSharpParameters["TrustedMailOutboundEnabled"] = value;
				}
			}

			public virtual bool TrustedMailInboundEnabled
			{
				set
				{
					base.PowerSharpParameters["TrustedMailInboundEnabled"] = value;
				}
			}

			public virtual bool UseSimpleDisplayName
			{
				set
				{
					base.PowerSharpParameters["UseSimpleDisplayName"] = value;
				}
			}

			public virtual bool NDRDiagnosticInfoEnabled
			{
				set
				{
					base.PowerSharpParameters["NDRDiagnosticInfoEnabled"] = value;
				}
			}

			public virtual int MessageCountThreshold
			{
				set
				{
					base.PowerSharpParameters["MessageCountThreshold"] = value;
				}
			}

			public virtual string Name
			{
				set
				{
					base.PowerSharpParameters["Name"] = value;
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
			public virtual RemoteDomainIdParameter Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = value;
				}
			}

			public virtual bool TargetDeliveryDomain
			{
				set
				{
					base.PowerSharpParameters["TargetDeliveryDomain"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual bool IsInternal
			{
				set
				{
					base.PowerSharpParameters["IsInternal"] = value;
				}
			}

			public virtual ByteEncoderTypeFor7BitCharsetsEnum ByteEncoderTypeFor7BitCharsets
			{
				set
				{
					base.PowerSharpParameters["ByteEncoderTypeFor7BitCharsets"] = value;
				}
			}

			public virtual string CharacterSet
			{
				set
				{
					base.PowerSharpParameters["CharacterSet"] = value;
				}
			}

			public virtual string NonMimeCharacterSet
			{
				set
				{
					base.PowerSharpParameters["NonMimeCharacterSet"] = value;
				}
			}

			public virtual AllowedOOFType AllowedOOFType
			{
				set
				{
					base.PowerSharpParameters["AllowedOOFType"] = value;
				}
			}

			public virtual bool AutoReplyEnabled
			{
				set
				{
					base.PowerSharpParameters["AutoReplyEnabled"] = value;
				}
			}

			public virtual bool AutoForwardEnabled
			{
				set
				{
					base.PowerSharpParameters["AutoForwardEnabled"] = value;
				}
			}

			public virtual bool DeliveryReportEnabled
			{
				set
				{
					base.PowerSharpParameters["DeliveryReportEnabled"] = value;
				}
			}

			public virtual bool NDREnabled
			{
				set
				{
					base.PowerSharpParameters["NDREnabled"] = value;
				}
			}

			public virtual bool MeetingForwardNotificationEnabled
			{
				set
				{
					base.PowerSharpParameters["MeetingForwardNotificationEnabled"] = value;
				}
			}

			public virtual ContentType ContentType
			{
				set
				{
					base.PowerSharpParameters["ContentType"] = value;
				}
			}

			public virtual bool DisplaySenderName
			{
				set
				{
					base.PowerSharpParameters["DisplaySenderName"] = value;
				}
			}

			public virtual PreferredInternetCodePageForShiftJisEnum PreferredInternetCodePageForShiftJis
			{
				set
				{
					base.PowerSharpParameters["PreferredInternetCodePageForShiftJis"] = value;
				}
			}

			public virtual int? RequiredCharsetCoverage
			{
				set
				{
					base.PowerSharpParameters["RequiredCharsetCoverage"] = value;
				}
			}

			public virtual bool? TNEFEnabled
			{
				set
				{
					base.PowerSharpParameters["TNEFEnabled"] = value;
				}
			}

			public virtual Unlimited<int> LineWrapSize
			{
				set
				{
					base.PowerSharpParameters["LineWrapSize"] = value;
				}
			}

			public virtual bool TrustedMailOutboundEnabled
			{
				set
				{
					base.PowerSharpParameters["TrustedMailOutboundEnabled"] = value;
				}
			}

			public virtual bool TrustedMailInboundEnabled
			{
				set
				{
					base.PowerSharpParameters["TrustedMailInboundEnabled"] = value;
				}
			}

			public virtual bool UseSimpleDisplayName
			{
				set
				{
					base.PowerSharpParameters["UseSimpleDisplayName"] = value;
				}
			}

			public virtual bool NDRDiagnosticInfoEnabled
			{
				set
				{
					base.PowerSharpParameters["NDRDiagnosticInfoEnabled"] = value;
				}
			}

			public virtual int MessageCountThreshold
			{
				set
				{
					base.PowerSharpParameters["MessageCountThreshold"] = value;
				}
			}

			public virtual string Name
			{
				set
				{
					base.PowerSharpParameters["Name"] = value;
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
