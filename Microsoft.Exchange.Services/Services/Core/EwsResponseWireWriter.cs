using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Services.Core
{
	internal abstract class EwsResponseWireWriter : IDisposable
	{
		public static EwsResponseWireWriter Create(CallContext context)
		{
			return EwsResponseWireWriter.Create(context, false);
		}

		public static EwsResponseWireWriter Create(CallContext context, bool initWaitEvent)
		{
			return new EwsResponseWireWriter.EwsHttpResponseWireWriter(context, initWaitEvent);
		}

		public abstract void WriteResponseToWire(BaseSoapResponse response, bool isLastResponse);

		public abstract void WriteResponseToWire(byte[] responseBytes, int offset, int count);

		public abstract void FinishWritesAndCompleteResponse(CompleteRequestAsyncCallback completeRequest);

		public abstract void WaitForSendCompletion();

		public abstract void Dispose();

		private class EwsHttpResponseWireWriter : EwsResponseWireWriter, IDisposeTrackable, IDisposable
		{
			private static HttpResponseMessageProperty WcfHttpResponseMessageProperty
			{
				get
				{
					if (EwsResponseWireWriter.EwsHttpResponseWireWriter.httpResponseMessageProperty == null)
					{
						EwsResponseWireWriter.EwsHttpResponseWireWriter.httpResponseMessageProperty = new HttpResponseMessageProperty();
						EwsResponseWireWriter.EwsHttpResponseWireWriter.httpResponseMessageProperty.SuppressPreamble = true;
						EwsResponseWireWriter.EwsHttpResponseWireWriter.httpResponseMessageProperty.SuppressEntityBody = true;
					}
					return EwsResponseWireWriter.EwsHttpResponseWireWriter.httpResponseMessageProperty;
				}
			}

			internal EwsHttpResponseWireWriter(CallContext callContext, bool initWaitEvent)
			{
				ExTraceGlobals.GetEventsCallTracer.TraceFunction<int, bool>((long)this.GetHashCode(), "EwsHttpResponseWireWriter.ctor: entering. hashcode: {0}, initWaitEvent: {1}", this.GetHashCode(), initWaitEvent);
				this.callContext = callContext;
				this.responsesToSend = new Queue<Tuple<object, bool>>();
				this.version = ExchangeVersion.Current;
				this.writeTimer = new Timer(new TimerCallback(this.WriteTimeoutCallback), this, -1, -1);
				this.callContext.OperationContext.OutgoingMessageProperties[HttpResponseMessageProperty.Name] = EwsResponseWireWriter.EwsHttpResponseWireWriter.WcfHttpResponseMessageProperty;
				this.callContext.HttpContext.Items["ResponseHasBegun"] = true;
				this.disposeTracker = this.GetDisposeTracker();
				this.initWaitEvent = initWaitEvent;
				bool flag = !this.initWaitEvent;
				ExTraceGlobals.GetEventsCallTracer.TraceDebug<bool>((long)this.GetHashCode(), "EwsHttpResponseWireWriter.ctor: useChunkedStreaming {0}", flag);
				this.streamingResponse = (flag ? new EwsResponseWireWriter.EwsHttpResponseWireWriter.ChunkedStreamingResponse(this.callContext.HttpContext.Response) : new EwsResponseWireWriter.EwsHttpResponseWireWriter.StreamingResponse(this.callContext.HttpContext.Response));
			}

			public virtual DisposeTracker GetDisposeTracker()
			{
				return DisposeTracker.Get<EwsResponseWireWriter.EwsHttpResponseWireWriter>(this);
			}

			public void SuppressDisposeTracker()
			{
				if (this.disposeTracker != null)
				{
					this.disposeTracker.Suppress();
				}
			}

			public override void WriteResponseToWire(BaseSoapResponse response, bool isLastResponse)
			{
				ExTraceGlobals.GetEventsCallTracer.TraceFunction<Type, bool>((long)this.GetHashCode(), "EwsHttpResponseWireWriter.WriteResponseToWire(BaseSoapResponse): {0}, isLastResponse: {1}", response.GetType(), isLastResponse);
				lock (this.lockObject)
				{
					if (this.finalCallback != null)
					{
						throw new InvalidOperationException("EwsHttpResponseWireWriter.WriteResponseToWire(BaseSoapResponse) Cannot write a new response when a final response has been given");
					}
					this.responsesToSend.Enqueue(new Tuple<object, bool>(response, isLastResponse));
					ExTraceGlobals.GetEventsCallTracer.TraceDebug<int>((long)this.GetHashCode(), "EwsHttpResponseWireWriter.WriteResponseToWire(BaseSoapResponse): responses length {0}", this.responsesToSend.Count);
				}
				this.BeginSendIfNecessary();
			}

			public override void WriteResponseToWire(byte[] responseBytes, int offset, int count)
			{
				ExTraceGlobals.GetEventsCallTracer.TraceFunction<int>((long)this.GetHashCode(), "EwsHttpResponseWireWriter.WriteResponseToWire(byte[]) entering, count: {0}", count);
				lock (this.lockObject)
				{
					if (this.finalCallback != null)
					{
						throw new InvalidOperationException("EwsHttpResponseWireWriter.WriteResponseToWire(byte[]) Cannot write a new response when a final response has been given");
					}
					if (responseBytes == null)
					{
						throw new ArgumentNullException("responseBytes");
					}
					if (offset < 0 || offset >= responseBytes.Length)
					{
						throw new ArgumentOutOfRangeException("offset");
					}
					if (count <= 0 || count + offset > responseBytes.Length)
					{
						throw new ArgumentOutOfRangeException("count");
					}
					byte[] array = new byte[count];
					Buffer.BlockCopy(responseBytes, offset, array, 0, count);
					this.responsesToSend.Enqueue(new Tuple<object, bool>(array, false));
					ExTraceGlobals.GetEventsCallTracer.TraceDebug<int>((long)this.GetHashCode(), "EwsHttpResponseWireWriter.WriteResponseToWire(byte[]): responses length {0}", this.responsesToSend.Count);
				}
				this.BeginSendIfNecessary();
			}

			public override void FinishWritesAndCompleteResponse(CompleteRequestAsyncCallback completeRequest)
			{
				ExTraceGlobals.GetEventsCallTracer.TraceFunction((long)this.GetHashCode(), "EwsHttpResponseWireWriter.FinishWritesAndCompleteResponse entering");
				lock (this.lockObject)
				{
					this.finalCallback = completeRequest;
					ExTraceGlobals.GetEventsCallTracer.TraceDebug<int, EwsResponseWireWriter.EwsHttpResponseWireWriter.SendState>((long)this.GetHashCode(), "EwsHttpResponseWireWriter.FinishWritesAndCompleteResponse: responses length: {0}, sendState: {1}", this.responsesToSend.Count, this.sendState);
					if (!this.callContext.HttpContext.Response.IsClientConnected || (this.responsesToSend.Count == 0 && this.sendState != EwsResponseWireWriter.EwsHttpResponseWireWriter.SendState.Sending))
					{
						this.CompleteResponse();
					}
				}
			}

			public override void WaitForSendCompletion()
			{
				ExTraceGlobals.GetEventsCallTracer.TraceFunction((long)this.GetHashCode(), "EwsHttpResponseWireWriter.WaitForSendCompletion entering");
				if (!this.initWaitEvent)
				{
					throw new InvalidOperationException("EwsResponseWireWriter was not created with wait handle.");
				}
				if (this.sendFinishEvent != null)
				{
					this.sendFinishEvent.WaitOne(60000);
				}
				ExTraceGlobals.GetEventsCallTracer.TraceFunction((long)this.GetHashCode(), "EwsHttpResponseWireWriter.WaitForSendCompletion exiting");
			}

			private static XmlSerializer GetSerializer(object objectToSerialize)
			{
				Type type = objectToSerialize.GetType();
				XmlSerializer result;
				lock (EwsResponseWireWriter.EwsHttpResponseWireWriter.serializers)
				{
					if (!EwsResponseWireWriter.EwsHttpResponseWireWriter.serializers.ContainsKey(type))
					{
						SafeXmlSerializer value = new SafeXmlSerializer(type);
						EwsResponseWireWriter.EwsHttpResponseWireWriter.serializers.Add(type, value);
					}
					result = EwsResponseWireWriter.EwsHttpResponseWireWriter.serializers[type];
				}
				return result;
			}

			private void WriteTimeoutCallback(object state)
			{
				ExTraceGlobals.GetEventsCallTracer.TraceFunction((long)this.GetHashCode(), "EwsHttpResponseWireWriter.WriteTimeoutCallback entering");
				lock (this.lockObject)
				{
					this.connectionTimedOut = true;
					try
					{
						if (this.writeOperationAsyncResult != null)
						{
							this.callContext.HttpContext.Response.OutputStream.Close();
							this.UpdateWriteTimer(-1);
						}
					}
					finally
					{
						this.SignalSendFinishEvent();
					}
				}
			}

			private void UpdateWriteTimer(int timeoutMilliseconds)
			{
				lock (this.lockObject)
				{
					if (this.writeTimer != null)
					{
						this.writeTimer.Change(timeoutMilliseconds, -1);
					}
				}
			}

			private void BeginSendIfNecessary()
			{
				ExTraceGlobals.GetEventsCallTracer.TraceFunction((long)this.GetHashCode(), "EwsHttpResponseWireWriter.BeginSendIfNecessary entering");
				bool flag = false;
				lock (this.lockObject)
				{
					EwsResponseWireWriter.EwsHttpResponseWireWriter.SendState arg = this.sendState;
					if (this.sendState != EwsResponseWireWriter.EwsHttpResponseWireWriter.SendState.Sending)
					{
						if (this.sendState == EwsResponseWireWriter.EwsHttpResponseWireWriter.SendState.Closed || !this.callContext.HttpContext.Response.IsClientConnected)
						{
							this.sendState = EwsResponseWireWriter.EwsHttpResponseWireWriter.SendState.Closed;
							ExTraceGlobals.GetEventsCallTracer.TraceDebug((long)this.GetHashCode(), "[EwsHttpResponseWireWriter::BeginSendIfNecessary] The client disconnected from this request.");
							throw new HttpException("Client connection is closed");
						}
						this.sendState = EwsResponseWireWriter.EwsHttpResponseWireWriter.SendState.Sending;
						flag = true;
					}
					ExTraceGlobals.GetEventsCallTracer.TraceDebug<EwsResponseWireWriter.EwsHttpResponseWireWriter.SendState, EwsResponseWireWriter.EwsHttpResponseWireWriter.SendState, bool>((long)this.GetHashCode(), "EwsHttpResponseWireWriter.BeginSendIfNecessary: old SendState: {0}, new: {1}, beginSend: {2}", arg, this.sendState, flag);
				}
				if (flag)
				{
					this.SendResponse();
				}
			}

			private void SendResponse()
			{
				ExchangeVersion.ExecuteWithSpecifiedVersion(this.version, new ExchangeVersion.ExchangeVersionDelegate(this.SendResponseWithVersionSet));
			}

			private void SendResponseWithVersionSet()
			{
				ExTraceGlobals.GetEventsCallTracer.TraceFunction((long)this.GetHashCode(), "EwsHttpResponseWireWriter.SendResponseWithVersionSet entering");
				object item;
				bool item2;
				lock (this.lockObject)
				{
					ExTraceGlobals.GetEventsCallTracer.TraceDebug<int>((long)this.GetHashCode(), "EwsHttpResponseWireWriter.SendResponseWithVersionSet: responses length: {0}", this.responsesToSend.Count);
					if (this.responsesToSend.Count <= 0)
					{
						this.OnSendingComplete();
						this.SignalSendFinishEvent();
						return;
					}
					Tuple<object, bool> tuple = this.responsesToSend.Dequeue();
					item = tuple.Item1;
					item2 = tuple.Item2;
				}
				XmlSerializer serializer = EwsResponseWireWriter.EwsHttpResponseWireWriter.GetSerializer(item);
				XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
				xmlWriterSettings.OmitXmlDeclaration = true;
				int num = 0;
				lock (this.lockObject)
				{
					byte[] array;
					int num2;
					if (item is BaseSoapResponse)
					{
						this.responseBufferStream.Seek(0L, SeekOrigin.Begin);
						XmlWriter xmlWriter = XmlWriter.Create(this.responseBufferStream, xmlWriterSettings);
						serializer.Serialize(xmlWriter, item);
						array = this.responseBufferStream.ToArray();
						num2 = (int)this.responseBufferStream.Position;
						if (num2 > 3 && array[0] == 239 && array[1] == 187 && array[2] == 191)
						{
							num = 3;
							num2 -= num;
						}
					}
					else
					{
						if (!(item is byte[]))
						{
							throw new InvalidOperationException("Stored responses must be either of type BaseSoapResponse or of type byte[].");
						}
						array = (item as byte[]);
						num2 = array.Length;
					}
					try
					{
						if (this.sendFinishEvent == null)
						{
							if (this.initWaitEvent)
							{
								this.sendFinishEvent = new ManualResetEvent(false);
							}
						}
						else
						{
							this.sendFinishEvent.Reset();
						}
						this.writeOperationAsyncResult = this.streamingResponse.BeginWrite(array, num, num2, item2, new AsyncCallback(this.EndSendResponse), this);
						this.UpdateWriteTimer(60000);
					}
					catch (Exception arg)
					{
						ExTraceGlobals.GetEventsCallTracer.TraceError<Exception>((long)this.GetHashCode(), "EwsHttpResponseWireWriter.SendResponseWithVersionSet: exception: {0}", arg);
						this.SignalSendFinishEvent();
						throw;
					}
				}
			}

			private void SignalSendFinishEvent()
			{
				ExTraceGlobals.GetEventsCallTracer.TraceFunction((long)this.GetHashCode(), "EwsHttpResponseWireWriter.SignalSendFinishEvent entering");
				lock (this.lockObject)
				{
					if (this.sendFinishEvent != null)
					{
						ExTraceGlobals.GetEventsCallTracer.TraceDebug((long)this.GetHashCode(), "EwsHttpResponseWireWriter.SignalSendFinishEvent: set sendFinishEvent");
						this.sendFinishEvent.Set();
					}
				}
			}

			private void EndSendResponse(IAsyncResult result)
			{
				ExTraceGlobals.GetEventsCallTracer.TraceFunction((long)this.GetHashCode(), "EwsHttpResponseWireWriter.EndSendResponse entering");
				bool flag = false;
				try
				{
					flag = this.InternalEndSendResponse(result);
					ExTraceGlobals.GetEventsCallTracer.TraceFunction<bool>((long)this.GetHashCode(), "EwsHttpResponseWireWriter.EndSendResponse: shouldContinue value is {0}", flag);
				}
				finally
				{
					if (!flag)
					{
						this.SignalSendFinishEvent();
					}
				}
			}

			private bool InternalEndSendResponse(IAsyncResult result)
			{
				bool result2 = false;
				Exception ex = null;
				try
				{
					lock (this.lockObject)
					{
						this.UpdateWriteTimer(-1);
						this.writeOperationAsyncResult = null;
						if (this.connectionTimedOut)
						{
							throw new TimeoutException("Write operation has timed out.");
						}
					}
					this.streamingResponse.EndWrite(result);
					this.streamingResponse.Flush();
				}
				catch (NullReferenceException ex2)
				{
					ex = ex2;
				}
				catch (IOException ex3)
				{
					ex = ex3;
				}
				catch (HttpException ex4)
				{
					ex = ex4;
				}
				catch (TimeoutException ex5)
				{
					ex = ex5;
				}
				catch (AggregateException ex6)
				{
					ex = ex6;
				}
				if (ex != null)
				{
					ExTraceGlobals.GetEventsCallTracer.TraceDebug<Exception>((long)this.GetHashCode(), "[EwsHttpResponseWireWriter::InternalEndSendResponse] Exception sending response: {0}", ex);
					lock (this.lockObject)
					{
						this.sendState = EwsResponseWireWriter.EwsHttpResponseWireWriter.SendState.Closed;
					}
					return result2;
				}
				EwsResponseWireWriter.EwsHttpResponseWireWriter.SendState sendState;
				lock (this.lockObject)
				{
					if (this.responsesToSend.Count == 0)
					{
						this.OnSendingComplete();
					}
					else if (!this.callContext.HttpContext.Response.IsClientConnected)
					{
						this.sendState = EwsResponseWireWriter.EwsHttpResponseWireWriter.SendState.Closed;
					}
					sendState = this.sendState;
					ExTraceGlobals.GetEventsCallTracer.TraceDebug<EwsResponseWireWriter.EwsHttpResponseWireWriter.SendState, int>((long)this.GetHashCode(), "[EwsHttpResponseWireWriter::InternalEndSendResponse] sendState: {0}, responses length: {1}", this.sendState, this.responsesToSend.Count);
				}
				if (sendState == EwsResponseWireWriter.EwsHttpResponseWireWriter.SendState.Sending)
				{
					ExTraceGlobals.GetEventsCallTracer.TraceDebug((long)this.GetHashCode(), "[EwsHttpResponseWireWriter::InternalEndSendResponse] going to call SendResponse again");
					try
					{
						this.SendResponse();
						return true;
					}
					catch (ObjectDisposedException arg)
					{
						ExTraceGlobals.CommonAlgorithmTracer.TraceError<ObjectDisposedException>((long)this.GetHashCode(), "[EwsResponseWireWriter.InternalEndSendResponse] ObjectDisposedException encountered.  {0}", arg);
						return false;
					}
				}
				if (sendState == EwsResponseWireWriter.EwsHttpResponseWireWriter.SendState.Closed && this.finalCallback != null)
				{
					ExTraceGlobals.GetEventsCallTracer.TraceDebug((long)this.GetHashCode(), "[EwsHttpResponseWireWriter::InternalEndSendResponse] going to call CompleteResponse");
					this.CompleteResponse();
				}
				return result2;
			}

			private void OnSendingComplete()
			{
				ExTraceGlobals.GetEventsCallTracer.TraceDebug<bool>((long)this.GetHashCode(), "EwsHttpResponseWireWriter.OnSendingComplete] entering, finalCallback == null: {0}", this.finalCallback == null);
				if (this.finalCallback == null)
				{
					this.sendState = EwsResponseWireWriter.EwsHttpResponseWireWriter.SendState.Idle;
					return;
				}
				this.sendState = EwsResponseWireWriter.EwsHttpResponseWireWriter.SendState.Closed;
			}

			private void CompleteResponse()
			{
				ExTraceGlobals.GetEventsCallTracer.TraceFunction((long)this.GetHashCode(), "EwsHttpResponseWireWriter.CompleteResponse] entering");
				lock (this.lockObject)
				{
					if (!this.finalWcfCallbackInvoked)
					{
						ExTraceGlobals.GetEventsCallTracer.TraceFunction((long)this.GetHashCode(), "EwsHttpResponseWireWriter.CompleteResponse] going to dispose stuffs and finalCallback");
						this.responseBufferStream.Close();
						if (this.writeTimer != null)
						{
							this.writeTimer.Dispose();
							this.writeTimer = null;
						}
						if (this.finalCallback != null)
						{
							this.finalCallback(null);
						}
						if (this.streamingResponse != null)
						{
							this.streamingResponse.Dispose();
							this.streamingResponse = null;
						}
						this.finalWcfCallbackInvoked = true;
					}
				}
			}

			public override void Dispose()
			{
				if (this.disposeTracker != null)
				{
					this.disposeTracker.Dispose();
				}
				this.Dispose(true);
			}

			private void Dispose(bool suppressFinalizer)
			{
				lock (this.lockObject)
				{
					if (!this.isDisposed)
					{
						if (suppressFinalizer)
						{
							GC.SuppressFinalize(this);
						}
						this.isDisposed = true;
					}
				}
			}

			private const int MaxWriteTimeout = 60000;

			private const string IsaNoBuffering = "X-NoBuffering";

			private readonly DisposeTracker disposeTracker;

			private readonly bool initWaitEvent;

			private static HttpResponseMessageProperty httpResponseMessageProperty;

			private static Dictionary<Type, XmlSerializer> serializers = new Dictionary<Type, XmlSerializer>();

			private CallContext callContext;

			private Queue<Tuple<object, bool>> responsesToSend;

			private EwsResponseWireWriter.EwsHttpResponseWireWriter.SendState sendState;

			private ExchangeVersion version;

			private CompleteRequestAsyncCallback finalCallback;

			private bool finalWcfCallbackInvoked;

			private object lockObject = new object();

			private MemoryStream responseBufferStream = new MemoryStream();

			private Timer writeTimer;

			private bool connectionTimedOut;

			private IAsyncResult writeOperationAsyncResult;

			private EventWaitHandle sendFinishEvent;

			private EwsResponseWireWriter.EwsHttpResponseWireWriter.StreamingResponse streamingResponse;

			private bool isDisposed;

			private enum SendState
			{
				Idle,
				Sending,
				Closed
			}

			private class StreamingResponse : DisposeTrackableBase
			{
				public StreamingResponse(HttpResponse httpResponse)
				{
					this.response = httpResponse;
					this.response.BufferOutput = false;
					this.response.Buffer = false;
					this.response.AppendHeader("X-NoBuffering", "1");
					this.response.ContentType = string.Empty;
					this.response.ClearContent();
				}

				public virtual IAsyncResult BeginWrite(byte[] buffer, int offset, int count, bool isLastResponse, AsyncCallback ac, object state)
				{
					return this.response.OutputStream.BeginWrite(buffer, offset, count, ac, state);
				}

				public virtual void EndWrite(IAsyncResult ar)
				{
					this.response.OutputStream.EndWrite(ar);
				}

				public virtual void Flush()
				{
					this.response.Flush();
				}

				protected override DisposeTracker InternalGetDisposeTracker()
				{
					return DisposeTracker.Get<EwsResponseWireWriter.EwsHttpResponseWireWriter.StreamingResponse>(this);
				}

				protected override void InternalDispose(bool isDisposing)
				{
				}

				protected HttpResponse response;
			}

			private sealed class ChunkedStreamingResponse : EwsResponseWireWriter.EwsHttpResponseWireWriter.StreamingResponse
			{
				internal ChunkedStreamingResponse(HttpResponse httpResponse) : base(httpResponse)
				{
					this.response.AddHeader("Transfer-Encoding", "chunked");
					this.streamWriter = new StreamWriter(this.response.OutputStream, new UTF8Encoding(false, false), 8192);
					this.sentBefore = false;
				}

				public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, bool isLastResponse, AsyncCallback ac, object state)
				{
					string notificationString = (offset == 0) ? Encoding.UTF8.GetString(buffer, 0, buffer.Length) : Encoding.UTF8.GetString(buffer, offset, count);
					Task task = Task.Factory.StartNew(delegate(object x)
					{
						this.Write(notificationString, isLastResponse);
					}, state);
					task.ContinueWith(delegate(Task res)
					{
						ac(task);
					});
					return task;
				}

				public override void EndWrite(IAsyncResult ar)
				{
					using (Task task = ar as Task)
					{
						if (task.Exception != null)
						{
							throw task.Exception;
						}
					}
				}

				public override void Flush()
				{
				}

				private void Write(string content, bool isLastResponse)
				{
					string value;
					if (isLastResponse)
					{
						value = string.Format("{0}\r\n{1:x}\r\n{2}\r\n{3:x}\r\n\r\n", new object[]
						{
							" ",
							content.Length,
							content,
							0
						});
					}
					else if (!this.sentBefore)
					{
						value = string.Format("{0:x}\r\n{1}\r\n{2:x}\r\n{3}", new object[]
						{
							content.Length,
							content,
							"  ".Length,
							" "
						});
						this.sentBefore = true;
					}
					else
					{
						value = string.Format("{0}\r\n{1:x}\r\n{2}\r\n{3:x}\r\n{4}", new object[]
						{
							" ",
							content.Length,
							content,
							"  ".Length,
							" "
						});
					}
					this.streamWriter.Write(value);
					this.streamWriter.Flush();
				}

				protected override void InternalDispose(bool isDisposing)
				{
					if (!this.disposed)
					{
						if (isDisposing && this.streamWriter != null)
						{
							this.streamWriter.Dispose();
						}
						this.disposed = true;
					}
				}

				private const string FullDummyResponse = "  ";

				private const string PartialDummyResponse = " ";

				private const string FirstChunkStringFormat = "{0:x}\r\n{1}\r\n{2:x}\r\n{3}";

				private const string MidChunkStringFormat = "{0}\r\n{1:x}\r\n{2}\r\n{3:x}\r\n{4}";

				private const string LastChunkStringFormat = "{0}\r\n{1:x}\r\n{2}\r\n{3:x}\r\n\r\n";

				private StreamWriter streamWriter;

				private bool disposed;

				private bool sentBefore;
			}
		}
	}
}
