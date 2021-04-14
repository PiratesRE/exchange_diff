using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Management.Transport;
using Microsoft.Office.CompliancePolicy.PolicyEvaluation;

namespace Microsoft.Office.CompliancePolicy.Tasks
{
	public sealed class PsContentPropertyContainsWordsPredicate : PsContainsWordsPredicate
	{
		public PsContentPropertyContainsWordsPredicate(IEnumerable<string> words) : base(words)
		{
		}

		internal override PredicateCondition ToEnginePredicate()
		{
			PredicateCondition result = null;
			try
			{
				result = new ContentMetadataContainsPredicate(base.Words.ToList<string>());
			}
			catch (CompliancePolicyValidationException innerException)
			{
				throw new InvalidContentContentPropertyContainsWordsException(Strings.InvalidContentPropertyContainsWordsPredicate, innerException);
			}
			return result;
		}

		internal static PsContentPropertyContainsWordsPredicate FromEnginePredicate(ContentMetadataContainsPredicate condition)
		{
			return new PsContentPropertyContainsWordsPredicate((IEnumerable<string>)condition.Value.ParsedValue);
		}
	}
}
