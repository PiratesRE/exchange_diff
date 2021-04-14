using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Entities.Calendaring.TypeConversion.PropertyAccessors.StorageAccessors;
using Microsoft.Exchange.Entities.DataModel.BirthdayCalendar;
using Microsoft.Exchange.Entities.TypeConversion;
using Microsoft.Exchange.Entities.TypeConversion.PropertyAccessors.StorageAccessors;
using Microsoft.Exchange.Entities.TypeConversion.Translators;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Entities.BirthdayCalendar.TypeConversion.Translators
{
	internal class BirthdayEventTranslator : StorageEntityTranslator<ICalendarItemBase, BirthdayEvent, BirthdayEventSchema>
	{
		protected BirthdayEventTranslator(IEnumerable<ITranslationRule<ICalendarItemBase, IBirthdayEvent>> additionalRules = null) : base(BirthdayEventTranslator.CreateTranslationRules().AddRules(additionalRules))
		{
		}

		public new static BirthdayEventTranslator Instance
		{
			get
			{
				return BirthdayEventTranslator.SingletonInstance;
			}
		}

		private static List<ITranslationRule<ICalendarItemBase, BirthdayEvent>> CreateTranslationRules()
		{
			return new List<ITranslationRule<ICalendarItemBase, BirthdayEvent>>
			{
				ItemAccessors<ICalendarItemBase>.Subject.MapTo(BirthdayEvent.Accessors.Subject),
				CalendarItemAccessors.BirthdayContactAttributionDisplayName.MapTo(BirthdayEvent.Accessors.Attribution),
				CalendarItemAccessors.Birthday.MapTo(BirthdayEvent.Accessors.Birthday),
				CalendarItemAccessors.BirthdayContactPersonId.MapTo(BirthdayEvent.Accessors.PersonId),
				CalendarItemAccessors.BirthdayContactId.MapTo(BirthdayEvent.Accessors.ContactId),
				CalendarItemAccessors.IsBirthdayContactWritable.MapTo(BirthdayEvent.Accessors.IsWritable)
			};
		}

		private static readonly BirthdayEventTranslator SingletonInstance = new BirthdayEventTranslator(null);
	}
}
