using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal class FolderShape : Shape
	{
		static FolderShape()
		{
			FolderShape.defaultProperties.Add(BaseFolderSchema.FolderId);
			FolderShape.defaultProperties.Add(BaseFolderSchema.DisplayName);
			FolderShape.defaultProperties.Add(FolderSchema.UnreadCount);
			FolderShape.defaultProperties.Add(BaseFolderSchema.TotalCount);
			FolderShape.defaultProperties.Add(BaseFolderSchema.ChildFolderCount);
		}

		private FolderShape() : base(Schema.Folder, FolderSchema.GetSchema(), new BaseFolderShape(), FolderShape.defaultProperties)
		{
		}

		internal static FolderShape CreateShape()
		{
			return new FolderShape();
		}

		private static List<PropertyInformation> defaultProperties = new List<PropertyInformation>();
	}
}
