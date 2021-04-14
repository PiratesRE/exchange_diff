using System;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Reflection;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Monad;
using Microsoft.Win32;

namespace Microsoft.Exchange.Configuration.MonadDataProvider
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MonadRunspaceConfiguration : RunspaceConfiguration
	{
		private MonadRunspaceConfiguration()
		{
		}

		public override string ShellId
		{
			get
			{
				return "Exchange";
			}
		}

		public override RunspaceConfigurationEntryCollection<CmdletConfigurationEntry> Cmdlets
		{
			get
			{
				return this.miniShellCmdlets;
			}
		}

		private static bool IsEdgeMachine
		{
			get
			{
				if (MonadRunspaceConfiguration.isEdgeMachine == null)
				{
					object value = Registry.LocalMachine.GetValue("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\EdgeTransportRole\\AdamSettings\\MSExchange", null);
					MonadRunspaceConfiguration.isEdgeMachine = new bool?(null != value);
				}
				return MonadRunspaceConfiguration.isEdgeMachine.Value;
			}
		}

		public static void EnsureMonadScriptsAreRunnable()
		{
			using (RegistryKey registryKey = Registry.LocalMachine.CreateSubKey("SOFTWARE\\Microsoft\\PowerShell\\1\\ShellIds\\Microsoft.PowerShell"))
			{
				registryKey.SetValue("ExecutionPolicy", "RemoteSigned");
			}
		}

		public static void AddArray(CmdletConfigurationEntry[] cmdlets)
		{
			if (MonadRunspaceConfiguration.singleShellConfiguration != MonadRunspaceConfiguration.SingleShellConfigurationMode.Mini)
			{
				MonadRunspaceConfiguration.singleShellConfiguration = MonadRunspaceConfiguration.SingleShellConfigurationMode.Mixed;
			}
			MonadRunspaceConfiguration.cmdletConfigurationEntries.Add(cmdlets);
		}

		public static void AddPSSnapInName(string mshSnapInName)
		{
			if (MonadRunspaceConfiguration.cmdletConfigurationEntries.Count == 0)
			{
				MonadRunspaceConfiguration.singleShellConfiguration = MonadRunspaceConfiguration.SingleShellConfigurationMode.Default;
			}
			else
			{
				MonadRunspaceConfiguration.singleShellConfiguration = MonadRunspaceConfiguration.SingleShellConfigurationMode.Mixed;
			}
			MonadRunspaceConfiguration.mshSnapInNames.Add(mshSnapInName);
		}

		public static void Clear()
		{
			MonadRunspaceConfiguration.singleShellConfiguration = MonadRunspaceConfiguration.SingleShellConfigurationMode.Clear;
			MonadRunspaceConfiguration.ClearEntries();
		}

		public static void ClearAll()
		{
			MonadRunspaceConfiguration.singleShellConfiguration = MonadRunspaceConfiguration.SingleShellConfigurationMode.Mini;
			MonadRunspaceConfiguration.ClearEntries();
		}

		public new static RunspaceConfiguration Create()
		{
			RunspaceConfiguration runspaceConfiguration = null;
			if (MonadRunspaceConfiguration.singleShellConfiguration == MonadRunspaceConfiguration.SingleShellConfigurationMode.Mini)
			{
				MonadRunspaceConfiguration monadRunspaceConfiguration = new MonadRunspaceConfiguration();
				monadRunspaceConfiguration.miniShellCmdlets = new RunspaceConfigurationEntryCollection<CmdletConfigurationEntry>();
				foreach (CmdletConfigurationEntry[] items in MonadRunspaceConfiguration.cmdletConfigurationEntries)
				{
					monadRunspaceConfiguration.miniShellCmdlets.Append(items);
				}
				runspaceConfiguration = monadRunspaceConfiguration;
			}
			else
			{
				runspaceConfiguration = RunspaceConfiguration.Create();
			}
			if (MonadRunspaceConfiguration.singleShellConfiguration == MonadRunspaceConfiguration.SingleShellConfigurationMode.Default)
			{
				if (MonadRunspaceConfiguration.IsEdgeMachine)
				{
					MonadRunspaceConfiguration.AddPSSnapIn(runspaceConfiguration, "Microsoft.Exchange.Management.PowerShell.E2010");
				}
				else
				{
					CmdletConfigurationEntry[] nonEdgeCmdletConfigurationEntries = MonadRunspaceConfiguration.GetNonEdgeCmdletConfigurationEntries();
					runspaceConfiguration.Cmdlets.Append(nonEdgeCmdletConfigurationEntries);
				}
			}
			if (MonadRunspaceConfiguration.IsMixedOrDefaultMode())
			{
				foreach (string mshSnapInName in MonadRunspaceConfiguration.mshSnapInNames)
				{
					MonadRunspaceConfiguration.AddPSSnapIn(runspaceConfiguration, mshSnapInName);
				}
				if (MonadRunspaceConfiguration.singleShellConfiguration == MonadRunspaceConfiguration.SingleShellConfigurationMode.Mixed)
				{
					foreach (CmdletConfigurationEntry[] items2 in MonadRunspaceConfiguration.cmdletConfigurationEntries)
					{
						runspaceConfiguration.Cmdlets.Append(items2);
					}
				}
			}
			return runspaceConfiguration;
		}

		private static void ClearEntries()
		{
			MonadRunspaceConfiguration.cmdletConfigurationEntries.Clear();
			MonadRunspaceConfiguration.mshSnapInNames.Clear();
		}

		private static void AddPSSnapIn(RunspaceConfiguration runspaceConfiguration, string mshSnapInName)
		{
			PSSnapInException ex = null;
			PSSnapInInfo pssnapInInfo = runspaceConfiguration.AddPSSnapIn(mshSnapInName, out ex);
			if (ex != null)
			{
				throw ex;
			}
			if (pssnapInInfo != null)
			{
				ExTraceGlobals.IntegrationTracer.Information(0L, mshSnapInName + " added to Runspace:" + pssnapInInfo.ToString());
			}
		}

		private static bool IsMixedOrDefaultMode()
		{
			return MonadRunspaceConfiguration.singleShellConfiguration == MonadRunspaceConfiguration.SingleShellConfigurationMode.Default || MonadRunspaceConfiguration.singleShellConfiguration == MonadRunspaceConfiguration.SingleShellConfigurationMode.Mixed;
		}

		private static CmdletConfigurationEntry[] GetNonEdgeCmdletConfigurationEntries()
		{
			string assemblyFile = Path.Combine(ConfigurationContext.Setup.BinPath, "Microsoft.Exchange.PowerShell.Configuration.dll");
			Assembly assembly = Assembly.LoadFrom(assemblyFile);
			Type type = assembly.GetType("Microsoft.Exchange.Management.PowerShell.CmdletConfigurationEntries", true, true);
			List<CmdletConfigurationEntry> list = new List<CmdletConfigurationEntry>();
			foreach (string name in new string[]
			{
				"ExchangeCmdletConfigurationEntries",
				"ExchangeNonEdgeCmdletConfigurationEntries"
			})
			{
				PropertyInfo property = type.GetProperty(name, BindingFlags.Static | BindingFlags.Public);
				CmdletConfigurationEntry[] collection = (CmdletConfigurationEntry[])property.GetValue(null, null);
				list.AddRange(collection);
			}
			return list.ToArray();
		}

		private const string monadShellIds = "SOFTWARE\\Microsoft\\PowerShell\\1\\ShellIds\\Microsoft.PowerShell";

		private const string shellPolicy = "ExecutionPolicy";

		private const string shellPolicyValue = "RemoteSigned";

		private const string edgeRegistryKey = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\EdgeTransportRole\\AdamSettings\\MSExchange";

		private const string shellId = "Exchange";

		private static MonadRunspaceConfiguration.SingleShellConfigurationMode singleShellConfiguration;

		private static List<CmdletConfigurationEntry[]> cmdletConfigurationEntries = new List<CmdletConfigurationEntry[]>();

		private static List<string> mshSnapInNames = new List<string>();

		private static bool? isEdgeMachine;

		private RunspaceConfigurationEntryCollection<CmdletConfigurationEntry> miniShellCmdlets;

		private enum SingleShellConfigurationMode
		{
			Default,
			Mixed,
			Clear,
			Mini
		}
	}
}
