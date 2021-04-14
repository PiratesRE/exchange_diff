using System;
using System.Reflection;
using System.Security;

namespace System.Runtime.CompilerServices
{
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true, Inherited = false)]
	[__DynamicallyInvokable]
	public sealed class TypeForwardedToAttribute : Attribute
	{
		[__DynamicallyInvokable]
		public TypeForwardedToAttribute(Type destination)
		{
			this._destination = destination;
		}

		[__DynamicallyInvokable]
		public Type Destination
		{
			[__DynamicallyInvokable]
			get
			{
				return this._destination;
			}
		}

		[SecurityCritical]
		internal static TypeForwardedToAttribute[] GetCustomAttribute(RuntimeAssembly assembly)
		{
			Type[] array = null;
			RuntimeAssembly.GetForwardedTypes(assembly.GetNativeHandle(), JitHelpers.GetObjectHandleOnStack<Type[]>(ref array));
			TypeForwardedToAttribute[] array2 = new TypeForwardedToAttribute[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array2[i] = new TypeForwardedToAttribute(array[i]);
			}
			return array2;
		}

		private Type _destination;
	}
}
