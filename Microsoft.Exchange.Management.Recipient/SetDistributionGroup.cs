using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Set", "DistributionGroup", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetDistributionGroup : SetDistributionGroupBase<DistributionGroup>
	{
		[Parameter(Mandatory = false)]
		public SwitchParameter ForceUpgrade
		{
			get
			{
				return (SwitchParameter)(base.Fields["ForceUpgrade"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["ForceUpgrade"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter GenerateExternalDirectoryObjectId
		{
			get
			{
				return (SwitchParameter)(base.Fields["GenerateExternalDirectoryObjectId"] ?? false);
			}
			set
			{
				base.Fields["GenerateExternalDirectoryObjectId"] = value;
			}
		}

		protected override IConfigurable PrepareDataObject()
		{
			ADGroup adgroup = (ADGroup)base.PrepareDataObject();
			if (adgroup != null && (adgroup.RecipientTypeDetails == RecipientTypeDetails.GroupMailbox || adgroup.RecipientTypeDetails == RecipientTypeDetails.RemoteGroupMailbox))
			{
				base.WriteError(new RecipientTaskException(Strings.NotAValidDistributionGroup), ExchangeErrorCategory.Client, this.Identity.ToString());
			}
			return adgroup;
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			if (this.GenerateExternalDirectoryObjectId && (RecipientTaskHelper.GetAcceptedRecipientTypes() & this.DataObject.RecipientTypeDetails) == RecipientTypeDetails.None)
			{
				base.WriteError(new RecipientTaskException(Strings.ErrorCannotGenerateExternalDirectoryObjectIdOnInternalRecipientType(this.Identity.ToString(), this.DataObject.RecipientTypeDetails.ToString())), ExchangeErrorCategory.Client, this.Identity);
			}
			if (base.Fields.IsModified(DistributionGroupSchema.ManagedBy) && (this.DataObject.ManagedBy == null || this.DataObject.ManagedBy.Count == 0))
			{
				base.WriteError(new RecipientTaskException(Strings.AutoGroupManagedByCannotBeEmpty), ExchangeErrorCategory.Client, null);
			}
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			if (!base.IsUpgrading || this.ForceUpgrade || base.ShouldContinue(Strings.ContinueUpgradeObjectVersion(this.DataObject.Name)))
			{
				if (this.GenerateExternalDirectoryObjectId && string.IsNullOrEmpty(this.DataObject.ExternalDirectoryObjectId))
				{
					this.DataObject.ExternalDirectoryObjectId = Guid.NewGuid().ToString();
				}
				base.InternalProcessRecord();
			}
		}

		protected override IConfigurable ConvertDataObjectToPresentationObject(IConfigurable dataObject)
		{
			return DistributionGroup.FromDataObject((ADGroup)dataObject);
		}
	}
}
