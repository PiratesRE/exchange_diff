using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class CreateContactFolder : ServiceCommand<ContactFolderResponse>
	{
		public CreateContactFolder(CallContext callContext, BaseFolderId parentFolderId, string displayName, int priority) : base(callContext)
		{
			this.parentFolderId = parentFolderId;
			this.displayName = displayName;
			this.priority = priority;
		}

		protected override ContactFolderResponse InternalExecute()
		{
			ExchangeVersion.Current = ExchangeVersion.Latest;
			MailboxSession mailboxIdentityMailboxSession = base.MailboxIdentityMailboxSession;
			IdAndSession idAndSession = base.IdConverter.ConvertFolderIdToIdAndSession(this.parentFolderId, IdConverter.ConvertOption.IgnoreChangeKey);
			StoreId id;
			try
			{
				using (Folder folder = ContactsFolder.Create(mailboxIdentityMailboxSession, idAndSession.Id, StoreObjectType.ContactsFolder, this.displayName, CreateMode.CreateNew))
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
			catch (ObjectExistedException)
			{
				return new ContactFolderResponse
				{
					ResponseCode = ResponseCodeType.ErrorFolderExists.ToString()
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

		private readonly BaseFolderId parentFolderId;

		private readonly string displayName;

		private readonly int priority;
	}
}
