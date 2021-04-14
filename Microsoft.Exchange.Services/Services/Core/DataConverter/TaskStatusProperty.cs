using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal class TaskStatusProperty : SimpleProperty
	{
		private TaskStatusProperty(CommandContext commandContext, BaseConverter converter) : base(commandContext, converter)
		{
		}

		public new static TaskStatusProperty CreateCommand(CommandContext commandContext)
		{
			return new TaskStatusProperty(commandContext, new TaskStatusConverter());
		}

		protected override void SetStoreObjectProperty(StoreObject storeObject, PropertyDefinition propertyDefinition, object value)
		{
			Task task = storeObject as Task;
			switch ((TaskStatus)value)
			{
			case TaskStatus.NotStarted:
				task.SetStatusNotStarted();
				return;
			case TaskStatus.InProgress:
				task.SetStatusInProgress();
				return;
			case TaskStatus.Completed:
				CompleteDateProperty.SetStatusCompleted(task, ExDateTime.Now);
				return;
			case TaskStatus.WaitingOnOthers:
				task.SetStatusWaitingOnOthers();
				return;
			case TaskStatus.Deferred:
				task.SetStatusDeferred();
				return;
			default:
				return;
			}
		}
	}
}
