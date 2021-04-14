using System;
using System.Collections.Generic;
using Microsoft.Office.CompliancePolicy.Classification;
using Microsoft.Office.CompliancePolicy.ComplianceData;

namespace Microsoft.Office.CompliancePolicy.PolicyEvaluation
{
	internal sealed class DataClassificationConfig
	{
		internal DataClassificationConfig(List<KeyValuePair<string, string>> config, IClassificationRuleStore classificationConfig = null)
		{
			this.Id = Guid.Empty;
			this.MinCount = DataClassificationConfig.MinAllowedCount;
			this.MaxCount = DataClassificationConfig.IgnoreMaxCount;
			this.MinConfidence = DataClassificationConfig.UseRecommendedMinConfidence;
			this.MaxConfidence = DataClassificationConfig.MaxAllowedConfidence;
			for (int i = 0; i < config.Count; i++)
			{
				KeyValuePair<string, string> keyValuePair = config[i];
				if (string.Compare(keyValuePair.Key, DataClassificationConfig.IdKey, true) == 0)
				{
					if (classificationConfig != null)
					{
						try
						{
							RuleDefinitionDetails ruleDetails = classificationConfig.GetRuleDetails(keyValuePair.Value, null);
							this.Id = ruleDetails.RuleId;
							goto IL_174;
						}
						catch (ClassificationRuleStorePermanentException innerException)
						{
							throw new CompliancePolicyValidationException(string.Format("invalid {0} for data classification!", DataClassificationConfig.IdKey), innerException);
						}
					}
					Guid id;
					if (Guid.TryParse(keyValuePair.Value, out id))
					{
						this.Id = id;
					}
				}
				else if (string.Compare(keyValuePair.Key, DataClassificationConfig.MinCountKey, true) == 0)
				{
					this.MinCount = Convert.ToInt32(keyValuePair.Value);
				}
				else if (string.Compare(keyValuePair.Key, DataClassificationConfig.MaxCountKey, true) == 0)
				{
					this.MaxCount = Convert.ToInt32(keyValuePair.Value);
				}
				else if (string.Compare(keyValuePair.Key, DataClassificationConfig.MinConfidenceKey, true) == 0)
				{
					this.MinConfidence = Convert.ToInt32(keyValuePair.Value);
				}
				else
				{
					if (string.Compare(keyValuePair.Key, DataClassificationConfig.MaxConfidenceKey, true) != 0)
					{
						throw new CompliancePolicyValidationException(keyValuePair.Key + " is not supported by data classification!");
					}
					this.MaxConfidence = Convert.ToInt32(keyValuePair.Value);
				}
				IL_174:;
			}
			if (this.Id == Guid.Empty)
			{
				throw new CompliancePolicyValidationException(string.Format("invalid {0} for data classification!", DataClassificationConfig.IdKey));
			}
			if (this.MinCount < DataClassificationConfig.MinAllowedCount)
			{
				throw new CompliancePolicyValidationException(string.Format("invalid {0} for data classification!", DataClassificationConfig.MinCountKey));
			}
			if (this.MaxCount != DataClassificationConfig.IgnoreMaxCount && this.MaxCount < this.MinCount)
			{
				throw new CompliancePolicyValidationException(string.Format("invalid {0} for data classification!", DataClassificationConfig.MaxCountKey));
			}
			if (this.MinConfidence != DataClassificationConfig.UseRecommendedMinConfidence && this.MinConfidence < DataClassificationConfig.MinAllowedConfidence)
			{
				throw new CompliancePolicyValidationException(string.Format("invalid {0} for data classification!", DataClassificationConfig.MinConfidenceKey));
			}
			if (this.MaxConfidence < DataClassificationConfig.MinAllowedConfidence || this.MaxConfidence < this.MinConfidence || this.MaxConfidence > DataClassificationConfig.MaxAllowedConfidence)
			{
				throw new CompliancePolicyValidationException(string.Format("invalid {0} for data classification!", DataClassificationConfig.MaxConfidenceKey));
			}
			config.Clear();
			config.Add(new KeyValuePair<string, string>(DataClassificationConfig.IdKey, this.Id.ToString()));
			config.Add(new KeyValuePair<string, string>(DataClassificationConfig.MinCountKey, this.MinCount.ToString()));
			config.Add(new KeyValuePair<string, string>(DataClassificationConfig.MaxCountKey, this.MaxCount.ToString()));
			config.Add(new KeyValuePair<string, string>(DataClassificationConfig.MinConfidenceKey, this.MinConfidence.ToString()));
			config.Add(new KeyValuePair<string, string>(DataClassificationConfig.MaxConfidenceKey, this.MaxConfidence.ToString()));
		}

		internal Guid Id { get; private set; }

		internal int MinCount { get; private set; }

		internal int MaxCount { get; private set; }

		internal int MinConfidence { get; private set; }

		internal int MaxConfidence { get; private set; }

		internal bool Matches(ClassificationResult classificationResult, IClassificationRuleStore classificationStore)
		{
			int num = this.MinConfidence;
			if (num == DataClassificationConfig.UseRecommendedMinConfidence)
			{
				RuleDefinitionDetails ruleDetails = classificationStore.GetRuleDetails(this.Id.ToString(), null);
				num = ruleDetails.RecommendedConfidence;
			}
			return this.Id == classificationResult.ClassificationId && (this.MaxCount == DataClassificationConfig.IgnoreMaxCount || this.MaxCount >= classificationResult.Count) && this.MinCount <= classificationResult.Count && this.MaxConfidence >= classificationResult.Confidence && num <= classificationResult.Confidence;
		}

		internal static readonly int MinAllowedCount = 1;

		internal static readonly int IgnoreMaxCount = -1;

		internal static readonly int UseRecommendedMinConfidence = -1;

		internal static readonly int MinAllowedConfidence = 1;

		internal static readonly int MaxAllowedConfidence = 100;

		internal static readonly string IdKey = "id";

		internal static readonly string MinCountKey = "minCount";

		internal static readonly string MaxCountKey = "maxCount";

		internal static readonly string MinConfidenceKey = "minConfidence";

		internal static readonly string MaxConfidenceKey = "maxConfidence";
	}
}
