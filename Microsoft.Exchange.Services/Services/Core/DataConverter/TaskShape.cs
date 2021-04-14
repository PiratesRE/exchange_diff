using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal sealed class TaskShape : Shape
	{
		static TaskShape()
		{
			TaskShape.defaultProperties.Add(ItemSchema.ItemId);
			TaskShape.defaultProperties.Add(ItemSchema.Subject);
			TaskShape.defaultProperties.Add(ItemSchema.Attachments);
			TaskShape.defaultProperties.Add(ItemSchema.HasAttachments);
			TaskShape.defaultProperties.Add(TaskSchema.DueDate);
			TaskShape.defaultProperties.Add(TaskSchema.PercentComplete);
			TaskShape.defaultProperties.Add(TaskSchema.StartDate);
			TaskShape.defaultProperties.Add(TaskSchema.Status);
		}

		private TaskShape() : base(Schema.Task, TaskSchema.GetSchema(), ItemShape.CreateShape(), TaskShape.defaultProperties)
		{
		}

		internal static TaskShape CreateShape()
		{
			return new TaskShape();
		}

		private static List<PropertyInformation> defaultProperties = new List<PropertyInformation>();
	}
}
