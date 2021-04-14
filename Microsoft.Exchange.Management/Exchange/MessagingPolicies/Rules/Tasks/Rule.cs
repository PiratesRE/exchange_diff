using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Core.RuleTasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.RightsManagement;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.MessagingPolicies.CompliancePrograms.Tasks;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Tasks
{
	[Serializable]
	public class Rule : RulePresentationObjectBase
	{
		public Rule()
		{
		}

		internal Rule(TransportRule transportRule, string name, int priority, string dlpPolicy, Guid dlpPolicyId, string comments, bool manuallyModified, DateTime? activationDate, DateTime? expiryDate, TransportRulePredicate[] conditions, TransportRulePredicate[] exceptions, TransportRuleAction[] actions, RuleState state, RuleMode mode, RuleSubType subType, RuleErrorAction ruleErrorAction, SenderAddressLocation senderAddressLocation = SenderAddressLocation.Header) : base(transportRule)
		{
			if (transportRule == null)
			{
				base.Name = name;
			}
			this.priority = priority;
			this.dlpPolicy = dlpPolicy;
			this.dlpPolicyId = dlpPolicyId;
			this.comments = comments;
			this.manuallyModified = manuallyModified;
			this.activationDate = activationDate;
			this.expiryDate = expiryDate;
			this.conditions = conditions;
			this.exceptions = exceptions;
			this.actions = actions;
			this.State = state;
			this.Mode = mode;
			this.RuleErrorAction = ruleErrorAction;
			this.SenderAddressLocation = senderAddressLocation;
			this.RuleSubType = subType;
			OrganizationId orgId = null;
			if (transportRule != null)
			{
				orgId = transportRule.OrganizationId;
			}
			if (this.conditions != null)
			{
				foreach (TransportRulePredicate predicate in this.conditions)
				{
					this.SetParametersFromPredicate(predicate, false, orgId);
				}
			}
			if (this.exceptions != null)
			{
				foreach (TransportRulePredicate predicate2 in this.exceptions)
				{
					this.SetParametersFromPredicate(predicate2, true, orgId);
				}
			}
			if (this.actions != null)
			{
				foreach (TransportRuleAction parametersFromAction in this.actions)
				{
					this.SetParametersFromAction(parametersFromAction);
				}
			}
		}

		internal ObjectSchema Schema
		{
			get
			{
				return Rule.schema;
			}
		}

		public int Priority
		{
			get
			{
				return this.priority;
			}
			set
			{
				if (value < 0)
				{
					throw new ArgumentException(RulesTasksStrings.NegativePriority, "Priority");
				}
				this.priority = value;
			}
		}

		public string DlpPolicy
		{
			get
			{
				return this.dlpPolicy;
			}
			set
			{
				this.dlpPolicy = value;
			}
		}

		public Guid DlpPolicyId
		{
			get
			{
				return this.dlpPolicyId;
			}
			set
			{
				this.dlpPolicyId = value;
			}
		}

		public string Comments
		{
			get
			{
				return this.comments;
			}
			set
			{
				ArgumentException ex;
				if (!Utils.ValidateRuleComments(value, out ex))
				{
					throw ex;
				}
				this.comments = value;
			}
		}

		public bool ManuallyModified
		{
			get
			{
				return this.manuallyModified;
			}
		}

		public DateTime? ActivationDate
		{
			get
			{
				return this.activationDate;
			}
			set
			{
				this.activationDate = value;
			}
		}

		public DateTime? ExpiryDate
		{
			get
			{
				return this.expiryDate;
			}
			set
			{
				this.expiryDate = value;
			}
		}

		public RuleDescription Description
		{
			get
			{
				return Utils.BuildRuleDescription(this, 200);
			}
		}

		public Version RuleVersion
		{
			get
			{
				if (string.IsNullOrEmpty(base.TransportRuleXml))
				{
					return null;
				}
				Version result;
				try
				{
					TransportRule transportRule = (TransportRule)TransportRuleParser.Instance.GetRule(base.TransportRuleXml);
					if (transportRule.IsTooAdvancedToParse)
					{
						result = null;
					}
					else
					{
						result = transportRule.MinimumVersion;
					}
				}
				catch (ParserException)
				{
					result = null;
				}
				return result;
			}
		}

		public TransportRulePredicate[] Conditions
		{
			get
			{
				return this.conditions;
			}
			set
			{
				if (value != null)
				{
					Rule.ValidatePhraseArray(value, true, true);
				}
				this.conditions = value;
			}
		}

		public TransportRulePredicate[] Exceptions
		{
			get
			{
				return this.exceptions;
			}
			set
			{
				if (value != null)
				{
					Rule.ValidatePhraseArray(value, true, true);
				}
				this.exceptions = value;
			}
		}

		public TransportRuleAction[] Actions
		{
			get
			{
				return this.actions;
			}
			set
			{
				if (value == null || value.Length == 0)
				{
					throw new ArgumentException(RulesTasksStrings.NoAction);
				}
				Rule.ValidatePhraseArray(value, false, true);
				this.actions = value;
			}
		}

		public RuleState State { get; set; }

		public RuleMode Mode { get; set; }

		public RuleErrorAction RuleErrorAction { get; set; }

		public SenderAddressLocation SenderAddressLocation { get; set; }

		public RuleSubType RuleSubType { get; set; }

		public bool UseLegacyRegex
		{
			get
			{
				if (this.conditions != null)
				{
					if (this.Conditions.Any((TransportRulePredicate predicate) => Utils.IsLegacyRegexPredicate(predicate)))
					{
						return true;
					}
				}
				if (this.exceptions != null)
				{
					if (this.Exceptions.Any((TransportRulePredicate predicate) => Utils.IsLegacyRegexPredicate(predicate)))
					{
						return true;
					}
				}
				return false;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.FromAddressesDisplayName)]
		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.FromAddressesDescription)]
		public RecipientIdParameter[] From
		{
			get
			{
				return this.from;
			}
			set
			{
				this.from = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.FromDLAddressDescription)]
		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.FromDLAddressDisplayName)]
		public RecipientIdParameter[] FromMemberOf
		{
			get
			{
				return this.fromMemberOf;
			}
			set
			{
				this.fromMemberOf = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.FromScopeDisplayName)]
		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.FromScopeDescription)]
		public FromUserScope? FromScope
		{
			get
			{
				return this.fromScope;
			}
			set
			{
				this.fromScope = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.ToAddressesDescription)]
		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.ToAddressesDisplayName)]
		public RecipientIdParameter[] SentTo
		{
			get
			{
				return this.sentTo;
			}
			set
			{
				this.sentTo = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.ToDLAddressDescription)]
		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.ToDLAddressDisplayName)]
		public RecipientIdParameter[] SentToMemberOf
		{
			get
			{
				return this.sentToMemberOf;
			}
			set
			{
				this.sentToMemberOf = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.ToScopeDisplayName)]
		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.ToScopeDescription)]
		public ToUserScope? SentToScope
		{
			get
			{
				return this.sentToScope;
			}
			set
			{
				this.sentToScope = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.ToDLAddressDescription)]
		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.ToDLAddressDisplayName)]
		public RecipientIdParameter[] BetweenMemberOf1
		{
			get
			{
				return this.betweenMemberOf1;
			}
			set
			{
				this.betweenMemberOf1 = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.ToDLAddressDescription)]
		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.ToDLAddressDisplayName)]
		public RecipientIdParameter[] BetweenMemberOf2
		{
			get
			{
				return this.betweenMemberOf2;
			}
			set
			{
				this.betweenMemberOf2 = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.ToAddressesDescription)]
		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.ToAddressesDisplayName)]
		public RecipientIdParameter[] ManagerAddresses
		{
			get
			{
				return this.managerAddresses;
			}
			set
			{
				this.managerAddresses = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.EvaluatedUserDisplayName)]
		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.EvaluatedUserDescription)]
		public EvaluatedUser? ManagerForEvaluatedUser
		{
			get
			{
				return this.managerForEvaluatedUser;
			}
			set
			{
				this.managerForEvaluatedUser = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.ManagementRelationshipDisplayName)]
		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.ManagementRelationshipDescription)]
		public ManagementRelationship? SenderManagementRelationship
		{
			get
			{
				return this.senderManagementRelationship;
			}
			set
			{
				this.senderManagementRelationship = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.ADAttributeDescription)]
		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.ADAttributeDisplayName)]
		public ADAttribute? ADComparisonAttribute
		{
			get
			{
				return this.adComparisonAttribute;
			}
			set
			{
				this.adComparisonAttribute = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.EvaluationDescription)]
		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.EvaluationDisplayName)]
		public Evaluation? ADComparisonOperator
		{
			get
			{
				return this.adComparisonOperator;
			}
			set
			{
				this.adComparisonOperator = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.WordsDisplayName)]
		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.WordsDescription)]
		public Word[] SenderADAttributeContainsWords
		{
			get
			{
				return this.senderADAttributeContainsWords;
			}
			set
			{
				this.senderADAttributeContainsWords = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.TextPatternsDisplayName)]
		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.TextPatternsDescription)]
		public Pattern[] SenderADAttributeMatchesPatterns
		{
			get
			{
				return this.senderADAttributeMatchesPatterns;
			}
			set
			{
				this.senderADAttributeMatchesPatterns = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.WordsDescription)]
		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.WordsDisplayName)]
		public Word[] RecipientADAttributeContainsWords
		{
			get
			{
				return this.recipientADAttributeContainsWords;
			}
			set
			{
				this.recipientADAttributeContainsWords = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.TextPatternsDescription)]
		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.TextPatternsDisplayName)]
		public Pattern[] RecipientADAttributeMatchesPatterns
		{
			get
			{
				return this.recipientADAttributeMatchesPatterns;
			}
			set
			{
				this.recipientADAttributeMatchesPatterns = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.ToAddressesDescription)]
		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.ToAddressesDisplayName)]
		public RecipientIdParameter[] AnyOfToHeader
		{
			get
			{
				return this.anyOfToHeader;
			}
			set
			{
				this.anyOfToHeader = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.ToDLAddressDescription)]
		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.ToDLAddressDisplayName)]
		public RecipientIdParameter[] AnyOfToHeaderMemberOf
		{
			get
			{
				return this.anyOfToHeaderMemberOf;
			}
			set
			{
				this.anyOfToHeaderMemberOf = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.ToAddressesDisplayName)]
		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.ToAddressesDescription)]
		public RecipientIdParameter[] AnyOfCcHeader
		{
			get
			{
				return this.anyOfCcHeader;
			}
			set
			{
				this.anyOfCcHeader = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.ToDLAddressDisplayName)]
		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.ToDLAddressDescription)]
		public RecipientIdParameter[] AnyOfCcHeaderMemberOf
		{
			get
			{
				return this.anyOfCcHeaderMemberOf;
			}
			set
			{
				this.anyOfCcHeaderMemberOf = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.ToAddressesDescription)]
		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.ToAddressesDisplayName)]
		public RecipientIdParameter[] AnyOfToCcHeader
		{
			get
			{
				return this.anyOfToCcHeader;
			}
			set
			{
				this.anyOfToCcHeader = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.ToDLAddressDisplayName)]
		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.ToDLAddressDescription)]
		public RecipientIdParameter[] AnyOfToCcHeaderMemberOf
		{
			get
			{
				return this.anyOfToCcHeaderMemberOf;
			}
			set
			{
				this.anyOfToCcHeaderMemberOf = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.ClassificationDisplayName)]
		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.ClassificationDescription)]
		public ADObjectId HasClassification
		{
			get
			{
				return this.hasClassification;
			}
			set
			{
				this.hasClassification = value;
			}
		}

		public bool HasNoClassification
		{
			get
			{
				return this.hasNoClassification;
			}
			set
			{
				this.hasNoClassification = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.WordsDisplayName)]
		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.WordsDescription)]
		public Word[] SubjectContainsWords
		{
			get
			{
				return this.subjectContainsWords;
			}
			set
			{
				this.subjectContainsWords = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.WordsDisplayName)]
		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.WordsDescription)]
		public Word[] SubjectOrBodyContainsWords
		{
			get
			{
				return this.subjectOrBodyContainsWords;
			}
			set
			{
				this.subjectOrBodyContainsWords = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.MessageHeaderDescription)]
		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.MessageHeaderDisplayName)]
		public HeaderName? HeaderContainsMessageHeader
		{
			get
			{
				return this.headerContainsMessageHeader;
			}
			set
			{
				this.headerContainsMessageHeader = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.WordsDisplayName)]
		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.WordsDescription)]
		public Word[] HeaderContainsWords
		{
			get
			{
				return this.headerContainsWords;
			}
			set
			{
				this.headerContainsWords = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.WordsDisplayName)]
		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.WordsDescription)]
		public Word[] FromAddressContainsWords
		{
			get
			{
				return this.fromAddressContainsWords;
			}
			set
			{
				this.fromAddressContainsWords = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.WordsDisplayName)]
		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.WordsDescription)]
		public Word[] SenderDomainIs
		{
			get
			{
				return this.senderDomainIs;
			}
			set
			{
				this.senderDomainIs = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.WordsDisplayName)]
		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.WordsDescription)]
		public Word[] RecipientDomainIs
		{
			get
			{
				return this.recipientDomainIs;
			}
			set
			{
				this.recipientDomainIs = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.TextPatternsDescription)]
		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.TextPatternsDisplayName)]
		public Pattern[] SubjectMatchesPatterns
		{
			get
			{
				return this.subjectMatchesPatterns;
			}
			set
			{
				this.subjectMatchesPatterns = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.TextPatternsDescription)]
		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.TextPatternsDisplayName)]
		public Pattern[] SubjectOrBodyMatchesPatterns
		{
			get
			{
				return this.subjectOrBodyMatchesPatterns;
			}
			set
			{
				this.subjectOrBodyMatchesPatterns = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.MessageHeaderDisplayName)]
		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.MessageHeaderDescription)]
		public HeaderName? HeaderMatchesMessageHeader
		{
			get
			{
				return this.headerMatchesMessageHeader;
			}
			set
			{
				this.headerMatchesMessageHeader = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.TextPatternsDescription)]
		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.TextPatternsDisplayName)]
		public Pattern[] HeaderMatchesPatterns
		{
			get
			{
				return this.headerMatchesPatterns;
			}
			set
			{
				this.headerMatchesPatterns = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.TextPatternsDescription)]
		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.TextPatternsDisplayName)]
		public Pattern[] FromAddressMatchesPatterns
		{
			get
			{
				return this.fromAddressMatchesPatterns;
			}
			set
			{
				this.fromAddressMatchesPatterns = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.TextPatternsDescription)]
		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.TextPatternsDisplayName)]
		public Pattern[] AttachmentNameMatchesPatterns
		{
			get
			{
				return this.attachmentNameMatchesPatterns;
			}
			set
			{
				this.attachmentNameMatchesPatterns = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.WordsDescription)]
		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.WordsDisplayName)]
		public Word[] AttachmentExtensionMatchesWords
		{
			get
			{
				return this.attachmentExtensionMatchesWords;
			}
			set
			{
				this.attachmentExtensionMatchesWords = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.WordsDescription)]
		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.WordsDisplayName)]
		public Word[] AttachmentPropertyContainsWords
		{
			get
			{
				return this.attachmentPropertyContainsWords;
			}
			set
			{
				this.attachmentPropertyContainsWords = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.WordsDescription)]
		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.WordsDisplayName)]
		public Word[] ContentCharacterSetContainsWords
		{
			get
			{
				return this.contentCharacterSetContainsWords;
			}
			set
			{
				this.contentCharacterSetContainsWords = value;
			}
		}

		public bool HasSenderOverride
		{
			get
			{
				return this.hasSenderOverride;
			}
			set
			{
				this.hasSenderOverride = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.MessageDataClassificationDisplayName)]
		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.MessageDataClassificationDescription)]
		public string[] MessageContainsDataClassifications
		{
			get
			{
				return this.messageContainsDataClassifications;
			}
			set
			{
				this.messageContainsDataClassifications = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.SenderIpRangesDisplayName)]
		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.SenderIpRangesDescription)]
		public MultiValuedProperty<IPRange> SenderIpRanges
		{
			get
			{
				return this.senderIpRanges;
			}
			set
			{
				this.senderIpRanges = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.SclValueDescription)]
		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.SclValueDisplayName)]
		public SclValue? SCLOver
		{
			get
			{
				return this.sclOver;
			}
			set
			{
				this.sclOver = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.AttachmentSizeDisplayName)]
		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.AttachmentSizeDescription)]
		public ByteQuantifiedSize? AttachmentSizeOver
		{
			get
			{
				return this.attachmentSizeOver;
			}
			set
			{
				this.attachmentSizeOver = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.MessageSizeDescription)]
		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.MessageSizeDisplayName)]
		public ByteQuantifiedSize? MessageSizeOver
		{
			get
			{
				return this.messageSizeOver;
			}
			set
			{
				this.messageSizeOver = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.ImportanceDescription)]
		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.ImportanceDisplayName)]
		public Importance? WithImportance
		{
			get
			{
				return this.withImportance;
			}
			set
			{
				this.withImportance = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.MessageTypeDisplayName)]
		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.MessageTypeDescription)]
		public MessageType? MessageTypeMatches
		{
			get
			{
				return this.messageTypeMatches;
			}
			set
			{
				this.messageTypeMatches = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.WordsDescription)]
		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.WordsDisplayName)]
		public Word[] RecipientAddressContainsWords
		{
			get
			{
				return this.recipientAddressContainsWords;
			}
			set
			{
				this.recipientAddressContainsWords = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.TextPatternsDescription)]
		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.TextPatternsDisplayName)]
		public Pattern[] RecipientAddressMatchesPatterns
		{
			get
			{
				return this.recipientAddressMatchesPatterns;
			}
			set
			{
				this.recipientAddressMatchesPatterns = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.ListsDisplayName)]
		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.ListsDescription)]
		public Word[] SenderInRecipientList
		{
			get
			{
				return this.senderInRecipientList;
			}
			set
			{
				this.senderInRecipientList = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.ListsDescription)]
		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.ListsDisplayName)]
		public Word[] RecipientInSenderList
		{
			get
			{
				return this.recipientInSenderList;
			}
			set
			{
				this.recipientInSenderList = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.WordsDescription)]
		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.WordsDisplayName)]
		public Word[] AttachmentContainsWords
		{
			get
			{
				return this.attachmentContainsWords;
			}
			set
			{
				this.attachmentContainsWords = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.TextPatternsDisplayName)]
		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.TextPatternsDescription)]
		public Pattern[] AttachmentMatchesPatterns
		{
			get
			{
				return this.attachmentMatchesPatterns;
			}
			set
			{
				this.attachmentMatchesPatterns = value;
			}
		}

		public bool AttachmentIsUnsupported
		{
			get
			{
				return this.attachmentIsUnsupported;
			}
			set
			{
				this.attachmentIsUnsupported = value;
			}
		}

		public bool AttachmentProcessingLimitExceeded
		{
			get
			{
				return this.attachmentProcessingLimitExceeded;
			}
			set
			{
				this.attachmentProcessingLimitExceeded = value;
			}
		}

		public bool AttachmentHasExecutableContent
		{
			get
			{
				return this.attachmentHasExecutableContent;
			}
			set
			{
				this.attachmentHasExecutableContent = value;
			}
		}

		public bool AttachmentIsPasswordProtected
		{
			get
			{
				return this.attachmentIsPasswordProtected;
			}
			set
			{
				this.attachmentIsPasswordProtected = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.WordsDescription)]
		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.WordsDisplayName)]
		public Word[] AnyOfRecipientAddressContainsWords
		{
			get
			{
				return this.anyOfRecipientAddressContainsWords;
			}
			set
			{
				this.anyOfRecipientAddressContainsWords = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.TextPatternsDescription)]
		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.TextPatternsDisplayName)]
		public Pattern[] AnyOfRecipientAddressMatchesPatterns
		{
			get
			{
				return this.anyOfRecipientAddressMatchesPatterns;
			}
			set
			{
				this.anyOfRecipientAddressMatchesPatterns = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.FromAddressesDisplayName)]
		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.FromAddressesDescription)]
		public RecipientIdParameter[] ExceptIfFrom
		{
			get
			{
				return this.exceptIfFrom;
			}
			set
			{
				this.exceptIfFrom = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.FromDLAddressDisplayName)]
		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.FromDLAddressDescription)]
		public RecipientIdParameter[] ExceptIfFromMemberOf
		{
			get
			{
				return this.exceptIfFromMemberOf;
			}
			set
			{
				this.exceptIfFromMemberOf = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.FromScopeDisplayName)]
		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.FromScopeDescription)]
		public FromUserScope? ExceptIfFromScope
		{
			get
			{
				return this.exceptIfFromScope;
			}
			set
			{
				this.exceptIfFromScope = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.ToAddressesDisplayName)]
		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.ToAddressesDescription)]
		public RecipientIdParameter[] ExceptIfSentTo
		{
			get
			{
				return this.exceptIfSentTo;
			}
			set
			{
				this.exceptIfSentTo = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.ToDLAddressDisplayName)]
		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.ToDLAddressDescription)]
		public RecipientIdParameter[] ExceptIfSentToMemberOf
		{
			get
			{
				return this.exceptIfSentToMemberOf;
			}
			set
			{
				this.exceptIfSentToMemberOf = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.ToScopeDisplayName)]
		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.ToScopeDescription)]
		public ToUserScope? ExceptIfSentToScope
		{
			get
			{
				return this.exceptIfSentToScope;
			}
			set
			{
				this.exceptIfSentToScope = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.ToDLAddressDisplayName)]
		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.ToDLAddressDescription)]
		public RecipientIdParameter[] ExceptIfBetweenMemberOf1
		{
			get
			{
				return this.exceptIfBetweenMemberOf1;
			}
			set
			{
				this.exceptIfBetweenMemberOf1 = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.ToDLAddressDescription)]
		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.ToDLAddressDisplayName)]
		public RecipientIdParameter[] ExceptIfBetweenMemberOf2
		{
			get
			{
				return this.exceptIfBetweenMemberOf2;
			}
			set
			{
				this.exceptIfBetweenMemberOf2 = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.ToAddressesDescription)]
		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.ToAddressesDisplayName)]
		public RecipientIdParameter[] ExceptIfManagerAddresses
		{
			get
			{
				return this.exceptIfManagerAddresses;
			}
			set
			{
				this.exceptIfManagerAddresses = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.EvaluatedUserDescription)]
		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.EvaluatedUserDisplayName)]
		public EvaluatedUser? ExceptIfManagerForEvaluatedUser
		{
			get
			{
				return this.exceptIfManagerForEvaluatedUser;
			}
			set
			{
				this.exceptIfManagerForEvaluatedUser = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.ManagementRelationshipDescription)]
		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.ManagementRelationshipDisplayName)]
		public ManagementRelationship? ExceptIfSenderManagementRelationship
		{
			get
			{
				return this.exceptIfSenderManagementRelationship;
			}
			set
			{
				this.exceptIfSenderManagementRelationship = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.ADAttributeDisplayName)]
		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.ADAttributeDescription)]
		public ADAttribute? ExceptIfADComparisonAttribute
		{
			get
			{
				return this.exceptIfADComparisonAttribute;
			}
			set
			{
				this.exceptIfADComparisonAttribute = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.EvaluationDescription)]
		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.EvaluationDisplayName)]
		public Evaluation? ExceptIfADComparisonOperator
		{
			get
			{
				return this.exceptIfADComparisonOperator;
			}
			set
			{
				this.exceptIfADComparisonOperator = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.WordsDescription)]
		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.WordsDisplayName)]
		public Word[] ExceptIfSenderADAttributeContainsWords
		{
			get
			{
				return this.exceptIfSenderADAttributeContainsWords;
			}
			set
			{
				this.exceptIfSenderADAttributeContainsWords = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.TextPatternsDescription)]
		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.TextPatternsDisplayName)]
		public Pattern[] ExceptIfSenderADAttributeMatchesPatterns
		{
			get
			{
				return this.exceptIfSenderADAttributeMatchesPatterns;
			}
			set
			{
				this.exceptIfSenderADAttributeMatchesPatterns = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.WordsDisplayName)]
		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.WordsDescription)]
		public Word[] ExceptIfRecipientADAttributeContainsWords
		{
			get
			{
				return this.exceptIfRecipientADAttributeContainsWords;
			}
			set
			{
				this.exceptIfRecipientADAttributeContainsWords = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.TextPatternsDescription)]
		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.TextPatternsDisplayName)]
		public Pattern[] ExceptIfRecipientADAttributeMatchesPatterns
		{
			get
			{
				return this.exceptIfRecipientADAttributeMatchesPatterns;
			}
			set
			{
				this.exceptIfRecipientADAttributeMatchesPatterns = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.ToAddressesDisplayName)]
		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.ToAddressesDescription)]
		public RecipientIdParameter[] ExceptIfAnyOfToHeader
		{
			get
			{
				return this.exceptIfAnyOfToHeader;
			}
			set
			{
				this.exceptIfAnyOfToHeader = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.ToDLAddressDescription)]
		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.ToDLAddressDisplayName)]
		public RecipientIdParameter[] ExceptIfAnyOfToHeaderMemberOf
		{
			get
			{
				return this.exceptIfAnyOfToHeaderMemberOf;
			}
			set
			{
				this.exceptIfAnyOfToHeaderMemberOf = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.ToAddressesDescription)]
		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.ToAddressesDisplayName)]
		public RecipientIdParameter[] ExceptIfAnyOfCcHeader
		{
			get
			{
				return this.exceptIfAnyOfCcHeader;
			}
			set
			{
				this.exceptIfAnyOfCcHeader = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.ToDLAddressDisplayName)]
		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.ToDLAddressDescription)]
		public RecipientIdParameter[] ExceptIfAnyOfCcHeaderMemberOf
		{
			get
			{
				return this.exceptIfAnyOfCcHeaderMemberOf;
			}
			set
			{
				this.exceptIfAnyOfCcHeaderMemberOf = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.ToAddressesDisplayName)]
		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.ToAddressesDescription)]
		public RecipientIdParameter[] ExceptIfAnyOfToCcHeader
		{
			get
			{
				return this.exceptIfAnyOfToCcHeader;
			}
			set
			{
				this.exceptIfAnyOfToCcHeader = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.ToDLAddressDisplayName)]
		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.ToDLAddressDescription)]
		public RecipientIdParameter[] ExceptIfAnyOfToCcHeaderMemberOf
		{
			get
			{
				return this.exceptIfAnyOfToCcHeaderMemberOf;
			}
			set
			{
				this.exceptIfAnyOfToCcHeaderMemberOf = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.ClassificationDisplayName)]
		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.ClassificationDescription)]
		public ADObjectId ExceptIfHasClassification
		{
			get
			{
				return this.exceptIfHasClassification;
			}
			set
			{
				this.exceptIfHasClassification = value;
			}
		}

		public bool ExceptIfHasNoClassification
		{
			get
			{
				return this.exceptIfHasNoClassification;
			}
			set
			{
				this.exceptIfHasNoClassification = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.WordsDescription)]
		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.WordsDisplayName)]
		public Word[] ExceptIfSubjectContainsWords
		{
			get
			{
				return this.exceptIfSubjectContainsWords;
			}
			set
			{
				this.exceptIfSubjectContainsWords = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.WordsDescription)]
		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.WordsDisplayName)]
		public Word[] ExceptIfSubjectOrBodyContainsWords
		{
			get
			{
				return this.exceptIfSubjectOrBodyContainsWords;
			}
			set
			{
				this.exceptIfSubjectOrBodyContainsWords = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.MessageHeaderDescription)]
		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.MessageHeaderDisplayName)]
		public HeaderName? ExceptIfHeaderContainsMessageHeader
		{
			get
			{
				return this.exceptIfHeaderContainsMessageHeader;
			}
			set
			{
				this.exceptIfHeaderContainsMessageHeader = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.WordsDisplayName)]
		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.WordsDescription)]
		public Word[] ExceptIfHeaderContainsWords
		{
			get
			{
				return this.exceptIfHeaderContainsWords;
			}
			set
			{
				this.exceptIfHeaderContainsWords = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.WordsDisplayName)]
		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.WordsDescription)]
		public Word[] ExceptIfFromAddressContainsWords
		{
			get
			{
				return this.exceptIfFromAddressContainsWords;
			}
			set
			{
				this.exceptIfFromAddressContainsWords = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.WordsDisplayName)]
		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.WordsDescription)]
		public Word[] ExceptIfSenderDomainIs
		{
			get
			{
				return this.exceptIfSenderDomainIs;
			}
			set
			{
				this.exceptIfSenderDomainIs = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.WordsDescription)]
		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.WordsDisplayName)]
		public Word[] ExceptIfRecipientDomainIs
		{
			get
			{
				return this.exceptIfRecipientDomainIs;
			}
			set
			{
				this.exceptIfRecipientDomainIs = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.TextPatternsDescription)]
		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.TextPatternsDisplayName)]
		public Pattern[] ExceptIfSubjectMatchesPatterns
		{
			get
			{
				return this.exceptIfSubjectMatchesPatterns;
			}
			set
			{
				this.exceptIfSubjectMatchesPatterns = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.TextPatternsDescription)]
		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.TextPatternsDisplayName)]
		public Pattern[] ExceptIfSubjectOrBodyMatchesPatterns
		{
			get
			{
				return this.exceptIfSubjectOrBodyMatchesPatterns;
			}
			set
			{
				this.exceptIfSubjectOrBodyMatchesPatterns = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.MessageHeaderDisplayName)]
		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.MessageHeaderDescription)]
		public HeaderName? ExceptIfHeaderMatchesMessageHeader
		{
			get
			{
				return this.exceptIfHeaderMatchesMessageHeader;
			}
			set
			{
				this.exceptIfHeaderMatchesMessageHeader = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.TextPatternsDisplayName)]
		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.TextPatternsDescription)]
		public Pattern[] ExceptIfHeaderMatchesPatterns
		{
			get
			{
				return this.exceptIfHeaderMatchesPatterns;
			}
			set
			{
				this.exceptIfHeaderMatchesPatterns = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.TextPatternsDescription)]
		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.TextPatternsDisplayName)]
		public Pattern[] ExceptIfFromAddressMatchesPatterns
		{
			get
			{
				return this.exceptIfFromAddressMatchesPatterns;
			}
			set
			{
				this.exceptIfFromAddressMatchesPatterns = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.TextPatternsDisplayName)]
		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.TextPatternsDescription)]
		public Pattern[] ExceptIfAttachmentNameMatchesPatterns
		{
			get
			{
				return this.exceptIfAttachmentNameMatchesPatterns;
			}
			set
			{
				this.exceptIfAttachmentNameMatchesPatterns = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.TextPatternsDisplayName)]
		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.TextPatternsDescription)]
		public Word[] ExceptIfAttachmentExtensionMatchesWords
		{
			get
			{
				return this.exceptIfAttachmentExtensionMatchesWords;
			}
			set
			{
				this.exceptIfAttachmentExtensionMatchesWords = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.WordsDisplayName)]
		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.WordsDescription)]
		public Word[] ExceptIfAttachmentPropertyContainsWords
		{
			get
			{
				return this.exceptIfAttachmentPropertyContainsWords;
			}
			set
			{
				this.exceptIfAttachmentPropertyContainsWords = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.WordsDescription)]
		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.WordsDisplayName)]
		public Word[] ExceptIfContentCharacterSetContainsWords
		{
			get
			{
				return this.exceptIfContentCharacterSetContainsWords;
			}
			set
			{
				this.exceptIfContentCharacterSetContainsWords = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.SclValueDisplayName)]
		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.SclValueDescription)]
		public SclValue? ExceptIfSCLOver
		{
			get
			{
				return this.exceptIfSCLOver;
			}
			set
			{
				this.exceptIfSCLOver = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.AttachmentSizeDescription)]
		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.AttachmentSizeDisplayName)]
		public ByteQuantifiedSize? ExceptIfAttachmentSizeOver
		{
			get
			{
				return this.exceptIfAttachmentSizeOver;
			}
			set
			{
				this.exceptIfAttachmentSizeOver = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.MessageSizeDescription)]
		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.MessageSizeDisplayName)]
		public ByteQuantifiedSize? ExceptIfMessageSizeOver
		{
			get
			{
				return this.exceptIfMessageSizeOver;
			}
			set
			{
				this.exceptIfMessageSizeOver = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.ImportanceDisplayName)]
		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.ImportanceDescription)]
		public Importance? ExceptIfWithImportance
		{
			get
			{
				return this.exceptIfWithImportance;
			}
			set
			{
				this.exceptIfWithImportance = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.MessageTypeDescription)]
		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.MessageTypeDisplayName)]
		public MessageType? ExceptIfMessageTypeMatches
		{
			get
			{
				return this.exceptIfMessageTypeMatches;
			}
			set
			{
				this.exceptIfMessageTypeMatches = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.WordsDisplayName)]
		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.WordsDescription)]
		public Word[] ExceptIfRecipientAddressContainsWords
		{
			get
			{
				return this.exceptIfRecipientAddressContainsWords;
			}
			set
			{
				this.exceptIfRecipientAddressContainsWords = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.TextPatternsDisplayName)]
		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.TextPatternsDescription)]
		public Pattern[] ExceptIfRecipientAddressMatchesPatterns
		{
			get
			{
				return this.exceptIfRecipientAddressMatchesPatterns;
			}
			set
			{
				this.exceptIfRecipientAddressMatchesPatterns = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.ListsDisplayName)]
		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.ListsDescription)]
		public Word[] ExceptIfSenderInRecipientList
		{
			get
			{
				return this.exceptIfSenderInRecipientList;
			}
			set
			{
				this.exceptIfSenderInRecipientList = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.ListsDisplayName)]
		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.ListsDescription)]
		public Word[] ExceptIfRecipientInSenderList
		{
			get
			{
				return this.exceptIfRecipientInSenderList;
			}
			set
			{
				this.exceptIfRecipientInSenderList = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.WordsDisplayName)]
		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.WordsDescription)]
		public Word[] ExceptIfAttachmentContainsWords
		{
			get
			{
				return this.exceptIfAttachmentContainsWords;
			}
			set
			{
				this.exceptIfAttachmentContainsWords = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.TextPatternsDescription)]
		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.TextPatternsDisplayName)]
		public Pattern[] ExceptIfAttachmentMatchesPatterns
		{
			get
			{
				return this.exceptIfAttachmentMatchesPatterns;
			}
			set
			{
				this.exceptIfAttachmentMatchesPatterns = value;
			}
		}

		public bool ExceptIfAttachmentIsUnsupported
		{
			get
			{
				return this.exceptIfAttachmentIsUnsupported;
			}
			set
			{
				this.exceptIfAttachmentIsUnsupported = value;
			}
		}

		public bool ExceptIfAttachmentProcessingLimitExceeded
		{
			get
			{
				return this.exceptIfAttachmentProcessingLimitExceeded;
			}
			set
			{
				this.exceptIfAttachmentProcessingLimitExceeded = value;
			}
		}

		public bool ExceptIfAttachmentHasExecutableContent
		{
			get
			{
				return this.exceptIfAttachmentHasExecutableContent;
			}
			set
			{
				this.exceptIfAttachmentHasExecutableContent = value;
			}
		}

		public bool ExceptIfAttachmentIsPasswordProtected
		{
			get
			{
				return this.exceptIfAttachmentIsPasswordProtected;
			}
			set
			{
				this.exceptIfAttachmentIsPasswordProtected = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.WordsDisplayName)]
		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.WordsDescription)]
		public Word[] ExceptIfAnyOfRecipientAddressContainsWords
		{
			get
			{
				return this.exceptIfAnyOfRecipientAddressContainsWords;
			}
			set
			{
				this.exceptIfAnyOfRecipientAddressContainsWords = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.TextPatternsDescription)]
		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.TextPatternsDisplayName)]
		public Pattern[] ExceptIfAnyOfRecipientAddressMatchesPatterns
		{
			get
			{
				return this.exceptIfAnyOfRecipientAddressMatchesPatterns;
			}
			set
			{
				this.exceptIfAnyOfRecipientAddressMatchesPatterns = value;
			}
		}

		public bool ExceptIfHasSenderOverride
		{
			get
			{
				return this.exceptIfHasSenderOverride;
			}
			set
			{
				this.exceptIfHasSenderOverride = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.MessageDataClassificationDisplayName)]
		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.MessageDataClassificationDescription)]
		public string[] ExceptIfMessageContainsDataClassifications
		{
			get
			{
				return this.exceptIfMessageContainsDataClassifications;
			}
			set
			{
				this.exceptIfMessageContainsDataClassifications = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.SenderIpRangesDisplayName)]
		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.SenderIpRangesDescription)]
		public MultiValuedProperty<IPRange> ExceptIfSenderIpRanges
		{
			get
			{
				return this.exceptIfSenderIpanges;
			}
			set
			{
				this.exceptIfSenderIpanges = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.PrefixDisplayName)]
		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.PrefixDescription)]
		public string PrependSubject
		{
			get
			{
				return this.prependSubject;
			}
			set
			{
				this.prependSubject = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.SetAuditSeverityDescription)]
		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.SetAuditSeverityDisplayName)]
		public string SetAuditSeverity
		{
			get
			{
				return this.setAuditSeverity;
			}
			set
			{
				this.setAuditSeverity = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.ClassificationDisplayName)]
		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.ClassificationDescription)]
		public ADObjectId ApplyClassification
		{
			get
			{
				return this.applyClassification;
			}
			set
			{
				this.applyClassification = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.DisclaimerLocationDescription)]
		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.DisclaimerLocationDisplayName)]
		public DisclaimerLocation? ApplyHtmlDisclaimerLocation
		{
			get
			{
				return this.applyHtmlDisclaimerLocation;
			}
			set
			{
				this.applyHtmlDisclaimerLocation = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.DisclaimerTextDisplayName)]
		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.DisclaimerTextDescription)]
		public DisclaimerText? ApplyHtmlDisclaimerText
		{
			get
			{
				return this.applyHtmlDisclaimerText;
			}
			set
			{
				this.applyHtmlDisclaimerText = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.FallbackActionDisplayName)]
		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.FallbackActionDescription)]
		public DisclaimerFallbackAction? ApplyHtmlDisclaimerFallbackAction
		{
			get
			{
				return this.applyHtmlDisclaimerFallbackAction;
			}
			set
			{
				this.applyHtmlDisclaimerFallbackAction = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.RmsTemplateDisplayName)]
		[ActionParameterName("ApplyRightsProtectionTemplate")]
		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.RmsTemplateDescription)]
		public RmsTemplateIdentity ApplyRightsProtectionTemplate
		{
			get
			{
				return this.applyRightsProtectionTemplate;
			}
			set
			{
				this.applyRightsProtectionTemplate = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.SclValueDisplayName)]
		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.SclValueDescription)]
		public SclValue? SetSCL
		{
			get
			{
				return this.setSCL;
			}
			set
			{
				this.setSCL = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.MessageHeaderDisplayName)]
		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.MessageHeaderDescription)]
		public HeaderName? SetHeaderName
		{
			get
			{
				return this.setHeaderName;
			}
			set
			{
				this.setHeaderName = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.HeaderValueDisplayName)]
		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.HeaderValueDescription)]
		public HeaderValue? SetHeaderValue
		{
			get
			{
				return this.setHeaderValue;
			}
			set
			{
				this.setHeaderValue = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.MessageHeaderDisplayName)]
		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.MessageHeaderDescription)]
		public HeaderName? RemoveHeader
		{
			get
			{
				return this.removeHeader;
			}
			set
			{
				this.removeHeader = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.ToAddressesDisplayName)]
		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.ToAddressesDescription)]
		public RecipientIdParameter[] AddToRecipients
		{
			get
			{
				return this.addToRecipients;
			}
			set
			{
				this.addToRecipients = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.ToAddressesDisplayName)]
		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.ToAddressesDescription)]
		public RecipientIdParameter[] CopyTo
		{
			get
			{
				return this.copyTo;
			}
			set
			{
				this.copyTo = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.ToAddressesDisplayName)]
		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.ToAddressesDescription)]
		public RecipientIdParameter[] BlindCopyTo
		{
			get
			{
				return this.blindCopyTo;
			}
			set
			{
				this.blindCopyTo = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.RecipientTypeDescription)]
		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.RecipientTypeDisplayName)]
		public AddedRecipientType? AddManagerAsRecipientType
		{
			get
			{
				return this.addManagerAsRecipientType;
			}
			set
			{
				this.addManagerAsRecipientType = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.ToAddressesDescription)]
		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.ToAddressesDisplayName)]
		public RecipientIdParameter[] ModerateMessageByUser
		{
			get
			{
				return this.moderateMessageByUser;
			}
			set
			{
				this.moderateMessageByUser = value;
			}
		}

		public bool ModerateMessageByManager
		{
			get
			{
				return this.moderateMessageByManager;
			}
			set
			{
				this.moderateMessageByManager = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.ToAddressesDisplayName)]
		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.ToAddressesDescription)]
		public RecipientIdParameter[] RedirectMessageTo
		{
			get
			{
				return this.redirectMessageTo;
			}
			set
			{
				this.redirectMessageTo = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.EnhancedStatusCodeDescription)]
		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.EnhancedStatusCodeDisplayName)]
		public RejectEnhancedStatus? RejectMessageEnhancedStatusCode
		{
			get
			{
				return this.rejectMessageEnhancedStatusCode;
			}
			set
			{
				this.rejectMessageEnhancedStatusCode = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.RejectReasonDisplayName)]
		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.RejectReasonDescription)]
		public DsnText? RejectMessageReasonText
		{
			get
			{
				return this.rejectMessageReasonText;
			}
			set
			{
				this.rejectMessageReasonText = value;
			}
		}

		public bool DeleteMessage
		{
			get
			{
				return this.deleteMessage;
			}
			set
			{
				this.deleteMessage = value;
			}
		}

		public bool Disconnect
		{
			get
			{
				return this.disconnect;
			}
			set
			{
				this.disconnect = value;
			}
		}

		public bool Quarantine
		{
			get
			{
				return this.quarantine;
			}
			set
			{
				this.quarantine = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.RejectReasonDisplayName)]
		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.RejectReasonDescription)]
		public RejectText? SmtpRejectMessageRejectText
		{
			get
			{
				return this.smtpRejectMessageRejectText;
			}
			set
			{
				this.smtpRejectMessageRejectText = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.StatusCodeDescription)]
		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.StatusCodeDisplayName)]
		public RejectStatusCode? SmtpRejectMessageRejectStatusCode
		{
			get
			{
				return this.smtpRejectMessageRejectStatusCode;
			}
			set
			{
				this.smtpRejectMessageRejectStatusCode = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.EventMessageDisplayName)]
		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.EventMessageDescription)]
		public EventLogText? LogEventText
		{
			get
			{
				return this.logEventText;
			}
			set
			{
				this.logEventText = value;
			}
		}

		public bool StopRuleProcessing
		{
			get
			{
				return this.stopRuleProcessing;
			}
			set
			{
				this.stopRuleProcessing = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.SenderNotificationTypeDisplayName)]
		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.SenderNotificationTypeDescription)]
		public NotifySenderType? SenderNotificationType
		{
			get
			{
				return this.senderNotificationType;
			}
			set
			{
				this.senderNotificationType = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.ReportDestinationDescription)]
		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.ReportDestinationDisplayName)]
		public RecipientIdParameter GenerateIncidentReport
		{
			get
			{
				return this.generateIncidentReport;
			}
			set
			{
				this.generateIncidentReport = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.IncidentReportOriginalMailnDisplayName)]
		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.IncidentReportOriginalMailDescription)]
		public IncidentReportOriginalMail? IncidentReportOriginalMail
		{
			get
			{
				return this.incidentReportOriginalMail;
			}
			set
			{
				this.incidentReportOriginalMail = value;
			}
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.IncidentReportContentDisplayName)]
		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.IncidentReportContentDescription)]
		public IncidentReportContent[] IncidentReportContent
		{
			get
			{
				return this.incidentReportContent;
			}
			set
			{
				this.incidentReportContent = value;
			}
		}

		public string RouteMessageOutboundConnector
		{
			get
			{
				return this.connectorName;
			}
			set
			{
				this.connectorName = value;
			}
		}

		public bool RouteMessageOutboundRequireTls { get; set; }

		public bool ApplyOME { get; set; }

		public bool RemoveOME { get; set; }

		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.GenerateNotificationDescription)]
		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.GenerateNotificationDisplayName)]
		public DisclaimerText? GenerateNotification
		{
			get
			{
				return this.generateNotification;
			}
			set
			{
				this.generateNotification = value;
			}
		}

		public override ValidationError[] Validate()
		{
			if (!this.isValid)
			{
				return new ValidationError[]
				{
					new ObjectValidationError(this.errorText, base.Identity, null)
				};
			}
			List<ValidationError> list = new List<ValidationError>();
			ValidationError[] collection = Rule.ValidatePhraseArray(this.Conditions, true, false);
			list.AddRange(collection);
			collection = Rule.ValidatePhraseArray(this.Actions, true, false);
			list.AddRange(collection);
			collection = Rule.ValidatePhraseArray(this.Exceptions, true, false);
			list.AddRange(collection);
			if (list.Count != 0)
			{
				return list.ToArray();
			}
			return ValidationError.None;
		}

		internal static Rule CreateCorruptRule(int priority, TransportRule transportRule, LocalizedString errorText)
		{
			return new Rule(transportRule, transportRule.Name, priority, null, Guid.Empty, null, true, null, null, null, null, null, RuleState.Disabled, RuleMode.Enforce, RuleSubType.None, RuleErrorAction.Ignore, SenderAddressLocation.Header)
			{
				isValid = false,
				errorText = errorText
			};
		}

		internal static Rule CreateAdvancedRule(int priority, TransportRule transportRule, RuleState state)
		{
			return new Rule(transportRule, transportRule.Name, priority, null, Guid.Empty, null, true, null, null, null, null, null, state, RuleMode.Enforce, RuleSubType.None, RuleErrorAction.Ignore, SenderAddressLocation.Header);
		}

		internal static Rule CreateFromInternalRule(TypeMapping[] supportedPredicates, TypeMapping[] supportedActions, TransportRule rule, int priority, TransportRule transportRule)
		{
			IConfigDataProvider configDataProvider = null;
			if (transportRule != null)
			{
				configDataProvider = transportRule.Session;
			}
			TransportRulePredicate[] array;
			TransportRulePredicate[] array2;
			TransportRuleAction[] array3;
			bool flag = Rule.TryConvert(supportedPredicates, supportedActions, rule, out array, out array2, out array3, configDataProvider);
			string text = null;
			Guid guid;
			if (rule.TryGetDlpPolicyId(out guid) && configDataProvider != null)
			{
				ADComplianceProgram adcomplianceProgram = DlpUtils.GetInstalledTenantDlpPolicies(configDataProvider, guid.ToString()).FirstOrDefault<ADComplianceProgram>();
				if (adcomplianceProgram != null)
				{
					text = adcomplianceProgram.Name;
				}
			}
			return new Rule(transportRule, rule.Name, priority, text, guid, rule.Comments, !flag, rule.ActivationDate, rule.ExpiryDate, array, array2, array3, rule.Enabled, rule.Mode, rule.SubType, rule.ErrorAction, rule.SenderAddressLocation);
		}

		internal TransportRule ToInternalRule()
		{
			if (this.ManuallyModified)
			{
				throw new InvalidOperationException();
			}
			AndCondition andCondition = new AndCondition();
			List<RuleBifurcationInfo> list = new List<RuleBifurcationInfo>();
			if (this.actions == null || this.actions.Length == 0)
			{
				throw new ArgumentException(RulesTasksStrings.NoAction, "Actions");
			}
			andCondition.SubConditions.Add(Condition.True);
			int num = -1;
			if (this.conditions != null)
			{
				Rule.ValidatePhraseArray(this.conditions, true, true);
				foreach (TransportRulePredicate transportRulePredicate in this.conditions)
				{
					if (transportRulePredicate.Rank <= num)
					{
						throw new PredicateOrActionSequenceException(string.Format("Either multiple predicates of the same type have been added to this rule, or the order in which the predicates have been added is incorrect - detected when processing predicate type {0}", transportRulePredicate.GetType()), null);
					}
					num = transportRulePredicate.Rank;
					if (transportRulePredicate is BifurcationInfoPredicate)
					{
						BifurcationInfoPredicate bifurcationInfoPredicate = (BifurcationInfoPredicate)transportRulePredicate;
						RuleBifurcationInfo ruleBifurcationInfo;
						RuleBifurcationInfo item = bifurcationInfoPredicate.ToRuleBifurcationInfo(out ruleBifurcationInfo);
						list.Add(item);
						if (ruleBifurcationInfo != null)
						{
							list.Add(ruleBifurcationInfo);
						}
					}
					else
					{
						Condition item2 = transportRulePredicate.ToInternalCondition();
						andCondition.SubConditions.Add(item2);
					}
				}
			}
			if (this.exceptions != null && this.exceptions.Length > 0)
			{
				OrCondition orCondition = new OrCondition();
				andCondition.SubConditions.Add(new NotCondition(orCondition));
				Rule.ValidatePhraseArray(this.exceptions, true, true);
				num = -1;
				foreach (TransportRulePredicate transportRulePredicate2 in this.exceptions)
				{
					if (transportRulePredicate2.Rank <= num)
					{
						throw new PredicateOrActionSequenceException(string.Format("Either multiple exceptions of the same type have been added to this rule, or the order in which the exceptions have been added is incorrect - detected when processing exception type {0}", transportRulePredicate2.GetType()), null);
					}
					num = transportRulePredicate2.Rank;
					if (transportRulePredicate2 is BifurcationInfoPredicate)
					{
						BifurcationInfoPredicate bifurcationInfoPredicate2 = (BifurcationInfoPredicate)transportRulePredicate2;
						RuleBifurcationInfo ruleBifurcationInfo3;
						RuleBifurcationInfo ruleBifurcationInfo2 = bifurcationInfoPredicate2.ToRuleBifurcationInfo(out ruleBifurcationInfo3);
						ruleBifurcationInfo2.Exception = true;
						list.Add(ruleBifurcationInfo2);
						if (ruleBifurcationInfo3 != null)
						{
							ruleBifurcationInfo3.Exception = true;
							list.Add(ruleBifurcationInfo3);
						}
					}
					else
					{
						Condition item3 = transportRulePredicate2.ToInternalCondition();
						orCondition.SubConditions.Add(item3);
					}
				}
				if (orCondition.SubConditions.Count == 0)
				{
					orCondition.SubConditions.Add(Condition.False);
				}
			}
			TransportRule transportRule = new TransportRule(base.Name, andCondition);
			transportRule.Enabled = this.State;
			transportRule.Mode = this.Mode;
			transportRule.ErrorAction = this.RuleErrorAction;
			transportRule.SenderAddressLocation = this.SenderAddressLocation;
			transportRule.SubType = this.RuleSubType;
			if (!Guid.Empty.Equals(this.dlpPolicyId))
			{
				transportRule.SetDlpPolicy(this.dlpPolicyId, this.dlpPolicy);
			}
			if (this.comments != null)
			{
				transportRule.Comments = this.comments;
			}
			if (list.Count > 0)
			{
				transportRule.Fork = list;
			}
			if (this.activationDate != null)
			{
				transportRule.ActivationDate = this.activationDate;
			}
			if (this.expiryDate != null)
			{
				transportRule.ExpiryDate = this.expiryDate;
			}
			Rule.ValidatePhraseArray(this.actions, false, true);
			num = -1;
			foreach (TransportRuleAction transportRuleAction in this.actions)
			{
				if (transportRuleAction.Rank <= num)
				{
					throw new PredicateOrActionSequenceException(string.Format("Either multiple actions of the same type have been added to this rule, or the order in which the actions have been added is incorrect - detected when processing action type {0}", transportRuleAction.GetType()), null);
				}
				num = transportRuleAction.Rank;
				foreach (Action item4 in transportRuleAction.ToInternalActions())
				{
					transportRule.Actions.Add(item4);
				}
			}
			return transportRule;
		}

		internal static bool TryConvert(TypeMapping[] supportedPredicates, TypeMapping[] supportedActions, TransportRule rule, out TransportRulePredicate[] conditions, out TransportRulePredicate[] exceptions, out TransportRuleAction[] actions, IConfigDataProvider session)
		{
			conditions = null;
			exceptions = null;
			List<TransportRulePredicate> list = new List<TransportRulePredicate>();
			List<TransportRulePredicate> list2 = new List<TransportRulePredicate>();
			if (!Rule.TryConvertConditionAndException(supportedPredicates, rule, list, list2, session))
			{
				actions = null;
				return false;
			}
			if (list.Count > 0)
			{
				conditions = list.ToArray();
			}
			if (list2.Count > 0)
			{
				exceptions = list2.ToArray();
			}
			if (!Rule.TryConvertActions(supportedActions, rule, out actions, session))
			{
				conditions = null;
				exceptions = null;
				return false;
			}
			return true;
		}

		private static bool TryConvertConditionAndException(TypeMapping[] supportedPredicates, TransportRule rule, List<TransportRulePredicate> conditions, List<TransportRulePredicate> exceptions, IConfigDataProvider session)
		{
			if (rule.Condition.ConditionType != ConditionType.And)
			{
				return false;
			}
			AndCondition andCondition = (AndCondition)rule.Condition;
			if (andCondition.SubConditions.Count < 1 || andCondition.SubConditions[0].ConditionType != ConditionType.True)
			{
				return false;
			}
			foreach (Condition condition in andCondition.SubConditions)
			{
				if (condition.ConditionType != ConditionType.True)
				{
					TransportRulePredicate item;
					if (TransportRulePredicate.TryCreatePredicateFromCondition(supportedPredicates, condition, out item, session))
					{
						conditions.Add(item);
					}
					else
					{
						if (condition != andCondition.SubConditions[andCondition.SubConditions.Count - 1] || condition.ConditionType != ConditionType.Not)
						{
							return false;
						}
						if (!Rule.TryConvertException(supportedPredicates, ((NotCondition)condition).SubCondition, exceptions, session))
						{
							return false;
						}
					}
				}
			}
			if (rule.Fork != null)
			{
				for (int i = 0; i < rule.Fork.Count; i++)
				{
					RuleBifurcationInfo ruleBifurcationInfo = rule.Fork[i];
					RuleBifurcationInfo bifInfo = (i + 1 < rule.Fork.Count) ? rule.Fork[i + 1] : null;
					TransportRulePredicate predicate;
					bool flag;
					if (!BifurcationInfoPredicate.TryCreatePredicateFromBifInfo(supportedPredicates, ruleBifurcationInfo, bifInfo, out predicate, out flag))
					{
						return false;
					}
					bool flag2;
					if (ruleBifurcationInfo.Exception)
					{
						flag2 = Rule.TryInsertPredicateSorted(predicate, exceptions);
					}
					else
					{
						flag2 = Rule.TryInsertPredicateSorted(predicate, conditions);
					}
					if (!flag2)
					{
						return false;
					}
					if (flag)
					{
						i++;
					}
				}
			}
			return true;
		}

		private static bool TryInsertPredicateSorted(TransportRulePredicate predicate, List<TransportRulePredicate> predicateList)
		{
			int rank = predicate.Rank;
			int num = 0;
			while (num < predicateList.Count && rank >= predicateList[num].Rank)
			{
				if (rank == predicateList[num].Rank)
				{
					return false;
				}
				num++;
			}
			predicateList.Insert(num, predicate);
			return true;
		}

		private static bool TryConvertException(TypeMapping[] supportedPredicates, Condition conditionTree, List<TransportRulePredicate> exceptions, IConfigDataProvider session)
		{
			if (conditionTree.ConditionType != ConditionType.Or)
			{
				return false;
			}
			OrCondition orCondition = (OrCondition)conditionTree;
			foreach (Condition condition in orCondition.SubConditions)
			{
				if (condition.ConditionType != ConditionType.False)
				{
					TransportRulePredicate item;
					if (!TransportRulePredicate.TryCreatePredicateFromCondition(supportedPredicates, condition, out item, session))
					{
						return false;
					}
					exceptions.Add(item);
				}
			}
			return true;
		}

		private static bool TryConvertActions(TypeMapping[] supportedActions, TransportRule rule, out TransportRuleAction[] actions, IConfigDataProvider session)
		{
			List<TransportRuleAction> list = new List<TransportRuleAction>();
			actions = null;
			if (rule.Actions.Count == 0)
			{
				return false;
			}
			int num = -1;
			int i = 0;
			while (i < rule.Actions.Count)
			{
				TransportRuleAction transportRuleAction;
				if (!TransportRuleAction.TryCreateFromInternalAction(supportedActions, rule.Actions, ref i, out transportRuleAction, session))
				{
					return false;
				}
				if (transportRuleAction.Rank <= num)
				{
					return false;
				}
				num = transportRuleAction.Rank;
				list.Add(transportRuleAction);
			}
			actions = list.ToArray();
			return true;
		}

		private static ValidationError[] ValidatePhraseArray(RulePhrase[] phrases, bool isPredicate, bool throwException)
		{
			List<ValidationError> list = new List<ValidationError>();
			if (phrases != null)
			{
				foreach (RulePhrase rulePhrase in phrases)
				{
					if (rulePhrase == null)
					{
						if (isPredicate)
						{
							if (throwException)
							{
								throw new ArgumentException(RulesTasksStrings.InvalidPredicate, "Predicates");
							}
							list.Add(new RulePhrase.RulePhraseValidationError(RulesTasksStrings.InvalidPredicate, "Predicates"));
						}
						else
						{
							if (throwException)
							{
								throw new ArgumentException(RulesTasksStrings.InvalidAction, "Actions");
							}
							list.Add(new RulePhrase.RulePhraseValidationError(RulesTasksStrings.InvalidAction, "Actions"));
						}
					}
					else
					{
						ValidationError[] array = rulePhrase.Validate();
						if (array != null && array.Length != 0)
						{
							list.AddRange(array);
						}
					}
				}
			}
			if (throwException)
			{
				List<RulePhrase.RulePhraseValidationError> list2 = new List<RulePhrase.RulePhraseValidationError>();
				foreach (ValidationError validationError in list)
				{
					RulePhrase.RulePhraseValidationError rulePhraseValidationError = validationError as RulePhrase.RulePhraseValidationError;
					if (rulePhraseValidationError != null)
					{
						list2.Add(rulePhraseValidationError);
					}
				}
				if (list2.Any<RulePhrase.RulePhraseValidationError>())
				{
					List<ValidationError> errors = (from ruleError in list2
					select ruleError).ToList<ValidationError>();
					LocalizedString value = ValidationError.CombineErrorDescriptions(errors);
					string name = list2[0].Name;
					throw new ArgumentException(value, name);
				}
			}
			return list.ToArray();
		}

		private void SetParametersFromPredicate(TransportRulePredicate predicate, bool isException, OrganizationId orgId)
		{
			if (predicate is FromPredicate)
			{
				if (isException)
				{
					this.exceptIfFrom = Utils.BuildRecipientIdArray(((FromPredicate)predicate).Addresses);
					return;
				}
				this.from = Utils.BuildRecipientIdArray(((FromPredicate)predicate).Addresses);
				return;
			}
			else if (predicate is FromMemberOfPredicate)
			{
				if (isException)
				{
					this.exceptIfFromMemberOf = Utils.BuildRecipientIdArray(((FromMemberOfPredicate)predicate).Addresses);
					return;
				}
				this.fromMemberOf = Utils.BuildRecipientIdArray(((FromMemberOfPredicate)predicate).Addresses);
				return;
			}
			else if (predicate is FromScopePredicate)
			{
				if (isException)
				{
					this.exceptIfFromScope = new FromUserScope?(((FromScopePredicate)predicate).Scope);
					return;
				}
				this.fromScope = new FromUserScope?(((FromScopePredicate)predicate).Scope);
				return;
			}
			else if (predicate is SentToPredicate)
			{
				if (isException)
				{
					this.exceptIfSentTo = Utils.BuildRecipientIdArray(((SentToPredicate)predicate).Addresses);
					return;
				}
				this.sentTo = Utils.BuildRecipientIdArray(((SentToPredicate)predicate).Addresses);
				return;
			}
			else if (predicate is SentToMemberOfPredicate)
			{
				if (isException)
				{
					this.exceptIfSentToMemberOf = Utils.BuildRecipientIdArray(((SentToMemberOfPredicate)predicate).Addresses);
					return;
				}
				this.sentToMemberOf = Utils.BuildRecipientIdArray(((SentToMemberOfPredicate)predicate).Addresses);
				return;
			}
			else if (predicate is SentToScopePredicate)
			{
				if (isException)
				{
					this.exceptIfSentToScope = new ToUserScope?(((SentToScopePredicate)predicate).Scope);
					return;
				}
				this.sentToScope = new ToUserScope?(((SentToScopePredicate)predicate).Scope);
				return;
			}
			else if (predicate is BetweenMemberOfPredicate)
			{
				if (isException)
				{
					this.exceptIfBetweenMemberOf1 = Utils.BuildRecipientIdArray(((BetweenMemberOfPredicate)predicate).Addresses);
					this.exceptIfBetweenMemberOf2 = Utils.BuildRecipientIdArray(((BetweenMemberOfPredicate)predicate).Addresses2);
					return;
				}
				this.betweenMemberOf1 = Utils.BuildRecipientIdArray(((BetweenMemberOfPredicate)predicate).Addresses);
				this.betweenMemberOf2 = Utils.BuildRecipientIdArray(((BetweenMemberOfPredicate)predicate).Addresses2);
				return;
			}
			else if (predicate is ManagerIsPredicate)
			{
				if (isException)
				{
					this.exceptIfManagerAddresses = Utils.BuildRecipientIdArray(((ManagerIsPredicate)predicate).Addresses);
					this.exceptIfManagerForEvaluatedUser = new EvaluatedUser?(((ManagerIsPredicate)predicate).EvaluatedUser);
					return;
				}
				this.managerAddresses = Utils.BuildRecipientIdArray(((ManagerIsPredicate)predicate).Addresses);
				this.managerForEvaluatedUser = new EvaluatedUser?(((ManagerIsPredicate)predicate).EvaluatedUser);
				return;
			}
			else if (predicate is ManagementRelationshipPredicate)
			{
				if (isException)
				{
					this.exceptIfSenderManagementRelationship = new ManagementRelationship?(((ManagementRelationshipPredicate)predicate).ManagementRelationship);
					return;
				}
				this.senderManagementRelationship = new ManagementRelationship?(((ManagementRelationshipPredicate)predicate).ManagementRelationship);
				return;
			}
			else if (predicate is ADAttributeComparisonPredicate)
			{
				if (isException)
				{
					this.exceptIfADComparisonAttribute = new ADAttribute?(((ADAttributeComparisonPredicate)predicate).ADAttribute);
					this.exceptIfADComparisonOperator = new Evaluation?(((ADAttributeComparisonPredicate)predicate).Evaluation);
					return;
				}
				this.adComparisonAttribute = new ADAttribute?(((ADAttributeComparisonPredicate)predicate).ADAttribute);
				this.adComparisonOperator = new Evaluation?(((ADAttributeComparisonPredicate)predicate).Evaluation);
				return;
			}
			else if (predicate is AnyOfToHeaderPredicate)
			{
				if (isException)
				{
					this.exceptIfAnyOfToHeader = Utils.BuildRecipientIdArray(((AnyOfToHeaderPredicate)predicate).Addresses);
					return;
				}
				this.anyOfToHeader = Utils.BuildRecipientIdArray(((AnyOfToHeaderPredicate)predicate).Addresses);
				return;
			}
			else if (predicate is AnyOfToHeaderMemberOfPredicate)
			{
				if (isException)
				{
					this.exceptIfAnyOfToHeaderMemberOf = Utils.BuildRecipientIdArray(((AnyOfToHeaderMemberOfPredicate)predicate).Addresses);
					return;
				}
				this.anyOfToHeaderMemberOf = Utils.BuildRecipientIdArray(((AnyOfToHeaderMemberOfPredicate)predicate).Addresses);
				return;
			}
			else if (predicate is AnyOfCcHeaderPredicate)
			{
				if (isException)
				{
					this.exceptIfAnyOfCcHeader = Utils.BuildRecipientIdArray(((AnyOfCcHeaderPredicate)predicate).Addresses);
					return;
				}
				this.anyOfCcHeader = Utils.BuildRecipientIdArray(((AnyOfCcHeaderPredicate)predicate).Addresses);
				return;
			}
			else if (predicate is AnyOfCcHeaderMemberOfPredicate)
			{
				if (isException)
				{
					this.exceptIfAnyOfCcHeaderMemberOf = Utils.BuildRecipientIdArray(((AnyOfCcHeaderMemberOfPredicate)predicate).Addresses);
					return;
				}
				this.anyOfCcHeaderMemberOf = Utils.BuildRecipientIdArray(((AnyOfCcHeaderMemberOfPredicate)predicate).Addresses);
				return;
			}
			else if (predicate is AnyOfToCcHeaderPredicate)
			{
				if (isException)
				{
					this.exceptIfAnyOfToCcHeader = Utils.BuildRecipientIdArray(((AnyOfToCcHeaderPredicate)predicate).Addresses);
					return;
				}
				this.anyOfToCcHeader = Utils.BuildRecipientIdArray(((AnyOfToCcHeaderPredicate)predicate).Addresses);
				return;
			}
			else if (predicate is AnyOfToCcHeaderMemberOfPredicate)
			{
				if (isException)
				{
					this.exceptIfAnyOfToCcHeaderMemberOf = Utils.BuildRecipientIdArray(((AnyOfToCcHeaderMemberOfPredicate)predicate).Addresses);
					return;
				}
				this.anyOfToCcHeaderMemberOf = Utils.BuildRecipientIdArray(((AnyOfToCcHeaderMemberOfPredicate)predicate).Addresses);
				return;
			}
			else if (predicate is HasClassificationPredicate)
			{
				if (isException)
				{
					this.exceptIfHasClassification = ((HasClassificationPredicate)predicate).Classification;
					return;
				}
				this.hasClassification = ((HasClassificationPredicate)predicate).Classification;
				return;
			}
			else if (predicate is HasNoClassificationPredicate)
			{
				if (isException)
				{
					this.exceptIfHasNoClassification = true;
					return;
				}
				this.hasNoClassification = true;
				return;
			}
			else if (predicate is SubjectContainsPredicate)
			{
				if (isException)
				{
					this.exceptIfSubjectContainsWords = ((SubjectContainsPredicate)predicate).Words;
					return;
				}
				this.subjectContainsWords = ((SubjectContainsPredicate)predicate).Words;
				return;
			}
			else if (predicate is SubjectOrBodyContainsPredicate)
			{
				if (isException)
				{
					this.exceptIfSubjectOrBodyContainsWords = ((SubjectOrBodyContainsPredicate)predicate).Words;
					return;
				}
				this.subjectOrBodyContainsWords = ((SubjectOrBodyContainsPredicate)predicate).Words;
				return;
			}
			else if (predicate is HeaderContainsPredicate)
			{
				if (isException)
				{
					this.exceptIfHeaderContainsMessageHeader = new HeaderName?(((HeaderContainsPredicate)predicate).MessageHeader);
					this.exceptIfHeaderContainsWords = ((HeaderContainsPredicate)predicate).Words;
					return;
				}
				this.headerContainsMessageHeader = new HeaderName?(((HeaderContainsPredicate)predicate).MessageHeader);
				this.headerContainsWords = ((HeaderContainsPredicate)predicate).Words;
				return;
			}
			else if (predicate is FromAddressContainsPredicate)
			{
				if (isException)
				{
					this.exceptIfFromAddressContainsWords = ((FromAddressContainsPredicate)predicate).Words;
					return;
				}
				this.fromAddressContainsWords = ((FromAddressContainsPredicate)predicate).Words;
				return;
			}
			else if (predicate is SenderDomainIsPredicate)
			{
				if (isException)
				{
					this.exceptIfSenderDomainIs = (predicate as SenderDomainIsPredicate).Words;
					return;
				}
				this.SenderDomainIs = (predicate as SenderDomainIsPredicate).Words;
				return;
			}
			else if (predicate is RecipientDomainIsPredicate)
			{
				if (isException)
				{
					this.exceptIfRecipientDomainIs = (predicate as RecipientDomainIsPredicate).Words;
					return;
				}
				this.recipientDomainIs = (predicate as RecipientDomainIsPredicate).Words;
				return;
			}
			else if (predicate is RecipientAddressContainsWordsPredicate)
			{
				if (isException)
				{
					this.exceptIfRecipientAddressContainsWords = ((RecipientAddressContainsWordsPredicate)predicate).Words;
					return;
				}
				this.recipientAddressContainsWords = ((RecipientAddressContainsWordsPredicate)predicate).Words;
				return;
			}
			else if (predicate is RecipientAddressMatchesPatternsPredicate)
			{
				if (isException)
				{
					this.exceptIfRecipientAddressMatchesPatterns = ((RecipientAddressMatchesPatternsPredicate)predicate).Patterns;
					return;
				}
				this.recipientAddressMatchesPatterns = ((RecipientAddressMatchesPatternsPredicate)predicate).Patterns;
				return;
			}
			else if (predicate is SubjectMatchesPredicate)
			{
				if (isException)
				{
					this.exceptIfSubjectMatchesPatterns = ((SubjectMatchesPredicate)predicate).Patterns;
					return;
				}
				this.subjectMatchesPatterns = ((SubjectMatchesPredicate)predicate).Patterns;
				return;
			}
			else if (predicate is SubjectOrBodyMatchesPredicate)
			{
				if (isException)
				{
					this.exceptIfSubjectOrBodyMatchesPatterns = ((SubjectOrBodyMatchesPredicate)predicate).Patterns;
					return;
				}
				this.subjectOrBodyMatchesPatterns = ((SubjectOrBodyMatchesPredicate)predicate).Patterns;
				return;
			}
			else if (predicate is HeaderMatchesPredicate)
			{
				if (isException)
				{
					this.exceptIfHeaderMatchesMessageHeader = new HeaderName?(((HeaderMatchesPredicate)predicate).MessageHeader);
					this.exceptIfHeaderMatchesPatterns = ((HeaderMatchesPredicate)predicate).Patterns;
					return;
				}
				this.headerMatchesMessageHeader = new HeaderName?(((HeaderMatchesPredicate)predicate).MessageHeader);
				this.headerMatchesPatterns = ((HeaderMatchesPredicate)predicate).Patterns;
				return;
			}
			else if (predicate is FromAddressMatchesPredicate)
			{
				if (isException)
				{
					this.exceptIfFromAddressMatchesPatterns = ((FromAddressMatchesPredicate)predicate).Patterns;
					return;
				}
				this.fromAddressMatchesPatterns = ((FromAddressMatchesPredicate)predicate).Patterns;
				return;
			}
			else if (predicate is AttachmentNameMatchesPredicate)
			{
				if (isException)
				{
					this.exceptIfAttachmentNameMatchesPatterns = ((AttachmentNameMatchesPredicate)predicate).Patterns;
					return;
				}
				this.attachmentNameMatchesPatterns = ((AttachmentNameMatchesPredicate)predicate).Patterns;
				return;
			}
			else if (predicate is AttachmentExtensionMatchesWordsPredicate)
			{
				if (isException)
				{
					this.exceptIfAttachmentExtensionMatchesWords = ((AttachmentExtensionMatchesWordsPredicate)predicate).Words;
					return;
				}
				this.attachmentExtensionMatchesWords = ((AttachmentExtensionMatchesWordsPredicate)predicate).Words;
				return;
			}
			else if (predicate is AttachmentPropertyContainsWordsPredicate)
			{
				if (isException)
				{
					this.exceptIfAttachmentPropertyContainsWords = ((AttachmentPropertyContainsWordsPredicate)predicate).Words;
					return;
				}
				this.attachmentPropertyContainsWords = ((AttachmentPropertyContainsWordsPredicate)predicate).Words;
				return;
			}
			else if (predicate is ContentCharacterSetContainsWordsPredicate)
			{
				if (isException)
				{
					this.exceptIfContentCharacterSetContainsWords = ((ContentCharacterSetContainsWordsPredicate)predicate).Words;
					return;
				}
				this.contentCharacterSetContainsWords = ((ContentCharacterSetContainsWordsPredicate)predicate).Words;
				return;
			}
			else if (predicate is SclOverPredicate)
			{
				if (isException)
				{
					this.exceptIfSCLOver = new SclValue?(((SclOverPredicate)predicate).SclValue);
					return;
				}
				this.sclOver = new SclValue?(((SclOverPredicate)predicate).SclValue);
				return;
			}
			else if (predicate is AttachmentSizeOverPredicate)
			{
				if (isException)
				{
					this.exceptIfAttachmentSizeOver = new ByteQuantifiedSize?(((AttachmentSizeOverPredicate)predicate).Size);
					return;
				}
				this.attachmentSizeOver = new ByteQuantifiedSize?(((AttachmentSizeOverPredicate)predicate).Size);
				return;
			}
			else if (predicate is MessageSizeOverPredicate)
			{
				if (isException)
				{
					this.exceptIfMessageSizeOver = new ByteQuantifiedSize?(((MessageSizeOverPredicate)predicate).Size);
					return;
				}
				this.messageSizeOver = new ByteQuantifiedSize?(((MessageSizeOverPredicate)predicate).Size);
				return;
			}
			else if (predicate is WithImportancePredicate)
			{
				if (isException)
				{
					this.exceptIfWithImportance = new Importance?(((WithImportancePredicate)predicate).Importance);
					return;
				}
				this.withImportance = new Importance?(((WithImportancePredicate)predicate).Importance);
				return;
			}
			else if (predicate is MessageTypeMatchesPredicate)
			{
				if (isException)
				{
					this.exceptIfMessageTypeMatches = new MessageType?(((MessageTypeMatchesPredicate)predicate).MessageType);
					return;
				}
				this.messageTypeMatches = new MessageType?(((MessageTypeMatchesPredicate)predicate).MessageType);
				return;
			}
			else if (predicate is SenderAttributeContainsPredicate)
			{
				if (isException)
				{
					this.exceptIfSenderADAttributeContainsWords = ((SenderAttributeContainsPredicate)predicate).Words;
					return;
				}
				this.senderADAttributeContainsWords = ((SenderAttributeContainsPredicate)predicate).Words;
				return;
			}
			else if (predicate is RecipientAttributeContainsPredicate)
			{
				if (isException)
				{
					this.exceptIfRecipientADAttributeContainsWords = ((RecipientAttributeContainsPredicate)predicate).Words;
					return;
				}
				this.recipientADAttributeContainsWords = ((RecipientAttributeContainsPredicate)predicate).Words;
				return;
			}
			else if (predicate is SenderAttributeMatchesPredicate)
			{
				if (isException)
				{
					this.exceptIfSenderADAttributeMatchesPatterns = ((SenderAttributeMatchesPredicate)predicate).Patterns;
					return;
				}
				this.senderADAttributeMatchesPatterns = ((SenderAttributeMatchesPredicate)predicate).Patterns;
				return;
			}
			else if (predicate is RecipientAttributeMatchesPredicate)
			{
				if (isException)
				{
					this.exceptIfRecipientADAttributeMatchesPatterns = ((RecipientAttributeMatchesPredicate)predicate).Patterns;
					return;
				}
				this.recipientADAttributeMatchesPatterns = ((RecipientAttributeMatchesPredicate)predicate).Patterns;
				return;
			}
			else if (predicate is RecipientInSenderListPredicate)
			{
				if (isException)
				{
					this.exceptIfRecipientInSenderList = ((RecipientInSenderListPredicate)predicate).Lists;
					return;
				}
				this.recipientInSenderList = ((RecipientInSenderListPredicate)predicate).Lists;
				return;
			}
			else if (predicate is SenderInRecipientListPredicate)
			{
				if (isException)
				{
					this.exceptIfSenderInRecipientList = ((SenderInRecipientListPredicate)predicate).Lists;
					return;
				}
				this.senderInRecipientList = ((SenderInRecipientListPredicate)predicate).Lists;
				return;
			}
			else if (predicate is AttachmentContainsWordsPredicate)
			{
				if (isException)
				{
					this.exceptIfAttachmentContainsWords = ((AttachmentContainsWordsPredicate)predicate).Words;
					return;
				}
				this.attachmentContainsWords = ((AttachmentContainsWordsPredicate)predicate).Words;
				return;
			}
			else if (predicate is AttachmentMatchesPatternsPredicate)
			{
				if (isException)
				{
					this.exceptIfAttachmentMatchesPatterns = ((AttachmentMatchesPatternsPredicate)predicate).Patterns;
					return;
				}
				this.attachmentMatchesPatterns = ((AttachmentMatchesPatternsPredicate)predicate).Patterns;
				return;
			}
			else if (predicate is AttachmentIsUnsupportedPredicate)
			{
				if (isException)
				{
					this.exceptIfAttachmentIsUnsupported = true;
					return;
				}
				this.attachmentIsUnsupported = true;
				return;
			}
			else if (predicate is AttachmentProcessingLimitExceededPredicate)
			{
				if (isException)
				{
					this.exceptIfAttachmentProcessingLimitExceeded = true;
					return;
				}
				this.attachmentProcessingLimitExceeded = true;
				return;
			}
			else if (predicate is AttachmentHasExecutableContentPredicate)
			{
				if (isException)
				{
					this.exceptIfAttachmentHasExecutableContent = true;
					return;
				}
				this.attachmentHasExecutableContent = true;
				return;
			}
			else if (predicate is AttachmentIsPasswordProtectedPredicate)
			{
				if (isException)
				{
					this.exceptIfAttachmentIsPasswordProtected = true;
					return;
				}
				this.attachmentIsPasswordProtected = true;
				return;
			}
			else if (predicate is AnyOfRecipientAddressContainsPredicate)
			{
				if (isException)
				{
					this.exceptIfAnyOfRecipientAddressContainsWords = ((AnyOfRecipientAddressContainsPredicate)predicate).Words;
					return;
				}
				this.anyOfRecipientAddressContainsWords = ((AnyOfRecipientAddressContainsPredicate)predicate).Words;
				return;
			}
			else if (predicate is AnyOfRecipientAddressMatchesPredicate)
			{
				if (isException)
				{
					this.exceptIfAnyOfRecipientAddressMatchesPatterns = ((AnyOfRecipientAddressMatchesPredicate)predicate).Patterns;
					return;
				}
				this.anyOfRecipientAddressMatchesPatterns = ((AnyOfRecipientAddressMatchesPredicate)predicate).Patterns;
				return;
			}
			else if (predicate is MessageContainsDataClassificationsPredicate)
			{
				((MessageContainsDataClassificationsPredicate)predicate).OrganizationId = orgId;
				if (isException)
				{
					this.exceptIfMessageContainsDataClassifications = MessageContainsDataClassificationsPredicate.HashtablesToStrings(((MessageContainsDataClassificationsPredicate)predicate).DataClassifications, orgId).ToArray<string>();
					return;
				}
				this.messageContainsDataClassifications = MessageContainsDataClassificationsPredicate.HashtablesToStrings(((MessageContainsDataClassificationsPredicate)predicate).DataClassifications, orgId).ToArray<string>();
				return;
			}
			else
			{
				if (!(predicate is SenderIpRangesPredicate))
				{
					if (predicate is HasSenderOverridePredicate)
					{
						if (isException)
						{
							this.exceptIfHasSenderOverride = true;
							return;
						}
						this.hasSenderOverride = true;
					}
					return;
				}
				if (isException)
				{
					this.ExceptIfSenderIpRanges = ((SenderIpRangesPredicate)predicate).IpRanges.ToArray();
					return;
				}
				this.SenderIpRanges = ((SenderIpRangesPredicate)predicate).IpRanges.ToArray();
				return;
			}
		}

		private void SetParametersFromAction(TransportRuleAction action)
		{
			if (action is PrependSubjectAction)
			{
				this.prependSubject = ((PrependSubjectAction)action).Prefix.ToString();
				return;
			}
			if (action is SetAuditSeverityAction)
			{
				this.setAuditSeverity = ((SetAuditSeverityAction)action).SeverityLevel.ToString();
				return;
			}
			if (action is ApplyClassificationAction)
			{
				this.applyClassification = ((ApplyClassificationAction)action).Classification;
				return;
			}
			if (action is ApplyHtmlDisclaimerAction)
			{
				this.applyHtmlDisclaimerLocation = new DisclaimerLocation?(((ApplyHtmlDisclaimerAction)action).Location);
				this.applyHtmlDisclaimerText = new DisclaimerText?(((ApplyHtmlDisclaimerAction)action).Text);
				this.applyHtmlDisclaimerFallbackAction = new DisclaimerFallbackAction?(((ApplyHtmlDisclaimerAction)action).FallbackAction);
				return;
			}
			if (action is RightsProtectMessageAction)
			{
				this.applyRightsProtectionTemplate = ((RightsProtectMessageAction)action).Template;
				return;
			}
			if (action is SetSclAction)
			{
				this.setSCL = new SclValue?(((SetSclAction)action).SclValue);
				return;
			}
			if (action is SetHeaderAction)
			{
				this.setHeaderName = new HeaderName?(((SetHeaderAction)action).MessageHeader);
				this.setHeaderValue = new HeaderValue?(((SetHeaderAction)action).HeaderValue);
				return;
			}
			if (action is RemoveHeaderAction)
			{
				this.removeHeader = new HeaderName?(((RemoveHeaderAction)action).MessageHeader);
				return;
			}
			if (action is AddToRecipientAction)
			{
				this.addToRecipients = Utils.BuildRecipientIdArray(((AddToRecipientAction)action).Addresses);
				return;
			}
			if (action is CopyToAction)
			{
				this.copyTo = Utils.BuildRecipientIdArray(((CopyToAction)action).Addresses);
				return;
			}
			if (action is BlindCopyToAction)
			{
				this.blindCopyTo = Utils.BuildRecipientIdArray(((BlindCopyToAction)action).Addresses);
				return;
			}
			if (action is AddManagerAsRecipientTypeAction)
			{
				this.addManagerAsRecipientType = new AddedRecipientType?(((AddManagerAsRecipientTypeAction)action).RecipientType);
				return;
			}
			if (action is ModerateMessageByUserAction)
			{
				this.moderateMessageByUser = Utils.BuildRecipientIdArray(((ModerateMessageByUserAction)action).Addresses);
				return;
			}
			if (action is ModerateMessageByManagerAction)
			{
				this.moderateMessageByManager = true;
				return;
			}
			if (action is RedirectMessageAction)
			{
				this.redirectMessageTo = Utils.BuildRecipientIdArray(((RedirectMessageAction)action).Addresses);
				return;
			}
			if (action is RejectMessageAction)
			{
				this.rejectMessageReasonText = new DsnText?(((RejectMessageAction)action).RejectReason);
				this.rejectMessageEnhancedStatusCode = new RejectEnhancedStatus?(((RejectMessageAction)action).EnhancedStatusCode);
				return;
			}
			if (action is DeleteMessageAction)
			{
				this.deleteMessage = true;
				return;
			}
			if (action is DisconnectAction)
			{
				this.disconnect = true;
				return;
			}
			if (action is QuarantineAction)
			{
				this.quarantine = true;
				return;
			}
			if (action is LogEventAction)
			{
				this.logEventText = new EventLogText?(((LogEventAction)action).EventMessage);
				return;
			}
			if (action is SmtpRejectMessageAction)
			{
				this.smtpRejectMessageRejectStatusCode = new RejectStatusCode?(((SmtpRejectMessageAction)action).StatusCode);
				this.smtpRejectMessageRejectText = new RejectText?(((SmtpRejectMessageAction)action).RejectReason);
				return;
			}
			if (action is StopRuleProcessingAction)
			{
				this.stopRuleProcessing = true;
				return;
			}
			if (action is NotifySenderAction)
			{
				this.senderNotificationType = new NotifySenderType?(((NotifySenderAction)action).SenderNotificationType);
				this.rejectMessageReasonText = new DsnText?(((NotifySenderAction)action).RejectReason);
				this.rejectMessageEnhancedStatusCode = new RejectEnhancedStatus?(((NotifySenderAction)action).EnhancedStatusCode);
				return;
			}
			if (action is GenerateIncidentReportAction)
			{
				this.GenerateIncidentReport = new RecipientIdParameter(((GenerateIncidentReportAction)action).ReportDestination.ToString());
				this.IncidentReportOriginalMail = new IncidentReportOriginalMail?(((GenerateIncidentReportAction)action).IncidentReportOriginalMail);
				this.IncidentReportContent = ((GenerateIncidentReportAction)action).IncidentReportContent;
				return;
			}
			if (action is RouteMessageOutboundConnectorAction)
			{
				this.RouteMessageOutboundConnector = ((RouteMessageOutboundConnectorAction)action).ConnectorName;
				return;
			}
			if (action is RouteMessageOutboundRequireTlsAction)
			{
				this.RouteMessageOutboundRequireTls = true;
				return;
			}
			if (action is ApplyOMEAction)
			{
				this.ApplyOME = true;
				return;
			}
			if (action is RemoveOMEAction)
			{
				this.RemoveOME = true;
				return;
			}
			if (action is GenerateNotificationAction)
			{
				this.GenerateNotification = new DisclaimerText?(((GenerateNotificationAction)action).NotificationContent);
			}
		}

		internal string ToCmdlet()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("New-TransportRule ");
			stringBuilder.Append(string.Format("-{0} {1} ", "Name", Utils.QuoteCmdletParameter(base.Name)));
			if (Guid.Empty != this.DlpPolicyId)
			{
				stringBuilder.Append(string.Format("-{0} {1} ", "DlpPolicy", this.DlpPolicyId));
			}
			if (!string.IsNullOrEmpty(this.Comments))
			{
				stringBuilder.Append(string.Format("-{0} {1} ", "Comments", Utils.QuoteCmdletParameter(this.Comments)));
			}
			if (this.ActivationDate != null)
			{
				stringBuilder.Append(string.Format("-{0} {1} ", "ActivationDate", Utils.QuoteCmdletParameter(this.ActivationDate.Value.ToString())));
			}
			if (this.ExpiryDate != null)
			{
				stringBuilder.Append(string.Format("-{0} {1} ", "ExpiryDate", Utils.QuoteCmdletParameter(this.ExpiryDate.Value.ToString())));
			}
			if (this.State != RuleState.Enabled)
			{
				stringBuilder.Append(string.Format("-{0} $False ", "Enabled"));
			}
			stringBuilder.Append(string.Format("-{0} {1} ", "Mode", Enum.GetName(typeof(RuleMode), this.Mode)));
			if (this.RuleSubType != RuleSubType.None)
			{
				stringBuilder.Append(string.Format("-{0} {1} ", "RuleSubType", Enum.GetName(typeof(RuleSubType), this.RuleSubType)));
			}
			if (this.RuleErrorAction != RuleErrorAction.Ignore)
			{
				stringBuilder.Append(string.Format("-{0} {1} ", "RuleErrorAction", Enum.GetName(typeof(RuleErrorAction), this.RuleErrorAction)));
			}
			if (this.SenderAddressLocation != SenderAddressLocation.Header)
			{
				stringBuilder.Append(string.Format("-{0} {1} ", "SenderAddressLocation", Enum.GetName(typeof(SenderAddressLocation), this.SenderAddressLocation)));
			}
			if (this.Conditions != null)
			{
				foreach (TransportRulePredicate transportRulePredicate in this.Conditions)
				{
					stringBuilder.Append(transportRulePredicate.ToCmdletParameter(false));
					stringBuilder.Append(' ');
				}
			}
			if (this.Exceptions != null)
			{
				foreach (TransportRulePredicate transportRulePredicate2 in this.Exceptions)
				{
					stringBuilder.Append(transportRulePredicate2.ToCmdletParameter(true));
					stringBuilder.Append(' ');
				}
			}
			if (this.Actions != null)
			{
				foreach (TransportRuleAction transportRuleAction in this.Actions)
				{
					stringBuilder.Append(transportRuleAction.ToCmdletParameter());
					stringBuilder.Append(' ');
				}
			}
			return stringBuilder.ToString().Trim();
		}

		internal override void SuppressPiiData(PiiMap piiMap)
		{
			base.SuppressPiiData(piiMap);
			if (this.conditions != null)
			{
				foreach (TransportRulePredicate transportRulePredicate in this.conditions)
				{
					transportRulePredicate.SuppressPiiData();
				}
			}
			if (this.exceptions != null)
			{
				foreach (TransportRulePredicate transportRulePredicate2 in this.exceptions)
				{
					transportRulePredicate2.SuppressPiiData();
				}
			}
			if (this.actions != null)
			{
				foreach (TransportRuleAction transportRuleAction in this.actions)
				{
					transportRuleAction.SuppressPiiData();
				}
			}
			this.dlpPolicy = SuppressingPiiProperty.TryRedactValue<string>(RuleSchema.DlpPolicy, this.dlpPolicy);
			this.comments = SuppressingPiiProperty.TryRedactValue<string>(RuleSchema.Comments, this.comments);
			this.from = SuppressingPiiProperty.TryRedactValue<RecipientIdParameter[]>(RuleSchema.From, this.from);
			this.fromMemberOf = SuppressingPiiProperty.TryRedactValue<RecipientIdParameter[]>(RuleSchema.FromMemberOf, this.fromMemberOf);
			this.sentTo = SuppressingPiiProperty.TryRedactValue<RecipientIdParameter[]>(RuleSchema.SentTo, this.sentTo);
			this.sentToMemberOf = SuppressingPiiProperty.TryRedactValue<RecipientIdParameter[]>(RuleSchema.SentToMemberOf, this.sentToMemberOf);
			this.betweenMemberOf1 = SuppressingPiiProperty.TryRedactValue<RecipientIdParameter[]>(RuleSchema.BetweenMemberOf1, this.betweenMemberOf1);
			this.betweenMemberOf2 = SuppressingPiiProperty.TryRedactValue<RecipientIdParameter[]>(RuleSchema.BetweenMemberOf2, this.betweenMemberOf2);
			this.managerAddresses = SuppressingPiiProperty.TryRedactValue<RecipientIdParameter[]>(RuleSchema.ManagerAddresses, this.managerAddresses);
			this.senderADAttributeContainsWords = Utils.RedactNameValuePairWords(this.senderADAttributeContainsWords);
			this.senderADAttributeMatchesPatterns = Utils.RedactNameValuePairPatterns(this.senderADAttributeMatchesPatterns);
			this.recipientADAttributeContainsWords = Utils.RedactNameValuePairWords(this.recipientADAttributeContainsWords);
			this.recipientADAttributeMatchesPatterns = Utils.RedactNameValuePairPatterns(this.recipientADAttributeMatchesPatterns);
			this.anyOfToHeader = SuppressingPiiProperty.TryRedactValue<RecipientIdParameter[]>(RuleSchema.AnyOfToHeader, this.anyOfToHeader);
			this.anyOfToHeaderMemberOf = SuppressingPiiProperty.TryRedactValue<RecipientIdParameter[]>(RuleSchema.AnyOfToHeaderMemberOf, this.anyOfToHeaderMemberOf);
			this.anyOfCcHeader = SuppressingPiiProperty.TryRedactValue<RecipientIdParameter[]>(RuleSchema.AnyOfCcHeader, this.anyOfCcHeader);
			this.anyOfCcHeaderMemberOf = SuppressingPiiProperty.TryRedactValue<RecipientIdParameter[]>(RuleSchema.AnyOfCcHeaderMemberOf, this.anyOfCcHeaderMemberOf);
			this.anyOfToCcHeader = SuppressingPiiProperty.TryRedactValue<RecipientIdParameter[]>(RuleSchema.AnyOfToCcHeader, this.anyOfToCcHeader);
			this.anyOfToCcHeaderMemberOf = SuppressingPiiProperty.TryRedactValue<RecipientIdParameter[]>(RuleSchema.AnyOfToCcHeaderMemberOf, this.anyOfToCcHeaderMemberOf);
			this.subjectContainsWords = SuppressingPiiProperty.TryRedactValue<Word[]>(RuleSchema.SubjectContainsWords, this.subjectContainsWords);
			this.subjectOrBodyContainsWords = SuppressingPiiProperty.TryRedactValue<Word[]>(RuleSchema.SubjectOrBodyContainsWords, this.subjectOrBodyContainsWords);
			this.headerContainsMessageHeader = SuppressingPiiProperty.TryRedactValue<HeaderName?>(RuleSchema.HeaderContainsMessageHeader, this.headerContainsMessageHeader);
			this.headerContainsWords = SuppressingPiiProperty.TryRedactValue<Word[]>(RuleSchema.HeaderContainsWords, this.headerContainsWords);
			this.fromAddressContainsWords = SuppressingPiiProperty.TryRedactValue<Word[]>(RuleSchema.FromAddressContainsWords, this.fromAddressContainsWords);
			this.senderDomainIs = SuppressingPiiProperty.TryRedactValue<Word[]>(RuleSchema.SenderDomainIs, this.senderDomainIs);
			this.recipientDomainIs = SuppressingPiiProperty.TryRedactValue<Word[]>(RuleSchema.RecipientDomainIs, this.recipientDomainIs);
			this.subjectMatchesPatterns = Utils.RedactPatterns(this.subjectMatchesPatterns);
			this.subjectOrBodyMatchesPatterns = Utils.RedactPatterns(this.subjectOrBodyMatchesPatterns);
			this.headerMatchesMessageHeader = SuppressingPiiProperty.TryRedactValue<HeaderName?>(RuleSchema.HeaderMatchesMessageHeader, this.headerMatchesMessageHeader);
			this.headerMatchesPatterns = Utils.RedactPatterns(this.headerMatchesPatterns);
			this.fromAddressMatchesPatterns = Utils.RedactPatterns(this.fromAddressMatchesPatterns);
			this.attachmentNameMatchesPatterns = Utils.RedactPatterns(this.attachmentNameMatchesPatterns);
			this.attachmentExtensionMatchesWords = SuppressingPiiProperty.TryRedactValue<Word[]>(RuleSchema.AttachmentExtensionMatchesWords, this.attachmentExtensionMatchesWords);
			this.attachmentPropertyContainsWords = SuppressingPiiProperty.TryRedactValue<Word[]>(RuleSchema.AttachmentPropertyContainsWords, this.AttachmentPropertyContainsWords);
			this.contentCharacterSetContainsWords = SuppressingPiiProperty.TryRedactValue<Word[]>(RuleSchema.ContentCharacterSetContainsWords, this.contentCharacterSetContainsWords);
			this.recipientAddressContainsWords = SuppressingPiiProperty.TryRedactValue<Word[]>(RuleSchema.RecipientAddressContainsWords, this.recipientAddressContainsWords);
			this.recipientAddressMatchesPatterns = Utils.RedactPatterns(this.recipientAddressMatchesPatterns);
			this.senderInRecipientList = SuppressingPiiProperty.TryRedactValue<Word[]>(RuleSchema.SenderInRecipientList, this.senderInRecipientList);
			this.recipientInSenderList = SuppressingPiiProperty.TryRedactValue<Word[]>(RuleSchema.RecipientInSenderList, this.recipientInSenderList);
			this.attachmentContainsWords = SuppressingPiiProperty.TryRedactValue<Word[]>(RuleSchema.AttachmentContainsWords, this.attachmentContainsWords);
			this.attachmentMatchesPatterns = Utils.RedactPatterns(this.attachmentMatchesPatterns);
			this.anyOfRecipientAddressContainsWords = SuppressingPiiProperty.TryRedactValue<Word[]>(RuleSchema.AnyOfRecipientAddressContainsWords, this.anyOfRecipientAddressContainsWords);
			this.anyOfRecipientAddressMatchesPatterns = Utils.RedactPatterns(this.anyOfRecipientAddressMatchesPatterns);
			this.exceptIfFrom = SuppressingPiiProperty.TryRedactValue<RecipientIdParameter[]>(RuleSchema.ExceptIfFrom, this.exceptIfFrom);
			this.exceptIfFromMemberOf = SuppressingPiiProperty.TryRedactValue<RecipientIdParameter[]>(RuleSchema.ExceptIfFromMemberOf, this.exceptIfFromMemberOf);
			this.exceptIfSentTo = SuppressingPiiProperty.TryRedactValue<RecipientIdParameter[]>(RuleSchema.ExceptIfSentTo, this.exceptIfSentTo);
			this.exceptIfSentToMemberOf = SuppressingPiiProperty.TryRedactValue<RecipientIdParameter[]>(RuleSchema.ExceptIfSentToMemberOf, this.exceptIfSentToMemberOf);
			this.exceptIfBetweenMemberOf1 = SuppressingPiiProperty.TryRedactValue<RecipientIdParameter[]>(RuleSchema.ExceptIfBetweenMemberOf1, this.exceptIfBetweenMemberOf1);
			this.exceptIfBetweenMemberOf2 = SuppressingPiiProperty.TryRedactValue<RecipientIdParameter[]>(RuleSchema.ExceptIfBetweenMemberOf2, this.exceptIfBetweenMemberOf2);
			this.exceptIfManagerAddresses = SuppressingPiiProperty.TryRedactValue<RecipientIdParameter[]>(RuleSchema.ExceptIfManagerAddresses, this.exceptIfManagerAddresses);
			this.exceptIfSenderADAttributeContainsWords = SuppressingPiiProperty.TryRedactValue<Word[]>(RuleSchema.ExceptIfSenderADAttributeContainsWords, this.exceptIfSenderADAttributeContainsWords);
			this.exceptIfSenderADAttributeMatchesPatterns = Utils.RedactPatterns(this.exceptIfSenderADAttributeMatchesPatterns);
			this.exceptIfRecipientADAttributeContainsWords = SuppressingPiiProperty.TryRedactValue<Word[]>(RuleSchema.ExceptIfRecipientADAttributeContainsWords, this.exceptIfRecipientADAttributeContainsWords);
			this.exceptIfRecipientADAttributeMatchesPatterns = Utils.RedactPatterns(this.exceptIfRecipientADAttributeMatchesPatterns);
			this.exceptIfAnyOfToHeader = SuppressingPiiProperty.TryRedactValue<RecipientIdParameter[]>(RuleSchema.ExceptIfAnyOfToHeader, this.exceptIfAnyOfToHeader);
			this.exceptIfAnyOfToHeaderMemberOf = SuppressingPiiProperty.TryRedactValue<RecipientIdParameter[]>(RuleSchema.ExceptIfAnyOfToHeaderMemberOf, this.exceptIfAnyOfToHeaderMemberOf);
			this.exceptIfAnyOfCcHeader = SuppressingPiiProperty.TryRedactValue<RecipientIdParameter[]>(RuleSchema.ExceptIfAnyOfCcHeader, this.exceptIfAnyOfCcHeader);
			this.exceptIfAnyOfCcHeaderMemberOf = SuppressingPiiProperty.TryRedactValue<RecipientIdParameter[]>(RuleSchema.ExceptIfAnyOfCcHeaderMemberOf, this.exceptIfAnyOfCcHeaderMemberOf);
			this.exceptIfAnyOfToCcHeader = SuppressingPiiProperty.TryRedactValue<RecipientIdParameter[]>(RuleSchema.ExceptIfAnyOfToCcHeader, this.exceptIfAnyOfToCcHeader);
			this.exceptIfAnyOfToCcHeaderMemberOf = SuppressingPiiProperty.TryRedactValue<RecipientIdParameter[]>(RuleSchema.ExceptIfAnyOfToCcHeaderMemberOf, this.exceptIfAnyOfToCcHeaderMemberOf);
			this.exceptIfSubjectContainsWords = SuppressingPiiProperty.TryRedactValue<Word[]>(RuleSchema.ExceptIfSubjectContainsWords, this.exceptIfSubjectContainsWords);
			this.exceptIfSubjectOrBodyContainsWords = SuppressingPiiProperty.TryRedactValue<Word[]>(RuleSchema.ExceptIfSubjectOrBodyContainsWords, this.exceptIfSubjectOrBodyContainsWords);
			this.exceptIfHeaderContainsMessageHeader = SuppressingPiiProperty.TryRedactValue<HeaderName?>(RuleSchema.ExceptIfHeaderContainsMessageHeader, this.exceptIfHeaderContainsMessageHeader);
			this.exceptIfHeaderContainsWords = SuppressingPiiProperty.TryRedactValue<Word[]>(RuleSchema.ExceptIfHeaderContainsWords, this.exceptIfHeaderContainsWords);
			this.exceptIfFromAddressContainsWords = SuppressingPiiProperty.TryRedactValue<Word[]>(RuleSchema.ExceptIfFromAddressContainsWords, this.exceptIfFromAddressContainsWords);
			this.exceptIfSenderDomainIs = SuppressingPiiProperty.TryRedactValue<Word[]>(RuleSchema.ExceptIfSenderDomainIs, this.exceptIfSenderDomainIs);
			this.exceptIfRecipientDomainIs = SuppressingPiiProperty.TryRedactValue<Word[]>(RuleSchema.ExceptIfRecipientDomainIs, this.exceptIfRecipientDomainIs);
			this.exceptIfSubjectMatchesPatterns = Utils.RedactPatterns(this.exceptIfSubjectMatchesPatterns);
			this.exceptIfSubjectOrBodyMatchesPatterns = Utils.RedactPatterns(this.exceptIfSubjectOrBodyMatchesPatterns);
			this.exceptIfHeaderMatchesMessageHeader = SuppressingPiiProperty.TryRedactValue<HeaderName?>(RuleSchema.ExceptIfHeaderMatchesMessageHeader, this.exceptIfHeaderMatchesMessageHeader);
			this.exceptIfHeaderMatchesPatterns = Utils.RedactPatterns(this.exceptIfHeaderMatchesPatterns);
			this.exceptIfFromAddressMatchesPatterns = Utils.RedactPatterns(this.exceptIfFromAddressMatchesPatterns);
			this.exceptIfAttachmentNameMatchesPatterns = Utils.RedactPatterns(this.exceptIfAttachmentNameMatchesPatterns);
			this.exceptIfAttachmentExtensionMatchesWords = SuppressingPiiProperty.TryRedactValue<Word[]>(RuleSchema.ExceptIfAttachmentExtensionMatchesWords, this.exceptIfAttachmentExtensionMatchesWords);
			this.exceptIfAttachmentPropertyContainsWords = SuppressingPiiProperty.TryRedactValue<Word[]>(RuleSchema.ExceptIfAttachmentPropertyContainsWords, this.exceptIfAttachmentPropertyContainsWords);
			this.exceptIfContentCharacterSetContainsWords = SuppressingPiiProperty.TryRedactValue<Word[]>(RuleSchema.ExceptIfContentCharacterSetContainsWords, this.exceptIfContentCharacterSetContainsWords);
			this.exceptIfRecipientAddressContainsWords = SuppressingPiiProperty.TryRedactValue<Word[]>(RuleSchema.ExceptIfRecipientAddressContainsWords, this.exceptIfRecipientAddressContainsWords);
			this.exceptIfRecipientAddressMatchesPatterns = Utils.RedactPatterns(this.exceptIfRecipientAddressMatchesPatterns);
			this.exceptIfSenderInRecipientList = SuppressingPiiProperty.TryRedactValue<Word[]>(RuleSchema.ExceptIfSenderInRecipientList, this.exceptIfSenderInRecipientList);
			this.exceptIfRecipientInSenderList = SuppressingPiiProperty.TryRedactValue<Word[]>(RuleSchema.ExceptIfRecipientInSenderList, this.exceptIfRecipientInSenderList);
			this.exceptIfAttachmentContainsWords = SuppressingPiiProperty.TryRedactValue<Word[]>(RuleSchema.ExceptIfAttachmentContainsWords, this.exceptIfAttachmentContainsWords);
			this.exceptIfAttachmentMatchesPatterns = Utils.RedactPatterns(this.exceptIfAttachmentMatchesPatterns);
			this.exceptIfAnyOfRecipientAddressContainsWords = SuppressingPiiProperty.TryRedactValue<Word[]>(RuleSchema.ExceptIfAnyOfRecipientAddressContainsWords, this.exceptIfAnyOfRecipientAddressContainsWords);
			this.exceptIfAnyOfRecipientAddressMatchesPatterns = Utils.RedactPatterns(this.exceptIfAnyOfRecipientAddressMatchesPatterns);
			this.exceptIfMessageContainsDataClassifications = SuppressingPiiProperty.TryRedactValue<string[]>(RuleSchema.ExceptIfMessageContainsDataClassifications, this.exceptIfMessageContainsDataClassifications);
			this.prependSubject = SuppressingPiiProperty.TryRedactValue<string>(RuleSchema.PrependSubject, this.prependSubject);
			this.applyHtmlDisclaimerText = SuppressingPiiProperty.TryRedactValue<DisclaimerText?>(RuleSchema.ApplyHtmlDisclaimerText, this.applyHtmlDisclaimerText);
			this.applyRightsProtectionTemplate = SuppressingPiiProperty.TryRedactValue<RmsTemplateIdentity>(RuleSchema.ApplyRightsProtectionTemplate, this.applyRightsProtectionTemplate);
			this.setHeaderName = SuppressingPiiProperty.TryRedactValue<HeaderName?>(RuleSchema.SetHeaderName, this.setHeaderName);
			this.setHeaderValue = SuppressingPiiProperty.TryRedactValue<HeaderValue?>(RuleSchema.SetHeaderValue, this.setHeaderValue);
			this.removeHeader = SuppressingPiiProperty.TryRedactValue<HeaderName?>(RuleSchema.RemoveHeader, this.removeHeader);
			this.addToRecipients = SuppressingPiiProperty.TryRedactValue<RecipientIdParameter[]>(RuleSchema.AddToRecipients, this.addToRecipients);
			this.copyTo = SuppressingPiiProperty.TryRedactValue<RecipientIdParameter[]>(RuleSchema.CopyTo, this.copyTo);
			this.blindCopyTo = SuppressingPiiProperty.TryRedactValue<RecipientIdParameter[]>(RuleSchema.BlindCopyTo, this.blindCopyTo);
			this.moderateMessageByUser = SuppressingPiiProperty.TryRedactValue<RecipientIdParameter[]>(RuleSchema.ModerateMessageByUser, this.moderateMessageByUser);
			this.redirectMessageTo = SuppressingPiiProperty.TryRedactValue<RecipientIdParameter[]>(RuleSchema.RedirectMessageTo, this.redirectMessageTo);
			this.rejectMessageReasonText = SuppressingPiiProperty.TryRedactValue<DsnText?>(RuleSchema.RejectMessageReasonText, this.rejectMessageReasonText);
			this.smtpRejectMessageRejectText = SuppressingPiiProperty.TryRedactValue<RejectText?>(RuleSchema.SmtpRejectMessageRejectText, this.smtpRejectMessageRejectText);
			this.logEventText = SuppressingPiiProperty.TryRedactValue<EventLogText?>(RuleSchema.LogEventText, this.logEventText);
			this.generateIncidentReport = SuppressingPiiProperty.TryRedactValue<RecipientIdParameter>(RuleSchema.GenerateIncidentReport, this.generateIncidentReport);
			this.connectorName = SuppressingPiiProperty.TryRedactValue<string>(RuleSchema.RouteMessageOutboundConnector, this.connectorName);
			this.generateNotification = SuppressingPiiProperty.TryRedactValue<DisclaimerText?>(RuleSchema.GenerateNotification, this.generateNotification);
		}

		public const int MaxCommentLength = 1024;

		public const int MaxDlpPolicyLength = 64;

		private static readonly RuleSchema schema = ObjectSchema.GetInstance<RuleSchema>();

		private int priority;

		private string dlpPolicy;

		private Guid dlpPolicyId;

		private string comments;

		private readonly bool manuallyModified;

		private TransportRulePredicate[] conditions;

		private TransportRulePredicate[] exceptions;

		private TransportRuleAction[] actions;

		private LocalizedString errorText = LocalizedString.Empty;

		private RecipientIdParameter[] from;

		private RecipientIdParameter[] fromMemberOf;

		private FromUserScope? fromScope;

		private RecipientIdParameter[] sentTo;

		private RecipientIdParameter[] sentToMemberOf;

		private ToUserScope? sentToScope;

		private RecipientIdParameter[] betweenMemberOf1;

		private RecipientIdParameter[] betweenMemberOf2;

		private RecipientIdParameter[] managerAddresses;

		private EvaluatedUser? managerForEvaluatedUser;

		private ManagementRelationship? senderManagementRelationship;

		private ADAttribute? adComparisonAttribute;

		private Evaluation? adComparisonOperator;

		private Word[] senderADAttributeContainsWords;

		private Pattern[] senderADAttributeMatchesPatterns;

		private Word[] recipientADAttributeContainsWords;

		private Pattern[] recipientADAttributeMatchesPatterns;

		private RecipientIdParameter[] anyOfToHeader;

		private RecipientIdParameter[] anyOfToHeaderMemberOf;

		private RecipientIdParameter[] anyOfCcHeader;

		private RecipientIdParameter[] anyOfCcHeaderMemberOf;

		private RecipientIdParameter[] anyOfToCcHeader;

		private RecipientIdParameter[] anyOfToCcHeaderMemberOf;

		private ADObjectId hasClassification;

		private bool hasNoClassification;

		private Word[] subjectContainsWords;

		private Word[] subjectOrBodyContainsWords;

		private HeaderName? headerContainsMessageHeader;

		private Word[] headerContainsWords;

		private Word[] fromAddressContainsWords;

		private Word[] senderDomainIs;

		private Word[] recipientDomainIs;

		private Pattern[] subjectMatchesPatterns;

		private Pattern[] subjectOrBodyMatchesPatterns;

		private HeaderName? headerMatchesMessageHeader;

		private Pattern[] headerMatchesPatterns;

		private Pattern[] fromAddressMatchesPatterns;

		private Pattern[] attachmentNameMatchesPatterns;

		private Word[] attachmentExtensionMatchesWords;

		private Word[] attachmentPropertyContainsWords;

		private Word[] contentCharacterSetContainsWords;

		private bool hasSenderOverride;

		private string[] messageContainsDataClassifications;

		private MultiValuedProperty<IPRange> senderIpRanges;

		private SclValue? sclOver;

		private ByteQuantifiedSize? attachmentSizeOver;

		private ByteQuantifiedSize? messageSizeOver;

		private Importance? withImportance;

		private MessageType? messageTypeMatches;

		private Word[] recipientAddressContainsWords;

		private Pattern[] recipientAddressMatchesPatterns;

		private Word[] senderInRecipientList;

		private Word[] recipientInSenderList;

		private Word[] attachmentContainsWords;

		private Pattern[] attachmentMatchesPatterns;

		private bool attachmentIsUnsupported;

		private bool attachmentProcessingLimitExceeded;

		private bool attachmentHasExecutableContent;

		private bool attachmentIsPasswordProtected;

		private Word[] anyOfRecipientAddressContainsWords;

		private Pattern[] anyOfRecipientAddressMatchesPatterns;

		private RecipientIdParameter[] exceptIfFrom;

		private RecipientIdParameter[] exceptIfFromMemberOf;

		private FromUserScope? exceptIfFromScope;

		private RecipientIdParameter[] exceptIfSentTo;

		private RecipientIdParameter[] exceptIfSentToMemberOf;

		private ToUserScope? exceptIfSentToScope;

		private RecipientIdParameter[] exceptIfBetweenMemberOf1;

		private RecipientIdParameter[] exceptIfBetweenMemberOf2;

		private RecipientIdParameter[] exceptIfManagerAddresses;

		private EvaluatedUser? exceptIfManagerForEvaluatedUser;

		private ManagementRelationship? exceptIfSenderManagementRelationship;

		private ADAttribute? exceptIfADComparisonAttribute;

		private Evaluation? exceptIfADComparisonOperator;

		private Word[] exceptIfSenderADAttributeContainsWords;

		private Pattern[] exceptIfSenderADAttributeMatchesPatterns;

		private Word[] exceptIfRecipientADAttributeContainsWords;

		private Pattern[] exceptIfRecipientADAttributeMatchesPatterns;

		private RecipientIdParameter[] exceptIfAnyOfToHeader;

		private RecipientIdParameter[] exceptIfAnyOfToHeaderMemberOf;

		private RecipientIdParameter[] exceptIfAnyOfCcHeader;

		private RecipientIdParameter[] exceptIfAnyOfCcHeaderMemberOf;

		private RecipientIdParameter[] exceptIfAnyOfToCcHeader;

		private RecipientIdParameter[] exceptIfAnyOfToCcHeaderMemberOf;

		private ADObjectId exceptIfHasClassification;

		private bool exceptIfHasNoClassification;

		private Word[] exceptIfSubjectContainsWords;

		private Word[] exceptIfSubjectOrBodyContainsWords;

		private HeaderName? exceptIfHeaderContainsMessageHeader;

		private Word[] exceptIfHeaderContainsWords;

		private Word[] exceptIfFromAddressContainsWords;

		private Word[] exceptIfSenderDomainIs;

		private Word[] exceptIfRecipientDomainIs;

		private Pattern[] exceptIfSubjectMatchesPatterns;

		private Pattern[] exceptIfSubjectOrBodyMatchesPatterns;

		private HeaderName? exceptIfHeaderMatchesMessageHeader;

		private Pattern[] exceptIfHeaderMatchesPatterns;

		private Pattern[] exceptIfFromAddressMatchesPatterns;

		private Pattern[] exceptIfAttachmentNameMatchesPatterns;

		private Word[] exceptIfAttachmentExtensionMatchesWords;

		private Word[] exceptIfAttachmentPropertyContainsWords;

		private Word[] exceptIfContentCharacterSetContainsWords;

		private bool exceptIfHasSenderOverride;

		private string[] exceptIfMessageContainsDataClassifications;

		private MultiValuedProperty<IPRange> exceptIfSenderIpanges;

		private SclValue? exceptIfSCLOver;

		private ByteQuantifiedSize? exceptIfAttachmentSizeOver;

		private ByteQuantifiedSize? exceptIfMessageSizeOver;

		private Importance? exceptIfWithImportance;

		private MessageType? exceptIfMessageTypeMatches;

		private Word[] exceptIfRecipientAddressContainsWords;

		private Pattern[] exceptIfRecipientAddressMatchesPatterns;

		private Word[] exceptIfSenderInRecipientList;

		private Word[] exceptIfRecipientInSenderList;

		private Word[] exceptIfAttachmentContainsWords;

		private Pattern[] exceptIfAttachmentMatchesPatterns;

		private bool exceptIfAttachmentIsUnsupported;

		private bool exceptIfAttachmentProcessingLimitExceeded;

		private bool exceptIfAttachmentHasExecutableContent;

		private bool exceptIfAttachmentIsPasswordProtected;

		private Word[] exceptIfAnyOfRecipientAddressContainsWords;

		private Pattern[] exceptIfAnyOfRecipientAddressMatchesPatterns;

		private string prependSubject;

		private string setAuditSeverity;

		private ADObjectId applyClassification;

		private DisclaimerLocation? applyHtmlDisclaimerLocation;

		private DisclaimerText? applyHtmlDisclaimerText;

		private DisclaimerFallbackAction? applyHtmlDisclaimerFallbackAction;

		private RmsTemplateIdentity applyRightsProtectionTemplate;

		private SclValue? setSCL;

		private HeaderName? setHeaderName;

		private HeaderValue? setHeaderValue;

		private HeaderName? removeHeader;

		private RecipientIdParameter[] addToRecipients;

		private RecipientIdParameter[] copyTo;

		private RecipientIdParameter[] blindCopyTo;

		private AddedRecipientType? addManagerAsRecipientType;

		private RecipientIdParameter[] moderateMessageByUser;

		private bool moderateMessageByManager;

		private RecipientIdParameter[] redirectMessageTo;

		private RejectEnhancedStatus? rejectMessageEnhancedStatusCode;

		private DsnText? rejectMessageReasonText;

		private bool deleteMessage;

		private bool disconnect;

		private bool quarantine;

		private RejectText? smtpRejectMessageRejectText;

		private RejectStatusCode? smtpRejectMessageRejectStatusCode;

		private EventLogText? logEventText;

		private DateTime? activationDate;

		private DateTime? expiryDate;

		private bool stopRuleProcessing;

		private NotifySenderType? senderNotificationType;

		private RecipientIdParameter generateIncidentReport;

		private IncidentReportOriginalMail? incidentReportOriginalMail;

		private IncidentReportContent[] incidentReportContent;

		private DisclaimerText? generateNotification;

		private string connectorName;
	}
}
