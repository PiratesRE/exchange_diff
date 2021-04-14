using System;
using System.Linq;
using Microsoft.Exchange.Core.RuleTasks;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Tasks
{
	[Serializable]
	public class CopyToSmtpAction : SmtpAddressAction, IEquatable<CopyToSmtpAction>
	{
		public CopyToSmtpAction() : base("AddCcRecipientSmtpOnly")
		{
		}

		public override int GetHashCode()
		{
			return Utils.GetHashCodeForArray<SmtpAddress>(this.Addresses);
		}

		public override bool Equals(object right)
		{
			return !object.ReferenceEquals(right, null) && (object.ReferenceEquals(this, right) || (!(base.GetType() != right.GetType()) && this.Equals(right as CopyToSmtpAction)));
		}

		public bool Equals(CopyToSmtpAction other)
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
				return new SmtpAddressAction.LocalizedStringDescriptionDelegate(RulesTasksStrings.RuleDescriptionCopyTo);
			}
		}

		internal static TransportRuleAction CreateFromInternalActions(ShortList<Action> actions, ref int index)
		{
			return SmtpAddressAction.CreateFromInternalActions<CopyToSmtpAction>(actions, ref index, "AddCcRecipientSmtpOnly");
		}

		private const string InternalActionName = "AddCcRecipientSmtpOnly";
	}
}
