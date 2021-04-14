using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;

namespace Microsoft.Exchange.Services.Diagnostics
{
	internal static class OwsLogRegistry
	{
		public static void Register(string action, Type metadataEnumType, params Type[] additionalTypes)
		{
			if (OwsLogRegistry.IsRegistered(action))
			{
				return;
			}
			lock (OwsLogRegistry.RegisterLock)
			{
				if (!OwsLogRegistry.IsRegistered(action))
				{
					OwsLogRegistry.RegisterType(action, metadataEnumType);
					foreach (Type type in additionalTypes)
					{
						OwsLogRegistry.RegisterType(action, type);
					}
				}
			}
		}

		public static IEnumerable<Enum> GetRegisteredValues(string action)
		{
			HashSet<Enum> result;
			if (OwsLogRegistry.actionPropertyMappings.TryGetValue(action, out result))
			{
				return result;
			}
			return OwsLogRegistry.EmptySet;
		}

		private static bool IsRegistered(string action)
		{
			return OwsLogRegistry.actionPropertyMappings.ContainsKey(action);
		}

		private static void RegisterType(string action, Type type)
		{
			if (!type.GetTypeInfo().IsEnum)
			{
				throw new ArgumentException("Only enum types are allowed: " + type, "type");
			}
			HashSet<Enum> hashSet;
			if (!OwsLogRegistry.actionPropertyMappings.TryGetValue(action, out hashSet))
			{
				hashSet = new HashSet<Enum>();
				OwsLogRegistry.actionPropertyMappings[action] = hashSet;
			}
			foreach (object obj in Enum.GetValues(type))
			{
				Enum item = (Enum)obj;
				hashSet.Add(item);
			}
			ActivityContext.RegisterMetadata(type);
		}

		private static readonly object RegisterLock = new object();

		private static readonly IDictionary<string, HashSet<Enum>> actionPropertyMappings = new Dictionary<string, HashSet<Enum>>(64);

		private static readonly HashSet<Enum> EmptySet = new HashSet<Enum>();
	}
}
