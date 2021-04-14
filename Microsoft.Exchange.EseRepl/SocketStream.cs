using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using Microsoft.Exchange.Cluster.Common;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.EseRepl
{
	internal class SocketStream : Stream
	{
		public static Trace Tracer
		{
			get
			{
				return ExTraceGlobals.TcpChannelTracer;
			}
		}

		public Socket Socket
		{
			get
			{
				return this.m_StreamSocket;
			}
		}

		public SocketStream(Socket socket, ISimpleBufferPool bufPool, IPool<SocketStreamAsyncArgs> asyncPool, SocketStream.ISocketStreamPerfCounters perfCtrs)
		{
			this.m_StreamSocket = socket;
			this.m_bufPool = bufPool;
			this.m_asyncArgsPool = asyncPool;
			this.m_perfCounters = perfCtrs;
		}

		protected override void Dispose(bool disposing)
		{
			bool cleanedUp = this.m_CleanedUp;
			this.m_CleanedUp = true;
			if (!cleanedUp && disposing && this.m_StreamSocket != null)
			{
				Socket streamSocket = this.m_StreamSocket;
				if (streamSocket != null)
				{
					Exception ex = null;
					try
					{
						streamSocket.Shutdown(SocketShutdown.Both);
					}
					catch (SocketException ex2)
					{
						ex = ex2;
					}
					catch (ObjectDisposedException ex3)
					{
						ex = ex3;
					}
					if (ex != null)
					{
						SocketStream.Tracer.TraceError<Exception>((long)this.GetHashCode(), "Dispose.Shutdown got exception: {0}", ex);
					}
					streamSocket.Close(0);
				}
			}
			base.Dispose(disposing);
			this.ReleaseInternalIoResources();
		}

		private void ReleaseInternalIoResources()
		{
			int num = Interlocked.Exchange(ref this.m_readIsBusy, 1);
			if (num != 0)
			{
				int num2 = 10;
				int num3 = num2 * 10;
				do
				{
					Thread.Sleep(100);
					num = Interlocked.Exchange(ref this.m_readIsBusy, 1);
				}
				while (num != 0 && --num3 > 0);
				if (num != 0)
				{
					this.m_buffersWereLeaked = true;
					SocketStream.Tracer.TraceError<bool>((long)this.GetHashCode(), "buffer leak: {0}", this.m_buffersWereLeaked);
					ReplayCrimsonEvents.SocketStreamLeakOnClose.LogPeriodic<int>(Environment.MachineName, Parameters.CurrentValues.DefaultEventSuppressionInterval, num2);
					return;
				}
			}
			if (this.m_readArgs != null)
			{
				SocketStreamAsyncArgs readArgs = this.m_readArgs;
				this.m_readArgs = null;
				this.ReleaseArgsToPools(readArgs);
			}
			this.m_bufIsAllocated = false;
			Interlocked.Exchange(ref this.m_readIsBusy, 0);
		}

		public override void Flush()
		{
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException();
		}

		public override void SetLength(long value)
		{
			throw new NotSupportedException();
		}

		public override int ReadTimeout
		{
			get
			{
				int num = (int)this.m_StreamSocket.GetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout);
				if (num == 0)
				{
					return -1;
				}
				return num;
			}
			set
			{
				if (value <= 0 && value != -1)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this.SetSocketTimeoutOption(SocketShutdown.Receive, value);
			}
		}

		public override int WriteTimeout
		{
			get
			{
				int num = (int)this.m_StreamSocket.GetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout);
				if (num == 0)
				{
					return -1;
				}
				return num;
			}
			set
			{
				if (value <= 0 && value != -1)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this.SetSocketTimeoutOption(SocketShutdown.Send, value);
			}
		}

		internal void SetSocketTimeoutOption(SocketShutdown mode, int timeout)
		{
			if (timeout < 0)
			{
				timeout = 0;
			}
			Socket streamSocket = this.m_StreamSocket;
			if (streamSocket == null)
			{
				return;
			}
			if ((mode == SocketShutdown.Send || mode == SocketShutdown.Both) && timeout != this.m_CurrentWriteTimeout)
			{
				streamSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, timeout);
				this.m_CurrentWriteTimeout = timeout;
			}
			if ((mode == SocketShutdown.Receive || mode == SocketShutdown.Both) && timeout != this.m_CurrentReadTimeout)
			{
				streamSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, timeout);
				this.m_CurrentReadTimeout = timeout;
			}
		}

		public override int Read(byte[] buffer, int offset, int size)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (offset < 0 || offset > buffer.Length)
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			if (size < 0 || size > buffer.Length - offset)
			{
				throw new ArgumentOutOfRangeException("size");
			}
			Socket socket = this.Socket;
			if (socket == null)
			{
				throw new IOException("no socket");
			}
			int result;
			try
			{
				int num = socket.Receive(buffer, offset, size, SocketFlags.None);
				result = num;
			}
			catch (Exception ex)
			{
				if (ex is ThreadAbortException || ex is StackOverflowException || ex is OutOfMemoryException)
				{
					throw;
				}
				throw new IOException(string.Format("net_io_readfailure: {0}", ex.Message), ex);
			}
			return result;
		}

		public override IAsyncResult BeginRead(byte[] buffer, int offset, int size, AsyncCallback callback, object state)
		{
			if (!this.CanRead)
			{
				throw new InvalidOperationException("net_writeonlystream");
			}
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (offset < 0 || offset > buffer.Length)
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			if (size < 0 || size > buffer.Length - offset)
			{
				throw new ArgumentOutOfRangeException("size");
			}
			Socket streamSocket = this.m_StreamSocket;
			if (streamSocket == null)
			{
				throw new IOException("net_io_readfailure");
			}
			SocketStreamAsyncArgs socketStreamAsyncArgs = this.ObtainArgsForStartRead(size);
			if (socketStreamAsyncArgs != null)
			{
				return this.StartInternalRead(streamSocket, socketStreamAsyncArgs, buffer, offset, size, callback, state);
			}
			return streamSocket.BeginReceive(buffer, offset, size, SocketFlags.None, callback, state);
		}

		private SocketStreamAsyncArgs ObtainArgsForStartRead(int readSize)
		{
			SocketStreamAsyncArgs socketStreamAsyncArgs = null;
			if (this.m_asyncArgsPool == null)
			{
				return null;
			}
			int num = Interlocked.Exchange(ref this.m_readIsBusy, 1);
			if (num == 1)
			{
				return null;
			}
			try
			{
				if (!this.m_bufIsAllocated)
				{
					socketStreamAsyncArgs = this.ObtainInternalBuffer(readSize);
					if (socketStreamAsyncArgs == null)
					{
						return null;
					}
				}
				socketStreamAsyncArgs = this.m_readArgs;
				if (readSize > socketStreamAsyncArgs.InternalBuffer.Buffer.Length)
				{
					socketStreamAsyncArgs = null;
				}
			}
			finally
			{
				if (socketStreamAsyncArgs == null)
				{
					Interlocked.Exchange(ref this.m_readIsBusy, 0);
				}
			}
			return socketStreamAsyncArgs;
		}

		private SocketStreamAsyncArgs ObtainInternalBuffer(int readSize)
		{
			SocketStreamAsyncArgs socketStreamAsyncArgs = null;
			SimpleBuffer simpleBuffer = null;
			bool flag = false;
			try
			{
				simpleBuffer = this.m_bufPool.TryGetObject(readSize);
				if (simpleBuffer == null)
				{
					if (readSize <= this.m_bufPool.BufferSize)
					{
						ReplayCrimsonEvents.SocketStreamBufferExhaustion.LogPeriodic(Environment.MachineName, Parameters.CurrentValues.DefaultEventSuppressionInterval);
					}
					return null;
				}
				socketStreamAsyncArgs = this.m_asyncArgsPool.TryGetObject();
				if (socketStreamAsyncArgs == null)
				{
					this.m_extraArgCount++;
					socketStreamAsyncArgs = new SocketStreamAsyncArgs(false);
				}
				this.BindArgsForUse(simpleBuffer, socketStreamAsyncArgs);
				this.m_readArgs = socketStreamAsyncArgs;
				this.m_bufIsAllocated = true;
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					if (socketStreamAsyncArgs != null)
					{
						this.ReleaseArgsToPools(socketStreamAsyncArgs);
						socketStreamAsyncArgs = null;
					}
					else if (simpleBuffer != null)
					{
						this.m_bufPool.TryReturnObject(simpleBuffer);
						simpleBuffer = null;
					}
				}
			}
			return socketStreamAsyncArgs;
		}

		private void BindArgsForUse(SimpleBuffer buf, SocketStreamAsyncArgs args)
		{
			args.InternalBuffer = buf;
			args.CompletionRtn = new EventHandler<SocketAsyncEventArgs>(this.IO_Completed);
			args.Completed += args.CompletionRtn;
			args.SetBuffer(buf.Buffer, 0, buf.Buffer.Length);
		}

		private void ReleaseArgsToPools(SocketStreamAsyncArgs args)
		{
			SimpleBuffer internalBuffer = args.InternalBuffer;
			args.Completed -= args.CompletionRtn;
			args.CompletionRtn = null;
			args.InternalBuffer = null;
			if (internalBuffer != null)
			{
				this.m_bufPool.TryReturnObject(internalBuffer);
			}
			if (!this.m_asyncArgsPool.TryReturnObject(args))
			{
				args.Dispose();
			}
		}

		private IAsyncResult StartInternalRead(Socket streamSocket, SocketStreamAsyncArgs readArgs, byte[] buffer, int offset, int size, AsyncCallback callback, object state)
		{
			IAsyncResult asyncResult;
			try
			{
				SocketStream.IoReq ioReq = new SocketStream.IoReq(state, callback);
				ioReq.RecordUserBuffer(buffer, offset, size);
				ioReq.InternalBuffer = readArgs.InternalBuffer;
				readArgs.SetBuffer(ioReq.InternalBuffer.Buffer, 0, size);
				readArgs.UserToken = ioReq;
				SocketStream.Tracer.TraceDebug((long)this.GetHashCode(), "ReceiveAsync called");
				if (!streamSocket.ReceiveAsync(readArgs))
				{
					SocketStream.Tracer.TraceDebug((long)this.GetHashCode(), "ReceiveAsync sync completion");
					this.IO_Completed(null, readArgs);
				}
				asyncResult = ioReq.AsyncResult;
			}
			catch (Exception ex)
			{
				if (ex is ThreadAbortException || ex is StackOverflowException || ex is OutOfMemoryException)
				{
					throw;
				}
				throw new IOException(ex.Message, ex);
			}
			return asyncResult;
		}

		private void IO_Completed(object sender, SocketAsyncEventArgs e)
		{
			SocketAsyncOperation lastOperation = e.LastOperation;
			if (lastOperation == SocketAsyncOperation.Receive)
			{
				this.ProcessReceive(e);
				return;
			}
			if (lastOperation != SocketAsyncOperation.Send)
			{
				throw new ArgumentException("The last operation completed on the socket was not a receive or send");
			}
		}

		private void ProcessReceive(SocketAsyncEventArgs e)
		{
			SocketStream.IoReq ioReq = e.UserToken as SocketStream.IoReq;
			if (ioReq == null)
			{
				throw new ArgumentException("UserToken must be ReadReq");
			}
			try
			{
				ioReq.SocketError = e.SocketError;
				ioReq.BytesTransferred = e.BytesTransferred;
				Array.Copy(ioReq.InternalBuffer.Buffer, 0, ioReq.UserBuffer, ioReq.UserOffset, ioReq.BytesTransferred);
				ioReq.InternalBuffer = null;
				e.SetBuffer(null, 0, 0);
			}
			finally
			{
				SocketStream.Tracer.TraceDebug((long)this.GetHashCode(), "ProcessReceive marks read idle");
				Interlocked.Exchange(ref this.m_readIsBusy, 0);
				ioReq.Completed = true;
			}
			ioReq.AsyncResult.InvokeCallback();
		}

		public override int EndRead(IAsyncResult asyncResult)
		{
			if (asyncResult == null)
			{
				throw new ArgumentNullException("asyncResult");
			}
			LazyAsyncResult lazyAsyncResult = asyncResult as LazyAsyncResult;
			if (lazyAsyncResult != null)
			{
				return this.EndReadFromInternalBuf(lazyAsyncResult);
			}
			Socket streamSocket = this.m_StreamSocket;
			if (streamSocket == null)
			{
				throw new IOException("EndRead with no socket");
			}
			return streamSocket.EndReceive(asyncResult);
		}

		private int EndReadFromInternalBuf(LazyAsyncResult result)
		{
			SocketStream.IoReq ioReq = result.AsyncObject as SocketStream.IoReq;
			if (ioReq == null)
			{
				throw new ArgumentException("AsyncObject corrupt");
			}
			result.InternalWaitForCompletion();
			if (ioReq.SocketError != SocketError.Success)
			{
				throw new SocketException((int)ioReq.SocketError);
			}
			return ioReq.BytesTransferred;
		}

		public override void Write(byte[] buffer, int offset, int size)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (offset < 0 || offset > buffer.Length)
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			if (size < 0 || size > buffer.Length - offset)
			{
				throw new ArgumentOutOfRangeException("size");
			}
			Socket socket = this.Socket;
			if (socket == null)
			{
				throw new IOException("net_io_writefailure:nosocket");
			}
			try
			{
				StopwatchStamp stamp = StopwatchStamp.GetStamp();
				socket.Send(buffer, offset, size, SocketFlags.None);
				if (this.m_perfCounters != null)
				{
					this.m_perfCounters.RecordWriteLatency(stamp.ElapsedTicks);
				}
			}
			catch (Exception ex)
			{
				if (ex is ThreadAbortException || ex is StackOverflowException || ex is OutOfMemoryException)
				{
					throw;
				}
				throw new IOException(string.Format("net_io_writefailure: {0}", ex.Message), ex);
			}
		}

		public override bool CanRead
		{
			get
			{
				return this.Socket.Connected;
			}
		}

		public override bool CanSeek
		{
			get
			{
				return false;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return this.Socket.Connected;
			}
		}

		public override long Length
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public override long Position
		{
			get
			{
				throw new NotSupportedException();
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		private Socket m_StreamSocket;

		private int m_CurrentReadTimeout = -1;

		private int m_CurrentWriteTimeout = -1;

		private bool m_CleanedUp;

		private bool m_buffersWereLeaked;

		private ISimpleBufferPool m_bufPool;

		private IPool<SocketStreamAsyncArgs> m_asyncArgsPool;

		private SocketStream.ISocketStreamPerfCounters m_perfCounters;

		private int m_readIsBusy;

		private bool m_bufIsAllocated;

		private int m_extraArgCount;

		private SocketStreamAsyncArgs m_readArgs;

		public interface ISocketStreamPerfCounters
		{
			void RecordWriteLatency(long tics);
		}

		private class IoReq
		{
			public IoReq(object callerState, AsyncCallback callerCallback)
			{
				this.AsyncResult = new LazyAsyncResult(this, callerState, callerCallback);
			}

			public LazyAsyncResult AsyncResult { get; set; }

			public SocketError SocketError { get; set; }

			public int BytesTransferred { get; set; }

			public SimpleBuffer InternalBuffer { get; set; }

			public byte[] UserBuffer { get; set; }

			public int UserOffset { get; set; }

			public int UserSize { get; set; }

			public bool Completed { get; set; }

			public void RecordUserBuffer(byte[] buffer, int offset, int size)
			{
				this.UserBuffer = buffer;
				this.UserOffset = offset;
				this.UserSize = size;
			}
		}
	}
}
