using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage.Management;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class NewMigrationEndpointCommand : SyntheticCommandWithPipelineInput<MigrationEndpoint, MigrationEndpoint>
	{
		private NewMigrationEndpointCommand() : base("New-MigrationEndpoint")
		{
		}

		public NewMigrationEndpointCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual NewMigrationEndpointCommand SetParameters(NewMigrationEndpointCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual NewMigrationEndpointCommand SetParameters(NewMigrationEndpointCommand.PSTImportParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual NewMigrationEndpointCommand SetParameters(NewMigrationEndpointCommand.ExchangeOutlookAnywhereParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual NewMigrationEndpointCommand SetParameters(NewMigrationEndpointCommand.ExchangeOutlookAnywhereAutoDiscoverParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual NewMigrationEndpointCommand SetParameters(NewMigrationEndpointCommand.ExchangeRemoteMoveParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual NewMigrationEndpointCommand SetParameters(NewMigrationEndpointCommand.ExchangeRemoteMoveAutoDiscoverParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual NewMigrationEndpointCommand SetParameters(NewMigrationEndpointCommand.PublicFolderParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual NewMigrationEndpointCommand SetParameters(NewMigrationEndpointCommand.IMAPParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual string Name
			{
				set
				{
					base.PowerSharpParameters["Name"] = value;
				}
			}

			public virtual Unlimited<int> MaxConcurrentMigrations
			{
				set
				{
					base.PowerSharpParameters["MaxConcurrentMigrations"] = value;
				}
			}

			public virtual Unlimited<int> MaxConcurrentIncrementalSyncs
			{
				set
				{
					base.PowerSharpParameters["MaxConcurrentIncrementalSyncs"] = value;
				}
			}

			public virtual SwitchParameter SkipVerification
			{
				set
				{
					base.PowerSharpParameters["SkipVerification"] = value;
				}
			}

			public virtual string Organization
			{
				set
				{
					base.PowerSharpParameters["Organization"] = ((value != null) ? new OrganizationIdParameter(value) : null);
				}
			}

			public virtual string Partition
			{
				set
				{
					base.PowerSharpParameters["Partition"] = ((value != null) ? new MailboxIdParameter(value) : null);
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

		public class PSTImportParameters : ParametersBase
		{
			public virtual PSCredential Credentials
			{
				set
				{
					base.PowerSharpParameters["Credentials"] = value;
				}
			}

			public virtual Fqdn RemoteServer
			{
				set
				{
					base.PowerSharpParameters["RemoteServer"] = value;
				}
			}

			public virtual SwitchParameter PSTImport
			{
				set
				{
					base.PowerSharpParameters["PSTImport"] = value;
				}
			}

			public virtual string Name
			{
				set
				{
					base.PowerSharpParameters["Name"] = value;
				}
			}

			public virtual Unlimited<int> MaxConcurrentMigrations
			{
				set
				{
					base.PowerSharpParameters["MaxConcurrentMigrations"] = value;
				}
			}

			public virtual Unlimited<int> MaxConcurrentIncrementalSyncs
			{
				set
				{
					base.PowerSharpParameters["MaxConcurrentIncrementalSyncs"] = value;
				}
			}

			public virtual SwitchParameter SkipVerification
			{
				set
				{
					base.PowerSharpParameters["SkipVerification"] = value;
				}
			}

			public virtual string Organization
			{
				set
				{
					base.PowerSharpParameters["Organization"] = ((value != null) ? new OrganizationIdParameter(value) : null);
				}
			}

			public virtual string Partition
			{
				set
				{
					base.PowerSharpParameters["Partition"] = ((value != null) ? new MailboxIdParameter(value) : null);
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

		public class ExchangeOutlookAnywhereParameters : ParametersBase
		{
			public virtual PSCredential Credentials
			{
				set
				{
					base.PowerSharpParameters["Credentials"] = value;
				}
			}

			public virtual SmtpAddress EmailAddress
			{
				set
				{
					base.PowerSharpParameters["EmailAddress"] = value;
				}
			}

			public virtual MigrationMailboxPermission MailboxPermission
			{
				set
				{
					base.PowerSharpParameters["MailboxPermission"] = value;
				}
			}

			public virtual string SourceMailboxLegacyDN
			{
				set
				{
					base.PowerSharpParameters["SourceMailboxLegacyDN"] = value;
				}
			}

			public virtual string TestMailbox
			{
				set
				{
					base.PowerSharpParameters["TestMailbox"] = ((value != null) ? new MailboxIdParameter(value) : null);
				}
			}

			public virtual string ExchangeServer
			{
				set
				{
					base.PowerSharpParameters["ExchangeServer"] = value;
				}
			}

			public virtual Fqdn RpcProxyServer
			{
				set
				{
					base.PowerSharpParameters["RpcProxyServer"] = value;
				}
			}

			public virtual string NspiServer
			{
				set
				{
					base.PowerSharpParameters["NspiServer"] = value;
				}
			}

			public virtual AuthenticationMethod Authentication
			{
				set
				{
					base.PowerSharpParameters["Authentication"] = value;
				}
			}

			public virtual SwitchParameter ExchangeOutlookAnywhere
			{
				set
				{
					base.PowerSharpParameters["ExchangeOutlookAnywhere"] = value;
				}
			}

			public virtual string Name
			{
				set
				{
					base.PowerSharpParameters["Name"] = value;
				}
			}

			public virtual Unlimited<int> MaxConcurrentMigrations
			{
				set
				{
					base.PowerSharpParameters["MaxConcurrentMigrations"] = value;
				}
			}

			public virtual Unlimited<int> MaxConcurrentIncrementalSyncs
			{
				set
				{
					base.PowerSharpParameters["MaxConcurrentIncrementalSyncs"] = value;
				}
			}

			public virtual SwitchParameter SkipVerification
			{
				set
				{
					base.PowerSharpParameters["SkipVerification"] = value;
				}
			}

			public virtual string Organization
			{
				set
				{
					base.PowerSharpParameters["Organization"] = ((value != null) ? new OrganizationIdParameter(value) : null);
				}
			}

			public virtual string Partition
			{
				set
				{
					base.PowerSharpParameters["Partition"] = ((value != null) ? new MailboxIdParameter(value) : null);
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

		public class ExchangeOutlookAnywhereAutoDiscoverParameters : ParametersBase
		{
			public virtual PSCredential Credentials
			{
				set
				{
					base.PowerSharpParameters["Credentials"] = value;
				}
			}

			public virtual SmtpAddress EmailAddress
			{
				set
				{
					base.PowerSharpParameters["EmailAddress"] = value;
				}
			}

			public virtual MigrationMailboxPermission MailboxPermission
			{
				set
				{
					base.PowerSharpParameters["MailboxPermission"] = value;
				}
			}

			public virtual string SourceMailboxLegacyDN
			{
				set
				{
					base.PowerSharpParameters["SourceMailboxLegacyDN"] = value;
				}
			}

			public virtual string TestMailbox
			{
				set
				{
					base.PowerSharpParameters["TestMailbox"] = ((value != null) ? new MailboxIdParameter(value) : null);
				}
			}

			public virtual SwitchParameter ExchangeOutlookAnywhere
			{
				set
				{
					base.PowerSharpParameters["ExchangeOutlookAnywhere"] = value;
				}
			}

			public virtual SwitchParameter Autodiscover
			{
				set
				{
					base.PowerSharpParameters["Autodiscover"] = value;
				}
			}

			public virtual string Name
			{
				set
				{
					base.PowerSharpParameters["Name"] = value;
				}
			}

			public virtual Unlimited<int> MaxConcurrentMigrations
			{
				set
				{
					base.PowerSharpParameters["MaxConcurrentMigrations"] = value;
				}
			}

			public virtual Unlimited<int> MaxConcurrentIncrementalSyncs
			{
				set
				{
					base.PowerSharpParameters["MaxConcurrentIncrementalSyncs"] = value;
				}
			}

			public virtual SwitchParameter SkipVerification
			{
				set
				{
					base.PowerSharpParameters["SkipVerification"] = value;
				}
			}

			public virtual string Organization
			{
				set
				{
					base.PowerSharpParameters["Organization"] = ((value != null) ? new OrganizationIdParameter(value) : null);
				}
			}

			public virtual string Partition
			{
				set
				{
					base.PowerSharpParameters["Partition"] = ((value != null) ? new MailboxIdParameter(value) : null);
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

		public class ExchangeRemoteMoveParameters : ParametersBase
		{
			public virtual PSCredential Credentials
			{
				set
				{
					base.PowerSharpParameters["Credentials"] = value;
				}
			}

			public virtual Fqdn RemoteServer
			{
				set
				{
					base.PowerSharpParameters["RemoteServer"] = value;
				}
			}

			public virtual SwitchParameter ExchangeRemoteMove
			{
				set
				{
					base.PowerSharpParameters["ExchangeRemoteMove"] = value;
				}
			}

			public virtual string Name
			{
				set
				{
					base.PowerSharpParameters["Name"] = value;
				}
			}

			public virtual Unlimited<int> MaxConcurrentMigrations
			{
				set
				{
					base.PowerSharpParameters["MaxConcurrentMigrations"] = value;
				}
			}

			public virtual Unlimited<int> MaxConcurrentIncrementalSyncs
			{
				set
				{
					base.PowerSharpParameters["MaxConcurrentIncrementalSyncs"] = value;
				}
			}

			public virtual SwitchParameter SkipVerification
			{
				set
				{
					base.PowerSharpParameters["SkipVerification"] = value;
				}
			}

			public virtual string Organization
			{
				set
				{
					base.PowerSharpParameters["Organization"] = ((value != null) ? new OrganizationIdParameter(value) : null);
				}
			}

			public virtual string Partition
			{
				set
				{
					base.PowerSharpParameters["Partition"] = ((value != null) ? new MailboxIdParameter(value) : null);
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

		public class ExchangeRemoteMoveAutoDiscoverParameters : ParametersBase
		{
			public virtual PSCredential Credentials
			{
				set
				{
					base.PowerSharpParameters["Credentials"] = value;
				}
			}

			public virtual SmtpAddress EmailAddress
			{
				set
				{
					base.PowerSharpParameters["EmailAddress"] = value;
				}
			}

			public virtual SwitchParameter ExchangeRemoteMove
			{
				set
				{
					base.PowerSharpParameters["ExchangeRemoteMove"] = value;
				}
			}

			public virtual SwitchParameter Autodiscover
			{
				set
				{
					base.PowerSharpParameters["Autodiscover"] = value;
				}
			}

			public virtual string Name
			{
				set
				{
					base.PowerSharpParameters["Name"] = value;
				}
			}

			public virtual Unlimited<int> MaxConcurrentMigrations
			{
				set
				{
					base.PowerSharpParameters["MaxConcurrentMigrations"] = value;
				}
			}

			public virtual Unlimited<int> MaxConcurrentIncrementalSyncs
			{
				set
				{
					base.PowerSharpParameters["MaxConcurrentIncrementalSyncs"] = value;
				}
			}

			public virtual SwitchParameter SkipVerification
			{
				set
				{
					base.PowerSharpParameters["SkipVerification"] = value;
				}
			}

			public virtual string Organization
			{
				set
				{
					base.PowerSharpParameters["Organization"] = ((value != null) ? new OrganizationIdParameter(value) : null);
				}
			}

			public virtual string Partition
			{
				set
				{
					base.PowerSharpParameters["Partition"] = ((value != null) ? new MailboxIdParameter(value) : null);
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

		public class PublicFolderParameters : ParametersBase
		{
			public virtual PSCredential Credentials
			{
				set
				{
					base.PowerSharpParameters["Credentials"] = value;
				}
			}

			public virtual string SourceMailboxLegacyDN
			{
				set
				{
					base.PowerSharpParameters["SourceMailboxLegacyDN"] = value;
				}
			}

			public virtual string PublicFolderDatabaseServerLegacyDN
			{
				set
				{
					base.PowerSharpParameters["PublicFolderDatabaseServerLegacyDN"] = value;
				}
			}

			public virtual string TestMailbox
			{
				set
				{
					base.PowerSharpParameters["TestMailbox"] = ((value != null) ? new MailboxIdParameter(value) : null);
				}
			}

			public virtual Fqdn RpcProxyServer
			{
				set
				{
					base.PowerSharpParameters["RpcProxyServer"] = value;
				}
			}

			public virtual AuthenticationMethod Authentication
			{
				set
				{
					base.PowerSharpParameters["Authentication"] = value;
				}
			}

			public virtual SwitchParameter PublicFolder
			{
				set
				{
					base.PowerSharpParameters["PublicFolder"] = value;
				}
			}

			public virtual string Name
			{
				set
				{
					base.PowerSharpParameters["Name"] = value;
				}
			}

			public virtual Unlimited<int> MaxConcurrentMigrations
			{
				set
				{
					base.PowerSharpParameters["MaxConcurrentMigrations"] = value;
				}
			}

			public virtual Unlimited<int> MaxConcurrentIncrementalSyncs
			{
				set
				{
					base.PowerSharpParameters["MaxConcurrentIncrementalSyncs"] = value;
				}
			}

			public virtual SwitchParameter SkipVerification
			{
				set
				{
					base.PowerSharpParameters["SkipVerification"] = value;
				}
			}

			public virtual string Organization
			{
				set
				{
					base.PowerSharpParameters["Organization"] = ((value != null) ? new OrganizationIdParameter(value) : null);
				}
			}

			public virtual string Partition
			{
				set
				{
					base.PowerSharpParameters["Partition"] = ((value != null) ? new MailboxIdParameter(value) : null);
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

		public class IMAPParameters : ParametersBase
		{
			public virtual Fqdn RemoteServer
			{
				set
				{
					base.PowerSharpParameters["RemoteServer"] = value;
				}
			}

			public virtual int Port
			{
				set
				{
					base.PowerSharpParameters["Port"] = value;
				}
			}

			public virtual AuthenticationMethod Authentication
			{
				set
				{
					base.PowerSharpParameters["Authentication"] = value;
				}
			}

			public virtual IMAPSecurityMechanism Security
			{
				set
				{
					base.PowerSharpParameters["Security"] = value;
				}
			}

			public virtual SwitchParameter IMAP
			{
				set
				{
					base.PowerSharpParameters["IMAP"] = value;
				}
			}

			public virtual string Name
			{
				set
				{
					base.PowerSharpParameters["Name"] = value;
				}
			}

			public virtual Unlimited<int> MaxConcurrentMigrations
			{
				set
				{
					base.PowerSharpParameters["MaxConcurrentMigrations"] = value;
				}
			}

			public virtual Unlimited<int> MaxConcurrentIncrementalSyncs
			{
				set
				{
					base.PowerSharpParameters["MaxConcurrentIncrementalSyncs"] = value;
				}
			}

			public virtual SwitchParameter SkipVerification
			{
				set
				{
					base.PowerSharpParameters["SkipVerification"] = value;
				}
			}

			public virtual string Organization
			{
				set
				{
					base.PowerSharpParameters["Organization"] = ((value != null) ? new OrganizationIdParameter(value) : null);
				}
			}

			public virtual string Partition
			{
				set
				{
					base.PowerSharpParameters["Partition"] = ((value != null) ? new MailboxIdParameter(value) : null);
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
