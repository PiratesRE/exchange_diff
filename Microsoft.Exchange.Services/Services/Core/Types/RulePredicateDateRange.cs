using System;
using System.Xml.Serialization;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Services.Core.DataConverter;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "RulePredicateDateRangeType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	public class RulePredicateDateRange
	{
		[XmlElement(Order = 0)]
		public string StartDateTime { get; set; }

		[XmlIgnore]
		public bool StartDateTimeSpecified { get; set; }

		[XmlElement(Order = 1)]
		public string EndDateTime { get; set; }

		[XmlIgnore]
		public bool EndDateTimeSpecified { get; set; }

		public RulePredicateDateRange()
		{
		}

		public RulePredicateDateRange(ExDateTime? startTime, ExDateTime? endTime)
		{
			if (startTime != null)
			{
				this.StartDateTime = ExDateTimeConverter.ToSoapHeaderTimeZoneRelatedXsdDateTime(startTime.Value);
				this.StartDateTimeSpecified = true;
			}
			if (endTime != null)
			{
				this.EndDateTime = ExDateTimeConverter.ToSoapHeaderTimeZoneRelatedXsdDateTime(endTime.Value);
				this.EndDateTimeSpecified = true;
			}
		}
	}
}
