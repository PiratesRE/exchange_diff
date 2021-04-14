using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.RecipientTasks;
using Microsoft.Exchange.MessagingPolicies.Journaling;
using Microsoft.Exchange.MessagingPolicies.Rules;
using Microsoft.Exchange.MessagingPolicies.Rules.Tasks;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class RuleRow : BaseRow
	{
		public RuleRow(Microsoft.Exchange.MessagingPolicies.Rules.Tasks.Rule rule) : base(rule)
		{
			this.Name = rule.Name;
			this.Priority = rule.Priority;
			this.Enabled = (rule.State == RuleState.Enabled);
			this.CaptionText = Strings.EditRuleCaption(rule.Name);
			this.DlpPolicy = rule.DlpPolicy;
			this.RuleVersion = rule.RuleVersion;
			this.RuleMode = rule.Mode.ToStringWithNull();
			int num = 0;
			num += ((!rule.From.IsNullOrEmpty()) ? 1 : 0);
			num += ((!rule.SentTo.IsNullOrEmpty()) ? 1 : 0);
			num += ((rule.FromScope != null) ? 1 : 0);
			num += ((rule.SentToScope != null) ? 1 : 0);
			num += ((!rule.FromMemberOf.IsNullOrEmpty()) ? 1 : 0);
			num += ((!rule.SentToMemberOf.IsNullOrEmpty()) ? 1 : 0);
			num += ((!rule.SubjectOrBodyContainsWords.IsNullOrEmpty()) ? 1 : 0);
			num += ((!rule.FromAddressContainsWords.IsNullOrEmpty()) ? 1 : 0);
			num += ((!rule.RecipientAddressContainsWords.IsNullOrEmpty()) ? 1 : 0);
			num += ((!rule.AttachmentContainsWords.IsNullOrEmpty()) ? 1 : 0);
			num += ((!rule.AttachmentMatchesPatterns.IsNullOrEmpty()) ? 1 : 0);
			num += (rule.AttachmentIsUnsupported ? 1 : 0);
			num += ((!rule.SubjectOrBodyMatchesPatterns.IsNullOrEmpty()) ? 1 : 0);
			num += ((!rule.FromAddressMatchesPatterns.IsNullOrEmpty()) ? 1 : 0);
			num += ((!rule.RecipientAddressMatchesPatterns.IsNullOrEmpty()) ? 1 : 0);
			num += ((!rule.AttachmentNameMatchesPatterns.IsNullOrEmpty()) ? 1 : 0);
			num += (rule.HasNoClassification ? 1 : 0);
			num += ((!rule.SubjectContainsWords.IsNullOrEmpty()) ? 1 : 0);
			num += ((!rule.SubjectMatchesPatterns.IsNullOrEmpty()) ? 1 : 0);
			num += ((!rule.AnyOfToHeader.IsNullOrEmpty()) ? 1 : 0);
			num += ((!rule.AnyOfToHeaderMemberOf.IsNullOrEmpty()) ? 1 : 0);
			num += ((!rule.AnyOfCcHeader.IsNullOrEmpty()) ? 1 : 0);
			num += ((!rule.AnyOfCcHeaderMemberOf.IsNullOrEmpty()) ? 1 : 0);
			num += ((!rule.AnyOfToCcHeader.IsNullOrEmpty()) ? 1 : 0);
			num += ((!rule.AnyOfToCcHeaderMemberOf.IsNullOrEmpty()) ? 1 : 0);
			num += ((rule.MessageTypeMatches != null) ? 1 : 0);
			num += ((rule.SenderManagementRelationship != null) ? 1 : 0);
			num += ((rule.WithImportance != null) ? 1 : 0);
			num += ((!rule.RecipientInSenderList.IsNullOrEmpty()) ? 1 : 0);
			num += ((!rule.SenderInRecipientList.IsNullOrEmpty()) ? 1 : 0);
			num += ((rule.HeaderContainsMessageHeader != null && !rule.HeaderContainsWords.IsNullOrEmpty()) ? 1 : 0);
			num += ((!rule.BetweenMemberOf1.IsNullOrEmpty() && !rule.BetweenMemberOf2.IsNullOrEmpty()) ? 1 : 0);
			num += ((rule.HeaderMatchesMessageHeader != null && !rule.HeaderMatchesPatterns.IsNullOrEmpty()) ? 1 : 0);
			num += ((rule.ManagerForEvaluatedUser != null && !rule.ManagerAddresses.IsNullOrEmpty()) ? 1 : 0);
			num += ((rule.ADComparisonAttribute != null && rule.ADComparisonOperator != null) ? 1 : 0);
			num += ((rule.AttachmentSizeOver != null) ? 1 : 0);
			num += ((rule.SCLOver != null) ? 1 : 0);
			num += ((rule.HasClassification != null) ? 1 : 0);
			num += ((!rule.SenderADAttributeContainsWords.IsNullOrEmpty()) ? 1 : 0);
			num += ((!rule.SenderADAttributeMatchesPatterns.IsNullOrEmpty()) ? 1 : 0);
			num += ((!rule.RecipientADAttributeContainsWords.IsNullOrEmpty()) ? 1 : 0);
			num += ((!rule.RecipientADAttributeMatchesPatterns.IsNullOrEmpty()) ? 1 : 0);
			num += ((rule.MessageSizeOver != null) ? 1 : 0);
			num += ((!rule.MessageContainsDataClassifications.IsNullOrEmpty()) ? 1 : 0);
			num += ((!rule.AttachmentExtensionMatchesWords.IsNullOrEmpty()) ? 1 : 0);
			num += (rule.HasSenderOverride ? 1 : 0);
			num += (rule.AttachmentHasExecutableContent ? 1 : 0);
			num += ((rule.SenderIpRanges != null) ? 1 : 0);
			num += (rule.AttachmentProcessingLimitExceeded ? 1 : 0);
			num += ((!rule.SenderDomainIs.IsNullOrEmpty()) ? 1 : 0);
			num += ((!rule.RecipientDomainIs.IsNullOrEmpty()) ? 1 : 0);
			num += ((!rule.ContentCharacterSetContainsWords.IsNullOrEmpty()) ? 1 : 0);
			num += (rule.AttachmentIsPasswordProtected ? 1 : 0);
			num += ((!rule.AnyOfRecipientAddressContainsWords.IsNullOrEmpty()) ? 1 : 0);
			num += ((!rule.AnyOfRecipientAddressMatchesPatterns.IsNullOrEmpty()) ? 1 : 0);
			int num2 = 0;
			num2 += ((!rule.ModerateMessageByUser.IsNullOrEmpty()) ? 1 : 0);
			num2 += ((!rule.RedirectMessageTo.IsNullOrEmpty()) ? 1 : 0);
			num2 += ((rule.RejectMessageReasonText != null || rule.SenderNotificationType != null) ? 1 : 0);
			num2 += (rule.DeleteMessage ? 1 : 0);
			num2 += ((!rule.BlindCopyTo.IsNullOrEmpty()) ? 1 : 0);
			num2 += ((rule.ApplyHtmlDisclaimerText != null) ? 1 : 0);
			num2 += ((rule.RemoveHeader != null) ? 1 : 0);
			num2 += ((!rule.AddToRecipients.IsNullOrEmpty()) ? 1 : 0);
			num2 += ((!rule.CopyTo.IsNullOrEmpty()) ? 1 : 0);
			num2 += (rule.ModerateMessageByManager ? 1 : 0);
			num2 += ((!string.IsNullOrEmpty(rule.PrependSubject)) ? 1 : 0);
			num2 += ((rule.AddManagerAsRecipientType != null) ? 1 : 0);
			num2 += ((rule.ApplyRightsProtectionTemplate != null) ? 1 : 0);
			num2 += ((rule.SetHeaderName != null && rule.SetHeaderValue != null) ? 1 : 0);
			num2 += ((rule.SetSCL != null) ? 1 : 0);
			num2 += ((rule.ApplyClassification != null) ? 1 : 0);
			num2 += (rule.StopRuleProcessing ? 1 : 0);
			num2 += ((!string.IsNullOrEmpty(rule.SetAuditSeverity)) ? 1 : 0);
			num2 += ((rule.GenerateIncidentReport != null) ? 1 : 0);
			num2 += (rule.RouteMessageOutboundRequireTls ? 1 : 0);
			num2 += (rule.ApplyOME ? 1 : 0);
			num2 += (rule.RemoveOME ? 1 : 0);
			num2 += (rule.Quarantine ? 1 : 0);
			num2 += ((rule.RouteMessageOutboundConnector != null) ? 1 : 0);
			num2 += ((rule.GenerateNotification != null) ? 1 : 0);
			int num3 = 0;
			num3 += ((!rule.ExceptIfFrom.IsNullOrEmpty()) ? 1 : 0);
			num3 += ((!rule.ExceptIfSentTo.IsNullOrEmpty()) ? 1 : 0);
			num3 += ((rule.ExceptIfFromScope != null) ? 1 : 0);
			num3 += ((rule.ExceptIfSentToScope != null) ? 1 : 0);
			num3 += ((!rule.ExceptIfFromMemberOf.IsNullOrEmpty()) ? 1 : 0);
			num3 += ((!rule.ExceptIfSentToMemberOf.IsNullOrEmpty()) ? 1 : 0);
			num3 += ((!rule.ExceptIfSubjectOrBodyContainsWords.IsNullOrEmpty()) ? 1 : 0);
			num3 += ((!rule.ExceptIfFromAddressContainsWords.IsNullOrEmpty()) ? 1 : 0);
			num3 += ((!rule.ExceptIfRecipientAddressContainsWords.IsNullOrEmpty()) ? 1 : 0);
			num3 += ((!rule.ExceptIfAttachmentContainsWords.IsNullOrEmpty()) ? 1 : 0);
			num3 += ((!rule.ExceptIfAttachmentMatchesPatterns.IsNullOrEmpty()) ? 1 : 0);
			num3 += (rule.ExceptIfAttachmentIsUnsupported ? 1 : 0);
			num3 += ((!rule.ExceptIfSubjectOrBodyMatchesPatterns.IsNullOrEmpty()) ? 1 : 0);
			num3 += ((!rule.ExceptIfFromAddressMatchesPatterns.IsNullOrEmpty()) ? 1 : 0);
			num3 += ((!rule.ExceptIfRecipientAddressMatchesPatterns.IsNullOrEmpty()) ? 1 : 0);
			num3 += ((!rule.ExceptIfAttachmentNameMatchesPatterns.IsNullOrEmpty()) ? 1 : 0);
			num3 += (rule.ExceptIfHasNoClassification ? 1 : 0);
			num3 += ((!rule.ExceptIfSubjectContainsWords.IsNullOrEmpty()) ? 1 : 0);
			num3 += ((!rule.ExceptIfSubjectMatchesPatterns.IsNullOrEmpty()) ? 1 : 0);
			num3 += ((!rule.ExceptIfAnyOfToHeader.IsNullOrEmpty()) ? 1 : 0);
			num3 += ((!rule.ExceptIfAnyOfToHeaderMemberOf.IsNullOrEmpty()) ? 1 : 0);
			num3 += ((!rule.ExceptIfAnyOfCcHeader.IsNullOrEmpty()) ? 1 : 0);
			num3 += ((!rule.ExceptIfAnyOfCcHeaderMemberOf.IsNullOrEmpty()) ? 1 : 0);
			num3 += ((!rule.ExceptIfAnyOfToCcHeader.IsNullOrEmpty()) ? 1 : 0);
			num3 += ((!rule.ExceptIfAnyOfToCcHeaderMemberOf.IsNullOrEmpty()) ? 1 : 0);
			num3 += ((rule.ExceptIfMessageTypeMatches != null) ? 1 : 0);
			num3 += ((rule.ExceptIfSenderManagementRelationship != null) ? 1 : 0);
			num3 += ((rule.ExceptIfWithImportance != null) ? 1 : 0);
			num3 += ((!rule.ExceptIfRecipientInSenderList.IsNullOrEmpty()) ? 1 : 0);
			num3 += ((!rule.ExceptIfSenderInRecipientList.IsNullOrEmpty()) ? 1 : 0);
			num3 += ((rule.ExceptIfHeaderContainsMessageHeader != null && !rule.ExceptIfHeaderContainsWords.IsNullOrEmpty()) ? 1 : 0);
			num3 += ((!rule.ExceptIfBetweenMemberOf1.IsNullOrEmpty() && !rule.ExceptIfBetweenMemberOf2.IsNullOrEmpty()) ? 1 : 0);
			num3 += ((rule.ExceptIfHeaderMatchesMessageHeader != null && !rule.ExceptIfHeaderMatchesPatterns.IsNullOrEmpty()) ? 1 : 0);
			num3 += ((rule.ExceptIfManagerForEvaluatedUser != null && !rule.ExceptIfManagerAddresses.IsNullOrEmpty()) ? 1 : 0);
			num3 += ((rule.ExceptIfADComparisonAttribute != null && rule.ExceptIfADComparisonOperator != null) ? 1 : 0);
			num3 += ((rule.ExceptIfAttachmentSizeOver != null) ? 1 : 0);
			num3 += ((rule.ExceptIfSCLOver != null) ? 1 : 0);
			num3 += ((rule.ExceptIfHasClassification != null) ? 1 : 0);
			num3 += ((!rule.ExceptIfSenderADAttributeContainsWords.IsNullOrEmpty()) ? 1 : 0);
			num3 += ((!rule.ExceptIfSenderADAttributeMatchesPatterns.IsNullOrEmpty()) ? 1 : 0);
			num3 += ((!rule.ExceptIfRecipientADAttributeContainsWords.IsNullOrEmpty()) ? 1 : 0);
			num3 += ((!rule.ExceptIfRecipientADAttributeMatchesPatterns.IsNullOrEmpty()) ? 1 : 0);
			num3 += ((rule.ExceptIfMessageSizeOver != null) ? 1 : 0);
			num3 += ((!rule.ExceptIfMessageContainsDataClassifications.IsNullOrEmpty()) ? 1 : 0);
			num3 += ((!rule.ExceptIfAttachmentExtensionMatchesWords.IsNullOrEmpty()) ? 1 : 0);
			num3 += (rule.ExceptIfHasSenderOverride ? 1 : 0);
			num3 += (rule.ExceptIfAttachmentHasExecutableContent ? 1 : 0);
			num3 += ((rule.ExceptIfSenderIpRanges != null) ? 1 : 0);
			num3 += (rule.ExceptIfAttachmentProcessingLimitExceeded ? 1 : 0);
			num3 += ((!rule.ExceptIfSenderDomainIs.IsNullOrEmpty()) ? 1 : 0);
			num3 += ((!rule.ExceptIfRecipientDomainIs.IsNullOrEmpty()) ? 1 : 0);
			num3 += ((!rule.ExceptIfContentCharacterSetContainsWords.IsNullOrEmpty()) ? 1 : 0);
			num3 += (rule.ExceptIfAttachmentIsPasswordProtected ? 1 : 0);
			num3 += ((!rule.ExceptIfAnyOfRecipientAddressMatchesPatterns.IsNullOrEmpty()) ? 1 : 0);
			num3 += ((!rule.ExceptIfAnyOfRecipientAddressContainsWords.IsNullOrEmpty()) ? 1 : 0);
			this.Supported = (num2 > 0 && !rule.Name.StartsWith("__", StringComparison.Ordinal) && num == (rule.Conditions.IsNullOrEmpty() ? 0 : rule.Conditions.Length) && num2 == (rule.Actions.IsNullOrEmpty() ? 0 : rule.Actions.Length) && num3 == (rule.Exceptions.IsNullOrEmpty() ? 0 : rule.Exceptions.Length));
		}

		public RuleRow(InboxRule rule) : base(((InboxRuleId)rule.Identity).ToIdentity(), rule)
		{
			this.Name = rule.Name;
			this.Priority = rule.Priority;
			this.Enabled = rule.Enabled;
			this.CaptionText = Strings.EditRuleCaption(rule.Name);
			this.Supported = rule.SupportedByTask;
		}

		public RuleRow(JournalRuleObject rule) : base(rule)
		{
			this.Name = rule.Name;
			this.Enabled = rule.Enabled;
			this.CaptionText = Strings.EditRuleCaption(rule.Name);
		}

		public RuleRow(UMCallAnsweringRule rule) : base(((UMCallAnsweringRuleId)rule.Identity).ToIdentity(), rule)
		{
			this.Name = rule.Name;
			this.Enabled = rule.Enabled;
			this.Priority = rule.Priority;
			this.CaptionText = Strings.EditRuleCaption(rule.Name);
			this.Supported = (rule.Validate().Length == 0);
		}

		[DataMember]
		public string CaptionText { get; protected set; }

		[DataMember]
		public string Name { get; protected set; }

		[DataMember]
		public int Priority { get; protected set; }

		[DataMember]
		public bool Enabled { get; protected set; }

		public string[] ConditionDescriptions { get; protected set; }

		public RuleDescription DescriptionObject
		{
			get
			{
				return this.descriptionObject;
			}
			protected set
			{
				this.descriptionObject = value;
			}
		}

		public string[] ActionDescriptions { get; protected set; }

		public string[] ExceptionDescriptions { get; protected set; }

		public string ActivationDateDescription { get; protected set; }

		public string ExpiryDateDescription { get; protected set; }

		[DataMember]
		public bool Supported { get; protected set; }

		[DataMember]
		public string DlpPolicy { get; private set; }

		[DataMember]
		public string RuleMode { get; private set; }

		[DataMember]
		public Version RuleVersion { get; private set; }

		private RuleDescription descriptionObject;
	}
}
