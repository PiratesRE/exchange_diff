using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[Serializable]
	public class AttachmentResponseShapeType
	{
		public bool IncludeMimeContent
		{
			get
			{
				return this.includeMimeContentField;
			}
			set
			{
				this.includeMimeContentField = value;
			}
		}

		[XmlIgnore]
		public bool IncludeMimeContentSpecified
		{
			get
			{
				return this.includeMimeContentFieldSpecified;
			}
			set
			{
				this.includeMimeContentFieldSpecified = value;
			}
		}

		public BodyTypeResponseType BodyType
		{
			get
			{
				return this.bodyTypeField;
			}
			set
			{
				this.bodyTypeField = value;
			}
		}

		[XmlIgnore]
		public bool BodyTypeSpecified
		{
			get
			{
				return this.bodyTypeFieldSpecified;
			}
			set
			{
				this.bodyTypeFieldSpecified = value;
			}
		}

		public bool FilterHtmlContent
		{
			get
			{
				return this.filterHtmlContentField;
			}
			set
			{
				this.filterHtmlContentField = value;
			}
		}

		[XmlIgnore]
		public bool FilterHtmlContentSpecified
		{
			get
			{
				return this.filterHtmlContentFieldSpecified;
			}
			set
			{
				this.filterHtmlContentFieldSpecified = value;
			}
		}

		[XmlArrayItem("FieldURI", typeof(PathToUnindexedFieldType), IsNullable = false)]
		[XmlArrayItem("IndexedFieldURI", typeof(PathToIndexedFieldType), IsNullable = false)]
		[XmlArrayItem("ExtendedFieldURI", typeof(PathToExtendedFieldType), IsNullable = false)]
		[XmlArrayItem("Path", IsNullable = false)]
		public BasePathToElementType[] AdditionalProperties
		{
			get
			{
				return this.additionalPropertiesField;
			}
			set
			{
				this.additionalPropertiesField = value;
			}
		}

		private bool includeMimeContentField;

		private bool includeMimeContentFieldSpecified;

		private BodyTypeResponseType bodyTypeField;

		private bool bodyTypeFieldSpecified;

		private bool filterHtmlContentField;

		private bool filterHtmlContentFieldSpecified;

		private BasePathToElementType[] additionalPropertiesField;
	}
}
