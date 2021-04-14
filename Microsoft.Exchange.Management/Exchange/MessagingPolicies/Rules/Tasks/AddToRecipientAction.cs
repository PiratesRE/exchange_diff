using System;
using System.Linq;
using Microsoft.Exchange.Core.RuleTasks;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Tasks
{
	[Serializable]
	public class AddToRecipientAction : RecipientAction, IEquatable<AddToRecipientAction>
	{
		public AddToRecipientAction() : base("AddToRecipientSmtpOnly")
		{
		}

		public override int GetHashCode()
		{
			return Utils.GetHashCodeForArray<SmtpAddress>(this.Addresses);
		}

		public override bool Equals(object right)
		{
			return !object.ReferenceEquals(right, null) && (object.ReferenceEquals(this, right) || (!(base.GetType() != right.GetType()) && this.Equals(right as AddToRecipientAction)));
		}

		public bool Equals(AddToRecipientAction other)
		{
			if (this.Addresses == null)
			{
				return null == other.Addresses;
			}
			return this.Addresses.SequenceEqual(other.Addresses);
		}

		[LocDescription(RulesTasksStrings.IDs.ToAddressesDescription)]
		[ActionParameterName("AddToRecipients")]
		[LocDisplayName(RulesTasksStrings.IDs.ToAddressesDisplayName)]
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

		protected override RecipientAction.LocalizedStringDescriptionDelegate LocalizedStringDescription
		{
			get
			{
				return new RecipientAction.LocalizedStringDescriptionDelegate(RulesTasksStrings.RuleDescriptionAddToRecipient);
			}
		}

		internal static TransportRuleAction CreateFromInternalActions(ShortList<Action> actions, ref int index)
		{
			return RecipientAction.CreateFromInternalActions<AddToRecipientAction>(actions, ref index, "AddToRecipientSmtpOnly");
		}

		private const string InternalActionName = "AddToRecipientSmtpOnly";
	}
}
