using System;
using Microsoft.Exchange.Clients.Common;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	[OwaEventNamespace("EditTask")]
	[OwaEventSegmentation(Feature.Tasks)]
	internal sealed class EditTaskEventHandler : RecurringItemEventHandler
	{
		public static void Register()
		{
			OwaEventRegistry.RegisterEnum(typeof(RecurrenceRangeType));
			OwaEventRegistry.RegisterHandler(typeof(EditTaskEventHandler));
		}

		[OwaEventVerb(OwaEventVerb.Get)]
		[OwaEvent("LRD")]
		public void LoadRecurrenceDialog()
		{
			this.HttpContext.Server.Execute("forms/premium/taskrecurrence.aspx", this.Writer);
		}

		[OwaEventParameter("RcrM", typeof(int), false, true)]
		[OwaEventParameter("rd", typeof(ExDateTime), false, true)]
		[OwaEventParameter("aw", typeof(int), false, true)]
		[OwaEventParameter("mi", typeof(string), false, true)]
		[OwaEventParameter("bl", typeof(string), false, true)]
		[OwaEventParameter("rs", typeof(bool), false, true)]
		[OwaEventParameter("nt", typeof(string), false, true)]
		[OwaEventParameter("RcrT", typeof(int), false, true)]
		[OwaEventParameter("RcrI", typeof(int), false, true)]
		[OwaEventParameter("RgrI", typeof(int), false, true)]
		[OwaEventParameter("RcrDys", typeof(int), false, true)]
		[OwaEventParameter("RcrDy", typeof(int), false, true)]
		[OwaEventParameter("Id", typeof(OwaStoreObjectId), false, true)]
		[OwaEventParameter("RcrO", typeof(int), false, true)]
		[OwaEventParameter("RcrRngT", typeof(RecurrenceRangeType), false, true)]
		[OwaEventParameter("RcrRngS", typeof(ExDateTime), false, true)]
		[OwaEventParameter("RcrRngO", typeof(int), false, true)]
		[OwaEventParameter("RcrRngE", typeof(ExDateTime), false, true)]
		[OwaEvent("Save")]
		[OwaEventParameter("co", typeof(string), false, true)]
		[OwaEventParameter("ps", typeof(bool), false, true)]
		[OwaEventParameter("tw", typeof(int), false, true)]
		[OwaEventParameter("CK", typeof(string), false, true)]
		[OwaEventParameter("fId", typeof(OwaStoreObjectId), false, true)]
		[OwaEventParameter("subj", typeof(string), false, true)]
		[OwaEventParameter("sd", typeof(ExDateTime), false, true)]
		[OwaEventParameter("dd", typeof(ExDateTime), false, true)]
		[OwaEventParameter("dc", typeof(ExDateTime), false, true)]
		[OwaEventParameter("st", typeof(TaskStatus), false, true)]
		[OwaEventParameter("pri", typeof(Importance), false, true)]
		[OwaEventParameter("pc", typeof(int), false, true)]
		public void Save()
		{
			ExTraceGlobals.TasksCallTracer.TraceDebug((long)this.GetHashCode(), "EditTaskEventHandler.Save");
			bool flag = base.IsParameterSet("Id");
			bool flag2 = false;
			bool flag3 = false;
			ExDateTime? exDateTime = null;
			Task task = this.GetTask(new PropertyDefinition[0]);
			try
			{
				if (!base.IsParameterSet("Id"))
				{
					OwaStoreObjectId owaStoreObjectId = (OwaStoreObjectId)base.GetParameter("fId");
					if (owaStoreObjectId != null && owaStoreObjectId.IsOtherMailbox)
					{
						ADSessionSettings adSettings = Utilities.CreateScopedADSessionSettings(base.UserContext.LogonIdentity.DomainName);
						ExchangePrincipal exchangePrincipal = ExchangePrincipal.FromLegacyDN(adSettings, owaStoreObjectId.MailboxOwnerLegacyDN);
						task[TaskSchema.TaskOwner] = exchangePrincipal.MailboxInfo.DisplayName;
					}
					else
					{
						task[TaskSchema.TaskOwner] = base.UserContext.ExchangePrincipal.MailboxInfo.DisplayName;
					}
				}
				if (base.IsParameterSet("subj"))
				{
					task.Subject = (string)base.GetParameter("subj");
				}
				if (base.IsParameterSet("sd"))
				{
					task.StartDate = this.GetDateValue("sd");
				}
				if (base.IsParameterSet("dd"))
				{
					task.DueDate = this.GetDateValue("dd");
				}
				if (base.IsParameterSet("dc"))
				{
					exDateTime = this.GetDateValue("dc");
					if (exDateTime != null)
					{
						flag2 = true;
					}
				}
				if (base.IsParameterSet("st"))
				{
					TaskStatus taskStatus = (TaskStatus)base.GetParameter("st");
					if (taskStatus == TaskStatus.Completed)
					{
						flag2 = true;
					}
					else
					{
						TaskUtilities.SetIncomplete(task, taskStatus);
					}
				}
				if (base.IsParameterSet("pri"))
				{
					task[ItemSchema.Importance] = (Importance)base.GetParameter("pri");
				}
				if (base.IsParameterSet("pc"))
				{
					double num = (double)((int)base.GetParameter("pc")) / 100.0;
					if (!flag2 || num != 1.0)
					{
						if (num >= 0.0 && num < 1.0)
						{
							task.PercentComplete = num;
						}
						else if (num == 1.0)
						{
							flag2 = true;
						}
					}
				}
				if (base.IsParameterSet("rs"))
				{
					bool flag4 = (bool)base.GetParameter("rs");
					task[ItemSchema.ReminderIsSet] = flag4;
					if (flag4 && base.IsParameterSet("rd"))
					{
						ExDateTime? dateValue = this.GetDateValue("rd");
						if (dateValue != null)
						{
							task[ItemSchema.ReminderDueBy] = dateValue.Value;
						}
					}
				}
				if (base.IsParameterSet("ps"))
				{
					task[ItemSchema.Sensitivity] = (((bool)base.GetParameter("ps")) ? Sensitivity.Private : Sensitivity.Normal);
				}
				if (base.IsParameterSet("tw"))
				{
					int num2 = (int)base.GetParameter("tw");
					if (num2 < 0 || num2 > 1525252319)
					{
						throw new OwaInvalidRequestException(LocalizedStrings.GetNonEncoded(-1310086288));
					}
					task[TaskSchema.TotalWork] = num2;
				}
				if (base.IsParameterSet("aw"))
				{
					int num3 = (int)base.GetParameter("aw");
					if (num3 < 0 || num3 > 1525252319)
					{
						throw new OwaInvalidRequestException(LocalizedStrings.GetNonEncoded(210380742));
					}
					task[TaskSchema.ActualWork] = num3;
				}
				if (base.IsParameterSet("mi"))
				{
					task[TaskSchema.Mileage] = (string)base.GetParameter("mi");
				}
				if (base.IsParameterSet("bl"))
				{
					task[TaskSchema.BillingInformation] = (string)base.GetParameter("bl");
				}
				if (base.IsParameterSet("co"))
				{
					string text = (string)base.GetParameter("co");
					string[] value;
					if (string.IsNullOrEmpty(text))
					{
						value = new string[0];
					}
					else
					{
						value = new string[]
						{
							text
						};
					}
					task[TaskSchema.Companies] = value;
				}
				if (base.IsParameterSet("nt"))
				{
					string text2 = (string)base.GetParameter("nt");
					if (text2 != null)
					{
						BodyConversionUtilities.SetBody(task, text2, Markup.PlainText, base.UserContext);
					}
				}
				if (base.IsParameterSet("RcrT"))
				{
					Recurrence recurrence = base.CreateRecurrenceFromRequest();
					if ((recurrence != null || task.Recurrence != null) && (recurrence == null || task.Recurrence == null || !recurrence.Equals(task.Recurrence)))
					{
						task.Recurrence = recurrence;
						flag3 = true;
					}
				}
				if (flag2 && exDateTime == null)
				{
					if (task.CompleteDate == null)
					{
						exDateTime = new ExDateTime?(DateTimeUtilities.GetLocalTime());
					}
					else
					{
						exDateTime = new ExDateTime?(task.CompleteDate.Value);
					}
				}
				if (!flag3 && flag2)
				{
					task.SetStatusCompleted(exDateTime.Value);
				}
				Utilities.SaveItem(task, flag);
				task.Load();
				if (flag3 && flag2)
				{
					OwaStoreObjectId owaStoreObjectId2 = OwaStoreObjectId.CreateFromStoreObject(task);
					string changeKey = task.Id.ChangeKeyAsBase64String();
					task.Dispose();
					task = Utilities.GetItem<Task>(base.UserContext, owaStoreObjectId2, changeKey, TaskUtilities.TaskPrefetchProperties);
					task.SetStatusCompleted(exDateTime.Value);
					Utilities.SaveItem(task);
					task.Load();
				}
				if (!flag)
				{
					OwaStoreObjectId owaStoreObjectId3 = OwaStoreObjectId.CreateFromStoreObject(task);
					if (ExTraceGlobals.TasksDataTracer.IsTraceEnabled(TraceType.DebugTrace))
					{
						ExTraceGlobals.TasksDataTracer.TraceDebug<string>((long)this.GetHashCode(), "New task item ID is '{0}'", owaStoreObjectId3.ToBase64String());
					}
					this.Writer.Write("<div id=itemId>");
					this.Writer.Write(owaStoreObjectId3.ToBase64String());
					this.Writer.Write("</div>");
				}
				this.Writer.Write("<div id=ck>");
				this.Writer.Write(task.Id.ChangeKeyAsBase64String());
				this.Writer.Write("</div>");
				base.MoveItemToDestinationFolderIfInScratchPad(task);
			}
			finally
			{
				task.Dispose();
			}
		}

		[OwaEventParameter("dc", typeof(ExDateTime))]
		[OwaEventParameter("dd", typeof(ExDateTime))]
		[OwaEvent("GetDueBy")]
		public void GetDueBy()
		{
			if (this.GetDateValue("dc") == null)
			{
				SanitizedHtmlString dueByString = TaskUtilities.GetDueByString(this.GetDateValue("dd"));
				if (dueByString != null)
				{
					this.SanitizingWriter.Write(dueByString);
				}
			}
		}

		[OwaEventParameter("RcrM", typeof(int), false, true)]
		[OwaEventParameter("RcrDys", typeof(int), false, true)]
		[OwaEventParameter("RcrO", typeof(int), false, true)]
		[OwaEvent("GenerateWhen")]
		[OwaEventParameter("RgrI", typeof(int), false, true)]
		[OwaEventParameter("RcrDy", typeof(int), false, true)]
		[OwaEventParameter("RcrT", typeof(int), false, true)]
		[OwaEventParameter("RcrI", typeof(int), false, true)]
		[OwaEventParameter("RcrRngT", typeof(RecurrenceRangeType), false, true)]
		[OwaEventParameter("RcrRngS", typeof(ExDateTime), false, true)]
		[OwaEventParameter("RcrRngO", typeof(int), false, true)]
		[OwaEventParameter("RcrRngE", typeof(ExDateTime), false, true)]
		public void GenerateWhen()
		{
			this.Writer.Write(TaskUtilities.GenerateWhen(base.UserContext, base.CreateRecurrenceFromRequest()));
		}

		[OwaEvent("DetailVisible")]
		public void PersistDetailsState()
		{
			ExTraceGlobals.TasksCallTracer.TraceDebug((long)this.GetHashCode(), "EditTaskEventHandler.PersistDetailsState");
			if (!base.UserContext.IsWebPartRequest)
			{
				base.UserContext.UserOptions.IsTaskDetailsVisible = !base.UserContext.UserOptions.IsTaskDetailsVisible;
				base.UserContext.UserOptions.CommitChanges();
			}
		}

		[OwaEventParameter("CK", typeof(string), false, true)]
		[OwaEventParameter("fId", typeof(OwaStoreObjectId), false, true)]
		[OwaEventParameter("rs", typeof(bool), false, true)]
		[OwaEventParameter("Id", typeof(OwaStoreObjectId))]
		[OwaEventParameter("rd", typeof(ExDateTime), false, true)]
		[OwaEventParameter("ps", typeof(bool), false, true)]
		[OwaEvent("SaveReadForm")]
		public void SaveReadForm()
		{
			ExTraceGlobals.TasksCallTracer.TraceDebug((long)this.GetHashCode(), "EditTaskEventHandler.SaveReadForm");
			using (Task task = this.GetTask(new PropertyDefinition[0]))
			{
				if (base.IsParameterSet("rs"))
				{
					bool flag = (bool)base.GetParameter("rs");
					task[ItemSchema.ReminderIsSet] = flag;
					if (flag && base.IsParameterSet("rd"))
					{
						task[ItemSchema.ReminderDueBy] = (ExDateTime)base.GetParameter("rd");
					}
				}
				if (base.IsParameterSet("ps"))
				{
					task[ItemSchema.Sensitivity] = (((bool)base.GetParameter("ps")) ? Sensitivity.Private : Sensitivity.Normal);
				}
				Utilities.SaveItem(task, true);
				task.Load();
				this.Writer.Write("<div id=ck>");
				this.Writer.Write(task.Id.ChangeKeyAsBase64String());
				this.Writer.Write("</div>");
			}
		}

		private Task GetTask(params PropertyDefinition[] prefetchProperties)
		{
			Task result;
			if (base.IsParameterSet("Id"))
			{
				result = base.GetRequestItem<Task>(prefetchProperties);
			}
			else
			{
				ExTraceGlobals.TasksTracer.TraceDebug((long)this.GetHashCode(), "ItemId is null. Creating new task item.");
				OwaStoreObjectId folderId = (OwaStoreObjectId)base.GetParameter("fId");
				result = Utilities.CreateItem<Task>(folderId);
				if (Globals.ArePerfCountersEnabled)
				{
					OwaSingleCounters.ItemsCreated.Increment();
				}
			}
			return result;
		}

		private ExDateTime? GetDateValue(string dateId)
		{
			ExDateTime exDateTime = (ExDateTime)base.GetParameter(dateId);
			if (exDateTime != ExDateTime.MinValue && exDateTime.Year != 1901)
			{
				return new ExDateTime?(exDateTime);
			}
			return null;
		}

		public const string EventNamespace = "EditTask";

		public const string MethodDetailsState = "DetailVisible";

		public const string MethodGetDueBy = "GetDueBy";

		public const string MethodGenerateWhen = "GenerateWhen";

		public const string MethodSaveReadForm = "SaveReadForm";

		public const string MethodLoadRecurrenceDialog = "LRD";

		public const string SubjectId = "subj";

		public const string StartDateId = "sd";

		public const string DueDateId = "dd";

		public const string DateCompletedId = "dc";

		public const string StatusId = "st";

		public const string PriorityId = "pri";

		public const string PercentCompleteId = "pc";

		public const string ReminderSetId = "rs";

		public const string ReminderDateId = "rd";

		public const string PrivateSetId = "ps";

		public const string CategoriesId = "cat";

		public const string AttachmentsId = "att";

		public const string TotalWorkId = "tw";

		public const string ActualWorkId = "aw";

		public const string MileageId = "mi";

		public const string BillingId = "bl";

		public const string CompaniesId = "co";

		public const string NotesId = "nt";
	}
}
