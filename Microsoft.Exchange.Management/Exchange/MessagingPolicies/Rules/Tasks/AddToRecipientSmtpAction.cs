using System;
using System.Linq;
using Microsoft.Exchange.Core.RuleTasks;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Tasks
{
	[Serializable]
	public class AddToRecipientSmtpAction : SmtpAddressAction, IEquatable<AddToRecipientSmtpAction>
	{
		public AddToRecipientSmtpAction() : base("AddToRecipientSmtpOnly")
		{
		}

		public override int GetHashCode()
		{
			return Utils.GetHashCodeForArray<SmtpAddress>(this.Addresses);
		}

		public override bool Equals(object right)
		{
			return !object.ReferenceEquals(right, null) && (object.ReferenceEquals(this, right) || (!(base.GetType() != right.GetType()) && this.Equals(right as AddToRecipientSmtpAction)));
		}

		public bool Equals(AddToRecipientSmtpAction other)
		{
			if (this.Addresses == null)
			{
				return null == other.Addresses;
			}
			return this.Addresses.SequenceEqual(other.Addresses);
		}

		protected override SmtpAddressAction.LocalizedStringDescriptionDelegate LocalizedStringDescription
		{
			get
			{
				return new SmtpAddressAction.LocalizedStringDescriptionDelegate(RulesTasksStrings.RuleDescriptionAddToRecipient);
			}
		}

		internal static TransportRuleAction CreateFromInternalActions(ShortList<Action> actions, ref int index)
		{
			return SmtpAddressAction.CreateFromInternalActions<AddToRecipientSmtpAction>(actions, ref index, "AddToRecipientSmtpOnly");
		}

		private const string InternalActionName = "AddToRecipientSmtpOnly";
	}
}
