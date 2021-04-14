using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal class TasksFolderShape : Shape
	{
		static TasksFolderShape()
		{
			TasksFolderShape.defaultProperties.Add(BaseFolderSchema.FolderId);
			TasksFolderShape.defaultProperties.Add(BaseFolderSchema.DisplayName);
			TasksFolderShape.defaultProperties.Add(FolderSchema.UnreadCount);
			TasksFolderShape.defaultProperties.Add(BaseFolderSchema.TotalCount);
			TasksFolderShape.defaultProperties.Add(BaseFolderSchema.ChildFolderCount);
		}

		private TasksFolderShape() : base(Schema.TasksFolder, FolderSchema.GetSchema(), new BaseFolderShape(), TasksFolderShape.defaultProperties)
		{
		}

		internal static TasksFolderShape CreateShape()
		{
			return new TasksFolderShape();
		}

		private static List<PropertyInformation> defaultProperties = new List<PropertyInformation>();
	}
}
