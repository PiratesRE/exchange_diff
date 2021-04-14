using System;
using System.CodeDom.Compiler;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[Serializable]
	public enum ExceptionPropertyURIType
	{
		[XmlEnum("attachment:Name")]
		attachmentName,
		[XmlEnum("attachment:ContentType")]
		attachmentContentType,
		[XmlEnum("attachment:Content")]
		attachmentContent,
		[XmlEnum("recurrence:Month")]
		recurrenceMonth,
		[XmlEnum("recurrence:DayOfWeekIndex")]
		recurrenceDayOfWeekIndex,
		[XmlEnum("recurrence:DaysOfWeek")]
		recurrenceDaysOfWeek,
		[XmlEnum("recurrence:DayOfMonth")]
		recurrenceDayOfMonth,
		[XmlEnum("recurrence:Interval")]
		recurrenceInterval,
		[XmlEnum("recurrence:NumberOfOccurrences")]
		recurrenceNumberOfOccurrences,
		[XmlEnum("timezone:Offset")]
		timezoneOffset
	}
}
