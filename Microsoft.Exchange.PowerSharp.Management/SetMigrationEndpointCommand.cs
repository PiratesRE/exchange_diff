using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Management.Migration;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class SetMigrationEndpointCommand : SyntheticCommandWithPipelineInputNoOutput<MigrationEndpoint>
	{
		private SetMigrationEndpointCommand() : base("Set-MigrationEndpoint")
		{
		}

		public SetMigrationEndpointCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual SetMigrationEndpointCommand SetParameters(SetMigrationEndpointCommand.IdentityParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SetMigrationEndpointCommand SetParameters(SetMigrationEndpointCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class IdentityParameters : ParametersBase
		{
			public virtual string Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = ((value != null) ? new MigrationEndpointIdParameter(value) : null);
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

			public virtual PSCredential Credentials
			{
				set
				{
					base.PowerSharpParameters["Credentials"] = value;
				}
			}

			public virtual MigrationMailboxPermission MailboxPermission
			{
				set
				{
					base.PowerSharpParameters["MailboxPermission"] = value;
				}
			}

			public virtual string ExchangeServer
			{
				set
				{
					base.PowerSharpParameters["ExchangeServer"] = value;
				}
			}

			public virtual Fqdn RemoteServer
			{
				set
				{
					base.PowerSharpParameters["RemoteServer"] = value;
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

			public virtual string TestMailbox
			{
				set
				{
					base.PowerSharpParameters["TestMailbox"] = ((value != null) ? new MailboxIdParameter(value) : null);
				}
			}

			public virtual SmtpAddress EmailAddress
			{
				set
				{
					base.PowerSharpParameters["EmailAddress"] = value;
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

		public class DefaultParameters : ParametersBase
		{
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

			public virtual PSCredential Credentials
			{
				set
				{
					base.PowerSharpParameters["Credentials"] = value;
				}
			}

			public virtual MigrationMailboxPermission MailboxPermission
			{
				set
				{
					base.PowerSharpParameters["MailboxPermission"] = value;
				}
			}

			public virtual string ExchangeServer
			{
				set
				{
					base.PowerSharpParameters["ExchangeServer"] = value;
				}
			}

			public virtual Fqdn RemoteServer
			{
				set
				{
					base.PowerSharpParameters["RemoteServer"] = value;
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

			public virtual string TestMailbox
			{
				set
				{
					base.PowerSharpParameters["TestMailbox"] = ((value != null) ? new MailboxIdParameter(value) : null);
				}
			}

			public virtual SmtpAddress EmailAddress
			{
				set
				{
					base.PowerSharpParameters["EmailAddress"] = value;
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
