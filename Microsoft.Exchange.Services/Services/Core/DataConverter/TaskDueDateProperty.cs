using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal class TaskDueDateProperty : DateTimeProperty
	{
		private TaskDueDateProperty(CommandContext commandContext) : base(commandContext)
		{
		}

		public new static TaskDueDateProperty CreateCommand(CommandContext commandContext)
		{
			return new TaskDueDateProperty(commandContext);
		}

		protected override void SetStoreObjectProperty(StoreObject storeObject, PropertyDefinition propertyDefinition, object value)
		{
			Task task = storeObject as Task;
			ExDateTime exDateTime = (ExDateTime)value;
			if (task.Session.ExTimeZone == ExTimeZone.UtcTimeZone)
			{
				ExDateTime value2 = ExTimeZone.UtcTimeZone.ConvertDateTime(exDateTime);
				task.SetDueDatesForUtcSession(new ExDateTime?(value2), new ExDateTime?(value2));
				return;
			}
			task.DueDate = new ExDateTime?(exDateTime);
		}
	}
}
