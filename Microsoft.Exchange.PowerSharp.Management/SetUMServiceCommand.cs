using System;
using System.Management.Automation;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class SetUMServiceCommand : SyntheticCommandWithPipelineInputNoOutput<UMServer>
	{
		private SetUMServiceCommand() : base("Set-UMService")
		{
		}

		public SetUMServiceCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual SetUMServiceCommand SetParameters(SetUMServiceCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SetUMServiceCommand SetParameters(SetUMServiceCommand.IdentityParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual MultiValuedProperty<UMDialPlanIdParameter> DialPlans
			{
				set
				{
					base.PowerSharpParameters["DialPlans"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual int? MaxCallsAllowed
			{
				set
				{
					base.PowerSharpParameters["MaxCallsAllowed"] = value;
				}
			}

			public virtual ServerStatus Status
			{
				set
				{
					base.PowerSharpParameters["Status"] = value;
				}
			}

			public virtual int SipTcpListeningPort
			{
				set
				{
					base.PowerSharpParameters["SipTcpListeningPort"] = value;
				}
			}

			public virtual int SipTlsListeningPort
			{
				set
				{
					base.PowerSharpParameters["SipTlsListeningPort"] = value;
				}
			}

			public virtual ScheduleInterval GrammarGenerationSchedule
			{
				set
				{
					base.PowerSharpParameters["GrammarGenerationSchedule"] = value;
				}
			}

			public virtual UMSmartHost ExternalHostFqdn
			{
				set
				{
					base.PowerSharpParameters["ExternalHostFqdn"] = value;
				}
			}

			public virtual UMSmartHost ExternalServiceFqdn
			{
				set
				{
					base.PowerSharpParameters["ExternalServiceFqdn"] = value;
				}
			}

			public virtual string UMPodRedirectTemplate
			{
				set
				{
					base.PowerSharpParameters["UMPodRedirectTemplate"] = value;
				}
			}

			public virtual string UMForwardingAddressTemplate
			{
				set
				{
					base.PowerSharpParameters["UMForwardingAddressTemplate"] = value;
				}
			}

			public virtual ProtocolConnectionSettings SIPAccessService
			{
				set
				{
					base.PowerSharpParameters["SIPAccessService"] = value;
				}
			}

			public virtual UMStartupMode UMStartupMode
			{
				set
				{
					base.PowerSharpParameters["UMStartupMode"] = value;
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

			public virtual bool IPAddressFamilyConfigurable
			{
				set
				{
					base.PowerSharpParameters["IPAddressFamilyConfigurable"] = value;
				}
			}

			public virtual IPAddressFamily IPAddressFamily
			{
				set
				{
					base.PowerSharpParameters["IPAddressFamily"] = value;
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
			public virtual UMServerIdParameter Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = value;
				}
			}

			public virtual MultiValuedProperty<UMDialPlanIdParameter> DialPlans
			{
				set
				{
					base.PowerSharpParameters["DialPlans"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual int? MaxCallsAllowed
			{
				set
				{
					base.PowerSharpParameters["MaxCallsAllowed"] = value;
				}
			}

			public virtual ServerStatus Status
			{
				set
				{
					base.PowerSharpParameters["Status"] = value;
				}
			}

			public virtual int SipTcpListeningPort
			{
				set
				{
					base.PowerSharpParameters["SipTcpListeningPort"] = value;
				}
			}

			public virtual int SipTlsListeningPort
			{
				set
				{
					base.PowerSharpParameters["SipTlsListeningPort"] = value;
				}
			}

			public virtual ScheduleInterval GrammarGenerationSchedule
			{
				set
				{
					base.PowerSharpParameters["GrammarGenerationSchedule"] = value;
				}
			}

			public virtual UMSmartHost ExternalHostFqdn
			{
				set
				{
					base.PowerSharpParameters["ExternalHostFqdn"] = value;
				}
			}

			public virtual UMSmartHost ExternalServiceFqdn
			{
				set
				{
					base.PowerSharpParameters["ExternalServiceFqdn"] = value;
				}
			}

			public virtual string UMPodRedirectTemplate
			{
				set
				{
					base.PowerSharpParameters["UMPodRedirectTemplate"] = value;
				}
			}

			public virtual string UMForwardingAddressTemplate
			{
				set
				{
					base.PowerSharpParameters["UMForwardingAddressTemplate"] = value;
				}
			}

			public virtual ProtocolConnectionSettings SIPAccessService
			{
				set
				{
					base.PowerSharpParameters["SIPAccessService"] = value;
				}
			}

			public virtual UMStartupMode UMStartupMode
			{
				set
				{
					base.PowerSharpParameters["UMStartupMode"] = value;
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

			public virtual bool IPAddressFamilyConfigurable
			{
				set
				{
					base.PowerSharpParameters["IPAddressFamilyConfigurable"] = value;
				}
			}

			public virtual IPAddressFamily IPAddressFamily
			{
				set
				{
					base.PowerSharpParameters["IPAddressFamily"] = value;
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
