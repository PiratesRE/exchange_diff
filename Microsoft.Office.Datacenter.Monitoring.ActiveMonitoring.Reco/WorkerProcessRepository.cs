using System;

namespace Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery
{
	internal class WorkerProcessRepository
	{
		private WorkerProcessRepository()
		{
		}

		public static WorkerProcessRepository Instance
		{
			get
			{
				if (WorkerProcessRepository.instance == null)
				{
					lock (WorkerProcessRepository.initLock)
					{
						if (WorkerProcessRepository.instance == null)
						{
							WorkerProcessRepository.instance = new WorkerProcessRepository();
						}
					}
				}
				return WorkerProcessRepository.instance;
			}
		}

		internal RpcSetWorkerProcessInfoImpl.WorkerProcessInfo WorkerProcessInfo { get; set; }

		internal DateTime GetWorkerProcessStartTime()
		{
			DateTime result = DateTime.MinValue;
			if (this.WorkerProcessInfo != null)
			{
				result = this.WorkerProcessInfo.ProcessStartTime;
			}
			return result;
		}

		private static readonly object initLock = new object();

		private static WorkerProcessRepository instance = null;
	}
}
