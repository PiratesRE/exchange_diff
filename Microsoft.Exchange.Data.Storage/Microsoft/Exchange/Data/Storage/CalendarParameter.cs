using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.ContentTypes.iCalendar;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class CalendarParameter
	{
		internal CalendarParameter()
		{
		}

		internal bool Parse(CalendarParameterReader parameterReader)
		{
			this.parameterId = parameterReader.ParameterId;
			this.parameterName = parameterReader.Name;
			this.value = null;
			List<object> list = new List<object>();
			while (parameterReader.ReadNextValue())
			{
				if (this.value == null)
				{
					this.value = parameterReader.ReadValue();
				}
				else if (list == null)
				{
					list = new List<object>();
					list.Add(this.value);
					this.value = list;
				}
				else
				{
					list.Add(this.value);
				}
			}
			return this.value != null;
		}

		internal ParameterId ParameterId
		{
			get
			{
				return this.parameterId;
			}
		}

		internal string Name
		{
			get
			{
				return this.parameterName;
			}
		}

		internal object Value
		{
			get
			{
				return this.value;
			}
		}

		private ParameterId parameterId;

		private string parameterName;

		private object value;
	}
}
