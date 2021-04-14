using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage.Management;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class SetMailboxMessageConfigurationCommand : SyntheticCommandWithPipelineInputNoOutput<MailboxMessageConfiguration>
	{
		private SetMailboxMessageConfigurationCommand() : base("Set-MailboxMessageConfiguration")
		{
		}

		public SetMailboxMessageConfigurationCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual SetMailboxMessageConfigurationCommand SetParameters(SetMailboxMessageConfigurationCommand.IdentityParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SetMailboxMessageConfigurationCommand SetParameters(SetMailboxMessageConfigurationCommand.DefaultParameters parameters)
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
					base.PowerSharpParameters["Identity"] = ((value != null) ? new MailboxIdParameter(value) : null);
				}
			}

			public virtual SwitchParameter IgnoreDefaultScope
			{
				set
				{
					base.PowerSharpParameters["IgnoreDefaultScope"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual AfterMoveOrDeleteBehavior AfterMoveOrDeleteBehavior
			{
				set
				{
					base.PowerSharpParameters["AfterMoveOrDeleteBehavior"] = value;
				}
			}

			public virtual NewItemNotification NewItemNotification
			{
				set
				{
					base.PowerSharpParameters["NewItemNotification"] = value;
				}
			}

			public virtual bool EmptyDeletedItemsOnLogoff
			{
				set
				{
					base.PowerSharpParameters["EmptyDeletedItemsOnLogoff"] = value;
				}
			}

			public virtual bool AutoAddSignature
			{
				set
				{
					base.PowerSharpParameters["AutoAddSignature"] = value;
				}
			}

			public virtual string SignatureText
			{
				set
				{
					base.PowerSharpParameters["SignatureText"] = value;
				}
			}

			public virtual string SignatureHtml
			{
				set
				{
					base.PowerSharpParameters["SignatureHtml"] = value;
				}
			}

			public virtual bool AutoAddSignatureOnMobile
			{
				set
				{
					base.PowerSharpParameters["AutoAddSignatureOnMobile"] = value;
				}
			}

			public virtual string SignatureTextOnMobile
			{
				set
				{
					base.PowerSharpParameters["SignatureTextOnMobile"] = value;
				}
			}

			public virtual bool UseDefaultSignatureOnMobile
			{
				set
				{
					base.PowerSharpParameters["UseDefaultSignatureOnMobile"] = value;
				}
			}

			public virtual string DefaultFontName
			{
				set
				{
					base.PowerSharpParameters["DefaultFontName"] = value;
				}
			}

			public virtual int DefaultFontSize
			{
				set
				{
					base.PowerSharpParameters["DefaultFontSize"] = value;
				}
			}

			public virtual string DefaultFontColor
			{
				set
				{
					base.PowerSharpParameters["DefaultFontColor"] = value;
				}
			}

			public virtual FontFlags DefaultFontFlags
			{
				set
				{
					base.PowerSharpParameters["DefaultFontFlags"] = value;
				}
			}

			public virtual bool AlwaysShowBcc
			{
				set
				{
					base.PowerSharpParameters["AlwaysShowBcc"] = value;
				}
			}

			public virtual bool AlwaysShowFrom
			{
				set
				{
					base.PowerSharpParameters["AlwaysShowFrom"] = value;
				}
			}

			public virtual MailFormat DefaultFormat
			{
				set
				{
					base.PowerSharpParameters["DefaultFormat"] = value;
				}
			}

			public virtual ReadReceiptResponse ReadReceiptResponse
			{
				set
				{
					base.PowerSharpParameters["ReadReceiptResponse"] = value;
				}
			}

			public virtual PreviewMarkAsReadBehavior PreviewMarkAsReadBehavior
			{
				set
				{
					base.PowerSharpParameters["PreviewMarkAsReadBehavior"] = value;
				}
			}

			public virtual int PreviewMarkAsReadDelaytime
			{
				set
				{
					base.PowerSharpParameters["PreviewMarkAsReadDelaytime"] = value;
				}
			}

			public virtual ConversationSortOrder ConversationSortOrder
			{
				set
				{
					base.PowerSharpParameters["ConversationSortOrder"] = value;
				}
			}

			public virtual bool ShowConversationAsTree
			{
				set
				{
					base.PowerSharpParameters["ShowConversationAsTree"] = value;
				}
			}

			public virtual bool HideDeletedItems
			{
				set
				{
					base.PowerSharpParameters["HideDeletedItems"] = value;
				}
			}

			public virtual string SendAddressDefault
			{
				set
				{
					base.PowerSharpParameters["SendAddressDefault"] = value;
				}
			}

			public virtual EmailComposeMode EmailComposeMode
			{
				set
				{
					base.PowerSharpParameters["EmailComposeMode"] = value;
				}
			}

			public virtual bool CheckForForgottenAttachments
			{
				set
				{
					base.PowerSharpParameters["CheckForForgottenAttachments"] = value;
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
			public virtual SwitchParameter IgnoreDefaultScope
			{
				set
				{
					base.PowerSharpParameters["IgnoreDefaultScope"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual AfterMoveOrDeleteBehavior AfterMoveOrDeleteBehavior
			{
				set
				{
					base.PowerSharpParameters["AfterMoveOrDeleteBehavior"] = value;
				}
			}

			public virtual NewItemNotification NewItemNotification
			{
				set
				{
					base.PowerSharpParameters["NewItemNotification"] = value;
				}
			}

			public virtual bool EmptyDeletedItemsOnLogoff
			{
				set
				{
					base.PowerSharpParameters["EmptyDeletedItemsOnLogoff"] = value;
				}
			}

			public virtual bool AutoAddSignature
			{
				set
				{
					base.PowerSharpParameters["AutoAddSignature"] = value;
				}
			}

			public virtual string SignatureText
			{
				set
				{
					base.PowerSharpParameters["SignatureText"] = value;
				}
			}

			public virtual string SignatureHtml
			{
				set
				{
					base.PowerSharpParameters["SignatureHtml"] = value;
				}
			}

			public virtual bool AutoAddSignatureOnMobile
			{
				set
				{
					base.PowerSharpParameters["AutoAddSignatureOnMobile"] = value;
				}
			}

			public virtual string SignatureTextOnMobile
			{
				set
				{
					base.PowerSharpParameters["SignatureTextOnMobile"] = value;
				}
			}

			public virtual bool UseDefaultSignatureOnMobile
			{
				set
				{
					base.PowerSharpParameters["UseDefaultSignatureOnMobile"] = value;
				}
			}

			public virtual string DefaultFontName
			{
				set
				{
					base.PowerSharpParameters["DefaultFontName"] = value;
				}
			}

			public virtual int DefaultFontSize
			{
				set
				{
					base.PowerSharpParameters["DefaultFontSize"] = value;
				}
			}

			public virtual string DefaultFontColor
			{
				set
				{
					base.PowerSharpParameters["DefaultFontColor"] = value;
				}
			}

			public virtual FontFlags DefaultFontFlags
			{
				set
				{
					base.PowerSharpParameters["DefaultFontFlags"] = value;
				}
			}

			public virtual bool AlwaysShowBcc
			{
				set
				{
					base.PowerSharpParameters["AlwaysShowBcc"] = value;
				}
			}

			public virtual bool AlwaysShowFrom
			{
				set
				{
					base.PowerSharpParameters["AlwaysShowFrom"] = value;
				}
			}

			public virtual MailFormat DefaultFormat
			{
				set
				{
					base.PowerSharpParameters["DefaultFormat"] = value;
				}
			}

			public virtual ReadReceiptResponse ReadReceiptResponse
			{
				set
				{
					base.PowerSharpParameters["ReadReceiptResponse"] = value;
				}
			}

			public virtual PreviewMarkAsReadBehavior PreviewMarkAsReadBehavior
			{
				set
				{
					base.PowerSharpParameters["PreviewMarkAsReadBehavior"] = value;
				}
			}

			public virtual int PreviewMarkAsReadDelaytime
			{
				set
				{
					base.PowerSharpParameters["PreviewMarkAsReadDelaytime"] = value;
				}
			}

			public virtual ConversationSortOrder ConversationSortOrder
			{
				set
				{
					base.PowerSharpParameters["ConversationSortOrder"] = value;
				}
			}

			public virtual bool ShowConversationAsTree
			{
				set
				{
					base.PowerSharpParameters["ShowConversationAsTree"] = value;
				}
			}

			public virtual bool HideDeletedItems
			{
				set
				{
					base.PowerSharpParameters["HideDeletedItems"] = value;
				}
			}

			public virtual string SendAddressDefault
			{
				set
				{
					base.PowerSharpParameters["SendAddressDefault"] = value;
				}
			}

			public virtual EmailComposeMode EmailComposeMode
			{
				set
				{
					base.PowerSharpParameters["EmailComposeMode"] = value;
				}
			}

			public virtual bool CheckForForgottenAttachments
			{
				set
				{
					base.PowerSharpParameters["CheckForForgottenAttachments"] = value;
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
