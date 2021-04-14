using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.OutlookClassIds
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class NavigationNodeParentGroup
	{
		internal static readonly OutlookClassId MyFoldersClassId = new OutlookClassId(new Guid("{0006F0B7-0000-0000-C000-000000000046}"));

		internal static readonly OutlookClassId PeoplesFoldersClassId = new OutlookClassId(new Guid("{0006F0B9-0000-0000-C000-000000000046}"));

		internal static readonly OutlookClassId OtherFoldersClassId = new OutlookClassId(new Guid("{0006F0B8-0000-0000-C000-000000000046}"));
	}
}
