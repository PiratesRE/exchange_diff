using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[DataContract]
	internal class GroupAssociationNotificationPayload : NotificationPayloadBase
	{
		[DataMember]
		public ModernGroupType Group { get; set; }
	}
}
