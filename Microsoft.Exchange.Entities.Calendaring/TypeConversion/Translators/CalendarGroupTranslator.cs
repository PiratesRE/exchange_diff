using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Entities.Calendaring.TypeConversion.PropertyAccessors.StorageAccessors;
using Microsoft.Exchange.Entities.DataModel.Calendaring;
using Microsoft.Exchange.Entities.TypeConversion;
using Microsoft.Exchange.Entities.TypeConversion.Translators;

namespace Microsoft.Exchange.Entities.Calendaring.TypeConversion.Translators
{
	internal class CalendarGroupTranslator : StorageEntityTranslator<ICalendarGroup, CalendarGroup, CalendarGroupSchema>
	{
		protected CalendarGroupTranslator(IEnumerable<ITranslationRule<ICalendarGroup, CalendarGroup>> additionalRules = null) : base(CalendarGroupTranslator.CreateTranslationRules().AddRules(additionalRules))
		{
		}

		public new static CalendarGroupTranslator Instance
		{
			get
			{
				return CalendarGroupTranslator.SingletonInstance;
			}
		}

		private static List<ITranslationRule<ICalendarGroup, CalendarGroup>> CreateTranslationRules()
		{
			return new List<ITranslationRule<ICalendarGroup, CalendarGroup>>
			{
				CalendarGroupAccessors.GroupClassId.MapTo(CalendarGroup.Accessors.ClassId),
				CalendarGroupAccessors.GroupName.MapTo(CalendarGroup.Accessors.Name)
			};
		}

		private static readonly CalendarGroupTranslator SingletonInstance = new CalendarGroupTranslator(null);
	}
}
