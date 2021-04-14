using System;
using System.Collections.Generic;
using Microsoft.Exchange.Core.RuleTasks;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Tasks
{
	[Serializable]
	public class PrependSubjectAction : TransportRuleAction, IEquatable<PrependSubjectAction>
	{
		public override int GetHashCode()
		{
			return this.Prefix.GetHashCode();
		}

		public override bool Equals(object right)
		{
			return !object.ReferenceEquals(right, null) && (object.ReferenceEquals(this, right) || (!(base.GetType() != right.GetType()) && this.Equals(right as PrependSubjectAction)));
		}

		public bool Equals(PrependSubjectAction other)
		{
			return this.Prefix.Equals(other.Prefix);
		}

		[ActionParameterName("PrependSubject")]
		[LocDescription(RulesTasksStrings.IDs.PrefixDescription)]
		[LocDisplayName(RulesTasksStrings.IDs.PrefixDisplayName)]
		public SubjectPrefix Prefix
		{
			get
			{
				return this.prefix;
			}
			set
			{
				this.prefix = value;
			}
		}

		internal override string Description
		{
			get
			{
				return RulesTasksStrings.RuleDescriptionPrependSubject(this.Prefix.ToString());
			}
		}

		internal static TransportRuleAction CreateFromInternalAction(Action action)
		{
			if (action.Name != "PrependSubject")
			{
				return null;
			}
			PrependSubjectAction prependSubjectAction = new PrependSubjectAction();
			try
			{
				prependSubjectAction.Prefix = new SubjectPrefix(TransportRuleAction.GetStringValue(action.Arguments[0]));
			}
			catch (ArgumentOutOfRangeException)
			{
				return null;
			}
			return prependSubjectAction;
		}

		internal override void Reset()
		{
			this.prefix = SubjectPrefix.Empty;
			base.Reset();
		}

		protected override void ValidateRead(List<ValidationError> errors)
		{
			if (this.prefix == SubjectPrefix.Empty)
			{
				errors.Add(new RulePhrase.RulePhraseValidationError(RulesTasksStrings.ArgumentNotSet, base.Name));
				return;
			}
			int index;
			if (!Utils.CheckIsUnicodeStringWellFormed(this.prefix.Value, out index))
			{
				errors.Add(new RulePhrase.RulePhraseValidationError(RulesTasksStrings.CommentsHaveInvalidChars((int)this.prefix.Value[index]), base.Name));
				return;
			}
			base.ValidateRead(errors);
		}

		internal override Action ToInternalAction()
		{
			return TransportRuleParser.Instance.CreateAction("PrependSubject", new ShortList<Argument>
			{
				new Value(this.Prefix.ToString())
			}, Utils.GetActionName(this));
		}

		internal override string GetActionParameters()
		{
			return Utils.QuoteCmdletParameter(this.Prefix.ToString());
		}

		internal override void SuppressPiiData()
		{
			string text = SuppressingPiiProperty.TryRedactValue<string>(RuleSchema.PrependSubject, this.Prefix.ToString());
			if (text != null && text.Length > 32)
			{
				text = text.Substring(0, 32);
			}
			this.Prefix = new SubjectPrefix(text);
		}

		private SubjectPrefix prefix;
	}
}
