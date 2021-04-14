using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Name = "NonIndexableItemDetailResult", Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType(TypeName = "NonIndexableItemDetailResultType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class NonIndexableItemDetailResult
	{
		[XmlArray(ElementName = "Items", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
		[XmlArrayItem(ElementName = "NonIndexableItemDetail", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", Type = typeof(NonIndexableItemDetail))]
		[DataMember(Name = "Items", EmitDefaultValue = false, IsRequired = false)]
		public NonIndexableItemDetail[] Items { get; set; }

		[DataMember(Name = "FailedMailboxes", EmitDefaultValue = false, IsRequired = false)]
		[XmlArray(ElementName = "FailedMailboxes", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
		[XmlArrayItem(ElementName = "FailedMailbox", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", Type = typeof(FailedSearchMailbox))]
		public FailedSearchMailbox[] FailedMailboxes { get; set; }
	}
}
