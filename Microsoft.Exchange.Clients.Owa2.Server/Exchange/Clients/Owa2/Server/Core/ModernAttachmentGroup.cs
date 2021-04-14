using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class ModernAttachmentGroup
	{
		[DataMember(Order = 1)]
		public int AttachmentsReturned { get; set; }

		[DataMember(Order = 1)]
		public int ItemsProcessed { get; set; }

		[DataMember(Order = 1)]
		public int ItemsOffsetNext { get; set; }

		[DataMember(Order = 1)]
		public int ItemsTotal { get; set; }

		[DataMember(Order = 1)]
		public bool RetrievedLastItem { get; set; }

		[DataMember(Order = 2)]
		public string[] Path { get; set; }

		[DataMember(Order = 3)]
		public ModernAttachment[] AttachmentGroup { get; set; }
	}
}
