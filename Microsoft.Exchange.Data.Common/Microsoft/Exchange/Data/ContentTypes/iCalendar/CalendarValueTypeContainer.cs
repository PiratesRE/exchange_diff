using System;
using Microsoft.Exchange.Data.ContentTypes.Internal;

namespace Microsoft.Exchange.Data.ContentTypes.iCalendar
{
	internal class CalendarValueTypeContainer : ValueTypeContainer
	{
		private void CalculateValueType()
		{
			if (this.isValueTypeInitialized)
			{
				return;
			}
			this.valueType = CalendarValueType.Unknown;
			if (this.valueTypeParameter != null)
			{
				this.valueType = CalendarCommon.GetValueTypeEnum(this.valueTypeParameter);
			}
			else
			{
				PropertyId propertyEnum = CalendarCommon.GetPropertyEnum(this.propertyName);
				if (propertyEnum != PropertyId.Unknown)
				{
					this.valueType = CalendarCommon.GetDefaultValueType(propertyEnum);
				}
			}
			if (this.valueType == CalendarValueType.Unknown)
			{
				this.valueType = CalendarValueType.Text;
			}
			this.isValueTypeInitialized = true;
		}

		public override bool IsTextType
		{
			get
			{
				this.CalculateValueType();
				return this.valueType == CalendarValueType.Text;
			}
		}

		public override bool CanBeMultivalued
		{
			get
			{
				this.CalculateValueType();
				return this.valueType != CalendarValueType.Recurrence && this.valueType != CalendarValueType.Binary;
			}
		}

		public override bool CanBeCompound
		{
			get
			{
				this.CalculateValueType();
				return this.valueType != CalendarValueType.Recurrence && this.valueType != CalendarValueType.Binary;
			}
		}

		public CalendarValueType ValueType
		{
			get
			{
				this.CalculateValueType();
				return this.valueType;
			}
		}

		private CalendarValueType valueType;
	}
}
