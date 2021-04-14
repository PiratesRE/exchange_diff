using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	public class TargetDataClassification : DataClassification
	{
		public int MinCount { get; protected set; }

		public int MaxCount { get; protected set; }

		public int MinConfidence { get; protected set; }

		public int MaxConfidence { get; protected set; }

		public string OpaqueData { get; protected set; }

		public TargetDataClassification(string id, int minCount, int maxCount, int minConfidence, int maxConfidence, string opaqueData) : base(id)
		{
			this.MinCount = minCount;
			this.MaxCount = maxCount;
			this.MinConfidence = minConfidence;
			this.MaxConfidence = maxConfidence;
			this.OpaqueData = opaqueData;
		}

		internal TargetDataClassification(IEnumerable<KeyValuePair<string, string>> keyValueParameters)
		{
			base.Id = string.Empty;
			this.MinCount = 0;
			this.MaxCount = TargetDataClassification.IgnoreMaxCount;
			this.MinConfidence = TargetDataClassification.UseRecommendedMinConfidence;
			this.MaxConfidence = 0;
			foreach (KeyValuePair<string, string> keyValuePair in keyValueParameters)
			{
				if (string.Compare(keyValuePair.Key, TargetDataClassification.IdKey, true) == 0)
				{
					base.Id = keyValuePair.Value;
				}
				else if (string.Compare(keyValuePair.Key, TargetDataClassification.MinCountKey, true) == 0)
				{
					this.MinCount = Convert.ToInt32(keyValuePair.Value);
				}
				else if (string.Compare(keyValuePair.Key, TargetDataClassification.MaxCountKey, true) == 0)
				{
					this.MaxCount = Convert.ToInt32(keyValuePair.Value);
				}
				else if (string.Compare(keyValuePair.Key, TargetDataClassification.MinConfidenceKey, true) == 0)
				{
					this.MinConfidence = Convert.ToInt32(keyValuePair.Value);
				}
				else if (string.Compare(keyValuePair.Key, TargetDataClassification.MaxConfidenceKey, true) == 0)
				{
					this.MaxConfidence = Convert.ToInt32(keyValuePair.Value);
				}
				else if (string.Compare(keyValuePair.Key, TargetDataClassification.OpaqueDataKey, true) == 0)
				{
					this.OpaqueData = keyValuePair.Value;
				}
			}
			if (string.IsNullOrEmpty(base.Id) || this.MinCount < 1 || (this.MaxCount < 1 && this.MaxCount != TargetDataClassification.IgnoreMaxCount) || (this.MinConfidence < 1 && this.MinConfidence != TargetDataClassification.UseRecommendedMinConfidence) || this.MaxConfidence < 1 || this.MaxConfidence > TargetDataClassification.MaxAllowedConfidenceValue)
			{
				throw new ArgumentException(RulesStrings.InvalidKeyValueParameter("DataClassification"));
			}
		}

		internal ShortList<KeyValuePair<string, string>> ToKeyValueCollection()
		{
			return new ShortList<KeyValuePair<string, string>>
			{
				new KeyValuePair<string, string>(TargetDataClassification.IdKey, base.Id),
				new KeyValuePair<string, string>(TargetDataClassification.MinCountKey, this.MinCount.ToString()),
				new KeyValuePair<string, string>(TargetDataClassification.MaxCountKey, this.MaxCount.ToString()),
				new KeyValuePair<string, string>(TargetDataClassification.MinConfidenceKey, this.MinConfidence.ToString()),
				new KeyValuePair<string, string>(TargetDataClassification.MaxConfidenceKey, this.MaxConfidence.ToString()),
				new KeyValuePair<string, string>(TargetDataClassification.OpaqueDataKey, this.OpaqueData)
			};
		}

		internal bool Matches(DiscoveredDataClassification discovered)
		{
			return discovered.Id == base.Id && discovered.TotalCount >= this.MinCount && (this.MaxCount == TargetDataClassification.IgnoreMaxCount || discovered.TotalCount <= this.MaxCount) && ((this.MinConfidence == TargetDataClassification.UseRecommendedMinConfidence && discovered.MaxConfidenceLevel >= discovered.RecommendedMinimumConfidence) || (this.MinConfidence != TargetDataClassification.UseRecommendedMinConfidence && (ulong)discovered.MaxConfidenceLevel >= (ulong)((long)this.MinConfidence))) && (ulong)discovered.MaxConfidenceLevel <= (ulong)((long)this.MaxConfidence);
		}

		public static readonly int UseRecommendedMinConfidence = -1;

		public static readonly int IgnoreMaxCount = -1;

		public static readonly int MaxAllowedConfidenceValue = 100;

		public static readonly string IdKey = "id";

		public static readonly string MinCountKey = "minCount";

		public static readonly string MaxCountKey = "maxCount";

		public static readonly string MinConfidenceKey = "minConfidence";

		public static readonly string MaxConfidenceKey = "maxConfidence";

		public static readonly string OpaqueDataKey = "opaqueData";
	}
}
