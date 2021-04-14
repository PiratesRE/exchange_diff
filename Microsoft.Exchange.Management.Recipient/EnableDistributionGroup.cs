using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("enable", "DistributionGroup", SupportsShouldProcess = true)]
	public sealed class EnableDistributionGroup : EnableRecipientObjectTask<GroupIdParameter, ADGroup>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageEnableDistributionGroup(this.Identity.ToString());
			}
		}

		protected override bool DelayProvisioning
		{
			get
			{
				return true;
			}
		}

		protected override void PrepareRecipientObject(ref ADGroup group)
		{
			TaskLogger.LogEnter();
			base.PrepareRecipientObject(ref group);
			if (group != null && (group.RecipientTypeDetails == RecipientTypeDetails.GroupMailbox || group.RecipientTypeDetails == RecipientTypeDetails.RemoteGroupMailbox))
			{
				base.WriteError(new RecipientTaskException(Strings.NotAValidDistributionGroup), ExchangeErrorCategory.Client, this.Identity.ToString());
			}
			if (RecipientType.MailUniversalDistributionGroup == group.RecipientType || RecipientType.MailUniversalSecurityGroup == group.RecipientType || RecipientType.MailNonUniversalGroup == group.RecipientType)
			{
				base.WriteError(new InvalidOperationException(Strings.ErrorGroupAlreadyMailEnabled(this.Identity.ToString())), ErrorCategory.InvalidOperation, group.Identity);
			}
			if ((group.GroupType & GroupTypeFlags.Universal) == GroupTypeFlags.None)
			{
				base.WriteError(new InvalidOperationException(Strings.ErrorEnableNonUniversalGroup), ErrorCategory.InvalidOperation, group.Identity);
			}
			if (group.RecipientTypeDetails == RecipientTypeDetails.RoleGroup)
			{
				base.WriteError(new CannotEnableRoleGroupException(), ErrorCategory.InvalidOperation, group.Identity);
			}
			group.SetExchangeVersion(group.MaximumSupportedExchangeObjectVersion);
			List<PropertyDefinition> list = new List<PropertyDefinition>(DisableDistributionGroup.PropertiesToReset);
			MailboxTaskHelper.RemovePersistentProperties(list);
			MailboxTaskHelper.ClearExchangeProperties(group, list);
			group.SetExchangeVersion(group.MaximumSupportedExchangeObjectVersion);
			if (this.DelayProvisioning && base.IsProvisioningLayerAvailable)
			{
				this.ProvisionDefaultValues(new ADGroup(), group);
			}
			if (VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).CmdletInfra.ReportToOriginator.Enabled)
			{
				group.ReportToOriginatorEnabled = true;
			}
			else
			{
				group.ReportToOriginatorEnabled = false;
			}
			group.RequireAllSendersAreAuthenticated = true;
			if ((group.GroupType & GroupTypeFlags.SecurityEnabled) == GroupTypeFlags.SecurityEnabled)
			{
				group.RecipientDisplayType = new RecipientDisplayType?(RecipientDisplayType.SecurityDistributionGroup);
			}
			else
			{
				group.RecipientDisplayType = new RecipientDisplayType?(RecipientDisplayType.DistributionGroup);
			}
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter(new object[]
			{
				this.DataObject
			});
			if (!VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).CmdletInfra.EmailAddressPolicy.Enabled)
			{
				this.DataObject.EmailAddressPolicyEnabled = false;
			}
			base.InternalProcessRecord();
			this.WriteResult();
			TaskLogger.LogExit();
		}

		private void WriteResult()
		{
			TaskLogger.LogEnter(new object[]
			{
				this.DataObject.Id
			});
			DistributionGroup sendToPipeline = new DistributionGroup(this.DataObject);
			base.WriteObject(sendToPipeline);
			TaskLogger.LogExit();
		}

		protected override IConfigurable ConvertDataObjectToPresentationObject(IConfigurable dataObject)
		{
			return DistributionGroup.FromDataObject((ADGroup)dataObject);
		}

		protected override void PrepareRecipientAlias(ADGroup dataObject)
		{
			if (!string.IsNullOrEmpty(base.Alias))
			{
				dataObject.Alias = base.Alias;
				return;
			}
			dataObject.Alias = RecipientTaskHelper.GenerateUniqueAlias(base.TenantGlobalCatalogSession, dataObject.OrganizationId, dataObject.SamAccountName, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose));
		}
	}
}
