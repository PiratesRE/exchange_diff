using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class SetInboxRuleCommand : SyntheticCommandWithPipelineInputNoOutput<InboxRule>
	{
		private SetInboxRuleCommand() : base("Set-InboxRule")
		{
		}

		public SetInboxRuleCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual SetInboxRuleCommand SetParameters(SetInboxRuleCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SetInboxRuleCommand SetParameters(SetInboxRuleCommand.IdentityParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual SwitchParameter Force
			{
				set
				{
					base.PowerSharpParameters["Force"] = value;
				}
			}

			public virtual SwitchParameter AlwaysDeleteOutlookRulesBlob
			{
				set
				{
					base.PowerSharpParameters["AlwaysDeleteOutlookRulesBlob"] = value;
				}
			}

			public virtual string Mailbox
			{
				set
				{
					base.PowerSharpParameters["Mailbox"] = ((value != null) ? new MailboxIdParameter(value) : null);
				}
			}

			public virtual RecipientIdParameter From
			{
				set
				{
					base.PowerSharpParameters["From"] = value;
				}
			}

			public virtual RecipientIdParameter ExceptIfFrom
			{
				set
				{
					base.PowerSharpParameters["ExceptIfFrom"] = value;
				}
			}

			public virtual MessageClassificationIdParameter HasClassification
			{
				set
				{
					base.PowerSharpParameters["HasClassification"] = value;
				}
			}

			public virtual MessageClassificationIdParameter ExceptIfHasClassification
			{
				set
				{
					base.PowerSharpParameters["ExceptIfHasClassification"] = value;
				}
			}

			public virtual RecipientIdParameter SentTo
			{
				set
				{
					base.PowerSharpParameters["SentTo"] = value;
				}
			}

			public virtual RecipientIdParameter ExceptIfSentTo
			{
				set
				{
					base.PowerSharpParameters["ExceptIfSentTo"] = value;
				}
			}

			public virtual string CopyToFolder
			{
				set
				{
					base.PowerSharpParameters["CopyToFolder"] = ((value != null) ? new MailboxFolderIdParameter(value) : null);
				}
			}

			public virtual RecipientIdParameter ForwardAsAttachmentTo
			{
				set
				{
					base.PowerSharpParameters["ForwardAsAttachmentTo"] = value;
				}
			}

			public virtual RecipientIdParameter ForwardTo
			{
				set
				{
					base.PowerSharpParameters["ForwardTo"] = value;
				}
			}

			public virtual string MoveToFolder
			{
				set
				{
					base.PowerSharpParameters["MoveToFolder"] = ((value != null) ? new MailboxFolderIdParameter(value) : null);
				}
			}

			public virtual RecipientIdParameter RedirectTo
			{
				set
				{
					base.PowerSharpParameters["RedirectTo"] = value;
				}
			}

			public virtual AggregationSubscriptionIdentity FromSubscription
			{
				set
				{
					base.PowerSharpParameters["FromSubscription"] = value;
				}
			}

			public virtual AggregationSubscriptionIdentity ExceptIfFromSubscription
			{
				set
				{
					base.PowerSharpParameters["ExceptIfFromSubscription"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual string Name
			{
				set
				{
					base.PowerSharpParameters["Name"] = value;
				}
			}

			public virtual int Priority
			{
				set
				{
					base.PowerSharpParameters["Priority"] = value;
				}
			}

			public virtual MultiValuedProperty<string> BodyContainsWords
			{
				set
				{
					base.PowerSharpParameters["BodyContainsWords"] = value;
				}
			}

			public virtual MultiValuedProperty<string> ExceptIfBodyContainsWords
			{
				set
				{
					base.PowerSharpParameters["ExceptIfBodyContainsWords"] = value;
				}
			}

			public virtual string FlaggedForAction
			{
				set
				{
					base.PowerSharpParameters["FlaggedForAction"] = value;
				}
			}

			public virtual string ExceptIfFlaggedForAction
			{
				set
				{
					base.PowerSharpParameters["ExceptIfFlaggedForAction"] = value;
				}
			}

			public virtual MultiValuedProperty<string> FromAddressContainsWords
			{
				set
				{
					base.PowerSharpParameters["FromAddressContainsWords"] = value;
				}
			}

			public virtual MultiValuedProperty<string> ExceptIfFromAddressContainsWords
			{
				set
				{
					base.PowerSharpParameters["ExceptIfFromAddressContainsWords"] = value;
				}
			}

			public virtual bool HasAttachment
			{
				set
				{
					base.PowerSharpParameters["HasAttachment"] = value;
				}
			}

			public virtual bool ExceptIfHasAttachment
			{
				set
				{
					base.PowerSharpParameters["ExceptIfHasAttachment"] = value;
				}
			}

			public virtual MultiValuedProperty<string> HeaderContainsWords
			{
				set
				{
					base.PowerSharpParameters["HeaderContainsWords"] = value;
				}
			}

			public virtual MultiValuedProperty<string> ExceptIfHeaderContainsWords
			{
				set
				{
					base.PowerSharpParameters["ExceptIfHeaderContainsWords"] = value;
				}
			}

			public virtual InboxRuleMessageType? MessageTypeMatches
			{
				set
				{
					base.PowerSharpParameters["MessageTypeMatches"] = value;
				}
			}

			public virtual InboxRuleMessageType? ExceptIfMessageTypeMatches
			{
				set
				{
					base.PowerSharpParameters["ExceptIfMessageTypeMatches"] = value;
				}
			}

			public virtual bool MyNameInCcBox
			{
				set
				{
					base.PowerSharpParameters["MyNameInCcBox"] = value;
				}
			}

			public virtual bool ExceptIfMyNameInCcBox
			{
				set
				{
					base.PowerSharpParameters["ExceptIfMyNameInCcBox"] = value;
				}
			}

			public virtual bool MyNameInToBox
			{
				set
				{
					base.PowerSharpParameters["MyNameInToBox"] = value;
				}
			}

			public virtual bool ExceptIfMyNameInToBox
			{
				set
				{
					base.PowerSharpParameters["ExceptIfMyNameInToBox"] = value;
				}
			}

			public virtual bool MyNameInToOrCcBox
			{
				set
				{
					base.PowerSharpParameters["MyNameInToOrCcBox"] = value;
				}
			}

			public virtual bool ExceptIfMyNameInToOrCcBox
			{
				set
				{
					base.PowerSharpParameters["ExceptIfMyNameInToOrCcBox"] = value;
				}
			}

			public virtual bool MyNameNotInToBox
			{
				set
				{
					base.PowerSharpParameters["MyNameNotInToBox"] = value;
				}
			}

			public virtual bool ExceptIfMyNameNotInToBox
			{
				set
				{
					base.PowerSharpParameters["ExceptIfMyNameNotInToBox"] = value;
				}
			}

			public virtual ExDateTime? ReceivedAfterDate
			{
				set
				{
					base.PowerSharpParameters["ReceivedAfterDate"] = value;
				}
			}

			public virtual ExDateTime? ExceptIfReceivedAfterDate
			{
				set
				{
					base.PowerSharpParameters["ExceptIfReceivedAfterDate"] = value;
				}
			}

			public virtual ExDateTime? ReceivedBeforeDate
			{
				set
				{
					base.PowerSharpParameters["ReceivedBeforeDate"] = value;
				}
			}

			public virtual ExDateTime? ExceptIfReceivedBeforeDate
			{
				set
				{
					base.PowerSharpParameters["ExceptIfReceivedBeforeDate"] = value;
				}
			}

			public virtual MultiValuedProperty<string> RecipientAddressContainsWords
			{
				set
				{
					base.PowerSharpParameters["RecipientAddressContainsWords"] = value;
				}
			}

			public virtual MultiValuedProperty<string> ExceptIfRecipientAddressContainsWords
			{
				set
				{
					base.PowerSharpParameters["ExceptIfRecipientAddressContainsWords"] = value;
				}
			}

			public virtual bool SentOnlyToMe
			{
				set
				{
					base.PowerSharpParameters["SentOnlyToMe"] = value;
				}
			}

			public virtual bool ExceptIfSentOnlyToMe
			{
				set
				{
					base.PowerSharpParameters["ExceptIfSentOnlyToMe"] = value;
				}
			}

			public virtual MultiValuedProperty<string> SubjectContainsWords
			{
				set
				{
					base.PowerSharpParameters["SubjectContainsWords"] = value;
				}
			}

			public virtual MultiValuedProperty<string> ExceptIfSubjectContainsWords
			{
				set
				{
					base.PowerSharpParameters["ExceptIfSubjectContainsWords"] = value;
				}
			}

			public virtual MultiValuedProperty<string> SubjectOrBodyContainsWords
			{
				set
				{
					base.PowerSharpParameters["SubjectOrBodyContainsWords"] = value;
				}
			}

			public virtual MultiValuedProperty<string> ExceptIfSubjectOrBodyContainsWords
			{
				set
				{
					base.PowerSharpParameters["ExceptIfSubjectOrBodyContainsWords"] = value;
				}
			}

			public virtual Importance? WithImportance
			{
				set
				{
					base.PowerSharpParameters["WithImportance"] = value;
				}
			}

			public virtual Importance? ExceptIfWithImportance
			{
				set
				{
					base.PowerSharpParameters["ExceptIfWithImportance"] = value;
				}
			}

			public virtual ByteQuantifiedSize? WithinSizeRangeMaximum
			{
				set
				{
					base.PowerSharpParameters["WithinSizeRangeMaximum"] = value;
				}
			}

			public virtual ByteQuantifiedSize? ExceptIfWithinSizeRangeMaximum
			{
				set
				{
					base.PowerSharpParameters["ExceptIfWithinSizeRangeMaximum"] = value;
				}
			}

			public virtual ByteQuantifiedSize? WithinSizeRangeMinimum
			{
				set
				{
					base.PowerSharpParameters["WithinSizeRangeMinimum"] = value;
				}
			}

			public virtual ByteQuantifiedSize? ExceptIfWithinSizeRangeMinimum
			{
				set
				{
					base.PowerSharpParameters["ExceptIfWithinSizeRangeMinimum"] = value;
				}
			}

			public virtual Sensitivity? WithSensitivity
			{
				set
				{
					base.PowerSharpParameters["WithSensitivity"] = value;
				}
			}

			public virtual Sensitivity? ExceptIfWithSensitivity
			{
				set
				{
					base.PowerSharpParameters["ExceptIfWithSensitivity"] = value;
				}
			}

			public virtual MultiValuedProperty<string> ApplyCategory
			{
				set
				{
					base.PowerSharpParameters["ApplyCategory"] = value;
				}
			}

			public virtual bool DeleteMessage
			{
				set
				{
					base.PowerSharpParameters["DeleteMessage"] = value;
				}
			}

			public virtual bool MarkAsRead
			{
				set
				{
					base.PowerSharpParameters["MarkAsRead"] = value;
				}
			}

			public virtual Importance? MarkImportance
			{
				set
				{
					base.PowerSharpParameters["MarkImportance"] = value;
				}
			}

			public virtual MultiValuedProperty<E164Number> SendTextMessageNotificationTo
			{
				set
				{
					base.PowerSharpParameters["SendTextMessageNotificationTo"] = value;
				}
			}

			public virtual bool StopProcessingRules
			{
				set
				{
					base.PowerSharpParameters["StopProcessingRules"] = value;
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
					base.PowerSharpParameters["Identity"] = ((value != null) ? new InboxRuleIdParameter(value) : null);
				}
			}

			public virtual SwitchParameter Force
			{
				set
				{
					base.PowerSharpParameters["Force"] = value;
				}
			}

			public virtual SwitchParameter AlwaysDeleteOutlookRulesBlob
			{
				set
				{
					base.PowerSharpParameters["AlwaysDeleteOutlookRulesBlob"] = value;
				}
			}

			public virtual string Mailbox
			{
				set
				{
					base.PowerSharpParameters["Mailbox"] = ((value != null) ? new MailboxIdParameter(value) : null);
				}
			}

			public virtual RecipientIdParameter From
			{
				set
				{
					base.PowerSharpParameters["From"] = value;
				}
			}

			public virtual RecipientIdParameter ExceptIfFrom
			{
				set
				{
					base.PowerSharpParameters["ExceptIfFrom"] = value;
				}
			}

			public virtual MessageClassificationIdParameter HasClassification
			{
				set
				{
					base.PowerSharpParameters["HasClassification"] = value;
				}
			}

			public virtual MessageClassificationIdParameter ExceptIfHasClassification
			{
				set
				{
					base.PowerSharpParameters["ExceptIfHasClassification"] = value;
				}
			}

			public virtual RecipientIdParameter SentTo
			{
				set
				{
					base.PowerSharpParameters["SentTo"] = value;
				}
			}

			public virtual RecipientIdParameter ExceptIfSentTo
			{
				set
				{
					base.PowerSharpParameters["ExceptIfSentTo"] = value;
				}
			}

			public virtual string CopyToFolder
			{
				set
				{
					base.PowerSharpParameters["CopyToFolder"] = ((value != null) ? new MailboxFolderIdParameter(value) : null);
				}
			}

			public virtual RecipientIdParameter ForwardAsAttachmentTo
			{
				set
				{
					base.PowerSharpParameters["ForwardAsAttachmentTo"] = value;
				}
			}

			public virtual RecipientIdParameter ForwardTo
			{
				set
				{
					base.PowerSharpParameters["ForwardTo"] = value;
				}
			}

			public virtual string MoveToFolder
			{
				set
				{
					base.PowerSharpParameters["MoveToFolder"] = ((value != null) ? new MailboxFolderIdParameter(value) : null);
				}
			}

			public virtual RecipientIdParameter RedirectTo
			{
				set
				{
					base.PowerSharpParameters["RedirectTo"] = value;
				}
			}

			public virtual AggregationSubscriptionIdentity FromSubscription
			{
				set
				{
					base.PowerSharpParameters["FromSubscription"] = value;
				}
			}

			public virtual AggregationSubscriptionIdentity ExceptIfFromSubscription
			{
				set
				{
					base.PowerSharpParameters["ExceptIfFromSubscription"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual string Name
			{
				set
				{
					base.PowerSharpParameters["Name"] = value;
				}
			}

			public virtual int Priority
			{
				set
				{
					base.PowerSharpParameters["Priority"] = value;
				}
			}

			public virtual MultiValuedProperty<string> BodyContainsWords
			{
				set
				{
					base.PowerSharpParameters["BodyContainsWords"] = value;
				}
			}

			public virtual MultiValuedProperty<string> ExceptIfBodyContainsWords
			{
				set
				{
					base.PowerSharpParameters["ExceptIfBodyContainsWords"] = value;
				}
			}

			public virtual string FlaggedForAction
			{
				set
				{
					base.PowerSharpParameters["FlaggedForAction"] = value;
				}
			}

			public virtual string ExceptIfFlaggedForAction
			{
				set
				{
					base.PowerSharpParameters["ExceptIfFlaggedForAction"] = value;
				}
			}

			public virtual MultiValuedProperty<string> FromAddressContainsWords
			{
				set
				{
					base.PowerSharpParameters["FromAddressContainsWords"] = value;
				}
			}

			public virtual MultiValuedProperty<string> ExceptIfFromAddressContainsWords
			{
				set
				{
					base.PowerSharpParameters["ExceptIfFromAddressContainsWords"] = value;
				}
			}

			public virtual bool HasAttachment
			{
				set
				{
					base.PowerSharpParameters["HasAttachment"] = value;
				}
			}

			public virtual bool ExceptIfHasAttachment
			{
				set
				{
					base.PowerSharpParameters["ExceptIfHasAttachment"] = value;
				}
			}

			public virtual MultiValuedProperty<string> HeaderContainsWords
			{
				set
				{
					base.PowerSharpParameters["HeaderContainsWords"] = value;
				}
			}

			public virtual MultiValuedProperty<string> ExceptIfHeaderContainsWords
			{
				set
				{
					base.PowerSharpParameters["ExceptIfHeaderContainsWords"] = value;
				}
			}

			public virtual InboxRuleMessageType? MessageTypeMatches
			{
				set
				{
					base.PowerSharpParameters["MessageTypeMatches"] = value;
				}
			}

			public virtual InboxRuleMessageType? ExceptIfMessageTypeMatches
			{
				set
				{
					base.PowerSharpParameters["ExceptIfMessageTypeMatches"] = value;
				}
			}

			public virtual bool MyNameInCcBox
			{
				set
				{
					base.PowerSharpParameters["MyNameInCcBox"] = value;
				}
			}

			public virtual bool ExceptIfMyNameInCcBox
			{
				set
				{
					base.PowerSharpParameters["ExceptIfMyNameInCcBox"] = value;
				}
			}

			public virtual bool MyNameInToBox
			{
				set
				{
					base.PowerSharpParameters["MyNameInToBox"] = value;
				}
			}

			public virtual bool ExceptIfMyNameInToBox
			{
				set
				{
					base.PowerSharpParameters["ExceptIfMyNameInToBox"] = value;
				}
			}

			public virtual bool MyNameInToOrCcBox
			{
				set
				{
					base.PowerSharpParameters["MyNameInToOrCcBox"] = value;
				}
			}

			public virtual bool ExceptIfMyNameInToOrCcBox
			{
				set
				{
					base.PowerSharpParameters["ExceptIfMyNameInToOrCcBox"] = value;
				}
			}

			public virtual bool MyNameNotInToBox
			{
				set
				{
					base.PowerSharpParameters["MyNameNotInToBox"] = value;
				}
			}

			public virtual bool ExceptIfMyNameNotInToBox
			{
				set
				{
					base.PowerSharpParameters["ExceptIfMyNameNotInToBox"] = value;
				}
			}

			public virtual ExDateTime? ReceivedAfterDate
			{
				set
				{
					base.PowerSharpParameters["ReceivedAfterDate"] = value;
				}
			}

			public virtual ExDateTime? ExceptIfReceivedAfterDate
			{
				set
				{
					base.PowerSharpParameters["ExceptIfReceivedAfterDate"] = value;
				}
			}

			public virtual ExDateTime? ReceivedBeforeDate
			{
				set
				{
					base.PowerSharpParameters["ReceivedBeforeDate"] = value;
				}
			}

			public virtual ExDateTime? ExceptIfReceivedBeforeDate
			{
				set
				{
					base.PowerSharpParameters["ExceptIfReceivedBeforeDate"] = value;
				}
			}

			public virtual MultiValuedProperty<string> RecipientAddressContainsWords
			{
				set
				{
					base.PowerSharpParameters["RecipientAddressContainsWords"] = value;
				}
			}

			public virtual MultiValuedProperty<string> ExceptIfRecipientAddressContainsWords
			{
				set
				{
					base.PowerSharpParameters["ExceptIfRecipientAddressContainsWords"] = value;
				}
			}

			public virtual bool SentOnlyToMe
			{
				set
				{
					base.PowerSharpParameters["SentOnlyToMe"] = value;
				}
			}

			public virtual bool ExceptIfSentOnlyToMe
			{
				set
				{
					base.PowerSharpParameters["ExceptIfSentOnlyToMe"] = value;
				}
			}

			public virtual MultiValuedProperty<string> SubjectContainsWords
			{
				set
				{
					base.PowerSharpParameters["SubjectContainsWords"] = value;
				}
			}

			public virtual MultiValuedProperty<string> ExceptIfSubjectContainsWords
			{
				set
				{
					base.PowerSharpParameters["ExceptIfSubjectContainsWords"] = value;
				}
			}

			public virtual MultiValuedProperty<string> SubjectOrBodyContainsWords
			{
				set
				{
					base.PowerSharpParameters["SubjectOrBodyContainsWords"] = value;
				}
			}

			public virtual MultiValuedProperty<string> ExceptIfSubjectOrBodyContainsWords
			{
				set
				{
					base.PowerSharpParameters["ExceptIfSubjectOrBodyContainsWords"] = value;
				}
			}

			public virtual Importance? WithImportance
			{
				set
				{
					base.PowerSharpParameters["WithImportance"] = value;
				}
			}

			public virtual Importance? ExceptIfWithImportance
			{
				set
				{
					base.PowerSharpParameters["ExceptIfWithImportance"] = value;
				}
			}

			public virtual ByteQuantifiedSize? WithinSizeRangeMaximum
			{
				set
				{
					base.PowerSharpParameters["WithinSizeRangeMaximum"] = value;
				}
			}

			public virtual ByteQuantifiedSize? ExceptIfWithinSizeRangeMaximum
			{
				set
				{
					base.PowerSharpParameters["ExceptIfWithinSizeRangeMaximum"] = value;
				}
			}

			public virtual ByteQuantifiedSize? WithinSizeRangeMinimum
			{
				set
				{
					base.PowerSharpParameters["WithinSizeRangeMinimum"] = value;
				}
			}

			public virtual ByteQuantifiedSize? ExceptIfWithinSizeRangeMinimum
			{
				set
				{
					base.PowerSharpParameters["ExceptIfWithinSizeRangeMinimum"] = value;
				}
			}

			public virtual Sensitivity? WithSensitivity
			{
				set
				{
					base.PowerSharpParameters["WithSensitivity"] = value;
				}
			}

			public virtual Sensitivity? ExceptIfWithSensitivity
			{
				set
				{
					base.PowerSharpParameters["ExceptIfWithSensitivity"] = value;
				}
			}

			public virtual MultiValuedProperty<string> ApplyCategory
			{
				set
				{
					base.PowerSharpParameters["ApplyCategory"] = value;
				}
			}

			public virtual bool DeleteMessage
			{
				set
				{
					base.PowerSharpParameters["DeleteMessage"] = value;
				}
			}

			public virtual bool MarkAsRead
			{
				set
				{
					base.PowerSharpParameters["MarkAsRead"] = value;
				}
			}

			public virtual Importance? MarkImportance
			{
				set
				{
					base.PowerSharpParameters["MarkImportance"] = value;
				}
			}

			public virtual MultiValuedProperty<E164Number> SendTextMessageNotificationTo
			{
				set
				{
					base.PowerSharpParameters["SendTextMessageNotificationTo"] = value;
				}
			}

			public virtual bool StopProcessingRules
			{
				set
				{
					base.PowerSharpParameters["StopProcessingRules"] = value;
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
