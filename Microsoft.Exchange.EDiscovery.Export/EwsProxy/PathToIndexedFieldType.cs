using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[DesignerCategory("code")]
	[DebuggerStepThrough]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class PathToIndexedFieldType : BasePathToElementType
	{
		[XmlAttribute]
		public DictionaryURIType FieldURI
		{
			get
			{
				return this.fieldURIField;
			}
			set
			{
				this.fieldURIField = value;
			}
		}

		[XmlAttribute]
		public string FieldIndex
		{
			get
			{
				return this.fieldIndexField;
			}
			set
			{
				this.fieldIndexField = value;
			}
		}

		private DictionaryURIType fieldURIField;

		private string fieldIndexField;
	}
}
