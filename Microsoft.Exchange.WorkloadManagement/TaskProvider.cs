using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.WorkloadManagement
{
	internal class TaskProvider : DisposeTrackableBase, ITaskProvider, IDisposable
	{
		public TaskProvider(ClassificationBlock classificationBlock)
		{
			this.classificationBlock = classificationBlock;
		}

		public SystemTaskBase GetNextTask()
		{
			SystemWorkloadBase nextWorkload = this.classificationBlock.GetNextWorkload();
			if (nextWorkload != null)
			{
				return nextWorkload.InternalGetTask();
			}
			return null;
		}

		protected override void InternalDispose(bool disposing)
		{
			this.classificationBlock.Deactivate();
			this.classificationBlock = null;
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<TaskProvider>(this);
		}

		private ClassificationBlock classificationBlock;
	}
}
