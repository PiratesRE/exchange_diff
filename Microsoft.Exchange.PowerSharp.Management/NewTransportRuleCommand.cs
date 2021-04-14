using System;
using System.Collections;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.MessagingPolicies.Rules;
using Microsoft.Exchange.MessagingPolicies.Rules.Tasks;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class NewTransportRuleCommand : SyntheticCommandWithPipelineInput<TransportRule, TransportRule>
	{
		private NewTransportRuleCommand() : base("New-TransportRule")
		{
		}

		public NewTransportRuleCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual NewTransportRuleCommand SetParameters(NewTransportRuleCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
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

			public virtual string Comments
			{
				set
				{
					base.PowerSharpParameters["Comments"] = value;
				}
			}

			public virtual bool UseLegacyRegex
			{
				set
				{
					base.PowerSharpParameters["UseLegacyRegex"] = value;
				}
			}

			public virtual string DlpPolicy
			{
				set
				{
					base.PowerSharpParameters["DlpPolicy"] = value;
				}
			}

			public virtual bool Enabled
			{
				set
				{
					base.PowerSharpParameters["Enabled"] = value;
				}
			}

			public virtual RuleMode Mode
			{
				set
				{
					base.PowerSharpParameters["Mode"] = value;
				}
			}

			public virtual RecipientIdParameter From
			{
				set
				{
					base.PowerSharpParameters["From"] = value;
				}
			}

			public virtual RecipientIdParameter FromMemberOf
			{
				set
				{
					base.PowerSharpParameters["FromMemberOf"] = value;
				}
			}

			public virtual FromUserScope FromScope
			{
				set
				{
					base.PowerSharpParameters["FromScope"] = value;
				}
			}

			public virtual RecipientIdParameter SentTo
			{
				set
				{
					base.PowerSharpParameters["SentTo"] = value;
				}
			}

			public virtual RecipientIdParameter SentToMemberOf
			{
				set
				{
					base.PowerSharpParameters["SentToMemberOf"] = value;
				}
			}

			public virtual ToUserScope SentToScope
			{
				set
				{
					base.PowerSharpParameters["SentToScope"] = value;
				}
			}

			public virtual RecipientIdParameter BetweenMemberOf1
			{
				set
				{
					base.PowerSharpParameters["BetweenMemberOf1"] = value;
				}
			}

			public virtual RecipientIdParameter BetweenMemberOf2
			{
				set
				{
					base.PowerSharpParameters["BetweenMemberOf2"] = value;
				}
			}

			public virtual RecipientIdParameter ManagerAddresses
			{
				set
				{
					base.PowerSharpParameters["ManagerAddresses"] = value;
				}
			}

			public virtual EvaluatedUser ManagerForEvaluatedUser
			{
				set
				{
					base.PowerSharpParameters["ManagerForEvaluatedUser"] = value;
				}
			}

			public virtual ManagementRelationship SenderManagementRelationship
			{
				set
				{
					base.PowerSharpParameters["SenderManagementRelationship"] = value;
				}
			}

			public virtual ADAttribute ADComparisonAttribute
			{
				set
				{
					base.PowerSharpParameters["ADComparisonAttribute"] = value;
				}
			}

			public virtual Evaluation ADComparisonOperator
			{
				set
				{
					base.PowerSharpParameters["ADComparisonOperator"] = value;
				}
			}

			public virtual Word SenderADAttributeContainsWords
			{
				set
				{
					base.PowerSharpParameters["SenderADAttributeContainsWords"] = value;
				}
			}

			public virtual Pattern SenderADAttributeMatchesPatterns
			{
				set
				{
					base.PowerSharpParameters["SenderADAttributeMatchesPatterns"] = value;
				}
			}

			public virtual Word RecipientADAttributeContainsWords
			{
				set
				{
					base.PowerSharpParameters["RecipientADAttributeContainsWords"] = value;
				}
			}

			public virtual Pattern RecipientADAttributeMatchesPatterns
			{
				set
				{
					base.PowerSharpParameters["RecipientADAttributeMatchesPatterns"] = value;
				}
			}

			public virtual RecipientIdParameter AnyOfToHeader
			{
				set
				{
					base.PowerSharpParameters["AnyOfToHeader"] = value;
				}
			}

			public virtual RecipientIdParameter AnyOfToHeaderMemberOf
			{
				set
				{
					base.PowerSharpParameters["AnyOfToHeaderMemberOf"] = value;
				}
			}

			public virtual RecipientIdParameter AnyOfCcHeader
			{
				set
				{
					base.PowerSharpParameters["AnyOfCcHeader"] = value;
				}
			}

			public virtual RecipientIdParameter AnyOfCcHeaderMemberOf
			{
				set
				{
					base.PowerSharpParameters["AnyOfCcHeaderMemberOf"] = value;
				}
			}

			public virtual RecipientIdParameter AnyOfToCcHeader
			{
				set
				{
					base.PowerSharpParameters["AnyOfToCcHeader"] = value;
				}
			}

			public virtual RecipientIdParameter AnyOfToCcHeaderMemberOf
			{
				set
				{
					base.PowerSharpParameters["AnyOfToCcHeaderMemberOf"] = value;
				}
			}

			public virtual string HasClassification
			{
				set
				{
					base.PowerSharpParameters["HasClassification"] = value;
				}
			}

			public virtual bool HasNoClassification
			{
				set
				{
					base.PowerSharpParameters["HasNoClassification"] = value;
				}
			}

			public virtual Word SubjectContainsWords
			{
				set
				{
					base.PowerSharpParameters["SubjectContainsWords"] = value;
				}
			}

			public virtual Word SubjectOrBodyContainsWords
			{
				set
				{
					base.PowerSharpParameters["SubjectOrBodyContainsWords"] = value;
				}
			}

			public virtual HeaderName HeaderContainsMessageHeader
			{
				set
				{
					base.PowerSharpParameters["HeaderContainsMessageHeader"] = value;
				}
			}

			public virtual Word HeaderContainsWords
			{
				set
				{
					base.PowerSharpParameters["HeaderContainsWords"] = value;
				}
			}

			public virtual Word FromAddressContainsWords
			{
				set
				{
					base.PowerSharpParameters["FromAddressContainsWords"] = value;
				}
			}

			public virtual Word SenderDomainIs
			{
				set
				{
					base.PowerSharpParameters["SenderDomainIs"] = value;
				}
			}

			public virtual Word RecipientDomainIs
			{
				set
				{
					base.PowerSharpParameters["RecipientDomainIs"] = value;
				}
			}

			public virtual Pattern SubjectMatchesPatterns
			{
				set
				{
					base.PowerSharpParameters["SubjectMatchesPatterns"] = value;
				}
			}

			public virtual Pattern SubjectOrBodyMatchesPatterns
			{
				set
				{
					base.PowerSharpParameters["SubjectOrBodyMatchesPatterns"] = value;
				}
			}

			public virtual HeaderName HeaderMatchesMessageHeader
			{
				set
				{
					base.PowerSharpParameters["HeaderMatchesMessageHeader"] = value;
				}
			}

			public virtual Pattern HeaderMatchesPatterns
			{
				set
				{
					base.PowerSharpParameters["HeaderMatchesPatterns"] = value;
				}
			}

			public virtual Pattern FromAddressMatchesPatterns
			{
				set
				{
					base.PowerSharpParameters["FromAddressMatchesPatterns"] = value;
				}
			}

			public virtual Pattern AttachmentNameMatchesPatterns
			{
				set
				{
					base.PowerSharpParameters["AttachmentNameMatchesPatterns"] = value;
				}
			}

			public virtual Word AttachmentExtensionMatchesWords
			{
				set
				{
					base.PowerSharpParameters["AttachmentExtensionMatchesWords"] = value;
				}
			}

			public virtual Word AttachmentPropertyContainsWords
			{
				set
				{
					base.PowerSharpParameters["AttachmentPropertyContainsWords"] = value;
				}
			}

			public virtual Word ContentCharacterSetContainsWords
			{
				set
				{
					base.PowerSharpParameters["ContentCharacterSetContainsWords"] = value;
				}
			}

			public virtual SclValue SCLOver
			{
				set
				{
					base.PowerSharpParameters["SCLOver"] = value;
				}
			}

			public virtual ByteQuantifiedSize AttachmentSizeOver
			{
				set
				{
					base.PowerSharpParameters["AttachmentSizeOver"] = value;
				}
			}

			public virtual ByteQuantifiedSize MessageSizeOver
			{
				set
				{
					base.PowerSharpParameters["MessageSizeOver"] = value;
				}
			}

			public virtual Importance WithImportance
			{
				set
				{
					base.PowerSharpParameters["WithImportance"] = value;
				}
			}

			public virtual MessageType MessageTypeMatches
			{
				set
				{
					base.PowerSharpParameters["MessageTypeMatches"] = value;
				}
			}

			public virtual Word RecipientAddressContainsWords
			{
				set
				{
					base.PowerSharpParameters["RecipientAddressContainsWords"] = value;
				}
			}

			public virtual Pattern RecipientAddressMatchesPatterns
			{
				set
				{
					base.PowerSharpParameters["RecipientAddressMatchesPatterns"] = value;
				}
			}

			public virtual Word SenderInRecipientList
			{
				set
				{
					base.PowerSharpParameters["SenderInRecipientList"] = value;
				}
			}

			public virtual Word RecipientInSenderList
			{
				set
				{
					base.PowerSharpParameters["RecipientInSenderList"] = value;
				}
			}

			public virtual Word AttachmentContainsWords
			{
				set
				{
					base.PowerSharpParameters["AttachmentContainsWords"] = value;
				}
			}

			public virtual Pattern AttachmentMatchesPatterns
			{
				set
				{
					base.PowerSharpParameters["AttachmentMatchesPatterns"] = value;
				}
			}

			public virtual bool AttachmentIsUnsupported
			{
				set
				{
					base.PowerSharpParameters["AttachmentIsUnsupported"] = value;
				}
			}

			public virtual bool AttachmentProcessingLimitExceeded
			{
				set
				{
					base.PowerSharpParameters["AttachmentProcessingLimitExceeded"] = value;
				}
			}

			public virtual bool AttachmentHasExecutableContent
			{
				set
				{
					base.PowerSharpParameters["AttachmentHasExecutableContent"] = value;
				}
			}

			public virtual bool AttachmentIsPasswordProtected
			{
				set
				{
					base.PowerSharpParameters["AttachmentIsPasswordProtected"] = value;
				}
			}

			public virtual Word AnyOfRecipientAddressContainsWords
			{
				set
				{
					base.PowerSharpParameters["AnyOfRecipientAddressContainsWords"] = value;
				}
			}

			public virtual Pattern AnyOfRecipientAddressMatchesPatterns
			{
				set
				{
					base.PowerSharpParameters["AnyOfRecipientAddressMatchesPatterns"] = value;
				}
			}

			public virtual bool HasSenderOverride
			{
				set
				{
					base.PowerSharpParameters["HasSenderOverride"] = value;
				}
			}

			public virtual bool ExceptIfHasSenderOverride
			{
				set
				{
					base.PowerSharpParameters["ExceptIfHasSenderOverride"] = value;
				}
			}

			public virtual RecipientIdParameter ExceptIfFrom
			{
				set
				{
					base.PowerSharpParameters["ExceptIfFrom"] = value;
				}
			}

			public virtual Hashtable MessageContainsDataClassifications
			{
				set
				{
					base.PowerSharpParameters["MessageContainsDataClassifications"] = value;
				}
			}

			public virtual MultiValuedProperty<IPRange> SenderIpRanges
			{
				set
				{
					base.PowerSharpParameters["SenderIpRanges"] = value;
				}
			}

			public virtual RecipientIdParameter ExceptIfFromMemberOf
			{
				set
				{
					base.PowerSharpParameters["ExceptIfFromMemberOf"] = value;
				}
			}

			public virtual FromUserScope ExceptIfFromScope
			{
				set
				{
					base.PowerSharpParameters["ExceptIfFromScope"] = value;
				}
			}

			public virtual RecipientIdParameter ExceptIfSentTo
			{
				set
				{
					base.PowerSharpParameters["ExceptIfSentTo"] = value;
				}
			}

			public virtual RecipientIdParameter ExceptIfSentToMemberOf
			{
				set
				{
					base.PowerSharpParameters["ExceptIfSentToMemberOf"] = value;
				}
			}

			public virtual ToUserScope ExceptIfSentToScope
			{
				set
				{
					base.PowerSharpParameters["ExceptIfSentToScope"] = value;
				}
			}

			public virtual RecipientIdParameter ExceptIfBetweenMemberOf1
			{
				set
				{
					base.PowerSharpParameters["ExceptIfBetweenMemberOf1"] = value;
				}
			}

			public virtual RecipientIdParameter ExceptIfBetweenMemberOf2
			{
				set
				{
					base.PowerSharpParameters["ExceptIfBetweenMemberOf2"] = value;
				}
			}

			public virtual RecipientIdParameter ExceptIfManagerAddresses
			{
				set
				{
					base.PowerSharpParameters["ExceptIfManagerAddresses"] = value;
				}
			}

			public virtual EvaluatedUser ExceptIfManagerForEvaluatedUser
			{
				set
				{
					base.PowerSharpParameters["ExceptIfManagerForEvaluatedUser"] = value;
				}
			}

			public virtual ManagementRelationship ExceptIfSenderManagementRelationship
			{
				set
				{
					base.PowerSharpParameters["ExceptIfSenderManagementRelationship"] = value;
				}
			}

			public virtual ADAttribute ExceptIfADComparisonAttribute
			{
				set
				{
					base.PowerSharpParameters["ExceptIfADComparisonAttribute"] = value;
				}
			}

			public virtual Evaluation ExceptIfADComparisonOperator
			{
				set
				{
					base.PowerSharpParameters["ExceptIfADComparisonOperator"] = value;
				}
			}

			public virtual Word ExceptIfSenderADAttributeContainsWords
			{
				set
				{
					base.PowerSharpParameters["ExceptIfSenderADAttributeContainsWords"] = value;
				}
			}

			public virtual Pattern ExceptIfSenderADAttributeMatchesPatterns
			{
				set
				{
					base.PowerSharpParameters["ExceptIfSenderADAttributeMatchesPatterns"] = value;
				}
			}

			public virtual Word ExceptIfRecipientADAttributeContainsWords
			{
				set
				{
					base.PowerSharpParameters["ExceptIfRecipientADAttributeContainsWords"] = value;
				}
			}

			public virtual Pattern ExceptIfRecipientADAttributeMatchesPatterns
			{
				set
				{
					base.PowerSharpParameters["ExceptIfRecipientADAttributeMatchesPatterns"] = value;
				}
			}

			public virtual RecipientIdParameter ExceptIfAnyOfToHeader
			{
				set
				{
					base.PowerSharpParameters["ExceptIfAnyOfToHeader"] = value;
				}
			}

			public virtual RecipientIdParameter ExceptIfAnyOfToHeaderMemberOf
			{
				set
				{
					base.PowerSharpParameters["ExceptIfAnyOfToHeaderMemberOf"] = value;
				}
			}

			public virtual RecipientIdParameter ExceptIfAnyOfCcHeader
			{
				set
				{
					base.PowerSharpParameters["ExceptIfAnyOfCcHeader"] = value;
				}
			}

			public virtual RecipientIdParameter ExceptIfAnyOfCcHeaderMemberOf
			{
				set
				{
					base.PowerSharpParameters["ExceptIfAnyOfCcHeaderMemberOf"] = value;
				}
			}

			public virtual RecipientIdParameter ExceptIfAnyOfToCcHeader
			{
				set
				{
					base.PowerSharpParameters["ExceptIfAnyOfToCcHeader"] = value;
				}
			}

			public virtual RecipientIdParameter ExceptIfAnyOfToCcHeaderMemberOf
			{
				set
				{
					base.PowerSharpParameters["ExceptIfAnyOfToCcHeaderMemberOf"] = value;
				}
			}

			public virtual string ExceptIfHasClassification
			{
				set
				{
					base.PowerSharpParameters["ExceptIfHasClassification"] = value;
				}
			}

			public virtual bool ExceptIfHasNoClassification
			{
				set
				{
					base.PowerSharpParameters["ExceptIfHasNoClassification"] = value;
				}
			}

			public virtual Word ExceptIfSubjectContainsWords
			{
				set
				{
					base.PowerSharpParameters["ExceptIfSubjectContainsWords"] = value;
				}
			}

			public virtual Word ExceptIfSubjectOrBodyContainsWords
			{
				set
				{
					base.PowerSharpParameters["ExceptIfSubjectOrBodyContainsWords"] = value;
				}
			}

			public virtual HeaderName ExceptIfHeaderContainsMessageHeader
			{
				set
				{
					base.PowerSharpParameters["ExceptIfHeaderContainsMessageHeader"] = value;
				}
			}

			public virtual Word ExceptIfHeaderContainsWords
			{
				set
				{
					base.PowerSharpParameters["ExceptIfHeaderContainsWords"] = value;
				}
			}

			public virtual Word ExceptIfFromAddressContainsWords
			{
				set
				{
					base.PowerSharpParameters["ExceptIfFromAddressContainsWords"] = value;
				}
			}

			public virtual Word ExceptIfSenderDomainIs
			{
				set
				{
					base.PowerSharpParameters["ExceptIfSenderDomainIs"] = value;
				}
			}

			public virtual Word ExceptIfRecipientDomainIs
			{
				set
				{
					base.PowerSharpParameters["ExceptIfRecipientDomainIs"] = value;
				}
			}

			public virtual Pattern ExceptIfSubjectMatchesPatterns
			{
				set
				{
					base.PowerSharpParameters["ExceptIfSubjectMatchesPatterns"] = value;
				}
			}

			public virtual Pattern ExceptIfSubjectOrBodyMatchesPatterns
			{
				set
				{
					base.PowerSharpParameters["ExceptIfSubjectOrBodyMatchesPatterns"] = value;
				}
			}

			public virtual HeaderName ExceptIfHeaderMatchesMessageHeader
			{
				set
				{
					base.PowerSharpParameters["ExceptIfHeaderMatchesMessageHeader"] = value;
				}
			}

			public virtual Pattern ExceptIfHeaderMatchesPatterns
			{
				set
				{
					base.PowerSharpParameters["ExceptIfHeaderMatchesPatterns"] = value;
				}
			}

			public virtual Pattern ExceptIfFromAddressMatchesPatterns
			{
				set
				{
					base.PowerSharpParameters["ExceptIfFromAddressMatchesPatterns"] = value;
				}
			}

			public virtual Pattern ExceptIfAttachmentNameMatchesPatterns
			{
				set
				{
					base.PowerSharpParameters["ExceptIfAttachmentNameMatchesPatterns"] = value;
				}
			}

			public virtual Word ExceptIfAttachmentExtensionMatchesWords
			{
				set
				{
					base.PowerSharpParameters["ExceptIfAttachmentExtensionMatchesWords"] = value;
				}
			}

			public virtual Word ExceptIfAttachmentPropertyContainsWords
			{
				set
				{
					base.PowerSharpParameters["ExceptIfAttachmentPropertyContainsWords"] = value;
				}
			}

			public virtual Word ExceptIfContentCharacterSetContainsWords
			{
				set
				{
					base.PowerSharpParameters["ExceptIfContentCharacterSetContainsWords"] = value;
				}
			}

			public virtual SclValue ExceptIfSCLOver
			{
				set
				{
					base.PowerSharpParameters["ExceptIfSCLOver"] = value;
				}
			}

			public virtual ByteQuantifiedSize ExceptIfAttachmentSizeOver
			{
				set
				{
					base.PowerSharpParameters["ExceptIfAttachmentSizeOver"] = value;
				}
			}

			public virtual ByteQuantifiedSize ExceptIfMessageSizeOver
			{
				set
				{
					base.PowerSharpParameters["ExceptIfMessageSizeOver"] = value;
				}
			}

			public virtual Importance ExceptIfWithImportance
			{
				set
				{
					base.PowerSharpParameters["ExceptIfWithImportance"] = value;
				}
			}

			public virtual MessageType ExceptIfMessageTypeMatches
			{
				set
				{
					base.PowerSharpParameters["ExceptIfMessageTypeMatches"] = value;
				}
			}

			public virtual Word ExceptIfRecipientAddressContainsWords
			{
				set
				{
					base.PowerSharpParameters["ExceptIfRecipientAddressContainsWords"] = value;
				}
			}

			public virtual Pattern ExceptIfRecipientAddressMatchesPatterns
			{
				set
				{
					base.PowerSharpParameters["ExceptIfRecipientAddressMatchesPatterns"] = value;
				}
			}

			public virtual Word ExceptIfSenderInRecipientList
			{
				set
				{
					base.PowerSharpParameters["ExceptIfSenderInRecipientList"] = value;
				}
			}

			public virtual Word ExceptIfRecipientInSenderList
			{
				set
				{
					base.PowerSharpParameters["ExceptIfRecipientInSenderList"] = value;
				}
			}

			public virtual Word ExceptIfAttachmentContainsWords
			{
				set
				{
					base.PowerSharpParameters["ExceptIfAttachmentContainsWords"] = value;
				}
			}

			public virtual Pattern ExceptIfAttachmentMatchesPatterns
			{
				set
				{
					base.PowerSharpParameters["ExceptIfAttachmentMatchesPatterns"] = value;
				}
			}

			public virtual bool ExceptIfAttachmentIsUnsupported
			{
				set
				{
					base.PowerSharpParameters["ExceptIfAttachmentIsUnsupported"] = value;
				}
			}

			public virtual bool ExceptIfAttachmentProcessingLimitExceeded
			{
				set
				{
					base.PowerSharpParameters["ExceptIfAttachmentProcessingLimitExceeded"] = value;
				}
			}

			public virtual bool ExceptIfAttachmentHasExecutableContent
			{
				set
				{
					base.PowerSharpParameters["ExceptIfAttachmentHasExecutableContent"] = value;
				}
			}

			public virtual bool ExceptIfAttachmentIsPasswordProtected
			{
				set
				{
					base.PowerSharpParameters["ExceptIfAttachmentIsPasswordProtected"] = value;
				}
			}

			public virtual Word ExceptIfAnyOfRecipientAddressContainsWords
			{
				set
				{
					base.PowerSharpParameters["ExceptIfAnyOfRecipientAddressContainsWords"] = value;
				}
			}

			public virtual Pattern ExceptIfAnyOfRecipientAddressMatchesPatterns
			{
				set
				{
					base.PowerSharpParameters["ExceptIfAnyOfRecipientAddressMatchesPatterns"] = value;
				}
			}

			public virtual Hashtable ExceptIfMessageContainsDataClassifications
			{
				set
				{
					base.PowerSharpParameters["ExceptIfMessageContainsDataClassifications"] = value;
				}
			}

			public virtual MultiValuedProperty<IPRange> ExceptIfSenderIpRanges
			{
				set
				{
					base.PowerSharpParameters["ExceptIfSenderIpRanges"] = value;
				}
			}

			public virtual SubjectPrefix? PrependSubject
			{
				set
				{
					base.PowerSharpParameters["PrependSubject"] = value;
				}
			}

			public virtual string SetAuditSeverity
			{
				set
				{
					base.PowerSharpParameters["SetAuditSeverity"] = value;
				}
			}

			public virtual string ApplyClassification
			{
				set
				{
					base.PowerSharpParameters["ApplyClassification"] = value;
				}
			}

			public virtual DisclaimerLocation ApplyHtmlDisclaimerLocation
			{
				set
				{
					base.PowerSharpParameters["ApplyHtmlDisclaimerLocation"] = value;
				}
			}

			public virtual DisclaimerText ApplyHtmlDisclaimerText
			{
				set
				{
					base.PowerSharpParameters["ApplyHtmlDisclaimerText"] = value;
				}
			}

			public virtual DisclaimerFallbackAction ApplyHtmlDisclaimerFallbackAction
			{
				set
				{
					base.PowerSharpParameters["ApplyHtmlDisclaimerFallbackAction"] = value;
				}
			}

			public virtual string ApplyRightsProtectionTemplate
			{
				set
				{
					base.PowerSharpParameters["ApplyRightsProtectionTemplate"] = ((value != null) ? new RmsTemplateIdParameter(value) : null);
				}
			}

			public virtual SclValue SetSCL
			{
				set
				{
					base.PowerSharpParameters["SetSCL"] = value;
				}
			}

			public virtual HeaderName SetHeaderName
			{
				set
				{
					base.PowerSharpParameters["SetHeaderName"] = value;
				}
			}

			public virtual HeaderValue SetHeaderValue
			{
				set
				{
					base.PowerSharpParameters["SetHeaderValue"] = value;
				}
			}

			public virtual HeaderName RemoveHeader
			{
				set
				{
					base.PowerSharpParameters["RemoveHeader"] = value;
				}
			}

			public virtual RecipientIdParameter AddToRecipients
			{
				set
				{
					base.PowerSharpParameters["AddToRecipients"] = value;
				}
			}

			public virtual RecipientIdParameter CopyTo
			{
				set
				{
					base.PowerSharpParameters["CopyTo"] = value;
				}
			}

			public virtual RecipientIdParameter BlindCopyTo
			{
				set
				{
					base.PowerSharpParameters["BlindCopyTo"] = value;
				}
			}

			public virtual AddedRecipientType AddManagerAsRecipientType
			{
				set
				{
					base.PowerSharpParameters["AddManagerAsRecipientType"] = value;
				}
			}

			public virtual RecipientIdParameter ModerateMessageByUser
			{
				set
				{
					base.PowerSharpParameters["ModerateMessageByUser"] = value;
				}
			}

			public virtual bool ModerateMessageByManager
			{
				set
				{
					base.PowerSharpParameters["ModerateMessageByManager"] = value;
				}
			}

			public virtual RecipientIdParameter RedirectMessageTo
			{
				set
				{
					base.PowerSharpParameters["RedirectMessageTo"] = value;
				}
			}

			public virtual NotifySenderType NotifySender
			{
				set
				{
					base.PowerSharpParameters["NotifySender"] = value;
				}
			}

			public virtual RejectEnhancedStatus? RejectMessageEnhancedStatusCode
			{
				set
				{
					base.PowerSharpParameters["RejectMessageEnhancedStatusCode"] = value;
				}
			}

			public virtual DsnText? RejectMessageReasonText
			{
				set
				{
					base.PowerSharpParameters["RejectMessageReasonText"] = value;
				}
			}

			public virtual bool DeleteMessage
			{
				set
				{
					base.PowerSharpParameters["DeleteMessage"] = value;
				}
			}

			public virtual bool Disconnect
			{
				set
				{
					base.PowerSharpParameters["Disconnect"] = value;
				}
			}

			public virtual bool Quarantine
			{
				set
				{
					base.PowerSharpParameters["Quarantine"] = value;
				}
			}

			public virtual RejectText? SmtpRejectMessageRejectText
			{
				set
				{
					base.PowerSharpParameters["SmtpRejectMessageRejectText"] = value;
				}
			}

			public virtual RejectStatusCode? SmtpRejectMessageRejectStatusCode
			{
				set
				{
					base.PowerSharpParameters["SmtpRejectMessageRejectStatusCode"] = value;
				}
			}

			public virtual EventLogText? LogEventText
			{
				set
				{
					base.PowerSharpParameters["LogEventText"] = value;
				}
			}

			public virtual bool StopRuleProcessing
			{
				set
				{
					base.PowerSharpParameters["StopRuleProcessing"] = value;
				}
			}

			public virtual DateTime? ActivationDate
			{
				set
				{
					base.PowerSharpParameters["ActivationDate"] = value;
				}
			}

			public virtual DateTime? ExpiryDate
			{
				set
				{
					base.PowerSharpParameters["ExpiryDate"] = value;
				}
			}

			public virtual OutboundConnectorIdParameter RouteMessageOutboundConnector
			{
				set
				{
					base.PowerSharpParameters["RouteMessageOutboundConnector"] = value;
				}
			}

			public virtual bool RouteMessageOutboundRequireTls
			{
				set
				{
					base.PowerSharpParameters["RouteMessageOutboundRequireTls"] = value;
				}
			}

			public virtual bool ApplyOME
			{
				set
				{
					base.PowerSharpParameters["ApplyOME"] = value;
				}
			}

			public virtual bool RemoveOME
			{
				set
				{
					base.PowerSharpParameters["RemoveOME"] = value;
				}
			}

			public virtual RuleSubType RuleSubType
			{
				set
				{
					base.PowerSharpParameters["RuleSubType"] = value;
				}
			}

			public virtual RuleErrorAction RuleErrorAction
			{
				set
				{
					base.PowerSharpParameters["RuleErrorAction"] = value;
				}
			}

			public virtual SenderAddressLocation SenderAddressLocation
			{
				set
				{
					base.PowerSharpParameters["SenderAddressLocation"] = value;
				}
			}

			public virtual string GenerateIncidentReport
			{
				set
				{
					base.PowerSharpParameters["GenerateIncidentReport"] = ((value != null) ? new RecipientIdParameter(value) : null);
				}
			}

			public virtual IncidentReportOriginalMail? IncidentReportOriginalMail
			{
				set
				{
					base.PowerSharpParameters["IncidentReportOriginalMail"] = value;
				}
			}

			public virtual IncidentReportContent IncidentReportContent
			{
				set
				{
					base.PowerSharpParameters["IncidentReportContent"] = value;
				}
			}

			public virtual DisclaimerText? GenerateNotification
			{
				set
				{
					base.PowerSharpParameters["GenerateNotification"] = value;
				}
			}

			public virtual string Organization
			{
				set
				{
					base.PowerSharpParameters["Organization"] = ((value != null) ? new OrganizationIdParameter(value) : null);
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
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
