using System;
using System.Net;
using System.Net.Mail;
using System.Threading;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.Smtp.Probes
{
	public interface IMinimalSmtpClient : IDisposable
	{
		CancellationToken CancellationToken { get; set; }

		ICredentialsByHost Credentials { get; set; }

		int Port { get; set; }

		bool UseDefaultCredentials { get; set; }

		int Timeout { get; set; }

		bool EnableSsl { get; set; }

		void Send(MailMessage message);
	}
}
