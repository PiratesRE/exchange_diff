using System;
using System.IO;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.HttpProxy;
using Microsoft.Exchange.HttpProxy.Common;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.HttpProxy
{
	internal class StreamProxy : IDisposeTrackable, IDisposable
	{
		public StreamProxy(StreamProxy.StreamProxyType streamProxyType, Stream source, Stream target, byte[] buffer, IRequestContext requestContext) : this(streamProxyType, source, target, requestContext)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			this.buffer = buffer;
		}

		public StreamProxy(StreamProxy.StreamProxyType streamProxyType, Stream source, Stream target, BufferPoolCollection.BufferSize maxBufferPoolSize, BufferPoolCollection.BufferSize minBufferPoolSize, IRequestContext requestContext) : this(streamProxyType, source, target, requestContext)
		{
			this.maxBufferPoolSize = maxBufferPoolSize;
			this.minBufferPoolSize = minBufferPoolSize;
			this.currentBufferPoolSize = minBufferPoolSize;
			this.currentBufferPool = BufferPoolCollection.AutoCleanupCollection.Acquire(this.currentBufferPoolSize);
			this.buffer = this.currentBufferPool.Acquire();
			this.previousBufferSize = this.buffer.Length;
		}

		private StreamProxy(StreamProxy.StreamProxyType streamProxyType, Stream source, Stream target, IRequestContext requestContext)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (target == null)
			{
				throw new ArgumentNullException("target");
			}
			if (requestContext == null)
			{
				throw new ArgumentException("requestContext");
			}
			this.disposeTracker = this.GetDisposeTracker();
			this.isDisposed = false;
			this.proxyType = streamProxyType;
			this.sourceStream = source;
			this.targetStream = target;
			this.requestContext = requestContext;
		}

		public StreamProxy.StreamProxyType ProxyType
		{
			get
			{
				this.CheckDispose();
				return this.proxyType;
			}
		}

		public StreamProxy.StreamProxyState StreamState
		{
			get
			{
				this.CheckDispose();
				return this.streamState;
			}
		}

		public Stream SourceStream
		{
			get
			{
				this.CheckDispose();
				return this.sourceStream;
			}
		}

		public Stream TargetStream
		{
			get
			{
				this.CheckDispose();
				return this.targetStream;
			}
		}

		public IRequestContext RequestContext
		{
			get
			{
				this.CheckDispose();
				return this.requestContext;
			}
		}

		public Stream AuxTargetStream
		{
			get
			{
				this.CheckDispose();
				return this.auxTargetStream;
			}
			set
			{
				this.CheckDispose();
				this.auxTargetStream = value;
			}
		}

		public long TotalBytesProxied
		{
			get
			{
				this.CheckDispose();
				return this.totalBytesProxied;
			}
		}

		public long NumberOfReadsCompleted
		{
			get
			{
				this.CheckDispose();
				return this.numberOfReadsCompleted;
			}
		}

		public IAsyncResult BeginProcess(AsyncCallback asyncCallback, object asyncState)
		{
			this.CheckDispose();
			this.LogElapsedTime("E_BegProc");
			if (asyncCallback == null)
			{
				throw new ArgumentNullException("asyncCallback");
			}
			IAsyncResult result;
			try
			{
				lock (this.lockObject)
				{
					if (this.lazyAsyncResult != null)
					{
						throw new InvalidOperationException("BeginProcess() cannot be called more than once.");
					}
					ExTraceGlobals.VerboseTracer.TraceDebug<int, StreamProxy.StreamProxyType>((long)this.GetHashCode(), "[StreamProxy::BeginProcess] Context: {0}, Type :{1}.", this.requestContext.TraceContext, this.proxyType);
					this.lazyAsyncResult = new LazyAsyncResult(this, asyncState, asyncCallback);
					this.asyncException = null;
					this.streamState = StreamProxy.StreamProxyState.None;
					this.asyncStateHolder = new AsyncStateHolder(this);
					try
					{
						if (this.sourceStream != null)
						{
							this.BeginRead();
						}
						else
						{
							this.BeginSend((int)this.totalBytesProxied);
						}
					}
					catch (Exception innerException)
					{
						throw new StreamProxyException(innerException);
					}
					result = this.lazyAsyncResult;
				}
			}
			finally
			{
				this.LogElapsedTime("L_BegProc");
			}
			return result;
		}

		public void EndProcess(IAsyncResult asyncResult)
		{
			this.CheckDispose();
			this.LogElapsedTime("E_EndProc");
			try
			{
				if (asyncResult == null)
				{
					throw new ArgumentNullException("asyncResult");
				}
				lock (this.lockObject)
				{
					if (this.lazyAsyncResult == null)
					{
						throw new InvalidOperationException("BeginProcess() was not called.");
					}
					if (!object.ReferenceEquals(asyncResult, this.lazyAsyncResult))
					{
						throw new InvalidOperationException("The wrong asyncResult is passed.");
					}
				}
				ExTraceGlobals.VerboseTracer.TraceDebug<int, StreamProxy.StreamProxyType>((long)this.GetHashCode(), "[StreamProxy::EndProcess] Context: {0}, Type :{1}. ", this.requestContext.TraceContext, this.proxyType);
				this.lazyAsyncResult.InternalWaitForCompletion();
				this.lazyAsyncResult = null;
				this.asyncStateHolder.Dispose();
				this.asyncStateHolder = null;
				if (this.asyncException != null)
				{
					throw new StreamProxyException(this.asyncException);
				}
			}
			finally
			{
				this.LogElapsedTime("L_EndProc");
			}
		}

		public void SetTargetStreamForBufferedSend(Stream newTargetStream)
		{
			this.CheckDispose();
			this.LogElapsedTime("E_SetTargetStream");
			lock (this.lockObject)
			{
				this.sourceStream = null;
				this.targetStream = newTargetStream;
				this.OnTargetStreamUpdate();
			}
			this.LogElapsedTime("L_SetTargetStream");
		}

		public DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<StreamProxy>(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
				this.disposeTracker = null;
			}
		}

		public void Dispose()
		{
			if (!this.isDisposed)
			{
				this.ReleaseBuffer();
				if (this.disposeTracker != null)
				{
					this.disposeTracker.Dispose();
					this.disposeTracker = null;
				}
				GC.SuppressFinalize(this);
				this.isDisposed = true;
			}
		}

		protected virtual byte[] GetUpdatedBufferToSend(ArraySegment<byte> buffer)
		{
			return null;
		}

		protected virtual void OnTargetStreamUpdate()
		{
		}

		private static void ReadCompleteCallback(IAsyncResult asyncResult)
		{
			StreamProxy streamProxy = AsyncStateHolder.Unwrap<StreamProxy>(asyncResult);
			if (asyncResult.CompletedSynchronously)
			{
				ThreadPool.QueueUserWorkItem(new WaitCallback(streamProxy.OnReadComplete), asyncResult);
				return;
			}
			streamProxy.OnReadComplete(asyncResult);
		}

		private static void WriteCompleteCallback(IAsyncResult asyncResult)
		{
			StreamProxy streamProxy = AsyncStateHolder.Unwrap<StreamProxy>(asyncResult);
			if (asyncResult.CompletedSynchronously)
			{
				ThreadPool.QueueUserWorkItem(new WaitCallback(streamProxy.OnWriteComplete), asyncResult);
				return;
			}
			streamProxy.OnWriteComplete(asyncResult);
		}

		private void BeginRead()
		{
			this.LogElapsedTime("E_BeginRead");
			try
			{
				ExTraceGlobals.VerboseTracer.TraceDebug<int, StreamProxy.StreamProxyType>((long)this.GetHashCode(), "[StreamProxy::BeginRead] Context: {0}, Type :{1}. ", this.requestContext.TraceContext, this.proxyType);
				this.requestContext.LatencyTracker.StartTracking(LatencyTrackerKey.StreamingLatency, true);
				this.sourceStream.BeginRead(this.buffer, 0, this.buffer.Length, new AsyncCallback(StreamProxy.ReadCompleteCallback), this.asyncStateHolder);
				this.streamState = StreamProxy.StreamProxyState.ExpectReadCallback;
			}
			finally
			{
				this.LogElapsedTime("L_BeginRead");
			}
		}

		private void BeginSend(int bytesToSend)
		{
			this.LogElapsedTime("E_BeginSend");
			try
			{
				ExTraceGlobals.VerboseTracer.TraceDebug<int, StreamProxy.StreamProxyType>((long)this.GetHashCode(), "[StreamProxy::BeginSend] Context: {0}, Type :{1}. ", this.requestContext.TraceContext, this.proxyType);
				if (bytesToSend != this.numberOfBytesInBuffer)
				{
					throw new InvalidOperationException(string.Format("Invalid SendBuffer - {0} bytes in buffer, {1} bytes to be sent", this.numberOfBytesInBuffer, bytesToSend));
				}
				byte[] updatedBufferToSend = this.GetUpdatedBufferToSend(new ArraySegment<byte>(this.buffer, 0, bytesToSend));
				if (updatedBufferToSend != null)
				{
					bytesToSend = updatedBufferToSend.Length;
					ExTraceGlobals.VerboseTracer.TraceDebug<int, StreamProxy.StreamProxyType, int>((long)this.GetHashCode(), "[StreamProxy::BeginSend] Context: {0}, Type :{1}. GetUpdatedBufferToSend() returns new buffer with size {2}.", this.requestContext.TraceContext, this.proxyType, bytesToSend);
				}
				this.requestContext.LatencyTracker.StartTracking(LatencyTrackerKey.StreamingLatency, true);
				this.BeginWrite(updatedBufferToSend ?? this.buffer, bytesToSend);
			}
			finally
			{
				this.LogElapsedTime("L_BeginSend");
			}
		}

		private void BeginWrite(byte[] buffer, int count)
		{
			this.LogElapsedTime("E_BegWrite");
			try
			{
				ExTraceGlobals.VerboseTracer.TraceDebug((long)this.GetHashCode(), "[StreamProxy::BeginWrite] Context: {0}, Type :{1}. Writing buffer with size {2} and count {3}.", new object[]
				{
					this.requestContext.TraceContext,
					this.proxyType,
					buffer.Length,
					count
				});
				if (this.AuxTargetStream != null)
				{
					this.AuxTargetStream.Write(buffer, 0, count);
				}
				this.targetStream.BeginWrite(buffer, 0, count, new AsyncCallback(StreamProxy.WriteCompleteCallback), this.asyncStateHolder);
				this.streamState = StreamProxy.StreamProxyState.ExpectWriteCallback;
			}
			finally
			{
				this.LogElapsedTime("L_BegWrite");
			}
		}

		private void LogElapsedTime(string latencyName)
		{
			if (HttpProxySettings.DetailedLatencyTracingEnabled.Value && this.requestContext != null && this.requestContext.LatencyTracker != null)
			{
				this.requestContext.LatencyTracker.LogElapsedTime(this.requestContext.Logger, latencyName + "_" + this.proxyType.ToString());
			}
		}

		private void OnReadComplete(object asyncState)
		{
			Diagnostics.SendWatsonReportOnUnhandledException(delegate()
			{
				IAsyncResult asyncResult = (IAsyncResult)asyncState;
				this.LogElapsedTime("E_OnReadComp");
				try
				{
					lock (this.lockObject)
					{
						ExTraceGlobals.VerboseTracer.TraceDebug<int, StreamProxy.StreamProxyType>((long)this.GetHashCode(), "[StreamProxy::OnReadComplete] Context: {0}, Type :{1}. ", this.requestContext.TraceContext, this.proxyType);
						int num = this.sourceStream.EndRead(asyncResult);
						this.streamState = StreamProxy.StreamProxyState.None;
						if (num > 0)
						{
							switch (this.proxyType)
							{
							case StreamProxy.StreamProxyType.Request:
								PerfCounters.HttpProxyCountersInstance.TotalBytesIn.IncrementBy((long)num);
								break;
							case StreamProxy.StreamProxyType.Response:
								PerfCounters.HttpProxyCountersInstance.TotalBytesOut.IncrementBy((long)num);
								break;
							}
							this.requestContext.LatencyTracker.LogElapsedTimeAsLatency(this.requestContext.Logger, LatencyTrackerKey.StreamingLatency, this.GetReadProtocolLogKey());
							this.numberOfBytesInBuffer = num;
							this.numberOfReadsCompleted += 1L;
							this.totalBytesProxied += (long)num;
							this.BeginSend(num);
						}
						else
						{
							this.Complete(null);
						}
					}
				}
				catch (Exception ex)
				{
					ExTraceGlobals.VerboseTracer.TraceError<int, StreamProxy.StreamProxyType, Exception>((long)this.GetHashCode(), "[StreamProxy::OnReadComplete] Context: {0}, Type :{1}. Error occured thrown when processing read. Exception: {2}", this.requestContext.TraceContext, this.proxyType, ex);
					this.Complete(ex);
				}
				finally
				{
					this.LogElapsedTime("L_OnReadComp");
				}
			}, new Diagnostics.LastChanceExceptionHandler(RequestDetailsLogger.LastChanceExceptionHandler));
		}

		private void OnWriteComplete(object asyncState)
		{
			Diagnostics.SendWatsonReportOnUnhandledException(delegate()
			{
				IAsyncResult asyncResult = (IAsyncResult)asyncState;
				this.LogElapsedTime("E_OnWriteComp");
				try
				{
					lock (this.lockObject)
					{
						ExTraceGlobals.VerboseTracer.TraceDebug<int, StreamProxy.StreamProxyType>((long)this.GetHashCode(), "[StreamProxy::OnWriteComplete] Context: {0}, Type :{1}. ", this.requestContext.TraceContext, this.proxyType);
						this.targetStream.EndWrite(asyncResult);
						this.streamState = StreamProxy.StreamProxyState.None;
						this.requestContext.LatencyTracker.LogElapsedTimeAsLatency(this.requestContext.Logger, LatencyTrackerKey.StreamingLatency, this.GetWriteProtocolLogKey());
						if (this.sourceStream != null)
						{
							this.AdjustBuffer();
							this.BeginRead();
						}
						else
						{
							this.Complete(null);
						}
					}
				}
				catch (Exception ex)
				{
					ExTraceGlobals.VerboseTracer.TraceError<int, StreamProxy.StreamProxyType, Exception>((long)this.GetHashCode(), "[StreamProxy::OnWriteComplete] Context: {0}, Type :{1}. Error occured thrown when processing write. Exception: {2}", this.requestContext.TraceContext, this.proxyType, ex);
					this.Complete(ex);
				}
				finally
				{
					this.LogElapsedTime("L_OnWriteComp");
				}
			}, new Diagnostics.LastChanceExceptionHandler(RequestDetailsLogger.LastChanceExceptionHandler));
		}

		private void Complete(Exception exception)
		{
			this.LogElapsedTime("E_SPComplete");
			try
			{
				ExTraceGlobals.VerboseTracer.TraceDebug<int, StreamProxy.StreamProxyType, Exception>((long)this.GetHashCode(), "[StreamProxy::Complete] Context: {0}, Type :{1}. Complete with exception: {2}", this.requestContext.TraceContext, this.proxyType, exception);
				this.asyncException = exception;
				this.lazyAsyncResult.InvokeCallback();
			}
			finally
			{
				this.LogElapsedTime("L_SPComplete");
			}
		}

		private HttpProxyMetadata GetReadProtocolLogKey()
		{
			if (this.proxyType == StreamProxy.StreamProxyType.Request)
			{
				return HttpProxyMetadata.ClientRequestStreamingLatency;
			}
			return HttpProxyMetadata.BackendResponseStreamingLatency;
		}

		private HttpProxyMetadata GetWriteProtocolLogKey()
		{
			if (this.proxyType == StreamProxy.StreamProxyType.Request)
			{
				return HttpProxyMetadata.BackendRequestStreamingLatency;
			}
			return HttpProxyMetadata.ClientResponseStreamingLatency;
		}

		private void ReleaseBuffer()
		{
			if (this.buffer != null && this.currentBufferPool != null)
			{
				try
				{
					this.currentBufferPool.Release(this.buffer);
				}
				finally
				{
					this.buffer = null;
				}
			}
		}

		private void AdjustBuffer()
		{
			if (this.currentBufferPool == null)
			{
				return;
			}
			if (this.numberOfBytesInBuffer >= this.buffer.Length)
			{
				if (this.currentBufferPoolSize < this.maxBufferPoolSize)
				{
					this.previousBufferSize = this.buffer.Length;
					this.ReleaseBuffer();
					this.currentBufferPoolSize++;
					this.currentBufferPool = BufferPoolCollection.AutoCleanupCollection.Acquire(this.currentBufferPoolSize);
					this.buffer = this.currentBufferPool.Acquire();
					return;
				}
			}
			else if (this.currentBufferPoolSize > this.minBufferPoolSize)
			{
				if (this.numberOfBytesInBuffer == this.previousBufferSize)
				{
					this.ReleaseBuffer();
					this.currentBufferPoolSize--;
					this.currentBufferPool = BufferPoolCollection.AutoCleanupCollection.Acquire(this.currentBufferPoolSize);
					this.buffer = this.currentBufferPool.Acquire();
					this.maxBufferPoolSize = this.currentBufferPoolSize;
					this.minBufferPoolSize = this.currentBufferPoolSize;
					return;
				}
				if (this.numberOfBytesInBuffer > this.previousBufferSize)
				{
					this.previousBufferSize = this.buffer.Length;
				}
			}
		}

		private void CheckDispose()
		{
			if (!this.isDisposed)
			{
				return;
			}
			throw new ObjectDisposedException("StreamProxy");
		}

		private readonly object lockObject = new object();

		private readonly StreamProxy.StreamProxyType proxyType;

		private readonly IRequestContext requestContext;

		private Stream sourceStream;

		private Stream targetStream;

		private StreamProxy.StreamProxyState streamState;

		private Stream auxTargetStream;

		private long totalBytesProxied;

		private long numberOfReadsCompleted;

		private int numberOfBytesInBuffer;

		private LazyAsyncResult lazyAsyncResult;

		private AsyncStateHolder asyncStateHolder;

		private Exception asyncException;

		private byte[] buffer;

		private BufferPoolCollection.BufferSize maxBufferPoolSize;

		private BufferPoolCollection.BufferSize minBufferPoolSize;

		private BufferPoolCollection.BufferSize currentBufferPoolSize;

		private int previousBufferSize;

		private BufferPool currentBufferPool;

		private DisposeTracker disposeTracker;

		private bool isDisposed;

		internal enum StreamProxyType
		{
			Request,
			Response
		}

		internal enum StreamProxyState
		{
			None,
			ExpectReadCallback,
			ExpectWriteCallback
		}
	}
}
