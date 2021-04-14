using System;
using System.Linq;
using Microsoft.Exchange.Core.RuleTasks;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Tasks
{
	[Serializable]
	public class CopyToAction : RecipientAction, IEquatable<CopyToAction>
	{
		public CopyToAction() : base("AddCcRecipientSmtpOnly")
		{
		}

		public override int GetHashCode()
		{
			return Utils.GetHashCodeForArray<SmtpAddress>(this.Addresses);
		}

		public override bool Equals(object right)
		{
			return !object.ReferenceEquals(right, null) && (object.ReferenceEquals(this, right) || (!(base.GetType() != right.GetType()) && this.Equals(right as CopyToAction)));
		}

		public bool Equals(CopyToAction other)
		{
			if (this.Addresses == null)
			{
				return null == other.Addresses;
			}
			return this.Addresses.SequenceEqual(other.Addresses);
		}

		[LocDescription(RulesTasksStrings.IDs.ToAddressesDescription)]
		[LocDisplayName(RulesTasksStrings.IDs.ToAddressesDisplayName)]
		[ActionParameterName("CopyTo")]
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
				return new RecipientAction.LocalizedStringDescriptionDelegate(RulesTasksStrings.RuleDescriptionCopyTo);
			}
		}

		internal static TransportRuleAction CreateFromInternalActions(ShortList<Action> actions, ref int index)
		{
			return RecipientAction.CreateFromInternalActions<CopyToAction>(actions, ref index, "AddCcRecipientSmtpOnly");
		}

		internal override string GetActionParameters()
		{
			return string.Join(", ", Utils.BuildSmtpAddressStringList(this.Addresses, true));
		}

		private const string InternalActionName = "AddCcRecipientSmtpOnly";
	}
}
