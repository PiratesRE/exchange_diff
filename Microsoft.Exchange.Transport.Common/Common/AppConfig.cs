using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Globalization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Transport.Common
{
	internal abstract class AppConfig
	{
		protected AppConfig(NameValueCollection appSettings = null)
		{
			this.AppSettings = (appSettings ?? ConfigurationManager.AppSettings);
		}

		protected T GetConfigValue<T>(string label, T min, T max, T defaultValue, AppConfig.TryParse<T> tryParse) where T : IComparable<T>
		{
			string value = this.AppSettings[label];
			T result;
			this.TryParseConfigValue<T>(label, value, min, max, defaultValue, tryParse, out result);
			return result;
		}

		protected T GetConfigValue<T>(string label, T defaultValue, AppConfig.TryParse<T> tryParse)
		{
			string value = this.AppSettings[label];
			T result;
			this.TryParseConfigValue<T>(value, defaultValue, tryParse, out result);
			return result;
		}

		protected List<T> GetConfigList<T>(string label, char separator, AppConfig.TryParse<T> tryParse)
		{
			string configValuesString = this.AppSettings[label];
			return this.GetConfigListFromValue<T>(configValuesString, separator, tryParse);
		}

		protected ByteQuantifiedSize GetConfigByteQuantifiedSize(string label, ByteQuantifiedSize min, ByteQuantifiedSize max, ByteQuantifiedSize defaultValue)
		{
			return this.GetConfigValue<ByteQuantifiedSize>(label, min, max, defaultValue, new AppConfig.TryParse<ByteQuantifiedSize>(ByteQuantifiedSize.TryParse));
		}

		protected int GetConfigInt(string label, int min, int max, int defaultValue)
		{
			return this.GetConfigValue<int>(label, min, max, defaultValue, new AppConfig.TryParse<int>(int.TryParse));
		}

		protected long GetConfigLong(string label, long min, long max, long defaultValue)
		{
			return this.GetConfigValue<long>(label, min, max, defaultValue, new AppConfig.TryParse<long>(long.TryParse));
		}

		protected List<int> GetConfigIntList(string label, int min, int max, int defaultValue, char separator)
		{
			List<int> configList = this.GetConfigList<int>(label, separator, new AppConfig.TryParse<int>(int.TryParse));
			for (int i = 0; i < configList.Count; i++)
			{
				if (configList[i] < min || configList[i] > max)
				{
					configList[i] = defaultValue;
				}
			}
			return configList;
		}

		protected double GetConfigDouble(string label, double min, double max, double defaultValue)
		{
			return this.GetConfigValue<double>(label, min, max, defaultValue, new AppConfig.TryParse<double>(double.TryParse));
		}

		protected TimeSpan GetConfigTimeSpan(string label, TimeSpan min, TimeSpan max, TimeSpan defaultValue)
		{
			return this.GetConfigValue<TimeSpan>(label, min, max, defaultValue, new AppConfig.TryParse<TimeSpan>(TimeSpan.TryParse));
		}

		protected bool GetConfigBool(string label, bool defaultValue)
		{
			return this.GetConfigValue<bool>(label, defaultValue, new AppConfig.TryParse<bool>(bool.TryParse));
		}

		protected bool? GetConfigNullableBool(string label)
		{
			return this.GetConfigValue<bool?>(label, null, delegate(string s, out bool? parsed)
			{
				bool value = false;
				if (!string.IsNullOrEmpty(s) && bool.TryParse(s, out value))
				{
					parsed = new bool?(value);
					return true;
				}
				parsed = null;
				return false;
			});
		}

		protected T GetConfigEnum<T>(string label, T defaultValue) where T : struct
		{
			return this.GetConfigEnum<T>(label, defaultValue, EnumParseOptions.IgnoreCase);
		}

		protected T GetConfigEnum<T>(string label, T defaultValue, EnumParseOptions options) where T : struct
		{
			return this.GetConfigValue<T>(label, defaultValue, delegate(string s, out T parsed)
			{
				return EnumValidator.TryParse<T>(s, options, out parsed);
			});
		}

		protected string GetConfigString(string label, string defaultValue)
		{
			return this.AppSettings[label] ?? defaultValue;
		}

		private bool TryParseConfigValue<T>(string value, T defaultValue, AppConfig.TryParse<T> tryParse, out T configValue)
		{
			if (!string.IsNullOrEmpty(value) && tryParse(value, out configValue))
			{
				return true;
			}
			configValue = defaultValue;
			return false;
		}

		private bool TryParseConfigValue<T>(string label, string value, T min, T max, T defaultValue, AppConfig.TryParse<T> tryParse, out T configValue) where T : IComparable<T>
		{
			if (min != null && max != null && min.CompareTo(max) > 0)
			{
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Minimum must be smaller than or equal to Maximum (Config='{0}', Min='{1}', Max='{2}', Default='{3}').", new object[]
				{
					label,
					min,
					max,
					defaultValue
				}));
			}
			if (this.TryParseConfigValue<T>(value, defaultValue, tryParse, out configValue) && (min == null || configValue.CompareTo(min) >= 0) && (max == null || configValue.CompareTo(max) <= 0))
			{
				return true;
			}
			configValue = defaultValue;
			return false;
		}

		private List<T> GetConfigListFromValue<T>(string configValuesString, char separator, AppConfig.TryParse<T> tryParse)
		{
			List<T> list = new List<T>();
			if (!string.IsNullOrEmpty(configValuesString))
			{
				string[] array = configValuesString.Split(new char[]
				{
					separator
				});
				foreach (string value in array)
				{
					T item;
					if (this.TryParseConfigValue<T>(value, default(T), tryParse, out item))
					{
						list.Add(item);
					}
				}
			}
			return list;
		}

		protected readonly NameValueCollection AppSettings;

		public delegate bool TryParse<T>(string config, out T parsedConfig);
	}
}
