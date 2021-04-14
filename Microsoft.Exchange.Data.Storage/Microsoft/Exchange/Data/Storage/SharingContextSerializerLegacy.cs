using System;
using System.Globalization;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class SharingContextSerializerLegacy
	{
		internal SharingContextSerializerLegacy(SharingContext context)
		{
			Util.ThrowOnNullArgument(context, "context");
			this.context = context;
		}

		internal void ReadFromMessageItem(MessageItem messageItem, bool isDraft)
		{
			byte[] array = messageItem.GetValueOrDefault<byte[]>(InternalSchema.ProviderGuidBinary, null);
			if (array == null)
			{
				ExTraceGlobals.SharingTracer.TraceDebug<string>((long)this.GetHashCode(), "{0}: ProviderGuidBinary is missing, reading from XSharingProviderGuid", messageItem.Session.UserLegacyDN);
				string valueOrDefault = messageItem.GetValueOrDefault<string>(InternalSchema.XSharingProviderGuid, null);
				if (valueOrDefault == null)
				{
					ExTraceGlobals.SharingTracer.TraceError<string>((long)this.GetHashCode(), "{0}: XSharingProviderGuid is missing", messageItem.Session.UserLegacyDN);
					throw new NotSupportedSharingMessageException();
				}
				try
				{
					array = HexConverter.HexStringToByteArray(valueOrDefault);
				}
				catch (FormatException)
				{
					ExTraceGlobals.SharingTracer.TraceError<string>((long)this.GetHashCode(), "{0}: XSharingProviderGuid is invalid", messageItem.Session.UserLegacyDN);
					throw new NotSupportedSharingMessageException();
				}
			}
			SharingProvider sharingProvider = SharingProvider.FromGuid(new Guid(array));
			if (sharingProvider == null)
			{
				ExTraceGlobals.SharingTracer.TraceError<string, byte[]>((long)this.GetHashCode(), "{0}: Unknown sharing provider guid: {1}", messageItem.Session.UserLegacyDN, array);
				throw new NotSupportedSharingMessageException();
			}
			ExTraceGlobals.SharingTracer.TraceDebug<string, SharingProvider>((long)this.GetHashCode(), "{0}: Find provider {1} that is specified in message.", messageItem.Session.UserLegacyDN, sharingProvider);
			if (sharingProvider == SharingProvider.SharingProviderPublish)
			{
				ExTraceGlobals.SharingTracer.TraceDebug<string, SharingProvider>((long)this.GetHashCode(), "{0}: Read from message x-properties for provider {1}.", messageItem.Session.UserLegacyDN, sharingProvider);
				this.ReadFromMessageXProperties(messageItem);
			}
			if (isDraft || sharingProvider != SharingProvider.SharingProviderPublish)
			{
				ExTraceGlobals.SharingTracer.TraceDebug<string, SharingProvider>((long)this.GetHashCode(), "{0}: Read from message properties for provider {1}.", messageItem.Session.UserLegacyDN, sharingProvider);
				this.ReadFromMessageProperties(messageItem);
			}
			if (this.context.SharingMessageType == SharingMessageType.Unknown && !isDraft)
			{
				ExTraceGlobals.SharingTracer.TraceError<string>((long)this.GetHashCode(), "{0}: SharingMessageType is unknown", messageItem.Session.UserLegacyDN);
				throw new InvalidSharingMessageException("SharingFlavor");
			}
			this.context.AvailableSharingProviders.Clear();
			if (isDraft && !this.context.SharingMessageType.IsResponseToRequest)
			{
				SharingProvider[] array2 = null;
				using (Folder folder = Folder.Bind(messageItem.Session, this.context.FolderId))
				{
					array2 = SharingProvider.GetCompatibleProviders(sharingProvider, folder);
				}
				foreach (SharingProvider sharingProvider2 in array2)
				{
					ExTraceGlobals.SharingTracer.TraceDebug<string, SharingProvider>((long)this.GetHashCode(), "{0}: Find compatible provider {1}.", messageItem.Session.UserLegacyDN, sharingProvider2);
					this.context.AvailableSharingProviders.Add(sharingProvider2, null);
				}
				return;
			}
			this.context.AvailableSharingProviders.Add(sharingProvider, null);
		}

		private void ReadFromMessageProperties(MessageItem messageItem)
		{
			SharingFlavor? valueAsNullable = messageItem.GetValueAsNullable<SharingFlavor>(InternalSchema.SharingFlavor);
			if (valueAsNullable == null)
			{
				ExTraceGlobals.SharingTracer.TraceError<string>((long)this.GetHashCode(), "{0}: SharingFlavor is missing", messageItem.Session.UserLegacyDN);
				throw new InvalidSharingMessageException("SharingFlavor");
			}
			this.context.SharingFlavor = valueAsNullable.Value;
			SharingCapabilities? valueAsNullable2 = messageItem.GetValueAsNullable<SharingCapabilities>(InternalSchema.SharingCapabilities);
			if (valueAsNullable2 == null)
			{
				ExTraceGlobals.SharingTracer.TraceDebug<string>((long)this.GetHashCode(), "{0}: SharingCapabilities is missing, use default value", messageItem.Session.UserLegacyDN);
				this.context.SetDefaultCapabilities();
			}
			else
			{
				this.context.SharingCapabilities = valueAsNullable2.Value;
			}
			string valueOrDefault = messageItem.GetValueOrDefault<string>(InternalSchema.SharingInitiatorName, null);
			if (valueOrDefault == null)
			{
				ExTraceGlobals.SharingTracer.TraceError<string>((long)this.GetHashCode(), "{0}: SharingInitiatorName is missing.", messageItem.Session.UserLegacyDN);
				throw new InvalidSharingMessageException("SharingInitiatorName");
			}
			this.context.InitiatorName = valueOrDefault;
			string valueOrDefault2 = messageItem.GetValueOrDefault<string>(InternalSchema.SharingInitiatorSmtp, null);
			if (valueOrDefault2 == null)
			{
				ExTraceGlobals.SharingTracer.TraceError<string>((long)this.GetHashCode(), "{0}: SharingInitiatorSmtp is missing.", messageItem.Session.UserLegacyDN);
				throw new InvalidSharingMessageException("SharingInitiatorSmtp");
			}
			if (!SmtpAddress.IsValidSmtpAddress(valueOrDefault2))
			{
				ExTraceGlobals.SharingTracer.TraceError<string, string>((long)this.GetHashCode(), "{0}: SharingInitiatorSmtp is invalid: {1}", messageItem.Session.UserLegacyDN, valueOrDefault2);
				throw new InvalidSharingMessageException("SharingInitiatorSmtp");
			}
			this.context.InitiatorSmtpAddress = valueOrDefault2;
			byte[] valueOrDefault3 = messageItem.GetValueOrDefault<byte[]>(InternalSchema.SharingInitiatorEntryId, null);
			if (valueOrDefault3 == null)
			{
				ExTraceGlobals.SharingTracer.TraceError<string>((long)this.GetHashCode(), "{0}: SharingInitiatorEntryId is missing.", messageItem.Session.UserLegacyDN);
				throw new InvalidSharingMessageException("SharingInitiatorEntryId");
			}
			if (!AddressBookEntryId.IsAddressBookEntryId(valueOrDefault3))
			{
				ExTraceGlobals.SharingTracer.TraceError<string, byte[]>((long)this.GetHashCode(), "{0}: SharingInitiatorEntryId is invalid: {1}", messageItem.Session.UserLegacyDN, valueOrDefault3);
				throw new InvalidSharingMessageException("SharingInitiatorEntryId");
			}
			this.context.InitiatorEntryId = valueOrDefault3;
			string valueOrDefault4 = messageItem.GetValueOrDefault<string>(InternalSchema.SharingRemoteType, null);
			if (valueOrDefault4 == null)
			{
				ExTraceGlobals.SharingTracer.TraceError<string>((long)this.GetHashCode(), "{0}: SharingRemoteType is missing.", messageItem.Session.UserLegacyDN);
				throw new InvalidSharingMessageException("SharingRemoteType");
			}
			if (SharingDataType.FromContainerClass(valueOrDefault4) == null)
			{
				ExTraceGlobals.SharingTracer.TraceError<string, string>((long)this.GetHashCode(), "{0}: SharingRemoteType is invalid: {1}.", messageItem.Session.UserLegacyDN, valueOrDefault4);
				throw new InvalidSharingMessageException("SharingRemoteType");
			}
			this.context.FolderClass = valueOrDefault4;
			string valueOrDefault5 = messageItem.GetValueOrDefault<string>(InternalSchema.SharingRemoteName, null);
			if (valueOrDefault5 == null)
			{
				ExTraceGlobals.SharingTracer.TraceError<string>((long)this.GetHashCode(), "{0}: SharingRemoteName is missing.", messageItem.Session.UserLegacyDN);
				throw new InvalidSharingMessageException("SharingRemoteName");
			}
			this.context.FolderName = valueOrDefault5;
			string valueOrDefault6 = messageItem.GetValueOrDefault<string>(InternalSchema.SharingRemoteUid, null);
			if (valueOrDefault6 == null)
			{
				ExTraceGlobals.SharingTracer.TraceError<string>((long)this.GetHashCode(), "{0}: SharingRemoteUid is missing.", messageItem.Session.UserLegacyDN);
				throw new InvalidSharingMessageException("SharingRemoteUid");
			}
			try
			{
				this.context.FolderId = StoreObjectId.FromHexEntryId(valueOrDefault6, ObjectClass.GetObjectType(valueOrDefault4));
			}
			catch (CorruptDataException)
			{
				ExTraceGlobals.SharingTracer.TraceError<string, string>((long)this.GetHashCode(), "{0}: SharingRemoteUid is invalid: {1}", messageItem.Session.UserLegacyDN, valueOrDefault6);
				throw new InvalidSharingMessageException("SharingRemoteUid");
			}
			string valueOrDefault7 = messageItem.GetValueOrDefault<string>(InternalSchema.SharingRemoteStoreUid, null);
			if (valueOrDefault7 == null)
			{
				ExTraceGlobals.SharingTracer.TraceError<string>((long)this.GetHashCode(), "{0}: SharingRemoteStoreUid is missing.", messageItem.Session.UserLegacyDN);
				throw new InvalidSharingMessageException("SharingRemoteStoreUid");
			}
			byte[] array = null;
			try
			{
				array = HexConverter.HexStringToByteArray(valueOrDefault7);
			}
			catch (FormatException)
			{
			}
			if (array == null || StoreEntryId.TryParseStoreEntryIdMailboxDN(array) == null)
			{
				ExTraceGlobals.SharingTracer.TraceError<string, string>((long)this.GetHashCode(), "{0}: SharingRemoteStoreUid is invalid: {1}", messageItem.Session.UserLegacyDN, valueOrDefault7);
				throw new InvalidSharingMessageException("SharingRemoteStoreUid");
			}
			this.context.MailboxId = array;
			SharingContextPermissions? valueAsNullable3 = messageItem.GetValueAsNullable<SharingContextPermissions>(InternalSchema.SharingPermissions);
			if (valueAsNullable3 != null)
			{
				this.context.SharingPermissions = valueAsNullable3.Value;
			}
			SharingContextDetailLevel? valueAsNullable4 = messageItem.GetValueAsNullable<SharingContextDetailLevel>(InternalSchema.SharingDetail);
			if (valueAsNullable4 != null)
			{
				this.context.SharingDetail = valueAsNullable4.Value;
				return;
			}
			if (this.context.DataType == SharingDataType.Calendar)
			{
				this.context.SharingDetail = SharingContextDetailLevel.FullDetails;
			}
		}

		private void ReadFromMessageXProperties(MessageItem messageItem)
		{
			string valueOrDefault = messageItem.GetValueOrDefault<string>(InternalSchema.XSharingFlavor, null);
			if (valueOrDefault == null)
			{
				ExTraceGlobals.SharingTracer.TraceError<string>((long)this.GetHashCode(), "{0}: XSharingFlavor is missing", messageItem.Session.UserLegacyDN);
				throw new InvalidSharingMessageException("XSharingFlavor");
			}
			try
			{
				this.context.SharingFlavor = (SharingFlavor)int.Parse(valueOrDefault, NumberStyles.HexNumber);
			}
			catch (FormatException)
			{
				ExTraceGlobals.SharingTracer.TraceError<string, string>((long)this.GetHashCode(), "{0}: XSharingFlavor is invalid: {1}", messageItem.Session.UserLegacyDN, valueOrDefault);
				throw new InvalidSharingMessageException("XSharingFlavor");
			}
			string valueOrDefault2 = messageItem.GetValueOrDefault<string>(InternalSchema.XSharingRemoteType, null);
			if (valueOrDefault2 == null)
			{
				ExTraceGlobals.SharingTracer.TraceError<string>((long)this.GetHashCode(), "{0}: XSharingRemoteType is missing.", messageItem.Session.UserLegacyDN);
				throw new InvalidSharingMessageException("XSharingRemoteType");
			}
			if (SharingDataType.FromPublishName(valueOrDefault2) == null)
			{
				ExTraceGlobals.SharingTracer.TraceError<string, string>((long)this.GetHashCode(), "{0}: XSharingRemoteType is invalid: {1}.", messageItem.Session.UserLegacyDN, valueOrDefault2);
				throw new InvalidSharingMessageException("XSharingRemoteType");
			}
			string valueOrDefault3 = messageItem.GetValueOrDefault<string>(InternalSchema.XSharingLocalType, null);
			if (valueOrDefault3 == null)
			{
				ExTraceGlobals.SharingTracer.TraceError<string>((long)this.GetHashCode(), "{0}: XSharingLocalType is missing.", messageItem.Session.UserLegacyDN);
				throw new InvalidSharingMessageException("XSharingLocalType");
			}
			if (SharingDataType.FromContainerClass(valueOrDefault3) == null)
			{
				ExTraceGlobals.SharingTracer.TraceError<string, string>((long)this.GetHashCode(), "{0}: XSharingLocalType is invalid: {1}.", messageItem.Session.UserLegacyDN, valueOrDefault3);
				throw new InvalidSharingMessageException("XSharingLocalType");
			}
			this.context.FolderClass = valueOrDefault3;
			string valueOrDefault4 = messageItem.GetValueOrDefault<string>(InternalSchema.XSharingRemoteName, null);
			if (valueOrDefault4 == null)
			{
				ExTraceGlobals.SharingTracer.TraceError<string>((long)this.GetHashCode(), "{0}: XSharingRemoteName is missing.", messageItem.Session.UserLegacyDN);
				throw new InvalidSharingMessageException("XSharingRemoteName");
			}
			this.context.FolderName = valueOrDefault4;
			Uri uri = null;
			string valueOrDefault5 = messageItem.GetValueOrDefault<string>(InternalSchema.XSharingBrowseUrl, null);
			if (string.IsNullOrEmpty(valueOrDefault5))
			{
				ExTraceGlobals.SharingTracer.TraceDebug<string>((long)this.GetHashCode(), "{0}: XSharingBrowseUrl is missing or empty.", messageItem.Session.UserLegacyDN);
			}
			else
			{
				if (!PublishingUrl.IsAbsoluteUriString(valueOrDefault5, out uri))
				{
					ExTraceGlobals.SharingTracer.TraceError<string, string>((long)this.GetHashCode(), "{0}: XSharingBrowseUrl is not well formed : {1}", messageItem.Session.UserLegacyDN, valueOrDefault5);
					throw new InvalidSharingMessageException("XSharingBrowseUrl");
				}
				this.context.BrowseUrl = valueOrDefault5;
			}
			string valueOrDefault6 = messageItem.GetValueOrDefault<string>(InternalSchema.XSharingRemotePath, null);
			if (valueOrDefault6 == null)
			{
				ExTraceGlobals.SharingTracer.TraceError<string>((long)this.GetHashCode(), "{0}: XSharingRemotePath is missing.", messageItem.Session.UserLegacyDN);
				throw new InvalidSharingMessageException("XSharingRemotePath");
			}
			if (!PublishingUrl.IsAbsoluteUriString(valueOrDefault6, out uri))
			{
				ExTraceGlobals.SharingTracer.TraceError<string, string>((long)this.GetHashCode(), "{0}: XSharingRemotePath is not well formed : {1}", messageItem.Session.UserLegacyDN, valueOrDefault6);
				throw new InvalidSharingMessageException("XSharingRemotePath");
			}
			this.context.ICalUrl = valueOrDefault6;
		}

		internal void SaveIntoMessageProperties(MessageItem messageItem, bool isClear)
		{
			SharingProvider primarySharingProvider = this.context.PrimarySharingProvider;
			messageItem.SetOrDeleteProperty(InternalSchema.ProviderGuidBinary, isClear ? null : primarySharingProvider.Guid.ToByteArray());
			messageItem.SetOrDeleteProperty(InternalSchema.SharingProviderName, isClear ? null : primarySharingProvider.Name);
			messageItem.SetOrDeleteProperty(InternalSchema.SharingProviderUrl, isClear ? null : primarySharingProvider.Url);
			messageItem.SetOrDeleteProperty(InternalSchema.SharingCapabilities, isClear ? null : this.context.SharingCapabilities);
			messageItem.SetOrDeleteProperty(InternalSchema.SharingFlavor, isClear ? null : this.context.SharingFlavor);
			messageItem.SetOrDeleteProperty(InternalSchema.SharingRemoteType, isClear ? null : this.context.FolderClass);
			messageItem.SetOrDeleteProperty(InternalSchema.SharingLocalType, isClear ? null : this.context.FolderClass);
			messageItem.SetOrDeleteProperty(InternalSchema.SharingRemoteName, isClear ? null : this.context.FolderName);
			messageItem.SetOrDeleteProperty(InternalSchema.SharingLocalName, isClear ? null : this.context.FolderName);
			messageItem.SetOrDeleteProperty(InternalSchema.SharingRemoteUid, isClear ? null : this.context.FolderId.ToHexEntryId());
			messageItem.SetOrDeleteProperty(InternalSchema.SharingLocalUid, isClear ? null : this.context.FolderId.ToHexEntryId());
			messageItem.SetOrDeleteProperty(InternalSchema.SharingRemoteStoreUid, isClear ? null : HexConverter.ByteArrayToHexString(this.context.MailboxId));
			messageItem.SetOrDeleteProperty(InternalSchema.SharingLocalStoreUid, isClear ? null : HexConverter.ByteArrayToHexString(this.context.MailboxId));
			messageItem.SetOrDeleteProperty(InternalSchema.SharingInitiatorName, isClear ? null : this.context.InitiatorName);
			messageItem.SetOrDeleteProperty(InternalSchema.SharingInitiatorSmtp, isClear ? null : this.context.InitiatorSmtpAddress);
			messageItem.SetOrDeleteProperty(InternalSchema.SharingInitiatorEntryId, isClear ? null : this.context.InitiatorEntryId);
			messageItem.SetOrDeleteProperty(InternalSchema.SharingPermissions, isClear ? null : this.context.SharingPermissions);
			messageItem.SetOrDeleteProperty(InternalSchema.SharingDetail, isClear ? null : this.context.SharingDetail);
		}

		internal void SaveIntoMessageXProperties(MessageItem messageItem, bool isClear)
		{
			SharingProvider sharingProviderPublish = SharingProvider.SharingProviderPublish;
			messageItem.SetOrDeleteProperty(InternalSchema.XSharingProviderGuid, isClear ? null : HexConverter.ByteArrayToHexString(sharingProviderPublish.Guid.ToByteArray()));
			messageItem.SetOrDeleteProperty(InternalSchema.XSharingProviderName, isClear ? null : sharingProviderPublish.Name);
			messageItem.SetOrDeleteProperty(InternalSchema.XSharingProviderUrl, isClear ? null : sharingProviderPublish.Url);
			messageItem.SetOrDeleteProperty(InternalSchema.XSharingCapabilities, isClear ? null : string.Format("{0:X}", 521));
			messageItem.SetOrDeleteProperty(InternalSchema.XSharingFlavor, isClear ? null : string.Format("{0:X}", 784));
			messageItem.SetOrDeleteProperty(InternalSchema.XSharingRemoteType, isClear ? null : this.context.DataType.PublishName);
			messageItem.SetOrDeleteProperty(InternalSchema.XSharingLocalType, isClear ? null : this.context.FolderClass);
			messageItem.SetOrDeleteProperty(InternalSchema.XSharingRemoteName, isClear ? null : this.context.FolderName);
			messageItem.SetOrDeleteProperty(InternalSchema.XSharingBrowseUrl, isClear ? null : this.context.BrowseUrl.ToString());
			messageItem.SetOrDeleteProperty(InternalSchema.XSharingRemotePath, isClear ? null : this.context.ICalUrl.ToString());
			messageItem.SetOrDeleteProperty(InternalSchema.XSharingInstanceGuid, isClear ? null : HexConverter.ByteArrayToHexString(Guid.NewGuid().ToByteArray()));
		}

		private const SharingCapabilities XCapabilities = SharingCapabilities.UrlConfiguration | SharingCapabilities.RestrictedStorage | SharingCapabilities.ScopeSingleFolder;

		private const SharingFlavor XFlavor = SharingFlavor.SharingOut | SharingFlavor.SharingMessage | SharingFlavor.SharingMessageInvitation;

		private SharingContext context;
	}
}
