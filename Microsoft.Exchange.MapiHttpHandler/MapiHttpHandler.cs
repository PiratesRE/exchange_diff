using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Rpc;

namespace Microsoft.Exchange.MapiHttp
{
	internal abstract class MapiHttpHandler : IHttpAsyncHandler, IHttpHandler
	{
		public bool IsReusable
		{
			get
			{
				return true;
			}
		}

		internal static Action ShutdownHandlerDelegate
		{
			set
			{
				MapiHttpHandler.delegateShutdownHandler = value;
			}
		}

		internal static Func<object, bool> IsValidContextHandleDelegate
		{
			set
			{
				MapiHttpHandler.delegateIsValidContextHandle = value;
			}
		}

		internal static Func<object, bool> TryContextHandleRundownDelegate
		{
			set
			{
				MapiHttpHandler.delegateTryContextHandleRundown = value;
			}
		}

		internal static Action<object> QueueDroppedConnectionDelegate
		{
			set
			{
				MapiHttpHandler.delegateQueueDroppedConnection = value;
			}
		}

		internal static Func<string, bool> NeedTokenRehydrationDelegate
		{
			set
			{
				MapiHttpHandler.delegateNeedTokenRehydration = value;
			}
		}

		internal static bool CanTrustEntireForwardedForHeader
		{
			get
			{
				if (MapiHttpHandler.canTrustEntireForwardedForHeader == null)
				{
					MapiHttpHandler.canTrustEntireForwardedForHeader = new bool?(MapiHttpHandler.ReadBoolAppSetting("TrustEntireForwardedFor", false));
				}
				return MapiHttpHandler.canTrustEntireForwardedForHeader.Value;
			}
		}

		internal static bool UseBufferedReadStream
		{
			get
			{
				if (MapiHttpHandler.useBufferedReadStream == null)
				{
					MapiHttpHandler.useBufferedReadStream = new bool?(MapiHttpHandler.ReadBoolAppSetting("UseBufferedReadStream", false));
				}
				return MapiHttpHandler.useBufferedReadStream.Value;
			}
		}

		internal static TimeSpan ClientTunnelExpirationTime
		{
			get
			{
				if (MapiHttpHandler.clientTunnelExpirationTime == null)
				{
					MapiHttpHandler.clientTunnelExpirationTime = new TimeSpan?(MapiHttpHandler.ReadTimeSpanAppSetting("ClientTunnelExpirationTime", (double x) => TimeSpan.FromMinutes(x), Constants.ClientTunnelExpirationTimeMin, Constants.ClientTunnelExpirationTimeMax, Constants.ClientTunnelExpirationTimeDefault));
				}
				return MapiHttpHandler.clientTunnelExpirationTime.Value;
			}
		}

		internal abstract string EndpointVdirPath { get; }

		internal abstract IAsyncOperationFactory OperationFactory { get; }

		public void ProcessRequest(HttpContext context)
		{
			throw new NotSupportedException("Handler not synchronously callable.");
		}

		public IAsyncResult BeginProcessRequest(HttpContext context, AsyncCallback asyncCallback, object asyncState)
		{
			MapiHttpRequestState mapiHttpRequestState = new MapiHttpRequestState(this, MapiHttpContextWrapper.GetWrapper(context), asyncCallback, asyncState);
			mapiHttpRequestState.Begin();
			return mapiHttpRequestState;
		}

		public void EndProcessRequest(IAsyncResult asyncResult)
		{
			MapiHttpRequestState mapiHttpRequestState = asyncResult as MapiHttpRequestState;
			if (mapiHttpRequestState == null)
			{
				throw new InvalidOperationException("IAsyncResult isn't a MapiHttpRequestState object.");
			}
			mapiHttpRequestState.End();
		}

		internal static async Task<MapiHttpDispatchedCallResult> DispatchCallAsync(Func<AsyncCallback, object, IAsyncResult> beginMethod, Action<IAsyncResult> endMethod)
		{
			uint statusCode = 0U;
			Exception thrownException = null;
			try
			{
				await Task.Factory.FromAsync((AsyncCallback asyncCallback, object asyncState) => beginMethod(delegate(IAsyncResult asyncResult)
				{
					if (!ThreadPool.QueueUserWorkItem(delegate(object state)
					{
						asyncCallback(asyncResult);
					}))
					{
						asyncCallback(asyncResult);
					}
				}, asyncState), endMethod, null);
			}
			catch (Exception ex)
			{
				thrownException = ex;
				if (!MapiHttpHandler.TryHandleException(ex, out statusCode))
				{
					throw;
				}
			}
			return new MapiHttpDispatchedCallResult(statusCode, thrownException);
		}

		internal static uint DispatchCallSync(Action callMethod)
		{
			uint result = 0U;
			try
			{
				callMethod();
			}
			catch (Exception exception)
			{
				if (!MapiHttpHandler.TryHandleException(exception, out result))
				{
					throw;
				}
			}
			return result;
		}

		internal static void ShutdownHandler()
		{
			if (MapiHttpHandler.delegateShutdownHandler != null)
			{
				MapiHttpHandler.delegateShutdownHandler();
			}
		}

		internal static bool IsValidContextHandle(object contextHandle)
		{
			return MapiHttpHandler.delegateIsValidContextHandle != null && MapiHttpHandler.delegateIsValidContextHandle(contextHandle);
		}

		internal static bool TryContextHandleRundown(object contextHandle)
		{
			return MapiHttpHandler.delegateTryContextHandleRundown != null && MapiHttpHandler.delegateTryContextHandleRundown(contextHandle);
		}

		internal static void QueueDroppedConnection(object contextHandle)
		{
			if (contextHandle != null && MapiHttpHandler.delegateQueueDroppedConnection != null)
			{
				MapiHttpHandler.delegateQueueDroppedConnection(contextHandle);
			}
		}

		internal static bool NeedTokenRehydration(string requestType)
		{
			return MapiHttpHandler.delegateNeedTokenRehydration == null || MapiHttpHandler.delegateNeedTokenRehydration(requestType);
		}

		internal AsyncOperation BuildAsyncOperation(string requestType, HttpContextBase context)
		{
			return this.OperationFactory.Create(requestType, context);
		}

		internal abstract bool TryEnsureHandlerIsInitialized();

		internal abstract void LogFailure(IList<string> requestIds, IList<string> cookies, string message, string userName, string protocolSequence, string clientAddress, string organization, Exception exception, Trace trace);

		private static bool TryHandleException(Exception exception, out uint statusCode)
		{
			statusCode = 0U;
			AggregateException ex = exception as AggregateException;
			if (ex != null)
			{
				foreach (Exception exception2 in ex.InnerExceptions)
				{
					if (MapiHttpHandler.TryHandleException(exception2, out statusCode))
					{
						return true;
					}
				}
			}
			RpcException ex2 = exception as RpcException;
			if (ex2 != null)
			{
				statusCode = (uint)ex2.ErrorCode;
				return true;
			}
			if (exception is ThreadAbortException)
			{
				statusCode = 1726U;
				return true;
			}
			if (exception is OutOfMemoryException)
			{
				statusCode = 14U;
				return true;
			}
			return false;
		}

		private static bool ReadBoolAppSetting(string appSettingName, bool appSettingDefault)
		{
			string value = WebConfigurationManager.AppSettings[appSettingName];
			bool result;
			if (!string.IsNullOrEmpty(value) && bool.TryParse(value, out result))
			{
				return result;
			}
			return appSettingDefault;
		}

		private static TimeSpan ReadTimeSpanAppSetting(string appSettingName, Func<double, TimeSpan> timeSpanConversion, TimeSpan timeSpanMin, TimeSpan timeSpanMax, TimeSpan timeSpanDefault)
		{
			string text = WebConfigurationManager.AppSettings[appSettingName];
			double num;
			if (!string.IsNullOrEmpty(text) && double.TryParse(text, out num) && num >= 0.0)
			{
				TimeSpan timeSpan = timeSpanConversion(num);
				if (timeSpan < timeSpanMin)
				{
					timeSpan = timeSpanMin;
				}
				else if (timeSpan > timeSpanMax)
				{
					timeSpan = timeSpanMax;
				}
				return timeSpan;
			}
			return timeSpanDefault;
		}

		private static Action delegateShutdownHandler = null;

		private static Func<object, bool> delegateIsValidContextHandle = null;

		private static Func<object, bool> delegateTryContextHandleRundown = null;

		private static Action<object> delegateQueueDroppedConnection = null;

		private static Func<string, bool> delegateNeedTokenRehydration = null;

		private static bool? canTrustEntireForwardedForHeader = null;

		private static bool? useBufferedReadStream = null;

		private static TimeSpan? clientTunnelExpirationTime = null;
	}
}
