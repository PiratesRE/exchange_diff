using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.ExceptionServices;
using System.Security;

namespace System.Threading
{
	internal struct CompressedStackSwitcher : IDisposable
	{
		public override bool Equals(object obj)
		{
			if (obj == null || !(obj is CompressedStackSwitcher))
			{
				return false;
			}
			CompressedStackSwitcher compressedStackSwitcher = (CompressedStackSwitcher)obj;
			return this.curr_CS == compressedStackSwitcher.curr_CS && this.prev_CS == compressedStackSwitcher.prev_CS && this.prev_ADStack == compressedStackSwitcher.prev_ADStack;
		}

		public override int GetHashCode()
		{
			return this.ToString().GetHashCode();
		}

		public static bool operator ==(CompressedStackSwitcher c1, CompressedStackSwitcher c2)
		{
			return c1.Equals(c2);
		}

		public static bool operator !=(CompressedStackSwitcher c1, CompressedStackSwitcher c2)
		{
			return !c1.Equals(c2);
		}

		[SecuritySafeCritical]
		public void Dispose()
		{
			this.Undo();
		}

		[SecurityCritical]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		[HandleProcessCorruptedStateExceptions]
		internal bool UndoNoThrow()
		{
			try
			{
				this.Undo();
			}
			catch (Exception exception)
			{
				if (!AppContextSwitches.UseLegacyExecutionContextBehaviorUponUndoFailure)
				{
					Environment.FailFast(Environment.GetResourceString("ExecutionContext_UndoFailed"), exception);
				}
				return false;
			}
			return true;
		}

		[SecurityCritical]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		public void Undo()
		{
			if (this.curr_CS == null && this.prev_CS == null)
			{
				return;
			}
			if (this.prev_ADStack != (IntPtr)0)
			{
				CompressedStack.RestoreAppDomainStack(this.prev_ADStack);
			}
			CompressedStack.SetCompressedStackThread(this.prev_CS);
			this.prev_CS = null;
			this.curr_CS = null;
			this.prev_ADStack = (IntPtr)0;
		}

		internal CompressedStack curr_CS;

		internal CompressedStack prev_CS;

		internal IntPtr prev_ADStack;
	}
}
