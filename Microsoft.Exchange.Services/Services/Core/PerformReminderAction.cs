using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Services.Core.DataConverter;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class PerformReminderAction : SingleStepServiceCommand<PerformReminderActionRequest, ItemId[]>
	{
		public PerformReminderAction(CallContext callContext, PerformReminderActionRequest request) : base(callContext, request)
		{
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			return new PerformReminderActionResponse(base.Result.Code, base.Result.Error, base.Result.Value);
		}

		internal override ServiceResult<ItemId[]> Execute()
		{
			ReminderItemActionType[] reminderItemActions = base.Request.ReminderItemActions;
			if (reminderItemActions == null || reminderItemActions.Length == 0)
			{
				ExTraceGlobals.PerformReminderActionCallTracer.TraceError((long)this.GetHashCode(), "No ReminderActions are specified.");
				throw new ArgumentException("This service should never be called without at least 1 reminder action specified.");
			}
			List<ItemId> list = new List<ItemId>();
			foreach (ReminderItemActionType reminderItemActionType in reminderItemActions)
			{
				if (reminderItemActionType.ItemId == null || string.IsNullOrEmpty(reminderItemActionType.ItemId.Id) || string.IsNullOrEmpty(reminderItemActionType.ItemId.ChangeKey))
				{
					ExTraceGlobals.PerformReminderActionCallTracer.TraceError<string, string>((long)this.GetHashCode(), "ReminderAction without ItemId correctly specified - skipping. Id:{0}, ChangeKey:{1}", (reminderItemActionType.ItemId == null || reminderItemActionType.ItemId.Id == null) ? "is null" : reminderItemActionType.ItemId.Id, (reminderItemActionType.ItemId == null || reminderItemActionType.ItemId.ChangeKey == null) ? "is null" : reminderItemActionType.ItemId.ChangeKey);
				}
				else
				{
					ExTraceGlobals.PerformReminderActionCallTracer.TraceDebug<string, string>((long)this.GetHashCode(), "Processing ReminderAction - ActionType: {0}, ItemId: {1}", reminderItemActionType.ActionType.ToString(), reminderItemActionType.ItemId.Id);
					this.PerformActionOnItem(reminderItemActionType.ActionType, reminderItemActionType.ItemId, reminderItemActionType.NewReminderTime, list);
				}
			}
			return new ServiceResult<ItemId[]>(list.ToArray());
		}

		private void PerformActionOnItem(ReminderActionType actionType, ItemId itemId, string newReminderTimeString, List<ItemId> returnedItemIds)
		{
			ItemId item = new ItemId(itemId.Id, itemId.ChangeKey);
			ExDateTime? newReminderTime = null;
			try
			{
				StoreId storeId = IdConverter.ConvertItemIdToStoreId(itemId, BasicTypes.Item);
				using (Item item2 = Item.Bind(base.MailboxIdentityMailboxSession, storeId))
				{
					CalendarItemBase calendarItemBase = item2 as CalendarItemBase;
					if (calendarItemBase != null && calendarItemBase.CalendarItemType == CalendarItemType.RecurringMaster)
					{
						ExTraceGlobals.PerformReminderActionCallTracer.TraceError<string, string>((long)this.GetHashCode(), "PerformReminderAction should never be called on a RecurringMaster Calendar Item. ActionType: {0}, ItemId: {1}", actionType.ToString(), itemId.Id);
						throw new ArgumentException("PerformReminderAction should never be called on a RecurringMaster Calendar Item.");
					}
					if (actionType == ReminderActionType.Snooze)
					{
						ExTraceGlobals.PerformReminderActionCallTracer.TraceDebug<string, string>((long)this.GetHashCode(), "ActionType = Snooze - Converting NewReminderTime to ExDateTime. ItemId: {0}. NewReminderTimeString: {1}", itemId.Id, (newReminderTimeString == null) ? "is null" : newReminderTimeString);
						newReminderTime = new ExDateTime?(ExDateTimeConverter.ParseTimeZoneRelated(newReminderTimeString, (calendarItemBase == null) ? item2.Session.ExTimeZone : calendarItemBase.StartTimeZone));
						ExTraceGlobals.PerformReminderActionCallTracer.TraceDebug<string, string>((long)this.GetHashCode(), "NewReminderTime converted to ExDateTime. ItemId: {0}, NewReminderTime:{1}", itemId.Id, newReminderTime.ToString());
					}
					Item item3 = this.UpdateReminderForItem(actionType, itemId, newReminderTime, item2);
					item = this.PersistReminderChanges(item3);
				}
			}
			catch (Exception ex)
			{
				string text = string.Empty;
				if (ex is InvalidValueForPropertyException)
				{
					text = "May be due to invalid NewReminderTime - NewReminderTimeString: " + ((newReminderTimeString == null) ? "is null" : newReminderTimeString);
				}
				ExTraceGlobals.PerformReminderActionCallTracer.TraceError((long)this.GetHashCode(), "Unable to {0} item reminder due to {1}. RequestItemId.Id: {2} Message: {3}. {4}", new object[]
				{
					actionType.ToString(),
					ex.GetType().Name,
					itemId.Id,
					ex.Message,
					text
				});
				if (!(ex is StoragePermanentException) && !(ex is StorageTransientException) && !(ex is ArgumentException) && !(ex is InvalidOperationException) && !(ex is InvalidValueForPropertyException) && !(ex is NoSupportException))
				{
					throw;
				}
			}
			returnedItemIds.Add(item);
		}

		private Item UpdateReminderForItem(ReminderActionType actionType, ItemId itemId, ExDateTime? newReminderTime, Item item)
		{
			CalendarItemOccurrence calendarItemOccurrence = item as CalendarItemOccurrence;
			if (calendarItemOccurrence != null)
			{
				ExTraceGlobals.PerformReminderActionCallTracer.TraceDebug<string>((long)this.GetHashCode(), "Retrieving Recurring master for occurrence. Converted ItemId: {0}", (itemId.Id == null) ? "is null" : item.Id.ToBase64String());
				CalendarItem masterCalendarItem = calendarItemOccurrence.OccurrencePropertyBag.MasterCalendarItem;
				if (actionType == ReminderActionType.Snooze)
				{
					ExTraceGlobals.PerformReminderActionCallTracer.TraceDebug<VersionedId>((long)this.GetHashCode(), "Snoozing reminder for occurrence. ItemId: {0}", calendarItemOccurrence.Id);
					masterCalendarItem.Reminder.Snooze(ExDateTime.Now, newReminderTime.Value);
				}
				else if (masterCalendarItem.Reminder.IsSet)
				{
					ExTraceGlobals.PerformReminderActionCallTracer.TraceDebug<VersionedId>((long)this.GetHashCode(), "Dismissing reminder for occurrence with reminder set on master. ItemId: {0}", calendarItemOccurrence.Id);
					masterCalendarItem.Reminder.Dismiss(calendarItemOccurrence.OriginalStartTime);
				}
				else
				{
					if (calendarItemOccurrence.IsException)
					{
						ExTraceGlobals.PerformReminderActionCallTracer.TraceDebug<VersionedId>((long)this.GetHashCode(), "Dismissing reminder on exception with no reminder set on master. ItemId: {0}", calendarItemOccurrence.Id);
						item.Reminder.Disable();
						return item;
					}
					ExTraceGlobals.PerformReminderActionCallTracer.TraceWarning<VersionedId>((long)this.GetHashCode(), "Received a perform reminder action call for an occurrence with no reminder set on the master. ItemId: {0}", calendarItemOccurrence.Id);
				}
				return masterCalendarItem;
			}
			if (actionType == ReminderActionType.Dismiss)
			{
				item.Reminder.Dismiss(ExDateTime.Now);
			}
			else
			{
				item.Reminder.Snooze(ExDateTime.Now, newReminderTime.Value);
			}
			return item;
		}

		private ItemId PersistReminderChanges(Item item)
		{
			VersionedId id = item.Id;
			ConflictResolutionResult conflictResolutionResult = item.Save(SaveMode.ResolveConflicts);
			if (conflictResolutionResult.SaveStatus == SaveResult.IrresolvableConflict)
			{
				ExTraceGlobals.PerformReminderActionCallTracer.TraceError<string, string>((long)this.GetHashCode(), "Irresolvable Conflicts encountered when updating Reminder. SaveResult: {0}, ItemId: {1}", (conflictResolutionResult == null) ? "is null" : conflictResolutionResult.SaveStatus.ToString(), (item.Id == null) ? "is null" : item.Id.ToBase64String());
				throw new InvalidOperationException("Could not persist changes to Reminder.");
			}
			item.Load();
			if (item.Id == null)
			{
				ExTraceGlobals.PerformReminderActionCallTracer.TraceError<string>((long)this.GetHashCode(), "Could not retrieve Id of Item after Save(). Original Item Id: {0}", (id == null) ? "is null" : id.ToBase64String());
				throw new InvalidOperationException("Could not retrieve Id of Item after Save().");
			}
			return IdConverter.GetItemIdFromStoreId(item.Id, new MailboxId(base.MailboxIdentityMailboxSession));
		}
	}
}
