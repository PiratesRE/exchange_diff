using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class SetRetentionPolicyTagCommand : SyntheticCommandWithPipelineInputNoOutput<RetentionPolicyTag>
	{
		private SetRetentionPolicyTagCommand() : base("Set-RetentionPolicyTag")
		{
		}

		public SetRetentionPolicyTagCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual SetRetentionPolicyTagCommand SetParameters(SetRetentionPolicyTagCommand.IdentityParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SetRetentionPolicyTagCommand SetParameters(SetRetentionPolicyTagCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SetRetentionPolicyTagCommand SetParameters(SetRetentionPolicyTagCommand.ParameterSetMailboxTaskParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class IdentityParameters : ParametersBase
		{
			public virtual bool RetentionEnabled
			{
				set
				{
					base.PowerSharpParameters["RetentionEnabled"] = value;
				}
			}

			public virtual RetentionActionType RetentionAction
			{
				set
				{
					base.PowerSharpParameters["RetentionAction"] = value;
				}
			}

			public virtual string MessageClass
			{
				set
				{
					base.PowerSharpParameters["MessageClass"] = value;
				}
			}

			public virtual EnhancedTimeSpan? AgeLimitForRetention
			{
				set
				{
					base.PowerSharpParameters["AgeLimitForRetention"] = value;
				}
			}

			public virtual bool JournalingEnabled
			{
				set
				{
					base.PowerSharpParameters["JournalingEnabled"] = value;
				}
			}

			public virtual string AddressForJournaling
			{
				set
				{
					base.PowerSharpParameters["AddressForJournaling"] = ((value != null) ? new RecipientIdParameter(value) : null);
				}
			}

			public virtual JournalingFormat MessageFormatForJournaling
			{
				set
				{
					base.PowerSharpParameters["MessageFormatForJournaling"] = value;
				}
			}

			public virtual string LabelForJournaling
			{
				set
				{
					base.PowerSharpParameters["LabelForJournaling"] = value;
				}
			}

			public virtual string Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = ((value != null) ? new RetentionPolicyTagIdParameter(value) : null);
				}
			}

			public virtual string LegacyManagedFolder
			{
				set
				{
					base.PowerSharpParameters["LegacyManagedFolder"] = ((value != null) ? new ELCFolderIdParameter(value) : null);
				}
			}

			public virtual SwitchParameter Force
			{
				set
				{
					base.PowerSharpParameters["Force"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual bool SystemTag
			{
				set
				{
					base.PowerSharpParameters["SystemTag"] = value;
				}
			}

			public virtual MultiValuedProperty<string> LocalizedRetentionPolicyTagName
			{
				set
				{
					base.PowerSharpParameters["LocalizedRetentionPolicyTagName"] = value;
				}
			}

			public virtual string Comment
			{
				set
				{
					base.PowerSharpParameters["Comment"] = value;
				}
			}

			public virtual Guid RetentionId
			{
				set
				{
					base.PowerSharpParameters["RetentionId"] = value;
				}
			}

			public virtual MultiValuedProperty<string> LocalizedComment
			{
				set
				{
					base.PowerSharpParameters["LocalizedComment"] = value;
				}
			}

			public virtual bool MustDisplayCommentEnabled
			{
				set
				{
					base.PowerSharpParameters["MustDisplayCommentEnabled"] = value;
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

		public class DefaultParameters : ParametersBase
		{
			public virtual string LegacyManagedFolder
			{
				set
				{
					base.PowerSharpParameters["LegacyManagedFolder"] = ((value != null) ? new ELCFolderIdParameter(value) : null);
				}
			}

			public virtual SwitchParameter Force
			{
				set
				{
					base.PowerSharpParameters["Force"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual bool SystemTag
			{
				set
				{
					base.PowerSharpParameters["SystemTag"] = value;
				}
			}

			public virtual MultiValuedProperty<string> LocalizedRetentionPolicyTagName
			{
				set
				{
					base.PowerSharpParameters["LocalizedRetentionPolicyTagName"] = value;
				}
			}

			public virtual string Comment
			{
				set
				{
					base.PowerSharpParameters["Comment"] = value;
				}
			}

			public virtual Guid RetentionId
			{
				set
				{
					base.PowerSharpParameters["RetentionId"] = value;
				}
			}

			public virtual MultiValuedProperty<string> LocalizedComment
			{
				set
				{
					base.PowerSharpParameters["LocalizedComment"] = value;
				}
			}

			public virtual bool MustDisplayCommentEnabled
			{
				set
				{
					base.PowerSharpParameters["MustDisplayCommentEnabled"] = value;
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

		public class ParameterSetMailboxTaskParameters : ParametersBase
		{
			public virtual string Mailbox
			{
				set
				{
					base.PowerSharpParameters["Mailbox"] = ((value != null) ? new MailboxIdParameter(value) : null);
				}
			}

			public virtual RetentionPolicyTagIdParameter OptionalInMailbox
			{
				set
				{
					base.PowerSharpParameters["OptionalInMailbox"] = value;
				}
			}

			public virtual string LegacyManagedFolder
			{
				set
				{
					base.PowerSharpParameters["LegacyManagedFolder"] = ((value != null) ? new ELCFolderIdParameter(value) : null);
				}
			}

			public virtual SwitchParameter Force
			{
				set
				{
					base.PowerSharpParameters["Force"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual bool SystemTag
			{
				set
				{
					base.PowerSharpParameters["SystemTag"] = value;
				}
			}

			public virtual MultiValuedProperty<string> LocalizedRetentionPolicyTagName
			{
				set
				{
					base.PowerSharpParameters["LocalizedRetentionPolicyTagName"] = value;
				}
			}

			public virtual string Comment
			{
				set
				{
					base.PowerSharpParameters["Comment"] = value;
				}
			}

			public virtual Guid RetentionId
			{
				set
				{
					base.PowerSharpParameters["RetentionId"] = value;
				}
			}

			public virtual MultiValuedProperty<string> LocalizedComment
			{
				set
				{
					base.PowerSharpParameters["LocalizedComment"] = value;
				}
			}

			public virtual bool MustDisplayCommentEnabled
			{
				set
				{
					base.PowerSharpParameters["MustDisplayCommentEnabled"] = value;
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
