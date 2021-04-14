using System;
using System.Linq;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.EDiscovery.MailboxSearch.WebService.Infrastructure;
using Microsoft.Exchange.EDiscovery.MailboxSearch.WebService.Model;

namespace Microsoft.Exchange.EDiscovery.MailboxSearch.WebService.Task
{
	internal class InitializeGetSearchablebleMailbox : SearchTask<GetSearchableMailboxesInputs>
	{
		public override void Process(GetSearchableMailboxesInputs item)
		{
			Recorder.Trace(4L, TraceType.InfoTrace, new object[]
			{
				"InitializeGetSearchablebleMailbox.Process Item:",
				item,
				"Filter:",
				item.Filter,
				"ExpandGroups:",
				item.ExpandGroups
			});
			SearchSource[] source = new SearchSource[]
			{
				new SearchSource
				{
					ReferenceId = item.Filter,
					SourceType = SourceType.AllMailboxes
				}
			};
			DirectoryQueryParameters item2 = new DirectoryQueryParameters
			{
				ExpandPublicFolders = false,
				ExpandGroups = item.ExpandGroups,
				MatchRecipientsToSources = false,
				PageSize = (int)base.Policy.ExecutionSettings.DiscoveryDisplaySearchPageSize,
				Properties = SearchRecipient.DisplayProperties,
				RequestGroups = true,
				Sources = source.ToList<SearchSource>()
			};
			base.Executor.EnqueueNext(item2);
		}
	}
}
