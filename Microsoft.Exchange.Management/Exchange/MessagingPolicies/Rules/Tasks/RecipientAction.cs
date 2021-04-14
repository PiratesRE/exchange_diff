using System;
using System.Collections.Generic;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Core.RuleTasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Tasks
{
	[Serializable]
	public abstract class RecipientAction : TransportRuleAction
	{
		protected RecipientAction(string internalActionName)
		{
			this.internalActionName = internalActionName;
		}

		public abstract SmtpAddress[] Addresses { get; set; }

		internal override string Description
		{
			get
			{
				return this.LocalizedStringDescription(RuleDescription.BuildDescriptionStringFromStringArray(Utils.BuildSmtpAddressStringList(this.Addresses, false), RulesTasksStrings.RuleDescriptionAndDelimiter, base.MaxDescriptionListLength));
			}
		}

		protected abstract RecipientAction.LocalizedStringDescriptionDelegate LocalizedStringDescription { get; }

		internal static TransportRuleAction CreateFromInternalActions<T>(ShortList<Action> actions, ref int index, string internalActionName) where T : RecipientAction, new()
		{
			List<SmtpAddress> list = new List<SmtpAddress>();
			while (index < actions.Count && !(actions[index].Name != internalActionName))
			{
				string stringValue = TransportRuleAction.GetStringValue(actions[index].Arguments[0]);
				SmtpAddress item = new SmtpAddress(stringValue);
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
					errors.Add(new RulePhrase.RulePhraseValidationError(RulesTasksStrings.InvalidRecipient(smtpAddress.ToString()), base.Name));
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
				ShortList<Argument> shortList = new ShortList<Argument>();
				string text = smtpAddress.ToString();
				if (string.IsNullOrEmpty(text))
				{
					throw new ArgumentException(RulesTasksStrings.InvalidRecipient(text), "Addresses");
				}
				shortList.Add(new Value(text));
				Action item = TransportRuleParser.Instance.CreateAction(this.internalActionName, shortList, Utils.GetActionName(this));
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

		protected SmtpAddress[] addresses;

		protected delegate LocalizedString LocalizedStringDescriptionDelegate(string addresses);
	}
}
