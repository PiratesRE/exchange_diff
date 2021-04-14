using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Internal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.PopImap.Core
{
	internal abstract class BaseSession : IDisposeTrackable, IDisposable
	{
		public BaseSession(NetworkConnection connection, int connectionTimeout, int maxCommandLength)
		{
			if (connection == null)
			{
				throw new ArgumentNullException("connection");
			}
			this.Started = ExDateTime.UtcNow;
			this.connection = connection;
			this.responseItems = new ResponseQueue(100);
			this.sessionId = connection.ConnectionId;
			connection.Timeout = connectionTimeout;
			this.SetMaxCommandLength(maxCommandLength);
			this.disposeTracker = this.GetDisposeTracker();
		}

		public static byte[] LowerC
		{
			get
			{
				return BufferParser.LowerC;
			}
		}

		public StringResponseItemProcessor StringResponseItemProcessor
		{
			get
			{
				return this.stringResponseItemProcessor;
			}
		}

		public NetworkConnection Connection
		{
			get
			{
				return this.connection;
			}
			protected set
			{
				this.connection = value;
			}
		}

		public IPEndPoint LocalEndPoint
		{
			get
			{
				if (this.connection != null)
				{
					return this.connection.LocalEndPoint;
				}
				return null;
			}
		}

		public IPEndPoint RemoteEndPoint
		{
			get
			{
				if (this.connection != null)
				{
					return this.connection.RemoteEndPoint;
				}
				return null;
			}
		}

		public long SessionId
		{
			get
			{
				return this.sessionId;
			}
		}

		public bool Disconnected
		{
			get
			{
				return this.disconnected;
			}
			set
			{
				this.disconnected = value;
			}
		}

		public bool NegotiatingTls
		{
			get
			{
				return this.negotiatingTls;
			}
			set
			{
				this.negotiatingTls = value;
			}
		}

		public ExDateTime Started { get; set; }

		public ExDateTime LastActivityTime { get; set; }

		public bool Disposed
		{
			get
			{
				return this.disposed;
			}
		}

		protected BaseSession.ConnectionShutdownDelegate ConnectionShutdown
		{
			get
			{
				return this.connectionShutdown;
			}
			set
			{
				this.connectionShutdown = value;
			}
		}

		protected int ProcessingCommandRefCounter
		{
			get
			{
				return this.processingCommandRefCounter;
			}
		}

		protected EndResponseItem BeginReadResponseItem
		{
			get
			{
				if (this.beginReadResponseItem == null)
				{
					this.beginReadResponseItem = new EndResponseItem(new BaseSession.SendCompleteDelegate(this.BeginRead));
				}
				return this.beginReadResponseItem;
			}
		}

		protected byte[] ProxyBuffer
		{
			get
			{
				if (BaseSession.proxyBufferPool == null)
				{
					lock (BaseSession.proxyBufferPoolLock)
					{
						if (BaseSession.proxyBufferPool == null)
						{
							BaseSession.proxyBufferPool = new BufferPool(4096);
						}
					}
				}
				if (this.proxyBuffer == null)
				{
					lock (this.LockObject)
					{
						if (this.proxyBuffer == null)
						{
							this.proxyBuffer = BaseSession.proxyBufferPool.Acquire();
						}
					}
				}
				return this.proxyBuffer;
			}
		}

		protected virtual object LockObject
		{
			get
			{
				return this;
			}
		}

		public void SetMaxCommandLength(int maxCommandLength)
		{
			this.maxCommandLength = maxCommandLength;
			this.connection.MaxLineLength = Math.Min(maxCommandLength, 4096);
		}

		public bool SendToClient(IResponseItem responseitem)
		{
			if (this.disposed)
			{
				using (responseitem as IDisposable)
				{
				}
				return !this.disconnected;
			}
			lock (this.LockObject)
			{
				if (this.disposed)
				{
					using (responseitem as IDisposable)
					{
					}
					return !this.disconnected;
				}
				this.responseItems.Enqueue(responseitem);
				if (this.responseItems.IsSending || this.negotiatingTls)
				{
					return !this.disconnected;
				}
				this.responseItems.DequeueForSend();
			}
			if (this.connection != null)
			{
				this.SendNextChunk(this.connection);
			}
			return !this.disconnected;
		}

		public override string ToString()
		{
			string result;
			lock (this.LockObject)
			{
				if (this.connection != null)
				{
					string text = this.SessionId.ToString();
					string text2 = this.RemoteEndPoint.ToString();
					string text3 = this.LocalEndPoint.ToString();
					StringBuilder stringBuilder = new StringBuilder(text.Length + text2.Length + text3.Length + 21);
					stringBuilder.Append("Connection ");
					stringBuilder.Append(text);
					stringBuilder.Append(" from ");
					stringBuilder.Append(text2);
					stringBuilder.Append(" to ");
					stringBuilder.Append(text3);
					result = stringBuilder.ToString();
				}
				else
				{
					result = "Disconnected connection " + this.SessionId;
				}
			}
			return result;
		}

		public virtual void EnterReadLoop(NetworkConnection networkConnection)
		{
			networkConnection.BeginReadLine(new AsyncCallback(this.ReadLineCompleteCallback), networkConnection);
		}

		public void SendBufferAsCommand(byte[] buf, int offset, int size)
		{
			byte[] array;
			if (size > this.ProxyBuffer.Length - Strings.CRLFByteArray.Length)
			{
				array = new byte[size + Strings.CRLFByteArray.Length];
			}
			else
			{
				array = this.ProxyBuffer;
			}
			Array.Copy(buf, offset, array, 0, size);
			Array.Copy(Strings.CRLFByteArray, 0, array, size, Strings.CRLFByteArray.Length);
			this.SendToClient(new BufferResponseItem(array, 0, size + Strings.CRLFByteArray.Length));
		}

		public void Dispose()
		{
			if (!this.disposed)
			{
				this.InternalDispose();
				GC.SuppressFinalize(this);
			}
		}

		public void EnterCommandProcessing()
		{
			Interlocked.Increment(ref this.processingCommandRefCounter);
		}

		public void LeaveCommandProcessing()
		{
			lock (this.LockObject)
			{
				if (Interlocked.Decrement(ref this.processingCommandRefCounter) == 0 && this.disconnected)
				{
					this.Dispose();
				}
			}
		}

		public abstract bool IsUserTraceEnabled();

		public abstract string GetUserNameForLogging();

		public abstract string GetUserConfigurationName();

		public virtual bool CheckNonCriticalException(Exception exception)
		{
			if (!ProtocolBaseServices.IsCriticalException(exception))
			{
				if (!this.disposed && !(exception is LocalizedException))
				{
					ProtocolBaseServices.SendWatson(exception, false);
				}
				else
				{
					ProtocolBaseServices.ServerTracer.TraceError<Exception>(0L, "Non-critical exception is caught and handled: {0}", exception);
				}
				return true;
			}
			ProtocolBaseServices.ServerTracer.TraceError<Exception>(0L, "Critical exception is not handled: {0}", exception);
			return false;
		}

		public void EnableUserTracing()
		{
			if (this.IsUserTraceEnabled())
			{
				BaseTrace.CurrentThreadSettings.EnableTracing();
			}
		}

		public void DisableUserTracing()
		{
			if (BaseTrace.CurrentThreadSettings.IsEnabled)
			{
				BaseTrace.CurrentThreadSettings.DisableTracing();
			}
		}

		public DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<BaseSession>(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		internal static int GetNextToken(byte[] buffer, int beginIndex, int size, out int tokenEnd)
		{
			return BufferParser.GetNextToken(buffer, beginIndex, size, out tokenEnd);
		}

		internal static bool CompareArg(byte[] byteEncodedStrBuf, byte[] buffer, int beginOffset, int count)
		{
			return BufferParser.CompareArg(byteEncodedStrBuf, buffer, beginOffset, count);
		}

		internal abstract void HandleCommandTooLongError(NetworkConnection nc, byte[] buf, int offset, int amount);

		internal abstract void HandleIdleTimeout();

		protected virtual void InternalDispose()
		{
			this.LastActivityTime = ExDateTime.UtcNow;
			lock (this.LockObject)
			{
				ProtocolBaseServices.SessionTracer.TraceDebug(this.SessionId, "InternalDispose has been called!");
				try
				{
					if (this.connectionShutdown != null)
					{
						this.connectionShutdown();
						this.connectionShutdown = null;
					}
				}
				finally
				{
					if (this.disposeTracker != null)
					{
						this.disposeTracker.Dispose();
						this.disposeTracker = null;
					}
					if (this.connection != null)
					{
						this.connection.Dispose();
						this.connection = null;
					}
					if (this.responseItems != null)
					{
						this.responseItems.Dispose();
						this.responseItems = null;
					}
					if (this.stringResponseItemProcessor != null)
					{
						this.stringResponseItemProcessor.Dispose();
						this.stringResponseItemProcessor = null;
					}
					if (this.proxyBuffer != null)
					{
						BaseSession.proxyBufferPool.Release(this.proxyBuffer);
						this.proxyBuffer = null;
					}
					this.disposed = true;
				}
			}
		}

		protected virtual void ReadLineCompleteCallback(IAsyncResult iar)
		{
			if (iar.CompletedSynchronously)
			{
				ThreadPool.QueueUserWorkItem(delegate(object _)
				{
					this.ReadLineCallBackFunction(iar);
				});
				return;
			}
			this.ReadLineCallBackFunction(iar);
		}

		private void ReadLineCallBackFunction(IAsyncResult iar)
		{
			this.LastActivityTime = ExDateTime.UtcNow;
			this.EnableUserTracing();
			try
			{
				ProtocolBaseServices.SessionTracer.TraceDebug<string>(this.SessionId, "User {0} entering BaseSession.ReadLineCompleteCallback.", this.GetUserNameForLogging());
				NetworkConnection networkConnection = (NetworkConnection)iar.AsyncState;
				byte[] array;
				int num;
				int num2;
				object obj;
				networkConnection.EndReadLine(iar, out array, out num, out num2, out obj);
				if (obj != null && (!(obj is SocketError) || (SocketError)obj != SocketError.MessageSize))
				{
					this.HandleError(obj, networkConnection);
				}
				else if (this.disconnected || this.disposed)
				{
					ProtocolBaseServices.SessionTracer.TraceDebug(this.SessionId, "Session unexpectedly disposed");
				}
				else
				{
					this.EnterCommandProcessing();
					try
					{
						if (obj != null || this.largeReceiveBuffer != null)
						{
							if (num2 + this.largeReceiveBufferLength >= this.maxCommandLength)
							{
								if (this.largeReceiveBuffer != null)
								{
									this.HandleCommandTooLongError(networkConnection, this.largeReceiveBuffer, 0, this.largeReceiveBufferLength);
								}
								else
								{
									this.HandleCommandTooLongError(networkConnection, array, num, num2);
								}
								return;
							}
							if (this.largeReceiveBuffer == null)
							{
								this.largeReceiveBuffer = new byte[this.maxCommandLength];
							}
							Buffer.BlockCopy(array, num, this.largeReceiveBuffer, this.largeReceiveBufferLength, num2);
							this.largeReceiveBufferLength += num2;
							if (obj != null)
							{
								this.BeginRead(networkConnection);
								return;
							}
						}
						if (this.largeReceiveBuffer == null)
						{
							this.HandleCommand(networkConnection, array, num, num2);
						}
						else
						{
							this.HandleCommand(networkConnection, this.largeReceiveBuffer, 0, this.largeReceiveBufferLength);
							this.largeReceiveBuffer = null;
							this.largeReceiveBufferLength = 0;
						}
					}
					finally
					{
						this.LeaveCommandProcessing();
					}
				}
			}
			catch (Exception exception)
			{
				if (!this.CheckNonCriticalException(exception))
				{
					throw;
				}
				this.HandleError(null, iar.AsyncState as NetworkConnection);
			}
			finally
			{
				try
				{
					this.ReadLineCompletePostProcessing();
				}
				finally
				{
					this.DisableUserTracing();
					ProtocolBaseServices.InMemoryTraceOperationCompleted(this.SessionId);
				}
			}
		}

		protected virtual void ReadLineCompletePostProcessing()
		{
		}

		protected abstract void HandleCommand(NetworkConnection nc, byte[] buf, int offset, int size);

		protected virtual void BeginRead(NetworkConnection nc)
		{
			this.LastActivityTime = ExDateTime.UtcNow;
			if (nc != null)
			{
				nc.BeginReadLine(new AsyncCallback(this.ReadLineCompleteCallback), nc);
			}
		}

		protected virtual int SendNextChunk(NetworkConnection nc)
		{
			this.LastActivityTime = ExDateTime.UtcNow;
			for (;;)
			{
				byte[] buffer;
				int offset;
				int nextChunk = this.GetNextChunk(out buffer, out offset);
				if (nextChunk == 0 || this.disposed)
				{
					break;
				}
				IAsyncResult asyncResult = nc.BeginWrite(buffer, offset, nextChunk, new AsyncCallback(this.WriteCompleteCallback), nc);
				if (!asyncResult.IsCompleted || !asyncResult.CompletedSynchronously)
				{
					return nextChunk;
				}
			}
			return 0;
		}

		protected void BeginRead()
		{
			if (this.Connection != null)
			{
				this.BeginRead(this.Connection);
			}
		}

		protected virtual void HandleError(object error, NetworkConnection connection)
		{
			ProtocolBaseServices.SessionTracer.TraceDebug<object, NetworkConnection>(this.SessionId, "Handling error \"{0}\" on connection {1}.", error, connection);
			if (error is SocketError && (SocketError)error == SocketError.TimedOut)
			{
				this.HandleIdleTimeout();
				return;
			}
			lock (this.LockObject)
			{
				if (this.processingCommandRefCounter > 0)
				{
					this.disconnected = true;
				}
				else
				{
					this.Dispose();
				}
			}
		}

		private int GetNextChunk(out byte[] buffer, out int offset)
		{
			buffer = null;
			offset = 0;
			if (this.disposed)
			{
				return 0;
			}
			int result;
			lock (this.LockObject)
			{
				if (this.disposed)
				{
					result = 0;
				}
				else
				{
					result = this.responseItems.GetNextChunk(this, out buffer, out offset);
				}
			}
			return result;
		}

		private void WriteCompleteCallback(IAsyncResult iar)
		{
			this.LastActivityTime = ExDateTime.UtcNow;
			this.EnableUserTracing();
			try
			{
				ProtocolBaseServices.SessionTracer.TraceDebug<string, bool>(this.SessionId, "User {0} entering BaseSession.WriteCompleteCallback. CompletedSynchronously {1}", this.GetUserNameForLogging(), iar.CompletedSynchronously);
				NetworkConnection networkConnection = (NetworkConnection)iar.AsyncState;
				object obj;
				networkConnection.EndWrite(iar, out obj);
				if (obj != null)
				{
					this.HandleError(obj, networkConnection);
				}
				else if (!iar.CompletedSynchronously)
				{
					this.SendNextChunk(networkConnection);
				}
			}
			catch (Exception exception)
			{
				if (!this.CheckNonCriticalException(exception))
				{
					throw;
				}
				this.HandleError(null, iar.AsyncState as NetworkConnection);
			}
			finally
			{
				this.DisableUserTracing();
				ProtocolBaseServices.InMemoryTraceOperationCompleted(this.SessionId);
			}
		}

		private static volatile BufferPool proxyBufferPool;

		private static object proxyBufferPoolLock = new object();

		private readonly long sessionId;

		private NetworkConnection connection;

		private bool negotiatingTls;

		private int processingCommandRefCounter;

		private bool disconnected;

		private BaseSession.ConnectionShutdownDelegate connectionShutdown;

		private ResponseQueue responseItems;

		private bool disposed;

		private int maxCommandLength;

		private byte[] largeReceiveBuffer;

		private int largeReceiveBufferLength;

		private DisposeTracker disposeTracker;

		private StringResponseItemProcessor stringResponseItemProcessor = new StringResponseItemProcessor();

		private EndResponseItem beginReadResponseItem;

		private volatile byte[] proxyBuffer;

		public delegate void ConnectionShutdownDelegate();

		public delegate void SendCompleteDelegate();
	}
}
