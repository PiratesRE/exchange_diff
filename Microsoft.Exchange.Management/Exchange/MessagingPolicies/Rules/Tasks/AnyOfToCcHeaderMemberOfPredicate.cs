using System;
using System.Linq;
using Microsoft.Exchange.Core.RuleTasks;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Tasks
{
	[Serializable]
	public class AnyOfToCcHeaderMemberOfPredicate : SmtpAddressesPredicate, IEquatable<AnyOfToCcHeaderMemberOfPredicate>
	{
		public AnyOfToCcHeaderMemberOfPredicate() : base("isMemberOf", "Message.ToCc")
		{
		}

		public override int GetHashCode()
		{
			return Utils.GetHashCodeForArray<SmtpAddress>(this.Addresses);
		}

		public override bool Equals(object right)
		{
			return !object.ReferenceEquals(right, null) && (object.ReferenceEquals(this, right) || (!(base.GetType() != right.GetType()) && this.Equals(right as AnyOfToCcHeaderMemberOfPredicate)));
		}

		public bool Equals(AnyOfToCcHeaderMemberOfPredicate other)
		{
			if (this.Addresses == null)
			{
				return null == other.Addresses;
			}
			return this.Addresses.SequenceEqual(other.Addresses);
		}

		[LocDisplayName(RulesTasksStrings.IDs.ToDLAddressDisplayName)]
		[LocDescription(RulesTasksStrings.IDs.ToDLAddressDescription)]
		[ConditionParameterName("AnyOfToCcHeaderMemberOf")]
		[ExceptionParameterName("ExceptIfAnyOfToCcHeaderMemberOf")]
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
				return new SmtpAddressesPredicate.LocalizedStringDescriptionDelegate(RulesTasksStrings.RuleDescriptionAnyOfToCcHeaderMemberOf);
			}
		}

		internal static TransportRulePredicate CreateFromInternalCondition(Condition condition)
		{
			return SmtpAddressesPredicate.CreateFromInternalCondition<AnyOfToCcHeaderMemberOfPredicate>(condition, "isMemberOf", "Message.ToCc");
		}

		private const string InternalPredicateName = "isMemberOf";

		private const string InternalPropertyName = "Message.ToCc";
	}
}
