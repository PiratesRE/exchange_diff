using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[Serializable]
	public class ExtendedPropertyType
	{
		public PathToExtendedFieldType ExtendedFieldURI
		{
			get
			{
				return this.extendedFieldURIField;
			}
			set
			{
				this.extendedFieldURIField = value;
			}
		}

		[XmlElement("Value", typeof(string))]
		[XmlElement("Values", typeof(NonEmptyArrayOfPropertyValuesType))]
		public object Item
		{
			get
			{
				return this.itemField;
			}
			set
			{
				this.itemField = value;
			}
		}

		private PathToExtendedFieldType extendedFieldURIField;

		private object itemField;
	}
}
