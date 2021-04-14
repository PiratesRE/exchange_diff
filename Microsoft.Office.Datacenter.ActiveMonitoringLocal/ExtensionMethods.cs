using System;
using System.Collections.Generic;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Office.Datacenter.ActiveMonitoring
{
	public static class ExtensionMethods
	{
		public static T GetValueOrDefault<K, T>(this IDictionary<K, T> dictionary, K key, T defaultValue)
		{
			T result;
			if (!dictionary.TryGetValue(key, out result))
			{
				result = defaultValue;
			}
			return result;
		}

		public static int ReadAttribute(this WorkItem workItem, string name, int defaultValue)
		{
			int result;
			if (workItem.Definition.Attributes.ContainsKey(name) && int.TryParse(workItem.Definition.Attributes[name], out result))
			{
				return result;
			}
			return defaultValue;
		}

		public static double ReadAttribute(this WorkItem workItem, string name, double defaultValue)
		{
			double result;
			if (workItem.Definition.Attributes.ContainsKey(name) && double.TryParse(workItem.Definition.Attributes[name], out result))
			{
				return result;
			}
			return defaultValue;
		}

		public static string ReadAttribute(this WorkItem workItem, string name, string defaultValue)
		{
			if (workItem.Definition.Attributes.ContainsKey(name) && !string.IsNullOrEmpty(workItem.Definition.Attributes[name]))
			{
				return workItem.Definition.Attributes[name];
			}
			return defaultValue;
		}

		public static TimeSpan ReadAttribute(this WorkItem workItem, string name, TimeSpan defaultValue)
		{
			TimeSpan result;
			if (workItem.Definition.Attributes.ContainsKey(name) && TimeSpan.TryParse(workItem.Definition.Attributes[name], out result))
			{
				return result;
			}
			return defaultValue;
		}

		public static bool ReadAttribute(this WorkItem workItem, string name, bool defaultValue)
		{
			bool result;
			if (workItem.Definition.Attributes.ContainsKey(name) && bool.TryParse(workItem.Definition.Attributes[name], out result))
			{
				return result;
			}
			return defaultValue;
		}

		public static void CopyAttributes(this WorkItem workItem, IList<string> names, WorkDefinition destination)
		{
			foreach (string key in names)
			{
				if (workItem.Definition.Attributes.ContainsKey(key))
				{
					destination.Attributes[key] = workItem.Definition.Attributes[key];
				}
			}
		}

		public static TDefinition ApplyModifier<TDefinition>(this TDefinition definition, Func<TDefinition, TDefinition> modifier) where TDefinition : WorkDefinition
		{
			return modifier(definition);
		}

		public static TDefinition ApplyModifier<TDefinition, TArg>(this TDefinition definition, Func<TDefinition, TArg, TDefinition> modifier, TArg arg) where TDefinition : WorkDefinition
		{
			return modifier(definition, arg);
		}
	}
}
