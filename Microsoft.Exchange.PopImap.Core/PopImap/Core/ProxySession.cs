using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.PopImap.Core
{
	internal abstract class ProxySession : BaseSession
	{
		public ProxySession(ResponseFactory responseFactory, NetworkConnection networkConnection) : base(networkConnection, ProtocolBaseServices.Service.ConnectionTimeout, 4096)
		{
			this.responseFactory = responseFactory;
			this.incomingSession = responseFactory.Session;
			if (this.responseFactory == null || this.IncomingSession == null || this.IncomingSession.VirtualServer == null)
			{
				return;
			}
			this.IncomingSession.ProxySession = this;
			this.IncomingSession.VirtualServer.Proxy_Connections_Total.Increment();
			this.IncomingSession.VirtualServer.Proxy_Connections_Current.Increment();
			if (responseFactory.ProxyEncryptionType == EncryptionType.TLS)
			{
				this.proxyState = ProxySession.ProxyState.initTls;
				this.EnterReadLoop(base.Connection);
				return;
			}
			if (responseFactory.ProxyEncryptionType == EncryptionType.SSL)
			{
				this.proxyState = ProxySession.ProxyState.startTls;
				this.TransitProxyState(null, 0, 0);
				return;
			}
			if (responseFactory.ProxyEncryptionType == null)
			{
				this.proxyState = ProxySession.ProxyState.initialization;
				this.EnterReadLoop(base.Connection);
			}
		}

		public ProxySession.ProxyState State
		{
			get
			{
				return this.proxyState;
			}
			set
			{
				lock (this.LockObject)
				{
					if (this.proxyState != ProxySession.ProxyState.failed && value == ProxySession.ProxyState.failed && this.IncomingSession.VirtualServer != null && this.IncomingSession.VirtualServer.Proxy_Connections_Failed != null)
					{
						this.IncomingSession.VirtualServer.Proxy_Connections_Failed.Increment();
					}
					this.proxyState = value;
					if (this.responseFactory != null && this.responseFactory.ConnectionCreated != null && (this.proxyState == ProxySession.ProxyState.completed || this.proxyState == ProxySession.ProxyState.failed))
					{
						lock (this.responseFactory)
						{
							if (this.responseFactory.ConnectionCreated != null && !this.responseFactory.ConnectionCreated.SafeWaitHandle.IsClosed)
							{
								this.responseFactory.ConnectionCreated.Set();
							}
						}
					}
				}
			}
		}

		public bool IsConnected
		{
			get
			{
				return this.proxyState == ProxySession.ProxyState.completed;
			}
		}

		public ProtocolSession IncomingSession
		{
			get
			{
				return this.incomingSession;
			}
		}

		protected override object LockObject
		{
			get
			{
				return this.incomingSession;
			}
		}

		public override string ToString()
		{
			if (base.Connection != null)
			{
				return string.Format("Proxy connection {0} from {1} to {2}", this.GetHashCode(), base.LocalEndPoint, base.RemoteEndPoint);
			}
			return string.Format("Disconnected proxy connection {0}", base.SessionId);
		}

		public override bool IsUserTraceEnabled()
		{
			return this.incomingSession.IsUserTraceEnabled();
		}

		public override string GetUserNameForLogging()
		{
			return this.IncomingSession.GetUserNameForLogging();
		}

		public override bool CheckNonCriticalException(Exception exception)
		{
			if (this.incomingSession.LightLogSession != null)
			{
				this.incomingSession.LightLogSession.ExceptionCaught = exception;
			}
			return base.CheckNonCriticalException(exception);
		}

		public void TransitProxyState(byte[] buf, int offset, int size)
		{
			lock (this.LockObject)
			{
				this.responseFactory.DoProxyConnect(buf, offset, size, this);
				if (this.State != ProxySession.ProxyState.initialization)
				{
					base.SendToClient(base.BeginReadResponseItem);
				}
			}
		}

		internal void Shutdown()
		{
			lock (this.LockObject)
			{
				if (this.IsConnected)
				{
					this.incomingSession.BeginShutdown();
				}
				else
				{
					this.State = ProxySession.ProxyState.failed;
					if (base.Connection != null)
					{
						try
						{
							base.Connection.Shutdown();
							base.Connection.Dispose();
						}
						catch (Exception arg)
						{
							ProtocolBaseServices.SessionTracer.TraceDebug<Exception>(base.SessionId, "Exception caught while closing proxy connection:\r\n{0}.", arg);
						}
						base.Connection = null;
					}
				}
			}
		}

		internal override void HandleCommandTooLongError(NetworkConnection nc, byte[] buf, int offset, int amount)
		{
			if (base.Disposed)
			{
				ProtocolBaseServices.SessionTracer.TraceDebug(base.SessionId, "ProxySession.HandleCommandTooLongError. Proxy session disposed");
				return;
			}
			if (!this.IsConnected)
			{
				this.incomingSession.HandleCommandTooLongError(nc, buf, offset, amount);
				return;
			}
			if (!this.incomingSession.SendToClient(new BufferResponseItem(buf, offset, amount)))
			{
				return;
			}
			this.IncomingSession.SendToClient(base.BeginReadResponseItem);
		}

		internal override void HandleIdleTimeout()
		{
			this.incomingSession.HandleIdleTimeout();
		}

		protected override void HandleError(object error, NetworkConnection connection)
		{
			base.HandleError(error, connection);
			this.Shutdown();
		}

		protected override void HandleCommand(NetworkConnection nc, byte[] buf, int offset, int size)
		{
			if (base.Disposed)
			{
				ProtocolBaseServices.SessionTracer.TraceDebug(base.SessionId, "ProxySession.HandleCommand. Proxy session disposed");
				return;
			}
			if (!this.IsConnected)
			{
				this.TransitProxyState(buf, offset, size);
				return;
			}
			if (this.ForwardToClient(buf, offset, size))
			{
				this.incomingSession.SendBufferAsCommand(buf, offset, size);
				if (this.incomingSession.LightLogSession != null)
				{
					this.incomingSession.LightLogSession.ResponseSize += (long)size;
					this.incomingSession.LightLogSession.FlushProxyTraffic();
				}
			}
			this.IncomingSession.SendToClient(base.BeginReadResponseItem);
		}

		protected virtual void ReadCompleteCallback(IAsyncResult iar)
		{
			try
			{
				ProtocolBaseServices.SessionTracer.TraceDebug<string>(base.SessionId, "User {0} entering ProxySession.ReadCompleteCallback.", this.IncomingSession.GetUserNameForLogging());
				NetworkConnection networkConnection = (NetworkConnection)iar.AsyncState;
				byte[] src;
				int srcOffset;
				int num;
				object obj;
				networkConnection.EndRead(iar, out src, out srcOffset, out num, out obj);
				if (obj != null)
				{
					this.HandleError(obj, networkConnection);
				}
				else if (base.Disposed)
				{
					ProtocolBaseServices.SessionTracer.TraceDebug(base.SessionId, "ProxySession.ReadCompleteCallback. Proxy session disposed");
				}
				else
				{
					byte[] array;
					if (num > base.ProxyBuffer.Length)
					{
						array = new byte[num];
					}
					else
					{
						array = base.ProxyBuffer;
					}
					Buffer.BlockCopy(src, srcOffset, array, 0, num);
					if (this.IncomingSession.SendToClient(new BufferResponseItem(array, 0, num)))
					{
						this.IncomingSession.SendToClient(base.BeginReadResponseItem);
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
				ProtocolBaseServices.InMemoryTraceOperationCompleted(base.SessionId);
			}
		}

		protected override void InternalDispose()
		{
			lock (this.LockObject)
			{
				try
				{
					if (this.IncomingSession != null && this.IncomingSession.VirtualServer != null)
					{
						this.IncomingSession.VirtualServer.Proxy_Connections_Current.Decrement();
					}
				}
				finally
				{
					base.InternalDispose();
				}
			}
		}

		protected abstract bool ForwardToClient(byte[] buf, int offset, int size);

		private ProxySession.ProxyState proxyState;

		private ProtocolSession incomingSession;

		private ResponseFactory responseFactory;

		public enum ProxyState
		{
			failed,
			initTls,
			startTls,
			initialization,
			user,
			waitCapaOk,
			sendAuthPlain,
			sendAuthS2S,
			sendAuthBlob,
			sendS2SAuthBlob,
			authenticated,
			sendXproxy,
			sendXproxy3,
			getNamespace,
			waitingOK,
			completed
		}
	}
}
