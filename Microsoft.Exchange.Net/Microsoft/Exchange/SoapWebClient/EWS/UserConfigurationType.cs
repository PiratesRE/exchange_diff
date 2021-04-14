using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[Serializable]
	public class UserConfigurationType
	{
		public UserConfigurationNameType UserConfigurationName;

		public ItemIdType ItemId;

		[XmlArrayItem("DictionaryEntry", IsNullable = false)]
		public UserConfigurationDictionaryEntryType[] Dictionary;

		[XmlElement(DataType = "base64Binary")]
		public byte[] XmlData;

		[XmlElement(DataType = "base64Binary")]
		public byte[] BinaryData;
	}
}
