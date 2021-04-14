using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.PSDirectInvoke;
using Microsoft.Exchange.Management.RecipientTasks;
using Microsoft.Exchange.Services.Core.DataConverter;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf.Types;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;

namespace Microsoft.Exchange.Services.Wcf
{
	internal sealed class NewInboxRuleCommand : SingleCmdletCommandBase<NewInboxRuleRequest, NewInboxRuleResponse, NewInboxRule, Microsoft.Exchange.Management.Common.InboxRule>
	{
		public NewInboxRuleCommand(CallContext callContext, NewInboxRuleRequest request) : base(callContext, request, "New-InboxRule", ScopeLocation.RecipientWrite)
		{
		}

		protected override void PopulateTaskParameters()
		{
			NewInboxRule task = this.cmdletRunner.TaskWrapper.Task;
			NewInboxRuleData inboxRule = this.request.InboxRule;
			this.cmdletRunner.SetTaskParameter("AlwaysDeleteOutlookRulesBlob", task, new SwitchParameter(this.request.AlwaysDeleteOutlookRulesBlob));
			this.cmdletRunner.SetTaskParameter("Force", task, new SwitchParameter(this.request.Force));
			this.cmdletRunner.SetTaskParameterIfModified("CopyToFolder", inboxRule, task, inboxRule.CopyToFolder.ToMailboxFolderIdParameter());
			this.cmdletRunner.SetTaskParameterIfModified("ExceptIfFrom", inboxRule, task, inboxRule.ExceptIfFrom.ToRecipientIdParameters());
			IEnumerable<AggregationSubscriptionIdentity> enumerable = inboxRule.ExceptIfFromSubscription.ToIdParameters();
			this.cmdletRunner.SetTaskParameterIfModified("ExceptIfFromSubscription", inboxRule, task, (enumerable == null) ? null : enumerable.ToArray<AggregationSubscriptionIdentity>());
			IEnumerable<MessageClassificationIdParameter> enumerable2 = inboxRule.ExceptIfHasClassification.ToIdParameters<MessageClassificationIdParameter>();
			this.cmdletRunner.SetTaskParameterIfModified("ExceptIfHasClassification", inboxRule, task, (enumerable2 == null) ? null : enumerable2.ToArray<MessageClassificationIdParameter>());
			this.cmdletRunner.SetTaskParameterIfModified("ExceptIfMessageTypeMatches", inboxRule, task, (inboxRule.ExceptIfMessageTypeMatches == NullableInboxRuleMessageType.NullInboxRuleMessageType) ? null : new InboxRuleMessageType?((InboxRuleMessageType)inboxRule.ExceptIfMessageTypeMatches));
			this.cmdletRunner.SetTaskParameterIfModified("ExceptIfReceivedAfterDate", inboxRule, task, (inboxRule.ExceptIfReceivedAfterDate == null) ? null : new ExDateTime?(ExDateTimeConverter.Parse(inboxRule.ExceptIfReceivedAfterDate)));
			this.cmdletRunner.SetTaskParameterIfModified("ExceptIfReceivedBeforeDate", inboxRule, task, (inboxRule.ExceptIfReceivedBeforeDate == null) ? null : new ExDateTime?(ExDateTimeConverter.Parse(inboxRule.ExceptIfReceivedBeforeDate)));
			this.cmdletRunner.SetTaskParameterIfModified("ExceptIfSentTo", inboxRule, task, inboxRule.ExceptIfSentTo.ToRecipientIdParameters());
			this.cmdletRunner.SetTaskParameterIfModified("ExceptIfWithImportance", inboxRule, task, (inboxRule.ExceptIfWithImportance == NullableImportance.NullImportance) ? null : new Importance?((Importance)inboxRule.ExceptIfWithImportance));
			this.cmdletRunner.SetTaskParameterIfModified("ExceptIfWithinSizeRangeMaximum", inboxRule, task, (inboxRule.ExceptIfWithinSizeRangeMaximum == null) ? null : new ByteQuantifiedSize?(new ByteQuantifiedSize(inboxRule.ExceptIfWithinSizeRangeMaximum.Value)));
			this.cmdletRunner.SetTaskParameterIfModified("ExceptIfWithinSizeRangeMinimum", inboxRule, task, (inboxRule.ExceptIfWithinSizeRangeMinimum == null) ? null : new ByteQuantifiedSize?(new ByteQuantifiedSize(inboxRule.ExceptIfWithinSizeRangeMinimum.Value)));
			this.cmdletRunner.SetTaskParameterIfModified("ExceptIfWithSensitivity", inboxRule, task, (inboxRule.ExceptIfWithSensitivity == NullableSensitivity.NullSensitivity) ? null : new Sensitivity?((Sensitivity)inboxRule.ExceptIfWithSensitivity));
			this.cmdletRunner.SetTaskParameterIfModified("ForwardAsAttachmentTo", inboxRule, task, inboxRule.ForwardAsAttachmentTo.ToRecipientIdParameters());
			this.cmdletRunner.SetTaskParameterIfModified("ForwardTo", inboxRule, task, inboxRule.ForwardTo.ToRecipientIdParameters());
			this.cmdletRunner.SetTaskParameterIfModified("From", inboxRule, task, inboxRule.From.ToRecipientIdParameters());
			IEnumerable<AggregationSubscriptionIdentity> enumerable3 = inboxRule.FromSubscription.ToIdParameters();
			this.cmdletRunner.SetTaskParameterIfModified("FromSubscription", inboxRule, task, (enumerable3 == null) ? null : enumerable3.ToArray<AggregationSubscriptionIdentity>());
			IEnumerable<MessageClassificationIdParameter> enumerable4 = inboxRule.HasClassification.ToIdParameters<MessageClassificationIdParameter>();
			this.cmdletRunner.SetTaskParameterIfModified("HasClassification", inboxRule, task, (enumerable4 == null) ? null : enumerable4.ToArray<MessageClassificationIdParameter>());
			this.cmdletRunner.SetTaskParameterIfModified("MarkImportance", inboxRule, task, (inboxRule.MarkImportance == NullableImportance.NullImportance) ? null : new Importance?((Importance)inboxRule.MarkImportance));
			this.cmdletRunner.SetTaskParameterIfModified("MessageTypeMatches", inboxRule, task, (inboxRule.MessageTypeMatches == NullableInboxRuleMessageType.NullInboxRuleMessageType) ? null : new InboxRuleMessageType?((InboxRuleMessageType)inboxRule.MessageTypeMatches));
			this.cmdletRunner.SetTaskParameterIfModified("MoveToFolder", inboxRule, task, inboxRule.MoveToFolder.ToMailboxFolderIdParameter());
			this.cmdletRunner.SetTaskParameterIfModified("ReceivedAfterDate", inboxRule, task, (inboxRule.ReceivedAfterDate == null) ? null : new ExDateTime?(ExDateTimeConverter.Parse(inboxRule.ReceivedAfterDate)));
			this.cmdletRunner.SetTaskParameterIfModified("ReceivedBeforeDate", inboxRule, task, (inboxRule.ReceivedBeforeDate == null) ? null : new ExDateTime?(ExDateTimeConverter.Parse(inboxRule.ReceivedBeforeDate)));
			this.cmdletRunner.SetTaskParameterIfModified("RedirectTo", inboxRule, task, inboxRule.RedirectTo.ToRecipientIdParameters());
			this.cmdletRunner.SetTaskParameterIfModified("SendTextMessageNotificationTo", inboxRule, task, (inboxRule.SendTextMessageNotificationTo == null) ? null : new MultiValuedProperty<E164Number>(inboxRule.SendTextMessageNotificationTo));
			this.cmdletRunner.SetTaskParameterIfModified("SentTo", inboxRule, task, inboxRule.SentTo.ToRecipientIdParameters());
			this.cmdletRunner.SetTaskParameterIfModified("WithImportance", inboxRule, task, (inboxRule.WithImportance == NullableImportance.NullImportance) ? null : new Importance?((Importance)inboxRule.WithImportance));
			this.cmdletRunner.SetTaskParameterIfModified("WithinSizeRangeMaximum", inboxRule, task, (inboxRule.WithinSizeRangeMaximum == null) ? null : new ByteQuantifiedSize?(new ByteQuantifiedSize(inboxRule.WithinSizeRangeMaximum.Value)));
			this.cmdletRunner.SetTaskParameterIfModified("WithinSizeRangeMinimum", inboxRule, task, (inboxRule.WithinSizeRangeMinimum == null) ? null : new ByteQuantifiedSize?(new ByteQuantifiedSize(inboxRule.WithinSizeRangeMinimum.Value)));
			this.cmdletRunner.SetTaskParameterIfModified("WithSensitivity", inboxRule, task, (inboxRule.WithSensitivity == NullableSensitivity.NullSensitivity) ? null : new Sensitivity?((Sensitivity)inboxRule.WithSensitivity));
			this.cmdletRunner.SetRemainingModifiedTaskParameters(inboxRule, task);
		}

		protected override void PopulateResponseData(NewInboxRuleResponse response)
		{
			Microsoft.Exchange.Management.Common.InboxRule result = this.cmdletRunner.TaskWrapper.Result;
			response.InboxRule = new Microsoft.Exchange.Services.Wcf.Types.InboxRule
			{
				Identity = new Identity(result.Identity.ToString()),
				ApplyCategory = result.ApplyCategory.ToStringArray<string>(),
				BodyContainsWords = result.BodyContainsWords.ToStringArray<string>(),
				CopyToFolder = ((result.CopyToFolder == null) ? null : result.CopyToFolder.ToIdentity()),
				DeleteMessage = result.DeleteMessage,
				Description = ((result.Description == null) ? null : new RuleDescription(result.Description)),
				DescriptionTimeFormat = result.DescriptionTimeFormat,
				DescriptionTimeZone = ((result.DescriptionTimeZone == null || result.DescriptionTimeZone.ExTimeZone == null) ? new ExTimeZoneValue(ExTimeZone.UtcTimeZone) : result.DescriptionTimeZone).ToString(),
				Enabled = result.Enabled,
				ExceptIfBodyContainsWords = result.ExceptIfBodyContainsWords.ToStringArray<string>(),
				ExceptIfFlaggedForAction = result.ExceptIfFlaggedForAction,
				ExceptIfFrom = result.ExceptIfFrom.ToPeopleIdentityArray(),
				ExceptIfFromAddressContainsWords = result.ExceptIfFromAddressContainsWords.ToStringArray<string>(),
				ExceptIfFromSubscription = result.ExceptIfFromSubscription.ToIdentityArray(result),
				ExceptIfHasAttachment = result.ExceptIfHasAttachment,
				ExceptIfHasClassification = result.ExceptIfHasClassification.ToIdentityArray(),
				ExceptIfHeaderContainsWords = result.ExceptIfHeaderContainsWords.ToStringArray<string>(),
				ExceptIfMessageTypeMatches = (NullableInboxRuleMessageType)((result.ExceptIfMessageTypeMatches == null) ? ((InboxRuleMessageType)(-1)) : result.ExceptIfMessageTypeMatches.Value),
				ExceptIfMyNameInCcBox = result.ExceptIfMyNameInCcBox,
				ExceptIfMyNameInToBox = result.ExceptIfMyNameInToBox,
				ExceptIfMyNameInToOrCcBox = result.ExceptIfMyNameInToOrCcBox,
				ExceptIfMyNameNotInToBox = result.ExceptIfMyNameNotInToBox,
				ExceptIfReceivedAfterDate = ((result.ExceptIfReceivedAfterDate == null) ? null : ExDateTimeConverter.ToSoapHeaderTimeZoneRelatedXsdDateTime(result.ExceptIfReceivedAfterDate.Value)),
				ExceptIfReceivedBeforeDate = ((result.ExceptIfReceivedBeforeDate == null) ? null : ExDateTimeConverter.ToSoapHeaderTimeZoneRelatedXsdDateTime(result.ExceptIfReceivedBeforeDate.Value)),
				ExceptIfRecipientAddressContainsWords = result.ExceptIfRecipientAddressContainsWords.ToStringArray<string>(),
				ExceptIfSentOnlyToMe = result.ExceptIfSentOnlyToMe,
				ExceptIfSentTo = result.ExceptIfSentTo.ToPeopleIdentityArray(),
				ExceptIfSubjectContainsWords = result.ExceptIfSubjectContainsWords.ToStringArray<string>(),
				ExceptIfSubjectOrBodyContainsWords = result.ExceptIfSubjectOrBodyContainsWords.ToStringArray<string>(),
				ExceptIfWithImportance = (NullableImportance)((result.ExceptIfWithImportance == null) ? ((Importance)(-1)) : result.ExceptIfWithImportance.Value),
				ExceptIfWithinSizeRangeMaximum = ((result.ExceptIfWithinSizeRangeMaximum == null) ? null : new ulong?(result.ExceptIfWithinSizeRangeMaximum.Value.ToBytes())),
				ExceptIfWithinSizeRangeMinimum = ((result.ExceptIfWithinSizeRangeMinimum == null) ? null : new ulong?(result.ExceptIfWithinSizeRangeMinimum.Value.ToBytes())),
				ExceptIfWithSensitivity = (NullableSensitivity)((result.ExceptIfWithSensitivity == null) ? ((Sensitivity)(-1)) : result.ExceptIfWithSensitivity.Value),
				FlaggedForAction = result.FlaggedForAction,
				ForwardAsAttachmentTo = result.ForwardAsAttachmentTo.ToPeopleIdentityArray(),
				ForwardTo = result.ForwardTo.ToPeopleIdentityArray(),
				From = result.From.ToPeopleIdentityArray(),
				FromAddressContainsWords = result.FromAddressContainsWords.ToStringArray<string>(),
				FromSubscription = result.FromSubscription.ToIdentityArray(result),
				HasAttachment = result.HasAttachment,
				HasClassification = result.HasClassification.ToIdentityArray(),
				HeaderContainsWords = result.HeaderContainsWords.ToStringArray<string>(),
				InError = result.InError,
				MarkAsRead = result.MarkAsRead,
				MarkImportance = (NullableImportance)((result.MarkImportance == null) ? ((Importance)(-1)) : result.MarkImportance.Value),
				MessageTypeMatches = (NullableInboxRuleMessageType)((result.MessageTypeMatches == null) ? ((InboxRuleMessageType)(-1)) : result.MessageTypeMatches.Value),
				MyNameInCcBox = result.MyNameInCcBox,
				MyNameInToBox = result.MyNameInToBox,
				MyNameInToOrCcBox = result.MyNameInToOrCcBox,
				MyNameNotInToBox = result.MyNameNotInToBox,
				MoveToFolder = ((result.MoveToFolder == null) ? null : result.MoveToFolder.ToIdentity()),
				Name = result.Name,
				Priority = result.Priority,
				ReceivedAfterDate = ((result.ReceivedAfterDate == null) ? null : ExDateTimeConverter.ToSoapHeaderTimeZoneRelatedXsdDateTime(result.ReceivedAfterDate.Value)),
				ReceivedBeforeDate = ((result.ReceivedBeforeDate == null) ? null : ExDateTimeConverter.ToSoapHeaderTimeZoneRelatedXsdDateTime(result.ReceivedBeforeDate.Value)),
				RecipientAddressContainsWords = result.RecipientAddressContainsWords.ToStringArray<string>(),
				RedirectTo = result.RedirectTo.ToPeopleIdentityArray(),
				SendTextMessageNotificationTo = result.SendTextMessageNotificationTo.ToStringArray<E164Number>(),
				SentOnlyToMe = result.SentOnlyToMe,
				SentTo = result.SentTo.ToPeopleIdentityArray(),
				StopProcessingRules = result.StopProcessingRules,
				SubjectContainsWords = result.SubjectContainsWords.ToStringArray<string>(),
				SubjectOrBodyContainsWords = result.SubjectOrBodyContainsWords.ToStringArray<string>(),
				SupportedByTask = result.SupportedByTask,
				WithImportance = (NullableImportance)((result.WithImportance == null) ? ((Importance)(-1)) : result.WithImportance.Value),
				WithinSizeRangeMinimum = ((result.WithinSizeRangeMinimum == null) ? null : new ulong?(result.WithinSizeRangeMinimum.Value.ToBytes())),
				WithinSizeRangeMaximum = ((result.WithinSizeRangeMaximum == null) ? null : new ulong?(result.WithinSizeRangeMaximum.Value.ToBytes())),
				WithSensitivity = (NullableSensitivity)((result.WithSensitivity == null) ? ((Sensitivity)(-1)) : result.WithSensitivity.Value)
			};
		}

		protected override PSLocalTask<NewInboxRule, Microsoft.Exchange.Management.Common.InboxRule> InvokeCmdletFactory()
		{
			return CmdletTaskFactory.Instance.CreateNewInboxRuleTask(base.CallContext.AccessingPrincipal);
		}
	}
}
