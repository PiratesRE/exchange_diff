using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Web.Services.Protocols;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.AutoDiscoverProxy
{
	[GeneratedCode("wsdl", "4.0.30319.1")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://www.w3.org/2005/08/addressing")]
	[XmlRoot("To", Namespace = "http://www.w3.org/2005/08/addressing", IsNullable = false)]
	[Serializable]
	public class To : SoapHeader
	{
		[XmlText]
		public string Text
		{
			get
			{
				return this.textField;
			}
			set
			{
				this.textField = value;
			}
		}

		private string textField;
	}
}
