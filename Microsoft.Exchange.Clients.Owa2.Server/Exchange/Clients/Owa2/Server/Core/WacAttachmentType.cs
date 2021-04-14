using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class WacAttachmentType : FileAttachmentType
	{
		[DataMember]
		public string WacUrl { get; set; }

		[DataMember]
		public bool IsEdit { get; set; }

		[DataMember]
		public bool IsInDraft { get; set; }

		[DataMember]
		public WacAttachmentStatus Status { get; set; }
	}
}
