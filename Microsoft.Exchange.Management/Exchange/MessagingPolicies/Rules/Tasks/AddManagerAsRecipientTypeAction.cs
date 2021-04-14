using System;
using Microsoft.Exchange.Core.RuleTasks;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Tasks
{
	[Serializable]
	public class AddManagerAsRecipientTypeAction : TransportRuleAction, IEquatable<AddManagerAsRecipientTypeAction>
	{
		public override int GetHashCode()
		{
			return this.RecipientType.GetHashCode();
		}

		public override bool Equals(object right)
		{
			return !object.ReferenceEquals(right, null) && (object.ReferenceEquals(this, right) || (!(base.GetType() != right.GetType()) && this.Equals(right as AddManagerAsRecipientTypeAction)));
		}

		public bool Equals(AddManagerAsRecipientTypeAction other)
		{
			return this.RecipientType.Equals(other.RecipientType);
		}

		[LocDescription(RulesTasksStrings.IDs.RecipientTypeDescription)]
		[ActionParameterName("AddManagerAsRecipientType")]
		[LocDisplayName(RulesTasksStrings.IDs.RecipientTypeDisplayName)]
		public AddedRecipientType RecipientType
		{
			get
			{
				return this.addedRecipientType;
			}
			set
			{
				this.addedRecipientType = value;
			}
		}

		internal override string Description
		{
			get
			{
				return RulesTasksStrings.RuleDescriptionAddManagerAsRecipientType(LocalizedDescriptionAttribute.FromEnum(typeof(AddedRecipientType), this.RecipientType));
			}
		}

		internal static TransportRuleAction CreateFromInternalAction(Action action)
		{
			if (action.Name != "AddManagerAsRecipientType")
			{
				return null;
			}
			AddedRecipientType recipientType;
			try
			{
				recipientType = (AddedRecipientType)Enum.Parse(typeof(AddedRecipientType), TransportRuleAction.GetStringValue(action.Arguments[0]));
			}
			catch (ArgumentException)
			{
				return null;
			}
			catch (OverflowException)
			{
				return null;
			}
			return new AddManagerAsRecipientTypeAction
			{
				RecipientType = recipientType
			};
		}

		internal override void Reset()
		{
			this.addedRecipientType = AddedRecipientType.To;
			base.Reset();
		}

		internal override Action ToInternalAction()
		{
			return TransportRuleParser.Instance.CreateAction("AddManagerAsRecipientType", new ShortList<Argument>
			{
				new Value(this.addedRecipientType.ToString())
			}, Utils.GetActionName(this));
		}

		internal override string GetActionParameters()
		{
			return Enum.GetName(typeof(AddedRecipientType), this.RecipientType);
		}

		private AddedRecipientType addedRecipientType;
	}
}
