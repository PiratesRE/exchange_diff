using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.OData.Edm;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal static class EntityTypeManager
	{
		public static Entity CreateEntityByType(string typeName)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("typeName", typeName);
			Func<Entity> func;
			if (EntityTypeManager.EntitiesCreatorMap.TryGetValue(typeName, out func))
			{
				return func();
			}
			throw new NotSupportedException(string.Format("Entity type {0} not supported.", typeName));
		}

		public static EntitySchema GetSchema(this IEdmEntityType edmType)
		{
			ArgumentValidator.ThrowIfNull("edmType", edmType);
			EntitySchema result;
			if (EntityTypeManager.EdmTypeSchemaMap.TryGetValue(edmType, out result))
			{
				return result;
			}
			throw new NotSupportedException(string.Format("Missing map entry for {0}.", edmType));
		}

		// Note: this type is marked as 'beforefieldinit'.
		static EntityTypeManager()
		{
			Dictionary<string, Func<Entity>> dictionary = new Dictionary<string, Func<Entity>>();
			dictionary.Add(typeof(User).FullName, () => new User());
			dictionary.Add(typeof(Folder).FullName, () => new Folder());
			dictionary.Add(typeof(Message).FullName, () => new Message());
			dictionary.Add(typeof(Attachment).FullName, () => new FileAttachment());
			dictionary.Add(typeof(FileAttachment).FullName, () => new FileAttachment());
			dictionary.Add(typeof(ItemAttachment).FullName, () => new ItemAttachment());
			dictionary.Add(typeof(Calendar).FullName, () => new Calendar());
			dictionary.Add(typeof(CalendarGroup).FullName, () => new CalendarGroup());
			dictionary.Add(typeof(Event).FullName, () => new Event());
			dictionary.Add(typeof(Contact).FullName, () => new Contact());
			dictionary.Add(typeof(ContactFolder).FullName, () => new ContactFolder());
			EntityTypeManager.EntitiesCreatorMap = dictionary;
			EntityTypeManager.EdmTypeSchemaMap = new Dictionary<IEdmEntityType, EntitySchema>
			{
				{
					Entity.EdmEntityType,
					EntitySchema.SchemaInstance
				},
				{
					Folder.EdmEntityType,
					FolderSchema.SchemaInstance
				},
				{
					Item.EdmEntityType,
					ItemSchema.SchemaInstance
				},
				{
					Message.EdmEntityType,
					MessageSchema.SchemaInstance
				},
				{
					Calendar.EdmEntityType,
					CalendarSchema.SchemaInstance
				},
				{
					CalendarGroup.EdmEntityType,
					CalendarGroupSchema.SchemaInstance
				},
				{
					Event.EdmEntityType,
					EventSchema.SchemaInstance
				},
				{
					Attachment.EdmEntityType,
					AttachmentSchema.SchemaInstance
				},
				{
					FileAttachment.EdmEntityType,
					FileAttachmentSchema.SchemaInstance
				},
				{
					ItemAttachment.EdmEntityType,
					ItemAttachmentSchema.SchemaInstance
				},
				{
					Contact.EdmEntityType,
					ContactSchema.SchemaInstance
				},
				{
					ContactFolder.EdmEntityType,
					ContactFolderSchema.SchemaInstance
				}
			};
		}

		private static readonly Dictionary<string, Func<Entity>> EntitiesCreatorMap;

		private static readonly Dictionary<IEdmEntityType, EntitySchema> EdmTypeSchemaMap;
	}
}
