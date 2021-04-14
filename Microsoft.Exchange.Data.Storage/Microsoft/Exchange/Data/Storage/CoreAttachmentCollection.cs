using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class CoreAttachmentCollection : DisposableObject, IEnumerable<AttachmentHandle>, IEnumerable
	{
		internal CoreAttachmentCollection(IAttachmentProvider attachmentProvider, ICoreItem message, bool forceReadOnly, bool hasAttachments)
		{
			Util.ThrowOnNullArgument(message, "message");
			if (!hasAttachments)
			{
				this.isInitialized = true;
			}
			this.coreItem = message;
			this.forceReadOnly = forceReadOnly;
			this.attachmentProvider = attachmentProvider;
			this.fetchProperties = CoreAttachmentCollection.PrefetchPropertySet;
		}

		internal bool IsReadOnly
		{
			get
			{
				this.CheckDisposed(null);
				return this.forceReadOnly || this.ContainerItem.IsReadOnly;
			}
		}

		internal bool IsDirty
		{
			get
			{
				this.CheckDisposed(null);
				return this.isDirty;
			}
		}

		internal bool IsClonedFromAnExistingAttachmentCollection { get; private set; }

		internal bool IsInitialized
		{
			get
			{
				this.CheckDisposed(null);
				return this.isInitialized;
			}
		}

		internal int Count
		{
			get
			{
				this.CheckDisposed(null);
				this.InitCollection("GetEnumerator", false);
				return this.savedAttachmentNumberMap.Count;
			}
		}

		internal NativeStorePropertyDefinition[] AttachmentTablePropertyList
		{
			get
			{
				this.CheckDisposed(null);
				return this.fetchProperties.ToArray<NativeStorePropertyDefinition>();
			}
		}

		internal ICoreItem ContainerItem
		{
			get
			{
				this.CheckDisposed(null);
				return this.coreItem;
			}
		}

		internal ExTimeZone ExTimeZone
		{
			get
			{
				this.CheckDisposed(null);
				return CoreObject.GetPersistablePropertyBag(this.ContainerItem).ExTimeZone;
			}
		}

		public CoreAttachment Create(AttachmentType type)
		{
			this.CheckDisposed(null);
			EnumValidator.ThrowIfInvalid<AttachmentType>(type, "type");
			ExTraceGlobals.StorageTracer.Information((long)this.GetHashCode(), "Storage.CoreAttachmentCollection.Create1");
			this.InitCollection("CreateFromExistingItem", true);
			return this.InternalCreate(new AttachmentType?(type), null, null, null);
		}

		public CoreAttachment Open(AttachmentHandle handle)
		{
			return this.Open(handle, null);
		}

		public CoreAttachment Open(AttachmentHandle handle, ICollection<PropertyDefinition> preloadProperties)
		{
			this.CheckDisposed(null);
			AttachmentHandle objB = null;
			if (!this.savedAttachmentNumberMap.TryGetValue(handle.AttachNumber, out objB) || !object.ReferenceEquals(handle, objB))
			{
				throw new ArgumentException("handle");
			}
			return this.InternalOpen(handle, preloadProperties);
		}

		public IEnumerator<AttachmentHandle> GetEnumerator()
		{
			this.CheckDisposed(null);
			this.InitCollection("GetEnumerator", false);
			return this.savedAttachmentNumberMap.Values.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			this.CheckDisposed(null);
			return this.GetEnumerator();
		}

		public bool Remove(AttachmentHandle attachmentHandle)
		{
			this.CheckDisposed(null);
			Util.ThrowOnNullArgument(attachmentHandle, "attachmentHandle");
			ExTraceGlobals.StorageTracer.Information((long)this.GetHashCode(), "Storage.CoreAttachmentCollection.Remove(AttachmentHandle)");
			this.InitCollection("Remove", true);
			int attachNumber = attachmentHandle.AttachNumber;
			this.attachmentProvider.DeleteAttachment(attachNumber);
			if (attachmentHandle.AttachmentId != null)
			{
				this.attachmentIdMap.Remove(attachmentHandle.AttachmentId);
			}
			this.savedAttachmentNumberMap.Remove(attachNumber);
			this.isDirty = true;
			this.IsClonedFromAnExistingAttachmentCollection = false;
			return true;
		}

		internal static void CloneAttachmentCollection(ICoreItem sourceItem, ICoreItem destinationItem)
		{
			foreach (AttachmentHandle handle in sourceItem.AttachmentCollection)
			{
				using (CoreAttachment coreAttachment = sourceItem.AttachmentCollection.Open(handle, CoreObjectSchema.AllPropertiesOnStore))
				{
					using (CoreAttachment coreAttachment2 = destinationItem.AttachmentCollection.CreateCopy(coreAttachment))
					{
						using (Attachment attachment = AttachmentCollection.CreateTypedAttachment(coreAttachment2, new AttachmentType?(coreAttachment2.AttachmentType)))
						{
							attachment.SaveFlags |= (coreAttachment.PropertyBag.SaveFlags | PropertyBagSaveFlags.IgnoreMapiComputedErrors);
							attachment.Save();
						}
					}
				}
			}
			destinationItem.AttachmentCollection.IsClonedFromAnExistingAttachmentCollection = (sourceItem.AttachmentCollection.Count > 0 && !sourceItem.AttachmentCollection.IsDirty && sourceItem.AttachmentCollection.Count == destinationItem.AttachmentCollection.Count);
		}

		internal static bool IsCalendarException(int attachCalendarFlags)
		{
			return (attachCalendarFlags & 6) != 0;
		}

		internal static bool IsCalendarException(AttachmentHandle handle)
		{
			return handle.IsCalendarException;
		}

		internal static bool IsInlineAttachment(AttachmentHandle handle)
		{
			return handle.IsInline;
		}

		internal static Schema GetAttachmentSchema(int attachMethod)
		{
			switch (attachMethod)
			{
			case 1:
			case 6:
				return StreamAttachmentBaseSchema.Instance;
			case 2:
			case 3:
			case 4:
				return ReferenceAttachmentSchema.Instance;
			default:
				return ItemAttachmentSchema.Instance;
			}
		}

		internal static Schema GetAttachmentSchema(AttachmentType? attachmentType)
		{
			if (attachmentType == null || attachmentType.Value == AttachmentType.EmbeddedMessage)
			{
				return ItemAttachmentSchema.Instance;
			}
			if (attachmentType.Value == AttachmentType.Reference)
			{
				return ReferenceAttachmentSchema.Instance;
			}
			return StreamAttachmentBaseSchema.Instance;
		}

		internal static AttachmentType GetAttachmentType(int? attachMethod)
		{
			if (attachMethod != null)
			{
				switch (attachMethod.Value)
				{
				case 2:
				case 3:
				case 4:
				case 7:
					return AttachmentType.Reference;
				case 5:
					return AttachmentType.EmbeddedMessage;
				case 6:
					return AttachmentType.Ole;
				}
				return AttachmentType.Stream;
			}
			return AttachmentType.Stream;
		}

		internal void Load(ICollection<PropertyDefinition> propertyList)
		{
			this.CheckDisposed(null);
			if (propertyList == InternalSchema.ContentConversionProperties)
			{
				throw new InvalidOperationException("Cannot load ContentConversionProperties on the attachment collection. Call Load() on individual attachments instead");
			}
			if (propertyList != null && propertyList.Count != 0)
			{
				this.fetchProperties = CoreAttachmentCollection.CreatePrefetchPropertySet<PropertyDefinition>(this.fetchProperties, propertyList);
			}
		}

		internal CoreAttachment CreateFromExistingItem(IItem item)
		{
			this.CheckDisposed(null);
			Util.ThrowOnNullArgument(item, "item");
			ExTraceGlobals.StorageTracer.Information((long)this.GetHashCode(), "Storage.CoreAttachmentCollection.AddExistingItem.");
			this.InitCollection("CreateFromExistingItem", true);
			return this.InternalCreate(new AttachmentType?(AttachmentType.EmbeddedMessage), null, null, item);
		}

		internal CoreAttachment CreateCopy(CoreAttachment attachmentToCopy)
		{
			this.CheckDisposed(null);
			Util.ThrowOnNullArgument(attachmentToCopy, "attachmentToCopy");
			ExTraceGlobals.StorageTracer.Information((long)this.GetHashCode(), "Storage.CoreAttachmentCollection.CreateCopy");
			this.InitCollection("CreateCopy", true);
			return this.InternalCreateCopy(new AttachmentType?(attachmentToCopy.AttachmentType), attachmentToCopy);
		}

		internal CoreAttachment CreateItemAttachment(StoreObjectType type)
		{
			this.CheckDisposed(null);
			EnumValidator.ThrowIfInvalid<StoreObjectType>(type, "type");
			ExTraceGlobals.StorageTracer.Information((long)this.GetHashCode(), "Storage.CoreAttachmentCollection.Create2");
			this.InitCollection("CreateItemAttachment", true);
			string containerMessageClass = ObjectClass.GetContainerMessageClass(type);
			return this.InternalCreate(new AttachmentType?(AttachmentType.EmbeddedMessage), containerMessageClass, null, null);
		}

		internal bool Contains(AttachmentId id)
		{
			this.CheckDisposed(null);
			Util.ThrowOnNullArgument(id, "id");
			ExTraceGlobals.StorageTracer.Information((long)this.GetHashCode(), "Storage.CoreAttachmentCollection.Contains");
			this.InitCollection("Contains", false);
			AttachmentHandle attachmentHandle = null;
			return this.attachmentIdMap.TryGetValue(id, out attachmentHandle) && attachmentHandle != null;
		}

		internal bool Remove(AttachmentId id)
		{
			this.CheckDisposed(null);
			Util.ThrowOnNullArgument(id, "id");
			ExTraceGlobals.StorageTracer.Information((long)this.GetHashCode(), "Storage.CoreAttachmentCollection.Remove(AttachmentId)");
			this.InitCollection("Remove", true);
			AttachmentHandle attachmentHandle = null;
			return this.attachmentIdMap.TryGetValue(id, out attachmentHandle) && this.Remove(attachmentHandle);
		}

		internal void RemoveAll()
		{
			this.CheckDisposed(null);
			this.InitCollection("RemoveAll", true);
			IList<AttachmentHandle> allHandles = this.GetAllHandles();
			if (allHandles != null)
			{
				foreach (AttachmentHandle attachmentHandle in allHandles)
				{
					this.Remove(attachmentHandle);
				}
			}
			this.Reset(true);
		}

		internal CoreAttachment Open(AttachmentId attachmentId)
		{
			return this.Open(attachmentId, null);
		}

		internal CoreAttachment Open(AttachmentId attachmentId, ICollection<PropertyDefinition> preloadProperties)
		{
			this.CheckDisposed(null);
			Util.ThrowOnNullArgument(attachmentId, "attachmentId");
			this.InitCollection("Open", false);
			AttachmentHandle handle;
			if (!this.attachmentIdMap.TryGetValue(attachmentId, out handle))
			{
				throw new ObjectNotFoundException(ServerStrings.MapiCannotOpenAttachment);
			}
			return this.InternalOpen(handle, preloadProperties);
		}

		internal ICollection<PropertyDefinition> GetAttachmentLoadList(ICollection<PropertyDefinition> prefetchProperties, Schema attachmentSchema)
		{
			this.CheckDisposed(null);
			prefetchProperties = InternalSchema.Combine<PropertyDefinition>((ICollection<PropertyDefinition>)this.fetchProperties, prefetchProperties);
			return InternalSchema.Combine<PropertyDefinition>(attachmentSchema.AutoloadProperties, prefetchProperties);
		}

		internal CoreAttachment InternalCreate(AttachmentType? type)
		{
			this.CheckDisposed(null);
			this.InitCollection("InternalCreate", true);
			return this.InternalCreate(type, null, null, null);
		}

		internal CoreAttachment InternalCreateCopy(AttachmentType? type, CoreAttachment attachmentToClone)
		{
			this.CheckDisposed(null);
			this.InitCollection("InternalCreateCopy", true);
			CoreAttachment result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				CoreAttachment coreAttachment = this.InternalCreate(type, null, attachmentToClone, null);
				disposeGuard.Add<CoreAttachment>(coreAttachment);
				int valueOrDefault = attachmentToClone.PropertyBag.GetValueOrDefault<int>(InternalSchema.RenderingPosition, -1);
				coreAttachment.PropertyBag.SetOrDeleteProperty(InternalSchema.RenderingPosition, valueOrDefault);
				disposeGuard.Success();
				result = coreAttachment;
			}
			return result;
		}

		internal IList<AttachmentHandle> GetAllHandles()
		{
			this.CheckDisposed(null);
			this.InitCollection("AllAttachments", false);
			AttachmentHandle[] array = new AttachmentHandle[this.savedAttachmentNumberMap.Count];
			this.savedAttachmentNumberMap.Values.CopyTo(array, 0);
			return array;
		}

		internal void UpdateAttachmentId(AttachmentId attachmentId, int attachmentNumber)
		{
			this.CheckDisposed(null);
			if (this.isInitialized)
			{
				AttachmentHandle attachmentHandle = this.GetAttachmentHandle(attachmentNumber);
				attachmentHandle.AttachmentId = attachmentId;
				this.attachmentIdMap[attachmentId] = attachmentHandle;
			}
		}

		internal void OnBeforeAttachmentSave(AttachmentPropertyBag attachmentBag)
		{
			this.CheckDisposed(null);
			AttachmentHandle attachmentHandle = null;
			if (this.newAttachmentNumberMap.TryGetValue(attachmentBag.AttachmentNumber, out attachmentHandle))
			{
				attachmentHandle.UpdateProperties(attachmentBag);
				return;
			}
			if (this.savedAttachmentNumberMap.TryGetValue(attachmentBag.AttachmentNumber, out attachmentHandle))
			{
				attachmentHandle.UpdateProperties(attachmentBag);
			}
		}

		internal void OnAfterAttachmentSave(int attachmentNumber)
		{
			this.CheckDisposed(null);
			AttachmentHandle value = null;
			if (this.newAttachmentNumberMap.TryGetValue(attachmentNumber, out value))
			{
				this.newAttachmentNumberMap.Remove(attachmentNumber);
				this.savedAttachmentNumberMap.Add(attachmentNumber, value);
			}
			this.isDirty = true;
			this.IsClonedFromAnExistingAttachmentCollection = false;
		}

		internal void GetCharsetDetectionData(StringBuilder stringBuilder)
		{
			this.CheckDisposed(null);
			this.InitCollection("GetCharsetDetectionData", false);
			foreach (AttachmentHandle attachmentHandle in this.savedAttachmentNumberMap.Values)
			{
				stringBuilder.Append(attachmentHandle.CharsetDetectionData);
			}
		}

		internal bool Exists(int attachmentNumber)
		{
			this.CheckDisposed(null);
			this.InitCollection("Exists", false);
			return this.savedAttachmentNumberMap.ContainsKey(attachmentNumber) || this.newAttachmentNumberMap.ContainsKey(attachmentNumber);
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<CoreAttachmentCollection>(this);
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				this.Reset(false);
			}
			base.InternalDispose(disposing);
		}

		private static NativeStorePropertyDefinition[] CreatePrefetchPropertySet<T>(ICollection<T> propertySet) where T : PropertyDefinition
		{
			return StorePropertyDefinition.GetNativePropertyDefinitions<T>(PropertyDependencyType.AllRead, propertySet).ToArray<NativeStorePropertyDefinition>();
		}

		private static NativeStorePropertyDefinition[] CreatePrefetchPropertySet<T>(ICollection<NativeStorePropertyDefinition> oldSet, ICollection<T> additionalProperties) where T : PropertyDefinition
		{
			HashSet<NativeStorePropertyDefinition> hashSet = new HashSet<NativeStorePropertyDefinition>(oldSet);
			ICollection<NativeStorePropertyDefinition> nativePropertyDefinitions = StorePropertyDefinition.GetNativePropertyDefinitions<T>(PropertyDependencyType.AllRead, additionalProperties);
			foreach (NativeStorePropertyDefinition item in nativePropertyDefinitions)
			{
				hashSet.TryAdd(item);
			}
			return hashSet.ToArray();
		}

		internal void Reset()
		{
			this.CheckDisposed(null);
			this.Reset(false);
		}

		internal void OnAfterCoreItemSave(SaveResult saveResult)
		{
			if (saveResult == SaveResult.SuccessWithConflictResolution)
			{
				this.Reset();
			}
			else if (saveResult == SaveResult.IrresolvableConflict)
			{
				return;
			}
			if (this.coreItem.TopLevelItem == null && this.newAttachmentNumberMap.Count == 0)
			{
				this.isDirty = false;
			}
			this.IsClonedFromAnExistingAttachmentCollection = false;
		}

		internal void OpenAsReadWrite()
		{
			this.CheckDisposed(null);
			this.forceReadOnly = false;
		}

		private CoreAttachment InternalCreate(AttachmentType? type, string itemClass, CoreAttachment attachmentToClone, IItem itemToAttach)
		{
			this.InitCollection("InternalCreate", true);
			AttachmentPropertyBag attachmentPropertyBag = null;
			CoreAttachment coreAttachment = null;
			bool flag = false;
			try
			{
				bool flag2 = false;
				if (attachmentToClone != null)
				{
					flag2 = this.attachmentProvider.SupportsCreateClone(attachmentToClone.PropertyBag);
				}
				attachmentPropertyBag = this.InternalCreateAttachmentPropertyBag(type, flag2 ? attachmentToClone : null, itemToAttach);
				if (itemClass != null)
				{
					((IDirectPropertyBag)attachmentPropertyBag).SetValue(InternalSchema.ItemClass, itemClass);
				}
				AttachmentHandle attachmentHandle = new AttachmentHandle(attachmentPropertyBag.AttachmentNumber);
				coreAttachment = new CoreAttachment(this, attachmentPropertyBag, Origin.New);
				attachmentPropertyBag = null;
				if (attachmentToClone != null && !flag2)
				{
					coreAttachment.CopyAttachmentContentFrom(attachmentToClone);
				}
				this.newAttachmentNumberMap.Add(attachmentHandle.AttachNumber, attachmentHandle);
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					if (coreAttachment != null)
					{
						coreAttachment.Dispose();
						coreAttachment = null;
					}
					if (attachmentPropertyBag != null)
					{
						attachmentPropertyBag.Dispose();
						attachmentPropertyBag = null;
					}
				}
			}
			return coreAttachment;
		}

		private AttachmentPropertyBag InternalCreateAttachmentPropertyBag(AttachmentType? type, CoreAttachment attachmentToClone, IItem itemToAttach)
		{
			this.InitCollection("InternalCreateAttachmentPropertyBag", true);
			bool flag = false;
			int attachmentNumber = -1;
			PersistablePropertyBag persistablePropertyBag = null;
			AttachmentPropertyBag attachmentPropertyBag = null;
			try
			{
				Schema attachmentSchema = CoreAttachmentCollection.GetAttachmentSchema(type);
				ICollection<PropertyDefinition> prefetchProperties = InternalSchema.Combine<PropertyDefinition>(attachmentSchema.AutoloadProperties, (ICollection<PropertyDefinition>)this.fetchProperties);
				persistablePropertyBag = this.attachmentProvider.CreateAttachment(prefetchProperties, attachmentToClone, itemToAttach, out attachmentNumber);
				attachmentPropertyBag = new AttachmentPropertyBag(this.attachmentProvider, attachmentNumber, persistablePropertyBag, true);
				attachmentPropertyBag.ExTimeZone = this.ExTimeZone;
				if (type != null)
				{
					int num = CoreAttachment.AttachmentTypeToAttachMethod(type.Value);
					((IDirectPropertyBag)attachmentPropertyBag).SetValue(InternalSchema.AttachMethod, num);
				}
				this.isDirty = true;
				this.IsClonedFromAnExistingAttachmentCollection = false;
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					if (attachmentPropertyBag != null)
					{
						attachmentPropertyBag.Dispose();
						attachmentPropertyBag = null;
					}
					if (persistablePropertyBag != null)
					{
						persistablePropertyBag.Dispose();
						persistablePropertyBag = null;
					}
				}
			}
			return attachmentPropertyBag;
		}

		private CoreAttachment InternalOpen(AttachmentHandle handle, ICollection<PropertyDefinition> preloadProperties)
		{
			PropertyBag propertyBag = handle.GetAndRemoveCachedPropertyBag();
			if (propertyBag == null)
			{
				PropertyBag[] array = this.attachmentProvider.QueryAttachmentTable(this.fetchProperties.ToArray<NativeStorePropertyDefinition>());
				if (array == null || array.Length == 0)
				{
					throw new InvalidOperationException("Attachment table is empty.");
				}
				foreach (PropertyBag propertyBag2 in array)
				{
					int num = (int)propertyBag2[InternalSchema.AttachNum];
					if (handle.AttachNumber == num)
					{
						propertyBag = propertyBag2;
					}
					else
					{
						AttachmentHandle attachmentHandle = null;
						if (this.savedAttachmentNumberMap.TryGetValue(num, out attachmentHandle))
						{
							attachmentHandle.SetCachedPropertyBag(propertyBag2);
						}
					}
				}
			}
			if (propertyBag == null)
			{
				throw new InvalidOperationException("Attachment instance doesn't exist.");
			}
			CoreAttachment result = null;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				AttachmentPropertyBag attachmentPropertyBag = new AttachmentPropertyBag(this.attachmentProvider, handle.AttachNumber, propertyBag, this.fetchProperties, false);
				disposeGuard.Add<AttachmentPropertyBag>(attachmentPropertyBag);
				attachmentPropertyBag.ExTimeZone = this.ExTimeZone;
				if (preloadProperties == (ICollection<PropertyDefinition>)InternalSchema.ContentConversionProperties || (preloadProperties != null && preloadProperties.Count != 0))
				{
					attachmentPropertyBag.Load(preloadProperties);
				}
				this.UpdateAttachmentId(attachmentPropertyBag.AttachmentId, handle.AttachNumber);
				result = new CoreAttachment(this, attachmentPropertyBag, Origin.Existing);
				disposeGuard.Success();
			}
			return result;
		}

		private void Reset(bool newAttachmentsOnly)
		{
			if (this.isInitialized)
			{
				this.newAttachmentNumberMap.Clear();
				if (!newAttachmentsOnly)
				{
					this.savedAttachmentNumberMap.Clear();
					this.attachmentIdMap.Clear();
					this.isInitialized = false;
					this.IsClonedFromAnExistingAttachmentCollection = false;
				}
			}
		}

		private void InitCollection(string methodName, bool forWrite)
		{
			if (forWrite && this.IsReadOnly)
			{
				ExTraceGlobals.StorageTracer.TraceError<string>((long)this.GetHashCode(), "CoreAttachmentCollection::{0}. Cannot modify the collection because it's read only.", methodName);
				throw new AccessDeniedException(ServerStrings.ExItemIsOpenedInReadOnlyMode);
			}
			if (this.isInitialized)
			{
				return;
			}
			this.UpdateCollection();
			this.isInitialized = true;
		}

		private void UpdateCollection()
		{
			PropertyBag[] array = this.attachmentProvider.QueryAttachmentTable(this.fetchProperties.ToArray<NativeStorePropertyDefinition>());
			if (array == null || array.Length == 0)
			{
				return;
			}
			foreach (PropertyBag propertyBag in array)
			{
				int num = (int)propertyBag[InternalSchema.AttachNum];
				AttachmentHandle attachmentHandle = null;
				if (!this.savedAttachmentNumberMap.TryGetValue(num, out attachmentHandle))
				{
					attachmentHandle = new AttachmentHandle(num);
					this.savedAttachmentNumberMap.Add(num, attachmentHandle);
				}
				using (AttachmentPropertyBag attachmentPropertyBag = new AttachmentPropertyBag(this.attachmentProvider, num, propertyBag, this.fetchProperties, false))
				{
					attachmentHandle.UpdateProperties(attachmentPropertyBag);
					this.attachmentIdMap[attachmentHandle.AttachmentId] = attachmentHandle;
					attachmentHandle.SetCachedPropertyBag(propertyBag);
				}
			}
		}

		private AttachmentHandle GetAttachmentHandle(int attachmentNumber)
		{
			AttachmentHandle result = null;
			if (!this.savedAttachmentNumberMap.TryGetValue(attachmentNumber, out result))
			{
				throw new InvalidOperationException();
			}
			return result;
		}

		private static readonly NativeStorePropertyDefinition[] PrefetchPropertySet = CoreAttachmentCollection.CreatePrefetchPropertySet<PropertyDefinition>(AttachmentTableSchema.Instance.AutoloadProperties);

		private readonly ICoreItem coreItem;

		private readonly SortedDictionary<int, AttachmentHandle> savedAttachmentNumberMap = new SortedDictionary<int, AttachmentHandle>();

		private readonly Dictionary<int, AttachmentHandle> newAttachmentNumberMap = new Dictionary<int, AttachmentHandle>();

		private readonly Dictionary<AttachmentId, AttachmentHandle> attachmentIdMap = new Dictionary<AttachmentId, AttachmentHandle>();

		private readonly IAttachmentProvider attachmentProvider;

		private bool forceReadOnly;

		private bool isInitialized;

		private NativeStorePropertyDefinition[] fetchProperties;

		private bool isDirty;
	}
}
