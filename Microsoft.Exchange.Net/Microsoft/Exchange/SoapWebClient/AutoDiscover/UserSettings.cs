using System;
using System.Text;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.SoapWebClient.AutoDiscover
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class UserSettings
	{
		public UserSettings(UserResponse userResponse)
		{
			this.userResponse = userResponse;
		}

		public bool IsSettingError(string settingsName)
		{
			if (this.userResponse.UserSettingErrors != null)
			{
				foreach (UserSettingError userSettingError in this.userResponse.UserSettingErrors)
				{
					if (userSettingError != null && StringComparer.Ordinal.Equals(userSettingError.SettingName, settingsName) && userSettingError.ErrorCode != ErrorCode.NoError)
					{
						return true;
					}
				}
			}
			return false;
		}

		public UserSetting GetSetting(string settingsName)
		{
			if (this.userResponse.UserSettings != null)
			{
				foreach (UserSetting userSetting in this.userResponse.UserSettings)
				{
					if (userSetting != null && StringComparer.Ordinal.Equals(userSetting.Name, settingsName))
					{
						return userSetting;
					}
				}
			}
			return null;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(1000);
			if (this.userResponse != null)
			{
				if (this.userResponse.UserSettingErrors != null)
				{
					foreach (UserSettingError userSettingError in this.userResponse.UserSettingErrors)
					{
						if (userSettingError != null && userSettingError.ErrorCode != ErrorCode.NoError)
						{
							stringBuilder.AppendLine(string.Format("Error:{0}:{1}:{2}", userSettingError.SettingName, userSettingError.ErrorCode, userSettingError.ErrorMessage));
						}
					}
				}
				if (this.userResponse.UserSettings != null)
				{
					foreach (UserSetting userSetting in this.userResponse.UserSettings)
					{
						if (userSetting != null)
						{
							stringBuilder.AppendLine(userSetting.Name + "={" + UserSettings.GetUserSettingValue(userSetting) + "}");
						}
					}
				}
			}
			return stringBuilder.ToString();
		}

		private static string GetUserSettingValue(UserSetting userSetting)
		{
			string userSettingValue = UserSettings.GetUserSettingValue(userSetting as StringSetting);
			if (userSettingValue != null)
			{
				return userSettingValue;
			}
			userSettingValue = UserSettings.GetUserSettingValue(userSetting as AlternateMailboxCollectionSetting);
			if (userSettingValue != null)
			{
				return userSettingValue;
			}
			userSettingValue = UserSettings.GetUserSettingValue(userSetting as WebClientUrlCollectionSetting);
			if (userSettingValue != null)
			{
				return userSettingValue;
			}
			userSettingValue = UserSettings.GetUserSettingValue(userSetting as ProtocolConnectionCollectionSetting);
			if (userSettingValue != null)
			{
				return userSettingValue;
			}
			return "<unknown:" + userSetting.GetType().Name + ">";
		}

		private static string GetUserSettingValue(StringSetting setting)
		{
			if (setting != null)
			{
				return "string:" + setting.Value;
			}
			return null;
		}

		private static string GetUserSettingValue(AlternateMailboxCollectionSetting setting)
		{
			if (setting != null)
			{
				StringBuilder stringBuilder = new StringBuilder(100);
				stringBuilder.AppendLine("AlternateMailboxCollection: ");
				foreach (AlternateMailbox alternateMailbox in setting.AlternateMailboxes)
				{
					stringBuilder.AppendLine(string.Format("Type={0}, DisplayName={1}, LegacyDN={2}, Server={3}, SmtpAddress={4}, OwnerSmtpAddress={5}", new object[]
					{
						alternateMailbox.Type ?? "<null>",
						alternateMailbox.DisplayName ?? "<null>",
						alternateMailbox.LegacyDN ?? "<null>",
						alternateMailbox.Server ?? "<null>",
						alternateMailbox.SmtpAddress ?? "<null>",
						alternateMailbox.OwnerSmtpAddress ?? "<null>"
					}));
				}
				return stringBuilder.ToString();
			}
			return null;
		}

		private static string GetUserSettingValue(WebClientUrlCollectionSetting setting)
		{
			if (setting != null)
			{
				StringBuilder stringBuilder = new StringBuilder(100);
				stringBuilder.AppendLine("WebClientUrlCollection: ");
				foreach (WebClientUrl webClientUrl in setting.WebClientUrls)
				{
					stringBuilder.AppendLine(string.Format("AuthenticationMethods={0}, Url={1}", webClientUrl.AuthenticationMethods ?? "<null>", webClientUrl.Url ?? "<null>"));
				}
				return stringBuilder.ToString();
			}
			return null;
		}

		private static string GetUserSettingValue(ProtocolConnectionCollectionSetting setting)
		{
			if (setting != null)
			{
				StringBuilder stringBuilder = new StringBuilder(100);
				stringBuilder.AppendLine("ProtocolConnectionCollection: ");
				foreach (ProtocolConnection protocolConnection in setting.ProtocolConnections)
				{
					stringBuilder.AppendLine(string.Format("Hostname={0}, Port={1}, EncryptionMethod={2}", protocolConnection.Hostname ?? "<null>", protocolConnection.Port, protocolConnection.EncryptionMethod ?? "<null>"));
				}
				return stringBuilder.ToString();
			}
			return null;
		}

		private UserResponse userResponse;
	}
}
