using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.MicrosoftOnline
{
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11")]
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "2.0.50727.1432")]
	[DebuggerStepThrough]
	[Serializable]
	public class EncryptedSecretKeyValue
	{
		[XmlElement(DataType = "base64Binary")]
		public byte[] EncryptedSecret
		{
			get
			{
				return this.encryptedSecretField;
			}
			set
			{
				this.encryptedSecretField = value;
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
		public SecretKeyType SecretKeyType
		{
			get
			{
				return this.secretKeyTypeField;
			}
			set
			{
				this.secretKeyTypeField = value;
			}
		}

		private byte[] encryptedSecretField;

		private string keyIdentifierField;

		private int versionField;

		private SecretKeyType secretKeyTypeField;
	}
}
