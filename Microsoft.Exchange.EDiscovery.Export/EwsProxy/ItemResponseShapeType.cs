using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[DebuggerStepThrough]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DesignerCategory("code")]
	[Serializable]
	public class ItemResponseShapeType
	{
		public DefaultShapeNamesType BaseShape
		{
			get
			{
				return this.baseShapeField;
			}
			set
			{
				this.baseShapeField = value;
			}
		}

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

		public BodyTypeResponseType UniqueBodyType
		{
			get
			{
				return this.uniqueBodyTypeField;
			}
			set
			{
				this.uniqueBodyTypeField = value;
			}
		}

		[XmlIgnore]
		public bool UniqueBodyTypeSpecified
		{
			get
			{
				return this.uniqueBodyTypeFieldSpecified;
			}
			set
			{
				this.uniqueBodyTypeFieldSpecified = value;
			}
		}

		public BodyTypeResponseType NormalizedBodyType
		{
			get
			{
				return this.normalizedBodyTypeField;
			}
			set
			{
				this.normalizedBodyTypeField = value;
			}
		}

		[XmlIgnore]
		public bool NormalizedBodyTypeSpecified
		{
			get
			{
				return this.normalizedBodyTypeFieldSpecified;
			}
			set
			{
				this.normalizedBodyTypeFieldSpecified = value;
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

		public bool ConvertHtmlCodePageToUTF8
		{
			get
			{
				return this.convertHtmlCodePageToUTF8Field;
			}
			set
			{
				this.convertHtmlCodePageToUTF8Field = value;
			}
		}

		[XmlIgnore]
		public bool ConvertHtmlCodePageToUTF8Specified
		{
			get
			{
				return this.convertHtmlCodePageToUTF8FieldSpecified;
			}
			set
			{
				this.convertHtmlCodePageToUTF8FieldSpecified = value;
			}
		}

		public string InlineImageUrlTemplate
		{
			get
			{
				return this.inlineImageUrlTemplateField;
			}
			set
			{
				this.inlineImageUrlTemplateField = value;
			}
		}

		public bool BlockExternalImages
		{
			get
			{
				return this.blockExternalImagesField;
			}
			set
			{
				this.blockExternalImagesField = value;
			}
		}

		[XmlIgnore]
		public bool BlockExternalImagesSpecified
		{
			get
			{
				return this.blockExternalImagesFieldSpecified;
			}
			set
			{
				this.blockExternalImagesFieldSpecified = value;
			}
		}

		public bool AddBlankTargetToLinks
		{
			get
			{
				return this.addBlankTargetToLinksField;
			}
			set
			{
				this.addBlankTargetToLinksField = value;
			}
		}

		[XmlIgnore]
		public bool AddBlankTargetToLinksSpecified
		{
			get
			{
				return this.addBlankTargetToLinksFieldSpecified;
			}
			set
			{
				this.addBlankTargetToLinksFieldSpecified = value;
			}
		}

		public int MaximumBodySize
		{
			get
			{
				return this.maximumBodySizeField;
			}
			set
			{
				this.maximumBodySizeField = value;
			}
		}

		[XmlIgnore]
		public bool MaximumBodySizeSpecified
		{
			get
			{
				return this.maximumBodySizeFieldSpecified;
			}
			set
			{
				this.maximumBodySizeFieldSpecified = value;
			}
		}

		[XmlArrayItem("ExtendedFieldURI", typeof(PathToExtendedFieldType), IsNullable = false)]
		[XmlArrayItem("IndexedFieldURI", typeof(PathToIndexedFieldType), IsNullable = false)]
		[XmlArrayItem("FieldURI", typeof(PathToUnindexedFieldType), IsNullable = false)]
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

		private DefaultShapeNamesType baseShapeField;

		private bool includeMimeContentField;

		private bool includeMimeContentFieldSpecified;

		private BodyTypeResponseType bodyTypeField;

		private bool bodyTypeFieldSpecified;

		private BodyTypeResponseType uniqueBodyTypeField;

		private bool uniqueBodyTypeFieldSpecified;

		private BodyTypeResponseType normalizedBodyTypeField;

		private bool normalizedBodyTypeFieldSpecified;

		private bool filterHtmlContentField;

		private bool filterHtmlContentFieldSpecified;

		private bool convertHtmlCodePageToUTF8Field;

		private bool convertHtmlCodePageToUTF8FieldSpecified;

		private string inlineImageUrlTemplateField;

		private bool blockExternalImagesField;

		private bool blockExternalImagesFieldSpecified;

		private bool addBlankTargetToLinksField;

		private bool addBlankTargetToLinksFieldSpecified;

		private int maximumBodySizeField;

		private bool maximumBodySizeFieldSpecified;

		private BasePathToElementType[] additionalPropertiesField;
	}
}
