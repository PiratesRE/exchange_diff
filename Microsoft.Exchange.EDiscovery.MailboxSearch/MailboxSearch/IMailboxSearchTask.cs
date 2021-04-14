using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage.Infoworker.MailboxSearch;
using Microsoft.Exchange.EDiscovery.Export;

namespace Microsoft.Exchange.EDiscovery.MailboxSearch
{
	internal interface IMailboxSearchTask : IDisposable
	{
		EventHandler<ExportStatusEventArgs> OnReportStatistics { get; set; }

		Action<int, long, long, long, List<KeywordHit>> OnEstimateCompleted { get; set; }

		Action<ISearchResults> OnPrepareCompleted { get; set; }

		Action OnExportCompleted { get; set; }

		IExportContext ExportContext { get; }

		ISearchResults SearchResults { get; }

		SearchState CurrentState { get; }

		IList<string> Errors { get; }

		ITargetMailbox TargetMailbox { get; }

		void Abort();

		void Start();
	}
}
