using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType(TypeName = "ItemResponseShapeType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class ItemResponseShape : ResponseShape
	{
		public ItemResponseShape()
		{
			this.Init();
		}

		public ItemResponseShape(ItemResponseShape source)
		{
			base.BaseShape = source.BaseShape;
			this.IncludeMimeContent = source.IncludeMimeContent;
			this.BodyType = source.bodyType;
			this.UniqueBodyType = source.UniqueBodyType;
			this.NormalizedBodyType = source.NormalizedBodyType;
			this.FilterHtmlContent = source.FilterHtmlContent;
			this.ConvertHtmlCodePageToUTF8 = source.ConvertHtmlCodePageToUTF8;
			this.InlineImageUrlTemplate = source.InlineImageUrlTemplate;
			this.InlineImageUrlOnLoadTemplate = source.InlineImageUrlOnLoadTemplate;
			this.InlineImageCustomDataTemplate = source.InlineImageCustomDataTemplate;
			this.BlockExternalImages = source.BlockExternalImages;
			this.BlockExternalImagesIfSenderUntrusted = source.BlockExternalImagesIfSenderUntrusted;
			this.AddBlankTargetToLinks = source.AddBlankTargetToLinks;
			this.CssScopeClassName = source.CssScopeClassName;
			this.ClientSupportsIrm = source.ClientSupportsIrm;
			this.MaximumBodySize = source.MaximumBodySize;
			this.InferenceEnabled = source.InferenceEnabled;
			this.ShouldUseNarrowGapForPTagHtmlToTextConversion = source.ShouldUseNarrowGapForPTagHtmlToTextConversion;
			this.MaximumRecipientsToReturn = source.MaximumRecipientsToReturn;
			this.CalculateAttachmentInlineProps = source.CalculateAttachmentInlineProps;
			this.SeparateQuotedTextFromBody = source.SeparateQuotedTextFromBody;
			this.UseSafeHtml = source.UseSafeHtml;
			if (source.AdditionalProperties != null && source.AdditionalProperties.Length > 0)
			{
				base.AdditionalProperties = new PropertyPath[source.AdditionalProperties.Length];
				Array.Copy(source.AdditionalProperties, base.AdditionalProperties, source.AdditionalProperties.Length);
			}
		}

		internal ItemResponseShape(ShapeEnum baseShape, BodyResponseType bodyType, bool includeMimeContent, PropertyPath[] additionalProperties) : base(baseShape, additionalProperties)
		{
			this.convertHtmlCodePageToUTF8 = true;
			this.IncludeMimeContent = includeMimeContent;
			this.bodyType = bodyType;
		}

		private void Init()
		{
			this.bodyType = BodyResponseType.Best;
			this.uniqueBodyType = null;
			this.normalizedBodyType = null;
			this.convertHtmlCodePageToUTF8 = true;
			this.CalculateAttachmentInlineProps = false;
		}

		[DataMember(IsRequired = false, Order = 1)]
		[DefaultValue(false)]
		[XmlElement]
		public bool IncludeMimeContent { get; set; }

		[IgnoreDataMember]
		[DefaultValue(BodyResponseType.Best)]
		[XmlElement("BodyType")]
		public BodyResponseType BodyType
		{
			get
			{
				return this.bodyType;
			}
			set
			{
				this.bodyType = value;
			}
		}

		[XmlIgnore]
		[DataMember(Name = "BodyType", IsRequired = false)]
		public string BodyTypeString
		{
			get
			{
				return EnumUtilities.ToString<BodyResponseType>(this.bodyType);
			}
			set
			{
				this.bodyType = EnumUtilities.Parse<BodyResponseType>(value);
			}
		}

		[IgnoreDataMember]
		[XmlElement("UniqueBodyType")]
		public BodyResponseType UniqueBodyType
		{
			get
			{
				BodyResponseType? bodyResponseType = this.uniqueBodyType;
				if (bodyResponseType == null)
				{
					return this.BodyType;
				}
				return bodyResponseType.GetValueOrDefault();
			}
			set
			{
				this.uniqueBodyType = new BodyResponseType?(value);
			}
		}

		[XmlIgnore]
		[DataMember(Name = "UniqueBodyType", IsRequired = false)]
		public string UniqueBodyTypeString
		{
			get
			{
				return EnumUtilities.ToString<BodyResponseType>(this.UniqueBodyType);
			}
			set
			{
				this.uniqueBodyType = ((!string.IsNullOrEmpty(value)) ? new BodyResponseType?(EnumUtilities.Parse<BodyResponseType>(value)) : null);
			}
		}

		[IgnoreDataMember]
		[XmlElement("NormalizedBodyType")]
		public BodyResponseType NormalizedBodyType
		{
			get
			{
				BodyResponseType? bodyResponseType = this.normalizedBodyType;
				if (bodyResponseType == null)
				{
					return this.BodyType;
				}
				return bodyResponseType.GetValueOrDefault();
			}
			set
			{
				this.normalizedBodyType = new BodyResponseType?(value);
			}
		}

		[DataMember(Name = "NormalizedBodyType", IsRequired = false)]
		[XmlIgnore]
		public string NormalizedBodyTypeString
		{
			get
			{
				return EnumUtilities.ToString<BodyResponseType>(this.NormalizedBodyType);
			}
			set
			{
				this.normalizedBodyType = ((!string.IsNullOrEmpty(value)) ? new BodyResponseType?(EnumUtilities.Parse<BodyResponseType>(value)) : null);
			}
		}

		[DefaultValue(false)]
		[DataMember(IsRequired = false)]
		[XmlElement]
		public bool FilterHtmlContent { get; set; }

		[DefaultValue(true)]
		[DataMember(IsRequired = false)]
		[XmlElement]
		public bool ConvertHtmlCodePageToUTF8
		{
			get
			{
				return this.convertHtmlCodePageToUTF8;
			}
			set
			{
				this.convertHtmlCodePageToUTF8 = value;
			}
		}

		[DataMember(IsRequired = false)]
		[XmlElement]
		public string InlineImageUrlTemplate { get; set; }

		[DataMember(IsRequired = false)]
		[XmlElement]
		public string InlineImageUrlOnLoadTemplate { get; set; }

		[XmlElement]
		[DataMember(IsRequired = false)]
		public string InlineImageCustomDataTemplate { get; set; }

		[XmlElement]
		[DataMember(IsRequired = false)]
		public bool BlockExternalImages { get; set; }

		[XmlIgnore]
		[DataMember(IsRequired = false)]
		public bool BlockExternalImagesIfSenderUntrusted { get; set; }

		[XmlElement]
		[DataMember(IsRequired = false)]
		public bool AddBlankTargetToLinks { get; set; }

		[DataMember(IsRequired = false)]
		[XmlIgnore]
		public bool ClientSupportsIrm { get; set; }

		[XmlElement]
		[DataMember(IsRequired = false)]
		public int MaximumBodySize { get; set; }

		[XmlIgnore]
		[DataMember(IsRequired = false)]
		public bool InferenceEnabled { get; set; }

		[XmlIgnore]
		[DataMember(IsRequired = false)]
		public string ConversationShapeName { get; set; }

		[XmlIgnore]
		[DataMember(IsRequired = false)]
		public TargetFolderId ConversationFolderId { get; set; }

		[DataMember(IsRequired = false)]
		[XmlIgnore]
		public bool ShouldUseNarrowGapForPTagHtmlToTextConversion { get; set; }

		[DataMember(IsRequired = false)]
		[XmlIgnore]
		public int MaximumRecipientsToReturn { get; set; }

		[DataMember(IsRequired = false)]
		[XmlIgnore]
		public bool CalculateAttachmentInlineProps { get; set; }

		[DataMember(IsRequired = false)]
		[XmlIgnore]
		public string CssScopeClassName { get; set; }

		[XmlIgnore]
		[DataMember(IsRequired = false)]
		public bool SeparateQuotedTextFromBody { get; set; }

		[DataMember(IsRequired = false)]
		[XmlIgnore]
		public bool UseSafeHtml
		{
			get
			{
				return Global.SafeHtmlLoaded && this.useSafeHtml;
			}
			set
			{
				this.useSafeHtml = value;
			}
		}

		public bool CanCreateNormalizedBodyServiceObject
		{
			get
			{
				return !this.UseSafeHtml && (string.IsNullOrEmpty(this.CssScopeClassName) || this.NormalizedBodyType == BodyResponseType.Text);
			}
		}

		[OnDeserializing]
		private void Init(StreamingContext context)
		{
			this.Init();
		}

		private BodyResponseType bodyType;

		private BodyResponseType? uniqueBodyType;

		private BodyResponseType? normalizedBodyType;

		private bool convertHtmlCodePageToUTF8;

		private bool useSafeHtml;
	}
}
