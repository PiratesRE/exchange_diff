using System;
using System.Security;

namespace System
{
	internal struct RuntimeFieldHandleInternal
	{
		internal static RuntimeFieldHandleInternal EmptyHandle
		{
			get
			{
				return default(RuntimeFieldHandleInternal);
			}
		}

		internal bool IsNullHandle()
		{
			return this.m_handle.IsNull();
		}

		internal IntPtr Value
		{
			[SecurityCritical]
			get
			{
				return this.m_handle;
			}
		}

		[SecurityCritical]
		internal RuntimeFieldHandleInternal(IntPtr value)
		{
			this.m_handle = value;
		}

		internal IntPtr m_handle;
	}
}
