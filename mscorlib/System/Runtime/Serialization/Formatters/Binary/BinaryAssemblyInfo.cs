using System;
using System.Reflection;

namespace System.Runtime.Serialization.Formatters.Binary
{
	internal sealed class BinaryAssemblyInfo
	{
		internal BinaryAssemblyInfo(string assemblyString)
		{
			this.assemblyString = assemblyString;
		}

		internal BinaryAssemblyInfo(string assemblyString, Assembly assembly)
		{
			this.assemblyString = assemblyString;
			this.assembly = assembly;
		}

		internal Assembly GetAssembly()
		{
			if (this.assembly == null)
			{
				this.assembly = FormatterServices.LoadAssemblyFromStringNoThrow(this.assemblyString);
				if (this.assembly == null)
				{
					throw new SerializationException(Environment.GetResourceString("Serialization_AssemblyNotFound", new object[]
					{
						this.assemblyString
					}));
				}
			}
			return this.assembly;
		}

		internal string assemblyString;

		private Assembly assembly;
	}
}
