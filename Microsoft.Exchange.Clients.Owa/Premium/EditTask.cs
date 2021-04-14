using System;
using System.Collections;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Premium.Controls;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	public class EditTask : EditItemForm, IRegistryOnlyForm
	{
		protected override void OnLoad(EventArgs e)
		{
			ExTraceGlobals.TasksCallTracer.TraceDebug((long)this.GetHashCode(), "EditTask.OnLoad");
			base.OnLoad(e);
			this.task = base.Initialize<Task>(false, TaskUtilities.TaskPrefetchProperties);
			if (this.task != null)
			{
				if (this.task.StartDate != null)
				{
					this.startDate = this.task.StartDate.Value;
				}
				if (this.task.DueDate != null)
				{
					this.dueDate = this.task.DueDate.Value;
				}
				if (this.task.CompleteDate != null)
				{
					this.completeDate = this.task.CompleteDate.Value;
				}
				object obj = this.task.TryGetProperty(ItemSchema.ReminderDueBy);
				if (obj is ExDateTime)
				{
					this.reminderDate = (ExDateTime)obj;
				}
				this.SetWorkTime();
				TaskUtilities.RenderInfobarMessages(this.task, this.infobar);
				this.recurrenceUtilities = new RecurrenceUtilities(this.task.Recurrence, base.Response.Output);
			}
			else
			{
				int workDayStartTimeInWorkingHoursTimeZone = base.UserContext.WorkingHours.WorkDayStartTimeInWorkingHoursTimeZone;
				this.recurrenceUtilities = new RecurrenceUtilities(null, base.Response.Output);
			}
			this.toolbar = new EditTaskToolbar(base.IsEmbeddedItem, base.UserCanDeleteItem);
		}

		private string GetPropertyValue(PropertyDefinition property)
		{
			string result = string.Empty;
			if (base.Item == null)
			{
				return result;
			}
			string text = base.Item.TryGetProperty(property) as string;
			if (text != null)
			{
				result = text;
			}
			return result;
		}

		protected void LoadMessageBodyIntoStream()
		{
			BodyConversionUtilities.RenderMeetingPlainTextBody(base.Response.Output, base.Item, base.UserContext, false);
		}

		protected RecurrenceUtilities RecurrenceUtilities
		{
			get
			{
				return this.recurrenceUtilities;
			}
		}

		protected void RenderAttachments()
		{
			AttachmentWell.RenderAttachmentWell(base.Response.Output, AttachmentWellType.ReadWrite, this.AttachmentWellRenderObjects, base.UserContext);
		}

		protected void CreateAttachmentHelpers()
		{
			if (base.Item != null)
			{
				this.attachmentWellRenderObjects = AttachmentWell.GetAttachmentInformation(base.Item, base.AttachmentLinks, base.UserContext.IsPublicLogon, base.IsEmbeddedItem);
				InfobarRenderingHelper infobarRenderingHelper = new InfobarRenderingHelper(this.attachmentWellRenderObjects);
				if (infobarRenderingHelper.HasLevelOne)
				{
					this.infobar.AddMessage(SanitizedHtmlString.FromStringId(-2118248931), InfobarMessageType.Informational, AttachmentWell.AttachmentInfobarHtmlTag);
				}
			}
		}

		protected void SetWorkTime()
		{
			object obj = this.task.TryGetProperty(TaskSchema.TotalWork);
			if (obj is int && (int)obj > 0)
			{
				this.totalWork = TaskUtilities.MinutesToWork((int)obj);
			}
			object obj2 = this.task.TryGetProperty(TaskSchema.ActualWork);
			if (obj2 is int && (int)obj2 > 0)
			{
				this.actualWork = TaskUtilities.MinutesToWork((int)obj2);
			}
		}

		protected void RenderStartDateScriptObject()
		{
			RenderingUtilities.RenderDateTimeScriptObject(base.Response.Output, this.startDate);
		}

		protected void RenderDueDateScriptObject()
		{
			RenderingUtilities.RenderDateTimeScriptObject(base.Response.Output, this.dueDate);
		}

		protected void RenderCompleteDateScriptObject()
		{
			RenderingUtilities.RenderDateTimeScriptObject(base.Response.Output, this.completeDate);
		}

		protected void RenderReminderDateScriptObject()
		{
			RenderingUtilities.RenderDateTimeScriptObject(base.Response.Output, this.reminderDate);
		}

		protected void RenderCurrentDateScriptObject()
		{
			RenderingUtilities.RenderDateTimeScriptObject(base.Response.Output, DateTimeUtilities.GetLocalTime());
		}

		protected void RenderMinimumDateScriptObject()
		{
			RenderingUtilities.RenderDateTimeScriptObject(base.Response.Output, ExDateTime.MinValue);
		}

		protected void RenderStartDate()
		{
			DatePickerDropDownCombo.RenderDatePicker(base.Response.Output, "divSDate", this.startDate, DatePicker.Features.TodayButton | DatePicker.Features.NoneButton, true);
		}

		protected void RenderDueDate()
		{
			DatePickerDropDownCombo.RenderDatePicker(base.Response.Output, "divDateDue", this.dueDate, DatePicker.Features.TodayButton | DatePicker.Features.NoneButton, true);
		}

		protected void RenderCompleteDate()
		{
			DatePickerDropDownCombo.RenderDatePicker(base.Response.Output, "divDateCmplt", this.completeDate, DatePicker.Features.TodayButton | DatePicker.Features.NoneButton, true);
		}

		protected void RenderStatusDropDownList()
		{
			TaskStatus statusMapping = TaskStatus.NotStarted;
			if (this.task != null && TaskUtilities.IsValidTaskStatus(this.task.Status))
			{
				statusMapping = this.task.Status;
			}
			StatusDropDownList statusDropDownList = new StatusDropDownList("divStatus", statusMapping);
			statusDropDownList.Render(base.Response.Output);
		}

		protected void RenderPriorityDropDownList()
		{
			Importance priority = Importance.Normal;
			if (this.task != null)
			{
				object obj = this.task.TryGetProperty(ItemSchema.Importance);
				Importance importance = (Importance)obj;
				if (TaskUtilities.IsValidTaskPriority(importance))
				{
					priority = importance;
				}
			}
			PriorityDropDownList priorityDropDownList = new PriorityDropDownList("divPriority", priority);
			priorityDropDownList.Render(base.Response.Output);
		}

		protected void RenderTotalWorkDurationDropDownList()
		{
			WorkDurationDropDownList workDurationDropDownList = new WorkDurationDropDownList("divTtlWrkT", this.totalWork.WorkUnit);
			workDurationDropDownList.Render(base.Response.Output);
		}

		protected void RenderActualWorkDurationDropDownList()
		{
			WorkDurationDropDownList workDurationDropDownList = new WorkDurationDropDownList("divActWrkT", this.actualWork.WorkUnit);
			workDurationDropDownList.Render(base.Response.Output);
		}

		protected void RenderTaskOwner()
		{
			if (this.task != null)
			{
				string text = this.task.TryGetProperty(TaskSchema.TaskOwner) as string;
				if (!string.IsNullOrEmpty(text))
				{
					base.Response.Output.Write("<div class=\"w100\"><div class=\"hdrHr\">&nbsp;</div></div>");
					base.Response.Output.Write("<div class=\"hdrRow\">");
					base.Response.Output.Write("<div id=\"divOwner\" class=\"hdrLabel1\">");
					base.Response.Output.Write(LocalizedStrings.GetHtmlEncoded(1425891972));
					base.Response.Output.Write("</div>");
					base.Response.Output.Write("<div class=\"hdrField\">");
					Utilities.HtmlEncode(text, base.Response.Output);
					base.Response.Output.Write("</div></div>");
				}
			}
		}

		protected void RenderPercentCompleteDropDownList()
		{
			PercentCompleteDropDownList percentCompleteDropDownList = new PercentCompleteDropDownList("divPerCmplt", this.PercentComplete.ToString());
			percentCompleteDropDownList.Render(base.Response.Output);
		}

		protected void RenderTotalWorkValue()
		{
			if (this.task != null && this.totalWork.WorkAmount > 0f)
			{
				base.Response.Output.Write(this.totalWork.WorkAmount);
			}
		}

		protected void RenderActualWorkValue()
		{
			if (this.task != null && this.actualWork.WorkAmount > 0f)
			{
				base.Response.Output.Write(this.actualWork.WorkAmount);
			}
		}

		protected void RenderMileageValue()
		{
			if (this.task != null)
			{
				string text = this.task.TryGetProperty(TaskSchema.Mileage) as string;
				if (text != null)
				{
					Utilities.HtmlEncode(text, base.Response.Output);
				}
			}
		}

		protected void RenderBillingValue()
		{
			if (this.task != null)
			{
				string text = this.task.TryGetProperty(TaskSchema.BillingInformation) as string;
				if (text != null)
				{
					Utilities.HtmlEncode(text, base.Response.Output);
				}
			}
		}

		protected void RenderCompaniesValue()
		{
			if (this.task != null)
			{
				string[] array = this.task.TryGetProperty(TaskSchema.Companies) as string[];
				if (array != null)
				{
					foreach (string str in array)
					{
						Utilities.HtmlEncode(str + ((array.Length > 1) ? " " : string.Empty), base.Response.Output);
					}
				}
			}
		}

		protected void RenderCategoriesJavascriptArray()
		{
			CategorySwatch.RenderCategoriesJavascriptArray(base.SanitizingResponse, base.Item);
		}

		protected void RenderCategories()
		{
			if (base.Item != null)
			{
				CategorySwatch.RenderCategories(base.OwaContext, base.SanitizingResponse, base.Item);
			}
		}

		protected void RenderShowHideDetailIcon()
		{
			string text;
			if (this.IsDetailVisible)
			{
				text = "_d=\"1\"";
			}
			else
			{
				text = "_d=\"0\"";
			}
			base.UserContext.RenderThemeImage(base.Response.Output, this.IsDetailVisible ? ThemeFileId.Collapse : ThemeFileId.Expand, string.Empty, new object[]
			{
				"id=\"imgTsk\"",
				text
			});
		}

		protected bool IsReminderSet
		{
			get
			{
				if (this.task != null)
				{
					object obj = this.task.TryGetProperty(ItemSchema.ReminderIsSet);
					return obj is bool && (bool)obj;
				}
				return false;
			}
		}

		protected bool IsPrivate
		{
			get
			{
				if (this.task != null)
				{
					object obj = this.task.TryGetProperty(ItemSchema.Sensitivity);
					return obj is Sensitivity && (Sensitivity)obj == Sensitivity.Private;
				}
				return false;
			}
		}

		protected bool IsRecurring
		{
			get
			{
				return this.task != null && this.task.IsRecurring;
			}
		}

		protected bool IsDetailVisible
		{
			get
			{
				return base.UserContext.UserOptions.IsTaskDetailsVisible;
			}
		}

		protected int PercentComplete
		{
			get
			{
				if (this.task != null)
				{
					return (int)(this.task.PercentComplete * 100.0);
				}
				return 0;
			}
		}

		protected bool HasAttachments
		{
			get
			{
				return this.task != null && this.task.AttachmentCollection.Count > 0;
			}
		}

		protected string GetWhen
		{
			get
			{
				if (this.task != null)
				{
					return Utilities.HtmlEncode(TaskUtilities.GetWhen(this.task));
				}
				return string.Empty;
			}
		}

		protected ArrayList AttachmentWellRenderObjects
		{
			get
			{
				return this.attachmentWellRenderObjects;
			}
		}

		protected Infobar Infobar
		{
			get
			{
				return this.infobar;
			}
		}

		protected EditTaskToolbar Toolbar
		{
			get
			{
				return this.toolbar;
			}
		}

		protected static int WorkMinutesInDay
		{
			get
			{
				return 480;
			}
		}

		protected static int WorkMinutesInWeek
		{
			get
			{
				return 2400;
			}
		}

		protected static int MaxWorkMinutes
		{
			get
			{
				return 1525252319;
			}
		}

		protected static int ImportanceLow
		{
			get
			{
				return 0;
			}
		}

		protected static int ImportanceNormal
		{
			get
			{
				return 1;
			}
		}

		protected static int ImportanceHigh
		{
			get
			{
				return 2;
			}
		}

		protected static int TaskStatusNotStarted
		{
			get
			{
				return 0;
			}
		}

		protected static int TaskStatusInProgress
		{
			get
			{
				return 1;
			}
		}

		protected static int TaskStatusCompleted
		{
			get
			{
				return 2;
			}
		}

		protected static int TaskStatusWaitingOnOthers
		{
			get
			{
				return 3;
			}
		}

		protected static int TaskStatusDeferred
		{
			get
			{
				return 4;
			}
		}

		protected static int StoreObjectTypeTask
		{
			get
			{
				return 19;
			}
		}

		protected bool IsItemNullOrEmbeddedInNonSMimeItem
		{
			get
			{
				return base.Item == null || base.IsEmbeddedItemInNonSMimeItem;
			}
		}

		protected void RenderTitle()
		{
			if (base.Item == null)
			{
				base.Response.Write(LocalizedStrings.GetHtmlEncoded(151903378));
				return;
			}
			string propertyValue = this.GetPropertyValue(ItemSchema.Subject);
			if (string.IsNullOrEmpty(propertyValue))
			{
				base.Response.Write(LocalizedStrings.GetHtmlEncoded(151903378));
				return;
			}
			Utilities.HtmlEncode(propertyValue, base.Response.Output);
		}

		protected void RenderReminderDate()
		{
			TaskUtilities.RenderReminderDate(base.Response.Output, base.Item, this.IsReminderSet && !this.IsPublicItem);
		}

		protected void RenderReminderTimeDropDownList()
		{
			TaskUtilities.RenderReminderTimeDropDownList(base.UserContext, base.Response.Output, base.Item, this.IsReminderSet && !this.IsPublicItem);
		}

		protected void RenderSubject()
		{
			RenderingUtilities.RenderSubject(base.Response.Output, base.Item);
		}

		private Infobar infobar = new Infobar();

		private EditTaskToolbar toolbar;

		private ArrayList attachmentWellRenderObjects;

		private Task task;

		private ExDateTime startDate = ExDateTime.MinValue;

		private ExDateTime dueDate = ExDateTime.MinValue;

		private ExDateTime completeDate = ExDateTime.MinValue;

		private ExDateTime reminderDate = ExDateTime.MinValue;

		private Work actualWork = new Work(0f, DurationUnit.Hours);

		private Work totalWork = new Work(0f, DurationUnit.Hours);

		private RecurrenceUtilities recurrenceUtilities;
	}
}
