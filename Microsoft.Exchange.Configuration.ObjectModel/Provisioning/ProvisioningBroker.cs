using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Security;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Provisioning.Agent;

namespace Microsoft.Exchange.Provisioning
{
	public class ProvisioningBroker
	{
		internal Exception InitializationException
		{
			get
			{
				return this.initializationException;
			}
		}

		internal ProvisioningBroker()
		{
			this.lookupTable = new Dictionary<string, List<ProvisioningBroker.ClassFactoryBucket>>(StringComparer.CurrentCultureIgnoreCase);
			this.agentNameLookupTable = new Dictionary<IProvisioningAgent, string>();
			CmdletExtensionAgent[] enabledAgents = null;
			try
			{
				ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 53, ".ctor", "f:\\15.00.1497\\sources\\dev\\Configuration\\src\\ObjectModel\\Provisioning\\ProvisioningBroker.cs");
				enabledAgents = topologyConfigurationSession.FindCmdletExtensionAgents(true, true);
			}
			catch (Exception ex)
			{
				if (DataAccessHelper.IsDataAccessKnownException(ex))
				{
					throw new ProvisioningBrokerException(Strings.ProvisioningBrokerInitializationFailed(ex.Message), ex);
				}
				throw;
			}
			this.BuildHandlerLookupTable(enabledAgents, out this.initializationException);
		}

		public ProvisioningHandler[] GetProvisioningHandlers(Task task)
		{
			string commandName = task.CurrentTaskContext.InvocationInfo.CommandName;
			if (commandName.StartsWith("Get-"))
			{
				return new ProvisioningHandler[0];
			}
			List<ProvisioningHandler> list = new List<ProvisioningHandler>();
			List<ProvisioningBroker.ClassFactoryBucket> list2 = new List<ProvisioningBroker.ClassFactoryBucket>();
			List<ProvisioningBroker.ClassFactoryBucket> collection;
			if (this.lookupTable.TryGetValue(commandName, out collection))
			{
				list2.AddRange(collection);
			}
			if (this.lookupTable.TryGetValue("*", out collection))
			{
				bool flag = list2.Count > 0;
				list2.AddRange(collection);
				if (flag)
				{
					list2.Sort();
				}
			}
			for (int i = 0; i < list2.Count; i++)
			{
				if (!TaskLogger.IsSetupLogging)
				{
					task.WriteVerbose(Strings.InstantiatingHandlerForAgent(i, this.agentNameLookupTable[list2[i].ClassFactory]));
				}
				ProvisioningHandler cmdletHandler = list2[i].ClassFactory.GetCmdletHandler(commandName);
				if (cmdletHandler != null)
				{
					cmdletHandler.TaskName = commandName;
					cmdletHandler.AgentName = list2[i].AgentName;
					list.Add(cmdletHandler);
				}
			}
			return list.ToArray();
		}

		private void BuildHandlerLookupTable(CmdletExtensionAgent[] enabledAgents, out Exception ex)
		{
			ex = null;
			CmdletExtensionAgent cmdletExtensionAgent = null;
			try
			{
				for (int i = 0; i < enabledAgents.Length; i++)
				{
					if (cmdletExtensionAgent != null && cmdletExtensionAgent.Priority == enabledAgents[i].Priority)
					{
						throw new ArgumentException(Strings.ClashingPriorities(cmdletExtensionAgent.Priority.ToString(), enabledAgents[i].Name, cmdletExtensionAgent.Name));
					}
					cmdletExtensionAgent = enabledAgents[i];
					IProvisioningAgent classFactoryInstance = ProvisioningBroker.GetClassFactoryInstance(enabledAgents[i].Assembly, enabledAgents[i].ClassFactory, out ex);
					if (ex != null)
					{
						break;
					}
					this.agentNameLookupTable.Add(classFactoryInstance, enabledAgents[i].Name);
					IEnumerable<string> supportedCmdlets = classFactoryInstance.GetSupportedCmdlets();
					foreach (string key in supportedCmdlets)
					{
						List<ProvisioningBroker.ClassFactoryBucket> list;
						if (!this.lookupTable.TryGetValue(key, out list))
						{
							list = new List<ProvisioningBroker.ClassFactoryBucket>();
							this.lookupTable.Add(key, list);
						}
						list.Add(new ProvisioningBroker.ClassFactoryBucket(classFactoryInstance, enabledAgents[i].Name, enabledAgents[i].Priority));
					}
				}
			}
			catch (ConfigurationErrorsException ex2)
			{
				ex = ex2;
			}
			catch (ProvisioningException ex3)
			{
				ex = ex3;
			}
			catch (FileNotFoundException ex4)
			{
				ex = ex4;
			}
			catch (FileLoadException ex5)
			{
				ex = ex5;
			}
			catch (BadImageFormatException ex6)
			{
				ex = ex6;
			}
			catch (SecurityException ex7)
			{
				ex = ex7;
			}
			catch (UnauthorizedAccessException ex8)
			{
				ex = ex8;
			}
			catch (ArgumentException ex9)
			{
				ex = ex9;
			}
			catch (IOException ex10)
			{
				ex = ex10;
			}
			catch (MissingMethodException ex11)
			{
				ex = ex11;
			}
			catch (AmbiguousMatchException ex12)
			{
				ex = ex12;
			}
			catch (ReflectionTypeLoadException ex13)
			{
				ex = ex13;
			}
		}

		internal static IProvisioningAgent GetClassFactoryInstance(string assemblyName, string classFactoryName, out Exception ex)
		{
			string assemblyFile = Path.Combine(CmdletExtensionAgentsGlobalConfig.CmdletExtensionAgentsFolder, assemblyName);
			IProvisioningAgent provisioningAgent = null;
			ex = null;
			try
			{
				Assembly assembly = Assembly.LoadFrom(assemblyFile);
				Type type = assembly.GetType(classFactoryName);
				if (type == null)
				{
					ex = new ArgumentException(Strings.CouldntFindClassFactoryInAssembly(classFactoryName, assembly.FullName));
					return provisioningAgent;
				}
				object obj = assembly.CreateInstance(type.FullName);
				provisioningAgent = (obj as IProvisioningAgent);
				if (provisioningAgent == null)
				{
					ex = new ArgumentException(Strings.ClassFactoryDoesNotImplementIProvisioningAgent(classFactoryName, assembly.FullName));
					return provisioningAgent;
				}
			}
			catch (TargetInvocationException ex2)
			{
				ex = ex2.InnerException;
			}
			catch (ReflectionTypeLoadException ex3)
			{
				ex = ex3;
			}
			catch (MissingMethodException ex4)
			{
				ex = ex4;
			}
			catch (FileNotFoundException ex5)
			{
				ex = ex5;
			}
			catch (FileLoadException ex6)
			{
				ex = ex6;
			}
			catch (BadImageFormatException ex7)
			{
				ex = ex7;
			}
			catch (ArgumentException ex8)
			{
				ex = ex8;
			}
			return provisioningAgent;
		}

		private Dictionary<string, List<ProvisioningBroker.ClassFactoryBucket>> lookupTable;

		private Dictionary<IProvisioningAgent, string> agentNameLookupTable;

		private Exception initializationException;

		private class ClassFactoryBucket : IComparable<ProvisioningBroker.ClassFactoryBucket>
		{
			public ClassFactoryBucket(IProvisioningAgent classFactory, string agentName, byte priority)
			{
				this.ClassFactory = classFactory;
				this.Priority = priority;
				this.AgentName = agentName;
			}

			int IComparable<ProvisioningBroker.ClassFactoryBucket>.CompareTo(ProvisioningBroker.ClassFactoryBucket other)
			{
				if (other != null)
				{
					return (int)(this.Priority - other.Priority);
				}
				return -1;
			}

			public readonly IProvisioningAgent ClassFactory;

			public readonly string AgentName;

			public readonly byte Priority;
		}
	}
}
