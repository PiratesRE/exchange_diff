using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class Reminder
	{
		internal Reminder(Item item)
		{
			this.item = item;
			this.SaveStateAsInitial(false);
		}

		public virtual ExDateTime? DueBy
		{
			get
			{
				ExDateTime? valueAsNullable;
				try
				{
					valueAsNullable = this.Item.GetValueAsNullable<ExDateTime>(InternalSchema.ReminderDueBy);
				}
				catch (PropertyErrorException ex)
				{
					if (ex.PropertyErrors.Length == 1 && ex.PropertyErrors[0].PropertyDefinition.Equals(InternalSchema.ReminderDueBy) && ex.PropertyErrors[0].PropertyErrorCode == PropertyErrorCode.GetCalculatedPropertyError)
					{
						throw new CorruptDataException(ex.LocalizedString, ex);
					}
					throw;
				}
				return valueAsNullable;
			}
			set
			{
				if (value != null)
				{
					this.Item[InternalSchema.ReminderDueBy] = value.Value;
					return;
				}
				throw new ArgumentNullException("value");
			}
		}

		public virtual bool IsSet
		{
			get
			{
				return this.Item.GetValueOrDefault<bool>(InternalSchema.ReminderIsSet);
			}
			set
			{
				this.Item[InternalSchema.ReminderIsSet] = value;
			}
		}

		public virtual int MinutesBeforeStart
		{
			get
			{
				return this.Item.GetValueOrDefault<int>(InternalSchema.ReminderMinutesBeforeStart);
			}
			set
			{
				throw this.PropertyNotSupported("MinutesBeforeStart");
			}
		}

		public static ExDateTime GetNominalReminderTimeForOccurrence(IStorePropertyBag recurringMasterItem, OccurrenceInfo occurrence)
		{
			ExceptionInfo exceptionInfo = occurrence as ExceptionInfo;
			int num;
			if (exceptionInfo != null && (exceptionInfo.ModificationType & ModificationType.ReminderDelta) == ModificationType.ReminderDelta)
			{
				num = Reminder.NormalizeMinutesBeforeStart(exceptionInfo.PropertyBag.GetValueOrDefault<int>(ItemSchema.ReminderMinutesBeforeStart, 15), 15);
			}
			else
			{
				num = Reminder.NormalizeMinutesBeforeStart(recurringMasterItem.GetValueOrDefault<int>(ItemSchema.ReminderMinutesBeforeStartInternal, 15), 15);
			}
			return occurrence.StartTime.AddMinutes((double)(-(double)num));
		}

		public virtual ExDateTime? ReminderNextTime
		{
			get
			{
				return StartTimeProperty.GetNormalizedTime(this.Item.PropertyBag, InternalSchema.ReminderNextTime, null);
			}
			protected set
			{
				if (value != null)
				{
					this.Item.LocationIdentifierHelperInstance.SetLocationIdentifier(63989U);
					this.Item[InternalSchema.ReminderNextTime] = value.Value;
					EndTimeProperty.DenormalizeTimeProperty(this.Item.PropertyBag, value.Value, InternalSchema.ReminderNextTime, null);
					return;
				}
				this.Item.LocationIdentifierHelperInstance.SetLocationIdentifier(37525U);
				this.Item.DeleteProperties(new PropertyDefinition[]
				{
					InternalSchema.ReminderNextTime
				});
			}
		}

		protected ExDateTime? DefaultReminderNextTime
		{
			get
			{
				return Reminder.GetDefaultReminderNextTime(this.DueBy, this.MinutesBeforeStart);
			}
		}

		protected Item Item
		{
			get
			{
				return this.item;
			}
		}

		protected ExDateTime MaxOutlookDate
		{
			get
			{
				return this.Item.Session.ExTimeZone.ConvertDateTime(Reminder.MaxOutlookDateUtc);
			}
		}

		private ExDateTime Now
		{
			get
			{
				return Reminder.GetTimeNow(this.item.PropertyBag.ExTimeZone);
			}
		}

		public virtual void Dismiss(ExDateTime actualizationTime)
		{
			ExDateTime probeTime = this.GetProbeTime(actualizationTime);
			Reminder.ReminderInfo nextPertinentItemInfo = this.GetNextPertinentItemInfo(probeTime);
			this.SetReminderTo(nextPertinentItemInfo);
			CalendarItemBase calendarItemBase = this.item as CalendarItemBase;
			if (calendarItemBase != null)
			{
				calendarItemBase.LocationIdentifierHelperInstance.SetLocationIdentifier(39413U, LastChangeAction.DismissReminder);
			}
		}

		public void Disable()
		{
			this.IsSet = false;
			this.MinutesBeforeStart = 0;
		}

		public Item GetPertinentItem(ExDateTime actualizationTime)
		{
			Reminder.ReminderInfo pertinentItemInfo = this.GetPertinentItemInfo(this.GetProbeTime(actualizationTime));
			if (pertinentItemInfo == null || pertinentItemInfo.PertinentItemId == null)
			{
				return null;
			}
			return Item.Bind(this.Item.Session, pertinentItemInfo.PertinentItemId);
		}

		public virtual void Snooze(ExDateTime actualizationTime, ExDateTime snoozeTime)
		{
			ExDateTime probeTime = this.GetProbeTime(actualizationTime);
			Reminder.ReminderInfo nextPertinentItemInfo = this.GetNextPertinentItemInfo(probeTime);
			if (probeTime < snoozeTime)
			{
				if (nextPertinentItemInfo == null || nextPertinentItemInfo.DefaultReminderNextTime > snoozeTime)
				{
					this.ReminderNextTime = new ExDateTime?(snoozeTime);
				}
				else
				{
					this.Dismiss(actualizationTime);
				}
				CalendarItemBase calendarItemBase = this.item as CalendarItemBase;
				if (calendarItemBase != null)
				{
					calendarItemBase.LocationIdentifierHelperInstance.SetLocationIdentifier(55797U, LastChangeAction.SnoozeReminder);
				}
			}
		}

		public void SnoozeBeforeDueBy(ExDateTime actualizationTime, TimeSpan beforeDueBy)
		{
			if (beforeDueBy.TotalMilliseconds < 0.0)
			{
				throw new ArgumentOutOfRangeException("beforeDueBy");
			}
			Reminder.ReminderInfo pertinentItemInfo = this.GetPertinentItemInfo(this.GetProbeTime(actualizationTime));
			if (pertinentItemInfo == null)
			{
				return;
			}
			ExDateTime exDateTime = pertinentItemInfo.DefaultDueBy;
			if (exDateTime - beforeDueBy > this.Now)
			{
				exDateTime -= beforeDueBy;
			}
			this.Snooze(actualizationTime, exDateTime);
		}

		public virtual void Adjust()
		{
			this.Adjust(this.Now);
		}

		internal static void Adjust(StoreObject storeObject)
		{
			Item item = storeObject as Item;
			if (item != null && item.Reminder != null)
			{
				item.LocationIdentifierHelperInstance.SetLocationIdentifier(43509U);
				item.Reminder.Adjust();
			}
		}

		internal static void EnsureMinutesBeforeStartIsInRange(Item item)
		{
			Reminder.EnsureMinutesBeforeStartIsInRange(item, 15);
		}

		internal static void EnsureMinutesBeforeStartIsInRange(Item item, int consumerDefaultMinutesBeforeStart)
		{
			int valueOrDefault = item.GetValueOrDefault<int>(InternalSchema.ReminderMinutesBeforeStart);
			int num = Reminder.NormalizeMinutesBeforeStart(valueOrDefault, consumerDefaultMinutesBeforeStart);
			if (valueOrDefault != num)
			{
				if (valueOrDefault != 1525252321 || !(item is MeetingMessage))
				{
					ExTraceGlobals.StorageTracer.TraceDebug<int, int, string>((long)item.GetHashCode(), "Value for ReminderMinutesBeforeStart is outside of the legitimate bounds: {0}, using {1} instead. Item class = {2}", valueOrDefault, num, item.ClassName);
				}
				item[InternalSchema.ReminderMinutesBeforeStart] = num;
			}
		}

		internal static int NormalizeMinutesBeforeStart(int minutesBeforeStart, int consumerDefaultMinutesBeforeStart)
		{
			if (minutesBeforeStart != 1525252321 && minutesBeforeStart >= 0 && minutesBeforeStart <= 2629800)
			{
				return minutesBeforeStart;
			}
			return consumerDefaultMinutesBeforeStart;
		}

		internal static ExDateTime GetProbeTime(ExDateTime actualizationTime, ExDateTime? reminderNextTime)
		{
			ExDateTime exDateTime = reminderNextTime ?? ExDateTime.MaxValue;
			if (!(actualizationTime > exDateTime))
			{
				return exDateTime;
			}
			return actualizationTime;
		}

		internal static ExDateTime GetTimeNow(ExTimeZone timeZone)
		{
			ExDateTime exDateTime = ExDateTime.GetNow(timeZone);
			Reminder.TestTimeHook testTimeHook = Reminder.testTimeHook;
			if (testTimeHook != null)
			{
				exDateTime = testTimeHook(exDateTime);
			}
			return exDateTime;
		}

		internal static Reminder.TestTimeHook SetTestTimeHook(Reminder.TestTimeHook newTestTimeHook)
		{
			Reminder.TestTimeHook result = Reminder.testTimeHook;
			Reminder.testTimeHook = newTestTimeHook;
			return result;
		}

		protected internal bool HasAcrAffectedReminders(ConflictResolutionResult acrResults)
		{
			PropertyDefinition[] array = new PropertyDefinition[]
			{
				InternalSchema.ReminderIsSetInternal,
				InternalSchema.ReminderDueByInternal,
				InternalSchema.ReminderNextTime
			};
			if (acrResults.SaveStatus == SaveResult.SuccessWithConflictResolution)
			{
				foreach (PropertyConflict propertyConflict in acrResults.PropertyConflicts)
				{
					foreach (PropertyDefinition obj in array)
					{
						if (propertyConflict.PropertyDefinition.Equals(obj))
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		protected internal virtual void SaveStateAsInitial(bool throwOnFailure)
		{
			this.lastSetTo = null;
			this.isSnoozed = false;
			if (this.ReminderNextTime != null)
			{
				try
				{
					Reminder.ReminderInfo pertinentItemInfo = this.GetPertinentItemInfo(this.ReminderNextTime.Value);
					this.isSnoozed = (pertinentItemInfo != null && this.ReminderNextTime != pertinentItemInfo.DefaultReminderNextTime);
					this.lastSetTo = pertinentItemInfo;
				}
				catch (CorruptDataException arg)
				{
					ExTraceGlobals.RecurrenceTracer.Information<Type, CorruptDataException>((long)this.GetHashCode(), "{0}.SaveStateAsInitial failed: {1}", base.GetType(), arg);
					if (throwOnFailure)
					{
						throw;
					}
				}
			}
		}

		protected static void Adjust(Reminder reminder, ExDateTime actualizationTime)
		{
			reminder.Adjust(actualizationTime);
		}

		protected static ExDateTime? GetDefaultReminderNextTime(ExDateTime? dueBy, int minutesBeforeStart)
		{
			if (dueBy == null)
			{
				return null;
			}
			int num = Reminder.NormalizeMinutesBeforeStart(minutesBeforeStart, 15);
			ExDateTime? result;
			try
			{
				if (dueBy <= ExDateTime.MinValue.AddMinutes((double)num))
				{
					num = 15;
				}
				result = new ExDateTime?(dueBy.Value.AddMinutes((double)(-(double)num)));
			}
			catch (ArgumentOutOfRangeException)
			{
				result = new ExDateTime?(dueBy.Value);
			}
			return result;
		}

		protected virtual void Adjust(ExDateTime actualizationTime)
		{
			this.EnsureRequiredPropertiesArePresent();
			Reminder.ReminderInfo pertinentItemInfo = this.GetPertinentItemInfo(actualizationTime);
			Reminder.ReminderInfo nextPertinentItemInfo = this.GetNextPertinentItemInfo(actualizationTime);
			Reminder.ReminderInfo reminderInfo;
			if (this.ReminderNextTime == null || this.lastSetTo == null || pertinentItemInfo == null)
			{
				reminderInfo = (nextPertinentItemInfo ?? pertinentItemInfo);
			}
			else if (nextPertinentItemInfo != null && this.ReminderNextTime >= nextPertinentItemInfo.DefaultReminderNextTime)
			{
				reminderInfo = nextPertinentItemInfo;
			}
			else if (this.ReminderNextTime >= pertinentItemInfo.DefaultReminderNextTime)
			{
				if (Reminder.ReminderInfo.IsForSamePertinentItem(pertinentItemInfo, this.lastSetTo))
				{
					if (this.lastSetTo.DefaultReminderNextTime != pertinentItemInfo.DefaultReminderNextTime || this.lastSetTo.DefaultDueBy != pertinentItemInfo.DefaultDueBy)
					{
						reminderInfo = pertinentItemInfo;
					}
					else
					{
						reminderInfo = null;
					}
				}
				else
				{
					reminderInfo = nextPertinentItemInfo;
				}
			}
			else
			{
				reminderInfo = null;
			}
			if (reminderInfo != null)
			{
				this.SetReminderTo(reminderInfo);
			}
		}

		protected virtual Reminder.ReminderInfo GetNextPertinentItemInfo(ExDateTime actualizationTime)
		{
			if (!this.IsSet || !(this.DefaultReminderNextTime > actualizationTime))
			{
				return null;
			}
			return this.GetPertinentItemInfo(this.DefaultReminderNextTime.Value);
		}

		protected virtual Reminder.ReminderInfo GetPertinentItemInfo(ExDateTime actualizationTime)
		{
			if (!this.IsSet || !(this.DefaultReminderNextTime <= actualizationTime))
			{
				return null;
			}
			return new Reminder.ReminderInfo(this.DueBy.Value, this.DefaultReminderNextTime.Value, this.Item.Id);
		}

		protected Exception PropertyNotSupported(string propertyName)
		{
			return new NotSupportedException(ServerStrings.ReminderPropertyNotSupported(this.Item.GetType().Name, propertyName));
		}

		private void EnsureRequiredPropertiesArePresent()
		{
			if (PropertyError.IsPropertyNotFound(this.Item.TryGetProperty(InternalSchema.ReminderIsSetInternal)))
			{
				this.Item.LocationIdentifierHelperInstance.SetLocationIdentifier(57205U);
				this.Item[InternalSchema.ReminderIsSetInternal] = false;
			}
			if (PropertyError.IsPropertyNotFound(this.Item.TryGetProperty(InternalSchema.ReminderMinutesBeforeStartInternal)))
			{
				this.Item.LocationIdentifierHelperInstance.SetLocationIdentifier(44917U);
				this.Item[InternalSchema.ReminderMinutesBeforeStartInternal] = 0;
			}
		}

		private ExDateTime GetProbeTime(ExDateTime actualizationTime)
		{
			return Reminder.GetProbeTime(actualizationTime, this.ReminderNextTime);
		}

		private void SetReminderTo(Reminder.ReminderInfo newPertinentItem)
		{
			if (newPertinentItem != null)
			{
				this.Item.LocationIdentifierHelperInstance.SetLocationIdentifier(61301U);
				this.Item[InternalSchema.ReminderIsSetInternal] = true;
				this.Item[InternalSchema.ReminderDueByInternal] = newPertinentItem.DefaultDueBy;
				this.ReminderNextTime = new ExDateTime?(newPertinentItem.DefaultReminderNextTime);
			}
			else
			{
				this.Item.LocationIdentifierHelperInstance.SetLocationIdentifier(53109U);
				this.Item[InternalSchema.ReminderIsSetInternal] = false;
				this.Item[InternalSchema.TaskResetReminder] = true;
			}
			this.lastSetTo = newPertinentItem;
			this.isSnoozed = false;
		}

		public const int DefaultMinutesBeforeStart = 15;

		public const int MarkerForDefaultMinutesBeforeStart = 1525252321;

		public const int MinutesBeforeStartMin = 0;

		public const int MinutesBeforeStartMax = 2629800;

		internal static ExDateTime MaxOutlookDateUtc = new ExDateTime(ExTimeZone.UtcTimeZone, new DateTime(4501, 1, 1));

		protected Reminder.ReminderInfo lastSetTo;

		protected bool isSnoozed;

		private readonly Item item;

		private static Reminder.TestTimeHook testTimeHook;

		internal delegate ExDateTime TestTimeHook(ExDateTime localNow);

		protected class ReminderInfo
		{
			public ReminderInfo(ExDateTime defaultDueBy, ExDateTime defaultReminderNextTime, StoreId pertinentItemId)
			{
				this.DefaultDueBy = defaultDueBy;
				this.DefaultReminderNextTime = defaultReminderNextTime;
				this.PertinentItemId = pertinentItemId;
			}

			public static bool IsForSamePertinentItem(Reminder.ReminderInfo v1, Reminder.ReminderInfo v2)
			{
				bool? flag = v1.IsForSamePertinentItem(v2);
				if (flag == null)
				{
					return v2.IsForSamePertinentItem(v1) ?? true;
				}
				return flag.GetValueOrDefault();
			}

			protected virtual bool? IsForSamePertinentItem(Reminder.ReminderInfo reminderInfo)
			{
				return null;
			}

			public readonly ExDateTime DefaultDueBy;

			public readonly ExDateTime DefaultReminderNextTime;

			public readonly StoreId PertinentItemId;
		}
	}
}
