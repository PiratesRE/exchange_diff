using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using Microsoft.Win32.SafeHandles;

namespace System.Security.Principal
{
	[ComVisible(true)]
	public class WindowsImpersonationContext : IDisposable
	{
		[SecurityCritical]
		private WindowsImpersonationContext()
		{
		}

		[SecurityCritical]
		internal WindowsImpersonationContext(SafeAccessTokenHandle safeTokenHandle, WindowsIdentity wi, bool isImpersonating, FrameSecurityDescriptor fsd)
		{
			if (safeTokenHandle.IsInvalid)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_InvalidImpersonationToken"));
			}
			if (isImpersonating)
			{
				if (!Win32Native.DuplicateHandle(Win32Native.GetCurrentProcess(), safeTokenHandle, Win32Native.GetCurrentProcess(), ref this.m_safeTokenHandle, 0U, true, 2U))
				{
					throw new SecurityException(Win32Native.GetMessage(Marshal.GetLastWin32Error()));
				}
				this.m_wi = wi;
			}
			this.m_fsd = fsd;
		}

		[SecuritySafeCritical]
		public void Undo()
		{
			if (this.m_safeTokenHandle.IsInvalid)
			{
				int num = Win32.RevertToSelf();
				if (num < 0)
				{
					Environment.FailFast(Win32Native.GetMessage(num));
				}
			}
			else
			{
				int num = Win32.RevertToSelf();
				if (num < 0)
				{
					Environment.FailFast(Win32Native.GetMessage(num));
				}
				num = Win32.ImpersonateLoggedOnUser(this.m_safeTokenHandle);
				if (num < 0)
				{
					throw new SecurityException(Win32Native.GetMessage(num));
				}
			}
			WindowsIdentity.UpdateThreadWI(this.m_wi);
			if (this.m_fsd != null)
			{
				this.m_fsd.SetTokenHandles(null, null);
			}
		}

		[SecurityCritical]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		[HandleProcessCorruptedStateExceptions]
		internal bool UndoNoThrow()
		{
			bool result = false;
			try
			{
				int num;
				if (this.m_safeTokenHandle.IsInvalid)
				{
					num = Win32.RevertToSelf();
					if (num < 0)
					{
						Environment.FailFast(Win32Native.GetMessage(num));
					}
				}
				else
				{
					num = Win32.RevertToSelf();
					if (num >= 0)
					{
						num = Win32.ImpersonateLoggedOnUser(this.m_safeTokenHandle);
					}
					else
					{
						Environment.FailFast(Win32Native.GetMessage(num));
					}
				}
				result = (num >= 0);
				if (this.m_fsd != null)
				{
					this.m_fsd.SetTokenHandles(null, null);
				}
			}
			catch (Exception exception)
			{
				if (!AppContextSwitches.UseLegacyExecutionContextBehaviorUponUndoFailure)
				{
					Environment.FailFast(Environment.GetResourceString("ExecutionContext_UndoFailed"), exception);
				}
				result = false;
			}
			return result;
		}

		[SecuritySafeCritical]
		[ComVisible(false)]
		protected virtual void Dispose(bool disposing)
		{
			if (disposing && this.m_safeTokenHandle != null && !this.m_safeTokenHandle.IsClosed)
			{
				this.Undo();
				this.m_safeTokenHandle.Dispose();
			}
		}

		[ComVisible(false)]
		public void Dispose()
		{
			this.Dispose(true);
		}

		[SecurityCritical]
		private SafeAccessTokenHandle m_safeTokenHandle = SafeAccessTokenHandle.InvalidHandle;

		private WindowsIdentity m_wi;

		private FrameSecurityDescriptor m_fsd;
	}
}
