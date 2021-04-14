using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.WebServices.Data;

namespace Microsoft.Exchange.Data.Storage.Compliance.DAR
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class StoreObjectProvider : TenantStoreDataProvider
	{
		public StoreObjectProvider(OrganizationId organizationId) : base(organizationId)
		{
		}

		public IEnumerable<T> FindPaged<T>(SearchFilter filter, ObjectId rootId, bool deepSearch, SortBy sortBy, int pageSize, params ProviderPropertyDefinition[] properties) where T : IConfigurable, new()
		{
			FolderId rootId2 = null;
			if (rootId != null)
			{
				rootId2 = new FolderId(((EwsStoreObjectId)rootId).EwsObjectId.UniqueId);
			}
			return this.InternalFindPaged<T>(filter, rootId2, deepSearch, (sortBy == null) ? null : new SortBy[]
			{
				sortBy
			}, pageSize, properties);
		}

		protected override FolderId GetDefaultFolder()
		{
			if (this.containerFolderId == null)
			{
				this.containerFolderId = base.GetOrCreateFolder("DarTasks", new FolderId(10, new Mailbox(base.Mailbox.MailboxInfo.PrimarySmtpAddress.ToString()))).Id;
			}
			return this.containerFolderId;
		}

		public const string ContainerFolderName = "DarTasks";

		private FolderId containerFolderId;
	}
}
