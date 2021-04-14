using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[XmlInclude(typeof(AlternatePublicFolderItemIdType))]
	[DebuggerStepThrough]
	[XmlInclude(typeof(AlternateIdType))]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[XmlInclude(typeof(AlternatePublicFolderIdType))]
	[Serializable]
	public abstract class AlternateIdBaseType
	{
		[XmlAttribute]
		public IdFormatType Format
		{
			get
			{
				return this.formatField;
			}
			set
			{
				this.formatField = value;
			}
		}

		private IdFormatType formatField;
	}
}
