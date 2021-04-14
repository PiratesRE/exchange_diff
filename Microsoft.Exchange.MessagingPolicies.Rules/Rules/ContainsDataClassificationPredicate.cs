using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal class ContainsDataClassificationPredicate : PredicateCondition
	{
		public ContainsDataClassificationPredicate(Property property, ShortList<ShortList<KeyValuePair<string, string>>> valueEntries, RulesCreationContext creationContext) : base(property, valueEntries, creationContext)
		{
			if (!typeof(IEnumerable<DiscoveredDataClassification>).IsAssignableFrom(base.Property.Type))
			{
				throw new RulesValidationException(TransportRulesStrings.DataClassificationPropertyRequired(this.Name));
			}
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
				return ContainsDataClassificationPredicate.DataClassificationBaseVersion;
			}
		}

		protected override Value BuildValue(ShortList<ShortList<KeyValuePair<string, string>>> entries, RulesCreationContext creationContext)
		{
			if (entries.Count == 0)
			{
				throw new RulesValidationException(RulesStrings.StringPropertyOrValueRequired(this.Name));
			}
			foreach (ShortList<KeyValuePair<string, string>> keyValueParameters in entries)
			{
				this.targetClassifications.Add(new TargetDataClassification(keyValueParameters));
			}
			return Value.CreateValue(entries);
		}

		public override bool Evaluate(RulesEvaluationContext baseContext)
		{
			BaseTransportRulesEvaluationContext baseTransportRulesEvaluationContext = (BaseTransportRulesEvaluationContext)baseContext;
			if (baseTransportRulesEvaluationContext == null)
			{
				throw new ArgumentException("context is either null or not of type: BaseTransportRulesEvaluationContext");
			}
			baseTransportRulesEvaluationContext.PredicateName = this.Name;
			bool flag = false;
			IEnumerable<DiscoveredDataClassification> enumerable = (IEnumerable<DiscoveredDataClassification>)base.Property.GetValue(baseTransportRulesEvaluationContext);
			List<DiscoveredDataClassification> list = new List<DiscoveredDataClassification>();
			HashSet<string> hashSet = new HashSet<string>();
			using (IEnumerator<DiscoveredDataClassification> enumerator = enumerable.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					DiscoveredDataClassification discoveredClassification = enumerator.Current;
					if (this.targetClassifications.Any((TargetDataClassification x) => x.Matches(discoveredClassification)))
					{
						flag = true;
						list.Add(discoveredClassification);
						foreach (DataClassificationSourceInfo dataClassificationSourceInfo in discoveredClassification.MatchingSourceInfos)
						{
							if (!string.IsNullOrEmpty(dataClassificationSourceInfo.TopLevelSourceName))
							{
								hashSet.Add(dataClassificationSourceInfo.TopLevelSourceName);
							}
						}
					}
				}
			}
			base.UpdateEvaluationHistory(baseContext, flag, (from c in list
			select c.ClassificationName).ToList<string>(), 0);
			base.UpdateEvaluationHistory(baseContext, flag, (from c in list
			select c.Id).ToList<string>(), 2);
			base.UpdateEvaluationHistory(baseContext, flag, hashSet.ToList<string>(), 1);
			return flag;
		}

		public override void GetSupplementalData(SupplementalData data)
		{
			foreach (TargetDataClassification targetDataClassification in this.targetClassifications)
			{
				data.Add("DataClassification", new KeyValuePair<string, string>(targetDataClassification.Id, targetDataClassification.OpaqueData));
			}
		}

		internal static readonly Version DataClassificationBaseVersion = new Version("15.00.0003.01");

		private readonly List<TargetDataClassification> targetClassifications = new List<TargetDataClassification>();

		internal enum MatchResultTypes
		{
			ClassificationNames,
			TopLevelSourceNames,
			ClassificationIds
		}
	}
}
