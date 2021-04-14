using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class StoreClassNameMatch
	{
		public static StoreClassNameMatch CreatePrefixMatch(string prefix)
		{
			return StoreClassNameMatch.CreateAndInitialize(StoreClassNameMatchType.Prefix, prefix, null);
		}

		public static StoreClassNameMatch CreatePrefixSuffixMatch(string prefix, string suffix)
		{
			return StoreClassNameMatch.CreateAndInitialize(StoreClassNameMatchType.PrefixSuffix, prefix, suffix);
		}

		private static StoreClassNameMatch CreateAndInitialize(StoreClassNameMatchType matchType, string prefix, string suffix)
		{
			return new StoreClassNameMatch
			{
				matchType = matchType,
				prefix = prefix,
				suffix = suffix
			};
		}

		public bool Match(string stringToTest)
		{
			switch (this.matchType)
			{
			case StoreClassNameMatchType.Prefix:
				return stringToTest.StartsWith(this.prefix, StringComparison.OrdinalIgnoreCase);
			case StoreClassNameMatchType.PrefixSuffix:
				return stringToTest.StartsWith(this.prefix, StringComparison.OrdinalIgnoreCase) && stringToTest.EndsWith(this.suffix, StringComparison.OrdinalIgnoreCase);
			default:
				return false;
			}
		}

		private string prefix;

		private string suffix;

		private StoreClassNameMatchType matchType;
	}
}
