using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange", Name = "ItemId")]
	[XmlType(TypeName = "ItemIdType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	public class ItemId : BaseItemId
	{
		[XmlAttribute]
		[DataMember(IsRequired = true, Order = 0)]
		public string Id { get; set; }

		[DataMember(IsRequired = false, EmitDefaultValue = false, Order = 0)]
		[XmlAttribute]
		public string ChangeKey { get; set; }

		public ItemId()
		{
		}

		public ItemId(string id, string changeKey)
		{
			this.Id = id;
			this.ChangeKey = changeKey;
		}

		public override string GetId()
		{
			return this.Id;
		}

		public override string GetChangeKey()
		{
			return this.ChangeKey;
		}
	}
}
