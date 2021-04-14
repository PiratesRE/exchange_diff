using System;
using System.Runtime.Serialization;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[DataContract]
	internal abstract class RuleActionBaseReplyData : RuleActionData
	{
		public RuleActionBaseReplyData()
		{
		}

		[DataMember(EmitDefaultValue = false)]
		public byte[] ReplyTemplateMessageEntryID { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public Guid ReplyTemplateGuid { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public uint Flags { get; set; }

		public RuleActionBaseReplyData(RuleAction.BaseReply ruleAction, byte[] replyTemplateMessageEntryID, Guid replyTemplateGuid, uint flags) : base(ruleAction)
		{
			this.ReplyTemplateMessageEntryID = replyTemplateMessageEntryID;
			this.ReplyTemplateGuid = replyTemplateGuid;
			this.Flags = flags;
		}

		protected override void EnumPropValuesInternal(CommonUtils.EnumPropValueDelegate del)
		{
			base.EnumPropValuesInternal(del);
			PropValueData propValueData = new PropValueData(PropTag.ReplyTemplateID, this.ReplyTemplateMessageEntryID);
			del(propValueData);
			this.ReplyTemplateMessageEntryID = (byte[])propValueData.Value;
		}

		protected override string ToStringInternal()
		{
			return string.Format("TemplateEID:{0}, TemplateGuid:{1}, Flags:{2}", TraceUtils.DumpEntryId(this.ReplyTemplateMessageEntryID), this.ReplyTemplateGuid, this.Flags);
		}
	}
}
