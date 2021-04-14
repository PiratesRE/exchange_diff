using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage.Infoworker.MailboxSearch;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.EDiscovery.MailboxSearch.WebService.Infrastructure;
using Microsoft.Exchange.EDiscovery.MailboxSearch.WebService.Model;

namespace Microsoft.Exchange.EDiscovery.MailboxSearch.WebService.External
{
	internal class ArbitrationSearchConfigurationProvider : ISearchConfigurationProvider
	{
		public void ApplyConfiguration(ISearchPolicy policy, ref SearchMailboxesInputs inputs)
		{
			Recorder.Trace(5L, TraceType.InfoTrace, "ArbitrationSearchConfigurationProvider.ApplyConfiguration Inputs:", inputs);
			if (inputs.Sources != null && inputs.Sources.Count > 0 && inputs.Sources[0].SourceType == SourceType.SavedSearchId)
			{
				inputs.SearchConfigurationId = inputs.Sources[0].ReferenceId;
				inputs.Sources.RemoveAt(0);
				Recorder.Trace(5L, TraceType.InfoTrace, "ArbitrationSearchConfigurationProvider.ApplyConfiguration Found In Sources SearchId:", inputs.SearchConfigurationId);
			}
			if (!string.IsNullOrWhiteSpace(inputs.SearchConfigurationId))
			{
				Recorder.Trace(5L, TraceType.InfoTrace, "ArbitrationSearchConfigurationProvider.ApplyConfiguration Found SearchId:", inputs.SearchConfigurationId);
				IDiscoverySearchDataProvider discoverySearchDataProvider = new DiscoverySearchDataProvider(policy.RecipientSession.SessionSettings.CurrentOrganizationId);
				MailboxDiscoverySearch mailboxDiscoverySearch = discoverySearchDataProvider.Find<MailboxDiscoverySearch>(inputs.SearchConfigurationId);
				if (mailboxDiscoverySearch == null)
				{
					Recorder.Trace(5L, TraceType.ErrorTrace, "ArbitrationSearchConfigurationProvider.ApplyConfiguration Invalid SearchId:", inputs.SearchConfigurationId);
					throw new SearchException(KnownError.ErrorInvalidSearchId);
				}
				inputs.SearchQuery = mailboxDiscoverySearch.CalculatedQuery;
				inputs.Language = mailboxDiscoverySearch.Language;
				Recorder.Trace(5L, TraceType.InfoTrace, new object[]
				{
					"ArbitrationSearchConfigurationProvider.ApplyConfiguration Query:",
					inputs.SearchQuery,
					"Language:",
					inputs.Language
				});
				if (inputs.Sources == null || inputs.Sources.Count == 0)
				{
					Recorder.Trace(5L, TraceType.InfoTrace, "ArbitrationSearchConfigurationProvider.ApplyConfiguration No Sources");
					inputs.Sources = new List<SearchSource>();
					if (mailboxDiscoverySearch.Sources != null && mailboxDiscoverySearch.Sources.Count > 0)
					{
						Recorder.Trace(5L, TraceType.InfoTrace, "ArbitrationSearchConfigurationProvider.ApplyConfiguration Mailboxes:", mailboxDiscoverySearch.Sources.Count);
						using (MultiValuedProperty<string>.Enumerator enumerator = mailboxDiscoverySearch.Sources.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								string referenceId = enumerator.Current;
								inputs.Sources.Add(new SearchSource
								{
									ReferenceId = referenceId,
									SourceLocation = SourceLocation.All,
									SourceType = SourceType.AutoDetect
								});
							}
							goto IL_234;
						}
					}
					if (mailboxDiscoverySearch.Version == SearchObjectVersion.Original || mailboxDiscoverySearch.AllSourceMailboxes)
					{
						Recorder.Trace(5L, TraceType.InfoTrace, "ArbitrationSearchConfigurationProvider.ApplyConfiguration AllMailboxes");
						inputs.Sources.Add(new SearchSource
						{
							SourceLocation = SourceLocation.All,
							SourceType = SourceType.AllMailboxes
						});
					}
					IL_234:
					if (mailboxDiscoverySearch.PublicFolderSources != null && mailboxDiscoverySearch.PublicFolderSources.Count > 0)
					{
						Recorder.Trace(5L, TraceType.InfoTrace, "ArbitrationSearchConfigurationProvider.ApplyConfiguration PublicFoiders:", mailboxDiscoverySearch.PublicFolderSources.Count);
						using (MultiValuedProperty<string>.Enumerator enumerator2 = mailboxDiscoverySearch.PublicFolderSources.GetEnumerator())
						{
							while (enumerator2.MoveNext())
							{
								string referenceId2 = enumerator2.Current;
								inputs.Sources.Add(new SearchSource
								{
									ReferenceId = referenceId2,
									SourceLocation = SourceLocation.All,
									SourceType = SourceType.PublicFolder
								});
							}
							return;
						}
					}
					if (mailboxDiscoverySearch.AllPublicFolderSources)
					{
						Recorder.Trace(5L, TraceType.InfoTrace, "ArbitrationSearchConfigurationProvider.ApplyConfiguration AllPublicFoiders");
						inputs.Sources.Add(new SearchSource
						{
							SourceLocation = SourceLocation.PrimaryOnly,
							SourceType = SourceType.AllPublicFolders
						});
					}
				}
			}
		}
	}
}
