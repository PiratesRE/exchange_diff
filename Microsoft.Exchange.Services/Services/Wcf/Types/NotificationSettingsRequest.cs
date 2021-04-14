using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class NotificationSettingsRequest
	{
		[DataMember]
		public bool EnableReminders { get; set; }

		[DataMember]
		public bool EnableReminderSound { get; set; }

		[DataMember]
		public int NewItemNotify { get; set; }
	}
}
