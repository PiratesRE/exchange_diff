using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal class BaseFolderShape : Shape
	{
		static BaseFolderShape()
		{
			BaseFolderShape.defaultProperties.Add(BaseFolderSchema.FolderId);
			BaseFolderShape.defaultProperties.Add(BaseFolderSchema.DisplayName);
		}

		public BaseFolderShape() : base(Schema.Folder, BaseFolderSchema.GetSchema(), null, BaseFolderShape.defaultProperties)
		{
		}

		private static List<PropertyInformation> defaultProperties = new List<PropertyInformation>();
	}
}
