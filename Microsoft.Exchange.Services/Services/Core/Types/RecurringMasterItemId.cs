using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType(TypeName = "RecurringMasterItemIdType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	public class RecurringMasterItemId : BaseItemId
	{
		[XmlAttribute]
		[DataMember(IsRequired = true, Order = 1)]
		public string OccurrenceId { get; set; }

		[XmlAttribute]
		[DataMember(IsRequired = false, Order = 2)]
		public string ChangeKey { get; set; }

		public override string GetId()
		{
			return this.OccurrenceId;
		}

		public override string GetChangeKey()
		{
			return this.ChangeKey;
		}
	}
}
