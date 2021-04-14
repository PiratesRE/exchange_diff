using System;
using Microsoft.Exchange.Configuration.Authorization;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.EDiscovery.MailboxSearch.WebService.Infrastructure;
using Microsoft.Exchange.InfoWorker.Common.MultiMailboxSearch;

namespace Microsoft.Exchange.EDiscovery.MailboxSearch.WebService.Model
{
	internal interface ISearchPolicy
	{
		CallerInfo CallerInfo { get; }

		IRecipientSession RecipientSession { get; }

		ExchangeRunspaceConfiguration RunspaceConfiguration { get; }

		IThrottlingSettings ThrottlingSettings { get; }

		IExecutionSettings ExecutionSettings { get; }

		IBudget Budget { get; }

		Recorder Recorder { get; }

		ActivityScope GetActivityScope();
	}
}
