using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class GetBingMapsPreviewResponse
	{
		[DataMember]
		public string Error { get; set; }

		[DataMember]
		public string ErrorMessage { get; set; }

		[DataMember]
		public AttachmentType Attachment { get; set; }

		[DataMember]
		public string ImageData { get; set; }

		[DataMember]
		public int ImageDataSize { get; set; }

		[DataMember]
		public string ImageDataType { get; set; }
	}
}
