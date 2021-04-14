using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[Serializable]
	public class UserConfigurationType
	{
		public UserConfigurationNameType UserConfigurationName
		{
			get
			{
				return this.userConfigurationNameField;
			}
			set
			{
				this.userConfigurationNameField = value;
			}
		}

		public ItemIdType ItemId
		{
			get
			{
				return this.itemIdField;
			}
			set
			{
				this.itemIdField = value;
			}
		}

		[XmlArrayItem("DictionaryEntry", IsNullable = false)]
		public UserConfigurationDictionaryEntryType[] Dictionary
		{
			get
			{
				return this.dictionaryField;
			}
			set
			{
				this.dictionaryField = value;
			}
		}

		[XmlElement(DataType = "base64Binary")]
		public byte[] XmlData
		{
			get
			{
				return this.xmlDataField;
			}
			set
			{
				this.xmlDataField = value;
			}
		}

		[XmlElement(DataType = "base64Binary")]
		public byte[] BinaryData
		{
			get
			{
				return this.binaryDataField;
			}
			set
			{
				this.binaryDataField = value;
			}
		}

		private UserConfigurationNameType userConfigurationNameField;

		private ItemIdType itemIdField;

		private UserConfigurationDictionaryEntryType[] dictionaryField;

		private byte[] xmlDataField;

		private byte[] binaryDataField;
	}
}
