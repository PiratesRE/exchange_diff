using System;

namespace System.Reflection.Emit
{
	internal sealed class GenericMethodInfo
	{
		internal GenericMethodInfo(RuntimeMethodHandle methodHandle, RuntimeTypeHandle context)
		{
			this.m_methodHandle = methodHandle;
			this.m_context = context;
		}

		internal RuntimeMethodHandle m_methodHandle;

		internal RuntimeTypeHandle m_context;
	}
}
