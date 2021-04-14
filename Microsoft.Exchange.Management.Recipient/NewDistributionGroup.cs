using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("New", "DistributionGroup", SupportsShouldProcess = true)]
	public sealed class NewDistributionGroup : NewDistributionGroupBase
	{
		protected override void InternalBeginProcessing()
		{
			TaskLogger.LogEnter();
			base.InternalBeginProcessing();
			if (base.ManagedBy != null && base.ManagedBy.IsChangesOnlyCopy)
			{
				MultiValuedProperty<GeneralRecipientIdParameter> managedBy = base.ManagedBy;
				base.ManagedBy = new MultiValuedProperty<GeneralRecipientIdParameter>();
				base.ManagedBy.CopyChangesFrom(managedBy);
			}
			ADObjectId adobjectId = null;
			base.TryGetExecutingUserId(out adobjectId);
			if (!base.Fields.IsModified(ADGroupSchema.ManagedBy) && adobjectId != null)
			{
				if (!base.CurrentOrganizationId.Equals(base.ExecutingUserOrganizationId))
				{
					base.WriteError(new RecipientTaskException(Strings.ErrorManagedByMustBeSpecifiedWIthOrganization), ExchangeErrorCategory.Client, null);
				}
				else
				{
					base.ManagedBy = new MultiValuedProperty<GeneralRecipientIdParameter>(adobjectId);
				}
				MailboxTaskHelper.CheckAndResolveManagedBy<ADGroup>(this, new DataAccessHelper.CategorizedGetDataObjectDelegate(base.GetDataObject<ADRecipient>), ExchangeErrorCategory.ServerOperation, (base.ManagedBy == null) ? null : base.ManagedBy.ToArray(), out this.managedByRecipients);
			}
			else
			{
				MailboxTaskHelper.CheckAndResolveManagedBy<ADGroup>(this, new DataAccessHelper.CategorizedGetDataObjectDelegate(base.GetDataObject<ADRecipient>), ExchangeErrorCategory.Client, (base.ManagedBy == null) ? null : base.ManagedBy.ToArray(), out this.managedByRecipients);
			}
			TaskLogger.LogExit();
		}

		protected override void WriteResult(ADObject result)
		{
			TaskLogger.LogEnter(new object[]
			{
				this.DataObject.Identity
			});
			DistributionGroup result2 = new DistributionGroup((ADGroup)result);
			base.WriteResult(result2);
			TaskLogger.LogExit();
		}

		protected override string ClonableTypeName
		{
			get
			{
				return typeof(DistributionGroup).FullName;
			}
		}

		protected override IConfigurable ConvertDataObjectToPresentationObject(IConfigurable dataObject)
		{
			return DistributionGroup.FromDataObject((ADGroup)dataObject);
		}

		protected override void PrepareRecipientObject(ADGroup group)
		{
			base.PrepareRecipientObject(group);
			if (!VariantConfiguration.InvariantNoFlightingSnapshot.CmdletInfra.ReportToOriginator.Enabled)
			{
				group.ReportToOriginatorEnabled = false;
			}
		}
	}
}
