using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[DataContract]
	internal sealed class CalendarUserSettings : ItemPropertiesBase
	{
		[DataMember]
		public bool IsClock24Hour { get; set; }

		[DataMember]
		public string FirstDayOfWeek { get; set; }

		[DataMember]
		public string StartTimeOfDay { get; set; }

		[DataMember]
		public bool IsWeatherInCelsius { get; set; }
	}
}
