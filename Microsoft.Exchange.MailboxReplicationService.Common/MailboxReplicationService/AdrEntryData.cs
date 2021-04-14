using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[DataContract]
	internal sealed class AdrEntryData
	{
		[DataMember]
		public PropValueData[] Values { get; set; }

		public override string ToString()
		{
			return string.Format("ADRENTRY: {0}", CommonUtils.ConcatEntries<PropValueData>(this.Values, null));
		}
	}
}
