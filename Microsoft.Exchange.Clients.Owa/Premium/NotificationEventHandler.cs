using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	[OwaEventNamespace("Notify")]
	[OwaEventSegmentation(Feature.Notifications)]
	internal sealed class NotificationEventHandler : OwaEventHandlerBase
	{
		public static void Register()
		{
			OwaEventRegistry.RegisterHandler(typeof(NotificationEventHandler));
		}

		[OwaEvent("Poll")]
		[OwaEventParameter("LRT", typeof(ExDateTime), false, true)]
		public void PollAndProcess()
		{
			ExTraceGlobals.OehCallTracer.TraceDebug((long)this.GetHashCode(), "NotificationEventHandler.PollAndProcess");
			this.ProcessOwaConditionAdvisors();
			this.ProcessOwaFolderCountAdvisor();
			this.ProcessOwaLastEventAdvisor();
			long num = Globals.ApplicationTime - base.UserContext.LastUserRequestTime;
			int num2 = 1;
			long num3 = (long)(base.UserContext.Configuration.NotificationInterval * 1000);
			if (num >= num3 * 16L)
			{
				if (num < num3 * 64L)
				{
					num2 = 3;
				}
				else
				{
					num2 = 5;
				}
			}
			long val = Math.Max(base.UserContext.Timeout - 60000L, 30000L);
			long num4 = Math.Min((long)num2 * num3, val);
			this.Writer.Write("setPllInt(" + num4 + "); ");
			if (num2 == 1)
			{
				long num5 = Globals.ApplicationTime - base.UserContext.LastQuotaUpdateTime;
				if ((base.UserContext.IsQuotaAboveWarning && num5 >= 900000L) || num5 >= 1800000L)
				{
					this.Writer.Write("updateMailboxUsage(\"");
					using (StringWriter stringWriter = new StringWriter())
					{
						RenderingUtilities.RenderMailboxQuota(stringWriter, base.UserContext);
						Utilities.JavascriptEncode(stringWriter.ToString(), this.Writer);
					}
					this.Writer.Write("\"); ");
				}
			}
		}

		[OwaEventParameter("LRT", typeof(ExDateTime), false, true)]
		[OwaEvent("ReminderPoll")]
		public void ReminderPollAndProcess()
		{
			ExTraceGlobals.OehCallTracer.TraceDebug((long)this.GetHashCode(), "NotificationEventHandler.ReminderPollAndProcess");
			if (base.UserContext.IsPushNotificationsEnabled)
			{
				OwaMapiNotificationHandler.ProcessReminders(base.UserContext, this.Writer);
				this.Writer.Write("setRmPllInt(" + 28800000L + "); ");
			}
		}

		[OwaEvent("DeleteCA")]
		[OwaEventParameter("sId", typeof(OwaStoreObjectId))]
		public void DeleteOwaConditionAdvisor()
		{
			ExTraceGlobals.OehCallTracer.TraceDebug((long)this.GetHashCode(), "NotificationEventHandler.DeleteOwaConditionAdvisor");
			if (base.UserContext.IsPushNotificationsEnabled)
			{
				base.UserContext.MapiNotificationManager.UnsubscribeFolderChanges((OwaStoreObjectId)base.GetParameter("sId"), null);
			}
			if (base.UserContext.IsPullNotificationsEnabled)
			{
				base.UserContext.NotificationManager.DeleteOwaConditionAdvisor((OwaStoreObjectId)base.GetParameter("sId"));
			}
		}

		[OwaEventParameter("sId", typeof(OwaStoreObjectId))]
		[OwaEvent("Tickle")]
		public void TickleXTC()
		{
			OwaStoreObjectId owaStoreObjectId = base.GetParameter("sId") as OwaStoreObjectId;
			if (owaStoreObjectId == null)
			{
				throw new ArgumentException("sId is not a valid OwaStoreObjectId");
			}
			MailboxSession mailboxSession = owaStoreObjectId.GetSession(base.UserContext) as MailboxSession;
			if (mailboxSession == null)
			{
				throw new OwaInvalidRequestException("The provided Id does not have a valid mailboxSession");
			}
			if (!mailboxSession.IsRemote)
			{
				throw new OwaInvalidRequestException("The mailbox session for the provided Id is not a remote mailbox");
			}
			mailboxSession.CheckForNotifications();
		}

		private void ProcessOwaLastEventAdvisor()
		{
			Event @event = null;
			Event event2 = null;
			Event event3 = null;
			bool flag = false;
			OwaLastEventAdvisor lastEventAdvisor = base.UserContext.NotificationManager.LastEventAdvisor;
			if (lastEventAdvisor != null)
			{
				Event lastEvent = lastEventAdvisor.GetLastEvent(base.UserContext);
				if (lastEvent != null)
				{
					if (ObjectClass.IsOfClass(lastEvent.ClassName, "IPM.Note.Microsoft.Voicemail.UM.CA"))
					{
						event2 = lastEvent;
					}
					else if (ObjectClass.IsOfClass(lastEvent.ClassName, "IPM.Note.Microsoft.Fax.CA"))
					{
						event3 = lastEvent;
					}
					else
					{
						@event = lastEvent;
					}
				}
				if (event3 != null)
				{
					flag = true;
					if ((base.UserContext.UserOptions.NewItemNotify & NewNotification.FaxToast) == NewNotification.FaxToast)
					{
						this.Writer.Write("shwNF(1);");
						this.Writer.Write("g_sFId=\"");
						Utilities.JavascriptEncode(event3.ObjectId.ToBase64String(), this.Writer);
						this.Writer.Write("\";");
						flag = this.BindItemAndShowDialog(event3, "lnkNwFx");
					}
				}
				if (event2 != null)
				{
					flag = true;
					if ((base.UserContext.UserOptions.NewItemNotify & NewNotification.VoiceMailToast) == NewNotification.VoiceMailToast)
					{
						this.Writer.Write("shwNVM(1);");
						this.Writer.Write("g_sVMId=\"");
						Utilities.JavascriptEncode(event2.ObjectId.ToBase64String(), this.Writer);
						this.Writer.Write("\";");
						flag = this.BindItemAndShowDialog(event2, "lnkNwVMl");
					}
				}
				if (@event != null)
				{
					flag = true;
					if ((base.UserContext.UserOptions.NewItemNotify & NewNotification.EMailToast) == NewNotification.EMailToast)
					{
						this.Writer.Write("shwNM(1);");
						this.Writer.Write("g_sMId=\"");
						Utilities.JavascriptEncode(@event.ObjectId.ToBase64String(), this.Writer);
						this.Writer.Write("\";");
						flag = this.BindItemAndShowDialog(@event, "lnkNwMl");
					}
				}
				if ((base.UserContext.UserOptions.NewItemNotify & NewNotification.Sound) == NewNotification.Sound && flag)
				{
					this.Writer.Write("plySnd();");
				}
			}
		}

		private bool BindItemAndShowDialog(Event eventItem, string type)
		{
			MessageItem messageItem = null;
			bool result;
			try
			{
				messageItem = Item.BindAsMessage(base.UserContext.MailboxSession, eventItem.ObjectId);
				if (messageItem != null)
				{
					string text = ItemUtility.GetProperty<string>(messageItem, StoreObjectSchema.ItemClass, null);
					if (text == null)
					{
						text = "IPM.Note";
					}
					this.Writer.Write("shwNwItmDlg(\"");
					if (messageItem.From != null && messageItem.From.DisplayName != null)
					{
						Utilities.JavascriptEncode(Utilities.HtmlEncode(messageItem.From.DisplayName), this.Writer);
					}
					this.Writer.Write("\",\"");
					if (messageItem.Subject != null)
					{
						Utilities.JavascriptEncode(Utilities.HtmlEncode(messageItem.Subject), this.Writer);
					}
					this.Writer.Write("\",\"" + type + "\",\"");
					using (StringWriter stringWriter = new StringWriter())
					{
						SmallIconManager.RenderItemIcon(stringWriter, base.UserContext, text, false, "nwItmImg", new string[0]);
						Utilities.JavascriptEncode(stringWriter.ToString(), this.Writer);
					}
					this.Writer.Write("\");");
				}
				result = true;
			}
			catch (ObjectNotFoundException)
			{
				if (type != null)
				{
					if (!(type == "lnkNwMl"))
					{
						if (!(type == "lnkNwVMl"))
						{
							if (type == "lnkNwFx")
							{
								this.Writer.Write("shwNF(0);");
							}
						}
						else
						{
							this.Writer.Write("shwNVM(0);");
						}
					}
					else
					{
						this.Writer.Write("shwNM(0);");
					}
				}
				result = false;
			}
			finally
			{
				if (messageItem != null)
				{
					messageItem.Dispose();
				}
			}
			return result;
		}

		private void ProcessOwaConditionAdvisors()
		{
			Dictionary<OwaStoreObjectId, OwaConditionAdvisor> conditionAdvisorTable = base.UserContext.NotificationManager.ConditionAdvisorTable;
			if (conditionAdvisorTable != null)
			{
				OwaStoreObjectId remindersSearchFolderOwaId = base.UserContext.RemindersSearchFolderOwaId;
				bool flag = false;
				IDictionaryEnumerator dictionaryEnumerator = conditionAdvisorTable.GetEnumerator();
				while (dictionaryEnumerator.MoveNext())
				{
					OwaConditionAdvisor owaConditionAdvisor = (OwaConditionAdvisor)dictionaryEnumerator.Value;
					OwaStoreObjectId folderId = owaConditionAdvisor.FolderId;
					try
					{
						MailboxSession mailboxSession = folderId.GetSession(base.UserContext) as MailboxSession;
						if (mailboxSession != null)
						{
							bool flag2 = owaConditionAdvisor.IsConditionTrue(mailboxSession);
							if (base.UserContext.UserOptions.EnableReminders && !flag && folderId.Equals(remindersSearchFolderOwaId))
							{
								flag = flag2;
							}
							if (flag2 || owaConditionAdvisor.IsRecycled)
							{
								if (!folderId.Equals(remindersSearchFolderOwaId))
								{
									this.Writer.Write("stDrty(\"");
									Utilities.JavascriptEncode(folderId.ToBase64String(), this.Writer);
									this.Writer.Write("\");");
								}
								owaConditionAdvisor.ResetCondition(mailboxSession);
								owaConditionAdvisor.IsRecycled = false;
							}
						}
					}
					catch (ObjectNotFoundException)
					{
					}
				}
				if (base.UserContext.UserOptions.EnableReminders)
				{
					this.ProcessReminders(flag);
				}
			}
		}

		private void ProcessOwaFolderCountAdvisor()
		{
			OwaFolderCountAdvisor folderCountAdvisor = base.UserContext.NotificationManager.FolderCountAdvisor;
			if (folderCountAdvisor != null)
			{
				this.ProcessFolderCountAdvisor(folderCountAdvisor);
			}
			if (base.UserContext.NotificationManager.ArchiveFolderCountAdvisorTable != null)
			{
				this.ProcessArchiveOwaFolderCountAdvisor();
			}
			if (base.UserContext.NotificationManager.DelegateFolderCountAdvisorTable != null)
			{
				this.ProcessDelegateOwaFolderCountAdvisor();
			}
		}

		private void ProcessArchiveOwaFolderCountAdvisor()
		{
			Dictionary<string, OwaFolderCountAdvisor> archiveFolderCountAdvisorTable = base.UserContext.NotificationManager.ArchiveFolderCountAdvisorTable;
			IDictionaryEnumerator dictionaryEnumerator = archiveFolderCountAdvisorTable.GetEnumerator();
			while (dictionaryEnumerator.MoveNext())
			{
				this.ProcessFolderCountAdvisor((OwaFolderCountAdvisor)dictionaryEnumerator.Value);
			}
		}

		private void ProcessDelegateOwaFolderCountAdvisor()
		{
			Dictionary<OwaStoreObjectId, OwaFolderCountAdvisor> delegateFolderCountAdvisorTable = base.UserContext.NotificationManager.DelegateFolderCountAdvisorTable;
			IDictionaryEnumerator dictionaryEnumerator = delegateFolderCountAdvisorTable.GetEnumerator();
			while (dictionaryEnumerator.MoveNext())
			{
				this.ProcessFolderCountAdvisor((OwaFolderCountAdvisor)dictionaryEnumerator.Value);
			}
		}

		private void ProcessFolderCountAdvisor(OwaFolderCountAdvisor folderCountAdvisor)
		{
			if (folderCountAdvisor == null)
			{
				throw new ArgumentNullException("folderCountAdvisor");
			}
			MailboxSession mailboxSession = null;
			if (folderCountAdvisor.FolderId == null)
			{
				if (!folderCountAdvisor.MailboxOwner.MailboxInfo.IsAggregated)
				{
					mailboxSession = base.UserContext.MailboxSession;
				}
				else
				{
					mailboxSession = base.UserContext.GetArchiveMailboxSession(folderCountAdvisor.MailboxOwner);
				}
			}
			else
			{
				try
				{
					mailboxSession = (folderCountAdvisor.FolderId.GetSession(base.UserContext) as MailboxSession);
				}
				catch (ObjectNotFoundException)
				{
					return;
				}
			}
			if (mailboxSession != null)
			{
				Dictionary<StoreObjectId, ItemCountPair> itemCounts = folderCountAdvisor.GetItemCounts(mailboxSession);
				if (itemCounts == null)
				{
					return;
				}
				IDictionaryEnumerator dictionaryEnumerator = itemCounts.GetEnumerator();
				while (dictionaryEnumerator.MoveNext())
				{
					ItemCountPair countPair = (ItemCountPair)dictionaryEnumerator.Value;
					if (countPair.ItemCount != -1L || countPair.UnreadItemCount != -1L)
					{
						OwaStoreObjectId owaStoreObjectId = OwaStoreObjectId.CreateFromSessionFolderId(base.UserContext, mailboxSession, (StoreObjectId)dictionaryEnumerator.Key);
						this.RenderUpdateCountJavascript(owaStoreObjectId, countPair);
						if (owaStoreObjectId.StoreObjectType == StoreObjectType.Folder)
						{
							StoreObjectId folderId = StoreObjectId.FromProviderSpecificId(owaStoreObjectId.StoreObjectId.ProviderLevelItemId, StoreObjectType.OutlookSearchFolder);
							this.RenderUpdateCountJavascript(OwaStoreObjectId.CreateFromSessionFolderId(base.UserContext, mailboxSession, folderId), countPair);
							StoreObjectId folderId2 = StoreObjectId.FromProviderSpecificId(owaStoreObjectId.StoreObjectId.ProviderLevelItemId, StoreObjectType.SearchFolder);
							this.RenderUpdateCountJavascript(OwaStoreObjectId.CreateFromSessionFolderId(base.UserContext, mailboxSession, folderId2), countPair);
						}
					}
				}
				return;
			}
		}

		private void RenderUpdateCountJavascript(OwaStoreObjectId folderId, ItemCountPair countPair)
		{
			this.Writer.Write("updCnt(\"");
			Utilities.JavascriptEncode(folderId.ToBase64String(), this.Writer);
			this.Writer.Write("\",");
			this.Writer.Write(countPair.ItemCount);
			this.Writer.Write(",");
			this.Writer.Write(countPair.UnreadItemCount);
			this.Writer.Write(");");
		}

		private void ProcessReminders(bool isReminderModified)
		{
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = base.IsParameterSet("LRT");
			ExDateTime localTime = DateTimeUtilities.GetLocalTime();
			if (!base.UserContext.IsRemindersSessionStarted || !flag4)
			{
				base.UserContext.IsRemindersSessionStarted = true;
				flag2 = true;
			}
			if (flag4)
			{
				ExDateTime dt = (ExDateTime)base.GetParameter("LRT");
				if ((int)(ExDateTime.UtcNow - dt).TotalHours > 12)
				{
					flag = true;
				}
			}
			int num = (int)localTime.Bias.TotalMinutes;
			if (num != base.UserContext.RemindersTimeZoneOffset)
			{
				base.UserContext.RemindersTimeZoneOffset = num;
				flag3 = true;
			}
			if (flag || flag2 || isReminderModified)
			{
				this.Writer.Write("rmNotfy(");
				this.Writer.Write(num);
				this.Writer.Write(", 1, \"");
				Utilities.JavascriptEncode(DateTimeUtilities.GetJavascriptDate(localTime), this.Writer);
				this.Writer.Write("\", \"");
				bool reminderItems = RemindersRenderingUtilities.GetReminderItems(localTime, this.Writer);
				if (reminderItems)
				{
					this.Writer.Write("\", \"");
					Utilities.JavascriptEncode(LocalizedStrings.GetHtmlEncoded(-1707229168), this.Writer);
				}
				this.Writer.Write("\");");
				return;
			}
			if (flag3)
			{
				this.Writer.Write("rmNotfy(");
				this.Writer.Write(num);
				this.Writer.Write(", 0, \"\");");
			}
		}

		public const string EventNamespace = "Notify";

		public const string MethodPoll = "Poll";

		public const string MethodReminderPoll = "ReminderPoll";

		public const string MethodDeleteOwaConditionAdvisor = "DeleteCA";

		public const string MethodTickleXTC = "Tickle";

		public const string SourceId = "sId";

		public const string LastReminderTime = "LRT";

		public const int BelowWarningQuotaUpdateInterval = 1800000;

		public const int AboveWarningQuotaUpdateInterval = 900000;
	}
}
