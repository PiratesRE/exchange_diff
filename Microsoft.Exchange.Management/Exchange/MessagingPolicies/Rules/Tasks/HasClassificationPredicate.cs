using System;
using System.Collections.Generic;
using Microsoft.Exchange.Core.RuleTasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Tasks
{
	[Serializable]
	public class HasClassificationPredicate : TransportRulePredicate, IEquatable<HasClassificationPredicate>
	{
		public HasClassificationPredicate(IConfigDataProvider session)
		{
			this.session = session;
		}

		public override int GetHashCode()
		{
			if (this.Classification != null)
			{
				return this.Classification.GetHashCode();
			}
			return 0;
		}

		public override bool Equals(object right)
		{
			return !object.ReferenceEquals(right, null) && (object.ReferenceEquals(this, right) || (!(base.GetType() != right.GetType()) && this.Equals(right as HasClassificationPredicate)));
		}

		public bool Equals(HasClassificationPredicate other)
		{
			if (this.Classification == null)
			{
				return null == other.Classification;
			}
			return this.Classification.Equals(other.Classification);
		}

		[LocDescription(RulesTasksStrings.IDs.ClassificationDescription)]
		[LocDisplayName(RulesTasksStrings.IDs.ClassificationDisplayName)]
		[ConditionParameterName("HasClassification")]
		[ExceptionParameterName("ExceptIfHasClassification")]
		public ADObjectId Classification
		{
			get
			{
				return this.classification;
			}
			set
			{
				this.classification = value;
			}
		}

		internal override string Description
		{
			get
			{
				return RulesTasksStrings.RuleDescriptionHasClassification(Utils.GetClassificationDisplayName(this.Classification, this.session));
			}
		}

		internal static TransportRulePredicate CreateFromInternalConditionWithSession(Condition condition, IConfigDataProvider session)
		{
			if (condition.ConditionType != ConditionType.Predicate)
			{
				return null;
			}
			PredicateCondition predicateCondition = (PredicateCondition)condition;
			if ((!predicateCondition.Name.Equals("is") && !predicateCondition.Name.Equals("contains")) || !(predicateCondition.Property is HeaderProperty) || !predicateCondition.Property.Name.Equals("X-MS-Exchange-Organization-Classification") || predicateCondition.Value.RawValues.Count != 1)
			{
				return null;
			}
			return new HasClassificationPredicate(session)
			{
				Classification = Utils.GetClassificationADObjectId(predicateCondition.Value.RawValues[0], session)
			};
		}

		internal override void Reset()
		{
			this.classification = null;
			base.Reset();
		}

		protected override void ValidateRead(List<ValidationError> errors)
		{
			if (this.classification == null)
			{
				errors.Add(new RulePhrase.RulePhraseValidationError(RulesTasksStrings.ArgumentNotSet, base.Name));
				return;
			}
			base.ValidateRead(errors);
		}

		internal override Condition ToInternalCondition()
		{
			string classificationId = Utils.GetClassificationId(this.classification, this.session);
			if (string.IsNullOrEmpty(classificationId))
			{
				throw new ArgumentException(RulesTasksStrings.InvalidClassification, "Classification");
			}
			Property property = TransportRuleParser.Instance.CreateProperty("Message.Headers:X-MS-Exchange-Organization-Classification");
			ShortList<string> valueEntries = new ShortList<string>
			{
				classificationId
			};
			return TransportRuleParser.Instance.CreatePredicate("contains", property, valueEntries);
		}

		internal override string GetPredicateParameters()
		{
			return this.Classification.ToString();
		}

		[NonSerialized]
		private readonly IConfigDataProvider session;

		private ADObjectId classification;
	}
}
