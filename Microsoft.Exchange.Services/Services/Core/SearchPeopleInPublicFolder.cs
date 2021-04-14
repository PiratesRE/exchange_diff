using System;
using System.Diagnostics;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Diagnostics;

namespace Microsoft.Exchange.Services.Core
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class SearchPeopleInPublicFolder : FindPeopleImplementation
	{
		public SearchPeopleInPublicFolder(FindPeopleParameters parameters, IdAndSession idAndSession) : base(parameters, SearchPeopleStrategy.AdditionalSupportedProperties, false)
		{
			ServiceCommandBase.ThrowIfNull(parameters, "parameters", "SearchPeopleInPublicFolder::SearchPeopleInPublicFolder");
			ServiceCommandBase.ThrowIfNull(idAndSession, "idAndSession", "SearchPeopleInPublicFolder::SearchPeopleInPublicFolder");
			this.parameters = parameters;
			this.idAndSession = idAndSession;
		}

		public override FindPeopleResult Execute()
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			Persona[] array = this.ExecuteInternal();
			stopwatch.Stop();
			base.Log(FindPeopleMetadata.PersonalSearchTime, stopwatch.ElapsedMilliseconds);
			base.Log(FindPeopleMetadata.PersonalCount, array.Length);
			return FindPeopleResult.CreateSearchResult(array);
		}

		private Persona[] ExecuteInternal()
		{
			QueryFilter andValidateRestrictionFilter = base.GetAndValidateRestrictionFilter();
			PublicFolderSession session = (PublicFolderSession)this.idAndSession.Session;
			SearchPeopleInPublicFolderStrategy searchPeopleInPublicFolderStrategy = new SearchPeopleInPublicFolderStrategy(session, this.parameters, this.idAndSession.Id, andValidateRestrictionFilter);
			return searchPeopleInPublicFolderStrategy.Execute();
		}

		private readonly FindPeopleParameters parameters;

		private readonly IdAndSession idAndSession;
	}
}
