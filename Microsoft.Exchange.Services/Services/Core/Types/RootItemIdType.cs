using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange", Name = "RootItemId")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", TypeName = "RootItemIdType")]
	[Serializable]
	public class RootItemIdType : BaseItemId
	{
		[XmlAttribute]
		[DataMember(EmitDefaultValue = false)]
		public string RootItemId { get; set; }

		[DataMember(EmitDefaultValue = false)]
		[XmlAttribute]
		public string RootItemChangeKey { get; set; }

		public RootItemIdType()
		{
		}

		public RootItemIdType(string id, string changeKey)
		{
			this.RootItemId = id;
			this.RootItemChangeKey = changeKey;
		}

		public override string GetId()
		{
			return this.RootItemId;
		}

		public override string GetChangeKey()
		{
			return this.RootItemChangeKey;
		}
	}
}
