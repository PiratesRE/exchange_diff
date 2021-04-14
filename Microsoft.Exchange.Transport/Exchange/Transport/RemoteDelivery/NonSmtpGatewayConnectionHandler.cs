using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Transport;

namespace Microsoft.Exchange.Transport.RemoteDelivery
{
	internal class NonSmtpGatewayConnectionHandler : IStartableTransportComponent, ITransportComponent
	{
		public static ExEventLog EventLogger
		{
			get
			{
				return NonSmtpGatewayConnectionHandler.eventLogger;
			}
		}

		public static void HandleConnection(NextHopConnection connection)
		{
			ExTraceGlobals.QueuingTracer.TraceDebug<string>(0L, "Invoking NonSmtpGatewayConnection for {0}", connection.Key.NextHopDomain);
			ThreadPool.QueueUserWorkItem(new WaitCallback(NonSmtpGatewayConnectionHandler.DeliveryCallback), connection);
		}

		public void Load()
		{
		}

		public void Unload()
		{
		}

		public string OnUnhandledException(Exception e)
		{
			return null;
		}

		public void Start(bool initiallyPaused, ServiceState targetRunningState)
		{
		}

		public void Pause()
		{
		}

		public void Continue()
		{
		}

		public void Stop()
		{
			ExTraceGlobals.QueuingTracer.TraceDebug(0L, "Shutdown called for Gateway Connection Handler");
			lock (NonSmtpGatewayConnectionHandler.syncObject)
			{
				foreach (NonSmtpGatewayConnection nonSmtpGatewayConnection in NonSmtpGatewayConnectionHandler.connections)
				{
					nonSmtpGatewayConnection.Retire();
				}
				goto IL_6B;
			}
			IL_61:
			Thread.Sleep(1000);
			IL_6B:
			if (NonSmtpGatewayConnectionHandler.connections.Count <= 0)
			{
				return;
			}
			goto IL_61;
		}

		public string CurrentState
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder(256);
				stringBuilder.Append("Connection count=");
				stringBuilder.AppendLine(NonSmtpGatewayConnectionHandler.connections.Count.ToString());
				return stringBuilder.ToString();
			}
		}

		private static void DeliveryCallback(object connection)
		{
			NextHopConnection nextHopConnection = connection as NextHopConnection;
			NonSmtpGatewayConnection nonSmtpGatewayConnection = null;
			try
			{
				ExTraceGlobals.QueuingTracer.TraceDebug<string>(0L, "Initiating new outbound non SMTP Gateway connection for {0}", nextHopConnection.Key.NextHopDomain);
				lock (NonSmtpGatewayConnectionHandler.syncObject)
				{
					nonSmtpGatewayConnection = new NonSmtpGatewayConnection(nextHopConnection);
					NonSmtpGatewayConnectionHandler.connections.Add(nonSmtpGatewayConnection);
				}
				nonSmtpGatewayConnection.StartConnection();
			}
			catch (LocalizedException arg)
			{
				nextHopConnection.AckConnection(AckStatus.Retry, AckReason.UnexpectedException, null);
				ExTraceGlobals.QueuingTracer.TraceError<LocalizedException>(0L, "Unexpected exception while starting a non SMTP gateway connection . Exception text: {0}", arg);
			}
			finally
			{
				ExTraceGlobals.QueuingTracer.TraceDebug<string>(0L, "Stop non smtp gateway delivery for connection to {0}", nextHopConnection.Key.NextHopDomain);
				lock (NonSmtpGatewayConnectionHandler.syncObject)
				{
					if (nonSmtpGatewayConnection != null)
					{
						NonSmtpGatewayConnectionHandler.connections.Remove(nonSmtpGatewayConnection);
					}
				}
			}
		}

		private static ExEventLog eventLogger = new ExEventLog(ExTraceGlobals.QueuingTracer.Category, TransportEventLog.GetEventSource());

		private static List<NonSmtpGatewayConnection> connections = new List<NonSmtpGatewayConnection>();

		private static object syncObject = new object();
	}
}
