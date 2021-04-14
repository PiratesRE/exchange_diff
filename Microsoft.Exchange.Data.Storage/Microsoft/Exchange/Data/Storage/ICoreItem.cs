using System;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface ICoreItem : ICoreObject, ICoreState, IValidatable, IDisposeTrackable, IDisposable, ILocationIdentifierController
	{
		CoreAttachmentCollection AttachmentCollection { get; }

		CoreRecipientCollection Recipients { get; }

		MapiMessage MapiMessage { get; }

		bool IsReadOnly { get; }

		PropertyBagSaveFlags SaveFlags { get; set; }

		void OpenAsReadWrite();

		ConflictResolutionResult Save(SaveMode saveMode);

		ConflictResolutionResult Flush(SaveMode saveMode);

		void OpenAttachmentCollection();

		void OpenAttachmentCollection(ICoreItem owner);

		CoreRecipientCollection GetRecipientCollection(bool forceOpen);

		void DisposeAttachmentCollection();

		ConflictResolutionResult InternalFlush(SaveMode saveMode, CoreItemOperation operation, CallbackContext callbackContext);

		ConflictResolutionResult InternalFlush(SaveMode saveMode, CallbackContext callbackContext);

		ConflictResolutionResult InternalSave(SaveMode saveMode, CallbackContext callbackContext);

		void SaveRecipients();

		void AbandonRecipientChanges();

		void Submit();

		void Submit(SubmitMessageFlags submitFlags);

		void TransportSend(out PropertyDefinition[] properties, out object[] values);

		PropertyError[] CopyItem(ICoreItem destinationItem, CopyPropertiesFlags copyPropertiesFlags, CopySubObjects copySubObjects, NativeStorePropertyDefinition[] excludeProperties);

		PropertyError[] CopyProperties(ICoreItem destinationItem, CopyPropertiesFlags copyPropertiesFlags, NativeStorePropertyDefinition[] includeProperties);

		bool IsAttachmentCollectionLoaded { get; }

		void Reload();

		bool AreOptionalAutoloadPropertiesLoaded { get; }

		ICoreItem TopLevelItem { get; set; }

		void SetIrresolvableChange();

		event Action BeforeSend;

		Body Body { get; }

		ItemCharsetDetector CharsetDetector { get; }

		int PreferredInternetCodePageForShiftJis { set; }

		int RequiredCoverage { set; }

		void GetCharsetDetectionData(StringBuilder stringBuilder, CharsetDetectionDataFlags flags);

		void SetCoreItemContext(ICoreItemContext context);

		void SetReadFlag(int flags, bool deferErrors);
	}
}
