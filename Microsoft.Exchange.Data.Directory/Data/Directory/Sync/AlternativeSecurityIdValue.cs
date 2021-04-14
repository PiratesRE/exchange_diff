using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	[GeneratedCode("svcutil", "4.0.30319.17627")]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11")]
	[DebuggerStepThrough]
	[Serializable]
	public class AlternativeSecurityIdValue
	{
		[XmlElement(DataType = "base64Binary", Order = 0)]
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
