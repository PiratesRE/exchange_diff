using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.Security;
using Microsoft.Exchange.Security.OAuth.OAuthProtocols;

namespace Microsoft.Exchange.Security.OAuth
{
	internal sealed class ACSTokenBuildRequest
	{
		public ACSTokenBuildRequest(X509Certificate2 signingKey, string localIssuerId, AuthServer authServer, string tenantId, string resource, string caller = null)
		{
			this.signingKey = signingKey;
			this.localIssuerId = localIssuerId;
			this.acsTokenIssuerMetadata = IssuerMetadata.Create(authServer);
			this.acsTokenIssuingEndpoint = authServer.TokenIssuingEndpoint;
			this.selfKey = string.Format("L:{0}-AS:{1}", this.localIssuerId, this.acsTokenIssuerMetadata);
			this.tenantId = tenantId;
			this.resource = resource;
			this.caller = caller;
			this.partnerKey = string.Format("T:{0}-R:{1}", this.tenantId.ToLowerInvariant(), this.resource);
		}

		public string SelfKey
		{
			get
			{
				return this.selfKey;
			}
		}

		public string PartnerKey
		{
			get
			{
				return this.partnerKey;
			}
		}

		public TokenResult TokenResult
		{
			get
			{
				return this.tokenResult;
			}
		}

		public string TenantId
		{
			get
			{
				return this.tenantId;
			}
		}

		public string Resource
		{
			get
			{
				return this.resource;
			}
		}

		public string Caller
		{
			get
			{
				return this.caller;
			}
		}

		public string ACSTokenIssuingEndpoint
		{
			get
			{
				return this.acsTokenIssuingEndpoint;
			}
		}

		public IOutboundTracer Tracer
		{
			get
			{
				return this.tracer ?? DefaultOutboundTracer.Instance;
			}
			set
			{
				this.tracer = value;
			}
		}

		public Guid? ClientRequestId
		{
			get
			{
				return this.clientRequestId;
			}
			set
			{
				this.clientRequestId = value;
			}
		}

		internal void BuildToken(bool throwOnError = true)
		{
			this.tracer.LogInformation(this.GetHashCode(), "[ACSTokenBuildRequest:BuildToken] started", new object[0]);
			TokenResult actorTokenFromAuthServer = this.GetActorTokenFromAuthServer(throwOnError);
			if (actorTokenFromAuthServer != null)
			{
				this.tokenResult = actorTokenFromAuthServer;
			}
			this.tracer.LogInformation(this.GetHashCode(), "[ACSTokenBuildRequest:BuildToken] finished", new object[0]);
		}

		internal void RefreshTokenIfNeed()
		{
			bool isRecentlyRefreshed = this.IsRecentlyRefreshed;
			this.tracer.LogInformation(this.GetHashCode(), "[ACSTokenBuildRequest:RefreshTokenIfNeed] the last time we tried to get the ACS token was {0}, it {1} recently tried.", new object[]
			{
				this.lastRefreshDateTime,
				isRecentlyRefreshed ? "is" : "is not"
			});
			if (!isRecentlyRefreshed)
			{
				lock (this.lockObj)
				{
					if (!this.IsRecentlyRefreshed)
					{
						this.lastRefreshDateTime = DateTime.UtcNow;
						this.tracer.LogInformation(this.GetHashCode(), "[ACSTokenBuildRequest:RefreshTokenIfNeed] started", new object[0]);
						this.BuildToken(false);
					}
				}
			}
		}

		private bool IsRecentlyRefreshed
		{
			get
			{
				return DateTime.UtcNow - this.lastRefreshDateTime < ACSTokenBuildRequest.ACSTokenRequestInterval;
			}
		}

		internal TokenResult GetActorTokenFromAuthServer(bool throwOnError)
		{
			TokenResult tokenResult = null;
			string acsAudience = string.Format(CultureInfo.InvariantCulture, "{0}/{1}@{2}", new object[]
			{
				this.acsTokenIssuerMetadata.Id,
				new Uri(this.acsTokenIssuingEndpoint).Authority,
				this.tenantId
			});
			LocalTokenIssuer localTokenIssuer = new LocalTokenIssuer(this.signingKey ?? ConfigProvider.Instance.Configuration.SigningKey, new LocalTokenIssuerMetadata(this.localIssuerId, this.tenantId));
			TokenResult tokenForACS = localTokenIssuer.GetTokenForACS(this.tenantId, acsAudience);
			this.Tracer.LogInformation(this.GetHashCode(), "[ACSTokenBuildRequest:GetActorTokenFromAuthServer] Sending token request to '{0}' for the resource '{1}' with token: {2}", new object[]
			{
				this.acsTokenIssuingEndpoint,
				this.resource,
				tokenForACS
			});
			OAuth2AccessTokenRequest oauth2AccessTokenRequest = OAuth2MessageFactory.CreateAccessTokenRequestWithAssertion(tokenForACS.Token, this.resource);
			Stopwatch stopwatch = Stopwatch.StartNew();
			string text = string.Empty;
			string text2 = string.Empty;
			try
			{
				OAuthCommon.PerfCounters.NumberOfAuthServerTokenRequests.Increment();
				string text3 = oauth2AccessTokenRequest.ToString();
				WebRequest webRequest = WebRequest.Create(this.acsTokenIssuingEndpoint);
				webRequest.AuthenticationLevel = AuthenticationLevel.None;
				webRequest.ContentLength = (long)text3.Length;
				webRequest.ContentType = "application/x-www-form-urlencoded";
				webRequest.Method = "POST";
				webRequest.Timeout = (int)ACSTokenBuildRequest.ACSTokenRequestTimeout.TotalMilliseconds;
				Server localServer = LocalServerCache.LocalServer;
				if (localServer != null && localServer.InternetWebProxy != null)
				{
					webRequest.Proxy = new WebProxy(localServer.InternetWebProxy);
					this.Tracer.LogInformation(this.GetHashCode(), "Using custom InternetWebProxy {0}.", new object[]
					{
						localServer.InternetWebProxy
					});
				}
				if (this.clientRequestId != null)
				{
					webRequest.Headers["client-request-id"] = this.clientRequestId.Value.ToString();
					webRequest.Headers["return-client-request-id"] = bool.TrueString;
				}
				using (Stream stream = webRequest.EndGetRequestStream(webRequest.BeginGetRequestStream(null, null)))
				{
					using (StreamWriter streamWriter = new StreamWriter(stream, Encoding.ASCII))
					{
						streamWriter.Write(text3);
					}
				}
				OAuth2AccessTokenResponse response = null;
				using (WebResponse response2 = webRequest.GetResponse())
				{
					this.Tracer.LogInformation(0, "[ACSTokenBuildRequest:GetActorTokenFromAuthServer] response headers was \n{0}", new object[]
					{
						response2.Headers
					});
					using (Stream responseStream = response2.GetResponseStream())
					{
						using (StreamReader streamReader = new StreamReader(responseStream))
						{
							response = (OAuth2MessageFactory.CreateFromEncodedResponse(streamReader) as OAuth2AccessTokenResponse);
						}
					}
				}
				tokenResult = new TokenResult(response);
				ACSTokenLifeTime.Instance.SetValue(tokenResult.RemainingTokenLifeTime);
			}
			catch (WebException ex)
			{
				string errorDescription = this.GetErrorDescription(ex);
				string text4 = ex.Status.ToString();
				Exception ex2 = null;
				if (!string.IsNullOrEmpty(errorDescription))
				{
					try
					{
						Dictionary<string, object> dictionary = errorDescription.DeserializeFromJson<Dictionary<string, object>>();
						if (dictionary != null)
						{
							object obj;
							text = ((dictionary.TryGetValue("error", out obj) && obj != null) ? obj.ToString() : text4);
							text2 = ((dictionary.TryGetValue("error_description", out obj) && obj != null) ? obj.ToString() : errorDescription);
						}
					}
					catch (ArgumentException ex3)
					{
						ex2 = ex3;
					}
					catch (InvalidOperationException ex4)
					{
						ex2 = ex4;
					}
					if (ex2 != null)
					{
						this.Tracer.LogInformation(this.GetHashCode(), "[ACSTokenBuildRequest:GetActorTokenFromAuthServer] fail to deserialize the ACS error string. Exception: {0}", new object[]
						{
							ex2
						});
					}
				}
				this.Tracer.LogError(this.GetHashCode(), "[ACSTokenBuildRequest:GetActorTokenFromAuthServer] Unable to get the token from auth server '{0}'. The request has token {1}, the error from ACS is {2}, the exception is {3}", new object[]
				{
					this.acsTokenIssuingEndpoint,
					tokenForACS,
					errorDescription,
					ex
				});
				if (ex.Status == WebExceptionStatus.Timeout)
				{
					OAuthCommon.PerfCounters.NumberOfAuthServerTimeoutTokenRequests.Increment();
				}
				if (ex.InnerException != null)
				{
					this.Tracer.LogError(this.GetHashCode(), "[ACSTokenBuildRequest:GetActorTokenFromAuthServer] the inner exception is {0}", new object[]
					{
						ex.InnerException
					});
				}
				if (throwOnError)
				{
					throw new OAuthTokenRequestFailedException(OAuthOutboundErrorCodes.UnableToGetTokenFromACS, new string[]
					{
						text,
						text2
					}, ex);
				}
			}
			finally
			{
				long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
				OAuthCommon.UpdateMovingAveragePerformanceCounter(OAuthCommon.PerfCounters.AverageAuthServerResponseTime, elapsedMilliseconds);
				ExTraceGlobals.OAuthTracer.TracePerformance<long>((long)this.GetHashCode(), "[ACSTokenBuildRequest:GetActorTokenFromAuthServer] Request token from ACS took {0} ms", elapsedMilliseconds);
				OutboundProtocolLog.BeginAppend(throwOnError ? "GetNewACSToken" : "RefreshACSToken", (tokenResult != null) ? "ok" : "fail", elapsedMilliseconds, this.caller, this.clientRequestId, this.acsTokenIssuingEndpoint, this.tenantId, this.resource, (tokenResult != null) ? null : OAuthOutboundErrorCodes.UnableToGetTokenFromACS.ToString(), (tokenResult != null) ? null : OAuthOutboundErrorsUtil.GetDescription(OAuthOutboundErrorCodes.UnableToGetTokenFromACS, new string[]
				{
					text,
					text2
				}), null, (this.tokenResult == null) ? TimeSpan.Zero : this.tokenResult.RemainingTokenLifeTime, tokenResult);
			}
			return tokenResult;
		}

		private string GetErrorDescription(WebException webException)
		{
			string result = string.Empty;
			WebResponse response = webException.Response;
			if (response != null)
			{
				if (response.Headers != null)
				{
					this.Tracer.LogInformation(0, "[ACSTokenBuildRequest:GetErrorDescription] response headers was\n{0}", new object[]
					{
						response.Headers
					});
				}
				try
				{
					using (Stream responseStream = response.GetResponseStream())
					{
						using (StreamReader streamReader = new StreamReader(responseStream))
						{
							result = streamReader.ReadToEnd();
						}
					}
				}
				catch (Exception ex)
				{
					this.Tracer.LogInformation(0, "[ACSTokenBuildRequest:GetErrorDescription] hit exception: {0}", new object[]
					{
						ex
					});
				}
			}
			return result;
		}

		private static readonly TimeSpan ACSTokenRequestTimeout = TimeSpan.FromMinutes(2.0);

		private static readonly TimeSpan ACSTokenRequestInterval = TimeSpan.FromMinutes(10.0);

		private readonly object lockObj = new object();

		private readonly string localIssuerId;

		private readonly X509Certificate2 signingKey;

		private readonly IssuerMetadata acsTokenIssuerMetadata;

		private readonly string acsTokenIssuingEndpoint;

		private readonly string tenantId;

		private readonly string resource;

		private readonly string caller;

		private readonly string selfKey;

		private readonly string partnerKey;

		private IOutboundTracer tracer = DefaultOutboundTracer.Instance;

		private Guid? clientRequestId;

		private DateTime lastRefreshDateTime = DateTime.MinValue;

		private TokenResult tokenResult;
	}
}
