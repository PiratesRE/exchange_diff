using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class NewConsumerMailboxCommand : SyntheticCommandWithPipelineInput<ADUser, ADUser>
	{
		private NewConsumerMailboxCommand() : base("New-ConsumerMailbox")
		{
		}

		public NewConsumerMailboxCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual NewConsumerMailboxCommand SetParameters(NewConsumerMailboxCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual NewConsumerMailboxCommand SetParameters(NewConsumerMailboxCommand.ConsumerPrimaryMailboxParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual NewConsumerMailboxCommand SetParameters(NewConsumerMailboxCommand.ConsumerSecondaryMailboxParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual WindowsLiveId WindowsLiveID
			{
				set
				{
					base.PowerSharpParameters["WindowsLiveID"] = value;
				}
			}

			public virtual DatabaseIdParameter Database
			{
				set
				{
					base.PowerSharpParameters["Database"] = value;
				}
			}

			public virtual SwitchParameter Repair
			{
				set
				{
					base.PowerSharpParameters["Repair"] = value;
				}
			}

			public virtual string Gender
			{
				set
				{
					base.PowerSharpParameters["Gender"] = value;
				}
			}

			public virtual string Occupation
			{
				set
				{
					base.PowerSharpParameters["Occupation"] = value;
				}
			}

			public virtual string Region
			{
				set
				{
					base.PowerSharpParameters["Region"] = value;
				}
			}

			public virtual string Timezone
			{
				set
				{
					base.PowerSharpParameters["Timezone"] = value;
				}
			}

			public virtual DateTime Birthdate
			{
				set
				{
					base.PowerSharpParameters["Birthdate"] = value;
				}
			}

			public virtual string BirthdayPrecision
			{
				set
				{
					base.PowerSharpParameters["BirthdayPrecision"] = value;
				}
			}

			public virtual string NameVersion
			{
				set
				{
					base.PowerSharpParameters["NameVersion"] = value;
				}
			}

			public virtual string AlternateSupportEmailAddresses
			{
				set
				{
					base.PowerSharpParameters["AlternateSupportEmailAddresses"] = value;
				}
			}

			public virtual string PostalCode
			{
				set
				{
					base.PowerSharpParameters["PostalCode"] = value;
				}
			}

			public virtual string OptInUser
			{
				set
				{
					base.PowerSharpParameters["OptInUser"] = value;
				}
			}

			public virtual string MigrationDryRun
			{
				set
				{
					base.PowerSharpParameters["MigrationDryRun"] = value;
				}
			}

			public virtual string FirstName
			{
				set
				{
					base.PowerSharpParameters["FirstName"] = value;
				}
			}

			public virtual string LastName
			{
				set
				{
					base.PowerSharpParameters["LastName"] = value;
				}
			}

			public virtual CountryInfo UsageLocation
			{
				set
				{
					base.PowerSharpParameters["UsageLocation"] = value;
				}
			}

			public virtual ProxyAddressCollection EmailAddresses
			{
				set
				{
					base.PowerSharpParameters["EmailAddresses"] = value;
				}
			}

			public virtual MultiValuedProperty<int> LocaleID
			{
				set
				{
					base.PowerSharpParameters["LocaleID"] = value;
				}
			}

			public virtual bool FblEnabled
			{
				set
				{
					base.PowerSharpParameters["FblEnabled"] = value;
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

		public class ConsumerPrimaryMailboxParameters : ParametersBase
		{
			public virtual SwitchParameter MakeExoPrimary
			{
				set
				{
					base.PowerSharpParameters["MakeExoPrimary"] = value;
				}
			}

			public virtual SwitchParameter SkipMigration
			{
				set
				{
					base.PowerSharpParameters["SkipMigration"] = value;
				}
			}

			public virtual WindowsLiveId WindowsLiveID
			{
				set
				{
					base.PowerSharpParameters["WindowsLiveID"] = value;
				}
			}

			public virtual DatabaseIdParameter Database
			{
				set
				{
					base.PowerSharpParameters["Database"] = value;
				}
			}

			public virtual SwitchParameter Repair
			{
				set
				{
					base.PowerSharpParameters["Repair"] = value;
				}
			}

			public virtual string Gender
			{
				set
				{
					base.PowerSharpParameters["Gender"] = value;
				}
			}

			public virtual string Occupation
			{
				set
				{
					base.PowerSharpParameters["Occupation"] = value;
				}
			}

			public virtual string Region
			{
				set
				{
					base.PowerSharpParameters["Region"] = value;
				}
			}

			public virtual string Timezone
			{
				set
				{
					base.PowerSharpParameters["Timezone"] = value;
				}
			}

			public virtual DateTime Birthdate
			{
				set
				{
					base.PowerSharpParameters["Birthdate"] = value;
				}
			}

			public virtual string BirthdayPrecision
			{
				set
				{
					base.PowerSharpParameters["BirthdayPrecision"] = value;
				}
			}

			public virtual string NameVersion
			{
				set
				{
					base.PowerSharpParameters["NameVersion"] = value;
				}
			}

			public virtual string AlternateSupportEmailAddresses
			{
				set
				{
					base.PowerSharpParameters["AlternateSupportEmailAddresses"] = value;
				}
			}

			public virtual string PostalCode
			{
				set
				{
					base.PowerSharpParameters["PostalCode"] = value;
				}
			}

			public virtual string OptInUser
			{
				set
				{
					base.PowerSharpParameters["OptInUser"] = value;
				}
			}

			public virtual string MigrationDryRun
			{
				set
				{
					base.PowerSharpParameters["MigrationDryRun"] = value;
				}
			}

			public virtual string FirstName
			{
				set
				{
					base.PowerSharpParameters["FirstName"] = value;
				}
			}

			public virtual string LastName
			{
				set
				{
					base.PowerSharpParameters["LastName"] = value;
				}
			}

			public virtual CountryInfo UsageLocation
			{
				set
				{
					base.PowerSharpParameters["UsageLocation"] = value;
				}
			}

			public virtual ProxyAddressCollection EmailAddresses
			{
				set
				{
					base.PowerSharpParameters["EmailAddresses"] = value;
				}
			}

			public virtual MultiValuedProperty<int> LocaleID
			{
				set
				{
					base.PowerSharpParameters["LocaleID"] = value;
				}
			}

			public virtual bool FblEnabled
			{
				set
				{
					base.PowerSharpParameters["FblEnabled"] = value;
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

		public class ConsumerSecondaryMailboxParameters : ParametersBase
		{
			public virtual SwitchParameter MakeExoSecondary
			{
				set
				{
					base.PowerSharpParameters["MakeExoSecondary"] = value;
				}
			}

			public virtual WindowsLiveId WindowsLiveID
			{
				set
				{
					base.PowerSharpParameters["WindowsLiveID"] = value;
				}
			}

			public virtual DatabaseIdParameter Database
			{
				set
				{
					base.PowerSharpParameters["Database"] = value;
				}
			}

			public virtual SwitchParameter Repair
			{
				set
				{
					base.PowerSharpParameters["Repair"] = value;
				}
			}

			public virtual string Gender
			{
				set
				{
					base.PowerSharpParameters["Gender"] = value;
				}
			}

			public virtual string Occupation
			{
				set
				{
					base.PowerSharpParameters["Occupation"] = value;
				}
			}

			public virtual string Region
			{
				set
				{
					base.PowerSharpParameters["Region"] = value;
				}
			}

			public virtual string Timezone
			{
				set
				{
					base.PowerSharpParameters["Timezone"] = value;
				}
			}

			public virtual DateTime Birthdate
			{
				set
				{
					base.PowerSharpParameters["Birthdate"] = value;
				}
			}

			public virtual string BirthdayPrecision
			{
				set
				{
					base.PowerSharpParameters["BirthdayPrecision"] = value;
				}
			}

			public virtual string NameVersion
			{
				set
				{
					base.PowerSharpParameters["NameVersion"] = value;
				}
			}

			public virtual string AlternateSupportEmailAddresses
			{
				set
				{
					base.PowerSharpParameters["AlternateSupportEmailAddresses"] = value;
				}
			}

			public virtual string PostalCode
			{
				set
				{
					base.PowerSharpParameters["PostalCode"] = value;
				}
			}

			public virtual string OptInUser
			{
				set
				{
					base.PowerSharpParameters["OptInUser"] = value;
				}
			}

			public virtual string MigrationDryRun
			{
				set
				{
					base.PowerSharpParameters["MigrationDryRun"] = value;
				}
			}

			public virtual string FirstName
			{
				set
				{
					base.PowerSharpParameters["FirstName"] = value;
				}
			}

			public virtual string LastName
			{
				set
				{
					base.PowerSharpParameters["LastName"] = value;
				}
			}

			public virtual CountryInfo UsageLocation
			{
				set
				{
					base.PowerSharpParameters["UsageLocation"] = value;
				}
			}

			public virtual ProxyAddressCollection EmailAddresses
			{
				set
				{
					base.PowerSharpParameters["EmailAddresses"] = value;
				}
			}

			public virtual MultiValuedProperty<int> LocaleID
			{
				set
				{
					base.PowerSharpParameters["LocaleID"] = value;
				}
			}

			public virtual bool FblEnabled
			{
				set
				{
					base.PowerSharpParameters["FblEnabled"] = value;
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
