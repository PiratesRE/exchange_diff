using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal struct SyncStateProp
	{
		public static string ClientState
		{
			get
			{
				return SyncStateProp.GetStaticString("ClientState_C70D1604-09E7-489f-B18B-DB337B9A3F0F");
			}
		}

		public static string ColdDataKeys
		{
			get
			{
				return SyncStateProp.GetStaticString("ColdDataKeys_C70D1604-09E7-489f-B18B-DB337B9A3F0F");
			}
		}

		public static string CumulativeClientManifest
		{
			get
			{
				return SyncStateProp.GetStaticString("CumulativeClientManifest_C70D1604-09E7-489f-B18B-DB337B9A3F0F");
			}
		}

		public static string CurDelayedServerOperationQueue
		{
			get
			{
				return SyncStateProp.GetStaticString("CurDelayedServerOperationQueue_C70D1604-09E7-489f-B18B-DB337B9A3F0F");
			}
		}

		public static string CurFilterId
		{
			get
			{
				return SyncStateProp.GetStaticString("CurFilterId_C70D1604-09E7-489f-B18B-DB337B9A3F0F");
			}
		}

		public static string CurMaxWatermark
		{
			get
			{
				return SyncStateProp.GetStaticString("CurMaxWatermark_C70D1604-09E7-489f-B18B-DB337B9A3F0F");
			}
		}

		public static string CurFTSMaxWatermark
		{
			get
			{
				return SyncStateProp.GetStaticString("CurFTSMaxWatermark_C70D1604-09E7-489f-B18B-DB337B9A3F0F");
			}
		}

		public static string CurServerManifest
		{
			get
			{
				return SyncStateProp.GetStaticString("CurServerManifest_C70D1604-09E7-489f-B18B-DB337B9A3F0F");
			}
		}

		public static string CurSnapShotWatermark
		{
			get
			{
				return SyncStateProp.GetStaticString("CurSnapShotWatermark_C70D1604-09E7-489f-B18B-DB337B9A3F0F");
			}
		}

		public static string CustomVersion
		{
			get
			{
				return SyncStateProp.GetStaticString("CustomVersion_C70D1604-09E7-489f-B18B-DB337B9A3F0F");
			}
		}

		public static string PrevDelayedServerOperationQueue
		{
			get
			{
				return SyncStateProp.GetStaticString("PrevDelayedServerOperationQueue_C70D1604-09E7-489f-B18B-DB337B9A3F0F");
			}
		}

		public static string PrevFilterId
		{
			get
			{
				return SyncStateProp.GetStaticString("PrevFilterId_C70D1604-09E7-489f-B18B-DB337B9A3F0F");
			}
		}

		public static string PrevMaxWatermark
		{
			get
			{
				return SyncStateProp.GetStaticString("PrevMaxWatermark_C70D1604-09E7-489f-B18B-DB337B9A3F0F");
			}
		}

		public static string PrevFTSMaxWatermark
		{
			get
			{
				return SyncStateProp.GetStaticString("PrevFTSMaxWatermark_C70D1604-09E7-489f-B18B-DB337B9A3F0F");
			}
		}

		public static string PrevServerManifest
		{
			get
			{
				return SyncStateProp.GetStaticString("PrevServerManifest_C70D1604-09E7-489f-B18B-DB337B9A3F0F");
			}
		}

		public static string PrevSnapShotWatermark
		{
			get
			{
				return SyncStateProp.GetStaticString("PrevSnapShotWatermark_C70D1604-09E7-489f-B18B-DB337B9A3F0F");
			}
		}

		public static string RecoveryClientManifest
		{
			get
			{
				return SyncStateProp.GetStaticString("RecoveryClientManifest_C70D1604-09E7-489f-B18B-DB337B9A3F0F");
			}
		}

		public static string UsingQueryBasedFilter
		{
			get
			{
				return SyncStateProp.GetStaticString("UsingQueryBasedFilter_C70D1604-09E7-489f-B18B-DB337B9A3F0F");
			}
		}

		public static string Version
		{
			get
			{
				return SyncStateProp.GetStaticString("Version_C70D1604-09E7-489f-B18B-DB337B9A3F0F");
			}
		}

		public static string CurLastSyncConversationMode
		{
			get
			{
				return SyncStateProp.GetStaticString("CurLastSyncConversationMode_C70D1604-09E7-489f-B18B-DB337B9A3F0F");
			}
		}

		public static string PrevLastSyncConversationMode
		{
			get
			{
				return SyncStateProp.GetStaticString("PrevLastSyncConversationMode_C70D1604-09E7-489f-B18B-DB337B9A3F0F");
			}
		}

		private static string GetStaticString(string key)
		{
			if (!SyncStateProp.hasBeenInitialized)
			{
				SyncStateProp.Initialize();
			}
			return key;
		}

		private static void Initialize()
		{
			lock ("_C70D1604-09E7-489f-B18B-DB337B9A3F0F")
			{
				if (!SyncStateProp.hasBeenInitialized)
				{
					StaticStringPool.Instance.Intern("UsingQueryBasedFilter_C70D1604-09E7-489f-B18B-DB337B9A3F0F");
					StaticStringPool.Instance.Intern("ColdDataKeys_C70D1604-09E7-489f-B18B-DB337B9A3F0F");
					StaticStringPool.Instance.Intern("Version_C70D1604-09E7-489f-B18B-DB337B9A3F0F");
					StaticStringPool.Instance.Intern("CustomVersion_C70D1604-09E7-489f-B18B-DB337B9A3F0F");
					StaticStringPool.Instance.Intern("CurMaxWatermark_C70D1604-09E7-489f-B18B-DB337B9A3F0F");
					StaticStringPool.Instance.Intern("PrevMaxWatermark_C70D1604-09E7-489f-B18B-DB337B9A3F0F");
					StaticStringPool.Instance.Intern("ClientState_C70D1604-09E7-489f-B18B-DB337B9A3F0F");
					StaticStringPool.Instance.Intern("CumulativeClientManifest_C70D1604-09E7-489f-B18B-DB337B9A3F0F");
					StaticStringPool.Instance.Intern("RecoveryClientManifest_C70D1604-09E7-489f-B18B-DB337B9A3F0F");
					StaticStringPool.Instance.Intern("CurServerManifest_C70D1604-09E7-489f-B18B-DB337B9A3F0F");
					StaticStringPool.Instance.Intern("PrevServerManifest_C70D1604-09E7-489f-B18B-DB337B9A3F0F");
					StaticStringPool.Instance.Intern("CurFilterId_C70D1604-09E7-489f-B18B-DB337B9A3F0F");
					StaticStringPool.Instance.Intern("PrevFilterId_C70D1604-09E7-489f-B18B-DB337B9A3F0F");
					StaticStringPool.Instance.Intern("CurSnapShotWatermark_C70D1604-09E7-489f-B18B-DB337B9A3F0F");
					StaticStringPool.Instance.Intern("PrevSnapShotWatermark_C70D1604-09E7-489f-B18B-DB337B9A3F0F");
					StaticStringPool.Instance.Intern("CurDelayedServerOperationQueue_C70D1604-09E7-489f-B18B-DB337B9A3F0F");
					StaticStringPool.Instance.Intern("PrevDelayedServerOperationQueue_C70D1604-09E7-489f-B18B-DB337B9A3F0F");
					StaticStringPool.Instance.Intern("CurLastSyncConversationMode_C70D1604-09E7-489f-B18B-DB337B9A3F0F");
					StaticStringPool.Instance.Intern("PrevLastSyncConversationMode_C70D1604-09E7-489f-B18B-DB337B9A3F0F");
					SyncStateProp.hasBeenInitialized = true;
				}
			}
		}

		public const string PrevDelayedServerOperationQueueString = "PrevDelayedServerOperationQueue_C70D1604-09E7-489f-B18B-DB337B9A3F0F";

		public const string PrevMaxWatermarkString = "PrevMaxWatermark_C70D1604-09E7-489f-B18B-DB337B9A3F0F";

		public const string PrevFTSMaxWatermarkString = "PrevFTSMaxWatermark_C70D1604-09E7-489f-B18B-DB337B9A3F0F";

		private const string ClientStateString = "ClientState_C70D1604-09E7-489f-B18B-DB337B9A3F0F";

		private const string ColdDataKeysString = "ColdDataKeys_C70D1604-09E7-489f-B18B-DB337B9A3F0F";

		private const string CumulativeClientManifestString = "CumulativeClientManifest_C70D1604-09E7-489f-B18B-DB337B9A3F0F";

		private const string CurDelayedServerOperationQueueString = "CurDelayedServerOperationQueue_C70D1604-09E7-489f-B18B-DB337B9A3F0F";

		private const string CurFilterIdString = "CurFilterId_C70D1604-09E7-489f-B18B-DB337B9A3F0F";

		private const string CurMaxWatermarkString = "CurMaxWatermark_C70D1604-09E7-489f-B18B-DB337B9A3F0F";

		private const string CurFTSMaxWatermarkString = "CurFTSMaxWatermark_C70D1604-09E7-489f-B18B-DB337B9A3F0F";

		private const string CurServerManifestString = "CurServerManifest_C70D1604-09E7-489f-B18B-DB337B9A3F0F";

		private const string CurSnapShotWatermarkString = "CurSnapShotWatermark_C70D1604-09E7-489f-B18B-DB337B9A3F0F";

		private const string CustomVersionString = "CustomVersion_C70D1604-09E7-489f-B18B-DB337B9A3F0F";

		private const string PrevFilterIdString = "PrevFilterId_C70D1604-09E7-489f-B18B-DB337B9A3F0F";

		private const string PrevServerManifestString = "PrevServerManifest_C70D1604-09E7-489f-B18B-DB337B9A3F0F";

		private const string PrevSnapShotWatermarkString = "PrevSnapShotWatermark_C70D1604-09E7-489f-B18B-DB337B9A3F0F";

		private const string RecoveryClientManifestString = "RecoveryClientManifest_C70D1604-09E7-489f-B18B-DB337B9A3F0F";

		private const string UsingQueryBasedFilterString = "UsingQueryBasedFilter_C70D1604-09E7-489f-B18B-DB337B9A3F0F";

		private const string VersionString = "Version_C70D1604-09E7-489f-B18B-DB337B9A3F0F";

		private const string CurLastSyncConversationModeString = "CurLastSyncConversationMode_C70D1604-09E7-489f-B18B-DB337B9A3F0F";

		private const string PrevLastSyncConversationModeString = "PrevLastSyncConversationMode_C70D1604-09E7-489f-B18B-DB337B9A3F0F";

		private const string UID = "_C70D1604-09E7-489f-B18B-DB337B9A3F0F";

		private static bool hasBeenInitialized;
	}
}
