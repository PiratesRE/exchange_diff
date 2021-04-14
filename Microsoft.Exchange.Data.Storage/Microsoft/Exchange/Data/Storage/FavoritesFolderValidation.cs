using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class FavoritesFolderValidation : SearchFolderValidation
	{
		internal FavoritesFolderValidation() : base(new IValidator[]
		{
			new MatchMapiFolderType(FolderType.Search)
		})
		{
		}

		protected override void SetPropertiesInternal(DefaultFolderContext context, Folder folder)
		{
			base.SetPropertiesInternal(context, folder);
			folder.Save();
			SearchFolder searchFolder = (SearchFolder)folder;
			searchFolder.ApplyContinuousSearch(this.CreateSearchCriteria(context));
			folder.Load(null);
		}

		private static QueryFilter GetQueryFilter()
		{
			return new AndFilter(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.Equal, StoreObjectSchema.ItemClass, "IPM.Contact"),
				new ComparisonFilter(ComparisonOperator.Equal, ContactSchema.IsFavorite, true)
			});
		}

		private SearchFolderCriteria CreateSearchCriteria(DefaultFolderContext context)
		{
			return new SearchFolderCriteria(FavoritesFolderValidation.GetQueryFilter(), new StoreId[]
			{
				context[DefaultFolderType.QuickContacts]
			});
		}
	}
}
