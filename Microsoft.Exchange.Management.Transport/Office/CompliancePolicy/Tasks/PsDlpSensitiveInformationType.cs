using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Office.CompliancePolicy.Classification;

namespace Microsoft.Office.CompliancePolicy.Tasks
{
	[Serializable]
	public sealed class PsDlpSensitiveInformationType
	{
		public PsDlpSensitiveInformationType(RuleDefinitionDetails ruleDefinitionDetails)
		{
			ArgumentValidator.ThrowIfNull("ruleDefinitionDetails", ruleDefinitionDetails);
			ArgumentValidator.ThrowIfCollectionNullOrEmpty<KeyValuePair<string, CLASSIFICATION_DEFINITION_DETAILS>>("LocalizableDetails", ruleDefinitionDetails.LocalizableDetails);
			this.Id = ruleDefinitionDetails.RuleId;
			this.Name = ruleDefinitionDetails.LocalizableDetails.Values.First<CLASSIFICATION_DEFINITION_DETAILS>().DefinitionName;
			this.Description = ruleDefinitionDetails.LocalizableDetails.Values.First<CLASSIFICATION_DEFINITION_DETAILS>().Description;
			this.Publisher = ruleDefinitionDetails.LocalizableDetails.Values.First<CLASSIFICATION_DEFINITION_DETAILS>().PublisherName;
			this.RecommendedConfidence = ruleDefinitionDetails.RecommendedConfidence;
		}

		public Guid Id { get; private set; }

		public string Name { get; private set; }

		public string Description { get; private set; }

		public int RecommendedConfidence { get; private set; }

		public string Publisher { get; private set; }

		internal const string DefaultLocale = "en-us";
	}
}
