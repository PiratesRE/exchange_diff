using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class FromFavoriteSendersFolderValidation : SearchFolderValidation
	{
		internal FromFavoriteSendersFolderValidation() : base(new IValidator[]
		{
			new MatchMapiFolderType(FolderType.Search)
		})
		{
		}

		internal override bool EnsureIsValid(DefaultFolderContext context, Folder folder)
		{
			FromFavoriteSendersFolderValidation.Tracer.TraceFunction((long)this.GetHashCode(), "Entering FromFavoriteSendersFolderValidation.EnsureIsValid");
			if (!base.EnsureIsValid(context, folder))
			{
				FromFavoriteSendersFolderValidation.Tracer.TraceFunction((long)this.GetHashCode(), "Exiting FromFavoriteSendersFolderValidation.EnsureIsValid:  folder failed base class validation.");
				return false;
			}
			SearchFolder searchFolder = folder as SearchFolder;
			if (searchFolder == null)
			{
				FromFavoriteSendersFolderValidation.Tracer.TraceFunction((long)this.GetHashCode(), "Exiting FromFavoriteSendersFolderValidation.Validate:  not a SearchFolder instance.");
				return false;
			}
			SearchFolderCriteria searchFolderCriteria = SearchFolderValidation.TryGetSearchCriteria(searchFolder);
			SearchFolderCriteria searchCriteria = FromFavoriteSendersFolderValidation.GetSearchCriteria(context);
			if (searchFolderCriteria == null || !SearchFolderValidation.MatchSearchFolderCriteria(searchFolderCriteria, searchCriteria) || searchFolderCriteria.DeepTraversal != searchCriteria.DeepTraversal)
			{
				FromFavoriteSendersFolderValidation.Tracer.TraceDebug((long)this.GetHashCode(), "Current criteria are NOT initialized or don't match desired criteria.  Updating.");
				searchFolder.ApplyContinuousSearch(searchCriteria);
			}
			FromFavoriteSendersFolderValidation.Tracer.TraceFunction((long)this.GetHashCode(), "Exiting FromFavoriteSendersFolderValidation.EnsureIsValid.  Validation is done.");
			return true;
		}

		protected override void SetPropertiesInternal(DefaultFolderContext context, Folder folder)
		{
			FromFavoriteSendersFolderValidation.Tracer.TraceFunction((long)this.GetHashCode(), "FromFavoriteSendersFolderValidation.SetPropertiesInternal");
			base.SetPropertiesInternal(context, folder);
			folder.Save();
			SearchFolder searchFolder = (SearchFolder)folder;
			searchFolder.ApplyContinuousSearch(FromFavoriteSendersFolderValidation.GetSearchCriteria(context));
			folder.Load(null);
			FromFavoriteSendersFolderValidation.Tracer.TraceFunction((long)this.GetHashCode(), "Exiting FromFavoriteSendersFolderValidation.SetPropertiesInternal.  Initialization is done.");
		}

		private static SearchFolderCriteria GetSearchCriteria(DefaultFolderContext context)
		{
			return new SearchFolderCriteria(FromFavoriteSendersFolderValidation.SearchQueryFilter, new StoreId[]
			{
				context.Session.GetDefaultFolderId(DefaultFolderType.Inbox)
			});
		}

		public const DefaultFolderType FolderScope = DefaultFolderType.Inbox;

		private static readonly Trace Tracer = ExTraceGlobals.StorageTracer;

		private static readonly ExistsFilter ExchangeApplicationFlagsExists = new ExistsFilter(ItemSchema.ExchangeApplicationFlags);

		private static readonly BitMaskFilter IsFromFavoriteSenderFilter = new BitMaskFilter(ItemSchema.ExchangeApplicationFlags, 1UL, true);

		public static readonly QueryFilter SearchQueryFilter = new AndFilter(new QueryFilter[]
		{
			FromFavoriteSendersFolderValidation.ExchangeApplicationFlagsExists,
			FromFavoriteSendersFolderValidation.IsFromFavoriteSenderFilter
		});
	}
}
