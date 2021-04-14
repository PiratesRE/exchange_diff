using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class AllItemsFolderValidation : SearchFolderValidation
	{
		internal AllItemsFolderValidation() : base(new IValidator[]
		{
			new MatchContainerClass("IPF"),
			new MatchMapiFolderType(FolderType.Search)
		})
		{
		}

		internal override bool EnsureIsValid(DefaultFolderContext context, Folder folder)
		{
			return base.EnsureIsValid(context, folder) && AllItemsFolderValidation.VerifyAndFixSearchFolder(context, (SearchFolder)folder);
		}

		private static void TryApplyContinuousSearch(SearchFolder folder, SearchFolderCriteria criteria)
		{
			try
			{
				folder.ApplyContinuousSearch(criteria);
			}
			catch (QueryInProgressException)
			{
				SearchFolderCriteria searchFolderCriteria = SearchFolderValidation.TryGetSearchCriteria(folder);
				if (searchFolderCriteria == null || !SearchFolderValidation.MatchSearchFolderCriteria(criteria, searchFolderCriteria))
				{
					throw;
				}
			}
		}

		protected override void SetPropertiesInternal(DefaultFolderContext context, Folder folder)
		{
			base.SetPropertiesInternal(context, folder);
			SearchFolder searchFolder = (SearchFolder)folder;
			searchFolder.Save();
			searchFolder.Load(null);
			AllItemsFolderHelper.InitializeTransportIndexes(folder);
			AllItemsFolderValidation.TryApplyContinuousSearch(searchFolder, AllItemsFolderValidation.CreateSearchCriteria(context));
		}

		private static SearchFolderCriteria CreateSearchCriteria(DefaultFolderContext context)
		{
			return new SearchFolderCriteria(new ExistsFilter(InternalSchema.ItemClass), new StoreId[]
			{
				context.Session.GetDefaultFolderId(DefaultFolderType.Root)
			})
			{
				DeepTraversal = true
			};
		}

		private static bool VerifyAndFixSearchFolder(DefaultFolderContext context, SearchFolder folder)
		{
			SearchFolderCriteria searchFolderCriteria = SearchFolderValidation.TryGetSearchCriteria(folder);
			SearchFolderCriteria searchFolderCriteria2 = AllItemsFolderValidation.CreateSearchCriteria(context);
			if (searchFolderCriteria == null || !SearchFolderValidation.MatchSearchFolderCriteria(searchFolderCriteria, searchFolderCriteria2))
			{
				AllItemsFolderValidation.TryApplyContinuousSearch(folder, searchFolderCriteria2);
			}
			return true;
		}
	}
}
