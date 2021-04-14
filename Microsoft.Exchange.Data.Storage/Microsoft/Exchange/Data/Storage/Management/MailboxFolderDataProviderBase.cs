using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class MailboxFolderDataProviderBase : XsoMailboxDataProviderBase
	{
		public MailboxFolderDataProviderBase(ADSessionSettings adSessionSettings, ADUser mailboxOwner, ISecurityAccessToken userToken, string action) : base(adSessionSettings, mailboxOwner, userToken, action)
		{
		}

		public MailboxFolderDataProviderBase(ADSessionSettings adSessionSettings, ADUser mailboxOwner, string action) : base(adSessionSettings, mailboxOwner, action)
		{
		}

		internal MailboxFolderDataProviderBase()
		{
		}

		public StoreObjectId ResolveStoreObjectIdFromFolderPath(MapiFolderPath folderPath)
		{
			Util.ThrowOnNullArgument(folderPath, "folderPath");
			StoreObjectId storeObjectId;
			if (folderPath.IsNonIpmPath)
			{
				storeObjectId = base.MailboxSession.GetDefaultFolderId(DefaultFolderType.Configuration);
			}
			else
			{
				storeObjectId = base.MailboxSession.GetDefaultFolderId(DefaultFolderType.Root);
			}
			if (folderPath.Depth <= 0)
			{
				return storeObjectId;
			}
			foreach (string propertyValue in folderPath)
			{
				QueryFilter seekFilter = new ComparisonFilter(ComparisonOperator.Equal, FolderSchema.DisplayName, propertyValue);
				using (Folder folder = Folder.Bind(base.MailboxSession, storeObjectId, MailboxFolderDataProviderBase.FolderQueryReturnColumns))
				{
					using (QueryResult queryResult = folder.FolderQuery(FolderQueryFlags.None, null, MailboxFolderDataProviderBase.FolderQuerySorts, MailboxFolderDataProviderBase.FolderQueryReturnColumns))
					{
						if (!queryResult.SeekToCondition(SeekReference.OriginBeginning, seekFilter))
						{
							return null;
						}
						object[][] rows = queryResult.GetRows(1);
						storeObjectId = ((VersionedId)rows[0][0]).ObjectId;
					}
				}
			}
			return storeObjectId;
		}

		private static readonly PropertyDefinition[] FolderQueryReturnColumns = new PropertyDefinition[]
		{
			FolderSchema.Id,
			FolderSchema.DisplayName
		};

		private static readonly SortBy[] FolderQuerySorts = new SortBy[]
		{
			new SortBy(FolderSchema.DisplayName, SortOrder.Ascending)
		};

		private enum FolderQueryReturnColumnIndex
		{
			Id,
			DisplayName
		}
	}
}
