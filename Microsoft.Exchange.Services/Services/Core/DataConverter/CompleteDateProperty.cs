using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal class CompleteDateProperty : DateTimeProperty
	{
		private CompleteDateProperty(CommandContext commandContext) : base(commandContext)
		{
		}

		public new static CompleteDateProperty CreateCommand(CommandContext commandContext)
		{
			return new CompleteDateProperty(commandContext);
		}

		internal static void SetStatusCompleted(Task task, ExDateTime completeTime)
		{
			if (task.Session.ExTimeZone == ExTimeZone.UtcTimeZone)
			{
				ExDateTime exDateTime = ExTimeZone.UtcTimeZone.ConvertDateTime(completeTime);
				task.SetCompleteTimesForUtcSession(exDateTime, new ExDateTime?(exDateTime));
				return;
			}
			task.SetStatusCompleted(completeTime);
		}

		protected override void SetStoreObjectProperty(StoreObject storeObject, PropertyDefinition propertyDefinition, object value)
		{
			Task task = storeObject as Task;
			ExDateTime exDateTime = (ExDateTime)value;
			if (ExDateTime.Compare(ExDateTime.Now, exDateTime) < 0 && !ExchangeVersion.Current.Supports(ExchangeVersion.Exchange2012))
			{
				throw new InvalidCompleteDateException((CoreResources.IDs)3098927940U);
			}
			try
			{
				CompleteDateProperty.SetStatusCompleted(task, exDateTime);
			}
			catch (ArgumentOutOfRangeException innerException)
			{
				throw new InvalidCompleteDateException((CoreResources.IDs)3371984686U, innerException);
			}
		}
	}
}
