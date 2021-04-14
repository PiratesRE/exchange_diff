using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Security;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Extensions;
using Microsoft.Exchange.Migration.DataAccessLayer;
using Microsoft.Exchange.Migration.Logging;
using Microsoft.Exchange.WebServices.Autodiscover;
using Microsoft.Exchange.WebServices.Data;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class MigrationAutodiscoverClient : IMigrationAutodiscoverClient
	{
		public AutodiscoverClientResponse GetUserSettings(ExchangeOutlookAnywhereEndpoint endpoint, string emailAddress)
		{
			return this.GetUserSettings(null, endpoint.Username, endpoint.EncryptedPassword, endpoint.Domain, emailAddress);
		}

		public AutodiscoverClientResponse GetUserSettings(string userName, string encryptedPassword, string userDomain, string emailAddress)
		{
			return this.GetUserSettings(null, userName, encryptedPassword, userDomain, emailAddress);
		}

		private static string GetUserSettingsString(GetUserSettingsResponse userSettingsResponse)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("<GetUserSettingsResponse SmtpAddress='{0}'>", userSettingsResponse.SmtpAddress);
			stringBuilder.AppendFormat("<Error Code='{0}' Message='{1}' />", userSettingsResponse.ErrorCode, userSettingsResponse.ErrorMessage);
			stringBuilder.AppendFormat("<RedirectTarget Value='{0}' />", userSettingsResponse.RedirectTarget);
			stringBuilder.Append("<UserSettings>");
			foreach (KeyValuePair<UserSettingName, object> keyValuePair in userSettingsResponse.Settings)
			{
				stringBuilder.AppendFormat("<UserSetting Name='{0}' Value='{1}' />", keyValuePair.Key, keyValuePair.Value);
			}
			stringBuilder.Append("</UserSettings>");
			stringBuilder.Append("<UserSettingErrors>");
			foreach (UserSettingError userSettingError in userSettingsResponse.UserSettingErrors)
			{
				stringBuilder.AppendFormat("<UserSettingError Name='{0}' Code='{1}' Message='{2}' />", userSettingError.SettingName, userSettingError.ErrorCode, userSettingError.ErrorMessage);
			}
			stringBuilder.Append("</UserSettingErrors>");
			stringBuilder.Append("</GetUserSettingsResponse>");
			return stringBuilder.ToString();
		}

		private AutodiscoverClientResponse GetUserSettings(ExchangeVersion? version, string userName, string encryptedPassword, string userDomain, string emailAddress)
		{
			MigrationUtil.ThrowOnNullOrEmptyArgument(encryptedPassword, "encryptedPassword");
			MigrationUtil.ThrowOnNullOrEmptyArgument(emailAddress, "emailAddress");
			SecureString securePassword;
			Exception innerException;
			if (!MigrationServiceFactory.Instance.GetCryptoAdapter().TryEncryptedStringToSecureString(encryptedPassword, out securePassword, out innerException))
			{
				throw new MigrationDataCorruptionException(string.Format("could not decrypt password {0} for {1}", encryptedPassword, userName), innerException);
			}
			NetworkCredential credential = new NetworkCredential(userName, securePassword.AsUnsecureString(), userDomain);
			IEnumerable enumerable = (version != null) ? new ExchangeVersion[]
			{
				version.Value
			} : Enum.GetValues(typeof(ExchangeVersion));
			AutodiscoverClientResponse autodiscoverClientResponse = null;
			foreach (object obj in enumerable)
			{
				ExchangeVersion exchangeVersion = (ExchangeVersion)obj;
				autodiscoverClientResponse = this.GetUserSettings(exchangeVersion, credential, emailAddress);
				if (autodiscoverClientResponse.Status != AutodiscoverClientStatus.ConfigurationError)
				{
					break;
				}
			}
			return autodiscoverClientResponse;
		}

		private AutodiscoverClientResponse GetUserSettings(ExchangeVersion exchangeVersion, NetworkCredential credential, string emailAddress)
		{
			IAutodiscoverService autodiscoverService = MigrationServiceFactory.Instance.GetAutodiscoverService(exchangeVersion, credential);
			int num = 0;
			while (num++ <= 10)
			{
				GetUserSettingsResponse userSettings;
				try
				{
					userSettings = autodiscoverService.GetUserSettings(emailAddress, new UserSettingName[]
					{
						1,
						5,
						4,
						3,
						29,
						31
					});
					MigrationLogger.Log(MigrationEventType.Information, "Autodiscover completed. Exchange Version = {0}. Request = '{1}'. Response = {2}.", new object[]
					{
						exchangeVersion,
						emailAddress,
						MigrationAutodiscoverClient.GetUserSettingsString(userSettings)
					});
				}
				catch (ServiceLocalException ex)
				{
					MigrationLogger.Log(MigrationEventType.Warning, ex, "Autodiscover has thrown ServiceLocalException. Exchange Version = {0}. Request = '{1}'.", new object[]
					{
						exchangeVersion,
						emailAddress
					});
					return new AutodiscoverClientResponse(autodiscoverService.Url, ex, exchangeVersion);
				}
				catch (ServiceRemoteException ex2)
				{
					MigrationLogger.Log(MigrationEventType.Error, ex2, "Autodiscover has thrown ServiceRemoteException. Exchange Version = {0}. Request = '{1}'.", new object[]
					{
						exchangeVersion,
						emailAddress
					});
					return new AutodiscoverClientResponse(autodiscoverService.Url, ex2, exchangeVersion);
				}
				switch (userSettings.ErrorCode)
				{
				case 0:
					return new AutodiscoverClientResponse(autodiscoverService.Url, userSettings, exchangeVersion);
				case 1:
				{
					string text = string.Format("Autodiscover ('{0}') Address-Redirection ('{1}'). Discontinuing Autodiscover. Exchange Version = {2}. Request = '{3}'.", new object[]
					{
						autodiscoverService.Url,
						userSettings.RedirectTarget,
						exchangeVersion,
						emailAddress
					});
					MigrationLogger.Log(MigrationEventType.Error, text, new object[0]);
					return new AutodiscoverClientResponse(autodiscoverService.Url, text, 1, exchangeVersion);
				}
				case 2:
					MigrationLogger.Log(MigrationEventType.Warning, "Autodiscover Redirected! Old Url = '{0}', New Url = '{1}'. Exchange Version = {2}. Request = '{3}'.", new object[]
					{
						autodiscoverService.Url,
						userSettings.RedirectTarget,
						exchangeVersion,
						emailAddress
					});
					autodiscoverService.Url = new Uri(userSettings.RedirectTarget);
					continue;
				default:
					MigrationLogger.Log(MigrationEventType.Error, "Autodiscover ('{0}') failed ({1}). {2}. Discontinuing Autodiscover. Exchange Version = {3}. Request = '{4}'.", new object[]
					{
						autodiscoverService.Url,
						userSettings.ErrorCode,
						userSettings.ErrorMessage,
						exchangeVersion,
						emailAddress
					});
					return new AutodiscoverClientResponse(autodiscoverService.Url, userSettings.ErrorMessage, userSettings.ErrorCode, exchangeVersion);
				}
				AutodiscoverClientResponse result;
				return result;
			}
			string text2 = string.Format("Autodiscover ('{0}') Url-Redirection reached limit ({1} times). Discontinuing Autodiscover. Exchange Version = {2}. Request = '{3}'.", new object[]
			{
				autodiscoverService.Url,
				10,
				exchangeVersion,
				emailAddress
			});
			MigrationLogger.Log(MigrationEventType.Error, text2, new object[0]);
			return new AutodiscoverClientResponse(autodiscoverService.Url, text2, 2, exchangeVersion);
		}

		public const int MaxRedirectCount = 10;
	}
}
