using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class TaskRequestSchema : MessageItemSchema
	{
		private TaskRequestSchema()
		{
			base.AddDependencies(new Schema[]
			{
				TaskSchema.Instance
			});
		}

		public new static TaskRequestSchema Instance
		{
			get
			{
				if (TaskRequestSchema.instance == null)
				{
					TaskRequestSchema.instance = new TaskRequestSchema();
				}
				return TaskRequestSchema.instance;
			}
		}

		private static TaskRequestSchema instance;
	}
}
