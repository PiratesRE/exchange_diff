using System;
using System.Globalization;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Management.RecipientTasks;
using Microsoft.Exchange.SoapWebClient.EWS;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class SetGroupMailboxCommand : SyntheticCommandWithPipelineInputNoOutput<GroupMailbox>
	{
		private SetGroupMailboxCommand() : base("Set-GroupMailbox")
		{
		}

		public SetGroupMailboxCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual SetGroupMailboxCommand SetParameters(SetGroupMailboxCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SetGroupMailboxCommand SetParameters(SetGroupMailboxCommand.IdentityParameters parameters)
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

			public virtual string DisplayName
			{
				set
				{
					base.PowerSharpParameters["DisplayName"] = value;
				}
			}

			public virtual string Description
			{
				set
				{
					base.PowerSharpParameters["Description"] = value;
				}
			}

			public virtual string ExecutingUser
			{
				set
				{
					base.PowerSharpParameters["ExecutingUser"] = ((value != null) ? new RecipientIdParameter(value) : null);
				}
			}

			public virtual RecipientIdParameter Owners
			{
				set
				{
					base.PowerSharpParameters["Owners"] = value;
				}
			}

			public virtual RecipientIdParameter AddOwners
			{
				set
				{
					base.PowerSharpParameters["AddOwners"] = value;
				}
			}

			public virtual RecipientIdParameter RemoveOwners
			{
				set
				{
					base.PowerSharpParameters["RemoveOwners"] = value;
				}
			}

			public virtual RecipientIdParameter AddedMembers
			{
				set
				{
					base.PowerSharpParameters["AddedMembers"] = value;
				}
			}

			public virtual RecipientIdParameter RemovedMembers
			{
				set
				{
					base.PowerSharpParameters["RemovedMembers"] = value;
				}
			}

			public virtual Uri SharePointUrl
			{
				set
				{
					base.PowerSharpParameters["SharePointUrl"] = value;
				}
			}

			public virtual MultiValuedProperty<string> SharePointResources
			{
				set
				{
					base.PowerSharpParameters["SharePointResources"] = value;
				}
			}

			public virtual ModernGroupTypeInfo SwitchToGroupType
			{
				set
				{
					base.PowerSharpParameters["SwitchToGroupType"] = value;
				}
			}

			public virtual bool RequireSenderAuthenticationEnabled
			{
				set
				{
					base.PowerSharpParameters["RequireSenderAuthenticationEnabled"] = value;
				}
			}

			public virtual string YammerGroupEmailAddress
			{
				set
				{
					base.PowerSharpParameters["YammerGroupEmailAddress"] = value;
				}
			}

			public virtual RecipientIdType RecipientIdType
			{
				set
				{
					base.PowerSharpParameters["RecipientIdType"] = value;
				}
			}

			public virtual SwitchParameter FromSyncClient
			{
				set
				{
					base.PowerSharpParameters["FromSyncClient"] = value;
				}
			}

			public virtual SmtpAddress PrimarySmtpAddress
			{
				set
				{
					base.PowerSharpParameters["PrimarySmtpAddress"] = value;
				}
			}

			public virtual ProxyAddressCollection EmailAddresses
			{
				set
				{
					base.PowerSharpParameters["EmailAddresses"] = value;
				}
			}

			public virtual string ExternalDirectoryObjectId
			{
				set
				{
					base.PowerSharpParameters["ExternalDirectoryObjectId"] = value;
				}
			}

			public virtual SwitchParameter ForcePublishExternalResources
			{
				set
				{
					base.PowerSharpParameters["ForcePublishExternalResources"] = value;
				}
			}

			public virtual MultiValuedProperty<GroupMailboxConfigurationActionType> ConfigurationActions
			{
				set
				{
					base.PowerSharpParameters["ConfigurationActions"] = value;
				}
			}

			public virtual CultureInfo Language
			{
				set
				{
					base.PowerSharpParameters["Language"] = value;
				}
			}

			public virtual SwitchParameter AutoSubscribeNewGroupMembers
			{
				set
				{
					base.PowerSharpParameters["AutoSubscribeNewGroupMembers"] = value;
				}
			}

			public virtual int PermissionsVersion
			{
				set
				{
					base.PowerSharpParameters["PermissionsVersion"] = value;
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

		public class IdentityParameters : ParametersBase
		{
			public virtual string Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = ((value != null) ? new RecipientIdParameter(value) : null);
				}
			}

			public virtual string Name
			{
				set
				{
					base.PowerSharpParameters["Name"] = value;
				}
			}

			public virtual string DisplayName
			{
				set
				{
					base.PowerSharpParameters["DisplayName"] = value;
				}
			}

			public virtual string Description
			{
				set
				{
					base.PowerSharpParameters["Description"] = value;
				}
			}

			public virtual string ExecutingUser
			{
				set
				{
					base.PowerSharpParameters["ExecutingUser"] = ((value != null) ? new RecipientIdParameter(value) : null);
				}
			}

			public virtual RecipientIdParameter Owners
			{
				set
				{
					base.PowerSharpParameters["Owners"] = value;
				}
			}

			public virtual RecipientIdParameter AddOwners
			{
				set
				{
					base.PowerSharpParameters["AddOwners"] = value;
				}
			}

			public virtual RecipientIdParameter RemoveOwners
			{
				set
				{
					base.PowerSharpParameters["RemoveOwners"] = value;
				}
			}

			public virtual RecipientIdParameter AddedMembers
			{
				set
				{
					base.PowerSharpParameters["AddedMembers"] = value;
				}
			}

			public virtual RecipientIdParameter RemovedMembers
			{
				set
				{
					base.PowerSharpParameters["RemovedMembers"] = value;
				}
			}

			public virtual Uri SharePointUrl
			{
				set
				{
					base.PowerSharpParameters["SharePointUrl"] = value;
				}
			}

			public virtual MultiValuedProperty<string> SharePointResources
			{
				set
				{
					base.PowerSharpParameters["SharePointResources"] = value;
				}
			}

			public virtual ModernGroupTypeInfo SwitchToGroupType
			{
				set
				{
					base.PowerSharpParameters["SwitchToGroupType"] = value;
				}
			}

			public virtual bool RequireSenderAuthenticationEnabled
			{
				set
				{
					base.PowerSharpParameters["RequireSenderAuthenticationEnabled"] = value;
				}
			}

			public virtual string YammerGroupEmailAddress
			{
				set
				{
					base.PowerSharpParameters["YammerGroupEmailAddress"] = value;
				}
			}

			public virtual RecipientIdType RecipientIdType
			{
				set
				{
					base.PowerSharpParameters["RecipientIdType"] = value;
				}
			}

			public virtual SwitchParameter FromSyncClient
			{
				set
				{
					base.PowerSharpParameters["FromSyncClient"] = value;
				}
			}

			public virtual SmtpAddress PrimarySmtpAddress
			{
				set
				{
					base.PowerSharpParameters["PrimarySmtpAddress"] = value;
				}
			}

			public virtual ProxyAddressCollection EmailAddresses
			{
				set
				{
					base.PowerSharpParameters["EmailAddresses"] = value;
				}
			}

			public virtual string ExternalDirectoryObjectId
			{
				set
				{
					base.PowerSharpParameters["ExternalDirectoryObjectId"] = value;
				}
			}

			public virtual SwitchParameter ForcePublishExternalResources
			{
				set
				{
					base.PowerSharpParameters["ForcePublishExternalResources"] = value;
				}
			}

			public virtual MultiValuedProperty<GroupMailboxConfigurationActionType> ConfigurationActions
			{
				set
				{
					base.PowerSharpParameters["ConfigurationActions"] = value;
				}
			}

			public virtual CultureInfo Language
			{
				set
				{
					base.PowerSharpParameters["Language"] = value;
				}
			}

			public virtual SwitchParameter AutoSubscribeNewGroupMembers
			{
				set
				{
					base.PowerSharpParameters["AutoSubscribeNewGroupMembers"] = value;
				}
			}

			public virtual int PermissionsVersion
			{
				set
				{
					base.PowerSharpParameters["PermissionsVersion"] = value;
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
