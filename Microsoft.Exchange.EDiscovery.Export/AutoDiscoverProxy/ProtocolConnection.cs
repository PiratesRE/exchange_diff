using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.AutoDiscoverProxy
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/2010/Autodiscover")]
	[Serializable]
	public class ProtocolConnection
	{
		[XmlElement(IsNullable = true)]
		public string Hostname
		{
			get
			{
				return this.hostnameField;
			}
			set
			{
				this.hostnameField = value;
			}
		}

		public int Port
		{
			get
			{
				return this.portField;
			}
			set
			{
				this.portField = value;
			}
		}

		[XmlElement(IsNullable = true)]
		public string EncryptionMethod
		{
			get
			{
				return this.encryptionMethodField;
			}
			set
			{
				this.encryptionMethodField = value;
			}
		}

		private string hostnameField;

		private int portField;

		private string encryptionMethodField;
	}
}
