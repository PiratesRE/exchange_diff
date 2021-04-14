using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Remove", "DistributionGroupMember", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class RemoveDistributionGroupMember : DistributionGroupMemberTaskBase<GeneralRecipientIdParameter>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageRemoveDistributionGroupMember(this.Identity.ToString(), base.Member.ToString());
			}
		}

		protected override string ApprovalAction
		{
			get
			{
				return "Remove-DistributionGroupMember";
			}
		}

		protected override void PerformGroupMemberAction()
		{
			try
			{
				ADRecipient memberRecipient = (ADRecipient)base.GetDataObject<ADRecipient>(base.Member, base.TenantGlobalCatalogSession, this.DataObject.OrganizationId.OrganizationalUnit, null, new LocalizedString?(Strings.ErrorRecipientNotFound((string)base.Member)), new LocalizedString?(Strings.ErrorRecipientNotUnique((string)base.Member)));
				MailboxTaskHelper.ValidateAndRemoveMember(this.DataObject, memberRecipient);
			}
			catch (ManagementObjectNotFoundException)
			{
				ADRecipient adrecipient = (ADRecipient)base.GetDataObject<ADRecipient>(base.Member, base.PartitionOrRootOrgGlobalCatalogSession, this.DataObject.OrganizationId.OrganizationalUnit, null, new LocalizedString?(Strings.ErrorRecipientNotFound((string)base.Member)), new LocalizedString?(Strings.ErrorRecipientNotUnique((string)base.Member)), ExchangeErrorCategory.Client);
				ADObjectId x;
				if (adrecipient == null || !base.TryGetExecutingUserId(out x) || !ADObjectId.Equals(x, adrecipient.Id) || !adrecipient.HiddenFromAddressListsEnabled)
				{
					throw;
				}
				MailboxTaskHelper.ValidateAndRemoveMember(this.DataObject, adrecipient);
			}
		}

		protected override void InternalProcessRecord()
		{
			try
			{
				base.InternalProcessRecord();
			}
			catch (ADNotAMemberException)
			{
				MailboxTaskHelper.WriteMemberNotFoundError(this.DataObject, base.Member, this.Identity.RawIdentity, base.IsSelfMemberAction, new Task.TaskErrorLoggingDelegate(base.WriteError));
			}
		}

		protected override void GroupMemberCheck(ADRecipient requester)
		{
			if (!this.DataObject.Members.Contains(requester.Id))
			{
				base.WriteError(new SelfMemberNotFoundException(this.Identity.ToString()), ErrorCategory.InvalidData, new RecipientIdParameter(requester.Id));
			}
		}

		protected override MemberUpdateType MemberUpdateRestriction
		{
			get
			{
				return this.DataObject.MemberDepartRestriction;
			}
		}

		protected override GeneralRecipientIdParameter IdentityFactory(ADObjectId id)
		{
			return new GeneralRecipientIdParameter(id);
		}

		protected override void WriteApprovalRequiredWarning(string messageId)
		{
			this.WriteWarning(Strings.WarningDistributionListDepartApprovalRequired(this.requester.DisplayName, this.DataObject.DisplayName, messageId));
		}

		protected override void WriteClosedUpdateError()
		{
			base.WriteError(new RecipientTaskException(Strings.ErrorDistributionListDepartClosed(this.DataObject.DisplayName)), ErrorCategory.PermissionDenied, this.Identity);
		}

		protected override LocalizedString ApprovalMessageSubject()
		{
			return Strings.AutoGroupDepartMessageSubject(this.requester.DisplayName, this.DataObject.DisplayName);
		}
	}
}
