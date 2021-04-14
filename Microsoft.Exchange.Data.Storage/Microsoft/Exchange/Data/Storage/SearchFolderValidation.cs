using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal abstract class SearchFolderValidation : DefaultFolderValidator
	{
		internal SearchFolderValidation(params IValidator[] validators) : base(validators)
		{
		}

		internal static SearchFolderCriteria TryGetSearchCriteria(SearchFolder folder)
		{
			SearchFolderCriteria result = null;
			try
			{
				result = folder.GetSearchCriteria();
			}
			catch (ObjectNotInitializedException)
			{
			}
			catch (CorruptDataException)
			{
			}
			return result;
		}

		internal static QueryFilter GetSearchExclusionFoldersFilter(DefaultFolderContext context, IEnumerable<QueryFilter> currentExclusionCriteria, DefaultFolderType[] excludedDefaultFolders)
		{
			List<QueryFilter> list = new List<QueryFilter>();
			if (currentExclusionCriteria != null)
			{
				list.AddRange(currentExclusionCriteria);
			}
			foreach (DefaultFolderType defaultFolderType in excludedDefaultFolders)
			{
				StoreObjectId storeObjectId = context[defaultFolderType];
				if (storeObjectId != null)
				{
					ComparisonFilter item = new ComparisonFilter(ComparisonOperator.NotEqual, InternalSchema.ParentItemId, storeObjectId);
					if (!list.Contains(item))
					{
						list.Add(item);
					}
				}
			}
			return new AndFilter(list.ToArray());
		}

		protected static bool MatchSearchFolderCriteria(SearchFolderCriteria currentCriteria, SearchFolderCriteria expectedCriteria)
		{
			if (currentCriteria.FolderScope.Length != expectedCriteria.FolderScope.Length)
			{
				return false;
			}
			for (int i = 0; i < expectedCriteria.FolderScope.Length; i++)
			{
				bool flag = false;
				for (int j = 0; j < currentCriteria.FolderScope.Length; j++)
				{
					if (currentCriteria.FolderScope[j].Equals(expectedCriteria.FolderScope[i]))
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					ExTraceGlobals.DefaultFoldersTracer.TraceError<int, string>(-1L, "SearchFolderValidation::MatchSearchFolderCriteria. We failed to find the folder scope from the current search criteria. Index = {0}, Id = {1}.", i, expectedCriteria.FolderScope[i].ToBase64String());
					return false;
				}
			}
			return currentCriteria.SearchQuery.Equals(expectedCriteria.SearchQuery);
		}

		internal static readonly DefaultFolderType[] ExcludeFromRemindersSearchFolder = new DefaultFolderType[]
		{
			DefaultFolderType.Conflicts,
			DefaultFolderType.LocalFailures,
			DefaultFolderType.ServerFailures,
			DefaultFolderType.SyncIssues,
			DefaultFolderType.DeletedItems,
			DefaultFolderType.JunkEmail,
			DefaultFolderType.Outbox,
			DefaultFolderType.Drafts,
			DefaultFolderType.DocumentSyncIssues
		};
	}
}
