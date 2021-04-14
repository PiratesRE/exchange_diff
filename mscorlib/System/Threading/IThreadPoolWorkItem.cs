using System;
using System.Security;

namespace System.Threading
{
	internal interface IThreadPoolWorkItem
	{
		[SecurityCritical]
		void ExecuteWorkItem();

		[SecurityCritical]
		void MarkAborted(ThreadAbortException tae);
	}
}
