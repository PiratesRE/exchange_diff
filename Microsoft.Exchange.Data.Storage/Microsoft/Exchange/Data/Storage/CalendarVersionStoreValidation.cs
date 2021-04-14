using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class CalendarVersionStoreValidation : SearchFolderValidation
	{
		internal CalendarVersionStoreValidation() : base(new IValidator[]
		{
			new MatchMapiFolderType(FolderType.Search)
		})
		{
		}

		private static QueryFilter[] GetItemSubClassQueryFilter(IList<string> itemClasses)
		{
			QueryFilter[] array = new QueryFilter[itemClasses.Count * 2];
			for (int i = 0; i < itemClasses.Count; i++)
			{
				string text = itemClasses[i];
				array[i * 2] = new TextFilter(InternalSchema.ItemClass, text, MatchOptions.ExactPhrase, MatchFlags.IgnoreCase);
				array[i * 2 + 1] = new TextFilter(InternalSchema.ItemClass, string.Format("{0}.", text), MatchOptions.Prefix, MatchFlags.IgnoreCase);
			}
			return array;
		}

		internal static SearchFolderCriteria CreateCalendarVersionSearchCriteria(DefaultFolderContext context)
		{
			List<StoreId> list = new List<StoreId>();
			list.Add(context[DefaultFolderType.Root]);
			if (context[DefaultFolderType.RecoverableItemsRoot] != null)
			{
				list.Add(context[DefaultFolderType.RecoverableItemsRoot]);
			}
			CalendarVersionStoreValidation.AddAdditionalFoldersForCalendarVersionSearch(list, context);
			return new SearchFolderCriteria(CalendarVersionStoreValidation.GetCalendarVersionQueryFilter(context), list.ToArray())
			{
				DeepTraversal = true
			};
		}

		internal override bool EnsureIsValid(DefaultFolderContext context, Folder folder)
		{
			if (!base.EnsureIsValid(context, folder) || !(folder is SearchFolder))
			{
				return false;
			}
			SearchFolder searchFolder = (SearchFolder)folder;
			SearchFolderCriteria searchFolderCriteria = CalendarVersionStoreValidation.CreateCalendarVersionSearchCriteria(context);
			SearchFolderCriteria searchCriteria = searchFolder.GetSearchCriteria();
			if (!SearchFolderValidation.MatchSearchFolderCriteria(searchCriteria, searchFolderCriteria))
			{
				searchFolder.ApplyContinuousSearch(searchFolderCriteria);
			}
			return true;
		}

		protected override void SetPropertiesInternal(DefaultFolderContext context, Folder folder)
		{
			base.SetPropertiesInternal(context, folder);
			SearchFolder searchFolder = (SearchFolder)folder;
			searchFolder.Save();
			searchFolder.ApplyContinuousSearch(CalendarVersionStoreValidation.CreateCalendarVersionSearchCriteria(context));
			searchFolder.Load();
		}

		private static QueryFilter GetCalendarVersionQueryFilter(DefaultFolderContext context)
		{
			List<string> list = new List<string>
			{
				"IPM.Appointment",
				"IPM.Schedule.Meeting",
				"IPM.Schedule.Inquiry",
				"IPM.Notification.Meeting",
				"IPM.OLE.CLASS.{00061055-0000-0000-C000-000000000046}"
			};
			if (CalendarVersionStoreValidation.IsIncludeSeriesMeetingMessagesInCVSEnabled(context))
			{
				list.Add("IPM.AppointmentSeries");
				list.Add("IPM.MeetingMessageSeries");
				list.Add("IPM.Parked.MeetingMessage");
			}
			return new OrFilter(CalendarVersionStoreValidation.GetItemSubClassQueryFilter(list));
		}

		private static void AddAdditionalFoldersForCalendarVersionSearch(List<StoreId> folderScope, DefaultFolderContext context)
		{
			if (CalendarVersionStoreValidation.IsIncludeSeriesMeetingMessagesInCVSEnabled(context) && context[DefaultFolderType.ParkedMessages] != null)
			{
				folderScope.Add(context[DefaultFolderType.ParkedMessages]);
			}
		}

		private static bool IsIncludeSeriesMeetingMessagesInCVSEnabled(DefaultFolderContext context)
		{
			return context.Session.MailboxOwner.GetConfiguration().CalendarLogging.CalendarLoggingIncludeSeriesMeetingMessagesInCVS.Enabled;
		}
	}
}
