using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal class TaskStatusDescriptionProperty : SimpleProperty
	{
		private TaskStatusDescriptionProperty(CommandContext commandContext, BaseConverter converter) : base(commandContext, converter)
		{
		}

		public new static TaskStatusDescriptionProperty CreateCommand(CommandContext commandContext)
		{
			return new TaskStatusDescriptionProperty(commandContext, new StringConverter());
		}

		protected override bool StorePropertyExists(StoreObject storeObject)
		{
			return true;
		}

		protected override object GetPropertyValue(StoreObject storeObject)
		{
			Task task = storeObject as Task;
			CallContext callContext = CallContext.Current;
			return task.StatusDescription.ToString(callContext.ClientCulture);
		}
	}
}
