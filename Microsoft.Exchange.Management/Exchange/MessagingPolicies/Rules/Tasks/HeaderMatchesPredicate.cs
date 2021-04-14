using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Core.RuleTasks;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Tasks
{
	[Serializable]
	public class HeaderMatchesPredicate : SinglePropertyMatchesPredicate, IEquatable<HeaderMatchesPredicate>
	{
		public HeaderMatchesPredicate() : this(true)
		{
		}

		public HeaderMatchesPredicate(bool useLegacyRegex) : base("Message.Headers", useLegacyRegex)
		{
		}

		public override int GetHashCode()
		{
			return Utils.GetHashCodeForArray<Pattern>(this.Patterns) + this.MessageHeader.GetHashCode();
		}

		public override bool Equals(object right)
		{
			return !object.ReferenceEquals(right, null) && (object.ReferenceEquals(this, right) || (!(base.GetType() != right.GetType()) && this.Equals(right as HeaderMatchesPredicate)));
		}

		public bool Equals(HeaderMatchesPredicate other)
		{
			if (this.Patterns == null)
			{
				return other.Patterns == null && this.MessageHeader.Equals(other.MessageHeader);
			}
			return this.MessageHeader.Equals(other.MessageHeader) && this.Patterns.SequenceEqual(other.Patterns);
		}

		[ConditionParameterName("HeaderMatchesMessageHeader")]
		[LocDescription(RulesTasksStrings.IDs.MessageHeaderDescription)]
		[LocDisplayName(RulesTasksStrings.IDs.MessageHeaderDisplayName)]
		[ExceptionParameterName("ExceptIfHeaderMatchesMessageHeader")]
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

		[LocDisplayName(RulesTasksStrings.IDs.TextPatternsDisplayName)]
		[LocDescription(RulesTasksStrings.IDs.TextPatternsDescription)]
		[ConditionParameterName("HeaderMatchesPatterns")]
		[ExceptionParameterName("ExceptIfHeaderMatchesPatterns")]
		public override Pattern[] Patterns
		{
			get
			{
				return this.patterns;
			}
			set
			{
				this.patterns = value;
			}
		}

		internal override MatchesPatternsPredicate.LocalizedStringDescriptionDelegate LocalizedStringDescription
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
				return RulesTasksStrings.RuleDescriptionHeaderMatches(this.MessageHeader.ToString(), RuleDescription.BuildDescriptionStringFromStringArray(Utils.BuildPatternStringList(this.Patterns), RulesTasksStrings.RuleDescriptionOrDelimiter, base.MaxDescriptionListLength));
			}
		}

		internal static TransportRulePredicate CreateFromInternalCondition(Condition condition)
		{
			PredicateCondition predicateCondition = condition as PredicateCondition;
			if (predicateCondition == null || predicateCondition.ConditionType != ConditionType.Predicate || !(predicateCondition.Property is HeaderProperty))
			{
				return null;
			}
			HeaderMatchesPredicate headerMatchesPredicate = (HeaderMatchesPredicate)SinglePropertyMatchesPredicate.CreateFromInternalCondition<HeaderMatchesPredicate>(condition, predicateCondition.Property.Name);
			if (headerMatchesPredicate == null)
			{
				return null;
			}
			try
			{
				headerMatchesPredicate.MessageHeader = new HeaderName(predicateCondition.Property.Name);
			}
			catch (ArgumentOutOfRangeException)
			{
				return null;
			}
			return headerMatchesPredicate;
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
				isException ? "ExceptIfHeaderMatchesMessageHeader" : "HeaderMatchesMessageHeader",
				Utils.QuoteCmdletParameter(this.MessageHeader.ToString()),
				isException ? "ExceptIfHeaderMatchesPatterns" : "HeaderMatchesPatterns",
				this.GetPredicateParameters()
			});
		}

		internal override void SuppressPiiData()
		{
			this.MessageHeader = SuppressingPiiProperty.TryRedactValue<HeaderName>(RuleSchema.HeaderMatchesMessageHeader, this.MessageHeader);
			this.Patterns = Utils.RedactPatterns(this.Patterns);
		}

		private HeaderName messageHeader;
	}
}
