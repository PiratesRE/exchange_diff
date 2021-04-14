using System;
using System.Linq;
using Microsoft.Exchange.Core.RuleTasks;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Tasks
{
	[Serializable]
	public class AnyOfToHeaderMemberOfPredicate : SmtpAddressesPredicate, IEquatable<AnyOfToHeaderMemberOfPredicate>
	{
		public AnyOfToHeaderMemberOfPredicate() : base("isMemberOf", "Message.To")
		{
		}

		public override int GetHashCode()
		{
			return Utils.GetHashCodeForArray<SmtpAddress>(this.Addresses);
		}

		public override bool Equals(object right)
		{
			return !object.ReferenceEquals(right, null) && (object.ReferenceEquals(this, right) || (!(base.GetType() != right.GetType()) && this.Equals(right as AnyOfToHeaderMemberOfPredicate)));
		}

		public bool Equals(AnyOfToHeaderMemberOfPredicate other)
		{
			if (this.Addresses == null)
			{
				return null == other.Addresses;
			}
			return this.Addresses.SequenceEqual(other.Addresses);
		}

		[ExceptionParameterName("ExceptIfAnyOfToHeaderMemberOf")]
		[LocDisplayName(RulesTasksStrings.IDs.ToDLAddressDisplayName)]
		[LocDescription(RulesTasksStrings.IDs.ToDLAddressDescription)]
		[ConditionParameterName("AnyOfToHeaderMemberOf")]
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
				return new SmtpAddressesPredicate.LocalizedStringDescriptionDelegate(RulesTasksStrings.RuleDescriptionAnyOfToHeaderMemberOf);
			}
		}

		internal static TransportRulePredicate CreateFromInternalCondition(Condition condition)
		{
			return SmtpAddressesPredicate.CreateFromInternalCondition<AnyOfToHeaderMemberOfPredicate>(condition, "isMemberOf", "Message.To");
		}

		private const string InternalPredicateName = "isMemberOf";

		private const string InternalPropertyName = "Message.To";
	}
}
