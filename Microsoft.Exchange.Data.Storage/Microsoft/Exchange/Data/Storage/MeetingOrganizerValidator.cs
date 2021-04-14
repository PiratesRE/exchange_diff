using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class MeetingOrganizerValidator
	{
		public static bool? IsOrganizer(MailboxSession mailboxSession, GlobalObjectId globalObjectId, out string subject, out string organizerEmailAddress)
		{
			ExTraceGlobals.SyncTracer.TraceDebug<GlobalObjectId, bool>(0L, "[MeetingOrganizerValidatior.IsOrganizer] Search IsOrganizer GOID: {0}, IsCleanGoID: {1}", globalObjectId, globalObjectId.IsCleanGlobalObjectId);
			subject = null;
			organizerEmailAddress = null;
			if (!globalObjectId.IsCleanGlobalObjectId)
			{
				globalObjectId = new GlobalObjectId(globalObjectId.CleanGlobalObjectIdBytes);
			}
			bool value = false;
			bool flag = MeetingOrganizerValidator.SearchForCalendarItem(mailboxSession, mailboxSession.GetDefaultFolderId(DefaultFolderType.Calendar), globalObjectId, out value, out subject, out organizerEmailAddress);
			if (!flag)
			{
				ExTraceGlobals.SyncTracer.TraceDebug(0L, "[MeetingOrganizerValidatior.IsOrganizer] Correlated CalendarItem not found in default calendar folder.  Searching in calendar sub folders...");
				using (Folder folder = Folder.Bind(mailboxSession, DefaultFolderType.Calendar))
				{
					using (QueryResult queryResult = folder.FolderQuery(FolderQueryFlags.DeepTraversal, null, null, MeetingOrganizerValidator.FetchProperties))
					{
						bool flag2 = true;
						while (flag2)
						{
							object[][] rows = queryResult.GetRows(10000);
							if (rows == null || rows.Length == 0)
							{
								break;
							}
							for (int i = 0; i < rows.Length; i++)
							{
								VersionedId versionedId = rows[i][0] as VersionedId;
								StoreObjectId storeObjectId = (versionedId != null) ? versionedId.ObjectId : null;
								int? num = rows[i][1] as int?;
								int extendedFolderFlags = (num != null) ? num.Value : 0;
								if (storeObjectId == null || CalendarFolder.IsCrossOrgShareFolder(extendedFolderFlags) || CalendarFolder.IsInternetCalendar(extendedFolderFlags))
								{
									ExTraceGlobals.SyncTracer.TraceDebug<StoreObjectId>(0L, "[MeetingOrganizerValidator.IsOrganizer] Skipping Calendar '{0}' as it is either a sharing folder or iCal folder", storeObjectId);
								}
								else
								{
									flag = MeetingOrganizerValidator.SearchForCalendarItem(mailboxSession, storeObjectId, globalObjectId, out value, out subject, out organizerEmailAddress);
									if (flag)
									{
										flag2 = false;
										break;
									}
								}
							}
						}
					}
					goto IL_16D;
				}
			}
			ExTraceGlobals.SyncTracer.TraceDebug(0L, "[MeetingOrganizerValidatior.IsOrganizer] Correlated CalendarItem found in default calendar folder");
			IL_16D:
			if (!flag)
			{
				return null;
			}
			return new bool?(value);
		}

		private static bool SearchForCalendarItem(MailboxSession session, StoreObjectId folderId, GlobalObjectId globalObjectId, out bool isOrganizer, out string subject, out string organizerEmailAddress)
		{
			StoreId arg = null;
			isOrganizer = false;
			subject = null;
			organizerEmailAddress = null;
			using (CalendarFolder calendarFolder = CalendarFolder.Bind(session, folderId, null))
			{
				PropertyBag calendarItemProperties = calendarFolder.GetCalendarItemProperties(globalObjectId.Bytes, MeetingOrganizerValidator.CalendarOrganizerProperties);
				if (calendarItemProperties == null)
				{
					ExTraceGlobals.StorageTracer.TraceDebug<string>(0L, "Related Calendar Item not found in calendar folder : '{0}'", calendarFolder.DisplayName);
					return false;
				}
				arg = calendarItemProperties.GetValueOrDefault<VersionedId>(InternalSchema.ItemId);
				string valueOrDefault = calendarItemProperties.GetValueOrDefault<string>(InternalSchema.ItemClass);
				AppointmentStateFlags valueOrDefault2 = calendarItemProperties.GetValueOrDefault<AppointmentStateFlags>(InternalSchema.AppointmentStateInternal);
				isOrganizer = IsOrganizerProperty.GetForCalendarItem(valueOrDefault, valueOrDefault2);
				subject = calendarItemProperties.GetValueOrDefault<string>(InternalSchema.MapiSubject);
				organizerEmailAddress = calendarItemProperties.GetValueOrDefault<string>(InternalSchema.SentRepresentingEmailAddress);
			}
			ExTraceGlobals.SyncTracer.TraceDebug<StoreId, bool>(0L, "Related Calendar Item found. Id: {0}.  Is Organizer? {1}", arg, isOrganizer);
			return true;
		}

		private const int FolderIdIndex = 0;

		private const int ExtendedFolderFlagsIndex = 1;

		private static readonly PropertyDefinition[] FetchProperties = new PropertyDefinition[]
		{
			MessageItemSchema.FolderId,
			FolderSchema.ExtendedFolderFlags
		};

		private static readonly List<PropertyDefinition> CalendarOrganizerProperties = new List<PropertyDefinition>
		{
			InternalSchema.ItemClass,
			InternalSchema.AppointmentStateInternal,
			InternalSchema.SentRepresentingEmailAddress,
			InternalSchema.MapiSubject
		};
	}
}
