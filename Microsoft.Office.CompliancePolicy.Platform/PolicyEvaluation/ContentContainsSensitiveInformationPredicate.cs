using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Office.CompliancePolicy.Classification;
using Microsoft.Office.CompliancePolicy.ComplianceData;

namespace Microsoft.Office.CompliancePolicy.PolicyEvaluation
{
	public class ContentContainsSensitiveInformationPredicate : PredicateCondition
	{
		public ContentContainsSensitiveInformationPredicate(List<List<KeyValuePair<string, string>>> entries, IClassificationRuleStore classificationStore)
		{
			ArgumentValidator.ThrowIfNull("classificationStore", classificationStore);
			if (entries != null)
			{
				if (!entries.Any((List<KeyValuePair<string, string>> entry) => entry == null))
				{
					this.classificationStore = classificationStore;
					base.Property = new Property("Item.ClassificationDiscovered", typeof(IDictionary<Guid, ClassificationResult>));
					base.Value = this.BuildValue(entries);
					return;
				}
			}
			throw new ArgumentNullException("entries");
		}

		internal ContentContainsSensitiveInformationPredicate(List<List<KeyValuePair<string, string>>> entries) : base(new Property("Item.ClassificationDiscovered", typeof(IDictionary<Guid, ClassificationResult>)), entries)
		{
		}

		public override string Name
		{
			get
			{
				return "containsDataClassification";
			}
		}

		public override Version MinimumVersion
		{
			get
			{
				return ContentContainsSensitiveInformationPredicate.minVersion;
			}
		}

		internal IDictionary<Guid, DataClassificationConfig> ClassificationConfig { get; private set; }

		public override bool Evaluate(PolicyEvaluationContext context)
		{
			if (!(base.Property.Type == typeof(IDictionary<Guid, ClassificationResult>)))
			{
				throw new CompliancePolicyValidationException(string.Format("Rule '{0}' is in inconsitent state due to unknown property '{1}'", context.CurrentRule.Name, base.Property.Name));
			}
			IDictionary<Guid, ClassificationResult> dictionary = base.Property.GetValue(context) as IDictionary<Guid, ClassificationResult>;
			IDictionary<Guid, DataClassificationConfig> classificationConfig = this.ClassificationConfig;
			if (classificationConfig == null || !classificationConfig.Any<KeyValuePair<Guid, DataClassificationConfig>>())
			{
				throw new CompliancePolicyValidationException(string.Format("Rule '{0}' contains an invalid property '{1}'", context.CurrentRule.Name, base.Property.Name));
			}
			ArgumentValidator.ThrowIfNull("context.ClassificationStore", context.ClassificationStore);
			if (dictionary != null && dictionary.Any<KeyValuePair<Guid, ClassificationResult>>())
			{
				foreach (Guid key in classificationConfig.Keys)
				{
					if (dictionary.ContainsKey(key) && dictionary[key] != null && classificationConfig[key].Matches(dictionary[key], context.ClassificationStore))
					{
						return true;
					}
				}
				return false;
			}
			return false;
		}

		protected override Value BuildValue(List<List<KeyValuePair<string, string>>> entries)
		{
			if (!entries.Any<List<KeyValuePair<string, string>>>())
			{
				throw new CompliancePolicyValidationException("entries can not be empty for ContentContainsDataClassificationPredicate!");
			}
			IDictionary<Guid, DataClassificationConfig> dictionary = new Dictionary<Guid, DataClassificationConfig>();
			foreach (List<KeyValuePair<string, string>> config in entries)
			{
				DataClassificationConfig dataClassificationConfig = new DataClassificationConfig(config, this.classificationStore);
				if (dictionary.ContainsKey(dataClassificationConfig.Id))
				{
					throw new CompliancePolicyValidationException(string.Format("Duplicate data classification found: {0}", dataClassificationConfig.Id.ToString()));
				}
				dictionary.Add(dataClassificationConfig.Id, dataClassificationConfig);
			}
			this.ClassificationConfig = dictionary;
			return Value.CreateValue(entries);
		}

		private static readonly Version minVersion = new Version("1.00.0002.000");

		private readonly IClassificationRuleStore classificationStore;
	}
}
