using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "DeleteFolderFieldType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DataContract(Name = "DeleteFolderField", Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class DeleteFolderPropertyUpdate : DeletePropertyUpdate
	{
	}
}
