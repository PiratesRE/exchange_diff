using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Core.RuleTasks;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Tasks
{
	[Serializable]
	public class SenderIpRangesPredicate : TransportRulePredicate, IEquatable<SenderIpRangesPredicate>
	{
		public override int GetHashCode()
		{
			return Utils.GetHashCodeForArray<IPRange>(this.IpRanges);
		}

		public override bool Equals(object right)
		{
			return !object.ReferenceEquals(right, null) && (object.ReferenceEquals(this, right) || (!(base.GetType() != right.GetType()) && this.Equals(right as SenderIpRangesPredicate)));
		}

		public bool Equals(SenderIpRangesPredicate other)
		{
			if (this.IpRanges == null)
			{
				return null == other.IpRanges;
			}
			return this.IpRanges.SequenceEqual(other.IpRanges);
		}

		[LocDescription(RulesTasksStrings.IDs.SenderIpRangesDescription)]
		[ExceptionParameterName("ExceptIfSenderIpRanges")]
		[ConditionParameterName("SenderIpRanges")]
		[LocDisplayName(RulesTasksStrings.IDs.SenderIpRangesDisplayName)]
		public List<IPRange> IpRanges { get; set; }

		internal override string Description
		{
			get
			{
				List<string> stringValues = (from range in this.IpRanges
				select range.ToString()).ToList<string>();
				string lists = RuleDescription.BuildDescriptionStringFromStringArray(stringValues, RulesTasksStrings.RuleDescriptionOrDelimiter, base.MaxDescriptionListLength);
				return RulesTasksStrings.RuleDescriptionIpRanges(lists);
			}
		}

		public SenderIpRangesPredicate()
		{
		}

		internal SenderIpRangesPredicate(IEnumerable<IPRange> ipRanges)
		{
			this.IpRanges = ipRanges.ToList<IPRange>();
		}

		internal override Condition ToInternalCondition()
		{
			ShortList<string> valueEntries = new ShortList<string>(from x in this.IpRanges
			select x.ToString());
			return TransportRuleParser.Instance.CreatePredicate("ipMatch", TransportRuleParser.Instance.CreateProperty("Message.SenderIp"), valueEntries);
		}

		internal static TransportRulePredicate CreateFromInternalCondition(Condition condition)
		{
			IpMatchPredicate ipMatchPredicate = condition as IpMatchPredicate;
			if (ipMatchPredicate == null || !ipMatchPredicate.Name.Equals("ipMatch"))
			{
				return null;
			}
			object value = ipMatchPredicate.Value.GetValue(null);
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
			return new SenderIpRangesPredicate(source.Select(new Func<string, IPRange>(IPRange.Parse)));
		}

		internal override void Reset()
		{
			this.IpRanges = null;
			base.Reset();
		}

		protected override void ValidateRead(List<ValidationError> errors)
		{
			if (this.IpRanges == null || !this.IpRanges.Any<IPRange>())
			{
				errors.Add(new RulePhrase.RulePhraseValidationError(RulesTasksStrings.ArgumentNotSet, base.Name));
				return;
			}
			base.ValidateRead(errors);
		}

		internal override string GetPredicateParameters()
		{
			return string.Join(", ", (from range in this.IpRanges
			select range.ToString()).ToArray<string>());
		}

		private const string PropertyName = "Message.SenderIp";
	}
}
