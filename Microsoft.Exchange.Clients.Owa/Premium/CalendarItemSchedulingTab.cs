using System;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Clients.Owa.Premium.Controls;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	public class CalendarItemSchedulingTab : OwaForm
	{
		protected static int FreeBusyInterval
		{
			get
			{
				return 30;
			}
		}

		protected ExDateTime StartDate
		{
			get
			{
				return this.startDateTime;
			}
		}

		protected ExDateTime EndDate
		{
			get
			{
				return this.endDateTime;
			}
		}

		protected int MeetingDuration
		{
			get
			{
				return this.meetingDuration;
			}
		}

		protected bool Show24Hours
		{
			get
			{
				return this.show24Hours;
			}
		}

		protected int Days
		{
			get
			{
				return ((DateTime)this.endDateTime - (DateTime)this.startDateTime).Days + 1;
			}
		}

		internal WorkingHours WorkingHours
		{
			get
			{
				if (this.workingHours == null)
				{
					if (this.folderId == null || !this.folderId.IsOtherMailbox)
					{
						this.workingHours = base.UserContext.WorkingHours;
					}
					else
					{
						this.workingHours = base.UserContext.GetOthersWorkingHours(this.folderId);
					}
				}
				return this.workingHours;
			}
		}

		protected int HoursPerDay
		{
			get
			{
				if (this.show24Hours)
				{
					return 24;
				}
				return SchedulingTabRenderingUtilities.CalculateTotalWorkingHours(this.WorkingHours);
			}
		}

		internal IExchangePrincipal OrganizerExchangePrincipal
		{
			get
			{
				if (this.organizerExchangePrincipal == null)
				{
					this.organizerExchangePrincipal = Utilities.GetFolderOwnerExchangePrincipal(this.folderId, base.UserContext);
				}
				return this.organizerExchangePrincipal;
			}
		}

		protected string OrganizerSmtpAddress
		{
			get
			{
				return this.OrganizerExchangePrincipal.MailboxInfo.PrimarySmtpAddress.ToString();
			}
		}

		protected string OrganizerEmailAddress
		{
			get
			{
				return this.OrganizerExchangePrincipal.LegacyDn;
			}
		}

		protected string OrganizerDisplayName
		{
			get
			{
				return this.OrganizerExchangePrincipal.MailboxInfo.DisplayName;
			}
		}

		protected string OrganizerAdObjectId
		{
			get
			{
				return Convert.ToBase64String(this.OrganizerExchangePrincipal.ObjectId.ObjectGuid.ToByteArray());
			}
		}

		protected RecipientWell RecipientWell
		{
			get
			{
				return this.recipientWell;
			}
		}

		protected int WorkDayStart
		{
			get
			{
				return SchedulingTabRenderingUtilities.GetWorkDayStartHour(this.WorkingHours, this.selectedDate) * 60;
			}
		}

		protected int WorkDayEnd
		{
			get
			{
				return SchedulingTabRenderingUtilities.GetWorkDayEndHour(this.WorkingHours, this.selectedDate) * 60;
			}
		}

		private bool DifferentWorkingHoursTimeZone
		{
			get
			{
				return this.WorkingHours.IsTimeZoneDifferent;
			}
		}

		protected bool ForceShowing24Hours
		{
			get
			{
				return this.WorkingHours != base.UserContext.WorkingHours || this.DifferentWorkingHoursTimeZone || this.WorkHoursDuration < 60;
			}
		}

		private int WorkHoursDuration
		{
			get
			{
				return this.WorkingHours.WorkDayEndTimeInWorkingHoursTimeZone - this.WorkingHours.WorkDayStartTimeInWorkingHoursTimeZone;
			}
		}

		protected bool IsFolderIdNull
		{
			get
			{
				return this.folderId == null;
			}
		}

		protected override void OnLoad(EventArgs e)
		{
			this.selectedDate = (this.startDateTime = Utilities.GetQueryStringParameterDateTime(base.Request, "sd", base.UserContext.TimeZone));
			this.endDateTime = Utilities.GetQueryStringParameterDateTime(base.Request, "ed", base.UserContext.TimeZone);
			string queryStringParameter = Utilities.GetQueryStringParameter(base.Request, "fid", false);
			if (!string.IsNullOrEmpty(queryStringParameter))
			{
				this.folderId = OwaStoreObjectId.CreateFromString(queryStringParameter);
			}
			if (this.startDateTime < this.endDateTime)
			{
				this.meetingDuration = (int)(this.endDateTime - this.startDateTime).TotalMinutes;
			}
			DatePickerBase.GetVisibleDateRange(this.selectedDate, out this.startDateTime, out this.endDateTime, base.UserContext.TimeZone);
			if (this.selectedDate.TimeOfDay.TotalMinutes < (double)this.WorkingHours.GetWorkDayStartTime(this.selectedDate) || (double)this.WorkingHours.GetWorkDayEndTime(this.selectedDate) <= this.selectedDate.TimeOfDay.TotalMinutes || (double)this.WorkingHours.GetWorkDayEndTime(this.selectedDate) < this.selectedDate.TimeOfDay.TotalMinutes + (double)this.meetingDuration || this.ForceShowing24Hours)
			{
				this.show24Hours = true;
			}
			this.recipientWell = new CalendarItemRecipientWell();
		}

		protected void RenderDatePicker()
		{
			DatePicker datePicker = new DatePicker("dpSchd", this.WorkingHours, new ExDateTime[]
			{
				this.selectedDate
			});
			datePicker.Render(base.Response.Output);
		}

		protected void RenderDurationDropdown()
		{
			DurationDropDownList.RenderDurationPicker(base.Response.Output, this.meetingDuration, "divSchedulingDur");
		}

		protected void RenderGridDayNames()
		{
			SchedulingTabRenderingUtilities.RenderGridDayNames(base.Response.Output, this.startDateTime, this.endDateTime);
		}

		protected void RenderJavaScriptEncodedFolderId()
		{
			if (this.folderId != null)
			{
				Utilities.JavascriptEncode(this.folderId.ToBase64String(), base.Response.Output);
			}
		}

		private ExDateTime selectedDate;

		private ExDateTime startDateTime;

		private ExDateTime endDateTime;

		private bool show24Hours;

		private int meetingDuration = 30;

		private CalendarItemRecipientWell recipientWell;

		private OwaStoreObjectId folderId;

		private WorkingHours workingHours;

		private IExchangePrincipal organizerExchangePrincipal;
	}
}
