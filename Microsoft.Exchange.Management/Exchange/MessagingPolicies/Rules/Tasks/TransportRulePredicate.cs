using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Exchange.Core.RuleTasks;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Tasks
{
	[Serializable]
	public abstract class TransportRulePredicate : RulePhrase
	{
		internal static TypeMapping[] GetAvailablePredicateMappings()
		{
			if (!Utils.IsEdgeRoleInstalled())
			{
				return TransportRulePredicate.BridgeheadMappings;
			}
			return TransportRulePredicate.EdgeMappings;
		}

		internal string LinkedDisplayTextException
		{
			get
			{
				return this.linkedDisplayTextException;
			}
		}

		internal static bool TryCreatePredicateFromCondition(TypeMapping[] mappings, Condition condition, out TransportRulePredicate predicate, IConfigDataProvider session)
		{
			foreach (TypeMapping typeMapping in mappings)
			{
				MethodInfo method = typeMapping.Type.GetMethod("CreateFromInternalConditionWithSession", BindingFlags.Static | BindingFlags.NonPublic);
				if (null != method)
				{
					TransportRulePredicate transportRulePredicate = (TransportRulePredicate)method.Invoke(null, new object[]
					{
						condition,
						session
					});
					if (transportRulePredicate != null)
					{
						predicate = transportRulePredicate;
						predicate.Initialize(mappings);
						return true;
					}
				}
				MethodInfo method2 = typeMapping.Type.GetMethod("CreateFromInternalCondition", BindingFlags.Static | BindingFlags.NonPublic);
				if (null != method2)
				{
					TransportRulePredicate transportRulePredicate = (TransportRulePredicate)method2.Invoke(null, new object[]
					{
						condition
					});
					if (transportRulePredicate != null)
					{
						predicate = transportRulePredicate;
						predicate.Initialize(mappings);
						return true;
					}
				}
			}
			predicate = null;
			return false;
		}

		internal static TransportRulePredicate CreatePredicate(TypeMapping[] mappings, string name, IConfigDataProvider session)
		{
			foreach (TypeMapping typeMapping in mappings)
			{
				if (name.Equals(typeMapping.Name, StringComparison.OrdinalIgnoreCase))
				{
					return TransportRulePredicate.InternalCreatePredicate(typeMapping.Type, mappings, session);
				}
			}
			return null;
		}

		internal static TransportRulePredicate[] CreateAllAvailablePredicates(TypeMapping[] mappings, IConfigDataProvider session)
		{
			TransportRulePredicate[] array = new TransportRulePredicate[mappings.Length];
			for (int i = 0; i < mappings.Length; i++)
			{
				array[i] = TransportRulePredicate.InternalCreatePredicate(mappings[i].Type, mappings, session);
			}
			return array;
		}

		internal void Initialize(TypeMapping[] mappings)
		{
			Type type = base.GetType();
			for (int i = 0; i < mappings.Length; i++)
			{
				TypeMapping typeMapping = mappings[i];
				if (type == typeMapping.Type)
				{
					this.SetReadOnlyProperties(typeMapping.Name, i, typeMapping.LinkedDisplayText, typeMapping.LinkedDisplayTextException);
					return;
				}
			}
			throw new NotSupportedException();
		}

		internal void SetReadOnlyProperties(string name, int rank, string linkedDisplayText, string linkedDisplayTextForException)
		{
			base.SetReadOnlyProperties(name, rank, linkedDisplayText);
			this.linkedDisplayTextException = linkedDisplayTextForException;
		}

		internal abstract Condition ToInternalCondition();

		[LocDisplayName(RulesTasksStrings.IDs.SubTypeDisplayName)]
		[LocDescription(RulesTasksStrings.IDs.SubTypeDescription)]
		public virtual IEnumerable<RuleSubType> RuleSubTypes
		{
			get
			{
				return new RuleSubType[]
				{
					RuleSubType.None
				};
			}
		}

		internal virtual string ToCmdletParameter(bool isException = false)
		{
			return ("-" + this.GetParameterName(isException) + " " + this.GetPredicateParameters()).Trim();
		}

		internal virtual string GetPredicateParameters()
		{
			return string.Empty;
		}

		private static TransportRulePredicate InternalCreatePredicate(Type predicateType, TypeMapping[] mappings, IConfigDataProvider session)
		{
			ConstructorInfo constructor = predicateType.GetConstructor(new Type[]
			{
				typeof(IConfigDataProvider)
			});
			TransportRulePredicate transportRulePredicate;
			if (constructor != null)
			{
				transportRulePredicate = (TransportRulePredicate)constructor.Invoke(new object[]
				{
					session
				});
			}
			else
			{
				transportRulePredicate = (TransportRulePredicate)Activator.CreateInstance(predicateType);
			}
			transportRulePredicate.Initialize(mappings);
			return transportRulePredicate;
		}

		protected string GetParameterName(bool isException = false)
		{
			if (isException)
			{
				IEnumerable<ExceptionParameterName> source = (IEnumerable<ExceptionParameterName>)base.GetType().GetCustomAttributes(typeof(ExceptionParameterName), true);
				if (source.Any<ExceptionParameterName>())
				{
					return source.First<ExceptionParameterName>().Name;
				}
			}
			else
			{
				IEnumerable<ConditionParameterName> source2 = (IEnumerable<ConditionParameterName>)base.GetType().GetCustomAttributes(typeof(ConditionParameterName), true);
				if (source2.Any<ConditionParameterName>())
				{
					return source2.First<ConditionParameterName>().Name;
				}
			}
			PropertyInfo[] properties = base.GetType().GetProperties();
			foreach (PropertyInfo propertyInfo in properties)
			{
				if (isException)
				{
					IEnumerable<ExceptionParameterName> source3 = (IEnumerable<ExceptionParameterName>)propertyInfo.GetCustomAttributes(typeof(ExceptionParameterName), true);
					if (source3.Any<ExceptionParameterName>())
					{
						return source3.First<ExceptionParameterName>().Name;
					}
				}
				else
				{
					IEnumerable<ConditionParameterName> source4 = (IEnumerable<ConditionParameterName>)propertyInfo.GetCustomAttributes(typeof(ConditionParameterName), true);
					if (source4.Any<ConditionParameterName>())
					{
						return source4.First<ConditionParameterName>().Name;
					}
				}
			}
			return string.Empty;
		}

		internal virtual void SuppressPiiData()
		{
		}

		internal static TypeMapping[] BridgeheadMappings = new TypeMapping[]
		{
			new TypeMapping("SentTo", typeof(SentToPredicate), RulesTasksStrings.LinkedPredicateSentTo, RulesTasksStrings.LinkedPredicateSentToException),
			new TypeMapping("RecipientAddressContainsWords", typeof(RecipientAddressContainsWordsPredicate), RulesTasksStrings.LinkedPredicateRecipientAddressContainsWords, RulesTasksStrings.LinkedPredicateRecipientAddressContainsWordsException),
			new TypeMapping("RecipientAddressMatchesPatterns", typeof(RecipientAddressMatchesPatternsPredicate), RulesTasksStrings.LinkedPredicateRecipientAddressMatchesPatterns, RulesTasksStrings.LinkedPredicateRecipientAddressMatchesPatternsException),
			new TypeMapping("SentToScope", typeof(SentToScopePredicate), RulesTasksStrings.LinkedPredicateSentToScope, RulesTasksStrings.LinkedPredicateSentToScopeException),
			new TypeMapping("BetweenMemberOf", typeof(BetweenMemberOfPredicate), RulesTasksStrings.LinkedPredicateBetweenMemberOf, RulesTasksStrings.LinkedPredicateBetweenMemberOfException),
			new TypeMapping("ManagerIs", typeof(ManagerIsPredicate), RulesTasksStrings.LinkedPredicateManagerIs, RulesTasksStrings.LinkedPredicateManagerIsException),
			new TypeMapping("ManagementRelationship", typeof(ManagementRelationshipPredicate), RulesTasksStrings.LinkedPredicateManagementRelationship, RulesTasksStrings.LinkedPredicateManagementRelationshipException),
			new TypeMapping("ADAttributeComparison", typeof(ADAttributeComparisonPredicate), RulesTasksStrings.LinkedPredicateADAttributeComparison, RulesTasksStrings.LinkedPredicateADAttributeComparisonException),
			new TypeMapping("RecipientADAttributeContainsWords", typeof(RecipientAttributeContainsPredicate), RulesTasksStrings.LinkedPredicateRecipientAttributeContains, RulesTasksStrings.LinkedPredicateRecipientAttributeContainsException),
			new TypeMapping("RecipientADAttributeMatchesPatterns", typeof(RecipientAttributeMatchesPredicate), RulesTasksStrings.LinkedPredicateRecipientAttributeMatches, RulesTasksStrings.LinkedPredicateRecipientAttributeMatchesException),
			new TypeMapping("SenderInRecipientList", typeof(SenderInRecipientListPredicate), RulesTasksStrings.LinkedPredicateSenderInRecipientList, RulesTasksStrings.LinkedPredicateSenderInRecipientListException),
			new TypeMapping("RecipientInSenderList", typeof(RecipientInSenderListPredicate), RulesTasksStrings.LinkedPredicateRecipientInSenderList, RulesTasksStrings.LinkedPredicateRecipientInSenderListException),
			new TypeMapping("SentToMemberOf", typeof(SentToMemberOfPredicate), RulesTasksStrings.LinkedPredicateSentToMemberOf, RulesTasksStrings.LinkedPredicateSentToMemberOfException),
			new TypeMapping("SCLOver", typeof(SclOverPredicate), RulesTasksStrings.LinkedPredicateSclOver, RulesTasksStrings.LinkedPredicateSclOverException),
			new TypeMapping("WithImportance", typeof(WithImportancePredicate), RulesTasksStrings.LinkedPredicateWithImportance, RulesTasksStrings.LinkedPredicateWithImportanceException),
			new TypeMapping("MessageTypeMatches", typeof(MessageTypeMatchesPredicate), RulesTasksStrings.LinkedPredicateMessageTypeMatches, RulesTasksStrings.LinkedPredicateMessageTypeMatchesException),
			new TypeMapping("From", typeof(FromPredicate), RulesTasksStrings.LinkedPredicateFrom, RulesTasksStrings.LinkedPredicateFromException),
			new TypeMapping("AnyOfToHeader", typeof(AnyOfToHeaderPredicate), RulesTasksStrings.LinkedPredicateAnyOfToHeader, RulesTasksStrings.LinkedPredicateAnyOfToHeaderException),
			new TypeMapping("AnyOfCcHeader", typeof(AnyOfCcHeaderPredicate), RulesTasksStrings.LinkedPredicateAnyOfCcHeader, RulesTasksStrings.LinkedPredicateAnyOfCcHeaderException),
			new TypeMapping("AnyOfToCcHeader", typeof(AnyOfToCcHeaderPredicate), RulesTasksStrings.LinkedPredicateAnyOfToCcHeader, RulesTasksStrings.LinkedPredicateAnyOfToCcHeaderException),
			new TypeMapping("SenderIpRanges", typeof(SenderIpRangesPredicate), RulesTasksStrings.LinkedPredicateSenderIpRanges, RulesTasksStrings.LinkedPredicateSenderIpRangesException),
			new TypeMapping("MessageSizeOver", typeof(MessageSizeOverPredicate), RulesTasksStrings.LinkedPredicateMessageSizeOver, RulesTasksStrings.LinkedPredicateMessageSizeOverException),
			new TypeMapping("AttachmentSizeOver", typeof(AttachmentSizeOverPredicate), RulesTasksStrings.LinkedPredicateAttachmentSizeOver, RulesTasksStrings.LinkedPredicateAttachmentSizeOverException),
			new TypeMapping("ContentCharacterSetContainsWords", typeof(ContentCharacterSetContainsWordsPredicate), RulesTasksStrings.LinkedPredicateContentCharacterSetContainsWords, RulesTasksStrings.LinkedPredicateContentCharacterSetContainsWordsException),
			new TypeMapping("HasClassification", typeof(HasClassificationPredicate), RulesTasksStrings.LinkedPredicateHasClassification, RulesTasksStrings.LinkedPredicateHasClassificationException),
			new TypeMapping("HasNoClassification", typeof(HasNoClassificationPredicate), RulesTasksStrings.LinkedPredicateHasNoClassification, RulesTasksStrings.LinkedPredicateHasNoClassificationException),
			new TypeMapping("HeaderContains", typeof(HeaderContainsPredicate), RulesTasksStrings.LinkedPredicateHeaderContains, RulesTasksStrings.LinkedPredicateHeaderContainsException),
			new TypeMapping("HasSenderOverride", typeof(HasSenderOverridePredicate), RulesTasksStrings.LinkedPredicateHasSenderOverride, RulesTasksStrings.LinkedPredicateHasSenderOverrideException),
			new TypeMapping("FromAddressContains", typeof(FromAddressContainsPredicate), RulesTasksStrings.LinkedPredicateFromAddressContains, RulesTasksStrings.LinkedPredicateFromAddressContainsException),
			new TypeMapping("RecipientDomainIs", typeof(RecipientDomainIsPredicate), RulesTasksStrings.LinkedPredicateRecipientDomainIs, RulesTasksStrings.LinkedPredicateRecipientDomainIsException),
			new TypeMapping("SenderDomainIs", typeof(SenderDomainIsPredicate), RulesTasksStrings.LinkedPredicateSenderDomainIs, RulesTasksStrings.LinkedPredicateSenderDomainIsException),
			new TypeMapping("AnyOfRecipientAddressContains", typeof(AnyOfRecipientAddressContainsPredicate), RulesTasksStrings.LinkedPredicateAnyOfRecipientAddressContains, RulesTasksStrings.LinkedPredicateAnyOfRecipientAddressContainsException),
			new TypeMapping("SubjectContains", typeof(SubjectContainsPredicate), RulesTasksStrings.LinkedPredicateSubjectContains, RulesTasksStrings.LinkedPredicateSubjectContainsException),
			new TypeMapping("SubjectOrBodyContains", typeof(SubjectOrBodyContainsPredicate), RulesTasksStrings.LinkedPredicateSubjectOrBodyContains, RulesTasksStrings.LinkedPredicateSubjectOrBodyContainsException),
			new TypeMapping("HeaderMatches", typeof(HeaderMatchesPredicate), RulesTasksStrings.LinkedPredicateHeaderMatches, RulesTasksStrings.LinkedPredicateHeaderMatchesException),
			new TypeMapping("AnyOfRecipientAddressMatches", typeof(AnyOfRecipientAddressMatchesPredicate), RulesTasksStrings.LinkedPredicateAnyOfRecipientAddressMatches, RulesTasksStrings.LinkedPredicateAnyOfRecipientAddressMatchesException),
			new TypeMapping("SubjectMatches", typeof(SubjectMatchesPredicate), RulesTasksStrings.LinkedPredicateSubjectMatches, RulesTasksStrings.LinkedPredicateSubjectMatchesException),
			new TypeMapping("SubjectOrBodyMatches", typeof(SubjectOrBodyMatchesPredicate), RulesTasksStrings.LinkedPredicateSubjectOrBodyMatches, RulesTasksStrings.LinkedPredicateSubjectOrBodyMatchesException),
			new TypeMapping("FromAddressMatches", typeof(FromAddressMatchesPredicate), RulesTasksStrings.LinkedPredicateFromAddressMatches, RulesTasksStrings.LinkedPredicateFromAddressMatchesException),
			new TypeMapping("AnyOfToHeaderMemberOf", typeof(AnyOfToHeaderMemberOfPredicate), RulesTasksStrings.LinkedPredicateAnyOfToHeaderMemberOf, RulesTasksStrings.LinkedPredicateAnyOfToHeaderMemberOfException),
			new TypeMapping("AnyOfCcHeaderMemberOf", typeof(AnyOfCcHeaderMemberOfPredicate), RulesTasksStrings.LinkedPredicateAnyOfCcHeaderMemberOf, RulesTasksStrings.LinkedPredicateAnyOfCcHeaderMemberOfException),
			new TypeMapping("AnyOfToCcHeaderMemberOf", typeof(AnyOfToCcHeaderMemberOfPredicate), RulesTasksStrings.LinkedPredicateAnyOfToCcHeaderMemberOf, RulesTasksStrings.LinkedPredicateAnyOfToCcHeaderMemberOfException),
			new TypeMapping("FromMemberOf", typeof(FromMemberOfPredicate), RulesTasksStrings.LinkedPredicateFromMemberOf, RulesTasksStrings.LinkedPredicateFromMemberOfException),
			new TypeMapping("FromScope", typeof(FromScopePredicate), RulesTasksStrings.LinkedPredicateFromScope, RulesTasksStrings.LinkedPredicateFromScopeException),
			new TypeMapping("SenderADAttributeContainsWords", typeof(SenderAttributeContainsPredicate), RulesTasksStrings.LinkedPredicateSenderAttributeContains, RulesTasksStrings.LinkedPredicateSenderAttributeContainsException),
			new TypeMapping("SenderADAttributeMatchesPatterns", typeof(SenderAttributeMatchesPredicate), RulesTasksStrings.LinkedPredicateSenderAttributeMatches, RulesTasksStrings.LinkedPredicateSenderAttributeMatchesException),
			new TypeMapping("AttachmentIsUnsupported", typeof(AttachmentIsUnsupportedPredicate), RulesTasksStrings.LinkedPredicateAttachmentIsUnsupported, RulesTasksStrings.LinkedPredicateAttachmentIsUnsupportedException),
			new TypeMapping("AttachmentIsPasswordProtected", typeof(AttachmentIsPasswordProtectedPredicate), RulesTasksStrings.LinkedPredicateAttachmentIsPasswordProtected, RulesTasksStrings.LinkedPredicateAttachmentIsPasswordProtectedException),
			new TypeMapping("AttachmentProcessingLimitExceeded", typeof(AttachmentProcessingLimitExceededPredicate), RulesTasksStrings.LinkedPredicateAttachmentProcessingLimitExceeded, RulesTasksStrings.LinkedPredicateAttachmentProcessingLimitExceededException),
			new TypeMapping("AttachmentHasExecutableContent", typeof(AttachmentHasExecutableContentPredicate), RulesTasksStrings.LinkedPredicateAttachmentHasExecutableContentPredicate, RulesTasksStrings.LinkedPredicateAttachmentHasExecutableContentPredicateException),
			new TypeMapping("AttachmentExtensionMatchesWords", typeof(AttachmentExtensionMatchesWordsPredicate), RulesTasksStrings.LinkedPredicateAttachmentExtensionMatchesWords, RulesTasksStrings.LinkedPredicateAttachmentExtensionMatchesWordsException),
			new TypeMapping("AttachmentNameMatches", typeof(AttachmentNameMatchesPredicate), RulesTasksStrings.LinkedPredicateAttachmentNameMatches, RulesTasksStrings.LinkedPredicateAttachmentNameMatchesException),
			new TypeMapping("AttachmentContainsWords", typeof(AttachmentContainsWordsPredicate), RulesTasksStrings.LinkedPredicateAttachmentContainsWords, RulesTasksStrings.LinkedPredicateAttachmentContainsWordsException),
			new TypeMapping("AttachmentMatchesPatterns", typeof(AttachmentMatchesPatternsPredicate), RulesTasksStrings.LinkedPredicateAttachmentMatchesPatterns, RulesTasksStrings.LinkedPredicateAttachmentMatchesPatternsException),
			new TypeMapping("AttachmentPropertyContainsWords", typeof(AttachmentPropertyContainsWordsPredicate), RulesTasksStrings.LinkedPredicateAttachmentPropertyContainsWords, RulesTasksStrings.LinkedPredicateAttachmentPropertyContainsWordsException),
			new TypeMapping("MessageContainsDataClassifications", typeof(MessageContainsDataClassificationsPredicate), RulesTasksStrings.LinkedPredicateMessageContainsDataClassifications, RulesTasksStrings.LinkedPredicateMessageContainsDataClassificationsException)
		};

		internal static TypeMapping[] EdgeMappings = new TypeMapping[]
		{
			new TypeMapping("SCLOver", typeof(SclOverPredicate), RulesTasksStrings.LinkedPredicateSclOver, RulesTasksStrings.LinkedPredicateSclOverException),
			new TypeMapping("AttachmentSizeOver", typeof(AttachmentSizeOverPredicate), RulesTasksStrings.LinkedPredicateAttachmentSizeOver, RulesTasksStrings.LinkedPredicateAttachmentSizeOverException),
			new TypeMapping("MessageSizeOver", typeof(MessageSizeOverPredicate), RulesTasksStrings.LinkedPredicateMessageSizeOver, RulesTasksStrings.LinkedPredicateMessageSizeOverException),
			new TypeMapping("FromScope", typeof(FromScopePredicate), RulesTasksStrings.LinkedPredicateFromScope, RulesTasksStrings.LinkedPredicateFromScopeException),
			new TypeMapping("HeaderContains", typeof(HeaderContainsPredicate), RulesTasksStrings.LinkedPredicateHeaderContains, RulesTasksStrings.LinkedPredicateHeaderContainsException),
			new TypeMapping("SubjectContains", typeof(SubjectContainsPredicate), RulesTasksStrings.LinkedPredicateSubjectContains, RulesTasksStrings.LinkedPredicateSubjectContainsException),
			new TypeMapping("FromAddressContains", typeof(FromAddressContainsPredicate), RulesTasksStrings.LinkedPredicateFromAddressContains, RulesTasksStrings.LinkedPredicateFromAddressContainsException),
			new TypeMapping("SubjectOrBodyContains", typeof(SubjectOrBodyContainsPredicate), RulesTasksStrings.LinkedPredicateSubjectOrBodyContains, RulesTasksStrings.LinkedPredicateSubjectOrBodyContainsException),
			new TypeMapping("AnyOfRecipientAddressContains", typeof(AnyOfRecipientAddressContainsPredicate), RulesTasksStrings.LinkedPredicateAnyOfRecipientAddressContains, RulesTasksStrings.LinkedPredicateAnyOfRecipientAddressContainsException),
			new TypeMapping("HeaderMatches", typeof(HeaderMatchesPredicate), RulesTasksStrings.LinkedPredicateHeaderMatches, RulesTasksStrings.LinkedPredicateHeaderMatchesException),
			new TypeMapping("SubjectMatches", typeof(SubjectMatchesPredicate), RulesTasksStrings.LinkedPredicateSubjectMatches, RulesTasksStrings.LinkedPredicateSubjectMatchesException),
			new TypeMapping("SubjectOrBodyMatches", typeof(SubjectOrBodyMatchesPredicate), RulesTasksStrings.LinkedPredicateSubjectOrBodyMatches, RulesTasksStrings.LinkedPredicateSubjectOrBodyMatchesException),
			new TypeMapping("FromAddressMatches", typeof(FromAddressMatchesPredicate), RulesTasksStrings.LinkedPredicateFromAddressMatches, RulesTasksStrings.LinkedPredicateFromAddressMatchesException),
			new TypeMapping("AnyOfRecipientAddressMatches", typeof(AnyOfRecipientAddressMatchesPredicate), RulesTasksStrings.LinkedPredicateAnyOfRecipientAddressMatches, RulesTasksStrings.LinkedPredicateAnyOfRecipientAddressMatchesException)
		};

		private string linkedDisplayTextException;
	}
}
