using System;

namespace Microsoft.Exchange.TextProcessing
{
	internal class ClusterContext
	{
		public int LowSimilarityBoundInt { get; set; }

		public int InternalStoreNumber { get; set; }

		public int SwapTimeInMinutes { get; set; }

		public int MergeTimeInMinutes { get; set; }

		public int CleanTimeInMinutes { get; set; }

		public int MaxHashSetSize { get; set; }

		public int FnFeedSizeAbove { get; set; }

		public int HoneypotFeedSizeAbove { get; set; }

		public int SenFeedSizeAbove { get; set; }

		public int ThirdPartyFeedSizeAbove { get; set; }

		public int SewrFeedSizeAbove { get; set; }

		public int SpamSizeAbove { get; set; }

		public int NearOneSourcePercentageAbove { get; set; }

		public int NumberOfRecipientDomainAbove { get; set; }

		public int NumberofSourcesMadeOfMajorSourcesAbove { get; set; }

		public int SpamFeedClusterSizeAbove { get; set; }

		public int SpamVerdictFeedClusterSizeAbove { get; set; }

		public int AllOneSourceClusterSizeAbove { get; set; }

		public int OneAndMultiSourceClusterSizeAbove { get; set; }

		public int AllMultiSourceClusterSizeAbove { get; set; }

		public bool UseBloomFilter { get; set; }

		public int PowerIndexOf2 { get; set; }

		public int MaxCountValue { get; set; }

		public int HashNumbers { get; set; }
	}
}
