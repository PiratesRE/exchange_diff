using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class MoveContactFolder : ServiceCommand<ContactFolderResponse>
	{
		public MoveContactFolder(CallContext callContext, FolderId folderId, int priority) : base(callContext)
		{
			this.folderId = folderId;
			this.priority = priority;
		}

		protected override ContactFolderResponse InternalExecute()
		{
			ExchangeVersion.Current = ExchangeVersion.Latest;
			MailboxSession mailboxIdentityMailboxSession = base.MailboxIdentityMailboxSession;
			IdAndSession idAndSession = base.IdConverter.ConvertFolderIdToIdAndSession(this.folderId, IdConverter.ConvertOption.IgnoreChangeKey);
			StoreId id;
			try
			{
				using (Folder folder = Folder.Bind(mailboxIdentityMailboxSession, idAndSession.Id))
				{
					PeopleFilterGroupPriorityManager.SetSortGroupPriorityOnFolder(folder, this.priority);
					folder.Save();
					folder.Load(new PropertyDefinition[]
					{
						FolderSchema.Id
					});
					id = folder.Id;
				}
			}
			catch (ObjectNotFoundException)
			{
				return new ContactFolderResponse
				{
					ResponseCode = ResponseCodeType.ErrorItemNotFound.ToString()
				};
			}
			PeopleFilterGroupPriorityManager peopleFilterGroupPriorityManager = new PeopleFilterGroupPriorityManager(mailboxIdentityMailboxSession, new XSOFactory());
			mailboxIdentityMailboxSession.ContactFolders.MyContactFolders.Set(peopleFilterGroupPriorityManager.GetMyContactFolderIds());
			ConcatenatedIdAndChangeKey concatenatedId = IdConverter.GetConcatenatedId(id, new MailboxId(mailboxIdentityMailboxSession), null);
			return new ContactFolderResponse
			{
				ResponseCode = ResponseCodeType.NoError.ToString(),
				FolderId = new FolderId
				{
					Id = concatenatedId.Id,
					ChangeKey = concatenatedId.ChangeKey
				}
			};
		}

		private readonly FolderId folderId;

		private readonly int priority;
	}
}
