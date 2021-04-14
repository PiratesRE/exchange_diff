using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Name = "DeleteItemField", Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType(TypeName = "DeleteItemFieldType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	public class DeleteItemPropertyUpdate : DeletePropertyUpdate
	{
	}
}
