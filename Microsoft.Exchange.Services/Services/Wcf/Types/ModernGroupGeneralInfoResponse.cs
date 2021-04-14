using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Directory.Management;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class ModernGroupGeneralInfoResponse
	{
		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public string SmtpAddress { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public ModernGroupObjectType ModernGroupType { get; set; }

		[DataMember]
		public bool IsOwner { get; set; }

		[DataMember]
		public bool IsMember { get; set; }

		[DataMember]
		public bool ShouldEscalate { get; set; }

		[DataMember]
		public bool RequireSenderAuthenticationEnabled { get; set; }

		[DataMember]
		public bool AutoSubscribeNewGroupMembers { get; set; }

		[DataMember]
		public int OwnersCount { get; set; }
	}
}
