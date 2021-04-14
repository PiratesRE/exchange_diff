using System;
using System.Runtime.Serialization;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[DataContract]
	internal abstract class RuleActionFwdDelegateData : RuleActionData
	{
		public RuleActionFwdDelegateData()
		{
		}

		[DataMember(EmitDefaultValue = false)]
		public AdrEntryData[] Recipients { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public uint Flags { get; set; }

		public RuleActionFwdDelegateData(RuleAction.FwdDelegate ruleAction, uint flags) : base(ruleAction)
		{
			this.Recipients = DataConverter<AdrEntryConverter, AdrEntry, AdrEntryData>.GetData(ruleAction.Recipients);
			this.Flags = flags;
		}

		protected override void EnumPropTagsInternal(CommonUtils.EnumPropTagDelegate del)
		{
			foreach (AdrEntryData adrEntryData in this.Recipients)
			{
				foreach (PropValueData propValueData in adrEntryData.Values)
				{
					int propTag = propValueData.PropTag;
					del(ref propTag);
					propValueData.PropTag = propTag;
				}
			}
		}

		protected override void EnumAdrEntriesInternal(CommonUtils.EnumAdrEntryDelegate del)
		{
			base.EnumAdrEntriesInternal(del);
			foreach (AdrEntryData aed in this.Recipients)
			{
				del(aed);
			}
		}

		protected override string ToStringInternal()
		{
			return string.Format("{0}, Flags:{1}", CommonUtils.ConcatEntries<AdrEntryData>(this.Recipients, null), this.Flags);
		}
	}
}
