using System;

namespace System
{
	internal struct AppDomainHandle
	{
		internal AppDomainHandle(IntPtr domainHandle)
		{
			this.m_appDomainHandle = domainHandle;
		}

		private IntPtr m_appDomainHandle;
	}
}
