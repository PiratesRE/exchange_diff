using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Entities.TypeConversion.PropertyAccessors;

namespace Microsoft.Exchange.Entities.Calendaring.TypeConversion.PropertyAccessors.StorageAccessors
{
	internal static class CalendarFolderAccessors
	{
		public static readonly IStoragePropertyAccessor<ICalendarFolder, string> DisplayName = new DefaultStoragePropertyAccessor<ICalendarFolder, string>(FolderSchema.DisplayName, false);
	}
}
