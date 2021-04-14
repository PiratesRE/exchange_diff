using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;
using Microsoft.Exchange.EseRepl;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class TcpHealthCheck
	{
		internal static bool TestHealth(string targetServer, int targetPort, int timeOutInMs, out string errMsg)
		{
			errMsg = null;
			NetworkChannel networkChannel = null;
			Exception ex = null;
			ExTraceGlobals.TcpChannelTracer.TraceFunction<string>(0L, "TcpHealthCheck: testing {0}", targetServer);
			try
			{
				ushort num = (ushort)targetPort;
				if (num == 0)
				{
					num = 64327;
				}
				ITcpConnector tcpConnector = Dependencies.TcpConnector;
				NetworkPath netPath = tcpConnector.BuildDnsNetworkPath(targetServer, (int)num);
				networkChannel = NetworkChannel.Connect(netPath, TcpChannel.GetDefaultTimeoutInMs(), false);
				TestHealthRequest testHealthRequest = new TestHealthRequest(networkChannel);
				testHealthRequest.Send();
				NetworkChannelMessage message = networkChannel.GetMessage();
				if (!(message is TestHealthReply))
				{
					networkChannel.ThrowUnexpectedMessage(message);
				}
				ExTraceGlobals.TcpChannelTracer.TraceFunction<string>(0L, "TcpHealthCheck: {0} is healthy", targetServer);
				return true;
			}
			catch (NetworkRemoteException ex2)
			{
				ex = ex2.InnerException;
			}
			catch (NetworkTransportException ex3)
			{
				ex = ex3;
			}
			catch (Win32Exception ex4)
			{
				ex = ex4;
			}
			catch (COMException ex5)
			{
				ex = ex5;
			}
			catch (ClusCommonFailException ex6)
			{
				ex = ex6;
			}
			catch (ClusCommonTransientException ex7)
			{
				ex = ex7;
			}
			finally
			{
				if (networkChannel != null)
				{
					networkChannel.Close();
				}
			}
			if (ex != null)
			{
				ExTraceGlobals.TcpChannelTracer.TraceError<Exception>(0L, "TcpHealthCheck: failed: {0}", ex);
				errMsg = ex.Message;
			}
			return false;
		}
	}
}
