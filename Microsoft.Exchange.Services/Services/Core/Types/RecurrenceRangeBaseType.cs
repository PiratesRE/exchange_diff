using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlInclude(typeof(NumberedRecurrenceRangeType))]
	[XmlInclude(typeof(EndDateRecurrenceRangeType))]
	[XmlInclude(typeof(NoEndRecurrenceRangeType))]
	[KnownType(typeof(NoEndRecurrenceRangeType))]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[KnownType(typeof(NumberedRecurrenceRangeType))]
	[KnownType(typeof(EndDateRecurrenceRangeType))]
	[Serializable]
	public abstract class RecurrenceRangeBaseType
	{
		[DateTimeString]
		[XmlElement]
		[DataMember(EmitDefaultValue = false, IsRequired = true)]
		public string StartDate { get; set; }
	}
}
