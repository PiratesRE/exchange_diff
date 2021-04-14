using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class SyncStateInfo
	{
		public virtual SaveMode SaveModeForSyncState
		{
			get
			{
				return SaveMode.NoConflictResolutionForceSave;
			}
		}

		public abstract string UniqueName { get; set; }

		public abstract int Version { get; }

		public bool ReadOnly
		{
			get
			{
				return this.readOnly;
			}
			set
			{
				this.readOnly = value;
			}
		}

		public virtual void HandleSyncStateVersioning(SyncState syncState)
		{
			if (syncState == null)
			{
				throw new ArgumentNullException("syncState");
			}
			if (syncState.BackendVersion.Value != syncState.Version)
			{
				syncState.HandleCorruptSyncState();
			}
		}

		public static readonly PropertyDefinition StorageLocation = InternalSchema.SyncCustomState;

		private bool readOnly;
	}
}
