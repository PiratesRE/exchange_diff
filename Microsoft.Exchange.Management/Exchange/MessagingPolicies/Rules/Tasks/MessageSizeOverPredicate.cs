using System;
using Microsoft.Exchange.Core.RuleTasks;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Tasks
{
	[Serializable]
	public class MessageSizeOverPredicate : SizeOverPredicate, IEquatable<MessageSizeOverPredicate>
	{
		public MessageSizeOverPredicate() : base("Message.Size")
		{
		}

		public override int GetHashCode()
		{
			return this.Size.GetHashCode();
		}

		public override bool Equals(object right)
		{
			return !object.ReferenceEquals(right, null) && (object.ReferenceEquals(this, right) || (!(base.GetType() != right.GetType()) && this.Equals(right as MessageSizeOverPredicate)));
		}

		public bool Equals(MessageSizeOverPredicate other)
		{
			return this.Size.Equals(other.Size);
		}

		[LocDescription(RulesTasksStrings.IDs.MessageSizeDescription)]
		[LocDisplayName(RulesTasksStrings.IDs.MessageSizeDisplayName)]
		[ConditionParameterName("MessageSizeOver")]
		[ExceptionParameterName("ExceptIfMessageSizeOver")]
		public override ByteQuantifiedSize Size
		{
			get
			{
				return base.Size;
			}
			set
			{
				base.Size = value;
			}
		}

		internal override string Description
		{
			get
			{
				return RulesTasksStrings.RuleDescriptionMessageSizeOver(this.Size.ToString());
			}
		}

		internal static TransportRulePredicate CreateFromInternalCondition(Condition condition)
		{
			return SizeOverPredicate.CreateFromInternalCondition<MessageSizeOverPredicate>(condition, "Message.Size");
		}

		private const string InternalPropertyName = "Message.Size";
	}
}
