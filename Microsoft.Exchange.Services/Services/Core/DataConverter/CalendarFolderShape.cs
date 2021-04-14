using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal class CalendarFolderShape : Shape
	{
		static CalendarFolderShape()
		{
			CalendarFolderShape.defaultProperties.Add(BaseFolderSchema.FolderId);
			CalendarFolderShape.defaultProperties.Add(BaseFolderSchema.DisplayName);
			CalendarFolderShape.defaultProperties.Add(BaseFolderSchema.TotalCount);
			CalendarFolderShape.defaultProperties.Add(BaseFolderSchema.ChildFolderCount);
		}

		private CalendarFolderShape() : base(Schema.CalendarFolder, CalendarFolderSchema.GetSchema(), new BaseFolderShape(), CalendarFolderShape.defaultProperties)
		{
		}

		internal static CalendarFolderShape CreateShape()
		{
			return new CalendarFolderShape();
		}

		private static List<PropertyInformation> defaultProperties = new List<PropertyInformation>();
	}
}
