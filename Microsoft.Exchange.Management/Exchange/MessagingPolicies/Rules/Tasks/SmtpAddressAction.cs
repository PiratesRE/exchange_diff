using System;
using System.Collections.Generic;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Core.RuleTasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Tasks
{
	[Serializable]
	public abstract class SmtpAddressAction : TransportRuleAction
	{
		protected SmtpAddressAction(string internalActionName)
		{
			this.internalActionName = internalActionName;
		}

		[LocDescription(RulesTasksStrings.IDs.ToAddressesDescription)]
		[LocDisplayName(RulesTasksStrings.IDs.ToAddressesDisplayName)]
		public virtual SmtpAddress[] Addresses
		{
			get
			{
				return this.addresses;
			}
			set
			{
				if (value != null)
				{
					for (int i = 0; i < value.Length; i++)
					{
						SmtpAddress smtpAddress = value[i];
						if (!smtpAddress.IsValidAddress)
						{
							throw new ArgumentException(RulesTasksStrings.InvalidSmtpAddress(smtpAddress.ToString()), "Address");
						}
					}
				}
				this.addresses = value;
			}
		}

		internal override string Description
		{
			get
			{
				return RulesTasksStrings.RuleDescriptionAddToRecipient(RuleDescription.BuildDescriptionStringFromStringArray(Utils.BuildSmtpAddressStringList(this.Addresses, false), RulesTasksStrings.RuleDescriptionAndDelimiter, base.MaxDescriptionListLength));
			}
		}

		protected abstract SmtpAddressAction.LocalizedStringDescriptionDelegate LocalizedStringDescription { get; }

		internal static TransportRuleAction CreateFromInternalActions<T>(ShortList<Action> actions, ref int index, string internalActionName) where T : SmtpAddressAction, new()
		{
			List<SmtpAddress> list = new List<SmtpAddress>();
			while (index < actions.Count && !(actions[index].Name != internalActionName))
			{
				SmtpAddress item = new SmtpAddress(TransportRuleAction.GetStringValue(actions[index].Arguments[0]));
				if (!item.IsValidAddress)
				{
					break;
				}
				list.Add(item);
				index++;
			}
			if (list.Count == 0)
			{
				return null;
			}
			T t = Activator.CreateInstance<T>();
			t.Addresses = list.ToArray();
			return t;
		}

		internal override void Reset()
		{
			this.addresses = null;
			base.Reset();
		}

		protected override void ValidateRead(List<ValidationError> errors)
		{
			if (this.Addresses == null || this.Addresses.Length == 0)
			{
				errors.Add(new RulePhrase.RulePhraseValidationError(RulesTasksStrings.ArgumentNotSet, base.Name));
				return;
			}
			foreach (SmtpAddress smtpAddress in this.Addresses)
			{
				if (!smtpAddress.IsValidAddress)
				{
					errors.Add(new RulePhrase.RulePhraseValidationError(RulesTasksStrings.InvalidSmtpAddress(smtpAddress.ToString()), base.Name));
					return;
				}
			}
			base.ValidateRead(errors);
		}

		internal override Action[] ToInternalActions()
		{
			List<Action> list = new List<Action>();
			foreach (SmtpAddress smtpAddress in this.Addresses)
			{
				Action item = TransportRuleParser.Instance.CreateAction(this.internalActionName, new ShortList<Argument>
				{
					new Value(smtpAddress.ToString())
				}, Utils.GetActionName(this));
				list.Add(item);
			}
			return list.ToArray();
		}

		internal override string GetActionParameters()
		{
			return string.Join(", ", Utils.BuildSmtpAddressStringList(this.Addresses, true));
		}

		internal override void SuppressPiiData()
		{
			string[] array;
			string[] array2;
			this.Addresses = SuppressingPiiData.Redact(this.Addresses, out array, out array2);
		}

		private readonly string internalActionName;

		private SmtpAddress[] addresses;

		protected delegate LocalizedString LocalizedStringDescriptionDelegate(string addresses);
	}
}
