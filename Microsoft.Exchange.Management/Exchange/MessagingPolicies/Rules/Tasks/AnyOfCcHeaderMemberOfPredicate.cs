using System;
using System.Linq;
using Microsoft.Exchange.Core.RuleTasks;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Tasks
{
	[Serializable]
	public class AnyOfCcHeaderMemberOfPredicate : SmtpAddressesPredicate, IEquatable<AnyOfCcHeaderMemberOfPredicate>
	{
		public AnyOfCcHeaderMemberOfPredicate() : base("isMemberOf", "Message.Cc")
		{
		}

		public override int GetHashCode()
		{
			return Utils.GetHashCodeForArray<SmtpAddress>(this.Addresses);
		}

		public override bool Equals(object right)
		{
			return !object.ReferenceEquals(right, null) && (object.ReferenceEquals(this, right) || (!(base.GetType() != right.GetType()) && this.Equals(right as AnyOfCcHeaderMemberOfPredicate)));
		}

		public bool Equals(AnyOfCcHeaderMemberOfPredicate other)
		{
			if (this.Addresses == null)
			{
				return null == other.Addresses;
			}
			return this.Addresses.SequenceEqual(other.Addresses);
		}

		[ConditionParameterName("AnyOfCcHeaderMemberOf")]
		[ExceptionParameterName("ExceptIfAnyOfCcHeaderMemberOf")]
		[LocDisplayName(RulesTasksStrings.IDs.ToDLAddressDisplayName)]
		[LocDescription(RulesTasksStrings.IDs.ToDLAddressDescription)]
		public override SmtpAddress[] Addresses
		{
			get
			{
				return this.addresses;
			}
			set
			{
				this.addresses = value;
			}
		}

		protected override SmtpAddressesPredicate.LocalizedStringDescriptionDelegate LocalizedStringDescription
		{
			get
			{
				return new SmtpAddressesPredicate.LocalizedStringDescriptionDelegate(RulesTasksStrings.RuleDescriptionAnyOfCcHeaderMemberOf);
			}
		}

		internal static TransportRulePredicate CreateFromInternalCondition(Condition condition)
		{
			return SmtpAddressesPredicate.CreateFromInternalCondition<AnyOfCcHeaderMemberOfPredicate>(condition, "isMemberOf", "Message.Cc");
		}

		private const string InternalPredicateName = "isMemberOf";

		private const string InternalPropertyName = "Message.Cc";
	}
}
