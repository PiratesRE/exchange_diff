using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Net;
using Microsoft.Exchange.Security;

namespace Microsoft.Exchange.Net
{
	internal class NetworkConnection : DisposeTrackableBase, INetworkConnection, IDisposable
	{
		public NetworkConnection(Socket socket, int bufferSize = 4096)
		{
			ArgumentValidator.ThrowIfNull("socket", socket);
			this.ServerTlsProtocols = NetworkConnection.DefaultServerTlsProtocols;
			this.ClientTlsProtocols = NetworkConnection.DefaultClientTlsProtocols;
			this.socket = socket;
			this.recvBuffer = new NetworkBuffer(bufferSize);
			this.connectionId = Interlocked.Increment(ref NetworkConnection.lastConnectionId);
			try
			{
				this.localEndPoint = (socket.LocalEndPoint as IPEndPoint);
				this.remoteEndPoint = (socket.RemoteEndPoint as IPEndPoint);
			}
			catch (SocketException)
			{
			}
			catch (ObjectDisposedException)
			{
			}
			if (this.localEndPoint == null)
			{
				this.localEndPoint = NetworkConnection.nullEndPoint;
			}
			if (this.remoteEndPoint == null)
			{
				this.remoteEndPoint = NetworkConnection.nullEndPoint;
			}
			NetworkTimer.Add(this);
		}

		public SchannelProtocols ServerTlsProtocols { get; set; }

		public static SchannelProtocols DefaultServerTlsProtocols
		{
			get
			{
				return SspiContext.DefaultServerTlsProtocols;
			}
		}

		public SchannelProtocols ClientTlsProtocols { get; set; }

		public static SchannelProtocols DefaultClientTlsProtocols
		{
			get
			{
				return SspiContext.DefaultClientTlsProtocols;
			}
		}

		public ChannelBindingToken ChannelBindingToken
		{
			get
			{
				return this.channelBindingToken;
			}
		}

		public long ConnectionId
		{
			get
			{
				return this.connectionId;
			}
		}

		public int ReceiveTimeout
		{
			get
			{
				return (int)(this.recvTimeoutPeriod / 10000000L);
			}
			set
			{
				this.recvTimeoutPeriod = (long)value * 10000000L;
			}
		}

		public int SendTimeout
		{
			get
			{
				return (int)(this.sendTimeoutPeriod / 10000000L);
			}
			set
			{
				this.sendTimeoutPeriod = (long)value * 10000000L;
			}
		}

		public int Timeout
		{
			set
			{
				this.SendTimeout = value;
				this.ReceiveTimeout = value;
			}
		}

		public IPEndPoint LocalEndPoint
		{
			get
			{
				return this.localEndPoint;
			}
		}

		public IPEndPoint RemoteEndPoint
		{
			get
			{
				return this.remoteEndPoint;
			}
		}

		public int MaxLineLength
		{
			get
			{
				return this.maxLineLength;
			}
			set
			{
				if (value <= 0 || value > 4096)
				{
					throw new ArgumentException(NetException.OutOfRange, "value");
				}
				this.maxLineLength = value;
			}
		}

		public long BytesReceived
		{
			get
			{
				return this.bytesReceived;
			}
		}

		public long BytesSent
		{
			get
			{
				return this.bytesSent;
			}
		}

		public bool IsDataAvailable
		{
			get
			{
				return this.recvBuffer.Remaining > 0;
			}
		}

		public bool IsLineAvailable
		{
			get
			{
				bool flag;
				return this.recvBuffer.FindLine(this.maxLineLength, out flag) != -1;
			}
		}

		public bool IsTls
		{
			get
			{
				return this.sspiContext != null && this.sspiContext.State == ContextState.NegotiationComplete;
			}
		}

		public byte[] TlsEapKey
		{
			get
			{
				if (!this.IsTls)
				{
					return null;
				}
				EapKeyBlock eapKeyBlock;
				SecurityStatus securityStatus = this.sspiContext.QueryEapKeyBlock(out eapKeyBlock);
				if (securityStatus != SecurityStatus.OK)
				{
					return null;
				}
				return eapKeyBlock.rgbKeys;
			}
		}

		public int TlsCipherKeySize
		{
			get
			{
				return this.TlsConnectionInfo.CipherStrength;
			}
		}

		public ConnectionInfo TlsConnectionInfo
		{
			get
			{
				if (!this.IsTls)
				{
					return ConnectionInfo.Empty;
				}
				ConnectionInfo result;
				SecurityStatus securityStatus = this.sspiContext.QueryConnectionInfo(out result);
				if (securityStatus != SecurityStatus.OK)
				{
					return ConnectionInfo.Empty;
				}
				return result;
			}
		}

		public IX509Certificate2 RemoteCertificate
		{
			get
			{
				if (!this.IsTls)
				{
					return null;
				}
				if (this.remoteCertificate == null)
				{
					X509Certificate2 x509Certificate;
					SecurityStatus securityStatus = this.sspiContext.QueryRemoteCertificate(out x509Certificate);
					if (x509Certificate != null)
					{
						this.remoteCertificate = new X509Certificate2Wrapper(x509Certificate);
					}
					if (securityStatus != SecurityStatus.OK)
					{
						return null;
					}
				}
				return this.remoteCertificate;
			}
		}

		public IX509Certificate2 LocalCertificate
		{
			get
			{
				if (!this.IsTls)
				{
					return null;
				}
				if (this.localCertificate == null)
				{
					X509Certificate2 x509Certificate;
					SecurityStatus securityStatus = this.sspiContext.QueryLocalCertificate(out x509Certificate);
					if (x509Certificate != null)
					{
						this.localCertificate = new X509Certificate2Wrapper(x509Certificate);
					}
					if (securityStatus != SecurityStatus.OK)
					{
						throw new CryptographicException((int)securityStatus);
					}
				}
				return this.localCertificate;
			}
		}

		public Task<NetworkConnection.LazyAsyncResultWithTimeout> ReadAsync()
		{
			TaskCompletionSource<NetworkConnection.LazyAsyncResultWithTimeout> taskCompletionSource = new TaskCompletionSource<NetworkConnection.LazyAsyncResultWithTimeout>();
			this.BeginRead(new AsyncCallback(this.ReadComplete), taskCompletionSource);
			return taskCompletionSource.Task;
		}

		public Task<NetworkConnection.LazyAsyncResultWithTimeout> ReadLineAsync()
		{
			TaskCompletionSource<NetworkConnection.LazyAsyncResultWithTimeout> taskCompletionSource = new TaskCompletionSource<NetworkConnection.LazyAsyncResultWithTimeout>();
			this.BeginReadLine(new AsyncCallback(this.ReadLineComplete), taskCompletionSource);
			return taskCompletionSource.Task;
		}

		public Task<object> WriteAsync(byte[] buffer, int offset, int size)
		{
			ArgumentValidator.ThrowIfNull("buffer", buffer);
			ArgumentValidator.ThrowIfOutOfRange<int>("offset", offset, 0, buffer.Length - 1);
			ArgumentValidator.ThrowIfOutOfRange<int>("size", size, 1, buffer.Length - offset);
			TaskCompletionSource<object> taskCompletionSource = new TaskCompletionSource<object>();
			this.BeginWrite(buffer, offset, size, new AsyncCallback(this.WriteComplete), taskCompletionSource);
			return taskCompletionSource.Task;
		}

		public string GetBreadCrumbsInfo()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("LastIndex : ");
			stringBuilder.Append(this.breadcrumbs.LastFilledIndex);
			stringBuilder.AppendLine();
			stringBuilder.AppendLine("Network Connection BreadCrumb : ");
			for (int i = 0; i < 64; i++)
			{
				stringBuilder.Append(Enum.Format(typeof(NetworkConnection.Breadcrumb), this.breadcrumbs.BreadCrumb[i], "x"));
				stringBuilder.Append(" ");
			}
			stringBuilder.AppendLine();
			return stringBuilder.ToString();
		}

		public void Shutdown()
		{
			this.breadcrumbs.Drop(NetworkConnection.Breadcrumb.Shutdown);
			Socket socket = Interlocked.Exchange<Socket>(ref this.socket, null);
			if (socket != null)
			{
				socket.Close();
			}
		}

		public void Shutdown(int waitSeconds)
		{
			this.breadcrumbs.Drop(NetworkConnection.Breadcrumb.Shutdown);
			Socket socket = Interlocked.Exchange<Socket>(ref this.socket, null);
			if (socket != null)
			{
				try
				{
					socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontLinger, false);
					socket.LingerState = new LingerOption(true, waitSeconds);
				}
				catch (SocketException ex)
				{
					ExTraceGlobals.NetworkTracer.TraceError<SocketError, string>(this.connectionId, "SocketException {0} : {1} when setting linger state in Shutdown", ex.SocketErrorCode, ex.Message);
				}
				catch (ObjectDisposedException)
				{
					ExTraceGlobals.NetworkTracer.TraceError(this.connectionId, "ObjectDisposedException when setting linger state in Shutdown");
				}
				finally
				{
					socket.Close();
				}
			}
		}

		public void Read(out byte[] buffer, out int offset, out int size, out object errorCode)
		{
			NetworkConnection.LazyAsyncResultWithTimeout lazyAsyncResultWithTimeout = (NetworkConnection.LazyAsyncResultWithTimeout)this.BeginRead(null, null);
			try
			{
				this.EndRead(lazyAsyncResultWithTimeout, out buffer, out offset, out size, out errorCode);
			}
			finally
			{
				lazyAsyncResultWithTimeout.InternalCleanup();
			}
		}

		public IAsyncResult BeginRead(AsyncCallback callback, object state)
		{
			this.breadcrumbs.Drop(NetworkConnection.Breadcrumb.BeginRead);
			NetworkConnection.LazyAsyncResultWithTimeout lazyAsyncResultWithTimeout = new NetworkConnection.LazyAsyncResultWithTimeout(this.recvTimeoutPeriod, this, state, callback);
			if (!this.EnterConnection())
			{
				lazyAsyncResultWithTimeout.InvokeCallback(SocketError.Shutdown);
			}
			else if (this.recvBuffer.Remaining > 0)
			{
				lazyAsyncResultWithTimeout.InvokeCallback();
			}
			else
			{
				this.SetNextRecvTimeout(lazyAsyncResultWithTimeout);
				this.InternalBeginReceive(lazyAsyncResultWithTimeout, NetworkConnection.readDataAvailable);
			}
			return lazyAsyncResultWithTimeout;
		}

		public void EndRead(IAsyncResult asyncResult, out byte[] buffer, out int offset, out int size, out object errorCode)
		{
			this.breadcrumbs.Drop(NetworkConnection.Breadcrumb.EndRead);
			NetworkConnection.LazyAsyncResultWithTimeout lazyAsyncResultWithTimeout = NetworkConnection.LazyAsyncResultWithTimeout.EndAsyncOperation(asyncResult);
			try
			{
				if (lazyAsyncResultWithTimeout.Result != null)
				{
					errorCode = lazyAsyncResultWithTimeout.Result;
					buffer = null;
					size = 0;
					offset = 0;
				}
				else
				{
					buffer = this.recvBuffer.Buffer;
					offset = this.recvBuffer.DataStartOffset;
					size = this.recvBuffer.Remaining;
					errorCode = null;
					this.recvBuffer.ConsumeData(size);
				}
			}
			finally
			{
				if (!this.LeaveConnection())
				{
					errorCode = SocketError.Shutdown;
				}
			}
		}

		public void PutBackReceivedBytes(int bytesUnconsumed)
		{
			this.breadcrumbs.Drop(NetworkConnection.Breadcrumb.PutBackReceivedBytes);
			this.recvBuffer.PutBackUnconsumedData(bytesUnconsumed);
		}

		public void ReadLine(out byte[] buffer, out int offset, out int size, out object errorCode)
		{
			NetworkConnection.LazyAsyncResultWithTimeout lazyAsyncResultWithTimeout = (NetworkConnection.LazyAsyncResultWithTimeout)this.BeginReadLine(null, null);
			try
			{
				this.EndReadLine(lazyAsyncResultWithTimeout, out buffer, out offset, out size, out errorCode);
			}
			finally
			{
				lazyAsyncResultWithTimeout.InternalCleanup();
			}
		}

		public IAsyncResult BeginReadLine(AsyncCallback callback, object state)
		{
			this.breadcrumbs.Drop(NetworkConnection.Breadcrumb.BeginReadLine);
			NetworkConnection.LazyAsyncResultWithTimeout lazyAsyncResultWithTimeout = new NetworkConnection.LazyAsyncResultWithTimeout(this.recvTimeoutPeriod, this, state, callback);
			if (!this.EnterConnection())
			{
				lazyAsyncResultWithTimeout.InvokeCallback(SocketError.Shutdown);
			}
			else if (this.IsLineAvailable)
			{
				lazyAsyncResultWithTimeout.InvokeCallback();
			}
			else
			{
				this.SetNextRecvTimeout(lazyAsyncResultWithTimeout);
				this.InternalBeginReceive(lazyAsyncResultWithTimeout, NetworkConnection.readLineDataAvailable);
			}
			return lazyAsyncResultWithTimeout;
		}

		public void EndReadLine(IAsyncResult asyncResult, out byte[] buffer, out int offset, out int size, out object errorCode)
		{
			this.breadcrumbs.Drop(NetworkConnection.Breadcrumb.EndReadLine);
			NetworkConnection.LazyAsyncResultWithTimeout lazyAsyncResultWithTimeout = NetworkConnection.LazyAsyncResultWithTimeout.EndAsyncOperation(asyncResult);
			try
			{
				if (lazyAsyncResultWithTimeout.Result != null)
				{
					errorCode = lazyAsyncResultWithTimeout.Result;
					buffer = null;
					size = 0;
					offset = 0;
				}
				else
				{
					bool flag;
					size = this.recvBuffer.FindLine(this.maxLineLength, out flag);
					if (size < 0)
					{
						throw new InvalidOperationException(NetException.FindLineError);
					}
					buffer = this.recvBuffer.Buffer;
					offset = this.recvBuffer.DataStartOffset;
					if (flag)
					{
						this.recvBuffer.ConsumeData(size);
						errorCode = SocketError.MessageSize;
					}
					else
					{
						this.recvBuffer.ConsumeData(size + 2);
						errorCode = null;
					}
				}
			}
			finally
			{
				if (!this.LeaveConnection())
				{
					errorCode = SocketError.Shutdown;
				}
			}
		}

		public void Write(byte[] buffer, int offset, int size, out object errorCode)
		{
			NetworkConnection.LazyAsyncResultWithTimeout lazyAsyncResultWithTimeout = (NetworkConnection.LazyAsyncResultWithTimeout)this.BeginWrite(buffer, offset, size, null, null);
			try
			{
				this.EndWrite(lazyAsyncResultWithTimeout, out errorCode);
			}
			finally
			{
				lazyAsyncResultWithTimeout.InternalCleanup();
			}
		}

		public IAsyncResult BeginWrite(byte[] buffer, int offset, int size, AsyncCallback callback, object state)
		{
			this.breadcrumbs.Drop(NetworkConnection.Breadcrumb.BeginWrite);
			NetworkConnection.LazyAsyncResultWithTimeout lazyAsyncResultWithTimeout = new NetworkConnection.LazyAsyncResultWithTimeout(this.sendTimeoutPeriod, this, state, callback);
			if (!this.EnterConnection())
			{
				lazyAsyncResultWithTimeout.InvokeCallback(SocketError.Shutdown);
			}
			else
			{
				lazyAsyncResultWithTimeout.Buffer = buffer;
				lazyAsyncResultWithTimeout.Offset = offset;
				lazyAsyncResultWithTimeout.Size = size;
				this.SetNextSendTimeout(lazyAsyncResultWithTimeout);
				this.InternalBeginSend(lazyAsyncResultWithTimeout);
			}
			return lazyAsyncResultWithTimeout;
		}

		public IAsyncResult BeginWrite(Stream stream, AsyncCallback callback, object state)
		{
			this.breadcrumbs.Drop(NetworkConnection.Breadcrumb.BeginWriteStream);
			ArgumentValidator.ThrowIfNull("stream", stream);
			NetworkConnection.WriteStreamAsyncResult writeStreamAsyncResult = new NetworkConnection.WriteStreamAsyncResult(this.sendTimeoutPeriod, stream, this, state, callback);
			this.SetNextSendTimeout(writeStreamAsyncResult);
			if (!this.EnterConnection())
			{
				writeStreamAsyncResult.InvokeCallback(SocketError.Shutdown);
			}
			else
			{
				if (this.sendBuffer == null)
				{
					this.sendBuffer = new NetworkBuffer(16432);
				}
				this.ReadFromStream(writeStreamAsyncResult);
			}
			return writeStreamAsyncResult;
		}

		public void EndWrite(IAsyncResult asyncResult, out object errorCode)
		{
			this.breadcrumbs.Drop(NetworkConnection.Breadcrumb.EndWrite);
			NetworkConnection.LazyAsyncResultWithTimeout lazyAsyncResultWithTimeout = NetworkConnection.LazyAsyncResultWithTimeout.EndAsyncOperation(asyncResult);
			errorCode = (this.LeaveConnection() ? lazyAsyncResultWithTimeout.Result : SocketError.Shutdown);
		}

		public IAsyncResult BeginNegotiateTlsAsClient(AsyncCallback callback, object state)
		{
			return this.BeginNegotiateTlsAsClient(null, null, callback, state);
		}

		public IAsyncResult BeginNegotiateTlsAsClient(X509Certificate certificate, AsyncCallback callback, object state)
		{
			return this.BeginNegotiateTlsAsClient(certificate, null, callback, state);
		}

		public Task<object> NegotiateTlsAsClientAsync(IX509Certificate2 certificate, string targetName)
		{
			ArgumentValidator.ThrowIfNull("targetName", targetName);
			return Task<object>.Factory.FromAsync(this.BeginNegotiateTlsAsClient((certificate == null) ? null : certificate.Certificate, targetName, null, null), new Func<IAsyncResult, object>(this.EndNegotiateTlsAsClient));
		}

		public IAsyncResult BeginNegotiateTlsAsClient(X509Certificate certificate, string targetName, AsyncCallback callback, object state)
		{
			this.breadcrumbs.Drop(NetworkConnection.Breadcrumb.BeginNegotiateTlsAsClient);
			if (this.IsTls)
			{
				throw new InvalidOperationException(NetException.TlsAlreadyNegotiated);
			}
			NetworkConnection.LazyAsyncResultWithTimeout lazyAsyncResultWithTimeout = new NetworkConnection.LazyAsyncResultWithTimeout(this.sendTimeoutPeriod, this, state, callback);
			Socket socket = this.socket;
			if (!this.EnterConnection() || socket == null)
			{
				lazyAsyncResultWithTimeout.InvokeCallback(SocketError.Shutdown);
				return lazyAsyncResultWithTimeout;
			}
			this.sspiContext = new SspiContext
			{
				ClientTlsProtocols = this.ClientTlsProtocols
			};
			SecurityStatus securityStatus = this.sspiContext.InitializeForTls(CredentialUse.Outbound, false, certificate, targetName);
			if (securityStatus != SecurityStatus.OK)
			{
				lazyAsyncResultWithTimeout.InvokeCallback(securityStatus);
				return lazyAsyncResultWithTimeout;
			}
			this.recvBuffer.ExpandBuffer(this.sspiContext.MaxTokenSize);
			if (this.sendBuffer == null)
			{
				this.sendBuffer = new NetworkBuffer(this.sspiContext.MaxTokenSize);
			}
			else
			{
				this.sendBuffer.ExpandBuffer(this.sspiContext.MaxTokenSize);
			}
			securityStatus = this.sspiContext.NegotiateSecurityContext(null, this.sendBuffer);
			if (securityStatus != SecurityStatus.ContinueNeeded && securityStatus != SecurityStatus.OK)
			{
				lazyAsyncResultWithTimeout.InvokeCallback(securityStatus);
				return lazyAsyncResultWithTimeout;
			}
			this.SetNextSendTimeout(lazyAsyncResultWithTimeout);
			this.sendBuffer.ShuffleBuffer();
			SocketError socketError;
			try
			{
				socket.BeginSend(this.sendBuffer.Buffer, this.sendBuffer.BufferStartOffset, this.sendBuffer.Filled, SocketFlags.None, out socketError, NetworkConnection.sendTlsNegotiationCompleted, lazyAsyncResultWithTimeout);
			}
			catch (ObjectDisposedException)
			{
				socketError = SocketError.Shutdown;
			}
			if (socketError != SocketError.Success && socketError != SocketError.IOPending)
			{
				this.InvokeSendCallback(lazyAsyncResultWithTimeout, socketError);
				return lazyAsyncResultWithTimeout;
			}
			return lazyAsyncResultWithTimeout;
		}

		public void EndNegotiateTlsAsClient(IAsyncResult asyncResult, out object errorCode)
		{
			this.breadcrumbs.Drop(NetworkConnection.Breadcrumb.EndNegotiateTlsAsClient);
			ArgumentValidator.ThrowIfNull("asyncResult", asyncResult);
			NetworkConnection.LazyAsyncResultWithTimeout lazyAsyncResultWithTimeout = NetworkConnection.LazyAsyncResultWithTimeout.EndAsyncOperation(asyncResult);
			errorCode = lazyAsyncResultWithTimeout.Result;
			if (!this.LeaveConnection())
			{
				errorCode = SocketError.Shutdown;
			}
		}

		public object EndNegotiateTlsAsClient(IAsyncResult asyncResult)
		{
			this.breadcrumbs.Drop(NetworkConnection.Breadcrumb.EndNegotiateTlsAsClient);
			ArgumentValidator.ThrowIfNull("asyncResult", asyncResult);
			NetworkConnection.LazyAsyncResultWithTimeout lazyAsyncResultWithTimeout = NetworkConnection.LazyAsyncResultWithTimeout.EndAsyncOperation(asyncResult);
			object result = lazyAsyncResultWithTimeout.Result;
			if (!this.LeaveConnection())
			{
				result = SocketError.Shutdown;
			}
			return result;
		}

		public IAsyncResult BeginNegotiateTlsAsServer(X509Certificate2 cert, AsyncCallback callback, object state)
		{
			return this.BeginNegotiateTlsAsServer(cert, false, callback, state);
		}

		public Task<object> NegotiateTlsAsServerAsync(IX509Certificate2 certificate, bool requestClientCertificate)
		{
			ArgumentValidator.ThrowIfNull("certificate", certificate);
			return Task.Factory.FromAsync<X509Certificate2, object>(new Func<X509Certificate2, AsyncCallback, object, IAsyncResult>(this.BeginNegotiateTlsAsServer), new Func<IAsyncResult, object>(this.EndNegotiateTlsAsServerInternal), certificate.Certificate, null);
		}

		public IAsyncResult BeginNegotiateTlsAsServer(X509Certificate2 cert, bool requestClientCertificate, AsyncCallback callback, object state)
		{
			this.breadcrumbs.Drop(NetworkConnection.Breadcrumb.BeginNegotiateTlsAsServer);
			ArgumentValidator.ThrowIfNull("cert", cert);
			if (this.IsTls)
			{
				throw new InvalidOperationException(NetException.TlsAlreadyNegotiated);
			}
			ExTraceGlobals.NetworkTracer.Information((long)this.GetHashCode(), requestClientCertificate ? "Beginning TLS negotiation with request for client certificate." : "Beginning TLS negotiation without request for client certificate.");
			NetworkConnection.LazyAsyncResultWithTimeout lazyAsyncResultWithTimeout = new NetworkConnection.LazyAsyncResultWithTimeout(this.sendTimeoutPeriod, this, state, callback);
			Socket socket = this.socket;
			if (!this.EnterConnection() || socket == null)
			{
				lazyAsyncResultWithTimeout.InvokeCallback(SocketError.Shutdown);
				return lazyAsyncResultWithTimeout;
			}
			this.sspiContext = new SspiContext
			{
				ServerTlsProtocols = this.ServerTlsProtocols
			};
			SecurityStatus securityStatus = this.sspiContext.InitializeForTls(CredentialUse.Inbound, requestClientCertificate, cert, null);
			if (securityStatus != SecurityStatus.OK)
			{
				lazyAsyncResultWithTimeout.InvokeCallback(securityStatus);
				return lazyAsyncResultWithTimeout;
			}
			this.recvBuffer.ExpandBuffer(this.sspiContext.MaxTokenSize);
			if (this.sendBuffer == null)
			{
				this.sendBuffer = new NetworkBuffer(this.sspiContext.MaxTokenSize);
			}
			else
			{
				this.sendBuffer.ExpandBuffer(this.sspiContext.MaxTokenSize);
			}
			this.SetNextSendTimeout(lazyAsyncResultWithTimeout);
			this.recvBuffer.ShuffleBuffer();
			SocketError socketError;
			try
			{
				socket.BeginReceive(this.recvBuffer.Buffer, this.recvBuffer.UnusedStartOffset, this.recvBuffer.Unused, SocketFlags.None, out socketError, NetworkConnection.recvTlsNegotiationCompleted, lazyAsyncResultWithTimeout);
			}
			catch (ObjectDisposedException)
			{
				socketError = SocketError.Shutdown;
			}
			if (socketError != SocketError.Success && socketError != SocketError.IOPending)
			{
				this.InvokeSendCallback(lazyAsyncResultWithTimeout, securityStatus);
				return lazyAsyncResultWithTimeout;
			}
			return lazyAsyncResultWithTimeout;
		}

		public void EndNegotiateTlsAsServer(IAsyncResult asyncResult, out object errorCode)
		{
			this.breadcrumbs.Drop(NetworkConnection.Breadcrumb.EndNegotiateTlsAsServer);
			NetworkConnection.LazyAsyncResultWithTimeout lazyAsyncResultWithTimeout = NetworkConnection.LazyAsyncResultWithTimeout.EndAsyncOperation(asyncResult);
			errorCode = lazyAsyncResultWithTimeout.Result;
			if (errorCode == null)
			{
				ExTraceGlobals.NetworkTracer.Information((long)this.GetHashCode(), (this.RemoteCertificate != null) ? "Client certificate provided during TLS negotiation." : "Client certificate was not provided during TLS negotiation.");
			}
			if (!this.LeaveConnection())
			{
				errorCode = SocketError.Shutdown;
			}
		}

		private object EndNegotiateTlsAsServerInternal(IAsyncResult asyncResult)
		{
			object result;
			this.EndNegotiateTlsAsServer(asyncResult, out result);
			return result;
		}

		internal void CheckForTimeouts(long now)
		{
			if (Thread.VolatileRead(ref this.referenceCount) < 0)
			{
				return;
			}
			NetworkConnection.LazyAsyncResultWithTimeout lazyAsyncResultWithTimeout = (NetworkConnection.LazyAsyncResultWithTimeout)this.recvAsyncResult.Target;
			if (this.recvAsyncResultHashCode != 0 && lazyAsyncResultWithTimeout != null && lazyAsyncResultWithTimeout.TimedOut(now))
			{
				this.breadcrumbs.Drop(NetworkConnection.Breadcrumb.RecvTimeout);
				ThreadPool.QueueUserWorkItem(NetworkConnection.timeoutOccurredCallback, lazyAsyncResultWithTimeout);
			}
			lazyAsyncResultWithTimeout = (NetworkConnection.LazyAsyncResultWithTimeout)this.sendAsyncResult.Target;
			if (this.sendAsyncResultHashCode != 0 && lazyAsyncResultWithTimeout != null && lazyAsyncResultWithTimeout.TimedOut(now))
			{
				this.breadcrumbs.Drop(NetworkConnection.Breadcrumb.SendTimeout);
				ThreadPool.QueueUserWorkItem(NetworkConnection.timeoutOccurredCallback, lazyAsyncResultWithTimeout);
			}
		}

		protected override void InternalDispose(bool disposing)
		{
			NetworkTimer.Remove(this);
			if (disposing)
			{
				this.breadcrumbs.Drop(NetworkConnection.Breadcrumb.Dispose);
				Socket socket = Interlocked.Exchange<Socket>(ref this.socket, null);
				if (socket != null)
				{
					try
					{
						socket.Shutdown(SocketShutdown.Both);
					}
					catch (SocketException ex)
					{
						ExTraceGlobals.NetworkTracer.TraceError<SocketError, string>(this.connectionId, "SocketException {0} : {1} when socket shutdown in Dispose", ex.SocketErrorCode, ex.Message);
					}
					finally
					{
						socket.Close();
					}
				}
				int num = Thread.VolatileRead(ref this.referenceCount);
				int num2;
				while ((num2 = Interlocked.CompareExchange(ref this.referenceCount, num - 1 | -2147483648, num)) != num)
				{
					num = num2;
				}
				if (num2 == 1)
				{
					this.ReleaseResources();
				}
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<NetworkConnection>(this);
		}

		protected virtual void ReadComplete(IAsyncResult iar)
		{
			TaskCompletionSource<NetworkConnection.LazyAsyncResultWithTimeout> taskCompletionSource = (TaskCompletionSource<NetworkConnection.LazyAsyncResultWithTimeout>)iar.AsyncState;
			try
			{
				byte[] buffer;
				int offset;
				int size;
				object obj;
				this.EndRead(iar, out buffer, out offset, out size, out obj);
				NetworkConnection.LazyAsyncResultWithTimeout lazyAsyncResultWithTimeout = (NetworkConnection.LazyAsyncResultWithTimeout)iar;
				lazyAsyncResultWithTimeout.Buffer = buffer;
				lazyAsyncResultWithTimeout.Offset = offset;
				lazyAsyncResultWithTimeout.Size = size;
				taskCompletionSource.SetResult(lazyAsyncResultWithTimeout);
			}
			catch (Exception exception)
			{
				taskCompletionSource.SetException(exception);
			}
		}

		protected virtual void ReadLineComplete(IAsyncResult iar)
		{
			TaskCompletionSource<NetworkConnection.LazyAsyncResultWithTimeout> taskCompletionSource = (TaskCompletionSource<NetworkConnection.LazyAsyncResultWithTimeout>)iar.AsyncState;
			try
			{
				byte[] buffer;
				int offset;
				int size;
				object obj;
				this.EndReadLine(iar, out buffer, out offset, out size, out obj);
				NetworkConnection.LazyAsyncResultWithTimeout lazyAsyncResultWithTimeout = (NetworkConnection.LazyAsyncResultWithTimeout)iar;
				lazyAsyncResultWithTimeout.Buffer = buffer;
				lazyAsyncResultWithTimeout.Offset = offset;
				lazyAsyncResultWithTimeout.Size = size;
				taskCompletionSource.SetResult(lazyAsyncResultWithTimeout);
			}
			catch (Exception exception)
			{
				taskCompletionSource.SetException(exception);
			}
		}

		protected virtual void WriteComplete(IAsyncResult asyncResult)
		{
			TaskCompletionSource<object> taskCompletionSource = (TaskCompletionSource<object>)asyncResult.AsyncState;
			try
			{
				object result;
				this.EndWrite(asyncResult, out result);
				taskCompletionSource.SetResult(result);
			}
			catch (Exception exception)
			{
				taskCompletionSource.SetException(exception);
			}
		}

		private bool EnterConnection()
		{
			return Interlocked.Increment(ref this.referenceCount) > 0;
		}

		private bool LeaveConnection()
		{
			int num = Interlocked.Decrement(ref this.referenceCount);
			if (num == -2147483648)
			{
				this.ReleaseResources();
			}
			return num > 0;
		}

		private void ReleaseResources()
		{
			if (this.sspiContext != null)
			{
				this.sspiContext.Dispose();
				this.sspiContext = null;
			}
			if (this.recvBuffer != null)
			{
				this.recvBuffer.Dispose();
			}
			if (this.sendBuffer != null)
			{
				this.sendBuffer.Dispose();
				this.sendBuffer = null;
			}
		}

		private static void TimeoutOccurred(object state)
		{
			NetworkConnection.LazyAsyncResultWithTimeout lazyAsyncResultWithTimeout = (NetworkConnection.LazyAsyncResultWithTimeout)state;
			lazyAsyncResultWithTimeout.InvokeCallback(SocketError.TimedOut);
		}

		private static void ReadDataAvailable(IAsyncResult asyncResult)
		{
			NetworkConnection.LazyAsyncResultWithTimeout lazyAsyncResultWithTimeout = (NetworkConnection.LazyAsyncResultWithTimeout)asyncResult.AsyncState;
			NetworkConnection networkConnection = (NetworkConnection)lazyAsyncResultWithTimeout.AsyncObject;
			networkConnection.breadcrumbs.Drop(NetworkConnection.Breadcrumb.ReadDataAvailable);
			bool flag;
			object obj = networkConnection.EndReadAndDecrypt(asyncResult, out flag);
			if (obj == null && flag)
			{
				networkConnection.InternalBeginReceive(lazyAsyncResultWithTimeout, NetworkConnection.readDataAvailable);
				return;
			}
			if (obj != null)
			{
				networkConnection.breadcrumbs.Drop(NetworkConnection.Breadcrumb.ReadDataAvailableError);
			}
			networkConnection.InvokeRecvCallback(lazyAsyncResultWithTimeout, obj);
		}

		private static void ReadLineDataAvailable(IAsyncResult asyncResult)
		{
			NetworkConnection.LazyAsyncResultWithTimeout lazyAsyncResultWithTimeout = (NetworkConnection.LazyAsyncResultWithTimeout)asyncResult.AsyncState;
			NetworkConnection networkConnection = (NetworkConnection)lazyAsyncResultWithTimeout.AsyncObject;
			networkConnection.breadcrumbs.Drop(NetworkConnection.Breadcrumb.ReadLineDataAvailable);
			bool flag;
			object obj = networkConnection.EndReadAndDecrypt(asyncResult, out flag);
			if (obj == null && (flag || !networkConnection.IsLineAvailable))
			{
				networkConnection.InternalBeginReceive(lazyAsyncResultWithTimeout, NetworkConnection.readLineDataAvailable);
				return;
			}
			if (obj != null)
			{
				networkConnection.breadcrumbs.Drop(NetworkConnection.Breadcrumb.ReadLineDataAvailableError);
			}
			networkConnection.InvokeRecvCallback(lazyAsyncResultWithTimeout, obj);
		}

		private object EndReadAndDecrypt(IAsyncResult asyncResult, out bool incompleteMessage)
		{
			incompleteMessage = false;
			Socket socket = this.socket;
			if (socket == null)
			{
				return SocketError.Shutdown;
			}
			SocketError socketError;
			int num;
			try
			{
				num = socket.EndReceive(asyncResult, out socketError);
			}
			catch (ObjectDisposedException)
			{
				return SocketError.Shutdown;
			}
			if (num == 0 && socketError == SocketError.Success)
			{
				return SocketError.SocketError;
			}
			if (socketError != SocketError.Success)
			{
				return socketError;
			}
			if (this.IsTls)
			{
				this.recvBuffer.ReportEncryptedBytesFilled(num);
				SecurityStatus securityStatus = this.sspiContext.DecryptMessage(this.recvBuffer);
				if (securityStatus == SecurityStatus.IncompleteMessage)
				{
					incompleteMessage = true;
				}
				else if (securityStatus != SecurityStatus.OK)
				{
					return securityStatus;
				}
				this.bytesReceived += (long)num;
			}
			else
			{
				this.recvBuffer.ReportBytesFilled(num);
				this.bytesReceived += (long)num;
			}
			return null;
		}

		private static void SendStreamCompleted(IAsyncResult asyncResult)
		{
			NetworkConnection.WriteStreamAsyncResult writeStreamAsyncResult = (NetworkConnection.WriteStreamAsyncResult)asyncResult.AsyncState;
			NetworkConnection networkConnection = (NetworkConnection)writeStreamAsyncResult.AsyncObject;
			Socket socket = networkConnection.socket;
			int num = 0;
			SocketError socketError;
			if (socket == null)
			{
				socketError = SocketError.Shutdown;
			}
			else
			{
				try
				{
					num = socket.EndSend(asyncResult, out socketError);
				}
				catch (ObjectDisposedException)
				{
					socketError = SocketError.Shutdown;
				}
			}
			if (socketError != SocketError.Success)
			{
				networkConnection.InvokeSendCallback(writeStreamAsyncResult, socketError);
				return;
			}
			networkConnection.bytesSent += (long)num;
			writeStreamAsyncResult.UpdateTimeout(networkConnection.sendTimeoutPeriod);
			networkConnection.ReadFromStream(writeStreamAsyncResult);
		}

		private void ReadFromStream(NetworkConnection.WriteStreamAsyncResult userAsyncResult)
		{
			int count;
			if (this.IsTls)
			{
				this.sendBuffer.EmptyBufferReservingBytes(this.sspiContext.HeaderSize);
				count = Math.Min(this.sspiContext.MaxMessageSize, this.sendBuffer.Unused - this.sspiContext.TrailerSize);
			}
			else
			{
				this.sendBuffer.EmptyBuffer();
				count = this.sendBuffer.Unused;
			}
			int num;
			try
			{
				num = userAsyncResult.Stream.Read(this.sendBuffer.Buffer, this.sendBuffer.UnusedStartOffset, count);
			}
			catch (Exception ex)
			{
				if (ex is OutOfMemoryException || ex is StackOverflowException || ex is ThreadAbortException)
				{
					throw;
				}
				this.breadcrumbs.Drop(NetworkConnection.Breadcrumb.StreamBeginReadException);
				this.InvokeSendCallback(userAsyncResult, ex);
				return;
			}
			if (num != 0)
			{
				this.sendBuffer.ReportBytesFilled(num);
				if (this.IsTls)
				{
					SecurityStatus securityStatus = this.sspiContext.EncryptMessage(this.sendBuffer);
					if (securityStatus != SecurityStatus.OK)
					{
						this.InvokeSendCallback(userAsyncResult, securityStatus);
						return;
					}
				}
				Socket socket = this.socket;
				SocketError socketError;
				if (socket == null)
				{
					socketError = SocketError.Shutdown;
				}
				else
				{
					try
					{
						socket.BeginSend(this.sendBuffer.Buffer, this.sendBuffer.BufferStartOffset, this.sendBuffer.Filled, SocketFlags.None, out socketError, NetworkConnection.sendStreamCompleted, userAsyncResult);
					}
					catch (ObjectDisposedException)
					{
						socketError = SocketError.Shutdown;
					}
				}
				if (socketError != SocketError.Success && socketError != SocketError.IOPending)
				{
					this.InvokeSendCallback(userAsyncResult, socketError);
				}
				return;
			}
			this.InvokeSendCallback(userAsyncResult, null);
		}

		private void InternalBeginReceive(NetworkConnection.LazyAsyncResultWithTimeout asyncResult, AsyncCallback callback)
		{
			Socket socket = this.socket;
			SocketError socketError;
			if (socket == null)
			{
				socketError = SocketError.Shutdown;
			}
			else
			{
				this.recvBuffer.ShuffleBuffer();
				if (this.recvBuffer.Unused <= 0)
				{
					if (this.recvBuffer.Length >= 69680)
					{
						this.InvokeRecvCallback(asyncResult, SocketError.NoBufferSpaceAvailable);
						return;
					}
					this.recvBuffer.ExpandBuffer(this.recvBuffer.Length + 4096);
				}
				try
				{
					socket.BeginReceive(this.recvBuffer.Buffer, this.recvBuffer.UnusedStartOffset, this.recvBuffer.Unused, SocketFlags.None, out socketError, callback, asyncResult);
				}
				catch (ObjectDisposedException)
				{
					socketError = SocketError.Shutdown;
				}
			}
			if (socketError != SocketError.Success && socketError != SocketError.IOPending)
			{
				this.InvokeRecvCallback(asyncResult, socketError);
			}
		}

		private void InternalBeginSend(NetworkConnection.LazyAsyncResultWithTimeout asyncResult)
		{
			int num;
			byte[] buffer;
			int offset;
			int size;
			if (this.IsTls)
			{
				num = Math.Min(asyncResult.Size, this.sspiContext.MaxMessageSize);
				SecurityStatus securityStatus = this.sspiContext.EncryptMessage(asyncResult.Buffer, asyncResult.Offset, num, this.sendBuffer);
				if (securityStatus != SecurityStatus.OK)
				{
					this.InvokeSendCallback(asyncResult, securityStatus);
					return;
				}
				buffer = this.sendBuffer.Buffer;
				offset = this.sendBuffer.BufferStartOffset;
				size = this.sendBuffer.Filled;
			}
			else
			{
				num = asyncResult.Size;
				buffer = asyncResult.Buffer;
				offset = asyncResult.Offset;
				size = asyncResult.Size;
			}
			asyncResult.Size -= num;
			asyncResult.Offset += num;
			Socket socket = this.socket;
			SocketError socketError;
			if (socket == null)
			{
				socketError = SocketError.Shutdown;
			}
			else
			{
				try
				{
					socket.BeginSend(buffer, offset, size, SocketFlags.None, out socketError, NetworkConnection.internalSendCompleted, asyncResult);
				}
				catch (ObjectDisposedException)
				{
					socketError = SocketError.Shutdown;
				}
			}
			if (socketError != SocketError.Success && socketError != SocketError.IOPending)
			{
				this.InvokeSendCallback(asyncResult, socketError);
			}
		}

		private static void InternalSendCompleted(IAsyncResult asyncResult)
		{
			NetworkConnection.LazyAsyncResultWithTimeout lazyAsyncResultWithTimeout = (NetworkConnection.LazyAsyncResultWithTimeout)asyncResult.AsyncState;
			NetworkConnection networkConnection = (NetworkConnection)lazyAsyncResultWithTimeout.AsyncObject;
			Socket socket = networkConnection.socket;
			int num = 0;
			SocketError socketError;
			if (socket == null)
			{
				socketError = SocketError.Shutdown;
			}
			else
			{
				try
				{
					num = socket.EndSend(asyncResult, out socketError);
				}
				catch (ObjectDisposedException)
				{
					socketError = SocketError.Shutdown;
				}
			}
			if (socketError != SocketError.Success)
			{
				networkConnection.InvokeSendCallback(lazyAsyncResultWithTimeout, socketError);
				return;
			}
			networkConnection.bytesSent += (long)num;
			if (lazyAsyncResultWithTimeout.Size == 0)
			{
				networkConnection.InvokeSendCallback(lazyAsyncResultWithTimeout, null);
				return;
			}
			lazyAsyncResultWithTimeout.UpdateTimeout(networkConnection.sendTimeoutPeriod);
			networkConnection.InternalBeginSend(lazyAsyncResultWithTimeout);
		}

		private static void SendTlsNegotiationCompleted(IAsyncResult asyncResult)
		{
			NetworkConnection.LazyAsyncResultWithTimeout lazyAsyncResultWithTimeout = (NetworkConnection.LazyAsyncResultWithTimeout)asyncResult.AsyncState;
			NetworkConnection networkConnection = (NetworkConnection)lazyAsyncResultWithTimeout.AsyncObject;
			Socket socket = networkConnection.socket;
			int num = 0;
			SocketError socketError;
			if (socket == null)
			{
				socketError = SocketError.Shutdown;
			}
			else
			{
				try
				{
					num = socket.EndSend(asyncResult, out socketError);
				}
				catch (ObjectDisposedException)
				{
					socketError = SocketError.Shutdown;
				}
			}
			if (socketError != SocketError.Success)
			{
				networkConnection.InvokeSendCallback(lazyAsyncResultWithTimeout, socketError);
				return;
			}
			networkConnection.bytesSent += (long)num;
			networkConnection.sendBuffer.ConsumeData(num);
			if (networkConnection.sspiContext.State == ContextState.NegotiationComplete)
			{
				networkConnection.InvokeSendCallback(lazyAsyncResultWithTimeout, null);
				return;
			}
			networkConnection.recvBuffer.ShuffleBuffer();
			try
			{
				socket.BeginReceive(networkConnection.recvBuffer.Buffer, networkConnection.recvBuffer.UnusedStartOffset, networkConnection.recvBuffer.Unused, SocketFlags.None, out socketError, NetworkConnection.recvTlsNegotiationCompleted, lazyAsyncResultWithTimeout);
			}
			catch (ObjectDisposedException)
			{
				socketError = SocketError.Shutdown;
			}
			if (socketError != SocketError.Success && socketError != SocketError.IOPending)
			{
				networkConnection.InvokeSendCallback(lazyAsyncResultWithTimeout, socketError);
			}
		}

		private static void RecvTlsNegotiationCompleted(IAsyncResult asyncResult)
		{
			NetworkConnection.LazyAsyncResultWithTimeout lazyAsyncResultWithTimeout = (NetworkConnection.LazyAsyncResultWithTimeout)asyncResult.AsyncState;
			NetworkConnection networkConnection = (NetworkConnection)lazyAsyncResultWithTimeout.AsyncObject;
			Socket socket = networkConnection.socket;
			int num = 0;
			SocketError socketError;
			if (socket == null)
			{
				socketError = SocketError.Shutdown;
			}
			else
			{
				try
				{
					num = socket.EndReceive(asyncResult, out socketError);
				}
				catch (ObjectDisposedException)
				{
					socketError = SocketError.Shutdown;
				}
			}
			if (num == 0 && socketError == SocketError.Success)
			{
				socketError = SocketError.SocketError;
			}
			if (socketError != SocketError.Success)
			{
				networkConnection.InvokeSendCallback(lazyAsyncResultWithTimeout, socketError);
				return;
			}
			networkConnection.recvBuffer.ReportBytesFilled(num);
			networkConnection.bytesReceived += (long)num;
			SecurityStatus securityStatus = networkConnection.sspiContext.NegotiateSecurityContext(networkConnection.recvBuffer, networkConnection.sendBuffer);
			SecurityStatus securityStatus2 = securityStatus;
			if (securityStatus2 != SecurityStatus.IncompleteMessage)
			{
				if (securityStatus2 != SecurityStatus.OK)
				{
					if (securityStatus2 != SecurityStatus.ContinueNeeded)
					{
						networkConnection.InvokeSendCallback(lazyAsyncResultWithTimeout, securityStatus);
						return;
					}
				}
				else
				{
					securityStatus = networkConnection.sspiContext.CaptureChannelBindingToken(ChannelBindingType.Unique, out networkConnection.channelBindingToken);
					if (securityStatus == SecurityStatus.Unsupported)
					{
						networkConnection.channelBindingToken = null;
						securityStatus = SecurityStatus.OK;
					}
					else if (securityStatus != SecurityStatus.OK)
					{
						networkConnection.InvokeSendCallback(lazyAsyncResultWithTimeout, securityStatus);
						return;
					}
					networkConnection.recvBuffer.ExpandBuffer(networkConnection.sspiContext.MaxStreamSize + 4096);
					networkConnection.sendBuffer.ExpandBuffer(networkConnection.sspiContext.MaxStreamSize);
					if (networkConnection.recvBuffer.Remaining != 0)
					{
						networkConnection.recvBuffer.ShuffleBuffer();
						networkConnection.recvBuffer.EncryptedDataLength = networkConnection.recvBuffer.Remaining;
						networkConnection.recvBuffer.Filled = 0;
						networkConnection.recvBuffer.EncryptedDataOffset = 0;
						securityStatus = networkConnection.sspiContext.DecryptMessage(networkConnection.recvBuffer);
						if (securityStatus == SecurityStatus.IncompleteMessage)
						{
							securityStatus = SecurityStatus.OK;
						}
						else if (securityStatus != SecurityStatus.OK)
						{
							networkConnection.InvokeSendCallback(lazyAsyncResultWithTimeout, securityStatus);
							return;
						}
					}
				}
			}
			if (networkConnection.sendBuffer.Filled != 0)
			{
				try
				{
					socket.BeginSend(networkConnection.sendBuffer.Buffer, networkConnection.sendBuffer.BufferStartOffset, networkConnection.sendBuffer.Filled, SocketFlags.None, out socketError, NetworkConnection.sendTlsNegotiationCompleted, lazyAsyncResultWithTimeout);
					goto IL_262;
				}
				catch (ObjectDisposedException)
				{
					socketError = SocketError.Shutdown;
					goto IL_262;
				}
			}
			if (securityStatus == SecurityStatus.OK)
			{
				networkConnection.InvokeSendCallback(lazyAsyncResultWithTimeout, null);
				return;
			}
			networkConnection.recvBuffer.ShuffleBuffer();
			if (networkConnection.recvBuffer.Unused == 0)
			{
				socketError = SocketError.NoBufferSpaceAvailable;
			}
			else
			{
				try
				{
					socket.BeginReceive(networkConnection.recvBuffer.Buffer, networkConnection.recvBuffer.UnusedStartOffset, networkConnection.recvBuffer.Unused, SocketFlags.None, out socketError, NetworkConnection.recvTlsNegotiationCompleted, lazyAsyncResultWithTimeout);
				}
				catch (ObjectDisposedException)
				{
					socketError = SocketError.Shutdown;
				}
			}
			IL_262:
			if (socketError != SocketError.Success && socketError != SocketError.IOPending)
			{
				networkConnection.InvokeSendCallback(lazyAsyncResultWithTimeout, socketError);
			}
		}

		private void SetNextRecvTimeout(NetworkConnection.LazyAsyncResultWithTimeout asyncResult)
		{
			int hashCode = asyncResult.GetHashCode();
			int num = Interlocked.CompareExchange(ref this.recvAsyncResultHashCode, hashCode, 0);
			if (num != 0)
			{
				throw new InvalidOperationException(NetException.ReceiveInProgress);
			}
			this.recvAsyncResult.Target = asyncResult;
			this.EnterConnection();
		}

		private void InvokeRecvCallback(NetworkConnection.LazyAsyncResultWithTimeout asyncResult, object result)
		{
			int hashCode = asyncResult.GetHashCode();
			int num = Interlocked.CompareExchange(ref this.recvAsyncResultHashCode, 0, hashCode);
			if (hashCode != num)
			{
				throw new InvalidOperationException(NetException.IAsyncResultMismatch);
			}
			if (result != null)
			{
				ExTraceGlobals.NetworkTracer.Information<Type, object>(this.connectionId, "Invoking user receive callback with status {0}:{1}", result.GetType(), result);
			}
			asyncResult.InvokeCallback(result);
			this.LeaveConnection();
		}

		private void SetNextSendTimeout(NetworkConnection.LazyAsyncResultWithTimeout asyncResult)
		{
			int hashCode = asyncResult.GetHashCode();
			int num = Interlocked.CompareExchange(ref this.sendAsyncResultHashCode, hashCode, 0);
			if (num != 0)
			{
				throw new InvalidOperationException(NetException.SendInProgress);
			}
			this.sendAsyncResult.Target = asyncResult;
			this.EnterConnection();
		}

		private void InvokeSendCallback(NetworkConnection.LazyAsyncResultWithTimeout asyncResult, object result)
		{
			int hashCode = asyncResult.GetHashCode();
			int num = Interlocked.CompareExchange(ref this.sendAsyncResultHashCode, 0, hashCode);
			if (hashCode != num)
			{
				throw new InvalidOperationException(NetException.IAsyncResultMismatch);
			}
			if (result != null)
			{
				ExTraceGlobals.NetworkTracer.Information<Type, object>(this.connectionId, "Invoking user send callback with status {0}:{1}", result.GetType(), result);
			}
			asyncResult.InvokeCallback(result);
			this.LeaveConnection();
		}

		public const int MaxSupportedLineLength = 4096;

		public const int DefaultLineLength = 1000;

		public const int DefaultTimeout = 600;

		private const int MaxSupportedTlsBufferLength = 16432;

		private const int SignBit = -2147483648;

		private const int NumberOfBreadcrumbs = 64;

		private const int MaxRecvBufferLength = 69680;

		private static readonly AsyncCallback readDataAvailable = new AsyncCallback(NetworkConnection.ReadDataAvailable);

		private static readonly AsyncCallback internalSendCompleted = new AsyncCallback(NetworkConnection.InternalSendCompleted);

		private static readonly AsyncCallback readLineDataAvailable = new AsyncCallback(NetworkConnection.ReadLineDataAvailable);

		private static readonly AsyncCallback recvTlsNegotiationCompleted = new AsyncCallback(NetworkConnection.RecvTlsNegotiationCompleted);

		private static readonly AsyncCallback sendTlsNegotiationCompleted = new AsyncCallback(NetworkConnection.SendTlsNegotiationCompleted);

		private static readonly AsyncCallback sendStreamCompleted = new AsyncCallback(NetworkConnection.SendStreamCompleted);

		private static readonly WaitCallback timeoutOccurredCallback = new WaitCallback(NetworkConnection.TimeoutOccurred);

		private static readonly IPEndPoint nullEndPoint = new IPEndPoint(0L, 0);

		private readonly long connectionId;

		private static long lastConnectionId;

		private Socket socket;

		private int maxLineLength = 1000;

		private long bytesReceived;

		private long bytesSent;

		private long sendTimeoutPeriod = 6000000000L;

		private long recvTimeoutPeriod = 6000000000L;

		private readonly NetworkBuffer recvBuffer;

		private NetworkBuffer sendBuffer;

		private readonly WeakReference recvAsyncResult = new WeakReference(null);

		private readonly WeakReference sendAsyncResult = new WeakReference(null);

		private int recvAsyncResultHashCode;

		private int sendAsyncResultHashCode;

		private SspiContext sspiContext;

		private ChannelBindingToken channelBindingToken;

		protected readonly Breadcrumbs<NetworkConnection.Breadcrumb> breadcrumbs = new Breadcrumbs<NetworkConnection.Breadcrumb>(64);

		private IX509Certificate2 localCertificate;

		private IX509Certificate2 remoteCertificate;

		private readonly IPEndPoint remoteEndPoint;

		private readonly IPEndPoint localEndPoint;

		private int referenceCount = 1;

		internal class LazyAsyncResultWithTimeout : LazyAsyncResult
		{
			internal LazyAsyncResultWithTimeout(long timeoutPeriod, object workerObject, object callerState, AsyncCallback callback) : base(workerObject, callerState, callback)
			{
				this.UpdateTimeout(timeoutPeriod);
			}

			internal static NetworkConnection.LazyAsyncResultWithTimeout EndAsyncOperation(IAsyncResult asyncResult)
			{
				return LazyAsyncResult.EndAsyncOperation<NetworkConnection.LazyAsyncResultWithTimeout>(asyncResult);
			}

			internal byte[] Buffer
			{
				[DebuggerStepThrough]
				get
				{
					return this.buffer;
				}
				[DebuggerStepThrough]
				set
				{
					this.buffer = value;
				}
			}

			internal int Offset
			{
				[DebuggerStepThrough]
				get
				{
					return this.offset;
				}
				[DebuggerStepThrough]
				set
				{
					this.offset = value;
				}
			}

			internal int Size
			{
				[DebuggerStepThrough]
				get
				{
					return this.size;
				}
				[DebuggerStepThrough]
				set
				{
					this.size = value;
				}
			}

			internal bool TimedOut(long now)
			{
				return this.timeout < now && !base.InternalPeekCompleted;
			}

			internal void UpdateTimeout(long timeoutPeriod)
			{
				this.timeout = DateTime.UtcNow.Ticks + timeoutPeriod;
			}

			public override int GetHashCode()
			{
				int hashCode = base.GetHashCode();
				if (hashCode != 0)
				{
					return hashCode;
				}
				return 1;
			}

			private long timeout;

			private byte[] buffer;

			private int offset;

			private int size;
		}

		internal sealed class WriteStreamAsyncResult : NetworkConnection.LazyAsyncResultWithTimeout
		{
			internal WriteStreamAsyncResult(long timeoutPeriod, Stream stream, object workerObject, object callerState, AsyncCallback callback) : base(timeoutPeriod, workerObject, callerState, callback)
			{
				this.stream = stream;
			}

			internal Stream Stream
			{
				[DebuggerStepThrough]
				get
				{
					return this.stream;
				}
			}

			private readonly Stream stream;
		}

		protected enum Breadcrumb : byte
		{
			None,
			Dispose = 2,
			Shutdown,
			BeginRead = 16,
			EndRead,
			ReadDataAvailable,
			ReadDataAvailableError,
			BeginReadLine,
			EndReadLine,
			ReadLineDataAvailable,
			ReadLineDataAvailableError,
			RecvTimeout,
			PutBackReceivedBytes,
			BeginWrite = 32,
			BeginWriteStream,
			EndWrite,
			SendTimeout,
			StreamBeginReadException,
			BeginNegotiateTlsAsClient = 48,
			EndNegotiateTlsAsClient,
			BeginNegotiateTlsAsServer,
			EndNegotiateTlsAsServer,
			AsyncOperationCancelled
		}
	}
}
