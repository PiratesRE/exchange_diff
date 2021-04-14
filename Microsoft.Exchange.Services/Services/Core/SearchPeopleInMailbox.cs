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
	internal sealed class SearchPeopleInMailbox : FindPeopleImplementation
	{
		public SearchPeopleInMailbox(FindPeopleParameters parameters, IdAndSession idAndSession, AggregationExtension aggregationExtension) : base(parameters, SearchPeopleStrategy.AdditionalSupportedProperties, false)
		{
			ServiceCommandBase.ThrowIfNull(parameters, "parameters", "SearchPeopleInMailbox::SearchPeopleInMailbox");
			ServiceCommandBase.ThrowIfNull(idAndSession, "idAndSession", "SearchPeopleInMailbox::SearchPeopleInMailbox");
			ServiceCommandBase.ThrowIfNull(aggregationExtension, "aggregationExtension", "SearchPeopleInMailbox::SearchPeopleInMailbox");
			this.parameters = parameters;
			this.idAndSession = idAndSession;
			this.aggregationExtension = aggregationExtension;
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
			MailboxSession mailboxSession = (MailboxSession)this.idAndSession.Session;
			SearchPeopleInMailboxStrategy searchPeopleInMailboxStrategy = new SearchPeopleInMailboxStrategy(this.parameters, mailboxSession, this.idAndSession.Id, andValidateRestrictionFilter, base.GetAggregationRestrictionFilter(), this.aggregationExtension);
			return searchPeopleInMailboxStrategy.Execute();
		}

		private readonly FindPeopleParameters parameters;

		private readonly IdAndSession idAndSession;

		private readonly AggregationExtension aggregationExtension;
	}
}
