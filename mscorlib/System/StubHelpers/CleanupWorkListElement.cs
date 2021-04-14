using System;
using System.Runtime.InteropServices;
using System.Security;

namespace System.StubHelpers
{
	[SecurityCritical]
	internal sealed class CleanupWorkListElement
	{
		public CleanupWorkListElement(SafeHandle handle)
		{
			this.m_handle = handle;
		}

		public SafeHandle m_handle;

		public bool m_owned;
	}
}
