using System;
using System.Collections;
using System.Text;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Premium.Controls;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	public class ReadTask : OwaForm, IRegistryOnlyForm
	{
		protected override void OnLoad(EventArgs e)
		{
			ExTraceGlobals.TasksCallTracer.TraceDebug((long)this.GetHashCode(), "ReadTask.OnLoad");
			base.OnLoad(e);
			this.task = base.Initialize<Task>(TaskUtilities.TaskPrefetchProperties);
			TaskUtilities.RenderInfobarMessages(this.task, this.infobar);
			if (!this.IsAssignedTask && !this.IsPreview)
			{
				this.infobar.AddMessage(2078257811, InfobarMessageType.Informational);
			}
		}

		protected void LoadMessageBodyIntoStream()
		{
			BodyConversionUtilities.RenderMeetingPlainTextBody(base.Response.Output, base.Item, base.UserContext, false);
		}

		protected void RenderAttachments()
		{
			AttachmentWell.RenderAttachmentWell(base.Response.Output, AttachmentWellType.ReadOnly, this.AttachmentWellRenderObjects, base.UserContext);
		}

		protected void CreateAttachmentHelpers()
		{
			this.attachmentWellRenderObjects = AttachmentWell.GetAttachmentInformation(base.Item, base.AttachmentLinks, base.UserContext.IsPublicLogon, base.IsEmbeddedItem);
			this.shouldRenderAttachmentWell = RenderingUtilities.AddAttachmentInfobarMessages(base.Item, base.IsEmbeddedItem, false, this.infobar, this.attachmentWellRenderObjects);
		}

		private bool IsPreview
		{
			get
			{
				FormsRegistryContext formsRegistryContext = base.OwaContext.FormsRegistryContext;
				return formsRegistryContext != null && "Preview".Equals(formsRegistryContext.Action);
			}
		}

		protected void RenderToolbar()
		{
			if (!this.IsPreview)
			{
				ReadTaskToolbar readTaskToolbar = new ReadTaskToolbar(this.IsAssignedTask, base.UserCanDeleteItem);
				readTaskToolbar.Render(base.Response.Output);
			}
		}

		protected void RenderCurrentTimeScriptObject()
		{
			ExDateTime localTime = DateTimeUtilities.GetLocalTime();
			RenderingUtilities.RenderDateTimeScriptObject(base.Response.Output, localTime);
		}

		protected void RenderReminderTimeScriptObject()
		{
			object obj = this.task.TryGetProperty(ItemSchema.ReminderDueBy);
			if (obj is ExDateTime)
			{
				this.reminderDate = (ExDateTime)obj;
			}
			else
			{
				this.reminderDate = this.reminderDate.AddMinutes((double)base.UserContext.WorkingHours.WorkDayStartTimeInWorkingHoursTimeZone);
			}
			RenderingUtilities.RenderDateTimeScriptObject(base.Response.Output, this.reminderDate);
		}

		protected void RenderTitle()
		{
			RenderingUtilities.RenderSubject(base.Response.Output, base.Item);
		}

		protected void RenderSubject()
		{
			base.Response.Write("<div id=divSubj>");
			RenderingUtilities.RenderSubject(base.Response.Output, base.Item);
			base.Response.Write("</div>");
		}

		private void RenderInformationRow(string titleDivId, string valueDivId, Strings.IDs titleStringId, params string[] nonEncodedValues)
		{
			base.SanitizingResponse.Write("<div class=\"roWellRow\"><div id=\"");
			base.SanitizingResponse.Write(titleDivId);
			base.SanitizingResponse.Write("\" class=\"roWellLabel pvwLabel\">");
			base.SanitizingResponse.Write(LocalizedStrings.GetNonEncoded(titleStringId));
			base.SanitizingResponse.Write("</div><div class=\"roWellWrap\">");
			base.SanitizingResponse.Write("<div id=\"");
			base.SanitizingResponse.Write(valueDivId);
			base.SanitizingResponse.Write("\" class=\"wellField\">");
			base.SanitizingResponse.Write(string.Join(" ", nonEncodedValues));
			base.SanitizingResponse.Write("</div></div></div>");
		}

		protected void RenderDueDate()
		{
			if (this.task.DueDate != null)
			{
				this.RenderInformationRow("divToL", "divFieldTo", -828041243, new string[]
				{
					this.DueDate
				});
			}
		}

		protected void RenderCompleteDate()
		{
			if (this.task.CompleteDate != null)
			{
				this.RenderInformationRow("divDateCompletedL", "divFieldDateCompleted", -969999070, new string[]
				{
					this.CompleteDate
				});
			}
		}

		protected void RenderStatus()
		{
			this.RenderInformationRow("divStatusL", "divFieldStatus", -883489071, new string[]
			{
				this.Status
			});
		}

		protected void RenderPriority()
		{
			this.RenderInformationRow("divPriorityL", "divFieldPriority", 1501244451, new string[]
			{
				this.Priority
			});
		}

		protected void RenderPercentComplete()
		{
			this.RenderInformationRow("divPercentCompleteL", "divFieldPercentComplete", 2043350763, new string[]
			{
				(this.task.PercentComplete * 100.0).ToString()
			});
		}

		protected void RenderTaskRecurrence()
		{
			if (this.task.IsRecurring)
			{
				this.RenderInformationRow("divRecurrenceL", "divFieldRecurrence", 998368285, new string[]
				{
					TaskUtilities.GetWhen(this.task)
				});
			}
		}

		protected void RenderTaskOwner()
		{
			string text = this.task.TryGetProperty(TaskSchema.TaskOwner) as string;
			if (!string.IsNullOrEmpty(text))
			{
				this.RenderInformationRow("divOwnerL", "divFieldOwner", 1425891972, new string[]
				{
					text
				});
			}
		}

		private string GetWorkAmountNonEncodedString(Work work)
		{
			StringBuilder stringBuilder = new StringBuilder(32);
			stringBuilder.Append(work.WorkAmount);
			stringBuilder.Append(" ");
			switch (work.WorkUnit)
			{
			case DurationUnit.Minutes:
				stringBuilder.Append(LocalizedStrings.GetNonEncoded(-178797907));
				break;
			case DurationUnit.Hours:
				stringBuilder.Append(LocalizedStrings.GetNonEncoded(-1483270941));
				break;
			case DurationUnit.Days:
				stringBuilder.Append(LocalizedStrings.GetNonEncoded(-1872639189));
				break;
			case DurationUnit.Weeks:
				stringBuilder.Append(LocalizedStrings.GetNonEncoded(-1893458757));
				break;
			}
			return stringBuilder.ToString();
		}

		protected void RenderTotalWork()
		{
			object obj = this.task.TryGetProperty(TaskSchema.TotalWork);
			if (obj is int && (int)obj > 0)
			{
				Work work = TaskUtilities.MinutesToWork((int)obj);
				this.RenderInformationRow("divTotalWorkL", "divFieldTotalWork", -540606344, new string[]
				{
					this.GetWorkAmountNonEncodedString(work)
				});
			}
		}

		protected void RenderActualWork()
		{
			object obj = this.task.TryGetProperty(TaskSchema.ActualWork);
			if (obj is int && (int)obj > 0)
			{
				Work work = TaskUtilities.MinutesToWork((int)obj);
				this.RenderInformationRow("divActualWorkL", "divFieldActualWork", -1521146692, new string[]
				{
					this.GetWorkAmountNonEncodedString(work)
				});
			}
		}

		protected void RenderMileage()
		{
			string text = this.task.TryGetProperty(TaskSchema.Mileage) as string;
			if (!string.IsNullOrEmpty(text))
			{
				this.RenderInformationRow("divMileageL", "divFieldMileage", 631649291, new string[]
				{
					text
				});
			}
		}

		protected void RenderBillingInformation()
		{
			string text = this.task.TryGetProperty(TaskSchema.BillingInformation) as string;
			if (!string.IsNullOrEmpty(text))
			{
				this.RenderInformationRow("divBillingL", "divFieldBilling", -914943280, new string[]
				{
					text
				});
			}
		}

		protected void RenderCompanies()
		{
			string[] array = this.task.TryGetProperty(TaskSchema.Companies) as string[];
			if (array != null && array.Length > 0)
			{
				this.RenderInformationRow("divCompaniesL", "divFieldCompanies", -1940990688, array);
			}
		}

		protected void RenderReminderDate()
		{
			TaskUtilities.RenderReminderDate(base.Response.Output, base.Item, this.ReminderIsSet && !this.IsPublicItem && base.UserCanEditItem);
		}

		protected void RenderReminderTimeDropDownList()
		{
			TaskUtilities.RenderReminderTimeDropDownList(base.UserContext, base.Response.Output, base.Item, this.ReminderIsSet && !this.IsPublicItem && base.UserCanEditItem);
		}

		protected void RenderOwaPlainTextStyle()
		{
			OwaPlainTextStyle.WriteLocalizedStyleIntoHeadForPlainTextBody(base.Item, base.Response.Output, "DIV#divBdy");
		}

		protected void RenderCategories()
		{
			CategorySwatch.RenderCategories(base.OwaContext, base.SanitizingResponse, base.Item);
		}

		protected ArrayList AttachmentWellRenderObjects
		{
			get
			{
				return this.attachmentWellRenderObjects;
			}
		}

		protected bool ShouldRenderAttachmentWell
		{
			get
			{
				return this.shouldRenderAttachmentWell;
			}
		}

		protected string DueDate
		{
			get
			{
				string result = LocalizedStrings.GetNonEncoded(1414246128);
				if (this.task.DueDate != null)
				{
					ExDateTime value = this.task.DueDate.Value;
					if (this.task.StartDate != null)
					{
						ExDateTime value2 = this.task.StartDate.Value;
						result = string.Format(LocalizedStrings.GetNonEncoded(1970384503), value2.ToString("d"), value.ToString("d"));
					}
					else
					{
						result = string.Format(LocalizedStrings.GetNonEncoded(-535552699), value.ToString("d"));
					}
				}
				return result;
			}
		}

		protected string CompleteDate
		{
			get
			{
				string result = LocalizedStrings.GetNonEncoded(1414246128);
				if (this.task.CompleteDate != null)
				{
					result = this.task.CompleteDate.Value.ToString("d");
				}
				return result;
			}
		}

		protected string Status
		{
			get
			{
				string nonEncoded = LocalizedStrings.GetNonEncoded(-27287708);
				switch (this.task.Status)
				{
				case TaskStatus.InProgress:
					nonEncoded = LocalizedStrings.GetNonEncoded(558434074);
					break;
				case TaskStatus.Completed:
					nonEncoded = LocalizedStrings.GetNonEncoded(604411353);
					break;
				case TaskStatus.WaitingOnOthers:
					nonEncoded = LocalizedStrings.GetNonEncoded(1796266637);
					break;
				case TaskStatus.Deferred:
					nonEncoded = LocalizedStrings.GetNonEncoded(-341200625);
					break;
				}
				return nonEncoded;
			}
		}

		protected string Priority
		{
			get
			{
				string nonEncoded = LocalizedStrings.GetNonEncoded(1690472495);
				object obj = this.task.TryGetProperty(ItemSchema.Importance);
				if (obj is Importance)
				{
					switch ((Importance)obj)
					{
					case Importance.Low:
						nonEncoded = LocalizedStrings.GetNonEncoded(1502599728);
						break;
					case Importance.High:
						nonEncoded = LocalizedStrings.GetNonEncoded(-77932258);
						break;
					}
				}
				return nonEncoded;
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

		protected bool ReminderIsSet
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

		protected bool HasAttachments
		{
			get
			{
				return this.task.AttachmentCollection != null && this.task.AttachmentCollection.Count > 0;
			}
		}

		protected Infobar Infobar
		{
			get
			{
				return this.infobar;
			}
		}

		protected bool IsAssignedTask
		{
			get
			{
				return TaskUtilities.IsAssignedTask(this.task);
			}
		}

		protected static int StoreObjectTypeTask
		{
			get
			{
				return 19;
			}
		}

		private const string StateAssigned = "Assigned";

		private const string ActionOpen = "Open";

		private const string ActionPreview = "Preview";

		private Task task;

		private Infobar infobar = new Infobar("divErr", "infoBarRO");

		private ArrayList attachmentWellRenderObjects;

		private bool shouldRenderAttachmentWell;

		private ExDateTime reminderDate = ExDateTime.MinValue;
	}
}
