using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Management.Hybrid
{
	internal abstract class WorkflowBase : IWorkflow
	{
		public WorkflowBase()
		{
			this.tasks = new List<ITask>();
			this.Initialize();
		}

		public IEnumerable<ITask> Tasks
		{
			get
			{
				return this.tasks;
			}
		}

		public int PercentCompleted
		{
			get
			{
				if (this.totalWeight != 0)
				{
					return (int)(100f * (float)this.completed / (float)this.totalWeight + 0.5f);
				}
				return 0;
			}
		}

		public void Initialize()
		{
			this.completed = 0;
		}

		public void UpdateProgress(ITask task)
		{
			this.UpdateProgress(task.Weight);
		}

		protected void UpdateProgress(int weight)
		{
			this.completed += weight;
		}

		protected void AddTask(ITask task)
		{
			this.tasks.Add(task);
			this.CalculateTotalWeight();
		}

		protected void AddOverhead(int weight)
		{
			this.overheadWeight += weight;
			this.CalculateTotalWeight();
		}

		private void CalculateTotalWeight()
		{
			this.totalWeight = this.overheadWeight;
			foreach (ITask task in this.tasks)
			{
				this.totalWeight += task.Weight;
			}
		}

		private int overheadWeight;

		private int totalWeight;

		private int completed;

		private IList<ITask> tasks;
	}
}
