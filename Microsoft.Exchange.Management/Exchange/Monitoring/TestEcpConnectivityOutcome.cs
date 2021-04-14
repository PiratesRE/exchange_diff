using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Net.MonitoringWebClient;

namespace Microsoft.Exchange.Monitoring
{
	[Serializable]
	public class TestEcpConnectivityOutcome : CasTransactionOutcome
	{
		public TestEcpConnectivityOutcome(string virtualDirectoryName, string casServerName, string mailboxServerName, string localSite, Uri targetUri, VirtualDirectoryUriScope targetUriScope, string performanceCounterName, string userName) : base(casServerName, Strings.CasHealthEcpScenarioTestWebService, Strings.CasHealthEcpScenarioTestWebServiceDescription, performanceCounterName, localSite, targetUri != null && targetUri.AbsoluteUri != null && targetUri.AbsoluteUri.IndexOf("https", StringComparison.OrdinalIgnoreCase) == 0, userName, virtualDirectoryName, targetUri, targetUriScope)
		{
			this.MailboxServer = ((!string.IsNullOrEmpty(mailboxServerName)) ? ServerIdParameter.Parse(mailboxServerName) : null);
		}

		public TestEcpConnectivityOutcome(string casServerName, string mailboxServerName, string scenarioName, string scenarioDescription, string performanceCounterName, string localSite, bool secureAccess, string userName, string virtualDirectoryName, Uri targetUri, VirtualDirectoryUriScope targetUriScope) : base(casServerName, scenarioName, scenarioDescription, performanceCounterName, localSite, secureAccess, userName, virtualDirectoryName, targetUri, targetUriScope)
		{
			this.MailboxServer = ((!string.IsNullOrEmpty(mailboxServerName)) ? ServerIdParameter.Parse(mailboxServerName) : null);
		}

		public string AuthenticationMethod
		{
			get
			{
				return this.authenticationMethod;
			}
			internal set
			{
				this.authenticationMethod = value;
			}
		}

		public ServerIdParameter MailboxServer { get; private set; }

		public string FailureReason { get; internal set; }

		public string FailureSource { get; internal set; }

		public string FailingComponent { get; internal set; }

		public List<ResponseTrackerItem> HttpData { get; internal set; }

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(256);
			stringBuilder.Append(Strings.CasHealthOwaAuthMethodHeader);
			stringBuilder.Append(": ");
			stringBuilder.AppendLine((!string.IsNullOrEmpty(this.authenticationMethod)) ? this.authenticationMethod : "");
			stringBuilder.AppendLine(Strings.CasHealthOwaMailboxServerHeader);
			stringBuilder.AppendLine((this.MailboxServer != null) ? this.MailboxServer.ToString() : string.Empty);
			stringBuilder.Append(base.ToString());
			return stringBuilder.ToString();
		}

		private string authenticationMethod = string.Empty;
	}
}
