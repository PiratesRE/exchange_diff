using System;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.AirSync.SchemaConverter.XSO
{
	[Serializable]
	internal class XsoTaskStateProperty : XsoProperty, ITaskState, IProperty
	{
		public XsoTaskStateProperty() : base(null)
		{
		}

		public bool Complete
		{
			get
			{
				Task task = (Task)base.XsoItem;
				return task.IsComplete;
			}
		}

		public ExDateTime? DateCompleted
		{
			get
			{
				Task task = (Task)base.XsoItem;
				ExDateTime? result = task.CompleteDate;
				if (result == null)
				{
					task.Load(new PropertyDefinition[]
					{
						ItemSchema.FlagCompleteTime
					});
					result = task.GetValueAsNullable<ExDateTime>(ItemSchema.FlagCompleteTime);
				}
				return result;
			}
		}

		protected override void InternalSetToDefault(IProperty srcProperty)
		{
			Task task = (Task)base.XsoItem;
			task.SetStatusNotStarted();
		}

		protected override void InternalCopyFromModified(IProperty srcProperty)
		{
			Task task = (Task)base.XsoItem;
			ITaskState taskState = (ITaskState)srcProperty;
			if (taskState.Complete)
			{
				if (taskState.DateCompleted != null)
				{
					if (task.IsRecurring)
					{
						task.SuppressCreateOneOff = false;
					}
					task.SetCompleteTimesForUtcSession(taskState.DateCompleted.Value, taskState.DateCompleted);
					return;
				}
				throw new ConversionException("DateCompleted must be specified if Complete = 1");
			}
			else
			{
				if (taskState.DateCompleted != null)
				{
					throw new ConversionException("DateCompleted must not be specified if Complete = 0");
				}
				task.SetStatusNotStarted();
				return;
			}
		}
	}
}
