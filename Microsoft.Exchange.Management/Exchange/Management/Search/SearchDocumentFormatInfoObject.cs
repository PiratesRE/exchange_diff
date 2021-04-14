using System;
using Microsoft.Ceres.ContentEngine.Parsing.Component;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.Search
{
	[Serializable]
	internal class SearchDocumentFormatInfoObject : ConfigurableObject
	{
		internal SearchDocumentFormatInfoObject(FileFormatInfo ffInfo) : base(new SimpleProviderPropertyBag())
		{
			base.SetExchangeVersion(ExchangeObjectVersion.Current);
			this[SimpleProviderObjectSchema.Identity] = new SearchDocumentFormatId(ffInfo.Id);
			this[SearchDocumentFormatInfoSchema.DocumentClass] = ffInfo.DocumentClass;
			this[SearchDocumentFormatInfoSchema.Enabled] = ffInfo.Enabled;
			this[SearchDocumentFormatInfoSchema.Extension] = ffInfo.Extension;
			this[SearchDocumentFormatInfoSchema.FormatHandler] = ffInfo.FormatHandler;
			this[SearchDocumentFormatInfoSchema.IsBindUserDefined] = ffInfo.IsBindUserDefined;
			this[SearchDocumentFormatInfoSchema.IsFormatUserDefined] = ffInfo.IsFormatUserDefined;
			this[SearchDocumentFormatInfoSchema.MimeType] = ffInfo.Mime;
			this[SearchDocumentFormatInfoSchema.Name] = ffInfo.Name;
		}

		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return SearchDocumentFormatInfoObject.schema;
			}
		}

		public string DocumentClass
		{
			get
			{
				return (string)this[SearchDocumentFormatInfoSchema.DocumentClass];
			}
		}

		public bool Enabled
		{
			get
			{
				return (bool)this[SearchDocumentFormatInfoSchema.Enabled];
			}
		}

		public string Extension
		{
			get
			{
				return (string)this[SearchDocumentFormatInfoSchema.Extension];
			}
		}

		public string FormatHandler
		{
			get
			{
				return (string)this[SearchDocumentFormatInfoSchema.FormatHandler];
			}
		}

		public bool IsBindUserDefined
		{
			get
			{
				return (bool)this[SearchDocumentFormatInfoSchema.IsBindUserDefined];
			}
		}

		public bool IsFormatUserDefined
		{
			get
			{
				return (bool)this[SearchDocumentFormatInfoSchema.IsFormatUserDefined];
			}
		}

		public string MimeType
		{
			get
			{
				return (string)this[SearchDocumentFormatInfoSchema.MimeType];
			}
		}

		public string Name
		{
			get
			{
				return (string)this[SearchDocumentFormatInfoSchema.Name];
			}
		}

		private static ObjectSchema schema = ObjectSchema.GetInstance<SearchDocumentFormatInfoSchema>();
	}
}
