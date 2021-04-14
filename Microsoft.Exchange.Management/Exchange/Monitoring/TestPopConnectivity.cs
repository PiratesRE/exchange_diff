using System;
using System.Globalization;
using System.Management.Automation;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Monitoring
{
	[Cmdlet("Test", "PopConnectivity", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class TestPopConnectivity : ProtocolConnectivity
	{
		public TestPopConnectivity()
		{
			base.CmdletNoun = "PopConnectivity";
			base.MailSubjectPrefix = "Test-PopConnectivity-";
			base.CurrentProtocol = "POP3";
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
			return TransientErrorCache.POPTransientErrorCache;
		}

		protected override void ExecuteTestConnectivity(TestCasConnectivity.TestCasConnectivityRunInstance instance)
		{
			int perConnectionTimeout = base.PerConnectionTimeout;
			CasTransactionOutcome casTransactionOutcome = new CasTransactionOutcome(base.CasFqdn, Strings.TestProtocolConnectivity("POP3"), Strings.ProtocolConnectivityScenario("POP3"), "POPConnectivity-Latency", base.LocalSiteName, false, instance.credentials.UserName);
			if (base.PortClientAccessServer == 0)
			{
				switch (base.ConnectionType)
				{
				case ProtocolConnectionType.Plaintext:
				case ProtocolConnectionType.Tls:
					base.PortClientAccessServer = 110;
					break;
				case ProtocolConnectionType.Ssl:
					base.PortClientAccessServer = 995;
					break;
				}
			}
			else
			{
				switch (base.ConnectionType)
				{
				case ProtocolConnectionType.Plaintext:
				case ProtocolConnectionType.Tls:
					if (110 != base.PortClientAccessServer)
					{
						instance.Outcomes.Enqueue(new Warning(Strings.PopImapTransactionWarning(base.PortClientAccessServer, base.ConnectionType.ToString(), 995, 110)));
					}
					break;
				case ProtocolConnectionType.Ssl:
					if (995 != base.PortClientAccessServer)
					{
						instance.Outcomes.Enqueue(new Warning(Strings.PopImapTransactionWarning(base.PortClientAccessServer, base.ConnectionType.ToString(), 995, 110)));
					}
					break;
				}
			}
			casTransactionOutcome.Port = base.PortClientAccessServer;
			casTransactionOutcome.ConnectionType = base.ConnectionType;
			instance.Port = base.PortClientAccessServer;
			instance.ConnectionType = base.ConnectionType;
			LocalizedString localizedString = Strings.ProtocolTransactionsDetails(base.CasFqdn, string.IsNullOrEmpty(base.MailboxFqdn) ? base.User.ServerName : base.MailboxFqdn, base.User.Database.Name, base.User.PrimarySmtpAddress.ToString(), base.PortClientAccessServer.ToString(CultureInfo.InvariantCulture), base.ConnectionType.ToString(), this.trustAllCertificates);
			instance.Outcomes.Enqueue(localizedString);
			this.transactionTarget = new PopTransaction(base.User, this.casToTest.Name, instance.credentials.Password, base.PortClientAccessServer);
			this.transactionTarget.ConnectionType = base.ConnectionType;
			this.transactionTarget.TrustAnySslCertificate = instance.trustAllCertificates;
			this.transactionTarget.MailSubject = base.MailSubject;
			this.transactionTarget.TransactionResult = casTransactionOutcome;
			this.transactionTarget.Instance = instance;
			this.transactionTarget.Execute();
		}

		protected override string MonitoringLatencyPerformanceCounter()
		{
			return "POPConnectivity-Latency";
		}

		protected override uint GetDefaultTimeOut()
		{
			return 120U;
		}

		private const string PopProtocol = "POP3";

		private const string MonitoringPerformanceObject = "Exchange Monitoring";

		private const string MonitoringLatencyPerfCounter = "POPConnectivity-Latency";

		private const int MinimalTimeoutMSecForExtraTransaction = 12000;

		private const int SecurePort = 995;

		private const int NormalPort = 110;

		private const uint TaskSessionTimeoutSeconds = 120U;

		private PopTransaction transactionTarget;
	}
}
