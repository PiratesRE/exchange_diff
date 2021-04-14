using System;
using System.Linq;
using Microsoft.Exchange.Core.RuleTasks;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Tasks
{
	[Serializable]
	public class AnyOfToHeaderPredicate : SmtpAddressesPredicate, IEquatable<AnyOfToHeaderPredicate>
	{
		public AnyOfToHeaderPredicate() : base("isSameUser", "Message.To")
		{
		}

		public override int GetHashCode()
		{
			return Utils.GetHashCodeForArray<SmtpAddress>(this.Addresses);
		}

		public override bool Equals(object right)
		{
			return !object.ReferenceEquals(right, null) && (object.ReferenceEquals(this, right) || (!(base.GetType() != right.GetType()) && this.Equals(right as AnyOfToHeaderPredicate)));
		}

		public bool Equals(AnyOfToHeaderPredicate other)
		{
			if (this.Addresses == null)
			{
				return null == other.Addresses;
			}
			return this.Addresses.SequenceEqual(other.Addresses);
		}

		[ExceptionParameterName("ExceptIfAnyOfToHeader")]
		[ConditionParameterName("AnyOfToHeader")]
		[LocDisplayName(RulesTasksStrings.IDs.ToAddressesDisplayName)]
		[LocDescription(RulesTasksStrings.IDs.ToAddressesDescription)]
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
				return new SmtpAddressesPredicate.LocalizedStringDescriptionDelegate(RulesTasksStrings.RuleDescriptionAnyOfToHeader);
			}
		}

		internal static TransportRulePredicate CreateFromInternalCondition(Condition condition)
		{
			return SmtpAddressesPredicate.CreateFromInternalCondition<AnyOfToHeaderPredicate>(condition, "isSameUser", "Message.To");
		}

		private const string InternalPredicateName = "isSameUser";

		private const string InternalPropertyName = "Message.To";
	}
}
