using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType(TypeName = "OccurrenceItemIdType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	public class OccurrenceItemId : BaseItemId
	{
		[DataMember(IsRequired = true, Order = 0)]
		[XmlAttribute]
		public string RecurringMasterId { get; set; }

		[DataMember(IsRequired = false, Order = 0)]
		[XmlAttribute]
		public string ChangeKey { get; set; }

		[DataMember(IsRequired = true, Order = 0)]
		[XmlAttribute]
		public int InstanceIndex { get; set; }

		public override string GetId()
		{
			return this.RecurringMasterId;
		}

		public override string GetChangeKey()
		{
			return this.ChangeKey;
		}
	}
}
