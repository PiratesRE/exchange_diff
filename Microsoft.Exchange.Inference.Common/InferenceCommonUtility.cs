using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Search.Core.Abstraction;
using Microsoft.Exchange.Search.Core.Diagnostics;

namespace Microsoft.Exchange.Inference.Common
{
	internal static class InferenceCommonUtility
	{
		public static string ServerVersion
		{
			get
			{
				return "15.00.1497.012";
			}
		}

		public static bool ConfigTryParseHelper<T>(IPipelineComponentConfig config, InferenceCommonUtility.TryParseFunction<T> tryParseFunction, string keyName, out T value, IDiagnosticsSession trace)
		{
			value = default(T);
			if (config == null || !tryParseFunction(config[keyName], out value))
			{
				if (trace != null)
				{
					trace.TraceDebug<string>("Failed to parse config value referenced by keyName: {0}.", keyName);
				}
				return false;
			}
			return true;
		}

		public static bool ConfigTryParseHelper<T>(IPipelineComponentConfig config, InferenceCommonUtility.TryParseFunction<T> tryParseFunction, string keyName, out T value, IDiagnosticsSession trace, T defaultValue)
		{
			if (!InferenceCommonUtility.ConfigTryParseHelper<T>(config, tryParseFunction, keyName, out value, trace))
			{
				value = defaultValue;
				if (trace != null)
				{
					trace.TraceDebug<string, T>("Defaulting config value referenced by keyName: {0} to: {1}", keyName, defaultValue);
				}
				return false;
			}
			return true;
		}

		public static string GetExecutablePath(string fullPathWithParameters)
		{
			string result = null;
			if (!string.IsNullOrWhiteSpace(fullPathWithParameters))
			{
				char c;
				if (fullPathWithParameters.StartsWith("\""))
				{
					c = '"';
				}
				else
				{
					c = ' ';
				}
				string[] array = fullPathWithParameters.Split(new char[]
				{
					c
				}, StringSplitOptions.RemoveEmptyEntries);
				if (array.Length > 0)
				{
					result = array[0];
				}
			}
			return result;
		}

		internal static bool IsNonUserMailbox(string mailboxName)
		{
			return !string.IsNullOrEmpty(mailboxName) && (mailboxName.StartsWith("SystemMailbox", StringComparison.OrdinalIgnoreCase) || mailboxName.StartsWith("HealthMailbox", StringComparison.OrdinalIgnoreCase) || mailboxName.StartsWith("DiscoverySearchMailbox", StringComparison.OrdinalIgnoreCase) || mailboxName.StartsWith("Migration.", StringComparison.OrdinalIgnoreCase) || mailboxName.StartsWith("OrganizationalWorkflow", StringComparison.OrdinalIgnoreCase) || mailboxName.Equals("Microsoft Exchange", StringComparison.OrdinalIgnoreCase));
		}

		public static bool MatchBulkHeader(string data)
		{
			bool result = false;
			if (!string.IsNullOrEmpty(data))
			{
				result = Regex.IsMatch(data, "SRV:\\s?(SWL|BULK)");
			}
			return result;
		}

		public static string StringizeException(Exception ex)
		{
			StringBuilder stringBuilder = new StringBuilder(1024);
			int num = 0;
			while (ex != null && num < 5)
			{
				string text = string.Format("{0}:{1}:{2}", ex.GetType().FullName, ex.Message, ex.StackTrace);
				stringBuilder.Append(text.Replace("\n", "|").Replace("\r", "|").Replace("=", ":"));
				stringBuilder.Append("-----");
				num++;
				ex = ex.InnerException;
			}
			return stringBuilder.ToString();
		}

		public static string StringizeExceptions(List<Exception> exceptions)
		{
			if (exceptions == null || exceptions.Count == 0)
			{
				return string.Empty;
			}
			return string.Join("&", from ex in exceptions
			select InferenceCommonUtility.StringizeException(ex));
		}

		public delegate bool TryParseFunction<T>(string s, out T t);
	}
}
