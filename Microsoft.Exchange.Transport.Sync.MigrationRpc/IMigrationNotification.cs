using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Sync.Migration.Rpc
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IMigrationNotification
	{
		UpdateMigrationRequestResult UpdateMigrationRequest(UpdateMigrationRequestArgs args);

		RegisterMigrationBatchResult RegisterMigrationBatch(RegisterMigrationBatchArgs args);
	}
}
