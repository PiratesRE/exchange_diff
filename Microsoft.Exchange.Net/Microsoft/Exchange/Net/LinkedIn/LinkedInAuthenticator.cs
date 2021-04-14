using System;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Net.LinkedIn
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class LinkedInAuthenticator
	{
		public LinkedInAuthenticator(LinkedInConfig linkedInConfig, ILinkedInWebClient linkedInWebClient, ITracer tracer)
		{
			ArgumentValidator.ThrowIfNull("linkedInConfig", linkedInConfig);
			ArgumentValidator.ThrowIfNull("linkedInWebClient", linkedInWebClient);
			ArgumentValidator.ThrowIfNull("tracer", tracer);
			this.linkedInConfig = linkedInConfig;
			this.linkedInWebClient = linkedInWebClient;
			this.oauth = new LinkedInOAuth(tracer);
			this.tracer = tracer;
		}

		public LinkedInAppAuthorizationResponse AuthorizeApplication(NameValueCollection requestQueryString, HttpCookieCollection requestCookies, HttpCookieCollection responseCookies, Uri authorizationCallbackUrl)
		{
			if (requestQueryString == null)
			{
				throw new ArgumentNullException("requestQueryString");
			}
			if (requestCookies == null)
			{
				throw new ArgumentNullException("requestCookies");
			}
			if (responseCookies == null)
			{
				throw new ArgumentNullException("responseCookies");
			}
			if (authorizationCallbackUrl == null)
			{
				throw new ArgumentNullException("authorizationCallbackUrl");
			}
			this.tracer.TraceDebug<LinkedInConfig>((long)this.GetHashCode(), "Authorizing application with configuration: {0}", this.linkedInConfig);
			if (string.IsNullOrEmpty(requestQueryString["oauth_callback"]))
			{
				this.tracer.TraceDebug((long)this.GetHashCode(), "OAuth callback NOT present in request query string.  Getting request token.");
				return this.GetRequestTokenAndAuthorizationRedirectUri(responseCookies, authorizationCallbackUrl);
			}
			this.tracer.TraceDebug((long)this.GetHashCode(), "OAuth callback is present in request query string.  Processing authorization callback.");
			return this.ProcessAuthorizationCallback(requestQueryString, requestCookies);
		}

		public LinkedInTokenInformation GetRequestToken(string callbackUrl)
		{
			Uri url = new Uri(this.linkedInConfig.RequestTokenEndpoint);
			Uri uri;
			if (!string.IsNullOrEmpty(callbackUrl) && !Uri.TryCreate(callbackUrl, UriKind.Absolute, out uri))
			{
				throw new ArgumentException("callbackUrl");
			}
			NameValueCollection oauthParameters = this.oauth.GetOAuthParameters(url, null, "POST", this.linkedInConfig.AppId, this.linkedInConfig.AppSecret, null, null, null, callbackUrl);
			string text = this.oauth.BuildOAuthHeader(oauthParameters, string.Empty);
			this.tracer.TraceDebug<string, string>((long)this.GetHashCode(), "GetRequestToken: Making request to endpoint {0}, authorization header: {1}.", this.linkedInConfig.RequestTokenEndpoint, text);
			LinkedInTokenInformation linkedInTokenInformation = this.MakeAuthRequest(this.linkedInConfig.RequestTokenEndpoint, text, this.linkedInConfig.WebRequestTimeout, this.linkedInConfig.WebProxy);
			if (string.IsNullOrEmpty(linkedInTokenInformation.OAuthAccessTokenUrl))
			{
				throw new LinkedInAuthenticationException(NetServerException.EmptyRedirectUrlAuthenticationResponse);
			}
			return linkedInTokenInformation;
		}

		public LinkedInTokenInformation GetAccessToken(string requestToken, string requestSecret, string accessCode)
		{
			if (string.IsNullOrEmpty(requestToken))
			{
				throw new ArgumentNullException(requestToken);
			}
			if (string.IsNullOrEmpty(requestSecret))
			{
				throw new ArgumentNullException(requestSecret);
			}
			if (string.IsNullOrEmpty(accessCode))
			{
				throw new ArgumentNullException(accessCode);
			}
			Uri url = new Uri(this.linkedInConfig.AccessTokenEndpoint);
			NameValueCollection oauthParameters = this.oauth.GetOAuthParameters(url, null, "POST", this.linkedInConfig.AppId, this.linkedInConfig.AppSecret, accessCode, requestToken, requestSecret, null);
			string text = this.oauth.BuildOAuthHeader(oauthParameters, string.Empty);
			this.tracer.TraceDebug<string, string>((long)this.GetHashCode(), "GetAccessToken: Making request to endpoint {0}, authorization header: {1}.", this.linkedInConfig.AccessTokenEndpoint, text);
			return this.MakeAuthRequest(this.linkedInConfig.AccessTokenEndpoint, text, this.linkedInConfig.WebRequestTimeout, this.linkedInConfig.WebProxy);
		}

		private LinkedInTokenInformation MakeAuthRequest(string endPointUrl, string authorizationHeader, TimeSpan timeout, IWebProxy proxy)
		{
			LinkedInTokenInformation result;
			try
			{
				LinkedInResponse linkedInResponse = this.AuthenticateApplication(endPointUrl, authorizationHeader, timeout, proxy);
				HttpStatusCode code = linkedInResponse.Code;
				if (code != HttpStatusCode.OK)
				{
					switch (code)
					{
					case HttpStatusCode.BadRequest:
						this.tracer.TraceError<string, LinkedInConfig>((long)this.GetHashCode(), "AuthenticateApplication returned HTTP 400 BAD REQUEST.  This could be a result of misconfigured date/time on the server (clock skew).  Body: {0}.  Configuration: {1}", linkedInResponse.Body, this.linkedInConfig);
						throw new LinkedInAuthenticationException(NetServerException.LinkedInUnexpectedAppAuthenticationResponse(linkedInResponse.Code, linkedInResponse.Body, this.linkedInConfig));
					case HttpStatusCode.Unauthorized:
						this.tracer.TraceError<string, LinkedInConfig>((long)this.GetHashCode(), "AuthenticateApplication returned HTTP 401 UNAUTHORIZED.  This could be a result of an invalid signature due to incorrect app secret.  Body: {0}.  Configuration: {1}", linkedInResponse.Body, this.linkedInConfig);
						throw new LinkedInAuthenticationException(NetServerException.LinkedInUnexpectedAppAuthenticationResponse(linkedInResponse.Code, linkedInResponse.Body, this.linkedInConfig));
					default:
						this.tracer.TraceError<HttpStatusCode, string>((long)this.GetHashCode(), "AuthenticateApplication returned an unexpected status code.  Code: {0};  Body: {1}", linkedInResponse.Code, linkedInResponse.Body);
						throw new LinkedInAuthenticationException(NetServerException.LinkedInUnexpectedAppAuthenticationResponse(linkedInResponse.Code, linkedInResponse.Body, this.linkedInConfig));
					}
				}
				else
				{
					this.tracer.TraceDebug<string>((long)this.GetHashCode(), "Authentication response status is HTTP 200 OK.  Body: {0}", linkedInResponse.Body);
					result = this.ExtractTokenInformationFromResponse(linkedInResponse.Body);
				}
			}
			catch (WebException ex)
			{
				string arg = string.Empty;
				if (ex.Response != null)
				{
					using (Stream responseStream = ex.Response.GetResponseStream())
					{
						if (responseStream.CanRead)
						{
							char[] array = new char[1024];
							int num = 0;
							using (StreamReader streamReader = new StreamReader(responseStream))
							{
								num = streamReader.Read(array, 0, 1024);
							}
							if (num > 0)
							{
								arg = new string(array, 0, num);
							}
						}
					}
				}
				this.tracer.TraceError<WebException, LinkedInConfig, string>((long)this.GetHashCode(), "MakeAuthRequest: Failed request with web exception {0}.  Configuration: {1}.  Payload: {2}", ex, this.linkedInConfig, arg);
				throw new LinkedInAuthenticationException(NetServerException.LinkedInFailedToAuthenticateApp(this.linkedInConfig), ex);
			}
			catch (ProtocolViolationException ex2)
			{
				this.tracer.TraceError<ProtocolViolationException, LinkedInConfig>((long)this.GetHashCode(), "MakeAuthRequest: failed to authenticate app with protocol violation: {0}.  Configuration: {1}", ex2, this.linkedInConfig);
				throw new LinkedInAuthenticationException(NetServerException.LinkedInFailedToAuthenticateApp(this.linkedInConfig), ex2);
			}
			catch (IOException ex3)
			{
				this.tracer.TraceError<IOException, LinkedInConfig>((long)this.GetHashCode(), "MakeAuthRequest: failed to authenticate app with I/O exception: {0}.  Configuration: {1}", ex3, this.linkedInConfig);
				throw new LinkedInAuthenticationException(NetServerException.LinkedInFailedToAuthenticateApp(this.linkedInConfig), ex3);
			}
			return result;
		}

		private LinkedInResponse AuthenticateApplication(string endPointUrl, string authorizationHeader, TimeSpan timeout, IWebProxy proxy)
		{
			LinkedInResponse result;
			try
			{
				result = this.linkedInWebClient.AuthenticateApplication(endPointUrl, authorizationHeader, timeout, proxy);
			}
			catch (IOException arg)
			{
				this.tracer.TraceError<IOException, LinkedInConfig>((long)this.GetHashCode(), "AuthenticateApplication: caught IOException.  Will retry.  Exception: {0}.  Configuration: {1}", arg, this.linkedInConfig);
				result = this.linkedInWebClient.AuthenticateApplication(endPointUrl, authorizationHeader, timeout, proxy);
			}
			return result;
		}

		private LinkedInTokenInformation ExtractTokenInformationFromResponse(string response)
		{
			NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(response);
			string text = nameValueCollection["oauth_token"];
			if (string.IsNullOrEmpty(text))
			{
				this.tracer.TraceError((long)this.GetHashCode(), "MakeAuthRequest: auth response does not contain a token.");
				throw new LinkedInAuthenticationException(NetServerException.LinkedInInvalidOAuthResponseMissingToken(response));
			}
			string text2 = nameValueCollection["oauth_token_secret"];
			if (string.IsNullOrEmpty(text2))
			{
				this.tracer.TraceError((long)this.GetHashCode(), "MakeAuthRequest: auth response does not contain a secret.");
				throw new LinkedInAuthenticationException(NetServerException.LinkedInInvalidOAuthResponseMissingTokenSecret(response));
			}
			string text3 = nameValueCollection["xoauth_request_auth_url"];
			this.tracer.TraceDebug<string, string, string>((long)this.GetHashCode(), "Extracted the following token information from response:  oauth_token='{0}';  oauth_token_secret='{1}';  xoauth_request_auth_url='{2}'", text, text2, text3);
			return new LinkedInTokenInformation
			{
				Token = text,
				Secret = text2,
				OAuthAccessTokenUrl = text3
			};
		}

		private LinkedInAppAuthorizationResponse GetRequestTokenAndAuthorizationRedirectUri(HttpCookieCollection responseCookies, Uri authorizationCallback)
		{
			string text = this.AppendOAuthCallbackToCallbackUrl(authorizationCallback).ToString();
			this.tracer.TraceDebug<string>((long)this.GetHashCode(), "GetRequestTokenAndAuthorizationRedirectUri: about to get request token.  AuthZ callback URL = {0}", text);
			LinkedInTokenInformation requestToken = this.GetRequestToken(text);
			responseCookies.Add(new HttpCookie("LinkedInAuthenticator.ReqSecret", requestToken.Secret)
			{
				HttpOnly = true,
				Secure = true
			});
			return new LinkedInAppAuthorizationResponse
			{
				RequestToken = requestToken.Token,
				RequestSecret = requestToken.Secret,
				AppAuthorizationRedirectUri = this.AppendRequestTokenToAuthorizationEndpoint(requestToken.OAuthAccessTokenUrl, requestToken.Token)
			};
		}

		private LinkedInAppAuthorizationResponse ProcessAuthorizationCallback(NameValueCollection requestQueryString, HttpCookieCollection requestCookies)
		{
			if (string.IsNullOrEmpty(requestQueryString["oauth_problem"]))
			{
				return this.ProcessGrantedAuthorization(requestQueryString, requestCookies);
			}
			return this.ProcessRefusedAuthorization(requestQueryString);
		}

		private LinkedInAppAuthorizationResponse ProcessGrantedAuthorization(NameValueCollection requestQueryString, HttpCookieCollection requestCookies)
		{
			string text = requestQueryString["oauth_token"];
			if (string.IsNullOrEmpty(text))
			{
				this.tracer.TraceError((long)this.GetHashCode(), "ProcessGrantedAuthorization: missing oauth_token (request token)");
				throw new LinkedInAuthenticationException(NetServerException.LinkedInQueryStringMissingOAuthToken);
			}
			HttpCookie httpCookie = requestCookies["LinkedInAuthenticator.ReqSecret"];
			if (httpCookie == null)
			{
				this.tracer.TraceError((long)this.GetHashCode(), "ProcessGrantedAuthorization: missing request secret cookie");
				throw new LinkedInAuthenticationException(NetServerException.LinkedInRequestCookiesMissingOAuthSecret);
			}
			string value = httpCookie.Value;
			if (string.IsNullOrEmpty(value))
			{
				this.tracer.TraceError((long)this.GetHashCode(), "ProcessGrantedAuthorization: request secret is blank");
				throw new LinkedInAuthenticationException(NetServerException.LinkedInRequestCookiesMissingOAuthSecret);
			}
			string text2 = requestQueryString["oauth_verifier"];
			if (string.IsNullOrEmpty(text2))
			{
				this.tracer.TraceError((long)this.GetHashCode(), "ProcessGrantedAuthorization: missing oauth_verifier");
				throw new LinkedInAuthenticationException(NetServerException.LinkedInQueryStringMissingOAuthVerifier);
			}
			this.tracer.TraceDebug<string, string, string>((long)this.GetHashCode(), "ProcessGrantedAuthorization: successful authorization.  oauth_token (request token) = {0}; secret = {1}; oauth_verifier = {2}", text, value, text2);
			return new LinkedInAppAuthorizationResponse
			{
				RequestToken = text,
				RequestSecret = value,
				OAuthVerifier = text2
			};
		}

		private LinkedInAppAuthorizationResponse ProcessRefusedAuthorization(NameValueCollection requestQueryString)
		{
			this.tracer.TraceDebug<string>((long)this.GetHashCode(), "ProcessRefusedAuthorization: denied authorization.  oauth_problem = {0}", requestQueryString["oauth_problem"]);
			return new LinkedInAppAuthorizationResponse
			{
				OAuthProblem = requestQueryString["oauth_problem"]
			};
		}

		private string AppendRequestTokenToAuthorizationEndpoint(string authorizationEndpointUrl, string requestToken)
		{
			if (string.IsNullOrEmpty(authorizationEndpointUrl))
			{
				return string.Empty;
			}
			Uri url;
			if (!Uri.TryCreate(authorizationEndpointUrl, UriKind.Absolute, out url))
			{
				this.tracer.TraceError<string>((long)this.GetHashCode(), "AppendRequestTokenToAuthorizationEndpoint: not appending request token because input URL is malformed: {0}", authorizationEndpointUrl);
				return authorizationEndpointUrl;
			}
			return this.AppendNameValueToQueryString(url, "oauth_token", requestToken).ToString();
		}

		private Uri AppendOAuthCallbackToCallbackUrl(Uri authorizationCallbackUrl)
		{
			return this.AppendNameValueToQueryString(authorizationCallbackUrl, "oauth_callback", "1");
		}

		private Uri AppendNameValueToQueryString(Uri url, string name, string value)
		{
			NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(url.Query);
			if (nameValueCollection.AllKeys.Contains(name, StringComparer.OrdinalIgnoreCase))
			{
				return url;
			}
			StringBuilder stringBuilder = new StringBuilder(url.Query);
			if (stringBuilder.Length > 0)
			{
				stringBuilder.Remove(0, 1);
				stringBuilder.Append('&');
			}
			UriBuilder uriBuilder = new UriBuilder(url)
			{
				Query = stringBuilder.AppendFormat("{0}={1}", HttpUtility.UrlEncode(name), HttpUtility.UrlEncode(value)).ToString()
			};
			return uriBuilder.Uri;
		}

		private const string OAuthToken = "oauth_token";

		private const string OAuthTokenSecret = "oauth_token_secret";

		private const string OAuthRequestUrl = "xoauth_request_auth_url";

		private const string OAuthPostHttpMethod = "POST";

		private const string OAuthCallback = "oauth_callback";

		private const string OAuthCallbackValue = "1";

		private const string OAuthProblem = "oauth_problem";

		private const string OAuthVerifier = "oauth_verifier";

		private const string RequestSecretCookieName = "LinkedInAuthenticator.ReqSecret";

		private readonly LinkedInConfig linkedInConfig;

		private readonly ILinkedInWebClient linkedInWebClient;

		private readonly LinkedInOAuth oauth;

		private readonly ITracer tracer;
	}
}
