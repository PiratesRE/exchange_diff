using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Entities.TypeConversion.PropertyAccessors;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Entities.BirthdayCalendar.TypeConversion.StorageAccessors
{
	internal static class ContactAccessors
	{
		public static readonly IStoragePropertyAccessor<IContact, string> DisplayName = new DefaultStoragePropertyAccessor<IContact, string>(StoreObjectSchema.DisplayName, false);

		public static readonly IStoragePropertyAccessor<IContact, string> Attribution = new DefaultStoragePropertyAccessor<IContact, string>(ContactSchema.AttributionDisplayName, false);

		public static readonly IStoragePropertyAccessor<IContact, PersonId> PersonId = new DefaultStoragePropertyAccessor<IContact, PersonId>(ContactSchema.PersonId, false);

		public static readonly IStoragePropertyAccessor<IContact, bool> NotInBirthdayCalendar = new DefaultStoragePropertyAccessor<IContact, bool>(ContactSchema.NotInBirthdayCalendar, false);

		public static readonly IStoragePropertyAccessor<IContact, bool> IsWritable = new DefaultStoragePropertyAccessor<IContact, bool>(ContactSchema.IsWritable, false);

		public static readonly IStoragePropertyAccessor<IContact, ExDateTime?> Birthday = new DelegatedStoragePropertyAccessor<IContact, ExDateTime?>(delegate(IContact contact, out ExDateTime? birthday)
		{
			object obj = contact.TryGetProperty(ContactSchema.BirthdayLocal);
			if (obj is ExDateTime)
			{
				birthday = new ExDateTime?((ExDateTime)obj);
			}
			else
			{
				birthday = null;
			}
			return true;
		}, delegate(IContact contact, ExDateTime? newBirthday)
		{
			if (newBirthday != null)
			{
				contact[ContactSchema.BirthdayLocal] = newBirthday.Value;
			}
		}, null, null, new PropertyDefinition[0]);
	}
}
