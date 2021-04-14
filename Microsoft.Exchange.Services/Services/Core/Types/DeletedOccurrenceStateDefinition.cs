using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Services.Core.DataConverter;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "DeletedOccurrenceStateDefinitionType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public sealed class DeletedOccurrenceStateDefinition : BaseCalendarItemStateDefinition
	{
		[XmlElement]
		[DataMember(IsRequired = true, Order = 1)]
		public string OccurrenceDate { get; set; }

		[DataMember(IsRequired = false, Order = 2)]
		[XmlElement]
		public bool IsOccurrencePresent { get; set; }

		[XmlIgnore]
		public ExDateTime OccurrenceExDateTime
		{
			get
			{
				if (string.IsNullOrEmpty(this.OccurrenceDate))
				{
					return ExDateTime.MinValue;
				}
				return ExDateTimeConverter.ParseTimeZoneRelated(this.OccurrenceDate, EWSSettings.RequestTimeZone);
			}
		}
	}
}
