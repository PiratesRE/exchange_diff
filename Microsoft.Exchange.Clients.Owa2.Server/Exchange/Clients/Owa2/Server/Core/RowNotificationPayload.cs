using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[DataContract]
	public class RowNotificationPayload : NotificationPayloadBase
	{
		[DataMember(EmitDefaultValue = false)]
		public ItemType Item { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public ConversationType Conversation { get; set; }

		[DataMember]
		public string Prior { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public bool Reload { get; set; }

		[DataMember]
		public string FolderId { get; set; }
	}
}
