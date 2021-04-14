using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[Serializable]
	public class PostReplyItemType : PostReplyItemBaseType
	{
		public BodyType NewBodyContent
		{
			get
			{
				return this.newBodyContentField;
			}
			set
			{
				this.newBodyContentField = value;
			}
		}

		private BodyType newBodyContentField;
	}
}
