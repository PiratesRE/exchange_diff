using System;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal sealed class SearchFolderSchema : Schema
	{
		static SearchFolderSchema()
		{
			XmlElementInformation[] xmlElements = new XmlElementInformation[]
			{
				SearchFolderSchema.UnreadCount,
				SearchFolderSchema.SearchParameters
			};
			SearchFolderSchema.schema = new SearchFolderSchema(xmlElements);
		}

		private SearchFolderSchema(XmlElementInformation[] xmlElements) : base(xmlElements)
		{
		}

		public static Schema GetSchema()
		{
			return SearchFolderSchema.schema;
		}

		private static Schema schema;

		public static readonly PropertyInformation UnreadCount = FolderSchema.UnreadCount;

		public static readonly PropertyInformation SearchParameters = new PropertyInformation(PropertyUriEnum.SearchParameters, ExchangeVersion.Exchange2007, null, new PropertyCommand.CreatePropertyCommand(SearchParametersProperty.CreateCommand));
	}
}
