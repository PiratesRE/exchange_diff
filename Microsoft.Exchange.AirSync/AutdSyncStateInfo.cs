using System;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.AirSync
{
	internal class AutdSyncStateInfo : CustomSyncStateInfo
	{
		public override string UniqueName
		{
			get
			{
				return "Autd";
			}
			set
			{
				throw new InvalidOperationException("AutdSyncStateInfo.UniqueName is not settable.");
			}
		}

		public override int Version
		{
			get
			{
				return 2;
			}
		}

		public override void HandleSyncStateVersioning(SyncState syncState)
		{
			base.HandleSyncStateVersioning(syncState);
		}

		internal const string UniqueNameString = "Autd";

		internal struct PropertyNames
		{
			internal const string DPFolderList = "DPFolderList";

			internal const string LastPingHeartbeat = "LastPingHeartbeat";
		}
	}
}
