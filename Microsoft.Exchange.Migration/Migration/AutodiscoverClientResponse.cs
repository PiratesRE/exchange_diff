using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.WebServices.Autodiscover;
using Microsoft.Exchange.WebServices.Data;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class AutodiscoverClientResponse
	{
		public AutodiscoverClientResponse(Uri autodiscoverUrl, Exception ex, ExchangeVersion exchangeVersion)
		{
			MigrationUtil.ThrowOnNullArgument(ex, "ex");
			this.Status = AutodiscoverClientStatus.ConfigurationError;
			this.ErrorMessage = ServerStrings.MigrationAutodiscoverConfigurationFailure;
			this.ErrorDetail = ex.ToString();
			this.ExchangeVersion = new ExchangeVersion?(exchangeVersion);
			this.EffectiveAutodiscoverUrl = ((autodiscoverUrl == null) ? null : autodiscoverUrl.ToString());
		}

		public AutodiscoverClientResponse(Uri autodiscoverUrl, string message, AutodiscoverErrorCode errorCode, ExchangeVersion exchangeVersion)
		{
			this.Status = AutodiscoverClientStatus.ConfigurationError;
			this.ErrorMessage = ServerStrings.MigrationAutodiscoverConfigurationFailure;
			this.ExchangeVersion = new ExchangeVersion?(exchangeVersion);
			this.EffectiveAutodiscoverUrl = ((autodiscoverUrl == null) ? null : autodiscoverUrl.ToString());
			this.ErrorDetail = string.Format("Error code:{0} message:'{1}'", errorCode, message);
		}

		public AutodiscoverClientResponse(Uri autodiscoverUrl, GetUserSettingsResponse userSettingsResponse, ExchangeVersion exchangeVersion)
		{
			this.Status = AutodiscoverClientStatus.NoError;
			this.ExchangeVersion = new ExchangeVersion?(exchangeVersion);
			this.EffectiveAutodiscoverUrl = ((autodiscoverUrl == null) ? null : autodiscoverUrl.ToString());
			string autodiscoverSetting = AutodiscoverClientResponse.GetAutodiscoverSetting(userSettingsResponse, 4);
			if (autodiscoverSetting == null)
			{
				autodiscoverSetting = AutodiscoverClientResponse.GetAutodiscoverSetting(userSettingsResponse, 3);
			}
			this.MailboxDN = AutodiscoverClientResponse.GetAutodiscoverSetting(userSettingsResponse, 1);
			this.ExchangeServer = autodiscoverSetting;
			this.ExchangeServerDN = AutodiscoverClientResponse.GetAutodiscoverSetting(userSettingsResponse, 5);
			this.RPCProxyServer = AutodiscoverClientResponse.GetAutodiscoverSetting(userSettingsResponse, 29);
			AuthenticationMethod value;
			Enum.TryParse<AuthenticationMethod>(AutodiscoverClientResponse.GetAutodiscoverSetting(userSettingsResponse, 31), out value);
			this.AuthenticationMethod = new AuthenticationMethod?(value);
			if (!this.Validate())
			{
				this.Status = AutodiscoverClientStatus.ConfigurationError;
				this.ErrorMessage = ServerStrings.MigrationAutodiscoverConfigurationFailure;
			}
		}

		public AutodiscoverClientResponse(MigrationAutodiscoverGetUserSettingsRpcResult result)
		{
			MigrationUtil.ThrowOnNullArgument(result, "result");
			this.Status = result.Status.Value;
			this.ExchangeVersion = result.ExchangeVersion;
			this.EffectiveAutodiscoverUrl = result.AutodiscoverUrl;
			switch (this.Status)
			{
			case AutodiscoverClientStatus.NoError:
				this.MailboxDN = result.MailboxDN;
				this.ExchangeServerDN = result.ExchangeServerDN;
				this.ExchangeServer = result.ExchangeServer;
				this.RPCProxyServer = result.RpcProxyServer;
				this.AuthenticationMethod = result.AuthenticationMethod;
				if (!this.Validate())
				{
					this.Status = AutodiscoverClientStatus.ConfigurationError;
					this.ErrorMessage = ServerStrings.MigrationAutodiscoverConfigurationFailure;
					return;
				}
				return;
			case AutodiscoverClientStatus.ConfigurationError:
				this.ErrorMessage = ServerStrings.MigrationAutodiscoverConfigurationFailure;
				this.ErrorDetail = result.ErrorMessage;
				return;
			}
			this.ErrorMessage = ServerStrings.MigrationAutodiscoverConfigurationFailure;
			this.ErrorDetail = result.ErrorMessage;
		}

		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"AutodiscoverClientResponse('",
				this.MailboxDN,
				"':'",
				this.ExchangeServerDN,
				"':'",
				this.ExchangeServer,
				"':'",
				this.RPCProxyServer,
				"':'",
				this.ExchangeVersion,
				"':'",
				this.AuthenticationMethod,
				"':'",
				this.EffectiveAutodiscoverUrl,
				"')"
			});
		}

		private static string GetAutodiscoverSetting(GetUserSettingsResponse response, UserSettingName key)
		{
			object obj;
			if (response.Settings.TryGetValue(key, out obj))
			{
				return (string)obj;
			}
			return null;
		}

		private bool Validate()
		{
			return !string.IsNullOrEmpty(this.MailboxDN) && !string.IsNullOrEmpty(this.ExchangeServerDN) && this.ExchangeServer != null && this.RPCProxyServer != null && this.AuthenticationMethod != null && this.ExchangeVersion != null;
		}

		public readonly LocalizedString ErrorMessage;

		public readonly string MailboxDN;

		public readonly string ExchangeServerDN;

		public readonly string ExchangeServer;

		public readonly string RPCProxyServer;

		public readonly string EffectiveAutodiscoverUrl;

		public readonly AutodiscoverClientStatus Status;

		public readonly ExchangeVersion? ExchangeVersion;

		public readonly AuthenticationMethod? AuthenticationMethod;

		public readonly string ErrorDetail;
	}
}
