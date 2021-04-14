using System;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;

namespace Microsoft.Exchange.WorkloadManagement
{
	internal sealed class DummyWorkloadLogger : IWorkloadLogger
	{
		private DummyWorkloadLogger()
		{
		}

		internal static DummyWorkloadLogger Instance
		{
			get
			{
				if (DummyWorkloadLogger.instance == null)
				{
					DummyWorkloadLogger.instance = new DummyWorkloadLogger();
				}
				return DummyWorkloadLogger.instance;
			}
		}

		public void LogActivityEvent(IActivityScope activityScope, ActivityEventType eventType)
		{
		}

		private static DummyWorkloadLogger instance;
	}
}
