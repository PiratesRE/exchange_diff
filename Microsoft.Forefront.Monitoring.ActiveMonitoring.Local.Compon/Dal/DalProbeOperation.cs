using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.Dal
{
	public abstract class DalProbeOperation
	{
		[XmlAttribute]
		public string Return { get; set; }

		private static Assembly[] SafeAssemblies
		{
			get
			{
				if (DalProbeOperation.safeAssemblies == null)
				{
					Assembly[] array = new Assembly[DalProbeOperation.safeAssemblyNames.Length];
					int num = 0;
					foreach (string assemblyString in DalProbeOperation.safeAssemblyNames)
					{
						array[num++] = Assembly.Load(assemblyString);
					}
					DalProbeOperation.safeAssemblies = array;
				}
				return DalProbeOperation.safeAssemblies;
			}
		}

		public static Type ResolveDataType(string typeName)
		{
			Type type2 = DalProbeOperation.safeTypes.FirstOrDefault((Type safeType) => safeType.FullName == typeName);
			if (type2 != null)
			{
				return type2;
			}
			type2 = (from assembly in DalProbeOperation.SafeAssemblies
			select assembly.GetType(typeName)).FirstOrDefault((Type type) => type != null);
			if (type2 != null)
			{
				return type2;
			}
			throw new TypeLoadException(string.Format("Unable to load type {0}. It must be a public or internal type defined in {1}. Add the assembly containing the type to safeAssemblyNames in DalProbeOperation class", typeName, string.Join(", ", DalProbeOperation.safeAssemblyNames)));
		}

		public static bool IsVariable(string name)
		{
			return name.StartsWith("$");
		}

		public static object GetValue(string str, IDictionary<string, object> variables)
		{
			if (!DalProbeOperation.IsVariable(str))
			{
				return str;
			}
			string[] array = str.Split(new char[]
			{
				'.'
			});
			object propertyValue;
			if (!variables.TryGetValue(array[0], out propertyValue))
			{
				return null;
			}
			propertyValue = DalProbeOperation.GetPropertyValue(propertyValue, array, 1);
			return propertyValue;
		}

		public static object GetPropertyValue(object value, string[] parts, int index = 0)
		{
			for (int i = index; i < parts.Length; i++)
			{
				if (value == null)
				{
					throw new NullReferenceException(string.Format("{0} is null and hence could not evaluate {1}", string.Join(".", parts, 0, i), string.Join(".", parts, i, parts.Length - i)));
				}
				value = value.GetType().InvokeMember(parts[i], BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.GetProperty, null, value, null);
			}
			return value;
		}

		public abstract void Execute(IDictionary<string, object> variables);

		private static readonly string[] safeAssemblyNames = new string[]
		{
			"Microsoft.Exchange.Hygiene.Data",
			"Microsoft.Exchange.Data.Directory",
			"Microsoft.Exchange.Data"
		};

		private static readonly Type[] safeTypes = new Type[]
		{
			typeof(byte),
			typeof(sbyte),
			typeof(short),
			typeof(ushort),
			typeof(int),
			typeof(uint),
			typeof(long),
			typeof(ulong),
			typeof(bool),
			typeof(float),
			typeof(double),
			typeof(decimal),
			typeof(DateTime),
			typeof(TimeSpan),
			typeof(Guid)
		};

		private static Assembly[] safeAssemblies;
	}
}
