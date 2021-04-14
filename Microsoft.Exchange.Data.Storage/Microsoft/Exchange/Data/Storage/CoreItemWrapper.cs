using System;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class CoreItemWrapper : CoreObjectWrapper, ICoreItem, ICoreObject, ICoreState, IValidatable, IDisposeTrackable, IDisposable, ILocationIdentifierController
	{
		internal CoreItemWrapper(ICoreItem coreItem) : base(coreItem)
		{
		}

		public override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<CoreItemWrapper>(this);
		}

		protected override void UnadviseEvents()
		{
			this.CoreItem.BeforeSend -= this.beforeSendEventHandler;
			this.beforeSendEventHandler = null;
			base.UnadviseEvents();
		}

		private ICoreItem CoreItem
		{
			get
			{
				return (ICoreItem)base.CoreObject;
			}
		}

		public CoreRecipientCollection Recipients
		{
			get
			{
				return this.CoreItem.Recipients;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return this.CoreItem.IsReadOnly;
			}
		}

		public ICoreItem TopLevelItem
		{
			get
			{
				return this.CoreItem.TopLevelItem;
			}
			set
			{
				this.CoreItem.TopLevelItem = value;
			}
		}

		public CoreAttachmentCollection AttachmentCollection
		{
			get
			{
				return this.CoreItem.AttachmentCollection;
			}
		}

		CoreRecipientCollection ICoreItem.GetRecipientCollection(bool forceOpen)
		{
			return this.CoreItem.GetRecipientCollection(forceOpen);
		}

		MapiMessage ICoreItem.MapiMessage
		{
			get
			{
				return this.CoreItem.MapiMessage;
			}
		}

		void ICoreItem.OpenAttachmentCollection(ICoreItem coreItem)
		{
			this.CoreItem.OpenAttachmentCollection(coreItem);
		}

		void ICoreItem.OpenAttachmentCollection()
		{
			this.CoreItem.OpenAttachmentCollection();
		}

		void ICoreItem.DisposeAttachmentCollection()
		{
			this.CoreItem.DisposeAttachmentCollection();
		}

		void ICoreItem.OpenAsReadWrite()
		{
			this.CoreItem.OpenAsReadWrite();
		}

		ConflictResolutionResult ICoreItem.Save(SaveMode saveMode)
		{
			return this.CoreItem.Save(saveMode);
		}

		ConflictResolutionResult ICoreItem.Flush(SaveMode saveMode)
		{
			return this.CoreItem.Flush(saveMode);
		}

		ConflictResolutionResult ICoreItem.InternalFlush(SaveMode saveMode, CallbackContext callbackContext)
		{
			return this.CoreItem.InternalFlush(saveMode, callbackContext);
		}

		ConflictResolutionResult ICoreItem.InternalFlush(SaveMode saveMode, CoreItemOperation operation, CallbackContext callbackContext)
		{
			return this.CoreItem.InternalFlush(saveMode, operation, callbackContext);
		}

		ConflictResolutionResult ICoreItem.InternalSave(SaveMode saveMode, CallbackContext callbackContext)
		{
			return this.CoreItem.InternalSave(saveMode, callbackContext);
		}

		void ICoreItem.SaveRecipients()
		{
			this.CoreItem.SaveRecipients();
		}

		void ICoreItem.AbandonRecipientChanges()
		{
			this.CoreItem.AbandonRecipientChanges();
		}

		void ICoreItem.Submit()
		{
			this.CoreItem.Submit();
		}

		void ICoreItem.Submit(SubmitMessageFlags submitFlags)
		{
			this.CoreItem.Submit(submitFlags);
		}

		void ICoreItem.TransportSend(out PropertyDefinition[] properties, out object[] values)
		{
			this.CoreItem.TransportSend(out properties, out values);
		}

		PropertyError[] ICoreItem.CopyItem(ICoreItem destinationItem, CopyPropertiesFlags copyPropertiesFlags, CopySubObjects copySubObjects, NativeStorePropertyDefinition[] excludeProperties)
		{
			return this.CoreItem.CopyItem(destinationItem, copyPropertiesFlags, copySubObjects, excludeProperties);
		}

		PropertyError[] ICoreItem.CopyProperties(ICoreItem destinationItem, CopyPropertiesFlags copyPropertiesFlags, NativeStorePropertyDefinition[] includeProperties)
		{
			return this.CoreItem.CopyProperties(destinationItem, copyPropertiesFlags, includeProperties);
		}

		bool ICoreItem.IsAttachmentCollectionLoaded
		{
			get
			{
				return this.CoreItem.IsAttachmentCollectionLoaded;
			}
		}

		void ICoreItem.Reload()
		{
			this.CoreItem.Reload();
		}

		bool ICoreItem.AreOptionalAutoloadPropertiesLoaded
		{
			get
			{
				return this.CoreItem.AreOptionalAutoloadPropertiesLoaded;
			}
		}

		void ICoreItem.SetIrresolvableChange()
		{
			this.CoreItem.SetIrresolvableChange();
		}

		event Action ICoreItem.BeforeSend
		{
			add
			{
				this.beforeSendEventHandler = (Action)Delegate.Combine(this.beforeSendEventHandler, value);
				this.CoreItem.BeforeSend += value;
			}
			remove
			{
				this.beforeSendEventHandler = (Action)Delegate.Remove(this.beforeSendEventHandler, value);
				this.CoreItem.BeforeSend -= value;
			}
		}

		PropertyBagSaveFlags ICoreItem.SaveFlags
		{
			get
			{
				return this.CoreItem.SaveFlags;
			}
			set
			{
				this.CoreItem.SaveFlags = value;
			}
		}

		Body ICoreItem.Body
		{
			get
			{
				return this.CoreItem.Body;
			}
		}

		ItemCharsetDetector ICoreItem.CharsetDetector
		{
			get
			{
				return this.CoreItem.CharsetDetector;
			}
		}

		int ICoreItem.PreferredInternetCodePageForShiftJis
		{
			set
			{
				this.CoreItem.PreferredInternetCodePageForShiftJis = value;
			}
		}

		int ICoreItem.RequiredCoverage
		{
			set
			{
				this.CoreItem.RequiredCoverage = value;
			}
		}

		void ICoreItem.GetCharsetDetectionData(StringBuilder stringBuilder, CharsetDetectionDataFlags flags)
		{
			this.CoreItem.GetCharsetDetectionData(stringBuilder, flags);
		}

		void ICoreItem.SetCoreItemContext(ICoreItemContext context)
		{
			this.CoreItem.SetCoreItemContext(context);
		}

		LocationIdentifierHelper ILocationIdentifierController.LocationIdentifierHelperInstance
		{
			get
			{
				return this.CoreItem.LocationIdentifierHelperInstance;
			}
		}

		public void SetReadFlag(int flags, bool deferErrors)
		{
			this.CoreItem.SetReadFlag(flags, deferErrors);
		}

		private Action beforeSendEventHandler;
	}
}
