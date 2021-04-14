using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal class GetFavorites : ServiceCommand<GetFavoritesResponse>
	{
		public GetFavorites(CallContext context) : base(context)
		{
		}

		protected override GetFavoritesResponse InternalExecute()
		{
			FavoriteFolderCollection favoritesCollection = FavoriteFolderCollection.GetFavoritesCollection(base.MailboxIdentityMailboxSession, FolderTreeDataSection.First);
			return new GetFavoritesResponse
			{
				Favorites = favoritesCollection.FavoriteFolders
			};
		}
	}
}
