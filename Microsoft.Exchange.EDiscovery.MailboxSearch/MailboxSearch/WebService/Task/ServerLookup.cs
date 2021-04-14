using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.EDiscovery.MailboxSearch.WebService.Infrastructure;
using Microsoft.Exchange.EDiscovery.MailboxSearch.WebService.Model;
using Microsoft.Exchange.InfoWorker.Common.MultiMailboxSearch;

namespace Microsoft.Exchange.EDiscovery.MailboxSearch.WebService.Task
{
	internal class ServerLookup : SearchTask<SearchSource>
	{
		public ServerLookup.ServerLookupContext TaskContext
		{
			get
			{
				return (ServerLookup.ServerLookupContext)base.Context.TaskContext;
			}
		}

		public override void Process(IList<SearchSource> item)
		{
			Recorder.Trace(4L, TraceType.InfoTrace, "ServerLookup.Process Item:", item);
			item.FirstOrDefault<SearchSource>();
			IList<SearchSource> list = new List<SearchSource>();
			Func<SearchSource, string> func = delegate(SearchSource source)
			{
				if (source.MailboxInfo.IsRemoteMailbox)
				{
					return null;
				}
				if (source.MailboxInfo.IsArchive)
				{
					return source.MailboxInfo.ArchiveDatabase.ToString();
				}
				return source.MailboxInfo.MdbGuid.ToString();
			};
			foreach (SearchSource searchSource in item)
			{
				string text = func(searchSource);
				GroupId groupId;
				if (!string.IsNullOrEmpty(text) && this.TaskContext.LookupCache.TryGetValue(text, out groupId))
				{
					Recorder.Trace(4L, TraceType.InfoTrace, "ServerLookup.Process Cache Hit GroupId:", groupId);
					base.Executor.EnqueueNext(new FanoutParameters
					{
						GroupId = groupId,
						Source = searchSource
					});
				}
				else
				{
					list.Add(searchSource);
				}
			}
			if (list.Count > 0)
			{
				Recorder.Trace(4L, TraceType.InfoTrace, "ServerLookup.Process Cache Misses Count:", list.Count);
				IServerProvider serverProvider = SearchFactory.Current.GetServerProvider(base.Policy);
				foreach (FanoutParameters fanoutParameters in serverProvider.GetServer(base.Policy, list))
				{
					if (fanoutParameters.GroupId != null && fanoutParameters.GroupId.Uri != null && fanoutParameters.GroupId.GroupType != GroupType.SkippedError)
					{
						string text2 = func(fanoutParameters.Source);
						if (!string.IsNullOrEmpty(text2))
						{
							this.TaskContext.LookupCache.TryAdd(text2, fanoutParameters.GroupId);
						}
						base.Executor.EnqueueNext(fanoutParameters);
					}
					else
					{
						Recorder.Trace(4L, TraceType.InfoTrace, "ServerLookup.Process Ignoring an recipient group:", fanoutParameters.GroupId);
					}
				}
			}
		}

		public class ServerLookupContext
		{
			public ServerLookupContext()
			{
				this.LookupCache = new ConcurrentDictionary<string, GroupId>();
			}

			public ConcurrentDictionary<string, GroupId> LookupCache { get; private set; }
		}
	}
}
