using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Core.RuleTasks;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Tasks
{
	[Serializable]
	public class FromMemberOfPredicate : SmtpAddressesPredicate, IEquatable<FromMemberOfPredicate>
	{
		public FromMemberOfPredicate() : base("isMemberOf", "Message.From")
		{
		}

		public override int GetHashCode()
		{
			return Utils.GetHashCodeForArray<SmtpAddress>(this.Addresses);
		}

		public override bool Equals(object right)
		{
			return !object.ReferenceEquals(right, null) && (object.ReferenceEquals(this, right) || (!(base.GetType() != right.GetType()) && this.Equals(right as FromMemberOfPredicate)));
		}

		public bool Equals(FromMemberOfPredicate other)
		{
			if (this.Addresses == null)
			{
				return null == other.Addresses;
			}
			return this.Addresses.SequenceEqual(other.Addresses);
		}

		[LocDescription(RulesTasksStrings.IDs.FromDLAddressDescription)]
		[LocDisplayName(RulesTasksStrings.IDs.FromDLAddressDisplayName)]
		[ConditionParameterName("FromMemberOf")]
		[ExceptionParameterName("ExceptIfFromMemberOf")]
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
				return new SmtpAddressesPredicate.LocalizedStringDescriptionDelegate(RulesTasksStrings.RuleDescriptionFromMemberOf);
			}
		}

		[LocDisplayName(RulesTasksStrings.IDs.SubTypeDisplayName)]
		[LocDescription(RulesTasksStrings.IDs.SubTypeDescription)]
		public override IEnumerable<RuleSubType> RuleSubTypes
		{
			get
			{
				return new RuleSubType[]
				{
					RuleSubType.None,
					RuleSubType.Dlp
				};
			}
		}

		internal static TransportRulePredicate CreateFromInternalCondition(Condition condition)
		{
			return SmtpAddressesPredicate.CreateFromInternalCondition<FromMemberOfPredicate>(condition, "isMemberOf", "Message.From");
		}

		private const string InternalPredicateName = "isMemberOf";

		private const string InternalPropertyName = "Message.From";
	}
}
