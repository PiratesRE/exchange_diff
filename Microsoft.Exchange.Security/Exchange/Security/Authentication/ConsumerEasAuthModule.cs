using System;
using System.Security.Principal;
using System.Web;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Security;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Security.Authentication
{
	public class ConsumerEasAuthModule : IHttpModule
	{
		void IHttpModule.Init(HttpApplication application)
		{
			application.AddOnAuthenticateRequestAsync(new BeginEventHandler(this.BeginOnAuthenticate), new EndEventHandler(this.EndOnAuthenticate));
		}

		void IHttpModule.Dispose()
		{
		}

		private IAsyncResult BeginOnAuthenticate(object source, EventArgs args, AsyncCallback callback, object state)
		{
			return this.InternalBeginOnAuthenticate(source, args, callback, state);
		}

		private IAsyncResult InternalBeginOnAuthenticate(object source, EventArgs args, AsyncCallback callback, object state)
		{
			this.TraceEnterFunction("BeginOnAuthenticate");
			ExWatson.SendReportOnUnhandledException(delegate()
			{
				this.application = (HttpApplication)source;
				HttpContext context = this.application.Context;
				if (this.asyncOp != null)
				{
					ExTraceGlobals.AuthenticationTracer.TraceError((long)this.GetHashCode(), "BeginOnAuthenticate called with existing asyncOp");
					throw new InvalidOperationException("this.asyncOp should be null");
				}
				this.asyncOp = new LazyAsyncResult(this, state, callback);
				string text = context.Request.Headers["Authorization"];
				if (!string.IsNullOrEmpty(text) && text.StartsWith("RpsToken ", StringComparison.OrdinalIgnoreCase))
				{
					GenericIdentity identity = new GenericIdentity(AuthCommon.MemberNameNullSid.ToString(), "RpsTokenAuth");
					this.application.Context.User = new GenericPrincipal(identity, null);
				}
				this.asyncOp.InvokeCallback();
			}, (object exception) => true, ReportOptions.DoNotCollectDumps | ReportOptions.DeepStackTraceHash);
			this.TraceExitFunction("BeginOnAuthenticate");
			return this.asyncOp;
		}

		private void EndOnAuthenticate(IAsyncResult asyncResult)
		{
			this.InternalEndOnAuthenticate(asyncResult);
		}

		private void InternalEndOnAuthenticate(IAsyncResult asyncResult)
		{
			this.TraceEnterFunction("EndOnAuthenticate");
			if (asyncResult == null)
			{
				throw new ArgumentNullException("asyncResult");
			}
			LazyAsyncResult lazyAsyncResult = (LazyAsyncResult)asyncResult;
			if (!lazyAsyncResult.IsCompleted)
			{
				lazyAsyncResult.InternalWaitForCompletion();
			}
			this.asyncOp = null;
			this.TraceExitFunction("EndOnAuthenticate");
		}

		private void TraceEnterFunction(string functionName)
		{
			ExTraceGlobals.AuthenticationTracer.TraceFunction<string>((long)this.GetHashCode(), "Enter Function: LiveIdBasicAuthModule.{0}.", functionName);
		}

		private void TraceExitFunction(string functionName)
		{
			ExTraceGlobals.AuthenticationTracer.TraceFunction<string>((long)this.GetHashCode(), "Exit Function: LiveIdBasicAuthModule.{0}.", functionName);
		}

		internal const string RpsTokenAuthType = "RpsTokenAuth";

		private const string RpsTokenAuthHeaderType = "RpsToken ";

		private HttpApplication application;

		private LazyAsyncResult asyncOp;
	}
}
