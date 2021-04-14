using System;
using System.Security.Principal;
using System.Web;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics.CmdletInfra;
using Microsoft.Exchange.Diagnostics.Components.Configuration.Core;
using Microsoft.Exchange.Security.Authentication;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.Security.OAuth;

namespace Microsoft.Exchange.Configuration.Core
{
	public class BuildUserTokenModule : IHttpModule
	{
		void IHttpModule.Init(HttpApplication context)
		{
			context.AuthenticateRequest += this.OnAuthenticateRequest;
		}

		void IHttpModule.Dispose()
		{
		}

		private void OnAuthenticateRequest(object sender, EventArgs eventArgs)
		{
			ExTraceGlobals.HttpModuleTracer.TraceDebug((long)this.GetHashCode(), "[BuildUserTokenModule.OnAuthenticate] Enter.");
			HttpApplication httpApplication = (HttpApplication)sender;
			HttpContext context = httpApplication.Context;
			if (context.User == null)
			{
				ExTraceGlobals.HttpModuleTracer.TraceError((long)this.GetHashCode(), "[BuildUserTokenModule.OnAuthenticate] Request is Unauthorized.");
				HttpLogger.SafeAppendGenericError("BuildUerTokenModule", "context.User is NULL", false);
				context.Response.StatusCode = 401;
				httpApplication.CompleteRequest();
				return;
			}
			UserToken userToken = this.BuildCurrentUserToken();
			if (userToken == null)
			{
				ExTraceGlobals.HttpModuleTracer.TraceError((long)this.GetHashCode(), "[BuildUserTokenModule.OnAuthenticate] Request is Unauthorized.");
				HttpLogger.SafeAppendGenericError("BuildUerTokenModule", "userToken is NULL", true);
				context.Response.StatusCode = 401;
				httpApplication.CompleteRequest();
				return;
			}
			context.Items["X-EX-UserToken"] = userToken;
			ExTraceGlobals.HttpModuleTracer.TraceDebug((long)this.GetHashCode(), "[BuildUserTokenModule.OnAuthenticate] Leave.");
		}

		private UserToken BuildCurrentUserToken()
		{
			HttpContext httpContext = HttpContext.Current;
			CommonAccessToken commonAccessToken = httpContext.Items["Item-CommonAccessToken"] as CommonAccessToken;
			IIdentity identity = httpContext.User.Identity;
			AccessTokenType accessTokenType;
			return new UserToken(this.GetAuthenticationType(commonAccessToken, identity, out accessTokenType), httpContext.User as DelegatedPrincipal, this.GetWindowsLiveId(commonAccessToken, accessTokenType), this.GetUserName(commonAccessToken, identity, accessTokenType), this.GetUserSid(commonAccessToken, identity), this.GetPartitionId(commonAccessToken, identity), this.GetOrganization(commonAccessToken, identity, accessTokenType), this.GetManagedOrganization(identity), this.GetAppPasswordUsed(commonAccessToken), commonAccessToken);
		}

		private AuthenticationType GetAuthenticationType(CommonAccessToken commonAccessToken, IIdentity identity, out AccessTokenType accessTokenType)
		{
			AuthenticationType result = AuthenticationType.Unknown;
			accessTokenType = AccessTokenType.Anonymous;
			if (commonAccessToken != null)
			{
				if (!Enum.TryParse<AccessTokenType>(commonAccessToken.TokenType, true, out accessTokenType))
				{
					HttpLogger.SafeAppendGenericError("Invalid-CAT-Type", accessTokenType.ToString(), false);
					ExTraceGlobals.UserTokenTracer.TraceError<string>((long)this.GetHashCode(), "[BuildUserTokenModule::GetAuthenticationType] Invalid token type {0}", commonAccessToken.TokenType);
					throw new ArgumentException(string.Format("The token type {0} is unexpected.", commonAccessToken.TokenType));
				}
				ExTraceGlobals.UserTokenTracer.TraceDebug<AccessTokenType>((long)this.GetHashCode(), "[BuildUserTokenModule::GetAuthenticationType] Access token type {0}", accessTokenType);
				switch (accessTokenType)
				{
				case AccessTokenType.Windows:
					return AuthenticationType.Kerberos;
				case AccessTokenType.LiveIdBasic:
					return AuthenticationType.LiveIdBasic;
				case AccessTokenType.LiveIdNego2:
					return AuthenticationType.LiveIdNego2;
				case AccessTokenType.OAuth:
					return AuthenticationType.OAuth;
				case AccessTokenType.CertificateSid:
					return AuthenticationType.Certificate;
				case AccessTokenType.RemotePowerShellDelegated:
					return AuthenticationType.RemotePowerShellDelegated;
				}
				HttpLogger.SafeAppendGenericError("NotSupported-CAT-Type", accessTokenType.ToString(), false);
				ExTraceGlobals.UserTokenTracer.TraceError<AccessTokenType>((long)this.GetHashCode(), "[BuildUserTokenModule::GetAuthenticationType] Not supported token type {0}", accessTokenType);
				throw new NotSupportedException(string.Format("Unknown token type {0}", accessTokenType));
			}
			else if (identity is SidOAuthIdentity)
			{
				result = AuthenticationType.OAuth;
			}
			return result;
		}

		private string GetWindowsLiveId(CommonAccessToken commonAccessToken, AccessTokenType accessTokenType)
		{
			if (accessTokenType != AccessTokenType.LiveIdBasic)
			{
				ExTraceGlobals.UserTokenTracer.TraceDebug<AccessTokenType>((long)this.GetHashCode(), "[BuildUserTokenModule::GetWindowsLiveId] accessTokenType={0}", accessTokenType);
				return null;
			}
			if (commonAccessToken == null)
			{
				ExTraceGlobals.UserTokenTracer.TraceDebug<AccessTokenType>((long)this.GetHashCode(), "[BuildUserTokenModule::GetWindowsLiveId] commonAccessToken={0}", accessTokenType);
				return null;
			}
			if (!commonAccessToken.ExtensionData.ContainsKey("MemberName"))
			{
				ExTraceGlobals.UserTokenTracer.TraceDebug((long)this.GetHashCode(), "[BuildUserTokenModule::GetWindowsLiveId] CAT doesn't contain MemberName");
				return null;
			}
			return commonAccessToken.ExtensionData["MemberName"];
		}

		private string GetUserName(CommonAccessToken commonAccessToken, IIdentity identity, AccessTokenType accessTokenType)
		{
			string windowsLiveId = this.GetWindowsLiveId(commonAccessToken, accessTokenType);
			if (!string.IsNullOrWhiteSpace(windowsLiveId))
			{
				ExTraceGlobals.UserTokenTracer.TraceDebug<string>((long)this.GetHashCode(), "[BuildUserTokenModule::GetUserName] From windows Live Id {0}", windowsLiveId);
				return windowsLiveId;
			}
			if (commonAccessToken != null && commonAccessToken.WindowsAccessToken != null)
			{
				ExTraceGlobals.UserTokenTracer.TraceDebug((long)this.GetHashCode(), "[BuildUserTokenModule::GetUserName] From windows access token");
				return commonAccessToken.WindowsAccessToken.LogonName ?? commonAccessToken.WindowsAccessToken.UserSid;
			}
			DelegatedPrincipal delegatedPrincipal = HttpContext.Current.User as DelegatedPrincipal;
			if (delegatedPrincipal != null)
			{
				ExTraceGlobals.UserTokenTracer.TraceDebug<string>((long)this.GetHashCode(), "[BuildUserTokenModule::GetUserName] From delegated principal {0}", delegatedPrincipal.ToString());
				return delegatedPrincipal.GetUserName();
			}
			try
			{
				return identity.GetSafeName(true);
			}
			catch (Exception ex)
			{
				ExTraceGlobals.UserTokenTracer.TraceError<Exception>((long)this.GetHashCode(), "[BuildUserTokenModule::GetUserName] GetSafeName throws exception {0}", ex);
				HttpLogger.SafeAppendGenericError("BuildUserTokenModule.GetUserName", ex.ToString(), false);
			}
			SecurityIdentifier userSid = this.GetUserSid(commonAccessToken, identity);
			if (userSid != null)
			{
				ExTraceGlobals.UserTokenTracer.TraceDebug<string>((long)this.GetHashCode(), "[BuildUserTokenModule::GetUserName] From user sid {0}", userSid.ToString());
				return userSid.ToString();
			}
			return null;
		}

		private SecurityIdentifier GetUserSid(CommonAccessToken commonAccessToken, IIdentity identity)
		{
			if (!(HttpContext.Current.User is DelegatedPrincipal))
			{
				try
				{
					SecurityIdentifier securityIdentifier = identity.GetSecurityIdentifier();
					ExTraceGlobals.HttpModuleTracer.TraceDebug<string>((long)this.GetHashCode(), "[BuildUserTokenModule::GetUserSid] Get user sid from HttpContext.Current.User. sid = {0}", securityIdentifier.ToString());
					return securityIdentifier;
				}
				catch (Exception ex)
				{
					Exception ex3;
					ExTraceGlobals.HttpModuleTracer.TraceError<Exception>((long)this.GetHashCode(), "[BuildUserTokenModule::GetUserSid] Exception {0}", ex3);
					HttpLogger.SafeAppendGenericError("BuildUserTokenModule.GetUserSid", ex3, (Exception ex) => false);
				}
			}
			HttpContext httpContext = HttpContext.Current;
			string text = null;
			if (commonAccessToken != null && commonAccessToken.ExtensionData.ContainsKey("UserSid"))
			{
				text = commonAccessToken.ExtensionData["UserSid"];
				ExTraceGlobals.HttpModuleTracer.TraceDebug<string>((long)this.GetHashCode(), "[BuildUserTokenModule::GetUserSid] Get user sid from CAT. sid = {0}", text);
			}
			else if (!string.IsNullOrWhiteSpace((string)httpContext.Items["X-EX-UserSid"]))
			{
				text = (string)httpContext.Items["X-EX-UserSid"];
				ExTraceGlobals.HttpModuleTracer.TraceDebug<string>((long)this.GetHashCode(), "[BuildUserTokenModule::GetUserSid] Get user sid from HttpContext. sid = {0}", text);
			}
			else if (commonAccessToken != null && "Windows".Equals(commonAccessToken.TokenType) && commonAccessToken.WindowsAccessToken != null)
			{
				text = commonAccessToken.WindowsAccessToken.UserSid;
				ExTraceGlobals.HttpModuleTracer.TraceDebug<string>((long)this.GetHashCode(), "[BuildUserTokenModule::GetUserSid] Get user sid from CAT for Kerberos. sid = {0}", text);
			}
			if (string.IsNullOrWhiteSpace(text))
			{
				ExTraceGlobals.HttpModuleTracer.TraceDebug((long)this.GetHashCode(), "[BuildUserTokenModule::GetUserSid] Get user sid from HttpContext. sid = null");
				return null;
			}
			SecurityIdentifier result;
			try
			{
				result = new SecurityIdentifier(text);
			}
			catch (Exception ex2)
			{
				HttpLogger.SafeAppendGenericError("BuildUserTokenModule.GetUserSid", ex2, new Func<Exception, bool>(KnownException.IsUnhandledException));
				ExTraceGlobals.HttpModuleTracer.TraceError<Exception>((long)this.GetHashCode(), "[BuildUserTokenModule::GetUserSid] Exception {0}", ex2);
				result = null;
			}
			return result;
		}

		private PartitionId GetPartitionId(CommonAccessToken commonAccessToken, IIdentity identity)
		{
			string text = null;
			if (commonAccessToken != null && commonAccessToken.ExtensionData.ContainsKey("Partition"))
			{
				ExTraceGlobals.HttpModuleTracer.TraceDebug((long)this.GetHashCode(), "[BuildUserTokenModule::GetPartitionId] From CAT.");
				text = commonAccessToken.ExtensionData["Partition"];
			}
			else
			{
				GenericSidIdentity genericSidIdentity = identity as GenericSidIdentity;
				if (genericSidIdentity != null)
				{
					ExTraceGlobals.HttpModuleTracer.TraceDebug((long)this.GetHashCode(), "[BuildUserTokenModule::GetPartitionId] From Generic Sid Identity.");
					text = genericSidIdentity.PartitionId;
				}
			}
			if (text != null)
			{
				PartitionId result;
				if (PartitionId.TryParse(text, out result))
				{
					return result;
				}
				ExTraceGlobals.HttpModuleTracer.TraceError<string>((long)this.GetHashCode(), "[BuildUserTokenModule::GetPartitionId] Invalid partition id {0}.", text);
				HttpLogger.SafeAppendGenericError("BuildUserTokenModule.GetPartition", "Invalid partition id " + text, false);
			}
			return null;
		}

		private OrganizationId GetOrganization(CommonAccessToken commonAccessToken, IIdentity identity, AccessTokenType accessTokenType)
		{
			if (commonAccessToken != null && commonAccessToken.ExtensionData.ContainsKey("OrganizationIdBase64"))
			{
				string text = commonAccessToken.ExtensionData["OrganizationIdBase64"];
				if (!string.IsNullOrEmpty(text))
				{
					ExTraceGlobals.HttpModuleTracer.TraceDebug((long)this.GetHashCode(), "[BuildUserTokenModule::GetOrganization] From CAT.");
					return CommonAccessTokenAccessor.DeserializeOrganizationId(text);
				}
			}
			if (identity is LiveIDIdentity)
			{
				LiveIDIdentity liveIDIdentity = (LiveIDIdentity)identity;
				if (liveIDIdentity.UserOrganizationId != null)
				{
					ExTraceGlobals.HttpModuleTracer.TraceDebug((long)this.GetHashCode(), "[BuildUserTokenModule::GetOrganization] From LiveIdIdentity.");
					return liveIDIdentity.UserOrganizationId;
				}
			}
			if (identity is SidOAuthIdentity)
			{
				SidOAuthIdentity sidOAuthIdentity = (SidOAuthIdentity)identity;
				if (sidOAuthIdentity.OAuthIdentity != null && !sidOAuthIdentity.OAuthIdentity.IsAppOnly)
				{
					ExTraceGlobals.HttpModuleTracer.TraceDebug((long)this.GetHashCode(), "[BuildUserTokenModule::GetOrganization] From SidOAuthIdentity.");
					return sidOAuthIdentity.OAuthIdentity.OrganizationId;
				}
			}
			if (commonAccessToken != null)
			{
				if (accessTokenType == AccessTokenType.CertificateSid)
				{
					ExTraceGlobals.HttpModuleTracer.TraceDebug((long)this.GetHashCode(), "[BuildUserTokenModule::GetOrganization] Certificate - First Org.");
					return OrganizationId.ForestWideOrgId;
				}
				if (accessTokenType == AccessTokenType.Windows && commonAccessToken.WindowsAccessToken != null)
				{
					ExTraceGlobals.HttpModuleTracer.TraceDebug((long)this.GetHashCode(), "[BuildUserTokenModule::GetOrganization] Kerberos - First Org.");
					return OrganizationId.ForestWideOrgId;
				}
			}
			ExTraceGlobals.HttpModuleTracer.TraceDebug((long)this.GetHashCode(), "[BuildUserTokenModule::GetOrganization] Org=Null.");
			return null;
		}

		private string GetManagedOrganization(IIdentity identity)
		{
			SidOAuthIdentity sidOAuthIdentity = identity as SidOAuthIdentity;
			if (sidOAuthIdentity != null)
			{
				ExTraceGlobals.HttpModuleTracer.TraceDebug((long)this.GetHashCode(), "[BuildUserTokenModule::GetManagedOrganization] From SidOAuthIdentity.");
				return sidOAuthIdentity.ManagedTenantName;
			}
			DelegatedPrincipal delegatedPrincipal = HttpContext.Current.User as DelegatedPrincipal;
			if (delegatedPrincipal != null)
			{
				ExTraceGlobals.HttpModuleTracer.TraceDebug((long)this.GetHashCode(), "[BuildUserTokenModule::GetManagedOrganization] From DelegatedPrincipal.");
				return delegatedPrincipal.DelegatedOrganization;
			}
			ExTraceGlobals.HttpModuleTracer.TraceDebug((long)this.GetHashCode(), "[BuildUserTokenModule::GetManagedOrganization] ManagedOrg=Null.");
			return null;
		}

		private bool GetAppPasswordUsed(CommonAccessToken commonAccessToken)
		{
			return commonAccessToken != null && commonAccessToken.ExtensionData.ContainsKey("AppPasswordUsed") && commonAccessToken.ExtensionData["AppPasswordUsed"] == "1";
		}
	}
}
