using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Services.Core.DataConverter;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", TypeName = "ReminderMessageDataType")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[Serializable]
	public class ReminderMessageDataType
	{
		public ReminderMessageDataType()
		{
		}

		internal ReminderMessageDataType(ReminderMessage reminderMessage)
		{
			this.ReminderText = reminderMessage.GetValueOrDefault<string>(ReminderMessageSchema.ReminderText);
			this.Location = reminderMessage.GetValueOrDefault<string>(CalendarItemBaseSchema.Location);
			ExDateTime valueOrDefault = reminderMessage.GetValueOrDefault<ExDateTime>(ReminderMessageSchema.ReminderStartTime);
			this.StartTime = ExDateTimeConverter.ToSoapHeaderTimeZoneRelatedXsdDateTime(valueOrDefault);
			ExDateTime valueOrDefault2 = reminderMessage.GetValueOrDefault<ExDateTime>(ReminderMessageSchema.ReminderEndTime);
			this.EndTime = ExDateTimeConverter.ToSoapHeaderTimeZoneRelatedXsdDateTime(valueOrDefault2);
			CalendarItemBase cachedCorrelatedOccurrence = reminderMessage.GetCachedCorrelatedOccurrence();
			if (cachedCorrelatedOccurrence != null)
			{
				IdAndSession idAndSession = new IdAndSession(cachedCorrelatedOccurrence.Id, cachedCorrelatedOccurrence.Session);
				ConcatenatedIdAndChangeKey concatenatedId = IdConverter.GetConcatenatedId(idAndSession.Id, idAndSession, null);
				this.AssociatedCalendarItemId = new ItemId(concatenatedId.Id, concatenatedId.ChangeKey);
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 1)]
		public string ReminderText { get; set; }

		[DataMember(EmitDefaultValue = false, Order = 2)]
		public string Location { get; set; }

		[DataMember(EmitDefaultValue = false, Order = 3)]
		[DateTimeString]
		public string StartTime { get; set; }

		[DateTimeString]
		[DataMember(EmitDefaultValue = false, Order = 4)]
		public string EndTime { get; set; }

		[DataMember(EmitDefaultValue = false, Order = 5)]
		public ItemId AssociatedCalendarItemId { get; set; }
	}
}
