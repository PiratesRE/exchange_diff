using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class GetModernGroupResponse
	{
		[DataMember]
		public ModernGroupGeneralInfoResponse GeneralInfo { get; set; }

		[DataMember]
		public ModernGroupMembersResponse MembersInfo { get; set; }

		[DataMember]
		public ModernGroupMembersResponse OwnerList { get; set; }

		[DataMember]
		public ModernGroupExternalResources ExternalResources { get; set; }

		[DataMember]
		public GroupMailboxProperties MailboxProperties { get; set; }
	}
}
