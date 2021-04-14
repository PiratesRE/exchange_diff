using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.EDiscovery.MailboxSearch.WebService.Infrastructure;
using Microsoft.Exchange.EDiscovery.MailboxSearch.WebService.Model;
using Microsoft.Exchange.InfoWorker.Common.MultiMailboxSearch;

namespace Microsoft.Exchange.EDiscovery.MailboxSearch.WebService.External
{
	internal class AutoDiscoveryServerProvider : IServerProvider
	{
		public IEnumerable<FanoutParameters> GetServer(ISearchPolicy policy, IEnumerable<SearchSource> sources)
		{
			Recorder.Trace(5L, TraceType.InfoTrace, "AutoDiscoveryServerProvider.GetServer Sources:", sources);
			long discoveryTimeLocal = 0L;
			long discoveryTimeCrossPremise = 0L;
			IEwsEndpointDiscovery endpointDiscovery = Factory.Current.GetEwsEndpointDiscovery((from t in sources
			select t.MailboxInfo).ToList<MailboxInfo>(), policy.RecipientSession.SessionSettings.CurrentOrganizationId, policy.CallerInfo);
			Dictionary<GroupId, List<MailboxInfo>> mailboxServerMap = endpointDiscovery.FindEwsEndpoints(out discoveryTimeLocal, out discoveryTimeCrossPremise);
			foreach (GroupId key in mailboxServerMap.Keys)
			{
				using (List<MailboxInfo>.Enumerator enumerator2 = mailboxServerMap[key].GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						MailboxInfo mailbox = enumerator2.Current;
						FanoutParameters fanoutParameters = new FanoutParameters();
						fanoutParameters.GroupId = key;
						fanoutParameters.Source = sources.FirstOrDefault((SearchSource t) => t.MailboxInfo == mailbox);
						yield return fanoutParameters;
					}
				}
			}
			yield break;
		}
	}
}
