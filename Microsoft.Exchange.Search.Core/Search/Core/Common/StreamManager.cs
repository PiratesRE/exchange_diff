using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using Microsoft.Exchange.Collections.TimeoutCache;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Search;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.ProcessManager;
using Microsoft.Exchange.Search.Core.Abstraction;
using Microsoft.Exchange.Search.Core.Diagnostics;

namespace Microsoft.Exchange.Search.Core.Common
{
	internal class StreamManager : IStreamManager, IDisposable
	{
		internal StreamManager(string eventSource, TcpListener.HandleFailure failureHandler)
		{
			this.diagnosticsSession = DiagnosticsSession.CreateComponentDiagnosticsSession("StreamManager", ComponentInstance.Globals.Search.ServiceName, ExTraceGlobals.StreamManagerTracer, (long)this.GetHashCode());
			this.failureHandler = failureHandler;
			if (eventSource != null)
			{
				this.eventLogger = new ExEventLog(Guid.Empty, eventSource);
			}
		}

		public static TimeSpan DefaultTimeout
		{
			[DebuggerStepThrough]
			get
			{
				return TimeSpan.FromSeconds(30.0);
			}
		}

		public int ListenPort
		{
			[DebuggerStepThrough]
			get
			{
				return this.listenPort;
			}
			[DebuggerStepThrough]
			set
			{
				this.listenPort = value;
			}
		}

		public TimeSpan CacheTimeout
		{
			[DebuggerStepThrough]
			get
			{
				return this.cacheTimeout;
			}
			[DebuggerStepThrough]
			set
			{
				this.cacheTimeout = value;
			}
		}

		public TimeSpan ConnectionTimeout
		{
			[DebuggerStepThrough]
			get
			{
				return this.connectionTimeout;
			}
			[DebuggerStepThrough]
			set
			{
				this.connectionTimeout = value;
			}
		}

		public bool Listening
		{
			[DebuggerStepThrough]
			get
			{
				return this.tcpListener != null;
			}
		}

		internal int CacheCount
		{
			[DebuggerStepThrough]
			get
			{
				return this.timeoutCache.Count;
			}
		}

		private bool CanListen
		{
			[DebuggerStepThrough]
			get
			{
				return this.eventLogger != null;
			}
		}

		public static StreamManager Create()
		{
			return new StreamManager(null, null);
		}

		public static StreamManager CreateForListen(string eventSource, TcpListener.HandleFailure failureHandler)
		{
			Util.ThrowOnNullArgument(eventSource, "eventSource");
			return new StreamManager(eventSource, failureHandler);
		}

		public void Dispose()
		{
			this.Dispose(true);
		}

		public void StartListening()
		{
			this.CheckDisposed();
			if (!this.CanListen)
			{
				throw new InvalidOperationException("Object is not configured to listen");
			}
			lock (this.eventLogger)
			{
				if (this.tcpListener != null)
				{
					throw new InvalidOperationException("Already listening");
				}
				this.AssignListenPortIfNecessary();
				IPEndPoint ipendPoint = StreamManager.GetIPEndPoint(this.listenPort);
				this.diagnosticsSession.TraceDebug<IPEndPoint>("StartListening on {0}", ipendPoint);
				this.tcpListener = new TcpListener(new TcpListener.HandleFailure(this.HandleTcpListenerFailure), new TcpListener.HandleConnection(this.AcceptCompleteCallback), new IPEndPoint[]
				{
					ipendPoint
				}, this.diagnosticsSession.Tracer, this.eventLogger, int.MaxValue, false, false);
				this.tcpListener.StartListening(true);
			}
		}

		public void StopListening()
		{
			this.diagnosticsSession.TraceDebug("StopListening", new object[0]);
			if (!this.CanListen)
			{
				return;
			}
			lock (this.eventLogger)
			{
				TcpListener tcpListener = this.tcpListener;
				this.tcpListener = null;
				if (tcpListener != null)
				{
					tcpListener.ProcessStopping = true;
					tcpListener.StopListening();
					tcpListener.Shutdown();
				}
			}
		}

		public Stream WaitForConnection(Guid contextId)
		{
			this.diagnosticsSession.TraceDebug("WaitForConnection", new object[0]);
			StreamManager.AsyncResult asyncResult = (StreamManager.AsyncResult)this.BeginWaitForConnection(contextId, null, null);
			Stream result;
			try
			{
				result = this.EndWaitForConnection(asyncResult);
			}
			finally
			{
				asyncResult.InternalCleanup();
			}
			return result;
		}

		public ICancelableAsyncResult BeginWaitForConnection(Guid contextId, AsyncCallback callback, object state)
		{
			this.diagnosticsSession.TraceDebug<Guid>("BeginWaitForConnection: {0}", contextId);
			if (!this.Listening)
			{
				throw new TimeoutException();
			}
			ICancelableAsyncResult result;
			lock (this.asyncResultDictionary)
			{
				this.CheckDisposed();
				if (this.asyncResultDictionary.ContainsKey(contextId))
				{
					throw new ArgumentException("contextId");
				}
				StreamManager.AsyncResult asyncResult = new StreamManager.AsyncResult(contextId, callback, state);
				asyncResult.StartTimer(this.connectionTimeout);
				this.asyncResultDictionary.Add(contextId, asyncResult);
				result = asyncResult;
			}
			return result;
		}

		public Stream EndWaitForConnection(IAsyncResult asyncResult)
		{
			this.diagnosticsSession.TraceDebug("EndWaitForConnection", new object[0]);
			StreamManager.AsyncResult asyncResult2 = StreamManager.AsyncResult.EndAsyncOperation(asyncResult);
			this.diagnosticsSession.TraceDebug<Guid>("EndWaitForConnection complete: {0}", asyncResult2.ContextId);
			lock (this.asyncResultDictionary)
			{
				this.asyncResultDictionary.Remove(asyncResult2.ContextId);
			}
			Exception ex = asyncResult2.Result as Exception;
			if (ex != null)
			{
				this.diagnosticsSession.TraceError<Exception>("EndWaitForConnection exception: {0}", ex);
				asyncResult2.ReleaseResources();
				throw ex;
			}
			return asyncResult2.Channel;
		}

		public Stream Connect(int port, Guid contextId)
		{
			this.diagnosticsSession.TraceDebug("Connect", new object[0]);
			StreamManager.AsyncResult asyncResult = (StreamManager.AsyncResult)this.BeginConnect(port, contextId, null, null);
			Stream result;
			try
			{
				result = this.EndConnect(asyncResult);
			}
			finally
			{
				asyncResult.InternalCleanup();
			}
			return result;
		}

		public ICancelableAsyncResult BeginConnect(int port, Guid contextId, AsyncCallback callback, object state)
		{
			this.diagnosticsSession.TraceDebug<int, Guid>("BeginConnect, port: {0}, context: {1}", port, contextId);
			IPEndPoint ipendPoint = StreamManager.GetIPEndPoint(port);
			StreamManager.AsyncResult asyncResult;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				Socket socket = new Socket(ipendPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
				disposeGuard.Add<Socket>(socket);
				lock (this.asyncResultDictionary)
				{
					this.CheckDisposed();
					if (this.asyncResultDictionary.ContainsKey(contextId))
					{
						throw new ArgumentException("contextId");
					}
					asyncResult = new StreamManager.AsyncResult(contextId, callback, state);
					asyncResult.Socket = socket;
					asyncResult.StartTimer(this.connectionTimeout);
					this.asyncResultDictionary.Add(contextId, asyncResult);
				}
				disposeGuard.Success();
			}
			try
			{
				asyncResult.Socket.BeginConnect(ipendPoint, new AsyncCallback(this.ConnectComplete), asyncResult);
			}
			catch (Exception ex)
			{
				this.diagnosticsSession.TraceError<Exception>("Socket.BeginConnect exception: {0}", ex);
				asyncResult.InvokeCallback(ex);
			}
			return asyncResult;
		}

		public Stream EndConnect(IAsyncResult asyncResult)
		{
			StreamManager.AsyncResult asyncResult2 = StreamManager.AsyncResult.EndAsyncOperation(asyncResult);
			lock (this.asyncResultDictionary)
			{
				this.asyncResultDictionary.Remove(asyncResult2.ContextId);
			}
			Exception ex = asyncResult2.Result as Exception;
			if (ex != null)
			{
				asyncResult2.ReleaseResources();
				throw ex;
			}
			return asyncResult2.Channel;
		}

		public void CancelPendingOperation(Guid contextId)
		{
			StreamManager.AsyncResult asyncResult;
			lock (this.asyncResultDictionary)
			{
				asyncResult = this.asyncResultDictionary[contextId];
			}
			this.diagnosticsSession.TraceDebug<Guid, string>("CancelPendingOperation, context: {0} - {1}", contextId, (asyncResult != null) ? "Attempting to cancel" : "Nothing to cancel");
			if (asyncResult != null)
			{
				asyncResult.InvokeCallback(new OperationCanceledException());
			}
		}

		public void CheckIn(Stream channel)
		{
			StreamChannel streamChannel = (StreamChannel)channel;
			this.timeoutCache.AddSliding(streamChannel.ContextId, streamChannel, this.cacheTimeout, new RemoveItemDelegate<Guid, StreamChannel>(this.RemoveFromCacheCallback));
		}

		public Stream CheckOut(Guid contextId)
		{
			return this.timeoutCache.Remove(contextId);
		}

		internal static IPEndPoint GetIPEndPoint(int port)
		{
			if (Socket.OSSupportsIPv4)
			{
				return new IPEndPoint(IPAddress.Loopback, port);
			}
			return new IPEndPoint(IPAddress.IPv6Loopback, port);
		}

		protected virtual void Dispose(bool calledFromDispose)
		{
			if (calledFromDispose)
			{
				this.StopListening();
				List<StreamManager.AsyncResult> list;
				lock (this.asyncResultDictionary)
				{
					this.disposed = true;
					list = new List<StreamManager.AsyncResult>(this.asyncResultDictionary.Values);
				}
				foreach (StreamManager.AsyncResult asyncResult in list)
				{
					asyncResult.TimeoutOperation(null);
				}
				this.timeoutCache.Dispose();
			}
			GC.SuppressFinalize(this);
		}

		private void AssignListenPortIfNecessary()
		{
			if (this.listenPort != 0)
			{
				return;
			}
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				IPEndPoint ipendPoint = StreamManager.GetIPEndPoint(0);
				Socket socket = new Socket(ipendPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
				disposeGuard.Add<Socket>(socket);
				socket.Bind(ipendPoint);
				ipendPoint = (IPEndPoint)socket.LocalEndPoint;
				this.listenPort = ipendPoint.Port;
			}
		}

		private void RemoveFromCacheCallback(Guid contextId, StreamChannel channel, RemoveReason reason)
		{
			if (reason != RemoveReason.Removed)
			{
				try
				{
					channel.Dispose();
				}
				catch (Exception arg)
				{
					this.diagnosticsSession.TraceError<Guid, Exception>("Exception disposing context {0}: {1}", contextId, arg);
				}
			}
		}

		private void HandleTcpListenerFailure(bool addressAlreadyInUseFailure)
		{
			this.diagnosticsSession.TraceError("TcpListener told us to stop listening", new object[0]);
			this.StopListening();
			if (this.failureHandler != null)
			{
				this.failureHandler(false);
			}
		}

		private bool AcceptCompleteCallback(Socket connection)
		{
			bool result;
			try
			{
				using (DisposeGuard disposeGuard = default(DisposeGuard))
				{
					StreamChannel streamChannel = new StreamChannel(connection);
					disposeGuard.Add<StreamChannel>(streamChannel);
					this.diagnosticsSession.TraceDebug<StreamChannel>("Accept complete, {0}", streamChannel);
					streamChannel.ReadTimeout = (int)this.connectionTimeout.TotalMilliseconds;
					streamChannel.WriteTimeout = (int)this.connectionTimeout.TotalMilliseconds;
					this.CheckDisposed();
					StreamManager.ChannelAndBuffer channelAndBuffer = new StreamManager.ChannelAndBuffer(streamChannel, new byte[16]);
					streamChannel.BeginRead(channelAndBuffer.Buffer, 0, channelAndBuffer.Buffer.Length, new AsyncCallback(this.ContextIdReceivedCallback), channelAndBuffer);
					disposeGuard.Success();
					result = true;
				}
			}
			catch (Exception arg)
			{
				this.diagnosticsSession.TraceError<Exception>("AcceptCompleteCallback exception: {0}", arg);
				result = false;
			}
			return result;
		}

		private void ContextIdReceivedCallback(IAsyncResult asyncResult)
		{
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				StreamManager.AsyncResult asyncResult2 = null;
				StreamManager.ChannelAndBuffer channelAndBuffer = (StreamManager.ChannelAndBuffer)asyncResult.AsyncState;
				StreamChannel channel = channelAndBuffer.Channel;
				disposeGuard.Add<StreamChannel>(channel);
				int num;
				try
				{
					num = channel.EndRead(asyncResult);
				}
				catch (IOException arg)
				{
					this.diagnosticsSession.TraceError<StreamChannel, IOException>("ContextIdReceivedCallback failed for {0}, Exception: {1}", channel, arg);
					return;
				}
				catch (InvalidDataException arg2)
				{
					this.diagnosticsSession.TraceError<StreamChannel, InvalidDataException>("ContextIdReceivedCallback failed for {0}, Exception: {1}", channel, arg2);
					return;
				}
				if (num != 16)
				{
					this.diagnosticsSession.TraceError<StreamChannel, int, int>("ContextId length mismatch for channel {0}, expected {1}, received {2}", channel, 16, num);
				}
				else
				{
					channel.ContextId = new Guid(channelAndBuffer.Buffer);
					this.diagnosticsSession.TraceDebug<Guid>("ContextId: {0}", channel.ContextId);
					lock (this.asyncResultDictionary)
					{
						this.asyncResultDictionary.TryGetValue(channel.ContextId, out asyncResult2);
					}
					if (asyncResult2 != null)
					{
						disposeGuard.Success();
						asyncResult2.Channel = channel;
						asyncResult2.InvokeCallback();
					}
				}
			}
		}

		private void ConnectComplete(IAsyncResult asyncResult)
		{
			StreamManager.AsyncResult asyncResult2 = (StreamManager.AsyncResult)asyncResult.AsyncState;
			try
			{
				asyncResult2.Socket.EndConnect(asyncResult);
				this.CheckDisposed();
				asyncResult2.ConvertSocketToStreamChannel();
				StreamChannel channel = asyncResult2.Channel;
				channel.ReadTimeout = (int)this.connectionTimeout.TotalMilliseconds;
				channel.WriteTimeout = (int)this.connectionTimeout.TotalMilliseconds;
				byte[] array = asyncResult2.ContextId.ToByteArray();
				channel.BeginWrite(array, 0, array.Length, new AsyncCallback(this.SendContextIdComplete), asyncResult2);
			}
			catch (Exception ex)
			{
				this.diagnosticsSession.TraceError<Exception>("ConnectComplete failed, exception: {0}", ex);
				asyncResult2.InvokeCallback(ex);
			}
		}

		private void SendContextIdComplete(IAsyncResult asyncResult)
		{
			StreamManager.AsyncResult asyncResult2 = (StreamManager.AsyncResult)asyncResult.AsyncState;
			try
			{
				asyncResult2.Channel.EndWrite(asyncResult);
				asyncResult2.InvokeCallback();
			}
			catch (Exception ex)
			{
				this.diagnosticsSession.TraceError<Exception>("SendContextIdComplete failed, exception: {0}", ex);
				asyncResult2.InvokeCallback(ex);
			}
		}

		private void CheckDisposed()
		{
			if (this.disposed)
			{
				throw new ObjectDisposedException("StreamManager");
			}
		}

		private const int MaxListenBacklog = 2147483647;

		private const int ContextIdByteLength = 16;

		private readonly IDiagnosticsSession diagnosticsSession;

		private readonly ExEventLog eventLogger;

		private TimeSpan cacheTimeout = StreamManager.DefaultTimeout;

		private TimeSpan connectionTimeout = StreamManager.DefaultTimeout;

		private TcpListener tcpListener;

		private int listenPort;

		private Dictionary<Guid, StreamManager.AsyncResult> asyncResultDictionary = new Dictionary<Guid, StreamManager.AsyncResult>();

		private TimeoutCache<Guid, StreamChannel> timeoutCache = new TimeoutCache<Guid, StreamChannel>(1, 250, true);

		private bool disposed;

		private TcpListener.HandleFailure failureHandler;

		private class ChannelAndBuffer
		{
			internal ChannelAndBuffer(StreamChannel channel, byte[] buffer)
			{
				this.Channel = channel;
				this.Buffer = buffer;
			}

			internal StreamChannel Channel { get; set; }

			internal byte[] Buffer { get; set; }
		}

		private class AsyncResult : LazyAsyncResultWithTimeout
		{
			internal AsyncResult(Guid contextId, AsyncCallback callback, object callerState) : base(null, callerState, callback)
			{
				this.contextId = contextId;
			}

			internal Socket Socket
			{
				[DebuggerStepThrough]
				get
				{
					return this.socket;
				}
				[DebuggerStepThrough]
				set
				{
					this.socket = value;
				}
			}

			internal StreamChannel Channel
			{
				[DebuggerStepThrough]
				get
				{
					return this.channel;
				}
				[DebuggerStepThrough]
				set
				{
					this.channel = value;
				}
			}

			internal Guid ContextId
			{
				[DebuggerStepThrough]
				get
				{
					return this.contextId;
				}
			}

			internal static StreamManager.AsyncResult EndAsyncOperation(IAsyncResult asyncResult)
			{
				StreamManager.AsyncResult asyncResult2 = LazyAsyncResult.EndAsyncOperation<StreamManager.AsyncResult>(asyncResult);
				asyncResult2.DisposeTimer();
				return asyncResult2;
			}

			internal void ConvertSocketToStreamChannel()
			{
				this.channel = new StreamChannel(this.socket);
				this.channel.ContextId = this.contextId;
				this.socket = null;
			}

			internal void ReleaseResources()
			{
				if (this.socket != null)
				{
					this.socket.Dispose();
					this.socket = null;
				}
				if (this.channel != null)
				{
					this.channel.Dispose();
					this.channel = null;
				}
			}

			private readonly Guid contextId;

			private StreamChannel channel;

			private Socket socket;
		}
	}
}
