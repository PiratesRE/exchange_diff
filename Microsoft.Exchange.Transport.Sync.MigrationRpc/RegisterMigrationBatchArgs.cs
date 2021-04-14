using System;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Sync.Common;

namespace Microsoft.Exchange.Transport.Sync.Migration.Rpc
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class RegisterMigrationBatchArgs
	{
		internal RegisterMigrationBatchArgs(Guid mdbGuid, string mailboxOwnerLegacyDN, ADObjectId organizationName, bool refresh)
		{
			SyncUtilities.ThrowIfGuidEmpty("mdbGuid", mdbGuid);
			SyncUtilities.ThrowIfArgumentNullOrEmpty("mailboxOwnerLegacyDN", mailboxOwnerLegacyDN);
			this.mdbGuid = mdbGuid;
			this.mailboxOwnerLegacyDN = mailboxOwnerLegacyDN;
			this.organizationName = (organizationName ?? new ADObjectId());
			this.refresh = refresh;
		}

		internal Guid MdbGuid
		{
			get
			{
				return this.mdbGuid;
			}
		}

		internal string MailboxOwnerLegacyDN
		{
			get
			{
				return this.mailboxOwnerLegacyDN;
			}
		}

		internal bool Refresh
		{
			get
			{
				return this.refresh;
			}
		}

		internal ADObjectId OrganizationId
		{
			get
			{
				return this.organizationName;
			}
		}

		internal static RegisterMigrationBatchArgs UnMarshal(MdbefPropertyCollection inputArgs)
		{
			return new RegisterMigrationBatchArgs(MigrationRpcHelper.ReadValue<Guid>(inputArgs, 2688614472U), MigrationRpcHelper.ReadValue<string>(inputArgs, 2688679967U), MigrationRpcHelper.ReadADObjectId(inputArgs, 2688876802U), MigrationRpcHelper.ReadValue<bool>(inputArgs, 2688876555U, true));
		}

		internal MdbefPropertyCollection Marshal()
		{
			MdbefPropertyCollection mdbefPropertyCollection = new MdbefPropertyCollection();
			mdbefPropertyCollection[2688614472U] = this.mdbGuid;
			mdbefPropertyCollection[2688679967U] = this.mailboxOwnerLegacyDN;
			mdbefPropertyCollection[2688876802U] = this.organizationName.GetBytes();
			mdbefPropertyCollection[2688876555U] = this.refresh;
			return mdbefPropertyCollection;
		}

		private readonly Guid mdbGuid;

		private readonly string mailboxOwnerLegacyDN;

		private readonly ADObjectId organizationName;

		private readonly bool refresh;
	}
}
