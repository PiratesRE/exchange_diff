using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Security;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.QueueDigest;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Net.DiagnosticsAggregation;
using Microsoft.Exchange.Transport.Categorizer;
using Microsoft.Exchange.Transport.DiagnosticsAggregationService;

namespace Microsoft.Exchange.Management.QueueDigest
{
	internal class GetQueueDigestWebServiceImpl : GetQueueDigestImpl
	{
		public GetQueueDigestWebServiceImpl(GetQueueDigestAdapter cmdlet, ITopologyConfigurationSession session, ADObjectId localSiteId, int webServicePortNumber)
		{
			this.cmdlet = cmdlet;
			this.session = session;
			this.localSiteId = localSiteId;
			this.webServicePortNumber = webServicePortNumber;
		}

		public override void ResolveForForest()
		{
			foreach (Server server in this.GetAllServersInForest())
			{
				this.ResolveServer(server);
			}
		}

		public override void ResolveDag(DatabaseAvailabilityGroup dag)
		{
			foreach (Server server in this.GetAllServersInForest())
			{
				if (dag.Id.Equals(server.DatabaseAvailabilityGroup))
				{
					this.ResolveServer(server);
				}
			}
		}

		public override void ResolveAdSite(ADSite adSite)
		{
			foreach (Server server in this.GetAllServersInForest())
			{
				if (adSite.Id.Equals(server.ServerSite))
				{
					this.ResolveServer(server);
				}
			}
		}

		public override void ResolveServer(Server server)
		{
			bool flag = ServerComponentStates.IsServerOnline(server.ComponentStates);
			if (!flag || !server.IsHubTransportServer || (!server.IsE15OrLater && !this.cmdlet.IncludeE14Servers))
			{
				return;
			}
			if (server.ServerSite != null)
			{
				this.serverToSiteMap[server.Id] = server.ServerSite;
			}
			if (server.DatabaseAvailabilityGroup != null || server.ServerSite != null)
			{
				GroupOfServersKey key = (server.DatabaseAvailabilityGroup != null) ? GroupOfServersKey.CreateFromDag(server.DatabaseAvailabilityGroup) : GroupOfServersKey.CreateFromSite(server.ServerSite, server.MajorVersion);
				if (!this.dagToServersMap.ContainsKey(key))
				{
					this.dagToServersMap[key] = new HashSet<ADObjectId>();
				}
				this.dagToServersMap[key].Add(server.Id);
				return;
			}
			this.serversNotBelongingToAnyDag.Add(server.Id);
		}

		public override void ProcessRecord()
		{
			this.aggregator = new QueueAggregator(this.cmdlet.GroupBy, this.cmdlet.DetailsLevel);
			this.webServiceRequestsPending = this.serversNotBelongingToAnyDag.Count + this.dagToServersMap.Count;
			this.webServiceRequestsDone = new AutoResetEvent(this.webServiceRequestsPending == 0);
			this.pendingRequestsToDag = new int[this.dagToServersMap.Count];
			List<ADObjectId>[] array = new List<ADObjectId>[this.dagToServersMap.Count];
			int num = 0;
			foreach (HashSet<ADObjectId> serversInDag in this.dagToServersMap.Values)
			{
				List<ADObjectId> serversToConnectPreferringLocalToSite = this.GetServersToConnectPreferringLocalToSite(serversInDag, 3);
				array[num] = serversToConnectPreferringLocalToSite;
				this.pendingRequestsToDag[num] = serversToConnectPreferringLocalToSite.Count;
				num++;
			}
			foreach (ADObjectId adobjectId in this.serversNotBelongingToAnyDag)
			{
				this.cmdlet.WriteDebug(string.Format("connecting to server {0}", adobjectId.ToString()));
				this.InvokeWebService(adobjectId, new HashSet<ADObjectId>
				{
					adobjectId
				}, false, -1, this.webServicePortNumber);
			}
			for (int i = 0; i < 3; i++)
			{
				num = 0;
				foreach (KeyValuePair<GroupOfServersKey, HashSet<ADObjectId>> keyValuePair in this.dagToServersMap)
				{
					List<ADObjectId> list = array[num];
					HashSet<ADObjectId> value = keyValuePair.Value;
					if (this.pendingRequestsToDag[num] > 0 && i < list.Count)
					{
						this.cmdlet.WriteDebug(string.Format("connecting to server {0} for group {1} attempt # {2}", list[i].Name, keyValuePair.Key.ToString(), i + 1));
						this.InvokeWebService(list[i], value, true, num, this.webServicePortNumber);
					}
					num++;
				}
			}
			this.webServiceRequestsDone.WaitOne();
			uint maxRecords = this.cmdlet.ResultSize.IsUnlimited ? uint.MaxValue : this.cmdlet.ResultSize.Value;
			this.Display(this.aggregator.GetResultSortedByMessageCount(maxRecords));
			foreach (LocalizedString text in this.verboseMessages)
			{
				this.cmdlet.WriteVerbose(text);
			}
			foreach (LocalizedString text2 in this.debugMessages)
			{
				this.cmdlet.WriteDebug(text2);
			}
			foreach (LocalizedString text3 in this.warningMessages)
			{
				this.cmdlet.WriteWarning(text3);
			}
			if (this.failedToConnectServers.Count != 0)
			{
				this.WriteWarningForConnectionFailure(this.failedToConnectServers);
			}
			if (this.failedToConnectDags.Count != 0)
			{
				this.WriteWarningForConnectionFailure(this.failedToConnectDags);
			}
		}

		public List<AggregatedQueueInfo> GetAggregationResult()
		{
			return this.aggregator.GetResultSortedByMessageCount(uint.MaxValue);
		}

		private void InvokeWebService(ADObjectId serverToConnectTo, HashSet<ADObjectId> serversToInclude, bool isConnectingToDag, int dagIndex, int portNumber)
		{
			string uri = string.Format(CultureInfo.InvariantCulture, DiagnosticsAggregationHelper.DiagnosticsAggregationEndpointFormat, new object[]
			{
				serverToConnectTo.Name,
				portNumber
			});
			Exception ex = null;
			IDiagnosticsAggregationService diagnosticsAggregationService = null;
			try
			{
				diagnosticsAggregationService = this.cmdlet.CreateWebServiceClient(GetQueueDigestWebServiceImpl.GetWebServiceBinding(this.cmdlet.Timeout), new EndpointAddress(uri));
			}
			catch (UriFormatException ex2)
			{
				ex = ex2;
			}
			List<string> list = new List<string>();
			if (serversToInclude != null && serversToInclude.Count != 0)
			{
				foreach (ADObjectId adobjectId in serversToInclude)
				{
					list.Add(adobjectId.Name);
				}
			}
			AggregatedViewRequest aggregatedViewRequest = new AggregatedViewRequest(RequestType.Queues, list, uint.MaxValue);
			aggregatedViewRequest.QueueAggregatedViewRequest = new QueueAggregatedViewRequest(this.cmdlet.GroupBy, this.cmdlet.DetailsLevel, this.cmdlet.Filter);
			GetQueueDigestWebServiceImpl.WebServiceRequestAsyncState webServiceRequestAsyncState = new GetQueueDigestWebServiceImpl.WebServiceRequestAsyncState
			{
				Client = diagnosticsAggregationService,
				ServerToConnectTo = serverToConnectTo,
				ServersToInclude = serversToInclude,
				IsConnectingToDag = isConnectingToDag,
				DagIndex = dagIndex,
				WebServicePortNumber = portNumber
			};
			try
			{
				if (diagnosticsAggregationService != null)
				{
					diagnosticsAggregationService.BeginGetAggregatedView(aggregatedViewRequest, new AsyncCallback(this.OnInvokeWebServiceCompleted), webServiceRequestAsyncState);
				}
			}
			catch (EndpointNotFoundException ex3)
			{
				ex = ex3;
			}
			catch (InsufficientMemoryException ex4)
			{
				ex = ex4;
			}
			catch (CommunicationException ex5)
			{
				ex = ex5;
			}
			catch (TimeoutException ex6)
			{
				ex = ex6;
			}
			if (ex != null)
			{
				webServiceRequestAsyncState.FailedOnBegin = true;
				webServiceRequestAsyncState.FailedOnBeginException = ex;
				ThreadPool.QueueUserWorkItem(new WaitCallback(this.ThreadProcForBeginGetAggregatedViewFailed), new AsyncResult(null, webServiceRequestAsyncState));
			}
		}

		private void OnInvokeWebServiceCompleted(IAsyncResult ar)
		{
			GetQueueDigestWebServiceImpl.WebServiceRequestAsyncState webServiceRequestAsyncState = (GetQueueDigestWebServiceImpl.WebServiceRequestAsyncState)ar.AsyncState;
			IDiagnosticsAggregationService client = webServiceRequestAsyncState.Client;
			ADObjectId serverToConnectTo = webServiceRequestAsyncState.ServerToConnectTo;
			int dagIndex = webServiceRequestAsyncState.DagIndex;
			string text = null;
			LocalizedString? localizedString = null;
			AggregatedViewResponse aggregatedViewResponse = null;
			try
			{
				if (webServiceRequestAsyncState.FailedOnBegin)
				{
					text = webServiceRequestAsyncState.FailedOnBeginException.ToString();
				}
				else
				{
					aggregatedViewResponse = client.EndGetAggregatedView(ar);
				}
			}
			catch (FaultException<DiagnosticsAggregationFault> faultException)
			{
				text = string.Format("{0}: {1}", faultException.Detail.ErrorCode, faultException.Detail.Message);
			}
			catch (CommunicationException ex)
			{
				if (GetQueueDigestWebServiceImpl.IsQuotaExceeded(ex))
				{
					localizedString = new LocalizedString?(Strings.GetQueueDigestQuotaExceeded(serverToConnectTo.Name));
				}
				text = ex.ToString();
			}
			catch (TimeoutException ex2)
			{
				text = ex2.ToString();
			}
			finally
			{
				this.cmdlet.DisposeWebServiceClient(client);
			}
			bool flag = false;
			lock (this)
			{
				if (!webServiceRequestAsyncState.IsConnectingToDag || this.pendingRequestsToDag[dagIndex] != 0)
				{
					if (aggregatedViewResponse != null)
					{
						this.aggregator.AddAggregatedQueues(aggregatedViewResponse.QueueAggregatedViewResponse.AggregatedQueues);
						if (this.cmdlet.IsVerbose)
						{
							foreach (ServerSnapshotStatus serverSnapshotStatus in aggregatedViewResponse.SnapshotStatusOfServers)
							{
								this.verboseMessages.Add(new LocalizedString(string.Format("{0}: {1}", serverToConnectTo.Name, serverSnapshotStatus.ToString())));
							}
						}
						if (webServiceRequestAsyncState.IsConnectingToDag)
						{
							this.pendingRequestsToDag[dagIndex] = 0;
						}
					}
					else if (webServiceRequestAsyncState.IsConnectingToDag)
					{
						this.pendingRequestsToDag[dagIndex]--;
						if (this.pendingRequestsToDag[dagIndex] > 0)
						{
							flag = true;
						}
						else
						{
							this.failedToConnectDags.Add(serverToConnectTo.Name);
						}
					}
					else
					{
						this.failedToConnectServers.Add(serverToConnectTo.Name);
					}
					if (text != null)
					{
						this.debugMessages.Add(new LocalizedString(string.Format("{0}: {1}", serverToConnectTo.Name, text)));
					}
					if (localizedString != null)
					{
						this.warningMessages.Add(localizedString.Value);
					}
					if (!flag)
					{
						this.webServiceRequestsPending--;
					}
					if (this.webServiceRequestsPending == 0)
					{
						this.webServiceRequestsDone.Set();
					}
				}
			}
		}

		private void ThreadProcForBeginGetAggregatedViewFailed(object state)
		{
			this.OnInvokeWebServiceCompleted((AsyncResult)state);
		}

		private List<ADObjectId> GetServersToConnectPreferringLocalToSite(HashSet<ADObjectId> serversInDag, int numEntries)
		{
			List<ADObjectId> list = new List<ADObjectId>(numEntries);
			List<ADObjectId> list2 = new List<ADObjectId>(serversInDag.Count);
			List<ADObjectId> list3 = new List<ADObjectId>(serversInDag.Count);
			foreach (ADObjectId adobjectId in serversInDag)
			{
				ADObjectId adobjectId2 = null;
				if (this.serverToSiteMap.TryGetValue(adobjectId, out adobjectId2) && adobjectId2.Equals(this.localSiteId))
				{
					list2.Add(adobjectId);
				}
				else
				{
					list3.Add(adobjectId);
				}
			}
			if (list2.Count > 0)
			{
				RoutingUtils.ShuffleList<ADObjectId>(list2);
				foreach (ADObjectId item in list2)
				{
					list.Add(item);
					if (list.Count == numEntries)
					{
						break;
					}
				}
			}
			if (list.Count < numEntries)
			{
				RoutingUtils.ShuffleList<ADObjectId>(list3);
				foreach (ADObjectId item2 in list3)
				{
					list.Add(item2);
					if (list.Count == numEntries)
					{
						break;
					}
				}
			}
			return list;
		}

		private IEnumerable<Server> GetAllServersInForest()
		{
			if (this.allServersInForest == null)
			{
				ADPagedReader<Server> adpagedReader = this.session.FindPaged<Server>(null, QueryScope.SubTree, new AndFilter(new QueryFilter[]
				{
					new BitMaskAndFilter(ServerSchema.CurrentServerRole, 32UL),
					this.cmdlet.IncludeE14Servers ? DiagnosticsAggregationHelper.IsE14OrHigherQueryFilter : DiagnosticsAggregationHelper.IsE15OrHigherQueryFilter
				}), null, 0);
				this.allServersInForest = new List<Server>();
				foreach (Server item in adpagedReader)
				{
					this.allServersInForest.Add(item);
				}
			}
			return this.allServersInForest;
		}

		private void WriteWarningForConnectionFailure(List<string> failedToConnectToServer)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (string value in failedToConnectToServer)
			{
				if (stringBuilder.Length != 0)
				{
					stringBuilder.Append(", ");
				}
				stringBuilder.Append(value);
			}
			this.cmdlet.WriteWarning(Strings.GetQueueDigestFailedToConnectTo(stringBuilder.ToString()));
		}

		private void Display(IEnumerable<AggregatedQueueInfo> aggregatedQueues)
		{
			foreach (AggregatedQueueInfo aggregatedQueueInfo in aggregatedQueues)
			{
				QueueDigestPresentationObject sendToPipeline = QueueDigestPresentationObject.Create(aggregatedQueueInfo);
				this.cmdlet.WriteObject(sendToPipeline);
			}
		}

		private static Binding GetWebServiceBinding(EnhancedTimeSpan timeout)
		{
			if (GetQueueDigestWebServiceImpl.webServiceBinding == null)
			{
				GetQueueDigestWebServiceImpl.webServiceBinding = new NetTcpBinding
				{
					Security = 
					{
						Transport = 
						{
							ProtectionLevel = ProtectionLevel.EncryptAndSign,
							ClientCredentialType = TcpClientCredentialType.Windows
						},
						Message = 
						{
							ClientCredentialType = MessageCredentialType.Windows
						}
					},
					MaxReceivedMessageSize = (long)((int)ByteQuantifiedSize.FromMB(5UL).ToBytes()),
					OpenTimeout = timeout,
					CloseTimeout = timeout,
					SendTimeout = timeout,
					ReceiveTimeout = timeout
				};
			}
			return GetQueueDigestWebServiceImpl.webServiceBinding;
		}

		private static bool IsQuotaExceeded(CommunicationException communicationException)
		{
			return communicationException.InnerException != null && communicationException.InnerException is QuotaExceededException;
		}

		private const int MaxRequestsPerDag = 3;

		private static Binding webServiceBinding;

		private List<Server> allServersInForest;

		private Dictionary<ADObjectId, ADObjectId> serverToSiteMap = new Dictionary<ADObjectId, ADObjectId>();

		private Dictionary<GroupOfServersKey, HashSet<ADObjectId>> dagToServersMap = new Dictionary<GroupOfServersKey, HashSet<ADObjectId>>();

		private HashSet<ADObjectId> serversNotBelongingToAnyDag = new HashSet<ADObjectId>();

		private ADObjectId localSiteId;

		private AutoResetEvent webServiceRequestsDone;

		private int webServiceRequestsPending;

		private int[] pendingRequestsToDag;

		private QueueAggregator aggregator;

		private List<string> failedToConnectServers = new List<string>();

		private List<string> failedToConnectDags = new List<string>();

		private List<LocalizedString> warningMessages = new List<LocalizedString>();

		private List<LocalizedString> debugMessages = new List<LocalizedString>();

		private List<LocalizedString> verboseMessages = new List<LocalizedString>();

		private GetQueueDigestAdapter cmdlet;

		private readonly int webServicePortNumber;

		private ITopologyConfigurationSession session;

		internal class WebServiceRequestAsyncState
		{
			public IDiagnosticsAggregationService Client { get; set; }

			public ADObjectId ServerToConnectTo { get; set; }

			public HashSet<ADObjectId> ServersToInclude { get; set; }

			public bool IsConnectingToDag { get; set; }

			public int DagIndex { get; set; }

			public bool FailedOnBegin { get; set; }

			public Exception FailedOnBeginException { get; set; }

			public int WebServicePortNumber { get; set; }
		}
	}
}
