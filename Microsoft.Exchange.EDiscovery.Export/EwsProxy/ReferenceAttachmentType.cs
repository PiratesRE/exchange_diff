using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[DebuggerStepThrough]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class ReferenceAttachmentType : AttachmentType
	{
		public string AttachLongPathName
		{
			get
			{
				return this.attachLongPathNameField;
			}
			set
			{
				this.attachLongPathNameField = value;
			}
		}

		private string attachLongPathNameField;
	}
}
