using System;
using System.Globalization;
using System.Threading;
using Microsoft.Office.Datacenter.WorkerTaskFramework;
using Microsoft.Win32;

namespace Microsoft.Office.Datacenter.ActiveMonitoring
{
	internal static class DefinitionIdGenerator<TDefinition> where TDefinition : WorkDefinition, new()
	{
		public static void Initialize()
		{
			using (RegistryKey registryKey = Registry.LocalMachine.CreateSubKey(DefinitionIdGenerator<TDefinition>.RegistryPath))
			{
				string[] valueNames = registryKey.GetValueNames();
				foreach (string name in valueNames)
				{
					object value = registryKey.GetValue(name);
					if (value is int && (int)value > DefinitionIdGenerator<TDefinition>.maxId)
					{
						DefinitionIdGenerator<TDefinition>.maxId = (int)value;
					}
				}
			}
		}

		public static void AssignId(TDefinition definition)
		{
			using (RegistryKey registryKey = Registry.LocalMachine.CreateSubKey(DefinitionIdGenerator<TDefinition>.RegistryPath))
			{
				string name = DefinitionIdGenerator<TDefinition>.ConstructValueName(definition);
				object value = registryKey.GetValue(name);
				if (value != null && value is int)
				{
					definition.Id = (int)value;
				}
				else
				{
					definition.Id = (Interlocked.Increment(ref DefinitionIdGenerator<TDefinition>.maxId) & int.MaxValue);
					registryKey.SetValue(name, definition.Id);
				}
			}
		}

		public static int GetIdForNotification(string resultName)
		{
			return resultName.GetHashCode() | int.MinValue;
		}

		private static string ConstructValueName(TDefinition definition)
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}~{1}~{2}~{3}~{4}~{5}", new object[]
			{
				definition.ServiceName,
				definition.Name,
				definition.TargetResource,
				definition.TargetPartition,
				definition.TargetGroup,
				definition.TargetExtension
			});
		}

		private static readonly string RegistryPath = string.Format("SOFTWARE\\Microsoft\\ExchangeServer\\{0}\\WorkerTaskFramework\\IdStore\\{1}", "v15", typeof(TDefinition).Name);

		private static int maxId = 0;
	}
}
