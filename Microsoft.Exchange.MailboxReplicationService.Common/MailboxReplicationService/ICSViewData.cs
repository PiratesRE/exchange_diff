using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[DataContract]
	internal sealed class ICSViewData
	{
		[DataMember(EmitDefaultValue = false)]
		public bool Conversation { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public int[] CoveringPropertyTags { get; set; }

		public override string ToString()
		{
			return string.Format("ICS: Conversation={0}, {1}", this.Conversation, CommonUtils.ConcatEntries<int>(this.CoveringPropertyTags, (int ptag) => ptag.ToString("X")));
		}
	}
}
