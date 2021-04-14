using System;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Web;
using System.Web.Configuration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Net.Protocols;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.RpcHttpModules
{
	public class RpcHttpConnectionRegistrationModule : RpcHttpModule
	{
		public RpcHttpConnectionRegistrationModule() : this(CachedDirectory.DefaultCachedDirectory, new RpcHttpConnectionRegistrationClient())
		{
		}

		internal RpcHttpConnectionRegistrationModule(IDirectory directoryService, IRpcHttpConnectionRegistration rpcHttpConnectionRegistration)
		{
			this.directoryService = directoryService;
			this.rpcHttpConnectionRegistration = rpcHttpConnectionRegistration;
			this.EnsureConnectionRegistrationCacheIsInitialized();
		}

		private bool CanTrustEntireForwardedForHeader
		{
			get
			{
				bool flag;
				return bool.TryParse(WebConfigurationManager.AppSettings["TrustEntireForwardedFor"], out flag) && flag;
			}
		}

		internal static string ExtractAssociationGuid(HttpRequestBase httpRequest)
		{
			string text = httpRequest.Headers[WellKnownHeader.Pragma];
			if (!string.IsNullOrEmpty(text))
			{
				int num = text.IndexOf("SessionId=", StringComparison.Ordinal);
				if (num >= 0 && num + "SessionId=".Length + 36 <= text.Length)
				{
					return text.Substring(num + "SessionId=".Length, 36);
				}
			}
			return null;
		}

		internal override void InitializeModule(HttpApplication application)
		{
			application.AuthorizeRequest += delegate(object sender, EventArgs args)
			{
				this.OnAuthorizeRequest(new HttpContextWrapper(((HttpApplication)sender).Context));
			};
			application.EndRequest += delegate(object sender, EventArgs args)
			{
				this.OnEndRequest(new HttpContextWrapper(((HttpApplication)sender).Context));
			};
		}

		internal override void OnAuthorizeRequest(HttpContextBase context)
		{
			HttpRequestBase request = context.Request;
			if (request.HttpMethod != "RPC_OUT_DATA" && request.HttpMethod != "RPC_IN_DATA")
			{
				return;
			}
			string serverTarget = this.GetServerTarget(request);
			if (request.IsSecureConnection)
			{
				CommonAccessToken commonAccessToken = context.Items["Item-CommonAccessToken"] as CommonAccessToken;
				string text;
				bool flag;
				if (commonAccessToken != null)
				{
					text = commonAccessToken.Serialize();
					flag = true;
				}
				else
				{
					text = request.Headers["X-CommonAccessToken"];
					if (string.IsNullOrEmpty(text))
					{
						WindowsIdentity windowsIdentity = context.User.Identity as WindowsIdentity;
						if (windowsIdentity == null)
						{
							return;
						}
						text = new CommonAccessToken(windowsIdentity).Serialize();
						flag = true;
					}
					else
					{
						flag = this.IsTokenSerializationAllowed(context);
					}
				}
				Guid guid;
				Guid.TryParse(RpcHttpConnectionRegistrationModule.ExtractAssociationGuid(request), out guid);
				context.Items["associationGuid"] = guid;
				string text2 = base.GetOutlookSessionId(context) ?? string.Empty;
				string text3 = string.Empty;
				IPAddress ipaddress;
				IPAddress ipaddress2;
				if (GccUtils.GetClientIPEndpointsFromHttpRequest(context, out ipaddress, out ipaddress2, false, this.CanTrustEntireForwardedForHeader) && ipaddress != null && ipaddress != IPAddress.IPv6None)
				{
					text3 = ipaddress.ToString();
				}
				Guid requestId = base.GetRequestId(context);
				if (!flag)
				{
					base.SendErrorResponse(context, 403, "Insufficient permission");
					return;
				}
				if (request.HttpMethod == "RPC_IN_DATA")
				{
					context.SetServerVariable("ShimServerTarget", serverTarget);
					context.SetServerVariable("ShimAccessToken", text);
					context.SetServerVariable("ShimSessionCookie", text2);
					context.SetServerVariable("ShimForwardedFor", text3);
					context.SetServerVariable("ShimRequestId", requestId.ToString("D"));
					return;
				}
				if (request.HttpMethod == "RPC_OUT_DATA" && guid != Guid.Empty && !string.IsNullOrEmpty(serverTarget))
				{
					this.RegisterRequest(context, guid, text, serverTarget, text2, text3, requestId);
					return;
				}
			}
			else if (!string.IsNullOrEmpty(request.Headers["X-CommonAccessToken"]) || !this.IsRequestFromExchangeServer(context))
			{
				base.SendErrorResponse(context, 403, "Insufficient permission");
			}
		}

		internal override void OnEndRequest(HttpContextBase context)
		{
			if (context.Items.Contains("connectionIsRegistered") && context.Items.Contains("associationGuid"))
			{
				Guid associationGroupId = (Guid)context.Items["associationGuid"];
				Guid requestId = base.GetRequestId(context);
				this.UnregisterRequest(associationGroupId, requestId);
			}
		}

		private string GetServerTarget(HttpRequestBase httpRequest)
		{
			string text = httpRequest.Headers[WellKnownHeader.RpcHttpProxyServerTarget];
			if (string.IsNullOrEmpty(text))
			{
				text = new RpcHttpQueryString(httpRequest.Url.Query).RcaServer;
			}
			return text;
		}

		private void EnsureConnectionRegistrationCacheIsInitialized()
		{
			if (!RpcHttpConnectionRegistrationModule.haveClearedRegistrationCache)
			{
				lock (RpcHttpConnectionRegistrationModule.ClearCacheLock)
				{
					if (!RpcHttpConnectionRegistrationModule.haveClearedRegistrationCache)
					{
						this.ClearRegistrations();
						RpcHttpConnectionRegistrationModule.haveClearedRegistrationCache = true;
					}
				}
			}
		}

		private bool IsTokenSerializationAllowed(HttpContextBase httpContext)
		{
			WindowsIdentity windowsIdentity = httpContext.User.Identity as WindowsIdentity;
			return windowsIdentity != null && !(windowsIdentity.User == null) && this.directoryService.AllowsTokenSerializationBy(windowsIdentity);
		}

		private bool IsRequestFromExchangeServer(HttpContextBase httpContext)
		{
			bool result = false;
			WindowsIdentity windowsIdentity = (WindowsIdentity)httpContext.User.Identity;
			if (windowsIdentity.User != null && windowsIdentity.User.IsWellKnown(WellKnownSidType.LocalSystemSid))
			{
				result = true;
			}
			else if (windowsIdentity.Groups != null)
			{
				SecurityIdentifier exchangeServersSid = this.directoryService.GetExchangeServersUsgSid();
				if (exchangeServersSid != null)
				{
					if ((from identityReference in windowsIdentity.Groups
					select identityReference as SecurityIdentifier).Any((SecurityIdentifier sid) => sid == exchangeServersSid))
					{
						result = true;
					}
				}
			}
			return result;
		}

		private void RegisterRequest(HttpContextBase context, Guid associationGroupId, string token, string serverTarget, string sessionCookie, string forwardedFor, Guid requestId)
		{
			try
			{
				string text = null;
				string value = null;
				int num = this.rpcHttpConnectionRegistration.Register(associationGroupId, token, serverTarget, sessionCookie, forwardedFor, requestId, out text, out value);
				if (num != 0)
				{
					string httpStatusText = string.Format("Connection Registration Failed:{0} ({1})", num, text ?? string.Empty);
					if (!string.IsNullOrEmpty(value))
					{
						context.Items["ExtendedStatus"] = value;
					}
					base.SendErrorResponse(context, 500, num, httpStatusText);
				}
				else
				{
					context.Items["connectionIsRegistered"] = true;
				}
			}
			catch (RpcHttpConnectionRegistrationException ex)
			{
				context.Items["ExtendedStatus"] = ex.ToString();
				base.SendErrorResponse(context, 500, ex.ErrorCode, "Connection Registration Threw Exception: " + ex.Message);
			}
		}

		private void UnregisterRequest(Guid associationGroupId, Guid requestId)
		{
			try
			{
				this.rpcHttpConnectionRegistration.Unregister(associationGroupId, requestId);
			}
			catch (RpcHttpConnectionRegistrationInternalException)
			{
			}
			catch (RpcHttpConnectionRegistrationException)
			{
			}
		}

		private void ClearRegistrations()
		{
			try
			{
				this.rpcHttpConnectionRegistration.Clear();
			}
			catch (RpcHttpConnectionRegistrationInternalException)
			{
			}
			catch (RpcHttpConnectionRegistrationException)
			{
			}
		}

		public const string RpcInMethod = "RPC_IN_DATA";

		public const string RpcOutMethod = "RPC_OUT_DATA";

		public const string TrustEntireForwardedForConfigurationKey = "TrustEntireForwardedFor";

		internal const string MissingPuidError = "Password sync failure because of missing PUID";

		internal const string MissingMemberNameError = "Password sync failure because of missing MemberName";

		internal const string PasswordSyncResultPrefix = "Password sync result";

		private const int AssociationGuidStringLength = 36;

		private const string AssociationGuidContextKey = "associationGuid";

		private const string ConnectionIsRegisteredContextKey = "connectionIsRegistered";

		private static readonly object ClearCacheLock = new object();

		private static bool haveClearedRegistrationCache = false;

		private readonly IDirectory directoryService;

		private readonly IRpcHttpConnectionRegistration rpcHttpConnectionRegistration;
	}
}
