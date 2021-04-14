using System;
using System.Globalization;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IStoreSession : IDisposable
	{
		AggregateOperationResult Delete(DeleteItemFlags deleteFlags, params StoreId[] ids);

		IXSOMailbox Mailbox { get; }

		IActivitySession ActivitySession { get; }

		CultureInfo Culture { get; }

		string DisplayAddress { get; }

		OrganizationId OrganizationId { get; }

		Guid MdbGuid { get; }

		IdConverter IdConverter { get; }

		bool IsMoveUser { get; }

		IExchangePrincipal MailboxOwner { get; }

		Guid MailboxGuid { get; }

		LogonType LogonType { get; }

		SessionCapabilities Capabilities { get; }

		ExTimeZone ExTimeZone { get; set; }

		StoreObjectId GetParentFolderId(StoreObjectId objectId);

		IRecipientSession GetADRecipientSession(bool isReadOnly, ConsistencyMode consistencyMode);

		IConfigurationSession GetADConfigurationSession(bool isReadOnly, ConsistencyMode consistencyMode);
	}
}
