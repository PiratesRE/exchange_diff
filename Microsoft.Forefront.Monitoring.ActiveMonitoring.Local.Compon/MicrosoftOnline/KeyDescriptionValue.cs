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
	public class KeyDescriptionValue
	{
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
		public KeyType KeyType
		{
			get
			{
				return this.keyTypeField;
			}
			set
			{
				this.keyTypeField = value;
			}
		}

		[XmlAttribute]
		public KeyUsage KeyUsage
		{
			get
			{
				return this.keyUsageField;
			}
			set
			{
				this.keyUsageField = value;
			}
		}

		[XmlAttribute]
		public DateTime StartTimestamp
		{
			get
			{
				return this.startTimestampField;
			}
			set
			{
				this.startTimestampField = value;
			}
		}

		[XmlAttribute]
		public DateTime EndTimestamp
		{
			get
			{
				return this.endTimestampField;
			}
			set
			{
				this.endTimestampField = value;
			}
		}

		[XmlAttribute(DataType = "base64Binary")]
		public byte[] ApplicationKeyIdentifier
		{
			get
			{
				return this.applicationKeyIdentifierField;
			}
			set
			{
				this.applicationKeyIdentifierField = value;
			}
		}

		private string keyIdentifierField;

		private int versionField;

		private KeyType keyTypeField;

		private KeyUsage keyUsageField;

		private DateTime startTimestampField;

		private DateTime endTimestampField;

		private byte[] applicationKeyIdentifierField;
	}
}
