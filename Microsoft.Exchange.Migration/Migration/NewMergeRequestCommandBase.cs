using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Migration.DataAccessLayer;

namespace Microsoft.Exchange.Migration
{
	internal class NewMergeRequestCommandBase : NewMrsRequestCommandBase
	{
		protected NewMergeRequestCommandBase(string commandName, ExchangeOutlookAnywhereEndpoint endpoint, ExchangeJobItemSubscriptionSettings subscriptionSettings, bool whatIf, bool useAdmin) : base(commandName, NewMergeRequestCommandBase.ExceptionsToIgnore)
		{
			MigrationUtil.ThrowOnNullArgument(endpoint, "endpoint");
			MigrationUtil.ThrowOnNullArgument(subscriptionSettings, "subscriptionSettings");
			base.BadItemLimit = Unlimited<int>.UnlimitedValue;
			base.LargeItemLimit = Unlimited<int>.UnlimitedValue;
			base.WhatIf = whatIf;
			this.IsAdministrativeCredential = useAdmin;
			this.ApplyConnectionSettings(endpoint, subscriptionSettings);
			this.AuthenticationMethod = endpoint.AuthenticationMethod;
			this.SourceMailboxLegacyDN = subscriptionSettings.MailboxDN;
			if (!whatIf)
			{
				base.AddParameter("SkipMerging", "InitialConnectionValidation");
			}
		}

		public string SourceMailboxLegacyDN
		{
			set
			{
				base.AddParameter("RemoteSourceMailboxLegacyDN", value);
			}
		}

		public AuthenticationMethod AuthenticationMethod
		{
			set
			{
				base.AddParameter("AuthenticationMethod", value);
			}
		}

		public bool IsAdministrativeCredential
		{
			set
			{
				base.AddParameter("IsAdministrativeCredential", value);
			}
		}

		public string RequestName
		{
			set
			{
				base.AddParameter("Name", value);
			}
		}

		public DateTime? StartAfter
		{
			set
			{
				base.AddParameter("StartAfter", value);
			}
		}

		protected void ApplyConnectionSettings(ExchangeOutlookAnywhereEndpoint endpoint, ExchangeJobItemSubscriptionSettings subscriptionSettings)
		{
			base.AddParameter("RemoteCredential", endpoint.Credentials);
			Fqdn fqdn = string.IsNullOrEmpty(subscriptionSettings.RPCProxyServer) ? endpoint.RpcProxyServer : new Fqdn(subscriptionSettings.RPCProxyServer);
			base.AddParameter("OutlookAnywhereHostName", fqdn.ToString());
			base.AddParameter("RemoteSourceMailboxServerLegacyDN", subscriptionSettings.ExchangeServerDN);
		}

		internal const string AuthenticationMethodParameter = "AuthenticationMethod";

		internal const string IsAdministrativeCredentialParameter = "IsAdministrativeCredential";

		internal const string StartAfterParameter = "StartAfter";

		protected static readonly Type[] ExceptionsToIgnore = new Type[]
		{
			typeof(ManagementObjectAlreadyExistsException)
		};
	}
}
