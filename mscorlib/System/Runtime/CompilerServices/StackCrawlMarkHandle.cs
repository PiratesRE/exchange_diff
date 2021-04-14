using System;

namespace System.Runtime.CompilerServices
{
	internal struct StackCrawlMarkHandle
	{
		internal StackCrawlMarkHandle(IntPtr stackMark)
		{
			this.m_ptr = stackMark;
		}

		private IntPtr m_ptr;
	}
}
