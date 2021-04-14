using System;
using Microsoft.Exchange.Management.Transport;
using Microsoft.Office.CompliancePolicy.PolicyEvaluation;

namespace Microsoft.Office.CompliancePolicy.Tasks
{
	public abstract class PsComplianceRulePredicateBase
	{
		internal abstract PredicateCondition ToEnginePredicate();

		internal static PsComplianceRulePredicateBase FromEnginePredicate(PredicateCondition predicate)
		{
			if (predicate is GreaterThanOrEqualPredicate && predicate.Property.Name.Equals("Item.WhenCreated"))
			{
				return PsContentDateFromPredicate.FromEnginePredicate(predicate as GreaterThanOrEqualPredicate);
			}
			if (predicate is LessThanOrEqualPredicate && predicate.Property.Name.Equals("Item.WhenCreated"))
			{
				return PsContentDateToPredicate.FromEnginePredicate(predicate as LessThanOrEqualPredicate);
			}
			if (predicate is TextQueryPredicate)
			{
				return PsContentMatchQueryPredicate.FromEnginePredicate(predicate as TextQueryPredicate);
			}
			if (predicate is EqualPredicate && predicate.Property.Name.Equals("Item.AccessScope"))
			{
				return PsAccessScopeIsPredicate.FromEnginePredicate(predicate as EqualPredicate);
			}
			if (predicate is ContentContainsSensitiveInformationPredicate)
			{
				return PsContentContainsSensitiveInformationPredicate.FromEnginePredicate(predicate as ContentContainsSensitiveInformationPredicate);
			}
			if (predicate is ContentMetadataContainsPredicate)
			{
				return PsContentPropertyContainsWordsPredicate.FromEnginePredicate(predicate as ContentMetadataContainsPredicate);
			}
			throw new UnexpectedConditionOrActionDetectedException();
		}
	}
}
