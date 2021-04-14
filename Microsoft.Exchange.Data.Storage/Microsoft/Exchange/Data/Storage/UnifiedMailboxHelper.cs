using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class UnifiedMailboxHelper
	{
		internal static QueryFilter CreateQueryFilter(DefaultFolderType defaultFolderType)
		{
			if (defaultFolderType == DefaultFolderType.UnifiedInbox)
			{
				return UnifiedMailboxHelper.UnifiedInboxQueryFilter;
			}
			return null;
		}

		internal static DefaultFolderType[] GetSearchScopeForDefaultSearchFolder(DefaultFolderType defaultFolderType)
		{
			if (defaultFolderType == DefaultFolderType.UnifiedInbox)
			{
				return new DefaultFolderType[]
				{
					DefaultFolderType.Inbox,
					DefaultFolderType.Drafts
				};
			}
			return Array<DefaultFolderType>.Empty;
		}

		internal static readonly DefaultFolderType[] DefaultSearchFolderTypesSupportedForUnifiedViews = new DefaultFolderType[]
		{
			DefaultFolderType.UnifiedInbox
		};

		private static readonly ComparisonFilter NonDraftMessageQueryFilter = new ComparisonFilter(ComparisonOperator.Equal, InternalSchema.IsDraft, false);

		private static readonly QueryFilter DraftMessageQueryFilter = new ComparisonFilter(ComparisonOperator.Equal, InternalSchema.IsDraft, true);

		private static readonly QueryFilter NonActionMessageQueryFilter = new OrFilter(new QueryFilter[]
		{
			new ComparisonFilter(ComparisonOperator.Equal, InternalSchema.SubjectPrefix, null),
			new ComparisonFilter(ComparisonOperator.Equal, InternalSchema.SubjectPrefix, string.Empty)
		});

		private static readonly QueryFilter DraftNewConversationMessageQueryFilter = new AndFilter(new QueryFilter[]
		{
			UnifiedMailboxHelper.DraftMessageQueryFilter,
			UnifiedMailboxHelper.NonActionMessageQueryFilter
		});

		private static readonly QueryFilter UnifiedInboxQueryFilter = new OrFilter(new QueryFilter[]
		{
			UnifiedMailboxHelper.NonDraftMessageQueryFilter,
			UnifiedMailboxHelper.DraftNewConversationMessageQueryFilter
		});
	}
}
