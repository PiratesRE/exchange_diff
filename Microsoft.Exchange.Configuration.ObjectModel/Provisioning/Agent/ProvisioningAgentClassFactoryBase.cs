using System;
using System.Collections.Generic;
using System.Reflection;

namespace Microsoft.Exchange.Provisioning.Agent
{
	public abstract class ProvisioningAgentClassFactoryBase : IProvisioningAgent
	{
		public ProvisioningAgentClassFactoryBase()
		{
			this.lookupTable = new Dictionary<string, Type>(StringComparer.CurrentCultureIgnoreCase);
			this.PopulateDictionary();
		}

		private void PopulateDictionary()
		{
			Assembly assembly = Assembly.GetAssembly(base.GetType());
			Type[] types = assembly.GetTypes();
			foreach (Type type in types)
			{
				if (type.IsPublic && type.IsVisible && !type.IsAbstract && !type.IsInterface)
				{
					object[] customAttributes = type.GetCustomAttributes(typeof(CmdletHandlerAttribute), false);
					foreach (CmdletHandlerAttribute cmdletHandlerAttribute in customAttributes)
					{
						if (this.lookupTable.ContainsKey(cmdletHandlerAttribute.TaskName))
						{
							throw new ArgumentException("Task " + cmdletHandlerAttribute.TaskName + " is handled by two or more handlers defined in assembly a");
						}
						this.lookupTable.Add(cmdletHandlerAttribute.TaskName, type);
					}
				}
			}
		}

		public IEnumerable<string> GetSupportedCmdlets()
		{
			return this.lookupTable.Keys;
		}

		public ProvisioningHandler GetCmdletHandler(string cmdletName)
		{
			Type type;
			if (this.lookupTable.TryGetValue(cmdletName, out type))
			{
				return Activator.CreateInstance(type) as ProvisioningHandler;
			}
			if (this.lookupTable.TryGetValue("*", out type))
			{
				return Activator.CreateInstance(type) as ProvisioningHandler;
			}
			throw new ArgumentException(cmdletName);
		}

		private Dictionary<string, Type> lookupTable;
	}
}
