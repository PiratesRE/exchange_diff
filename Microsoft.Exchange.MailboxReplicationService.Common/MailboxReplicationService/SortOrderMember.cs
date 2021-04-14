using System;
using System.Runtime.Serialization;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[DataContract]
	internal sealed class SortOrderMember
	{
		[DataMember(IsRequired = true)]
		public int PropTag { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public int Flags { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public bool IsCategory { get; set; }

		public override string ToString()
		{
			return string.Format("[{0}{1}:{2}]", this.IsCategory ? "Cat:" : string.Empty, TraceUtils.DumpPropTag((PropTag)this.PropTag), ((SortFlags)this.Flags).ToString());
		}
	}
}
