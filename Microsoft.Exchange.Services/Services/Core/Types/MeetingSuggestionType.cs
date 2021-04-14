using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Services.Core.DataConverter;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[Serializable]
	public class MeetingSuggestionType : BaseEntityType
	{
		[DataMember(IsRequired = false, EmitDefaultValue = false)]
		[XmlArrayItem(ElementName = "EmailUser", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", Type = typeof(EmailUserType))]
		public EmailUserType[] Attendees { get; set; }

		[DataMember(IsRequired = false, EmitDefaultValue = false)]
		public string Location { get; set; }

		[DataMember(IsRequired = false, EmitDefaultValue = false)]
		public string Subject { get; set; }

		[DataMember(IsRequired = false, EmitDefaultValue = false)]
		public string MeetingString { get; set; }

		[IgnoreDataMember]
		public DateTime StartTime { get; set; }

		[XmlIgnore]
		[DataMember(Name = "StartTime", IsRequired = false, EmitDefaultValue = false)]
		[DateTimeString]
		public string StartTimeString
		{
			get
			{
				ExDateTime dateTime = (ExDateTime)this.StartTime;
				return ExDateTimeConverter.ToOffsetXsdDateTime(dateTime, dateTime.TimeZone);
			}
			set
			{
				this.StartTime = (DateTime)ExDateTimeConverter.Parse(value);
			}
		}

		[IgnoreDataMember]
		public DateTime EndTime { get; set; }

		[XmlIgnore]
		[DateTimeString]
		[DataMember(Name = "EndTime", IsRequired = false, EmitDefaultValue = false)]
		public string EndTimeString
		{
			get
			{
				ExDateTime dateTime = (ExDateTime)this.EndTime;
				return ExDateTimeConverter.ToOffsetXsdDateTime(dateTime, dateTime.TimeZone);
			}
			set
			{
				this.EndTime = (DateTime)ExDateTimeConverter.Parse(value);
			}
		}
	}
}
