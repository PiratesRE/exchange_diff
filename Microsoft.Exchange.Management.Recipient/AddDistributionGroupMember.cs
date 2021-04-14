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
	[Cmdlet("Add", "DistributionGroupMember", SupportsShouldProcess = true)]
	public sealed class AddDistributionGroupMember : DistributionGroupMemberTaskBase<RecipientWithAdUserGroupIdParameter<RecipientIdParameter>>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageAddDistributionGroupMember(this.Identity.ToString(), base.Member.ToString());
			}
		}

		protected override string ApprovalAction
		{
			get
			{
				return "Add-DistributionGroupMember";
			}
		}

		protected override void PerformGroupMemberAction()
		{
			try
			{
				ADRecipient memberRecipient = (ADRecipient)base.GetDataObject<ADRecipient>(base.Member, base.GlobalCatalogRBACSession, this.DataObject.OrganizationId.OrganizationalUnit, null, new LocalizedString?(Strings.ErrorRecipientNotFound((string)base.Member)), new LocalizedString?(Strings.ErrorRecipientNotUnique((string)base.Member)), ExchangeErrorCategory.Client);
				MailboxTaskHelper.ValidateAndAddMember(this.DataObject, base.Member, memberRecipient, new Task.ErrorLoggerDelegate(base.WriteError));
			}
			catch (ManagementObjectNotFoundException)
			{
				ADRecipient adrecipient = (ADRecipient)base.GetDataObject<ADRecipient>(base.Member, base.PartitionOrRootOrgGlobalCatalogSession, this.DataObject.OrganizationId.OrganizationalUnit, null, new LocalizedString?(Strings.ErrorRecipientNotFound((string)base.Member)), new LocalizedString?(Strings.ErrorRecipientNotUnique((string)base.Member)), ExchangeErrorCategory.Client);
				ADObjectId x;
				if (adrecipient == null || !base.TryGetExecutingUserId(out x) || !ADObjectId.Equals(x, adrecipient.Id) || !adrecipient.HiddenFromAddressListsEnabled)
				{
					throw;
				}
				MailboxTaskHelper.ValidateAndAddMember(this.DataObject, base.Member, adrecipient, new Task.ErrorLoggerDelegate(base.WriteError));
			}
		}

		protected override void InternalProcessRecord()
		{
			try
			{
				base.InternalProcessRecord();
			}
			catch (ADObjectEntryAlreadyExistsException)
			{
				MailboxTaskHelper.WriteMemberAlreadyExistsError(this.DataObject, base.Member, base.IsSelfMemberAction, new Task.ErrorLoggerDelegate(base.WriteError));
			}
		}

		protected override void GroupMemberCheck(ADRecipient requester)
		{
			if (this.DataObject.Members.Contains(requester.Id))
			{
				base.WriteError(new SelfMemberAlreadyExistsException(this.Identity.ToString()), ErrorCategory.InvalidData, new RecipientWithAdUserGroupIdParameter<RecipientIdParameter>(requester.Id));
			}
		}

		protected override MemberUpdateType MemberUpdateRestriction
		{
			get
			{
				return this.DataObject.MemberJoinRestriction;
			}
		}

		protected override RecipientWithAdUserGroupIdParameter<RecipientIdParameter> IdentityFactory(ADObjectId id)
		{
			return new RecipientWithAdUserGroupIdParameter<RecipientIdParameter>(id);
		}

		protected override void WriteApprovalRequiredWarning(string messageId)
		{
			this.WriteWarning(Strings.WarningDistributionListJoinApprovalRequired(this.requester.DisplayName, this.DataObject.DisplayName, messageId));
		}

		protected override void WriteClosedUpdateError()
		{
			base.WriteError(new RecipientTaskException(Strings.ErrorDistributionListJoinClosed(this.DataObject.DisplayName)), ExchangeErrorCategory.Client, this.Identity);
		}

		protected override LocalizedString ApprovalMessageSubject()
		{
			return Strings.AutoGroupJoinMessageSubject;
		}
	}
}
