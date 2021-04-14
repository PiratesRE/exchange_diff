using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ForefrontActiveMonitoring;
using Microsoft.Exchange.Net.ExSmtpClient;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.Smtp.Probes
{
	public class SimpleSmtpClient : ISimpleSmtpClient, IDisposable
	{
		public SimpleSmtpClient(CancellationToken cancellationToken)
		{
			this.cancellationToken = cancellationToken;
			this.lastServerResponse = string.Empty;
			this.lastServerResponseCode = SimpleSmtpClient.SmtpResponseCode.Other;
			this.sessionText = new StringBuilder();
		}

		public bool IsConnected
		{
			get
			{
				return this.client != null && this.client.Connected;
			}
		}

		public string LastCommand { get; private set; }

		public string LastResponse
		{
			get
			{
				return this.lastServerResponse;
			}
		}

		public SimpleSmtpClient.SmtpResponseCode LastResponseCode
		{
			get
			{
				return this.lastServerResponseCode;
			}
		}

		public string Server
		{
			get
			{
				return this.server;
			}
		}

		public int Port
		{
			get
			{
				return this.port;
			}
		}

		public Stream Stream
		{
			get
			{
				return this.stream;
			}
		}

		public bool IsXSysProbeAdvertised
		{
			get
			{
				return this.isXSysProbeAdvertised;
			}
		}

		public bool IsXStartTlsAdvertised
		{
			get
			{
				return this.isStartTlsAdvertised;
			}
		}

		public string AdvertisedServerName
		{
			get
			{
				return this.advertisedServerName;
			}
		}

		public string SessionText
		{
			get
			{
				return this.sessionText.ToString();
			}
		}

		public X509CertificateCollection ClientCertificates
		{
			get
			{
				return this.clientCertificates;
			}
			set
			{
				this.clientCertificates = value;
			}
		}

		public SmtpConnectionProbeWorkDefinition.CertificateProperties RemoteCertificate
		{
			get
			{
				return this.remoteCertificate;
			}
		}

		public bool IgnoreCertificateNameMismatchPolicyError
		{
			get
			{
				return this.ignoreCertificateNameMismatchPolicyError;
			}
			set
			{
				this.ignoreCertificateNameMismatchPolicyError = value;
			}
		}

		public bool IgnoreCertificateChainPolicyErrorForSelfSigned
		{
			get
			{
				return this.ignoreCertificateChainPolicyErrorForSelfSigned;
			}
			set
			{
				this.ignoreCertificateChainPolicyErrorForSelfSigned = value;
			}
		}

		public bool Connect(string server, int port, bool disconnectIfConnected)
		{
			if (this.client != null && !disconnectIfConnected)
			{
				return false;
			}
			if (this.client != null && this.client.Connected)
			{
				this.Disconnect();
			}
			this.client = new TcpClient(server, port);
			this.stream = this.client.GetStream();
			this.server = server;
			this.port = port;
			this.receiveBufferSize = this.client.ReceiveBufferSize;
			this.GetServerResponse(true);
			this.LastCommand = "connect";
			return true;
		}

		public void Disconnect()
		{
			if (this.client != null && this.stream != null)
			{
				this.Quit();
			}
		}

		public void Helo(string domain = null)
		{
			this.SendCommand(string.Format("HELO{0}", string.IsNullOrEmpty(domain) ? string.Empty : (" " + domain)), true, true, true);
			this.GetAdvertisedServerName();
		}

		public void Ehlo(string domain = null)
		{
			this.SendCommand(string.Format("EHLO{0}", string.IsNullOrEmpty(domain) ? string.Empty : (" " + domain)), true, true, true);
			this.GetAdvertisedServerName();
			this.GetIsXSysProbeAdvertised();
			this.GetIsStartTlsAdvertised();
		}

		public void AuthLogin(string username, string password)
		{
			this.SendCommand("AUTH LOGIN", true, true, true);
			if (this.lastServerResponseCode == SimpleSmtpClient.SmtpResponseCode.AuthPrompt)
			{
				this.SendCommand(this.ConvertToBase64(username), true, false, true);
			}
			if (this.lastServerResponseCode == SimpleSmtpClient.SmtpResponseCode.AuthPrompt)
			{
				this.SendCommand(this.ConvertToBase64(password), true, false, true);
			}
		}

		public void AuthPlain(string username, string password)
		{
			this.SendCommand("AUTH PLAIN " + this.ConvertToBase64(string.Format("{0}\\0{0}\\0{1}", username, password)), true, true, true);
		}

		public void StartTls(bool useAnonymousTls)
		{
			if (useAnonymousTls)
			{
				this.SendCommand("X-ANONYMOUSTLS", true, true, true);
				if (this.lastServerResponseCode == SimpleSmtpClient.SmtpResponseCode.ServiceReady)
				{
					SmtpSslStream smtpSslStream = new SmtpSslStream(this.client.GetStream(), new SimpleSmtpClient.SmtpClientDebugOutput());
					smtpSslStream.Handshake();
					this.stream = smtpSslStream;
					return;
				}
			}
			else
			{
				this.SendCommand("STARTTLS", true, true, true);
				if (this.lastServerResponseCode == SimpleSmtpClient.SmtpResponseCode.ServiceReady)
				{
					SslStream sslStream = new SslStream(this.client.GetStream(), false, new RemoteCertificateValidationCallback(this.ValidateCertificate));
					Task task = sslStream.AuthenticateAsClientAsync(this.server, this.clientCertificates, SslProtocols.Default, false);
					task.Wait(this.cancellationToken);
					this.stream = sslStream;
				}
			}
		}

		public void ExchangeAuth()
		{
			SmtpSslStream smtpSslStream = this.stream as SmtpSslStream;
			if (smtpSslStream == null)
			{
				throw new AuthenticationException("X-ANONYMOUSTLS must be called before Exchange Authentication or the call to X-ANONYMOUSTLS was unsuccessful.");
			}
			using (SmtpAuth smtpAuth = new SmtpAuth(new SimpleSmtpClient.SmtpClientDebugOutput()))
			{
				string targetSPN = string.Format("smtpsvc/{0}", this.advertisedServerName);
				string str = smtpAuth.HandleOutboundAuth(null, targetSPN, smtpSslStream.CertificatePublicKey, smtpSslStream.SessionKey, true);
				string text = "X-EXPS EXCHANGEAUTH SHA256 ";
				this.sessionText.Append(text);
				this.SendCommand(text + str, true, true, false);
				if (this.lastServerResponseCode == SimpleSmtpClient.SmtpResponseCode.AuthAccepted)
				{
					string mutualBlob = ((string)this.lastServerResponse.Split(new char[]
					{
						' '
					}).GetValue(1)).Trim();
					smtpAuth.HandleOutboundAuth(mutualBlob, targetSPN, smtpSslStream.CertificatePublicKey, smtpSslStream.SessionKey, false);
					this.sessionText.Append("235 Exchange Auth Success ");
				}
				else
				{
					this.sessionText.Append(this.lastServerResponse);
				}
			}
		}

		public void MailFrom(string from)
		{
			this.SendCommand("MAIL FROM: " + from, true, true, true);
		}

		public void RcptTo(string to)
		{
			this.SendCommand("RCPT TO: " + to, true, true, true);
		}

		public void Data(string data)
		{
			this.SendCommand("DATA", true, true, true);
			this.SendCommand(string.Format("{0}\r\n.", data), true, true, true);
		}

		public void BDat(Stream stream, bool last)
		{
			if (stream.Length > 2147483647L)
			{
				throw new InvalidOperationException("Stream length should not be longer than max integer value");
			}
			this.SendCommand(string.Format("BDAT {0}{1}", stream.Length, last ? " LAST" : string.Empty), false, true, true);
			MemoryStream memoryStream = stream as MemoryStream;
			byte[] buffer;
			if (memoryStream == null)
			{
				buffer = new byte[stream.Length];
				Task<int> task = stream.ReadAsync(buffer, 0, (int)stream.Length, this.cancellationToken);
				task.Wait(this.cancellationToken);
			}
			else
			{
				buffer = memoryStream.GetBuffer();
			}
			this.SendCommand(buffer, (int)stream.Length, true, true);
		}

		public void Reset()
		{
			this.SendCommand("RSET", true, true, true);
		}

		public void Verify()
		{
			this.SendCommand("VRFY", true, true, true);
		}

		public void NoOp()
		{
			this.SendCommand("NOOP", true, true, true);
		}

		public void Quit()
		{
			this.SendCommand("QUIT", false, true, true);
		}

		public void Send(string text)
		{
			this.SendCommand(text, true, true, true);
			if (text.StartsWith("XPROXYTO", StringComparison.InvariantCultureIgnoreCase))
			{
				this.GetIsXSysProbeAdvertised();
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void SendCommand(string command, bool getServerResponse = true, bool recordLastCommand = true, bool recordCommandInHistory = true)
		{
			if (recordLastCommand)
			{
				this.LastCommand = command;
			}
			byte[] bytes = Encoding.ASCII.GetBytes(command + Environment.NewLine);
			if (recordCommandInHistory)
			{
				this.sessionText.AppendLine(command);
			}
			this.SendCommand(bytes, getServerResponse, recordCommandInHistory);
		}

		private void SendCommand(byte[] buffer, bool getServerResponse = true, bool recordCommandInHistory = true)
		{
			this.SendCommand(buffer, buffer.Length, getServerResponse, recordCommandInHistory);
		}

		private void SendCommand(byte[] buffer, int length, bool getServerResponse = true, bool recordCommandInHistory = true)
		{
			Task task = this.stream.WriteAsync(buffer, 0, length, this.cancellationToken);
			task.Wait(this.cancellationToken);
			if (getServerResponse)
			{
				this.GetServerResponse(recordCommandInHistory);
			}
		}

		private void GetServerResponse(bool recordResponseInHistory = true)
		{
			byte[] array = new byte[this.receiveBufferSize];
			Task<int> task = this.stream.ReadAsync(array, 0, array.Length, this.cancellationToken);
			task.Wait(this.cancellationToken);
			int result = task.Result;
			this.lastServerResponse = Encoding.UTF8.GetString(array, 0, result);
			this.lastServerResponseCode = this.GetServerResponseCode(this.lastServerResponse);
			if (recordResponseInHistory)
			{
				if (SimpleSmtpClient.responseCodeToTruncate.Contains(this.lastServerResponseCode) && !string.IsNullOrWhiteSpace(this.lastServerResponse))
				{
					string text = this.lastServerResponse;
					int num = text.IndexOf(Environment.NewLine);
					if (num >= 0)
					{
						text = text.Substring(0, num);
					}
					this.sessionText.Append(text);
					return;
				}
				this.sessionText.Append(this.lastServerResponse);
			}
		}

		private SimpleSmtpClient.SmtpResponseCode GetServerResponseCode(string responseText)
		{
			SimpleSmtpClient.SmtpResponseCode result = SimpleSmtpClient.SmtpResponseCode.Other;
			if (responseText.Length > 2)
			{
				int num = 0;
				if (int.TryParse(responseText.Substring(0, 3), out num) && Enum.IsDefined(typeof(SimpleSmtpClient.SmtpResponseCode), num))
				{
					result = (SimpleSmtpClient.SmtpResponseCode)num;
				}
			}
			return result;
		}

		private string ConvertToBase64(string text)
		{
			return Convert.ToBase64String(Encoding.ASCII.GetBytes(text));
		}

		private bool ValidateCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors policyErrors)
		{
			string text = "N/A";
			bool flag = false;
			if (policyErrors.HasFlag(SslPolicyErrors.RemoteCertificateNotAvailable))
			{
				throw new AuthenticationException("Remote certificate not found.");
			}
			if (certificate != null)
			{
				text = string.Format("Subject: {0}, Issuer: {1}, Effective: {2} to {3}.", new object[]
				{
					certificate.Subject,
					certificate.Issuer,
					certificate.GetEffectiveDateString(),
					certificate.GetExpirationDateString()
				});
				flag = certificate.Subject.Equals(certificate.Issuer, StringComparison.InvariantCultureIgnoreCase);
			}
			if (!this.ignoreCertificateNameMismatchPolicyError && policyErrors.HasFlag(SslPolicyErrors.RemoteCertificateNameMismatch))
			{
				throw new AuthenticationException("Remote certificate name mismatch. Certificate information - " + text);
			}
			if ((!this.IgnoreCertificateChainPolicyErrorForSelfSigned || !flag) && policyErrors.HasFlag(SslPolicyErrors.RemoteCertificateChainErrors))
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (X509ChainStatus x509ChainStatus in chain.ChainStatus)
				{
					stringBuilder.AppendLine(x509ChainStatus.StatusInformation);
				}
				text = text + " Chain status information: " + stringBuilder.ToString();
				throw new AuthenticationException("Remote certificate chain errors. Certificate information - " + text);
			}
			this.remoteCertificate = new SmtpConnectionProbeWorkDefinition.CertificateProperties();
			this.remoteCertificate.Subject = certificate.Subject;
			this.remoteCertificate.Issuer = certificate.Issuer;
			DateTime value;
			if (!DateTime.TryParse(certificate.GetEffectiveDateString(), null, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeLocal, out value))
			{
				throw new FormatException("Unable to recognize the effective date format of the server certificate.");
			}
			DateTime value2;
			if (!DateTime.TryParse(certificate.GetExpirationDateString(), null, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeLocal, out value2))
			{
				throw new FormatException("Unable to recognize the expiration date format of the server certificate.");
			}
			this.remoteCertificate.ValidFrom = new DateTime?(value);
			this.remoteCertificate.ValidTo = new DateTime?(value2);
			if (this.ignoreCertificateNameMismatchPolicyError)
			{
				policyErrors &= ~SslPolicyErrors.RemoteCertificateNameMismatch;
			}
			if (this.ignoreCertificateChainPolicyErrorForSelfSigned && flag)
			{
				policyErrors &= ~SslPolicyErrors.RemoteCertificateChainErrors;
			}
			return policyErrors == SslPolicyErrors.None;
		}

		private void GetAdvertisedServerName()
		{
			if (this.lastServerResponseCode == SimpleSmtpClient.SmtpResponseCode.OK)
			{
				int num = this.lastServerResponse.IndexOf(' ', 4);
				if (num > 4)
				{
					this.advertisedServerName = this.lastServerResponse.Substring(4, num - 4);
				}
			}
		}

		private void GetIsXSysProbeAdvertised()
		{
			this.isXSysProbeAdvertised = (this.lastServerResponseCode == SimpleSmtpClient.SmtpResponseCode.OK && this.lastServerResponse.IndexOf("XSYSPROBE", StringComparison.InvariantCultureIgnoreCase) > 0);
		}

		private void GetIsStartTlsAdvertised()
		{
			this.isStartTlsAdvertised = (this.lastServerResponseCode == SimpleSmtpClient.SmtpResponseCode.OK && this.lastServerResponse.IndexOf("STARTTLS", StringComparison.InvariantCultureIgnoreCase) > 0);
		}

		private void Dispose(bool disposing)
		{
			if (!this.isDisposed && disposing)
			{
				if (this.IsConnected)
				{
					this.Disconnect();
				}
				if (this.stream != null)
				{
					this.stream.Dispose();
				}
				this.isDisposed = true;
			}
		}

		private static HashSet<SimpleSmtpClient.SmtpResponseCode> responseCodeToTruncate = new HashSet<SimpleSmtpClient.SmtpResponseCode>
		{
			SimpleSmtpClient.SmtpResponseCode.ServiceReady,
			SimpleSmtpClient.SmtpResponseCode.Disconnect,
			SimpleSmtpClient.SmtpResponseCode.AuthAccepted,
			SimpleSmtpClient.SmtpResponseCode.OK
		};

		private readonly CancellationToken cancellationToken;

		private string server;

		private int port;

		private TcpClient client;

		private Stream stream;

		private int receiveBufferSize;

		private SimpleSmtpClient.SmtpResponseCode lastServerResponseCode;

		private string lastServerResponse;

		private StringBuilder sessionText;

		private string advertisedServerName;

		private X509CertificateCollection clientCertificates;

		private SmtpConnectionProbeWorkDefinition.CertificateProperties remoteCertificate;

		private bool ignoreCertificateNameMismatchPolicyError;

		private bool ignoreCertificateChainPolicyErrorForSelfSigned;

		private bool isDisposed;

		private bool isXSysProbeAdvertised;

		private bool isStartTlsAdvertised;

		public enum SmtpResponseCode
		{
			Other,
			ServiceReady = 220,
			Disconnect,
			AuthAccepted = 235,
			OK = 250,
			AuthPrompt = 334,
			DataAccepted = 354,
			BadCommand = 500,
			AuthRejected = 535,
			Rejected = 550,
			Failed = 554
		}

		internal class SmtpClientDebugOutput : ISmtpClientDebugOutput
		{
			public void Output(Trace tracer, object context, string message, params object[] args)
			{
				if (!string.IsNullOrEmpty(message))
				{
					WTFDiagnostics.TraceDebug(ExTraceGlobals.SMTPConnectionTracer, new TracingContext(), message, args, null, "Output", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\Smtp\\Probes\\SimpleSmtpClient.cs", 876);
				}
			}
		}
	}
}
