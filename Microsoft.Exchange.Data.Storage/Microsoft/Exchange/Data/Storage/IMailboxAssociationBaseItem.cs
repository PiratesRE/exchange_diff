using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IMailboxAssociationBaseItem : IPropertyBag, IReadOnlyPropertyBag, IDisposable
	{
		string LegacyDN { get; set; }

		string ExternalId { get; set; }

		SmtpAddress SmtpAddress { get; set; }

		bool IsMember { get; set; }

		bool ShouldEscalate { get; set; }

		bool IsAutoSubscribed { get; set; }

		bool IsPin { get; set; }

		ExDateTime JoinDate { get; set; }

		string SyncedIdentityHash { get; set; }

		int CurrentVersion { get; set; }

		int SyncedVersion { get; set; }

		string LastSyncError { get; set; }

		int SyncAttempts { get; set; }

		string SyncedSchemaVersion { get; set; }
	}
}
