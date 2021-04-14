using System;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.AirSync
{
	internal class SyncStatusSyncStateInfo : CustomSyncStateInfo
	{
		public override string UniqueName
		{
			get
			{
				return "SyncStatus";
			}
			set
			{
				throw new InvalidOperationException("SyncStatusSyncStateInfo.UniqueName is not settable.");
			}
		}

		public override int Version
		{
			get
			{
				return 26;
			}
		}

		public override void HandleSyncStateVersioning(SyncState syncState)
		{
			if (syncState == null)
			{
				throw new ArgumentNullException("syncState");
			}
			if (syncState.BackendVersion == null)
			{
				return;
			}
			bool flag = true;
			if (syncState.BackendVersion < 3 || syncState.BackendVersion > this.Version)
			{
				flag = false;
			}
			else if (syncState.BackendVersion.Value != this.Version)
			{
				int value = syncState.BackendVersion.Value;
				switch (value)
				{
				case 3:
					syncState["ClientCanSendUpEmptyRequests"] = new BooleanData(false);
					syncState["LastSyncRequestRandomString"] = new StringData(string.Empty);
					break;
				case 4:
					break;
				case 5:
					goto IL_114;
				default:
					switch (value)
					{
					case 20:
						goto IL_124;
					case 21:
					case 22:
						goto IL_150;
					case 23:
						goto IL_166;
					case 24:
						goto IL_172;
					case 25:
						goto IL_17D;
					default:
						flag = false;
						goto IL_18C;
					}
					break;
				}
				syncState["ClientCanSendUpEmptyRequests"] = new BooleanData(false);
				syncState.Remove("IsXmlValidBool");
				IL_114:
				syncState["LastClientIdsSent"] = new GenericListData<StringData, string>();
				IL_124:
				syncState["LastCachableWbxmlDocument"] = new ByteArrayData();
				syncState["ClientCanSendUpEmptyRequests"] = new BooleanData(false);
				syncState.Remove("XmlDocumentString");
				IL_150:
				syncState["LastAdUpdateTime"] = syncState.GetData<DateTimeData>("LastAdUpdateTime");
				IL_166:
				syncState["ClientCategoryList"] = null;
				IL_172:
				syncState.Remove("LastAdUpdateTime");
				IL_17D:
				syncState.Remove("MailboxLog");
			}
			IL_18C:
			if (!flag)
			{
				syncState.HandleCorruptSyncState();
			}
		}

		internal static bool IsPreE14SyncState(SyncState syncState)
		{
			return syncState != null && syncState.BackendVersion != null && syncState.BackendVersion.Value < 20;
		}

		internal const string UniqueNameString = "SyncStatus";

		internal const int E14BaseVersion = 20;

		internal struct PropertyNames
		{
			internal const string ClientCanSendUpEmptyRequests = "ClientCanSendUpEmptyRequests";

			internal const string LastSyncAttemptTime = "LastSyncAttemptTime";

			internal const string LastSyncRequestRandomString = "LastSyncRequestRandomString";

			internal const string LastSyncSuccessTime = "LastSyncSuccessTime";

			internal const string UserAgent = "UserAgent";

			internal const string LastCachableWbxmlDocument = "LastCachableWbxmlDocument";

			internal const string LastClientIdsSent = "LastClientIdsSent";

			internal const string LastAdUpdateTime = "LastAdUpdateTime";

			internal const string ClientCategoryList = "ClientCategoryList";
		}
	}
}
