using System;
using System.Diagnostics;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Services.Core.Search;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Diagnostics;

namespace Microsoft.Exchange.Services.Core
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class BrowsePeopleInPublicFolder : FindPeopleImplementation
	{
		public BrowsePeopleInPublicFolder(FindPeopleParameters parameters, IdAndSession idAndSession) : base(parameters, null, true)
		{
			ServiceCommandBase.ThrowIfNull(idAndSession, "idAndSession", "BrowsePeopleInPublicFolder::BrowsePeopleInPublicFolder");
			this.idAndSession = idAndSession;
		}

		public override void Validate()
		{
			this.ValidatePaging();
		}

		protected override void ValidatePaging()
		{
			base.ValidatePaging();
			if (!(base.Paging is IndexedPageView))
			{
				throw new ServiceArgumentException(CoreResources.IDs.ErrorInvalidIndexedPagingParameters);
			}
		}

		public override FindPeopleResult Execute()
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			FindPeopleResult findPeopleResult = this.ExecuteInternal();
			stopwatch.Stop();
			base.Log(FindPeopleMetadata.PersonalSearchTime, stopwatch.ElapsedMilliseconds);
			base.Log(FindPeopleMetadata.PersonalCount, findPeopleResult.PersonaList.Length);
			base.Log(FindPeopleMetadata.TotalNumberOfPeopleInView, findPeopleResult.TotalNumberOfPeopleInView);
			return findPeopleResult;
		}

		private FindPeopleResult ExecuteInternal()
		{
			SortBy[] sortBy = Microsoft.Exchange.Services.Core.Search.SortResults.ToXsoSortBy(base.SortResults);
			FindPeopleResult result;
			using (Folder folder = Folder.Bind(this.idAndSession.Session, this.idAndSession.Id, null))
			{
				result = FindPeopleImplementation.QueryContactsInPublicFolder((PublicFolderSession)this.idAndSession.Session, folder, sortBy, (IndexedPageView)base.Paging, null);
			}
			return result;
		}

		private readonly IdAndSession idAndSession;
	}
}
