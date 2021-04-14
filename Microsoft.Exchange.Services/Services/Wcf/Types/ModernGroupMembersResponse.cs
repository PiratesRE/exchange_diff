using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class ModernGroupMembersResponse
	{
		[DataMember]
		public ModernGroupMemberType[] Members { get; set; }

		[DataMember]
		public int Count { get; set; }

		[DataMember]
		public bool HasMoreMembers { get; set; }
	}
}
