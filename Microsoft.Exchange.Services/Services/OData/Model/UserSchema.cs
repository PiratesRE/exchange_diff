using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.OData.Edm.Library;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class UserSchema : EntitySchema
	{
		public new static UserSchema SchemaInstance
		{
			get
			{
				return UserSchema.UserSchemaInstance.Member;
			}
		}

		public override EdmEntityType EdmEntityType
		{
			get
			{
				return User.EdmEntityType;
			}
		}

		public override ReadOnlyCollection<PropertyDefinition> DeclaredProperties
		{
			get
			{
				return UserSchema.DeclaredUserProperties;
			}
		}

		public override ReadOnlyCollection<PropertyDefinition> AllProperties
		{
			get
			{
				return UserSchema.AllUserProperties;
			}
		}

		public override ReadOnlyCollection<PropertyDefinition> DefaultProperties
		{
			get
			{
				return UserSchema.DefaultUserProperties;
			}
		}

		public static readonly PropertyDefinition DisplayName = new PropertyDefinition("DisplayName", typeof(string))
		{
			EdmType = EdmCoreModel.Instance.GetString(true),
			Flags = PropertyDefinitionFlags.CanFilter,
			ADDriverPropertyProvider = new SimpleDirectoryPropertyProvider(ADRecipientSchema.DisplayName)
		};

		public static readonly PropertyDefinition Alias = new PropertyDefinition("Alias", typeof(string))
		{
			EdmType = EdmCoreModel.Instance.GetString(true),
			Flags = PropertyDefinitionFlags.CanFilter,
			ADDriverPropertyProvider = new SimpleDirectoryPropertyProvider(ADRecipientSchema.Alias)
		};

		public static readonly PropertyDefinition MailboxGuid = new PropertyDefinition("MailboxGuid", typeof(Guid))
		{
			EdmType = EdmCoreModel.Instance.GetGuid(true),
			Flags = PropertyDefinitionFlags.CanFilter,
			ADDriverPropertyProvider = new SimpleDirectoryPropertyProvider(ADMailboxRecipientSchema.ExchangeGuid)
		};

		public static readonly PropertyDefinition Folders = new PropertyDefinition("Folders", typeof(IEnumerable<Folder>))
		{
			Flags = PropertyDefinitionFlags.Navigation,
			NavigationTargetEntity = Folder.EdmEntityType
		};

		public static readonly PropertyDefinition Messages = new PropertyDefinition("Messages", typeof(IEnumerable<Message>))
		{
			Flags = PropertyDefinitionFlags.Navigation,
			NavigationTargetEntity = Message.EdmEntityType
		};

		public static readonly PropertyDefinition Calendars = new PropertyDefinition("Calendars", typeof(IEnumerable<Calendar>))
		{
			Flags = PropertyDefinitionFlags.Navigation,
			NavigationTargetEntity = Microsoft.Exchange.Services.OData.Model.Calendar.EdmEntityType
		};

		public static readonly PropertyDefinition Calendar = new PropertyDefinition("Calendar", typeof(Calendar))
		{
			Flags = PropertyDefinitionFlags.Navigation,
			NavigationTargetEntity = Microsoft.Exchange.Services.OData.Model.Calendar.EdmEntityType
		};

		public static readonly PropertyDefinition CalendarGroups = new PropertyDefinition("CalendarGroups", typeof(IEnumerable<CalendarGroup>))
		{
			Flags = PropertyDefinitionFlags.Navigation,
			NavigationTargetEntity = CalendarGroup.EdmEntityType
		};

		public static readonly PropertyDefinition Events = new PropertyDefinition("Events", typeof(IEnumerable<Event>))
		{
			Flags = PropertyDefinitionFlags.Navigation,
			NavigationTargetEntity = Event.EdmEntityType
		};

		public static readonly PropertyDefinition RootFolder = new PropertyDefinition("RootFolder", typeof(Folder))
		{
			Flags = PropertyDefinitionFlags.Navigation,
			NavigationTargetEntity = Folder.EdmEntityType
		};

		public static readonly PropertyDefinition Inbox = new PropertyDefinition("Inbox", typeof(Folder))
		{
			Flags = PropertyDefinitionFlags.Navigation,
			NavigationTargetEntity = Folder.EdmEntityType
		};

		public static readonly PropertyDefinition Drafts = new PropertyDefinition("Drafts", typeof(Folder))
		{
			Flags = PropertyDefinitionFlags.Navigation,
			NavigationTargetEntity = Folder.EdmEntityType
		};

		public static readonly PropertyDefinition SentItems = new PropertyDefinition("SentItems", typeof(Folder))
		{
			Flags = PropertyDefinitionFlags.Navigation,
			NavigationTargetEntity = Folder.EdmEntityType
		};

		public static readonly PropertyDefinition DeletedItems = new PropertyDefinition("DeletedItems", typeof(Folder))
		{
			Flags = PropertyDefinitionFlags.Navigation,
			NavigationTargetEntity = Folder.EdmEntityType
		};

		public static readonly PropertyDefinition Contacts = new PropertyDefinition("Contacts", typeof(IEnumerable<Contact>))
		{
			Flags = PropertyDefinitionFlags.Navigation,
			NavigationTargetEntity = Contact.EdmEntityType
		};

		public static readonly PropertyDefinition ContactFolders = new PropertyDefinition("ContactFolders", typeof(IEnumerable<ContactFolder>))
		{
			Flags = PropertyDefinitionFlags.Navigation,
			NavigationTargetEntity = ContactFolder.EdmEntityType
		};

		public static readonly ReadOnlyCollection<PropertyDefinition> DeclaredUserProperties = new ReadOnlyCollection<PropertyDefinition>(new List<PropertyDefinition>
		{
			UserSchema.DisplayName,
			UserSchema.Alias,
			UserSchema.MailboxGuid,
			UserSchema.Folders,
			UserSchema.Messages,
			UserSchema.RootFolder,
			UserSchema.Inbox,
			UserSchema.Drafts,
			UserSchema.SentItems,
			UserSchema.DeletedItems,
			UserSchema.Calendars,
			UserSchema.Calendar,
			UserSchema.CalendarGroups,
			UserSchema.Events,
			UserSchema.Contacts,
			UserSchema.ContactFolders
		});

		public static readonly ReadOnlyCollection<PropertyDefinition> AllUserProperties = new ReadOnlyCollection<PropertyDefinition>(new List<PropertyDefinition>(EntitySchema.AllEntityProperties.Union(UserSchema.DeclaredUserProperties)));

		public static readonly ReadOnlyCollection<PropertyDefinition> DefaultUserProperties = new ReadOnlyCollection<PropertyDefinition>(new List<PropertyDefinition>(EntitySchema.DefaultEntityProperties)
		{
			UserSchema.DisplayName,
			UserSchema.Alias,
			UserSchema.MailboxGuid
		});

		private static readonly LazyMember<UserSchema> UserSchemaInstance = new LazyMember<UserSchema>(() => new UserSchema());
	}
}
