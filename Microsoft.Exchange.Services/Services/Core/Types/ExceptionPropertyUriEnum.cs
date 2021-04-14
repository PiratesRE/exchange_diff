using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "ExceptionPropertyURIType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public enum ExceptionPropertyUriEnum
	{
		[XmlEnum("attachment:Name")]
		AttachmentName,
		[XmlEnum("attachment:ContentType")]
		ContentType,
		[XmlEnum("attachment:Content")]
		Content,
		[XmlEnum("recurrence:Month")]
		Month,
		[XmlEnum("recurrence:DayOfWeekIndex")]
		DayOfWeekIndex,
		[XmlEnum("recurrence:DaysOfWeek")]
		DaysOfWeek,
		[XmlEnum("recurrence:DayOfMonth")]
		DayOfMonth,
		[XmlEnum("recurrence:Interval")]
		Interval,
		[XmlEnum("recurrence:NumberOfOccurrences")]
		NumberOfOccurrences,
		[XmlEnum("timezone:Offset")]
		Offset
	}
}
