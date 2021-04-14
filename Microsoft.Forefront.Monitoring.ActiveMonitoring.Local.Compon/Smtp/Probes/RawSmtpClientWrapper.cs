using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Security.Cryptography.X509Certificates;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.Smtp.Probes
{
	public class RawSmtpClientWrapper : IMinimalSmtpClient, IDisposable
	{
		public RawSmtpClientWrapper(string host, SmtpProbeWorkDefinition workDefinition, DelTraceDebug traceDebug)
		{
			if (string.IsNullOrWhiteSpace(host))
			{
				throw new ArgumentException("host must not be null or whitespace.");
			}
			if (workDefinition == null)
			{
				throw new ArgumentException("workDefinition must not be null.");
			}
			this.host = host;
			this.workDefinition = workDefinition;
			this.traceDebug = traceDebug;
			ChainEnginePool pool = new ChainEnginePool();
			this.cache = new CertificateCache(pool);
			this.cache.Open(OpenFlags.ReadOnly);
		}

		public string Host
		{
			get
			{
				return this.host;
			}
		}

		public CancellationToken CancellationToken { get; set; }

		public ICredentialsByHost Credentials { get; set; }

		public int Port { get; set; }

		public bool UseDefaultCredentials { get; set; }

		public int Timeout { get; set; }

		public bool EnableSsl { get; set; }

		public string LastResponse { get; private set; }

		public string EhloSent { get; private set; }

		public string FDContacted { get; private set; }

		public string ExchangeMessageId { get; private set; }

		public string HubServer { get; private set; }

		public SimpleSmtpClient.SmtpResponseCode LastResponseCode { get; private set; }

		public Exception LastEncounteredException { get; private set; }

		public string LastCommand { get; private set; }

		public bool SuccessfullySentLastMail { get; private set; }

		public void Send(MailMessage message)
		{
			this.SuccessfullySentLastMail = false;
			Guid guid = CombGuidGenerator.NewGuid();
			bool flag = false;
			using (SimpleSmtpClient simpleSmtpClient = new SimpleSmtpClient(this.CancellationToken))
			{
				try
				{
					this.ConnectWithRetry(simpleSmtpClient, this.workDefinition.SendMail.ConnectRetryCount);
					flag = true;
					this.CheckCancellation();
					this.CheckLastResponse(simpleSmtpClient);
					this.ExctractFDServer(simpleSmtpClient);
					this.EhloSent = guid.ToString() + "." + ComputerInformation.DnsPhysicalFullyQualifiedDomainName + ".OutsideInProbe";
					simpleSmtpClient.Ehlo(this.EhloSent);
					this.CheckCancellation();
					this.CheckLastResponse(simpleSmtpClient);
					this.ExtractClientIp(simpleSmtpClient);
					if (this.EnableSsl)
					{
						if (!simpleSmtpClient.IsXStartTlsAdvertised && this.workDefinition.SendMail.RequireTLS)
						{
							throw new RawSmtpClientWrapper.StartTlsNotAdvertisedException("STARTTLS was not advertised");
						}
						if (!simpleSmtpClient.IsXStartTlsAdvertised)
						{
							this.traceDebug("Starttls not advertised", new object[0]);
						}
						else
						{
							simpleSmtpClient.IgnoreCertificateNameMismatchPolicyError = this.workDefinition.SendMail.IgnoreCertificateNameMismatchPolicyError;
							if (this.workDefinition.ClientCertificate != null)
							{
								this.AddClientCertificatesToSmtp(simpleSmtpClient);
							}
							this.TraceDebug("STARTTLS", new object[0]);
							simpleSmtpClient.StartTls(false);
							this.CheckCancellation();
							this.CheckLastResponse(simpleSmtpClient);
							simpleSmtpClient.Ehlo(this.EhloSent);
							this.CheckCancellation();
							this.CheckLastResponse(simpleSmtpClient);
						}
					}
					if (!this.workDefinition.SendMail.Anonymous)
					{
						this.TraceDebug("AUTH", new object[0]);
						simpleSmtpClient.AuthLogin(this.workDefinition.SendMail.SenderUsername, this.workDefinition.SendMail.SenderPassword);
						this.CheckCancellation();
						this.CheckLastResponse(simpleSmtpClient);
					}
					if (this.workDefinition.SendMail.AuthOnly)
					{
						this.TraceDebug("Quit After Auth", new object[0]);
					}
					else
					{
						string text = simpleSmtpClient.IsXSysProbeAdvertised ? string.Format("{0} XSYSPROBEID={1}", this.workDefinition.SendMail.Message.Mail.From.ToString(), guid) : this.workDefinition.SendMail.Message.Mail.From.ToString();
						this.TraceDebug("MAILFROM:{0}", new object[]
						{
							text
						});
						simpleSmtpClient.MailFrom(text);
						this.CheckCancellation();
						this.CheckLastResponse(simpleSmtpClient);
						foreach (MailAddress mailAddress in this.workDefinition.SendMail.Message.Mail.To)
						{
							this.TraceDebug("RCPTTO:{0}", new object[]
							{
								mailAddress.ToString()
							});
							simpleSmtpClient.RcptTo(mailAddress.ToString());
							this.CheckCancellation();
							this.CheckLastResponse(simpleSmtpClient);
						}
						if (this.workDefinition.SendMail.RcptOnly)
						{
							this.TraceDebug("Quit After RCPT", new object[0]);
						}
						else
						{
							string text2 = Path.Combine(Path.GetTempPath(), guid.ToString());
							try
							{
								Directory.CreateDirectory(text2);
								Directory.GetFiles(text2, "*.eml").ToList<string>().ForEach(delegate(string f)
								{
									File.Delete(f);
								});
								using (SmtpClient smtpClient = new SmtpClient(this.host))
								{
									smtpClient.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
									smtpClient.PickupDirectoryLocation = text2;
									smtpClient.Send(message);
								}
								List<string> list = Directory.GetFiles(text2, "*.eml").ToList<string>();
								using (FileStream fileStream = File.OpenRead(list[0]))
								{
									this.TraceDebug("BDAT", new object[0]);
									simpleSmtpClient.BDat(fileStream, true);
									this.CheckCancellation();
									this.CheckLastResponse(simpleSmtpClient);
									this.SuccessfullySentLastMail = true;
								}
								this.TraceDebug("Response to BDAT: {0}", new object[]
								{
									simpleSmtpClient.LastResponse.Trim()
								});
								this.ExtractMessageId(simpleSmtpClient);
								this.ExtractHubServer(simpleSmtpClient);
							}
							finally
							{
								try
								{
									Directory.Delete(text2, true);
								}
								catch (IOException)
								{
								}
							}
						}
					}
				}
				catch (Exception ex)
				{
					this.LastEncounteredException = ex;
					if (!flag && !(ex is OperationCanceledException))
					{
						this.LastEncounteredException = new RawSmtpClientWrapper.MiscellaneousConnectionException(string.Format("Connection failure while trying to connect to {0} on port {1}{2}", this.host, this.Port, Environment.NewLine), ex);
					}
					this.LastCommand = ((simpleSmtpClient.LastCommand == null) ? null : simpleSmtpClient.LastCommand.Trim());
					this.LastResponse = ((simpleSmtpClient.LastResponse == null) ? null : simpleSmtpClient.LastResponse.Trim());
					this.LastResponseCode = simpleSmtpClient.LastResponseCode;
					throw;
				}
			}
		}

		public void Dispose()
		{
		}

		private static void ValidateCertificateExpiration(X509Certificate2 tlsCertificate)
		{
			string expirationDateString = tlsCertificate.GetExpirationDateString();
			DateTime t;
			if (!DateTime.TryParse(expirationDateString, out t))
			{
				throw new RawSmtpClientWrapper.InvalidCertificateException(string.Format("Unable to parse expiration time: {0}", expirationDateString));
			}
			if (t < DateTime.UtcNow)
			{
				throw new RawSmtpClientWrapper.InvalidCertificateException(string.Format("The certificate expired on  {0}{1} The Current Time is: {2}", expirationDateString, Environment.NewLine, DateTime.UtcNow));
			}
		}

		private static bool TryFindIp(string input, out string output)
		{
			Regex regex = new Regex("\\d{1,3}\\.\\d{1,3}\\.\\d{1,3}\\.\\d{1,3}");
			MatchCollection matchCollection = regex.Matches(input);
			if (matchCollection != null && matchCollection.Count > 0)
			{
				output = matchCollection[0].Value;
				return true;
			}
			regex = new Regex("([0-9a-fA-F]{1,4}:+){1,7}[0-9a-fA-F]{1,4}");
			matchCollection = regex.Matches(input);
			if (matchCollection != null && matchCollection.Count > 0)
			{
				output = matchCollection[0].Value;
				return true;
			}
			regex = new Regex("\\[.*\\]");
			matchCollection = regex.Matches(input);
			if (matchCollection == null || matchCollection.Count == 0)
			{
				output = null;
				return false;
			}
			foreach (object obj in matchCollection)
			{
				Match match = (Match)obj;
				string ipString = match.Value.Trim(new char[]
				{
					'[',
					']'
				});
				IPAddress ipaddress;
				if (IPAddress.TryParse(ipString, out ipaddress))
				{
					output = ipaddress.ToString();
					return true;
				}
			}
			output = null;
			return false;
		}

		private void ConnectWithRetry(SimpleSmtpClient client, int retryCount)
		{
			int num = 0;
			this.TraceDebug("Connecting to {0} on {1}", new object[]
			{
				this.host,
				this.Port
			});
			while (!client.IsConnected && num <= retryCount)
			{
				num++;
				this.CheckCancellation();
				try
				{
					client.Connect(this.host, this.Port, false);
				}
				catch
				{
					if (num > retryCount)
					{
						throw;
					}
				}
				if (!client.IsConnected && num <= retryCount)
				{
					this.TraceDebug("Failed to connect, retrying", new object[0]);
				}
			}
			if (!client.IsConnected)
			{
				throw new Exception(string.Format("[Connection failure while trying to connect to {0} on port {1}]", this.host, this.Port));
			}
			this.TraceDebug("connected", new object[0]);
		}

		private void ExctractFDServer(SimpleSmtpClient client)
		{
			this.TraceDebug("Found Banner: {0}", new object[]
			{
				client.LastResponse.Trim()
			});
			string[] array = client.LastResponse.Split(new char[]
			{
				' '
			});
			if (array.Count<string>() < 2)
			{
				this.FDContacted = "Unparsable Banner. See Execution Context";
				return;
			}
			this.FDContacted = array[1];
		}

		private void ExtractClientIp(SimpleSmtpClient client)
		{
			string text = null;
			if (RawSmtpClientWrapper.TryFindIp(client.LastResponse, out text))
			{
				this.traceDebug("Client IP: {0}", new object[]
				{
					text
				});
				return;
			}
			this.traceDebug("Couldn't Find IP.  Last Response: {0}", new object[]
			{
				client.LastResponse
			});
		}

		private void ExtractMessageId(SimpleSmtpClient client)
		{
			string[] array = client.LastResponse.Split(new char[]
			{
				' '
			});
			if (array.Count<string>() < 3)
			{
				this.ExchangeMessageId = "Unparsable MessageId. See Execution Context";
				return;
			}
			this.ExchangeMessageId = array[2];
		}

		private void ExtractHubServer(SimpleSmtpClient client)
		{
			Regex regex = new Regex("Hostname=(.*)] Queued mail for delivery");
			Match match = regex.Match(client.LastResponse);
			if (match != null && match.Success && match.Groups.Count > 0)
			{
				this.HubServer = match.Groups[1].Value;
				return;
			}
			this.HubServer = "Unparsable Hub Server Name.  See Execution Context";
		}

		private void CheckCancellation()
		{
			if (this.CancellationToken.IsCancellationRequested)
			{
				throw new OperationCanceledException();
			}
		}

		private void CheckLastResponse(SimpleSmtpClient client)
		{
			this.LastCommand = client.LastCommand.Trim();
			this.LastResponse = client.LastResponse.Trim();
			this.LastResponseCode = client.LastResponseCode;
			if (this.LastResponseCode == SimpleSmtpClient.SmtpResponseCode.OK || this.LastResponseCode == SimpleSmtpClient.SmtpResponseCode.AuthAccepted || this.LastResponseCode == SimpleSmtpClient.SmtpResponseCode.AuthPrompt || this.LastResponseCode == SimpleSmtpClient.SmtpResponseCode.ServiceReady || this.LastResponseCode == SimpleSmtpClient.SmtpResponseCode.DataAccepted)
			{
				return;
			}
			throw new Exception(string.Format("Unexpected SMTP Response: {0}", this.LastResponse));
		}

		private void AddClientCertificatesToSmtp(SimpleSmtpClient client)
		{
			client.ClientCertificates = new X509CertificateCollection();
			if (!string.IsNullOrWhiteSpace(this.workDefinition.ClientCertificate.FindValue))
			{
				X509Store x509Store = new X509Store(this.workDefinition.ClientCertificate.StoreName, this.workDefinition.ClientCertificate.StoreLocation);
				x509Store.Open(OpenFlags.OpenExistingOnly);
				try
				{
					X509Certificate2Collection x509Certificate2Collection = x509Store.Certificates.Find(this.workDefinition.ClientCertificate.FindType, this.workDefinition.ClientCertificate.FindValue, true);
					if (x509Certificate2Collection != null && x509Certificate2Collection.Count != 0)
					{
						this.TraceDebug("Certs Loaded", new object[0]);
						client.ClientCertificates.AddRange(x509Certificate2Collection);
					}
					else
					{
						this.TraceDebug("No Certs Found", new object[0]);
					}
					return;
				}
				finally
				{
					x509Store.Close();
				}
			}
			if (!string.IsNullOrWhiteSpace(this.workDefinition.ClientCertificate.TransportCertificateName))
			{
				X509Certificate2 x509Certificate = this.cache.Find(this.workDefinition.ClientCertificate.TransportCertificateName);
				if (x509Certificate != null)
				{
					RawSmtpClientWrapper.ValidateCertificateExpiration(x509Certificate);
					this.TraceDebug("Cert Loaded", new object[0]);
					client.ClientCertificates.Add(x509Certificate);
					return;
				}
				this.TraceDebug("No Certs Found", new object[0]);
				return;
			}
			else if (!string.IsNullOrWhiteSpace(this.workDefinition.ClientCertificate.TransportCertificateFqdn))
			{
				string text = string.IsNullOrEmpty(this.workDefinition.ClientCertificate.TransportCertificateFqdn) ? ComputerInformation.DnsPhysicalFullyQualifiedDomainName : this.workDefinition.ClientCertificate.TransportCertificateFqdn;
				X509Certificate2 x509Certificate2 = this.cache.Find(new string[]
				{
					text
				}, true, this.workDefinition.ClientCertificate.TransportWildcardMatchType);
				if (x509Certificate2 != null)
				{
					RawSmtpClientWrapper.ValidateCertificateExpiration(x509Certificate2);
					this.TraceDebug("Cert Loaded", new object[0]);
					client.ClientCertificates.Add(x509Certificate2);
					return;
				}
				this.TraceDebug("No Cert Found", new object[0]);
			}
		}

		private void TraceDebug(string format, params object[] args)
		{
			if (this.traceDebug != null)
			{
				this.traceDebug(format, args);
			}
		}

		private const string IpV4AddressRegEx = "\\d{1,3}\\.\\d{1,3}\\.\\d{1,3}\\.\\d{1,3}";

		private const string IpV6AddressRegEx = "([0-9a-fA-F]{1,4}:+){1,7}[0-9a-fA-F]{1,4}";

		private const string IpFallbackRegEx = "\\[.*\\]";

		private const string HostNameRegEx = "Hostname=(.*)] Queued mail for delivery";

		private readonly string host;

		private readonly CertificateCache cache;

		private readonly SmtpProbeWorkDefinition workDefinition;

		private readonly DelTraceDebug traceDebug;

		public class InvalidCertificateException : Exception
		{
			public InvalidCertificateException()
			{
			}

			public InvalidCertificateException(string message) : base(message)
			{
			}
		}

		public class StartTlsNotAdvertisedException : Exception
		{
			public StartTlsNotAdvertisedException()
			{
			}

			public StartTlsNotAdvertisedException(string message) : base(message)
			{
			}
		}

		public class MiscellaneousConnectionException : Exception
		{
			public MiscellaneousConnectionException()
			{
			}

			public MiscellaneousConnectionException(string message) : base(message)
			{
			}

			public MiscellaneousConnectionException(string message, Exception innerException) : base(message, innerException)
			{
			}
		}
	}
}
