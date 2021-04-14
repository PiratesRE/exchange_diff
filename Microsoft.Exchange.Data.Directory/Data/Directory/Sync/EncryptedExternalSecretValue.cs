using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	[DebuggerStepThrough]
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11")]
	[DesignerCategory("code")]
	[GeneratedCode("svcutil", "4.0.30319.17627")]
	[Serializable]
	public class EncryptedExternalSecretValue
	{
		[XmlElement(DataType = "base64Binary", Order = 0)]
		public byte[] EncryptedExternalSecret
		{
			get
			{
				return this.encryptedExternalSecretField;
			}
			set
			{
				this.encryptedExternalSecretField = value;
			}
		}

		private byte[] encryptedExternalSecretField;
	}
}
