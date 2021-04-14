using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Timers;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.MapiHttpClient;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.RpcClientAccess;

namespace Microsoft.Exchange.MapiHttp
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class MapiHttpClient : BaseObject
	{
		protected MapiHttpClient(MapiHttpBindingInfo bindingInfo)
		{
			using (DisposeGuard disposeGuard = this.Guard())
			{
				Util.ThrowOnNullArgument(bindingInfo, "bindingInfo");
				this.bindingInfo = bindingInfo;
				this.pingCompletionEvent = new AutoResetEvent(false);
				this.backgroundTimer = new System.Timers.Timer();
				this.backgroundTimer.AutoReset = false;
				this.backgroundTimer.Interval = MapiHttpClient.backgroundWakeupPeriod.TotalMilliseconds;
				this.backgroundTimer.Elapsed += this.BackgroundTimerEvent;
				this.backgroundTimer.Start();
				disposeGuard.Success();
			}
		}

		public WebHeaderCollection LastHttpResponseHeaders { get; protected set; }

		public WebHeaderCollection LastHttpRequestHeaders { get; protected set; }

		public HttpStatusCode? LastResponseStatusCode { get; protected set; }

		public string LastResponseStatusDescription { get; protected set; }

		internal abstract string VdirPath { get; }

		protected IntPtr[] ContextHandles
		{
			get
			{
				return this.contextCache.Keys.ToArray<IntPtr>();
			}
		}

		public bool TryGetContextInfo(IntPtr contextHandle, out MapiHttpContextInfo contextInfo)
		{
			base.CheckDisposed();
			contextInfo = null;
			ClientSessionContext clientSessionContext;
			if (this.TryGetContext(contextHandle, out clientSessionContext))
			{
				contextInfo = new MapiHttpContextInfo(clientSessionContext);
				return true;
			}
			return false;
		}

		public void SetAdditionalRequestHeaders(IntPtr contextHandle, HttpWebRequest request)
		{
			ClientSessionContext clientSessionContext;
			if (this.TryGetContext(contextHandle, out clientSessionContext))
			{
				clientSessionContext.SetAdditionalRequestHeaders(request);
			}
		}

		internal HttpWebRequest CreateRequest(IntPtr contextHandle, out string requestId)
		{
			ClientSessionContext clientSessionContext;
			if (!this.TryGetContext(contextHandle, out clientSessionContext))
			{
				throw ProtocolException.FromResponseCode((LID)45788, string.Format("Context handle {0} is invalid", contextHandle), ResponseCode.ContextNotFound, null);
			}
			return clientSessionContext.CreateRequest(out requestId);
		}

		protected ICancelableAsyncResult BeginWrapper<T>(IntPtr contextHandle, bool needNewContext, Func<ClientSessionContext, T> beginFuncToWrap) where T : ClientAsyncOperation
		{
			ClientSessionContext arg;
			if (needNewContext)
			{
				arg = new ClientSessionContext(this.bindingInfo, this.VdirPath, contextHandle);
			}
			else if (!this.TryGetContext(contextHandle, out arg))
			{
				throw ProtocolException.FromResponseCode((LID)55840, string.Format("Context handle {0} is invalid", contextHandle), ResponseCode.ContextNotFound, null);
			}
			return beginFuncToWrap(arg);
		}

		protected ErrorCode EndWrapper<T>(ICancelableAsyncResult asyncResult, bool dropContextOnSuccess, bool dropContextOnFailure, out IntPtr contextHandle, Func<T, ErrorCode> endFuncToWrap) where T : ClientAsyncOperation
		{
			T arg = (T)((object)asyncResult);
			bool flag = false;
			bool flag2 = false;
			ErrorCode result;
			try
			{
				ClientAsyncOperation clientAsyncOperation = (ClientAsyncOperation)asyncResult;
				this.LastHttpResponseHeaders = clientAsyncOperation.HttpWebResponseHeaders;
				this.LastResponseStatusCode = new HttpStatusCode?(clientAsyncOperation.LastResponseStatusCode);
				this.LastResponseStatusDescription = clientAsyncOperation.LastResponseStatusDescription;
				this.LastHttpRequestHeaders = clientAsyncOperation.HttpWebRequestHeaders;
				ErrorCode errorCode = endFuncToWrap(arg);
				contextHandle = arg.Context.ContextHandle;
				if (errorCode == ErrorCode.None)
				{
					if (dropContextOnSuccess)
					{
						flag = true;
					}
					else
					{
						this.AddContext(arg.Context);
					}
				}
				else if (dropContextOnFailure)
				{
					flag = true;
				}
				result = errorCode;
			}
			catch (AggregateException exception)
			{
				if (exception.FindException<ContextNotFoundException>() != null)
				{
					flag = true;
				}
				else if (exception.FindException<ProtocolTransportException>() != null)
				{
					flag2 = true;
				}
				throw;
			}
			catch (ContextNotFoundException)
			{
				flag = true;
				throw;
			}
			catch (ProtocolTransportException)
			{
				flag2 = true;
				throw;
			}
			finally
			{
				if (flag)
				{
					if (arg.Context.ContextHandle != IntPtr.Zero)
					{
						this.RemoveContext(arg.Context.ContextHandle);
					}
					contextHandle = IntPtr.Zero;
				}
				if (flag2)
				{
					arg.Context.Reset();
				}
			}
			return result;
		}

		protected IntPtr CreateNewContextHandle(Func<IntPtr, IntPtr> contextHandleModifier)
		{
			IntPtr intPtr;
			ClientSessionContext clientSessionContext;
			do
			{
				intPtr = new IntPtr(Interlocked.Increment(ref this.contextHandleCounter));
				if (contextHandleModifier != null)
				{
					intPtr = contextHandleModifier(intPtr);
				}
			}
			while (this.TryGetContext(intPtr, out clientSessionContext));
			return intPtr;
		}

		protected override void InternalDispose()
		{
			lock (this.backgroundTimerDisposeLock)
			{
				Util.DisposeIfPresent(this.backgroundTimer);
				this.isTimerDisposed = true;
				if (!this.isBackgroundProcessing)
				{
					Util.DisposeIfPresent(this.pingCompletionEvent);
				}
			}
			IntPtr[] array = this.contextCache.Keys.ToArray<IntPtr>();
			foreach (IntPtr contextHandle in array)
			{
				this.RemoveContext(contextHandle);
			}
			base.InternalDispose();
		}

		private void AddContext(ClientSessionContext context)
		{
			if (this.contextCache.TryAdd(context.ContextHandle, context))
			{
				ExTraceGlobals.ClientSessionContextTracer.TraceInformation<IntPtr, string>(49568, 0L, "MapiHttpClient: Added new session context to context cache; ContextHandle={0}, RequestGroupId={1}", context.ContextHandle, context.RequestGroupId);
			}
		}

		private bool TryGetContext(IntPtr contextHandle, out ClientSessionContext context)
		{
			return this.contextCache.TryGetValue(contextHandle, out context);
		}

		private void RemoveContext(IntPtr contextHandle)
		{
			ClientSessionContext clientSessionContext;
			if (this.contextCache.TryRemove(contextHandle, out clientSessionContext))
			{
				clientSessionContext.Reset();
				ExTraceGlobals.ClientSessionContextTracer.TraceInformation<IntPtr, string>(49056, 0L, "MapiHttpClient: Removed session context from context cache; ContextHandle={0}, RequestGroupId={1}", clientSessionContext.ContextHandle, clientSessionContext.RequestGroupId);
			}
		}

		private void BackgroundTimerEvent(object sender, ElapsedEventArgs e)
		{
			lock (this.backgroundTimerDisposeLock)
			{
				if (this.isTimerDisposed)
				{
					return;
				}
				this.isBackgroundProcessing = true;
			}
			int tickCount = Environment.TickCount;
			try
			{
				if (this.bindingInfo.KeepContextsAlive)
				{
					foreach (KeyValuePair<IntPtr, ClientSessionContext> keyValuePair in this.contextCache)
					{
						if (this.isTimerDisposed)
						{
							break;
						}
						if (keyValuePair.Value != null && keyValuePair.Value.NeedsRefresh)
						{
							this.RefreshContext(keyValuePair.Value);
						}
					}
				}
			}
			finally
			{
				lock (this.backgroundTimerDisposeLock)
				{
					if (!this.isTimerDisposed)
					{
						TimeSpan t = TimeSpan.FromMilliseconds((double)(Environment.TickCount - tickCount));
						if (t > MapiHttpClient.backgroundWakeupPeriod - TimeSpan.FromSeconds(5.0))
						{
							t = MapiHttpClient.backgroundWakeupPeriod - TimeSpan.FromSeconds(5.0);
						}
						this.backgroundTimer.Interval = MapiHttpClient.backgroundWakeupPeriod.TotalMilliseconds - t.TotalMilliseconds;
						this.backgroundTimer.Start();
					}
					else
					{
						Util.DisposeIfPresent(this.pingCompletionEvent);
					}
					this.isBackgroundProcessing = false;
				}
			}
		}

		private void RefreshContext(ClientSessionContext clientSessionContext)
		{
			try
			{
				this.pingCompletionEvent.Reset();
				ICancelableAsyncResult asyncResult = this.BeginWrapper<PingClientAsyncOperation>(clientSessionContext.ContextHandle, false, delegate(ClientSessionContext context)
				{
					PingClientAsyncOperation pingClientAsyncOperation = new PingClientAsyncOperation(context, delegate(ICancelableAsyncResult innerAsyncResult)
					{
						this.pingCompletionEvent.Set();
					}, null);
					pingClientAsyncOperation.Begin();
					return pingClientAsyncOperation;
				});
				this.pingCompletionEvent.WaitOne();
				IntPtr intPtr;
				this.EndWrapper<PingClientAsyncOperation>(asyncResult, false, false, out intPtr, (PingClientAsyncOperation operation) => operation.End());
				ExTraceGlobals.ClientSessionContextTracer.TraceInformation<IntPtr, string, ExDateTime?>(57248, 0L, "MapiHttpClient: Refreshed session context; ContextHandle={0}, RequestGroupId={1}, Expires={2}", clientSessionContext.ContextHandle, clientSessionContext.RequestGroupId, clientSessionContext.Expires);
			}
			catch (AggregateException arg)
			{
				ExTraceGlobals.ClientSessionContextTracer.TraceInformation<IntPtr, string, AggregateException>(65440, 0L, "MapiHttpClient: Failed to refreshed session context; ContextHandle={0}, RequestGroupId={1}, Exception={2}", clientSessionContext.ContextHandle, clientSessionContext.RequestGroupId, arg);
			}
			catch (ProtocolException arg2)
			{
				ExTraceGlobals.ClientSessionContextTracer.TraceInformation<IntPtr, string, ProtocolException>(40864, 0L, "MapiHttpClient: Failed to refreshed session context; ContextHandle={0}, RequestGroupId={1}, Exception={2}", clientSessionContext.ContextHandle, clientSessionContext.RequestGroupId, arg2);
			}
		}

		private static readonly TimeSpan backgroundWakeupPeriod = TimeSpan.FromSeconds(10.0);

		private readonly MapiHttpBindingInfo bindingInfo;

		private readonly ConcurrentDictionary<IntPtr, ClientSessionContext> contextCache = new ConcurrentDictionary<IntPtr, ClientSessionContext>();

		private readonly AutoResetEvent pingCompletionEvent;

		private readonly object backgroundTimerDisposeLock = new object();

		private readonly System.Timers.Timer backgroundTimer;

		private bool isTimerDisposed;

		private int contextHandleCounter;

		private bool isBackgroundProcessing;
	}
}
