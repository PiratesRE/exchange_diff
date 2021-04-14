using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.RightsManagement;
using Microsoft.Exchange.Data.Storage.RightsManagement;
using Microsoft.Exchange.Management.RightsManagement;
using Microsoft.Exchange.Management.SystemConfigurationTasks;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.MessagingPolicies.CompliancePrograms.Tasks;
using Microsoft.Exchange.Security.RightsManagement;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Tasks
{
	[Cmdlet("Set", "TransportRule", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetTransportRule : SetSystemConfigurationObjectTask<RuleIdParameter, Rule, TransportRule>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetTransportRule(this.Identity.ToString());
			}
		}

		public SetTransportRule()
		{
			this.ruleCollectionName = Utils.RuleCollectionNameFromRole();
			this.supportedPredicates = TransportRulePredicate.GetAvailablePredicateMappings();
			this.supportedActions = TransportRuleAction.GetAvailableActionMappings();
			this.Priority = 0;
			base.Fields.ResetChangeTracking();
		}

		[Parameter(Mandatory = false)]
		public string Name
		{
			get
			{
				return (string)base.Fields["Name"];
			}
			set
			{
				base.Fields["Name"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int Priority
		{
			get
			{
				return (int)base.Fields["Priority"];
			}
			set
			{
				if (value < 0)
				{
					throw new ArgumentException(Strings.NegativePriority);
				}
				base.Fields["Priority"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public RuleMode Mode
		{
			get
			{
				return (RuleMode)base.Fields["Mode"];
			}
			set
			{
				base.Fields["Mode"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string Comments
		{
			get
			{
				return (string)base.Fields["Comments"];
			}
			set
			{
				base.Fields["Comments"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string DlpPolicy
		{
			get
			{
				return (string)base.Fields["DlpPolicy"];
			}
			set
			{
				base.Fields["DlpPolicy"] = value;
			}
		}

		public TransportRulePredicate[] Conditions
		{
			get
			{
				return (TransportRulePredicate[])base.Fields["Conditions"];
			}
			set
			{
				base.Fields["Conditions"] = value;
			}
		}

		public TransportRulePredicate[] Exceptions
		{
			get
			{
				return (TransportRulePredicate[])base.Fields["Exceptions"];
			}
			set
			{
				base.Fields["Exceptions"] = value;
			}
		}

		public TransportRuleAction[] Actions
		{
			get
			{
				return (TransportRuleAction[])base.Fields["Actions"];
			}
			set
			{
				base.Fields["Actions"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public RecipientIdParameter[] From
		{
			get
			{
				return (RecipientIdParameter[])base.Fields["From"];
			}
			set
			{
				base.Fields["From"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public RecipientIdParameter[] FromMemberOf
		{
			get
			{
				return (RecipientIdParameter[])base.Fields["FromMemberOf"];
			}
			set
			{
				base.Fields["FromMemberOf"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public FromUserScope? FromScope
		{
			get
			{
				return (FromUserScope?)base.Fields["FromScope"];
			}
			set
			{
				base.Fields["FromScope"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public RecipientIdParameter[] SentTo
		{
			get
			{
				return (RecipientIdParameter[])base.Fields["SentTo"];
			}
			set
			{
				base.Fields["SentTo"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public RecipientIdParameter[] SentToMemberOf
		{
			get
			{
				return (RecipientIdParameter[])base.Fields["SentToMemberOf"];
			}
			set
			{
				base.Fields["SentToMemberOf"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ToUserScope? SentToScope
		{
			get
			{
				return (ToUserScope?)base.Fields["SentToScope"];
			}
			set
			{
				base.Fields["SentToScope"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public RecipientIdParameter[] BetweenMemberOf1
		{
			get
			{
				return (RecipientIdParameter[])base.Fields["BetweenMemberOf1"];
			}
			set
			{
				base.Fields["BetweenMemberOf1"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public RecipientIdParameter[] BetweenMemberOf2
		{
			get
			{
				return (RecipientIdParameter[])base.Fields["BetweenMemberOf2"];
			}
			set
			{
				base.Fields["BetweenMemberOf2"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public RecipientIdParameter[] ManagerAddresses
		{
			get
			{
				return (RecipientIdParameter[])base.Fields["ManagerAddresses"];
			}
			set
			{
				base.Fields["ManagerAddresses"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EvaluatedUser? ManagerForEvaluatedUser
		{
			get
			{
				return (EvaluatedUser?)base.Fields["ManagerForEvaluatedUser"];
			}
			set
			{
				base.Fields["ManagerForEvaluatedUser"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ManagementRelationship? SenderManagementRelationship
		{
			get
			{
				return (ManagementRelationship?)base.Fields["SenderManagementRelationship"];
			}
			set
			{
				base.Fields["SenderManagementRelationship"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ADAttribute? ADComparisonAttribute
		{
			get
			{
				return (ADAttribute?)base.Fields["ADComparisonAttribute"];
			}
			set
			{
				base.Fields["ADComparisonAttribute"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Evaluation? ADComparisonOperator
		{
			get
			{
				return (Evaluation?)base.Fields["ADComparisonOperator"];
			}
			set
			{
				base.Fields["ADComparisonOperator"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Word[] SenderADAttributeContainsWords
		{
			get
			{
				return (Word[])base.Fields["SenderADAttributeContainsWords"];
			}
			set
			{
				base.Fields["SenderADAttributeContainsWords"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Pattern[] SenderADAttributeMatchesPatterns
		{
			get
			{
				return (Pattern[])base.Fields["SenderADAttributeMatchesPatterns"];
			}
			set
			{
				base.Fields["SenderADAttributeMatchesPatterns"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Word[] RecipientADAttributeContainsWords
		{
			get
			{
				return (Word[])base.Fields["RecipientADAttributeContainsWords"];
			}
			set
			{
				base.Fields["RecipientADAttributeContainsWords"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Pattern[] RecipientADAttributeMatchesPatterns
		{
			get
			{
				return (Pattern[])base.Fields["RecipientADAttributeMatchesPatterns"];
			}
			set
			{
				base.Fields["RecipientADAttributeMatchesPatterns"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public RecipientIdParameter[] AnyOfToHeader
		{
			get
			{
				return (RecipientIdParameter[])base.Fields["AnyOfToHeader"];
			}
			set
			{
				base.Fields["AnyOfToHeader"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public RecipientIdParameter[] AnyOfToHeaderMemberOf
		{
			get
			{
				return (RecipientIdParameter[])base.Fields["AnyOfToHeaderMemberOf"];
			}
			set
			{
				base.Fields["AnyOfToHeaderMemberOf"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public RecipientIdParameter[] AnyOfCcHeader
		{
			get
			{
				return (RecipientIdParameter[])base.Fields["AnyOfCcHeader"];
			}
			set
			{
				base.Fields["AnyOfCcHeader"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public RecipientIdParameter[] AnyOfCcHeaderMemberOf
		{
			get
			{
				return (RecipientIdParameter[])base.Fields["AnyOfCcHeaderMemberOf"];
			}
			set
			{
				base.Fields["AnyOfCcHeaderMemberOf"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public RecipientIdParameter[] AnyOfToCcHeader
		{
			get
			{
				return (RecipientIdParameter[])base.Fields["AnyOfToCcHeader"];
			}
			set
			{
				base.Fields["AnyOfToCcHeader"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public RecipientIdParameter[] AnyOfToCcHeaderMemberOf
		{
			get
			{
				return (RecipientIdParameter[])base.Fields["AnyOfToCcHeaderMemberOf"];
			}
			set
			{
				base.Fields["AnyOfToCcHeaderMemberOf"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string HasClassification
		{
			get
			{
				return (string)base.Fields["HasClassification"];
			}
			set
			{
				base.Fields["HasClassification"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool HasNoClassification
		{
			get
			{
				return (bool)base.Fields["HasNoClassification"];
			}
			set
			{
				base.Fields["HasNoClassification"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Word[] SubjectContainsWords
		{
			get
			{
				return (Word[])base.Fields["SubjectContainsWords"];
			}
			set
			{
				base.Fields["SubjectContainsWords"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Word[] SubjectOrBodyContainsWords
		{
			get
			{
				return (Word[])base.Fields["SubjectOrBodyContainsWords"];
			}
			set
			{
				base.Fields["SubjectOrBodyContainsWords"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public HeaderName? HeaderContainsMessageHeader
		{
			get
			{
				return (HeaderName?)base.Fields["HeaderContainsMessageHeader"];
			}
			set
			{
				base.Fields["HeaderContainsMessageHeader"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Word[] HeaderContainsWords
		{
			get
			{
				return (Word[])base.Fields["HeaderContainsWords"];
			}
			set
			{
				base.Fields["HeaderContainsWords"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Word[] FromAddressContainsWords
		{
			get
			{
				return (Word[])base.Fields["FromAddressContainsWords"];
			}
			set
			{
				base.Fields["FromAddressContainsWords"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Word[] SenderDomainIs
		{
			get
			{
				return (Word[])base.Fields["SenderDomainIs"];
			}
			set
			{
				base.Fields["SenderDomainIs"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Word[] RecipientDomainIs
		{
			get
			{
				return (Word[])base.Fields["RecipientDomainIs"];
			}
			set
			{
				base.Fields["RecipientDomainIs"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Pattern[] SubjectMatchesPatterns
		{
			get
			{
				return (Pattern[])base.Fields["SubjectMatchesPatterns"];
			}
			set
			{
				base.Fields["SubjectMatchesPatterns"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Pattern[] SubjectOrBodyMatchesPatterns
		{
			get
			{
				return (Pattern[])base.Fields["SubjectOrBodyMatchesPatterns"];
			}
			set
			{
				base.Fields["SubjectOrBodyMatchesPatterns"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public HeaderName? HeaderMatchesMessageHeader
		{
			get
			{
				return (HeaderName?)base.Fields["HeaderMatchesMessageHeader"];
			}
			set
			{
				base.Fields["HeaderMatchesMessageHeader"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Pattern[] HeaderMatchesPatterns
		{
			get
			{
				return (Pattern[])base.Fields["HeaderMatchesPatterns"];
			}
			set
			{
				base.Fields["HeaderMatchesPatterns"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Pattern[] FromAddressMatchesPatterns
		{
			get
			{
				return (Pattern[])base.Fields["FromAddressMatchesPatterns"];
			}
			set
			{
				base.Fields["FromAddressMatchesPatterns"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Pattern[] AttachmentNameMatchesPatterns
		{
			get
			{
				return (Pattern[])base.Fields["AttachmentNameMatchesPatterns"];
			}
			set
			{
				base.Fields["AttachmentNameMatchesPatterns"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Word[] AttachmentExtensionMatchesWords
		{
			get
			{
				return (Word[])base.Fields["AttachmentExtensionMatchesWords"];
			}
			set
			{
				base.Fields["AttachmentExtensionMatchesWords"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Word[] AttachmentPropertyContainsWords
		{
			get
			{
				return (Word[])base.Fields["AttachmentPropertyContainsWords"];
			}
			set
			{
				base.Fields["AttachmentPropertyContainsWords"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Word[] ContentCharacterSetContainsWords
		{
			get
			{
				return (Word[])base.Fields["ContentCharacterSetContainsWords"];
			}
			set
			{
				base.Fields["ContentCharacterSetContainsWords"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SclValue? SCLOver
		{
			get
			{
				return (SclValue?)base.Fields["SCLOver"];
			}
			set
			{
				base.Fields["SCLOver"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ByteQuantifiedSize? AttachmentSizeOver
		{
			get
			{
				return (ByteQuantifiedSize?)base.Fields["AttachmentSizeOver"];
			}
			set
			{
				base.Fields["AttachmentSizeOver"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ByteQuantifiedSize? MessageSizeOver
		{
			get
			{
				return new ByteQuantifiedSize?((ByteQuantifiedSize)base.Fields["MessageSizeOver"]);
			}
			set
			{
				base.Fields["MessageSizeOver"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Importance? WithImportance
		{
			get
			{
				return (Importance?)base.Fields["WithImportance"];
			}
			set
			{
				base.Fields["WithImportance"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MessageType? MessageTypeMatches
		{
			get
			{
				return (MessageType?)base.Fields["MessageTypeMatches"];
			}
			set
			{
				base.Fields["MessageTypeMatches"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Word[] RecipientAddressContainsWords
		{
			get
			{
				return (Word[])base.Fields["RecipientAddressContainsWords"];
			}
			set
			{
				base.Fields["RecipientAddressContainsWords"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Pattern[] RecipientAddressMatchesPatterns
		{
			get
			{
				return (Pattern[])base.Fields["RecipientAddressMatchesPatterns"];
			}
			set
			{
				base.Fields["RecipientAddressMatchesPatterns"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Word[] SenderInRecipientList
		{
			get
			{
				return (Word[])base.Fields["SenderInRecipientList"];
			}
			set
			{
				base.Fields["SenderInRecipientList"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Word[] RecipientInSenderList
		{
			get
			{
				return (Word[])base.Fields["RecipientInSenderList"];
			}
			set
			{
				base.Fields["RecipientInSenderList"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Word[] AttachmentContainsWords
		{
			get
			{
				return (Word[])base.Fields["AttachmentContainsWords"];
			}
			set
			{
				base.Fields["AttachmentContainsWords"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Pattern[] AttachmentMatchesPatterns
		{
			get
			{
				return (Pattern[])base.Fields["AttachmentMatchesPatterns"];
			}
			set
			{
				base.Fields["AttachmentMatchesPatterns"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AttachmentIsUnsupported
		{
			get
			{
				return (bool)base.Fields["AttachmentIsUnsupported"];
			}
			set
			{
				base.Fields["AttachmentIsUnsupported"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AttachmentProcessingLimitExceeded
		{
			get
			{
				return (bool)base.Fields["AttachmentProcessingLimitExceeded"];
			}
			set
			{
				base.Fields["AttachmentProcessingLimitExceeded"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AttachmentHasExecutableContent
		{
			get
			{
				return (bool)base.Fields["AttachmentHasExecutableContent"];
			}
			set
			{
				base.Fields["AttachmentHasExecutableContent"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AttachmentIsPasswordProtected
		{
			get
			{
				return (bool)base.Fields["AttachmentIsPasswordProtected"];
			}
			set
			{
				base.Fields["AttachmentIsPasswordProtected"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Word[] AnyOfRecipientAddressContainsWords
		{
			get
			{
				return (Word[])base.Fields["AnyOfRecipientAddressContainsWords"];
			}
			set
			{
				base.Fields["AnyOfRecipientAddressContainsWords"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Pattern[] AnyOfRecipientAddressMatchesPatterns
		{
			get
			{
				return (Pattern[])base.Fields["AnyOfRecipientAddressMatchesPatterns"];
			}
			set
			{
				base.Fields["AnyOfRecipientAddressMatchesPatterns"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public RecipientIdParameter[] ExceptIfFrom
		{
			get
			{
				return (RecipientIdParameter[])base.Fields["ExceptIfFrom"];
			}
			set
			{
				base.Fields["ExceptIfFrom"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool HasSenderOverride
		{
			get
			{
				return (bool)base.Fields["HasSenderOverride"];
			}
			set
			{
				base.Fields["HasSenderOverride"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Hashtable[] MessageContainsDataClassifications
		{
			get
			{
				return (Hashtable[])base.Fields["MessageContainsDataClassifications"];
			}
			set
			{
				base.Fields["MessageContainsDataClassifications"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<IPRange> SenderIpRanges
		{
			get
			{
				return (MultiValuedProperty<IPRange>)base.Fields["SenderIpRanges"];
			}
			set
			{
				base.Fields["SenderIpRanges"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public RecipientIdParameter[] ExceptIfFromMemberOf
		{
			get
			{
				return (RecipientIdParameter[])base.Fields["ExceptIfFromMemberOf"];
			}
			set
			{
				base.Fields["ExceptIfFromMemberOf"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public FromUserScope? ExceptIfFromScope
		{
			get
			{
				return (FromUserScope?)base.Fields["ExceptIfFromScope"];
			}
			set
			{
				base.Fields["ExceptIfFromScope"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public RecipientIdParameter[] ExceptIfSentTo
		{
			get
			{
				return (RecipientIdParameter[])base.Fields["ExceptIfSentTo"];
			}
			set
			{
				base.Fields["ExceptIfSentTo"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public RecipientIdParameter[] ExceptIfSentToMemberOf
		{
			get
			{
				return (RecipientIdParameter[])base.Fields["ExceptIfSentToMemberOf"];
			}
			set
			{
				base.Fields["ExceptIfSentToMemberOf"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ToUserScope? ExceptIfSentToScope
		{
			get
			{
				return (ToUserScope?)base.Fields["ExceptIfSentToScope"];
			}
			set
			{
				base.Fields["ExceptIfSentToScope"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public RecipientIdParameter[] ExceptIfBetweenMemberOf1
		{
			get
			{
				return (RecipientIdParameter[])base.Fields["ExceptIfBetweenMemberOf1"];
			}
			set
			{
				base.Fields["ExceptIfBetweenMemberOf1"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public RecipientIdParameter[] ExceptIfBetweenMemberOf2
		{
			get
			{
				return (RecipientIdParameter[])base.Fields["ExceptIfBetweenMemberOf2"];
			}
			set
			{
				base.Fields["ExceptIfBetweenMemberOf2"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public RecipientIdParameter[] ExceptIfManagerAddresses
		{
			get
			{
				return (RecipientIdParameter[])base.Fields["ExceptIfManagerAddresses"];
			}
			set
			{
				base.Fields["ExceptIfManagerAddresses"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EvaluatedUser? ExceptIfManagerForEvaluatedUser
		{
			get
			{
				return (EvaluatedUser?)base.Fields["ExceptIfManagerForEvaluatedUser"];
			}
			set
			{
				base.Fields["ExceptIfManagerForEvaluatedUser"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ManagementRelationship? ExceptIfSenderManagementRelationship
		{
			get
			{
				return (ManagementRelationship?)base.Fields["ExceptIfSenderManagementRelationship"];
			}
			set
			{
				base.Fields["ExceptIfSenderManagementRelationship"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ADAttribute? ExceptIfADComparisonAttribute
		{
			get
			{
				return (ADAttribute?)base.Fields["ExceptIfADComparisonAttribute"];
			}
			set
			{
				base.Fields["ExceptIfADComparisonAttribute"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Evaluation? ExceptIfADComparisonOperator
		{
			get
			{
				return (Evaluation?)base.Fields["ExceptIfADComparisonOperator"];
			}
			set
			{
				base.Fields["ExceptIfADComparisonOperator"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Word[] ExceptIfSenderADAttributeContainsWords
		{
			get
			{
				return (Word[])base.Fields["ExceptIfSenderADAttributeContainsWords"];
			}
			set
			{
				base.Fields["ExceptIfSenderADAttributeContainsWords"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Pattern[] ExceptIfSenderADAttributeMatchesPatterns
		{
			get
			{
				return (Pattern[])base.Fields["ExceptIfSenderADAttributeMatchesPatterns"];
			}
			set
			{
				base.Fields["ExceptIfSenderADAttributeMatchesPatterns"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Word[] ExceptIfRecipientADAttributeContainsWords
		{
			get
			{
				return (Word[])base.Fields["ExceptIfRecipientADAttributeContainsWords"];
			}
			set
			{
				base.Fields["ExceptIfRecipientADAttributeContainsWords"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Pattern[] ExceptIfRecipientADAttributeMatchesPatterns
		{
			get
			{
				return (Pattern[])base.Fields["ExceptIfRecipientADAttributeMatchesPatterns"];
			}
			set
			{
				base.Fields["ExceptIfRecipientADAttributeMatchesPatterns"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public RecipientIdParameter[] ExceptIfAnyOfToHeader
		{
			get
			{
				return (RecipientIdParameter[])base.Fields["ExceptIfAnyOfToHeader"];
			}
			set
			{
				base.Fields["ExceptIfAnyOfToHeader"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public RecipientIdParameter[] ExceptIfAnyOfToHeaderMemberOf
		{
			get
			{
				return (RecipientIdParameter[])base.Fields["ExceptIfAnyOfToHeaderMemberOf"];
			}
			set
			{
				base.Fields["ExceptIfAnyOfToHeaderMemberOf"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public RecipientIdParameter[] ExceptIfAnyOfCcHeader
		{
			get
			{
				return (RecipientIdParameter[])base.Fields["ExceptIfAnyOfCcHeader"];
			}
			set
			{
				base.Fields["ExceptIfAnyOfCcHeader"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public RecipientIdParameter[] ExceptIfAnyOfCcHeaderMemberOf
		{
			get
			{
				return (RecipientIdParameter[])base.Fields["ExceptIfAnyOfCcHeaderMemberOf"];
			}
			set
			{
				base.Fields["ExceptIfAnyOfCcHeaderMemberOf"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public RecipientIdParameter[] ExceptIfAnyOfToCcHeader
		{
			get
			{
				return (RecipientIdParameter[])base.Fields["ExceptIfAnyOfToCcHeader"];
			}
			set
			{
				base.Fields["ExceptIfAnyOfToCcHeader"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public RecipientIdParameter[] ExceptIfAnyOfToCcHeaderMemberOf
		{
			get
			{
				return (RecipientIdParameter[])base.Fields["ExceptIfAnyOfToCcHeaderMemberOf"];
			}
			set
			{
				base.Fields["ExceptIfAnyOfToCcHeaderMemberOf"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string ExceptIfHasClassification
		{
			get
			{
				return (string)base.Fields["ExceptIfHasClassification"];
			}
			set
			{
				base.Fields["ExceptIfHasClassification"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool ExceptIfHasNoClassification
		{
			get
			{
				return (bool)base.Fields["ExceptIfHasNoClassification"];
			}
			set
			{
				base.Fields["ExceptIfHasNoClassification"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Word[] ExceptIfSubjectContainsWords
		{
			get
			{
				return (Word[])base.Fields["ExceptIfSubjectContainsWords"];
			}
			set
			{
				base.Fields["ExceptIfSubjectContainsWords"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Word[] ExceptIfSubjectOrBodyContainsWords
		{
			get
			{
				return (Word[])base.Fields["ExceptIfSubjectOrBodyContainsWords"];
			}
			set
			{
				base.Fields["ExceptIfSubjectOrBodyContainsWords"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public HeaderName? ExceptIfHeaderContainsMessageHeader
		{
			get
			{
				return (HeaderName?)base.Fields["ExceptIfHeaderContainsMessageHeader"];
			}
			set
			{
				base.Fields["ExceptIfHeaderContainsMessageHeader"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Word[] ExceptIfHeaderContainsWords
		{
			get
			{
				return (Word[])base.Fields["ExceptIfHeaderContainsWords"];
			}
			set
			{
				base.Fields["ExceptIfHeaderContainsWords"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Word[] ExceptIfFromAddressContainsWords
		{
			get
			{
				return (Word[])base.Fields["ExceptIfFromAddressContainsWords"];
			}
			set
			{
				base.Fields["ExceptIfFromAddressContainsWords"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Word[] ExceptIfSenderDomainIs
		{
			get
			{
				return (Word[])base.Fields["ExceptIfSenderDomainIs"];
			}
			set
			{
				base.Fields["ExceptIfSenderDomainIs"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Word[] ExceptIfRecipientDomainIs
		{
			get
			{
				return (Word[])base.Fields["ExceptIfRecipientDomainIs"];
			}
			set
			{
				base.Fields["ExceptIfRecipientDomainIs"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Pattern[] ExceptIfSubjectMatchesPatterns
		{
			get
			{
				return (Pattern[])base.Fields["ExceptIfSubjectMatchesPatterns"];
			}
			set
			{
				base.Fields["ExceptIfSubjectMatchesPatterns"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Pattern[] ExceptIfSubjectOrBodyMatchesPatterns
		{
			get
			{
				return (Pattern[])base.Fields["ExceptIfSubjectOrBodyMatchesPatterns"];
			}
			set
			{
				base.Fields["ExceptIfSubjectOrBodyMatchesPatterns"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public HeaderName? ExceptIfHeaderMatchesMessageHeader
		{
			get
			{
				return (HeaderName?)base.Fields["ExceptIfHeaderMatchesMessageHeader"];
			}
			set
			{
				base.Fields["ExceptIfHeaderMatchesMessageHeader"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Pattern[] ExceptIfHeaderMatchesPatterns
		{
			get
			{
				return (Pattern[])base.Fields["ExceptIfHeaderMatchesPatterns"];
			}
			set
			{
				base.Fields["ExceptIfHeaderMatchesPatterns"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Pattern[] ExceptIfFromAddressMatchesPatterns
		{
			get
			{
				return (Pattern[])base.Fields["ExceptIfFromAddressMatchesPatterns"];
			}
			set
			{
				base.Fields["ExceptIfFromAddressMatchesPatterns"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Pattern[] ExceptIfAttachmentNameMatchesPatterns
		{
			get
			{
				return (Pattern[])base.Fields["ExceptIfAttachmentNameMatchesPatterns"];
			}
			set
			{
				base.Fields["ExceptIfAttachmentNameMatchesPatterns"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Word[] ExceptIfAttachmentExtensionMatchesWords
		{
			get
			{
				return (Word[])base.Fields["ExceptIfAttachmentExtensionMatchesWords"];
			}
			set
			{
				base.Fields["ExceptIfAttachmentExtensionMatchesWords"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Word[] ExceptIfAttachmentPropertyContainsWords
		{
			get
			{
				return (Word[])base.Fields["ExceptIfAttachmentPropertyContainsWords"];
			}
			set
			{
				base.Fields["ExceptIfAttachmentPropertyContainsWords"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Word[] ExceptIfContentCharacterSetContainsWords
		{
			get
			{
				return (Word[])base.Fields["ExceptIfContentCharacterSetContainsWords"];
			}
			set
			{
				base.Fields["ExceptIfContentCharacterSetContainsWords"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SclValue? ExceptIfSCLOver
		{
			get
			{
				return (SclValue?)base.Fields["ExceptIfSCLOver"];
			}
			set
			{
				base.Fields["ExceptIfSCLOver"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ByteQuantifiedSize? ExceptIfAttachmentSizeOver
		{
			get
			{
				return (ByteQuantifiedSize?)base.Fields["ExceptIfAttachmentSizeOver"];
			}
			set
			{
				base.Fields["ExceptIfAttachmentSizeOver"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ByteQuantifiedSize? ExceptIfMessageSizeOver
		{
			get
			{
				return new ByteQuantifiedSize?((ByteQuantifiedSize)base.Fields["ExceptIfMessageSizeOver"]);
			}
			set
			{
				base.Fields["ExceptIfMessageSizeOver"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Importance? ExceptIfWithImportance
		{
			get
			{
				return (Importance?)base.Fields["ExceptIfWithImportance"];
			}
			set
			{
				base.Fields["ExceptIfWithImportance"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MessageType? ExceptIfMessageTypeMatches
		{
			get
			{
				return (MessageType?)base.Fields["ExceptIfMessageTypeMatches"];
			}
			set
			{
				base.Fields["ExceptIfMessageTypeMatches"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Word[] ExceptIfRecipientAddressContainsWords
		{
			get
			{
				return (Word[])base.Fields["ExceptIfRecipientAddressContainsWords"];
			}
			set
			{
				base.Fields["ExceptIfRecipientAddressContainsWords"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Pattern[] ExceptIfRecipientAddressMatchesPatterns
		{
			get
			{
				return (Pattern[])base.Fields["ExceptIfRecipientAddressMatchesPatterns"];
			}
			set
			{
				base.Fields["ExceptIfRecipientAddressMatchesPatterns"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Word[] ExceptIfSenderInRecipientList
		{
			get
			{
				return (Word[])base.Fields["ExceptIfSenderInRecipientList"];
			}
			set
			{
				base.Fields["ExceptIfSenderInRecipientList"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Word[] ExceptIfRecipientInSenderList
		{
			get
			{
				return (Word[])base.Fields["ExceptIfRecipientInSenderList"];
			}
			set
			{
				base.Fields["ExceptIfRecipientInSenderList"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Word[] ExceptIfAttachmentContainsWords
		{
			get
			{
				return (Word[])base.Fields["ExceptIfAttachmentContainsWords"];
			}
			set
			{
				base.Fields["ExceptIfAttachmentContainsWords"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Pattern[] ExceptIfAttachmentMatchesPatterns
		{
			get
			{
				return (Pattern[])base.Fields["ExceptIfAttachmentMatchesPatterns"];
			}
			set
			{
				base.Fields["ExceptIfAttachmentMatchesPatterns"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool ExceptIfAttachmentIsUnsupported
		{
			get
			{
				return (bool)base.Fields["ExceptIfAttachmentIsUnsupported"];
			}
			set
			{
				base.Fields["ExceptIfAttachmentIsUnsupported"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool ExceptIfAttachmentProcessingLimitExceeded
		{
			get
			{
				return (bool)base.Fields["ExceptIfAttachmentProcessingLimitExceeded"];
			}
			set
			{
				base.Fields["ExceptIfAttachmentProcessingLimitExceeded"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool ExceptIfAttachmentHasExecutableContent
		{
			get
			{
				return (bool)base.Fields["ExceptIfAttachmentHasExecutableContent"];
			}
			set
			{
				base.Fields["ExceptIfAttachmentHasExecutableContent"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Word[] ExceptIfAnyOfRecipientAddressContainsWords
		{
			get
			{
				return (Word[])base.Fields["ExceptIfAnyOfRecipientAddressContainsWords"];
			}
			set
			{
				base.Fields["ExceptIfAnyOfRecipientAddressContainsWords"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool ExceptIfAttachmentIsPasswordProtected
		{
			get
			{
				return (bool)base.Fields["ExceptIfAttachmentIsPasswordProtected"];
			}
			set
			{
				base.Fields["ExceptIfAttachmentIsPasswordProtected"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Pattern[] ExceptIfAnyOfRecipientAddressMatchesPatterns
		{
			get
			{
				return (Pattern[])base.Fields["ExceptIfAnyOfRecipientAddressMatchesPatterns"];
			}
			set
			{
				base.Fields["ExceptIfAnyOfRecipientAddressMatchesPatterns"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool ExceptIfHasSenderOverride
		{
			get
			{
				return (bool)base.Fields["ExceptIfHasSenderOverride"];
			}
			set
			{
				base.Fields["ExceptIfHasSenderOverride"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Hashtable[] ExceptIfMessageContainsDataClassifications
		{
			get
			{
				return (Hashtable[])base.Fields["ExceptIfMessageContainsDataClassifications"];
			}
			set
			{
				base.Fields["ExceptIfMessageContainsDataClassifications"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<IPRange> ExceptIfSenderIpRanges
		{
			get
			{
				return (MultiValuedProperty<IPRange>)base.Fields["ExceptIfSenderIpRanges"];
			}
			set
			{
				base.Fields["ExceptIfSenderIpRanges"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SubjectPrefix? PrependSubject
		{
			get
			{
				return (SubjectPrefix?)base.Fields["PrependSubject"];
			}
			set
			{
				base.Fields["PrependSubject"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string SetAuditSeverity
		{
			get
			{
				return (string)base.Fields["SetAuditSeverity"];
			}
			set
			{
				base.Fields["SetAuditSeverity"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string ApplyClassification
		{
			get
			{
				return (string)base.Fields["ApplyClassification"];
			}
			set
			{
				base.Fields["ApplyClassification"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public DisclaimerLocation? ApplyHtmlDisclaimerLocation
		{
			get
			{
				return (DisclaimerLocation?)base.Fields["ApplyHtmlDisclaimerLocation"];
			}
			set
			{
				base.Fields["ApplyHtmlDisclaimerLocation"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public DisclaimerText? ApplyHtmlDisclaimerText
		{
			get
			{
				return (DisclaimerText?)base.Fields["ApplyHtmlDisclaimerText"];
			}
			set
			{
				base.Fields["ApplyHtmlDisclaimerText"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public DisclaimerFallbackAction? ApplyHtmlDisclaimerFallbackAction
		{
			get
			{
				return (DisclaimerFallbackAction?)base.Fields["ApplyHtmlDisclaimerFallbackAction"];
			}
			set
			{
				base.Fields["ApplyHtmlDisclaimerFallbackAction"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public RmsTemplateIdParameter ApplyRightsProtectionTemplate
		{
			get
			{
				return (RmsTemplateIdParameter)base.Fields["ApplyRightsProtectionTemplate"];
			}
			set
			{
				base.Fields["ApplyRightsProtectionTemplate"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SclValue? SetSCL
		{
			get
			{
				return (SclValue?)base.Fields["SetSCL"];
			}
			set
			{
				base.Fields["SetSCL"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public HeaderName? SetHeaderName
		{
			get
			{
				return (HeaderName?)base.Fields["SetHeaderName"];
			}
			set
			{
				base.Fields["SetHeaderName"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public HeaderValue? SetHeaderValue
		{
			get
			{
				return (HeaderValue?)base.Fields["SetHeaderValue"];
			}
			set
			{
				base.Fields["SetHeaderValue"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public HeaderName? RemoveHeader
		{
			get
			{
				return (HeaderName?)base.Fields["RemoveHeader"];
			}
			set
			{
				base.Fields["RemoveHeader"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public RecipientIdParameter[] AddToRecipients
		{
			get
			{
				return (RecipientIdParameter[])base.Fields["AddToRecipients"];
			}
			set
			{
				base.Fields["AddToRecipients"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public RecipientIdParameter[] CopyTo
		{
			get
			{
				return (RecipientIdParameter[])base.Fields["CopyTo"];
			}
			set
			{
				base.Fields["CopyTo"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public RecipientIdParameter[] BlindCopyTo
		{
			get
			{
				return (RecipientIdParameter[])base.Fields["BlindCopyTo"];
			}
			set
			{
				base.Fields["BlindCopyTo"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public AddedRecipientType? AddManagerAsRecipientType
		{
			get
			{
				return new AddedRecipientType?((AddedRecipientType)base.Fields["AddManagerAsRecipientType"]);
			}
			set
			{
				base.Fields["AddManagerAsRecipientType"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public RecipientIdParameter[] ModerateMessageByUser
		{
			get
			{
				return (RecipientIdParameter[])base.Fields["ModerateMessageByUser"];
			}
			set
			{
				base.Fields["ModerateMessageByUser"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool ModerateMessageByManager
		{
			get
			{
				return (bool)base.Fields["ModerateMessageByManager"];
			}
			set
			{
				base.Fields["ModerateMessageByManager"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public RecipientIdParameter[] RedirectMessageTo
		{
			get
			{
				return (RecipientIdParameter[])base.Fields["RedirectMessageTo"];
			}
			set
			{
				base.Fields["RedirectMessageTo"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public NotifySenderType? NotifySender
		{
			get
			{
				return new NotifySenderType?((NotifySenderType)base.Fields["NotifySender"]);
			}
			set
			{
				base.Fields["NotifySender"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public RejectEnhancedStatus? RejectMessageEnhancedStatusCode
		{
			get
			{
				return (RejectEnhancedStatus?)base.Fields["RejectMessageEnhancedStatusCode"];
			}
			set
			{
				base.Fields["RejectMessageEnhancedStatusCode"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public DsnText? RejectMessageReasonText
		{
			get
			{
				return (DsnText?)base.Fields["RejectMessageReasonText"];
			}
			set
			{
				base.Fields["RejectMessageReasonText"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool DeleteMessage
		{
			get
			{
				return (bool)base.Fields["DeleteMessage"];
			}
			set
			{
				base.Fields["DeleteMessage"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool Disconnect
		{
			get
			{
				return (bool)base.Fields["Disconnect"];
			}
			set
			{
				base.Fields["Disconnect"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool Quarantine
		{
			get
			{
				return (bool)base.Fields["Quarantine"];
			}
			set
			{
				base.Fields["Quarantine"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public RejectText? SmtpRejectMessageRejectText
		{
			get
			{
				return (RejectText?)base.Fields["SmtpRejectMessageRejectText"];
			}
			set
			{
				base.Fields["SmtpRejectMessageRejectText"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public RejectStatusCode? SmtpRejectMessageRejectStatusCode
		{
			get
			{
				return (RejectStatusCode?)base.Fields["SmtpRejectMessageRejectStatusCode"];
			}
			set
			{
				base.Fields["SmtpRejectMessageRejectStatusCode"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public DateTime? ActivationDate
		{
			get
			{
				return (DateTime?)base.Fields["ActivationDate"];
			}
			set
			{
				base.Fields["ActivationDate"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public DateTime? ExpiryDate
		{
			get
			{
				return (DateTime?)base.Fields["ExpiryDate"];
			}
			set
			{
				base.Fields["ExpiryDate"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public RuleSubType RuleSubType
		{
			get
			{
				return (RuleSubType)base.Fields["RuleSubType"];
			}
			set
			{
				base.Fields["RuleSubType"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public RuleErrorAction RuleErrorAction
		{
			get
			{
				return (RuleErrorAction)base.Fields["RuleErrorAction"];
			}
			set
			{
				base.Fields["RuleErrorAction"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SenderAddressLocation SenderAddressLocation
		{
			get
			{
				return (SenderAddressLocation)base.Fields["SenderAddressLocation"];
			}
			set
			{
				base.Fields["SenderAddressLocation"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EventLogText? LogEventText
		{
			get
			{
				return (EventLogText?)base.Fields["LogEventText"];
			}
			set
			{
				base.Fields["LogEventText"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool StopRuleProcessing
		{
			get
			{
				return (bool)base.Fields["StopRuleProcessing"];
			}
			set
			{
				base.Fields["StopRuleProcessing"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public RecipientIdParameter GenerateIncidentReport
		{
			get
			{
				return (RecipientIdParameter)base.Fields["GenerateIncidentReport"];
			}
			set
			{
				base.Fields["GenerateIncidentReport"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public IncidentReportOriginalMail? IncidentReportOriginalMail
		{
			get
			{
				return (IncidentReportOriginalMail?)base.Fields["IncidentReportOriginalMail"];
			}
			set
			{
				base.Fields["IncidentReportOriginalMail"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public IncidentReportContent[] IncidentReportContent
		{
			get
			{
				return (IncidentReportContent[])base.Fields["IncidentReportContent"];
			}
			set
			{
				base.Fields["IncidentReportContent"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public DisclaimerText? GenerateNotification
		{
			get
			{
				return (DisclaimerText?)base.Fields["GenerateNotification"];
			}
			set
			{
				base.Fields["GenerateNotification"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public OutboundConnectorIdParameter RouteMessageOutboundConnector
		{
			get
			{
				return (OutboundConnectorIdParameter)base.Fields["RouteMessageOutboundConnector"];
			}
			set
			{
				base.Fields["RouteMessageOutboundConnector"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool RouteMessageOutboundRequireTls
		{
			get
			{
				return (bool)base.Fields["RouteMessageOutboundRequireTls"];
			}
			set
			{
				base.Fields["RouteMessageOutboundRequireTls"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool ApplyOME
		{
			get
			{
				return (bool)base.Fields["ApplyOME"];
			}
			set
			{
				base.Fields["ApplyOME"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool RemoveOME
		{
			get
			{
				return (bool)base.Fields["RemoveOME"];
			}
			set
			{
				base.Fields["RemoveOME"] = value;
			}
		}

		protected override void InternalValidate()
		{
			if (base.OptionalIdentityData != null)
			{
				base.OptionalIdentityData.ConfigurationContainerRdn = RuleIdParameter.GetRuleCollectionRdn(this.ruleCollectionName);
			}
			this.DataObject = (TransportRule)this.ResolveDataObject();
			if (!this.DataObject.OrganizationId.Equals(OrganizationId.ForestWideOrgId) && !((IDirectorySession)base.DataSession).SessionSettings.CurrentOrganizationId.Equals(this.DataObject.OrganizationId))
			{
				base.UnderscopeDataSession(this.DataObject.OrganizationId);
			}
			if (base.HasErrors)
			{
				return;
			}
			if (!Utils.IsChildOfRuleContainer(this.Identity, this.ruleCollectionName))
			{
				throw new ManagementObjectNotFoundException(base.GetErrorMessageObjectNotFound((this.Identity != null) ? this.Identity.ToString() : null, typeof(RuleIdParameter).ToString(), (base.DataSession != null) ? base.DataSession.Source : null));
			}
			TransportRule transportRule = null;
			try
			{
				transportRule = (TransportRule)TransportRuleParser.Instance.GetRule(this.DataObject.Xml);
			}
			catch (ParserException exception)
			{
				base.WriteError(exception, ErrorCategory.InvalidData, null);
			}
			if (transportRule.IsTooAdvancedToParse)
			{
				base.WriteError(new InvalidOperationException(Strings.CannotModifyRuleDueToVersion(transportRule.Name)), ErrorCategory.InvalidOperation, this.Name);
			}
			Exception exception2;
			string target;
			if (!Utils.ValidateParametersForRole(base.Fields, out exception2, out target))
			{
				base.WriteError(exception2, ErrorCategory.InvalidArgument, target);
			}
			ArgumentException exception3;
			if (!Utils.ValidateRuleComments(this.Comments, out exception3))
			{
				base.WriteError(exception3, ErrorCategory.InvalidArgument, this.Comments);
			}
			if (!Utils.ValidateRestrictedHeaders(base.Fields, true, out exception3, out target))
			{
				base.WriteError(exception3, ErrorCategory.InvalidArgument, target);
			}
			string target2;
			if (!Utils.ValidateParameterGroups(base.Fields, out exception3, out target2))
			{
				base.WriteError(exception3, ErrorCategory.InvalidArgument, target2);
			}
			if (!Utils.ValidateMessageClassification(base.Fields, out exception3, out target2, base.DataSession))
			{
				base.WriteError(exception3, ErrorCategory.InvalidArgument, target2);
			}
			if (!Utils.ValidateContainsWordsPredicate(base.Fields, out exception3, out target2))
			{
				base.WriteError(exception3, ErrorCategory.InvalidArgument, target2);
			}
			if (!Utils.ValidateMatchesPatternsPredicate(base.Fields, out exception3, out target2))
			{
				base.WriteError(exception3, ErrorCategory.InvalidArgument, target2);
			}
			if (!Utils.ValidateAdAttributePredicate(base.Fields, out exception3, out target2))
			{
				base.WriteError(exception3, ErrorCategory.InvalidArgument, target2);
			}
			if (!Utils.ValidatePropertyContainsWordsPredicates(base.Fields, out exception3, out target2))
			{
				base.WriteError(exception3, ErrorCategory.InvalidArgument, target2);
				return;
			}
			Exception exception4;
			if (!Utils.ValidateRecipientIdParameters(base.Fields, base.TenantGlobalCatalogSession, new DataAccessHelper.GetDataObjectDelegate(base.GetDataObject<ADRecipient>), out exception4, out target2))
			{
				base.WriteError(exception4, ErrorCategory.InvalidArgument, target2);
			}
			if (!Utils.ValidateMessageDataClassification(base.Fields, base.CurrentOrganizationId, out exception3, out target2))
			{
				base.WriteError(exception3, ErrorCategory.InvalidArgument, target2);
			}
			if (!Utils.ValidateAuditSeverityLevel(base.Fields, out exception3, out target2))
			{
				base.WriteError(exception3, ErrorCategory.InvalidArgument, target2);
			}
			if (!Utils.ValidateDlpPolicy(base.DataSession, base.Fields, out exception3, out target2))
			{
				base.WriteError(exception3, ErrorCategory.InvalidArgument, target2);
			}
			if (base.Fields.IsModified("DlpPolicy") && !this.DlpPolicy.Equals(string.Empty))
			{
				ADComplianceProgram adcomplianceProgram = DlpUtils.GetInstalledTenantDlpPolicies(base.DataSession, this.DlpPolicy).First<ADComplianceProgram>();
				this.dlpPolicyId = adcomplianceProgram.ImmutableId;
				this.DlpPolicy = adcomplianceProgram.Name;
			}
			if (base.Fields.IsModified("ApplyRightsProtectionTemplate") && base.Fields["ApplyRightsProtectionTemplate"] != null)
			{
				RmsTemplateDataProvider session = new RmsTemplateDataProvider((IConfigurationSession)base.DataSession);
				string name = (this.ApplyRightsProtectionTemplate != null) ? this.ApplyRightsProtectionTemplate.ToString() : string.Empty;
				RmsTemplatePresentation rmsTemplatePresentation = (RmsTemplatePresentation)base.GetDataObject<RmsTemplatePresentation>(this.ApplyRightsProtectionTemplate, session, null, new LocalizedString?(Strings.OutlookProtectionRuleRmsTemplateNotFound(name)), new LocalizedString?(Strings.OutlookProtectionRuleRmsTemplateNotUnique(name)));
				base.Fields["ResolvedRmsTemplateIdentity"] = rmsTemplatePresentation.Identity;
			}
			if ((base.Fields.IsModified("ApplyOME") && this.ApplyOME) || (base.Fields.IsModified("RemoveOME") && this.RemoveOME))
			{
				IRMConfiguration irmconfiguration = IRMConfiguration.Read((IConfigurationSession)base.DataSession);
				if (irmconfiguration == null || !irmconfiguration.InternalLicensingEnabled)
				{
					base.WriteError(new E4eLicensingIsDisabledExceptionSetRule(), ErrorCategory.InvalidArgument, null);
				}
				if (RmsClientManager.IRMConfig.GetRmsTemplate(((IDirectorySession)base.DataSession).SessionSettings.CurrentOrganizationId, RmsTemplate.InternetConfidential.Id) == null)
				{
					base.WriteError(new E4eRuleRmsTemplateNotFoundException(RmsTemplate.InternetConfidential.Name), ErrorCategory.InvalidArgument, null);
				}
			}
			if (base.Fields.IsModified("GenerateIncidentReport"))
			{
				GenerateIncidentReport generateIncidentReport = (GenerateIncidentReport)transportRule.Actions.FirstOrDefault((Action action) => action.GetType() == typeof(GenerateIncidentReport));
				if (generateIncidentReport != null && generateIncidentReport.IsLegacyFormat(null))
				{
					this.WriteWarning(Strings.LegacyIncludeOriginalMailParameterWillBeUpgraded);
				}
			}
			if (base.Fields.IsModified("RedirectMessageTo") && this.RedirectMessageTo != null)
			{
				int num;
				this.RedirectMessageTo = Utils.RemoveDuplicateRecipients(this.RedirectMessageTo, out num);
				if (num > 0)
				{
					this.WriteWarning(Strings.RemovedDuplicateRecipients(num, "RedirectMessageTo"));
				}
			}
			if (!Utils.ValidateConnectorParameter(base.Fields, base.DataSession, out exception3, out target2))
			{
				base.WriteError(exception3, ErrorCategory.InvalidArgument, target2);
				return;
			}
			if (!Utils.ValidateSentToScope(base.Fields, out exception3, out target2))
			{
				base.WriteError(exception3, ErrorCategory.InvalidArgument, target2);
				return;
			}
			if (!Utils.ValidateDomainIsPredicates(base.Fields, out exception3, out target2))
			{
				base.WriteError(exception3, ErrorCategory.InvalidArgument, target2);
				return;
			}
			if (!Utils.ValidateAttachmentExtensionMatchesWordParameter(base.Fields, out exception3, out target2))
			{
				base.WriteError(exception3, ErrorCategory.InvalidArgument, target2);
				return;
			}
			Rule rule = Rule.CreateFromInternalRule(this.supportedPredicates, this.supportedActions, transportRule, 0, this.DataObject);
			try
			{
				Utils.BuildConditionsAndExceptionsFromParameters(base.Fields, base.TenantGlobalCatalogSession, base.DataSession, rule.UseLegacyRegex, out this.conditionTypesToUpdate, out this.exceptionTypesToUpdate, out this.conditionsSetByParameters, out this.exceptionsSetByParameters);
				this.actionsSetByParameters = Utils.BuildActionsFromParameters(base.Fields, base.TenantGlobalCatalogSession, base.DataSession, out this.actionTypesToUpdate);
			}
			catch (TransientException exception5)
			{
				base.WriteError(exception5, ErrorCategory.InvalidArgument, this.Name);
			}
			catch (DataValidationException exception6)
			{
				base.WriteError(exception6, ErrorCategory.InvalidArgument, this.Name);
			}
			catch (ArgumentException exception7)
			{
				base.WriteError(exception7, ErrorCategory.InvalidArgument, this.Name);
			}
			PredicatesAndActionsWrapper predicatesAndActionsToVerify = this.GetPredicatesAndActionsToVerify();
			if (predicatesAndActionsToVerify.Actions != null)
			{
				if (predicatesAndActionsToVerify.Actions.FirstOrDefault((TransportRuleAction action) => action.GetType() == typeof(GenerateIncidentReportAction)) == null && base.Fields.IsModified("IncidentReportContent") && this.IncidentReportContent != null)
				{
					base.WriteError(new ArgumentException(Strings.InvalidIncidentReportContent), ErrorCategory.InvalidArgument, this.Name);
				}
				if (predicatesAndActionsToVerify.Actions.Any((TransportRuleAction action1) => action1.GetType() == typeof(NotifySenderAction)))
				{
					if (predicatesAndActionsToVerify.Actions.Any((TransportRuleAction action2) => action2.GetType() == typeof(RejectMessageAction)))
					{
						if (this.actionsSetByParameters.Any((TransportRuleAction ac) => ac.GetType() == typeof(NotifySenderAction)))
						{
							this.WriteWarning(Strings.RejectMessageActionIsBeingOverridded);
						}
						else
						{
							this.WriteWarning(Strings.NotifySenderActionIsBeingOverridded);
						}
					}
				}
				ArgumentException exception8;
				if (!Utils.ValidateNotifySender(predicatesAndActionsToVerify.Conditions, predicatesAndActionsToVerify.Exceptions, predicatesAndActionsToVerify.Actions, new Action<LocalizedString>(this.WriteWarning), out exception8))
				{
					base.WriteError(exception8, ErrorCategory.InvalidArgument, this.Name);
				}
				if (Utils.IsNotifySenderIgnoringRejectParameters(base.Fields))
				{
					this.RejectMessageEnhancedStatusCode = null;
					this.RejectMessageReasonText = null;
					this.WriteWarning(Strings.RejectMessageParameterWillBeIgnored);
				}
				foreach (TransportRuleAction transportRuleAction in predicatesAndActionsToVerify.Actions)
				{
					if (transportRuleAction is RejectMessageAction)
					{
						RejectMessageAction rejectMessageAction = (RejectMessageAction)transportRuleAction;
						string text = rejectMessageAction.EnhancedStatusCode.ToString();
						if (!Utils.IsCustomizedDsnConfigured(text))
						{
							this.WriteWarning(Strings.CustomizedDsnNotConfigured(text));
						}
					}
					else if (transportRuleAction is ApplyHtmlDisclaimerAction)
					{
						ApplyHtmlDisclaimerAction applyHtmlDisclaimerAction = (ApplyHtmlDisclaimerAction)transportRuleAction;
						string disclaimerText = applyHtmlDisclaimerAction.Text.ToString();
						string text2 = TransportUtils.CheckForInvalidMacroName(disclaimerText);
						if (!string.IsNullOrEmpty(text2))
						{
							base.WriteError(new ArgumentException(Strings.InvalidDisclaimerMacroName(text2)), ErrorCategory.InvalidArgument, this.Name);
							return;
						}
					}
					else if (transportRuleAction is RightsProtectMessageAction)
					{
						RmsTemplateDataProvider session2 = new RmsTemplateDataProvider((IConfigurationSession)base.DataSession);
						RightsProtectMessageAction rightsProtectMessageAction = (RightsProtectMessageAction)transportRuleAction;
						RmsTemplateIdentity template = rightsProtectMessageAction.Template;
						base.GetDataObject<RmsTemplatePresentation>(new RmsTemplateIdParameter(template), session2, null, new LocalizedString?(Strings.OutlookProtectionRuleRmsTemplateNotFound(template.TemplateName)), new LocalizedString?(Strings.OutlookProtectionRuleRmsTemplateNotUnique(template.TemplateName)));
						base.Fields["ResolvedRmsTemplateIdentity"] = template;
					}
					if (!Utils.ValidateSingletonAction(predicatesAndActionsToVerify.Actions) && Utils.ActionWhichMustBeSingleton.ContainsKey(transportRuleAction.GetType()))
					{
						base.WriteError(new ArgumentException(Utils.ActionWhichMustBeSingleton[transportRuleAction.GetType()]), ErrorCategory.InvalidArgument, this.Name);
						return;
					}
				}
			}
			Utils.ValidateTransportRuleRegexCpuTimeLimit((IConfigurationSession)base.DataSession, base.Fields);
			if (!Utils.ValidateActivationAndExpiryDates(new Action<LocalizedString>(this.WriteWarning), transportRule, base.Fields, out exception3, out target2))
			{
				base.WriteError(exception3, ErrorCategory.InvalidArgument, target2);
			}
		}

		protected override void InternalProcessRecord()
		{
			ADRuleStorageManager adruleStorageManager;
			try
			{
				IConfigDataProvider session = new MessagingPoliciesSyncLogDataSession(base.DataSession, null, null);
				adruleStorageManager = new ADRuleStorageManager(this.ruleCollectionName, session);
			}
			catch (RuleCollectionNotInAdException)
			{
				base.WriteError(new ArgumentException(Strings.RuleNotFound(this.Identity.ToString()), "Identity"), ErrorCategory.InvalidArgument, this.Identity);
				return;
			}
			try
			{
				if (base.Fields.IsModified("Priority"))
				{
					this.SetRuleWithPriorityChange(adruleStorageManager);
				}
				else
				{
					this.SetRuleWithoutPriorityChange(adruleStorageManager);
				}
				if (Utils.Exchange12HubServersExist(this))
				{
					this.WriteWarning(Strings.SetRuleSyncAcrossDifferentVersionsNeeded);
				}
			}
			catch (ParserException exception)
			{
				base.WriteError(exception, ErrorCategory.InvalidData, null);
			}
		}

		private PredicatesAndActionsWrapper GetPredicatesAndActionsToVerify()
		{
			if (this.Actions != null)
			{
				return new PredicatesAndActionsWrapper(this.Conditions, this.Exceptions, this.Actions);
			}
			TransportRule transportRule = (TransportRule)TransportRuleParser.Instance.GetRule(this.DataObject.Xml);
			Rule rule = Rule.CreateFromInternalRule(this.supportedPredicates, this.supportedActions, transportRule, 0, this.DataObject);
			try
			{
				this.UpdateRuleFromParameters(rule);
			}
			catch (ArgumentException exception)
			{
				base.WriteError(exception, ErrorCategory.InvalidArgument, this.Name);
			}
			TransportRuleAction[] actions = rule.Actions;
			TransportRulePredicate[] conditions = this.Conditions ?? rule.Conditions;
			TransportRulePredicate[] exceptions = this.Exceptions ?? rule.Exceptions;
			if (!base.Fields.IsModified("RuleErrorAction"))
			{
				this.RuleErrorAction = transportRule.ErrorAction;
			}
			if (!base.Fields.IsModified("SenderAddressLocation"))
			{
				this.SenderAddressLocation = transportRule.SenderAddressLocation;
			}
			if (!base.Fields.IsModified("RuleSubType"))
			{
				this.RuleSubType = transportRule.SubType;
			}
			ArgumentException exception2;
			if (!Utils.ValidateSubtypes(this.RuleSubType, conditions, exceptions, actions, out exception2))
			{
				base.WriteError(exception2, ErrorCategory.InvalidArgument, this.Name);
			}
			return new PredicatesAndActionsWrapper(conditions, exceptions, actions);
		}

		private void SetRuleWithPriorityChange(ADRuleStorageManager storedRules)
		{
			storedRules.LoadRuleCollection();
			TransportRule transportRule;
			int priority;
			storedRules.GetRule(this.DataObject.Identity, out transportRule, out priority);
			TransportRule dataObject = this.DataObject;
			if (transportRule == null)
			{
				base.WriteError(new ArgumentException(Strings.RuleNotFound(this.Identity.ToString()), "Identity"), ErrorCategory.InvalidArgument, this.Identity);
				return;
			}
			Rule rule = Rule.CreateFromInternalRule(this.supportedPredicates, this.supportedActions, transportRule, priority, dataObject);
			if (rule.ManuallyModified)
			{
				if (!this.UpdateManuallyModifiedInternalRuleFromParameters(transportRule))
				{
					return;
				}
				rule.Priority = this.Priority;
			}
			else
			{
				try
				{
					this.UpdateRuleFromParameters(rule);
					transportRule = rule.ToInternalRule();
				}
				catch (ArgumentException exception)
				{
					base.WriteError(exception, ErrorCategory.InvalidArgument, null);
					return;
				}
				catch (RulesValidationException exception2)
				{
					base.WriteError(exception2, ErrorCategory.InvalidArgument, null);
					return;
				}
			}
			try
			{
				OrganizationId organizationId = this.DataObject.OrganizationId;
				if (organizationId != OrganizationId.ForestWideOrgId)
				{
					InvalidOperationException ex = Utils.CheckRuleForOrganizationLimits((IConfigurationSession)base.DataSession, base.TenantGlobalCatalogSession, storedRules, organizationId, transportRule, false);
					if (ex != null)
					{
						base.WriteError(ex, ErrorCategory.InvalidOperation, this.Name);
						return;
					}
				}
				storedRules.UpdateRule(transportRule, rule.Identity, rule.Priority);
			}
			catch (RulesValidationException)
			{
				base.WriteError(new ArgumentException(Strings.RuleNameAlreadyExist, "Name"), ErrorCategory.InvalidArgument, this.Name);
			}
			catch (InvalidPriorityException exception3)
			{
				base.WriteError(exception3, ErrorCategory.InvalidArgument, null);
			}
		}

		private void SetRuleWithoutPriorityChange(ADRuleStorageManager storedRules)
		{
			TransportRule transportRule = (TransportRule)TransportRuleParser.Instance.GetRule(this.DataObject.Xml);
			Rule rule = Rule.CreateFromInternalRule(this.supportedPredicates, this.supportedActions, transportRule, -1, this.DataObject);
			string name = rule.Name;
			if (rule.ManuallyModified)
			{
				transportRule.Name = rule.Name;
				if (!this.UpdateManuallyModifiedInternalRuleFromParameters(transportRule))
				{
					return;
				}
			}
			else
			{
				try
				{
					this.UpdateRuleFromParameters(rule);
					transportRule = rule.ToInternalRule();
				}
				catch (ArgumentException exception)
				{
					base.WriteError(exception, ErrorCategory.InvalidArgument, null);
					return;
				}
				catch (RulesValidationException exception2)
				{
					base.WriteError(exception2, ErrorCategory.InvalidArgument, null);
					return;
				}
			}
			OrganizationId organizationId = this.DataObject.OrganizationId;
			if (organizationId != OrganizationId.ForestWideOrgId)
			{
				storedRules.LoadRuleCollection();
				InvalidOperationException ex = Utils.CheckRuleForOrganizationLimits((IConfigurationSession)base.DataSession, base.TenantGlobalCatalogSession, storedRules, organizationId, transportRule, false);
				if (ex != null)
				{
					base.WriteError(ex, ErrorCategory.InvalidOperation, this.Name);
					return;
				}
			}
			string xml = TransportRuleSerializer.Instance.SaveRuleToString(transportRule);
			this.DataObject.Xml = xml;
			if (!storedRules.CanRename((ADObjectId)this.DataObject.Identity, name, transportRule.Name))
			{
				base.WriteError(new ArgumentException(Strings.RuleNameAlreadyExist, "Name"), ErrorCategory.InvalidArgument, this.Name);
				return;
			}
			this.DataObject.Name = transportRule.Name;
			storedRules.UpdateRule(this.DataObject);
		}

		private void UpdateRuleFromParameters(Rule rule)
		{
			if (this.Name != null)
			{
				rule.Name = this.Name;
			}
			if (base.Fields.IsModified("Priority"))
			{
				rule.Priority = this.Priority;
			}
			if (this.Comments != null)
			{
				rule.Comments = this.Comments;
			}
			if (base.Fields.IsModified("RuleErrorAction"))
			{
				rule.RuleErrorAction = this.RuleErrorAction;
			}
			if (base.Fields.IsModified("SenderAddressLocation"))
			{
				rule.SenderAddressLocation = this.SenderAddressLocation;
			}
			if (base.Fields.IsModified("RuleSubType"))
			{
				rule.RuleSubType = this.RuleSubType;
			}
			if (base.Fields.IsModified("ActivationDate"))
			{
				rule.ActivationDate = ((this.ActivationDate != null && this.ActivationDate.Value.ToUniversalTime() < DateTime.UtcNow) ? null : this.ActivationDate);
			}
			if (base.Fields.IsModified("ExpiryDate"))
			{
				rule.ExpiryDate = this.ExpiryDate;
			}
			if (this.DlpPolicy != null)
			{
				if (this.DlpPolicy.Equals(string.Empty))
				{
					rule.DlpPolicy = null;
					rule.DlpPolicyId = Guid.Empty;
				}
				else
				{
					rule.DlpPolicyId = this.dlpPolicyId;
					ADComplianceProgram adcomplianceProgram = DlpUtils.GetInstalledTenantDlpPolicies(base.DataSession, this.DlpPolicy).First<ADComplianceProgram>();
					rule.DlpPolicy = adcomplianceProgram.Name;
					Tuple<RuleState, RuleMode> tuple = DlpUtils.DlpStateToRuleState(adcomplianceProgram.State);
					rule.State = tuple.Item1;
					if (base.Fields.IsModified("Mode") && rule.Mode != tuple.Item2)
					{
						this.WriteWarning(Strings.DlpPolicyModeIsOverridenByModeParameter(this.Mode.ToString(), tuple.Item2.ToString()));
					}
					if (!base.Fields.IsModified("Mode"))
					{
						rule.Mode = tuple.Item2;
					}
				}
			}
			if (base.Fields.IsModified("Mode"))
			{
				rule.Mode = this.Mode;
			}
			if (this.Conditions != null)
			{
				rule.Conditions = this.Conditions;
			}
			else
			{
				List<TransportRulePredicate> list = new List<TransportRulePredicate>();
				if (rule.Conditions != null)
				{
					foreach (TransportRulePredicate transportRulePredicate in rule.Conditions)
					{
						if (!this.conditionTypesToUpdate.Contains(transportRulePredicate.GetType()))
						{
							Utils.InsertPredicateSorted(transportRulePredicate, list);
						}
					}
				}
				foreach (TransportRulePredicate predicate in this.conditionsSetByParameters)
				{
					Utils.InsertPredicateSorted(predicate, list);
				}
				if (list.Count > 0)
				{
					rule.Conditions = list.ToArray();
				}
				else
				{
					rule.Conditions = null;
				}
			}
			if (this.Exceptions != null)
			{
				rule.Exceptions = this.Exceptions;
			}
			else
			{
				List<TransportRulePredicate> list2 = new List<TransportRulePredicate>();
				if (rule.Exceptions != null)
				{
					foreach (TransportRulePredicate transportRulePredicate2 in rule.Exceptions)
					{
						if (!this.exceptionTypesToUpdate.Contains(transportRulePredicate2.GetType()))
						{
							Utils.InsertPredicateSorted(transportRulePredicate2, list2);
						}
					}
				}
				foreach (TransportRulePredicate predicate2 in this.exceptionsSetByParameters)
				{
					Utils.InsertPredicateSorted(predicate2, list2);
				}
				if (list2.Count > 0)
				{
					rule.Exceptions = list2.ToArray();
				}
				else
				{
					rule.Exceptions = null;
				}
			}
			if (this.Actions != null)
			{
				rule.Actions = this.Actions;
				return;
			}
			SetTransportRule.UpdateActionsFromParameters(this.actionsSetByParameters, this.actionTypesToUpdate, rule);
		}

		internal static void UpdateActionsFromParameters(IEnumerable<TransportRuleAction> actionsSetByParameters, List<Type> actionsToBeUpdated, Rule rule)
		{
			List<TransportRuleAction> list = new List<TransportRuleAction>();
			if (rule.Actions != null)
			{
				foreach (TransportRuleAction transportRuleAction in rule.Actions)
				{
					if (!actionsToBeUpdated.Contains(transportRuleAction.GetType()) && !SetTransportRule.IsActionOverriden(transportRuleAction, actionsSetByParameters))
					{
						Utils.InsertActionSorted(transportRuleAction, list);
					}
				}
			}
			foreach (TransportRuleAction action in actionsSetByParameters)
			{
				Utils.InsertActionSorted(action, list);
			}
			if (list.Count > 0)
			{
				rule.Actions = list.ToArray();
			}
		}

		private bool UpdateManuallyModifiedInternalRuleFromParameters(TransportRule rule)
		{
			if (this.Name != null)
			{
				rule.Name = this.Name;
			}
			if (this.Comments != null)
			{
				rule.Comments = this.Comments;
			}
			if (this.Conditions != null)
			{
				base.WriteError(new ArgumentException(Strings.CannotEditManuallyModifiedRule, "Conditions"), ErrorCategory.InvalidArgument, this.Conditions);
				return false;
			}
			if (this.Exceptions != null)
			{
				base.WriteError(new ArgumentException(Strings.CannotEditManuallyModifiedRule, "Exceptions"), ErrorCategory.InvalidArgument, this.Exceptions);
				return false;
			}
			if (this.Actions != null)
			{
				base.WriteError(new ArgumentException(Strings.CannotEditManuallyModifiedRule, "Actions"), ErrorCategory.InvalidArgument, this.Actions);
				return false;
			}
			return true;
		}

		protected override bool IsKnownException(Exception exception)
		{
			return base.IsKnownException(exception) || RmsUtil.IsKnownException(exception) || exception is ValidationArgumentException;
		}

		internal static bool IsActionOverriden(TransportRuleAction action, IEnumerable<TransportRuleAction> actionsSetByParameters)
		{
			bool flag = actionsSetByParameters.Any((TransportRuleAction ac) => ac.GetType() == typeof(NotifySenderAction));
			if (action.GetType() == typeof(RejectMessageAction) && flag)
			{
				return true;
			}
			bool flag2 = actionsSetByParameters.Any((TransportRuleAction ac) => ac.GetType() == typeof(RejectMessageAction));
			return action.GetType() == typeof(NotifySenderAction) && flag2;
		}

		private readonly string ruleCollectionName;

		private readonly TypeMapping[] supportedPredicates;

		private readonly TypeMapping[] supportedActions;

		private TransportRulePredicate[] conditionsSetByParameters;

		private TransportRulePredicate[] exceptionsSetByParameters;

		private TransportRuleAction[] actionsSetByParameters;

		private List<Type> conditionTypesToUpdate;

		private List<Type> exceptionTypesToUpdate;

		private List<Type> actionTypesToUpdate;

		private Guid dlpPolicyId;
	}
}
