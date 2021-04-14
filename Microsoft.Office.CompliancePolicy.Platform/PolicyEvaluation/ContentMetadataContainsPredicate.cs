using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Office.CompliancePolicy.PolicyEvaluation
{
	public class ContentMetadataContainsPredicate : PredicateCondition
	{
		public ContentMetadataContainsPredicate(List<string> entries) : base(new Property("Item.Metadata", typeof(IDictionary<string, List<string>>)), entries)
		{
		}

		public override string Name
		{
			get
			{
				return "contentMetadataContains";
			}
		}

		public override Version MinimumVersion
		{
			get
			{
				return ContentMetadataContainsPredicate.minVersion;
			}
		}

		internal IDictionary<string, List<string>> WordsConfig { get; private set; }

		public override bool Evaluate(PolicyEvaluationContext context)
		{
			if (!(base.Property.Type == typeof(IDictionary<string, List<string>>)))
			{
				throw new CompliancePolicyValidationException(string.Format("Rule '{0}' is in inconsitent state due to unknown property '{1}'", context.CurrentRule.Name, base.Property.Name));
			}
			IDictionary<string, List<string>> dictionary = base.Property.GetValue(context) as IDictionary<string, List<string>>;
			IDictionary<string, List<string>> conditionValue = this.WordsConfig;
			if (conditionValue == null || !conditionValue.Any<KeyValuePair<string, List<string>>>())
			{
				throw new CompliancePolicyValidationException(string.Format("Rule '{0}' contains an invalid property '{1}'", context.CurrentRule.Name, base.Property.Name));
			}
			if (dictionary != null && dictionary.Any<KeyValuePair<string, List<string>>>())
			{
				using (IEnumerator<string> enumerator = conditionValue.Keys.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						string key = enumerator.Current;
						if (dictionary.ContainsKey(key) && dictionary[key] != null)
						{
							if (dictionary[key].Any((string propVal) => conditionValue[key].Any((string condVal) => propVal.Equals(condVal, StringComparison.InvariantCultureIgnoreCase))))
							{
								return true;
							}
						}
					}
				}
				return false;
			}
			return false;
		}

		protected override Value BuildValue(List<string> entries)
		{
			if (!entries.Any<string>())
			{
				throw new CompliancePolicyValidationException("entries can not be empty for ContentMetadataContainsPredicate!");
			}
			IDictionary<string, List<string>> dictionary = new Dictionary<string, List<string>>();
			foreach (string text in entries)
			{
				int num = text.IndexOf(ContentMetadataContainsPredicate.PropertyAndValueDelimiter);
				if (num <= 0 || num == text.Length - 1)
				{
					throw new CompliancePolicyValidationException("Argument for ContentMetadataContains is in incorrect format");
				}
				string text2 = text.Substring(0, num).Trim();
				if (string.IsNullOrWhiteSpace(text2))
				{
					throw new CompliancePolicyValidationException("Argument for ContentMetadataContains is in incorrect format");
				}
				string[] source = text.Substring(num + ContentMetadataContainsPredicate.PropertyAndValueDelimiter.Length).Split(new string[]
				{
					ContentMetadataContainsPredicate.ValuesDelimiter
				}, StringSplitOptions.RemoveEmptyEntries);
				if (source.Any(new Func<string, bool>(string.IsNullOrWhiteSpace)))
				{
					throw new CompliancePolicyValidationException("Argument for ContentMetadataContains is in incorrect format");
				}
				List<string> list = (from p in source
				select p.Trim()).ToList<string>();
				if (dictionary.ContainsKey(text2))
				{
					dictionary[text2].AddRange(list);
				}
				else
				{
					dictionary.Add(text2, list);
				}
			}
			this.WordsConfig = dictionary;
			return Value.CreateValue(typeof(string[]), entries);
		}

		public static readonly string PropertyAndValueDelimiter = ":";

		public static readonly string ValuesDelimiter = ",";

		private static readonly Version minVersion = new Version("1.00.0002.000");
	}
}
