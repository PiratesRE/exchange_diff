using System;
using System.Configuration;
using System.Diagnostics;
using System.Web.Configuration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Common;

namespace Microsoft.Exchange.Common
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal static class AppConfigLoader
	{
		public static bool IsWebApp
		{
			get
			{
				lock (AppConfigLoader.initLock)
				{
					if (AppConfigLoader.isWebApp == null)
					{
						AppConfigLoader.isWebApp = new bool?(Process.GetCurrentProcess().MainModule.ModuleName.Equals("w3wp.exe", StringComparison.OrdinalIgnoreCase));
					}
				}
				return AppConfigLoader.isWebApp.Value;
			}
		}

		public static bool GetConfigBoolValue(string configName, bool defaultValue)
		{
			return AppConfigLoader.GetConfigValue<bool>(configName, null, null, defaultValue, new AppConfigLoader.TryParseValue<bool>(bool.TryParse));
		}

		public static string GetConfigStringValue(string configName, string defaultValue)
		{
			string result = null;
			if (!AppConfigLoader.TryGetConfigRawValue(configName, out result))
			{
				return defaultValue;
			}
			return result;
		}

		public static T GetConfigEnumValue<T>(string configName, T defaultValue)
		{
			string text = null;
			if (!AppConfigLoader.TryGetConfigRawValue(configName, out text))
			{
				return defaultValue;
			}
			T result;
			try
			{
				T t = (T)((object)Enum.Parse(typeof(T), text, true));
				result = t;
			}
			catch (ArgumentException arg)
			{
				AppConfigLoader.Tracer.TraceWarning<string, string, ArgumentException>(0L, "Invalid Value:{0}, for config:{1}, parsing failed with error:{2}", text, configName, arg);
				result = defaultValue;
			}
			return result;
		}

		public static int GetConfigIntValue(string configName, int minValue, int maxValue, int defaultValue)
		{
			return AppConfigLoader.GetConfigValue<int>(configName, new int?(minValue), new int?(maxValue), defaultValue, new AppConfigLoader.TryParseValue<int>(int.TryParse));
		}

		public static double GetConfigDoubleValue(string configName, double minValue, double maxValue, double defaultValue)
		{
			return AppConfigLoader.GetConfigValue<double>(configName, new double?(minValue), new double?(maxValue), defaultValue, new AppConfigLoader.TryParseValue<double>(double.TryParse));
		}

		public static TimeSpan GetConfigTimeSpanValue(string configName, TimeSpan minValue, TimeSpan maxValue, TimeSpan defaultValue)
		{
			return AppConfigLoader.GetConfigValue<TimeSpan>(configName, new TimeSpan?(minValue), new TimeSpan?(maxValue), defaultValue, new AppConfigLoader.TryParseValue<TimeSpan>(TimeSpan.TryParse));
		}

		public static bool TryGetConfigRawValue(string configName, out string rawValue)
		{
			if (configName == null)
			{
				throw new ArgumentNullException(configName);
			}
			if (configName.Equals(string.Empty, StringComparison.OrdinalIgnoreCase))
			{
				throw new ArgumentException("ConfigName cannot be empty", configName);
			}
			rawValue = null;
			try
			{
				if (AppConfigLoader.IsWebApp)
				{
					rawValue = WebConfigurationManager.AppSettings[configName];
				}
				else
				{
					rawValue = ConfigurationManager.AppSettings[configName];
				}
			}
			catch (ConfigurationErrorsException arg)
			{
				AppConfigLoader.Tracer.TraceWarning<string, ConfigurationErrorsException>(0L, "failed to read config {0}: {1}", configName, arg);
				return false;
			}
			if (rawValue == null)
			{
				AppConfigLoader.Tracer.TraceDebug<string>(0L, "cannot apply null config {0}", configName);
				return false;
			}
			return true;
		}

		private static T GetConfigValue<T>(string configName, T? minValue, T? maxValue, T defaultValue, AppConfigLoader.TryParseValue<T> tryParseValue) where T : struct, IComparable<T>
		{
			string text = null;
			if (!AppConfigLoader.TryGetConfigRawValue(configName, out text))
			{
				return defaultValue;
			}
			T t = defaultValue;
			if (!tryParseValue(text, out t))
			{
				AppConfigLoader.Tracer.TraceWarning<string, string>(0L, "cannot apply config {0} with invalid value: {1}", configName, text);
				return defaultValue;
			}
			if (minValue != null && t.CompareTo(minValue.Value) < 0)
			{
				AppConfigLoader.Tracer.TraceWarning<string, T, T>(0L, "cannot apply config:{0}, value:{1} is less than minValue:{2}", configName, t, minValue.Value);
				return defaultValue;
			}
			if (maxValue != null && t.CompareTo(maxValue.Value) > 0)
			{
				AppConfigLoader.Tracer.TraceWarning<string, T, T>(0L, "cannot apply config:{0}, value:{1} is greater than maxValue:{2}", configName, t, maxValue.Value);
				return defaultValue;
			}
			return t;
		}

		private static readonly Microsoft.Exchange.Diagnostics.Trace Tracer = ExTraceGlobals.AppConfigLoaderTracer;

		private static bool? isWebApp = null;

		private static object initLock = new object();

		private delegate bool TryParseValue<T>(string stringValue, out T configValue);
	}
}
