using System;
using System.Runtime.InteropServices;
using System.Security;

namespace System
{
	[StructLayout(LayoutKind.Sequential)]
	internal class RuntimeFieldInfoStub : IRuntimeFieldInfo
	{
		[SecuritySafeCritical]
		public RuntimeFieldInfoStub(IntPtr methodHandleValue, object keepalive)
		{
			this.m_keepalive = keepalive;
			this.m_fieldHandle = new RuntimeFieldHandleInternal(methodHandleValue);
		}

		RuntimeFieldHandleInternal IRuntimeFieldInfo.Value
		{
			get
			{
				return this.m_fieldHandle;
			}
		}

		private object m_keepalive;

		private object m_c;

		private object m_d;

		private int m_b;

		private object m_e;

		private object m_f;

		private RuntimeFieldHandleInternal m_fieldHandle;
	}
}
