using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class CalendarGroupSchema : FolderTreeDataSchema
	{
		public new static CalendarGroupSchema Instance
		{
			get
			{
				if (CalendarGroupSchema.instance == null)
				{
					CalendarGroupSchema.instance = new CalendarGroupSchema();
				}
				return CalendarGroupSchema.instance;
			}
		}

		[Autoload]
		public static readonly StorePropertyDefinition GroupClassId = InternalSchema.NavigationNodeGroupClassId;

		private static CalendarGroupSchema instance;
	}
}
