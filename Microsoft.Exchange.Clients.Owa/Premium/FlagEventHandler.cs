using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Premium.Controls;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	[OwaEventNamespace("Flg")]
	internal sealed class FlagEventHandler : OwaEventHandlerBase
	{
		public static ExDateTime? SetFlag(Item item, FlagAction flagAction, ExDateTime? dueDate)
		{
			UserContext userContext = UserContextManager.GetUserContext();
			ExDateTime? exDateTime = null;
			ExDateTime date = DateTimeUtilities.GetLocalTime().Date;
			int dayOfWeek = DateTimeUtilities.GetDayOfWeek(userContext, date);
			item.TryGetProperty(ItemSchema.FlagStatus);
			FlagStatus flagStatus = ItemUtility.GetProperty<FlagStatus>(item, ItemSchema.FlagStatus, FlagStatus.NotFlagged);
			Task task = item as Task;
			if (task != null && task.IsComplete)
			{
				flagStatus = FlagStatus.Complete;
			}
			if (flagAction == FlagAction.Default && flagStatus != FlagStatus.Complete)
			{
				flagAction = userContext.UserOptions.FlagAction;
			}
			switch (flagAction)
			{
			case FlagAction.Default:
				exDateTime = ItemUtility.GetProperty<ExDateTime?>(item, ItemSchema.UtcStartDate, null);
				dueDate = ItemUtility.GetProperty<ExDateTime?>(item, ItemSchema.UtcDueDate, null);
				break;
			case FlagAction.Today:
				dueDate = new ExDateTime?(date);
				dueDate = (exDateTime = dueDate);
				break;
			case FlagAction.Tomorrow:
				dueDate = new ExDateTime?(date.IncrementDays(1));
				dueDate = (exDateTime = dueDate);
				break;
			case FlagAction.ThisWeek:
			{
				int num = 7 - dayOfWeek;
				int value = 2;
				if (0 < num && num <= 2)
				{
					value = num - 1;
				}
				exDateTime = new ExDateTime?(date.IncrementDays(value));
				while (date < exDateTime && !DateTimeUtilities.IsWorkingDay(exDateTime.Value, userContext.WorkingHours.WorkDays))
				{
					exDateTime = new ExDateTime?(exDateTime.Value.IncrementDays(-1));
				}
				if (num == 0)
				{
					num = 7;
				}
				dueDate = new ExDateTime?(date.IncrementDays(num - 1));
				while (exDateTime < dueDate)
				{
					if (DateTimeUtilities.IsWorkingDay(dueDate.Value, userContext.WorkingHours.WorkDays))
					{
						break;
					}
					dueDate = new ExDateTime?(dueDate.Value.IncrementDays(-1));
				}
				break;
			}
			case FlagAction.NextWeek:
			{
				int num2 = 7 - dayOfWeek;
				if (num2 == 0)
				{
					num2 = 7;
				}
				exDateTime = new ExDateTime?(date.IncrementDays(num2));
				int num3 = 0;
				while (num3 < 7 && !DateTimeUtilities.IsWorkingDay(exDateTime.Value, userContext.WorkingHours.WorkDays))
				{
					exDateTime = new ExDateTime?(exDateTime.Value.IncrementDays(1));
					num3++;
				}
				int num4 = 7 - DateTimeUtilities.GetDayOfWeek(userContext, exDateTime.Value);
				if (num4 == 7)
				{
					num4 = 6;
				}
				dueDate = new ExDateTime?(exDateTime.Value.IncrementDays(num4));
				while (exDateTime < dueDate)
				{
					if (DateTimeUtilities.IsWorkingDay(dueDate.Value, userContext.WorkingHours.WorkDays))
					{
						break;
					}
					dueDate = new ExDateTime?(dueDate.Value.IncrementDays(-1));
				}
				break;
			}
			case FlagAction.SpecificDate:
				exDateTime = dueDate;
				break;
			}
			if (task != null)
			{
				task.StartDate = exDateTime;
				task.DueDate = dueDate;
				if (task.Status == TaskStatus.Completed)
				{
					TaskUtilities.SetIncomplete(task, TaskStatus.InProgress);
				}
			}
			else
			{
				item.SetFlag(LocalizedStrings.GetNonEncoded(-1950847676), exDateTime, dueDate);
			}
			return dueDate;
		}

		public static void ClearFlag(Item item)
		{
			item.ClearFlag();
			if (item.Reminder != null)
			{
				item.Reminder.IsSet = false;
			}
		}

		public static void FlagComplete(Item item)
		{
			Task task = item as Task;
			if (task != null)
			{
				task.SetStatusCompleted(DateTimeUtilities.GetLocalTime());
				return;
			}
			item.CompleteFlag(new ExDateTime?(DateTimeUtilities.GetLocalTime()));
			if (item.Reminder != null)
			{
				item.Reminder.IsSet = false;
			}
		}

		public static FlagStatus GetFlagStatus(Item item)
		{
			Task task = item as Task;
			if (task == null)
			{
				return ItemUtility.GetProperty<FlagStatus>(item, ItemSchema.FlagStatus, FlagStatus.NotFlagged);
			}
			if (task.IsComplete)
			{
				return FlagStatus.Complete;
			}
			return FlagStatus.Flagged;
		}

		[OwaEventVerb(OwaEventVerb.Post)]
		[OwaEvent("PFA")]
		[OwaEventParameter("flga", typeof(FlagAction))]
		public void PersistFlagAction()
		{
			if (!base.UserContext.IsWebPartRequest)
			{
				base.UserContext.UserOptions.FlagAction = (FlagAction)base.GetParameter("flga");
				base.UserContext.UserOptions.CommitChanges();
			}
		}

		[OwaEventVerb(OwaEventVerb.Get)]
		[OwaEvent("gtFM")]
		[OwaEventParameter("id", typeof(OwaStoreObjectId), false, true)]
		public void GetFlagMenu()
		{
			ExDateTime defaultDate = DateTimeUtilities.GetLocalTime().Date;
			OwaStoreObjectId owaStoreObjectId = base.GetParameter("id") as OwaStoreObjectId;
			FlagAction flagAction = FlagAction.None;
			if (owaStoreObjectId != null)
			{
				using (Item item = Utilities.GetItem<Item>(base.UserContext, owaStoreObjectId, FlagEventHandler.prefetchProperties))
				{
					ExDateTime property = ItemUtility.GetProperty<ExDateTime>(item, TaskSchema.DueDate, ExDateTime.MinValue);
					if (1601 < property.Year)
					{
						defaultDate = property;
					}
					flagAction = FlagContextMenu.GetFlagActionForItem(base.UserContext, item);
				}
			}
			FlagContextMenu flagContextMenu = new FlagContextMenu(base.UserContext, defaultDate, flagAction);
			flagContextMenu.Render(this.Writer);
		}

		[OwaEventParameter("fldrId", typeof(OwaStoreObjectId), false, true)]
		[OwaEventParameter("id", typeof(OwaStoreObjectId), false, true)]
		[OwaEventVerb(OwaEventVerb.Get)]
		[OwaEvent("gtFD")]
		public void GetFlagDialog()
		{
			ExDateTime date = DateTimeUtilities.GetLocalTime().Date;
			ExDateTime exDateTime = ExDateTime.MinValue;
			ExDateTime exDateTime2 = ExDateTime.MinValue;
			ExDateTime exDateTime3 = ExDateTime.MinValue;
			bool flag = false;
			FlagStatus flagStatus = FlagStatus.NotFlagged;
			bool flag2 = true;
			OwaStoreObjectId owaStoreObjectId = base.GetParameter("fldrId") as OwaStoreObjectId;
			OwaStoreObjectId latestFlaggedOrNormalItem = this.GetLatestFlaggedOrNormalItem();
			if (latestFlaggedOrNormalItem != null)
			{
				using (Item item = Utilities.GetItem<Item>(base.UserContext, latestFlaggedOrNormalItem, FlagEventHandler.prefetchProperties))
				{
					flag2 = (!Utilities.IsPublic(item) && !(item is MeetingMessage));
					flagStatus = FlagEventHandler.GetFlagStatus(item);
					exDateTime = ItemUtility.GetProperty<ExDateTime>(item, TaskSchema.StartDate, ExDateTime.MinValue);
					if (exDateTime.Year <= 1601)
					{
						exDateTime = ExDateTime.MinValue;
					}
					exDateTime2 = ItemUtility.GetProperty<ExDateTime>(item, TaskSchema.DueDate, ExDateTime.MinValue);
					if (exDateTime2.Year <= 1601)
					{
						exDateTime2 = ExDateTime.MinValue;
					}
					flag = ItemUtility.GetProperty<bool>(item, ItemSchema.ReminderIsSet, false);
					if (flag)
					{
						exDateTime3 = ItemUtility.GetProperty<ExDateTime>(item, ItemSchema.ReminderDueBy, ExDateTime.MinValue);
						if (exDateTime3.Year <= 1601)
						{
							exDateTime3 = ExDateTime.MinValue;
						}
					}
				}
			}
			if (owaStoreObjectId != null)
			{
				flag2 = !owaStoreObjectId.IsPublic;
			}
			if (flagStatus == FlagStatus.NotFlagged)
			{
				if (exDateTime2 == ExDateTime.MinValue)
				{
					exDateTime2 = DateTimeUtilities.GetLocalTime().Date;
				}
				if (exDateTime == ExDateTime.MinValue || exDateTime > exDateTime2)
				{
					exDateTime = exDateTime2;
				}
			}
			if (exDateTime3 == ExDateTime.MinValue)
			{
				if (exDateTime == ExDateTime.MinValue || exDateTime == date)
				{
					exDateTime3 = date;
					int num = base.UserContext.WorkingHours.GetWorkDayEndTime(exDateTime3) - 60;
					exDateTime3 = exDateTime3.AddMinutes((double)num);
				}
				else
				{
					exDateTime3 = exDateTime;
					exDateTime3 = exDateTime3.AddMinutes((double)base.UserContext.WorkingHours.GetWorkDayStartTime(exDateTime3));
				}
			}
			RenderingUtilities.RenderStringVariable(this.Writer, "sDT", LocalizedStrings.GetNonEncoded(-941242639));
			StringBuilder stringBuilder = new StringBuilder();
			StringWriter stringWriter = new StringWriter(stringBuilder);
			stringWriter.Write("<table class=flgDtRm><col><col class=pkr><tr><td nowrap>");
			stringWriter.Write(LocalizedStrings.GetHtmlEncoded(1089701318));
			stringWriter.Write("</td><td nowrap>");
			DatePickerDropDownCombo.RenderDatePicker(stringWriter, "tblFlgSD", exDateTime, date, DatePicker.Features.TodayButton | DatePicker.Features.NoneButton);
			stringWriter.Write("</td></tr>");
			stringWriter.Write("<tr><td>");
			stringWriter.Write(LocalizedStrings.GetHtmlEncoded(1012104992));
			stringWriter.Write("</td><td>");
			DatePickerDropDownCombo.RenderDatePicker(stringWriter, "tblFlgDD", exDateTime2, date, DatePicker.Features.TodayButton | DatePicker.Features.NoneButton);
			stringWriter.Write("</td></tr>");
			if (flag2)
			{
				stringWriter.Write("<tr><td colspan=2 nowrap><input type=checkbox id=chkRM");
				if (flag)
				{
					stringWriter.Write(" checked");
				}
				stringWriter.Write("><label for=chkRM> ");
				stringWriter.Write(LocalizedStrings.GetHtmlEncoded(-1024614879));
				stringWriter.Write("</label></td></tr>");
				stringWriter.Write("<tr><td nowrap class=rmd>");
				stringWriter.Write(LocalizedStrings.GetHtmlEncoded(-1966747526));
				stringWriter.Write("</td><td>");
				DatePickerDropDownCombo.RenderDatePicker(stringWriter, "tblFlgRD", exDateTime3);
				stringWriter.Write("</td></tr>");
				stringWriter.Write("<tr><td nowrap class=rmd>");
				stringWriter.Write(LocalizedStrings.GetHtmlEncoded(-837446999));
				stringWriter.Write("</td><td>");
				TimeDropDownList.RenderTimePicker(stringWriter, exDateTime3, "divFlgRT");
				stringWriter.Write("</td></tr>");
			}
			stringWriter.Write("</table>");
			stringWriter.Close();
			RenderingUtilities.RenderStringVariable(this.Writer, "sD", stringBuilder.ToString());
		}

		[OwaEventParameter("flga", typeof(FlagAction))]
		[OwaEventParameter("fldrId", typeof(OwaStoreObjectId), false, true)]
		[OwaEventParameter("ddt", typeof(ExDateTime), false, true)]
		[OwaEventParameter("id", typeof(OwaStoreObjectId), false, true)]
		[OwaEvent("flgItm")]
		[OwaEventParameter("ck", typeof(string), false, true)]
		[OwaEventParameter("typ", typeof(StoreObjectType), false, true)]
		public void FlagItem()
		{
			FlagAction flagAction = (FlagAction)base.GetParameter("flga");
			ExDateTime? dueDate = null;
			if (flagAction == FlagAction.SpecificDate)
			{
				if (!base.IsParameterSet("ddt"))
				{
					throw new OwaInvalidRequestException("Due date must be provided if specifying a specific due date");
				}
				dueDate = new ExDateTime?((ExDateTime)base.GetParameter("ddt"));
			}
			using (Item item = this.GetItem())
			{
				MessageItem messageItem = item as MessageItem;
				if (messageItem != null && messageItem.IsDraft)
				{
					throw new OwaOperationNotSupportedException(LocalizedStrings.GetNonEncoded(804173896));
				}
				switch (flagAction)
				{
				case FlagAction.MarkComplete:
					FlagEventHandler.FlagComplete(item);
					break;
				case FlagAction.ClearFlag:
					FlagEventHandler.ClearFlag(item);
					break;
				default:
					dueDate = FlagEventHandler.SetFlag(item, flagAction, dueDate);
					break;
				}
				Utilities.SaveItem(item);
				PropertyDefinition[] properties = new PropertyDefinition[]
				{
					ItemSchema.FlagStatus,
					ItemSchema.FlagCompleteTime,
					MessageItemSchema.ReplyTime
				};
				item.Load(properties);
				InfobarMessage flag = InfobarMessageBuilder.GetFlag(item, base.UserContext);
				this.Writer.Write("var sIBMsg = \"");
				if (flag != null)
				{
					StringBuilder stringBuilder = new StringBuilder();
					StringWriter stringWriter = new StringWriter(stringBuilder);
					Infobar.RenderMessage(stringWriter, flag, base.UserContext);
					stringWriter.Close();
					Utilities.JavascriptEncode(stringBuilder.ToString(), this.Writer);
				}
				this.Writer.Write("\";");
				this.Writer.Write("a_sId = \"");
				Utilities.JavascriptEncode(Utilities.GetIdAsString(item), this.Writer);
				this.Writer.Write("\";");
				this.Writer.Write("a_sCK = \"");
				Utilities.JavascriptEncode(item.Id.ChangeKeyAsBase64String(), this.Writer);
				this.Writer.Write("\";");
				this.Writer.Write("var dtDD = ");
				if (dueDate != null)
				{
					this.Writer.Write("new Date(\"");
					this.Writer.Write(DateTimeUtilities.GetJavascriptDate(dueDate.Value));
					this.Writer.Write("\");");
				}
				else
				{
					this.Writer.Write("0;");
				}
			}
		}

		[OwaEventParameter("ddt", typeof(ExDateTime), false, true)]
		[OwaEvent("dtRm")]
		[OwaEventParameter("rem", typeof(ExDateTime), false, true)]
		[OwaEventParameter("id", typeof(OwaStoreObjectId), false, true)]
		[OwaEventParameter("ck", typeof(string), false, true)]
		[OwaEventParameter("typ", typeof(StoreObjectType), false, true)]
		[OwaEventParameter("fldrId", typeof(OwaStoreObjectId), false, true)]
		[OwaEventParameter("sdt", typeof(ExDateTime), false, true)]
		public void SetDateAndReminder()
		{
			using (Item item = this.GetItem())
			{
				MessageItem messageItem = item as MessageItem;
				if (messageItem != null && messageItem.IsDraft)
				{
					this.Writer.Write("var iError = 1;");
					this.Writer.Write("alrt(\"");
					this.Writer.Write(LocalizedStrings.GetJavascriptEncoded(-1537113578));
					this.Writer.Write("\", null, Owa.BUTTON_DIALOG_ICON.WARNING, L_Wrng);");
				}
				else
				{
					this.Writer.Write("var iError = 0;");
					ExDateTime? exDateTime = null;
					ExDateTime? exDateTime2 = null;
					ExDateTime? dueBy = null;
					if (base.IsParameterSet("sdt"))
					{
						exDateTime = new ExDateTime?((ExDateTime)base.GetParameter("sdt"));
					}
					if (base.IsParameterSet("ddt"))
					{
						exDateTime2 = new ExDateTime?((ExDateTime)base.GetParameter("ddt"));
					}
					if (base.IsParameterSet("rem"))
					{
						dueBy = new ExDateTime?((ExDateTime)base.GetParameter("rem"));
					}
					if (exDateTime != null && exDateTime2 == null)
					{
						throw new OwaInvalidRequestException("A due date must be provided if a start date is specified");
					}
					if (exDateTime != null && exDateTime2 != null && exDateTime2 < exDateTime)
					{
						throw new OwaInvalidRequestException("The due date must be on or after the start date");
					}
					Task task = item as Task;
					if (task != null)
					{
						task.StartDate = exDateTime;
						task.DueDate = exDateTime2;
						task.SetStatusInProgress();
					}
					else
					{
						item.SetFlag(LocalizedStrings.GetNonEncoded(-1950847676), exDateTime, exDateTime2);
					}
					if (item.Reminder != null)
					{
						if (dueBy != null)
						{
							item.Reminder.IsSet = true;
							item.Reminder.DueBy = dueBy;
						}
						else
						{
							item.Reminder.IsSet = false;
						}
					}
					Utilities.SaveItem(item);
					item.Load(new PropertyDefinition[]
					{
						ItemSchema.FlagStatus,
						ItemSchema.FlagCompleteTime
					});
					this.Writer.Write("var iFA = ");
					this.Writer.Write((int)FlagContextMenu.GetFlagActionForItem(base.UserContext, item));
					this.Writer.Write(";");
					InfobarMessage flag = InfobarMessageBuilder.GetFlag(item, base.UserContext);
					this.Writer.Write("var sIBMsg = \"");
					if (flag != null)
					{
						StringBuilder stringBuilder = new StringBuilder();
						StringWriter stringWriter = new StringWriter(stringBuilder);
						Infobar.RenderMessage(stringWriter, flag, base.UserContext);
						stringWriter.Close();
						Utilities.JavascriptEncode(stringBuilder.ToString(), this.Writer);
					}
					this.Writer.Write("\";");
					OwaStoreObjectId owaStoreObjectId = base.GetParameter("id") as OwaStoreObjectId;
					this.Writer.Write("sId = \"");
					Utilities.JavascriptEncode((owaStoreObjectId == null) ? Utilities.GetIdAsString(item) : owaStoreObjectId.ToBase64String(), this.Writer);
					this.Writer.Write("\";");
					this.Writer.Write("sCK = \"");
					if (owaStoreObjectId == null || !owaStoreObjectId.IsConversationId)
					{
						Utilities.JavascriptEncode(item.Id.ChangeKeyAsBase64String(), this.Writer);
					}
					this.Writer.Write("\";");
					this.Writer.Write("var dtDD = ");
					if (exDateTime2 != null)
					{
						this.Writer.Write("new Date(\"");
						this.Writer.Write(DateTimeUtilities.GetJavascriptDate(exDateTime2.Value));
						this.Writer.Write("\");");
					}
					else
					{
						this.Writer.Write("0;");
					}
				}
			}
		}

		private Item GetItem()
		{
			OwaStoreObjectId latestFlaggedOrNormalItem = this.GetLatestFlaggedOrNormalItem();
			string text = base.GetParameter("ck") as string;
			Item item;
			if (latestFlaggedOrNormalItem == null)
			{
				if (!base.IsParameterSet("typ"))
				{
					throw new OwaInvalidRequestException();
				}
				StoreObjectType itemType = (StoreObjectType)base.GetParameter("typ");
				item = Utilities.CreateImplicitDraftItem(itemType, base.GetParameter("fldrId") as OwaStoreObjectId);
			}
			else if (text == null)
			{
				item = Utilities.GetItem<Item>(base.UserContext, latestFlaggedOrNormalItem, FlagEventHandler.prefetchProperties);
				item.OpenAsReadWrite();
			}
			else
			{
				item = Utilities.GetItem<Item>(base.UserContext, latestFlaggedOrNormalItem, text, FlagEventHandler.prefetchProperties);
			}
			return item;
		}

		private OwaStoreObjectId GetLatestFlaggedOrNormalItem()
		{
			OwaStoreObjectId owaStoreObjectId = base.GetParameter("id") as OwaStoreObjectId;
			if (owaStoreObjectId == null || !owaStoreObjectId.IsConversationId)
			{
				return owaStoreObjectId;
			}
			MailboxSession mailboxSession = (MailboxSession)owaStoreObjectId.GetSession(base.UserContext);
			Conversation conversation = Conversation.Load(mailboxSession, owaStoreObjectId.ConversationId, base.UserContext.IsIrmEnabled, new PropertyDefinition[]
			{
				ItemSchema.Id,
				StoreObjectSchema.ParentItemId,
				ItemSchema.ReceivedTime,
				ItemSchema.FlagStatus
			});
			conversation.ConversationTree.Sort(ConversationTreeSortOrder.ChronologicalDescending);
			IList<StoreObjectId> flagedItems = ConversationUtilities.GetFlagedItems(mailboxSession, conversation, owaStoreObjectId.ParentFolderId, new FlagStatus[]
			{
				FlagStatus.Flagged
			});
			if (flagedItems.Count > 0)
			{
				return OwaStoreObjectId.CreateFromStoreObjectId(flagedItems[0], owaStoreObjectId);
			}
			StoreObjectId latestMessage = ConversationUtilities.GetLatestMessage(mailboxSession, conversation, owaStoreObjectId.ParentFolderId);
			if (latestMessage != null)
			{
				return OwaStoreObjectId.CreateFromStoreObjectId(latestMessage, owaStoreObjectId);
			}
			return null;
		}

		public const string EventNamespace = "Flg";

		public const string MethodFlagItem = "flgItm";

		public const string MethodSetDateAndReminder = "dtRm";

		public const string MethodGetFlagMenu = "gtFM";

		public const string MethodGetFlagDialog = "gtFD";

		public const string MethodPersistFlagAction = "PFA";

		public const string Id = "id";

		public const string ChangeKey = "ck";

		public const string FolderId = "fldrId";

		public const string FlagActionParameter = "flga";

		public const string ItemType = "typ";

		public const string StartDate = "sdt";

		public const string DueDate = "ddt";

		public const string Reminder = "rem";

		public const string Mode = "md";

		private static readonly StorePropertyDefinition[] prefetchProperties = new StorePropertyDefinition[]
		{
			ItemSchema.FlagStatus,
			ItemSchema.FlagCompleteTime,
			MessageItemSchema.ReplyTime,
			ItemSchema.IsToDoItem,
			ItemSchema.UtcDueDate,
			ItemSchema.UtcStartDate,
			ItemSchema.ReminderDueBy,
			ItemSchema.ReminderIsSet
		};
	}
}
