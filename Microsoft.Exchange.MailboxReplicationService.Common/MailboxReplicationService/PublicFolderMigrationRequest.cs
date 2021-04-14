using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[Serializable]
	public sealed class PublicFolderMigrationRequest : RequestBase
	{
		public PublicFolderMigrationRequest()
		{
		}

		internal PublicFolderMigrationRequest(IRequestIndexEntry index) : base(index)
		{
		}

		public new ADObjectId SourceDatabase
		{
			get
			{
				return base.SourceDatabase;
			}
		}

		public override string ToString()
		{
			if (base.Name != null && base.OrganizationId != null)
			{
				return string.Format("{0}\\{1}", base.OrganizationId.ToString(), base.Name);
			}
			return base.ToString();
		}
	}
}
