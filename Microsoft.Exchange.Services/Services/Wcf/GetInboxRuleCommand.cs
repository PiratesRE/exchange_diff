using System;
using System.Collections.Generic;
using System.Linq;
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

namespace Microsoft.Exchange.Services.Wcf
{
	internal sealed class GetInboxRuleCommand : SingleCmdletCommandBase<GetInboxRuleRequest, GetInboxRuleResponse, GetInboxRule, Microsoft.Exchange.Management.Common.InboxRule>
	{
		public GetInboxRuleCommand(CallContext callContext, GetInboxRuleRequest request) : base(callContext, request, "Get-InboxRule", ScopeLocation.RecipientRead)
		{
		}

		protected override void PopulateTaskParameters()
		{
			GetInboxRule task = this.cmdletRunner.TaskWrapper.Task;
			if (this.request.DescriptionTimeFormat != null)
			{
				this.cmdletRunner.SetTaskParameter("DescriptionTimeFormat", task, this.request.DescriptionTimeFormat);
			}
			if (this.request.DescriptionTimeZone != null)
			{
				this.cmdletRunner.SetTaskParameter("DescriptionTimeZone", task, ExTimeZoneValue.Parse(this.request.DescriptionTimeZone));
			}
			this.warningCollector = new CmdletResultsWarningCollector();
			this.cmdletRunner.TaskWrapper.Task.PrependTaskIOPipelineHandler(this.warningCollector);
		}

		protected override void PopulateResponseData(GetInboxRuleResponse response)
		{
			IEnumerable<Microsoft.Exchange.Management.Common.InboxRule> allResults = this.cmdletRunner.TaskWrapper.AllResults;
			IEnumerable<Microsoft.Exchange.Services.Wcf.Types.InboxRule> source = allResults.Select((Microsoft.Exchange.Management.Common.InboxRule inboxRule, int resultIndex) => new Microsoft.Exchange.Services.Wcf.Types.InboxRule
			{
				Identity = new Identity(inboxRule.Identity.ToString()),
				ApplyCategory = inboxRule.ApplyCategory.ToStringArray<string>(),
				BodyContainsWords = inboxRule.BodyContainsWords.ToStringArray<string>(),
				CopyToFolder = ((inboxRule.CopyToFolder == null) ? null : inboxRule.CopyToFolder.ToIdentity()),
				DeleteMessage = inboxRule.DeleteMessage,
				Description = ((inboxRule.Description == null) ? null : new RuleDescription(inboxRule.Description)),
				DescriptionTimeFormat = inboxRule.DescriptionTimeFormat,
				DescriptionTimeZone = ((inboxRule.DescriptionTimeZone == null || inboxRule.DescriptionTimeZone.ExTimeZone == null) ? new ExTimeZoneValue(ExTimeZone.UtcTimeZone) : inboxRule.DescriptionTimeZone).ToString(),
				Enabled = inboxRule.Enabled,
				ExceptIfBodyContainsWords = inboxRule.ExceptIfBodyContainsWords.ToStringArray<string>(),
				ExceptIfFlaggedForAction = inboxRule.ExceptIfFlaggedForAction,
				ExceptIfFrom = inboxRule.ExceptIfFrom.ToPeopleIdentityArray(),
				ExceptIfFromAddressContainsWords = inboxRule.ExceptIfFromAddressContainsWords.ToStringArray<string>(),
				ExceptIfFromSubscription = inboxRule.ExceptIfFromSubscription.ToIdentityArray(inboxRule),
				ExceptIfHasClassification = inboxRule.ExceptIfHasClassification.ToIdentityArray(),
				ExceptIfHasAttachment = inboxRule.ExceptIfHasAttachment,
				ExceptIfHeaderContainsWords = inboxRule.ExceptIfHeaderContainsWords.ToStringArray<string>(),
				ExceptIfMessageTypeMatches = (NullableInboxRuleMessageType)((inboxRule.ExceptIfMessageTypeMatches == null) ? ((InboxRuleMessageType)(-1)) : inboxRule.ExceptIfMessageTypeMatches.Value),
				ExceptIfMyNameInCcBox = inboxRule.ExceptIfMyNameInCcBox,
				ExceptIfMyNameInToBox = inboxRule.ExceptIfMyNameInToBox,
				ExceptIfMyNameInToOrCcBox = inboxRule.ExceptIfMyNameInToOrCcBox,
				ExceptIfMyNameNotInToBox = inboxRule.ExceptIfMyNameNotInToBox,
				ExceptIfReceivedAfterDate = ((inboxRule.ExceptIfReceivedAfterDate == null) ? null : ExDateTimeConverter.ToSoapHeaderTimeZoneRelatedXsdDateTime(inboxRule.ExceptIfReceivedAfterDate.Value)),
				ExceptIfReceivedBeforeDate = ((inboxRule.ExceptIfReceivedBeforeDate == null) ? null : ExDateTimeConverter.ToSoapHeaderTimeZoneRelatedXsdDateTime(inboxRule.ExceptIfReceivedBeforeDate.Value)),
				ExceptIfRecipientAddressContainsWords = inboxRule.ExceptIfRecipientAddressContainsWords.ToStringArray<string>(),
				ExceptIfSentOnlyToMe = inboxRule.ExceptIfSentOnlyToMe,
				ExceptIfSentTo = inboxRule.ExceptIfSentTo.ToPeopleIdentityArray(),
				ExceptIfSubjectContainsWords = inboxRule.ExceptIfSubjectContainsWords.ToStringArray<string>(),
				ExceptIfSubjectOrBodyContainsWords = inboxRule.ExceptIfSubjectOrBodyContainsWords.ToStringArray<string>(),
				ExceptIfWithImportance = (NullableImportance)((inboxRule.ExceptIfWithImportance == null) ? ((Importance)(-1)) : inboxRule.ExceptIfWithImportance.Value),
				ExceptIfWithinSizeRangeMaximum = ((inboxRule.ExceptIfWithinSizeRangeMaximum == null) ? null : new ulong?(inboxRule.ExceptIfWithinSizeRangeMaximum.Value.ToBytes())),
				ExceptIfWithinSizeRangeMinimum = ((inboxRule.ExceptIfWithinSizeRangeMinimum == null) ? null : new ulong?(inboxRule.ExceptIfWithinSizeRangeMinimum.Value.ToBytes())),
				ExceptIfWithSensitivity = (NullableSensitivity)((inboxRule.ExceptIfWithSensitivity == null) ? ((Sensitivity)(-1)) : inboxRule.ExceptIfWithSensitivity.Value),
				FlaggedForAction = inboxRule.FlaggedForAction,
				ForwardAsAttachmentTo = inboxRule.ForwardAsAttachmentTo.ToPeopleIdentityArray(),
				ForwardTo = inboxRule.ForwardTo.ToPeopleIdentityArray(),
				From = inboxRule.From.ToPeopleIdentityArray(),
				FromAddressContainsWords = inboxRule.FromAddressContainsWords.ToStringArray<string>(),
				FromSubscription = inboxRule.FromSubscription.ToIdentityArray(inboxRule),
				HasAttachment = inboxRule.HasAttachment,
				HasClassification = inboxRule.HasClassification.ToIdentityArray(),
				HeaderContainsWords = inboxRule.HeaderContainsWords.ToStringArray<string>(),
				InError = inboxRule.InError,
				MarkAsRead = inboxRule.MarkAsRead,
				MarkImportance = (NullableImportance)((inboxRule.MarkImportance == null) ? ((Importance)(-1)) : inboxRule.MarkImportance.Value),
				MessageTypeMatches = (NullableInboxRuleMessageType)((inboxRule.MessageTypeMatches == null) ? ((InboxRuleMessageType)(-1)) : inboxRule.MessageTypeMatches.Value),
				MoveToFolder = ((inboxRule.MoveToFolder == null) ? null : inboxRule.MoveToFolder.ToIdentity()),
				MyNameInCcBox = inboxRule.MyNameInCcBox,
				MyNameInToBox = inboxRule.MyNameInToBox,
				MyNameInToOrCcBox = inboxRule.MyNameInToOrCcBox,
				MyNameNotInToBox = inboxRule.MyNameNotInToBox,
				Name = inboxRule.Name,
				Priority = inboxRule.Priority,
				ReceivedAfterDate = ((inboxRule.ReceivedAfterDate == null) ? null : ExDateTimeConverter.ToSoapHeaderTimeZoneRelatedXsdDateTime(inboxRule.ReceivedAfterDate.Value)),
				ReceivedBeforeDate = ((inboxRule.ReceivedBeforeDate == null) ? null : ExDateTimeConverter.ToSoapHeaderTimeZoneRelatedXsdDateTime(inboxRule.ReceivedBeforeDate.Value)),
				RecipientAddressContainsWords = inboxRule.RecipientAddressContainsWords.ToStringArray<string>(),
				RedirectTo = inboxRule.RedirectTo.ToPeopleIdentityArray(),
				SendTextMessageNotificationTo = inboxRule.SendTextMessageNotificationTo.ToStringArray<E164Number>(),
				SentOnlyToMe = inboxRule.SentOnlyToMe,
				SentTo = inboxRule.SentTo.ToPeopleIdentityArray(),
				StopProcessingRules = inboxRule.StopProcessingRules,
				SubjectContainsWords = inboxRule.SubjectContainsWords.ToStringArray<string>(),
				SupportedByTask = inboxRule.SupportedByTask,
				SubjectOrBodyContainsWords = inboxRule.SubjectOrBodyContainsWords.ToStringArray<string>(),
				WarningMessages = this.warningCollector.GetWarningMessagesForResult(resultIndex),
				WithImportance = (NullableImportance)((inboxRule.WithImportance == null) ? ((Importance)(-1)) : inboxRule.WithImportance.Value),
				WithinSizeRangeMinimum = ((inboxRule.WithinSizeRangeMinimum == null) ? null : new ulong?(inboxRule.WithinSizeRangeMinimum.Value.ToBytes())),
				WithinSizeRangeMaximum = ((inboxRule.WithinSizeRangeMaximum == null) ? null : new ulong?(inboxRule.WithinSizeRangeMaximum.Value.ToBytes())),
				WithSensitivity = (NullableSensitivity)((inboxRule.WithSensitivity == null) ? ((Sensitivity)(-1)) : inboxRule.WithSensitivity.Value)
			});
			response.InboxRuleCollection.InboxRules = source.ToArray<Microsoft.Exchange.Services.Wcf.Types.InboxRule>();
		}

		protected override PSLocalTask<GetInboxRule, Microsoft.Exchange.Management.Common.InboxRule> InvokeCmdletFactory()
		{
			return CmdletTaskFactory.Instance.CreateGetInboxRuleTask(base.CallContext.AccessingPrincipal);
		}

		private CmdletResultsWarningCollector warningCollector;
	}
}
