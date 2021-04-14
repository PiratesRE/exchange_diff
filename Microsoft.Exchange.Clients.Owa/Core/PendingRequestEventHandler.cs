using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	[OwaEventNamespace("PendingRequest")]
	internal class PendingRequestEventHandler : OwaEventHandlerBase
	{
		public PendingRequestEventHandler()
		{
			base.DontWriteHeaders = true;
		}

		[OwaEventVerb(OwaEventVerb.Get)]
		[OwaEvent("PendingNotificationRequest")]
		[OwaEventParameter("UA", typeof(bool))]
		[OwaEventParameter("n", typeof(int), false, true)]
		public IAsyncResult BeginUseNotificationPipe(AsyncCallback callback, object extraData)
		{
			ExTraceGlobals.NotificationsCallTracer.TraceDebug((long)this.GetHashCode(), "BeginUseNotificationPipe called from the client");
			HttpResponse httpResponse = base.OwaContext.HttpContext.Response;
			Utilities.DisableContentEncodingForThisResponse(httpResponse);
			httpResponse.AppendHeader("X-NoCompression", "1");
			httpResponse.AppendHeader("X-NoBuffering", "1");
			base.OwaContext.DoNotTriggerLatencyDetectionReport();
			this.pendingManager = base.OwaContext.UserContext.PendingRequestManager;
			bool isFullyInitialized = base.UserContext.IsFullyInitialized;
			int num;
			base.UserContext.DangerousBeginUnlockedAction(false, out num);
			if (num != 1)
			{
				ExWatson.SendReport(new InvalidOperationException("Thread held more than 1 lock before async operation"), ReportOptions.None, null);
			}
			this.asyncResult = new OwaAsyncResult(callback, extraData);
			try
			{
				this.response = new ChunkedHttpResponse(this.HttpContext);
				this.pendingManager.BeginSendNotification(new AsyncCallback(this.UseNotificationPipeCallback), this.response, isFullyInitialized, this);
			}
			catch (Exception ex)
			{
				ExTraceGlobals.NotificationsCallTracer.TraceError<Exception>((long)this.GetHashCode(), "An exception happened while executing BeginUseNotificationPipe. Exception:{0}", ex);
				lock (this.asyncResult)
				{
					if (!this.asyncResult.IsCompleted)
					{
						this.asyncResult.CompleteRequest(true, ex);
					}
				}
			}
			return this.asyncResult;
		}

		[OwaEvent("PendingNotificationRequest")]
		public void EndUseNotificationPipe(IAsyncResult async)
		{
			if (this.isDisposing)
			{
				return;
			}
			OwaAsyncResult owaAsyncResult = (OwaAsyncResult)async;
			if (HttpContext.Current != null && OwaContext.Current != base.OwaContext)
			{
				base.OwaContext.IgnoreUnlockForcefully = true;
			}
			try
			{
				if (owaAsyncResult.Exception != null)
				{
					bool isBasicAuthentication = Utilities.IsBasicAuthentication(base.OwaContext.HttpContext.Request);
					ErrorInformation exceptionHandlingInformation = Utilities.GetExceptionHandlingInformation(owaAsyncResult.Exception, base.OwaContext.MailboxIdentity, Utilities.IsWebPartRequest(base.OwaContext), string.Empty, string.Empty, isBasicAuthentication);
					Exception ex = (owaAsyncResult.Exception.InnerException == null) ? owaAsyncResult.Exception : owaAsyncResult.Exception.InnerException;
					ExTraceGlobals.NotificationsCallTracer.TraceDebug((long)this.GetHashCode(), "An exception was thrown during the processing of the async request");
					StringBuilder stringBuilder = new StringBuilder();
					using (StringWriter stringWriter = new StringWriter(stringBuilder, CultureInfo.InvariantCulture))
					{
						OwaEventHttpHandler.RenderErrorDiv(base.OwaContext, stringWriter, exceptionHandlingInformation.Message, exceptionHandlingInformation.MessageDetails, exceptionHandlingInformation.OwaEventHandlerErrorCode, exceptionHandlingInformation.HideDebugInformation ? null : ex);
					}
					if (this.response != null)
					{
						this.response.WriteError(Utilities.JavascriptEncode(stringBuilder.ToString()));
						base.OwaContext.ErrorSent = true;
					}
				}
				if (this.response != null)
				{
					this.response.WriteLastChunk();
				}
			}
			catch (OwaNotificationPipeWriteException)
			{
			}
			finally
			{
				if (owaAsyncResult.Exception != null)
				{
					Utilities.HandleException(base.OwaContext, owaAsyncResult.Exception);
				}
			}
		}

		[OwaEvent("FinishNotificationRequest")]
		[OwaEventParameter("Fn", typeof(bool), false, true)]
		[OwaEventVerb(OwaEventVerb.Post)]
		public void DisposePendingNotificationClientRequest()
		{
			if (base.OwaContext.UserContext == null)
			{
				return;
			}
			this.pendingManager = base.OwaContext.UserContext.PendingRequestManager;
			object parameter = base.GetParameter("Fn");
			if (parameter != null)
			{
				bool flag = (bool)parameter;
			}
			int num = 0;
			try
			{
				base.UserContext.DangerousBeginUnlockedAction(false, out num);
				if (num != 1)
				{
					ExWatson.SendReport(new InvalidOperationException("Thread held more than 1 lock in DisposePendingNotificationClientRequest method."), ReportOptions.None, null);
				}
				bool flag2 = this.pendingManager.HandleFinishRequestFromClient();
				HttpResponse httpResponse = base.OwaContext.HttpContext.Response;
				httpResponse.Write("var syncFnshRq = ");
				httpResponse.Write(flag2 ? "1" : "0");
				httpResponse.AppendHeader("X-OWA-EventResult", "0");
				Utilities.MakePageNoCacheNoStore(httpResponse);
				httpResponse.ContentType = Utilities.GetContentTypeString(base.ResponseContentType);
			}
			finally
			{
				base.UserContext.DangerousEndUnlockedAction(false, num);
			}
		}

		private void UseNotificationPipeCallback(IAsyncResult async)
		{
			OwaAsyncResult owaAsyncResult = (OwaAsyncResult)async;
			try
			{
				this.pendingManager.EndSendNotification(owaAsyncResult);
			}
			catch (Exception exception)
			{
				lock (this.asyncResult)
				{
					if (!this.asyncResult.IsCompleted)
					{
						this.asyncResult.Exception = exception;
					}
				}
			}
			try
			{
				lock (this.asyncResult)
				{
					if (!this.asyncResult.IsCompleted)
					{
						this.asyncResult.CompleteRequest(owaAsyncResult.CompletedSynchronously);
					}
				}
			}
			finally
			{
				this.pendingManager.RecordFinishPendingRequest();
			}
		}

		protected override void InternalDispose(bool isExplicitDispose)
		{
			if (isExplicitDispose && !this.isDisposing && this.asyncResult != null)
			{
				lock (this.asyncResult)
				{
					if (!this.asyncResult.IsCompleted)
					{
						this.isDisposing = true;
						this.asyncResult.CompleteRequest(false);
					}
				}
			}
		}

		internal static bool IsObsoleteRequest(OwaContext owaContext, UserContext userContext)
		{
			if (owaContext.RequestType == OwaRequestType.Oeh && userContext == null)
			{
				string queryStringParameter = Utilities.GetQueryStringParameter(owaContext.HttpContext.Request, "ns", false);
				string queryStringParameter2 = Utilities.GetQueryStringParameter(owaContext.HttpContext.Request, "ev", false);
				string queryStringParameter3 = Utilities.GetQueryStringParameter(owaContext.HttpContext.Request, "Fn", false);
				return queryStringParameter == "PendingRequest" && queryStringParameter2 == "FinishNotificationRequest" && queryStringParameter3 == "1";
			}
			return false;
		}

		private const string UserActivityParameter = "UA";

		private const string FinalizeNotifiersParameter = "Fn";

		internal const string EventNameSpace = "PendingRequest";

		internal const string MethodPendingNotificationRequest = "PendingNotificationRequest";

		internal const string MethodFinishNotificationRequest = "FinishNotificationRequest";

		private const string UniquePendingGetIdentifierParameter = "n";

		private OwaAsyncResult asyncResult;

		private ChunkedHttpResponse response;

		private PendingRequestManager pendingManager;

		private volatile bool isDisposing;
	}
}
