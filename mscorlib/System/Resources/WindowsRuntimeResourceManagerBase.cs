using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Security;

namespace System.Resources
{
	[FriendAccessAllowed]
	[SecurityCritical]
	internal class WindowsRuntimeResourceManagerBase
	{
		[SecurityCritical]
		public virtual bool Initialize(string libpath, string reswFilename, out PRIExceptionInfo exceptionInfo)
		{
			exceptionInfo = null;
			return false;
		}

		[SecurityCritical]
		public virtual string GetString(string stringName, string startingCulture, string neutralResourcesCulture)
		{
			return null;
		}

		public virtual CultureInfo GlobalResourceContextBestFitCultureInfo
		{
			[SecurityCritical]
			get
			{
				return null;
			}
		}

		[SecurityCritical]
		public virtual bool SetGlobalResourceContextDefaultCulture(CultureInfo ci)
		{
			return false;
		}
	}
}
