using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IMigrationADProvider
	{
		string TenantOrganizationName { get; }

		bool IsLicensingEnforced { get; }

		bool IsSmtpAddressCheckWithAcceptedDomain { get; }

		bool IsMigrationEnabled { get; }

		bool IsDirSyncEnabled { get; }

		bool IsMSOSyncEnabled { get; }

		MicrosoftExchangeRecipient PrimaryExchangeRecipient { get; }

		ADRecipient GetADRecipientByProxyAddress(string userEmail);

		ADRecipient GetADRecipientByObjectId(ADObjectId objectId);

		ADRecipient GetADRecipientByExchangeObjectId(Guid exchangeObjectGuid);

		MailboxData GetMailboxDataFromSmtpAddress(string userEmail, bool forceRefresh, bool throwOnNotFound = true);

		MailboxData GetMailboxDataFromLegacyDN(string userLegDN, bool forceRefresh, string userEmailAddressForDebug);

		MailboxData GetPublicFolderMailboxDataFromName(string name, bool forceRefresh, bool throwOnNotFound = true);

		string GetPublicFolderHierarchyMailboxName();

		string GetDatabaseServerFqdn(Guid mdbGuid, bool forceRefresh);

		void UpdateMigrationUpgradeConstraint(UpgradeConstraint constraint);

		void RemovePublicFolderMigrationLock();

		bool CheckPublicFoldersLockedForMigration();

		string GetPreferredDomainController();

		ExchangePrincipal GetExchangePrincipalFromMbxGuid(Guid mailboxGuid);
	}
}
