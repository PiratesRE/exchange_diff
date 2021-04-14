using System;
using System.Runtime.Serialization;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[DataContract]
	internal abstract class RuleActionData
	{
		public RuleActionData()
		{
		}

		[DataMember(EmitDefaultValue = false)]
		public uint UserFlags { get; set; }

		public RuleActionData(RuleAction ruleAction)
		{
			this.UserFlags = ruleAction.UserFlags;
		}

		public static RuleActionData GetRuleActionData(RuleAction ruleAction)
		{
			RuleActionData ruleActionData;
			if (ruleAction is RuleAction.InMailboxMove)
			{
				ruleActionData = new RuleActionInMailboxMoveData((RuleAction.InMailboxMove)ruleAction);
			}
			else if (ruleAction is RuleAction.InMailboxCopy)
			{
				ruleActionData = new RuleActionInMailboxCopyData((RuleAction.InMailboxCopy)ruleAction);
			}
			else if (ruleAction is RuleAction.ExternalMove)
			{
				ruleActionData = new RuleActionExternalMoveData((RuleAction.ExternalMove)ruleAction);
			}
			else if (ruleAction is RuleAction.ExternalCopy)
			{
				ruleActionData = new RuleActionExternalCopyData((RuleAction.ExternalCopy)ruleAction);
			}
			else if (ruleAction is RuleAction.Reply)
			{
				ruleActionData = new RuleActionReplyData((RuleAction.Reply)ruleAction);
			}
			else if (ruleAction is RuleAction.OOFReply)
			{
				ruleActionData = new RuleActionOOFReplyData((RuleAction.OOFReply)ruleAction);
			}
			else if (ruleAction is RuleAction.Forward)
			{
				ruleActionData = new RuleActionForwardData((RuleAction.Forward)ruleAction);
			}
			else if (ruleAction is RuleAction.Delegate)
			{
				ruleActionData = new RuleActionDelegateData((RuleAction.Delegate)ruleAction);
			}
			else if (ruleAction is RuleAction.Bounce)
			{
				ruleActionData = new RuleActionBounceData((RuleAction.Bounce)ruleAction);
			}
			else if (ruleAction is RuleAction.Tag)
			{
				ruleActionData = new RuleActionTagData((RuleAction.Tag)ruleAction);
			}
			else if (ruleAction is RuleAction.Defer)
			{
				ruleActionData = new RuleActionDeferData((RuleAction.Defer)ruleAction);
			}
			else if (ruleAction is RuleAction.Delete)
			{
				ruleActionData = new RuleActionDeleteData((RuleAction.Delete)ruleAction);
			}
			else
			{
				if (!(ruleAction is RuleAction.MarkAsRead))
				{
					return null;
				}
				ruleActionData = new RuleActionMarkAsReadData((RuleAction.MarkAsRead)ruleAction);
			}
			ruleActionData.UserFlags = ruleAction.UserFlags;
			return ruleActionData;
		}

		public RuleAction GetRuleAction()
		{
			RuleAction ruleActionInternal = this.GetRuleActionInternal();
			ruleActionInternal.UserFlags = this.UserFlags;
			return ruleActionInternal;
		}

		public override string ToString()
		{
			if (this.UserFlags != 0U)
			{
				return string.Format("RuleAction: UserFlags=0x{0:X}, {1}", this.UserFlags, this.ToStringInternal());
			}
			return string.Format("RuleAction: {0}", this.ToStringInternal());
		}

		public void Enumerate(CommonUtils.EnumPropTagDelegate propTagEnumerator, CommonUtils.EnumPropValueDelegate propValueEnumerator, CommonUtils.EnumAdrEntryDelegate adrEntryEnumerator)
		{
			if (propTagEnumerator != null)
			{
				this.EnumPropTagsInternal(propTagEnumerator);
			}
			if (propValueEnumerator != null)
			{
				this.EnumPropValuesInternal(propValueEnumerator);
			}
			if (adrEntryEnumerator != null)
			{
				this.EnumAdrEntriesInternal(adrEntryEnumerator);
			}
		}

		protected virtual void EnumPropTagsInternal(CommonUtils.EnumPropTagDelegate del)
		{
		}

		protected virtual void EnumPropValuesInternal(CommonUtils.EnumPropValueDelegate del)
		{
		}

		protected virtual void EnumAdrEntriesInternal(CommonUtils.EnumAdrEntryDelegate del)
		{
		}

		protected abstract RuleAction GetRuleActionInternal();

		protected abstract string ToStringInternal();
	}
}
