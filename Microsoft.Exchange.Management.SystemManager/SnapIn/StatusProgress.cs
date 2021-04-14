using System;
using System.ComponentModel;
using Microsoft.Exchange.Diagnostics.Components.Management.SystemManager;
using Microsoft.ManagementConsole;
using Microsoft.ManagementGUI;

namespace Microsoft.Exchange.Management.SnapIn
{
	[Serializable]
	internal sealed class StatusProgress : IProgress
	{
		public StatusProgress(Status status, ISynchronizeInvoke owner)
		{
			this.status = status;
			this.owner = owner;
		}

		bool IProgress.Canceled
		{
			get
			{
				return false;
			}
		}

		void IProgress.ReportProgress(int workProcessed, int totalWork, string statusText)
		{
			ExTraceGlobals.ProgramFlowTracer.TraceFunction<int, int, string>((long)this.GetHashCode(), "*--StatusProgress.ReportProgress: {0}/{1}: {2}", workProcessed, totalWork, statusText);
			if (!this.hasCompleted)
			{
				this.status.ReportProgress(workProcessed, totalWork, statusText);
				if (workProcessed == totalWork)
				{
					lock (this.statusLock)
					{
						if (!this.hasCompleted)
						{
							this.hasCompleted = true;
							this.owner.BeginInvoke(new StatusProgress.ReportCompletedDelegate(this.status.Complete), new object[]
							{
								statusText,
								totalWork > 0
							});
						}
					}
				}
			}
		}

		private Status status;

		private ISynchronizeInvoke owner;

		private bool hasCompleted;

		private object statusLock = new object();

		private delegate void ReportCompletedDelegate(string statusText, bool succeeded);
	}
}
