using System;
using System.Threading;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal sealed class CrashProcess
	{
		private CrashProcess()
		{
			this.signalEvent = new ManualResetEvent(false);
			this.waitEvent = new ManualResetEvent(false);
			Thread thread = new Thread(new ThreadStart(this.WorkerFunction));
			thread.Start();
		}

		internal static CrashProcess Instance
		{
			get
			{
				return CrashProcess.instance;
			}
		}

		internal void CrashThisProcess(Exception ex)
		{
			this.message = ex.ToString();
			this.signalEvent.Set();
			this.waitEvent.WaitOne();
		}

		private void WorkerFunction()
		{
			this.signalEvent.WaitOne();
			throw new InvalidOperationException(this.message);
		}

		private static CrashProcess instance = new CrashProcess();

		private ManualResetEvent signalEvent;

		private ManualResetEvent waitEvent;

		private string message;
	}
}
