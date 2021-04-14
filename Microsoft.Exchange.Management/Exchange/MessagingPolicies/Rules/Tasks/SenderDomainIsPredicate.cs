using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Core.RuleTasks;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Tasks
{
	[Serializable]
	public class SenderDomainIsPredicate : TransportRulePredicate, IEquatable<SenderDomainIsPredicate>
	{
		public override int GetHashCode()
		{
			return Utils.GetHashCodeForArray<Word>(this.Words);
		}

		public override bool Equals(object right)
		{
			return !object.ReferenceEquals(right, null) && (object.ReferenceEquals(this, right) || (!(base.GetType() != right.GetType()) && this.Equals(right as SenderDomainIsPredicate)));
		}

		public bool Equals(SenderDomainIsPredicate other)
		{
			if (this.Words == null)
			{
				return null == other.Words;
			}
			return this.Words.SequenceEqual(other.Words);
		}

		[LocDescription(RulesTasksStrings.IDs.WordsDescription)]
		[ConditionParameterName("SenderDomainIs")]
		[ExceptionParameterName("ExceptIfSenderDomainIs")]
		[LocDisplayName(RulesTasksStrings.IDs.WordsDisplayName)]
		public Word[] Words
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

		internal override string Description
		{
			get
			{
				List<string> stringValues = (from word in this.Words
				select word.ToString()).ToList<string>();
				string domains = RuleDescription.BuildDescriptionStringFromStringArray(stringValues, RulesTasksStrings.RuleDescriptionOrDelimiter, base.MaxDescriptionListLength);
				return RulesTasksStrings.RuleDescriptionSenderDomainIs(domains);
			}
		}

		internal override void Reset()
		{
			this.words = null;
			base.Reset();
		}

		protected override void ValidateRead(List<ValidationError> errors)
		{
			ContainsWordsPredicate.ValidateReadContainsWordsPredicate(this.words, base.Name, errors);
			base.ValidateRead(errors);
		}

		internal override string GetPredicateParameters()
		{
			return string.Join(", ", (from w in this.Words
			select Utils.QuoteCmdletParameter(w.ToString())).ToArray<string>());
		}

		internal override Condition ToInternalCondition()
		{
			ShortList<string> valueEntries = new ShortList<string>(from x in this.Words
			select x.ToString());
			return TransportRuleParser.Instance.CreatePredicate("domainIs", TransportRuleParser.Instance.CreateProperty("Message.SenderDomain"), valueEntries);
		}

		internal static TransportRulePredicate CreateFromInternalCondition(Condition condition)
		{
			DomainIsPredicate domainIsPredicate = condition as DomainIsPredicate;
			if (domainIsPredicate == null || !domainIsPredicate.Property.Name.Equals("Message.SenderDomain"))
			{
				return null;
			}
			object value = domainIsPredicate.Value.GetValue(null);
			IEnumerable<string> source;
			if (value is string)
			{
				source = new List<string>
				{
					value as string
				};
			}
			else
			{
				source = (IEnumerable<string>)value;
			}
			SenderDomainIsPredicate senderDomainIsPredicate = new SenderDomainIsPredicate();
			senderDomainIsPredicate.Words = (from s in source
			select new Word(s)).ToArray<Word>();
			return senderDomainIsPredicate;
		}

		internal override void SuppressPiiData()
		{
			this.Words = SuppressingPiiProperty.TryRedactValue<Word[]>(RuleSchema.HeaderContainsWords, this.Words);
		}

		internal const string InternalPropertyName = "Message.SenderDomain";

		protected Word[] words;
	}
}
