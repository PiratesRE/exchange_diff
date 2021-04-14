using System;
using System.Runtime.InteropServices;

namespace System
{
	[ComVisible(true)]
	public struct RuntimeArgumentHandle
	{
		internal IntPtr Value
		{
			get
			{
				return this.m_ptr;
			}
		}

		private IntPtr m_ptr;
	}
}
