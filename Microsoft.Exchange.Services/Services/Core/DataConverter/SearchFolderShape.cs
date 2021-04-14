using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal class SearchFolderShape : Shape
	{
		static SearchFolderShape()
		{
			SearchFolderShape.defaultProperties.Add(BaseFolderSchema.FolderId);
			SearchFolderShape.defaultProperties.Add(BaseFolderSchema.DisplayName);
			SearchFolderShape.defaultProperties.Add(SearchFolderSchema.UnreadCount);
			SearchFolderShape.defaultProperties.Add(BaseFolderSchema.TotalCount);
		}

		private SearchFolderShape() : base(Schema.SearchFolder, SearchFolderSchema.GetSchema(), new BaseFolderShape(), SearchFolderShape.defaultProperties)
		{
		}

		internal static SearchFolderShape CreateShape()
		{
			return new SearchFolderShape();
		}

		private static List<PropertyInformation> defaultProperties = new List<PropertyInformation>();
	}
}
