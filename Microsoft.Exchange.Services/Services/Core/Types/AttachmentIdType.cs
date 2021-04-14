using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange", Name = "AttachmentId")]
	[XmlType(TypeName = "AttachmentIdType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	public class AttachmentIdType : BaseItemId
	{
		[DataMember(IsRequired = true, Order = 1)]
		[XmlAttribute]
		public string Id { get; set; }

		[DataMember(EmitDefaultValue = false, IsRequired = false)]
		[XmlAttribute]
		public string RootItemId { get; set; }

		[DataMember(EmitDefaultValue = false, IsRequired = false)]
		[XmlAttribute]
		public string RootItemChangeKey { get; set; }

		public AttachmentIdType()
		{
		}

		public AttachmentIdType(string id)
		{
			this.Id = id;
		}

		public override string GetId()
		{
			return this.Id;
		}
	}
}
