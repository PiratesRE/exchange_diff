using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal class TaskStartDateProperty : DateTimeProperty
	{
		private TaskStartDateProperty(CommandContext commandContext) : base(commandContext)
		{
		}

		public new static TaskStartDateProperty CreateCommand(CommandContext commandContext)
		{
			return new TaskStartDateProperty(commandContext);
		}

		protected override void SetStoreObjectProperty(StoreObject storeObject, PropertyDefinition propertyDefinition, object value)
		{
			Task task = storeObject as Task;
			ExDateTime exDateTime = (ExDateTime)value;
			if (task.Session.ExTimeZone == ExTimeZone.UtcTimeZone)
			{
				ExDateTime value2 = ExTimeZone.UtcTimeZone.ConvertDateTime(exDateTime);
				task.SetStartDatesForUtcSession(new ExDateTime?(value2), new ExDateTime?(value2));
				return;
			}
			task.StartDate = new ExDateTime?(exDateTime);
		}
	}
}
