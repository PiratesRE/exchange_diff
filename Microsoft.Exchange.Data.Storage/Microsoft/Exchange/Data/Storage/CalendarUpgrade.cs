using System;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class CalendarUpgrade
	{
		public static CalendarUpgrade.CalendarUpgradeStatus MarkMailboxForUpgrade(IMailboxSession session, IXSOFactory xsoFactory)
		{
			ExAssert.RetailAssert(session != null, "session is null");
			ExAssert.RetailAssert(xsoFactory != null, "xsoFactory is null");
			VariantConfigurationSnapshot configuration = session.MailboxOwner.GetConfiguration();
			if (!configuration.DataStorage.CalendarUpgrade.Enabled)
			{
				ExTraceGlobals.StorageTracer.TraceDebug<Guid>(0L, "Mailbox {0}: Not flighted user", session.MailboxGuid);
				return CalendarUpgrade.CalendarUpgradeStatus.NotFlightedUser;
			}
			if (!CalendarUpgrade.IsInterestingMailboxType(session.MailboxOwner.RecipientTypeDetails))
			{
				ExTraceGlobals.StorageTracer.TraceDebug<Guid>(0L, "Mailbox {0}: Not user mailbox", session.MailboxGuid);
				return CalendarUpgrade.CalendarUpgradeStatus.NotUserMailbox;
			}
			IXSOMailbox mailbox = session.Mailbox;
			if (!CalendarUpgrade.IsMailboxActive(mailbox))
			{
				ExTraceGlobals.StorageTracer.TraceDebug<Guid>(0L, "Mailbox {0}: Inactive mailbox", session.MailboxGuid);
				return CalendarUpgrade.CalendarUpgradeStatus.InactiveMailbox;
			}
			if (mailbox.GetValueOrDefault<int?>(MailboxSchema.ItemsPendingUpgrade, null) != null)
			{
				ExTraceGlobals.StorageTracer.TraceDebug<Guid>(0L, "Mailbox {0}: Already marked for upgrade", session.MailboxGuid);
				return CalendarUpgrade.CalendarUpgradeStatus.AlreadyMarkedForUpgrade;
			}
			int? num = null;
			int? num2 = null;
			CalendarUpgrade.GetCalendarFolderProperties(session, xsoFactory, out num, out num2);
			if (num != null && num == 1)
			{
				ExTraceGlobals.StorageTracer.TraceDebug<Guid>(0L, "Mailbox {0}: Upgrade complete", session.MailboxGuid);
				return CalendarUpgrade.CalendarUpgradeStatus.UpgradeComplete;
			}
			int minCalendarItemsForUpgrade = VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).DataStorage.CalendarUpgradeSettings.MinCalendarItemsForUpgrade;
			if (num2 == null || num2 < minCalendarItemsForUpgrade)
			{
				ExTraceGlobals.StorageTracer.TraceDebug<Guid>(0L, "Mailbox {0}: Not enough calendar items", session.MailboxGuid);
				return CalendarUpgrade.CalendarUpgradeStatus.NotEnoughCalendarItems;
			}
			session.Mailbox.SetOrDeleteProperty(MailboxSchema.ItemsPendingUpgrade, num2);
			session.Mailbox.Save();
			ExTraceGlobals.StorageTracer.TraceDebug<Guid>(0L, "Mailbox {0}: Marked for upgrade", session.MailboxGuid);
			return CalendarUpgrade.CalendarUpgradeStatus.MarkedForUpgrade;
		}

		public static bool IsMailboxActive(IXSOMailbox mbx)
		{
			ExDateTime? valueOrDefault = mbx.GetValueOrDefault<ExDateTime?>(MailboxSchema.LastLogonTime, null);
			return CalendarUpgrade.IsMailboxActive(valueOrDefault);
		}

		public static bool IsMailboxActive(ExDateTime? lastLogonTime)
		{
			return lastLogonTime != null && !(lastLogonTime < ExDateTime.UtcNow - CalendarUpgrade.AllowedInactivity);
		}

		private static bool IsInterestingMailboxType(RecipientTypeDetails recipientTypeDetails)
		{
			return recipientTypeDetails == RecipientTypeDetails.UserMailbox || recipientTypeDetails == RecipientTypeDetails.LinkedMailbox || recipientTypeDetails == RecipientTypeDetails.GroupMailbox;
		}

		private static void GetCalendarFolderProperties(IMailboxSession session, IXSOFactory xsoFactory, out int? calendarFolderVersion, out int? calendarItemCount)
		{
			calendarFolderVersion = null;
			calendarItemCount = null;
			using (IFolder folder = xsoFactory.BindToFolder(session, DefaultFolderType.Calendar))
			{
				calendarFolderVersion = folder.GetValueOrDefault<int?>(FolderSchema.CalendarFolderVersion, null);
				calendarItemCount = folder.GetValueOrDefault<int?>(FolderSchema.ItemCount, null);
			}
		}

		private static readonly TimeSpan AllowedInactivity = TimeSpan.FromDays(30.0);

		public enum CalendarUpgradeStatus
		{
			NotFlightedUser,
			NotUserMailbox,
			InactiveMailbox,
			AlreadyMarkedForUpgrade,
			UpgradeComplete,
			NotEnoughCalendarItems,
			MarkedForUpgrade
		}
	}
}
