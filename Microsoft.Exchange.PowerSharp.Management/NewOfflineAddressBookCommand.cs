﻿using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class NewOfflineAddressBookCommand : SyntheticCommandWithPipelineInputNoOutput<AddressBookBaseIdParameter>
	{
		private NewOfflineAddressBookCommand() : base("New-OfflineAddressBook")
		{
		}

		public NewOfflineAddressBookCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual NewOfflineAddressBookCommand SetParameters(NewOfflineAddressBookCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual AddressBookBaseIdParameter AddressLists
			{
				set
				{
					base.PowerSharpParameters["AddressLists"] = value;
				}
			}

			public virtual bool IsDefault
			{
				set
				{
					base.PowerSharpParameters["IsDefault"] = value;
				}
			}

			public virtual string GeneratingMailbox
			{
				set
				{
					base.PowerSharpParameters["GeneratingMailbox"] = ((value != null) ? new MailboxIdParameter(value) : null);
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

			public virtual Unlimited<int>? DiffRetentionPeriod
			{
				set
				{
					base.PowerSharpParameters["DiffRetentionPeriod"] = value;
				}
			}

			public virtual VirtualDirectoryIdParameter VirtualDirectories
			{
				set
				{
					base.PowerSharpParameters["VirtualDirectories"] = value;
				}
			}

			public virtual string Organization
			{
				set
				{
					base.PowerSharpParameters["Organization"] = ((value != null) ? new OrganizationIdParameter(value) : null);
				}
			}

			public virtual string Name
			{
				set
				{
					base.PowerSharpParameters["Name"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
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
