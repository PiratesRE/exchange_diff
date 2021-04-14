using System;
using System.Text;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.WebServices.Data;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.CalendarSharing.Probes
{
	public static class CalendarSharingUtils
	{
		public static void GetSharedFolderAppointment(ExchangeService pubService, ExchangeService subService, string publisherEmail, Appointment appointment)
		{
			CalendarView calendarView = new CalendarView(appointment.Start.AddDays(-1.0), appointment.End.AddDays(1.0));
			FolderId folderId = new FolderId(0, publisherEmail);
			FindItemsResults<Appointment> findItemsResults = subService.FindAppointments(folderId, calendarView);
			bool flag = false;
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("\nSubscriber's shared calendar appt.s \n");
			foreach (Appointment appointment2 in findItemsResults)
			{
				stringBuilder.Append(string.Format("Subject: {0} Start Time {1} End Time {2}\n", appointment2.Subject, appointment2.Start, appointment2.End));
				if (string.Equals(appointment2.Subject, appointment.Subject, StringComparison.OrdinalIgnoreCase))
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				stringBuilder.Append("\nPublisher's calendar appt.s \n");
				new FolderId(0);
				pubService.FindAppointments(folderId, calendarView);
				foreach (Appointment appointment3 in findItemsResults)
				{
					stringBuilder.Append(string.Format("Subject: {0} Start Time {1} End Time {2}\n", appointment3.Subject, appointment3.Start, appointment3.End));
				}
				throw new CalendarSharingException(string.Format("The internal calendar sharing probe didn't succeed. New appointment on shared calendar with subject {0} could not be found. Appointments returned by FindAppointments: {1}", appointment.Subject, stringBuilder));
			}
		}

		public static Appointment CreateTestAppointment(ExchangeService exService)
		{
			ExDateTime now = ExDateTime.GetNow(ExTimeZone.CurrentTimeZone);
			string subject = "Test Appt. " + now.UtcTicks.ToString();
			return CalendarSharingUtils.CreateTestAppointment(exService, subject, now);
		}

		public static Appointment CreateTestAppointment(ExchangeService pubService, string subject, ExDateTime start)
		{
			Appointment appointment = new Appointment(pubService);
			appointment.Subject = subject;
			appointment.Start = (DateTime)start;
			appointment.End = appointment.Start.AddHours(2.0);
			appointment.Save(0);
			return appointment;
		}

		public static void DeleteTestAppointment(ExchangeService pubService, ItemId appointmentId)
		{
			Appointment appointment = Appointment.Bind(pubService, appointmentId);
			appointment.Delete(2);
		}

		public static void AddFolderPermission(ExchangeService pubService, string subscriberSmtp, FolderPermissionLevel fldPermLevel)
		{
			CalendarSharingUtils.RemoveFolderPermission(pubService, subscriberSmtp);
			Folder folder = Folder.Bind(pubService, new FolderId(0));
			FolderPermission folderPermission = new FolderPermission(subscriberSmtp, fldPermLevel);
			folder.Permissions.Add(folderPermission);
			folder.Update();
		}

		public static string GetFolderPermission(ExchangeService pubService, string subscriberSmtp)
		{
			Folder folder = Folder.Bind(pubService, new FolderId(0));
			string result = string.Empty;
			foreach (FolderPermission folderPermission in folder.Permissions)
			{
				if (folderPermission.UserId != null && string.Equals(folderPermission.UserId.PrimarySmtpAddress, subscriberSmtp, StringComparison.OrdinalIgnoreCase))
				{
					result = folderPermission.PermissionLevel.ToString();
					break;
				}
			}
			return result;
		}

		public static void RemoveFolderPermission(ExchangeService pubService, string subscriberSmtp)
		{
			Folder folder = Folder.Bind(pubService, new FolderId(0));
			foreach (FolderPermission folderPermission in folder.Permissions)
			{
				if (folderPermission.UserId != null && string.Equals(folderPermission.UserId.PrimarySmtpAddress, subscriberSmtp, StringComparison.OrdinalIgnoreCase))
				{
					folder.Permissions.Remove(folderPermission);
					folder.Update();
					break;
				}
			}
		}
	}
}
