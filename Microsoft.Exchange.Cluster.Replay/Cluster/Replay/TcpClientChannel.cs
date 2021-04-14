using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Principal;
using Microsoft.Exchange.Cluster.Common;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;
using Microsoft.Exchange.EseRepl;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class TcpClientChannel : TcpChannel
	{
		protected TcpClientChannel(string serverNodeName, Socket channel, NegotiateStream s, int timeout) : base(channel, s, timeout, TimeSpan.FromSeconds((double)RegistryParameters.TcpChannelIdleLimitInSec))
		{
			base.PartnerNodeName = serverNodeName;
		}

		internal static TcpClientChannel OpenChannel(NetworkPath netPath, int timeoutInMs)
		{
			NetworkTransportException ex = null;
			TcpClientChannel result = null;
			if (TcpClientChannel.TryOpenChannel(netPath, timeoutInMs, out result, out ex))
			{
				return result;
			}
			throw ex;
		}

		internal static bool TryOpenChannel(NetworkPath netPath, int timeoutInMs, out TcpClientChannel channel, out NetworkTransportException networkEx)
		{
			channel = null;
			networkEx = null;
			Exception ex = null;
			Socket socket = null;
			Stream stream = null;
			NegotiateStream negotiateStream = null;
			ReplayStopwatch replayStopwatch = new ReplayStopwatch();
			replayStopwatch.Start();
			try
			{
				socket = new Socket(netPath.TargetEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
				if (netPath.Purpose == NetworkPath.ConnectionPurpose.Seeding)
				{
					socket.ReceiveBufferSize = RegistryParameters.SeedingNetworkTransferSize;
					socket.SendBufferSize = RegistryParameters.SeedingNetworkTransferSize;
				}
				else
				{
					socket.ReceiveBufferSize = RegistryParameters.LogCopyNetworkTransferSize;
					socket.SendBufferSize = RegistryParameters.LogCopyNetworkTransferSize;
				}
				if (netPath.HasSourceEndpoint())
				{
					socket.Bind(netPath.SourceEndPoint);
				}
				TcpClientChannel.ConnectAbandon connectAbandon = new TcpClientChannel.ConnectAbandon(socket);
				IAsyncResult asyncResult = socket.BeginConnect(netPath.TargetEndPoint.Address, netPath.TargetEndPoint.Port, null, connectAbandon);
				if (!asyncResult.AsyncWaitHandle.WaitOne(timeoutInMs, false))
				{
					socket = null;
					connectAbandon.Cancel(asyncResult);
					TcpChannel.ThrowTimeoutException(netPath.TargetNodeName, ReplayStrings.NetworkConnectionTimeout(timeoutInMs / 1000));
				}
				socket.EndConnect(asyncResult);
				long elapsedMilliseconds = replayStopwatch.ElapsedMilliseconds;
				ExTraceGlobals.TcpClientTracer.TraceDebug<long>(0L, "Connection took {0}ms", elapsedMilliseconds);
				socket.LingerState = new LingerOption(true, 0);
				if (!netPath.UseSocketStream || RegistryParameters.DisableSocketStream)
				{
					stream = new NetworkStream(socket, false);
				}
				else
				{
					stream = new SocketStream(socket, netPath.SocketStreamBufferPool, netPath.SocketStreamAsyncArgPool, netPath.SocketStreamPerfCounters);
				}
				negotiateStream = new NegotiateStream(stream, false);
				stream = null;
				elapsedMilliseconds = replayStopwatch.ElapsedMilliseconds;
				if (elapsedMilliseconds >= (long)timeoutInMs)
				{
					TcpChannel.ThrowTimeoutException(netPath.TargetNodeName, ReplayStrings.NetworkConnectionTimeout(timeoutInMs / 1000));
				}
				int num = timeoutInMs - (int)elapsedMilliseconds;
				negotiateStream.WriteTimeout = num;
				negotiateStream.ReadTimeout = num;
				TcpClientChannel.AuthAbandon authAbandon = new TcpClientChannel.AuthAbandon(socket, negotiateStream);
				string targetName;
				if (netPath.UseNullSpn)
				{
					targetName = "";
				}
				else
				{
					targetName = "HOST/" + netPath.TargetNodeName;
				}
				bool encrypt = netPath.Encrypt;
				ProtectionLevel protectionLevel;
				if (encrypt)
				{
					protectionLevel = ProtectionLevel.EncryptAndSign;
				}
				else if (RegistryParameters.DisableNetworkSigning)
				{
					protectionLevel = ProtectionLevel.None;
				}
				else
				{
					protectionLevel = ProtectionLevel.Sign;
				}
				asyncResult = negotiateStream.BeginAuthenticateAsClient(CredentialCache.DefaultNetworkCredentials, targetName, protectionLevel, TokenImpersonationLevel.Identification, null, authAbandon);
				if (!asyncResult.AsyncWaitHandle.WaitOne(num, false))
				{
					negotiateStream = null;
					socket = null;
					authAbandon.Abandon(asyncResult);
					TcpChannel.ThrowTimeoutException(netPath.TargetNodeName, ReplayStrings.NetworkConnectionTimeout(timeoutInMs / 1000));
				}
				negotiateStream.EndAuthenticateAsClient(asyncResult);
				bool flag = false;
				if (!negotiateStream.IsAuthenticated)
				{
					flag = true;
				}
				else if (protectionLevel != ProtectionLevel.None && !negotiateStream.IsMutuallyAuthenticated)
				{
					if (netPath.IgnoreMutualAuth || MachineName.Comparer.Equals(netPath.TargetNodeName, Environment.MachineName))
					{
						ExTraceGlobals.TcpClientTracer.TraceDebug(0L, "Ignoring mutual auth since we are local");
					}
					else
					{
						flag = true;
					}
				}
				if (!flag && encrypt && !negotiateStream.IsEncrypted)
				{
					ExTraceGlobals.TcpClientTracer.TraceError(0L, "Encryption requested, but could not be negotiated");
					flag = true;
				}
				if (flag)
				{
					ExTraceGlobals.TcpClientTracer.TraceError<bool, bool, bool>(0L, "Security Negotiation failed. Auth={0},MAuth={1},Encrypt={2}", negotiateStream.IsAuthenticated, negotiateStream.IsMutuallyAuthenticated, negotiateStream.IsEncrypted);
					NetworkManager.ThrowException(new NetworkCommunicationException(netPath.TargetNodeName, ReplayStrings.NetworkSecurityFailed));
				}
				ExTraceGlobals.TcpClientTracer.TraceDebug<long, bool, ProtectionLevel>(0L, "Authenticated Connection took {0}ms. Encrypt={1} ProtRequested={2}", replayStopwatch.ElapsedMilliseconds, negotiateStream.IsEncrypted, protectionLevel);
				channel = new TcpClientChannel(netPath.TargetNodeName, socket, negotiateStream, timeoutInMs);
				return true;
			}
			catch (SocketException ex2)
			{
				ex = ex2;
			}
			catch (IOException ex3)
			{
				ex = ex3;
			}
			catch (AuthenticationException ex4)
			{
				ex = ex4;
			}
			catch (NetworkTransportException ex5)
			{
				ex = ex5;
			}
			finally
			{
				if (channel == null)
				{
					if (negotiateStream != null)
					{
						negotiateStream.Dispose();
					}
					else if (stream != null)
					{
						stream.Dispose();
					}
					if (socket != null)
					{
						socket.Close();
					}
				}
				else
				{
					ReplayCrimsonEvents.NetworkConnectionSuccess.Log<string, IPEndPoint, IPEndPoint>(netPath.TargetNodeName, netPath.TargetEndPoint, channel.LocalEndpoint);
				}
			}
			ExTraceGlobals.TcpClientTracer.TraceError<Exception>(0L, "TryOpenChannel failed. Ex={0}", ex);
			ReplayCrimsonEvents.NetworkConnectionFailure.Log<string, IPEndPoint, IPEndPoint, string>(netPath.TargetNodeName, netPath.TargetEndPoint, netPath.SourceEndPoint, ex.ToString());
			if (ex is NetworkTransportException)
			{
				networkEx = (NetworkTransportException)ex;
			}
			else
			{
				networkEx = new NetworkCommunicationException(netPath.TargetNodeName, ex.Message, ex);
			}
			return false;
		}

		public class ConnectAbandon : AbandonAsyncBase
		{
			public ConnectAbandon(Socket socket)
			{
				this.m_connection = socket;
			}

			protected override void EndProcessing(IAsyncResult ar)
			{
				try
				{
					this.m_connection.EndConnect(ar);
				}
				finally
				{
					this.m_connection.Close();
				}
			}

			public void Cancel(IAsyncResult ar)
			{
				base.Abandon(ar);
			}

			private Socket m_connection;
		}

		public class AuthAbandon : AbandonAsyncBase
		{
			public AuthAbandon(Socket socket, NegotiateStream stream)
			{
				this.m_connection = socket;
				this.m_authStream = stream;
			}

			protected override void EndProcessing(IAsyncResult ar)
			{
				try
				{
					this.m_authStream.EndAuthenticateAsClient(ar);
				}
				finally
				{
					this.m_authStream.Close();
					this.m_connection.Close();
				}
			}

			private Socket m_connection;

			private NegotiateStream m_authStream;
		}
	}
}
