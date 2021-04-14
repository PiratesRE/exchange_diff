using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Protocol;

namespace Microsoft.Exchange.Compliance.TaskDistributionCommon.Extensibility
{
	internal class Registry
	{
		private Registry()
		{
			this.registryTable = new object[this.GetEnumTableSize(RegistryComponent.Application)][];
		}

		public static Registry Instance
		{
			get
			{
				return Registry.instance;
			}
		}

		public void RegisterInstance<T>(RegistryComponent component, Enum subComponent, T instance) where T : class
		{
			this.AddTableEntry(component, subComponent, instance);
		}

		public void RegisterFactory<T, P>(RegistryComponent component, Enum subComponent, Func<P, T> factory) where T : class where P : class
		{
			this.AddTableEntry(component, subComponent, factory);
		}

		public bool TryGetInstance<T>(RegistryComponent component, Enum subComponent, out T instance, out FaultDefinition faultDefinition, [CallerMemberName] string callerMember = null, [CallerFilePath] string callerFilePath = null, [CallerLineNumber] int callerLineNumber = 0) where T : class
		{
			return this.TryGetInstance<T, object>(component, subComponent, out instance, null, out faultDefinition, callerMember, callerFilePath, callerLineNumber);
		}

		public bool TryGetInstance<T, P>(RegistryComponent component, Enum subComponent, out T instance, P factoryParam, out FaultDefinition faultDefinition, [CallerMemberName] string callerMember = null, [CallerFilePath] string callerFilePath = null, [CallerLineNumber] int callerLineNumber = 0) where T : class where P : class
		{
			faultDefinition = null;
			int num = Convert.ToInt32(subComponent);
			if (this.registryTable[(int)component] != null && this.registryTable[(int)component].Length > num)
			{
				object obj = this.registryTable[(int)component][num];
				if (obj != null)
				{
					if (!(obj is Delegate))
					{
						instance = (obj as T);
						return instance != null;
					}
					Func<P, T> func = obj as Func<P, T>;
					if (func != null)
					{
						instance = func(factoryParam);
						return instance != null;
					}
				}
			}
			faultDefinition = FaultDefinition.FromErrorString(string.Format("REGISTRY: Could not find a type {0}, for component {1} and sub component {2}", typeof(T), component, subComponent), "TryGetInstance", "f:\\15.00.1497\\sources\\dev\\EDiscovery\\src\\TaskDistributionSystem\\TaskDistributionCommon\\Extensibility\\Registry.cs", 157);
			instance = default(T);
			return false;
		}

		private void AddTableEntry(RegistryComponent component, Enum subComponent, object entry)
		{
			int num = Convert.ToInt32(subComponent);
			if (this.registryTable[(int)component] == null)
			{
				this.registryTable[(int)component] = new object[this.GetEnumTableSize(subComponent)];
			}
			this.registryTable[(int)component][num] = entry;
		}

		private int GetEnumTableSize(Enum enumeration)
		{
			return (from object t in Enum.GetValues(enumeration.GetType())
			select Convert.ToInt32(t)).Max() + 1;
		}

		private static Registry instance = new Registry();

		private object[][] registryTable;
	}
}
