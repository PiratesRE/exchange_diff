using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.Diagnostics.Components.Search;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Search.Core.Diagnostics;

namespace Microsoft.Exchange.Search.Core.Common
{
	internal class StreamChannel : Stream
	{
		internal StreamChannel(Socket socket)
		{
			this.diagnosticsSession = DiagnosticsSession.CreateComponentDiagnosticsSession("StreamChannel", ComponentInstance.Globals.Search.ServiceName, ExTraceGlobals.StreamChannelTracer, (long)this.GetHashCode());
			this.networkStream = new NetworkConnectionStream(socket);
			this.stream = this.networkStream;
		}

		protected StreamChannel()
		{
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
				this.diagnosticsSession.TraceError("GetLength: Not supported", new object[0]);
				throw new NotSupportedException();
			}
		}

		public override long Position
		{
			[DebuggerStepThrough]
			get
			{
				this.diagnosticsSession.TraceError("GetPosition: Not supported", new object[0]);
				throw new NotSupportedException();
			}
			[DebuggerStepThrough]
			set
			{
				this.diagnosticsSession.TraceError("SetPosition: Not supported", new object[0]);
				throw new NotSupportedException();
			}
		}

		public override int WriteTimeout
		{
			[DebuggerStepThrough]
			get
			{
				return this.networkStream.WriteTimeout;
			}
			[DebuggerStepThrough]
			set
			{
				this.networkStream.WriteTimeout = value;
			}
		}

		public override int ReadTimeout
		{
			[DebuggerStepThrough]
			get
			{
				return this.networkStream.ReadTimeout;
			}
			[DebuggerStepThrough]
			set
			{
				this.networkStream.ReadTimeout = value;
			}
		}

		public Guid ContextId
		{
			[DebuggerStepThrough]
			get
			{
				return this.contextId;
			}
			[DebuggerStepThrough]
			set
			{
				this.contextId = value;
			}
		}

		internal IPEndPoint LocalEndPoint
		{
			[DebuggerStepThrough]
			get
			{
				return this.networkStream.NetworkConnection.LocalEndPoint;
			}
		}

		internal IPEndPoint RemoteEndPoint
		{
			[DebuggerStepThrough]
			get
			{
				return this.networkStream.NetworkConnection.RemoteEndPoint;
			}
		}

		public override void Flush()
		{
			this.diagnosticsSession.TraceDebug("Flush", new object[0]);
			this.WritePacketHeader(0);
			this.stream.Flush();
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			this.diagnosticsSession.TraceError("Seek: Not supported", new object[0]);
			throw new NotSupportedException();
		}

		public override void SetLength(long value)
		{
			this.diagnosticsSession.TraceError("SetLength: Not supported", new object[0]);
			throw new NotSupportedException();
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			this.diagnosticsSession.TraceDebug<int>("Write: {0} bytes", count);
			StreamChannel.AsyncResult asyncResult = (StreamChannel.AsyncResult)this.BeginWrite(buffer, offset, count, null, null);
			try
			{
				this.EndWrite(asyncResult);
			}
			finally
			{
				asyncResult.InternalCleanup();
			}
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			this.diagnosticsSession.TraceDebug<int>("Read: requested={0}", count);
			StreamChannel.AsyncResult asyncResult = (StreamChannel.AsyncResult)this.BeginRead(buffer, offset, count, null, null);
			int num = 0;
			try
			{
				num = this.EndRead(asyncResult);
			}
			finally
			{
				asyncResult.InternalCleanup();
			}
			this.diagnosticsSession.TraceDebug<int, int>("Read: {0} bytes, {1} remaining in packet", num, this.bytesRemainingInPacket);
			return num;
		}

		public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
		{
			this.diagnosticsSession.TraceDebug<int>("BeginRead: requested={0}", count);
			StreamChannel.AsyncResult asyncResult = new StreamChannel.AsyncResult(buffer, offset, count, callback, state);
			if (this.bytesRemainingInPacket == 0)
			{
				this.bytesRemainingInPacket = 4;
				this.AsyncReadHeaderBytes(asyncResult);
			}
			else
			{
				this.AsyncReadData(asyncResult);
			}
			return asyncResult;
		}

		public override int EndRead(IAsyncResult asyncResult)
		{
			this.diagnosticsSession.TraceDebug("EndRead", new object[0]);
			StreamChannel.AsyncResult asyncResult2 = StreamChannel.AsyncResult.EndAsyncOperation(asyncResult);
			Exception ex = asyncResult2.Result as Exception;
			if (ex != null)
			{
				this.diagnosticsSession.TraceError<Exception>("EndRead exception: {0}", ex);
				throw ex;
			}
			int num = (int)asyncResult2.Result;
			this.diagnosticsSession.TraceDebug<int, int>("EndRead: {0} bytes, {1} remaining in packet", num, this.bytesRemainingInPacket);
			return num;
		}

		public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
		{
			this.diagnosticsSession.TraceDebug<int>("BeginWrite: {0} bytes", count);
			if (count <= 0 || count > 1048576)
			{
				throw new ArgumentException("count");
			}
			StreamChannel.AsyncResult asyncResult = new StreamChannel.AsyncResult(buffer, offset, count, callback, state);
			ExBitConverter.Write(count, this.packetLengthBuffer, 0);
			this.stream.BeginWrite(this.packetLengthBuffer, 0, this.packetLengthBuffer.Length, new AsyncCallback(this.WritePacketHeaderComplete), asyncResult);
			return asyncResult;
		}

		public override void EndWrite(IAsyncResult asyncResult)
		{
			this.diagnosticsSession.TraceDebug("EndWrite", new object[0]);
			StreamChannel.AsyncResult asyncResult2 = StreamChannel.AsyncResult.EndAsyncOperation(asyncResult);
			Exception ex = asyncResult2.Result as Exception;
			if (ex != null)
			{
				this.diagnosticsSession.TraceError<Exception>("EndWrite exception: {0}", ex);
				throw ex;
			}
		}

		public override string ToString()
		{
			return string.Format("StreamChannel {0} Local: {1} Remote: {2}", this.ContextId, this.LocalEndPoint, this.RemoteEndPoint);
		}

		protected override void Dispose(bool calledFromDispose)
		{
			if (calledFromDispose && this.stream != null)
			{
				this.stream.Dispose();
			}
			base.Dispose(calledFromDispose);
		}

		private void WritePacketHeader(int count)
		{
			ExBitConverter.Write(count, this.packetLengthBuffer, 0);
			this.stream.Write(this.packetLengthBuffer, 0, this.packetLengthBuffer.Length);
		}

		private void AsyncReadHeaderBytes(StreamChannel.AsyncResult readAsyncResult)
		{
			this.stream.BeginRead(this.packetLengthBuffer, 4 - this.bytesRemainingInPacket, this.bytesRemainingInPacket, new AsyncCallback(this.ReadHeaderBytesComplete), readAsyncResult);
		}

		private void ReadHeaderBytesComplete(IAsyncResult readHeaderAsyncResult)
		{
			StreamChannel.AsyncResult asyncResult = (StreamChannel.AsyncResult)readHeaderAsyncResult.AsyncState;
			try
			{
				int num = this.stream.EndRead(readHeaderAsyncResult);
				if (num == 0)
				{
					asyncResult.InvokeCallback(new EndOfStreamException());
				}
				else
				{
					this.bytesRemainingInPacket -= num;
					if (this.bytesRemainingInPacket > 0)
					{
						this.AsyncReadHeaderBytes(asyncResult);
					}
					else
					{
						this.bytesRemainingInPacket = BitConverter.ToInt32(this.packetLengthBuffer, 0);
						if (this.bytesRemainingInPacket < 0 || this.bytesRemainingInPacket > 1048576)
						{
							throw new InvalidDataException("bytesRemainingInPacket");
						}
						if (this.bytesRemainingInPacket == 0)
						{
							asyncResult.InvokeCallback(0);
						}
						else
						{
							this.AsyncReadData(asyncResult);
						}
					}
				}
			}
			catch (Exception ex)
			{
				this.diagnosticsSession.TraceError<Exception>("ReadHeaderBytesComplete exception: {0}", ex);
				asyncResult.InvokeCallback(ex);
			}
		}

		private void AsyncReadData(StreamChannel.AsyncResult readAsyncResult)
		{
			this.stream.BeginRead(readAsyncResult.Buffer, readAsyncResult.Offset, Math.Min(this.bytesRemainingInPacket, readAsyncResult.Count), new AsyncCallback(this.ReadDataComplete), readAsyncResult);
		}

		private void ReadDataComplete(IAsyncResult asyncResult)
		{
			StreamChannel.AsyncResult asyncResult2 = (StreamChannel.AsyncResult)asyncResult.AsyncState;
			try
			{
				int num = this.stream.EndRead(asyncResult);
				if (num == 0)
				{
					asyncResult2.InvokeCallback(new EndOfStreamException());
				}
				else
				{
					this.bytesRemainingInPacket -= num;
					asyncResult2.Offset += num;
					asyncResult2.Count -= num;
					if (this.bytesRemainingInPacket == 0 || asyncResult2.Count == 0)
					{
						asyncResult2.InvokeCallback(asyncResult2.BytesRequested - asyncResult2.Count);
					}
					else
					{
						this.AsyncReadData(asyncResult2);
					}
				}
			}
			catch (Exception ex)
			{
				this.diagnosticsSession.TraceError<Exception>("ReadDataComplete exception: {0}", ex);
				asyncResult2.InvokeCallback(ex);
			}
		}

		private void WritePacketHeaderComplete(IAsyncResult asyncResult)
		{
			StreamChannel.AsyncResult asyncResult2 = (StreamChannel.AsyncResult)asyncResult.AsyncState;
			try
			{
				this.stream.EndWrite(asyncResult);
				this.stream.BeginWrite(asyncResult2.Buffer, asyncResult2.Offset, asyncResult2.Count, new AsyncCallback(this.WriteDataComplete), asyncResult2);
			}
			catch (Exception ex)
			{
				this.diagnosticsSession.TraceError<Exception>("WritePacketHeaderComplete exception: {0}", ex);
				asyncResult2.InvokeCallback(ex);
			}
		}

		private void WriteDataComplete(IAsyncResult asyncResult)
		{
			StreamChannel.AsyncResult asyncResult2 = (StreamChannel.AsyncResult)asyncResult.AsyncState;
			try
			{
				this.stream.EndWrite(asyncResult);
				asyncResult2.InvokeCallback();
			}
			catch (Exception ex)
			{
				this.diagnosticsSession.TraceError<Exception>("WriteDataComplete exception: {0}", ex);
				asyncResult2.InvokeCallback(ex);
			}
		}

		private const int PacketLengthBufferLength = 4;

		private const int FlushPacket = 0;

		private const int MaxPacketSize = 1048576;

		private readonly IDiagnosticsSession diagnosticsSession;

		private byte[] packetLengthBuffer = new byte[4];

		private NetworkConnectionStream networkStream;

		private Stream stream;

		private Guid contextId;

		private int bytesRemainingInPacket;

		private class AsyncResult : LazyAsyncResult
		{
			internal AsyncResult(byte[] buffer, int offset, int count, AsyncCallback callback, object callerState) : base(null, callerState, callback)
			{
				this.buffer = buffer;
				this.offset = offset;
				this.count = count;
				this.bytesRequested = count;
			}

			internal byte[] Buffer
			{
				[DebuggerStepThrough]
				get
				{
					return this.buffer;
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

			internal int Count
			{
				[DebuggerStepThrough]
				get
				{
					return this.count;
				}
				[DebuggerStepThrough]
				set
				{
					this.count = value;
				}
			}

			internal int BytesRequested
			{
				[DebuggerStepThrough]
				get
				{
					return this.bytesRequested;
				}
			}

			internal static StreamChannel.AsyncResult EndAsyncOperation(IAsyncResult asyncResult)
			{
				return LazyAsyncResult.EndAsyncOperation<StreamChannel.AsyncResult>(asyncResult);
			}

			private readonly int bytesRequested;

			private byte[] buffer;

			private int offset;

			private int count;
		}
	}
}
