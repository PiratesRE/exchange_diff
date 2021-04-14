using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Autodiscover.ConfigurationSettings
{
	public class UserConfigurationSettings
	{
		public UserConfigurationSettings(HashSet<UserConfigurationSettingName> requestedSettings)
		{
			this.settings = new Dictionary<UserConfigurationSettingName, object>();
			this.requestedSettings = requestedSettings;
			this.errorCode = UserConfigurationSettingsErrorCode.NoError;
			this.errorMessage = string.Empty;
		}

		public UserConfigurationSettings() : this(new HashSet<UserConfigurationSettingName>())
		{
		}

		public UserConfigurationSettingsErrorCode ErrorCode
		{
			get
			{
				return this.errorCode;
			}
			set
			{
				this.errorCode = value;
			}
		}

		public string ErrorMessage
		{
			get
			{
				return this.errorMessage;
			}
			set
			{
				this.errorMessage = value;
			}
		}

		public string RedirectTarget
		{
			get
			{
				return this.redirectTarget;
			}
			set
			{
				this.redirectTarget = value;
			}
		}

		public void Add(UserConfigurationSettingName name, object value)
		{
			this.settings.Add(name, value);
		}

		public T Get<T>(UserConfigurationSettingName name)
		{
			object obj;
			if (this.settings.TryGetValue(name, out obj))
			{
				try
				{
					return (T)((object)obj);
				}
				catch (InvalidCastException)
				{
					string arg = string.Empty;
					if (obj != null)
					{
						arg = obj.GetType().FullName;
					}
					throw new ArgumentException(string.Format("Unable to cast Setting {0} value type '{1}' to '{2}'", name, arg, typeof(T).FullName));
				}
			}
			if (this.requestedSettings.Contains(name))
			{
				return default(T);
			}
			throw new ArgumentException(name + " was not a requested setting");
		}

		public string GetString(UserConfigurationSettingName name)
		{
			return this.Get<string>(name);
		}

		public IEnumerable<UserConfigurationSettingName> Keys
		{
			get
			{
				return this.settings.Keys;
			}
		}

		private Dictionary<UserConfigurationSettingName, object> settings;

		private HashSet<UserConfigurationSettingName> requestedSettings;

		private UserConfigurationSettingsErrorCode errorCode;

		private string errorMessage;

		private string redirectTarget;
	}
}
