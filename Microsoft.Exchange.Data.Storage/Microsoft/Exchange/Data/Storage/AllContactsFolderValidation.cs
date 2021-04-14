using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class AllContactsFolderValidation : SearchFolderValidation
	{
		internal AllContactsFolderValidation() : base(new IValidator[]
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

		private static QueryFilter GetQueryFilter(DefaultFolderContext context)
		{
			return new AndFilter(new QueryFilter[]
			{
				SearchFolderValidation.GetSearchExclusionFoldersFilter(context, null, AllContactsFolderValidation.ExcludedFolders),
				new OrFilter(new QueryFilter[]
				{
					new ComparisonFilter(ComparisonOperator.Equal, StoreObjectSchema.ItemClass, "IPM.Contact"),
					new ComparisonFilter(ComparisonOperator.Equal, StoreObjectSchema.ItemClass, "IPM.DistList")
				})
			});
		}

		private SearchFolderCriteria CreateSearchCriteria(DefaultFolderContext context)
		{
			return new SearchFolderCriteria(AllContactsFolderValidation.GetQueryFilter(context), new StoreId[]
			{
				context[DefaultFolderType.Root]
			})
			{
				DeepTraversal = true
			};
		}

		private static readonly DefaultFolderType[] ExcludedFolders = new DefaultFolderType[]
		{
			DefaultFolderType.Conflicts,
			DefaultFolderType.DeletedItems,
			DefaultFolderType.Drafts,
			DefaultFolderType.Inbox,
			DefaultFolderType.JunkEmail,
			DefaultFolderType.LocalFailures,
			DefaultFolderType.Outbox,
			DefaultFolderType.SentItems,
			DefaultFolderType.ServerFailures,
			DefaultFolderType.SyncIssues,
			DefaultFolderType.DocumentSyncIssues
		};
	}
}
