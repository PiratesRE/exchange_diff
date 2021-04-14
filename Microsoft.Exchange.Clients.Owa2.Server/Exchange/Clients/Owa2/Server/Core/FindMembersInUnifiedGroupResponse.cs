using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Services.Wcf;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[DataContract(Name = "FindMembersInUnifiedGroupResponse", Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class FindMembersInUnifiedGroupResponse : BaseJsonResponse
	{
		[DataMember(Name = "Members", IsRequired = true)]
		public ModernGroupMemberType[] Members { get; set; }

		[DataMember(Name = "HasMoreMembers", IsRequired = true)]
		public bool HasMoreMembers { get; set; }
	}
}
