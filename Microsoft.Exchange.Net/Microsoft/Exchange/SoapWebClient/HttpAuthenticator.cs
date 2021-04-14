using System;
using System.Net;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Net;
using Microsoft.Exchange.Security.Authentication;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.SoapWebClient
{
	internal abstract class HttpAuthenticator
	{
		public static HttpAuthenticator Create(ICredentials credentials)
		{
			return new HttpAuthenticator.GenericICredentialsHttpAuthenticator(credentials);
		}

		public static HttpAuthenticator Create(CommonAccessToken commonAccessToken)
		{
			return new HttpAuthenticator.CommonAccessTokenHttpAuthenticator(commonAccessToken);
		}

		public abstract T AuthenticateAndExecute<T>(CustomSoapHttpClientProtocol client, AuthenticateAndExecuteHandler<T> handler);

		public abstract T AuthenticateAndExecute<T>(WebRequest request, AuthenticateAndExecuteHandler<T> handler);

		public static readonly HttpAuthenticator None = new HttpAuthenticator.NoHttpAuthenticator();

		public static readonly HttpAuthenticator NetworkService = new HttpAuthenticator.NetworkServiceHttpAuthenticator();

		private static readonly Trace Tracer = ExTraceGlobals.EwsClientTracer;

		private sealed class NoHttpAuthenticator : HttpAuthenticator
		{
			public override T AuthenticateAndExecute<T>(CustomSoapHttpClientProtocol client, AuthenticateAndExecuteHandler<T> handler)
			{
				return handler();
			}

			public override T AuthenticateAndExecute<T>(WebRequest request, AuthenticateAndExecuteHandler<T> handler)
			{
				return handler();
			}
		}

		private sealed class GenericICredentialsHttpAuthenticator : HttpAuthenticator
		{
			public GenericICredentialsHttpAuthenticator(ICredentials credentials)
			{
				this.credentials = credentials;
			}

			public override T AuthenticateAndExecute<T>(CustomSoapHttpClientProtocol client, AuthenticateAndExecuteHandler<T> handler)
			{
				client.PreAuthenticate = true;
				client.Credentials = this.credentials;
				return handler();
			}

			public override T AuthenticateAndExecute<T>(WebRequest request, AuthenticateAndExecuteHandler<T> handler)
			{
				throw new NotImplementedException();
			}

			private ICredentials credentials;
		}

		private sealed class NetworkServiceHttpAuthenticator : HttpAuthenticator
		{
			public override T AuthenticateAndExecute<T>(CustomSoapHttpClientProtocol client, AuthenticateAndExecuteHandler<T> handler)
			{
				client.PreAuthenticate = true;
				T result;
				try
				{
					using (NetworkServiceImpersonator.Impersonate())
					{
						client.Credentials = CredentialCache.DefaultCredentials;
						HttpAuthenticator.Tracer.TraceDebug((long)this.GetHashCode(), "Impersonated network service.");
						result = handler();
					}
				}
				catch
				{
					throw;
				}
				return result;
			}

			public override T AuthenticateAndExecute<T>(WebRequest request, AuthenticateAndExecuteHandler<T> handler)
			{
				request.PreAuthenticate = true;
				T result;
				try
				{
					using (NetworkServiceImpersonator.Impersonate())
					{
						request.Credentials = CredentialCache.DefaultCredentials;
						HttpAuthenticator.Tracer.TraceDebug((long)this.GetHashCode(), "Impersonated network service.");
						result = handler();
					}
				}
				catch
				{
					throw;
				}
				return result;
			}
		}

		private sealed class CommonAccessTokenHttpAuthenticator : HttpAuthenticator
		{
			public CommonAccessTokenHttpAuthenticator(CommonAccessToken commonAccessToken)
			{
				this.token = commonAccessToken.Serialize();
			}

			public override T AuthenticateAndExecute<T>(CustomSoapHttpClientProtocol client, AuthenticateAndExecuteHandler<T> handler)
			{
				T result;
				try
				{
					using (NetworkServiceImpersonator.Impersonate())
					{
						HttpAuthenticator.Tracer.TraceDebug((long)this.GetHashCode(), "Impersonated network service.");
						client.PreAuthenticate = true;
						client.Credentials = CredentialCache.DefaultCredentials;
						client.HttpHeaders.Add("X-CommonAccessToken", this.token);
						result = handler();
					}
				}
				catch
				{
					throw;
				}
				return result;
			}

			public override T AuthenticateAndExecute<T>(WebRequest request, AuthenticateAndExecuteHandler<T> handler)
			{
				throw new NotImplementedException();
			}

			private readonly string token;
		}
	}
}
