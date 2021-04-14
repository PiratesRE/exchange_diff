using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.SoapWebClient.EWS;

namespace Microsoft.Exchange.InfoWorker.Common.Sharing
{
	internal class CalendarItemSynchronizer : ItemSynchronizer
	{
		public CalendarItemSynchronizer(LocalFolder localFolder, CultureInfo clientCulture) : base(localFolder)
		{
			this.appointmentTranslator = new AppointmentTranslator(clientCulture);
		}

		public override void Sync(ItemType item, MailboxSession mailboxSession, ExchangeService exchangeService)
		{
			CalendarItemType calendarItemType = item as CalendarItemType;
			if (calendarItemType == null)
			{
				ItemSynchronizer.Tracer.TraceDebug<CalendarItemSynchronizer, string>((long)this.GetHashCode(), "{0}: Found non-Calendar item in a Calendar folder: {1}. Skipping.", this, item.ItemId.Id);
				return;
			}
			CalendarItem calendarItem = base.FindLocalCopy(calendarItemType.ItemId.Id, mailboxSession) as CalendarItem;
			if (calendarItem == null)
			{
				ItemSynchronizer.Tracer.TraceDebug<CalendarItemSynchronizer, string>((long)this.GetHashCode(), "{0}: Item not found locally {1}.", this, calendarItemType.ItemId.Id);
				this.appointmentTranslator.CreateAppointment(exchangeService, mailboxSession, calendarItemType, this.localFolder);
			}
			else
			{
				try
				{
					this.appointmentTranslator.UpdateAppointment(exchangeService, mailboxSession, calendarItemType, calendarItem);
				}
				catch (LastOccurrenceDeletionException arg)
				{
					ItemSynchronizer.Tracer.TraceDebug<CalendarItemSynchronizer, string, LastOccurrenceDeletionException>((long)this.GetHashCode(), "{0}: All occurrences in the series were deleted - {1}, Error - {2}, We will delete the local item.", this, calendarItemType.ItemId.Id, arg);
					this.localFolder.SelectItemToDelete(calendarItem.Id.ObjectId);
				}
				finally
				{
					calendarItem.Dispose();
				}
			}
			PerformanceCounters.CalendarItemsSynced.Increment();
		}

		protected override Item Bind(MailboxSession mailboxSession, StoreId storeId)
		{
			return CalendarItem.Bind(mailboxSession, storeId);
		}

		public override void EnforceLevelOfDetails(MailboxSession mailboxSession, StoreId itemId, LevelOfDetails levelOfDetails)
		{
			using (CalendarItem calendarItem = CalendarItem.Bind(mailboxSession, itemId))
			{
				this.EnforceLevelOfDetailsOnItem(calendarItem, levelOfDetails);
				if (calendarItem.Recurrence != null)
				{
					IList<OccurrenceInfo> modifiedOccurrences = calendarItem.Recurrence.GetModifiedOccurrences();
					ItemSynchronizer.Tracer.TraceDebug<CalendarItemSynchronizer, int, StoreId>((long)this.GetHashCode(), "{0}: Found {1} modified occurrences for item {2}.", this, modifiedOccurrences.Count, itemId);
					foreach (OccurrenceInfo occurrenceInfo in modifiedOccurrences)
					{
						ExDateTime originalStartTime = occurrenceInfo.OriginalStartTime;
						if (!calendarItem.Recurrence.IsOccurrenceDeleted(originalStartTime) && calendarItem.Recurrence.IsValidOccurrenceId(originalStartTime))
						{
							using (CalendarItemOccurrence calendarItemOccurrence = calendarItem.OpenOccurrenceByOriginalStartTime(originalStartTime, new PropertyDefinition[0]))
							{
								this.EnforceLevelOfDetailsOnItem(calendarItemOccurrence, levelOfDetails);
								calendarItemOccurrence.Save(SaveMode.NoConflictResolution);
								continue;
							}
						}
						ItemSynchronizer.Tracer.TraceDebug<CalendarItemSynchronizer, ExDateTime>((long)this.GetHashCode(), "{0}: Exception {1} is deleted or invalid. Skipping enforcement of level of details.", this, originalStartTime);
					}
				}
				calendarItem.Save(SaveMode.NoConflictResolution);
			}
		}

		private void EnforceLevelOfDetailsOnItem(CalendarItemBase calendarItem, LevelOfDetails levelOfDetails)
		{
			ItemSynchronizer.Tracer.TraceDebug<CalendarItemSynchronizer, VersionedId>((long)this.GetHashCode(), "{0}: Enforcing level of details on item {1}.", this, calendarItem.Id);
			using (TextWriter textWriter = calendarItem.Body.OpenTextWriter(calendarItem.Body.Format))
			{
				textWriter.Write(string.Empty);
			}
			if (levelOfDetails == LevelOfDetails.Availability)
			{
				calendarItem.Subject = this.appointmentTranslator.ConvertFreeBusyToTitle(calendarItem.FreeBusyStatus);
				calendarItem.Location = string.Empty;
				calendarItem.Sensitivity = Sensitivity.Normal;
			}
		}

		private AppointmentTranslator appointmentTranslator;
	}
}
