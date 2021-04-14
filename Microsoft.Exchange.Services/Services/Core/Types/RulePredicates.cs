using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.DataConverter;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "RulePredicatesType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	public class RulePredicates
	{
		[XmlArrayItem("String", Type = typeof(string))]
		[XmlArray(Order = 0)]
		public string[] Categories { get; set; }

		[XmlArray(Order = 1)]
		[XmlArrayItem("String", Type = typeof(string))]
		public string[] ContainsBodyStrings { get; set; }

		[XmlArrayItem("String", Type = typeof(string))]
		[XmlArray(Order = 2)]
		public string[] ContainsHeaderStrings { get; set; }

		[XmlArrayItem("String", Type = typeof(string))]
		[XmlArray(Order = 3)]
		public string[] ContainsRecipientStrings { get; set; }

		[XmlArrayItem("String", Type = typeof(string))]
		[XmlArray(Order = 4)]
		public string[] ContainsSenderStrings { get; set; }

		[XmlArray(Order = 5)]
		[XmlArrayItem("String", Type = typeof(string))]
		public string[] ContainsSubjectOrBodyStrings { get; set; }

		[XmlArray(Order = 6)]
		[XmlArrayItem("String", Type = typeof(string))]
		public string[] ContainsSubjectStrings { get; set; }

		[XmlElement(Order = 7)]
		public FlaggedForAction FlaggedForAction { get; set; }

		[XmlIgnore]
		public bool FlaggedForActionSpecified { get; set; }

		[XmlArray(Order = 8)]
		[XmlArrayItem("Address", Type = typeof(EmailAddressWrapper))]
		public EmailAddressWrapper[] FromAddresses { get; set; }

		[XmlArray(Order = 9)]
		[XmlArrayItem("String", Type = typeof(string))]
		public string[] FromConnectedAccounts { get; set; }

		[XmlElement(Order = 10)]
		public bool HasAttachments { get; set; }

		[XmlIgnore]
		public bool HasAttachmentsSpecified { get; set; }

		[XmlElement(Order = 11)]
		public ImportanceType Importance { get; set; }

		[XmlIgnore]
		public bool ImportanceSpecified { get; set; }

		[XmlElement(Order = 12)]
		public bool IsApprovalRequest { get; set; }

		[XmlIgnore]
		public bool IsApprovalRequestSpecified { get; set; }

		[XmlElement(Order = 13)]
		public bool IsAutomaticForward { get; set; }

		[XmlIgnore]
		public bool IsAutomaticForwardSpecified { get; set; }

		[XmlElement(Order = 14)]
		public bool IsAutomaticReply { get; set; }

		[XmlIgnore]
		public bool IsAutomaticReplySpecified { get; set; }

		[XmlElement(Order = 15)]
		public bool IsEncrypted { get; set; }

		[XmlIgnore]
		public bool IsEncryptedSpecified { get; set; }

		[XmlElement(Order = 16)]
		public bool IsMeetingRequest { get; set; }

		[XmlIgnore]
		public bool IsMeetingRequestSpecified { get; set; }

		[XmlElement(Order = 17)]
		public bool IsMeetingResponse { get; set; }

		[XmlIgnore]
		public bool IsMeetingResponseSpecified { get; set; }

		[XmlElement(Order = 18)]
		public bool IsNDR { get; set; }

		[XmlIgnore]
		public bool IsNDRSpecified { get; set; }

		[XmlElement(Order = 19)]
		public bool IsPermissionControlled { get; set; }

		[XmlIgnore]
		public bool IsPermissionControlledSpecified { get; set; }

		[XmlElement(Order = 20)]
		public bool IsReadReceipt { get; set; }

		[XmlIgnore]
		public bool IsReadReceiptSpecified { get; set; }

		[XmlElement(Order = 21)]
		public bool IsSigned { get; set; }

		[XmlIgnore]
		public bool IsSignedSpecified { get; set; }

		[XmlElement(Order = 22)]
		public bool IsVoicemail { get; set; }

		[XmlIgnore]
		public bool IsVoicemailSpecified { get; set; }

		[XmlArrayItem("String", Type = typeof(string))]
		[XmlArray(Order = 23)]
		public string[] ItemClasses { get; set; }

		[XmlArray(Order = 24)]
		[XmlArrayItem("String", Type = typeof(string))]
		public string[] MessageClassifications { get; set; }

		[XmlElement(Order = 25)]
		public bool NotSentToMe { get; set; }

		[XmlIgnore]
		public bool NotSentToMeSpecified { get; set; }

		[XmlElement(Order = 26)]
		public bool SentCcMe { get; set; }

		[XmlIgnore]
		public bool SentCcMeSpecified { get; set; }

		[XmlElement(Order = 27)]
		public bool SentOnlyToMe { get; set; }

		[XmlIgnore]
		public bool SentOnlyToMeSpecified { get; set; }

		[XmlArrayItem("Address", Type = typeof(EmailAddressWrapper))]
		[XmlArray(Order = 28)]
		public EmailAddressWrapper[] SentToAddresses { get; set; }

		[XmlElement(Order = 29)]
		public bool SentToMe { get; set; }

		[XmlIgnore]
		public bool SentToMeSpecified { get; set; }

		[XmlElement(Order = 30)]
		public bool SentToOrCcMe { get; set; }

		[XmlIgnore]
		public bool SentToOrCcMeSpecified { get; set; }

		[XmlElement(Order = 31)]
		public SensitivityType Sensitivity { get; set; }

		[XmlIgnore]
		public bool SensitivitySpecified { get; set; }

		[XmlElement(Order = 32)]
		public RulePredicateDateRange WithinDateRange { get; set; }

		[XmlElement(Order = 33)]
		public RulePredicateSizeRange WithinSizeRange { get; set; }

		[XmlIgnore]
		internal EwsRule Rule { get; set; }

		public RulePredicates()
		{
		}

		private RulePredicates(EwsRule rule)
		{
			this.Rule = rule;
		}

		internal static RulePredicates CreateFromServerRuleConditions(IList<Condition> serverConditions, EwsRule rule, int hashCode, CultureInfo clientCulture)
		{
			RulePredicates rulePredicates = new RulePredicates(rule);
			foreach (Condition condition in serverConditions)
			{
				switch (condition.ConditionType)
				{
				case ConditionType.FromRecipientsCondition:
					rulePredicates.FromAddresses = ParticipantsAddressesConverter.ToAddresses(((FromRecipientsCondition)condition).Participants);
					continue;
				case ConditionType.ContainsSubjectStringCondition:
					rulePredicates.ContainsSubjectStrings = ((ContainsSubjectStringCondition)condition).Text;
					continue;
				case ConditionType.SentOnlyToMeCondition:
					rulePredicates.SentOnlyToMe = true;
					rulePredicates.SentOnlyToMeSpecified = true;
					continue;
				case ConditionType.SentToMeCondition:
					rulePredicates.SentToMe = true;
					rulePredicates.SentToMeSpecified = true;
					continue;
				case ConditionType.MarkedAsImportanceCondition:
					rulePredicates.Importance = (ImportanceType)((MarkedAsImportanceCondition)condition).Importance;
					rulePredicates.ImportanceSpecified = true;
					continue;
				case ConditionType.MarkedAsSensitivityCondition:
					rulePredicates.Sensitivity = (SensitivityType)((MarkedAsSensitivityCondition)condition).Sensitivity;
					rulePredicates.SensitivitySpecified = true;
					continue;
				case ConditionType.SentCcMeCondition:
					rulePredicates.SentCcMe = true;
					rulePredicates.SentCcMeSpecified = true;
					continue;
				case ConditionType.SentToOrCcMeCondition:
					rulePredicates.SentToOrCcMe = true;
					rulePredicates.SentToOrCcMeSpecified = true;
					continue;
				case ConditionType.NotSentToMeCondition:
					rulePredicates.NotSentToMe = true;
					rulePredicates.NotSentToMeSpecified = true;
					continue;
				case ConditionType.SentToRecipientsCondition:
					rulePredicates.SentToAddresses = ParticipantsAddressesConverter.ToAddresses(((SentToRecipientsCondition)condition).Participants);
					continue;
				case ConditionType.ContainsBodyStringCondition:
					rulePredicates.ContainsBodyStrings = ((ContainsBodyStringCondition)condition).Text;
					continue;
				case ConditionType.ContainsSubjectOrBodyStringCondition:
					rulePredicates.ContainsSubjectOrBodyStrings = ((ContainsSubjectOrBodyStringCondition)condition).Text;
					continue;
				case ConditionType.ContainsHeaderStringCondition:
					rulePredicates.ContainsHeaderStrings = ((ContainsHeaderStringCondition)condition).Text;
					continue;
				case ConditionType.ContainsSenderStringCondition:
					rulePredicates.ContainsSenderStrings = ((ContainsSenderStringCondition)condition).Text;
					continue;
				case ConditionType.MarkedAsOofCondition:
					rulePredicates.IsAutomaticReply = true;
					rulePredicates.IsAutomaticReplySpecified = true;
					continue;
				case ConditionType.HasAttachmentCondition:
					rulePredicates.HasAttachments = true;
					rulePredicates.HasAttachmentsSpecified = true;
					continue;
				case ConditionType.WithinSizeRangeCondition:
				{
					WithinSizeRangeCondition withinSizeRangeCondition = (WithinSizeRangeCondition)condition;
					rulePredicates.WithinSizeRange = new RulePredicateSizeRange(withinSizeRangeCondition.RangeLow, withinSizeRangeCondition.RangeHigh);
					continue;
				}
				case ConditionType.WithinDateRangeCondition:
				{
					WithinDateRangeCondition withinDateRangeCondition = (WithinDateRangeCondition)condition;
					rulePredicates.WithinDateRange = new RulePredicateDateRange(withinDateRangeCondition.RangeLow, withinDateRangeCondition.RangeHigh);
					continue;
				}
				case ConditionType.MeetingMessageCondition:
					rulePredicates.IsMeetingRequest = true;
					rulePredicates.IsMeetingRequestSpecified = true;
					continue;
				case ConditionType.ContainsRecipientStringCondition:
					rulePredicates.ContainsRecipientStrings = ((ContainsRecipientStringCondition)condition).Text;
					continue;
				case ConditionType.AssignedCategoriesCondition:
					rulePredicates.Categories = ((AssignedCategoriesCondition)condition).Text;
					continue;
				case ConditionType.FormsCondition:
					rulePredicates.ItemClasses = ((FormsCondition)condition).Text;
					continue;
				case ConditionType.MessageClassificationCondition:
					rulePredicates.MessageClassifications = ((MessageClassificationCondition)condition).Text;
					continue;
				case ConditionType.NdrCondition:
					rulePredicates.IsNDR = true;
					rulePredicates.IsNDRSpecified = true;
					continue;
				case ConditionType.AutomaticForwardCondition:
					rulePredicates.IsAutomaticForward = true;
					rulePredicates.IsAutomaticForwardSpecified = true;
					continue;
				case ConditionType.EncryptedCondition:
					rulePredicates.IsEncrypted = true;
					rulePredicates.IsEncryptedSpecified = true;
					continue;
				case ConditionType.SignedCondition:
					rulePredicates.IsSigned = true;
					rulePredicates.IsSignedSpecified = true;
					continue;
				case ConditionType.ReadReceiptCondition:
					rulePredicates.IsReadReceipt = true;
					rulePredicates.IsReadReceiptSpecified = true;
					continue;
				case ConditionType.MeetingResponseCondition:
					rulePredicates.IsMeetingResponse = true;
					rulePredicates.IsMeetingResponseSpecified = true;
					continue;
				case ConditionType.PermissionControlledCondition:
					rulePredicates.IsPermissionControlled = true;
					rulePredicates.IsPermissionControlledSpecified = true;
					continue;
				case ConditionType.ApprovalRequestCondition:
					rulePredicates.IsApprovalRequest = true;
					rulePredicates.IsApprovalRequestSpecified = true;
					continue;
				case ConditionType.VoicemailCondition:
					rulePredicates.IsVoicemail = true;
					rulePredicates.IsVoicemailSpecified = true;
					continue;
				case ConditionType.FlaggedForActionCondition:
				{
					string action = ((FlaggedForActionCondition)condition).Action;
					if (string.Equals(action, RequestedAction.Any.ToString(), StringComparison.OrdinalIgnoreCase))
					{
						rulePredicates.FlaggedForAction = FlaggedForAction.Any;
						rulePredicates.FlaggedForActionSpecified = true;
						continue;
					}
					int num = (clientCulture != null) ? FlaggedForActionCondition.GetRequestedActionLocalizedStringEnumIndex(action, clientCulture) : FlaggedForActionCondition.GetRequestedActionLocalizedStringEnumIndex(action);
					if (num >= 0)
					{
						rulePredicates.FlaggedForAction = (FlaggedForAction)num;
						rulePredicates.FlaggedForActionSpecified = true;
						continue;
					}
					ExTraceGlobals.GetInboxRulesCallTracer.TraceError<ConditionType>((long)hashCode, "UnsupportedPredicateType={0};", condition.ConditionType);
					rule.IsNotSupported = true;
					return null;
				}
				case ConditionType.FromSubscriptionCondition:
				{
					Guid[] guids = ((FromSubscriptionCondition)condition).Guids;
					string[] array = new string[guids.Length];
					for (int i = 0; i < guids.Length; i++)
					{
						array[i] = GuidConverter.ToString(guids[i]);
					}
					rulePredicates.FromConnectedAccounts = array;
					continue;
				}
				}
				ExTraceGlobals.GetInboxRulesCallTracer.TraceError<ConditionType>((long)hashCode, "UnsupportedPredicateType={0};", condition.ConditionType);
				rule.IsNotSupported = true;
				return null;
			}
			return rulePredicates;
		}

		internal bool SpecifiedPredicates()
		{
			return (this.Categories != null && 0 < this.Categories.Length) || (this.ContainsBodyStrings != null && 0 < this.ContainsBodyStrings.Length) || (this.ContainsHeaderStrings != null && 0 < this.ContainsHeaderStrings.Length) || (this.ContainsRecipientStrings != null && 0 < this.ContainsRecipientStrings.Length) || (this.ContainsSenderStrings != null && 0 < this.ContainsSenderStrings.Length) || (this.ContainsSubjectOrBodyStrings != null && 0 < this.ContainsSubjectOrBodyStrings.Length) || (this.ContainsSubjectStrings != null && 0 < this.ContainsSubjectStrings.Length) || this.FlaggedForActionSpecified || (this.FromAddresses != null && 0 < this.FromAddresses.Length) || (this.FromConnectedAccounts != null && 0 < this.FromConnectedAccounts.Length) || this.HasAttachmentsSpecified || this.ImportanceSpecified || this.IsApprovalRequestSpecified || this.IsAutomaticForwardSpecified || this.IsAutomaticReplySpecified || this.IsEncryptedSpecified || this.IsMeetingRequestSpecified || this.IsMeetingResponseSpecified || this.IsNDRSpecified || this.IsPermissionControlledSpecified || this.IsReadReceiptSpecified || this.IsSignedSpecified || this.IsVoicemailSpecified || (this.ItemClasses != null && 0 < this.ItemClasses.Length) || (this.MessageClassifications != null && 0 < this.MessageClassifications.Length) || this.NotSentToMeSpecified || this.SentCcMeSpecified || this.SentOnlyToMeSpecified || (this.SentToAddresses != null && 0 < this.SentToAddresses.Length) || this.SentToMeSpecified || this.SentToOrCcMeSpecified || this.SensitivitySpecified || this.WithinDateRange != null || this.WithinSizeRange != null;
		}
	}
}
