using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "AttachmentResponseShapeType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DataContract(Name = "AttachmentResponseShapeType", Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[Serializable]
	public class AttachmentResponseShape : ItemResponseShape
	{
		public AttachmentResponseShape()
		{
			base.BaseShape = ShapeEnum.AllProperties;
		}
	}
}
