using System;
using System.IdentityModel.Tokens;
using System.Security.Principal;
using System.Web;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.OABAuth;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Security;
using Microsoft.Exchange.Security.OAuth;

namespace Microsoft.Exchange.OABAuth
{
	public class OABAuthModule : IHttpModule
	{
		void IHttpModule.Init(HttpApplication application)
		{
			application.PostAuthenticateRequest += this.OnPostAuthenticateRequest;
			application.AuthenticateRequest += this.OnAuthenticateRequest;
		}

		void IHttpModule.Dispose()
		{
		}

		private void OnPostAuthenticateRequest(object source, EventArgs args)
		{
			HttpApplication httpApplication = source as HttpApplication;
			if (!httpApplication.Request.IsAuthenticated || !("SSL/PCT" == httpApplication.Request.ServerVariables["AUTH_TYPE"]) || !httpApplication.Request.ClientCertificate.IsValid)
			{
				return;
			}
			WindowsIdentity windowsIdentity = (WindowsIdentity)httpApplication.Context.User.Identity;
			OABAuthModule.TraceOABAuth.TraceDebug<WindowsIdentity>((long)this.GetHashCode(), "Attempting to impersonate {0} and retrieve UPN.", windowsIdentity);
			string userPrincipalName;
			try
			{
				using (WindowsImpersonationContext windowsImpersonationContext = windowsIdentity.Impersonate())
				{
					try
					{
						userPrincipalName = UserInformation.UserPrincipalName;
					}
					finally
					{
						windowsImpersonationContext.Undo();
					}
				}
			}
			catch (Exception ex)
			{
				OABAuthModule.TraceOABAuth.TraceDebug<Exception>((long)this.GetHashCode(), "Exception caught during impersonation. Exception = {0}.", ex);
				throw ex;
			}
			OABAuthModule.TraceOABAuth.TraceDebug<string, Uri>((long)this.GetHashCode(), "Attempting GetOabDownloadToken UPN={0}; Request uri={1}.", userPrincipalName, httpApplication.Context.Request.Url);
			TokenResult oabDownloadToken;
			try
			{
				oabDownloadToken = OABAuthModule.Issuer.GetOabDownloadToken(httpApplication.Context.Request.Url, userPrincipalName);
			}
			catch (OAuthTokenRequestFailedException ex2)
			{
				OABAuthModule.TraceOABAuth.TraceError<string, OAuthTokenRequestFailedException>((long)this.GetHashCode(), "Error while calling GetOabDownloadToken for upn {0}, hits exception: {1}.", userPrincipalName, ex2);
				OABAuthModule.eventLogger.LogEvent(SecurityEventLogConstants.Tuple_OAuthFailToIssueTokenForOAB, string.Empty, new object[]
				{
					userPrincipalName,
					httpApplication.Context.Request.Url,
					ex2
				});
				return;
			}
			OABAuthModule.TraceOABAuth.TraceDebug((long)this.GetHashCode(), "Success GetOabDownloadToken UPN={0}; Request uri={1}.  Token={2}. Token Expiration Date={3}.", new object[]
			{
				userPrincipalName,
				httpApplication.Context.Request.Url,
				oabDownloadToken.TokenString,
				oabDownloadToken.ExpirationDate
			});
			HttpCookie httpCookie = new HttpCookie("OABAuth");
			httpCookie.Value = oabDownloadToken.TokenString;
			httpCookie.Expires = oabDownloadToken.ExpirationDate;
			httpCookie.Secure = true;
			httpCookie.HttpOnly = true;
			httpCookie.Path = httpApplication.Request.Url.Segments[0];
			if (httpApplication.Request.Url.Segments.Length > 1)
			{
				HttpCookie httpCookie2 = httpCookie;
				httpCookie2.Path += httpApplication.Request.Url.Segments[1];
			}
			httpApplication.Response.AppendCookie(httpCookie);
			OABAuthModule.TraceOABAuth.TraceDebug<string, string>((long)this.GetHashCode(), "Cookie Name: {0} and Value: {1}  was successfully added to the response.", httpCookie.Name, httpCookie.Value);
		}

		private void OnAuthenticateRequest(object source, EventArgs args)
		{
			HttpApplication httpApplication = source as HttpApplication;
			if (httpApplication.Request.IsAuthenticated || httpApplication.Request.Cookies.Get("OABAuth") == null)
			{
				return;
			}
			string value = httpApplication.Request.Cookies.Get("OABAuth").Value;
			OABAuthModule.TraceOABAuth.TraceDebug<string>((long)this.GetHashCode(), "Found OABAuth cookie.  Value = {0}.", value);
			string arg;
			OAuthIdentity oauthIdentity;
			try
			{
				oauthIdentity = OAuthTokenHandler.GetOAuthIdentity(value, out arg);
			}
			catch (NullReferenceException exception)
			{
				this.LogError(value, httpApplication.Context.Request.Url, exception, OAuthErrorCategory.InvalidToken, "");
				return;
			}
			catch (ArgumentException exception2)
			{
				this.LogError(value, httpApplication.Context.Request.Url, exception2, OAuthErrorCategory.InvalidToken, "");
				return;
			}
			catch (InvalidOperationException exception3)
			{
				this.LogError(value, httpApplication.Context.Request.Url, exception3, OAuthErrorCategory.InternalError, "");
				return;
			}
			catch (SecurityTokenExpiredException exception4)
			{
				this.LogError(value, httpApplication.Context.Request.Url, exception4, OAuthErrorCategory.TokenExpired, "");
				return;
			}
			catch (SecurityTokenValidationException exception5)
			{
				this.LogError(value, httpApplication.Context.Request.Url, exception5, OAuthErrorCategory.InvalidToken, "");
				return;
			}
			catch (SecurityTokenException exception6)
			{
				this.LogError(value, httpApplication.Context.Request.Url, exception6, OAuthErrorCategory.InvalidToken, "");
				return;
			}
			catch (InvalidOAuthTokenException ex)
			{
				this.LogError(value, httpApplication.Context.Request.Url, ex, ex.ErrorCategory, ex.LogPeriodicKey);
				return;
			}
			catch (OAuthIdentityDeserializationException exception7)
			{
				this.LogError(value, httpApplication.Context.Request.Url, exception7, OAuthErrorCategory.InternalError, "");
				return;
			}
			string userPrincipalName = oauthIdentity.ActAsUser.UserPrincipalName;
			OABAuthModule.TraceOABAuth.TraceDebug<string, string, string>((long)this.GetHashCode(), "GetOAuthIdentity for {0} returned a UPN of {1} and loggable token as {2}.", value, userPrincipalName, arg);
			OABAuthModule.TraceOABAuth.TraceDebug<string>((long)this.GetHashCode(), "Generating WindowsIdentity for {0}.", userPrincipalName);
			WindowsIdentity ntIdentity = new WindowsIdentity(userPrincipalName);
			WindowsPrincipal windowsPrincipal = new WindowsPrincipal(ntIdentity);
			httpApplication.Context.User = windowsPrincipal;
			OABAuthModule.TraceOABAuth.TraceDebug<string, WindowsPrincipal>((long)this.GetHashCode(), "Set Context.User for {0} to {1}.", userPrincipalName, windowsPrincipal);
		}

		private void LogError(string tokenIn, Uri requestUri, Exception exception, OAuthErrorCategory errorCategory, string logPeriodicKey = "")
		{
			OABAuthModule.TraceOABAuth.TraceError<string, Exception, OAuthErrorCategory>((long)this.GetHashCode(), "Error validating the token {0}, hits exception: {1}. Error Category: {2}.", tokenIn, exception, errorCategory);
			OABAuthModule.eventLogger.LogEvent(SecurityEventLogConstants.Tuple_OAuthFailToAuthenticateTokenForOAB, logPeriodicKey ?? string.Empty, new object[]
			{
				tokenIn,
				requestUri,
				errorCategory,
				exception
			});
		}

		private const string EventSource = "MSExchange SmartCard Authentication for OAB";

		private const string OABAuthCookieName = "OABAuth";

		private const string SSLPCTAuthOptionName = "SSL/PCT";

		private const string ServerVariableAuthTypeName = "AUTH_TYPE";

		private static readonly Trace TraceOABAuth = ExTraceGlobals.GeneralTracer;

		private static readonly LocalTokenIssuer Issuer = new LocalTokenIssuer(OrganizationId.ForestWideOrgId);

		private static readonly ExEventLog eventLogger = new ExEventLog(ExTraceGlobals.GeneralTracer.Category, "MSExchange SmartCard Authentication for OAB");
	}
}
