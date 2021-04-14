using System;
using System.IO;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.Serialization;
using Microsoft.Exchange.Core;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;

namespace Microsoft.Exchange.EseRepl
{
	internal class NetworkChannel
	{
		public static Trace Tracer
		{
			get
			{
				return ExTraceGlobals.NetworkChannelTracer;
			}
		}

		public static NetworkChannel.DataEncodingScheme VerifyDataEncoding(NetworkChannel.DataEncodingScheme requestedEncoding)
		{
			if (requestedEncoding >= NetworkChannel.DataEncodingScheme.LastIndex)
			{
				requestedEncoding = NetworkChannel.DataEncodingScheme.Uncompressed;
			}
			return requestedEncoding;
		}

		public static Exception RunNetworkFunction(Action op)
		{
			Exception result = null;
			try
			{
				op();
			}
			catch (IOException ex)
			{
				result = ex;
			}
			catch (SocketException ex2)
			{
				result = ex2;
			}
			catch (NetworkTransportException ex3)
			{
				result = ex3;
			}
			catch (ObjectDisposedException ex4)
			{
				result = ex4;
			}
			catch (InvalidOperationException ex5)
			{
				result = ex5;
			}
			return result;
		}

		internal NetworkPackagingLayer PackagingLayer
		{
			get
			{
				return this.m_transport;
			}
		}

		internal TcpChannel TcpChannel
		{
			get
			{
				return this.m_channel;
			}
		}

		internal bool IsClosed
		{
			get
			{
				return this.m_isClosed;
			}
		}

		internal bool IsAborted
		{
			get
			{
				return this.m_isAborted;
			}
		}

		internal bool IsCompressionEnabled
		{
			get
			{
				return this.m_transport.Encoding != NetworkChannel.DataEncodingScheme.Uncompressed;
			}
		}

		internal bool IsEncryptionEnabled
		{
			get
			{
				return this.TcpChannel.IsEncrypted;
			}
		}

		internal string PartnerNodeName
		{
			get
			{
				return this.m_channel.PartnerNodeName;
			}
		}

		public string LocalNodeName { get; set; }

		internal NetworkPath NetworkPath
		{
			get
			{
				return this.m_networkPath;
			}
		}

		internal bool KeepAlive
		{
			get
			{
				return this.m_keepAlive;
			}
			set
			{
				this.m_keepAlive = value;
			}
		}

		internal string RemoteEndPointString
		{
			get
			{
				return this.m_remoteEndPointString;
			}
		}

		internal string LocalEndPointString
		{
			get
			{
				return this.m_localEndPointString;
			}
		}

		internal NetworkChannel(TcpClientChannel ch, NetworkPath path)
		{
			this.Init(ch, path);
		}

		protected void Init(TcpChannel ch, NetworkPath path)
		{
			this.NetworkChannelManagesAsyncReads = true;
			this.m_channel = ch;
			this.m_networkPath = path;
			this.m_transport = new NetworkPackagingLayer(this, ch);
			this.m_remoteEndPointString = this.m_channel.RemoteEndpoint.ToString();
			this.m_localEndPointString = this.m_channel.LocalEndpoint.ToString();
		}

		private static NetworkChannel FinishConnect(TcpClientChannel tcpChannel, NetworkPath netPath, bool suppressTransparentCompression)
		{
			bool flag = false;
			NetworkChannel networkChannel = null;
			try
			{
				networkChannel = new NetworkChannel(tcpChannel, netPath);
				if (netPath.Purpose != NetworkPath.ConnectionPurpose.TestHealth && netPath.Compress && !suppressTransparentCompression)
				{
					networkChannel.NegotiateCompression();
				}
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					if (networkChannel != null)
					{
						networkChannel.Close();
						networkChannel = null;
					}
					else if (tcpChannel != null)
					{
						tcpChannel.Close();
					}
				}
			}
			return networkChannel;
		}

		public static NetworkChannel OpenChannel(string targetServerName, ISimpleBufferPool socketStreamBufferPool, IPool<SocketStreamAsyncArgs> socketStreamAsyncArgPool, SocketStream.ISocketStreamPerfCounters perfCtrs, bool suppressTransparentCompression)
		{
			if (socketStreamAsyncArgPool != null ^ socketStreamBufferPool != null)
			{
				string message = "SocketStream use requires both pools or neither";
				throw new ArgumentException(message);
			}
			ITcpConnector tcpConnector = Dependencies.TcpConnector;
			NetworkPath netPath = null;
			TcpClientChannel tcpChannel = tcpConnector.OpenChannel(targetServerName, socketStreamBufferPool, socketStreamAsyncArgPool, perfCtrs, out netPath);
			return NetworkChannel.FinishConnect(tcpChannel, netPath, suppressTransparentCompression);
		}

		internal void NegotiateCompression()
		{
		}

		internal void SetEncoding(NetworkChannel.DataEncodingScheme scheme)
		{
			this.m_transport.Encoding = scheme;
			ExTraceGlobals.NetworkChannelTracer.TraceDebug<NetworkChannel.DataEncodingScheme>((long)this.GetHashCode(), "Compression selected: {0}", scheme);
		}

		internal void SetEncoding(CompressionConfig cfg)
		{
			this.m_transport.SetEncoding(cfg);
		}

		public bool ChecksumDataTransfer
		{
			get
			{
				return false;
			}
		}

		public bool NetworkChannelManagesAsyncReads { get; set; }

		internal static void StaticTraceDebug(string format, params object[] args)
		{
			ExTraceGlobals.NetworkChannelTracer.TraceDebug(0L, format, args);
		}

		internal static void StaticTraceError(string format, params object[] args)
		{
			ExTraceGlobals.NetworkChannelTracer.TraceError(0L, format, args);
		}

		internal void TraceDebug(string format, params object[] args)
		{
			ExTraceGlobals.NetworkChannelTracer.TraceDebug((long)this.GetHashCode(), format, args);
		}

		internal void TraceError(string format, params object[] args)
		{
			ExTraceGlobals.NetworkChannelTracer.TraceError((long)this.GetHashCode(), format, args);
		}

		internal void InvokeWithCatch(NetworkChannel.CatchableOperation op)
		{
			Exception ex = null;
			bool flag = true;
			try
			{
				op();
				flag = false;
			}
			catch (FileIOonSourceException ex2)
			{
				flag = false;
				ex = ex2;
			}
			catch (IOException ex3)
			{
				if (ex3.InnerException is ObjectDisposedException)
				{
					ex = new NetworkCancelledException(ex3);
				}
				else
				{
					ex = new NetworkCommunicationException(this.PartnerNodeName, ex3.Message, ex3);
				}
			}
			catch (SocketException ex4)
			{
				ex = new NetworkCommunicationException(this.PartnerNodeName, ex4.Message, ex4);
			}
			catch (NetworkCommunicationException ex5)
			{
				ex = ex5;
			}
			catch (NetworkTimeoutException ex6)
			{
				ex = ex6;
			}
			catch (NetworkRemoteException ex7)
			{
				flag = false;
				ex = ex7;
			}
			catch (NetworkEndOfDataException ex8)
			{
				ex = ex8;
			}
			catch (NetworkCorruptDataGenericException)
			{
				ex = new NetworkCorruptDataException(this.PartnerNodeName);
			}
			catch (NetworkTransportException ex9)
			{
				ex = ex9;
			}
			catch (CompressionException innerException)
			{
				ex = new NetworkCorruptDataException(this.PartnerNodeName, innerException);
			}
			catch (DecompressionException innerException2)
			{
				ex = new NetworkCorruptDataException(this.PartnerNodeName, innerException2);
			}
			catch (ObjectDisposedException innerException3)
			{
				ex = new NetworkCancelledException(innerException3);
			}
			catch (SerializationException ex10)
			{
				ex = new NetworkCommunicationException(this.PartnerNodeName, ex10.Message, ex10);
			}
			catch (TargetInvocationException ex11)
			{
				if (ex11.InnerException == null || !(ex11.InnerException is SerializationException))
				{
					throw;
				}
				ex = new NetworkCommunicationException(this.PartnerNodeName, ex11.Message, ex11);
			}
			catch (InvalidOperationException ex12)
			{
				ex = new NetworkCommunicationException(this.PartnerNodeName, ex12.Message, ex12);
			}
			finally
			{
				if (flag)
				{
					this.Abort();
				}
			}
			if (ex != null)
			{
				this.TraceError("InvokeWithCatch: Forwarding exception: {0}", new object[]
				{
					ex
				});
				throw ex;
			}
		}

		private void HandleFileIOException(string fullSourceFilename, bool throwCorruptLogDetectedException, Action fileAction)
		{
			try
			{
				fileAction();
			}
			catch (IOException ex)
			{
				ExTraceGlobals.NetworkChannelTracer.TraceError<IOException>((long)this.GetHashCode(), "HandleFileIOException(): Received IOException. Will rethrow it. Ex: {0}", ex);
				if (throwCorruptLogDetectedException)
				{
					CorruptLogDetectedException ex2 = new CorruptLogDetectedException(fullSourceFilename, ex.Message, ex);
					throw new FileIOonSourceException(Environment.MachineName, fullSourceFilename, ex2.Message, ex2);
				}
				throw new FileIOonSourceException(Environment.MachineName, fullSourceFilename, ex.Message, ex);
			}
			catch (UnauthorizedAccessException ex3)
			{
				ExTraceGlobals.NetworkChannelTracer.TraceError<UnauthorizedAccessException>((long)this.GetHashCode(), "HandleFileIOException(): Received UnauthorizedAccessException. Will rethrow it. Ex: {0}", ex3);
				throw new FileIOonSourceException(Environment.MachineName, fullSourceFilename, ex3.Message, ex3);
			}
		}

		internal NetworkChannelMessage GetMessage()
		{
			this.Read(this.m_tempHeaderBuf, 0, 16);
			return NetworkChannelMessage.ReadMessage(this, this.m_tempHeaderBuf);
		}

		internal NetworkChannelMessage TryGetMessage()
		{
			if (!this.m_transport.HasAsyncDataToRead())
			{
				return null;
			}
			return this.GetMessage();
		}

		internal void Read(byte[] buf, int off, int len)
		{
			this.InvokeWithCatch(delegate
			{
				this.m_transport.Read(buf, off, len);
			});
		}

		internal void StartRead(NetworkChannelCallback callback, object context)
		{
			this.InvokeWithCatch(delegate
			{
				this.m_transport.StartRead(callback, context);
			});
		}

		internal virtual void Close()
		{
			NetworkChannel.Tracer.TraceFunction((long)this.GetHashCode(), "Closing");
			lock (this)
			{
				if (!this.m_isClosed)
				{
					this.KeepAlive = false;
					if (this.m_transport != null)
					{
						this.m_transport.Close();
					}
					if (this.m_channel != null)
					{
						this.m_channel.Close();
					}
					this.m_isClosed = true;
				}
			}
			NetworkChannel.Tracer.TraceFunction((long)this.GetHashCode(), "Closed");
		}

		internal void Abort()
		{
			this.m_isAborted = true;
			this.KeepAlive = false;
			if (this.m_channel != null)
			{
				this.m_channel.Abort();
			}
		}

		internal void SendException(Exception ex)
		{
			NetworkChannel.Tracer.TraceError<Type>((long)this.GetHashCode(), "SendException: {0}", ex.GetType());
			this.InvokeWithCatch(delegate
			{
				this.m_transport.WriteException(ex);
			});
		}

		internal void SendMessage(byte[] buf, int off, int len)
		{
			this.InvokeWithCatch(delegate
			{
				this.m_transport.WriteMessage(buf, off, len);
			});
		}

		internal void Write(byte[] buf, int off, int len)
		{
			this.InvokeWithCatch(delegate
			{
				this.m_transport.Write(buf, off, len);
			});
		}

		internal void ThrowUnexpectedMessage(NetworkChannelMessage msg)
		{
			NetworkUnexpectedMessageException ex = new NetworkUnexpectedMessageException(this.PartnerNodeName, msg.ToString());
			throw ex;
		}

		internal static void ThrowTimeoutException(string nodeName, string reason)
		{
			throw new NetworkTimeoutException(nodeName, reason);
		}

		internal void ThrowTimeoutException(string reason)
		{
			NetworkChannel.ThrowTimeoutException(this.PartnerNodeName, reason);
		}

		private TcpChannel m_channel;

		protected NetworkPackagingLayer m_transport;

		private bool m_isClosed;

		private bool m_isAborted;

		protected NetworkPath m_networkPath;

		private bool m_keepAlive;

		private string m_remoteEndPointString;

		private string m_localEndPointString;

		private static readonly ServerVersion FirstVersionSupportingCoconet = new ServerVersion(15, 0, 800, 3);

		private byte[] m_tempHeaderBuf = new byte[16];

		public enum DataEncodingScheme
		{
			Uncompressed,
			CompressedXpress,
			Coconet,
			LastIndex
		}

		internal delegate void CatchableOperation();
	}
}
