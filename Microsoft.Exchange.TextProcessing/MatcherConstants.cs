using System;
using System.Text.RegularExpressions;

namespace Microsoft.Exchange.TextProcessing
{
	internal struct MatcherConstants
	{
		public const int MatchedTermsDictionaryInitialCapacity = 256;

		public const int TrieIDDictionaryInitialCapacity = 8;

		public static readonly double MinimumCoefficient = 0.5;

		public static readonly double MaximumCoefficient = 1.0;

		public static readonly TimeSpan DefaultRegexMatchTimeout = Regex.InfiniteMatchTimeout;
	}
}
