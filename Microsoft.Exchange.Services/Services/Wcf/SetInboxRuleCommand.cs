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
	internal sealed class SetInboxRuleCommand : SingleCmdletCommandBase<SetInboxRuleRequest, SetInboxRuleResponse, SetInboxRule, Microsoft.Exchange.Management.Common.InboxRule>
	{
		public SetInboxRuleCommand(CallContext callContext, SetInboxRuleRequest request) : base(callContext, request, "Set-InboxRule", ScopeLocation.RecipientWrite)
		{
		}

		protected override void PopulateTaskParameters()
		{
			SetInboxRule task = this.cmdletRunner.TaskWrapper.Task;
			SetInboxRuleData inboxRule = this.request.InboxRule;
			this.cmdletRunner.SetTaskParameter("AlwaysDeleteOutlookRulesBlob", task, new SwitchParameter(this.request.AlwaysDeleteOutlookRulesBlob));
			this.cmdletRunner.SetTaskParameter("Force", task, new SwitchParameter(this.request.Force));
			this.cmdletRunner.SetTaskParameter("Identity", task, new InboxRuleIdParameter(inboxRule.Identity));
			this.cmdletRunner.SetTaskParameterIfModified("CopyToFolder", inboxRule, task, inboxRule.CopyToFolder.ToMailboxFolderIdParameter());
			this.cmdletRunner.SetTaskParameterIfModified("ExceptIfFrom", inboxRule, task, inboxRule.ExceptIfFrom.ToRecipientIdParameters());
			IEnumerable<AggregationSubscriptionIdentity> enumerable = inboxRule.ExceptIfFromSubscription.ToIdParameters();
			this.cmdletRunner.SetTaskParameterIfModified("ExceptIfFromSubscription", inboxRule, task, (enumerable == null) ? null : enumerable.ToArray<AggregationSubscriptionIdentity>());
			IEnumerable<MessageClassificationIdParameter> enumerable2 = inboxRule.ExceptIfHasClassification.ToIdParameters<MessageClassificationIdParameter>();
			this.cmdletRunner.SetTaskParameterIfModified("ExceptIfHasClassification", inboxRule, task, (enumerable2 == null) ? null : enumerable2.ToArray<MessageClassificationIdParameter>());
			this.cmdletRunner.SetTaskParameterIfModified("ExceptIfSentTo", inboxRule, task, inboxRule.ExceptIfSentTo.ToRecipientIdParameters());
			this.cmdletRunner.SetTaskParameterIfModified("ForwardAsAttachmentTo", inboxRule, task, inboxRule.ForwardAsAttachmentTo.ToRecipientIdParameters());
			this.cmdletRunner.SetTaskParameterIfModified("ForwardTo", inboxRule, task, inboxRule.ForwardTo.ToRecipientIdParameters());
			this.cmdletRunner.SetTaskParameterIfModified("From", inboxRule, task, inboxRule.From.ToRecipientIdParameters());
			IEnumerable<AggregationSubscriptionIdentity> enumerable3 = inboxRule.FromSubscription.ToIdParameters();
			this.cmdletRunner.SetTaskParameterIfModified("FromSubscription", inboxRule, task, (enumerable3 == null) ? null : enumerable3.ToArray<AggregationSubscriptionIdentity>());
			IEnumerable<MessageClassificationIdParameter> enumerable4 = inboxRule.HasClassification.ToIdParameters<MessageClassificationIdParameter>();
			this.cmdletRunner.SetTaskParameterIfModified("HasClassification", inboxRule, task, (enumerable4 == null) ? null : enumerable4.ToArray<MessageClassificationIdParameter>());
			this.cmdletRunner.SetTaskParameterIfModified("MoveToFolder", inboxRule, task, inboxRule.MoveToFolder.ToMailboxFolderIdParameter());
			this.cmdletRunner.SetTaskParameterIfModified("RedirectTo", inboxRule, task, inboxRule.RedirectTo.ToRecipientIdParameters());
			this.cmdletRunner.SetTaskParameterIfModified("SentTo", inboxRule, task, inboxRule.SentTo.ToRecipientIdParameters());
			object dynamicParameters = task.GetDynamicParameters();
			this.cmdletRunner.SetTaskParameterIfModified("ExceptIfReceivedAfterDate", inboxRule, dynamicParameters, (inboxRule.ExceptIfReceivedAfterDate == null) ? null : new ExDateTime?(ExDateTimeConverter.Parse(inboxRule.ExceptIfReceivedAfterDate)));
			this.cmdletRunner.SetTaskParameterIfModified("ExceptIfReceivedBeforeDate", inboxRule, dynamicParameters, (inboxRule.ExceptIfReceivedBeforeDate == null) ? null : new ExDateTime?(ExDateTimeConverter.Parse(inboxRule.ExceptIfReceivedBeforeDate)));
			this.cmdletRunner.SetTaskParameterIfModified("ExceptIfMessageTypeMatches", inboxRule, dynamicParameters, (inboxRule.ExceptIfMessageTypeMatches == NullableInboxRuleMessageType.NullInboxRuleMessageType) ? null : new InboxRuleMessageType?((InboxRuleMessageType)inboxRule.ExceptIfMessageTypeMatches));
			this.cmdletRunner.SetTaskParameterIfModified("ExceptIfWithImportance", inboxRule, dynamicParameters, (inboxRule.ExceptIfWithImportance == NullableImportance.NullImportance) ? null : new Importance?((Importance)inboxRule.ExceptIfWithImportance));
			this.cmdletRunner.SetTaskParameterIfModified("ExceptIfWithinSizeRangeMaximum", inboxRule, dynamicParameters, (inboxRule.ExceptIfWithinSizeRangeMaximum == null) ? null : new ByteQuantifiedSize?(new ByteQuantifiedSize(inboxRule.ExceptIfWithinSizeRangeMaximum.Value)));
			this.cmdletRunner.SetTaskParameterIfModified("ExceptIfWithinSizeRangeMinimum", inboxRule, dynamicParameters, (inboxRule.ExceptIfWithinSizeRangeMinimum == null) ? null : new ByteQuantifiedSize?(new ByteQuantifiedSize(inboxRule.ExceptIfWithinSizeRangeMinimum.Value)));
			this.cmdletRunner.SetTaskParameterIfModified("ExceptIfWithSensitivity", inboxRule, dynamicParameters, (inboxRule.ExceptIfWithSensitivity == NullableSensitivity.NullSensitivity) ? null : new Sensitivity?((Sensitivity)inboxRule.ExceptIfWithSensitivity));
			this.cmdletRunner.SetTaskParameterIfModified("MarkImportance", inboxRule, dynamicParameters, (inboxRule.MarkImportance == NullableImportance.NullImportance) ? null : new Importance?((Importance)inboxRule.MarkImportance));
			this.cmdletRunner.SetTaskParameterIfModified("MessageTypeMatches", inboxRule, dynamicParameters, (inboxRule.MessageTypeMatches == NullableInboxRuleMessageType.NullInboxRuleMessageType) ? null : new InboxRuleMessageType?((InboxRuleMessageType)inboxRule.MessageTypeMatches));
			this.cmdletRunner.SetTaskParameterIfModified("ReceivedAfterDate", inboxRule, dynamicParameters, (inboxRule.ReceivedAfterDate == null) ? null : new ExDateTime?(ExDateTimeConverter.Parse(inboxRule.ReceivedAfterDate)));
			this.cmdletRunner.SetTaskParameterIfModified("ReceivedBeforeDate", inboxRule, dynamicParameters, (inboxRule.ReceivedBeforeDate == null) ? null : new ExDateTime?(ExDateTimeConverter.Parse(inboxRule.ReceivedBeforeDate)));
			this.cmdletRunner.SetTaskParameterIfModified("SendTextMessageNotificationTo", inboxRule, dynamicParameters, (inboxRule.SendTextMessageNotificationTo == null) ? null : new MultiValuedProperty<E164Number>(inboxRule.SendTextMessageNotificationTo));
			this.cmdletRunner.SetTaskParameterIfModified("WithImportance", inboxRule, dynamicParameters, (inboxRule.WithImportance == NullableImportance.NullImportance) ? null : new Importance?((Importance)inboxRule.WithImportance));
			this.cmdletRunner.SetTaskParameterIfModified("WithinSizeRangeMaximum", inboxRule, dynamicParameters, (inboxRule.WithinSizeRangeMaximum == null) ? null : new ByteQuantifiedSize?(new ByteQuantifiedSize(inboxRule.WithinSizeRangeMaximum.Value)));
			this.cmdletRunner.SetTaskParameterIfModified("WithinSizeRangeMinimum", inboxRule, dynamicParameters, (inboxRule.WithinSizeRangeMinimum == null) ? null : new ByteQuantifiedSize?(new ByteQuantifiedSize(inboxRule.WithinSizeRangeMinimum.Value)));
			this.cmdletRunner.SetTaskParameterIfModified("WithSensitivity", inboxRule, dynamicParameters, (inboxRule.WithSensitivity == NullableSensitivity.NullSensitivity) ? null : new Sensitivity?((Sensitivity)inboxRule.WithSensitivity));
			this.cmdletRunner.SetRemainingModifiedTaskParameters(this.request.InboxRule, dynamicParameters);
		}

		protected override PSLocalTask<SetInboxRule, Microsoft.Exchange.Management.Common.InboxRule> InvokeCmdletFactory()
		{
			return CmdletTaskFactory.Instance.CreateSetInboxRuleTask(base.CallContext.AccessingPrincipal);
		}
	}
}
