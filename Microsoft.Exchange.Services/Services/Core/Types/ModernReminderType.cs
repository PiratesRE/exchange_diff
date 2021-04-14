using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Services.Core.DataConverter;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange", Name = "ModernReminderType")]
	[Serializable]
	public class ModernReminderType
	{
		[DataMember(Order = 1)]
		public Guid Id { get; set; }

		[IgnoreDataMember]
		public ReminderTimeHint ReminderTimeHint { get; set; }

		[DataMember(Name = "ReminderTimeHint", Order = 2)]
		[XmlIgnore]
		public string ReminderTimeHintString
		{
			get
			{
				return EnumUtilities.ToString<ReminderTimeHint>(this.ReminderTimeHint);
			}
			set
			{
				this.ReminderTimeHint = EnumUtilities.Parse<ReminderTimeHint>(value);
			}
		}

		[IgnoreDataMember]
		public Hours Hours { get; set; }

		[DataMember(Name = "Hours", Order = 3)]
		[XmlIgnore]
		public string HoursString
		{
			get
			{
				return EnumUtilities.ToString<Hours>(this.Hours);
			}
			set
			{
				this.Hours = EnumUtilities.Parse<Hours>(value);
			}
		}

		[IgnoreDataMember]
		public Priority Priority { get; set; }

		[DataMember(Name = "Priority", Order = 4)]
		[XmlIgnore]
		public string PriorityString
		{
			get
			{
				return EnumUtilities.ToString<Priority>(this.Priority);
			}
			set
			{
				this.Priority = EnumUtilities.Parse<Priority>(value);
			}
		}

		[DataMember(Order = 5)]
		public int Duration { get; set; }

		[IgnoreDataMember]
		public DateTime ReferenceTime { get; set; }

		[XmlIgnore]
		[DataMember(Name = "ReferenceTime", Order = 6)]
		[DateTimeString]
		public string ReferenceTimeString
		{
			get
			{
				ExDateTime dateTime = (ExDateTime)this.ReferenceTime;
				return ExDateTimeConverter.ToOffsetXsdDateTime(dateTime, dateTime.TimeZone);
			}
			set
			{
				this.ReferenceTime = (DateTime)ExDateTimeConverter.Parse(value);
			}
		}

		[IgnoreDataMember]
		public DateTime CustomReminderTime { get; set; }

		[DataMember(Name = "CustomReminderTime", Order = 7)]
		[DateTimeString]
		[XmlIgnore]
		public string CustomReminderTimeString
		{
			get
			{
				ExDateTime dateTime = (ExDateTime)this.CustomReminderTime;
				return ExDateTimeConverter.ToOffsetXsdDateTime(dateTime, dateTime.TimeZone);
			}
			set
			{
				this.CustomReminderTime = (DateTime)ExDateTimeConverter.Parse(value);
			}
		}

		[IgnoreDataMember]
		public DateTime DueDate { get; set; }

		[DateTimeString]
		[XmlIgnore]
		[DataMember(Name = "DueDate", Order = 8)]
		public string DueDateString
		{
			get
			{
				ExDateTime dateTime = (ExDateTime)this.DueDate;
				return ExDateTimeConverter.ToOffsetXsdDateTime(dateTime, dateTime.TimeZone);
			}
			set
			{
				this.DueDate = (DateTime)ExDateTimeConverter.Parse(value);
			}
		}
	}
}
