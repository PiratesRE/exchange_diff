using System;
using System.IO;
using Microsoft.Win32;

namespace Microsoft.Exchange.Diagnostics
{
	internal class WatsonRegKeyReportAction : WatsonReportAction
	{
		public WatsonRegKeyReportAction(string keyName) : base(keyName, false)
		{
		}

		public override string ActionName
		{
			get
			{
				return "Registry Value";
			}
		}

		public override string Evaluate(WatsonReport watsonReport)
		{
			string registryValue = WatsonRegKeyReportAction.GetRegistryValue(base.Expression);
			string text;
			if (registryValue == null)
			{
				text = base.Expression + " not found.";
			}
			else
			{
				text = base.Expression + "=" + registryValue;
			}
			watsonReport.LogExtraData(text);
			return text;
		}

		private static string GetRegistryValue(string fullPath)
		{
			RegistryKey registryKey;
			if (fullPath.StartsWith("HKLM"))
			{
				registryKey = Registry.LocalMachine;
			}
			else if (fullPath.StartsWith("HKCU"))
			{
				registryKey = Registry.CurrentUser;
			}
			else if (fullPath.StartsWith("HKCR"))
			{
				registryKey = Registry.ClassesRoot;
			}
			else if (fullPath.StartsWith("HKCC"))
			{
				registryKey = Registry.CurrentConfig;
			}
			else
			{
				if (!fullPath.StartsWith("HKPD"))
				{
					return null;
				}
				registryKey = Registry.PerformanceData;
			}
			string path = fullPath.Substring("HKxx\\".Length);
			string directoryName = Path.GetDirectoryName(path);
			string fileName = Path.GetFileName(path);
			RegistryKey registryKey2 = null;
			string result;
			try
			{
				registryKey2 = registryKey.OpenSubKey(directoryName);
				object value = registryKey2.GetValue(fileName);
				if (value == null)
				{
					result = null;
				}
				else
				{
					result = value.ToString();
				}
			}
			finally
			{
				if (registryKey2 != null)
				{
					registryKey2.Close();
				}
			}
			return result;
		}
	}
}
