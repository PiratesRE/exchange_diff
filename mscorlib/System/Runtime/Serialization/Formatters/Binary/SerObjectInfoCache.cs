using System;
using System.Reflection;

namespace System.Runtime.Serialization.Formatters.Binary
{
	internal sealed class SerObjectInfoCache
	{
		internal SerObjectInfoCache(string typeName, string assemblyName, bool hasTypeForwardedFrom)
		{
			this.fullTypeName = typeName;
			this.assemblyString = assemblyName;
			this.hasTypeForwardedFrom = hasTypeForwardedFrom;
		}

		internal SerObjectInfoCache(Type type)
		{
			TypeInformation typeInformation = BinaryFormatter.GetTypeInformation(type);
			this.fullTypeName = typeInformation.FullTypeName;
			this.assemblyString = typeInformation.AssemblyString;
			this.hasTypeForwardedFrom = typeInformation.HasTypeForwardedFrom;
		}

		internal string fullTypeName;

		internal string assemblyString;

		internal bool hasTypeForwardedFrom;

		internal MemberInfo[] memberInfos;

		internal string[] memberNames;

		internal Type[] memberTypes;
	}
}
