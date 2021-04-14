using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[DataContract]
	public class PeopleIKnowRowNotificationPayload : NotificationPayloadBase
	{
		[DataMember]
		public Persona[] Personas { get; set; }

		[DataMember]
		public string PersonaEmailAdress { get; set; }

		[DataMember]
		public int PersonaUnreadCount { get; set; }
	}
}
