using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal abstract class TcpChannel
	{
		public static Trace Tracer
		{
			get
			{
				return ExTraceGlobals.TcpChannelTracer;
			}
		}

		internal static int GetDefaultTimeoutInMs()
		{
			return RegistryParameters.LogShipTimeoutInMsec;
		}

		internal static void ThrowTimeoutException(string nodeName, string reason)
		{
			throw new NetworkTimeoutException(nodeName, reason);
		}

		internal void ThrowTimeoutException(string reason)
		{
			TcpChannel.ThrowTimeoutException(this.PartnerNodeName, reason);
		}

		internal Socket Socket
		{
			get
			{
				return this.m_connection;
			}
		}

		internal IPEndPoint RemoteEndpoint
		{
			get
			{
				return (IPEndPoint)this.m_connection.RemoteEndPoint;
			}
		}

		internal IPEndPoint LocalEndpoint
		{
			get
			{
				return (IPEndPoint)this.m_connection.LocalEndPoint;
			}
		}

		internal string RemoteEndpointString { get; private set; }

		internal string LocalEndpointString { get; private set; }

		internal DateTime IdleSince { get; private set; }

		internal bool IsIdle { get; private set; }

		internal TimeSpan IdleTimeout { get; set; }

		internal void SetIdle()
		{
			this.IdleSince = DateTime.UtcNow;
			this.IsIdle = true;
		}

		internal void ClearIdle()
		{
			this.IsIdle = false;
		}

		internal bool CancelIfIdleTooLong()
		{
			if (this.IsIdle && this.m_open && !this.m_aborted)
			{
				TimeSpan t = DateTime.UtcNow - this.IdleSince;
				if (t > this.IdleTimeout)
				{
					ReplayCrimsonEvents.ServerNetworkConnectionTimeout.Log<string, string, string, string, string>(this.PartnerNodeName, this.RemoteEndpointString, this.LocalEndpointString, this.IdleSince.ToString("u"), t.ToString());
					this.Abort();
					return true;
				}
			}
			return false;
		}

		internal int ReadTimeoutInMs
		{
			get
			{
				return this.m_readTimeoutInMs;
			}
			set
			{
				TcpChannel.Tracer.TraceDebug<int>((long)this.GetHashCode(), "ReadTimeoutInMs={0}", value);
				this.m_readTimeoutInMs = value;
				try
				{
					this.m_authStream.ReadTimeout = this.m_readTimeoutInMs;
				}
				catch (ObjectDisposedException innerException)
				{
					throw new NetworkCancelledException(innerException);
				}
			}
		}

		internal int WriteTimeoutInMs
		{
			get
			{
				return this.m_writeTimeoutInMs;
			}
			set
			{
				TcpChannel.Tracer.TraceDebug<int>((long)this.GetHashCode(), "WriteTimeoutInMs={0}", value);
				this.m_writeTimeoutInMs = value;
				try
				{
					this.m_authStream.WriteTimeout = this.m_writeTimeoutInMs;
				}
				catch (ObjectDisposedException innerException)
				{
					throw new NetworkCancelledException(innerException);
				}
			}
		}

		internal int BufferSize
		{
			get
			{
				return this.m_bufferSize;
			}
			set
			{
				TcpChannel.Tracer.TraceDebug<int>((long)this.GetHashCode(), "BufferSize={0}", value);
				this.m_bufferSize = value;
				this.m_connection.SendBufferSize = this.m_bufferSize;
				this.m_connection.ReceiveBufferSize = this.m_bufferSize;
			}
		}

		public string PartnerNodeName
		{
			get
			{
				return this.m_partnerNodeName;
			}
			set
			{
				this.m_partnerNodeName = value;
			}
		}

		protected TcpChannel(Socket socket, NegotiateStream stream, int timeout, TimeSpan idleLimit)
		{
			this.m_connection = socket;
			this.m_authStream = stream;
			this.ReadTimeoutInMs = timeout;
			this.WriteTimeoutInMs = timeout;
			this.m_open = true;
			this.RemoteEndpointString = this.RemoteEndpoint.ToString();
			this.LocalEndpointString = this.LocalEndpoint.ToString();
			this.IdleTimeout = idleLimit;
		}

		protected NegotiateStream AuthStream
		{
			get
			{
				return this.m_authStream;
			}
		}

		internal Stream Stream
		{
			get
			{
				return this.m_authStream;
			}
		}

		internal bool IsEncrypted
		{
			get
			{
				return this.m_authStream.IsEncrypted;
			}
		}

		public void Close()
		{
			lock (this)
			{
				if (this.m_open)
				{
					this.m_open = false;
					ExTraceGlobals.TcpChannelTracer.TraceDebug((long)this.GetHashCode(), "Closing channel");
					if (this is TcpClientChannel)
					{
						ReplayCrimsonEvents.ClientNetworkConnectionClosed.Log<string, string, string>(this.PartnerNodeName, this.RemoteEndpointString, this.LocalEndpointString);
					}
					else
					{
						ReplayCrimsonEvents.ServerNetworkConnectionClosed.Log<string, string, string>(this.PartnerNodeName, this.RemoteEndpointString, this.LocalEndpointString);
					}
					this.Shutdown(true);
					NegotiateStream authStream = this.m_authStream;
					if (authStream != null)
					{
						authStream.Dispose();
					}
				}
			}
		}

		public void Abort()
		{
			lock (this)
			{
				if (this.m_open)
				{
					if (!this.m_aborted)
					{
						this.m_aborted = true;
						ExTraceGlobals.TcpChannelTracer.TraceDebug((long)this.GetHashCode(), "Aborting channel");
						this.Shutdown(true);
					}
				}
			}
		}

		private void Shutdown(bool performClose)
		{
			Exception ex = null;
			try
			{
				this.m_connection.Shutdown(SocketShutdown.Both);
			}
			catch (SocketException ex2)
			{
				ex = ex2;
			}
			catch (ObjectDisposedException ex3)
			{
				performClose = false;
				ex = ex3;
			}
			if (ex != null)
			{
				ExTraceGlobals.TcpChannelTracer.TraceError<Exception>((long)this.GetHashCode(), "Shutdown got exception: {0}", ex);
			}
			if (performClose)
			{
				ex = null;
				try
				{
					this.m_connection.Close();
				}
				catch (SocketException ex4)
				{
					ex = ex4;
				}
				catch (ObjectDisposedException ex5)
				{
					ex = ex5;
				}
				if (ex != null)
				{
					ExTraceGlobals.TcpChannelTracer.TraceError<Exception>((long)this.GetHashCode(), "Close got exception: {0}", ex);
				}
			}
		}

		public int Read(byte[] buffer, int offset, int maxSize)
		{
			return this.m_authStream.Read(buffer, offset, maxSize);
		}

		public int TryReadChunk(byte[] buf, int bufOffset, int totalSize)
		{
			int i = totalSize;
			while (i > 0)
			{
				int num = this.Read(buf, bufOffset, i);
				i -= num;
				bufOffset += num;
				if (i > 0 && num == 0)
				{
					ExTraceGlobals.TcpChannelTracer.TraceError(0L, "Connection unexpectedly closed");
					break;
				}
			}
			return totalSize - i;
		}

		public void ReadChunk(byte[] buf, int bufOffset, int totalSize)
		{
			if (totalSize != this.TryReadChunk(buf, bufOffset, totalSize))
			{
				throw new NetworkEndOfDataException(this.PartnerNodeName, ReplayStrings.NetworkReadEOF);
			}
		}

		public void Write(byte[] buffer, int offset, int size)
		{
			this.m_authStream.Write(buffer, offset, size);
		}

		public void Write(byte[] buffer, int offset, int size, bool flush)
		{
			this.m_authStream.Write(buffer, offset, size);
			if (flush)
			{
				this.m_authStream.Flush();
			}
		}

		public void WriteAndFlush(byte[] buffer, int offset, int size)
		{
			this.Write(buffer, offset, size);
			this.m_authStream.Flush();
		}

		public void Flush()
		{
			this.m_authStream.Flush();
		}

		public static void SetTcpKeepAlive(Socket socket, uint keepaliveTimeInMsec, uint keepaliveIntervalInMsec)
		{
			int num = 4;
			byte[] array = new byte[num * 3];
			BitConverter.GetBytes(keepaliveTimeInMsec).CopyTo(array, 0);
			BitConverter.GetBytes(keepaliveTimeInMsec).CopyTo(array, num);
			BitConverter.GetBytes(keepaliveIntervalInMsec).CopyTo(array, 2 * num);
			socket.IOControl((IOControlCode)((ulong)-1744830460), array, null);
		}

		protected bool m_open;

		protected bool m_aborted;

		protected Socket m_connection;

		protected NegotiateStream m_authStream;

		protected int m_readTimeoutInMs = 15000;

		protected int m_writeTimeoutInMs = 15000;

		protected int m_bufferSize;

		protected string m_partnerNodeName;
	}
}
