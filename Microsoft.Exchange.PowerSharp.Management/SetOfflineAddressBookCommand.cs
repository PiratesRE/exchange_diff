using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class SetOfflineAddressBookCommand : SyntheticCommandWithPipelineInputNoOutput<OfflineAddressBook>
	{
		private SetOfflineAddressBookCommand() : base("Set-OfflineAddressBook")
		{
		}

		public SetOfflineAddressBookCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual SetOfflineAddressBookCommand SetParameters(SetOfflineAddressBookCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SetOfflineAddressBookCommand SetParameters(SetOfflineAddressBookCommand.IdentityParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual string GeneratingMailbox
			{
				set
				{
					base.PowerSharpParameters["GeneratingMailbox"] = ((value != null) ? new MailboxIdParameter(value) : null);
				}
			}

			public virtual AddressBookBaseIdParameter AddressLists
			{
				set
				{
					base.PowerSharpParameters["AddressLists"] = value;
				}
			}

			public virtual VirtualDirectoryIdParameter VirtualDirectories
			{
				set
				{
					base.PowerSharpParameters["VirtualDirectories"] = value;
				}
			}

			public virtual SwitchParameter ApplyMandatoryProperties
			{
				set
				{
					base.PowerSharpParameters["ApplyMandatoryProperties"] = value;
				}
			}

			public virtual SwitchParameter UseDefaultAttributes
			{
				set
				{
					base.PowerSharpParameters["UseDefaultAttributes"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual MultiValuedProperty<OfflineAddressBookVersion> Versions
			{
				set
				{
					base.PowerSharpParameters["Versions"] = value;
				}
			}

			public virtual bool IsDefault
			{
				set
				{
					base.PowerSharpParameters["IsDefault"] = value;
				}
			}

			public virtual bool PublicFolderDistributionEnabled
			{
				set
				{
					base.PowerSharpParameters["PublicFolderDistributionEnabled"] = value;
				}
			}

			public virtual bool GlobalWebDistributionEnabled
			{
				set
				{
					base.PowerSharpParameters["GlobalWebDistributionEnabled"] = value;
				}
			}

			public virtual bool ShadowMailboxDistributionEnabled
			{
				set
				{
					base.PowerSharpParameters["ShadowMailboxDistributionEnabled"] = value;
				}
			}

			public virtual int MaxBinaryPropertySize
			{
				set
				{
					base.PowerSharpParameters["MaxBinaryPropertySize"] = value;
				}
			}

			public virtual int MaxMultivaluedBinaryPropertySize
			{
				set
				{
					base.PowerSharpParameters["MaxMultivaluedBinaryPropertySize"] = value;
				}
			}

			public virtual int MaxStringPropertySize
			{
				set
				{
					base.PowerSharpParameters["MaxStringPropertySize"] = value;
				}
			}

			public virtual int MaxMultivaluedStringPropertySize
			{
				set
				{
					base.PowerSharpParameters["MaxMultivaluedStringPropertySize"] = value;
				}
			}

			public virtual MultiValuedProperty<OfflineAddressBookMapiProperty> ConfiguredAttributes
			{
				set
				{
					base.PowerSharpParameters["ConfiguredAttributes"] = value;
				}
			}

			public virtual Unlimited<int>? DiffRetentionPeriod
			{
				set
				{
					base.PowerSharpParameters["DiffRetentionPeriod"] = value;
				}
			}

			public virtual Schedule Schedule
			{
				set
				{
					base.PowerSharpParameters["Schedule"] = value;
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
			public virtual OfflineAddressBookIdParameter Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = value;
				}
			}

			public virtual string GeneratingMailbox
			{
				set
				{
					base.PowerSharpParameters["GeneratingMailbox"] = ((value != null) ? new MailboxIdParameter(value) : null);
				}
			}

			public virtual AddressBookBaseIdParameter AddressLists
			{
				set
				{
					base.PowerSharpParameters["AddressLists"] = value;
				}
			}

			public virtual VirtualDirectoryIdParameter VirtualDirectories
			{
				set
				{
					base.PowerSharpParameters["VirtualDirectories"] = value;
				}
			}

			public virtual SwitchParameter ApplyMandatoryProperties
			{
				set
				{
					base.PowerSharpParameters["ApplyMandatoryProperties"] = value;
				}
			}

			public virtual SwitchParameter UseDefaultAttributes
			{
				set
				{
					base.PowerSharpParameters["UseDefaultAttributes"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual MultiValuedProperty<OfflineAddressBookVersion> Versions
			{
				set
				{
					base.PowerSharpParameters["Versions"] = value;
				}
			}

			public virtual bool IsDefault
			{
				set
				{
					base.PowerSharpParameters["IsDefault"] = value;
				}
			}

			public virtual bool PublicFolderDistributionEnabled
			{
				set
				{
					base.PowerSharpParameters["PublicFolderDistributionEnabled"] = value;
				}
			}

			public virtual bool GlobalWebDistributionEnabled
			{
				set
				{
					base.PowerSharpParameters["GlobalWebDistributionEnabled"] = value;
				}
			}

			public virtual bool ShadowMailboxDistributionEnabled
			{
				set
				{
					base.PowerSharpParameters["ShadowMailboxDistributionEnabled"] = value;
				}
			}

			public virtual int MaxBinaryPropertySize
			{
				set
				{
					base.PowerSharpParameters["MaxBinaryPropertySize"] = value;
				}
			}

			public virtual int MaxMultivaluedBinaryPropertySize
			{
				set
				{
					base.PowerSharpParameters["MaxMultivaluedBinaryPropertySize"] = value;
				}
			}

			public virtual int MaxStringPropertySize
			{
				set
				{
					base.PowerSharpParameters["MaxStringPropertySize"] = value;
				}
			}

			public virtual int MaxMultivaluedStringPropertySize
			{
				set
				{
					base.PowerSharpParameters["MaxMultivaluedStringPropertySize"] = value;
				}
			}

			public virtual MultiValuedProperty<OfflineAddressBookMapiProperty> ConfiguredAttributes
			{
				set
				{
					base.PowerSharpParameters["ConfiguredAttributes"] = value;
				}
			}

			public virtual Unlimited<int>? DiffRetentionPeriod
			{
				set
				{
					base.PowerSharpParameters["DiffRetentionPeriod"] = value;
				}
			}

			public virtual Schedule Schedule
			{
				set
				{
					base.PowerSharpParameters["Schedule"] = value;
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
