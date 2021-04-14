using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Entities.BirthdayCalendar;
using Microsoft.Exchange.Entities.DataModel.PropertyBags;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Entities.DataModel.BirthdayCalendar
{
	public class BirthdayContact : StorageEntity<BirthdayContactSchema>, IBirthdayContactInternal, IBirthdayContact, IStorageEntity, IEntity, IPropertyChangeTracker<PropertyDefinition>, IVersioned
	{
		public string DisplayName
		{
			get
			{
				return base.GetPropertyValueOrDefault<string>(base.Schema.DisplayNameProperty);
			}
			set
			{
				base.SetPropertyValue<string>(base.Schema.DisplayNameProperty, value);
			}
		}

		public string Attribution
		{
			get
			{
				return base.GetPropertyValueOrDefault<string>(base.Schema.AttributionProperty);
			}
			set
			{
				base.SetPropertyValue<string>(base.Schema.AttributionProperty, value);
			}
		}

		public ExDateTime? Birthday
		{
			get
			{
				return base.GetPropertyValueOrDefault<ExDateTime?>(base.Schema.BirthdayProperty);
			}
			set
			{
				base.SetPropertyValue<ExDateTime?>(base.Schema.BirthdayProperty, value);
			}
		}

		PersonId IBirthdayContactInternal.PersonId
		{
			get
			{
				return base.GetPropertyValueOrDefault<PersonId>(base.Schema.PersonIdProperty);
			}
			set
			{
				base.SetPropertyValue<PersonId>(base.Schema.PersonIdProperty, value);
			}
		}

		StoreId IBirthdayContactInternal.StoreId
		{
			get
			{
				return base.StoreId;
			}
			set
			{
				base.StoreId = value;
			}
		}

		public bool ShouldHideBirthday
		{
			get
			{
				return base.GetPropertyValueOrDefault<bool>(base.Schema.ShouldHideBirthdayProperty);
			}
			set
			{
				base.SetPropertyValue<bool>(base.Schema.ShouldHideBirthdayProperty, value);
			}
		}

		public bool IsWritable
		{
			get
			{
				return base.GetPropertyValueOrDefault<bool>(base.Schema.IsWritableProperty);
			}
			set
			{
				base.SetPropertyValue<bool>(base.Schema.IsWritableProperty, value);
			}
		}

		public override string ToString()
		{
			return this.DisplayName;
		}

		public new static class Accessors
		{
			public static readonly EntityPropertyAccessor<IBirthdayContact, ExDateTime?> Birthday = new EntityPropertyAccessor<IBirthdayContact, ExDateTime?>(SchematizedObject<BirthdayContactSchema>.SchemaInstance.BirthdayProperty, (IBirthdayContact theBirthdayContact) => theBirthdayContact.Birthday, delegate(IBirthdayContact theBirthdayContact, ExDateTime? birthday)
			{
				theBirthdayContact.Birthday = birthday;
			});

			public static readonly EntityPropertyAccessor<IBirthdayContact, string> DisplayName = new EntityPropertyAccessor<IBirthdayContact, string>(SchematizedObject<BirthdayContactSchema>.SchemaInstance.DisplayNameProperty, (IBirthdayContact theBirthdayContact) => theBirthdayContact.DisplayName, delegate(IBirthdayContact theBirthdayContact, string displayName)
			{
				theBirthdayContact.DisplayName = displayName;
			});

			public static readonly EntityPropertyAccessor<IBirthdayContact, bool> ShouldHideBirthday = new EntityPropertyAccessor<IBirthdayContact, bool>(SchematizedObject<BirthdayContactSchema>.SchemaInstance.ShouldHideBirthdayProperty, (IBirthdayContact theBirthdayContact) => theBirthdayContact.ShouldHideBirthday, delegate(IBirthdayContact theBirthdayContact, bool shouldHideBirthday)
			{
				theBirthdayContact.ShouldHideBirthday = shouldHideBirthday;
			});

			public static readonly EntityPropertyAccessor<IBirthdayContact, bool> IsWritable = new EntityPropertyAccessor<IBirthdayContact, bool>(SchematizedObject<BirthdayContactSchema>.SchemaInstance.IsWritableProperty, (IBirthdayContact theBirthdayContact) => theBirthdayContact.IsWritable, delegate(IBirthdayContact theBirthdayContact, bool isWritable)
			{
				theBirthdayContact.IsWritable = isWritable;
			});

			public static readonly EntityPropertyAccessor<IBirthdayContact, string> Attribution = new EntityPropertyAccessor<IBirthdayContact, string>(SchematizedObject<BirthdayContactSchema>.SchemaInstance.AttributionProperty, (IBirthdayContact theBirthdayContact) => theBirthdayContact.Attribution, delegate(IBirthdayContact theBirthdayContact, string attribution)
			{
				theBirthdayContact.Attribution = attribution;
			});

			internal static readonly EntityPropertyAccessor<IBirthdayContactInternal, PersonId> PersonId = new EntityPropertyAccessor<IBirthdayContactInternal, PersonId>(SchematizedObject<BirthdayContactSchema>.SchemaInstance.PersonIdProperty, (IBirthdayContactInternal theBirthdayContact) => theBirthdayContact.PersonId, delegate(IBirthdayContactInternal theBirthdayContact, PersonId personId)
			{
				theBirthdayContact.PersonId = personId;
			});
		}
	}
}
