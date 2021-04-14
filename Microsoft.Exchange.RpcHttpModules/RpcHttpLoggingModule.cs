using System;
using System.Text;
using System.Web;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net.Protocols;
using Microsoft.Exchange.Security.Authentication.FederatedAuthService;

namespace Microsoft.Exchange.RpcHttpModules
{
	public class RpcHttpLoggingModule : RpcHttpModule
	{
		public RpcHttpLoggingModule() : this(RpcHttpLoggingModule.GetDefaultLogger())
		{
		}

		internal RpcHttpLoggingModule(IExtensibleLogger logger)
		{
			this.logger = logger;
		}

		internal override void InitializeModule(HttpApplication application)
		{
			application.BeginRequest += delegate(object sender, EventArgs args)
			{
				this.OnBeginRequest(new HttpContextWrapper(((HttpApplication)sender).Context));
			};
			application.PostAuthorizeRequest += delegate(object sender, EventArgs args)
			{
				this.OnPostAuthorizeRequest(new HttpContextWrapper(((HttpApplication)sender).Context));
			};
			application.EndRequest += delegate(object sender, EventArgs args)
			{
				this.OnEndRequest(new HttpContextWrapper(((HttpApplication)sender).Context));
			};
		}

		internal override void OnBeginRequest(HttpContextBase context)
		{
			context.Response.AppendToLog("&RequestId=" + base.GetRequestId(context).ToString());
			this.LogRequest(context, RpcHttpLoggingModule.HttpPipelineStage.BeginRequest);
		}

		internal override void OnPostAuthorizeRequest(HttpContextBase context)
		{
			this.LogRequest(context, RpcHttpLoggingModule.HttpPipelineStage.PostAuthorizeRequest);
		}

		internal override void OnEndRequest(HttpContextBase context)
		{
			this.LogRequest(context, RpcHttpLoggingModule.HttpPipelineStage.EndRequest);
		}

		private static IExtensibleLogger GetDefaultLogger()
		{
			if (RpcHttpLoggingModule.defaultLogger == null)
			{
				lock (RpcHttpLoggingModule.defaultLoggerLock)
				{
					if (RpcHttpLoggingModule.defaultLogger == null)
					{
						RpcHttpLoggingModule.defaultLogger = new RpcHttpLogger();
						AppDomain.CurrentDomain.DomainUnload += RpcHttpLoggingModule.DisposeDefaultLogger;
					}
				}
			}
			return RpcHttpLoggingModule.defaultLogger;
		}

		private static void DisposeDefaultLogger(object sender, EventArgs e)
		{
			lock (RpcHttpLoggingModule.defaultLoggerLock)
			{
				if (RpcHttpLoggingModule.defaultLogger != null)
				{
					RpcHttpLoggingModule.defaultLogger.Dispose();
				}
			}
		}

		private void LogRequest(HttpContextBase context, RpcHttpLoggingModule.HttpPipelineStage stage)
		{
			RpcHttpLogEvent rpcHttpLogEvent = new RpcHttpLogEvent(stage.ToString());
			rpcHttpLogEvent.UserName = this.GetUserName(context);
			rpcHttpLogEvent.OutlookSessionId = base.GetOutlookSessionId(context);
			if (stage == RpcHttpLoggingModule.HttpPipelineStage.EndRequest)
			{
				if (context.Items.Contains("ExtendedStatus"))
				{
					rpcHttpLogEvent.Status = string.Format("{0} ({1})", this.GetResponseStatusInfo(context.Response), context.Items["ExtendedStatus"]);
				}
				else
				{
					rpcHttpLogEvent.Status = this.GetResponseStatusInfo(context.Response);
				}
			}
			rpcHttpLogEvent.HttpVerb = context.Request.HttpMethod;
			rpcHttpLogEvent.UriQueryString = context.Request.Url.Query;
			rpcHttpLogEvent.ClientIp = context.Request.UserHostAddress;
			rpcHttpLogEvent.AuthType = this.GetAuthType(context);
			rpcHttpLogEvent.RpcHttpUserName = this.GetLogonUserName(context);
			rpcHttpLogEvent.ServerTarget = context.Request.Headers[WellKnownHeader.RpcHttpProxyServerTarget];
			rpcHttpLogEvent.FEServer = context.Request.Headers[WellKnownHeader.XFEServer];
			rpcHttpLogEvent.RequestId = base.GetRequestId(context).ToString();
			rpcHttpLogEvent.AssociationGuid = RpcHttpConnectionRegistrationModule.ExtractAssociationGuid(context.Request);
			this.logger.LogEvent(rpcHttpLogEvent);
		}

		private string GetUserName(HttpContextBase context)
		{
			string text = null;
			if (context.User != null)
			{
				text = (context.Items["WLID-MemberName"] as string);
				if (string.IsNullOrEmpty(text))
				{
					text = context.Request.ServerVariables["LOGON_USER"];
				}
			}
			return text;
		}

		private string GetLogonUserName(HttpContextBase context)
		{
			string result = null;
			try
			{
				string text = context.Request.Headers[WellKnownHeader.RpcHttpProxyLogonUserName];
				if (!string.IsNullOrEmpty(text))
				{
					result = Encoding.UTF8.GetString(Convert.FromBase64String(text));
				}
			}
			catch (FormatException)
			{
			}
			return result;
		}

		private string GetAuthType(HttpContextBase context)
		{
			string text = context.Items["AuthType"] as string;
			if (string.IsNullOrEmpty(text))
			{
				text = AuthServiceHelper.GetAuthType(context.Request.Headers["Authorization"]);
				context.Items["AuthType"] = text;
			}
			return text;
		}

		private string GetResponseStatusInfo(HttpResponseBase response)
		{
			return string.Format("{0}.{1}.{2}", response.StatusCode, response.SubStatusCode, response.StatusDescription);
		}

		internal const string WlidMemberItemName = "WLID-MemberName";

		internal const string LogonUserVariableName = "LOGON_USER";

		internal const string AuthTypeItemName = "AuthType";

		private static readonly object defaultLoggerLock = new object();

		private static RpcHttpLogger defaultLogger = null;

		private readonly IExtensibleLogger logger;

		internal enum HttpPipelineStage
		{
			BeginRequest,
			PostAuthorizeRequest,
			EndRequest
		}
	}
}
