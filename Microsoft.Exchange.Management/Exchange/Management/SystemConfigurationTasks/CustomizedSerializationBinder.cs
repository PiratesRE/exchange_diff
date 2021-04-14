using System;
using System.Reflection;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	internal sealed class CustomizedSerializationBinder : SerializationBinder
	{
		public override Type BindToType(string assemblyName, string typeName)
		{
			foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				if (assemblyName.IndexOf(assembly.GetName().Name) >= 0)
				{
					return assembly.GetType(typeName);
				}
			}
			return null;
		}
	}
}
