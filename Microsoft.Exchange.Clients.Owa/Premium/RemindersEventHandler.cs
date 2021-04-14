using System;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	[OwaEventNamespace("RemindersEventHandler")]
	internal sealed class RemindersEventHandler : ItemEventHandler
	{
		public static void Register()
		{
			OwaEventRegistry.RegisterHandler(typeof(RemindersEventHandler));
		}

		[OwaEvent("Snooze")]
		[OwaEventParameter("LRT", typeof(ExDateTime), false, true)]
		[OwaEventParameter("Rm", typeof(ReminderInfo), true, false)]
		[OwaEventParameter("Snt", typeof(int))]
		public void Snooze()
		{
			Item item = null;
			ExDateTime actualizationTime = DateTimeUtilities.GetLocalTime();
			ReminderInfo[] array = (ReminderInfo[])base.GetParameter("Rm");
			int num = (int)base.GetParameter("Snt");
			TimeSpan t = TimeSpan.FromMinutes((double)num);
			if (base.IsParameterSet("LRT"))
			{
				actualizationTime = (ExDateTime)base.GetParameter("LRT");
			}
			bool flag = false;
			this.Writer.Write("new Array(");
			for (int i = 0; i < array.Length; i++)
			{
				StoreObjectId itemId = array[i].ItemId;
				VersionedId versionedId = Utilities.CreateItemId(base.UserContext.MailboxSession, itemId, array[i].ChangeKey);
				if (flag)
				{
					this.Writer.Write(",");
				}
				else
				{
					flag = true;
				}
				bool flag2 = true;
				try
				{
					try
					{
						item = base.GetRequestItem<Item>(versionedId, new PropertyDefinition[0]);
						object obj = item.TryGetProperty(ItemSchema.ReminderIsSet);
						if (obj is bool && !(bool)obj)
						{
							flag2 = false;
						}
					}
					catch (ObjectNotFoundException)
					{
						flag2 = false;
					}
					if (!flag2)
					{
						this.Writer.Write("new Rrsp(\"\",\"\",1)");
					}
					else
					{
						if (num <= 0)
						{
							item.Reminder.SnoozeBeforeDueBy(actualizationTime, -t);
						}
						else
						{
							item.Reminder.Snooze(actualizationTime, DateTimeUtilities.GetLocalTime() + t);
						}
						Utilities.SaveItem(item);
						item.Load();
						this.Writer.Write("new Rrsp(\"");
						Utilities.JavascriptEncode(item.Id.ChangeKeyAsBase64String(), this.Writer);
						this.Writer.Write("\",\"");
						this.Writer.Write(DateTimeUtilities.GetJavascriptDate(item.Reminder.ReminderNextTime.Value));
						this.Writer.Write("\")");
					}
				}
				finally
				{
					if (item != null)
					{
						item.Dispose();
						item = null;
					}
				}
			}
			this.Writer.Write(")");
		}

		[OwaEvent("Dismiss")]
		[OwaEventParameter("LRT", typeof(ExDateTime), false, true)]
		[OwaEventParameter("Rm", typeof(ReminderInfo), true, false)]
		public void Dismiss()
		{
			ReminderInfo[] array = (ReminderInfo[])base.GetParameter("Rm");
			ExDateTime actualizationTime = DateTimeUtilities.GetLocalTime();
			Item item = null;
			if (base.IsParameterSet("LRT"))
			{
				actualizationTime = (ExDateTime)base.GetParameter("LRT");
			}
			for (int i = 0; i < array.Length; i++)
			{
				StoreObjectId itemId = array[i].ItemId;
				VersionedId versionedId = Utilities.CreateItemId(base.UserContext.MailboxSession, itemId, array[i].ChangeKey);
				try
				{
					try
					{
						item = base.GetRequestItem<Item>(versionedId, ItemBindOption.LoadRequiredPropertiesOnly, new PropertyDefinition[0]);
					}
					catch (ObjectNotFoundException)
					{
						goto IL_9D;
					}
					item.Reminder.Dismiss(actualizationTime);
					item.EnableFullValidation = false;
					Utilities.SaveItem(item);
				}
				finally
				{
					if (item != null)
					{
						item.Dispose();
						item = null;
					}
				}
				IL_9D:;
			}
		}

		public const string EventNamespace = "RemindersEventHandler";

		public const string MethodSnooze = "Snooze";

		public const string MethodDismiss = "Dismiss";

		public const string SnoozeTime = "Snt";

		public const string Reminders = "Rm";

		public const string LastReminderTime = "LRT";
	}
}
