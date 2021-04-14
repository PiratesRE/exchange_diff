using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Handler
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal static class ViewClientProperties
	{
		internal static class HierarchyViewClientProperties
		{
			public static PropertyTag[] DisallowList = new PropertyTag[]
			{
				PropertyTag.OfflineFlags,
				PropertyTag.StoreSupportMask,
				PropertyTag.ReplicaServer,
				PropertyTag.ReplicaVersion,
				PropertyTag.StoreSupportMask,
				PropertyTag.AccessLevel
			};
		}

		internal static class ContentsViewClientProperties
		{
			public static PropertyTag[] DisallowList = new PropertyTag[]
			{
				PropertyTag.InstanceKey,
				PropertyTag.ObjectType,
				PropertyTag.EntryId,
				PropertyTag.RecordKey,
				PropertyTag.StoreEntryId,
				PropertyTag.StoreRecordKey,
				PropertyTag.ParentEntryId,
				PropertyTag.SentMailEntryId,
				PropertyTag.PostReplyFolderEntries,
				PropertyTag.RuleFolderFid
			};
		}
	}
}
