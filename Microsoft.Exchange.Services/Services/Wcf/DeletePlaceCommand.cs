using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal sealed class DeletePlaceCommand : ServiceCommand<ServiceResultNone>
	{
		public DeletePlaceCommand(CallContext callContext, DeletePlaceRequest request) : base(callContext)
		{
			this.request = request;
		}

		protected override ServiceResultNone InternalExecute()
		{
			if (!string.IsNullOrEmpty(this.request.Id))
			{
				MailboxSession mailboxIdentityMailboxSession = base.CallContext.SessionCache.GetMailboxIdentityMailboxSession();
				ComparisonFilter queryFilter = new ComparisonFilter(ComparisonOperator.Equal, ContactSchema.WorkLocationUri, this.request.Id);
				using (Folder folder = Folder.Bind(mailboxIdentityMailboxSession, mailboxIdentityMailboxSession.GetDefaultFolderId(DefaultFolderType.Location)))
				{
					using (QueryResult queryResult = folder.ItemQuery(ItemQueryType.None, queryFilter, null, new PropertyDefinition[]
					{
						ItemSchema.Id
					}))
					{
						object[][] rows = queryResult.GetRows(1);
						if (rows.Length == 1)
						{
							VersionedId versionedId = rows[0][0] as VersionedId;
							folder.DeleteObjects(DeleteItemFlags.HardDelete, new StoreId[]
							{
								versionedId
							});
						}
					}
				}
			}
			return new ServiceResultNone();
		}

		private readonly DeletePlaceRequest request;
	}
}
