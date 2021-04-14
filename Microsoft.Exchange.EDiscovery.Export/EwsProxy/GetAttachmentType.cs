using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[Serializable]
	public class GetAttachmentType : BaseRequestType
	{
		public AttachmentResponseShapeType AttachmentShape
		{
			get
			{
				return this.attachmentShapeField;
			}
			set
			{
				this.attachmentShapeField = value;
			}
		}

		[XmlArrayItem("AttachmentId", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		public RequestAttachmentIdType[] AttachmentIds
		{
			get
			{
				return this.attachmentIdsField;
			}
			set
			{
				this.attachmentIdsField = value;
			}
		}

		private AttachmentResponseShapeType attachmentShapeField;

		private RequestAttachmentIdType[] attachmentIdsField;
	}
}
