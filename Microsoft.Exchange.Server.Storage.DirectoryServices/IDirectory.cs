using System;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.DirectoryServices
{
	public interface IDirectory
	{
		bool BypassContextADAccessValidation { get; set; }

		ErrorCode PrimeDirectoryCaches(IExecutionContext context);

		ServerInfo GetServerInfo(IExecutionContext context);

		void RefreshServerInfo(IExecutionContext context);

		void RefreshDatabaseInfo(IExecutionContext context, Guid databaseGuid);

		void RefreshMailboxInfo(IExecutionContext context, Guid mailboxGuid);

		void RefreshOrganizationContainer(IExecutionContext context, Guid organizationGuid);

		DatabaseInfo GetDatabaseInfo(IExecutionContext context, Guid databaseGuid);

		MailboxInfo GetMailboxInfo(IExecutionContext context, TenantHint tenantHint, Guid mailboxGuid, GetMailboxInfoFlags flags);

		MailboxInfo GetMailboxInfo(IExecutionContext context, TenantHint tenantHint, string legacyDN);

		AddressInfo GetAddressInfoByMailboxGuid(IExecutionContext context, TenantHint tenantHint, Guid mailboxGuid, GetAddressInfoFlags flags);

		AddressInfo GetAddressInfoByObjectId(IExecutionContext context, TenantHint tenantHint, Guid objectId);

		AddressInfo GetAddressInfo(IExecutionContext context, TenantHint tenantHint, string legacyDN, bool loadPublicDelegates);

		TenantHint ResolveTenantHint(IExecutionContext context, byte[] tenantHintBlob);

		void PrePopulateCachesForMailbox(IExecutionContext context, TenantHint tenantHint, Guid mailboxGuid, string domainController);

		bool IsMemberOfDistributionList(IExecutionContext context, TenantHint tenantHint, AddressInfo addressInfo, Guid distributionListObjectId);
	}
}
