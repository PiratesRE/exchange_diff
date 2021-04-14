using System;
using System.Text;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Monitoring
{
	[Serializable]
	public class CasTransactionOutcome : TransactionOutcomeBase
	{
		internal CasTransactionOutcome(string clientAccessServer, string scenarioName, string scenarioDescription, string performanceCounterName, string localSite, bool secureAccess, string userName) : base((!string.IsNullOrEmpty(clientAccessServer)) ? ServerIdParameter.Parse(clientAccessServer).ToString() : string.Empty, scenarioName, scenarioDescription, performanceCounterName, userName)
		{
			this.LocalSite = ((!string.IsNullOrEmpty(localSite)) ? AdSiteIdParameter.Parse(localSite).ToString() : string.Empty);
			this.SecureAccess = secureAccess;
		}

		internal CasTransactionOutcome(string clientAccessServer, string scenarioName, string scenarioDescription, string performanceCounterName, string localSite, bool secureAccess, string userName, string virtualDirectoryName, Uri url, VirtualDirectoryUriScope urlType) : this(clientAccessServer, scenarioName, scenarioDescription, performanceCounterName, localSite, secureAccess, userName)
		{
			this.VirtualDirectoryName = virtualDirectoryName;
			this.Url = url;
			this.UrlType = urlType;
		}

		internal CasTransactionOutcome(string clientAccessServer, string scenarioName, string scenarioDescription, string performanceCounterName, string localSite, bool secureAccess, string userName, string virtualDirectoryName, Uri url, VirtualDirectoryUriScope urlType, int port, ProtocolConnectionType connectionType) : this(clientAccessServer, scenarioName, scenarioDescription, performanceCounterName, localSite, secureAccess, userName)
		{
			this.VirtualDirectoryName = virtualDirectoryName;
			this.Url = url;
			this.UrlType = urlType;
			this.Port = port;
			this.ConnectionType = connectionType;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(Strings.CasHealthClientAccessServerName + ": ");
			stringBuilder.Append(((base.ClientAccessServer != null) ? base.ClientAccessServer.ToString() : "") + "\r\n");
			stringBuilder.Append(Strings.CasHealthScenario + ": ");
			stringBuilder.Append(base.Scenario + "\r\n");
			stringBuilder.Append(Strings.CasHealthScenarioDescription + ": ");
			stringBuilder.Append(base.ScenarioDescription + "\r\n");
			stringBuilder.Append(Strings.CasHealthUserNameHeader + ": ");
			stringBuilder.Append((base.UserName ?? "null") + "\r\n");
			stringBuilder.Append(Strings.CasHealthPerformanceCounterName + ": ");
			stringBuilder.Append(base.PerformanceCounterName + "\r\n");
			stringBuilder.Append(Strings.CasHealthResult + ": ");
			stringBuilder.Append(base.Result + "\r\n");
			stringBuilder.Append(Strings.CasHealthSiteName + ": ");
			stringBuilder.Append(this.LocalSite + "\r\n");
			stringBuilder.Append(Strings.CasHealthLatency + ": ");
			stringBuilder.Append(base.Latency + "\r\n");
			stringBuilder.Append(Strings.CasHealthSecureAccess + ": ");
			stringBuilder.Append(this.SecureAccess + "\r\n");
			stringBuilder.Append(Strings.CasHealthConnectionType + ": ");
			stringBuilder.Append(this.ConnectionType + "\r\n");
			stringBuilder.Append(Strings.CasHealthPortnumber + ": ");
			stringBuilder.Append(this.Port + "\r\n");
			stringBuilder.Append(Strings.CasHealthLatencyHeader);
			stringBuilder.Append(": ");
			stringBuilder.Append(base.Latency.TotalMilliseconds);
			stringBuilder.Append(Environment.NewLine);
			stringBuilder.Append(Strings.CasHealthOwaVdirNameHeader);
			stringBuilder.Append(": ");
			stringBuilder.Append((this.VirtualDirectoryName != null) ? this.VirtualDirectoryName.ToString() : "null");
			stringBuilder.Append(Environment.NewLine);
			stringBuilder.Append(Strings.CasHealthOwaUriHeader);
			stringBuilder.Append(": ");
			stringBuilder.Append((this.Url != null) ? this.Url.ToString() : "null");
			stringBuilder.Append(Environment.NewLine);
			stringBuilder.Append(Strings.CasHealthOwaUriScopeHeader);
			stringBuilder.Append(": ");
			stringBuilder.Append(this.UrlType);
			stringBuilder.Append(Environment.NewLine);
			stringBuilder.Append(Strings.CasHealthAdditionalInformation + ": \r\n");
			stringBuilder.Append(base.Error + "\r\n");
			return stringBuilder.ToString();
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return CasTransactionOutcome.schema;
			}
		}

		public string LocalSite
		{
			get
			{
				return (string)this.propertyBag[CasTransactionOutcomeSchema.LocalSite];
			}
			internal set
			{
				this.propertyBag[CasTransactionOutcomeSchema.LocalSite] = value;
			}
		}

		public bool SecureAccess
		{
			get
			{
				return (bool)this.propertyBag[CasTransactionOutcomeSchema.SecureAccess];
			}
			internal set
			{
				this.propertyBag[CasTransactionOutcomeSchema.SecureAccess] = value;
			}
		}

		public string VirtualDirectoryName
		{
			get
			{
				return (string)this.propertyBag[CasTransactionOutcomeSchema.VirtualDirectoryName];
			}
			internal set
			{
				this.propertyBag[CasTransactionOutcomeSchema.VirtualDirectoryName] = value;
			}
		}

		public Uri Url
		{
			get
			{
				return (Uri)this.propertyBag[CasTransactionOutcomeSchema.Url];
			}
			internal set
			{
				this.propertyBag[CasTransactionOutcomeSchema.Url] = value;
			}
		}

		public VirtualDirectoryUriScope UrlType
		{
			get
			{
				return (VirtualDirectoryUriScope)this.propertyBag[CasTransactionOutcomeSchema.UrlType];
			}
			internal set
			{
				this.propertyBag[CasTransactionOutcomeSchema.UrlType] = value;
			}
		}

		public int Port
		{
			get
			{
				return (int)this.propertyBag[CasTransactionOutcomeSchema.Port];
			}
			internal set
			{
				this.propertyBag[CasTransactionOutcomeSchema.Port] = value;
			}
		}

		public ProtocolConnectionType ConnectionType
		{
			get
			{
				return (ProtocolConnectionType)this.propertyBag[CasTransactionOutcomeSchema.ConnectionType];
			}
			internal set
			{
				this.propertyBag[CasTransactionOutcomeSchema.ConnectionType] = value;
			}
		}

		public string ClientAccessServerShortName
		{
			get
			{
				string text = base.ClientAccessServer;
				if (!string.IsNullOrEmpty(text))
				{
					if (text.IndexOf('.') > 0)
					{
						text = text.Substring(0, text.IndexOf('.'));
					}
					return text;
				}
				return string.Empty;
			}
		}

		public string LocalSiteShortName
		{
			get
			{
				string text = this.LocalSite;
				if (!string.IsNullOrEmpty(text))
				{
					if (text.IndexOf('.') > 0)
					{
						text = text.Substring(0, text.IndexOf('.'));
					}
					return text;
				}
				return string.Empty;
			}
		}

		private static CasTransactionOutcomeSchema schema = ObjectSchema.GetInstance<CasTransactionOutcomeSchema>();
	}
}
