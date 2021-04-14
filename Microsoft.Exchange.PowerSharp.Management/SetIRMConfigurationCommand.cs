using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class SetIRMConfigurationCommand : SyntheticCommandWithPipelineInputNoOutput<IRMConfiguration>
	{
		private SetIRMConfigurationCommand() : base("Set-IRMConfiguration")
		{
		}

		public SetIRMConfigurationCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual SetIRMConfigurationCommand SetParameters(SetIRMConfigurationCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SetIRMConfigurationCommand SetParameters(SetIRMConfigurationCommand.IdentityParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual SwitchParameter Force
			{
				set
				{
					base.PowerSharpParameters["Force"] = value;
				}
			}

			public virtual SwitchParameter RefreshServerCertificates
			{
				set
				{
					base.PowerSharpParameters["RefreshServerCertificates"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual TransportDecryptionSetting TransportDecryptionSetting
			{
				set
				{
					base.PowerSharpParameters["TransportDecryptionSetting"] = value;
				}
			}

			public virtual Uri ServiceLocation
			{
				set
				{
					base.PowerSharpParameters["ServiceLocation"] = value;
				}
			}

			public virtual Uri PublishingLocation
			{
				set
				{
					base.PowerSharpParameters["PublishingLocation"] = value;
				}
			}

			public virtual MultiValuedProperty<Uri> LicensingLocation
			{
				set
				{
					base.PowerSharpParameters["LicensingLocation"] = value;
				}
			}

			public virtual bool JournalReportDecryptionEnabled
			{
				set
				{
					base.PowerSharpParameters["JournalReportDecryptionEnabled"] = value;
				}
			}

			public virtual bool ExternalLicensingEnabled
			{
				set
				{
					base.PowerSharpParameters["ExternalLicensingEnabled"] = value;
				}
			}

			public virtual bool InternalLicensingEnabled
			{
				set
				{
					base.PowerSharpParameters["InternalLicensingEnabled"] = value;
				}
			}

			public virtual bool SearchEnabled
			{
				set
				{
					base.PowerSharpParameters["SearchEnabled"] = value;
				}
			}

			public virtual bool ClientAccessServerEnabled
			{
				set
				{
					base.PowerSharpParameters["ClientAccessServerEnabled"] = value;
				}
			}

			public virtual bool EDiscoverySuperUserEnabled
			{
				set
				{
					base.PowerSharpParameters["EDiscoverySuperUserEnabled"] = value;
				}
			}

			public virtual Uri RMSOnlineKeySharingLocation
			{
				set
				{
					base.PowerSharpParameters["RMSOnlineKeySharingLocation"] = value;
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
			public virtual string Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = ((value != null) ? new OrganizationIdParameter(value) : null);
				}
			}

			public virtual SwitchParameter Force
			{
				set
				{
					base.PowerSharpParameters["Force"] = value;
				}
			}

			public virtual SwitchParameter RefreshServerCertificates
			{
				set
				{
					base.PowerSharpParameters["RefreshServerCertificates"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual TransportDecryptionSetting TransportDecryptionSetting
			{
				set
				{
					base.PowerSharpParameters["TransportDecryptionSetting"] = value;
				}
			}

			public virtual Uri ServiceLocation
			{
				set
				{
					base.PowerSharpParameters["ServiceLocation"] = value;
				}
			}

			public virtual Uri PublishingLocation
			{
				set
				{
					base.PowerSharpParameters["PublishingLocation"] = value;
				}
			}

			public virtual MultiValuedProperty<Uri> LicensingLocation
			{
				set
				{
					base.PowerSharpParameters["LicensingLocation"] = value;
				}
			}

			public virtual bool JournalReportDecryptionEnabled
			{
				set
				{
					base.PowerSharpParameters["JournalReportDecryptionEnabled"] = value;
				}
			}

			public virtual bool ExternalLicensingEnabled
			{
				set
				{
					base.PowerSharpParameters["ExternalLicensingEnabled"] = value;
				}
			}

			public virtual bool InternalLicensingEnabled
			{
				set
				{
					base.PowerSharpParameters["InternalLicensingEnabled"] = value;
				}
			}

			public virtual bool SearchEnabled
			{
				set
				{
					base.PowerSharpParameters["SearchEnabled"] = value;
				}
			}

			public virtual bool ClientAccessServerEnabled
			{
				set
				{
					base.PowerSharpParameters["ClientAccessServerEnabled"] = value;
				}
			}

			public virtual bool EDiscoverySuperUserEnabled
			{
				set
				{
					base.PowerSharpParameters["EDiscoverySuperUserEnabled"] = value;
				}
			}

			public virtual Uri RMSOnlineKeySharingLocation
			{
				set
				{
					base.PowerSharpParameters["RMSOnlineKeySharingLocation"] = value;
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
