using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net.MapiHttp;

namespace Microsoft.Exchange.MapiHttp
{
	public sealed class MapiHttpLoggingModule : MapiHttpModule
	{
		public MapiHttpLoggingModule()
		{
			this.perfDateTime = new PerfDateTime();
			this.logLine = new StringBuilder();
		}

		internal override void InitializeModule(HttpApplication application)
		{
			application.BeginRequest += delegate(object sender, EventArgs args)
			{
				this.OnBeginRequest(MapiHttpContextWrapper.GetWrapper(((HttpApplication)sender).Context));
			};
			application.PostAuthorizeRequest += delegate(object sender, EventArgs args)
			{
				this.OnPostAuthorizeRequest(MapiHttpContextWrapper.GetWrapper(((HttpApplication)sender).Context));
			};
			application.PreRequestHandlerExecute += delegate(object sender, EventArgs args)
			{
				this.OnPreRequestHandlerExecute(MapiHttpContextWrapper.GetWrapper(((HttpApplication)sender).Context));
			};
			application.PostRequestHandlerExecute += delegate(object sender, EventArgs args)
			{
				this.OnPostRequestHandlerExecute(MapiHttpContextWrapper.GetWrapper(((HttpApplication)sender).Context));
			};
			application.EndRequest += delegate(object sender, EventArgs args)
			{
				this.OnEndRequest(MapiHttpContextWrapper.GetWrapper(((HttpApplication)sender).Context));
			};
		}

		internal override void OnBeginRequest(HttpContextBase context)
		{
			this.beginRequestTime = new DateTime?(this.perfDateTime.UtcNow);
			context.Items["MapiHttpLoggingModuleLogger"] = this.logLine;
			this.SetDefaultResponseHeaders(context);
			this.AppendLogRequestInfo(context);
		}

		internal override void OnPostAuthorizeRequest(HttpContextBase context)
		{
			this.AppendAuthInfo(context);
			this.postAuthorizeRequestTime = new DateTime?(this.perfDateTime.UtcNow);
		}

		internal override void OnPreRequestHandlerExecute(HttpContextBase context)
		{
			this.preRequestHandlerExecuteTime = new DateTime?(this.perfDateTime.UtcNow);
		}

		internal override void OnPostRequestHandlerExecute(HttpContextBase context)
		{
			this.postRequestHandlerExecuteTime = new DateTime?(this.perfDateTime.UtcNow);
		}

		internal override void OnEndRequest(HttpContextBase context)
		{
			if (this.beginRequestTime != null)
			{
				this.logLine.AppendFormat("&Stage=BeginRequest:{0}", this.beginRequestTime.Value.ToString("o"));
				if (this.postAuthorizeRequestTime != null)
				{
					this.logLine.AppendFormat(";PostAuthorizeRequest:{0}", this.postAuthorizeRequestTime.Value.ToString("o"));
				}
				if (this.preRequestHandlerExecuteTime != null)
				{
					this.logLine.AppendFormat(";PreRequestHandlerExecute:{0}", this.preRequestHandlerExecuteTime.Value.ToString("o"));
				}
				if (this.postRequestHandlerExecuteTime != null)
				{
					this.logLine.AppendFormat(";PostRequestHandlerExecute:{0}", this.postRequestHandlerExecuteTime.Value.ToString("o"));
				}
				this.logLine.AppendFormat(";EndRequest:{0}", this.perfDateTime.UtcNow.ToString("o"));
			}
			else
			{
				this.logLine.AppendFormat("&Stage=EndRequest:{0}", this.perfDateTime.UtcNow.ToString("o"));
			}
			context.Response.AppendToLog(this.logLine.ToString());
			this.logLine.Clear();
		}

		private void AppendLogRequestInfo(HttpContextBase context)
		{
			string text;
			context.TryGetSourceCafeServer(out text);
			string text2;
			context.TryGetCafeActivityId(out text2);
			string clientRequestInfo = MapiHttpEndpoints.GetClientRequestInfo(context.Request.Headers);
			this.logLine.Append("&FrontEnd=");
			this.logLine.Append(string.IsNullOrEmpty(text) ? "<null>" : text);
			this.logLine.Append("&RequestId=");
			this.logLine.Append(string.IsNullOrEmpty(text2) ? "<null>" : text2);
			this.logLine.Append("&ClientRequestInfo=");
			this.logLine.Append(clientRequestInfo);
		}

		private void AppendAuthInfo(HttpContextBase context)
		{
			Dictionary<Enum, string> authValues = context.GetAuthValues();
			if (authValues.Count > 0)
			{
				this.logLine.Append("&AuthInfo=");
				foreach (KeyValuePair<Enum, string> keyValuePair in authValues)
				{
					this.logLine.Append(keyValuePair.Key);
					this.logLine.Append(":");
					this.logLine.Append(keyValuePair.Value);
					this.logLine.Append(";");
				}
			}
		}

		private void SetDefaultResponseHeaders(HttpContextBase context)
		{
			context.SetServerVersion();
			string text;
			if (context.TryGetRequestId(out text) && !string.IsNullOrEmpty(text))
			{
				context.SetRequestId(text);
			}
			string text2;
			if (context.TryGetClientInfo(out text2) && !string.IsNullOrEmpty(text2))
			{
				context.SetClientInfo(text2);
			}
			string text3;
			if (context.TryGetRequestType(out text3) && !string.IsNullOrEmpty(text3))
			{
				context.SetRequestType(text3);
			}
		}

		public const string AdditionalInfoLogger = "MapiHttpLoggingModuleLogger";

		private const string ClientRequestInfoLogParameter = "&ClientRequestInfo=";

		private const string FrontEndLogParameter = "&FrontEnd=";

		private const string RequestIdLogParameter = "&RequestId=";

		private const string AuthInfoLogParameter = "&AuthInfo=";

		private readonly PerfDateTime perfDateTime;

		private DateTime? beginRequestTime;

		private DateTime? postAuthorizeRequestTime;

		private DateTime? preRequestHandlerExecuteTime;

		private DateTime? postRequestHandlerExecuteTime;

		private StringBuilder logLine;
	}
}
