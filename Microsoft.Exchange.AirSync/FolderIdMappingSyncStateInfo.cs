using System;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.AirSync
{
	internal class FolderIdMappingSyncStateInfo : CustomSyncStateInfo
	{
		public override string UniqueName
		{
			get
			{
				return "FolderIdMapping";
			}
			set
			{
				throw new InvalidOperationException("FolderIdMappingSyncStateInfo.UniqueName is not settable.");
			}
		}

		public override int Version
		{
			get
			{
				return 4;
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
			if (syncState.BackendVersion < 2 || syncState.BackendVersion > this.Version)
			{
				flag = false;
			}
			else if (syncState.BackendVersion.Value != this.Version)
			{
				switch (syncState.BackendVersion.Value)
				{
				case 2:
					FolderTree.BuildFolderTree(syncState.StoreObject.Session as MailboxSession, syncState);
					break;
				case 3:
					break;
				default:
					flag = false;
					break;
				}
			}
			if (!flag)
			{
				syncState.HandleCorruptSyncState();
			}
		}

		internal const string UniqueNameString = "FolderIdMapping";

		internal const int E12BaseVersion = 2;

		internal struct PropertyNames
		{
			internal const string IdMapping = "IdMapping";

			internal const string FullFolderTree = "FullFolderTree";

			internal const string RecoveryFullFolderTree = "RecoveryFullFolderTree";
		}
	}
}
