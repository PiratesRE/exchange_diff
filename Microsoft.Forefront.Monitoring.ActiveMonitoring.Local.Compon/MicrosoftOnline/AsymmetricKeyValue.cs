using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.MicrosoftOnline
{
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11")]
	[GeneratedCode("wsdl", "2.0.50727.1432")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[Serializable]
	public class AsymmetricKeyValue
	{
		[XmlElement("AsymmetricKeyValue", DataType = "base64Binary")]
		public byte[] AsymmetricKeyValue1
		{
			get
			{
				return this.asymmetricKeyValue1Field;
			}
			set
			{
				this.asymmetricKeyValue1Field = value;
			}
		}

		[XmlAttribute]
		public string KeyIdentifier
		{
			get
			{
				return this.keyIdentifierField;
			}
			set
			{
				this.keyIdentifierField = value;
			}
		}

		[XmlAttribute]
		public int Version
		{
			get
			{
				return this.versionField;
			}
			set
			{
				this.versionField = value;
			}
		}

		[XmlAttribute]
		public AsymmetricKeyType AsymmetricKeyType
		{
			get
			{
				return this.asymmetricKeyTypeField;
			}
			set
			{
				this.asymmetricKeyTypeField = value;
			}
		}

		private byte[] asymmetricKeyValue1Field;

		private string keyIdentifierField;

		private int versionField;

		private AsymmetricKeyType asymmetricKeyTypeField;
	}
}
