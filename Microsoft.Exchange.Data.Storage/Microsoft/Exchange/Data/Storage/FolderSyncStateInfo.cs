using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class FolderSyncStateInfo : SyncStateInfo
	{
		public FolderSyncStateInfo() : this(null)
		{
		}

		public FolderSyncStateInfo(string uniqueName)
		{
			this.uniqueName = uniqueName;
		}

		public override int Version
		{
			get
			{
				return 6;
			}
		}

		public override string UniqueName
		{
			get
			{
				return this.uniqueName;
			}
			set
			{
				this.uniqueName = value;
			}
		}

		public override SaveMode SaveModeForSyncState
		{
			get
			{
				return SaveMode.FailOnAnyConflict;
			}
		}

		private string uniqueName;
	}
}
