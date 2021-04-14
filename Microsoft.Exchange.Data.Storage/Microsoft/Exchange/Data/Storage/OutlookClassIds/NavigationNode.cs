using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.OutlookClassIds
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class NavigationNode
	{
		internal static readonly OutlookClassId MailFolderFavoriteClassId = new OutlookClassId(new Guid("{00067800-0000-0000-C000-000000000046}"));

		internal static readonly OutlookClassId CalendarFolderFavoriteClassId = new OutlookClassId(new Guid("{00067802-0000-0000-C000-000000000046}"));

		internal static readonly OutlookClassId ContactFolderFavoriteClassId = new OutlookClassId(new Guid("{00067801-0000-0000-C000-000000000046}"));
	}
}
