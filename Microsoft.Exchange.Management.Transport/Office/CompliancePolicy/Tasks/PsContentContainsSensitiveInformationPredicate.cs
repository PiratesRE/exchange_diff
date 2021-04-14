using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Transport;
using Microsoft.Office.CompliancePolicy.Classification;
using Microsoft.Office.CompliancePolicy.PolicyEvaluation;

namespace Microsoft.Office.CompliancePolicy.Tasks
{
	public sealed class PsContentContainsSensitiveInformationPredicate : PsComplianceRulePredicateBase
	{
		public PsContentContainsSensitiveInformationPredicate()
		{
		}

		public PsContentContainsSensitiveInformationPredicate(IEnumerable<Hashtable> dataClassifications)
		{
			IClassificationRuleStore instance = InMemoryClassificationRuleStore.GetInstance();
			try
			{
				this.EnginePredicate = new ContentContainsSensitiveInformationPredicate(dataClassifications.Select(new Func<Hashtable, List<KeyValuePair<string, string>>>(PsContentContainsSensitiveInformationPredicate.HashtableToLowerCasedDictionary)).ToList<List<KeyValuePair<string, string>>>(), instance);
			}
			catch (CompliancePolicyValidationException innerException)
			{
				throw new InvalidContentContainsSensitiveInformationException(Strings.InvalidSensitiveInformationParameterValue, innerException);
			}
		}

		internal ContentContainsSensitiveInformationPredicate EnginePredicate { get; set; }

		internal Hashtable[] DataClassifications
		{
			get
			{
				IClassificationRuleStore instance = InMemoryClassificationRuleStore.GetInstance();
				IList<Hashtable> list = new List<Hashtable>(this.EnginePredicate.ClassificationConfig.Values.Count);
				foreach (DataClassificationConfig dataClassificationConfig in this.EnginePredicate.ClassificationConfig.Values)
				{
					RuleDefinitionDetails ruleDetails = instance.GetRuleDetails(dataClassificationConfig.Id.ToString(), CultureInfo.CurrentCulture.ToString());
					if (ruleDetails.LocalizableDetails == null || !ruleDetails.LocalizableDetails.Any<KeyValuePair<string, CLASSIFICATION_DEFINITION_DETAILS>>())
					{
						ruleDetails = instance.GetRuleDetails(dataClassificationConfig.Id.ToString(), "en-us");
					}
					list.Add(new Hashtable
					{
						{
							"id",
							dataClassificationConfig.Id
						},
						{
							"name",
							ruleDetails.LocalizableDetails.Values.First<CLASSIFICATION_DEFINITION_DETAILS>().DefinitionName
						},
						{
							"mincount",
							dataClassificationConfig.MinCount.ToString("G")
						},
						{
							"maxcount",
							dataClassificationConfig.MaxCount.ToString("G")
						},
						{
							"minconfidence",
							dataClassificationConfig.MinConfidence.ToString("G")
						},
						{
							"maxconfidence",
							dataClassificationConfig.MaxConfidence.ToString("G")
						}
					});
				}
				return list.ToArray<Hashtable>();
			}
		}

		internal override PredicateCondition ToEnginePredicate()
		{
			return this.EnginePredicate;
		}

		internal static PsContentContainsSensitiveInformationPredicate FromEnginePredicate(ContentContainsSensitiveInformationPredicate condition)
		{
			return new PsContentContainsSensitiveInformationPredicate
			{
				EnginePredicate = condition
			};
		}

		internal static List<KeyValuePair<string, string>> HashtableToLowerCasedDictionary(Hashtable hashtable)
		{
			ArgumentValidator.ThrowIfNull("hashtable", hashtable);
			return (from DictionaryEntry kvp in hashtable
			select new KeyValuePair<string, string>(PsContentContainsSensitiveInformationPredicate.CmdletParameterNameToEngineKeyMapping[((string)kvp.Key).ToLower()], (string)kvp.Value)).ToList<KeyValuePair<string, string>>();
		}

		internal const string IdParameter = "id";

		internal const string NameParameter = "name";

		internal const string MinCountParameter = "mincount";

		internal const string MaxCountParameter = "maxcount";

		internal const string MinConfidenceParameter = "minconfidence";

		internal const string MaxConfidenceParameter = "maxconfidence";

		internal const string DefaultLocale = "en-us";

		internal static readonly Dictionary<string, string> CmdletParameterNameToEngineKeyMapping = new Dictionary<string, string>
		{
			{
				"name",
				DataClassificationConfig.IdKey
			},
			{
				"mincount",
				DataClassificationConfig.MinCountKey
			},
			{
				"maxcount",
				DataClassificationConfig.MaxCountKey
			},
			{
				"minconfidence",
				DataClassificationConfig.MinConfidenceKey
			},
			{
				"maxconfidence",
				DataClassificationConfig.MaxConfidenceKey
			}
		};
	}
}
