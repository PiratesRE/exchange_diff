using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core.Wrappers
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class GetWacAttachmentInfoRequestWrapper
	{
		[DataMember(Name = "attachmentId")]
		public string AttachmentId { get; set; }

		[DataMember(Name = "isEdit")]
		public bool IsEdit { get; set; }

		[DataMember(Name = "draftId")]
		public string DraftId { get; set; }
	}
}
