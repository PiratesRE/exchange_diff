using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.Smtp.Probes
{
	public interface ISimpleSmtpClient : IDisposable
	{
		string AdvertisedServerName { get; }

		bool IsConnected { get; }

		string LastResponse { get; }

		SimpleSmtpClient.SmtpResponseCode LastResponseCode { get; }

		string Server { get; }

		int Port { get; }

		string SessionText { get; }

		Stream Stream { get; }

		bool IsXSysProbeAdvertised { get; }

		X509CertificateCollection ClientCertificates { get; set; }

		SmtpConnectionProbeWorkDefinition.CertificateProperties RemoteCertificate { get; }

		bool IgnoreCertificateNameMismatchPolicyError { get; set; }

		bool IgnoreCertificateChainPolicyErrorForSelfSigned { get; set; }

		bool Connect(string server, int port, bool disconnectIfConnected);

		void Disconnect();

		void Helo(string domain = null);

		void Ehlo(string domain = null);

		void AuthLogin(string username, string password);

		void AuthPlain(string username, string password);

		void ExchangeAuth();

		void StartTls(bool useAnonymousTls);

		void MailFrom(string from);

		void RcptTo(string to);

		void Data(string data);

		void BDat(Stream stream, bool last);

		void Reset();

		void Verify();

		void NoOp();

		void Send(string text);
	}
}
