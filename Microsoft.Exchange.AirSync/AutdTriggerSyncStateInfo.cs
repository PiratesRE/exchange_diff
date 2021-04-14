using System;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.AirSync
{
	internal class AutdTriggerSyncStateInfo : CustomSyncStateInfo
	{
		public override string UniqueName
		{
			get
			{
				return "AutdTrigger";
			}
			set
			{
				throw new InvalidOperationException("AutdTriggerSyncStateInfo.UniqueName is not settable.");
			}
		}

		public override int Version
		{
			get
			{
				return 1;
			}
		}

		public override void HandleSyncStateVersioning(SyncState syncState)
		{
			base.HandleSyncStateVersioning(syncState);
		}

		internal const string UniqueNameString = "AutdTrigger";
	}
}
