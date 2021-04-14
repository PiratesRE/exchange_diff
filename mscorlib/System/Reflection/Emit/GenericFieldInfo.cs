using System;

namespace System.Reflection.Emit
{
	internal sealed class GenericFieldInfo
	{
		internal GenericFieldInfo(RuntimeFieldHandle fieldHandle, RuntimeTypeHandle context)
		{
			this.m_fieldHandle = fieldHandle;
			this.m_context = context;
		}

		internal RuntimeFieldHandle m_fieldHandle;

		internal RuntimeTypeHandle m_context;
	}
}
