using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ServerManifestEntry : ICustomSerializable, IReadOnlyPropertyBag
	{
		public ServerManifestEntry()
		{
		}

		public ServerManifestEntry(ISyncItemId id)
		{
			this.itemId = id;
		}

		public ServerManifestEntry(ChangeType changeType, ISyncItemId id, ISyncWatermark watermark = null)
		{
			this.ChangeType = changeType;
			this.itemId = id;
			this.Watermark = watermark;
		}

		public ISyncItemId Id
		{
			get
			{
				return this.itemId;
			}
		}

		public int?[] ChangeTrackingInformation
		{
			get
			{
				return this.changeTrackingInformation;
			}
			set
			{
				this.changeTrackingInformation = value;
			}
		}

		public ChangeType ChangeType
		{
			get
			{
				return this.changeType;
			}
			set
			{
				EnumValidator.ThrowIfInvalid<ChangeType>(value);
				this.changeType = value;
			}
		}

		public bool IsAcknowledgedByClient
		{
			get
			{
				return this.acknowledgedByClient;
			}
			set
			{
				this.acknowledgedByClient = value;
			}
		}

		public bool IsDelayedServerOperation
		{
			get
			{
				return this.delayedServerOperation;
			}
			set
			{
				this.delayedServerOperation = value;
			}
		}

		public bool IsRejected
		{
			get
			{
				return this.rejected;
			}
			set
			{
				this.rejected = value;
			}
		}

		public ISyncWatermark Watermark
		{
			get
			{
				return this.watermark;
			}
			set
			{
				this.watermark = value;
			}
		}

		public bool IsRead
		{
			get
			{
				return this.itemRead == ServerManifestEntry.ReadFlagState.Read;
			}
			set
			{
				this.itemRead = (value ? ServerManifestEntry.ReadFlagState.Read : ServerManifestEntry.ReadFlagState.UnRead);
			}
		}

		public bool IsReadFlagInitialized
		{
			get
			{
				return this.itemRead != ServerManifestEntry.ReadFlagState.Unknown;
			}
		}

		public bool IsNew
		{
			get
			{
				return this.newItem;
			}
			set
			{
				this.newItem = value;
			}
		}

		public ConversationId ConversationId
		{
			get
			{
				return this.conversationId;
			}
			set
			{
				this.conversationId = value;
			}
		}

		public bool FirstMessageInConversation
		{
			get
			{
				return this.firstMessageInConversation;
			}
			set
			{
				this.firstMessageInConversation = value;
			}
		}

		public ExDateTime? FilterDate
		{
			get
			{
				return this.filterDate;
			}
			set
			{
				this.filterDate = value;
			}
		}

		public string MessageClass
		{
			get
			{
				return this.messageClass;
			}
			set
			{
				this.messageClass = value;
			}
		}

		public StoreId SeriesMasterId { get; set; }

		public CalendarItemType CalendarItemType { get; set; }

		public object this[PropertyDefinition propertyDefinition]
		{
			get
			{
				if (propertyDefinition.Equals(InternalSchema.ItemClass) && !string.IsNullOrEmpty(this.MessageClass))
				{
					return this.MessageClass;
				}
				if (propertyDefinition.Equals(MessageItemSchema.IsRead))
				{
					return this.IsRead;
				}
				if (propertyDefinition.Equals(InternalSchema.ItemId) && this.Id != null && this.Id.NativeId != null)
				{
					return this.Id.NativeId;
				}
				if (propertyDefinition.Equals(ItemSchema.ReceivedTime) && this.FilterDate != null)
				{
					return this.FilterDate.Value;
				}
				if (propertyDefinition.Equals(ItemSchema.ConversationId) && this.ConversationId != null)
				{
					return this.ConversationId;
				}
				return new PropertyError(propertyDefinition, PropertyErrorCode.NotFound);
			}
		}

		public virtual void DeserializeData(BinaryReader reader, ComponentDataPool componentDataPool)
		{
			DerivedData<ISyncItemId> isyncItemIdDataInstance = componentDataPool.GetISyncItemIdDataInstance();
			isyncItemIdDataInstance.DeserializeData(reader, componentDataPool);
			this.itemId = isyncItemIdDataInstance.Data;
			this.changeType = (ChangeType)reader.ReadByte();
			DerivedData<ISyncWatermark> isyncWatermarkDataInstance = componentDataPool.GetISyncWatermarkDataInstance();
			isyncWatermarkDataInstance.DeserializeData(reader, componentDataPool);
			this.watermark = isyncWatermarkDataInstance.Data;
			this.IsAcknowledgedByClient = reader.ReadBoolean();
			ArrayData<NullableData<Int32Data, int>, int?> nullableInt32ArrayInstance = componentDataPool.GetNullableInt32ArrayInstance();
			nullableInt32ArrayInstance.DeserializeData(reader, componentDataPool);
			this.changeTrackingInformation = nullableInt32ArrayInstance.Data;
			this.IsRejected = reader.ReadBoolean();
			this.IsDelayedServerOperation = reader.ReadBoolean();
			if (componentDataPool.InternalVersion > 0)
			{
				NullableDateTimeData nullableDateTimeDataInstance = componentDataPool.GetNullableDateTimeDataInstance();
				nullableDateTimeDataInstance.DeserializeData(reader, componentDataPool);
				this.filterDate = nullableDateTimeDataInstance.Data;
				StringData stringDataInstance = componentDataPool.GetStringDataInstance();
				stringDataInstance.DeserializeData(reader, componentDataPool);
				this.messageClass = stringDataInstance.Data;
				ConversationIdData conversationIdDataInstance = componentDataPool.GetConversationIdDataInstance();
				conversationIdDataInstance.DeserializeData(reader, componentDataPool);
				this.conversationId = conversationIdDataInstance.Data;
				this.FirstMessageInConversation = reader.ReadBoolean();
				if (componentDataPool.InternalVersion > 2)
				{
					this.itemRead = (ServerManifestEntry.ReadFlagState)reader.ReadByte();
				}
			}
		}

		public virtual void SerializeData(BinaryWriter writer, ComponentDataPool componentDataPool)
		{
			componentDataPool.GetISyncItemIdDataInstance().Bind(this.itemId).SerializeData(writer, componentDataPool);
			writer.Write((byte)this.changeType);
			ICustomClonable customClonable = this.watermark as ICustomClonable;
			if (customClonable != null)
			{
				this.watermark = (ISyncWatermark)customClonable.CustomClone();
			}
			componentDataPool.GetISyncWatermarkDataInstance().Bind(this.watermark).SerializeData(writer, componentDataPool);
			writer.Write(this.IsAcknowledgedByClient);
			componentDataPool.GetNullableInt32ArrayInstance().Bind(this.changeTrackingInformation).SerializeData(writer, componentDataPool);
			writer.Write(this.IsRejected);
			writer.Write(this.IsDelayedServerOperation);
			componentDataPool.GetNullableDateTimeDataInstance().Bind(this.filterDate).SerializeData(writer, componentDataPool);
			componentDataPool.GetStringDataInstance().Bind(this.messageClass).SerializeData(writer, componentDataPool);
			componentDataPool.GetConversationIdDataInstance().Bind(this.conversationId).SerializeData(writer, componentDataPool);
			writer.Write(this.FirstMessageInConversation);
			writer.Write((byte)this.itemRead);
		}

		public void UpdateManifestFromItem(ISyncItem item)
		{
			IReadOnlyPropertyBag readOnlyPropertyBag = item.NativeItem as IReadOnlyPropertyBag;
			if (readOnlyPropertyBag == null)
			{
				return;
			}
			this.UpdateManifestFromPropertyBag(readOnlyPropertyBag);
		}

		public void UpdateManifestFromPropertyBag(IReadOnlyPropertyBag propertyBag)
		{
			this.messageClass = (propertyBag[InternalSchema.ItemClass] as string);
			try
			{
				object obj = propertyBag[ItemSchema.ReceivedTime];
				if (obj is ExDateTime)
				{
					this.filterDate = new ExDateTime?((ExDateTime)obj);
				}
			}
			catch (PropertyErrorException)
			{
				this.filterDate = null;
			}
			try
			{
				object obj = propertyBag[ItemSchema.ConversationId];
				if (obj is ConversationId)
				{
					this.conversationId = (ConversationId)obj;
				}
				obj = propertyBag[ItemSchema.ConversationIndex];
				ConversationIndex index;
				if (obj is byte[] && ConversationIndex.TryCreate((byte[])obj, out index) && index != ConversationIndex.Empty && index.Components != null && index.Components.Count == 1)
				{
					this.firstMessageInConversation = true;
				}
			}
			catch (PropertyErrorException)
			{
				this.conversationId = null;
				this.firstMessageInConversation = false;
			}
			try
			{
				object obj = propertyBag[MessageItemSchema.IsRead];
				if (obj is bool)
				{
					this.itemRead = (((bool)obj) ? ServerManifestEntry.ReadFlagState.Read : ServerManifestEntry.ReadFlagState.UnRead);
				}
			}
			catch (PropertyErrorException)
			{
				this.itemRead = ServerManifestEntry.ReadFlagState.Unknown;
			}
		}

		public object[] GetProperties(ICollection<PropertyDefinition> propertyDefinitions)
		{
			throw new NotImplementedException();
		}

		public override string ToString()
		{
			return string.Format("SME Id:{0}, ChangeType:{1}, IsRead:{2}", this.Id, this.ChangeType, this.IsRead);
		}

		private int?[] changeTrackingInformation;

		private ChangeType changeType;

		private bool acknowledgedByClient;

		private bool delayedServerOperation;

		private bool rejected;

		private ISyncWatermark watermark;

		private ISyncItemId itemId;

		private ServerManifestEntry.ReadFlagState itemRead = ServerManifestEntry.ReadFlagState.Unknown;

		private bool newItem;

		private ConversationId conversationId;

		private bool firstMessageInConversation;

		private ExDateTime? filterDate;

		private string messageClass;

		private enum ReadFlagState : byte
		{
			Read,
			UnRead,
			Unknown
		}
	}
}
