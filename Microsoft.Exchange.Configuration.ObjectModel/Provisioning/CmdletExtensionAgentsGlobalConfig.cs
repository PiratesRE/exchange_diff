using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Reflection;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Provisioning
{
	internal class CmdletExtensionAgentsGlobalConfig
	{
		public CmdletExtensionAgentsGlobalConfig(ITopologyConfigurationSession session)
		{
			CmdletExtensionAgent[] array = session.FindCmdletExtensionAgents(false, false);
			this.prioritiesInUse = new CmdletExtensionAgent[256];
			this.agentIdentities = new List<string>(array.Length);
			this.configurationIssues = new List<LocalizedString>();
			foreach (CmdletExtensionAgent cmdletExtensionAgent in array)
			{
				if (this.prioritiesInUse[(int)cmdletExtensionAgent.Priority] != null)
				{
					this.configurationIssues.Add(Strings.ClashingPriorities(cmdletExtensionAgent.Priority.ToString(), cmdletExtensionAgent.Name, this.prioritiesInUse[(int)cmdletExtensionAgent.Priority].Name));
				}
				else
				{
					this.prioritiesInUse[(int)cmdletExtensionAgent.Priority] = cmdletExtensionAgent;
				}
				string factoryIdentity = CmdletExtensionAgentsGlobalConfig.GetFactoryIdentity(cmdletExtensionAgent.Assembly, cmdletExtensionAgent.ClassFactory);
				if (this.agentIdentities.Contains(factoryIdentity))
				{
					this.configurationIssues.Add(Strings.ClashingIdentities(cmdletExtensionAgent.Assembly, cmdletExtensionAgent.ClassFactory));
				}
				else
				{
					this.agentIdentities.Add(factoryIdentity);
				}
				if (this.nextAvailablePriority < (int)(cmdletExtensionAgent.Priority + 1))
				{
					this.nextAvailablePriority = (int)(cmdletExtensionAgent.Priority + 1);
				}
			}
		}

		public static string CmdletExtensionAgentsFolder
		{
			get
			{
				if (CmdletExtensionAgentsGlobalConfig.IsNotRunningDfpowa)
				{
					return Path.Combine(ConfigurationContext.Setup.BinPath, "CmdletExtensionAgents");
				}
				string codeBase = Assembly.GetExecutingAssembly().CodeBase;
				UriBuilder uriBuilder = new UriBuilder(codeBase);
				string path = Uri.UnescapeDataString(uriBuilder.Path);
				return Path.GetDirectoryName(path);
			}
		}

		public bool IsNextAvailablePriorityValid
		{
			get
			{
				return this.nextAvailablePriority < 256;
			}
		}

		public byte NextAvailablePriority
		{
			get
			{
				return (byte)this.nextAvailablePriority;
			}
		}

		internal CmdletExtensionAgent[] PrioritiesInUse
		{
			get
			{
				return this.prioritiesInUse;
			}
		}

		internal List<CmdletExtensionAgent> ObjectsToSave
		{
			get
			{
				return this.objectsToSave;
			}
		}

		internal LocalizedString[] ConfigurationIssues
		{
			get
			{
				return this.configurationIssues.ToArray();
			}
		}

		public bool IsPriorityAvailable(byte priority, CmdletExtensionAgent thisAgent)
		{
			return this.prioritiesInUse[(int)priority] == null || (thisAgent != null && this.prioritiesInUse[(int)priority].Guid == thisAgent.Guid);
		}

		public bool IsFactoryIdentityInUse(string assembly, string classFactory)
		{
			return this.agentIdentities.Contains(CmdletExtensionAgentsGlobalConfig.GetFactoryIdentity(assembly, classFactory));
		}

		public bool FreeUpPriorityValue(byte priority)
		{
			this.objectsToSave = new List<CmdletExtensionAgent>();
			for (byte b = priority; b <= 255; b += 1)
			{
				CmdletExtensionAgent cmdletExtensionAgent = this.PrioritiesInUse[(int)b];
				if (cmdletExtensionAgent == null)
				{
					return true;
				}
				if (b == 255)
				{
					return false;
				}
				CmdletExtensionAgent cmdletExtensionAgent2 = cmdletExtensionAgent;
				cmdletExtensionAgent2.Priority += 1;
				this.objectsToSave.Add(cmdletExtensionAgent);
			}
			return true;
		}

		internal static string GetFactoryIdentity(string assembly, string classFactory)
		{
			return assembly.ToLower() + "::" + classFactory;
		}

		private const string exchangeSetupLocation = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Setup";

		private const string exchangeInstallPathValue = "MsiInstallPath";

		private const string CmdletExtensionAgentsSubFolder = "CmdletExtensionAgents";

		private static readonly bool IsNotRunningDfpowa = string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["IsPreCheckinApp"]) || StringComparer.OrdinalIgnoreCase.Equals("false", ConfigurationManager.AppSettings["IsPreCheckinApp"]);

		private int nextAvailablePriority;

		private CmdletExtensionAgent[] prioritiesInUse;

		private List<string> agentIdentities;

		private List<CmdletExtensionAgent> objectsToSave;

		private List<LocalizedString> configurationIssues;
	}
}
