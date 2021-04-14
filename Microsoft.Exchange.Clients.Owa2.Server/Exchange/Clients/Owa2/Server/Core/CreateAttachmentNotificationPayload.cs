using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[DataContract]
	public class CreateAttachmentNotificationPayload : NotificationPayloadBase
	{
		[DataMember]
		public string Id { get; set; }

		[DataMember]
		public AttachmentResultCode ResultCode { get; set; }

		[DataMember]
		public FileAttachmentDataProviderItem Item { get; set; }

		[DataMember]
		public CreateAttachmentResponse Response { get; set; }

		public byte[] Bytes { get; set; }
	}
}
