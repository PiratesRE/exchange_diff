using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.OData.Edm.Library;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class User : Entity
	{
		public string DisplayName
		{
			get
			{
				return (string)base[UserSchema.DisplayName];
			}
			set
			{
				base[UserSchema.DisplayName] = value;
			}
		}

		public string Alias
		{
			get
			{
				return (string)base[UserSchema.Alias];
			}
			set
			{
				base[UserSchema.Alias] = value;
			}
		}

		public Guid MailboxGuid
		{
			get
			{
				return (Guid)base[UserSchema.MailboxGuid];
			}
			set
			{
				base[UserSchema.MailboxGuid] = value;
			}
		}

		public IEnumerable<Folder> Folders
		{
			get
			{
				return (IEnumerable<Folder>)base[UserSchema.Folders];
			}
			set
			{
				base[UserSchema.Folders] = value;
			}
		}

		public IEnumerable<Message> Messages
		{
			get
			{
				return (IEnumerable<Message>)base[UserSchema.Messages];
			}
			set
			{
				base[UserSchema.Messages] = value;
			}
		}

		public IEnumerable<Event> Events
		{
			get
			{
				return (IEnumerable<Event>)base[UserSchema.Events];
			}
			set
			{
				base[UserSchema.Events] = value;
			}
		}

		public IEnumerable<Calendar> Calendars
		{
			get
			{
				return (IEnumerable<Calendar>)base[UserSchema.Calendars];
			}
			set
			{
				base[UserSchema.Calendars] = value;
			}
		}

		public Calendar Calendar
		{
			get
			{
				return (Calendar)base[UserSchema.Calendar];
			}
			set
			{
				base[UserSchema.Calendar] = value;
			}
		}

		public IEnumerable<CalendarGroup> CalendarGroups
		{
			get
			{
				return (IEnumerable<CalendarGroup>)base[UserSchema.CalendarGroups];
			}
			set
			{
				base[UserSchema.CalendarGroups] = value;
			}
		}

		public Folder RootFolder
		{
			get
			{
				return (Folder)base[UserSchema.RootFolder];
			}
			set
			{
				base[UserSchema.RootFolder] = value;
			}
		}

		public Folder Inbox
		{
			get
			{
				return (Folder)base[UserSchema.Inbox];
			}
			set
			{
				base[UserSchema.Inbox] = value;
			}
		}

		public Folder Drafts
		{
			get
			{
				return (Folder)base[UserSchema.Drafts];
			}
			set
			{
				base[UserSchema.Drafts] = value;
			}
		}

		public Folder SentItems
		{
			get
			{
				return (Folder)base[UserSchema.SentItems];
			}
			set
			{
				base[UserSchema.SentItems] = value;
			}
		}

		public Folder DeletedItems
		{
			get
			{
				return (Folder)base[UserSchema.DeletedItems];
			}
			set
			{
				base[UserSchema.DeletedItems] = value;
			}
		}

		public IEnumerable<Contact> Contacts
		{
			get
			{
				return (IEnumerable<Contact>)base[UserSchema.Contacts];
			}
			set
			{
				base[UserSchema.Contacts] = value;
			}
		}

		public IEnumerable<ContactFolder> ContactFolders
		{
			get
			{
				return (IEnumerable<ContactFolder>)base[UserSchema.ContactFolders];
			}
			set
			{
				base[UserSchema.ContactFolders] = value;
			}
		}

		internal override EntitySchema Schema
		{
			get
			{
				return UserSchema.SchemaInstance;
			}
		}

		internal override Uri GetWebUri(ODataContext odataContext)
		{
			ArgumentValidator.ThrowIfNull("odataContext", odataContext);
			string uriString = string.Format("{0}Users('{1}')", odataContext.HttpContext.GetServiceRootUri(), base.Id);
			return new Uri(uriString);
		}

		internal new static readonly EdmEntityType EdmEntityType = new EdmEntityType(typeof(User).Namespace, typeof(User).Name, Entity.EdmEntityType);
	}
}
