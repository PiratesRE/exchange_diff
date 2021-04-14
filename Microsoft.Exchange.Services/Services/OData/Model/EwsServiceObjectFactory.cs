using System;
using System.Linq;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal static class EwsServiceObjectFactory
	{
		public static TEntity CreateEntity<TEntity>(object serviceObject) where TEntity : Entity
		{
			ArgumentValidator.ThrowIfNull("serviceObject", serviceObject);
			Type type = serviceObject.GetType();
			EwsServiceObjectFactory.ServiceObjectTypeMapEntry serviceObjectTypeMapEntry = null;
			foreach (EwsServiceObjectFactory.ServiceObjectTypeMapEntry serviceObjectTypeMapEntry2 in EwsServiceObjectFactory.map)
			{
				if (type.Equals(serviceObjectTypeMapEntry2.ServiceObjectType) || type.IsSubclassOf(serviceObjectTypeMapEntry2.ServiceObjectType))
				{
					serviceObjectTypeMapEntry = serviceObjectTypeMapEntry2;
					break;
				}
			}
			if (serviceObjectTypeMapEntry == null)
			{
				throw new NotSupportedException(string.Format("Service object type {0} not suppported", serviceObject.GetType()));
			}
			return (TEntity)((object)serviceObjectTypeMapEntry.EntityCreator());
		}

		public static TServiceObject CreateServiceObject<TServiceObject>(Entity entityObject) where TServiceObject : class
		{
			ArgumentValidator.ThrowIfNull("entityObject", entityObject);
			EwsServiceObjectFactory.ServiceObjectTypeMapEntry serviceObjectTypeMapEntry = EwsServiceObjectFactory.map.FirstOrDefault((EwsServiceObjectFactory.ServiceObjectTypeMapEntry x) => x.EntityType.Equals(entityObject.GetType()));
			if (serviceObjectTypeMapEntry == null)
			{
				throw new NotSupportedException(string.Format("Entity type {0} not suppported", entityObject.GetType()));
			}
			return (TServiceObject)((object)serviceObjectTypeMapEntry.ServiceObjectCreator());
		}

		// Note: this type is marked as 'beforefieldinit'.
		static EwsServiceObjectFactory()
		{
			EwsServiceObjectFactory.ServiceObjectTypeMapEntry[] array = new EwsServiceObjectFactory.ServiceObjectTypeMapEntry[7];
			array[0] = new EwsServiceObjectFactory.ServiceObjectTypeMapEntry(typeof(MessageType), typeof(Message), () => new MessageType(), () => new Message());
			array[1] = new EwsServiceObjectFactory.ServiceObjectTypeMapEntry(typeof(ContactItemType), typeof(Contact), () => new ContactItemType(), () => new Contact());
			array[2] = new EwsServiceObjectFactory.ServiceObjectTypeMapEntry(typeof(ItemType), typeof(Message), () => new ItemType(), () => new Message());
			array[3] = new EwsServiceObjectFactory.ServiceObjectTypeMapEntry(typeof(ContactsFolderType), typeof(ContactFolder), () => new ContactsFolderType(), () => new ContactFolder());
			array[4] = new EwsServiceObjectFactory.ServiceObjectTypeMapEntry(typeof(FolderType), typeof(Folder), () => new FolderType(), () => new Folder());
			array[5] = new EwsServiceObjectFactory.ServiceObjectTypeMapEntry(typeof(FileAttachmentType), typeof(FileAttachment), () => new FileAttachmentType(), () => new FileAttachment());
			array[6] = new EwsServiceObjectFactory.ServiceObjectTypeMapEntry(typeof(ItemAttachmentType), typeof(ItemAttachment), () => new ItemAttachmentType(), () => new ItemAttachment());
			EwsServiceObjectFactory.map = array;
		}

		private static readonly EwsServiceObjectFactory.ServiceObjectTypeMapEntry[] map;

		private class ServiceObjectTypeMapEntry
		{
			public ServiceObjectTypeMapEntry(Type serviceObjectType, Type entityType, Func<object> serviceObjectCreator, Func<Entity> entityCreator)
			{
				ArgumentValidator.ThrowIfNull("serviceObjectType", serviceObjectType);
				ArgumentValidator.ThrowIfNull("entityType", serviceObjectType);
				ArgumentValidator.ThrowIfNull("serviceObjectCreator", serviceObjectType);
				ArgumentValidator.ThrowIfNull("serviceObjectType", serviceObjectType);
				this.ServiceObjectType = serviceObjectType;
				this.EntityType = entityType;
				this.ServiceObjectCreator = serviceObjectCreator;
				this.EntityCreator = entityCreator;
			}

			public Type ServiceObjectType { get; private set; }

			public Type EntityType { get; private set; }

			public Func<object> ServiceObjectCreator { get; private set; }

			public Func<Entity> EntityCreator { get; private set; }
		}
	}
}
