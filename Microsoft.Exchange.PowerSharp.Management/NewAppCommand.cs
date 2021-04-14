using System;
using System.IO;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic.Extension;
using Microsoft.Exchange.Management.Extension;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class NewAppCommand : SyntheticCommandWithPipelineInput<App, App>
	{
		private NewAppCommand() : base("New-App")
		{
		}

		public NewAppCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual NewAppCommand SetParameters(NewAppCommand.ExtensionOfficeMarketplaceParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual NewAppCommand SetParameters(NewAppCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual NewAppCommand SetParameters(NewAppCommand.ExtensionFileDataParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual NewAppCommand SetParameters(NewAppCommand.ExtensionFileStreamParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual NewAppCommand SetParameters(NewAppCommand.ExtensionPrivateURLParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class ExtensionOfficeMarketplaceParameters : ParametersBase
		{
			public virtual string MarketplaceAssetID
			{
				set
				{
					base.PowerSharpParameters["MarketplaceAssetID"] = value;
				}
			}

			public virtual string MarketplaceQueryMarket
			{
				set
				{
					base.PowerSharpParameters["MarketplaceQueryMarket"] = value;
				}
			}

			public virtual string Etoken
			{
				set
				{
					base.PowerSharpParameters["Etoken"] = value;
				}
			}

			public virtual string MarketplaceServicesUrl
			{
				set
				{
					base.PowerSharpParameters["MarketplaceServicesUrl"] = value;
				}
			}

			public virtual SwitchParameter DownloadOnly
			{
				set
				{
					base.PowerSharpParameters["DownloadOnly"] = value;
				}
			}

			public virtual string Mailbox
			{
				set
				{
					base.PowerSharpParameters["Mailbox"] = ((value != null) ? new MailboxIdParameter(value) : null);
				}
			}

			public virtual SwitchParameter OrganizationApp
			{
				set
				{
					base.PowerSharpParameters["OrganizationApp"] = value;
				}
			}

			public virtual ClientExtensionProvidedTo ProvidedTo
			{
				set
				{
					base.PowerSharpParameters["ProvidedTo"] = value;
				}
			}

			public virtual MultiValuedProperty<RecipientWithAdUserIdParameter<RecipientIdParameter>> UserList
			{
				set
				{
					base.PowerSharpParameters["UserList"] = value;
				}
			}

			public virtual DefaultStateForUser DefaultStateForUser
			{
				set
				{
					base.PowerSharpParameters["DefaultStateForUser"] = value;
				}
			}

			public virtual bool Enabled
			{
				set
				{
					base.PowerSharpParameters["Enabled"] = value;
				}
			}

			public virtual SwitchParameter AllowReadWriteMailbox
			{
				set
				{
					base.PowerSharpParameters["AllowReadWriteMailbox"] = value;
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
			public virtual SwitchParameter DownloadOnly
			{
				set
				{
					base.PowerSharpParameters["DownloadOnly"] = value;
				}
			}

			public virtual string Mailbox
			{
				set
				{
					base.PowerSharpParameters["Mailbox"] = ((value != null) ? new MailboxIdParameter(value) : null);
				}
			}

			public virtual SwitchParameter OrganizationApp
			{
				set
				{
					base.PowerSharpParameters["OrganizationApp"] = value;
				}
			}

			public virtual ClientExtensionProvidedTo ProvidedTo
			{
				set
				{
					base.PowerSharpParameters["ProvidedTo"] = value;
				}
			}

			public virtual MultiValuedProperty<RecipientWithAdUserIdParameter<RecipientIdParameter>> UserList
			{
				set
				{
					base.PowerSharpParameters["UserList"] = value;
				}
			}

			public virtual DefaultStateForUser DefaultStateForUser
			{
				set
				{
					base.PowerSharpParameters["DefaultStateForUser"] = value;
				}
			}

			public virtual bool Enabled
			{
				set
				{
					base.PowerSharpParameters["Enabled"] = value;
				}
			}

			public virtual SwitchParameter AllowReadWriteMailbox
			{
				set
				{
					base.PowerSharpParameters["AllowReadWriteMailbox"] = value;
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

		public class ExtensionFileDataParameters : ParametersBase
		{
			public virtual byte FileData
			{
				set
				{
					base.PowerSharpParameters["FileData"] = value;
				}
			}

			public virtual SwitchParameter DownloadOnly
			{
				set
				{
					base.PowerSharpParameters["DownloadOnly"] = value;
				}
			}

			public virtual string Mailbox
			{
				set
				{
					base.PowerSharpParameters["Mailbox"] = ((value != null) ? new MailboxIdParameter(value) : null);
				}
			}

			public virtual SwitchParameter OrganizationApp
			{
				set
				{
					base.PowerSharpParameters["OrganizationApp"] = value;
				}
			}

			public virtual ClientExtensionProvidedTo ProvidedTo
			{
				set
				{
					base.PowerSharpParameters["ProvidedTo"] = value;
				}
			}

			public virtual MultiValuedProperty<RecipientWithAdUserIdParameter<RecipientIdParameter>> UserList
			{
				set
				{
					base.PowerSharpParameters["UserList"] = value;
				}
			}

			public virtual DefaultStateForUser DefaultStateForUser
			{
				set
				{
					base.PowerSharpParameters["DefaultStateForUser"] = value;
				}
			}

			public virtual bool Enabled
			{
				set
				{
					base.PowerSharpParameters["Enabled"] = value;
				}
			}

			public virtual SwitchParameter AllowReadWriteMailbox
			{
				set
				{
					base.PowerSharpParameters["AllowReadWriteMailbox"] = value;
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

		public class ExtensionFileStreamParameters : ParametersBase
		{
			public virtual Stream FileStream
			{
				set
				{
					base.PowerSharpParameters["FileStream"] = value;
				}
			}

			public virtual SwitchParameter DownloadOnly
			{
				set
				{
					base.PowerSharpParameters["DownloadOnly"] = value;
				}
			}

			public virtual string Mailbox
			{
				set
				{
					base.PowerSharpParameters["Mailbox"] = ((value != null) ? new MailboxIdParameter(value) : null);
				}
			}

			public virtual SwitchParameter OrganizationApp
			{
				set
				{
					base.PowerSharpParameters["OrganizationApp"] = value;
				}
			}

			public virtual ClientExtensionProvidedTo ProvidedTo
			{
				set
				{
					base.PowerSharpParameters["ProvidedTo"] = value;
				}
			}

			public virtual MultiValuedProperty<RecipientWithAdUserIdParameter<RecipientIdParameter>> UserList
			{
				set
				{
					base.PowerSharpParameters["UserList"] = value;
				}
			}

			public virtual DefaultStateForUser DefaultStateForUser
			{
				set
				{
					base.PowerSharpParameters["DefaultStateForUser"] = value;
				}
			}

			public virtual bool Enabled
			{
				set
				{
					base.PowerSharpParameters["Enabled"] = value;
				}
			}

			public virtual SwitchParameter AllowReadWriteMailbox
			{
				set
				{
					base.PowerSharpParameters["AllowReadWriteMailbox"] = value;
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

		public class ExtensionPrivateURLParameters : ParametersBase
		{
			public virtual Uri Url
			{
				set
				{
					base.PowerSharpParameters["Url"] = value;
				}
			}

			public virtual SwitchParameter DownloadOnly
			{
				set
				{
					base.PowerSharpParameters["DownloadOnly"] = value;
				}
			}

			public virtual string Mailbox
			{
				set
				{
					base.PowerSharpParameters["Mailbox"] = ((value != null) ? new MailboxIdParameter(value) : null);
				}
			}

			public virtual SwitchParameter OrganizationApp
			{
				set
				{
					base.PowerSharpParameters["OrganizationApp"] = value;
				}
			}

			public virtual ClientExtensionProvidedTo ProvidedTo
			{
				set
				{
					base.PowerSharpParameters["ProvidedTo"] = value;
				}
			}

			public virtual MultiValuedProperty<RecipientWithAdUserIdParameter<RecipientIdParameter>> UserList
			{
				set
				{
					base.PowerSharpParameters["UserList"] = value;
				}
			}

			public virtual DefaultStateForUser DefaultStateForUser
			{
				set
				{
					base.PowerSharpParameters["DefaultStateForUser"] = value;
				}
			}

			public virtual bool Enabled
			{
				set
				{
					base.PowerSharpParameters["Enabled"] = value;
				}
			}

			public virtual SwitchParameter AllowReadWriteMailbox
			{
				set
				{
					base.PowerSharpParameters["AllowReadWriteMailbox"] = value;
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
