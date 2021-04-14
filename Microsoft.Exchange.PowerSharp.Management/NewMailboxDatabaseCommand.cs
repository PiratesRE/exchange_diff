using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class NewMailboxDatabaseCommand : SyntheticCommandWithPipelineInput<MailboxDatabase, MailboxDatabase>
	{
		private NewMailboxDatabaseCommand() : base("New-MailboxDatabase")
		{
		}

		public NewMailboxDatabaseCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual NewMailboxDatabaseCommand SetParameters(NewMailboxDatabaseCommand.NonRecoveryParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual NewMailboxDatabaseCommand SetParameters(NewMailboxDatabaseCommand.RecoveryParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual NewMailboxDatabaseCommand SetParameters(NewMailboxDatabaseCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class NonRecoveryParameters : ParametersBase
		{
			public virtual string Name
			{
				set
				{
					base.PowerSharpParameters["Name"] = value;
				}
			}

			public virtual DatabaseIdParameter PublicFolderDatabase
			{
				set
				{
					base.PowerSharpParameters["PublicFolderDatabase"] = value;
				}
			}

			public virtual OfflineAddressBookIdParameter OfflineAddressBook
			{
				set
				{
					base.PowerSharpParameters["OfflineAddressBook"] = value;
				}
			}

			public virtual bool IsExcludedFromProvisioning
			{
				set
				{
					base.PowerSharpParameters["IsExcludedFromProvisioning"] = value;
				}
			}

			public virtual bool IsSuspendedFromProvisioning
			{
				set
				{
					base.PowerSharpParameters["IsSuspendedFromProvisioning"] = value;
				}
			}

			public virtual SwitchParameter IsExcludedFromInitialProvisioning
			{
				set
				{
					base.PowerSharpParameters["IsExcludedFromInitialProvisioning"] = value;
				}
			}

			public virtual bool AutoDagExcludeFromMonitoring
			{
				set
				{
					base.PowerSharpParameters["AutoDagExcludeFromMonitoring"] = value;
				}
			}

			public virtual MailboxProvisioningAttributes MailboxProvisioningAttributes
			{
				set
				{
					base.PowerSharpParameters["MailboxProvisioningAttributes"] = value;
				}
			}

			public virtual ServerIdParameter Server
			{
				set
				{
					base.PowerSharpParameters["Server"] = value;
				}
			}

			public virtual EdbFilePath EdbFilePath
			{
				set
				{
					base.PowerSharpParameters["EdbFilePath"] = value;
				}
			}

			public virtual NonRootLocalLongFullPath LogFolderPath
			{
				set
				{
					base.PowerSharpParameters["LogFolderPath"] = value;
				}
			}

			public virtual SwitchParameter SkipDatabaseLogFolderCreation
			{
				set
				{
					base.PowerSharpParameters["SkipDatabaseLogFolderCreation"] = value;
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

		public class RecoveryParameters : ParametersBase
		{
			public virtual string Name
			{
				set
				{
					base.PowerSharpParameters["Name"] = value;
				}
			}

			public virtual SwitchParameter Recovery
			{
				set
				{
					base.PowerSharpParameters["Recovery"] = value;
				}
			}

			public virtual MailboxProvisioningAttributes MailboxProvisioningAttributes
			{
				set
				{
					base.PowerSharpParameters["MailboxProvisioningAttributes"] = value;
				}
			}

			public virtual ServerIdParameter Server
			{
				set
				{
					base.PowerSharpParameters["Server"] = value;
				}
			}

			public virtual EdbFilePath EdbFilePath
			{
				set
				{
					base.PowerSharpParameters["EdbFilePath"] = value;
				}
			}

			public virtual NonRootLocalLongFullPath LogFolderPath
			{
				set
				{
					base.PowerSharpParameters["LogFolderPath"] = value;
				}
			}

			public virtual SwitchParameter SkipDatabaseLogFolderCreation
			{
				set
				{
					base.PowerSharpParameters["SkipDatabaseLogFolderCreation"] = value;
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

		public class DefaultParameters : ParametersBase
		{
			public virtual MailboxProvisioningAttributes MailboxProvisioningAttributes
			{
				set
				{
					base.PowerSharpParameters["MailboxProvisioningAttributes"] = value;
				}
			}

			public virtual ServerIdParameter Server
			{
				set
				{
					base.PowerSharpParameters["Server"] = value;
				}
			}

			public virtual EdbFilePath EdbFilePath
			{
				set
				{
					base.PowerSharpParameters["EdbFilePath"] = value;
				}
			}

			public virtual NonRootLocalLongFullPath LogFolderPath
			{
				set
				{
					base.PowerSharpParameters["LogFolderPath"] = value;
				}
			}

			public virtual SwitchParameter SkipDatabaseLogFolderCreation
			{
				set
				{
					base.PowerSharpParameters["SkipDatabaseLogFolderCreation"] = value;
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
