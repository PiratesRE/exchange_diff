using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Core.RuleTasks;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Tasks
{
	[Serializable]
	public class HeaderContainsPredicate : SinglePropertyContainsPredicate, IEquatable<HeaderContainsPredicate>
	{
		public HeaderContainsPredicate() : base("Message.Headers")
		{
		}

		public override int GetHashCode()
		{
			return Utils.GetHashCodeForArray<Word>(this.Words) + this.messageHeader.GetHashCode();
		}

		public override bool Equals(object right)
		{
			return !object.ReferenceEquals(right, null) && (object.ReferenceEquals(this, right) || (!(base.GetType() != right.GetType()) && this.Equals(right as HeaderContainsPredicate)));
		}

		public bool Equals(HeaderContainsPredicate other)
		{
			if (this.Words == null)
			{
				return other.Words == null && this.MessageHeader.Equals(other.MessageHeader);
			}
			return this.MessageHeader.Equals(other.MessageHeader) && this.Words.SequenceEqual(other.Words);
		}

		[ExceptionParameterName("ExceptIfHeaderContainsMessageHeader")]
		[LocDisplayName(RulesTasksStrings.IDs.MessageHeaderDisplayName)]
		[LocDescription(RulesTasksStrings.IDs.MessageHeaderDescription)]
		[ConditionParameterName("HeaderContainsMessageHeader")]
		public HeaderName MessageHeader
		{
			get
			{
				return this.messageHeader;
			}
			set
			{
				this.messageHeader = value;
			}
		}

		[LocDescription(RulesTasksStrings.IDs.WordsDescription)]
		[LocDisplayName(RulesTasksStrings.IDs.WordsDisplayName)]
		[ConditionParameterName("HeaderContainsWords")]
		[ExceptionParameterName("ExceptIfHeaderContainsWords")]
		public override Word[] Words
		{
			get
			{
				return this.words;
			}
			set
			{
				this.words = value;
			}
		}

		protected override ContainsWordsPredicate.LocalizedStringDescriptionDelegate LocalizedStringDescription
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		internal override string Description
		{
			get
			{
				return RulesTasksStrings.RuleDescriptionHeaderContains(this.MessageHeader.ToString(), RuleDescription.BuildDescriptionStringFromStringArray(Utils.BuildWordStringList(this.Words), RulesTasksStrings.RuleDescriptionOrDelimiter, base.MaxDescriptionListLength));
			}
		}

		internal static TransportRulePredicate CreateFromInternalCondition(Condition condition)
		{
			if (condition.ConditionType != ConditionType.Predicate)
			{
				return null;
			}
			PredicateCondition predicateCondition = (PredicateCondition)condition;
			if (!(predicateCondition.Property is HeaderProperty))
			{
				return null;
			}
			HeaderContainsPredicate headerContainsPredicate = (HeaderContainsPredicate)SinglePropertyContainsPredicate.CreateFromInternalCondition<HeaderContainsPredicate>(condition, predicateCondition.Property.Name);
			if (headerContainsPredicate == null)
			{
				return null;
			}
			try
			{
				headerContainsPredicate.MessageHeader = new HeaderName(predicateCondition.Property.Name);
			}
			catch (ArgumentOutOfRangeException)
			{
				return null;
			}
			return headerContainsPredicate;
		}

		internal override void Reset()
		{
			this.messageHeader = HeaderName.Empty;
			base.Reset();
		}

		protected override void ValidateRead(List<ValidationError> errors)
		{
			if (this.MessageHeader == HeaderName.Empty)
			{
				errors.Add(new RulePhrase.RulePhraseValidationError(RulesTasksStrings.ArgumentNotSet, base.Name));
				return;
			}
			base.ValidateRead(errors);
		}

		internal override Property CreateProperty()
		{
			return TransportRuleParser.Instance.CreateProperty("Message.Headers:" + this.MessageHeader.ToString());
		}

		internal override string ToCmdletParameter(bool isException = false)
		{
			return string.Format("-{0} {1} -{2} {3}", new object[]
			{
				isException ? "ExceptIfHeaderContainsMessageHeader" : "HeaderContainsMessageHeader",
				Utils.QuoteCmdletParameter(this.MessageHeader.ToString()),
				isException ? "ExceptIfHeaderContainsWords" : "HeaderContainsWords",
				this.GetPredicateParameters()
			});
		}

		internal override void SuppressPiiData()
		{
			this.MessageHeader = SuppressingPiiProperty.TryRedactValue<HeaderName>(RuleSchema.HeaderContainsMessageHeader, this.MessageHeader);
			this.Words = SuppressingPiiProperty.TryRedactValue<Word[]>(RuleSchema.HeaderContainsWords, this.Words);
		}

		private HeaderName messageHeader;
	}
}
