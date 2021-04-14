using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class InMemoryAttachmentProvider : DisposableObject, IAttachmentProvider, IDisposable
	{
		internal InMemoryAttachmentProvider()
		{
			this.attachmentCounter = 0;
			this.savedAttachmentList = new Dictionary<int, PersistablePropertyBag>();
			this.newAttachmentList = new Dictionary<int, PersistablePropertyBag>();
			this.attachedItems = new Dictionary<int, CoreItem>();
		}

		public void SetCollection(CoreAttachmentCollection attachmentCollection)
		{
			this.attachmentCollection = attachmentCollection;
		}

		public NativeStorePropertyDefinition[] AttachmentTablePropertyList
		{
			get
			{
				this.CheckDisposed(null);
				return this.AttachmentCollection.AttachmentTablePropertyList;
			}
		}

		public bool SupportsCreateClone(AttachmentPropertyBag propertyBag)
		{
			return false;
		}

		public void OnAttachmentLoad(AttachmentPropertyBag attachmentBag)
		{
			this.CheckDisposed(null);
		}

		public void OnBeforeAttachmentSave(AttachmentPropertyBag attachmentBag)
		{
			this.CheckDisposed(null);
			this.AttachmentCollection.OnBeforeAttachmentSave(attachmentBag);
		}

		public bool ExistsInCollection(AttachmentPropertyBag attachmentBag)
		{
			this.CheckDisposed(null);
			return this.AttachmentCollection.Exists(attachmentBag.AttachmentNumber);
		}

		public void OnAfterAttachmentSave(AttachmentPropertyBag attachmentBag)
		{
			this.CheckDisposed(null);
			int attachmentNumber = attachmentBag.AttachmentNumber;
			this.AttachmentCollection.OnAfterAttachmentSave(attachmentNumber);
			PersistablePropertyBag persistablePropertyBag = null;
			if (this.newAttachmentList.TryGetValue(attachmentNumber, out persistablePropertyBag))
			{
				this.newAttachmentList.Remove(attachmentNumber);
				this.savedAttachmentList.Add(attachmentNumber, attachmentBag.PersistablePropertyBag);
				byte[] bytes = BitConverter.GetBytes(attachmentNumber);
				persistablePropertyBag[InternalSchema.RecordKey] = bytes;
				this.AttachmentCollection.UpdateAttachmentId(attachmentBag.AttachmentId, attachmentNumber);
			}
		}

		public void OnAttachmentDisconnected(AttachmentPropertyBag attachmentBag, PersistablePropertyBag dataPropertyBag)
		{
			this.CheckDisposed(null);
			PersistablePropertyBag persistablePropertyBag = null;
			int attachmentNumber = attachmentBag.AttachmentNumber;
			if (this.newAttachmentList.TryGetValue(attachmentNumber, out persistablePropertyBag))
			{
				this.newAttachmentList.Remove(attachmentNumber);
				attachmentBag.Dispose();
			}
		}

		public void OnCollectionDisposed(AttachmentPropertyBag attachmentBag, PersistablePropertyBag dataPropertyBag)
		{
			this.CheckDisposed(null);
			int attachmentNumber = attachmentBag.AttachmentNumber;
			this.newAttachmentList.Remove(attachmentNumber);
			if (dataPropertyBag != null)
			{
				dataPropertyBag.Dispose();
			}
		}

		public PersistablePropertyBag OpenAttachment(ICollection<PropertyDefinition> prefetchProperties, AttachmentPropertyBag attachmentBag)
		{
			this.CheckDisposed(null);
			int attachmentNumber = attachmentBag.AttachmentNumber;
			PersistablePropertyBag persistablePropertyBag = null;
			if (!this.savedAttachmentList.TryGetValue(attachmentNumber, out persistablePropertyBag))
			{
				throw new InvalidOperationException("InMemoryAttachmentProvider::OpenAttachment - Invalid attachment number");
			}
			persistablePropertyBag.Load(prefetchProperties);
			return persistablePropertyBag;
		}

		public PersistablePropertyBag CreateAttachment(ICollection<PropertyDefinition> propertiesToLoad, CoreAttachment attachmentToClone, IItem itemToAttach, out int attachmentNumber)
		{
			this.CheckDisposed(null);
			InMemoryPersistablePropertyBag inMemoryPersistablePropertyBag = new InMemoryPersistablePropertyBag(propertiesToLoad);
			inMemoryPersistablePropertyBag.ExTimeZone = this.ExTimeZone;
			if (attachmentToClone != null)
			{
				throw new NotSupportedException("CreateAttachment for copied attachments is not supported");
			}
			attachmentNumber = this.attachmentCounter++;
			inMemoryPersistablePropertyBag[InternalSchema.AttachNum] = attachmentNumber;
			this.newAttachmentList.Add(attachmentNumber, inMemoryPersistablePropertyBag);
			if (itemToAttach != null)
			{
				string text = itemToAttach.TryGetProperty(InternalSchema.ItemClass) as string;
				Schema schema = (text != null) ? ObjectClass.GetSchema(text) : ItemSchema.Instance;
				propertiesToLoad = InternalSchema.Combine<PropertyDefinition>(schema.AutoloadProperties, propertiesToLoad);
				using (DisposeGuard disposeGuard = default(DisposeGuard))
				{
					PersistablePropertyBag persistablePropertyBag = new InMemoryPersistablePropertyBag(propertiesToLoad);
					disposeGuard.Add<PersistablePropertyBag>(persistablePropertyBag);
					persistablePropertyBag.ExTimeZone = this.ExTimeZone;
					CoreItem coreItem = new CoreItem(null, persistablePropertyBag, null, null, Origin.New, ItemLevel.Attached, propertiesToLoad, ItemBindOption.LoadRequiredPropertiesOnly);
					disposeGuard.Add<CoreItem>(coreItem);
					CoreItem.CopyItemContent(itemToAttach.CoreItem, coreItem);
					this.attachedItems.Add(attachmentNumber, coreItem);
					disposeGuard.Success();
				}
			}
			return inMemoryPersistablePropertyBag;
		}

		public ICoreItem OpenAttachedItem(ICollection<PropertyDefinition> propertiesToLoad, AttachmentPropertyBag attachmentBag, bool isNew)
		{
			this.CheckDisposed(null);
			CoreItem coreItem = null;
			int attachmentNumber = attachmentBag.AttachmentNumber;
			if (this.attachedItems.TryGetValue(attachmentNumber, out coreItem))
			{
				string text = coreItem.PropertyBag.TryGetProperty(InternalSchema.ItemClass) as string;
				Schema schema = (text != null) ? ObjectClass.GetSchema(text) : MessageItemSchema.Instance;
				propertiesToLoad = InternalSchema.Combine<PropertyDefinition>(schema.AutoloadProperties, propertiesToLoad);
				coreItem.PropertyBag.Load(propertiesToLoad);
			}
			else
			{
				if (!isNew)
				{
					throw new ObjectNotFoundException(ServerStrings.MapiCannotOpenEmbeddedMessage);
				}
				string text2 = attachmentBag.TryGetProperty(InternalSchema.ItemClass) as string;
				Schema schema2 = (text2 != null) ? ObjectClass.GetSchema(text2) : MessageItemSchema.Instance;
				propertiesToLoad = InternalSchema.Combine<PropertyDefinition>(schema2.AutoloadProperties, propertiesToLoad);
				coreItem = new CoreItem(null, new InMemoryPersistablePropertyBag(propertiesToLoad)
				{
					ExTimeZone = this.ExTimeZone
				}, StoreObjectId.DummyId, null, Origin.New, ItemLevel.Attached, propertiesToLoad, ItemBindOption.LoadRequiredPropertiesOnly);
				if (text2 != null)
				{
					coreItem.PropertyBag[InternalSchema.ItemClass] = text2;
				}
				this.attachedItems.Add(attachmentNumber, coreItem);
			}
			return new CoreItemWrapper(coreItem);
		}

		public void DeleteAttachment(int attachmentNumber)
		{
			this.CheckDisposed(null);
			this.savedAttachmentList.Remove(attachmentNumber);
		}

		public PropertyBag[] QueryAttachmentTable(NativeStorePropertyDefinition[] properties)
		{
			this.CheckDisposed(null);
			if (this.savedAttachmentList.Count == 0)
			{
				return null;
			}
			Dictionary<StorePropertyDefinition, int> propertyPositionsDictionary = QueryResultPropertyBag.CreatePropertyPositionsDictionary(properties);
			PropertyBag[] array = new PropertyBag[this.savedAttachmentList.Count];
			int num = 0;
			foreach (KeyValuePair<int, PersistablePropertyBag> keyValuePair in this.savedAttachmentList)
			{
				keyValuePair.Value[AttachmentSchema.AttachNum] = keyValuePair.Key;
				object[] array2 = new object[properties.Length];
				for (int num2 = 0; num2 != properties.Length; num2++)
				{
					NativeStorePropertyDefinition propertyDefinition = properties[num2];
					array2[num2] = ((IDirectPropertyBag)keyValuePair.Value).GetValue(propertyDefinition);
				}
				QueryResultPropertyBag queryResultPropertyBag = new QueryResultPropertyBag(null, propertyPositionsDictionary);
				queryResultPropertyBag.ExTimeZone = this.ExTimeZone;
				queryResultPropertyBag.ReturnErrorsOnTruncatedProperties = true;
				queryResultPropertyBag.SetQueryResultRow(array2);
				array[num++] = queryResultPropertyBag;
			}
			return array;
		}

		private CoreAttachmentCollection AttachmentCollection
		{
			get
			{
				return this.attachmentCollection;
			}
		}

		private ExTimeZone ExTimeZone
		{
			get
			{
				return this.AttachmentCollection.ExTimeZone;
			}
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				foreach (KeyValuePair<int, PersistablePropertyBag> keyValuePair in this.newAttachmentList)
				{
					PersistablePropertyBag value = keyValuePair.Value;
					value.Dispose();
				}
				foreach (KeyValuePair<int, PersistablePropertyBag> keyValuePair2 in this.savedAttachmentList)
				{
					PersistablePropertyBag value2 = keyValuePair2.Value;
					value2.Dispose();
				}
				foreach (KeyValuePair<int, CoreItem> keyValuePair3 in this.attachedItems)
				{
					CoreItem value3 = keyValuePair3.Value;
					value3.Dispose();
				}
				this.newAttachmentList.Clear();
				this.savedAttachmentList.Clear();
				this.attachedItems.Clear();
			}
			base.InternalDispose(disposing);
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<InMemoryAttachmentProvider>(this);
		}

		private CoreAttachmentCollection attachmentCollection;

		private int attachmentCounter;

		private Dictionary<int, PersistablePropertyBag> savedAttachmentList;

		private Dictionary<int, PersistablePropertyBag> newAttachmentList;

		private Dictionary<int, CoreItem> attachedItems;
	}
}
