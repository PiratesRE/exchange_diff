using System;
using System.Collections.Generic;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class SharingContext
	{
		internal SharingContext(Folder folderToShare) : this(folderToShare, null)
		{
		}

		internal SharingContext(Folder folderToShare, SharingProvider sharingProvider) : this()
		{
			Util.ThrowOnNullArgument(folderToShare, "folderToShare");
			MailboxSession mailboxSession = folderToShare.Session as MailboxSession;
			IExchangePrincipal mailboxOwner = mailboxSession.MailboxOwner;
			if (sharingProvider == null)
			{
				SharingProvider[] compatibleProviders = SharingProvider.GetCompatibleProviders(folderToShare);
				if (compatibleProviders.Length == 0)
				{
					ExTraceGlobals.SharingTracer.TraceError<IExchangePrincipal, VersionedId>((long)this.GetHashCode(), "{0}: Cannot share folder {1}: no compatible provider was found.", mailboxOwner, folderToShare.Id);
					throw new CannotShareFolderException(ServerStrings.NoProviderSupportShareFolder);
				}
				for (int i = 0; i < compatibleProviders.Length; i++)
				{
					ExTraceGlobals.SharingTracer.TraceDebug<IExchangePrincipal, SharingProvider, VersionedId>((long)this.GetHashCode(), "{0}: Find compatible provider {1} for folder {2}.", mailboxOwner, compatibleProviders[i], folderToShare.Id);
					this.AvailableSharingProviders.Add(compatibleProviders[i], null);
				}
			}
			else if (!sharingProvider.IsCompatible(folderToShare))
			{
				ExTraceGlobals.SharingTracer.TraceError<IExchangePrincipal, VersionedId, SharingProvider>((long)this.GetHashCode(), "{0}: Cannot share folder {1} with sharing provider: {2}.", mailboxOwner, folderToShare.Id, sharingProvider);
				if (sharingProvider == SharingProvider.SharingProviderPublish)
				{
					throw new FolderNotPublishedException();
				}
				throw new CannotShareFolderException(ServerStrings.NoProviderSupportShareFolder);
			}
			else
			{
				this.AvailableSharingProviders.Add(sharingProvider, null);
				if (sharingProvider == SharingProvider.SharingProviderPublish)
				{
					this.PopulateUrls(folderToShare);
				}
			}
			this.InitiatorName = mailboxOwner.MailboxInfo.DisplayName;
			this.InitiatorSmtpAddress = mailboxOwner.MailboxInfo.PrimarySmtpAddress.ToString();
			this.InitiatorEntryId = AddressBookEntryId.MakeAddressBookEntryID(mailboxOwner);
			this.FolderClass = folderToShare.ClassName;
			this.FolderId = folderToShare.StoreObjectId;
			this.IsPrimary = (mailboxSession.IsDefaultFolderType(this.FolderId) != DefaultFolderType.None);
			this.FolderName = (this.IsPrimary ? this.DataType.DisplayName.ToString(mailboxSession.InternalPreferedCulture) : folderToShare.DisplayName);
			this.MailboxId = StoreEntryId.ToProviderStoreEntryId(mailboxOwner);
			this.SharingMessageType = SharingMessageType.Invitation;
			this.SharingPermissions = SharingContextPermissions.Reviewer;
			if (StringComparer.OrdinalIgnoreCase.Equals(folderToShare.ClassName, "IPF.Appointment"))
			{
				this.SharingDetail = (this.IsPrimary ? SharingContextDetailLevel.AvailabilityOnly : SharingContextDetailLevel.FullDetails);
			}
			this.SetDefaultCapabilities();
			this.UserLegacyDN = mailboxOwner.LegacyDn;
		}

		private SharingContext()
		{
		}

		internal Dictionary<SharingProvider, CheckRecipientsResults> AvailableSharingProviders
		{
			get
			{
				return this.availableSharingProviders;
			}
		}

		internal SharingDataType DataType
		{
			get
			{
				return SharingDataType.FromContainerClass(this.FolderClass);
			}
		}

		internal bool IsPrimary
		{
			get
			{
				return (this.SharingFlavor & SharingFlavor.PrimaryOwnership) == SharingFlavor.PrimaryOwnership;
			}
			set
			{
				if (value)
				{
					this.SharingFlavor |= SharingFlavor.PrimaryOwnership;
					return;
				}
				this.SharingFlavor &= ~SharingFlavor.PrimaryOwnership;
			}
		}

		internal SharingMessageType SharingMessageType
		{
			get
			{
				return SharingMessageType.GetSharingMessageType(this.SharingFlavor);
			}
			set
			{
				if (value.IsRequest && !this.IsPrimary)
				{
					throw new ArgumentException("Cannot request non-default folder!");
				}
				this.SharingFlavor &= ~this.SharingMessageType.SharingFlavor;
				this.SharingFlavor |= value.SharingFlavor;
			}
		}

		internal string InitiatorName { get; set; }

		internal string InitiatorSmtpAddress { get; set; }

		internal byte[] InitiatorEntryId { get; set; }

		internal string FolderClass { get; set; }

		internal string FolderName { get; set; }

		internal string FolderEwsId { get; set; }

		internal StoreObjectId FolderId { get; set; }

		internal byte[] MailboxId { get; set; }

		internal SharingCapabilities SharingCapabilities { get; set; }

		internal SharingFlavor SharingFlavor { get; set; }

		internal EncryptedSharedFolderData[] EncryptedSharedFolderDataCollection { get; set; }

		internal SharingContextPermissions SharingPermissions { get; set; }

		internal SharingContextDetailLevel SharingDetail { get; set; }

		internal string BrowseUrl { get; set; }

		internal string ICalUrl { get; set; }

		internal string UserLegacyDN { get; set; }

		internal int CountOfApplied { get; set; }

		private SharingContextSerializer Serializer
		{
			get
			{
				if (this.serializer == null)
				{
					this.serializer = new SharingContextSerializer(this);
				}
				return this.serializer;
			}
		}

		private SharingContextSerializerLegacy SerializerLegacy
		{
			get
			{
				if (this.serializerLegacy == null)
				{
					this.serializerLegacy = new SharingContextSerializerLegacy(this);
				}
				return this.serializerLegacy;
			}
		}

		internal SharingProvider PrimarySharingProvider
		{
			get
			{
				if (this.AvailableSharingProviders.ContainsKey(SharingProvider.SharingProviderInternal))
				{
					return SharingProvider.SharingProviderInternal;
				}
				List<SharingProvider> list = new List<SharingProvider>(this.AvailableSharingProviders.Keys);
				if (list.Count <= 0)
				{
					return null;
				}
				return list[0];
			}
		}

		internal SharingProvider FallbackSharingProvider
		{
			get
			{
				if (this.AvailableSharingProviders.ContainsKey(SharingProvider.SharingProviderPublish))
				{
					return SharingProvider.SharingProviderPublish;
				}
				return null;
			}
		}

		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"SharingMessageType = ",
				this.SharingMessageType,
				",InitiatorName = ",
				this.InitiatorName,
				",InitiatorSmtpAddress = ",
				this.InitiatorSmtpAddress,
				",InitiatorEntryId = ",
				(this.InitiatorEntryId == null) ? string.Empty : HexConverter.ByteArrayToHexString(this.InitiatorEntryId),
				",FolderClass = ",
				this.FolderClass,
				",FolderName = ",
				this.FolderName,
				",FolderEwsId = ",
				this.FolderEwsId,
				",FolderId = ",
				(this.FolderId == null) ? string.Empty : this.FolderId.ToHexEntryId(),
				",MailboxId = ",
				(this.MailboxId == null) ? string.Empty : HexConverter.ByteArrayToHexString(this.MailboxId),
				",SharingCapabilities = ",
				this.SharingCapabilities,
				",SharingFlavor = ",
				this.SharingFlavor,
				",SharingPermissions = ",
				this.SharingPermissions,
				",SharingDetail = ",
				this.SharingDetail,
				",EncryptedSharedFolderDataCollection = ",
				this.EncryptedSharedFolderDataCollection
			});
		}

		internal static SharingContext DeserializeFromDraft(MessageItem messageItem)
		{
			Util.ThrowOnNullArgument(messageItem, "messageItem");
			return new SharingContext
			{
				UserLegacyDN = messageItem.Session.UserLegacyDN
			}.DeserializeFromMessage(messageItem, true);
		}

		internal static SharingContext DeserializeFromMessage(MessageItem messageItem)
		{
			Util.ThrowOnNullArgument(messageItem, "messageItem");
			return new SharingContext
			{
				UserLegacyDN = messageItem.Session.UserLegacyDN
			}.DeserializeFromMessage(messageItem, false);
		}

		internal void SerializeToDraft(MessageItem messageItem)
		{
			Util.ThrowOnNullArgument(messageItem, "messageItem");
			ExTraceGlobals.SharingTracer.TraceDebug<string, SharingContext>((long)this.GetHashCode(), "{0}: Serialize as MAPI properties into draft message. SharingContext: {1}", messageItem.Session.UserLegacyDN, this);
			this.SerializerLegacy.SaveIntoMessageProperties(messageItem, false);
			if (this.PrimarySharingProvider == SharingProvider.SharingProviderPublish)
			{
				ExTraceGlobals.SharingTracer.TraceDebug<string, SharingContext>((long)this.GetHashCode(), "{0}: Serialize as x-properties into draft message. SharingContext: {1}", messageItem.Session.UserLegacyDN, this);
				this.SerializerLegacy.SaveIntoMessageXProperties(messageItem, false);
			}
		}

		internal void SerializeToMessage(MessageItem messageItem)
		{
			Util.ThrowOnNullArgument(messageItem, "messageItem");
			if (this.ShouldGenerateXmlMetadata)
			{
				ExTraceGlobals.SharingTracer.TraceDebug<string, SharingContext>((long)this.GetHashCode(), "{0}: Serialize as sharing_metadata.xml into message. SharingContext: {1}", messageItem.Session.UserLegacyDN, this);
				this.Serializer.SaveIntoMetadataXml(messageItem);
			}
			if (this.NeedToBeCompatibleWithO12InternalSharing)
			{
				ExTraceGlobals.SharingTracer.TraceDebug<string, SharingContext>((long)this.GetHashCode(), "{0}: Serialize as MAPI properties into message. SharingContext: {1}", messageItem.Session.UserLegacyDN, this);
				this.SerializerLegacy.SaveIntoMessageProperties(messageItem, false);
			}
			else
			{
				this.SerializerLegacy.SaveIntoMessageProperties(messageItem, true);
			}
			if (this.NeedToBeCompatibleWithO12PubCalSharing)
			{
				ExTraceGlobals.SharingTracer.TraceDebug<string, SharingContext>((long)this.GetHashCode(), "{0}: Serialize as X-Sharing properties into message. SharingContext: {1}", messageItem.Session.UserLegacyDN, this);
				this.SerializerLegacy.SaveIntoMessageXProperties(messageItem, false);
				return;
			}
			this.SerializerLegacy.SaveIntoMessageXProperties(messageItem, true);
		}

		internal SharingProvider GetTargetSharingProvider(ADRecipient mailboxOwner)
		{
			Util.ThrowOnNullArgument(mailboxOwner, "mailboxOwner");
			if (this.AvailableSharingProviders.Keys.Count == 1)
			{
				using (Dictionary<SharingProvider, CheckRecipientsResults>.KeyCollection.Enumerator enumerator = this.AvailableSharingProviders.Keys.GetEnumerator())
				{
					if (enumerator.MoveNext())
					{
						SharingProvider sharingProvider = enumerator.Current;
						ExTraceGlobals.SharingTracer.TraceDebug<ADRecipient, SharingProvider>((long)this.GetHashCode(), "{0}: Found target provider {1} for current user.", mailboxOwner, sharingProvider);
						return sharingProvider;
					}
				}
			}
			foreach (KeyValuePair<SharingProvider, CheckRecipientsResults> keyValuePair in this.AvailableSharingProviders)
			{
				SharingProvider key = keyValuePair.Key;
				CheckRecipientsResults value = keyValuePair.Value;
				if (value != null && mailboxOwner.IsAnyAddressMatched(ValidRecipient.ConvertToStringArray(value.ValidRecipients)))
				{
					ExTraceGlobals.SharingTracer.TraceDebug<ADRecipient, SharingProvider>((long)this.GetHashCode(), "{0}: Found target provider {1} for current user.", mailboxOwner, key);
					return key;
				}
			}
			ExTraceGlobals.SharingTracer.TraceError<ADRecipient>((long)this.GetHashCode(), "{0}: No available provider is found for this user.", mailboxOwner);
			return null;
		}

		internal void PopulateUrls(Folder folderToShare)
		{
			using (PublishedFolder publishedFolder = PublishedFolder.Create(folderToShare))
			{
				PublishedCalendar publishedCalendar = publishedFolder as PublishedCalendar;
				if (publishedCalendar != null)
				{
					if (this.PrimarySharingProvider == SharingProvider.SharingProviderPublish)
					{
						publishedCalendar.TrySetObscureKind(ObscureKind.Normal);
					}
					this.ICalUrl = publishedCalendar.ICalUrl;
				}
				this.BrowseUrl = publishedFolder.BrowseUrl;
			}
		}

		internal void SetDefaultCapabilities()
		{
			if (this.IsPrimary)
			{
				this.SharingCapabilities = (SharingCapabilities.ReadSharing | SharingCapabilities.ItemPrivacy | SharingCapabilities.ScopeSingleFolder | SharingCapabilities.AccessControl);
				return;
			}
			this.SharingCapabilities = (SharingCapabilities.ReadSharing | SharingCapabilities.WriteSharing | SharingCapabilities.ItemPrivacy | SharingCapabilities.ScopeSingleFolder | SharingCapabilities.AccessControl);
		}

		private SharingContext DeserializeFromMessage(MessageItem messageItem, bool isDraft)
		{
			ExTraceGlobals.SharingTracer.TraceDebug<string, bool>((long)this.GetHashCode(), "{0}: Try reading from metadata xml of message attchment. IsDraft = {1}", messageItem.Session.UserLegacyDN, isDraft);
			if (!this.Serializer.ReadFromMetadataXml(messageItem))
			{
				ExTraceGlobals.SharingTracer.TraceDebug<string, bool>((long)this.GetHashCode(), "{0}: No metadata xml, try reading from properties. IsDraft = {1}", messageItem.Session.UserLegacyDN, isDraft);
				this.SerializerLegacy.ReadFromMessageItem(messageItem, isDraft);
			}
			ExTraceGlobals.SharingTracer.TraceDebug<string, SharingContext>((long)this.GetHashCode(), "{0}: Deserialized from message. SharingContext: {1}", messageItem.Session.UserLegacyDN, this);
			return this;
		}

		private bool ShouldGenerateXmlMetadata
		{
			get
			{
				return this.PrimarySharingProvider != SharingProvider.SharingProviderPublish;
			}
		}

		private bool NeedToBeCompatibleWithO12InternalSharing
		{
			get
			{
				return this.IsProviderEffective(SharingProvider.SharingProviderInternal) && (this.DataType != SharingDataType.Calendar || this.SharingDetail == SharingContextDetailLevel.FullDetails || this.SharingMessageType == SharingMessageType.Request || this.SharingMessageType == SharingMessageType.DenyOfRequest);
			}
		}

		private bool NeedToBeCompatibleWithO12PubCalSharing
		{
			get
			{
				return this.IsProviderEffective(SharingProvider.SharingProviderPublish);
			}
		}

		private bool IsProviderEffective(SharingProvider provider)
		{
			if (!this.AvailableSharingProviders.ContainsKey(provider))
			{
				return false;
			}
			CheckRecipientsResults checkRecipientsResults = this.AvailableSharingProviders[provider];
			return checkRecipientsResults != null && checkRecipientsResults.ValidRecipients != null && checkRecipientsResults.ValidRecipients.Length != 0;
		}

		private const SharingCapabilities DefaultSharingCapabilities = SharingCapabilities.ReadSharing | SharingCapabilities.WriteSharing | SharingCapabilities.ItemPrivacy | SharingCapabilities.ScopeSingleFolder | SharingCapabilities.AccessControl;

		private SharingContextSerializer serializer;

		private SharingContextSerializerLegacy serializerLegacy;

		private Dictionary<SharingProvider, CheckRecipientsResults> availableSharingProviders = new Dictionary<SharingProvider, CheckRecipientsResults>(SharingProvider.AllSharingProviders.Length);
	}
}
