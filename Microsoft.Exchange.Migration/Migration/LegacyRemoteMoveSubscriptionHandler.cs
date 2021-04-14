using System;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class LegacyRemoteMoveSubscriptionHandler : LegacyMoveSubscriptionHandlerBase
	{
		public LegacyRemoteMoveSubscriptionHandler(IMigrationDataProvider dataProvider, MigrationJob migrationJob) : base(dataProvider, migrationJob)
		{
		}

		public override MigrationType SupportedMigrationType
		{
			get
			{
				return MigrationType.ExchangeRemoteMove;
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<LegacyRemoteMoveSubscriptionHandler>(this);
		}
	}
}
