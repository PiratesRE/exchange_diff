using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.EDiscovery.MailboxSearch.WebService.Infrastructure;
using Microsoft.Exchange.EDiscovery.MailboxSearch.WebService.Model;

namespace Microsoft.Exchange.EDiscovery.MailboxSearch.WebService.Task
{
	internal class DirectoryLookup : SearchTask<DirectoryQueryParameters>
	{
		public override void Process(DirectoryQueryParameters item)
		{
			Recorder.Trace(4L, TraceType.InfoTrace, new object[]
			{
				"DirectoryLookup.Process Query:",
				item.Query,
				"ExpandPublicFolders:",
				item.ExpandPublicFolders,
				"ExpandGroups:",
				item.ExpandGroups,
				"MatchRecipientsToSources:",
				item.MatchRecipientsToSources
			});
			List<SearchSource> list = null;
			if (item.ExpandPublicFolders)
			{
				Recorder.Trace(4L, TraceType.InfoTrace, "DirectoryLookup.Process ExpandPublicFolders");
				list = new List<SearchSource>();
			}
			Dictionary<SearchSource, bool> matchMap = null;
			if (item.MatchRecipientsToSources)
			{
				Recorder.Trace(4L, TraceType.InfoTrace, "DirectoryLookup.Process MatchRecipients");
				matchMap = new Dictionary<SearchSource, bool>();
				item.Sources.ForEach(delegate(SearchSource t)
				{
					matchMap[t] = false;
				});
			}
			IDirectoryProvider directoryProvider = SearchFactory.Current.GetDirectoryProvider(base.Executor.Policy);
			foreach (SearchRecipient recipient in directoryProvider.Query(base.Executor.Policy, item))
			{
				if (item.MatchRecipientsToSources)
				{
					this.EnqueueMatches(recipient, matchMap, list);
				}
				else
				{
					SearchSource source = this.CreateDefaultSource(null, recipient);
					this.EnqueueSource(source, list);
				}
			}
			if (item.MatchRecipientsToSources)
			{
				foreach (SearchSource searchSource in from t in matchMap
				where !t.Value
				select t.Key)
				{
					Recorder.Trace(4L, TraceType.WarningTrace, new object[]
					{
						"DirectoryLookup.Process FailedSource:",
						searchSource.ReferenceId,
						"FailedSourceType:",
						searchSource.SourceType
					});
					base.Executor.Fail(new SearchException(KnownError.ErrorSearchableObjectNotFound)
					{
						ErrorSource = searchSource
					});
				}
			}
			if (item.ExpandPublicFolders && list.Count > 0)
			{
				Recorder.Trace(4L, TraceType.WarningTrace, "DirectoryLookup.Process ExapndPublicFolders Count:", list.Count);
				ISourceConverter sourceConverter = SearchFactory.Current.GetSourceConverter(base.Policy, SourceType.PublicFolder);
				foreach (SearchSource searchSource2 in sourceConverter.Convert(base.Policy, this.GetPublicFolderSources(list)))
				{
					QueryFilter sourceFilter = SearchRecipient.GetSourceFilter(searchSource2);
					item.Query = sourceFilter;
					item.PageSize = 1;
					using (IEnumerator<SearchRecipient> enumerator4 = directoryProvider.Query(base.Policy, item).GetEnumerator())
					{
						if (enumerator4.MoveNext())
						{
							SearchRecipient searchRecipient = enumerator4.Current;
							searchSource2.Recipient.ADEntry = searchRecipient.ADEntry;
							this.EnqueueSource(searchSource2, null);
						}
					}
				}
			}
		}

		private void EnqueueMatches(SearchRecipient recipient, Dictionary<SearchSource, bool> matchMap, List<SearchSource> publicFolders)
		{
			bool flag = false;
			Recorder.Trace(4L, TraceType.InfoTrace, "DirectoryLookup.EnqueueMatches Recipient:", recipient);
			if (recipient.Parent == null)
			{
				foreach (SearchSource searchSource in this.Match(recipient.ADEntry, matchMap, false))
				{
					flag = true;
					searchSource.Recipient = recipient;
					this.EnqueueSource(searchSource, publicFolders);
				}
			}
			if (!flag)
			{
				Recorder.Trace(4L, TraceType.InfoTrace, "DirectoryLookup.EnqueueMatches Match to Parent Recipient:", recipient);
				foreach (SearchSource originalSource in this.Match(recipient.Parent, matchMap, true))
				{
					flag = true;
					SearchSource source = this.CreateDefaultSource(originalSource, recipient);
					this.EnqueueSource(source, publicFolders);
					if (flag)
					{
						break;
					}
				}
			}
			if (!flag)
			{
				Recorder.Trace(4L, TraceType.WarningTrace, "DirectoryLookup.EnqueueMatches Orphaned AD Entry Entry:", recipient);
			}
		}

		private void EnqueueSource(SearchSource source, List<SearchSource> publicFolders)
		{
			if (source != null)
			{
				Recorder.Trace(4L, TraceType.InfoTrace, "DirectoryLookup.EnqueueSource Source:", source);
				if (SearchRecipient.IsPublicFolder(source.Recipient.ADEntry))
				{
					Recorder.Trace(4L, TraceType.InfoTrace, "DirectoryLookup.EnqueueSource PublicFolder:", source);
					if (publicFolders != null)
					{
						publicFolders.Add(source);
					}
					return;
				}
				base.Executor.EnqueueNext(source);
			}
		}

		private IEnumerable<SearchSource> Match(ADRawEntry entry, Dictionary<SearchSource, bool> matchMap, bool matchParents = false)
		{
			Recorder.Trace(4L, TraceType.InfoTrace, new object[]
			{
				"DirectoryLookup.Match Entry:",
				entry,
				"MatchParents:",
				matchParents
			});
			string legacyExchangeDn = null;
			string mailboxGuid = null;
			string recipientId = null;
			ProxyAddressCollection proxyAddresses = null;
			foreach (SearchSource source in matchMap.Keys.ToList<SearchSource>())
			{
				switch (source.SourceType)
				{
				case SourceType.LegacyExchangeDN:
					if (string.IsNullOrEmpty(legacyExchangeDn))
					{
						legacyExchangeDn = (string)entry[ADRecipientSchema.LegacyExchangeDN];
					}
					if (source.ReferenceId.Equals(legacyExchangeDn, StringComparison.InvariantCultureIgnoreCase))
					{
						Recorder.Trace(4L, TraceType.InfoTrace, new object[]
						{
							"DirectoryLookup.Match Entry:",
							entry,
							"Matching Source:",
							source
						});
						matchMap[source] = true;
						yield return source;
					}
					else if (source.CanBeCrossPremise)
					{
						if (proxyAddresses == null)
						{
							proxyAddresses = (entry[ADRecipientSchema.EmailAddresses] as ProxyAddressCollection);
						}
						if (proxyAddresses != null)
						{
							foreach (ProxyAddress proxy in proxyAddresses)
							{
								if (proxy.Prefix == ProxyAddressPrefix.X500 && source.ReferenceId.Equals(proxy.AddressString, StringComparison.OrdinalIgnoreCase))
								{
									matchMap[source] = true;
									yield return source;
								}
							}
						}
					}
					break;
				case SourceType.PublicFolder:
				case SourceType.MailboxGuid:
					if (string.IsNullOrEmpty(mailboxGuid))
					{
						mailboxGuid = ((Guid)entry[ADMailboxRecipientSchema.ExchangeGuid]).ToString();
					}
					if (source.ReferenceId.Equals(mailboxGuid, StringComparison.InvariantCultureIgnoreCase))
					{
						Recorder.Trace(4L, TraceType.InfoTrace, new object[]
						{
							"DirectoryLookup.Match Entry:",
							entry,
							"Matching Source:",
							source
						});
						matchMap[source] = true;
						yield return source;
					}
					break;
				case SourceType.Recipient:
					if (string.IsNullOrEmpty(recipientId))
					{
						recipientId = ((ADObjectId)entry[ADObjectSchema.Id]).Name;
					}
					if (source.ReferenceId.Equals(recipientId, StringComparison.InvariantCultureIgnoreCase))
					{
						Recorder.Trace(4L, TraceType.InfoTrace, new object[]
						{
							"DirectoryLookup.Match Entry:",
							entry,
							"Matching Source:",
							source
						});
						matchMap[source] = true;
						yield return source;
					}
					break;
				case SourceType.AllPublicFolders:
				case SourceType.AllMailboxes:
					matchMap[source] = true;
					if (matchParents)
					{
						Recorder.Trace(4L, TraceType.InfoTrace, new object[]
						{
							"DirectoryLookup.Match Entry:",
							entry,
							"Matching Source:",
							source
						});
						yield return source;
					}
					break;
				}
			}
			yield break;
		}

		private SearchSource CreateDefaultSource(SearchSource originalSource, SearchRecipient recipient)
		{
			SearchSource searchSource = (originalSource != null) ? originalSource.Clone() : new SearchSource();
			searchSource.ReferenceId = (string)recipient.ADEntry[ADRecipientSchema.LegacyExchangeDN];
			searchSource.SourceType = SourceType.LegacyExchangeDN;
			searchSource.Recipient = recipient;
			return searchSource;
		}

		private IEnumerable<SearchSource> GetPublicFolderSources(List<SearchSource> publicFolders)
		{
			Recorder.Trace(4L, TraceType.InfoTrace, "DirectoryLookup.GetPublicFolderSources PublicFolders:", publicFolders);
			foreach (SearchSource source in publicFolders)
			{
				if (source.Recipient.ADEntry.propertyBag.Contains(ADPublicFolderSchema.EntryId))
				{
					string publicFolderEntry = (string)source.Recipient.ADEntry[ADPublicFolderSchema.EntryId];
					if (!string.IsNullOrEmpty(publicFolderEntry))
					{
						source.SourceType = SourceType.PublicFolder;
						source.OriginalReferenceId = source.ReferenceId;
						source.ReferenceId = publicFolderEntry;
						yield return source;
					}
				}
			}
			yield break;
		}
	}
}
