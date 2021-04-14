using System;
using System.Collections.Generic;
using Microsoft.Exchange.Core.RuleTasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Tasks
{
	[Serializable]
	public class ApplyClassificationAction : TransportRuleAction, IEquatable<ApplyClassificationAction>
	{
		public ApplyClassificationAction(IConfigDataProvider session)
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
			return !object.ReferenceEquals(right, null) && (object.ReferenceEquals(this, right) || (!(base.GetType() != right.GetType()) && this.Equals(right as ApplyClassificationAction)));
		}

		public bool Equals(ApplyClassificationAction other)
		{
			if (this.Classification == null)
			{
				return null == other.Classification;
			}
			return this.Classification.Equals(other.Classification);
		}

		[LocDescription(RulesTasksStrings.IDs.ClassificationDescription)]
		[LocDisplayName(RulesTasksStrings.IDs.ClassificationDisplayName)]
		[ActionParameterName("ApplyClassification")]
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
				return RulesTasksStrings.RuleDescriptionApplyClassification(Utils.GetClassificationDisplayName(this.Classification, this.session));
			}
		}

		internal static TransportRuleAction CreateFromInternalActionWithSession(Action action, IConfigDataProvider session)
		{
			if (action.Name != "SetHeaderUniqueValue" || TransportRuleAction.GetStringValue(action.Arguments[0]) != "X-MS-Exchange-Organization-Classification")
			{
				return null;
			}
			return new ApplyClassificationAction(session)
			{
				Classification = Utils.GetClassificationADObjectId(TransportRuleAction.GetStringValue(action.Arguments[1]), session)
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

		internal override Action ToInternalAction()
		{
			string classificationId = Utils.GetClassificationId(this.classification, this.session);
			if (string.IsNullOrEmpty(classificationId))
			{
				throw new ArgumentException(RulesTasksStrings.InvalidClassification, "Classification");
			}
			ShortList<Argument> arguments = new ShortList<Argument>
			{
				new Value("X-MS-Exchange-Organization-Classification"),
				new Value(classificationId)
			};
			return TransportRuleParser.Instance.CreateAction("SetHeaderUniqueValue", arguments, Utils.GetActionName(this));
		}

		internal override string GetActionParameters()
		{
			return this.Classification.ToString();
		}

		private ADObjectId classification;

		[NonSerialized]
		private readonly IConfigDataProvider session;
	}
}
