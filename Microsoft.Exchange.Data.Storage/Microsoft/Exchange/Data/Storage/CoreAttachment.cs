using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class CoreAttachment : DisposableObject, ICoreState, IValidatable
	{
		internal CoreAttachment(CoreAttachmentCollection parentCollection, AttachmentPropertyBag propertyBag, Origin origin)
		{
			this.parentCollection = parentCollection;
			this.propertyBag = propertyBag;
			this.attachmentNumber = propertyBag.AttachmentNumber;
			this.Origin = origin;
			this.propertyBag.Context.CoreState = this;
			this.propertyBag.Context.Session = this.parentCollection.ContainerItem.Session;
		}

		public AttachmentPropertyBag PropertyBag
		{
			get
			{
				this.CheckDisposed(null);
				return this.propertyBag;
			}
		}

		public StoreSession Session
		{
			get
			{
				this.CheckDisposed(null);
				return this.parentCollection.ContainerItem.Session;
			}
		}

		public AttachmentType AttachmentType
		{
			get
			{
				this.CheckDisposed(null);
				int? attachMethod = this.PropertyBag.TryGetProperty(InternalSchema.AttachMethod) as int?;
				return CoreAttachmentCollection.GetAttachmentType(attachMethod);
			}
		}

		public int AttachmentNumber
		{
			get
			{
				this.CheckDisposed(null);
				return this.attachmentNumber;
			}
		}

		public PropertyBagSaveFlags SaveFlags
		{
			get
			{
				this.CheckDisposed(null);
				return this.PropertyBag.SaveFlags;
			}
			set
			{
				this.CheckDisposed(null);
				EnumValidator.ThrowIfInvalid<PropertyBagSaveFlags>(value, "value");
				this.PropertyBag.SaveFlags = value;
			}
		}

		public Origin Origin
		{
			get
			{
				this.CheckDisposed(null);
				return this.origin;
			}
			set
			{
				this.CheckDisposed(null);
				EnumValidator.ThrowIfInvalid<Origin>(value);
				this.origin = value;
			}
		}

		public ItemLevel ItemLevel
		{
			get
			{
				this.CheckDisposed(null);
				return ItemLevel.Attached;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				this.CheckDisposed(null);
				return this.parentCollection.IsReadOnly;
			}
		}

		bool IValidatable.ValidateAllProperties
		{
			get
			{
				this.CheckDisposed(null);
				return this.enableFullValidation || this.Origin == Origin.New;
			}
		}

		Schema IValidatable.Schema
		{
			get
			{
				this.CheckDisposed(null);
				return this.GetSchema();
			}
		}

		internal bool IsNew
		{
			get
			{
				this.CheckDisposed(null);
				return this.Origin == Origin.New;
			}
		}

		internal CoreAttachmentCollection ParentCollection
		{
			get
			{
				this.CheckDisposed(null);
				return this.parentCollection;
			}
		}

		internal AttachmentId Id
		{
			get
			{
				this.CheckDisposed(null);
				return this.PropertyBag.AttachmentId;
			}
		}

		internal bool IsInline
		{
			get
			{
				this.CheckDisposed(null);
				return this.PropertyBag.GetValueOrDefault<bool>(InternalSchema.AttachmentIsInline);
			}
			set
			{
				this.CheckDisposed(null);
				this.PropertyBag.SetOrDeleteProperty(InternalSchema.AttachmentIsInline, value);
				this.PropertyBag.SetOrDeleteProperty(InternalSchema.AttachCalendarHidden, value);
			}
		}

		internal bool IsCalendarException
		{
			get
			{
				return this.PropertyBag.IsCalendarException;
			}
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(base.GetType().FullName);
			stringBuilder.AppendLine();
			if (base.IsDisposed)
			{
				stringBuilder.AppendLine("disposed");
			}
			else
			{
				if (this.parentCollection != null && this.parentCollection.ContainerItem != null)
				{
					stringBuilder.AppendFormat("Item: {0}", this.parentCollection.ContainerItem.ToString());
					stringBuilder.AppendLine();
				}
				if (this.Id != null)
				{
					stringBuilder.AppendFormat("Attachment id: {0}", this.Id.ToBase64String());
					stringBuilder.AppendLine();
				}
			}
			return stringBuilder.ToString();
		}

		public void Flush()
		{
			this.CheckDisposed(null);
			this.CoreObjectUpdate();
			PersistablePropertyBag persistablePropertyBag = CoreObject.GetPersistablePropertyBag(this.ParentCollection.ContainerItem);
			if (!persistablePropertyBag.Context.IsValidationDisabled)
			{
				ValidationContext context = new ValidationContext(this.Session);
				Validation.Validate(this, context);
			}
			this.PropertyBag.FlushChanges();
		}

		public void Save()
		{
			this.CheckDisposed(null);
			this.Flush();
			this.PropertyBag.SaveChanges(false);
			this.origin = Origin.Existing;
		}

		public ICoreItem OpenEmbeddedItem(PropertyOpenMode openMode, params PropertyDefinition[] propertiesToLoad)
		{
			EnumValidator<PropertyOpenMode>.ThrowIfInvalid(openMode);
			this.CheckDisposed(null);
			if (openMode == PropertyOpenMode.Create)
			{
				if (this.AttachmentType != AttachmentType.EmbeddedMessage)
				{
					((IDirectPropertyBag)this.PropertyBag).SetValue(InternalSchema.AttachMethod, 5);
				}
			}
			else if (this.AttachmentType != AttachmentType.EmbeddedMessage)
			{
				throw new InvalidOperationException("Cannot get the embedded message of an attachment whose type is not AttachmentType.EmbeddedMessage.");
			}
			bool noMessageDecoding = this.parentCollection.ContainerItem.CharsetDetector.NoMessageDecoding;
			return this.PropertyBag.OpenAttachedItem(openMode, propertiesToLoad, noMessageDecoding);
		}

		public PropertyError[] CopyAttachment(CoreAttachment destinationAttachment, CopyPropertiesFlags copyPropertiesFlags, CopySubObjects copySubObjects, NativeStorePropertyDefinition[] excludeProperties)
		{
			this.CheckDisposed(null);
			Util.ThrowOnNullArgument(destinationAttachment, "destinationAttachment");
			Util.ThrowOnNullArgument(excludeProperties, "excludeProperties");
			EnumValidator.ThrowIfInvalid<CopyPropertiesFlags>(copyPropertiesFlags, "copyPropertiesFlags");
			EnumValidator.ThrowIfInvalid<CopySubObjects>(copySubObjects, "copySubObjects");
			return CoreObject.MapiCopyTo(this.PropertyBag.MapiProp, destinationAttachment.PropertyBag.MapiProp, this.Session, destinationAttachment.Session, copyPropertiesFlags, copySubObjects, excludeProperties);
		}

		public PropertyError[] CopyProperties(CoreAttachment destinationAttachment, CopyPropertiesFlags copyPropertiesFlags, NativeStorePropertyDefinition[] includeProperties)
		{
			this.CheckDisposed(null);
			Util.ThrowOnNullArgument(destinationAttachment, "destinationAttachment");
			Util.ThrowOnNullArgument(includeProperties, "includeProperties");
			EnumValidator.ThrowIfInvalid<CopyPropertiesFlags>(copyPropertiesFlags, "copyPropertiesFlags");
			return CoreObject.MapiCopyProps(this.PropertyBag.MapiProp, destinationAttachment.PropertyBag.MapiProp, this.Session, destinationAttachment.Session, copyPropertiesFlags, includeProperties);
		}

		void IValidatable.Validate(ValidationContext context, IList<StoreObjectValidationError> validationErrors)
		{
			this.CheckDisposed(null);
			Validation.ValidateProperties(context, this, this.PropertyBag, validationErrors);
		}

		internal static int AttachmentTypeToAttachMethod(AttachmentType type)
		{
			switch (type)
			{
			case AttachmentType.NoAttachment:
				return 0;
			case AttachmentType.Stream:
				return 1;
			case AttachmentType.EmbeddedMessage:
				return 5;
			case AttachmentType.Ole:
				return 6;
			case AttachmentType.Reference:
				return 7;
			default:
				throw new InvalidOperationException("AttachmentTypeToAttachMethod: Invalid attachment type");
			}
		}

		internal static Uri GetUriProperty(CoreAttachment coreAttachment, StorePropertyDefinition property)
		{
			string text = ((IDirectPropertyBag)coreAttachment.PropertyBag).GetValue(property) as string;
			if (string.IsNullOrEmpty(text))
			{
				return null;
			}
			Uri result;
			if (!Uri.TryCreate(text, UriKind.RelativeOrAbsolute, out result))
			{
				ExTraceGlobals.StorageTracer.TraceError<StorePropertyDefinition, string>((long)coreAttachment.GetHashCode(), "CoreAttachment.GetUriProperty: {0} is not a valid URI\r\n'{1}'", property, text);
				return null;
			}
			return result;
		}

		internal void SetEnableFullValidation(bool enableFullValidation)
		{
			this.CheckDisposed(null);
			this.enableFullValidation = enableFullValidation;
		}

		internal AttachmentSchema GetSchema()
		{
			this.CheckDisposed(null);
			switch (this.AttachmentType)
			{
			case AttachmentType.EmbeddedMessage:
				return ItemAttachmentSchema.Instance;
			case AttachmentType.Reference:
				return ReferenceAttachmentSchema.Instance;
			}
			return StreamAttachmentBaseSchema.Instance;
		}

		internal void CopyAttachmentContentFrom(CoreAttachment sourceAttachment)
		{
			this.CheckDisposed(null);
			sourceAttachment.PropertyBag.Load(InternalSchema.ContentConversionProperties);
			foreach (NativeStorePropertyDefinition property in sourceAttachment.PropertyBag.AllNativeProperties)
			{
				if (CoreAttachment.ShouldPropertyBeCopied(property, sourceAttachment.AttachmentType, this.AttachmentType))
				{
					PersistablePropertyBag.CopyProperty(sourceAttachment.PropertyBag, property, this.PropertyBag);
				}
			}
			if (sourceAttachment.AttachmentType == AttachmentType.EmbeddedMessage && this.AttachmentType == AttachmentType.EmbeddedMessage)
			{
				bool noMessageDecoding = sourceAttachment.ParentCollection.ContainerItem.CharsetDetector.NoMessageDecoding;
				bool noMessageDecoding2 = this.parentCollection.ContainerItem.CharsetDetector.NoMessageDecoding;
				using (ICoreItem coreItem = sourceAttachment.PropertyBag.OpenAttachedItem(PropertyOpenMode.ReadOnly, InternalSchema.ContentConversionProperties, noMessageDecoding))
				{
					using (ICoreItem coreItem2 = this.PropertyBag.OpenAttachedItem(PropertyOpenMode.Create, InternalSchema.ContentConversionProperties, noMessageDecoding2))
					{
						CoreItem.CopyItemContent(coreItem, coreItem2);
						using (Item item = Item.InternalBindCoreItem(coreItem2))
						{
							item.CharsetDetector.DetectionOptions.PreferredInternetCodePageForShiftJis = coreItem.PropertyBag.GetValueOrDefault<int>(ItemSchema.InternetCpid, 50222);
							item.LocationIdentifierHelperInstance.SetLocationIdentifier(64373U);
							item.SaveFlags = (((PersistablePropertyBag)coreItem.PropertyBag).SaveFlags | PropertyBagSaveFlags.IgnoreMapiComputedErrors | PropertyBagSaveFlags.IgnoreAccessDeniedErrors);
							item.Save(SaveMode.NoConflictResolution);
						}
					}
				}
			}
			this.PropertyBag.SaveFlags |= (PropertyBagSaveFlags.IgnoreMapiComputedErrors | PropertyBagSaveFlags.IgnoreUnresolvedHeaders);
		}

		internal void Validate(ValidationContext context)
		{
			this.CheckDisposed(null);
			Validation.Validate(this, context);
		}

		internal void UpdateParentMsgRichContentFlags(RichContentFlags richContentFlags)
		{
			this.CheckDisposed(null);
			if (this.parentCollection == null || this.parentCollection.ContainerItem == null)
			{
				return;
			}
			ICoreItem containerItem = this.parentCollection.ContainerItem;
			ICorePropertyBag corePropertyBag = containerItem.PropertyBag;
			corePropertyBag.Load(new PropertyDefinition[]
			{
				InternalSchema.RichContent
			});
			short valueOrDefault = corePropertyBag.GetValueOrDefault<short>(InternalSchema.RichContent, 0);
			short num = valueOrDefault | (short)richContentFlags;
			corePropertyBag[InternalSchema.RichContent] = num;
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				this.propertyBag.Dispose();
			}
			base.InternalDispose(disposing);
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<CoreAttachment>(this);
		}

		[Conditional("DEBUG")]
		private static void DebugCheckContains(PropertyDefinition[] properties, PropertyDefinition property)
		{
			ICollection<NativeStorePropertyDefinition> nativePropertyDefinitions = StorePropertyDefinition.GetNativePropertyDefinitions<PropertyDefinition>(PropertyDependencyType.AllRead, new PropertyDefinition[]
			{
				property
			});
			foreach (NativeStorePropertyDefinition nativeStorePropertyDefinition in nativePropertyDefinitions)
			{
			}
		}

		private static bool ShouldPropertyBeCopied(PropertyDefinition property, AttachmentType sourceType, AttachmentType targetType)
		{
			return property != InternalSchema.AttachMethod && (property != InternalSchema.AttachDataObj || (targetType != AttachmentType.EmbeddedMessage && sourceType == targetType));
		}

		private void CoreObjectUpdate()
		{
			this.GetSchema().CoreObjectUpdate(this);
		}

		private readonly AttachmentPropertyBag propertyBag;

		private readonly CoreAttachmentCollection parentCollection;

		private readonly int attachmentNumber;

		private bool enableFullValidation = true;

		private Origin origin;
	}
}
