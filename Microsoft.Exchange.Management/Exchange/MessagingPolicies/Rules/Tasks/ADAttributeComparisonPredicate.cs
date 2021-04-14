using System;
using Microsoft.Exchange.Core.RuleTasks;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Tasks
{
	[Serializable]
	public class ADAttributeComparisonPredicate : BifurcationInfoPredicate, IEquatable<ADAttributeComparisonPredicate>
	{
		public override int GetHashCode()
		{
			return this.ADAttribute.GetHashCode() ^ this.Evaluation.GetHashCode();
		}

		public override bool Equals(object right)
		{
			return !object.ReferenceEquals(right, null) && (object.ReferenceEquals(this, right) || (!(base.GetType() != right.GetType()) && this.Equals(right as ADAttributeComparisonPredicate)));
		}

		public bool Equals(ADAttributeComparisonPredicate other)
		{
			return this.ADAttribute.Equals(other.ADAttribute) && this.Evaluation.Equals(other.Evaluation);
		}

		[ExceptionParameterName("ExceptIfADComparisonAttribute")]
		[LocDisplayName(RulesTasksStrings.IDs.ADAttributeDisplayName)]
		[LocDescription(RulesTasksStrings.IDs.ADAttributeDescription)]
		[ConditionParameterName("ADComparisonAttribute")]
		public ADAttribute ADAttribute
		{
			get
			{
				return this.adAttribute;
			}
			set
			{
				this.adAttribute = value;
			}
		}

		[LocDisplayName(RulesTasksStrings.IDs.EvaluationDisplayName)]
		[LocDescription(RulesTasksStrings.IDs.EvaluationDescription)]
		[ConditionParameterName("ADComparisonOperator")]
		[ExceptionParameterName("ExceptIfADComparisonOperator")]
		public Evaluation Evaluation
		{
			get
			{
				return this.evaluation;
			}
			set
			{
				this.evaluation = value;
			}
		}

		internal override string Description
		{
			get
			{
				return RulesTasksStrings.RuleDescriptionADAttributeComparison(LocalizedDescriptionAttribute.FromEnum(typeof(ADAttribute), this.ADAttribute), LocalizedDescriptionAttribute.FromEnum(typeof(Evaluation), this.Evaluation));
			}
		}

		internal static TransportRulePredicate CreateFromInternalCondition(Condition condition)
		{
			return null;
		}

		internal static TransportRulePredicate CreatePredicateFromBifInfo(RuleBifurcationInfo bifInfo)
		{
			if (bifInfo.ADAttributesForTextMatch.Count > 0 || bifInfo.ADAttributes.Count == 0 || bifInfo.Managers.Count > 0 || bifInfo.Recipients.Count > 0 || bifInfo.Lists.Count > 0 || bifInfo.FromRecipients.Count > 0 || bifInfo.FromLists.Count > 0 || bifInfo.Partners.Count > 0 || bifInfo.RecipientAddressContainsWords.Count > 0 || bifInfo.RecipientDomainIs.Count > 0 || bifInfo.RecipientMatchesPatterns.Count > 0 || bifInfo.RecipientAttributeContains.Count > 0 || bifInfo.RecipientAttributeMatches.Count > 0 || bifInfo.SenderInRecipientList.Count > 0 || bifInfo.RecipientInSenderList.Count > 0)
			{
				return null;
			}
			if (bifInfo.InternalRecipients || bifInfo.ExternalRecipients || bifInfo.ExternalPartnerRecipients || bifInfo.ExternalNonPartnerRecipients)
			{
				return null;
			}
			if (!string.IsNullOrEmpty(bifInfo.ManagementRelationship))
			{
				return null;
			}
			ADAttributeComparisonPredicate adattributeComparisonPredicate = new ADAttributeComparisonPredicate
			{
				ADAttribute = (ADAttribute)Enum.Parse(typeof(ADAttribute), bifInfo.ADAttributes[0])
			};
			if (bifInfo.CheckADAttributeEquality)
			{
				adattributeComparisonPredicate.Evaluation = Evaluation.Equal;
			}
			else
			{
				adattributeComparisonPredicate.Evaluation = Evaluation.NotEqual;
			}
			return adattributeComparisonPredicate;
		}

		internal override RuleBifurcationInfo ToRuleBifurcationInfo(out RuleBifurcationInfo additionalBifurcationInfo)
		{
			additionalBifurcationInfo = null;
			return new RuleBifurcationInfo
			{
				ADAttributes = 
				{
					this.ADAttribute.ToString()
				},
				CheckADAttributeEquality = (this.Evaluation == Evaluation.Equal)
			};
		}

		internal override string ToCmdletParameter(bool isException = false)
		{
			return string.Format("-{0} {1} -{2} {3}", new object[]
			{
				isException ? "ExceptIfADComparisonAttribute" : "ADComparisonAttribute",
				Enum.GetName(typeof(ADAttribute), this.ADAttribute),
				isException ? "ExceptIfADComparisonOperator" : "ADComparisonOperator",
				Enum.GetName(typeof(Evaluation), this.Evaluation)
			});
		}

		private ADAttribute adAttribute;

		private Evaluation evaluation;
	}
}
