using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Sync.Common;

namespace Microsoft.Exchange.Transport.Sync.Manager
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class OnDatabaseDismountedEventArgs : EventArgs
	{
		internal OnDatabaseDismountedEventArgs(Guid databaseGuid)
		{
			SyncUtilities.ThrowIfGuidEmpty("databaseGuid", databaseGuid);
			this.databaseGuid = databaseGuid;
		}

		internal Guid DatabaseGuid
		{
			get
			{
				return this.databaseGuid;
			}
		}

		private readonly Guid databaseGuid;
	}
}
