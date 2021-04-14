using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Entities.Calendaring.TypeConversion.PropertyAccessors.StorageAccessors;
using Microsoft.Exchange.Entities.DataModel.Calendaring;
using Microsoft.Exchange.Entities.TypeConversion;
using Microsoft.Exchange.Entities.TypeConversion.PropertyAccessors.StorageAccessors;
using Microsoft.Exchange.Entities.TypeConversion.Translators;

namespace Microsoft.Exchange.Entities.Calendaring.TypeConversion.Translators
{
	internal class CalendarTranslator : StorageEntityTranslator<ICalendarFolder, Calendar, CalendarSchema>
	{
		protected CalendarTranslator(IEnumerable<ITranslationRule<ICalendarFolder, Calendar>> additionalRules = null) : base(CalendarTranslator.CreateTranslationRules().AddRules(additionalRules))
		{
		}

		public new static CalendarTranslator Instance
		{
			get
			{
				return CalendarTranslator.SingletonInstance;
			}
		}

		private static List<ITranslationRule<ICalendarFolder, Calendar>> CreateTranslationRules()
		{
			return new List<ITranslationRule<ICalendarFolder, Calendar>>
			{
				CalendarFolderAccessors.DisplayName.MapTo(Calendar.Accessors.Name),
				StoreObjectAccessors.RecordKey.MapTo(Calendar.Accessors.RecordKey)
			};
		}

		private static readonly CalendarTranslator SingletonInstance = new CalendarTranslator(null);
	}
}
