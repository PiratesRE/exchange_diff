using System;

namespace System.Threading
{
	internal struct ThreadHandle
	{
		internal ThreadHandle(IntPtr pThread)
		{
			this.m_ptr = pThread;
		}

		private IntPtr m_ptr;
	}
}
