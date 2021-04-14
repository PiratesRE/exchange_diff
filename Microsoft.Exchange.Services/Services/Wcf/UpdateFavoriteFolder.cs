using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal class UpdateFavoriteFolder : ServiceCommand<UpdateFavoriteFolderResponse>
	{
		public UpdateFavoriteFolder(CallContext context, UpdateFavoriteFolderRequest request) : base(context)
		{
			this.request = request;
		}

		protected override UpdateFavoriteFolderResponse InternalExecute()
		{
			if (this.request.Folder == null)
			{
				ExTraceGlobals.UpdateFavoriteFolderCallTracer.TraceError((long)this.GetHashCode(), CoreResources.UpdateFavoritesFolderCannotBeNull);
				return new UpdateFavoriteFolderResponse(CoreResources.UpdateFavoritesFolderCannotBeNull);
			}
			MailboxSession mailboxIdentityMailboxSession = base.MailboxIdentityMailboxSession;
			this.favoriteFolderCollection = FavoriteFolderCollection.GetFavoritesCollection(mailboxIdentityMailboxSession, FolderTreeDataSection.None);
			if (this.request.Folder.FolderId.IsPublicFolderId())
			{
				this.request.Folder.FolderId = this.ConvertPrivateStoreIdToPublicStoreId(this.request.Folder.FolderId);
				if (this.request.TargetFolderId != null && this.request.TargetFolderId.IsPublicFolderId())
				{
					this.request.TargetFolderId = this.ConvertPrivateStoreIdToPublicStoreId(this.request.TargetFolderId);
				}
			}
			switch (this.request.Operation)
			{
			case UpdateFavoriteOperationType.Add:
				return this.favoriteFolderCollection.AddFavoriteFolder(this.request.Folder, this.request.TargetFolderId, this.request.MoveType, base.IdConverter);
			case UpdateFavoriteOperationType.Remove:
				return this.favoriteFolderCollection.RemoveFavoriteFolder(this.request.Folder);
			case UpdateFavoriteOperationType.Move:
				return this.favoriteFolderCollection.MoveFavoriteFolder(this.request.Folder, this.request.TargetFolderId, this.request.MoveType);
			case UpdateFavoriteOperationType.Rename:
				return this.favoriteFolderCollection.RenameFavoriteFolder(this.request.Folder);
			default:
				return new UpdateFavoriteFolderResponse(string.Format(CoreResources.UpdateFavoritesInvalidUpdateFavoriteOperationType, this.request.Operation));
			}
		}

		private FolderId ConvertPrivateStoreIdToPublicStoreId(FolderId folderId)
		{
			StoreObjectId storeObjectId = base.IdConverter.ConvertFolderIdToIdAndSessionReadOnly(folderId).GetAsStoreObjectId();
			long idFromLongTermId = base.MailboxIdentityMailboxSession.IdConverter.GetIdFromLongTermId(storeObjectId.LongTermFolderId);
			storeObjectId = base.MailboxIdentityMailboxSession.IdConverter.CreatePublicFolderId(idFromLongTermId);
			ConcatenatedIdAndChangeKey concatenatedIdForPublicFolder = IdConverter.GetConcatenatedIdForPublicFolder(storeObjectId);
			string changeKey;
			if (!string.IsNullOrEmpty(folderId.ChangeKey))
			{
				changeKey = folderId.ChangeKey;
			}
			else
			{
				changeKey = concatenatedIdForPublicFolder.ChangeKey;
			}
			return new FolderId(concatenatedIdForPublicFolder.Id, changeKey);
		}

		private const int FolderLongTermIdLength = 22;

		private const int FolderLongTermIdIndex = 22;

		private UpdateFavoriteFolderRequest request;

		private FavoriteFolderCollection favoriteFolderCollection;
	}
}
