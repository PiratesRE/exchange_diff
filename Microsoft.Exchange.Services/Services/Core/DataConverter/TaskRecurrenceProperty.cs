using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal sealed class TaskRecurrenceProperty : ComplexPropertyBase, IToXmlCommand, IToServiceObjectCommand, ISetCommand, ISetUpdateCommand, IDeleteUpdateCommand, IUpdateCommand, IPropertyCommand
	{
		public TaskRecurrenceProperty(CommandContext commandContext) : base(commandContext)
		{
		}

		public static TaskRecurrenceProperty CreateCommand(CommandContext commandContext)
		{
			return new TaskRecurrenceProperty(commandContext);
		}

		public void ToServiceObject()
		{
			ToServiceObjectCommandSettings commandSettings = base.GetCommandSettings<ToServiceObjectCommandSettings>();
			ServiceObject serviceObject = commandSettings.ServiceObject;
			Task task = commandSettings.StoreObject as Task;
			serviceObject.PropertyBag[TaskSchema.Recurrence] = RecurrenceHelper.Recurrence.RenderForTask(task.Recurrence);
		}

		public void Set()
		{
			SetCommandSettings commandSettings = base.GetCommandSettings<SetCommandSettings>();
			Task task = commandSettings.StoreObject as Task;
			TaskRecurrenceProperty.SetProperty(task, (TaskRecurrenceType)commandSettings.ServiceObject[TaskSchema.Recurrence]);
		}

		public override void SetUpdate(SetPropertyUpdate setPropertyUpdate, UpdateCommandSettings updateCommandSettings)
		{
			Task task = updateCommandSettings.StoreObject as Task;
			TaskRecurrenceProperty.SetProperty(task, (TaskRecurrenceType)setPropertyUpdate.ServiceObject[TaskSchema.Recurrence]);
		}

		public override void DeleteUpdate(DeletePropertyUpdate deletePropertyUpdate, UpdateCommandSettings updateCommandSettings)
		{
			Task task = updateCommandSettings.StoreObject as Task;
			task.Recurrence = null;
		}

		private static void SetProperty(Task task, TaskRecurrenceType recurrenceType)
		{
			ExTimeZone timezone;
			if (ExchangeVersion.Current.Supports(ExchangeVersion.Exchange2007SP1))
			{
				timezone = task.Session.ExTimeZone;
			}
			else
			{
				timezone = null;
			}
			Recurrence recurrence;
			if (RecurrenceHelper.Recurrence.Parse(timezone, recurrenceType, out recurrence))
			{
				try
				{
					task.Recurrence = recurrence;
				}
				catch (NotSupportedException)
				{
					throw new UnsupportedRecurrenceException();
				}
			}
		}

		public void ToXml()
		{
			throw new InvalidOperationException("TaskRecurrenceProperty.ToXml should not be called.");
		}
	}
}
