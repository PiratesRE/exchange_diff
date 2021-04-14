using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Search.AqsParser;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.EDiscovery.MailboxSearch.WebService.Infrastructure;
using Microsoft.Exchange.EDiscovery.MailboxSearch.WebService.Model;
using Microsoft.Exchange.InfoWorker.Common.MultiMailboxSearch;

namespace Microsoft.Exchange.EDiscovery.MailboxSearch.WebService.Task
{
	internal class InitializeSearchMailbox : SearchTask<SearchMailboxesInputs>
	{
		public override void Process(SearchMailboxesInputs item)
		{
			Recorder.Trace(4L, TraceType.InfoTrace, "InitializeSearchMailbox.Process Item:", item);
			if (!item.IsLocalCall)
			{
				Recorder.Trace(4L, TraceType.InfoTrace, "InitializeSearchMailbox.Process ApplyConfiguration");
				ISearchConfigurationProvider searchConfigurationProvider = SearchFactory.Current.GetSearchConfigurationProvider(base.Policy);
				searchConfigurationProvider.ApplyConfiguration(base.Policy, ref item);
			}
			CultureInfo culture = CultureInfo.InvariantCulture;
			if (!string.IsNullOrEmpty(item.Language))
			{
				Recorder.Trace(4L, TraceType.InfoTrace, "InitializeSearchMailbox.Process Set Language:", item.Language);
				try
				{
					culture = new CultureInfo(item.Language);
				}
				catch (CultureNotFoundException)
				{
					Recorder.Trace(4L, TraceType.ErrorTrace, "InitializeSearchMailbox.Process Failed Language:", item.Language);
					throw new SearchException(KnownError.ErrorQueryLanguageNotValid);
				}
			}
			if (!string.IsNullOrWhiteSpace(item.SearchQuery))
			{
				Recorder.Trace(4L, TraceType.InfoTrace, "InitializeSearchMailbox.Process Set Query:", item.SearchQuery);
				try
				{
					IConfigurationSession configurationSession = SearchFactory.Current.GetConfigurationSession(base.Policy);
					item.Criteria = new SearchCriteria(item.SearchQuery, null, culture, item.SearchType, base.Policy.RecipientSession, configurationSession, item.RequestId, base.Policy.ExecutionSettings.ExcludedFolders);
					goto IL_195;
				}
				catch (ParserException ex)
				{
					Recorder.Trace(4L, TraceType.ErrorTrace, new object[]
					{
						"InitializeSearchMailbox.Process Failed Query:",
						item.SearchQuery,
						"Error:",
						ex
					});
					throw new SearchException(KnownError.ErrorInvalidSearchQuerySyntax, ex);
				}
				catch (TooManyKeywordsException ex2)
				{
					Recorder.Trace(4L, TraceType.ErrorTrace, new object[]
					{
						"InitializeSearchMailbox.Process Failed Query:",
						item.SearchQuery,
						"Error:",
						ex2
					});
					throw new SearchException(KnownError.TooManyKeywordsException, ex2);
				}
				goto IL_181;
				IL_195:
				if (item.Sources != null && item.Sources.Count > 0)
				{
					Recorder.Trace(4L, TraceType.ErrorTrace, "InitializeSearchMailbox.Process Source Count:", item.Sources.Count);
					if (item.Sources.Count > base.Policy.ExecutionSettings.DiscoveryMaxMailboxes && item.SearchType == SearchType.Preview)
					{
						Recorder.Trace(4L, TraceType.ErrorTrace, new object[]
						{
							"InitializeSearchMailbox.Process Source Failed Count:",
							item.Sources.Count,
							"Limit:",
							base.Policy.ThrottlingSettings.DiscoveryMaxMailboxes,
							"SearchType:",
							item.SearchType
						});
						throw new SearchException(KnownError.TooManyMailboxesException, new object[]
						{
							item.Sources.Count,
							base.Policy.ExecutionSettings.DiscoveryMaxMailboxes
						});
					}
					if ((long)item.Sources.Count > (long)((ulong)base.Policy.ThrottlingSettings.DiscoveryMaxStatsSearchMailboxes) && item.SearchType == SearchType.Statistics)
					{
						Recorder.Trace(4L, TraceType.ErrorTrace, new object[]
						{
							"InitializeSearchMailbox.Process Source Failed Count:",
							item.Sources.Count,
							"Limit:",
							base.Policy.ThrottlingSettings.DiscoveryMaxStatsSearchMailboxes,
							"SearchType:",
							item.SearchType
						});
						throw new SearchException(KnownError.TooManyMailboxesException, new object[]
						{
							item.Sources.Count,
							(int)base.Policy.ThrottlingSettings.DiscoveryMaxStatsSearchMailboxes
						});
					}
					if (!item.IsLocalCall)
					{
						Recorder.Trace(4L, TraceType.InfoTrace, "InitializeSearchMailbox.Process Format Query");
						DirectoryQueryParameters item2 = new DirectoryQueryParameters
						{
							ExpandPublicFolders = true,
							ExpandGroups = true,
							MatchRecipientsToSources = true,
							Properties = SearchRecipient.SearchProperties,
							Sources = item.Sources,
							PageSize = (int)base.Policy.ExecutionSettings.DiscoveryADPageSize
						};
						base.Executor.EnqueueNext(item2);
						return;
					}
					Recorder.Trace(4L, TraceType.InfoTrace, "InitializeSearchMailbox.Process Search Database");
					using (List<SearchSource>.Enumerator enumerator = item.Sources.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							SearchSource item3 = enumerator.Current;
							base.Executor.EnqueueNext(item3);
						}
						return;
					}
				}
				Recorder.Trace(4L, TraceType.ErrorTrace, "InitializeSearchMailbox.Process No Sources");
				throw new SearchException(KnownError.ErrorNoMailboxSpecifiedForSearchOperation);
			}
			IL_181:
			Recorder.Trace(4L, TraceType.ErrorTrace, "InitializeSearchMailbox.Process Failed Query Empty");
			throw new SearchException(KnownError.ErrorSearchQueryCannotBeEmpty);
		}
	}
}
