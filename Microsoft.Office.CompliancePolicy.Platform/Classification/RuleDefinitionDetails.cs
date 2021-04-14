using System;
using System.Collections.Generic;

namespace Microsoft.Office.CompliancePolicy.Classification
{
	public sealed class RuleDefinitionDetails
	{
		public int RecommendedConfidence
		{
			get
			{
				return this.recommendedConfidence;
			}
			set
			{
				ArgumentValidator.ThrowIfZeroOrNegative("RecommendedConfidence", value);
				this.recommendedConfidence = value;
			}
		}

		public Guid RuleId { get; set; }

		public IDictionary<string, CLASSIFICATION_DEFINITION_DETAILS> LocalizableDetails { get; set; }

		internal RuleDefinitionDetails Clone()
		{
			return (RuleDefinitionDetails)base.MemberwiseClone();
		}

		private int recommendedConfidence;
	}
}
