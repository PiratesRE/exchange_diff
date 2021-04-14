using System;
using System.Collections.Generic;
using Microsoft.Exchange.Clients.Owa.Premium;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.InfoWorker.Common.Availability;

namespace Microsoft.Exchange.Clients.Owa.Core.Controls
{
	internal sealed class AvailabilityDataSource : ICalendarDataSource
	{
		public AvailabilityDataSource(UserContext userContext, string smtpAddress, StoreObjectId folderStoreId, DateRange[] dateRanges, bool pendingLoad)
		{
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			if (smtpAddress == null)
			{
				throw new ArgumentNullException("smtpAddress");
			}
			if (dateRanges == null)
			{
				throw new ArgumentNullException("dateRanges");
			}
			if (dateRanges.Length == 0)
			{
				throw new ArgumentException("Length of dateRanges cannot be 0");
			}
			this.userContext = userContext;
			this.dateRanges = dateRanges;
			if (!pendingLoad)
			{
				FreeBusyQueryResult[] array = AvailabilityDataSource.BatchLoadData(userContext, new string[]
				{
					smtpAddress
				}, new StoreObjectId[]
				{
					folderStoreId
				}, dateRanges);
				if (array != null)
				{
					this.LoadFromQueryResult(array[0]);
				}
			}
		}

		public static FreeBusyQueryResult[] BatchLoadData(UserContext userContext, CalendarAdapter[] adapters, DateRange[] dateRanges)
		{
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			if (adapters == null)
			{
				throw new ArgumentNullException("adapters");
			}
			if (adapters.Length == 0)
			{
				throw new ArgumentException("Length of adapters cannot be 0");
			}
			if (dateRanges == null)
			{
				throw new ArgumentNullException("dateRanges");
			}
			if (dateRanges.Length == 0)
			{
				throw new ArgumentException("Length of dateRanges cannot be 0");
			}
			string[] array = new string[adapters.Length];
			StoreObjectId[] array2 = new StoreObjectId[adapters.Length];
			for (int i = 0; i < adapters.Length; i++)
			{
				array[i] = adapters[i].SmtpAddress;
				array2[i] = (adapters[i].IsGSCalendar ? null : adapters[i].FolderId.StoreObjectId);
			}
			return AvailabilityDataSource.BatchLoadData(userContext, array, array2, dateRanges);
		}

		public static FreeBusyQueryResult[] BatchLoadData(UserContext userContext, string[] smtpAddresses, StoreObjectId[] folderStoreIds, DateRange[] dateRanges)
		{
			if (smtpAddresses == null || smtpAddresses.Length == 0)
			{
				throw new ArgumentNullException("smtpAddresses");
			}
			if (dateRanges == null || dateRanges.Length == 0)
			{
				throw new ArgumentNullException("dateRanges");
			}
			ExTraceGlobals.CalendarCallTracer.TraceDebug(0L, "GSCalendarDataSource.Load");
			AvailabilityQuery availabilityQuery = new AvailabilityQuery();
			availabilityQuery.MailboxArray = new MailboxData[smtpAddresses.Length];
			availabilityQuery.ClientContext = ClientContext.Create(userContext.LogonIdentity.ClientSecurityContext, userContext.ExchangePrincipal.MailboxInfo.OrganizationId, OwaContext.TryGetCurrentBudget(), userContext.TimeZone, userContext.UserCulture, AvailabilityQuery.CreateNewMessageId());
			for (int i = 0; i < smtpAddresses.Length; i++)
			{
				availabilityQuery.MailboxArray[i] = new MailboxData();
				availabilityQuery.MailboxArray[i].Email = new EmailAddress();
				availabilityQuery.MailboxArray[i].Email.Address = smtpAddresses[i];
				availabilityQuery.MailboxArray[i].AssociatedFolderId = folderStoreIds[i];
			}
			availabilityQuery.DesiredFreeBusyView = new FreeBusyViewOptions
			{
				TimeWindow = new Duration(),
				TimeWindow = 
				{
					StartTime = (DateTime)DateRange.GetMinStartTimeInRangeArray(dateRanges),
					EndTime = (DateTime)DateRange.GetMaxEndTimeInRangeArray(dateRanges)
				},
				MergedFreeBusyIntervalInMinutes = userContext.UserOptions.HourIncrement,
				RequestedView = FreeBusyViewType.Detailed
			};
			AvailabilityQueryResult availabilityQueryResult = Utilities.ExecuteAvailabilityQuery(availabilityQuery);
			if (availabilityQueryResult == null)
			{
				return null;
			}
			return availabilityQueryResult.FreeBusyResults;
		}

		public void LoadFromQueryResult(FreeBusyQueryResult queryResult)
		{
			if (queryResult == null)
			{
				throw new ArgumentNullException("queryResult");
			}
			this.AssociatedCalendarType = AvailabilityDataSource.CalendarType.Unknown;
			if (queryResult.ExceptionInfo != null)
			{
				if (queryResult.ExceptionInfo is NotDefaultCalendarException)
				{
					this.AssociatedCalendarType = AvailabilityDataSource.CalendarType.Secondary;
				}
				this.userCanReadItem = false;
				return;
			}
			this.AssociatedCalendarType = AvailabilityDataSource.CalendarType.Primary;
			this.userCanReadItem = true;
			this.workingHours = WorkingHours.CreateFromAvailabilityWorkingHours(this.userContext, queryResult.WorkingHours);
			if (queryResult.CalendarEventArray == null && queryResult.MergedFreeBusy != null)
			{
				this.items = this.GetItemsFromMergedFreeBusy(queryResult.MergedFreeBusy);
				return;
			}
			if (queryResult.CalendarEventArray != null && queryResult.CalendarEventArray.Length > 0)
			{
				this.items = this.GetItemsFromEventArray(queryResult.CalendarEventArray);
			}
		}

		private GSCalendarItem[] GetItemsFromEventArray(CalendarEvent[] eventArray)
		{
			List<GSCalendarItem> list = new List<GSCalendarItem>(eventArray.Length);
			foreach (CalendarEvent calendarEvent in eventArray)
			{
				ExDateTime exDateTime = new ExDateTime(this.userContext.TimeZone, calendarEvent.StartTime);
				ExDateTime exDateTime2 = new ExDateTime(this.userContext.TimeZone, calendarEvent.EndTime);
				if (exDateTime > exDateTime2)
				{
					ExTraceGlobals.CalendarDataTracer.TraceDebug(0L, "Skipping appointment with an end time earlier than a start time");
				}
				else
				{
					for (int j = 0; j < this.dateRanges.Length; j++)
					{
						if (this.dateRanges[j].Intersects(exDateTime, exDateTime2))
						{
							GSCalendarItem gscalendarItem = new GSCalendarItem();
							gscalendarItem.StartTime = exDateTime;
							gscalendarItem.EndTime = exDateTime2;
							gscalendarItem.BusyType = this.ConvertBusyType(calendarEvent.BusyType);
							if (calendarEvent.CalendarEventDetails != null)
							{
								CalendarEventDetails calendarEventDetails = calendarEvent.CalendarEventDetails;
								gscalendarItem.Subject = calendarEventDetails.Subject;
								gscalendarItem.Location = calendarEventDetails.Location;
								gscalendarItem.IsMeeting = calendarEventDetails.IsMeeting;
								gscalendarItem.IsPrivate = calendarEventDetails.IsPrivate;
								if (calendarEventDetails.IsException)
								{
									gscalendarItem.CalendarItemType = CalendarItemTypeWrapper.Exception;
								}
								else if (calendarEventDetails.IsRecurring)
								{
									gscalendarItem.CalendarItemType = CalendarItemTypeWrapper.Occurrence;
								}
								else
								{
									gscalendarItem.CalendarItemType = CalendarItemTypeWrapper.Single;
								}
							}
							else
							{
								gscalendarItem.CalendarItemType = CalendarItemTypeWrapper.Single;
							}
							list.Add(gscalendarItem);
							break;
						}
					}
				}
			}
			return list.ToArray();
		}

		private GSCalendarItem[] GetItemsFromMergedFreeBusy(string mergedFreeBusy)
		{
			List<GSCalendarItem> list = new List<GSCalendarItem>(mergedFreeBusy.Length);
			GSCalendarItem gscalendarItem = null;
			BusyTypeWrapper busyTypeWrapper = BusyTypeWrapper.Free;
			int i;
			for (i = 0; i < mergedFreeBusy.Length; i++)
			{
				int num;
				if (!int.TryParse(mergedFreeBusy[i].ToString(), out num) || num < 0 || num > 4)
				{
					ExTraceGlobals.CalendarDataTracer.TraceDebug(0L, "Availability Service returns invalid data in MergedFreeBusy:" + mergedFreeBusy[i]);
				}
				else
				{
					BusyTypeWrapper busyTypeWrapper2 = (BusyTypeWrapper)num;
					if (busyTypeWrapper2 != busyTypeWrapper)
					{
						busyTypeWrapper = busyTypeWrapper2;
						if (gscalendarItem != null)
						{
							this.CheckAndAddCurrentItem(list, ref gscalendarItem, i);
						}
						if (busyTypeWrapper2 != BusyTypeWrapper.Free)
						{
							gscalendarItem = new GSCalendarItem
							{
								StartTime = this.GetDateTimeFromIndex(i),
								BusyType = busyTypeWrapper2,
								CalendarItemType = CalendarItemTypeWrapper.Single
							};
						}
					}
				}
			}
			if (gscalendarItem != null)
			{
				this.CheckAndAddCurrentItem(list, ref gscalendarItem, i);
			}
			return list.ToArray();
		}

		private void CheckAndAddCurrentItem(IList<GSCalendarItem> itemList, ref GSCalendarItem currentItem, int i)
		{
			currentItem.EndTime = this.GetDateTimeFromIndex(i);
			for (int j = 0; j < this.dateRanges.Length; j++)
			{
				if (this.dateRanges[j].Intersects(currentItem.StartTime, currentItem.EndTime))
				{
					itemList.Add(currentItem);
					break;
				}
			}
			currentItem = null;
		}

		private ExDateTime GetDateTimeFromIndex(int i)
		{
			return this.dateRanges[0].Start.AddMinutes((double)(i * this.userContext.UserOptions.HourIncrement));
		}

		private BusyTypeWrapper ConvertBusyType(Microsoft.Exchange.InfoWorker.Common.Availability.BusyType busyType)
		{
			switch (busyType)
			{
			case Microsoft.Exchange.InfoWorker.Common.Availability.BusyType.Free:
				return BusyTypeWrapper.Free;
			case Microsoft.Exchange.InfoWorker.Common.Availability.BusyType.Tentative:
				return BusyTypeWrapper.Tentative;
			case Microsoft.Exchange.InfoWorker.Common.Availability.BusyType.Busy:
				return BusyTypeWrapper.Busy;
			case Microsoft.Exchange.InfoWorker.Common.Availability.BusyType.OOF:
				return BusyTypeWrapper.OOF;
			default:
				return BusyTypeWrapper.Unknown;
			}
		}

		public int Count
		{
			get
			{
				if (this.items != null)
				{
					return this.items.Length;
				}
				return 0;
			}
		}

		public OwaStoreObjectId GetItemId(int index)
		{
			return null;
		}

		public string GetChangeKey(int index)
		{
			return null;
		}

		public ExDateTime GetStartTime(int index)
		{
			return this.items[index].StartTime;
		}

		public ExDateTime GetEndTime(int index)
		{
			return this.items[index].EndTime;
		}

		public string GetSubject(int index)
		{
			return this.items[index].Subject ?? this.GetSubjectOfFreeBusyOnlyItem(index);
		}

		public string GetLocation(int index)
		{
			return this.items[index].Location ?? string.Empty;
		}

		public bool IsMeeting(int index)
		{
			return this.items[index].IsMeeting;
		}

		public bool IsCancelled(int index)
		{
			return false;
		}

		public bool HasAttachment(int index)
		{
			return false;
		}

		public bool IsPrivate(int index)
		{
			return this.items[index].IsPrivate;
		}

		public CalendarItemTypeWrapper GetWrappedItemType(int index)
		{
			return this.items[index].CalendarItemType;
		}

		public string GetOrganizerDisplayName(int index)
		{
			return string.Empty;
		}

		public BusyTypeWrapper GetWrappedBusyType(int index)
		{
			return this.items[index].BusyType;
		}

		public bool IsOrganizer(int index)
		{
			return false;
		}

		public string[] GetCategories(int index)
		{
			return null;
		}

		public string GetCssClassName(int index)
		{
			return "noClrCal";
		}

		public string GetInviteesDisplayNames(int index)
		{
			return null;
		}

		private string GetSubjectOfFreeBusyOnlyItem(int index)
		{
			switch (this.items[index].BusyType)
			{
			case BusyTypeWrapper.Free:
				return LocalizedStrings.GetNonEncoded(-971703552);
			case BusyTypeWrapper.Tentative:
				return LocalizedStrings.GetNonEncoded(1797669216);
			case BusyTypeWrapper.Busy:
				return LocalizedStrings.GetNonEncoded(2052801377);
			case BusyTypeWrapper.OOF:
				return LocalizedStrings.GetNonEncoded(2047193656);
			default:
				return LocalizedStrings.GetNonEncoded(-1280331347);
			}
		}

		public SharedType SharedType
		{
			get
			{
				return SharedType.InternalFreeBusy;
			}
		}

		public WorkingHours WorkingHours
		{
			get
			{
				return this.workingHours;
			}
		}

		public bool UserCanReadItem
		{
			get
			{
				return this.userCanReadItem;
			}
		}

		public AvailabilityDataSource.CalendarType AssociatedCalendarType { get; private set; }

		public bool UserCanCreateItem
		{
			get
			{
				return false;
			}
		}

		public string FolderClassName
		{
			get
			{
				return string.Empty;
			}
		}

		private DateRange[] dateRanges;

		private UserContext userContext;

		private GSCalendarItem[] items;

		private bool userCanReadItem;

		private WorkingHours workingHours;

		public enum CalendarType
		{
			Unknown,
			Primary,
			Secondary
		}
	}
}
