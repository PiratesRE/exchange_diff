using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.ContentTypes.iCalendar;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class VCalendar : CalendarComponentBase
	{
		internal VCalendar(ICalContext context) : base(context)
		{
			Util.ThrowOnNullArgument(context, "context");
		}

		internal IEnumerable<Item> Promote(Func<Item> getItem)
		{
			if (this.vEvents == null)
			{
				this.vEvents = new List<VEvent>();
			}
			if (this.exceptions != null && this.exceptions.Count > 0)
			{
				this.PreProcessExceptions();
			}
			foreach (VEvent vEvent in this.vEvents)
			{
				if (vEvent.Validate())
				{
					Item item = getItem();
					if (vEvent.Promote(item, this.exceptions))
					{
						if (base.Context.Method == CalendarMethod.Publish)
						{
							base.InboundContext.AddressCache.Resolve();
							base.InboundContext.AddressCache.CopyDataToItem(item.CoreItem);
							base.InboundContext.AddressCache.ClearRecipients();
						}
						yield return item;
					}
				}
			}
			if (this.vTodos == null)
			{
				this.vTodos = new List<VTodo>();
			}
			foreach (VTodo vTodo in this.vTodos)
			{
				if (vTodo.Validate())
				{
					Item item2 = getItem();
					if (vTodo.Promote(item2))
					{
						yield return item2;
					}
				}
			}
			yield break;
		}

		internal void Demote(IList<Item> items)
		{
			Util.ThrowOnNullArgument(items, "items");
			this.calendarWriter = base.OutboundContext.Writer;
			VCalendar.GetCalendarMethodAndType(items, out base.Context.Method, out base.Context.Type);
			this.calendarWriter.StartComponent(ComponentId.VCalendar);
			this.DemoteProperties();
			if (items.Count > 0)
			{
				this.demotingTimeZones = VCalendar.CollectTimeZones(items);
				this.DemoteTimeZones();
				this.DemoteVEvents(items);
			}
			this.calendarWriter.EndComponent();
		}

		protected override void ProcessProperty(CalendarPropertyBase calendarProperty)
		{
			PropertyId propertyId = calendarProperty.CalendarPropertyId.PropertyId;
			if (propertyId == PropertyId.Unknown)
			{
				if (string.Compare(calendarProperty.CalendarPropertyId.PropertyName, "X-MICROSOFT-CALSCALE", StringComparison.CurrentCultureIgnoreCase) == 0)
				{
					CalendarType? calendarType = CalendarUtil.CalendarTypeFromString((string)calendarProperty.Value);
					if (calendarType != null)
					{
						base.Context.Type = calendarType.Value;
						return;
					}
				}
				else if (string.Compare(calendarProperty.CalendarPropertyId.PropertyName, "X-WR-CALNAME", StringComparison.CurrentCultureIgnoreCase) == 0)
				{
					base.Context.CalendarName = (string)calendarProperty.Value;
				}
				return;
			}
			if (propertyId != PropertyId.Method)
			{
				return;
			}
			string strA = (string)calendarProperty.Value;
			if (string.Compare(strA, "PUBLISH", StringComparison.CurrentCultureIgnoreCase) == 0)
			{
				base.Context.Method = CalendarMethod.Publish;
				return;
			}
			if (string.Compare(strA, "REQUEST", StringComparison.CurrentCultureIgnoreCase) == 0)
			{
				base.Context.Method = CalendarMethod.Request;
				return;
			}
			if (string.Compare(strA, "CANCEL", StringComparison.CurrentCultureIgnoreCase) == 0)
			{
				base.Context.Method = CalendarMethod.Cancel;
				return;
			}
			if (string.Compare(strA, "REFRESH", StringComparison.CurrentCultureIgnoreCase) == 0)
			{
				base.Context.Method = CalendarMethod.Refresh;
				return;
			}
			if (string.Compare(strA, "REPLY", StringComparison.CurrentCultureIgnoreCase) == 0)
			{
				base.Context.Method = CalendarMethod.Reply;
				return;
			}
			if (string.Compare(strA, "COUNTER", StringComparison.CurrentCultureIgnoreCase) == 0)
			{
				base.Context.Method = CalendarMethod.Counter;
				return;
			}
			base.Context.Method = CalendarMethod.None;
		}

		protected override bool ProcessSubComponent(CalendarComponentBase calendarComponent)
		{
			bool result = true;
			ComponentId componentId = calendarComponent.ComponentId;
			if (componentId != ComponentId.VEvent)
			{
				if (componentId != ComponentId.VTodo)
				{
					if (componentId == ComponentId.VTimeZone)
					{
						VTimeZone vtimeZone = (VTimeZone)calendarComponent;
						if (!vtimeZone.Validate())
						{
							result = false;
						}
						else
						{
							ExTimeZone exTimeZone = vtimeZone.Promote();
							if (exTimeZone == null)
							{
								result = false;
							}
							else if (base.InboundContext.DeclaredTimeZones.ContainsKey(exTimeZone.AlternativeId))
							{
								ExTraceGlobals.ICalTracer.TraceError<string>((long)this.GetHashCode(), "VCalendar::ProcessSubComponent. Duplicate time zone id in VTIMEZONE. TZID:'{0}'.", exTimeZone.AlternativeId);
								base.Context.AddError(ServerStrings.InvalidICalElement("VTIMEZONE"));
								result = false;
							}
							else
							{
								base.InboundContext.DeclaredTimeZones.Add(exTimeZone.AlternativeId, exTimeZone);
							}
						}
					}
				}
				else
				{
					if (this.vTodos == null)
					{
						this.vTodos = new List<VTodo>();
					}
					this.vTodos.Add((VTodo)calendarComponent);
				}
			}
			else
			{
				VEvent vevent = (VEvent)calendarComponent;
				if (vevent.HasRecurrenceId)
				{
					if (this.exceptions == null)
					{
						this.exceptions = new List<VEvent>();
					}
					this.exceptions.Add(vevent);
				}
				else
				{
					if (this.vEvents == null)
					{
						this.vEvents = new List<VEvent>();
					}
					this.vEvents.Add(vevent);
				}
			}
			return result;
		}

		protected override bool ValidateProperties()
		{
			if (base.Context.Method == CalendarMethod.None)
			{
				ExTraceGlobals.ICalTracer.TraceError<CalendarMethod>((long)this.GetHashCode(), "VCalendar::ValidateProperties. Invalid calendar method. METHOD:'{0}'.", base.Context.Method);
				base.Context.AddError(ServerStrings.InvalidICalElement("METHOD"));
				return false;
			}
			return true;
		}

		protected override bool ValidateStructure()
		{
			int num = (this.vEvents != null) ? this.vEvents.Count : 0;
			int num2 = (this.exceptions != null) ? this.exceptions.Count : 0;
			int num3 = (this.vTodos != null) ? this.vTodos.Count : 0;
			if (base.Context.Method != CalendarMethod.Publish)
			{
				if (num == 0 && num2 == 0 && num3 == 0)
				{
					ExTraceGlobals.ICalTracer.TraceError((long)this.GetHashCode(), "VCalendar::ValidateStructure. No VEVENT found for meeting message.");
					base.Context.AddError(ServerStrings.InvalidICalElement("VEVENT"));
					return false;
				}
			}
			else if (num + num2 + num3 > StorageLimits.Instance.CalendarMaxNumberVEventsForICalImport)
			{
				ExTraceGlobals.ICalTracer.TraceError((long)this.GetHashCode(), "VCalendar::ValidateStructure. Calendar over the limit of VEvents. Skipping.");
				base.Context.AddError(new LocalizedString("Calendar over the limit of VEvents. Skipping."));
				return false;
			}
			return base.ValidateTimeZoneInfo(false) && base.ValidateStructure();
		}

		private static void GetCalendarMethodAndType(IList<Item> items, out CalendarMethod calendarMethod, out CalendarType calendarType)
		{
			CalendarMethod? calendarMethod2 = null;
			CalendarType? calendarType2 = null;
			if (items.Count > 0)
			{
				foreach (Item item in items)
				{
					CalendarMethod icalMethod = CalendarUtil.GetICalMethod(item);
					if (icalMethod == CalendarMethod.None)
					{
						throw new ConversionFailedException(ConversionFailureReason.ConverterInternalFailure);
					}
					if (calendarMethod2 != null && icalMethod != calendarMethod2.Value)
					{
						throw new InvalidOperationException(ServerStrings.InconsistentCalendarMethod(calendarMethod2.Value.ToString(), item.Id.ToString()));
					}
					calendarMethod2 = new CalendarMethod?(icalMethod);
					CalendarType alternateCalendarType = VEvent.GetAlternateCalendarType(item);
					if (calendarType2 != null && alternateCalendarType != calendarType2)
					{
						throw new InvalidOperationException(ServerStrings.InconsistentCalendarMethod(calendarType2.Value.ToString(), item.Id.ToString()));
					}
					calendarType2 = new CalendarType?(alternateCalendarType);
				}
			}
			calendarMethod = ((calendarMethod2 != null) ? calendarMethod2.Value : CalendarMethod.Publish);
			calendarType = ((calendarType2 != null) ? calendarType2.Value : CalendarType.Default);
		}

		private static Dictionary<REG_TIMEZONE_INFO, string> CollectTimeZones(IList<Item> items)
		{
			Dictionary<REG_TIMEZONE_INFO, string> dictionary = new Dictionary<REG_TIMEZONE_INFO, string>();
			foreach (Item item in items)
			{
				ExTimeZone exTimeZoneFromItem = TimeZoneHelper.GetExTimeZoneFromItem(item);
				REG_TIMEZONE_INFO key = TimeZoneHelper.RegTimeZoneInfoFromExTimeZone(exTimeZoneFromItem);
				string text = CalendarUtil.RemoveDoubleQuotes(string.IsNullOrEmpty(exTimeZoneFromItem.AlternativeId) ? exTimeZoneFromItem.DisplayName : exTimeZoneFromItem.AlternativeId);
				if (!string.IsNullOrEmpty(text) && text.EndsWith("\r\n"))
				{
					text = text.Substring(0, text.Length - 2);
				}
				string value = text;
				if (!dictionary.ContainsKey(key))
				{
					int num = 0;
					while (string.IsNullOrEmpty(value) || dictionary.ContainsValue(value))
					{
						value = string.Format("{0} {1}", text, ++num);
					}
					dictionary.Add(key, value);
				}
			}
			return dictionary;
		}

		private void PreProcessExceptions()
		{
			if (base.InboundContext.HasExceptionPromotion)
			{
				this.vEvents.AddRange(this.exceptions);
				this.exceptions.Clear();
				return;
			}
			for (int i = this.exceptions.Count - 1; i >= 0; i--)
			{
				VEvent exception = this.exceptions[i];
				if (!this.vEvents.Any((VEvent e) => e.HasRRule && e.Uid == exception.Uid))
				{
					this.vEvents.Add(exception);
					this.exceptions.RemoveAt(i);
				}
			}
		}

		private void DemoteProperties()
		{
			this.calendarWriter.WriteProperty(PropertyId.Method, CalendarUtil.CalendarMethodToString(base.Context.Method));
			this.calendarWriter.WriteProperty(PropertyId.ProductId, "Microsoft Exchange Server 2010");
			this.calendarWriter.WriteProperty(PropertyId.Version, "2.0");
			if (!string.IsNullOrEmpty(base.Context.CalendarName))
			{
				this.calendarWriter.StartProperty("X-WR-CALNAME");
				this.calendarWriter.WritePropertyValue(base.Context.CalendarName);
			}
			if (base.Context.Type != CalendarType.Default)
			{
				this.calendarWriter.StartProperty("X-MICROSOFT-CALSCALE");
				this.calendarWriter.WritePropertyValue(CalendarUtil.CalendarTypeToString(base.Context.Type));
			}
		}

		private void DemoteTimeZones()
		{
			foreach (REG_TIMEZONE_INFO reg_TIMEZONE_INFO in this.demotingTimeZones.Keys)
			{
				string timeZoneId = this.demotingTimeZones[reg_TIMEZONE_INFO];
				VTimeZone vtimeZone = new VTimeZone(this, reg_TIMEZONE_INFO, timeZoneId);
				vtimeZone.Demote();
			}
		}

		private void DemoteVEvents(IList<Item> items)
		{
			foreach (Item item in items)
			{
				ExTimeZone exTimeZone = item.PropertyBag.ExTimeZone;
				try
				{
					REG_TIMEZONE_INFO reg_TIMEZONE_INFO = TimeZoneHelper.RegTimeZoneInfoFromExTimeZone(TimeZoneHelper.GetExTimeZoneFromItem(item));
					string text = this.demotingTimeZones[reg_TIMEZONE_INFO];
					item.PropertyBag.ExTimeZone = TimeZoneHelper.CreateCustomExTimeZoneFromRegTimeZoneInfo(reg_TIMEZONE_INFO, text, text);
					VEvent vevent = new VEvent(this);
					vevent.Demote(item);
					if (!base.OutboundContext.SuppressExceptionAndAttachmentDemotion && (base.Context.Method == CalendarMethod.Request || base.Context.Method == CalendarMethod.Cancel || base.Context.Method == CalendarMethod.Publish))
					{
						InternalRecurrence recurrenceFromItem = CalendarItem.GetRecurrenceFromItem(item);
						if (recurrenceFromItem != null)
						{
							IList<OccurrenceInfo> modifiedOccurrences = recurrenceFromItem.GetModifiedOccurrences();
							foreach (OccurrenceInfo occurrenceInfo in modifiedOccurrences)
							{
								ExceptionInfo exceptionInfo = (ExceptionInfo)occurrenceInfo;
								VEvent vevent2 = new VEvent(this);
								vevent2.DemoteException(exceptionInfo, vevent);
							}
						}
					}
				}
				catch (Exception ex)
				{
					ExTraceGlobals.ICalTracer.TraceError<Exception>((long)this.GetHashCode(), "VCalendar::DemoteVEvents. Skipping item due to {0}", ex);
					base.Context.AddError(ServerStrings.InvalidICalElement(ex.ToString()));
				}
				finally
				{
					item.PropertyBag.ExTimeZone = exTimeZone;
				}
			}
		}

		private const string IcalProductId = "Microsoft Exchange Server 2010";

		private List<VEvent> vEvents;

		private List<VEvent> exceptions;

		private List<VTodo> vTodos;

		private CalendarWriter calendarWriter;

		private Dictionary<REG_TIMEZONE_INFO, string> demotingTimeZones;
	}
}
