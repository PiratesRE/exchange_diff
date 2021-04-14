using System;
using Microsoft.Exchange.Data.ContentTypes.iCalendar;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class CalendarAttendee : CalendarPropertyBase
	{
		internal bool IsResponseRequested
		{
			get
			{
				return this.isResponseRequested;
			}
		}

		internal string Name
		{
			get
			{
				return this.name;
			}
		}

		internal string Address
		{
			get
			{
				return this.address;
			}
		}

		internal string ParticipationStatus
		{
			get
			{
				return this.partstat;
			}
		}

		internal string ParticipationRole
		{
			get
			{
				return this.role;
			}
		}

		internal string CalendarUserType
		{
			get
			{
				return this.calendarUserType;
			}
		}

		internal string SentBy
		{
			get
			{
				return this.sentBy;
			}
		}

		protected override bool ProcessParameter(CalendarParameter parameter)
		{
			ParameterId parameterId = parameter.ParameterId;
			if (parameterId <= ParameterId.Delegatee)
			{
				if (parameterId <= ParameterId.CalendarUserType)
				{
					if (parameterId != ParameterId.CommonName)
					{
						if (parameterId == ParameterId.CalendarUserType)
						{
							this.calendarUserType = (string)parameter.Value;
						}
					}
					else
					{
						this.name = CalendarUtil.RemoveDoubleQuotes((string)parameter.Value);
					}
				}
				else if (parameterId != ParameterId.Delegator && parameterId != ParameterId.Delegatee)
				{
				}
			}
			else if (parameterId <= ParameterId.ParticipationRole)
			{
				if (parameterId != ParameterId.ParticipationStatus)
				{
					if (parameterId == ParameterId.ParticipationRole)
					{
						this.role = (string)parameter.Value;
					}
				}
				else
				{
					this.partstat = (string)parameter.Value;
				}
			}
			else if (parameterId != ParameterId.RsvpExpectation)
			{
				if (parameterId == ParameterId.SentBy)
				{
					string text = CalendarUtil.RemoveDoubleQuotes((string)parameter.Value);
					if (text != null)
					{
						text = CalendarUtil.RemoveMailToPrefix(text);
					}
					this.sentBy = text;
				}
			}
			else if (!bool.TryParse(parameter.Value as string, out this.isResponseRequested))
			{
				this.isResponseRequested = true;
			}
			return true;
		}

		protected override bool Validate()
		{
			if (!base.Validate())
			{
				return false;
			}
			string text = base.Value as string;
			if (text != null)
			{
				this.address = CalendarUtil.RemoveMailToPrefix(text);
			}
			return this.address.Length > 0;
		}

		private string address = string.Empty;

		private string sentBy = string.Empty;

		private string name = string.Empty;

		private string partstat = string.Empty;

		private string role = string.Empty;

		private string calendarUserType = string.Empty;

		private bool isResponseRequested = true;
	}
}
