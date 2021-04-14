using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class ModernAttachment
	{
		[DataMember]
		public ModernAttachment.AttachmentInfo Info { get; set; }

		[DataMember]
		public ModernAttachment.AttachmentData Data { get; set; }

		public class AttachmentInfo
		{
			[DataMember]
			public int Index { get; set; }

			[DataMember]
			public string[] Path { get; set; }
		}

		public class AttachmentData
		{
			[DataMember]
			public ItemType Item { get; set; }

			[DataMember]
			public AttachmentType Attachment { get; set; }

			[DataMember]
			public AttachmentTypeEx AttachmentEx { get; set; }
		}
	}
}
