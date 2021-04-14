using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Microsoft.Exchange.HttpProxy
{
	public class LogData
	{
		public LogData()
		{
			this.tracker.Start();
		}

		internal static string[] LogColumnNames
		{
			get
			{
				return Enum.GetNames(typeof(LogKey));
			}
		}

		internal static LogKey[] LogKeys
		{
			get
			{
				return (LogKey[])Enum.GetValues(typeof(LogKey));
			}
		}

		internal object this[LogKey key]
		{
			get
			{
				if (key == LogKey.GenericInfo)
				{
					return this.genericInfo.ToString() + ";" + this.detailedLatencyInfo.ToString();
				}
				if (key == LogKey.GenericErrors)
				{
					return this.errorInfo.ToString();
				}
				if (this.data.Keys.Contains(key))
				{
					return this.data[key];
				}
				return null;
			}
			set
			{
				string text = string.Empty;
				if (value != null)
				{
					text = LogData.EscapeStringForCsvFormat(value.ToString());
				}
				if (key == LogKey.GenericInfo || key == LogKey.GenericErrors)
				{
					throw new InvalidOperationException("Cannot set GenericInfo or ErrorInfo directly. Use 'Append' methods.");
				}
				if (!this.data.ContainsKey(key))
				{
					this.data.Add(key, text);
					return;
				}
				string.Format("Attempting to overwrite logdata for key {0} with {1}.", key, text);
			}
		}

		public void AppendGenericInfo(string key, object value)
		{
			string text = LogData.EscapeStringForCsvFormat(value.ToString());
			if (!string.IsNullOrEmpty(key))
			{
				text = LogData.EscapeSeperatorsInGenericInfo(text);
				this.genericInfo.Append(key + "=" + text + ";");
			}
		}

		public void AppendErrorInfo(string key, object value)
		{
			string text = LogData.EscapeStringForCsvFormat(value.ToString());
			if (!string.IsNullOrEmpty(key))
			{
				text = LogData.EscapeSeperatorsInGenericInfo(text);
				this.errorInfo.Append(key + "=" + text + ";");
			}
		}

		public void LogElapsedTimeInDetailedLatencyInfo(string key)
		{
			this.detailedLatencyInfo.AppendFormat("{0}{1}{2}{3}", new object[]
			{
				key,
				"=",
				this.GetElapsedTime(),
				";"
			});
		}

		internal long GetElapsedTime()
		{
			return this.tracker.ElapsedMilliseconds;
		}

		private static string EscapeStringForCsvFormat(string value)
		{
			if (value.Contains(","))
			{
				value = value.Replace(',', ' ');
			}
			if (value.Contains("\r\n"))
			{
				value = value.Replace("\r\n", " ");
			}
			return value;
		}

		private static string EscapeSeperatorsInGenericInfo(string value)
		{
			if (value.Contains("="))
			{
				value = value.Replace("=", " ");
			}
			if (value.Contains(";"))
			{
				value = value.Replace(";", " ");
			}
			return value;
		}

		public const string KeyValueSeparator = "=";

		public const string PairSeparator = ";";

		private Dictionary<LogKey, object> data = new Dictionary<LogKey, object>();

		private StringBuilder genericInfo = new StringBuilder();

		private StringBuilder errorInfo = new StringBuilder();

		private Stopwatch tracker = new Stopwatch();

		private StringBuilder detailedLatencyInfo = new StringBuilder();
	}
}
