using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Web;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Net;

namespace Microsoft.Exchange.Net
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class FacebookAuthenticator
	{
		public FacebookAuthenticator(FacebookAuthenticatorConfig config)
		{
			if (config == null)
			{
				throw new ArgumentNullException("config");
			}
			this.config = config;
			this.hash = this.GetHashCode();
		}

		public Uri GetAppAuthorizationUri()
		{
			UriBuilder uriBuilder = new UriBuilder(this.config.AuthorizationEndpoint)
			{
				Query = string.Format("client_id={0}&locale={1}&scope={2}&redirect_uri={3}", new object[]
				{
					this.config.AppId,
					this.config.Locale,
					this.config.Scope,
					HttpUtility.UrlEncode(this.config.RedirectUri)
				})
			};
			return uriBuilder.Uri;
		}

		public static AppAuthorizationResponse ParseAppAuthorizationResponse(NameValueCollection requestParameters)
		{
			ArgumentValidator.ThrowIfNull("requestParameters", requestParameters);
			return new AppAuthorizationResponse
			{
				AppAuthorizationCode = requestParameters["code"],
				Error = requestParameters["error"],
				ErrorDescription = requestParameters["error_description"],
				ErrorReason = requestParameters["error_reason"]
			};
		}

		public static bool IsRedirectFromFacebook(AppAuthorizationResponse response)
		{
			ArgumentValidator.ThrowIfNull("response", response);
			return !string.IsNullOrEmpty(response.AppAuthorizationCode) || !string.IsNullOrEmpty(response.Error);
		}

		public bool IsAuthorizationGranted(AppAuthorizationResponse response)
		{
			ArgumentValidator.ThrowIfNull("response", response);
			bool flag = !string.IsNullOrEmpty(response.AppAuthorizationCode) && string.IsNullOrEmpty(response.Error);
			if (!flag)
			{
				FacebookAuthenticator.Tracer.TraceDebug((long)this.hash, "Authorization denied. Code: {0}, Error: {1}, Description: {2}, Reason: {3}", new object[]
				{
					response.AppAuthorizationCode,
					response.Error,
					response.ErrorDescription,
					response.ErrorReason
				});
			}
			return flag;
		}

		public string ExchangeAppAuthorizationCodeForAccessToken(string code)
		{
			if (string.IsNullOrEmpty(code))
			{
				throw new ArgumentNullException("code");
			}
			string result;
			try
			{
				Uri accessTokenEndpoint = this.BuildTokenEndpointUri(code);
				AuthenticateApplicationResponse authenticateApplicationResponse = this.AuthenticateApplication(accessTokenEndpoint);
				HttpStatusCode code2 = authenticateApplicationResponse.Code;
				if (code2 != HttpStatusCode.OK)
				{
					if (code2 != HttpStatusCode.BadRequest)
					{
						FacebookAuthenticator.Tracer.TraceError<HttpStatusCode, string>((long)this.hash, "AuthenticateApplication returned an unexpected status code.  Code: {0};  Body: {1}", authenticateApplicationResponse.Code, authenticateApplicationResponse.Body);
						throw new FacebookAuthenticationException(NetServerException.UnexpectedAppAuthenticationResponse(authenticateApplicationResponse.Code, authenticateApplicationResponse.Body, this.config));
					}
					FacebookAuthenticator.Tracer.TraceDebug<string, FacebookAuthenticatorConfig>((long)this.hash, "AuthenticateApplication returned BadRequest.  Body: {0}.  Configuration: {1}", authenticateApplicationResponse.Body, this.config);
					throw new FacebookAuthenticationException(NetServerException.InvalidAppAuthorizationCode(authenticateApplicationResponse.Body, this.config));
				}
				else
				{
					result = this.ExtractAccessToken(authenticateApplicationResponse.Body);
				}
			}
			catch (WebException ex)
			{
				if (FacebookAuthenticator.Tracer.IsTraceEnabled(TraceType.ErrorTrace))
				{
					FacebookAuthenticator.Tracer.TraceError<WebException, string, FacebookAuthenticatorConfig>((long)this.hash, "ExchangeAppAuthorizationCodeForAccessToken: caught WebException {0}.  Response: {1}.  Configuration: {2}", ex, this.GetResponseBodyForTracing(ex), this.config);
				}
				throw new FacebookAuthenticationException(NetServerException.FailedToAuthenticateApp(this.config), ex);
			}
			catch (ProtocolViolationException ex2)
			{
				FacebookAuthenticator.Tracer.TraceError<ProtocolViolationException>((long)this.hash, "ExchangeAppAuthorizationCodeForAccessToken: caught ProtocolViolationException {0}", ex2);
				throw new FacebookAuthenticationException(NetServerException.FailedToAuthenticateApp(this.config), ex2);
			}
			catch (IOException ex3)
			{
				FacebookAuthenticator.Tracer.TraceError<IOException>((long)this.hash, "ExchangeAppAuthorizationCodeForAccessToken: caught IOException {0}", ex3);
				throw new FacebookAuthenticationException(NetServerException.FailedToAuthenticateApp(this.config), ex3);
			}
			return result;
		}

		private string ExtractAccessToken(string authenticateAppResponse)
		{
			if (string.IsNullOrEmpty(authenticateAppResponse))
			{
				FacebookAuthenticator.Tracer.TraceError((long)this.hash, "ExtractAccessToken: authentication response is empty.");
				throw new FacebookAuthenticationException(NetServerException.EmptyAppAuthenticationResponse);
			}
			string text = HttpUtility.ParseQueryString(authenticateAppResponse)["access_token"];
			if (string.IsNullOrEmpty(text))
			{
				FacebookAuthenticator.Tracer.TraceError((long)this.hash, "ExtractAccessToken: authentication response does not contain an access token.");
				throw new FacebookAuthenticationException(NetServerException.InvalidAppAuthenticationResponse(authenticateAppResponse, this.config));
			}
			return text;
		}

		private Uri BuildTokenEndpointUri(string code)
		{
			UriBuilder uriBuilder = new UriBuilder(this.config.GraphTokenEndpoint)
			{
				Query = string.Format("client_id={0}&redirect_uri={1}&client_secret={2}&code={3}", new object[]
				{
					this.config.AppId,
					HttpUtility.UrlEncode(this.config.RedirectUri),
					this.config.AppSecret,
					code
				})
			};
			return uriBuilder.Uri;
		}

		private AuthenticateApplicationResponse AuthenticateApplication(Uri accessTokenEndpoint)
		{
			AuthenticateApplicationResponse result;
			try
			{
				result = this.config.AuthenticationClient.AuthenticateApplication(accessTokenEndpoint, this.config.WebRequestTimeout);
			}
			catch (IOException arg)
			{
				FacebookAuthenticator.Tracer.TraceError<IOException, FacebookAuthenticatorConfig>((long)this.hash, "AuthenticateApplication: caught IOException.  Will retry.  Exception: {0}.  Configuration: {1}", arg, this.config);
				result = this.config.AuthenticationClient.AuthenticateApplication(accessTokenEndpoint, this.config.WebRequestTimeout);
			}
			return result;
		}

		private string GetResponseBodyForTracing(WebException e)
		{
			if (e == null || e.Response == null)
			{
				return string.Empty;
			}
			string result;
			try
			{
				using (Stream responseStream = e.Response.GetResponseStream())
				{
					if (!responseStream.CanRead)
					{
						result = string.Empty;
					}
					else
					{
						char[] array = new char[1024];
						int num = 0;
						using (StreamReader streamReader = new StreamReader(responseStream))
						{
							num = streamReader.Read(array, 0, 1024);
						}
						if (num <= 0)
						{
							result = string.Empty;
						}
						else
						{
							result = new string(array, 0, num);
						}
					}
				}
			}
			catch (Exception ex)
			{
				result = ex.ToString();
			}
			return result;
		}

		private static readonly Trace Tracer = ExTraceGlobals.FacebookTracer;

		private readonly FacebookAuthenticatorConfig config;

		private readonly int hash;
	}
}
