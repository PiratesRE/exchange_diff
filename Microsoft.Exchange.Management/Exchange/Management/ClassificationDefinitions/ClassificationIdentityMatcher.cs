using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.ClassificationDefinitions
{
	[Serializable]
	internal abstract class ClassificationIdentityMatcher<TObjectWithIdentity> where TObjectWithIdentity : class
	{
		protected ClassificationIdentityMatcher(string searchName, string rawSearchName, MatchOptions matchOptions)
		{
			this.searchName = searchName;
			this.rawSearchName = rawSearchName;
			this.areSearchNameAndRawSearchNameEqual = string.Equals(this.searchName, this.rawSearchName, StringComparison.Ordinal);
			if (ClassificationIdentityMatcher<TObjectWithIdentity>.matchOperatorsTable.Value.ContainsKey(matchOptions))
			{
				this.matchOperator = ClassificationIdentityMatcher<TObjectWithIdentity>.matchOperatorsTable.Value[matchOptions];
				return;
			}
			ExAssert.RetailAssert(false, "Invalid MatchOptions value specified for ClassificationIdentityMatcher c'tor");
		}

		protected abstract bool EvaluateMatch(TObjectWithIdentity objectWithIdentityToMatch, Func<string, CultureInfo, CompareOptions, bool> matchEvaluator);

		protected static bool SubStringMatch(string objectPropertyValue, string searchTerm, CultureInfo cultureInfo, CompareOptions compareOption)
		{
			return cultureInfo.CompareInfo.IndexOf(objectPropertyValue, searchTerm, compareOption) != -1;
		}

		protected static bool PrefixMatch(string objectPropertyValue, string searchTerm, CultureInfo cultureInfo, CompareOptions compareOption)
		{
			return cultureInfo.CompareInfo.IsPrefix(objectPropertyValue, searchTerm, compareOption);
		}

		protected static bool SuffixMatch(string objectPropertyValue, string searchTerm, CultureInfo cultureInfo, CompareOptions compareOption)
		{
			return cultureInfo.CompareInfo.IsSuffix(objectPropertyValue, searchTerm, compareOption);
		}

		protected static bool ExactMatch(string objectPropertyValue, string searchTerm, CultureInfo cultureInfo, CompareOptions compareOption)
		{
			return 0 == cultureInfo.CompareInfo.Compare(objectPropertyValue, searchTerm, compareOption);
		}

		protected virtual bool MatchObjectPropertyValue(string objectPropertyValue, CultureInfo cultureInfo, CompareOptions compareOption)
		{
			bool flag = this.matchOperator(objectPropertyValue, this.searchName, cultureInfo, compareOption);
			if (!flag && this.matchOperator != new Func<string, string, CultureInfo, CompareOptions, bool>(ClassificationIdentityMatcher<TObjectWithIdentity>.ExactMatch) && !string.IsNullOrEmpty(this.rawSearchName) && !this.areSearchNameAndRawSearchNameEqual)
			{
				flag = ClassificationIdentityMatcher<TObjectWithIdentity>.ExactMatch(objectPropertyValue, this.rawSearchName, cultureInfo, compareOption);
			}
			return flag;
		}

		internal bool Match(TObjectWithIdentity objectWithIdentityToMatch)
		{
			return this.EvaluateMatch(objectWithIdentityToMatch, new Func<string, CultureInfo, CompareOptions, bool>(this.MatchObjectPropertyValue));
		}

		private static readonly Lazy<Dictionary<MatchOptions, Func<string, string, CultureInfo, CompareOptions, bool>>> matchOperatorsTable = new Lazy<Dictionary<MatchOptions, Func<string, string, CultureInfo, CompareOptions, bool>>>(() => new Dictionary<MatchOptions, Func<string, string, CultureInfo, CompareOptions, bool>>
		{
			{
				MatchOptions.FullString,
				new Func<string, string, CultureInfo, CompareOptions, bool>(ClassificationIdentityMatcher<TObjectWithIdentity>.ExactMatch)
			},
			{
				MatchOptions.Prefix,
				new Func<string, string, CultureInfo, CompareOptions, bool>(ClassificationIdentityMatcher<TObjectWithIdentity>.PrefixMatch)
			},
			{
				MatchOptions.Suffix,
				new Func<string, string, CultureInfo, CompareOptions, bool>(ClassificationIdentityMatcher<TObjectWithIdentity>.SuffixMatch)
			},
			{
				MatchOptions.SubString,
				new Func<string, string, CultureInfo, CompareOptions, bool>(ClassificationIdentityMatcher<TObjectWithIdentity>.SubStringMatch)
			}
		}, LazyThreadSafetyMode.PublicationOnly);

		private readonly string searchName;

		private readonly string rawSearchName;

		private readonly bool areSearchNameAndRawSearchNameEqual;

		protected readonly Func<string, string, CultureInfo, CompareOptions, bool> matchOperator;
	}
}
