using System;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Migration
{
	internal class LegacyLocalMoveSubscriptionHandler : LegacyMoveSubscriptionHandlerBase
	{
		public LegacyLocalMoveSubscriptionHandler(IMigrationDataProvider dataProvider, MigrationJob migrationJob) : base(dataProvider, migrationJob)
		{
		}

		public override MigrationType SupportedMigrationType
		{
			get
			{
				return MigrationType.ExchangeLocalMove;
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<LegacyLocalMoveSubscriptionHandler>(this);
		}
	}
}
