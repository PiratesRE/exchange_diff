using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Security;
using Microsoft.Exchange.Diagnostics.Components.Diagnostics;
using Microsoft.Win32;

namespace Microsoft.Exchange.Diagnostics
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class ExEnvironment
	{
		public static bool IsTest
		{
			get
			{
				return ExEnvironment.Instance.IsTest;
			}
			set
			{
				ExEnvironment.Instance.IsTest = value;
			}
		}

		public static bool IsTestDomain
		{
			get
			{
				return ExEnvironment.Instance.IsTestDomain;
			}
		}

		public static bool IsDogfoodDomain
		{
			get
			{
				return ExEnvironment.Instance.IsDogfoodDomain;
			}
		}

		public static bool IsSdfDomain
		{
			get
			{
				return ExEnvironment.Instance.IsSdfDomain;
			}
		}

		public static bool TestRegistryKeyExists
		{
			get
			{
				return ExEnvironment.Instance.TestRegistryKeyExists;
			}
		}

		public static bool IsTestProcess
		{
			get
			{
				return ExEnvironment.Instance.IsTestProcess;
			}
			set
			{
				ExEnvironment.Instance.IsTestProcess = value;
			}
		}

		public static void Reset()
		{
			ExEnvironment.instance = null;
		}

		public static int GetTestRegistryValue(string subPath, string keyName, int defaultValue)
		{
			string keyName2 = "HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Exchange_Test\\v15" + subPath;
			try
			{
				object value = Registry.GetValue(keyName2, keyName, defaultValue);
				if (value is int)
				{
					return (int)value;
				}
			}
			catch (SecurityException)
			{
				ExTraceGlobals.CommonTracer.TraceError(0L, "Access to the Exchange test registry key was denied.");
			}
			catch (IOException)
			{
				ExTraceGlobals.CommonTracer.TraceError(0L, "IO exception occurred to access to the Exchange test registry.");
			}
			catch (ArgumentException)
			{
				ExTraceGlobals.CommonTracer.TraceError(0L, "Argument Exception occurred to access to the Exchange test registry key.");
			}
			return defaultValue;
		}

		private static ExEnvironment.Singleton Instance
		{
			get
			{
				if (ExEnvironment.instance == null)
				{
					ExEnvironment.instance = new ExEnvironment.Singleton();
				}
				return ExEnvironment.instance;
			}
		}

		private static ExEnvironment.Singleton instance;

		private class Singleton
		{
			public Singleton()
			{
				this.isTestDomain = ExEnvironment.Singleton.GetIsTestDomain();
				this.isDogfoodDomain = ExEnvironment.Singleton.GetIsDogfoodDomain();
				this.isSdfDomain = ExEnvironment.Singleton.GetIsSdfDomain();
				this.testRegistryKeyExists = ExEnvironment.Singleton.GetTestRegistryKeyExists();
				this.isTestProcess = ExEnvironment.Singleton.GetIsTestProcess();
				this.isTest = false;
				if (this.isTestDomain && this.testRegistryKeyExists)
				{
					this.isTest = true;
				}
				if (!this.isTest)
				{
					this.isTest = this.isTestProcess;
				}
			}

			public bool IsTest
			{
				get
				{
					return this.isTest;
				}
				set
				{
					this.isTest = value;
				}
			}

			public bool IsTestDomain
			{
				get
				{
					return this.isTestDomain;
				}
			}

			public bool IsDogfoodDomain
			{
				get
				{
					return this.isDogfoodDomain;
				}
			}

			public bool IsSdfDomain
			{
				get
				{
					return this.isSdfDomain;
				}
			}

			public bool TestRegistryKeyExists
			{
				get
				{
					return this.testRegistryKeyExists;
				}
			}

			public bool IsTestProcess
			{
				get
				{
					return this.isTestProcess;
				}
				set
				{
					this.isTestProcess = value;
				}
			}

			public static bool GetIsTestDomain()
			{
				return ExEnvironment.Singleton.GetIsInDomain(".EXTEST.MICROSOFT.COM");
			}

			public static bool GetIsDogfoodDomain()
			{
				return ExEnvironment.Singleton.GetIsInDomain(".EXCHANGE.CORP.MICROSOFT.COM");
			}

			public static bool GetIsSdfDomain()
			{
				return ExEnvironment.Singleton.GetIsInDomain(".SDF.EXCHANGELABS.COM");
			}

			public static bool GetTestRegistryKeyExists()
			{
				bool result = false;
				try
				{
					using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\Exchange_Test"))
					{
						result = (registryKey != null);
					}
				}
				catch (SecurityException)
				{
					ExTraceGlobals.CommonTracer.TraceError(0L, "Access to the Exchange test registry key was denied.");
				}
				return result;
			}

			public static bool GetIsTestProcess()
			{
				bool result = false;
				using (Process currentProcess = Process.GetCurrentProcess())
				{
					if (string.Equals(currentProcess.ProcessName, "PERSEUSHARNESSSERVICE", StringComparison.OrdinalIgnoreCase) || string.Equals(currentProcess.ProcessName, "PERSEUSHARNESSRUNTIME", StringComparison.OrdinalIgnoreCase) || string.Equals(currentProcess.ProcessName, "PERSEUSSTUDIO", StringComparison.OrdinalIgnoreCase) || string.Equals(currentProcess.ProcessName, "CPERSEUS", StringComparison.OrdinalIgnoreCase) || string.Equals(currentProcess.ProcessName, "contactsimporter", StringComparison.OrdinalIgnoreCase) || string.Equals(currentProcess.ProcessName, "Internal.Exchange.Inference.InferenceTool", StringComparison.OrdinalIgnoreCase) || string.Equals(currentProcess.ProcessName, "Internal.Exchange.Shared.Resource.Service", StringComparison.OrdinalIgnoreCase) || string.Equals(currentProcess.ProcessName, "topoagent", StringComparison.OrdinalIgnoreCase) || string.Equals(currentProcess.ProcessName, "xsoexplorer", StringComparison.OrdinalIgnoreCase) || string.Equals(currentProcess.ProcessName, "UNITP", StringComparison.OrdinalIgnoreCase))
					{
						result = true;
					}
				}
				return result;
			}

			private static bool GetIsInDomain(string domainSuffix)
			{
				bool result = false;
				string hostName = Dns.GetHostName();
				if (!string.IsNullOrEmpty(hostName))
				{
					IPHostEntry hostEntry = Dns.GetHostEntry(hostName);
					if (hostEntry != null && !string.IsNullOrEmpty(hostEntry.HostName))
					{
						result = hostEntry.HostName.EndsWith(domainSuffix, StringComparison.OrdinalIgnoreCase);
					}
				}
				return result;
			}

			private readonly bool isTestDomain;

			private readonly bool isDogfoodDomain;

			private readonly bool isSdfDomain;

			private readonly bool testRegistryKeyExists;

			private bool isTestProcess;

			private bool isTest;
		}
	}
}
