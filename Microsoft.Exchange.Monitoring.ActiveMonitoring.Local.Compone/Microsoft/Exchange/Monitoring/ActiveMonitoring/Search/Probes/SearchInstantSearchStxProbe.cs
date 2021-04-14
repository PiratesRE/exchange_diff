using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Search.Engine;
using Microsoft.Exchange.Search.OperatorSchema;
using Microsoft.Exchange.Search.Query;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Search.Probes
{
	public class SearchInstantSearchStxProbe : SearchQueryStxProbeBase
	{
		protected override void InternalDoWork(CancellationToken cancellationToken)
		{
			this.InitializeAttributes();
			string targetResource = base.Definition.TargetResource;
			Guid databaseGuidFromName = SearchMonitoringHelper.GetDatabaseGuidFromName(targetResource);
			SearchConfig searchConfig = (SearchConfig)Factory.Current.CreateSearchServiceConfig(databaseGuidFromName);
			if (searchConfig.DisableInstantSearch)
			{
				base.Result.StateAttribute1 = "Disabled";
				base.Result.StateAttribute4 = "Disabled";
				return;
			}
			base.CheckSimpleQueryMode(targetResource);
			try
			{
				using (MailboxSession mailboxSession = SearchStoreHelper.GetMailboxSession(base.MonitoringMailboxSmtpAddress, false, "Monitoring"))
				{
					int queryHitCount = this.GetQueryHitCount(mailboxSession, "subject:SearchQueryStxProbe", 3);
					ExDateTime exDateTime;
					if (queryHitCount > 0)
					{
						base.Result.StateAttribute1 = Strings.SearchQueryStxSuccess(queryHitCount, "subject:SearchQueryStxProbe", base.MonitoringMailboxSmtpAddress);
						base.Result.StateAttribute4 = "Healthy";
					}
					else if (base.CheckExistenceAndCreateMessage(mailboxSession, out exDateTime))
					{
						base.Result.StateAttribute4 = "Failed";
						throw new SearchProbeFailureException(Strings.SearchInstantSearchStxZeroHitMonitoringMailbox("subject:SearchQueryStxProbe", base.MonitoringMailboxSmtpAddress, exDateTime.ToString()));
					}
				}
			}
			catch (ConnectionFailedTransientException ex)
			{
				base.Result.StateAttribute1 = ex.GetType().Name;
			}
			catch (InstantSearchPermanentException ex2)
			{
				throw new SearchProbeFailureException(Strings.SearchInstantSearchStxException("subject:SearchQueryStxProbe", base.MonitoringMailboxSmtpAddress, ex2.ToString(), ex2.QueryStatistics.ToString()));
			}
			catch (InstantSearchTransientException ex3)
			{
				throw new SearchProbeFailureException(Strings.SearchInstantSearchStxException("subject:SearchQueryStxProbe", base.MonitoringMailboxSmtpAddress, ex3.ToString(), ex3.QueryStatistics.ToString()));
			}
		}

		internal int GetQueryHitCount(MailboxSession mailboxSession, string query, int maxResultsCount)
		{
			List<IReadOnlyPropertyBag> results = new List<IReadOnlyPropertyBag>();
			using (InstantSearch instantSearch = new InstantSearch(mailboxSession, new List<StoreId>
			{
				mailboxSession.GetDefaultFolderId(DefaultFolderType.Inbox)
			}, null, Guid.NewGuid()))
			{
				IAsyncResult asyncResult3 = instantSearch.BeginStartSession(null, null);
				instantSearch.EndStartSession(asyncResult3);
				instantSearch.FastQueryPath = true;
				InstantSearchQueryParameters instantSearchQueryParameters = new InstantSearchQueryParameters("subject:SearchQueryStxProbe", null, QueryOptions.Results);
				instantSearch.ResultsUpdatedCallback = delegate(IReadOnlyCollection<IReadOnlyPropertyBag> page, ICancelableAsyncResult asyncResult)
				{
					results.Clear();
					results.AddRange(page);
				};
				instantSearchQueryParameters.MaximumResultCount = new int?(3);
				ICancelableAsyncResult asyncResult2 = instantSearch.BeginInstantSearchRequest(instantSearchQueryParameters, null, null);
				instantSearch.EndInstantSearchRequest(asyncResult2);
				asyncResult3 = instantSearch.BeginStopSession(null, null);
				instantSearch.EndStopSession(asyncResult3);
			}
			return results.Count;
		}
	}
}
