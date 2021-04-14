using System;
using System.Globalization;
using System.Management.Automation;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Monitoring
{
	[Cmdlet("Test", "ImapConnectivity", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class TestImapConnectivity : ProtocolConnectivity
	{
		public TestImapConnectivity()
		{
			base.CmdletNoun = "ImapConnectivity";
			base.MailSubjectPrefix = "Test-ImapConnectivity-";
			base.CurrentProtocol = "IMAP4";
			base.SendTestMessage = true;
			base.ConnectionType = ProtocolConnectionType.Ssl;
			this.trustAllCertificates = true;
		}

		protected override string PerformanceObject
		{
			get
			{
				return this.MonitoringEventSource;
			}
		}

		protected override string MonitoringEventSource
		{
			get
			{
				return base.CmdletMonitoringEventSource;
			}
		}

		internal override TransientErrorCache GetTransientErrorCache()
		{
			if (!base.MonitoringContext)
			{
				return null;
			}
			return TransientErrorCache.IMAPTransientErrorCache;
		}

		protected override void ExecuteTestConnectivity(TestCasConnectivity.TestCasConnectivityRunInstance instance)
		{
			int perConnectionTimeout = base.PerConnectionTimeout;
			CasTransactionOutcome casTransactionOutcome = new CasTransactionOutcome(base.CasFqdn, Strings.TestProtocolConnectivity("IMAP4"), Strings.ProtocolConnectivityScenario("IMAP4"), "ImapConnectivity-Latency", base.LocalSiteName, false, instance.credentials.UserName);
			if (base.PortClientAccessServer == 0)
			{
				if (ProtocolConnectionType.Ssl == base.ConnectionType)
				{
					base.PortClientAccessServer = 993;
				}
				else
				{
					base.PortClientAccessServer = 143;
				}
			}
			else if (ProtocolConnectionType.Ssl == base.ConnectionType)
			{
				if (993 != base.PortClientAccessServer)
				{
					instance.Outcomes.Enqueue(new Warning(Strings.PopImapTransactionWarning(base.PortClientAccessServer, base.ConnectionType.ToString(), 993, 143)));
				}
			}
			else if (143 != base.PortClientAccessServer)
			{
				instance.Outcomes.Enqueue(new Warning(Strings.PopImapTransactionWarning(base.PortClientAccessServer, base.ConnectionType.ToString(), 993, 143)));
			}
			casTransactionOutcome.Port = base.PortClientAccessServer;
			casTransactionOutcome.ConnectionType = base.ConnectionType;
			instance.Port = base.PortClientAccessServer;
			instance.ConnectionType = base.ConnectionType;
			LocalizedString localizedString = Strings.ProtocolTransactionsDetails(base.CasFqdn, string.IsNullOrEmpty(base.MailboxFqdn) ? base.User.ServerName : base.MailboxFqdn, base.User.Database.Name, base.User.PrimarySmtpAddress.ToString(), base.PortClientAccessServer.ToString(CultureInfo.InvariantCulture), base.ConnectionType.ToString(), this.trustAllCertificates);
			instance.Outcomes.Enqueue(localizedString);
			this.transactionTarget = new ImapTransaction(base.User, this.casToTest.Name, instance.credentials.Password, base.PortClientAccessServer);
			this.transactionTarget.ConnectionType = base.ConnectionType;
			this.transactionTarget.TrustAnySslCertificate = instance.trustAllCertificates;
			this.transactionTarget.MailSubject = base.MailSubject;
			this.transactionTarget.TransactionResult = casTransactionOutcome;
			this.transactionTarget.Instance = instance;
			this.transactionTarget.Execute();
		}

		protected override string MonitoringLatencyPerformanceCounter()
		{
			return "ImapConnectivity-Latency";
		}

		protected override uint GetDefaultTimeOut()
		{
			return 120U;
		}

		private const string ImapProtocol = "IMAP4";

		private const int DefaultConnectionTimeoutInSeconds = 180;

		private const string MonitoringPerformanceObject = "Exchange Monitoring";

		private const string MonitoringLatencyPerfCounter = "ImapConnectivity-Latency";

		private const int MinimalTimeoutMSecForExtraTransaction = 12000;

		private const int SecurePort = 993;

		private const int NormalPort = 143;

		private const uint TaskSessionTimeoutSeconds = 120U;

		private ImapTransaction transactionTarget;
	}
}
