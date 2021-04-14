using System;

namespace Microsoft.Office.Datacenter.WorkerTaskFramework
{
	public interface IWorkItemFactory
	{
		string LocalPath { get; }

		T CreateWorkItem<T>(WorkDefinition definition) where T : WorkItem;
	}
}
