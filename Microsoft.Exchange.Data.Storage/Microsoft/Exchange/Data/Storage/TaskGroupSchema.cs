using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class TaskGroupSchema : FolderTreeDataSchema
	{
		public new static TaskGroupSchema Instance
		{
			get
			{
				if (TaskGroupSchema.instance == null)
				{
					TaskGroupSchema.instance = new TaskGroupSchema();
				}
				return TaskGroupSchema.instance;
			}
		}

		[Autoload]
		public static readonly StorePropertyDefinition GroupClassId = InternalSchema.NavigationNodeGroupClassId;

		private static TaskGroupSchema instance;
	}
}
