using System;
using System.Runtime.ConstrainedExecution;
using System.Security;

namespace System.Threading
{
	internal class HostExecutionContextSwitcher
	{
		[SecurityCritical]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		public static void Undo(object switcherObject)
		{
			if (switcherObject == null)
			{
				return;
			}
			HostExecutionContextManager currentHostExecutionContextManager = HostExecutionContextManager.GetCurrentHostExecutionContextManager();
			if (currentHostExecutionContextManager != null)
			{
				currentHostExecutionContextManager.Revert(switcherObject);
			}
		}

		internal ExecutionContext executionContext;

		internal HostExecutionContext previousHostContext;

		internal HostExecutionContext currentHostContext;
	}
}
