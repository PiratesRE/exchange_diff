using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.ContentTypes.iCalendar;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class CalendarPropertyBase
	{
		internal bool Parse(CalendarPropertyReader propertyReader)
		{
			this.calendarPropertyId = new CalendarPropertyId(propertyReader.PropertyId, propertyReader.Name);
			this.value = null;
			this.valueType = propertyReader.ValueType;
			this.parameters = new List<CalendarParameter>();
			CalendarParameterReader parameterReader = propertyReader.ParameterReader;
			while (parameterReader.ReadNextParameter())
			{
				CalendarParameter calendarParameter = new CalendarParameter();
				if (!calendarParameter.Parse(parameterReader) || !this.ProcessParameter(calendarParameter))
				{
					return false;
				}
				this.parameters.Add(calendarParameter);
			}
			this.valueType = propertyReader.ValueType;
			SchemaInfo schemaInfo;
			if (VEvent.GetConversionSchema().TryGetValue(this.calendarPropertyId.Key, out schemaInfo) && schemaInfo.IsMultiValue)
			{
				List<object> list = new List<object>();
				while (propertyReader.ReadNextValue())
				{
					object item = this.ReadValue(propertyReader);
					list.Add(item);
				}
				this.value = list;
			}
			else
			{
				this.value = this.ReadValue(propertyReader);
			}
			return this.Validate();
		}

		internal CalendarValueType ValueType
		{
			get
			{
				return this.valueType;
			}
		}

		internal object Value
		{
			get
			{
				return this.value;
			}
			set
			{
				this.value = value;
			}
		}

		internal List<CalendarParameter> Parameters
		{
			get
			{
				return this.parameters;
			}
		}

		internal CalendarParameter GetParameter(ParameterId parameterId)
		{
			foreach (CalendarParameter calendarParameter in this.parameters)
			{
				if (calendarParameter.ParameterId == parameterId)
				{
					return calendarParameter;
				}
			}
			return null;
		}

		internal CalendarParameter GetParameter(string parameterName)
		{
			foreach (CalendarParameter calendarParameter in this.parameters)
			{
				if (string.Compare(calendarParameter.Name, parameterName, StringComparison.CurrentCultureIgnoreCase) == 0)
				{
					return calendarParameter;
				}
			}
			return null;
		}

		internal CalendarPropertyId CalendarPropertyId
		{
			get
			{
				return this.calendarPropertyId;
			}
		}

		protected virtual bool ProcessParameter(CalendarParameter parameter)
		{
			return true;
		}

		protected virtual bool Validate()
		{
			return this.calendarPropertyId.PropertyId == PropertyId.Unknown || this.value != null;
		}

		protected virtual object ReadValue(CalendarPropertyReader propertyReader)
		{
			return propertyReader.ReadValue();
		}

		private CalendarValueType valueType;

		private object value;

		private List<CalendarParameter> parameters;

		private CalendarPropertyId calendarPropertyId;
	}
}
