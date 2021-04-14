using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Exchange.Diagnostics.Components.Security;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Security.Authentication.FederatedAuthService
{
	internal abstract class AccountValidationContextBase : IAccountValidationContext
	{
		public string ApplicationName { get; protected set; }

		public ExDateTime AccountAuthTime { get; protected set; }

		public OrganizationId OrgId { get; protected set; }

		public AccountValidationContextBase(OrganizationId orgId, ExDateTime accountAuthTime, string appName)
		{
			this.OrgId = orgId;
			this.AccountAuthTime = accountAuthTime;
			this.ApplicationName = appName;
			this.protocolName = this.GetProtocolNameFromAppName(appName);
		}

		internal void SetOrgId(OrganizationId orgId)
		{
			if (orgId == null)
			{
				throw new ArgumentNullException("OrgId");
			}
			this.OrgId = orgId;
		}

		internal void SetAppName(string appName)
		{
			if (string.IsNullOrEmpty(appName))
			{
				throw new ArgumentNullException("appName");
			}
			this.ApplicationName = appName;
			this.protocolName = this.GetProtocolNameFromAppName(appName);
		}

		public virtual AccountState CheckAccount()
		{
			if (!ConfigBase<AdDriverConfigSchema>.GetConfig<bool>("AccountValidationEnabled"))
			{
				return AccountState.AccountEnabled;
			}
			if (this.OrgId == null)
			{
				throw new ArgumentNullException("OrgId");
			}
			if (this.protocolName == ProtocolName.Unknown)
			{
				throw new ArgumentException("ProtocolName is unknown, cannot check Protocol Settings");
			}
			ExDateTime accountAuthTime = this.AccountAuthTime;
			AccountState result;
			if (Enum.TryParse<AccountState>(Globals.GetValueFromRegistry<int>("SOFTWARE\\Microsoft\\ExchangeServer\\v15", "AccountState", 0, ExTraceGlobals.AuthenticationTracer).ToString(), true, out result))
			{
				return result;
			}
			return AccountState.AccountEnabled;
		}

		private ProtocolName GetProtocolNameFromAppName(string appName)
		{
			if (string.IsNullOrEmpty(appName))
			{
				return ProtocolName.Unknown;
			}
			string value = appName.Split(new char[]
			{
				'.'
			})[2];
			ProtocolName result;
			if (Enum.TryParse<ProtocolName>(value, true, out result))
			{
				return result;
			}
			return ProtocolName.Unknown;
		}

		public const string AccountValidationContextKey = "AccountValidationContext";

		internal const string RegistryPath = "SOFTWARE\\Microsoft\\ExchangeServer\\v15";

		internal const string AccountStateRegistryKey = "AccountState";

		private ProtocolName protocolName;
	}
}
