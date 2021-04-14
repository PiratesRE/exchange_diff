using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class AttachmentPropertyBag : PersistablePropertyBag
	{
		internal AttachmentPropertyBag(IAttachmentProvider attachmentProvider, int attachmentNumber, PropertyBag attachmentTablePropertyBag, ICollection<NativeStorePropertyDefinition> attachmentTablePropertySet, bool useCreateFlagOnConnect)
		{
			Util.ThrowOnNullArgument(attachmentProvider, "attachmentProvider");
			Util.ThrowOnNullArgument(attachmentTablePropertyBag, "attachmentTablePropertyBag");
			Util.ThrowOnNullArgument(attachmentTablePropertySet, "attachmentTablePropertySet");
			this.attachmentProvider = attachmentProvider;
			this.attachmentNumber = attachmentNumber;
			this.attachmentTablePropertyBag = attachmentTablePropertyBag;
			this.attachmentTablePropertySet = attachmentTablePropertySet;
			this.persistablePropertyBag = null;
			this.useCreateFlagOnConnect = useCreateFlagOnConnect;
			this.PrefetchPropertyArray = attachmentTablePropertySet.ToArray<NativeStorePropertyDefinition>();
		}

		internal AttachmentPropertyBag(IAttachmentProvider attachmentProvider, int attachmentNumber, PersistablePropertyBag persistablePropertyBag, bool useCreateFlagOnConnect)
		{
			Util.ThrowOnNullArgument(attachmentProvider, "attachmentProvider");
			Util.ThrowOnNullArgument(persistablePropertyBag, "persistablePropertyBag");
			this.attachmentProvider = attachmentProvider;
			this.attachmentNumber = attachmentNumber;
			this.attachmentTablePropertyBag = null;
			this.attachmentTablePropertySet = null;
			this.persistablePropertyBag = persistablePropertyBag;
			this.useCreateFlagOnConnect = useCreateFlagOnConnect;
			this.PrefetchPropertyArray = persistablePropertyBag.PrefetchPropertyArray;
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				this.attachmentProvider.OnCollectionDisposed(this, this.persistablePropertyBag);
				this.persistablePropertyBag = null;
				this.saveFlags = PropertyBagSaveFlags.Default;
			}
			base.InternalDispose(disposing);
		}

		public void ReleaseConnection()
		{
			this.attachmentProvider.OnAttachmentDisconnected(this, this.persistablePropertyBag);
			this.persistablePropertyBag = null;
			this.saveFlags = PropertyBagSaveFlags.Default;
		}

		public override void Load(ICollection<PropertyDefinition> properties)
		{
			base.CheckDisposed("Load");
			if (!this.attachmentProvider.ExistsInCollection(this))
			{
				throw new ObjectNotFoundException(ServerStrings.InvalidAttachmentNumber);
			}
			ICollection<NativeStorePropertyDefinition> collection = null;
			if (properties != InternalSchema.ContentConversionProperties)
			{
				collection = StorePropertyDefinition.GetNativePropertyDefinitions<PropertyDefinition>(PropertyDependencyType.AllRead, properties);
				properties = collection.ToArray<NativeStorePropertyDefinition>();
			}
			if (this.persistablePropertyBag == null)
			{
				bool flag = false;
				if (collection != null)
				{
					IDirectPropertyBag currentPropertyBag = this.CurrentPropertyBag;
					using (IEnumerator<NativeStorePropertyDefinition> enumerator = collection.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							NativeStorePropertyDefinition propertyDefinition = enumerator.Current;
							if (!currentPropertyBag.IsLoaded(propertyDefinition))
							{
								flag = true;
								break;
							}
						}
						goto IL_8A;
					}
				}
				flag = true;
				IL_8A:
				if (flag)
				{
					this.OnOpenConnection(properties);
					return;
				}
			}
			else
			{
				this.persistablePropertyBag.Load(properties);
				this.attachmentProvider.OnAttachmentLoad(this);
			}
		}

		protected override object TryGetStoreProperty(StorePropertyDefinition propertyDefinition)
		{
			base.CheckDisposed("AttachmentPropertyBag::TryGetStoreProperty");
			if (this.persistablePropertyBag == null)
			{
				NativeStorePropertyDefinition nativeStorePropertyDefinition = propertyDefinition as NativeStorePropertyDefinition;
				if (nativeStorePropertyDefinition == null)
				{
					return propertyDefinition.Get((PropertyBag.BasicPropertyStore)this);
				}
				if (this.attachmentTablePropertySet.Contains(nativeStorePropertyDefinition))
				{
					object value = ((IDirectPropertyBag)this.attachmentTablePropertyBag).GetValue(nativeStorePropertyDefinition);
					PropertyError propertyError = value as PropertyError;
					if (propertyError == null || propertyError.PropertyErrorCode != PropertyErrorCode.PropertyValueTruncated)
					{
						return value;
					}
				}
				this.OnOpenConnection();
			}
			return ((IDirectPropertyBag)this.CurrentPropertyBag).GetValue(propertyDefinition);
		}

		protected override void SetValidatedStoreProperty(StorePropertyDefinition propertyDefinition, object propertyValue)
		{
			base.CheckDisposed("SetValidatedStoreProperty");
			this.OnDataChanged();
			((IDirectPropertyBag)this.CurrentPropertyBag).SetValue(propertyDefinition, propertyValue);
		}

		protected override void DeleteStoreProperty(StorePropertyDefinition propertyDefinition)
		{
			base.CheckDisposed("DeleteStoreProperty");
			this.OnDataChanged();
			((IDirectPropertyBag)this.CurrentPropertyBag).Delete(propertyDefinition);
		}

		public override void Clear()
		{
			if (this.persistablePropertyBag != null)
			{
				this.persistablePropertyBag.Clear();
			}
			this.attachmentTablePropertyBag = null;
			this.saveFlags = PropertyBagSaveFlags.Default;
		}

		public override void Reload()
		{
			this.Clear();
			this.Load(null);
		}

		public override ICollection<PropertyDefinition> AllFoundProperties
		{
			get
			{
				base.CheckDisposed("AllFoundProperties");
				this.OnOpenConnection();
				return this.persistablePropertyBag.AllFoundProperties;
			}
		}

		internal override ICollection<NativeStorePropertyDefinition> AllNativeProperties
		{
			get
			{
				base.CheckDisposed("AllNativeProperties");
				if (this.persistablePropertyBag == null)
				{
					return this.attachmentTablePropertySet;
				}
				return this.persistablePropertyBag.AllNativeProperties;
			}
		}

		internal override PropertyBagContext Context
		{
			get
			{
				if (this.persistablePropertyBag != null)
				{
					return this.persistablePropertyBag.Context;
				}
				return this.attachmentTablePropertyBag.Context;
			}
		}

		protected override bool IsLoaded(NativeStorePropertyDefinition propertyDefinition)
		{
			base.CheckDisposed("IsLoaded");
			return ((IDirectPropertyBag)this.CurrentPropertyBag).IsLoaded(propertyDefinition);
		}

		public override bool HasAllPropertiesLoaded
		{
			get
			{
				base.CheckDisposed("HasAllPropertiesLoaded::get");
				return this.persistablePropertyBag != null && this.persistablePropertyBag.HasAllPropertiesLoaded;
			}
		}

		internal override MapiProp MapiProp
		{
			get
			{
				base.CheckDisposed("StoreObject");
				this.OnOpenConnection();
				return this.persistablePropertyBag.MapiProp;
			}
		}

		internal PersistablePropertyBag PersistablePropertyBag
		{
			get
			{
				base.CheckDisposed("PersistablePropertyBag");
				this.OnOpenConnection();
				return this.persistablePropertyBag;
			}
		}

		public override Stream OpenPropertyStream(PropertyDefinition propertyDefinition, PropertyOpenMode openMode)
		{
			base.CheckDisposed("OpenPropertyStream");
			EnumValidator.AssertValid<PropertyOpenMode>(openMode);
			this.OnOpenConnection();
			return this.persistablePropertyBag.OpenPropertyStream(propertyDefinition, openMode);
		}

		public void UpdateAttachmentTableData(PropertyBag attachmentTableData, ICollection<NativeStorePropertyDefinition> attachmentTablePropertySet, bool forceReset)
		{
			base.CheckDisposed("UpdateAttachmentTableData");
			if (this.attachmentTablePropertyBag == null || forceReset)
			{
				this.attachmentTablePropertyBag = attachmentTableData;
				this.attachmentTablePropertySet = attachmentTablePropertySet;
			}
		}

		internal override void FlushChanges()
		{
			base.CheckDisposed("FlushChanges");
			this.OnOpenConnection();
			this.savedData = this.ComputeSavedData();
			this.attachmentProvider.OnBeforeAttachmentSave(this);
			this.persistablePropertyBag.SaveFlags = this.saveFlags;
			this.persistablePropertyBag.FlushChanges();
		}

		internal override void SaveChanges(bool force)
		{
			base.CheckDisposed("SaveChanges");
			this.persistablePropertyBag.SaveChanges(force);
			this.attachmentTablePropertyBag = null;
			this.saveFlags = PropertyBagSaveFlags.Default;
			this.attachmentProvider.OnAfterAttachmentSave(this);
			this.useCreateFlagOnConnect = false;
		}

		private AttachmentPropertyBag.SavedData ComputeSavedData()
		{
			AttachmentPropertyBag.SavedData savedData = new AttachmentPropertyBag.SavedData();
			savedData.AttachmentId = this.AttachmentId;
			savedData.AttachMethod = this.AttachMethod;
			StringBuilder stringBuilder = new StringBuilder();
			this.ComputeCharsetDetectionData(stringBuilder);
			savedData.CharsetDetectionString = stringBuilder.ToString();
			int? num = ((IDirectPropertyBag)this.CurrentPropertyBag).GetValue(InternalSchema.AttachCalendarFlags) as int?;
			savedData.IsCalendarException = CoreAttachmentCollection.IsCalendarException((num != null) ? num.Value : 0);
			bool? flag = base.TryGetProperty(InternalSchema.AttachmentIsInline) as bool?;
			savedData.IsInline = (flag != null && flag.Value);
			return savedData;
		}

		internal void ComputeCharsetDetectionData(StringBuilder stringBuilder)
		{
			bool flag = this.persistablePropertyBag == null;
			if (flag && this.attachmentTablePropertyBag == null)
			{
				stringBuilder.Append(this.savedData.CharsetDetectionString);
				return;
			}
			foreach (StorePropertyDefinition propertyDefinition in this.Schema.DetectCodepageProperties)
			{
				string text = this.TryGetStoreProperty(propertyDefinition) as string;
				if (text != null)
				{
					stringBuilder.AppendLine(text);
				}
			}
			if (flag && this.persistablePropertyBag != null)
			{
				this.ReleaseConnection();
			}
		}

		public override bool IsDirty
		{
			get
			{
				base.CheckDisposed("IsDirty::get");
				return this.CurrentPropertyBag.IsDirty;
			}
		}

		internal bool IsCalendarException
		{
			get
			{
				if (this.attachmentTablePropertyBag != null)
				{
					int? num = this.TryGetStoreProperty(InternalSchema.AttachCalendarFlags) as int?;
					return CoreAttachmentCollection.IsCalendarException((num != null) ? num.Value : 0);
				}
				return this.savedData.IsCalendarException;
			}
		}

		internal bool IsInline
		{
			get
			{
				if (this.attachmentTablePropertyBag != null)
				{
					bool? flag = base.TryGetProperty(InternalSchema.AttachmentIsInline) as bool?;
					return flag != null && flag.Value;
				}
				return this.savedData.IsInline;
			}
		}

		protected override bool InternalIsPropertyDirty(AtomicStorePropertyDefinition propertyDefinition)
		{
			base.CheckDisposed("InternalIsPropertyDirty");
			return ((IDirectPropertyBag)this.CurrentPropertyBag).IsDirty(propertyDefinition);
		}

		internal override ExTimeZone ExTimeZone
		{
			get
			{
				return this.CurrentPropertyBag.ExTimeZone;
			}
			set
			{
				this.CurrentPropertyBag.ExTimeZone = value;
			}
		}

		internal bool UseCreateFlagOnConnect
		{
			get
			{
				return this.useCreateFlagOnConnect;
			}
		}

		internal override PropertyBagSaveFlags SaveFlags
		{
			get
			{
				base.CheckDisposed("AttachmentPropertyBag.SaveFlags.get");
				return this.saveFlags;
			}
			set
			{
				base.CheckDisposed("AttachmentPropertyBag.SaveFlags.set");
				EnumValidator.ThrowIfInvalid<PropertyBagSaveFlags>(value, "value");
				this.saveFlags = value;
			}
		}

		internal override void SetUpdateImapIdFlag()
		{
			throw new NotSupportedException();
		}

		public ICoreItem OpenAttachedItem(PropertyOpenMode openMode, ICollection<PropertyDefinition> propertiesToLoad, bool noMessageDecoding)
		{
			ICoreItem coreItem = this.attachmentProvider.OpenAttachedItem(propertiesToLoad, this, openMode == PropertyOpenMode.Create);
			coreItem.CharsetDetector.NoMessageDecoding = noMessageDecoding;
			return coreItem;
		}

		internal int AttachmentNumber
		{
			get
			{
				return this.attachmentNumber;
			}
		}

		internal AttachmentId AttachmentId
		{
			get
			{
				PropertyBag currentPropertyBag = this.CurrentPropertyBag;
				if (currentPropertyBag == null)
				{
					return this.savedData.AttachmentId;
				}
				byte[] array = ((IDirectPropertyBag)currentPropertyBag).GetValue(InternalSchema.RecordKey) as byte[];
				if (array == null)
				{
					return null;
				}
				return new AttachmentId(array);
			}
		}

		internal int AttachMethod
		{
			get
			{
				PropertyBag currentPropertyBag = this.CurrentPropertyBag;
				if (currentPropertyBag == null)
				{
					return this.savedData.AttachMethod;
				}
				int? num = ((IDirectPropertyBag)currentPropertyBag).GetValue(InternalSchema.AttachMethod) as int?;
				if (num != null)
				{
					return num.Value;
				}
				throw new InvalidOperationException("AttachMethod is not found");
			}
		}

		internal Schema Schema
		{
			get
			{
				return CoreAttachmentCollection.GetAttachmentSchema(this.AttachMethod);
			}
		}

		internal bool IsEmpty
		{
			get
			{
				base.CheckDisposed("IsEmpty");
				return this.CurrentPropertyBag == null;
			}
		}

		private PropertyBag CurrentPropertyBag
		{
			get
			{
				return this.persistablePropertyBag ?? this.attachmentTablePropertyBag;
			}
		}

		private static Schema GetAttachmentSchema(int attachMethod)
		{
			switch (attachMethod)
			{
			case 2:
			case 3:
			case 4:
			case 7:
				return ReferenceAttachmentSchema.Instance;
			case 5:
				return ItemAttachmentSchema.Instance;
			}
			return StreamAttachmentBaseSchema.Instance;
		}

		private void OnDataChanged()
		{
			this.OnOpenConnection();
		}

		private void OnOpenConnection()
		{
			this.OnOpenConnection(null);
		}

		private void OnOpenConnection(ICollection<PropertyDefinition> propertiesToLoad)
		{
			if (this.persistablePropertyBag == null)
			{
				Schema attachmentSchema = AttachmentPropertyBag.GetAttachmentSchema(this.AttachMethod);
				propertiesToLoad = InternalSchema.Combine<PropertyDefinition>(attachmentSchema.AutoloadProperties, propertiesToLoad);
				this.persistablePropertyBag = this.attachmentProvider.OpenAttachment(propertiesToLoad, this);
				this.persistablePropertyBag.Context.Session = this.attachmentTablePropertyBag.Context.Session;
				this.persistablePropertyBag.Context.CoreState = this.attachmentTablePropertyBag.Context.CoreState;
				this.UpdateAttachmentTableCache();
			}
			else if (propertiesToLoad != null)
			{
				this.persistablePropertyBag.Load(propertiesToLoad);
			}
			this.attachmentProvider.OnAttachmentLoad(this);
		}

		private void UpdateAttachmentTableCache()
		{
			if (this.attachmentTablePropertyBag != null)
			{
				bool flag = false;
				foreach (NativeStorePropertyDefinition propertyDefinition in this.attachmentTablePropertySet)
				{
					PropertyError propertyError = ((IDirectPropertyBag)this.attachmentTablePropertyBag).GetValue(propertyDefinition) as PropertyError;
					if (propertyError != null && propertyError.PropertyErrorCode == PropertyErrorCode.PropertyValueTruncated)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					return;
				}
			}
			MemoryPropertyBag memoryPropertyBag = new MemoryPropertyBag();
			IDirectPropertyBag directPropertyBag = memoryPropertyBag;
			foreach (NativeStorePropertyDefinition propertyDefinition2 in this.attachmentTablePropertySet)
			{
				object value = ((IDirectPropertyBag)this.persistablePropertyBag).GetValue(propertyDefinition2);
				directPropertyBag.SetValue(propertyDefinition2, value);
			}
			memoryPropertyBag.ClearChangeInfo();
			memoryPropertyBag.Context.Session = this.attachmentTablePropertyBag.Context.Session;
			memoryPropertyBag.Context.CoreState = this.attachmentTablePropertyBag.Context.CoreState;
			this.attachmentTablePropertyBag = memoryPropertyBag;
		}

		public override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<AttachmentPropertyBag>(this);
		}

		private int attachmentNumber;

		private bool useCreateFlagOnConnect;

		private AttachmentPropertyBag.SavedData savedData;

		private PropertyBagSaveFlags saveFlags;

		private PropertyBag attachmentTablePropertyBag;

		private ICollection<NativeStorePropertyDefinition> attachmentTablePropertySet;

		private PersistablePropertyBag persistablePropertyBag;

		private IAttachmentProvider attachmentProvider;

		private class SavedData
		{
			public AttachmentId AttachmentId;

			public string CharsetDetectionString;

			public int AttachMethod;

			public bool IsCalendarException;

			public bool IsInline;
		}
	}
}
