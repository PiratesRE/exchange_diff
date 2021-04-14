using System;
using System.Collections;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class TimeZoneSettings
	{
		public static bool TryFindOwaTimeZone(MailboxSession session, out ExTimeZone timeZone)
		{
			timeZone = null;
			try
			{
				using (UserConfiguration mailboxConfiguration = session.UserConfigurationManager.GetMailboxConfiguration("OWA.UserOptions", UserConfigurationTypes.Dictionary))
				{
					IDictionary dictionary = mailboxConfiguration.GetDictionary();
					string text = dictionary["timezone"] as string;
					if (text != null && ExTimeZoneEnumerator.Instance.TryGetTimeZoneByName(text, out timeZone))
					{
						return true;
					}
				}
			}
			catch (ObjectNotFoundException)
			{
			}
			catch (CorruptDataException)
			{
			}
			catch (AccessDeniedException)
			{
			}
			return false;
		}

		public static bool TryFindOutlookTimeZone(MailboxSession session, out byte[] blob)
		{
			blob = null;
			try
			{
				SortBy[] sortColumns = new SortBy[]
				{
					new SortBy(InternalSchema.LastModifiedTime, SortOrder.Descending)
				};
				PropertyDefinition[] dataColumns = new PropertyDefinition[]
				{
					InternalSchema.AppointmentRecurring,
					InternalSchema.AppointmentStateInternal,
					InternalSchema.ResponseState,
					InternalSchema.TimeZoneDefinitionRecurring
				};
				using (Folder folder = Folder.Bind(session, DefaultFolderType.Calendar))
				{
					using (QueryResult queryResult = folder.ItemQuery(ItemQueryType.None, null, sortColumns, dataColumns))
					{
						object[][] rows = queryResult.GetRows(10000);
						foreach (object[] row in rows)
						{
							if (TimeZoneSettings.TryFindTimezoneRow(row, out blob))
							{
								return true;
							}
						}
					}
				}
			}
			catch (ObjectNotFoundException)
			{
			}
			catch (AccessDeniedException)
			{
			}
			return false;
		}

		private static bool TryFindTimezoneRow(object[] row, out byte[] blob)
		{
			blob = null;
			if (!(row[0] is bool) || !(bool)row[0])
			{
				return false;
			}
			if (!(row[3] is byte[]))
			{
				return false;
			}
			if (row[2] is int)
			{
				if ((int)row[2] == 1)
				{
					blob = (byte[])row[3];
					return true;
				}
			}
			else if (row[1] is int)
			{
				AppointmentStateFlags appointmentStateFlags = (AppointmentStateFlags)((int)row[1]);
				if (((appointmentStateFlags & AppointmentStateFlags.Meeting) == AppointmentStateFlags.Meeting && (appointmentStateFlags & AppointmentStateFlags.Received) == AppointmentStateFlags.None && (appointmentStateFlags & AppointmentStateFlags.Forward) == AppointmentStateFlags.None) || appointmentStateFlags == AppointmentStateFlags.None)
				{
					blob = (byte[])row[3];
					return true;
				}
			}
			return false;
		}
	}
}
