using System;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.PswsClient;

namespace Microsoft.Exchange.Security.Authentication
{
	internal static class Authenticator
	{
		public static IAuthenticator Create(ICredentials credentials)
		{
			ArgumentValidator.ThrowIfNull("credentials", credentials);
			return new Authenticator.CredentialsAuthenticator(credentials);
		}

		public static IAuthenticator Create(X509Certificate2 certificate)
		{
			ArgumentValidator.ThrowIfNull("certificate", certificate);
			return new Authenticator.CertificateAuthenticator(certificate);
		}

		public static readonly IAuthenticator NetworkService = new Authenticator.NetworkServiceAuthenticator();

		private sealed class NetworkServiceAuthenticator : IAuthenticator
		{
			public IDisposable Authenticate(HttpWebRequest request)
			{
				NetworkServiceImpersonator.Initialize();
				IDisposable result = NetworkServiceImpersonator.Impersonate();
				request.Credentials = CredentialCache.DefaultCredentials;
				return result;
			}
		}

		private sealed class CredentialsAuthenticator : IAuthenticator
		{
			public CredentialsAuthenticator(ICredentials credentials)
			{
				this.credentials = credentials;
			}

			public IDisposable Authenticate(HttpWebRequest request)
			{
				request.Credentials = this.credentials;
				return null;
			}

			private ICredentials credentials;
		}

		private sealed class CertificateAuthenticator : IAuthenticator
		{
			public CertificateAuthenticator(X509Certificate2 certificate)
			{
				this.certificate = certificate;
			}

			public NetworkCredential GetCredential(Uri uri, string authType)
			{
				throw new InvalidOperationException();
			}

			public IDisposable Authenticate(HttpWebRequest request)
			{
				request.ClientCertificates.Add(this.certificate);
				return null;
			}

			private readonly X509Certificate2 certificate;
		}
	}
}
