using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace System
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Delegate, Inherited = false)]
	[ComVisible(true)]
	public sealed class SerializableAttribute : Attribute
	{
		internal static Attribute GetCustomAttribute(RuntimeType type)
		{
			if ((type.Attributes & TypeAttributes.Serializable) != TypeAttributes.Serializable)
			{
				return null;
			}
			return new SerializableAttribute();
		}

		internal static bool IsDefined(RuntimeType type)
		{
			return type.IsSerializable;
		}
	}
}
