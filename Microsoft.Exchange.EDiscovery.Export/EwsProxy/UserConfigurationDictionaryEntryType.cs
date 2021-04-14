using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DesignerCategory("code")]
	[DebuggerStepThrough]
	[Serializable]
	public class UserConfigurationDictionaryEntryType
	{
		public UserConfigurationDictionaryObjectType DictionaryKey
		{
			get
			{
				return this.dictionaryKeyField;
			}
			set
			{
				this.dictionaryKeyField = value;
			}
		}

		[XmlElement(IsNullable = true)]
		public UserConfigurationDictionaryObjectType DictionaryValue
		{
			get
			{
				return this.dictionaryValueField;
			}
			set
			{
				this.dictionaryValueField = value;
			}
		}

		private UserConfigurationDictionaryObjectType dictionaryKeyField;

		private UserConfigurationDictionaryObjectType dictionaryValueField;
	}
}
