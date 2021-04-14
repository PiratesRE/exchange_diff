using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic.Cafe;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Infoworker.MailboxSearch;
using Microsoft.Exchange.InfoWorker.EventLog;
using Microsoft.Exchange.Security.OAuth;
using Microsoft.Exchange.SoapWebClient;

namespace Microsoft.Exchange.InfoWorker.Common.MultiMailboxSearch
{
	internal class EwsEndpointDiscovery : IEwsEndpointDiscovery
	{
		public EwsEndpointDiscovery(List<MailboxInfo> mailboxes, OrganizationId orgId, CallerInfo callerInfo)
		{
			this.mailboxes = mailboxes;
			this.orgId = orgId;
			this.mailboxGroups = new Dictionary<GroupId, List<MailboxInfo>>(5);
			this.callerInfo = callerInfo;
		}

		protected List<MailboxInfo> Mailboxes
		{
			get
			{
				return this.mailboxes;
			}
		}

		protected Dictionary<GroupId, List<MailboxInfo>> MailboxGroups
		{
			get
			{
				return this.mailboxGroups;
			}
		}

		protected Func<OrganizationId, OrganizationIdCacheValue> GetOrgIdCacheValue { get; set; }

		protected Func<OrganizationIdCacheValue, string, IntraOrganizationConnector> GetIntraOrganizationConnector { get; set; }

		protected Func<OrganizationIdCacheValue, string, OrganizationRelationship> GetOrganizationRelationShip { get; set; }

		public Dictionary<GroupId, List<MailboxInfo>> FindEwsEndpoints(out long localDiscoverTime, out long autoDiscoverTime)
		{
			Stopwatch stopwatch = new Stopwatch();
			localDiscoverTime = 0L;
			autoDiscoverTime = 0L;
			stopwatch.Start();
			List<MailboxInfo> list;
			string text;
			this.FilterLocalForestMailboxes(out list, out text);
			stopwatch.Stop();
			localDiscoverTime = stopwatch.ElapsedMilliseconds;
			Factory.Current.MailboxGroupGeneratorTracer.TracePerformance<Guid, long>((long)this.GetHashCode(), "Correlation Id:{0}. Mapping local mailboxes to servers took {1}ms", this.callerInfo.QueryCorrelationId, localDiscoverTime);
			if (list == null || list.Count == 0)
			{
				return this.mailboxGroups;
			}
			string searchId = string.Empty;
			Match match = Regex.Match(this.callerInfo.UserAgent, "SID=([a-fA-F0-9\\-]*)");
			if (match.Success && match.Groups != null && match.Groups.Count > 1)
			{
				searchId = match.Groups[1].Value;
			}
			Uri url = null;
			EndPointDiscoveryInfo endPointDiscoveryInfo;
			bool flag = RemoteDiscoveryEndPoint.TryGetDiscoveryEndPoint(this.orgId, text, this.GetOrgIdCacheValue, this.GetIntraOrganizationConnector, this.GetOrganizationRelationShip, out url, out endPointDiscoveryInfo);
			if (endPointDiscoveryInfo != null && endPointDiscoveryInfo.Status != EndPointDiscoveryInfo.DiscoveryStatus.Success)
			{
				SearchEventLogger.Instance.LogSearchErrorEvent(searchId, endPointDiscoveryInfo.Message);
			}
			if (!flag)
			{
				Factory.Current.MailboxGroupGeneratorTracer.TraceDebug<Guid, string>((long)this.GetHashCode(), "Correlation Id:{0}. Unable to find the discovery end point for domain {1}", this.callerInfo.QueryCorrelationId, text);
				GroupId key = new GroupId(new MultiMailboxSearchException(Strings.CouldNotFindOrgRelationship(text)));
				this.mailboxGroups.Add(key, list);
				return this.mailboxGroups;
			}
			Factory.Current.MailboxGroupGeneratorTracer.TraceDebug<Guid, string, Uri>((long)this.GetHashCode(), "Correlation Id:{0}. EWS endpoint for domain {1} is {2}", this.callerInfo.QueryCorrelationId, text, EwsWsSecurityUrl.FixForAnonymous(url));
			OAuthCredentials oauthCredentialsForAppToken = OAuthCredentials.GetOAuthCredentialsForAppToken(this.orgId, text);
			stopwatch.Restart();
			List<MailboxInfo> list2 = (from mailboxInfo in list
			where mailboxInfo.IsArchive
			select mailboxInfo).ToList<MailboxInfo>();
			List<MailboxInfo> list3 = (from mailboxInfo in list
			where !mailboxInfo.IsArchive
			select mailboxInfo).ToList<MailboxInfo>();
			if (list2.Count > 0)
			{
				Factory.Current.MailboxGroupGeneratorTracer.TracePerformance<Guid, int>((long)this.GetHashCode(), "Correlation Id:{0}. Mbx Count:{1}. Autodiscover started for cross premise archive mailboxes.", this.callerInfo.QueryCorrelationId, list2.Count);
				this.DoAutodiscover(list2, EwsWsSecurityUrl.FixForAnonymous(url), oauthCredentialsForAppToken);
				Factory.Current.MailboxGroupGeneratorTracer.TracePerformance<Guid, int>((long)this.GetHashCode(), "Correlation Id:{0}. Mbx Count:{1}. Autodiscover completed for cross premise archive mailboxes.", this.callerInfo.QueryCorrelationId, list2.Count);
			}
			if (list3.Count > 0)
			{
				Factory.Current.MailboxGroupGeneratorTracer.TracePerformance<Guid, int>((long)this.GetHashCode(), "Correlation Id:{0}. Mbx Count:{1}. Autodiscover started for cross premise primary mailboxes.", this.callerInfo.QueryCorrelationId, list3.Count);
				this.DoAutodiscover(list3, EwsWsSecurityUrl.FixForAnonymous(url), oauthCredentialsForAppToken);
				Factory.Current.MailboxGroupGeneratorTracer.TracePerformance<Guid, int>((long)this.GetHashCode(), "Correlation Id:{0}. Mbx Count:{1}. Autodiscover completed for cross premise primary mailboxes.", this.callerInfo.QueryCorrelationId, list3.Count);
			}
			stopwatch.Stop();
			autoDiscoverTime = stopwatch.ElapsedMilliseconds;
			Factory.Current.MailboxGroupGeneratorTracer.TracePerformance<Guid, long>((long)this.GetHashCode(), "Correlation Id:{0}. Autodiscover call took {1}ms", this.callerInfo.QueryCorrelationId, autoDiscoverTime);
			return this.mailboxGroups;
		}

		private static bool ValidateCrossPremiseDomain(MailboxInfo mailbox, string domain)
		{
			return string.Equals(domain, mailbox.GetDomain(), StringComparison.OrdinalIgnoreCase);
		}

		private void FilterLocalForestMailboxes(out List<MailboxInfo> crossPremiseMailboxes, out string crossPremiseDomain)
		{
			crossPremiseDomain = null;
			crossPremiseMailboxes = new List<MailboxInfo>();
			List<MailboxInfo> list = new List<MailboxInfo>(this.mailboxes.Count);
			for (int i = 0; i < this.mailboxes.Count; i++)
			{
				if (!this.mailboxes[i].IsRemoteMailbox)
				{
					list.Add(this.mailboxes[i]);
				}
				if (this.mailboxes[i].IsCrossPremiseMailbox)
				{
					if (crossPremiseDomain == null)
					{
						crossPremiseDomain = this.mailboxes[i].GetDomain();
					}
					if (!EwsEndpointDiscovery.ValidateCrossPremiseDomain(this.mailboxes[i], crossPremiseDomain))
					{
						Factory.Current.MailboxGroupGeneratorTracer.TraceDebug<Guid, string, string>((long)this.GetHashCode(), "Correlation Id:{0}. Domain for mailbox {1} does not match {2}", this.callerInfo.QueryCorrelationId, this.mailboxes[i].DistinguishedName, crossPremiseDomain);
						this.AddMailboxToGroup(this.mailboxes[i], new GroupId(new Exception("The domain was invalid for this cross-premise mailbox")));
					}
					crossPremiseMailboxes.Add(this.mailboxes[i]);
				}
				if (this.mailboxes[i].IsCrossForestMailbox)
				{
					this.AddMailboxToGroup(this.mailboxes[i], new GroupId(new NotSupportedException(Strings.CrossForestNotSupported)));
				}
			}
			this.CreateGroupsForLocalMailboxes(list);
		}

		protected virtual void CreateGroupsForLocalMailboxes(List<MailboxInfo> localMailboxes)
		{
			Dictionary<Guid, BackEndServer> dictionary = new Dictionary<Guid, BackEndServer>(localMailboxes.Count);
			for (int i = 0; i < localMailboxes.Count; i++)
			{
				Guid key = (localMailboxes[i].Type == MailboxType.Primary) ? localMailboxes[i].MdbGuid : localMailboxes[i].ArchiveDatabase;
				if (!dictionary.ContainsKey(key))
				{
					dictionary.Add(key, null);
				}
			}
			for (int j = 0; j < localMailboxes.Count; j++)
			{
				Guid guid = (localMailboxes[j].Type == MailboxType.Primary) ? localMailboxes[j].MdbGuid : localMailboxes[j].ArchiveDatabase;
				try
				{
					BackEndServer backEndServer = null;
					dictionary.TryGetValue(guid, out backEndServer);
					if (backEndServer == null)
					{
						Factory.Current.MailboxGroupGeneratorTracer.TraceDebug<Guid, Guid>((long)this.GetHashCode(), "Correlation Id:{0}. Retrieving backend servers for database {1} and all the databases in the DAG", this.callerInfo.QueryCorrelationId, guid);
						int k = EwsEndpointDiscovery.MailboxServerLocatorRetryCount;
						while (k > 0)
						{
							using (MailboxServerLocator mailboxServerLocator = MailboxServerLocator.CreateWithResourceForestFqdn(guid, null))
							{
								Stopwatch stopwatch = new Stopwatch();
								stopwatch.Start();
								IAsyncResult asyncResult = mailboxServerLocator.BeginGetServer(null, null);
								bool flag = asyncResult.AsyncWaitHandle.WaitOne(EwsEndpointDiscovery.MailboxServerLocatorTimeout);
								if (flag)
								{
									BackEndServer value = mailboxServerLocator.EndGetServer(asyncResult);
									stopwatch.Stop();
									Factory.Current.EventLog.LogEvent(InfoWorkerEventLogConstants.Tuple_DiscoveryMailboxServerLocatorTime, null, new object[]
									{
										this.callerInfo.QueryCorrelationId.ToString(),
										guid.ToString(),
										stopwatch.ElapsedMilliseconds
									});
									dictionary[guid] = value;
									foreach (KeyValuePair<Guid, BackEndServer> keyValuePair in mailboxServerLocator.AvailabilityGroupServers)
									{
										if (dictionary.ContainsKey(keyValuePair.Key))
										{
											Factory.Current.MailboxGroupGeneratorTracer.TraceDebug<Guid, Guid, Guid>((long)this.GetHashCode(), "Correlation Id:{0}. While queried backend for {1}, also retrieved backend for {2}", this.callerInfo.QueryCorrelationId, guid, keyValuePair.Key);
											dictionary[keyValuePair.Key] = keyValuePair.Value;
										}
									}
									break;
								}
								stopwatch.Stop();
								Factory.Current.EventLog.LogEvent(InfoWorkerEventLogConstants.Tuple_DiscoveryServerLocatorTimeout, null, new object[]
								{
									guid.ToString(),
									this.callerInfo.QueryCorrelationId.ToString(),
									EwsEndpointDiscovery.MailboxServerLocatorRetryCount - k + 1
								});
								k--;
							}
						}
					}
					if (dictionary[guid] != null)
					{
						this.AddMailboxToGroup(localMailboxes[j], dictionary[guid]);
					}
					else
					{
						Factory.Current.MailboxGroupGeneratorTracer.TraceDebug<Guid, Guid, SmtpAddress>((long)this.GetHashCode(), "Correlation Id:{0}. Couldn't find the backend for database {1}. So adding an error group for mailbox {2}", this.callerInfo.QueryCorrelationId, guid, localMailboxes[j].PrimarySmtpAddress);
						this.AddMailboxToGroup(localMailboxes[j], new GroupId(new DatabaseLocationUnavailableException(Strings.DatabaseLocationUnavailable(localMailboxes[j].PrimarySmtpAddress.ToString()))));
					}
				}
				catch (MailboxServerLocatorException error)
				{
					Factory.Current.MailboxGroupGeneratorTracer.TraceDebug<Guid, Guid>((long)this.GetHashCode(), "Correlation Id:{0}. Encountered an Exception while querying backend for database {1}", this.callerInfo.QueryCorrelationId, guid);
					this.AddMailboxToGroup(localMailboxes[j], new GroupId(error));
				}
			}
		}

		private void AddMailboxToGroup(MailboxInfo mailbox, GroupId groupId)
		{
			List<MailboxInfo> list = null;
			if (!this.mailboxGroups.TryGetValue(groupId, out list))
			{
				list = new List<MailboxInfo>(1);
				this.mailboxGroups.Add(groupId, list);
			}
			list.Add(mailbox);
		}

		private void AddMailboxToGroup(MailboxInfo mailbox, BackEndServer server)
		{
			Uri backEndWebServicesUrl = BackEndLocator.GetBackEndWebServicesUrl(server);
			if (string.Equals(LocalServerCache.LocalServerFqdn, server.Fqdn, StringComparison.OrdinalIgnoreCase))
			{
				Factory.Current.MailboxGroupGeneratorTracer.TraceDebug<Guid, string>((long)this.GetHashCode(), "Correlation Id:{0}. Mailbox {1} is a local mailbox", this.callerInfo.QueryCorrelationId, mailbox.ToString());
				this.AddMailboxToGroup(mailbox, new GroupId(GroupType.Local, backEndWebServicesUrl, LocalServerCache.LocalServer.VersionNumber, mailbox.GetDomain()));
				return;
			}
			Factory.Current.MailboxGroupGeneratorTracer.TraceDebug<Guid, string, string>((long)this.GetHashCode(), "Correlation Id:{0}. Mailbox {1} is mapped to service {2}", this.callerInfo.QueryCorrelationId, mailbox.ToString(), backEndWebServicesUrl.ToString());
			this.AddMailboxToGroup(mailbox, new GroupId(GroupType.CrossServer, backEndWebServicesUrl, server.Version, mailbox.GetDomain()));
		}

		private void DoAutodiscover(List<MailboxInfo> crossPremiseMailboxes, Uri autodiscoverUrl, ICredentials credentials)
		{
			IAutodiscoveryClient autodiscoveryClient = null;
			try
			{
				IEnumerable<IEnumerable<MailboxInfo>> source = this.BatchData<MailboxInfo>(crossPremiseMailboxes, 90);
				int num = 0;
				foreach (IEnumerable<MailboxInfo> source2 in source.ToList<IEnumerable<MailboxInfo>>())
				{
					List<MailboxInfo> list = source2.ToList<MailboxInfo>();
					num++;
					autodiscoveryClient = Factory.Current.CreateUserSettingAutoDiscoveryClient(list, autodiscoverUrl, credentials, this.callerInfo);
					IAsyncResult asyncResult = autodiscoveryClient.BeginAutodiscover(null, null);
					if (!asyncResult.AsyncWaitHandle.WaitOne(60000))
					{
						Factory.Current.MailboxGroupGeneratorTracer.TraceDebug<Guid, int, int>((long)this.GetHashCode(), "Correlation Id:{0}. Batch Number:{1}. Cross Premise Mailboxes Count:{2}. Autodiscover timed out.", this.callerInfo.QueryCorrelationId, num, list.Count<MailboxInfo>());
						autodiscoveryClient.CancelAutodiscover();
					}
					else
					{
						Factory.Current.MailboxGroupGeneratorTracer.TraceDebug<Guid, int, int>((long)this.GetHashCode(), "Correlation Id:{0}.  Batch Number:{1}. Cross Premise Mailboxes Count:{2}. Autodiscover succeeded. Merging results", this.callerInfo.QueryCorrelationId, num, list.Count<MailboxInfo>());
					}
					Dictionary<GroupId, List<MailboxInfo>> dictionary = autodiscoveryClient.EndAutodiscover(asyncResult);
					foreach (KeyValuePair<GroupId, List<MailboxInfo>> keyValuePair in dictionary)
					{
						List<MailboxInfo> list2;
						if (!this.mailboxGroups.TryGetValue(keyValuePair.Key, out list2))
						{
							this.mailboxGroups.Add(keyValuePair.Key, keyValuePair.Value);
						}
						else
						{
							list2.AddRange(keyValuePair.Value);
						}
					}
				}
			}
			finally
			{
				autodiscoveryClient.CancelAutodiscover();
				IDisposable disposable = autodiscoveryClient as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
		}

		private IEnumerable<IEnumerable<T>> BatchData<T>(IEnumerable<T> collection, int batchSize)
		{
			List<T> nextbatch = new List<T>(batchSize);
			foreach (T item in collection)
			{
				nextbatch.Add(item);
				if (nextbatch.Count == batchSize)
				{
					yield return nextbatch;
					nextbatch = new List<T>(batchSize);
				}
			}
			if (nextbatch.Count > 0)
			{
				yield return nextbatch;
			}
			yield break;
		}

		private const int DefaultNumberOfGroups = 5;

		private const int AutoDiscoverTimeout = 60000;

		private static readonly TimeSpan MailboxServerLocatorTimeout = TimeSpan.FromSeconds(30.0);

		private static int MailboxServerLocatorRetryCount = 3;

		private static readonly Predicate<WebServicesService> ServiceVersionFilter = (WebServicesService service) => service.ServerVersionNumber >= Server.E15MinVersion && !service.IsFrontEnd;

		private readonly List<MailboxInfo> mailboxes;

		private readonly OrganizationId orgId;

		private Dictionary<GroupId, List<MailboxInfo>> mailboxGroups;

		private readonly CallerInfo callerInfo;
	}
}
