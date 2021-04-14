using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Entities.DataModel.PropertyBags;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Entities.DataModel.BirthdayCalendar
{
	public class BirthdayEvent : StorageEntity<BirthdayEventSchema>, IBirthdayEventInternal, IBirthdayEvent, IStorageEntity, IEntity, IPropertyChangeTracker<PropertyDefinition>, IVersioned
	{
		public ExDateTime Birthday
		{
			get
			{
				return base.GetPropertyValueOrDefault<ExDateTime>(base.Schema.BirthdayProperty);
			}
			set
			{
				base.SetPropertyValue<ExDateTime>(base.Schema.BirthdayProperty, value);
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

		public bool IsBirthYearKnown
		{
			get
			{
				return this.Birthday.Year != 1604;
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

		public string Subject
		{
			get
			{
				return base.GetPropertyValueOrDefault<string>(base.Schema.SubjectProperty);
			}
			set
			{
				base.SetPropertyValue<string>(base.Schema.SubjectProperty, value);
			}
		}

		PersonId IBirthdayEventInternal.PersonId
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

		StoreObjectId IBirthdayEventInternal.ContactId
		{
			get
			{
				return base.GetPropertyValueOrDefault<StoreObjectId>(base.Schema.ContactIdProperty);
			}
			set
			{
				base.SetPropertyValue<StoreObjectId>(base.Schema.ContactIdProperty, value);
			}
		}

		StoreId IBirthdayEventInternal.StoreId
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

		public new static class Accessors
		{
			public static readonly EntityPropertyAccessor<IBirthdayEvent, string> Subject = new EntityPropertyAccessor<IBirthdayEvent, string>(SchematizedObject<BirthdayEventSchema>.SchemaInstance.SubjectProperty, (IBirthdayEvent birthdayEvent) => birthdayEvent.Subject, delegate(IBirthdayEvent birthdayEvent, string s)
			{
				birthdayEvent.Subject = s;
			});

			public static readonly EntityPropertyAccessor<IBirthdayEvent, string> Attribution = new EntityPropertyAccessor<IBirthdayEvent, string>(SchematizedObject<BirthdayEventSchema>.SchemaInstance.AttributionProperty, (IBirthdayEvent birthdayEvent) => birthdayEvent.Attribution, delegate(IBirthdayEvent birthdayEvent, string attribution)
			{
				birthdayEvent.Attribution = attribution;
			});

			public static readonly EntityPropertyAccessor<IBirthdayEvent, ExDateTime> Birthday = new EntityPropertyAccessor<IBirthdayEvent, ExDateTime>(SchematizedObject<BirthdayEventSchema>.SchemaInstance.BirthdayProperty, (IBirthdayEvent birthdayEvent) => birthdayEvent.Birthday, delegate(IBirthdayEvent birthdayEvent, ExDateTime birthday)
			{
				birthdayEvent.Birthday = birthday;
			});

			public static readonly EntityPropertyAccessor<IBirthdayEvent, bool> IsWritable = new EntityPropertyAccessor<IBirthdayEvent, bool>(SchematizedObject<BirthdayEventSchema>.SchemaInstance.IsWritableProperty, (IBirthdayEvent birthdayEvent) => birthdayEvent.IsWritable, delegate(IBirthdayEvent birthdayEvent, bool isWritable)
			{
				birthdayEvent.IsWritable = isWritable;
			});

			internal static readonly EntityPropertyAccessor<IBirthdayEventInternal, PersonId> PersonId = new EntityPropertyAccessor<IBirthdayEventInternal, PersonId>(SchematizedObject<BirthdayEventSchema>.SchemaInstance.PersonIdProperty, (IBirthdayEventInternal birthdayEvent) => birthdayEvent.PersonId, delegate(IBirthdayEventInternal birthdayEvent, PersonId personId)
			{
				birthdayEvent.PersonId = personId;
			});

			internal static readonly EntityPropertyAccessor<IBirthdayEventInternal, StoreObjectId> ContactId = new EntityPropertyAccessor<IBirthdayEventInternal, StoreObjectId>(SchematizedObject<BirthdayEventSchema>.SchemaInstance.ContactIdProperty, (IBirthdayEventInternal birthdayEvent) => birthdayEvent.ContactId, delegate(IBirthdayEventInternal birthdayEvent, StoreObjectId contactId)
			{
				birthdayEvent.ContactId = contactId;
			});
		}
	}
}
