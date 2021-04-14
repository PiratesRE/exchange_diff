using System;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.AirSync
{
	internal class AirSyncRootSyncStateInfo : CustomSyncStateInfo
	{
		public override string UniqueName
		{
			get
			{
				return "AirSyncRoot";
			}
			set
			{
				throw new InvalidOperationException("AirSyncRootSyncStateInfo.UniqueName is not settable.");
			}
		}

		public override int Version
		{
			get
			{
				return 0;
			}
		}

		public override void HandleSyncStateVersioning(SyncState syncState)
		{
			base.HandleSyncStateVersioning(syncState);
		}

		internal const string UniqueNameString = "AirSyncRoot";

		internal const int CurrentVersion = 0;

		internal const int E14BaseVersion = 0;

		internal struct PropertyNames
		{
			internal const string LastMaxDevicesExceededMailSentTime = "LastMaxDevicesExceededMailSentTime";
		}
	}
}
