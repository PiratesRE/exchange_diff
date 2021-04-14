using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage.ReliableTimer
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ReliableTimer
	{
		public static void SetTimer(ReliableTimer.Feature feature, IItem item, ExDateTime timerValue, bool saveNow)
		{
			ExAssert.RetailAssert(item != null, "item is null");
			ExAssert.RetailAssert(timerValue != ReliableTimer.FiredTimerPropertyValue, "timerValue cannot be the Max value");
			ExTraceGlobals.ReliableTimerTracer.TraceDebug<ReliableTimer.Feature, ExDateTime, bool>(0L, "Setting reliable timer for feature={0}, timerValue={1}, saveNow={2}", feature, timerValue, saveNow);
			ExDateTime exDateTime = timerValue;
			ExDateTime utcNow = ExDateTime.UtcNow;
			if (utcNow > timerValue)
			{
				exDateTime = utcNow;
			}
			ExTraceGlobals.ReliableTimerTracer.TraceDebug<ReliableTimer.Feature, ExDateTime, ExDateTime>(0L, "After adjusting timer value for feature={0}, timerValue={1}, adjustedTimerValue={2}", feature, timerValue, exDateTime);
			StorePropertyDefinition propertyDefinition = ReliableTimer.featureToStorePropertyMapping[feature];
			item.SetOrDeleteProperty(propertyDefinition, exDateTime);
			if (saveNow)
			{
				item.Save(SaveMode.ResolveConflicts);
			}
		}

		public static void ClearTimer(ReliableTimer.Feature feature, IItem item, bool saveNow)
		{
			ExAssert.RetailAssert(item != null, "item is null");
			ExTraceGlobals.ReliableTimerTracer.TraceDebug<ReliableTimer.Feature, bool>(0L, "Clearing reliable timer for feature={0}, saveNow={1}", feature, saveNow);
			StorePropertyDefinition propertyDefinition = ReliableTimer.featureToStorePropertyMapping[feature];
			item.SetOrDeleteProperty(propertyDefinition, null);
			if (saveNow)
			{
				item.Save(SaveMode.ResolveConflicts);
			}
		}

		public static void ProcessTimerEvent(ReliableTimer.Feature feature, IItem item, Action handler)
		{
			ExAssert.RetailAssert(item != null, "item is null");
			ExAssert.RetailAssert(handler != null, "handler is null");
			ExTraceGlobals.ReliableTimerTracer.TraceDebug<ReliableTimer.Feature>(0L, "Processing timer event for feature={0}", feature);
			if (ReliableTimer.HasTimerFired(feature, item))
			{
				ExTraceGlobals.ReliableTimerTracer.TraceDebug<ReliableTimer.Feature>(0L, "Timer fired, calling handler for feature={0}", feature);
				handler();
				ReliableTimer.ClearTimer(feature, item, true);
			}
		}

		private static bool HasTimerFired(ReliableTimer.Feature feature, IItem item)
		{
			StorePropertyDefinition storePropertyDefinition = ReliableTimer.featureToStorePropertyMapping[feature];
			item.Load(new List<PropertyDefinition>
			{
				storePropertyDefinition
			});
			object obj = item.TryGetProperty(storePropertyDefinition);
			if (PropertyError.IsPropertyNotFound(obj))
			{
				ExTraceGlobals.ReliableTimerTracer.TraceDebug<ReliableTimer.Feature>(0L, "Timer not set for feature={0}", feature);
				return false;
			}
			if (PropertyError.IsPropertyError(obj))
			{
				PropertyError propertyError = (PropertyError)obj;
				ExTraceGlobals.ReliableTimerTracer.TraceError<ReliableTimer.Feature, PropertyErrorCode>(0L, "Property error for feature={0}, propertyErrorCode={1}", feature, propertyError.PropertyErrorCode);
				throw PropertyError.ToException(new PropertyError[]
				{
					propertyError
				});
			}
			ExDateTime exDateTime = (ExDateTime)obj;
			ExTraceGlobals.ReliableTimerTracer.TraceDebug<ReliableTimer.Feature, ExDateTime>(0L, "Timer value for feature={0}, timerValue={1}", feature, exDateTime);
			return exDateTime == ReliableTimer.FiredTimerPropertyValue;
		}

		internal static readonly Dictionary<ReliableTimer.Feature, StorePropertyDefinition> featureToStorePropertyMapping = new Dictionary<ReliableTimer.Feature, StorePropertyDefinition>
		{
			{
				ReliableTimer.Feature.EventEmailReminder,
				CalendarItemBaseSchema.EventEmailReminderTimer
			}
		};

		private static readonly ExDateTime FiredTimerPropertyValue = new ExDateTime(ExTimeZone.UtcTimeZone, ExDateTime.OutlookDateTimeMax);

		public enum Feature
		{
			EventEmailReminder
		}
	}
}
