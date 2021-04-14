using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.MicrosoftOnline
{
	[GeneratedCode("wsdl", "2.0.50727.1432")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11")]
	[Serializable]
	public class AlternativeSecurityIdValue
	{
		[XmlElement(DataType = "base64Binary")]
		public byte[] Key
		{
			get
			{
				return this.keyField;
			}
			set
			{
				this.keyField = value;
			}
		}

		[XmlAttribute]
		public int Type
		{
			get
			{
				return this.typeField;
			}
			set
			{
				this.typeField = value;
			}
		}

		[XmlAttribute]
		public string IdentityProvider
		{
			get
			{
				return this.identityProviderField;
			}
			set
			{
				this.identityProviderField = value;
			}
		}

		private byte[] keyField;

		private int typeField;

		private string identityProviderField;
	}
}
