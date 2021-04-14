using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "AttachmentFieldURIType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public enum AttachmentUriEnum
	{
		[XmlEnum("item:Attachment")]
		Attachment,
		[XmlEnum("item:AttachmentContent")]
		AttachmentContent
	}
}
