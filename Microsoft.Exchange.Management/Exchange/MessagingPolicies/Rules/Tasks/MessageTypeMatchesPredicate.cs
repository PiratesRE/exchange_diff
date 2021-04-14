using System;
using Microsoft.Exchange.Core.RuleTasks;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Tasks
{
	[Serializable]
	public class MessageTypeMatchesPredicate : TransportRulePredicate, IEquatable<MessageTypeMatchesPredicate>
	{
		public override int GetHashCode()
		{
			return this.MessageType.GetHashCode();
		}

		public override bool Equals(object right)
		{
			return !object.ReferenceEquals(right, null) && (object.ReferenceEquals(this, right) || (!(base.GetType() != right.GetType()) && this.Equals(right as MessageTypeMatchesPredicate)));
		}

		public bool Equals(MessageTypeMatchesPredicate other)
		{
			return this.MessageType.Equals(other.MessageType);
		}

		[LocDisplayName(RulesTasksStrings.IDs.MessageTypeDisplayName)]
		[LocDescription(RulesTasksStrings.IDs.MessageTypeDescription)]
		[ConditionParameterName("MessageTypeMatches")]
		[ExceptionParameterName("ExceptIfMessageTypeMatches")]
		public MessageType MessageType
		{
			get
			{
				return this.messageType;
			}
			set
			{
				this.messageType = value;
			}
		}

		internal override string Description
		{
			get
			{
				return RulesTasksStrings.RuleDescriptionMessageTypeMatches(LocalizedDescriptionAttribute.FromEnum(typeof(MessageType), this.MessageType));
			}
		}

		internal static TransportRulePredicate CreateFromInternalCondition(Condition condition)
		{
			if (condition.ConditionType != ConditionType.Predicate)
			{
				return null;
			}
			PredicateCondition predicateCondition = (PredicateCondition)condition;
			if (predicateCondition.Value.RawValues.Count != 1 || !predicateCondition.Name.Equals("isMessageType"))
			{
				return null;
			}
			MessageType messageType;
			try
			{
				messageType = (MessageType)Enum.Parse(typeof(MessageType), predicateCondition.Value.RawValues[0]);
			}
			catch (ArgumentException)
			{
				return null;
			}
			return new MessageTypeMatchesPredicate
			{
				MessageType = messageType
			};
		}

		internal override Condition ToInternalCondition()
		{
			return TransportRuleParser.Instance.CreatePredicate("isMessageType", null, new ShortList<string>
			{
				this.MessageType.ToString()
			});
		}

		internal override string GetPredicateParameters()
		{
			return Enum.GetName(typeof(MessageType), this.MessageType);
		}

		private MessageType messageType;
	}
}
