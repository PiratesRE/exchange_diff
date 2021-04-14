using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[DataContract]
	internal sealed class SortOrderData
	{
		[DataMember]
		public SortOrderMember[] Members { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public int LCID { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public bool FAI { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public bool Conversation { get; set; }

		public override string ToString()
		{
			return string.Format("SortOrder: LCID=0x{0:X}, FAI={1}, Conversation={2}, {3}", new object[]
			{
				this.LCID,
				this.FAI,
				this.Conversation,
				CommonUtils.ConcatEntries<SortOrderMember>(this.Members, null)
			});
		}

		internal void Enumerate(SortOrderData.EnumSortOrderMemberDelegate del)
		{
			foreach (SortOrderMember som in this.Members)
			{
				del(som);
			}
		}

		internal delegate void EnumSortOrderMemberDelegate(SortOrderMember som);
	}
}
