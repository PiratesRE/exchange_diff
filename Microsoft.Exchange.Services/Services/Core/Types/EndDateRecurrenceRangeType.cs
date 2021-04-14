using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange", Name = "EndDateRecurrence")]
	[Serializable]
	public class EndDateRecurrenceRangeType : RecurrenceRangeBaseType
	{
		[DataMember(EmitDefaultValue = false, IsRequired = true)]
		[XmlElement]
		[DateTimeString]
		public string EndDate { get; set; }
	}
}
