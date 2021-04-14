using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Entities.DataModel.Calendaring.Recurrence;
using Microsoft.Exchange.Entities.DataProviders;
using Microsoft.Exchange.Entities.TypeConversion.Converters;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Entities.Calendaring.TypeConversion.Converters.Recurrence
{
	internal class RecurrenceConverter : IConverter<Recurrence, PatternedRecurrence>, IConverter<PatternedRecurrence, Recurrence>
	{
		public RecurrenceConverter(ExTimeZone timeZone)
		{
			this.timeZone = RecurrenceConverter.GetSafeTimeZoneForRecurrence(timeZone);
		}

		private static ExTimeZone GmtTimeZone
		{
			get
			{
				return RecurrenceConverter.GmtTimeZoneLazy.Value;
			}
		}

		public Recurrence Convert(PatternedRecurrence value)
		{
			Recurrence result;
			if (value == null)
			{
				result = null;
			}
			else
			{
				RecurrencePattern recurrencePattern = RecurrenceConverter.PatternConverter.Convert(value.Pattern);
				if (recurrencePattern == null)
				{
					throw new InvalidRequestException(Strings.ErrorMissingRequiredParameter("Pattern"));
				}
				RecurrenceRange recurrenceRange = RecurrenceConverter.RangeConverter.Convert(value.Range);
				if (recurrenceRange == null)
				{
					throw new InvalidRequestException(Strings.ErrorMissingRequiredParameter("Range"));
				}
				result = ((this.timeZone == null) ? new Recurrence(recurrencePattern, recurrenceRange) : new Recurrence(recurrencePattern, recurrenceRange, this.timeZone, this.timeZone));
			}
			return result;
		}

		public PatternedRecurrence Convert(Recurrence value)
		{
			if (value != null)
			{
				return new PatternedRecurrence
				{
					Pattern = RecurrenceConverter.PatternConverter.Convert(value.Pattern),
					Range = RecurrenceConverter.RangeConverter.Convert(value.Range)
				};
			}
			return null;
		}

		private static ExTimeZone GetSafeTimeZoneForRecurrence(ExTimeZone timeZone)
		{
			if (timeZone != ExTimeZone.UtcTimeZone)
			{
				return timeZone;
			}
			return RecurrenceConverter.GmtTimeZone;
		}

		private static ExTimeZone LoadGmtTimeZone()
		{
			ExTimeZone result;
			if (!ExTimeZoneEnumerator.Instance.TryGetTimeZoneByName("Greenwich Standard Time", out result))
			{
				ExTimeZoneInformation exTimeZoneInformation = new ExTimeZoneInformation("Greenwich Standard Time", "Greenwich Standard Time");
				ExTimeZoneRuleGroup exTimeZoneRuleGroup = new ExTimeZoneRuleGroup(null);
				ExTimeZoneRule ruleInfo = new ExTimeZoneRule("Standard", "Standard", new TimeSpan(0L), null);
				exTimeZoneRuleGroup.AddRule(ruleInfo);
				exTimeZoneInformation.AddGroup(exTimeZoneRuleGroup);
				result = new ExTimeZone(exTimeZoneInformation);
			}
			return result;
		}

		private const string GreenwichStandardTime = "Greenwich Standard Time";

		private const string StandardTimeZoneRule = "Standard";

		private static readonly Lazy<ExTimeZone> GmtTimeZoneLazy = new Lazy<ExTimeZone>(new Func<ExTimeZone>(RecurrenceConverter.LoadGmtTimeZone), true);

		private static readonly PatternConverter PatternConverter = new PatternConverter();

		private static readonly RangeConverter RangeConverter = new RangeConverter();

		private readonly ExTimeZone timeZone;
	}
}
