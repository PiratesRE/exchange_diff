using System;

namespace System.Runtime.CompilerServices
{
	internal struct StringHandleOnStack
	{
		internal StringHandleOnStack(IntPtr pString)
		{
			this.m_ptr = pString;
		}

		private IntPtr m_ptr;
	}
}
