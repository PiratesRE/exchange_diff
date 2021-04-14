using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.ServiceModel;
using System.Threading;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Net.DiagnosticsAggregation;
using Microsoft.Exchange.Threading;
using Microsoft.Exchange.Transport.DiagnosticsAggregationService;

namespace Microsoft.Exchange.Servicelets.DiagnosticsAggregation
{
	internal class GroupQueuesDataProvider : IGroupQueuesDataProvider
	{
		public GroupQueuesDataProvider(DiagnosticsAggregationLog log)
		{
			this.log = log;
			this.currentGroupServers = new HashSet<ADObjectId>();
			this.currentGroupServerToQueuesMap = new Dictionary<ADObjectId, ServerQueuesSnapshot>();
			this.timer = null;
		}

		public void Start()
		{
			this.RefreshCurrentGroupServerToQueuesMap();
			TimeSpan timeSpan = DiagnosticsAggregationServicelet.TransportSettings.QueueDiagnosticsAggregationInterval;
			this.timer = new GuardedTimer(new TimerCallback(this.OnTimerFired), null, timeSpan, timeSpan);
		}

		public void Stop()
		{
			if (this.timer != null)
			{
				this.timer.Dispose(true);
			}
		}

		public IDictionary<ADObjectId, ServerQueuesSnapshot> GetCurrentGroupServerToQueuesMap()
		{
			IDictionary<ADObjectId, ServerQueuesSnapshot> result;
			lock (this)
			{
				result = new Dictionary<ADObjectId, ServerQueuesSnapshot>(this.currentGroupServerToQueuesMap);
			}
			return result;
		}

		private void OnTimerFired(object state)
		{
			this.RefreshCurrentGroupServerToQueuesMap();
		}

		private void RefreshCurrentGroupServerToQueuesMap()
		{
			ADNotificationAdapter.TryRunADOperation(new ADOperation(this.RefreshCurrentGroupServers), 2);
			lock (this)
			{
				ADObjectId[] array = new ADObjectId[this.currentGroupServerToQueuesMap.Keys.Count];
				this.currentGroupServerToQueuesMap.Keys.CopyTo(array, 0);
				foreach (ADObjectId adobjectId in array)
				{
					if (!this.currentGroupServers.Contains(adobjectId))
					{
						this.currentGroupServerToQueuesMap.Remove(adobjectId);
					}
				}
			}
			foreach (ADObjectId adobjectId2 in this.currentGroupServers)
			{
				string uri = string.Format(CultureInfo.InvariantCulture, DiagnosticsAggregationHelper.DiagnosticsAggregationEndpointFormat, new object[]
				{
					adobjectId2.Name,
					DiagnosticsAggregationServicelet.TransportSettings.DiagnosticsAggregationServicePort
				});
				Exception ex = null;
				DiagnosticsAggregationServiceClient diagnosticsAggregationServiceClient = null;
				try
				{
					diagnosticsAggregationServiceClient = new DiagnosticsAggregationServiceClient(DiagnosticsAggregationServicelet.GetTcpBinding(), new EndpointAddress(uri));
				}
				catch (UriFormatException ex2)
				{
					ex = ex2;
				}
				LocalViewRequest localViewRequest = new LocalViewRequest(RequestType.Queues);
				localViewRequest.QueueLocalViewRequest = new QueueLocalViewRequest();
				Stopwatch stopwatch = new Stopwatch();
				stopwatch.Start();
				GroupQueuesDataProvider.GetLocalViewAsyncState asyncState = new GroupQueuesDataProvider.GetLocalViewAsyncState
				{
					Client = diagnosticsAggregationServiceClient,
					Server = adobjectId2,
					Stopwatch = stopwatch,
					RequestSessionId = localViewRequest.ClientInformation.SessionId
				};
				try
				{
					if (diagnosticsAggregationServiceClient != null)
					{
						diagnosticsAggregationServiceClient.BeginGetLocalView(localViewRequest, new AsyncCallback(this.OnGetLocalViewCompleted), asyncState);
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
					WcfUtils.DisposeWcfClientGracefully(diagnosticsAggregationServiceClient, false);
					stopwatch.Stop();
					this.UpdateSnapshotForServer(adobjectId2, localViewRequest.ClientInformation.SessionId, stopwatch.Elapsed, null, ex.Message);
					if (ex is InsufficientMemoryException)
					{
						lock (this)
						{
							this.log.Log(DiagnosticsAggregationEvent.OutOfResources, "running out of ephemeral ports, will stop making further web service requests", new object[0]);
							break;
						}
					}
				}
			}
		}

		private void OnGetLocalViewCompleted(IAsyncResult ar)
		{
			GroupQueuesDataProvider.GetLocalViewAsyncState getLocalViewAsyncState = (GroupQueuesDataProvider.GetLocalViewAsyncState)ar.AsyncState;
			DiagnosticsAggregationServiceClient client = getLocalViewAsyncState.Client;
			ADObjectId server = getLocalViewAsyncState.Server;
			Stopwatch stopwatch = getLocalViewAsyncState.Stopwatch;
			LocalViewResponse response = null;
			string errorMessage = null;
			try
			{
				response = client.EndGetLocalView(ar);
			}
			catch (FaultException<DiagnosticsAggregationFault> faultException)
			{
				errorMessage = faultException.Detail.ToString();
			}
			catch (CommunicationException ex)
			{
				errorMessage = ex.Message;
			}
			catch (TimeoutException ex2)
			{
				errorMessage = ex2.Message;
			}
			finally
			{
				WcfUtils.DisposeWcfClientGracefully(client, false);
			}
			stopwatch.Stop();
			this.UpdateSnapshotForServer(server, getLocalViewAsyncState.RequestSessionId, stopwatch.Elapsed, response, errorMessage);
		}

		private void UpdateSnapshotForServer(ADObjectId server, uint requestSessionId, TimeSpan operationDuration, LocalViewResponse response, string errorMessage)
		{
			lock (this)
			{
				ServerQueuesSnapshot serverQueuesSnapshot = null;
				bool flag2 = this.currentGroupServerToQueuesMap.TryGetValue(server, out serverQueuesSnapshot);
				if (flag2)
				{
					serverQueuesSnapshot = serverQueuesSnapshot.Clone();
				}
				else
				{
					serverQueuesSnapshot = new ServerQueuesSnapshot(server);
				}
				if (response == null)
				{
					serverQueuesSnapshot.UpdateFailure(errorMessage);
					this.log.LogOperationToServer(DiagnosticsAggregationEvent.LocalViewRequestSentFailed, requestSessionId, server.Name, new TimeSpan?(operationDuration), errorMessage);
				}
				else
				{
					serverQueuesSnapshot.UpdateSuccess(response);
					this.log.LogOperationToServer(DiagnosticsAggregationEvent.LocalViewResponseReceived, requestSessionId, server.Name, new TimeSpan?(operationDuration), "");
				}
				this.currentGroupServerToQueuesMap[server] = serverQueuesSnapshot;
			}
		}

		private void RefreshCurrentGroupServers()
		{
			ITopologyConfigurationSession session = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(true, ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 306, "RefreshCurrentGroupServers", "f:\\15.00.1497\\sources\\dev\\Management\\src\\ServiceHost\\Servicelets\\DiagnosticsAggregation\\Program\\GroupQueuesDataProvider.cs");
			Server localServer = DiagnosticsAggregationServicelet.LocalServer;
			this.currentGroupServers = DiagnosticsAggregationHelper.GetGroupForServer(localServer, session);
			this.currentGroupServers.Remove(localServer.Id);
		}

		private DiagnosticsAggregationLog log;

		private ISet<ADObjectId> currentGroupServers;

		private IDictionary<ADObjectId, ServerQueuesSnapshot> currentGroupServerToQueuesMap;

		private GuardedTimer timer;

		private class GetLocalViewAsyncState
		{
			public DiagnosticsAggregationServiceClient Client { get; set; }

			public ADObjectId Server { get; set; }

			public Stopwatch Stopwatch { get; set; }

			public uint RequestSessionId { get; set; }
		}
	}
}
