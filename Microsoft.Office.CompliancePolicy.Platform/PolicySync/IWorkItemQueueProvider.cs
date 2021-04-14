using System;
using System.Collections.Generic;

namespace Microsoft.Office.CompliancePolicy.PolicySync
{
	public interface IWorkItemQueueProvider
	{
		void Enqueue(WorkItemBase item);

		IList<WorkItemBase> Dequeue(int maxCount);

		IList<WorkItemBase> GetAll();

		bool IsEmpty();

		void Update(WorkItemBase item);

		void Delete(WorkItemBase item);

		void OnWorkItemCompleted(WorkItemBase item);

		void OnAllWorkItemDispatched();
	}
}
