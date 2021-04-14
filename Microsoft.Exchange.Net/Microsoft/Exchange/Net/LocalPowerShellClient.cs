using System;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.PswsClient;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.Net
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class LocalPowerShellClient
	{
		public LocalPowerShellClient(IAuthenticator authenticator)
		{
			ArgumentValidator.ThrowIfNull("authenticator", authenticator);
			this.authenticator = authenticator;
		}

		public void ApplyTo(PswsCmdlet cmdlet)
		{
			ArgumentValidator.ThrowIfNull("cmdlet", cmdlet);
			cmdlet.Authenticator = this.authenticator;
			cmdlet.HostServerName = "localhost";
			cmdlet.Port = 444;
			CertificateValidationManager.SetComponentId(cmdlet.AdditionalHeaders, "LocalPowerShellClient");
			CertificateValidationManager.RegisterCallback("LocalPowerShellClient", new RemoteCertificateValidationCallback(LocalPowerShellClient.CertificateHandler));
		}

		private static bool CertificateHandler(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			return sslPolicyErrors == SslPolicyErrors.None || sslPolicyErrors == SslPolicyErrors.RemoteCertificateNameMismatch;
		}

		private const string ComponentId = "LocalPowerShellClient";

		private readonly IAuthenticator authenticator;
	}
}
