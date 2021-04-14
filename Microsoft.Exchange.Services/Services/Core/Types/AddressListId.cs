using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType(TypeName = "AddressListIdType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	public class AddressListId : BaseFolderId
	{
		[DataMember(IsRequired = true, Order = 1)]
		[XmlAttribute]
		public string Id { get; set; }

		public AddressListId()
		{
		}

		public AddressListId(string id)
		{
			this.Id = id;
		}

		public override string GetId()
		{
			return this.Id;
		}
	}
}
