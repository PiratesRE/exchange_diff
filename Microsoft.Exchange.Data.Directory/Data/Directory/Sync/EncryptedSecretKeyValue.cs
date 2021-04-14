using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11")]
	[DebuggerStepThrough]
	[GeneratedCode("svcutil", "4.0.30319.17627")]
	[Serializable]
	public class EncryptedSecretKeyValue
	{
		[XmlElement(DataType = "base64Binary", Order = 0)]
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
