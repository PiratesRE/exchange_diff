using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[Serializable]
	public class ConnectingSIDType
	{
		[XmlElement("SID", typeof(SIDType))]
		[XmlElement("PrimarySmtpAddress", typeof(PrimarySmtpAddressType))]
		[XmlElement("PrincipalName", typeof(PrincipalNameType))]
		[XmlElement("SmtpAddress", typeof(SmtpAddressType))]
		public object Item
		{
			get
			{
				return this.itemField;
			}
			set
			{
				this.itemField = value;
			}
		}

		private object itemField;
	}
}
