using System;
using System.Threading;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Diagnostics.Components.Transport;

namespace Microsoft.Exchange.Transport.RemoteDelivery
{
	internal class ConnectionManager
	{
		public ConnectionManager()
		{
			this.callBackDelegate = new TimerCallback(this.ScheduledConnectionCallback);
		}

		public TimerCallback CallBackDelegate
		{
			get
			{
				return this.callBackDelegate;
			}
		}

		public int ActiveConnections
		{
			get
			{
				return this.activeConnections;
			}
		}

		public void CreateConnectionIfNecessary(RoutedMessageQueue routedMessageQueue)
		{
			if (routedMessageQueue.SupportsFixedPriority)
			{
				this.CreateConnectionIfNecessary(routedMessageQueue, DeliveryPriority.High);
				this.CreateConnectionIfNecessary(routedMessageQueue, DeliveryPriority.Normal);
				this.CreateConnectionIfNecessary(routedMessageQueue, DeliveryPriority.Low);
				return;
			}
			this.CreateConnectionIfNecessary(routedMessageQueue, DeliveryPriority.Normal);
		}

		public void CreateConnectionIfNecessary(RoutedMessageQueue routedMessageQueue, DeliveryPriority priority)
		{
			if (Components.RemoteDeliveryComponent.IsPaused)
			{
				return;
			}
			if (!routedMessageQueue.EvaluateConnectionAttempt(priority))
			{
				return;
			}
			bool flag = true;
			bool flag2 = false;
			bool flag3 = false;
			ExTraceGlobals.FaultInjectionTracer.TraceTest(3401985341U);
			ExTraceGlobals.FaultInjectionTracer.TraceTest(3943050557U);
			ExTraceGlobals.FaultInjectionTracer.TraceTest<string>(3301322045U, routedMessageQueue.NextHopDomain);
			int num = this.IncrementActiveConnections();
			if (!Components.Configuration.LocalServer.TransportServer.MaxOutboundConnections.IsUnlimited && num > Components.Configuration.LocalServer.TransportServer.MaxOutboundConnections.Value)
			{
				ExTraceGlobals.QueuingTracer.TraceDebug<long, int>(0L, "No new connection for queue {0}. Connection {1} exceeds total connection limit", routedMessageQueue.Id, num);
				flag = false;
			}
			long connectionId = 0L;
			if (flag)
			{
				bool flag4 = false;
				if (routedMessageQueue.SupportsFixedPriority)
				{
					num = routedMessageQueue.CreateAttemptingConnection(priority, out connectionId);
					if (num > Components.TransportAppConfig.RemoteDelivery.MaxPerDomainPriorityConnections(priority))
					{
						flag4 = true;
					}
				}
				else
				{
					priority = DeliveryPriority.Normal;
					num = routedMessageQueue.CreateAttemptingConnection(priority, out connectionId);
					if (!Components.Configuration.LocalServer.TransportServer.MaxPerDomainOutboundConnections.IsUnlimited && num > Components.Configuration.LocalServer.TransportServer.MaxPerDomainOutboundConnections.Value)
					{
						flag4 = true;
					}
				}
				flag2 = true;
				if (flag4)
				{
					ExTraceGlobals.QueuingTracer.TraceDebug<DeliveryPriority, long, int>(0L, "No new {0} priority connection for queue {1}. Connection {2} exceeds per-queue connection limit", priority, routedMessageQueue.Id, num);
					flag = false;
				}
			}
			NextHopConnection nextHopConnection = null;
			if (flag)
			{
				nextHopConnection = new NextHopConnection(routedMessageQueue, connectionId, priority, this);
				if (DeliveryType.MapiDelivery == nextHopConnection.Key.NextHopType.DeliveryType)
				{
					int num2 = this.IncrementActiveMapiDeliveryConnections();
					flag3 = true;
					if (num2 > Components.Configuration.LocalServer.MaxConcurrentMailboxDeliveries)
					{
						ExTraceGlobals.QueuingTracer.TraceDebug<long, int, int>(0L, "No new connection for mapi delivery queue {0}. Connection {1} exceeds maximum concurrent delivery limit {2}.", routedMessageQueue.Id, num2, Components.Configuration.LocalServer.MaxConcurrentMailboxDeliveries);
						flag = false;
					}
				}
				else if (DeliveryType.DeliveryAgent == nextHopConnection.Key.NextHopType.DeliveryType)
				{
					DeliveryAgentConnector deliveryAgentConnector;
					if (!Components.RoutingComponent.MailRouter.TryGetLocalSendConnector<DeliveryAgentConnector>(nextHopConnection.Key.NextHopConnector, out deliveryAgentConnector))
					{
						ExTraceGlobals.QueuingTracer.TraceDebug<string, Guid>(0L, "No new connection for queue {0}.  Connector now found for guid {1}", nextHopConnection.Key.NextHopDomain, nextHopConnection.Key.NextHopConnector);
						flag = false;
					}
					else if (num > deliveryAgentConnector.MaxConcurrentConnections)
					{
						ExTraceGlobals.QueuingTracer.TraceDebug<string, string, int>(0L, "No new connection for queue {0}.  Connector {1} already has the maximum number of concurrent connections, {2}", nextHopConnection.Key.NextHopDomain, deliveryAgentConnector.Name, deliveryAgentConnector.MaxConcurrentConnections);
						flag = false;
					}
				}
				else if (DeliveryType.NonSmtpGatewayDelivery == nextHopConnection.Key.NextHopType.DeliveryType)
				{
					if (num > 1)
					{
						flag = false;
					}
					else
					{
						ExTraceGlobals.QueuingTracer.TraceDebug<long, string>(0L, "NonSmtpGatewayQueue ID {0} has Connector {1} .", routedMessageQueue.Id, routedMessageQueue.Key.NextHopDomain);
					}
				}
			}
			if (flag)
			{
				if (num > 1)
				{
					int messageCountThresholdForConcurrentConnections = ConnectionManager.GetMessageCountThresholdForConcurrentConnections(nextHopConnection.Key);
					ExTraceGlobals.FaultInjectionTracer.TraceTest<int>(3710266685U, ref messageCountThresholdForConcurrentConnections);
					if (routedMessageQueue.GetActiveQueueLength(priority) / (num - 1) < messageCountThresholdForConcurrentConnections)
					{
						ExTraceGlobals.QueuingTracer.TraceDebug(0L, "No new {0} priority connection for queue {1}. Current connections:{2}, QLength:{3}", new object[]
						{
							priority,
							routedMessageQueue.Id,
							num - 1,
							routedMessageQueue.GetActiveQueueLength(priority)
						});
						flag = false;
					}
				}
				else if (routedMessageQueue.GetActiveQueueLength(priority) == 0)
				{
					ExTraceGlobals.QueuingTracer.TraceDebug<DeliveryPriority, long>(0L, "No new {0} priority connection for queue {1} because it is empty", priority, routedMessageQueue.Id);
					flag = false;
				}
			}
			if (flag)
			{
				ExTraceGlobals.QueuingTracer.TraceDebug<DeliveryPriority, long, int>(0L, "Create new {0} priority connection for queue {1}, Connection {2}", priority, routedMessageQueue.Id, num);
				ConnectionManager.CreateConnectionInternal(nextHopConnection);
				return;
			}
			this.DecrementActiveConnections();
			if (flag3)
			{
				this.DecrementActiveMapiDeliveryConnections();
			}
			if (flag2)
			{
				routedMessageQueue.CloseConnection(priority, connectionId);
			}
		}

		public int IncrementActiveConnections()
		{
			return Interlocked.Increment(ref this.activeConnections);
		}

		public int DecrementActiveConnections()
		{
			return Interlocked.Decrement(ref this.activeConnections);
		}

		public void DecrementActiveConnections(DeliveryType deliveryType)
		{
			Interlocked.Decrement(ref this.activeConnections);
			if (DeliveryType.MapiDelivery == deliveryType)
			{
				Interlocked.Decrement(ref this.activeMapiDeliveryConnections);
			}
		}

		public int IncrementActiveMapiDeliveryConnections()
		{
			return Interlocked.Increment(ref this.activeMapiDeliveryConnections);
		}

		public int DecrementActiveMapiDeliveryConnections()
		{
			return Interlocked.Decrement(ref this.activeMapiDeliveryConnections);
		}

		private static void CreateConnectionInternal(NextHopConnection connection)
		{
			connection.ResetQueueLastRetryTimeAndError();
			DeliveryType deliveryType = connection.Key.NextHopType.DeliveryType;
			switch (deliveryType)
			{
			case DeliveryType.MapiDelivery:
				LocalDeliveryConnectionHandler.HandleConnection(connection);
				return;
			case DeliveryType.NonSmtpGatewayDelivery:
				NonSmtpGatewayConnectionHandler.HandleConnection(connection);
				return;
			default:
				if (deliveryType != DeliveryType.DeliveryAgent)
				{
					Components.SmtpOutConnectionHandler.HandleConnection(connection);
					return;
				}
				Components.DeliveryAgentConnectionHandler.HandleConnection(connection);
				return;
			}
		}

		private static int GetMessageCountThresholdForConcurrentConnections(NextHopSolutionKey destination)
		{
			if (NextHopType.IsMailboxDeliveryType(destination.NextHopType.DeliveryType))
			{
				return Components.TransportAppConfig.RemoteDelivery.MailboxQueueMessageCountThresholdForConcurrentConnections;
			}
			if (destination.NextHopType.IsSmtpConnectorDeliveryType)
			{
				return Components.TransportAppConfig.RemoteDelivery.SmtpConnectorQueueMessageCountThresholdForConcurrentConnections;
			}
			if (destination.NextHopType.IsHubRelayDeliveryType)
			{
				return Components.TransportAppConfig.RemoteDelivery.IntraorgSmtpQueueMessageCountThresholdForConcurrentConnections;
			}
			return Components.TransportAppConfig.RemoteDelivery.OtherQueueMessageCountThresholdForConcurrentConnections;
		}

		private void ScheduledConnectionCallback(object state)
		{
			RoutedMessageQueue routedMessageQueue = (RoutedMessageQueue)state;
			routedMessageQueue.ResetScheduledCallback();
			routedMessageQueue.IncrementConnectionRetryCount();
			this.CreateConnectionIfNecessary(routedMessageQueue);
		}

		private int activeConnections;

		private int activeMapiDeliveryConnections;

		private TimerCallback callBackDelegate;
	}
}
