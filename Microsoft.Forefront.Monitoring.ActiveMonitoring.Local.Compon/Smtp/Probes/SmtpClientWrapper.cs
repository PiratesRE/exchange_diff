using System;
using System.Net;
using System.Net.Mail;
using System.Threading;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.Smtp.Probes
{
	public class SmtpClientWrapper : IMinimalSmtpClient, IDisposable
	{
		public SmtpClientWrapper(string host)
		{
			this.smtpClient = new SmtpClient(host);
		}

		public CancellationToken CancellationToken { get; set; }

		public ICredentialsByHost Credentials
		{
			get
			{
				return this.smtpClient.Credentials;
			}
			set
			{
				this.smtpClient.Credentials = value;
			}
		}

		public int Port
		{
			get
			{
				return this.smtpClient.Port;
			}
			set
			{
				this.smtpClient.Port = value;
			}
		}

		public bool UseDefaultCredentials
		{
			get
			{
				return this.smtpClient.UseDefaultCredentials;
			}
			set
			{
				this.smtpClient.UseDefaultCredentials = value;
			}
		}

		public int Timeout
		{
			get
			{
				return this.smtpClient.Timeout;
			}
			set
			{
				this.smtpClient.Timeout = value;
			}
		}

		public bool EnableSsl
		{
			get
			{
				return this.smtpClient.EnableSsl;
			}
			set
			{
				this.smtpClient.EnableSsl = value;
			}
		}

		public void Send(MailMessage message)
		{
			this.smtpClient.Send(message);
		}

		public void Dispose()
		{
			this.smtpClient.Dispose();
		}

		private SmtpClient smtpClient;
	}
}
