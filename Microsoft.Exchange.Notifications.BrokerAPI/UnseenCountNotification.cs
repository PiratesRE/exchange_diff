using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Notifications.Broker
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[KnownType(typeof(UnseenDataType))]
	[Serializable]
	public class UnseenCountNotification : ApplicationNotification
	{
		public UnseenCountNotification() : base(NotificationType.UnseenCount)
		{
		}

		[DataMember(EmitDefaultValue = false)]
		public UnseenDataType UnseenData { get; set; }
	}
}
