using System;

namespace System.Runtime.CompilerServices
{
	internal struct ObjectHandleOnStack
	{
		internal ObjectHandleOnStack(IntPtr pObject)
		{
			this.m_ptr = pObject;
		}

		private IntPtr m_ptr;
	}
}
