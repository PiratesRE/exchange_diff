using System;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using Microsoft.Exchange.Diagnostics.Components.Net;

namespace Microsoft.Exchange.Net
{
	internal class NetworkConnectionStream : Stream
	{
		internal NetworkConnectionStream(Socket socket) : this(new NetworkConnection(socket, 4096))
		{
		}

		internal NetworkConnectionStream(NetworkConnection nc)
		{
			if (nc == null)
			{
				throw new ArgumentNullException("nc");
			}
			this.networkConnection = nc;
		}

		public override bool CanRead
		{
			[DebuggerStepThrough]
			get
			{
				return true;
			}
		}

		public override bool CanSeek
		{
			[DebuggerStepThrough]
			get
			{
				return false;
			}
		}

		public override bool CanTimeout
		{
			[DebuggerStepThrough]
			get
			{
				return true;
			}
		}

		public override bool CanWrite
		{
			[DebuggerStepThrough]
			get
			{
				return true;
			}
		}

		public override long Length
		{
			[DebuggerStepThrough]
			get
			{
				ExTraceGlobals.NetworkTracer.TraceError(this.networkConnection.ConnectionId, "GetLength: Not supported");
				throw new NotSupportedException();
			}
		}

		public override long Position
		{
			[DebuggerStepThrough]
			get
			{
				ExTraceGlobals.NetworkTracer.TraceError(this.networkConnection.ConnectionId, "GetPosition: Not supported");
				throw new NotSupportedException();
			}
			[DebuggerStepThrough]
			set
			{
				ExTraceGlobals.NetworkTracer.TraceError(this.networkConnection.ConnectionId, "SetPosition: Not supported");
				throw new NotSupportedException();
			}
		}

		public override int ReadTimeout
		{
			[DebuggerStepThrough]
			get
			{
				return this.networkConnection.ReceiveTimeout * 1000;
			}
			[DebuggerStepThrough]
			set
			{
				this.networkConnection.ReceiveTimeout = value / 1000;
			}
		}

		public override int WriteTimeout
		{
			[DebuggerStepThrough]
			get
			{
				return this.networkConnection.SendTimeout * 1000;
			}
			[DebuggerStepThrough]
			set
			{
				this.networkConnection.SendTimeout = value / 1000;
			}
		}

		public NetworkConnection NetworkConnection
		{
			[DebuggerStepThrough]
			get
			{
				return this.networkConnection;
			}
		}

		public override void Flush()
		{
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			ExTraceGlobals.NetworkTracer.TraceError(this.networkConnection.ConnectionId, "Seek: Not supported");
			throw new NotSupportedException();
		}

		public override void SetLength(long value)
		{
			ExTraceGlobals.NetworkTracer.TraceError(this.networkConnection.ConnectionId, "SetLength: Not supported");
			throw new NotSupportedException();
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			ExTraceGlobals.NetworkTracer.TraceDebug<int>(this.networkConnection.ConnectionId, "Write: {0} bytes", count);
			object errorCode;
			this.networkConnection.Write(buffer, offset, count, out errorCode);
			this.ThrowOnError(NetworkConnectionStream.Function.Write, errorCode);
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			ExTraceGlobals.NetworkTracer.TraceDebug<int>(this.networkConnection.ConnectionId, "Read: requested {0} bytes", count);
			byte[] src;
			int srcOffset;
			int num;
			object errorCode;
			this.networkConnection.Read(out src, out srcOffset, out num, out errorCode);
			this.ThrowOnError(NetworkConnectionStream.Function.Read, errorCode);
			int num2 = Math.Min(count, num);
			if (num2 != 0)
			{
				Buffer.BlockCopy(src, srcOffset, buffer, offset, num2);
			}
			this.networkConnection.PutBackReceivedBytes(num - num2);
			ExTraceGlobals.NetworkTracer.TraceDebug<int>(this.networkConnection.ConnectionId, "Read: {0} bytes returned", num2);
			return num2;
		}

		public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
		{
			ExTraceGlobals.NetworkTracer.TraceDebug<int>(this.networkConnection.ConnectionId, "BeginRead: requested {0} bytes", count);
			NetworkConnectionStream.AsyncResult asyncResult = new NetworkConnectionStream.AsyncResult(new ArraySegment<byte>(buffer, offset, count), callback, state);
			this.networkConnection.BeginRead(new AsyncCallback(this.ReadComplete), asyncResult);
			return asyncResult;
		}

		public override int EndRead(IAsyncResult asyncResult)
		{
			NetworkConnectionStream.AsyncResult asyncResult2 = NetworkConnectionStream.AsyncResult.EndAsyncOperation(asyncResult);
			if (asyncResult2.Result is ArraySegment<byte>)
			{
				ArraySegment<byte> arraySegment = (ArraySegment<byte>)asyncResult2.Result;
				ArraySegment<byte> arraySegment2 = (ArraySegment<byte>)asyncResult2.AsyncObject;
				int num = Math.Min(arraySegment2.Count, arraySegment.Count);
				if (num != 0)
				{
					Buffer.BlockCopy(arraySegment.Array, arraySegment.Offset, arraySegment2.Array, arraySegment2.Offset, num);
				}
				this.networkConnection.PutBackReceivedBytes(arraySegment.Count - num);
				ExTraceGlobals.NetworkTracer.TraceDebug<int>(this.networkConnection.ConnectionId, "EndRead: {0} bytes returned", num);
				return num;
			}
			this.ThrowOnError(NetworkConnectionStream.Function.EndRead, asyncResult2.Result);
			throw new InvalidOperationException();
		}

		public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
		{
			ExTraceGlobals.NetworkTracer.TraceDebug<int>(this.networkConnection.ConnectionId, "BeginWrite: {0} bytes", count);
			NetworkConnectionStream.AsyncResult asyncResult = new NetworkConnectionStream.AsyncResult(null, callback, state);
			this.networkConnection.BeginWrite(buffer, offset, count, new AsyncCallback(this.WriteComplete), asyncResult);
			return asyncResult;
		}

		public override void EndWrite(IAsyncResult asyncResult)
		{
			ExTraceGlobals.NetworkTracer.TraceDebug(this.networkConnection.ConnectionId, "EndWrite");
			NetworkConnectionStream.AsyncResult asyncResult2 = NetworkConnectionStream.AsyncResult.EndAsyncOperation(asyncResult);
			this.ThrowOnError(NetworkConnectionStream.Function.EndWrite, asyncResult2.Result);
		}

		public override string ToString()
		{
			return string.Format("NetworkConnectionChannel: {0}", this.networkConnection.ConnectionId);
		}

		protected override void Dispose(bool calledFromDispose)
		{
			if (calledFromDispose)
			{
				this.networkConnection.Dispose();
			}
			base.Dispose(calledFromDispose);
		}

		private void ReadComplete(IAsyncResult iasyncResult)
		{
			NetworkConnectionStream.AsyncResult asyncResult = (NetworkConnectionStream.AsyncResult)iasyncResult.AsyncState;
			byte[] array;
			int offset;
			int count;
			object obj;
			this.networkConnection.EndRead(iasyncResult, out array, out offset, out count, out obj);
			if (obj == null)
			{
				asyncResult.InvokeCallback(new ArraySegment<byte>(array, offset, count));
				return;
			}
			asyncResult.InvokeCallback(obj);
		}

		private void WriteComplete(IAsyncResult iasyncResult)
		{
			NetworkConnectionStream.AsyncResult asyncResult = (NetworkConnectionStream.AsyncResult)iasyncResult.AsyncState;
			object value;
			this.networkConnection.EndWrite(iasyncResult, out value);
			asyncResult.InvokeCallback(value);
		}

		private void ThrowOnError(NetworkConnectionStream.Function function, object errorCode)
		{
			if (errorCode == null)
			{
				return;
			}
			ExTraceGlobals.NetworkTracer.TraceDebug<NetworkConnectionStream.Function, object>(this.networkConnection.ConnectionId, "{0}: Error: {1}", function, errorCode);
			if (errorCode is SocketError)
			{
				throw new IOException(function.ToString(), new SocketException((int)errorCode));
			}
			throw new IOException(string.Format("Unknown error in function {0}: {1}", function, errorCode));
		}

		private const int MillisecondsPerSecond = 1000;

		private NetworkConnection networkConnection;

		private enum Function
		{
			Read,
			EndRead,
			Write,
			EndWrite
		}

		private class AsyncResult : LazyAsyncResult
		{
			internal AsyncResult(object worker, AsyncCallback callback, object state) : base(worker, state, callback)
			{
			}

			internal static NetworkConnectionStream.AsyncResult EndAsyncOperation(IAsyncResult asyncResult)
			{
				return LazyAsyncResult.EndAsyncOperation<NetworkConnectionStream.AsyncResult>(asyncResult);
			}
		}
	}
}
