using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal sealed class CalendarSettings
	{
		internal static CalendarSettings CreateForSession(UserContext userContext)
		{
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			CalendarSettings result;
			if (CalendarSettings.LoadFromMailbox(userContext.MailboxSession, userContext.CanActAsOwner, out result) != CalendarSettings.LoadResult.Success)
			{
				ExTraceGlobals.CalendarCallTracer.TraceDebug(0L, "Unable to load Calendar Settings User Configuration object");
				result = new CalendarSettings(15, true, false, true, false);
			}
			return result;
		}

		private static CalendarSettings.LoadResult LoadFromMailbox(MailboxSession mailboxSession, bool canActAsOwner, out CalendarSettings calendarSettings)
		{
			if (!canActAsOwner)
			{
				calendarSettings = null;
				return CalendarSettings.LoadResult.AccessDenied;
			}
			CalendarConfiguration calendarConfiguration;
			try
			{
				using (CalendarConfigurationDataProvider calendarConfigurationDataProvider = new CalendarConfigurationDataProvider(mailboxSession))
				{
					calendarConfiguration = (CalendarConfiguration)calendarConfigurationDataProvider.Read<CalendarConfiguration>(null);
				}
				if (calendarConfiguration == null)
				{
					string message = string.Format("Unable to load Calendar configuration object for mailbox {0}", mailboxSession.MailboxOwner.LegacyDn);
					ExTraceGlobals.CalendarCallTracer.TraceDebug(0L, message);
					calendarSettings = null;
					return CalendarSettings.LoadResult.Corrupt;
				}
			}
			catch (StoragePermanentException ex)
			{
				string message2 = string.Format("Unable to load Calendar configuration object for mailbox. Exception {0}", ex.Message);
				ExTraceGlobals.CalendarCallTracer.TraceDebug(0L, message2);
				calendarSettings = null;
				return CalendarSettings.LoadResult.Missing;
			}
			catch (StorageTransientException ex2)
			{
				string message3 = string.Format("Unable to load Calendar configuration object for mailbox. Exception {0}", ex2.Message);
				ExTraceGlobals.CalendarCallTracer.TraceDebug(0L, message3);
				calendarSettings = null;
				return CalendarSettings.LoadResult.Missing;
			}
			calendarSettings = new CalendarSettings(CalendarSettings.ValidateReminderInterval(calendarConfiguration.DefaultReminderTime), calendarConfiguration.AddNewRequestsTentatively, calendarConfiguration.ProcessExternalMeetingMessages, calendarConfiguration.RemoveOldMeetingMessages, calendarConfiguration.RemoveForwardedMeetingNotifications);
			return CalendarSettings.LoadResult.Success;
		}

		internal bool CommitChanges(MailboxSession mailboxSession)
		{
			if (mailboxSession == null)
			{
				throw new ArgumentNullException("mailboxSession");
			}
			try
			{
				using (CalendarConfigurationDataProvider calendarConfigurationDataProvider = new CalendarConfigurationDataProvider(mailboxSession))
				{
					CalendarConfiguration calendarConfiguration = (CalendarConfiguration)calendarConfigurationDataProvider.Read<CalendarConfiguration>(null);
					if (calendarConfiguration == null)
					{
						string message = string.Format("Unable to load Calendar configuration object for mailbox {0}", mailboxSession.MailboxOwner.LegacyDn);
						ExTraceGlobals.CalendarCallTracer.TraceDebug(0L, message);
						calendarConfigurationDataProvider.Delete(new CalendarConfiguration());
						calendarConfiguration = (CalendarConfiguration)calendarConfigurationDataProvider.Read<CalendarConfiguration>(null);
						if (calendarConfiguration == null)
						{
							message = string.Format("Unable to re-create Calendar configuration object for mailbox {0}", mailboxSession.MailboxOwner.LegacyDn);
							ExTraceGlobals.CalendarCallTracer.TraceDebug(0L, message);
							return false;
						}
					}
					calendarConfiguration.AddNewRequestsTentatively = this.addNewRequestsTentatively;
					calendarConfiguration.RemoveOldMeetingMessages = this.removeOldMeetingMessages;
					calendarConfiguration.RemoveForwardedMeetingNotifications = this.removeForwardedMeetingNotifications;
					calendarConfiguration.DefaultReminderTime = this.defaultReminderTime;
					calendarConfiguration.ProcessExternalMeetingMessages = this.processExternalMeetingMessages;
					calendarConfigurationDataProvider.Save(calendarConfiguration);
				}
			}
			catch (ObjectExistedException ex)
			{
				string message2 = string.Format("Unable to load Calendar configuration object for mailbox. Exception {0}", ex.Message);
				ExTraceGlobals.CalendarCallTracer.TraceDebug(0L, message2);
				return false;
			}
			catch (StoragePermanentException ex2)
			{
				string message3 = string.Format("Unable to load Calendar configuration object for mailbox. Exception {0}", ex2.Message);
				ExTraceGlobals.CalendarCallTracer.TraceDebug(0L, message3);
				return false;
			}
			catch (StorageTransientException ex3)
			{
				string message4 = string.Format("Unable to load Calendar configuration object for mailbox. Exception {0}", ex3.Message);
				ExTraceGlobals.CalendarCallTracer.TraceDebug(0L, message4);
				return false;
			}
			return true;
		}

		private static int ValidateReminderInterval(int reminderInterval)
		{
			if (reminderInterval <= 5040000 && reminderInterval >= 0)
			{
				ExTraceGlobals.CalendarCallTracer.TraceDebug<int>(0L, "Returning original value: {0}", reminderInterval);
				return reminderInterval;
			}
			if (reminderInterval > 5040000)
			{
				ExTraceGlobals.CalendarCallTracer.TraceDebug<int>(0L, "Reminder interval exceeds maximum.  Returning max reminder interval value: {0}", 5040000);
				return 5040000;
			}
			ExTraceGlobals.CalendarCallTracer.TraceDebug<int>(0L, "Reminder interval is a negative integer.  Returning default value: {0}", 15);
			return 15;
		}

		private CalendarSettings(int defaultReminderTime, bool autoCreate, bool processExternal, bool removeOldMeetingMessages, bool removeForwardedMeetingNotifications)
		{
			this.defaultReminderTime = defaultReminderTime;
			this.addNewRequestsTentatively = autoCreate;
			this.processExternalMeetingMessages = processExternal;
			this.removeOldMeetingMessages = removeOldMeetingMessages;
			this.removeForwardedMeetingNotifications = removeForwardedMeetingNotifications;
		}

		internal int DefaultReminderTime
		{
			get
			{
				return this.defaultReminderTime;
			}
			set
			{
				this.defaultReminderTime = CalendarSettings.ValidateReminderInterval(value);
			}
		}

		internal bool AddNewRequestsTentatively
		{
			get
			{
				return this.addNewRequestsTentatively;
			}
			set
			{
				this.addNewRequestsTentatively = value;
			}
		}

		internal bool ProcessExternalMeetingMessages
		{
			get
			{
				return this.processExternalMeetingMessages;
			}
			set
			{
				this.processExternalMeetingMessages = value;
			}
		}

		internal bool RemoveOldMeetingMessages
		{
			get
			{
				return this.removeOldMeetingMessages;
			}
			set
			{
				this.removeOldMeetingMessages = value;
			}
		}

		internal bool RemoveForwardedMeetingNotifications
		{
			get
			{
				return this.removeForwardedMeetingNotifications;
			}
			set
			{
				this.removeForwardedMeetingNotifications = value;
			}
		}

		private const int DefaultDefaultReminderTime = 15;

		private const bool DefaultAddNewRequestsTentatively = true;

		private const bool DefaultProcessExternalMeetingMessages = false;

		private const bool DefaultRemoveOldMeetingMessages = true;

		private const bool DefaultRemoveForwardedMeetingNotification = false;

		internal const int AllowedMaxReminderInterval = 5040000;

		private int defaultReminderTime = 15;

		private bool addNewRequestsTentatively = true;

		private bool processExternalMeetingMessages;

		private bool removeOldMeetingMessages = true;

		private bool removeForwardedMeetingNotifications;

		private enum LoadResult
		{
			Success,
			Missing,
			AccessDenied,
			Corrupt
		}
	}
}
