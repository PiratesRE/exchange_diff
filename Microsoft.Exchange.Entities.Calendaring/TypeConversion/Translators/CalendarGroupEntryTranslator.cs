using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Entities.Calendaring.TypeConversion.PropertyAccessors.StorageAccessors;
using Microsoft.Exchange.Entities.DataModel.Calendaring;
using Microsoft.Exchange.Entities.TypeConversion;
using Microsoft.Exchange.Entities.TypeConversion.Translators;

namespace Microsoft.Exchange.Entities.Calendaring.TypeConversion.Translators
{
	internal class CalendarGroupEntryTranslator : StorageEntityTranslator<ICalendarGroupEntry, Calendar, CalendarSchema>
	{
		protected CalendarGroupEntryTranslator(IEnumerable<ITranslationRule<ICalendarGroupEntry, Calendar>> additionalRules = null) : base(CalendarGroupEntryTranslator.CreateTranslationRules().AddRules(additionalRules))
		{
		}

		public new static CalendarGroupEntryTranslator Instance
		{
			get
			{
				return CalendarGroupEntryTranslator.SingletonInstance;
			}
		}

		private static List<ITranslationRule<ICalendarGroupEntry, Calendar>> CreateTranslationRules()
		{
			return new List<ITranslationRule<ICalendarGroupEntry, Calendar>>
			{
				CalendarGroupEntryAccessors.CalendarColor.MapTo(Calendar.Accessors.Color),
				CalendarGroupEntryAccessors.CalendarId.MapTo(Calendar.Accessors.CalendarFolderStoreId),
				CalendarGroupEntryAccessors.CalendarName.MapTo(Calendar.Accessors.Name),
				CalendarGroupEntryAccessors.CalendarRecordKey.MapTo(Calendar.Accessors.RecordKey)
			};
		}

		private static readonly CalendarGroupEntryTranslator SingletonInstance = new CalendarGroupEntryTranslator(null);
	}
}
