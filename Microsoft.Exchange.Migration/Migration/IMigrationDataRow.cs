using System;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IMigrationDataRow
	{
		MigrationType MigrationType { get; }

		MigrationUserRecipientType RecipientType { get; }

		string Identifier { get; }

		string LocalMailboxIdentifier { get; }

		int CursorPosition { get; }

		bool SupportsRemoteIdentifier { get; }

		string RemoteIdentifier { get; }
	}
}
