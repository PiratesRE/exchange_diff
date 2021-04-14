using System;
using Microsoft.Exchange.Core.RuleTasks;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Tasks
{
	[Serializable]
	public class AttachmentSizeOverPredicate : SizeOverPredicate, IEquatable<AttachmentSizeOverPredicate>
	{
		public AttachmentSizeOverPredicate() : base("Message.MaxAttachmentSize")
		{
		}

		public override int GetHashCode()
		{
			return this.Size.GetHashCode();
		}

		public override bool Equals(object right)
		{
			return !object.ReferenceEquals(right, null) && (object.ReferenceEquals(this, right) || (!(base.GetType() != right.GetType()) && this.Equals(right as AttachmentSizeOverPredicate)));
		}

		public bool Equals(AttachmentSizeOverPredicate other)
		{
			return this.Size.Equals(other.Size);
		}

		[LocDescription(RulesTasksStrings.IDs.AttachmentSizeDescription)]
		[LocDisplayName(RulesTasksStrings.IDs.AttachmentSizeDisplayName)]
		[ConditionParameterName("AttachmentSizeOver")]
		[ExceptionParameterName("ExceptIfAttachmentSizeOver")]
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
				return RulesTasksStrings.RuleDescriptionAttachmentSizeOver(this.Size.ToString());
			}
		}

		internal static TransportRulePredicate CreateFromInternalCondition(Condition condition)
		{
			return SizeOverPredicate.CreateFromInternalCondition<AttachmentSizeOverPredicate>(condition, "Message.MaxAttachmentSize");
		}

		private const string InternalPropertyName = "Message.MaxAttachmentSize";
	}
}
