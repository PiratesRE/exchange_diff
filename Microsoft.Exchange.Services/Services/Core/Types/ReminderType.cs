using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Services.Core.DataConverter;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange", Name = "Reminder")]
	[XmlType("Reminder", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class ReminderType
	{
		[DataMember(EmitDefaultValue = false, IsRequired = true, Order = 1)]
		public string Subject { get; set; }

		[DataMember(EmitDefaultValue = false, IsRequired = false, Order = 2)]
		public string Location { get; set; }

		[DataMember(EmitDefaultValue = false, IsRequired = true, Order = 3)]
		[DateTimeString]
		public string ReminderTime { get; set; }

		[DateTimeString]
		[DataMember(EmitDefaultValue = false, IsRequired = true, Order = 4)]
		public string StartDate { get; set; }

		[DateTimeString]
		[DataMember(EmitDefaultValue = false, IsRequired = true, Order = 5)]
		public string EndDate { get; set; }

		[DataMember(EmitDefaultValue = false, IsRequired = true, Order = 6)]
		public ItemId ItemId { get; set; }

		[DataMember(EmitDefaultValue = false, IsRequired = false, Order = 7)]
		public ItemId RecurringMasterItemId { get; set; }

		[DataMember(EmitDefaultValue = false, IsRequired = false, Order = 8)]
		public ReminderGroupType ReminderGroup { get; set; }

		[DataMember(EmitDefaultValue = false, IsRequired = true, Order = 9)]
		public string UID
		{
			get
			{
				if (this.uid != null)
				{
					return this.uid;
				}
				if (this.ItemId != null)
				{
					return this.ItemId.Id;
				}
				return null;
			}
			set
			{
				this.uid = value;
			}
		}

		[IgnoreDataMember]
		[XmlIgnore]
		public ExDateTime ReminderDateTime
		{
			get
			{
				return this.reminderDateTime;
			}
			set
			{
				this.reminderDateTime = value;
				this.ReminderTime = ExDateTimeConverter.ToSoapHeaderTimeZoneRelatedXsdDateTime(value);
			}
		}

		[IgnoreDataMember]
		[XmlIgnore]
		public ExDateTime StartDateTime
		{
			get
			{
				return this.startDateTime;
			}
			set
			{
				this.startDateTime = value;
				this.StartDate = ExDateTimeConverter.ToSoapHeaderTimeZoneRelatedXsdDateTime(value);
			}
		}

		[IgnoreDataMember]
		[XmlIgnore]
		public ExDateTime EndDateTime
		{
			get
			{
				return this.endDateTime;
			}
			set
			{
				this.endDateTime = value;
				this.EndDate = ExDateTimeConverter.ToSoapHeaderTimeZoneRelatedXsdDateTime(value);
			}
		}

		private ExDateTime reminderDateTime;

		private ExDateTime startDateTime;

		private ExDateTime endDateTime;

		private string uid;
	}
}
