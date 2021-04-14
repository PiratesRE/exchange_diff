using System;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.AirSync.SchemaConverter.XSO
{
	[Serializable]
	internal class XsoStartDueDateProperty : XsoProperty, IStartDueDateProperty, IProperty
	{
		public XsoStartDueDateProperty() : base(null)
		{
		}

		public ExDateTime? UtcStartDate
		{
			get
			{
				return base.XsoItem.GetValueAsNullable<ExDateTime>(ItemSchema.UtcStartDate);
			}
		}

		public ExDateTime? StartDate
		{
			get
			{
				return base.XsoItem.GetValueAsNullable<ExDateTime>(ItemSchema.LocalStartDate);
			}
		}

		public ExDateTime? UtcDueDate
		{
			get
			{
				return base.XsoItem.GetValueAsNullable<ExDateTime>(ItemSchema.UtcDueDate);
			}
		}

		public ExDateTime? DueDate
		{
			get
			{
				return base.XsoItem.GetValueAsNullable<ExDateTime>(ItemSchema.LocalDueDate);
			}
		}

		protected override void InternalSetToDefault(IProperty srcProperty)
		{
			Task task = base.XsoItem as Task;
			if (task == null)
			{
				throw new UnexpectedTypeException("Task", base.XsoItem);
			}
			task.StartDate = null;
			task.DueDate = null;
		}

		protected override void InternalCopyFromModified(IProperty srcProperty)
		{
			IStartDueDateProperty startDueDateProperty = (IStartDueDateProperty)srcProperty;
			Task task = base.XsoItem as Task;
			if ((startDueDateProperty.StartDate == null && startDueDateProperty.UtcStartDate != null) || (startDueDateProperty.StartDate != null && startDueDateProperty.UtcStartDate == null) || (startDueDateProperty.DueDate == null && startDueDateProperty.UtcDueDate != null) || (startDueDateProperty.DueDate != null && startDueDateProperty.UtcDueDate == null))
			{
				throw new ConversionException("Both Utc and local dates should be present for StartDate and DueDate");
			}
			if ((startDueDateProperty.UtcStartDate != null && startDueDateProperty.UtcDueDate != null && startDueDateProperty.UtcStartDate.Value > startDueDateProperty.UtcDueDate.Value) || (startDueDateProperty.StartDate != null && startDueDateProperty.DueDate != null && startDueDateProperty.StartDate.Value > startDueDateProperty.DueDate.Value))
			{
				throw new ConversionException("StartDate should be before DueDate");
			}
			if (startDueDateProperty.StartDate == null)
			{
				task.StartDate = null;
			}
			else
			{
				base.XsoItem.SetOrDeleteProperty(ItemSchema.UtcStartDate, startDueDateProperty.UtcStartDate);
				base.XsoItem.SetOrDeleteProperty(ItemSchema.LocalStartDate, startDueDateProperty.StartDate);
			}
			if (startDueDateProperty.DueDate == null)
			{
				task.DueDate = null;
				return;
			}
			base.XsoItem.SetOrDeleteProperty(ItemSchema.UtcDueDate, startDueDateProperty.UtcDueDate);
			base.XsoItem.SetOrDeleteProperty(ItemSchema.LocalDueDate, startDueDateProperty.DueDate);
		}
	}
}
