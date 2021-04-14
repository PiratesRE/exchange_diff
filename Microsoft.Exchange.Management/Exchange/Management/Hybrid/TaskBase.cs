using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Hybrid
{
	internal abstract class TaskBase : ITask
	{
		public TaskBase(string taskName, int weight)
		{
			this.Name = taskName;
			this.Weight = weight;
		}

		public string Name { get; private set; }

		public int Weight { get; private set; }

		private protected ITaskContext TaskContext { protected get; private set; }

		public virtual bool CheckPrereqs(ITaskContext taskContext)
		{
			this.TaskContext = taskContext;
			return true;
		}

		public virtual bool NeedsConfiguration(ITaskContext taskContext)
		{
			this.TaskContext = taskContext;
			return false;
		}

		public virtual bool Configure(ITaskContext taskContext)
		{
			this.TaskContext = taskContext;
			return true;
		}

		public virtual bool ValidateConfiguration(ITaskContext taskContext)
		{
			this.TaskContext = taskContext;
			return true;
		}

		protected void AddLocalizedStringError(LocalizedString errorMessage)
		{
			this.TaskContext.Errors.Add(errorMessage);
		}

		protected void AddLocalizedStringWarning(LocalizedString warningMessage)
		{
			this.TaskContext.Warnings.Add(warningMessage);
		}
	}
}
