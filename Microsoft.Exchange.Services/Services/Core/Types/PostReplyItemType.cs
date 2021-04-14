using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DataContract(Name = "PostReplyItem", Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[Serializable]
	public class PostReplyItemType : PostReplyItemBaseType
	{
		[DataMember(EmitDefaultValue = false)]
		public BodyContentType NewBodyContent { get; set; }
	}
}
