using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[DataContract]
	internal sealed class PropProblemData
	{
		[DataMember(IsRequired = true)]
		public int PropTag { get; set; }

		[DataMember(IsRequired = true)]
		public int Scode { get; set; }

		[DataMember]
		public int Index { get; set; }
	}
}
