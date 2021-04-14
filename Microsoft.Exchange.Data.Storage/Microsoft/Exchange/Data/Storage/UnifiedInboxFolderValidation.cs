using System;
using System.Linq;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class UnifiedInboxFolderValidation : SearchFolderValidation
	{
		internal UnifiedInboxFolderValidation() : base(new IValidator[]
		{
			new MatchMapiFolderType(FolderType.Search)
		})
		{
		}

		internal override bool EnsureIsValid(DefaultFolderContext context, Folder folder)
		{
			UnifiedInboxFolderValidation.Tracer.TraceFunction((long)this.GetHashCode(), "Entering UnifiedInboxFolderValidation.EnsureIsValid");
			if (!base.EnsureIsValid(context, folder))
			{
				UnifiedInboxFolderValidation.Tracer.TraceFunction((long)this.GetHashCode(), "Exiting UnifiedInboxFolderValidation.EnsureIsValid: folder failed base class validation.");
				return false;
			}
			SearchFolder searchFolder = folder as SearchFolder;
			if (searchFolder == null)
			{
				UnifiedInboxFolderValidation.Tracer.TraceFunction((long)this.GetHashCode(), "Exiting UnifiedInboxFolderValidation.Validate: not a SearchFolder instance.");
				return false;
			}
			SearchFolderCriteria searchFolderCriteria = SearchFolderValidation.TryGetSearchCriteria(searchFolder);
			SearchFolderCriteria searchCriteria = UnifiedInboxFolderValidation.GetSearchCriteria(context);
			if (searchFolderCriteria == null || !SearchFolderValidation.MatchSearchFolderCriteria(searchFolderCriteria, searchCriteria) || searchFolderCriteria.DeepTraversal != searchCriteria.DeepTraversal)
			{
				UnifiedInboxFolderValidation.Tracer.TraceDebug((long)this.GetHashCode(), "Current criteria are NOT initialized or don't match desired criteria. Updating.");
				searchFolder.ApplyContinuousSearch(searchCriteria);
			}
			UnifiedInboxFolderValidation.Tracer.TraceFunction((long)this.GetHashCode(), "Exiting UnifiedInboxFolderValidation.EnsureIsValid.  Validation is done.");
			return true;
		}

		protected override void SetPropertiesInternal(DefaultFolderContext context, Folder folder)
		{
			UnifiedInboxFolderValidation.Tracer.TraceFunction((long)this.GetHashCode(), "UnifiedInboxFolderValidation.SetPropertiesInternal");
			base.SetPropertiesInternal(context, folder);
			folder.Save();
			SearchFolder searchFolder = (SearchFolder)folder;
			searchFolder.ApplyContinuousSearch(UnifiedInboxFolderValidation.GetSearchCriteria(context));
			folder.Load(null);
			UnifiedInboxFolderValidation.Tracer.TraceFunction((long)this.GetHashCode(), "Exiting UnifiedInboxFolderValidation.SetPropertiesInternal. Initialization is done.");
		}

		private static SearchFolderCriteria GetSearchCriteria(DefaultFolderContext context)
		{
			return new SearchFolderCriteria(UnifiedInboxFolderValidation.SearchQueryFilter, (from defaultFolderType in UnifiedInboxFolderValidation.FolderScope
			select context.Session.GetDefaultFolderId(defaultFolderType)).ToArray<StoreObjectId>());
		}

		private static readonly Trace Tracer = ExTraceGlobals.StorageTracer;

		private static readonly DefaultFolderType DefaultFolderType = DefaultFolderType.UnifiedInbox;

		public static readonly DefaultFolderType[] FolderScope = UnifiedMailboxHelper.GetSearchScopeForDefaultSearchFolder(UnifiedInboxFolderValidation.DefaultFolderType);

		public static readonly QueryFilter SearchQueryFilter = UnifiedMailboxHelper.CreateQueryFilter(UnifiedInboxFolderValidation.DefaultFolderType);
	}
}
