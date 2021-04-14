using System;
using System.Linq;
using Microsoft.Exchange.Core.RuleTasks;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Tasks
{
	[Serializable]
	public class AnyOfCcHeaderPredicate : SmtpAddressesPredicate, IEquatable<AnyOfCcHeaderPredicate>
	{
		public AnyOfCcHeaderPredicate() : base("isSameUser", "Message.Cc")
		{
		}

		public override int GetHashCode()
		{
			return Utils.GetHashCodeForArray<SmtpAddress>(this.Addresses);
		}

		public override bool Equals(object right)
		{
			return !object.ReferenceEquals(right, null) && (object.ReferenceEquals(this, right) || (!(base.GetType() != right.GetType()) && this.Equals(right as AnyOfCcHeaderPredicate)));
		}

		public bool Equals(AnyOfCcHeaderPredicate other)
		{
			if (this.Addresses == null)
			{
				return null == other.Addresses;
			}
			return this.Addresses.SequenceEqual(other.Addresses);
		}

		[LocDisplayName(RulesTasksStrings.IDs.ToAddressesDisplayName)]
		[ConditionParameterName("AnyOfCcHeader")]
		[LocDescription(RulesTasksStrings.IDs.ToAddressesDescription)]
		[ExceptionParameterName("ExceptIfAnyOfCcHeader")]
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
				return new SmtpAddressesPredicate.LocalizedStringDescriptionDelegate(RulesTasksStrings.RuleDescriptionAnyOfCcHeader);
			}
		}

		internal static TransportRulePredicate CreateFromInternalCondition(Condition condition)
		{
			return SmtpAddressesPredicate.CreateFromInternalCondition<AnyOfCcHeaderPredicate>(condition, "isSameUser", "Message.Cc");
		}

		private const string InternalPredicateName = "isSameUser";

		private const string InternalPropertyName = "Message.Cc";
	}
}
