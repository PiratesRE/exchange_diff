using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class SharingMessageItem : MessageItem
	{
		internal SharingMessageItem(ICoreItem coreItem) : base(coreItem, false)
		{
		}

		public SharingMessageType SharingMessageType
		{
			get
			{
				this.CheckDisposed("SharingMessageType::get");
				return this.SharingContext.SharingMessageType;
			}
			set
			{
				this.CheckDisposed("SharingMessageType::set");
				this.SharingContext.SharingMessageType = value;
			}
		}

		public SharingContextPermissions SharingPermissions
		{
			get
			{
				this.CheckDisposed("SharingPermissions::get");
				return this.SharingContext.SharingPermissions;
			}
			set
			{
				this.CheckDisposed("SharingPermissions::set");
				EnumValidator.ThrowIfInvalid<SharingContextPermissions>(value, "value");
				this.SharingContext.SharingPermissions = value;
			}
		}

		public SharingContextDetailLevel SharingDetail
		{
			get
			{
				this.CheckDisposed("SharingDetail::get");
				return this.SharingContext.SharingDetail;
			}
			set
			{
				this.CheckDisposed("SharingDetail::set");
				EnumValidator.ThrowIfInvalid<SharingContextDetailLevel>(value, "value");
				this.SharingContext.SharingDetail = value;
			}
		}

		public string InitiatorName
		{
			get
			{
				this.CheckDisposed("InitiatorName::get");
				return this.SharingContext.InitiatorName;
			}
		}

		public string InitiatorSmtpAddress
		{
			get
			{
				this.CheckDisposed("InitiatorSmtpAddress::get");
				return this.SharingContext.InitiatorSmtpAddress;
			}
		}

		public string SharedFolderName
		{
			get
			{
				this.CheckDisposed("SharedFolderName::get");
				return this.SharingContext.FolderName;
			}
		}

		public DefaultFolderType SharedFolderType
		{
			get
			{
				this.CheckDisposed("SharedFolderType::get");
				return this.SharingContext.DataType.DefaultFolderType;
			}
		}

		public bool IsSharedFolderPrimary
		{
			get
			{
				this.CheckDisposed("IsSharedFolderPrimary::get");
				return this.SharingContext.IsPrimary;
			}
		}

		public string BrowseUrl
		{
			get
			{
				this.CheckDisposed("BrowseUrl::get");
				return this.SharingContext.BrowseUrl;
			}
		}

		public string ICalUrl
		{
			get
			{
				this.CheckDisposed("ICalUrl::get");
				return this.SharingContext.ICalUrl;
			}
		}

		public SharingResponseType SharingResponseType
		{
			get
			{
				this.CheckDisposed("SharingResponseType::get");
				SharingResponseType? valueAsNullable = base.GetValueAsNullable<SharingResponseType>(SharingMessageItemSchema.SharingResponseType);
				if (valueAsNullable == null)
				{
					return SharingResponseType.None;
				}
				return valueAsNullable.Value;
			}
			private set
			{
				this.CheckDisposed("SharingResponseType::set");
				this[SharingMessageItemSchema.SharingResponseType] = value;
			}
		}

		public ExDateTime? SharingResponseTime
		{
			get
			{
				this.CheckDisposed("SharingResponseTime::get");
				return base.GetValueAsNullable<ExDateTime>(SharingMessageItemSchema.SharingResponseTime);
			}
			private set
			{
				this.CheckDisposed("SharingResponseTime::set");
				this[SharingMessageItemSchema.SharingResponseTime] = value;
			}
		}

		public ExDateTime? SharingLastSubscribeTime
		{
			get
			{
				this.CheckDisposed("SharingLastSubscribeTime::get");
				return base.GetValueAsNullable<ExDateTime>(SharingMessageItemSchema.SharingLastSubscribeTime);
			}
			private set
			{
				this.CheckDisposed("SharingLastSubscribeTime::set");
				this[SharingMessageItemSchema.SharingLastSubscribeTime] = value;
			}
		}

		public bool IsPublishing
		{
			get
			{
				this.CheckDisposed("IsPublishing::get");
				return this.SharingContext.PrimarySharingProvider == SharingProvider.SharingProviderPublish || this.TryGetTargetSharingProvider() == SharingProvider.SharingProviderPublish;
			}
		}

		public override Schema Schema
		{
			get
			{
				this.CheckDisposed("Schema::get");
				return SharingMessageItemSchema.Instance;
			}
		}

		public IFrontEndLocator FrontEndLocator
		{
			get
			{
				this.CheckDisposed("FrontEndLocator::get");
				return this.frontEndLocator;
			}
			set
			{
				this.CheckDisposed("FrontEndLocator::set");
				Util.ThrowOnNullArgument(value, "FrontEndLocator");
				this.frontEndLocator = value;
			}
		}

		internal SharingProvider ForceSharingProvider
		{
			get
			{
				if (this.forceSharingProvider != null)
				{
					return this.forceSharingProvider;
				}
				if (this.IsPublishing)
				{
					return SharingProvider.SharingProviderPublish;
				}
				return null;
			}
			set
			{
				this.forceSharingProvider = value;
			}
		}

		internal bool CanUseFallback
		{
			get
			{
				return this.SharingContext.FallbackSharingProvider != null;
			}
		}

		internal bool FallbackEnabled
		{
			get
			{
				return this.fallbackEnabled;
			}
			private set
			{
				if (value && !this.CanUseFallback)
				{
					throw new InvalidOperationException("No provider to fall back on.");
				}
				this.fallbackEnabled = value;
			}
		}

		internal SharingContext RawSharingContext
		{
			get
			{
				return this.sharingContext;
			}
		}

		private MailboxSession MailboxSession
		{
			get
			{
				return base.Session as MailboxSession;
			}
		}

		private ADRecipient MailboxOwner
		{
			get
			{
				if (this.mailboxOwner == null)
				{
					this.mailboxOwner = DirectoryHelper.ReadADRecipient(this.MailboxSession.MailboxOwner.MailboxInfo.MailboxGuid, this.MailboxSession.MailboxOwner.MailboxInfo.IsArchive, this.MailboxSession.GetADRecipientSession(true, ConsistencyMode.IgnoreInvalid));
				}
				return this.mailboxOwner;
			}
		}

		private SharingContext SharingContext
		{
			get
			{
				if (this.sharingContext == null)
				{
					if (base.IsDraft)
					{
						this.sharingContext = SharingContext.DeserializeFromDraft(this);
					}
					else
					{
						this.sharingContext = SharingContext.DeserializeFromMessage(this);
					}
				}
				return this.sharingContext;
			}
			set
			{
				if (this.sharingContext != null)
				{
					throw new InvalidOperationException("Sharing context has been set already.");
				}
				this.sharingContext = value;
			}
		}

		public static SharingMessageItem Create(MailboxSession session, StoreId destFolderId, StoreId folderIdToShare)
		{
			return SharingMessageItem.InternalCreate(session, destFolderId, folderIdToShare, null);
		}

		public static SharingMessageItem CreateWithSpecficProvider(MailboxSession session, StoreId destFolderId, StoreId folderIdToShare, SharingProvider provider, bool force)
		{
			if (provider == null)
			{
				throw new ArgumentNullException("provider");
			}
			SharingMessageItem sharingMessageItem = SharingMessageItem.InternalCreate(session, destFolderId, folderIdToShare, provider);
			if (force)
			{
				sharingMessageItem.ForceSharingProvider = provider;
			}
			return sharingMessageItem;
		}

		public static SharingMessageItem CreateForPublishing(MailboxSession session, StoreId destFolderId, StoreId folderIdToShare)
		{
			return SharingMessageItem.InternalCreate(session, destFolderId, folderIdToShare, SharingProvider.SharingProviderPublish);
		}

		public new static SharingMessageItem Bind(StoreSession session, StoreId messageId)
		{
			return SharingMessageItem.Bind(session, messageId, null);
		}

		public new static SharingMessageItem Bind(StoreSession session, StoreId messageId, ICollection<PropertyDefinition> propsToReturn)
		{
			if (!(session is MailboxSession))
			{
				throw new ArgumentException("session");
			}
			return ItemBuilder.ItemBind<SharingMessageItem>(session, messageId, SharingMessageItemSchema.Instance, propsToReturn);
		}

		public SharingMessageItem AcceptRequest(StoreId destFolderId)
		{
			this.CheckDisposed("AcceptRequest");
			Util.ThrowOnNullArgument(destFolderId, "destFolderId");
			return this.CreateRequestResponseMessage(SharingResponseType.Allowed, destFolderId);
		}

		public SharingMessageItem DenyRequest(StoreId destFolderId)
		{
			this.CheckDisposed("DenyRequest");
			Util.ThrowOnNullArgument(destFolderId, "destFolderId");
			return this.CreateRequestResponseMessage(SharingResponseType.Denied, destFolderId);
		}

		public void DenyRequestWithoutResponse()
		{
			this.CheckDisposed("DenyRequestWithoutResponse");
			this.PostResponded(SharingResponseType.Denied);
		}

		public SubscribeResults SubscribeAndOpen()
		{
			this.CheckDisposed("SubscribeAndOpen");
			return this.Subscribe();
		}

		public SubscribeResults Subscribe()
		{
			this.CheckDisposed("Subscribe");
			if (base.IsDraft)
			{
				throw new InvalidOperationException("Cannot subscribe draft message.");
			}
			SharingProvider targetSharingProvider = this.GetTargetSharingProvider();
			SubscribeResults result = targetSharingProvider.PerformSubscribe(this.MailboxSession, this.SharingContext);
			this.SaveSubscribeTime();
			return result;
		}

		public void SetSubmitFlags(SharingSubmitFlags sharingSubmitFlags)
		{
			this.CheckDisposed("SetSubmitFlags");
			EnumValidator.ThrowIfInvalid<SharingSubmitFlags>(sharingSubmitFlags, "sharingSubmitFlags");
			if ((sharingSubmitFlags & SharingSubmitFlags.Auto) == SharingSubmitFlags.Auto && this.CanUseFallback)
			{
				this.FallbackEnabled = true;
				return;
			}
			this.FallbackEnabled = false;
		}

		protected override void OnBeforeSave()
		{
			if (base.IsDraft && this.sharingContext != null && !this.isSending)
			{
				this.sharingContext.SerializeToDraft(this);
			}
			base.OnBeforeSave();
		}

		protected override void OnBeforeSend()
		{
			base.CoreItem.SaveRecipients();
			CheckRecipientsResults checkRecipientsResults = this.CheckRecipients();
			if (checkRecipientsResults.InvalidRecipients != null && checkRecipientsResults.InvalidRecipients.Length > 0)
			{
				throw new InvalidSharingRecipientsException(checkRecipientsResults.InvalidRecipients, new RecipientNotSupportedByAnyProviderException());
			}
			this.PerformInvitation();
			this.SharingContext.SerializeToMessage(this);
			this.AddBodyPrefix(this.CreateBodyPrefix());
			this.isSending = true;
			if (this.SharingMessageType.IsResponseToRequest)
			{
				SharingMessageItem sharingMessageItem = this.TryGetOriginalMessage();
				if (sharingMessageItem != null)
				{
					try
					{
						SharingResponseType responseTypeFromMessageType = SharingMessageItem.GetResponseTypeFromMessageType(this.SharingMessageType);
						sharingMessageItem.PostResponded(responseTypeFromMessageType);
					}
					finally
					{
						sharingMessageItem.Dispose();
					}
				}
			}
			this[MessageItemSchema.RecipientReassignmentProhibited] = !this.IsPublishing;
			base.OnBeforeSend();
		}

		private static SharingMessageItem InternalCreate(MailboxSession mailboxSession, StoreId destFolderId, StoreId folderIdToShare, SharingProvider provider)
		{
			Util.ThrowOnNullArgument(mailboxSession, "mailboxSession");
			Util.ThrowOnNullArgument(destFolderId, "destFolderId");
			Util.ThrowOnNullArgument(folderIdToShare, "folderIdToShare");
			SharingMessageItem result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				SharingMessageItem sharingMessageItem = ItemBuilder.CreateNewItem<SharingMessageItem>(mailboxSession, destFolderId, ItemCreateInfo.SharingMessageItemInfo, CreateMessageType.Normal);
				disposeGuard.Add<SharingMessageItem>(sharingMessageItem);
				sharingMessageItem[InternalSchema.ItemClass] = "IPM.Sharing";
				using (Folder folder = Folder.Bind(mailboxSession, folderIdToShare))
				{
					sharingMessageItem.SharingContext = new SharingContext(folder, provider);
				}
				disposeGuard.Success();
				result = sharingMessageItem;
			}
			return result;
		}

		private static SharingResponseType GetResponseTypeFromMessageType(SharingMessageType messageType)
		{
			if (messageType == SharingMessageType.AcceptOfRequest)
			{
				return SharingResponseType.Allowed;
			}
			if (messageType == SharingMessageType.DenyOfRequest)
			{
				return SharingResponseType.Denied;
			}
			throw new ArgumentException("messageType");
		}

		private static SharingMessageType GetMessageTypeFromResponseType(SharingResponseType responseType)
		{
			switch (responseType)
			{
			case SharingResponseType.Allowed:
				return SharingMessageType.AcceptOfRequest;
			case SharingResponseType.Denied:
				return SharingMessageType.DenyOfRequest;
			default:
				throw new ArgumentException("responseType");
			}
		}

		private static LocalizedString GetSubjectPrefixFromResponseType(SharingResponseType responseType)
		{
			switch (responseType)
			{
			case SharingResponseType.Allowed:
				return ClientStrings.SharingRequestAllowed;
			case SharingResponseType.Denied:
				return ClientStrings.SharingRequestDenied;
			default:
				throw new ArgumentException("responseType");
			}
		}

		private SharingMessageItem CreateRequestResponseMessage(SharingResponseType responseType, StoreId destFolderId)
		{
			if (base.IsDraft)
			{
				throw new InvalidOperationException("Cannot response on draft message.");
			}
			if (!this.SharingMessageType.IsRequest)
			{
				throw new InvalidOperationException("Only can response to a request message.");
			}
			StoreId defaultFolderId = this.MailboxSession.GetDefaultFolderId(this.SharedFolderType);
			SharingMessageItem sharingMessageItem = SharingMessageItem.InternalCreate(this.MailboxSession, destFolderId, defaultFolderId, this.GetTargetSharingProvider());
			sharingMessageItem.SharingMessageType = SharingMessageItem.GetMessageTypeFromResponseType(responseType);
			sharingMessageItem.Recipients.Add(base.From);
			sharingMessageItem[SharingMessageItemSchema.SharingOriginalMessageEntryId] = HexConverter.HexStringToByteArray(base.StoreObjectId.ToHexEntryId());
			sharingMessageItem[InternalSchema.NormalizedSubject] = this[InternalSchema.NormalizedSubject];
			sharingMessageItem[InternalSchema.SubjectPrefix] = SharingMessageItem.GetSubjectPrefixFromResponseType(responseType).ToString(this.MailboxSession.InternalPreferedCulture);
			return sharingMessageItem;
		}

		private SharingProvider GetTargetSharingProvider()
		{
			SharingProvider sharingProvider = this.TryGetTargetSharingProvider();
			if (sharingProvider == null)
			{
				throw new InvalidSharingTargetRecipientException();
			}
			return sharingProvider;
		}

		private SharingProvider TryGetTargetSharingProvider()
		{
			if (this.MailboxSession.MailboxOwner.MailboxInfo.IsArchive)
			{
				return null;
			}
			return this.SharingContext.GetTargetSharingProvider(this.MailboxOwner);
		}

		private CheckRecipientsResults CheckRecipients()
		{
			List<string> list = new List<string>(base.Recipients.Count);
			List<string> list2 = new List<string>(base.Recipients.Count);
			foreach (Recipient recipient in base.Recipients)
			{
				string valueOrDefault = recipient.GetValueOrDefault<string>(ParticipantSchema.SmtpAddress, string.Empty);
				if (!string.IsNullOrEmpty(valueOrDefault))
				{
					list2.Add(valueOrDefault);
				}
				else
				{
					list.Add(recipient.Participant.EmailAddress);
				}
			}
			List<ValidRecipient> list3 = new List<ValidRecipient>(list2.Count);
			string[] array = list2.ToArray();
			List<SharingProvider> list4 = new List<SharingProvider>(this.SharingContext.AvailableSharingProviders.Keys);
			foreach (SharingProvider sharingProvider in list4)
			{
				if (array == null || array.Length == 0)
				{
					break;
				}
				CheckRecipientsResults checkRecipientsResults;
				if (this.ForceSharingProvider != null)
				{
					if (sharingProvider == this.ForceSharingProvider)
					{
						checkRecipientsResults = new CheckRecipientsResults(ValidRecipient.ConvertFromStringArray(array));
					}
					else
					{
						checkRecipientsResults = new CheckRecipientsResults(array);
					}
					this.SharingContext.AvailableSharingProviders[sharingProvider] = checkRecipientsResults;
				}
				else
				{
					checkRecipientsResults = sharingProvider.CheckRecipients(this.MailboxOwner, this.SharingContext, array);
				}
				list3.AddRange(checkRecipientsResults.ValidRecipients);
				array = checkRecipientsResults.InvalidRecipients;
			}
			list.AddRange(array);
			return new CheckRecipientsResults(list3.ToArray(), list.ToArray());
		}

		private SharingMessageItem TryGetOriginalMessage()
		{
			byte[] valueOrDefault = base.GetValueOrDefault<byte[]>(SharingMessageItemSchema.SharingOriginalMessageEntryId, null);
			if (valueOrDefault == null)
			{
				ExTraceGlobals.SharingTracer.TraceDebug<IExchangePrincipal>((long)this.GetHashCode(), "{0}: SharingOriginalMessageEntryId was not found", this.MailboxSession.MailboxOwner);
				return null;
			}
			StoreObjectId storeObjectId = null;
			try
			{
				storeObjectId = StoreObjectId.FromProviderSpecificId(valueOrDefault, StoreObjectType.Message);
			}
			catch (CorruptDataException)
			{
				ExTraceGlobals.SharingTracer.TraceDebug<IExchangePrincipal>((long)this.GetHashCode(), "{0}: SharingOriginalMessageEntryId is invalid", this.MailboxSession.MailboxOwner);
				return null;
			}
			SharingMessageItem result;
			try
			{
				result = SharingMessageItem.Bind(this.MailboxSession, storeObjectId);
			}
			catch (ObjectNotFoundException)
			{
				ExTraceGlobals.SharingTracer.TraceDebug<IExchangePrincipal, StoreObjectId>((long)this.GetHashCode(), "{0}: Original sharing request message was not found {1}", this.MailboxSession.MailboxOwner, storeObjectId);
				result = null;
			}
			return result;
		}

		private void PostResponded(SharingResponseType responseType)
		{
			if (!this.SharingMessageType.IsRequest)
			{
				throw new InvalidOperationException("Only can response to a request message.");
			}
			base.OpenAsReadWrite();
			this.SharingResponseType = responseType;
			this.SharingResponseTime = new ExDateTime?(ExDateTime.Now);
			ConflictResolutionResult conflictResolutionResult = base.Save(SaveMode.ResolveConflicts);
			if (conflictResolutionResult.SaveStatus == SaveResult.IrresolvableConflict)
			{
				ExTraceGlobals.SharingTracer.TraceError<IExchangePrincipal, VersionedId>((long)this.GetHashCode(), "{0}: Conflict occurred when saving response-status into request message {1}", this.MailboxSession.MailboxOwner, base.Id);
			}
			if (responseType == SharingResponseType.Denied)
			{
				try
				{
					SharingProvider targetSharingProvider = this.GetTargetSharingProvider();
					targetSharingProvider.PerformRevocation(this.MailboxSession, this.SharingContext);
				}
				catch (StoragePermanentException arg)
				{
					ExTraceGlobals.SharingTracer.TraceError<IExchangePrincipal, StoragePermanentException>((long)this.GetHashCode(), "{0}: Error occurred when revoking sharing from denied requester. Exception = {1}", this.MailboxSession.MailboxOwner, arg);
				}
			}
		}

		private void PerformInvitation()
		{
			Dictionary<SharingProvider, ValidRecipient[]> dictionary = new Dictionary<SharingProvider, ValidRecipient[]>();
			List<ValidRecipient> list = new List<ValidRecipient>();
			SharingProvider fallbackSharingProvider = this.SharingContext.FallbackSharingProvider;
			using (FolderPermissionContext current = FolderPermissionContext.GetCurrent(this.MailboxSession, this.SharingContext))
			{
				bool flag = false;
				try
				{
					foreach (KeyValuePair<SharingProvider, CheckRecipientsResults> keyValuePair in this.SharingContext.AvailableSharingProviders)
					{
						SharingProvider key = keyValuePair.Key;
						CheckRecipientsResults value = keyValuePair.Value;
						if (this.FallbackEnabled && key == fallbackSharingProvider)
						{
							if (value != null)
							{
								list.AddRange(value.ValidRecipients);
							}
						}
						else if (value != null)
						{
							PerformInvitationResults performInvitationResults = key.PerformInvitation(this.MailboxSession, this.SharingContext, value.ValidRecipients, this.FrontEndLocator);
							ExTraceGlobals.SharingTracer.TraceDebug<IExchangePrincipal, SharingProvider, PerformInvitationResults>((long)this.GetHashCode(), "{0}: Performed invitation by provider {1}. Result = {2}", this.MailboxSession.MailboxOwner, key, performInvitationResults);
							if (performInvitationResults.Result == PerformInvitationResultType.Failed || performInvitationResults.Result == PerformInvitationResultType.PartiallySuccess)
							{
								if (!this.FallbackEnabled)
								{
									StoreObjectId folderId = this.SharingContext.FolderId;
									InvalidSharingRecipientsResolution invalidSharingRecipientsResolution;
									if (!this.CanUseFallback)
									{
										invalidSharingRecipientsResolution = new InvalidSharingRecipientsResolution(folderId);
									}
									else
									{
										using (Folder folder = Folder.Bind(this.MailboxSession, folderId))
										{
											this.SharingContext.PopulateUrls(folder);
										}
										invalidSharingRecipientsResolution = new InvalidSharingRecipientsResolution(this.BrowseUrl, this.ICalUrl);
									}
									ExTraceGlobals.SharingTracer.TraceError<IExchangePrincipal, InvalidSharingRecipientsResolution>((long)this.GetHashCode(), "{0}: No fall back for these invalid recipients. Resolution = {1}", this.MailboxSession.MailboxOwner, invalidSharingRecipientsResolution);
									throw new InvalidSharingRecipientsException(performInvitationResults.FailedRecipients, invalidSharingRecipientsResolution);
								}
								ValidRecipient[] array = Array.ConvertAll<InvalidRecipient, ValidRecipient>(performInvitationResults.FailedRecipients, (InvalidRecipient invalidRecipient) => new ValidRecipient(invalidRecipient.SmtpAddress, null));
								ExTraceGlobals.SharingTracer.TraceDebug<IExchangePrincipal, SharingProvider, int>((long)this.GetHashCode(), "{0}: Fall back on provider {1} for these {2} failed recipients.", this.MailboxSession.MailboxOwner, fallbackSharingProvider, array.Length);
								list.AddRange(array);
								dictionary.Add(key, performInvitationResults.SucceededRecipients);
							}
						}
					}
					if (this.FallbackEnabled)
					{
						foreach (KeyValuePair<SharingProvider, ValidRecipient[]> keyValuePair2 in dictionary)
						{
							SharingProvider key2 = keyValuePair2.Key;
							ValidRecipient[] value2 = keyValuePair2.Value;
							this.SharingContext.AvailableSharingProviders[key2] = new CheckRecipientsResults(value2);
						}
						this.SharingContext.AvailableSharingProviders[fallbackSharingProvider] = new CheckRecipientsResults(list.ToArray());
						PerformInvitationResults performInvitationResults2 = fallbackSharingProvider.PerformInvitation(this.MailboxSession, this.SharingContext, list.ToArray(), this.FrontEndLocator);
						ExTraceGlobals.SharingTracer.TraceDebug<IExchangePrincipal, SharingProvider, PerformInvitationResults>((long)this.GetHashCode(), "{0}: Performed invitation by fallback provider {1}. Result = {2}", this.MailboxSession.MailboxOwner, fallbackSharingProvider, performInvitationResults2);
						if (performInvitationResults2.Result == PerformInvitationResultType.Failed || performInvitationResults2.Result == PerformInvitationResultType.PartiallySuccess)
						{
							throw new InvalidOperationException("The fallback provider should never fail.");
						}
					}
					flag = true;
				}
				finally
				{
					if (!flag)
					{
						current.Disable();
					}
				}
			}
		}

		private void SaveSubscribeTime()
		{
			base.OpenAsReadWrite();
			this.SharingLastSubscribeTime = new ExDateTime?(ExDateTime.Now);
			ConflictResolutionResult conflictResolutionResult = base.Save(SaveMode.ResolveConflicts);
			if (conflictResolutionResult.SaveStatus == SaveResult.IrresolvableConflict)
			{
				ExTraceGlobals.SharingTracer.TraceError<IExchangePrincipal>((long)this.GetHashCode(), "{0}: Conflict occurred when saving last-subscribe-time.", this.MailboxSession.MailboxOwner);
			}
		}

		private void AddBodyPrefix(string prefix)
		{
			byte[] array = null;
			BodyReadConfiguration configuration = new BodyReadConfiguration(base.Body.RawFormat, base.Body.RawCharset.Name);
			using (Stream stream = base.Body.OpenReadStream(configuration))
			{
				array = Util.StreamHandler.ReadBytesFromStream(stream);
			}
			BodyWriteConfiguration bodyWriteConfiguration = new BodyWriteConfiguration(base.Body.RawFormat, base.Body.RawCharset.Name);
			bodyWriteConfiguration.SetTargetFormat(base.Body.Format, base.Body.Charset);
			bodyWriteConfiguration.AddInjectedText(prefix, null, BodyInjectionFormat.Text);
			using (Stream stream2 = base.Body.OpenWriteStream(bodyWriteConfiguration))
			{
				stream2.Write(array, 0, array.Length);
			}
		}

		private string CreateBodyPrefix()
		{
			CultureInfo internalPreferedCulture = this.MailboxSession.InternalPreferedCulture;
			string displayName = this.MailboxSession.MailboxOwner.MailboxInfo.DisplayName;
			string email = this.MailboxSession.MailboxOwner.MailboxInfo.PrimarySmtpAddress.ToString();
			LocalizedString displayName2 = this.SharingContext.DataType.DisplayName;
			StringBuilder stringBuilder = new StringBuilder();
			if (!this.IsPublishing)
			{
				if (this.SharingContext.SharingMessageType == SharingMessageType.Invitation)
				{
					if (this.SharingContext.IsPrimary)
					{
						stringBuilder.AppendLine(ClientStrings.SharingInvitation(displayName, email, displayName2).ToString(internalPreferedCulture));
					}
					else
					{
						stringBuilder.AppendLine(ClientStrings.SharingInvitationNonPrimary(displayName, email, this.SharedFolderName, displayName2).ToString(internalPreferedCulture));
					}
					stringBuilder.AppendLine();
					stringBuilder.AppendLine(ClientStrings.SharingInvitationInstruction.ToString(internalPreferedCulture));
				}
				else if (this.SharingContext.SharingMessageType == SharingMessageType.Request)
				{
					stringBuilder.AppendLine(ClientStrings.SharingRequest(displayName, email, displayName2).ToString(internalPreferedCulture));
					stringBuilder.AppendLine();
					stringBuilder.AppendLine(ClientStrings.SharingRequestInstruction.ToString(internalPreferedCulture));
				}
				else if (this.SharingContext.SharingMessageType == SharingMessageType.InvitationAndRequest)
				{
					stringBuilder.AppendLine(ClientStrings.SharingInvitationAndRequest(displayName, email, displayName2).ToString(internalPreferedCulture));
					stringBuilder.AppendLine();
					stringBuilder.AppendLine(ClientStrings.SharingInvitationAndRequestInstruction.ToString(internalPreferedCulture));
				}
				else if (this.SharingContext.SharingMessageType == SharingMessageType.AcceptOfRequest)
				{
					stringBuilder.AppendLine(ClientStrings.SharingAccept(displayName, email, displayName2).ToString(internalPreferedCulture));
				}
				else if (this.SharingContext.SharingMessageType == SharingMessageType.DenyOfRequest)
				{
					stringBuilder.AppendLine(ClientStrings.SharingDecline(displayName, email, displayName2).ToString(internalPreferedCulture));
				}
			}
			if (this.SharingContext.SharingMessageType.IsInvitationOrAcceptOfRequest)
			{
				if (this.SharingContext.BrowseUrl != null)
				{
					stringBuilder.AppendLine();
					stringBuilder.AppendLine(ClientStrings.SharingAnonymous(displayName, displayName2, this.SharedFolderName, this.SharingContext.BrowseUrl.ToString()).ToString(internalPreferedCulture));
				}
				if (this.SharingContext.ICalUrl != null)
				{
					UriBuilder uriBuilder = new UriBuilder(this.SharingContext.ICalUrl);
					uriBuilder.Scheme = "webcal";
					uriBuilder.Port = -1;
					stringBuilder.AppendLine();
					stringBuilder.AppendLine(ClientStrings.SharingICal(uriBuilder.Uri.ToString()).ToString(internalPreferedCulture));
				}
			}
			stringBuilder.AppendLine();
			stringBuilder.AppendLine("*~*~*~*~*~*~*~*~*~*");
			stringBuilder.AppendLine();
			return stringBuilder.ToString();
		}

		private bool isSending;

		private SharingContext sharingContext;

		private ADRecipient mailboxOwner;

		private SharingProvider forceSharingProvider;

		private bool fallbackEnabled;

		private IFrontEndLocator frontEndLocator;
	}
}
