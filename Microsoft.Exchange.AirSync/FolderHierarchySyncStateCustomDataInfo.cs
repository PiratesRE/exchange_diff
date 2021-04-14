using System;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.AirSync
{
	internal struct FolderHierarchySyncStateCustomDataInfo
	{
		internal static void HandlerCustomDataVersioning(FolderHierarchySyncState syncState)
		{
			if (syncState == null)
			{
				throw new ArgumentNullException("syncState");
			}
			if (syncState.CustomVersion == null)
			{
				return;
			}
			bool flag = true;
			if (syncState.CustomVersion < 2 || syncState.CustomVersion > 5)
			{
				flag = false;
			}
			else if (syncState.CustomVersion.Value != 5)
			{
				int valueOrDefault = syncState.CustomVersion.GetValueOrDefault();
				int? num;
				if (num != null)
				{
					switch (valueOrDefault)
					{
					case 2:
						syncState["RecoverySyncKey"] = new Int32Data(0);
						break;
					case 3:
						break;
					case 4:
						goto IL_DD;
					default:
						goto IL_11E;
					}
					syncState[CustomStateDatumType.AirSyncProtocolVersion] = new ConstStringData(StaticStringPool.Instance.Intern("2.0"));
					IL_DD:
					object obj = syncState[CustomStateDatumType.AirSyncProtocolVersion];
					if (obj is ConstStringData)
					{
						string data = syncState.GetData<ConstStringData, string>(CustomStateDatumType.AirSyncProtocolVersion, null);
						int data2 = 20;
						if (data != null)
						{
							data2 = AirSyncUtility.ParseVersionString(data);
						}
						syncState[CustomStateDatumType.AirSyncProtocolVersion] = new Int32Data(data2);
						goto IL_120;
					}
					goto IL_120;
				}
				IL_11E:
				flag = false;
			}
			IL_120:
			if (!flag)
			{
				syncState.HandleCorruptSyncState();
			}
		}

		internal const string AirSyncProtocolVersion = "AirSyncProtocolVersion";

		internal const string RecoverySyncKey = "RecoverySyncKey";

		internal const string SyncKey = "SyncKey";

		internal const int E12BaseVersion = 2;

		internal const int Version = 5;
	}
}
