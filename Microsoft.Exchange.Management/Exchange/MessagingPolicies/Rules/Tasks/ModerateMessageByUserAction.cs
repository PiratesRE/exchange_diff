using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Core.RuleTasks;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Tasks
{
	[Serializable]
	public class ModerateMessageByUserAction : TransportRuleAction, IEquatable<ModerateMessageByUserAction>
	{
		public override int GetHashCode()
		{
			return Utils.GetHashCodeForArray<SmtpAddress>(this.Addresses);
		}

		public override bool Equals(object right)
		{
			return !object.ReferenceEquals(right, null) && (object.ReferenceEquals(this, right) || (!(base.GetType() != right.GetType()) && this.Equals(right as ModerateMessageByUserAction)));
		}

		public bool Equals(ModerateMessageByUserAction other)
		{
			if (this.Addresses == null)
			{
				return null == other.Addresses;
			}
			return this.Addresses.SequenceEqual(other.Addresses);
		}

		[LocDisplayName(RulesTasksStrings.IDs.ToAddressesDisplayName)]
		[ActionParameterName("ModerateMessageByUser")]
		[LocDescription(RulesTasksStrings.IDs.ToAddressesDescription)]
		public virtual SmtpAddress[] Addresses
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

		internal override string Description
		{
			get
			{
				return RulesTasksStrings.RuleDescriptionModerateMessageByUser(RuleDescription.BuildDescriptionStringFromStringArray(Utils.BuildSmtpAddressStringList(this.Addresses, false), RulesTasksStrings.RuleDescriptionAndDelimiter, base.MaxDescriptionListLength));
			}
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

		internal static TransportRuleAction CreateFromInternalAction(Action action)
		{
			if (action.Name != "ModerateMessageByUser")
			{
				return null;
			}
			List<SmtpAddress> list = new List<SmtpAddress>();
			string stringValue = TransportRuleAction.GetStringValue(action.Arguments[0]);
			string[] array = stringValue.Split(new string[]
			{
				";"
			}, StringSplitOptions.RemoveEmptyEntries);
			foreach (string text in array)
			{
				SmtpAddress item = new SmtpAddress(text.Trim());
				list.Add(item);
			}
			if (list.Count == 0)
			{
				return null;
			}
			return new ModerateMessageByUserAction
			{
				Addresses = list.ToArray()
			};
		}

		internal override Action ToInternalAction()
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (SmtpAddress smtpAddress in this.Addresses)
			{
				if (!smtpAddress.IsValidAddress)
				{
					throw new ArgumentException(RulesTasksStrings.InvalidRecipient(smtpAddress.ToString()), "Addresses");
				}
				string value = smtpAddress.ToString();
				if (string.IsNullOrEmpty(value))
				{
					throw new ArgumentException(RulesTasksStrings.InvalidRecipient(smtpAddress.ToString()), "Addresses");
				}
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append(";");
				}
				stringBuilder.Append(value);
			}
			return TransportRuleParser.Instance.CreateAction("ModerateMessageByUser", new ShortList<Argument>
			{
				new Value(stringBuilder.ToString())
			}, Utils.GetActionName(this));
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

		private const string InternalActionName = "ModerateMessageByUser";

		private const string AddressDelimiter = ";";

		private SmtpAddress[] addresses;
	}
}
