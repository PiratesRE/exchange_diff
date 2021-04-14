using System;
using System.Linq;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class FromPeopleFolderValidation : SearchFolderValidation
	{
		internal FromPeopleFolderValidation() : base(new IValidator[]
		{
			new MatchMapiFolderType(FolderType.Search)
		})
		{
		}

		internal override bool EnsureIsValid(DefaultFolderContext context, Folder folder)
		{
			FromPeopleFolderValidation.Tracer.TraceFunction((long)this.GetHashCode(), "Entering FromPeopleFolderValidation.EnsureIsValid");
			if (!base.EnsureIsValid(context, folder))
			{
				FromPeopleFolderValidation.Tracer.TraceFunction((long)this.GetHashCode(), "Exiting FromPeopleFolderValidation.EnsureIsValid:  folder failed base class validation.");
				return false;
			}
			SearchFolder searchFolder = folder as SearchFolder;
			if (searchFolder == null)
			{
				FromPeopleFolderValidation.Tracer.TraceFunction((long)this.GetHashCode(), "Exiting FromPeopleFolderValidation.Validate:  not a SearchFolder instance.");
				return false;
			}
			SearchFolderCriteria searchFolderCriteria = SearchFolderValidation.TryGetSearchCriteria(searchFolder);
			SearchFolderCriteria searchCriteria = FromPeopleFolderValidation.GetSearchCriteria(context);
			if (searchFolderCriteria == null || !SearchFolderValidation.MatchSearchFolderCriteria(searchFolderCriteria, searchCriteria))
			{
				FromPeopleFolderValidation.Tracer.TraceDebug((long)this.GetHashCode(), "Current criteria is not initialized or doesn't match desired criteria.  Updating.");
				searchFolder.ApplyContinuousSearch(searchCriteria);
			}
			FromPeopleFolderValidation.Tracer.TraceFunction((long)this.GetHashCode(), "Exiting FromPeopleFolderValidation.EnsureIsValid.  Validation is done.");
			return true;
		}

		protected override void SetPropertiesInternal(DefaultFolderContext context, Folder folder)
		{
			FromPeopleFolderValidation.Tracer.TraceFunction((long)this.GetHashCode(), "FromPeopleFolderValidation.SetPropertiesInternal");
			base.SetPropertiesInternal(context, folder);
			folder.Save();
			SearchFolder searchFolder = (SearchFolder)folder;
			searchFolder.ApplyContinuousSearch(FromPeopleFolderValidation.GetSearchCriteria(context));
			folder.Load(null);
			FromPeopleFolderValidation.Tracer.TraceFunction((long)this.GetHashCode(), "Exiting FromPeopleFolderValidation.SetPropertiesInternal.  Initialization is done.");
		}

		private static SearchFolderCriteria GetSearchCriteria(DefaultFolderContext context)
		{
			return new SearchFolderCriteria(FromPeopleFolderValidation.SearchQueryFilter, (from defaultFolderType in FromPeopleFolderValidation.FolderScope
			select context.Session.GetDefaultFolderId(defaultFolderType)).ToArray<StoreObjectId>());
		}

		private static readonly Trace Tracer = ExTraceGlobals.StorageTracer;

		private static readonly DefaultFolderType[] FolderScope = new DefaultFolderType[]
		{
			DefaultFolderType.Inbox,
			DefaultFolderType.SentItems
		};

		private static readonly ExistsFilter ExchangeApplicationFlagsExists = new ExistsFilter(ItemSchema.ExchangeApplicationFlags);

		private static readonly BitMaskFilter IsFromPersonFilter = new BitMaskFilter(ItemSchema.ExchangeApplicationFlags, 2048UL, true);

		private static readonly BitMaskFilter IsFromFavoriteSenderFilter = new BitMaskFilter(ItemSchema.ExchangeApplicationFlags, 1UL, true);

		private static readonly QueryFilter SearchQueryFilter = new AndFilter(new QueryFilter[]
		{
			FromPeopleFolderValidation.ExchangeApplicationFlagsExists,
			new OrFilter(new QueryFilter[]
			{
				FromPeopleFolderValidation.IsFromPersonFilter,
				FromPeopleFolderValidation.IsFromFavoriteSenderFilter
			})
		});
	}
}
