using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange", Name = "InboxReminderType")]
	[Serializable]
	public class InboxReminderType
	{
		[DataMember(Order = 1)]
		public Guid Id { get; set; }

		[DataMember(Order = 2)]
		public int ReminderOffset { get; set; }

		[DataMember(Order = 3)]
		public string Message { get; set; }

		[DataMember(Order = 4)]
		public bool IsOrganizerReminder { get; set; }

		[DataMember(Order = 5)]
		public EmailReminderChangeType OccurrenceChange { get; set; }
	}
}
