using System;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface ISubscriptionId : ISnapshotId, IMigrationSerializable
	{
		string ToString();

		MigrationType MigrationType { get; }

		IMailboxData MailboxData { get; }
	}
}
