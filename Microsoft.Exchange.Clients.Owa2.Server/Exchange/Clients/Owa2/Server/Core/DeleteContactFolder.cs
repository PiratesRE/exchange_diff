using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class DeleteContactFolder : ServiceCommand<bool>
	{
		public DeleteContactFolder(CallContext callContext, FolderId folderId) : base(callContext)
		{
			this.folderId = folderId;
		}

		protected override bool InternalExecute()
		{
			ExchangeVersion.Current = ExchangeVersion.Latest;
			MailboxSession mailboxIdentityMailboxSession = base.MailboxIdentityMailboxSession;
			IdAndSession idAndSession = base.IdConverter.ConvertFolderIdToIdAndSession(this.folderId, IdConverter.ConvertOption.IgnoreChangeKey);
			AggregateOperationResult aggregateOperationResult = mailboxIdentityMailboxSession.Delete(DeleteItemFlags.MoveToDeletedItems, new StoreId[]
			{
				idAndSession.Id
			});
			if (aggregateOperationResult.OperationResult == OperationResult.Succeeded)
			{
				PeopleFilterGroupPriorityManager peopleFilterGroupPriorityManager = new PeopleFilterGroupPriorityManager(mailboxIdentityMailboxSession, new XSOFactory());
				mailboxIdentityMailboxSession.ContactFolders.MyContactFolders.Set(peopleFilterGroupPriorityManager.GetMyContactFolderIds());
				return true;
			}
			return false;
		}

		private readonly FolderId folderId;
	}
}
