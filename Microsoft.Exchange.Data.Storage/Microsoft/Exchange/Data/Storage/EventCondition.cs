using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class EventCondition
	{
		public EventCondition()
		{
			this.containerFolderIds = new List<StoreObjectId>();
			this.objectIds = new List<StoreObjectId>();
			this.classNames = new List<string>();
		}

		public EventCondition(EventCondition condition)
		{
			this.classNames = new List<string>(condition.classNames);
			this.containerFolderIds = new List<StoreObjectId>(condition.containerFolderIds);
			this.objectIds = new List<StoreObjectId>(condition.objectIds);
			this.eventType = condition.eventType;
			this.objectType = condition.objectType;
			this.eventFlags = condition.eventFlags;
			this.eventSubtreeFlags = condition.eventSubtreeFlags;
		}

		public EventSubtreeFlag EventSubtree
		{
			get
			{
				return this.eventSubtreeFlags;
			}
			set
			{
				EnumValidator.ThrowIfInvalid<EventSubtreeFlag>(value);
				if ((value & EventSubtreeFlag.All) == (EventSubtreeFlag)0)
				{
					throw new ArgumentOutOfRangeException("value", ServerStrings.BadEnumValue(typeof(EventSubtreeFlag)));
				}
				this.eventSubtreeFlags = value;
			}
		}

		public EventType EventType
		{
			get
			{
				return this.eventType;
			}
			set
			{
				if ((value & (EventType.NewMail | EventType.ObjectCreated | EventType.ObjectDeleted | EventType.ObjectModified | EventType.ObjectMoved | EventType.ObjectCopied | EventType.FreeBusyChanged)) != value)
				{
					throw new EnumOutOfRangeException("value", ServerStrings.BadEnumValue(typeof(EventType)));
				}
				this.eventType = value;
			}
		}

		public ICollection<StoreObjectId> ContainerFolderIds
		{
			get
			{
				return this.containerFolderIds;
			}
		}

		public ICollection<StoreObjectId> ObjectIds
		{
			get
			{
				return this.objectIds;
			}
		}

		public ICollection<string> ClassNames
		{
			get
			{
				return this.classNames;
			}
		}

		public EventObjectType ObjectType
		{
			get
			{
				return this.objectType;
			}
			set
			{
				EnumValidator.ThrowIfInvalid<EventObjectType>(value);
				this.objectType = value;
			}
		}

		public EventFlags EventFlags
		{
			get
			{
				return this.eventFlags;
			}
			set
			{
				EnumValidator.ThrowIfInvalid<EventFlags>(value);
				this.eventFlags = value;
			}
		}

		public override string ToString()
		{
			return string.Format("EventType = {0}. ObjectType = {1}. ContainerFolderIdCount = {2}. ObjectIdCount= {3}. ClassNameCount ={4}.", new object[]
			{
				this.eventType,
				this.objectType,
				(this.containerFolderIds != null) ? this.containerFolderIds.Count : 0,
				(this.objectIds != null) ? this.objectIds.Count : 0,
				(this.classNames != null) ? this.classNames.Count : 0
			});
		}

		private const EventType ValidEventTypesForCondition = EventType.NewMail | EventType.ObjectCreated | EventType.ObjectDeleted | EventType.ObjectModified | EventType.ObjectMoved | EventType.ObjectCopied | EventType.FreeBusyChanged;

		private readonly List<StoreObjectId> containerFolderIds;

		private readonly List<StoreObjectId> objectIds;

		private readonly List<string> classNames;

		private EventType eventType;

		private EventObjectType objectType;

		private EventFlags eventFlags;

		private EventSubtreeFlag eventSubtreeFlags = EventSubtreeFlag.All;
	}
}
