using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.ContentTypes.iCalendar;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class CalendarDateTime : CalendarPropertyBase
	{
		protected override bool ProcessParameter(CalendarParameter parameter)
		{
			ParameterId parameterId = parameter.ParameterId;
			if (parameterId == ParameterId.TimeZoneId)
			{
				this.timeZoneId = (string)parameter.Value;
			}
			return true;
		}

		protected override bool Validate()
		{
			if (base.Validate())
			{
				if (base.Value is DateTime)
				{
					return true;
				}
				List<object> list = base.Value as List<object>;
				if (list != null)
				{
					for (int i = 0; i < list.Count; i++)
					{
						if (!(list[i] is DateTime))
						{
							return false;
						}
					}
					return true;
				}
			}
			return false;
		}

		protected override object ReadValue(CalendarPropertyReader propertyReader)
		{
			return propertyReader.ReadValueAsDateTime();
		}

		public string TimeZoneId
		{
			get
			{
				return this.timeZoneId;
			}
			set
			{
				this.timeZoneId = value;
			}
		}

		private string timeZoneId;
	}
}
