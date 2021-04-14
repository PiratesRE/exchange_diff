using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class NewRetentionPolicyTagCommand : SyntheticCommandWithPipelineInput<RetentionPolicyTag, RetentionPolicyTag>
	{
		private NewRetentionPolicyTagCommand() : base("New-RetentionPolicyTag")
		{
		}

		public NewRetentionPolicyTagCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual NewRetentionPolicyTagCommand SetParameters(NewRetentionPolicyTagCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual NewRetentionPolicyTagCommand SetParameters(NewRetentionPolicyTagCommand.RetentionPolicyParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual NewRetentionPolicyTagCommand SetParameters(NewRetentionPolicyTagCommand.UpgradeManagedFolderParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual MultiValuedProperty<string> LocalizedRetentionPolicyTagName
			{
				set
				{
					base.PowerSharpParameters["LocalizedRetentionPolicyTagName"] = value;
				}
			}

			public virtual SwitchParameter IgnoreDehydratedFlag
			{
				set
				{
					base.PowerSharpParameters["IgnoreDehydratedFlag"] = value;
				}
			}

			public virtual SwitchParameter IsDefaultAutoGroupPolicyTag
			{
				set
				{
					base.PowerSharpParameters["IsDefaultAutoGroupPolicyTag"] = value;
				}
			}

			public virtual SwitchParameter IsDefaultModeratedRecipientsPolicyTag
			{
				set
				{
					base.PowerSharpParameters["IsDefaultModeratedRecipientsPolicyTag"] = value;
				}
			}

			public virtual string Comment
			{
				set
				{
					base.PowerSharpParameters["Comment"] = value;
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

			public virtual ElcFolderType Type
			{
				set
				{
					base.PowerSharpParameters["Type"] = value;
				}
			}

			public virtual bool SystemTag
			{
				set
				{
					base.PowerSharpParameters["SystemTag"] = value;
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

		public class RetentionPolicyParameters : ParametersBase
		{
			public virtual Guid RetentionId
			{
				set
				{
					base.PowerSharpParameters["RetentionId"] = value;
				}
			}

			public virtual string MessageClass
			{
				set
				{
					base.PowerSharpParameters["MessageClass"] = value;
				}
			}

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

			public virtual MultiValuedProperty<string> LocalizedRetentionPolicyTagName
			{
				set
				{
					base.PowerSharpParameters["LocalizedRetentionPolicyTagName"] = value;
				}
			}

			public virtual SwitchParameter IgnoreDehydratedFlag
			{
				set
				{
					base.PowerSharpParameters["IgnoreDehydratedFlag"] = value;
				}
			}

			public virtual SwitchParameter IsDefaultAutoGroupPolicyTag
			{
				set
				{
					base.PowerSharpParameters["IsDefaultAutoGroupPolicyTag"] = value;
				}
			}

			public virtual SwitchParameter IsDefaultModeratedRecipientsPolicyTag
			{
				set
				{
					base.PowerSharpParameters["IsDefaultModeratedRecipientsPolicyTag"] = value;
				}
			}

			public virtual string Comment
			{
				set
				{
					base.PowerSharpParameters["Comment"] = value;
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

			public virtual ElcFolderType Type
			{
				set
				{
					base.PowerSharpParameters["Type"] = value;
				}
			}

			public virtual bool SystemTag
			{
				set
				{
					base.PowerSharpParameters["SystemTag"] = value;
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

		public class UpgradeManagedFolderParameters : ParametersBase
		{
			public virtual string ManagedFolderToUpgrade
			{
				set
				{
					base.PowerSharpParameters["ManagedFolderToUpgrade"] = ((value != null) ? new ELCFolderIdParameter(value) : null);
				}
			}

			public virtual MultiValuedProperty<string> LocalizedRetentionPolicyTagName
			{
				set
				{
					base.PowerSharpParameters["LocalizedRetentionPolicyTagName"] = value;
				}
			}

			public virtual SwitchParameter IgnoreDehydratedFlag
			{
				set
				{
					base.PowerSharpParameters["IgnoreDehydratedFlag"] = value;
				}
			}

			public virtual SwitchParameter IsDefaultAutoGroupPolicyTag
			{
				set
				{
					base.PowerSharpParameters["IsDefaultAutoGroupPolicyTag"] = value;
				}
			}

			public virtual SwitchParameter IsDefaultModeratedRecipientsPolicyTag
			{
				set
				{
					base.PowerSharpParameters["IsDefaultModeratedRecipientsPolicyTag"] = value;
				}
			}

			public virtual string Comment
			{
				set
				{
					base.PowerSharpParameters["Comment"] = value;
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

			public virtual ElcFolderType Type
			{
				set
				{
					base.PowerSharpParameters["Type"] = value;
				}
			}

			public virtual bool SystemTag
			{
				set
				{
					base.PowerSharpParameters["SystemTag"] = value;
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
