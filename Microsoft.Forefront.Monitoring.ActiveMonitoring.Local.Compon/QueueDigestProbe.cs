using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.Security;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading;
using System.Xml;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.ForefrontActiveMonitoring;
using Microsoft.Exchange.Management.QueueDigest;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Net.DiagnosticsAggregation;
using Microsoft.Exchange.Transport.DiagnosticsAggregationService;
using Microsoft.Forefront.Monitoring.ActiveMonitoring.Smtp.Probes;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring
{
	public class QueueDigestProbe : ProbeWorkItem
	{
		internal bool IsDataCenter
		{
			get
			{
				return LocalEndpointManager.IsDataCenter;
			}
		}

		internal string JobReport
		{
			get
			{
				return string.Format("QueueDigestProbe Job Report{0}{0}{1}", Environment.NewLine, this.jobDetails.ToString());
			}
		}

		protected override void DoWork(CancellationToken cancellationToken)
		{
			try
			{
				this.session = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(true, ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 114, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\QueueDigest\\Probes\\QueueDigestProbe.cs");
				if (!this.ShouldRun())
				{
					this.TraceDebug("QueueDigestProbe on {0} skipped because ShouldRun() returned false", new object[]
					{
						Environment.MachineName
					});
				}
				else
				{
					this.cancellationToken = cancellationToken;
					this.TraceDebug("QueueDigestProbe on {0} starts querying queue stats", new object[]
					{
						Environment.MachineName
					});
					this.GetConfiguration();
					this.DoProbeWork();
					WTFDiagnostics.TraceDebug(ExTraceGlobals.QueueDigestTracer, new TracingContext(), this.JobReport, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\QueueDigest\\Probes\\QueueDigestProbe.cs", 134);
				}
			}
			catch (Exception ex)
			{
				this.TraceDebug("QueueDigestProbe failded due to an exception: {0}", new object[]
				{
					ex.ToString()
				});
				throw;
			}
		}

		private bool ShouldRun()
		{
			Server server = this.session.FindLocalServer();
			IList<Database> list;
			if (!server.IsMailboxServer || !this.TryLoadAdObjects<Database>(this.session.GetDatabaseAvailabilityGroupContainerId(), out list))
			{
				return false;
			}
			LocalEndpointManager instance = LocalEndpointManager.Instance;
			if (instance == null || instance.MailboxDatabaseEndpoint == null)
			{
				throw new SmtpConnectionProbeException("No MailboxDatabaseEndpoint for Backend found on this server");
			}
			return instance.MailboxDatabaseEndpoint.MailboxDatabaseInfoCollectionForBackend.Count != 0;
		}

		private bool TryLoadAdObjects<T>(ADObjectId rootId, out IList<T> objects) where T : ADConfigurationObject, new()
		{
			List<T> results = new List<T>();
			bool result = ADNotificationAdapter.TryReadConfigurationPaged<T>(() => this.session.FindPaged<T>(rootId, QueryScope.SubTree, null, null, ADGenericPagedReader<T>.DefaultPageSize), delegate(T configObject)
			{
				results.Add(configObject);
			});
			objects = results;
			return result;
		}

		private void GetConfiguration()
		{
			if (!this.IsDataCenter && base.Definition.Name.Equals("WellKnownDestinationProbe", StringComparison.OrdinalIgnoreCase))
			{
				this.queues = QueueConfiguration.GetRemoteDomains(this.session);
			}
			else
			{
				if (string.IsNullOrWhiteSpace(base.Definition.ExtensionAttributes))
				{
					throw new XmlException("ExtensionAttributes in probe definition is null");
				}
				XmlDocument xmlDocument = new SafeXmlDocument();
				xmlDocument.LoadXml(base.Definition.ExtensionAttributes);
				XmlNode xmlNode = Utils.CheckNode(xmlDocument.SelectSingleNode("//Queues"), "Queues");
				foreach (object obj in xmlNode.ChildNodes)
				{
					XmlNode queueNode = (XmlNode)obj;
					QueueConfiguration queueConfiguration = new QueueConfiguration(queueNode);
					queueConfiguration.ResolveParameters();
					this.queues.Add(queueConfiguration);
				}
			}
			this.TraceDebug("Work definition processed. QueueCount={0}", new object[]
			{
				(this.queues == null) ? 0 : this.queues.Count
			});
		}

		private void DoProbeWork()
		{
			foreach (QueueConfiguration queueConfiguration in this.queues)
			{
				TransportConfigContainer transportConfigContainer = queueConfiguration.Session.FindSingletonConfigurationObject<TransportConfigContainer>();
				queueConfiguration.Aggregator = new QueueAggregator(queueConfiguration.AggregatedBy, queueConfiguration.DetailsLevel);
				if (queueConfiguration.ServersNotBelongingToAnyDag != null && queueConfiguration.ServersNotBelongingToAnyDag.ToList<ADObjectId>().Count > 0)
				{
					List<ADObjectId> list = (from serverObject in queueConfiguration.ServersNotBelongingToAnyDag
					where serverObject.Name.Equals(Environment.MachineName, StringComparison.InvariantCultureIgnoreCase)
					select serverObject).ToList<ADObjectId>();
					if (list.Count > 0)
					{
						throw new Exception(string.Format("This server {0} is not part of any DAG", Environment.MachineName));
					}
				}
				queueConfiguration.WebServiceRequestsPending = queueConfiguration.DagToServersMap.Count;
				queueConfiguration.WebServiceRequestsDone = new AutoResetEvent(queueConfiguration.WebServiceRequestsPending == 0);
				Stopwatch stopwatch = new Stopwatch();
				stopwatch.Start();
				foreach (KeyValuePair<GroupOfServersKey, List<ADObjectId>> serversInDag in queueConfiguration.DagToServersMap)
				{
					List<ADObjectId> list2;
					HashSet<ADObjectId> hashSet;
					queueConfiguration.GetServersToConnectPreferingServersSpecified(serversInDag, out list2, out hashSet);
					StringBuilder stringBuilder = new StringBuilder();
					foreach (ADObjectId adobjectId in list2)
					{
						stringBuilder.Append(adobjectId.Name + ", ");
					}
					this.TraceDebug("Connecting to servers {0} at port {1}", new object[]
					{
						stringBuilder.ToString(),
						transportConfigContainer.DiagnosticsAggregationServicePort
					});
					stringBuilder.Clear();
					foreach (ADObjectId adobjectId2 in hashSet)
					{
						stringBuilder.Append(adobjectId2.Name + ", ");
					}
					this.TraceDebug("Queue Digest to include servers {0}", new object[]
					{
						stringBuilder.ToString()
					});
					this.InvokeWebServiceAsync(list2, hashSet, true, 1, transportConfigContainer.DiagnosticsAggregationServicePort, queueConfiguration);
				}
				queueConfiguration.WebServiceRequestsDone.WaitOne(-1, this.cancellationToken.IsCancellationRequested);
				this.CheckCancellation();
				switch (queueConfiguration.QueueType)
				{
				case QueueType.WellKnownDestination:
					this.ProcessWellKnownDestinationQueueStats(queueConfiguration);
					break;
				case QueueType.Aggregated:
					this.ProcessAggregatedQueueStats(queueConfiguration);
					break;
				}
				stopwatch.Stop();
				this.TraceDebug("TotalStatsQueryTime={0}", new object[]
				{
					stopwatch.Elapsed
				});
				this.CheckCancellation();
			}
		}

		private void CheckCancellation()
		{
			if (this.cancellationToken.IsCancellationRequested)
			{
				throw new OperationCanceledException();
			}
		}

		private bool IsDesiredWellKnwonDestinationStats(QueueConfiguration config, TransportQueueStatistics queueStats)
		{
			return queueStats.NextHopDomain.ToLower() == config.Destination.ToLower();
		}

		private bool IsDesiredAggregatedQueueStats(QueueConfiguration config, TransportQueueStatistics queueStats)
		{
			return queueStats.MessageCount > config.MessageCountGreaterThan && (string.IsNullOrWhiteSpace(config.Destination) || !(config.Destination.ToLower() != queueStats.NextHopDomain.ToLower())) && (config.DeliveryType == DeliveryType.Undefined || !(config.DeliveryType.ToString().ToLower() != queueStats.DeliveryType.ToLower()));
		}

		private void ProcessWellKnownDestinationQueueStats(QueueConfiguration config)
		{
			int num = 0;
			foreach (AggregatedQueueInfo aggregatedQueueInfo in config.Aggregator.GetResultSortedByMessageCount(config.ResultSize))
			{
				this.ProcessWellKnownDestinationMessageCount(config, aggregatedQueueInfo.MessageCount, aggregatedQueueInfo.DeferredMessageCount, aggregatedQueueInfo.LockedMessageCount, this.ToString(aggregatedQueueInfo));
				this.AppendJobDetails(config, aggregatedQueueInfo);
				num++;
			}
			if (config.FailedToConnectServers.Count != 0)
			{
				this.TraceDebug("Failed to connected to servers: {0}", new object[]
				{
					string.Join(",", config.FailedToConnectServers)
				});
			}
			if (config.FailedToConnectDags.Count != 0)
			{
				this.TraceDebug("Failed to connected to dags: {0}", new object[]
				{
					string.Join(",", config.FailedToConnectDags)
				});
			}
			if (num == 0)
			{
				this.ActionOnNoQueueStats(config);
			}
		}

		private void ProcessAggregatedQueueStats(QueueConfiguration config)
		{
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			List<AggregatedQueueInfo> list = new List<AggregatedQueueInfo>();
			foreach (AggregatedQueueInfo aggregatedQueueInfo in config.Aggregator.GetResultSortedByMessageCount(config.ResultSize))
			{
				if (!config.ShouldExemptPoisonQueues || !aggregatedQueueInfo.GroupByValue.Equals("Poison Message", StringComparison.InvariantCultureIgnoreCase))
				{
					if (aggregatedQueueInfo.MessageCount > config.MessageCountGreaterThan)
					{
						list.Add(aggregatedQueueInfo);
						num2 += aggregatedQueueInfo.MessageCount;
						if (num3 == 0 || aggregatedQueueInfo.MessageCount < num3)
						{
							num3 = aggregatedQueueInfo.MessageCount;
						}
					}
					this.AppendJobDetails(config, aggregatedQueueInfo);
					num++;
				}
			}
			if (config.FailedToConnectServers.Count != 0)
			{
				this.TraceDebug("Failed to connected to servers: {0}", new object[]
				{
					string.Join(",", config.FailedToConnectServers)
				});
			}
			if (config.FailedToConnectDags.Count != 0)
			{
				this.TraceDebug("Failed to connected to dags: {0}", new object[]
				{
					string.Join(",", config.FailedToConnectDags)
				});
			}
			if (num == 0)
			{
				this.ActionOnNoQueueStats(config);
				return;
			}
			int averageMessageCount = (list.Count == 0) ? 0 : (num2 / list.Count);
			this.ProcessAggregatedQueueMessageCount(config, num2, averageMessageCount);
			foreach (AggregatedQueueInfo aggregatedQueueInfo2 in list)
			{
				this.ProcessAggregatedQueueMessageCount(config, aggregatedQueueInfo2.MessageCount, aggregatedQueueInfo2.DeferredMessageCount, aggregatedQueueInfo2.LockedMessageCount, averageMessageCount, num3, this.ToString(aggregatedQueueInfo2));
			}
		}

		private void InvokeWebServiceAsync(List<ADObjectId> serversToConnect, HashSet<ADObjectId> serversToInclude, bool isConnectingToDag, int connectionAttempt, int portNumber, QueueConfiguration config)
		{
			ADObjectId adobjectId = serversToConnect[0];
			string uri = string.Format(CultureInfo.InvariantCulture, DiagnosticsAggregationHelper.DiagnosticsAggregationEndpointFormat, new object[]
			{
				adobjectId.Name,
				portNumber
			});
			DiagnosticsAggregationServiceClient diagnosticsAggregationServiceClient = new DiagnosticsAggregationServiceClient(this.GetWebServiceBinding(config), new EndpointAddress(uri));
			List<string> list = new List<string>();
			if (serversToInclude != null && serversToInclude.Count != 0)
			{
				foreach (ADObjectId adobjectId2 in serversToInclude)
				{
					list.Add(adobjectId2.Name);
				}
			}
			AggregatedViewRequest aggregatedViewRequest = new AggregatedViewRequest(RequestType.Queues, list, config.ServerSideResultSize);
			aggregatedViewRequest.QueueAggregatedViewRequest = new QueueAggregatedViewRequest(config.AggregatedBy, config.DetailsLevel, config.Filter);
			QueueDigestProbe.WebServiceRequestAsyncState asyncState = new QueueDigestProbe.WebServiceRequestAsyncState
			{
				Client = diagnosticsAggregationServiceClient,
				ServersToConnect = serversToConnect,
				ServersToInclude = serversToInclude,
				IsConnectingToDag = isConnectingToDag,
				ConnectionAttempt = connectionAttempt,
				WebServicePortNumber = portNumber,
				Config = config
			};
			diagnosticsAggregationServiceClient.BeginGetAggregatedView(aggregatedViewRequest, new AsyncCallback(this.OnInvokeWebServiceCompleted), asyncState);
		}

		private Binding GetWebServiceBinding(QueueConfiguration config)
		{
			if (config.WebServiceBinding == null)
			{
				EnhancedTimeSpan enhancedTimeSpan = EnhancedTimeSpan.FromSeconds(20.0);
				config.WebServiceBinding = new NetTcpBinding
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
					OpenTimeout = enhancedTimeSpan,
					CloseTimeout = enhancedTimeSpan,
					SendTimeout = enhancedTimeSpan,
					ReceiveTimeout = enhancedTimeSpan
				};
			}
			return config.WebServiceBinding;
		}

		private void OnInvokeWebServiceCompleted(IAsyncResult ar)
		{
			QueueDigestProbe.WebServiceRequestAsyncState webServiceRequestAsyncState = (QueueDigestProbe.WebServiceRequestAsyncState)ar.AsyncState;
			DiagnosticsAggregationServiceClient client = webServiceRequestAsyncState.Client;
			ADObjectId adobjectId = webServiceRequestAsyncState.ServersToConnect[0];
			QueueConfiguration config = webServiceRequestAsyncState.Config;
			webServiceRequestAsyncState.ServersToConnect.RemoveAt(0);
			AggregatedViewResponse aggregatedViewResponse = null;
			try
			{
				aggregatedViewResponse = client.EndGetAggregatedView(ar);
			}
			catch (FaultException<DiagnosticsAggregationFault> faultException)
			{
				this.TraceDebug("DiagnosticsAggregationFault: {0}, {1}", new object[]
				{
					faultException.Detail.ErrorCode,
					faultException.Detail.Message
				});
			}
			catch (CommunicationException ex)
			{
				if (ex.InnerException != null && ex.InnerException is QuotaExceededException)
				{
					this.TraceDebug("CommunicationException: {0} quota exceeded", new object[]
					{
						adobjectId.Name
					});
				}
				this.TraceDebug(ex.ToString(), new object[0]);
			}
			catch (TimeoutException ex2)
			{
				this.TraceDebug("TimeoutException: {0}", new object[]
				{
					ex2.ToString()
				});
			}
			catch (Exception ex3)
			{
				this.TraceDebug("Exception: {0}", new object[]
				{
					ex3.ToString()
				});
			}
			finally
			{
				WcfUtils.DisposeWcfClientGracefully(client, false);
			}
			bool flag = false;
			lock (this)
			{
				if (aggregatedViewResponse != null)
				{
					config.Aggregator.AddAggregatedQueues(aggregatedViewResponse.QueueAggregatedViewResponse.AggregatedQueues);
					using (List<ServerSnapshotStatus>.Enumerator enumerator = aggregatedViewResponse.SnapshotStatusOfServers.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							ServerSnapshotStatus serverSnapshotStatus = enumerator.Current;
							this.TraceDebug("{0}: {1}", new object[]
							{
								adobjectId.Name,
								serverSnapshotStatus.ToString()
							});
						}
						goto IL_1EF;
					}
				}
				if (webServiceRequestAsyncState.IsConnectingToDag)
				{
					if (webServiceRequestAsyncState.ConnectionAttempt <= 3 && webServiceRequestAsyncState.ServersToConnect.Count > 0)
					{
						flag = true;
					}
					else
					{
						config.FailedToConnectDags.Add(adobjectId.Name);
					}
				}
				else
				{
					config.FailedToConnectServers.Add(adobjectId.Name);
				}
				IL_1EF:
				if (!flag)
				{
					config.WebServiceRequestsPending--;
				}
				if (config.WebServiceRequestsPending == 0)
				{
					config.WebServiceRequestsDone.Set();
				}
			}
			if (flag)
			{
				this.InvokeWebServiceAsync(webServiceRequestAsyncState.ServersToConnect, webServiceRequestAsyncState.ServersToInclude, webServiceRequestAsyncState.IsConnectingToDag, webServiceRequestAsyncState.ConnectionAttempt + 1, webServiceRequestAsyncState.WebServicePortNumber, webServiceRequestAsyncState.Config);
			}
		}

		private void ProcessWellKnownDestinationMessageCount(QueueConfiguration config, int messageCount, int deferredMessageCount, int lockedMessageCount, string statsDetails)
		{
			if (config.MessageCountThreshold > 0 && messageCount > config.MessageCountThreshold)
			{
				string text = string.Format("MessageCount of this destination has exceeded the configured threshold: {0}. QueueStats: {1}. Configuration: {2}", config.Destination, statsDetails, config.ToString());
				this.PublishEventNotification(config, text);
				this.TraceDebug(text, new object[0]);
			}
			if (config.DeferredMessageCountThreshold > 0 && deferredMessageCount > config.DeferredMessageCountThreshold)
			{
				string text2 = string.Format("DeferredMessageCount of this destination has exceeded the configured threshold: {0}. QueueStats: {1}. Configuration: {2}", config.Destination, statsDetails, config.ToString());
				this.PublishEventNotification(config, text2);
				this.TraceDebug(text2, new object[0]);
			}
			if (config.LockedMessageCountThreshold > 0 && lockedMessageCount > config.LockedMessageCountThreshold)
			{
				string text3 = string.Format("LockedMessageCount of this destination has exceeded the configured threshold: {0}. QueueStats: {1}. Configuration: {2}", config.Destination, statsDetails, config.ToString());
				this.PublishEventNotification(config, text3);
				this.TraceDebug(text3, new object[0]);
			}
		}

		private void ProcessAggregatedQueueMessageCount(QueueConfiguration config, int totalMessageCount, int averageMessageCount)
		{
			if (config.TotalMessageCountThreshold > 0 && totalMessageCount > config.TotalMessageCountThreshold)
			{
				string text = string.Format("Total message count {0} has exceeded the configured threshod. Configuration: {1}", totalMessageCount, config.ToString());
				this.PublishEventNotification(config, text);
				this.TraceDebug(text, new object[0]);
				return;
			}
			if (config.AverageMessageCountThreshold > 0 && averageMessageCount > config.AverageMessageCountThreshold)
			{
				string text2 = string.Format("Message count average {0} has exceeded the configured threshod. Configuration: {1}", averageMessageCount, config.ToString());
				this.PublishEventNotification(config, text2);
				this.TraceDebug(text2, new object[0]);
			}
		}

		private void ProcessAggregatedQueueMessageCount(QueueConfiguration config, int messageCount, int deferredMessageCount, int lockedMessageCount, int averageMessageCount, int lowestMessageCount, string statsDetails)
		{
			if (config.MessageCountThreshold > 0 && messageCount > config.MessageCountThreshold)
			{
				string text = string.Format("MessageCount has exceeded the configured threshold: {0}. QueueStats: {1}. Configuration: {2}", messageCount, statsDetails, config.ToString());
				this.PublishEventNotification(config, text);
				this.TraceDebug(text, new object[0]);
			}
			if (config.DeferredMessageCountThreshold > 0 && deferredMessageCount > config.DeferredMessageCountThreshold)
			{
				string text2 = string.Format("DeferredMessageCount has exceeded the configured threshold: {0}. QueueStats: {1}. Configuration: {2}", deferredMessageCount, statsDetails, config.ToString());
				this.PublishEventNotification(config, text2);
				this.TraceDebug(text2, new object[0]);
			}
			if (config.LockedMessageCountThreshold > 0 && lockedMessageCount > config.LockedMessageCountThreshold)
			{
				string text3 = string.Format("LockedMessageCountThreshold has exceeded the configured threshold: {0}. QueueStats: {1}. Configuration: {2}", lockedMessageCount, statsDetails, config.ToString());
				this.PublishEventNotification(config, text3);
				this.TraceDebug(text3, new object[0]);
			}
			if (averageMessageCount > 0 && config.ExceedsAverageByNumber > 0)
			{
				int num = messageCount - averageMessageCount;
				if (num > config.ExceedsAverageByNumber)
				{
					string text4 = string.Format("MessageCount has exceeded average by number: {0}. QueueStats: {1}. Configuration: {2}", num, statsDetails, config.ToString());
					this.PublishEventNotification(config, text4);
					this.TraceDebug(text4, new object[0]);
					return;
				}
			}
			if (averageMessageCount > 0 && config.ExceedsAverageByPercent > 0)
			{
				int num2 = (messageCount - averageMessageCount) * 100 / averageMessageCount;
				if (num2 > config.ExceedsAverageByPercent)
				{
					string text5 = string.Format("MessageCount has exceeded the average by percent: {0}. QueueStats: {1}. Configuration: {2}", num2, statsDetails, config.ToString());
					this.PublishEventNotification(config, text5);
					this.TraceDebug(text5, new object[0]);
					return;
				}
			}
			if (config.ExceedsLowestByNumber > 0)
			{
				int num3 = messageCount - lowestMessageCount;
				if (num3 > config.ExceedsLowestByNumber)
				{
					string text6 = string.Format("MessageCount has exceeded the lowest by number: {0}. QueueStats: {1}. Configuration: {2}", num3, statsDetails, config.ToString());
					this.PublishEventNotification(config, text6);
					this.TraceDebug(text6, new object[0]);
					return;
				}
			}
			if (config.ExceedsLowestByPercent > 0)
			{
				int num4 = (messageCount - lowestMessageCount) * 100 / lowestMessageCount;
				if (num4 > config.ExceedsLowestByPercent)
				{
					string text7 = string.Format("MessageCount has exceeded the lowest by percent: {0}. QueueStats: {1}. Configuration: {2}", num4, statsDetails, config.ToString());
					this.PublishEventNotification(config, text7);
					this.TraceDebug(text7, new object[0]);
				}
			}
		}

		private void PublishEventNotification(QueueConfiguration config, string message)
		{
			EventNotificationItem eventNotificationItem = new EventNotificationItem(config.EventNotificationServiceName, config.EventNotificationComponent, config.EventNotificationTag, message, config.EventNotificationSeverityLevel);
			eventNotificationItem.Publish(false);
		}

		private void ActionOnNoQueueStats(QueueConfiguration config)
		{
			this.TraceDebug("No queue stats available for this configuration: {0}", new object[]
			{
				config.ToString()
			});
			if (config.RaiseWarningOnNoStats)
			{
				EventNotificationItem eventNotificationItem = new EventNotificationItem(config.EventNotificationServiceName, "DestinationQueueStatsNotAvailable", config.EventNotificationTagForNoStats, string.Format("No queue stats available for this destination: {0}", config.ToString()), ResultSeverityLevel.Warning);
				eventNotificationItem.Publish(false);
			}
		}

		private void AppendJobDetails(QueueConfiguration config, TransportQueueStatistics stats)
		{
			this.jobDetails.AppendFormat("QueueConfiguration: {0}{1}", config.ToString(), Environment.NewLine);
			this.jobDetails.AppendFormat("QueueStats: {0}{1}{1}", this.ToString(stats), Environment.NewLine);
		}

		private void AppendJobDetails(QueueConfiguration config, AggregatedQueueInfo stats)
		{
			this.jobDetails.AppendFormat("QueueConfiguration: {0}{1}", config.ToString(), Environment.NewLine);
			this.jobDetails.AppendFormat("QueueStats: {0}{1}{1}", this.ToString(stats), Environment.NewLine);
		}

		private string ToString(TransportQueueStatistics stats)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("DeferredMessageCount={0}, ", stats.DeferredMessageCount);
			stringBuilder.AppendFormat("DeliveryType={0}, ", stats.DeliveryType);
			stringBuilder.AppendFormat("Identity={0}, ", stats.Identity);
			stringBuilder.AppendFormat("IncomingRate={0}, ", stats.IncomingRate);
			stringBuilder.AppendFormat("LastError={0}, ", stats.LastError);
			stringBuilder.AppendFormat("LockedMessageCount={0}, ", stats.LockedMessageCount);
			stringBuilder.AppendFormat("MessageCount={0}, ", stats.MessageCount);
			stringBuilder.AppendFormat("NextHopCategory={0}, ", stats.NextHopCategory);
			stringBuilder.AppendFormat("NextHopDomain={0}, ", stats.NextHopDomain);
			stringBuilder.AppendFormat("NextHopKey={0}", stats.NextHopKey);
			stringBuilder.AppendFormat("OutgoingRate={0}, ", stats.OutgoingRate);
			stringBuilder.AppendFormat("QueueCount={0}, ", stats.QueueCount);
			stringBuilder.AppendFormat("RiskLevel={0}, ", stats.RiskLevel);
			stringBuilder.AppendFormat("ServerName={0}, ", stats.ServerName);
			stringBuilder.AppendFormat("Status={0}, ", stats.Status);
			stringBuilder.AppendFormat("TlsDomain={0}, ", stats.TlsDomain);
			return stringBuilder.ToString();
		}

		private string ToString(AggregatedQueueInfo stats)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("DeferredMessageCount={0}, ", stats.DeferredMessageCount);
			stringBuilder.AppendFormat("IncomingRate={0}, ", stats.IncomingRate);
			stringBuilder.AppendFormat("LockedMessageCount={0}, ", stats.LockedMessageCount);
			stringBuilder.AppendFormat("MessageCount={0}, ", stats.MessageCount);
			stringBuilder.AppendFormat("OutgoingRate={0}, ", stats.OutgoingRate);
			foreach (AggregatedQueueVerboseDetails aggregatedQueueVerboseDetails in stats.VerboseDetails)
			{
				stringBuilder.AppendFormat("DeliveryType={0}, ", aggregatedQueueVerboseDetails.DeliveryType);
				stringBuilder.AppendFormat("LastError={0}, ", aggregatedQueueVerboseDetails.LastError);
				stringBuilder.AppendFormat("NextHopCategory={0}, ", aggregatedQueueVerboseDetails.NextHopCategory);
				stringBuilder.AppendFormat("NextHopConnector={0}, ", aggregatedQueueVerboseDetails.NextHopConnector);
				stringBuilder.AppendFormat("NextHopDomain={0}, ", aggregatedQueueVerboseDetails.NextHopDomain);
				stringBuilder.AppendFormat("QueueIdentity={0}, ", aggregatedQueueVerboseDetails.QueueIdentity);
				stringBuilder.AppendFormat("RiskLevel={0}, ", aggregatedQueueVerboseDetails.RiskLevel);
				stringBuilder.AppendFormat("ServerIdentity={0}, ", aggregatedQueueVerboseDetails.ServerIdentity);
				stringBuilder.AppendFormat("Status={0}, ", aggregatedQueueVerboseDetails.Status);
				stringBuilder.AppendFormat("TlsDomain={0}, ", aggregatedQueueVerboseDetails.TlsDomain);
			}
			return stringBuilder.ToString();
		}

		private void TraceDebug(string format, params object[] args)
		{
			string text = string.Format(format, args);
			ProbeResult result = base.Result;
			result.ExecutionContext += string.Format("[{0}] ", text);
			WTFDiagnostics.TraceDebug(ExTraceGlobals.QueueDigestTracer, new TracingContext(), text, null, "TraceDebug", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\QueueDigest\\Probes\\QueueDigestProbe.cs", 933);
		}

		private const int MaxRetryAttempts = 3;

		private CancellationToken cancellationToken;

		private List<QueueConfiguration> queues = new List<QueueConfiguration>();

		private StringBuilder jobDetails = new StringBuilder();

		private ITopologyConfigurationSession session;

		private class WebServiceRequestAsyncState
		{
			public DiagnosticsAggregationServiceClient Client { get; set; }

			public List<ADObjectId> ServersToConnect { get; set; }

			public HashSet<ADObjectId> ServersToInclude { get; set; }

			public bool IsConnectingToDag { get; set; }

			public int ConnectionAttempt { get; set; }

			public int WebServicePortNumber { get; set; }

			public QueueConfiguration Config { get; set; }
		}
	}
}
