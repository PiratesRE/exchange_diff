using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Microsoft.Exchange.Cluster.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class PingProber : IDisposable
	{
		public static Trace Tracer
		{
			get
			{
				return ExTraceGlobals.NetworkManagerTracer;
			}
		}

		public static PingRequest FindRequest(PingRequest[] requests, IPAddress searchForAddr)
		{
			if (requests != null)
			{
				foreach (PingRequest pingRequest in requests)
				{
					if (pingRequest.IPAddress.Equals(searchForAddr))
					{
						return pingRequest;
					}
				}
			}
			return null;
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			if (!this.m_disposed)
			{
				lock (this)
				{
					this.m_proberState = ProberState.Disposed;
					if (disposing)
					{
						this.m_sourcePort.Close();
						((IDisposable)this.m_probeCompleteEvent).Dispose();
					}
					this.m_disposed = true;
				}
			}
		}

		public PingProber(IPAddress sourceIp)
		{
			this.m_sourcePort = new PingSource(sourceIp, 3000);
			this.m_probeCompleteEvent = new ManualResetEvent(false);
		}

		private void CompletionCallback(IAsyncResult ar)
		{
			Exception ex = null;
			try
			{
				IPEndPoint ipendPoint = new IPEndPoint(IPAddress.Any, 0);
				EndPoint endPoint = ipendPoint;
				this.m_sourcePort.Socket.EndReceiveFrom(ar, ref endPoint);
				ipendPoint = (IPEndPoint)endPoint;
				PingRequest pingRequest = PingProber.FindRequest(this.m_activeRequests, ipendPoint.Address);
				if (pingRequest == null)
				{
					PingProber.Tracer.TraceError<IPEndPoint>((long)this.GetHashCode(), "Unexpected response from {0}", ipendPoint);
				}
				else
				{
					pingRequest.StopTimeStamp = Win32StopWatch.GetSystemPerformanceCounter();
					PingProber.Tracer.TraceDebug<IPEndPoint>((long)this.GetHashCode(), "Response from {0}.", ipendPoint);
					pingRequest.Success = true;
				}
			}
			catch (SocketException ex2)
			{
				ex = ex2;
			}
			catch (ObjectDisposedException ex3)
			{
				ex = ex3;
			}
			finally
			{
				lock (this)
				{
					this.m_completionCount++;
					if (this.m_completionCount == this.m_sendCount && (this.m_proberState == ProberState.Sent || this.m_proberState == ProberState.Gathering))
					{
						this.m_probeCompleteEvent.Set();
					}
				}
				if (ex != null && this.m_proberState != ProberState.Disposed)
				{
					PingProber.Tracer.TraceError<Exception>((long)this.GetHashCode(), "Ping completion failure: {0}", ex);
				}
			}
		}

		public void SendPings(PingRequest[] targets)
		{
			if (this.m_proberState != ProberState.Idle)
			{
				throw new ArgumentException("user must gather between send intervals");
			}
			try
			{
				this.m_proberState = ProberState.Sending;
				this.m_activeRequests = targets;
				foreach (PingRequest pingRequest in targets)
				{
					IPEndPoint ipendPoint = new IPEndPoint(pingRequest.IPAddress, 0);
					pingRequest.StartTimeStamp = Win32StopWatch.GetSystemPerformanceCounter();
					if (this.m_pingPacket.Length != this.m_sourcePort.Socket.SendTo(this.m_pingPacket, ipendPoint))
					{
						throw new SocketException();
					}
					this.m_sendCount++;
					EndPoint endPoint = ipendPoint;
					this.m_sourcePort.Socket.BeginReceiveFrom(pingRequest.ReplyBuffer, 0, pingRequest.ReplyBuffer.Length, SocketFlags.None, ref endPoint, new AsyncCallback(this.CompletionCallback), null);
				}
			}
			finally
			{
				this.m_proberState = ProberState.Sent;
			}
		}

		public void GatherReplies(int maxWaitInMs)
		{
			if (this.m_proberState != ProberState.Sent)
			{
				throw new ArgumentException("user must send befor gathering");
			}
			try
			{
				this.m_proberState = ProberState.Gathering;
				this.m_probeCompleteEvent.WaitOne(maxWaitInMs, false);
			}
			finally
			{
				this.m_proberState = ProberState.Idle;
				PingRequest[] activeRequests = this.m_activeRequests;
				this.m_activeRequests = null;
				Win32StopWatch.GetSystemPerformanceCounter();
				foreach (PingRequest pingRequest in activeRequests)
				{
					if (pingRequest.StopTimeStamp == 0L || pingRequest.StopTimeStamp < pingRequest.StartTimeStamp)
					{
						pingRequest.TimedOut = true;
					}
					else
					{
						pingRequest.LatencyInUSec = Win32StopWatch.ComputeElapsedTimeInUSec(pingRequest.StopTimeStamp, pingRequest.StartTimeStamp);
					}
				}
			}
		}

		public const int DefaultPingTimeout = 3000;

		private bool m_disposed;

		private ProberState m_proberState;

		private PingSource m_sourcePort;

		private ManualResetEvent m_probeCompleteEvent;

		private byte[] m_pingPacket = PingPacket.FormPacket();

		private PingRequest[] m_activeRequests;

		private int m_sendCount;

		private int m_completionCount;
	}
}
