using System;
using System.Linq;
using Microsoft.Exchange.Core.RuleTasks;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Tasks
{
	[Serializable]
	public class AnyOfToCcHeaderPredicate : SmtpAddressesPredicate, IEquatable<AnyOfToCcHeaderPredicate>
	{
		public AnyOfToCcHeaderPredicate() : base("isSameUser", "Message.ToCc")
		{
		}

		public override int GetHashCode()
		{
			return Utils.GetHashCodeForArray<SmtpAddress>(this.Addresses);
		}

		public override bool Equals(object right)
		{
			return !object.ReferenceEquals(right, null) && (object.ReferenceEquals(this, right) || (!(base.GetType() != right.GetType()) && this.Equals(right as AnyOfToCcHeaderPredicate)));
		}

		public bool Equals(AnyOfToCcHeaderPredicate other)
		{
			if (this.Addresses == null)
			{
				return null == other.Addresses;
			}
			return this.Addresses.SequenceEqual(other.Addresses);
		}

		[LocDisplayName(RulesTasksStrings.IDs.ToAddressesDisplayName)]
		[ExceptionParameterName("ExceptIfAnyOfToCcHeader")]
		[LocDescription(RulesTasksStrings.IDs.ToAddressesDescription)]
		[ConditionParameterName("AnyOfToCcHeader")]
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
				return new SmtpAddressesPredicate.LocalizedStringDescriptionDelegate(RulesTasksStrings.RuleDescriptionAnyOfToCcHeader);
			}
		}

		internal static TransportRulePredicate CreateFromInternalCondition(Condition condition)
		{
			return SmtpAddressesPredicate.CreateFromInternalCondition<AnyOfToCcHeaderPredicate>(condition, "isSameUser", "Message.ToCc");
		}

		private const string InternalPredicateName = "isSameUser";

		private const string InternalPropertyName = "Message.ToCc";
	}
}
