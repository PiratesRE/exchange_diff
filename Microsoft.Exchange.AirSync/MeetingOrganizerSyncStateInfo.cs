using System;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.AirSync
{
	internal class MeetingOrganizerSyncStateInfo : CustomSyncStateInfo
	{
		public override string UniqueName
		{
			get
			{
				return "MeetingOrganizerInfo";
			}
			set
			{
				throw new InvalidOperationException("MeetingOrganizerSyncStateInfo.UniqueName is not settable.");
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
			if (syncState == null)
			{
				throw new ArgumentNullException("syncState");
			}
			if (syncState.BackendVersion == null)
			{
				return;
			}
			if (!(syncState.BackendVersion == this.Version))
			{
				syncState.HandleCorruptSyncState();
			}
		}

		internal const string UniqueNameString = "MeetingOrganizerInfo";

		internal const int CurrentVersion = 0;

		internal const int E14BaseVersion = 20;

		internal struct PropertyNames
		{
			internal const string MeetingOrganizerInfo = "MeetingOrganizerInfo";
		}
	}
}
