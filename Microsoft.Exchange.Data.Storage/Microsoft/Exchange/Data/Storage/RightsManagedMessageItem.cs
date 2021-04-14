using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Microsoft.Exchange.Core;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage.RightsManagement;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Security.RightsManagement;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class RightsManagedMessageItem : MessageItem
	{
		internal RightsManagedMessageItem(ICoreItem coreItem) : base(coreItem, false)
		{
		}

		public RmsTemplate Restriction
		{
			get
			{
				this.CheckDisposed("Restriction::get");
				this.EnsureIsDecoded();
				return this.rmsTemplate;
			}
		}

		public ContentRight UsageRights
		{
			get
			{
				this.CheckDisposed("UsageRights::get");
				this.EnsureIsDecoded();
				return this.UsageRightsInternal;
			}
		}

		public Participant ConversationOwner
		{
			get
			{
				this.CheckDisposed("ConversationOwner::get");
				this.EnsureIsDecoded();
				if (this.conversationOwner == null)
				{
					return base.Sender;
				}
				return this.conversationOwner;
			}
		}

		public bool IsDecoded
		{
			get
			{
				this.CheckDisposed("IsDecoded::get");
				return this.decodedItem != null;
			}
		}

		public bool CanDecode
		{
			get
			{
				this.CheckDisposed("CanDecode::get");
				MailboxSession internalSession = this.InternalSession;
				return internalSession != null && (internalSession.MailboxOwner.RecipientTypeDetails == RecipientTypeDetails.GroupMailbox || internalSession.LogonType != LogonType.Delegated);
			}
		}

		public bool CanRepublish
		{
			get
			{
				this.CheckDisposed("CanRepublish::get");
				return !this.publishedByExternalRMS;
			}
		}

		public Body ProtectedBody
		{
			get
			{
				this.CheckDisposed("ProtectedBody::get");
				this.EnsureIsDecoded();
				this.CheckPermission(ContentRight.View);
				return this.decodedItem.Body;
			}
		}

		public AttachmentCollection ProtectedAttachmentCollection
		{
			get
			{
				this.CheckDisposed("ProtectedAttachmentCollection::get");
				this.EnsureIsDecoded();
				this.CheckPermission(ContentRight.View);
				return this.decodedItem.AttachmentCollection;
			}
		}

		public RightsManagedMessageDecryptionStatus DecryptionStatus
		{
			get
			{
				this.CheckDisposed("DecryptionStatus::get");
				return this.decryptionStatus;
			}
		}

		public override Schema Schema
		{
			get
			{
				this.CheckDisposed("Schema::get");
				return RightsManagedMessageItemSchema.Instance;
			}
		}

		private ContentRight UsageRightsInternal
		{
			get
			{
				if (this.restrictionInfo == null)
				{
					return ContentRight.Owner;
				}
				return this.restrictionInfo.UsageRights;
			}
		}

		private CultureInfo Culture
		{
			get
			{
				if (this.InternalSession == null)
				{
					return CultureInfo.CurrentCulture;
				}
				return this.InternalSession.InternalCulture;
			}
		}

		private MailboxSession InternalSession
		{
			get
			{
				return (base.Session ?? ((base.CoreItem.TopLevelItem != null) ? base.CoreItem.TopLevelItem.Session : null)) as MailboxSession;
			}
		}

		public static RightsManagedMessageItem Create(MailboxSession session, StoreId destFolderId, OutboundConversionOptions options)
		{
			Util.ThrowOnNullArgument(session, "session");
			Util.ThrowOnNullArgument(destFolderId, "destFolderId");
			Util.ThrowOnNullArgument(options, "options");
			RightsManagedMessageItem.CheckSession(session);
			RightsManagedMessageItem rightsManagedMessageItem = null;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				rightsManagedMessageItem = ItemBuilder.CreateNewItem<RightsManagedMessageItem>(session, destFolderId, ItemCreateInfo.RightsManagedMessageItemInfo, CreateMessageType.Normal);
				disposeGuard.Add<RightsManagedMessageItem>(rightsManagedMessageItem);
				rightsManagedMessageItem[InternalSchema.ItemClass] = "IPM.Note";
				rightsManagedMessageItem.InitNewItem(options);
				rightsManagedMessageItem.SetDefaultEnvelopeBody(null);
				disposeGuard.Success();
			}
			return rightsManagedMessageItem;
		}

		public static RightsManagedMessageItem CreateInMemory(OutboundConversionOptions options)
		{
			Util.ThrowOnNullArgument(options, "options");
			RightsManagedMessageItem rightsManagedMessageItem = null;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				rightsManagedMessageItem = ItemBuilder.ConstructItem<RightsManagedMessageItem>(null, null, null, StoreObjectSchema.ContentConversionProperties, () => new InMemoryPersistablePropertyBag(StoreObjectSchema.ContentConversionProperties), ItemCreateInfo.RightsManagedMessageItemInfo.Creator, Origin.Existing, ItemLevel.TopLevel);
				disposeGuard.Add<RightsManagedMessageItem>(rightsManagedMessageItem);
				rightsManagedMessageItem[InternalSchema.ItemClass] = "IPM.Note";
				rightsManagedMessageItem.InitNewItem(options);
				rightsManagedMessageItem.SetDefaultEnvelopeBody(null);
				disposeGuard.Success();
			}
			return rightsManagedMessageItem;
		}

		public static RightsManagedMessageItem CreateFromInMemory(MessageItem item, MailboxSession session, StoreId destFolderId, OutboundConversionOptions options)
		{
			Util.ThrowOnNullArgument(item, "item");
			Util.ThrowOnNullArgument(session, "session");
			Util.ThrowOnNullArgument(destFolderId, "destFolderId");
			Util.ThrowOnNullArgument(options, "options");
			RightsManagedMessageItem.CheckSession(session);
			if (item.Session != null)
			{
				throw new InvalidOperationException("Item should be in-memory, not backed by store.");
			}
			RightsManagedMessageItem result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				RightsManagedMessageItem rightsManagedMessageItem = RightsManagedMessageItem.Create(session, destFolderId, options);
				disposeGuard.Add<RightsManagedMessageItem>(rightsManagedMessageItem);
				RightsManagedMessageItem.CopyProtectableData(item, rightsManagedMessageItem.decodedItem);
				foreach (NativeStorePropertyDefinition nativeStorePropertyDefinition in item.AllNativeProperties)
				{
					if (!Body.BodyPropSet.Contains(nativeStorePropertyDefinition) && nativeStorePropertyDefinition != StoreObjectSchema.ContentClass)
					{
						object obj = item.TryGetProperty(nativeStorePropertyDefinition);
						if (!(obj is PropertyError))
						{
							rightsManagedMessageItem[nativeStorePropertyDefinition] = obj;
						}
						else if (PropertyError.IsPropertyValueTooBig(obj))
						{
							using (Stream stream = item.OpenPropertyStream(nativeStorePropertyDefinition, PropertyOpenMode.ReadOnly))
							{
								using (Stream stream2 = rightsManagedMessageItem.OpenPropertyStream(nativeStorePropertyDefinition, PropertyOpenMode.Create))
								{
									Util.StreamHandler.CopyStreamData(stream, stream2);
								}
							}
						}
					}
				}
				rightsManagedMessageItem.Recipients.CopyRecipientsFrom(item.Recipients);
				rightsManagedMessageItem.SaveFlags |= (PropertyBagSaveFlags.IgnoreMapiComputedErrors | PropertyBagSaveFlags.IgnoreUnresolvedHeaders);
				disposeGuard.Success();
				result = rightsManagedMessageItem;
			}
			return result;
		}

		public static RightsManagedMessageItem Create(MessageItem item, OutboundConversionOptions options)
		{
			Util.ThrowOnNullArgument(item, "item");
			Util.ThrowOnNullArgument(options, "options");
			RightsManagedMessageItem.CheckSession(item.Session);
			RightsManagedMessageItem result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				RightsManagedMessageItem rightsManagedMessageItem = new RightsManagedMessageItem(new CoreItemWrapper(item.CoreItem));
				disposeGuard.Add<RightsManagedMessageItem>(rightsManagedMessageItem);
				rightsManagedMessageItem.InitNewItem(options);
				RightsManagedMessageItem.CopyProtectableData(item, rightsManagedMessageItem.decodedItem);
				rightsManagedMessageItem.SetDefaultEnvelopeBody(null);
				disposeGuard.Success();
				rightsManagedMessageItem.originalItem = item;
				result = rightsManagedMessageItem;
			}
			return result;
		}

		public static RightsManagedMessageItem ReBind(MessageItem item, OutboundConversionOptions options, bool acquireLicense)
		{
			Util.ThrowOnNullArgument(item, "item");
			Util.ThrowOnNullArgument(options, "options");
			StoreSession storeSession = item.Session ?? ((item.CoreItem.TopLevelItem != null) ? item.CoreItem.TopLevelItem.Session : null);
			if (storeSession == null)
			{
				throw new ArgumentException("Cannot use ReBind() for in-memory message.", "item");
			}
			RightsManagedMessageItem.CheckSession(storeSession);
			if (!item.IsRestricted)
			{
				throw new ArgumentException("Only protected messages can be used for ReBind()");
			}
			RightsManagedMessageItem result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				RightsManagedMessageItem rightsManagedMessageItem = new RightsManagedMessageItem(new CoreItemWrapper(item.CoreItem));
				disposeGuard.Add<RightsManagedMessageItem>(rightsManagedMessageItem);
				rightsManagedMessageItem.Decode(options, acquireLicense);
				disposeGuard.Success();
				rightsManagedMessageItem.originalItem = item;
				result = rightsManagedMessageItem;
			}
			return result;
		}

		public static RightsManagedMessageItem Bind(MailboxSession session, StoreId messageId, OutboundConversionOptions options)
		{
			return RightsManagedMessageItem.Bind(session, messageId, options, true, new PropertyDefinition[0]);
		}

		public static RightsManagedMessageItem Bind(MailboxSession session, StoreId messageId, OutboundConversionOptions options, bool acquireLicense)
		{
			return RightsManagedMessageItem.Bind(session, messageId, options, acquireLicense, new PropertyDefinition[0]);
		}

		public static RightsManagedMessageItem Bind(MailboxSession session, StoreId messageId, OutboundConversionOptions options, ICollection<PropertyDefinition> propsToReturn)
		{
			return RightsManagedMessageItem.Bind(session, messageId, options, true, propsToReturn);
		}

		public static RightsManagedMessageItem Bind(MailboxSession session, StoreId messageId, OutboundConversionOptions options, bool acquireLicense, ICollection<PropertyDefinition> propsToReturn)
		{
			Util.ThrowOnNullArgument(session, "session");
			Util.ThrowOnNullArgument(messageId, "messageId");
			Util.ThrowOnNullArgument(options, "options");
			RightsManagedMessageItem.CheckSession(session);
			RightsManagedMessageItem rightsManagedMessageItem = null;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				rightsManagedMessageItem = ItemBuilder.ItemBind<RightsManagedMessageItem>(session, messageId, RightsManagedMessageItemSchema.Instance, propsToReturn);
				disposeGuard.Add<RightsManagedMessageItem>(rightsManagedMessageItem);
				rightsManagedMessageItem.Decode(options, acquireLicense);
				disposeGuard.Success();
			}
			return rightsManagedMessageItem;
		}

		public override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<RightsManagedMessageItem>(this);
		}

		public RightsManagedMessageDecryptionStatus TryDecode(OutboundConversionOptions options, bool acquireLicense)
		{
			try
			{
				this.Decode(options, acquireLicense);
			}
			catch (RightsManagementPermanentException)
			{
			}
			catch (RightsManagementTransientException)
			{
			}
			return this.decryptionStatus;
		}

		public bool Decode(OutboundConversionOptions options, bool acquireLicense)
		{
			this.CheckDisposed("Decode");
			Util.ThrowOnNullArgument(options, "options");
			this.decryptionStatus = RightsManagedMessageDecryptionStatus.Success;
			if (this.decodedItem != null)
			{
				return true;
			}
			if (this.InternalSession == null)
			{
				this.decryptionStatus = RightsManagedMessageDecryptionStatus.NotSupported;
				throw new InvalidOperationException("Decoding of in-memory messages is not supported.");
			}
			RightsManagedMessageItem.CheckSession(this.InternalSession);
			this.SetConversionOptions(options);
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				try
				{
					MessageItem messageItem = ItemConversion.OpenRestrictedContent(this, this.orgId, acquireLicense, out this.licenseAcquired, out this.useLicenseValue, out this.restrictionInfo);
					if (messageItem == null)
					{
						ExTraceGlobals.StorageTracer.TraceError(0L, "Failed to decode protected message - no user license is present.");
						throw new RightsManagementPermanentException(RightsManagementFailureCode.UnknownFailure, ServerStrings.GenericFailureRMDecryption);
					}
					disposeGuard.Add<MessageItem>(messageItem);
					this.UpdateEffectiveRights();
					this.conversationOwner = new Participant(this.restrictionInfo.ConversationOwner, this.restrictionInfo.ConversationOwner, "SMTP");
					this.CheckPermission(ContentRight.View);
					messageItem.CoreItem.TopLevelItem = (base.CoreItem.TopLevelItem ?? base.CoreItem);
					this.serverUseLicense = (messageItem.TryGetProperty(MessageItemSchema.DRMServerLicense) as string);
					this.publishLicense = (messageItem.TryGetProperty(MessageItemSchema.DrmPublishLicense) as string);
					this.rmsTemplate = RmsTemplate.CreateFromPublishLicense(this.publishLicense);
					MsgToRpMsgConverter.CallRM(delegate
					{
						this.publishedByExternalRMS = !RmsClientManager.IsPublishedByOrganizationRMS(this.orgId, this.publishLicense);
					}, ServerStrings.FailedToCheckPublishLicenseOwnership(this.orgId.ToString()));
					this.decodedItem = messageItem;
					disposeGuard.Success();
				}
				catch (RightsManagementPermanentException exception)
				{
					this.decryptionStatus = RightsManagedMessageDecryptionStatus.CreateFromException(exception);
					throw;
				}
				catch (RightsManagementTransientException exception2)
				{
					this.decryptionStatus = RightsManagedMessageDecryptionStatus.CreateFromException(exception2);
					throw;
				}
			}
			return true;
		}

		public void UnprotectAttachment(AttachmentId attachmentId)
		{
			using (Attachment attachment = this.ProtectedAttachmentCollection.Open(attachmentId))
			{
				StreamAttachment streamAttachment = attachment as StreamAttachment;
				if (streamAttachment == null)
				{
					throw new ObjectNotFoundException(ServerStrings.MapiCannotOpenAttachment);
				}
				using (Stream stream = StreamAttachment.OpenRestrictedContent(streamAttachment, this.orgId))
				{
					using (Stream contentStream = streamAttachment.GetContentStream(PropertyOpenMode.Create))
					{
						Util.StreamHandler.CopyStreamData(stream, contentStream);
					}
				}
				attachment.Save();
			}
		}

		public void SetRestriction(RmsTemplate template)
		{
			this.CheckDisposed("Restriction::set");
			this.EnsureIsDecoded();
			this.CheckPermission(ContentRight.Export);
			this.rmsTemplate = template;
			if (this.rmsTemplate != null)
			{
				this.restrictionInfo = new RestrictionInfo(ContentRight.Owner, ExDateTime.MaxValue, string.Empty);
			}
			else
			{
				this.restrictionInfo = null;
				switch (base.IconIndex)
				{
				case IconIndex.MailIrm:
				case IconIndex.MailIrmForwarded:
				case IconIndex.MailIrmReplied:
					base.IconIndex = IconIndex.Default;
					break;
				}
			}
			this.conversationOwner = null;
			this.serverUseLicense = null;
			this.publishLicense = null;
			this.UpdateEffectiveRights();
		}

		public void SetDefaultEnvelopeBody(LocalizedString? bodyString)
		{
			this.CheckDisposed("SetDefaultEnvelopeBody");
			this.EnsureIsDecoded();
			using (TextWriter textWriter = base.Body.OpenTextWriter(BodyFormat.TextPlain))
			{
				if (bodyString != null)
				{
					textWriter.Write(bodyString.Value.ToString(this.Culture));
				}
				else
				{
					string value = string.Format("{0} {1}", SystemMessages.BodyReceiveRMEmail.ToString(this.Culture), SystemMessages.BodyDownload.ToString(this.Culture));
					textWriter.Write(value);
				}
			}
		}

		public MessageItem DecodedItem
		{
			get
			{
				this.CheckDisposed("DecodedItem::get");
				this.EnsureIsDecoded();
				this.CheckPermission(ContentRight.View);
				return this.decodedItem;
			}
		}

		public ExDateTime UserLicenseExpiryTime
		{
			get
			{
				this.CheckDisposed("UseLicenseExpiry::get");
				this.EnsureIsDecoded();
				return this.restrictionInfo.ExpiryTime;
			}
		}

		public void SetProtectedData(Body body, AttachmentCollection attachments)
		{
			this.CheckDisposed("SetProtectedData");
			this.EnsureIsDecoded();
			RightsManagedMessageItem.CopyProtectableData(body, attachments, this.decodedItem);
		}

		public void AbandonChangesOnProtectedData()
		{
			this.CheckDisposed("AbandonChangesOnProtectedData");
			if (this.decodedItem != null)
			{
				this.decodedItem.Dispose();
				this.decodedItem = null;
				this.effectiveRights = ContentRight.Owner;
				this.publishLicense = null;
				this.restrictionInfo = null;
				this.rmsTemplate = null;
				this.serverUseLicense = null;
				this.conversationOwner = null;
			}
		}

		public void SaveUseLicense()
		{
			this.CheckDisposed("SaveUSeLicense");
			if (this.licenseAcquired)
			{
				base.OpenAsReadWrite();
				this.PrepareAcquiredLicensesBeforeSave();
				base.Save(SaveMode.ResolveConflicts);
			}
		}

		public void PrepareAcquiredLicensesBeforeSave()
		{
			this.CheckDisposed("PrepareAcquiredLicensesBeforeSave");
			if (this.licenseAcquired)
			{
				this[MessageItemSchema.DRMRights] = this.useLicenseValue.UsageRights;
				this[MessageItemSchema.DRMExpiryTime] = this.useLicenseValue.ExpiryTime;
				if (!DrmClientUtils.IsCachingOfLicenseDisabled(this.useLicenseValue.UseLicense))
				{
					using (Stream stream = base.OpenPropertyStream(MessageItemSchema.DRMServerLicenseCompressed, PropertyOpenMode.Create))
					{
						DrmEmailCompression.CompressUseLicense(this.useLicenseValue.UseLicense, stream);
					}
				}
				this[MessageItemSchema.DRMPropsSignature] = this.useLicenseValue.DRMPropsSignature;
			}
		}

		public override MessageItem CreateForward(MailboxSession session, StoreId parentFolderId, ReplyForwardConfiguration configuration)
		{
			this.CheckDisposed("CreateForward");
			ExTraceGlobals.StorageTracer.Information((long)this.GetHashCode(), "RightsManagedMessageItem::CreateForward.");
			if (this.decodedItem == null)
			{
				return base.CreateForward(session, parentFolderId, configuration);
			}
			this.CheckPermission(ContentRight.Forward);
			return this.CreateReplyForwardInternal(session, parentFolderId, configuration, delegate(RightsManagedMessageItem original, RightsManagedMessageItem result, ReplyForwardConfiguration configurationPassed)
			{
				RightsManagedForwardCreation rightsManagedForwardCreation = new RightsManagedForwardCreation(original, result, configurationPassed);
				rightsManagedForwardCreation.PopulateProperties();
			});
		}

		public override MessageItem CreateReply(MailboxSession session, StoreId parentFolderId, ReplyForwardConfiguration configuration)
		{
			this.CheckDisposed("CreateReply");
			ExTraceGlobals.StorageTracer.Information((long)this.GetHashCode(), "RightsManagedMessageItem::CreateReply.");
			if (this.decodedItem == null)
			{
				return base.CreateReply(session, parentFolderId, configuration);
			}
			this.CheckPermission(ContentRight.Reply);
			return this.CreateReplyForwardInternal(session, parentFolderId, configuration, delegate(RightsManagedMessageItem original, RightsManagedMessageItem result, ReplyForwardConfiguration configurationPassed)
			{
				RightsManagedReplyCreation rightsManagedReplyCreation = new RightsManagedReplyCreation(original, result, configurationPassed, false);
				rightsManagedReplyCreation.PopulateProperties();
			});
		}

		public override MessageItem CreateReplyAll(MailboxSession session, StoreId parentFolderId, ReplyForwardConfiguration configuration)
		{
			this.CheckDisposed("CreateReplyAll");
			Util.ThrowOnNullArgument(session, "session");
			Util.ThrowOnNullArgument(parentFolderId, "parentFolderId");
			Util.ThrowOnNullArgument(configuration, "configuration");
			ExTraceGlobals.StorageTracer.Information((long)this.GetHashCode(), "RightsManagedMessageItem::CreateReplyAll.");
			if (this.decodedItem == null)
			{
				return base.CreateReplyAll(session, parentFolderId, configuration);
			}
			this.CheckPermission(ContentRight.ReplyAll);
			return this.CreateReplyForwardInternal(session, parentFolderId, configuration, delegate(RightsManagedMessageItem original, RightsManagedMessageItem result, ReplyForwardConfiguration configurationPassed)
			{
				RightsManagedReplyCreation rightsManagedReplyCreation = new RightsManagedReplyCreation(original, result, configurationPassed, true);
				rightsManagedReplyCreation.PopulateProperties();
			});
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				Util.DisposeIfPresent(this.decodedItem);
				Util.DisposeIfPresent(this.originalItem);
			}
			base.InternalDispose(disposing);
		}

		protected override void InternalGetContextCharsetDetectionData(StringBuilder stringBuilder, CharsetDetectionDataFlags flags)
		{
			base.InternalGetContextCharsetDetectionData(stringBuilder, flags);
			if (this.charsetDetectionStringForProtectedData != null)
			{
				stringBuilder.Append(this.charsetDetectionStringForProtectedData.ToString());
				return;
			}
			if (this.decodedItem == null && ((flags & CharsetDetectionDataFlags.Complete) != CharsetDetectionDataFlags.Complete || (flags & CharsetDetectionDataFlags.NoMessageDecoding) == CharsetDetectionDataFlags.NoMessageDecoding || base.AttachmentCollection.IsDirty))
			{
				return;
			}
			this.GetCharsetDetectionStringFromProtectedData(stringBuilder);
		}

		protected override void OnAfterSave(ConflictResolutionResult acrResults)
		{
			this.charsetDetectionStringForProtectedData = null;
			base.OnAfterSave(acrResults);
		}

		protected override void OnBeforeSave()
		{
			if (this.decodedItem == null && !base.AttachmentCollection.IsDirty && base.IsRestricted && (base.Recipients.IsDirty || base.IsPropertyDirty(ItemSchema.Sender)))
			{
				this.EnsureIsDecoded();
			}
			if (this.decodedItem != null)
			{
				string contentClass = base.TryGetProperty(InternalSchema.ContentClass) as string;
				if (this.rmsTemplate == null)
				{
					this.UnprotectAllAttachments();
					RightsManagedMessageItem.CopyProtectableData(this.decodedItem, this);
					if (ObjectClass.IsRightsManagedContentClass(contentClass))
					{
						base.Delete(StoreObjectSchema.ContentClass);
					}
				}
				else
				{
					this.charsetDetectionStringForProtectedData = new StringBuilder((int)Math.Min(this.ProtectedBody.Size, 2147483647L));
					this.GetCharsetDetectionStringFromProtectedData(this.charsetDetectionStringForProtectedData);
					if (!ObjectClass.IsRightsManagedContentClass(contentClass))
					{
						this[StoreObjectSchema.ContentClass] = "rpmsg.message";
					}
					if (this.isSending)
					{
						byte[][] valueOrDefault = base.GetValueOrDefault<byte[][]>(InternalSchema.DRMLicense);
						if (valueOrDefault != null && valueOrDefault.Length == RightsManagedMessageItem.EmptyDrmLicense.Length && valueOrDefault[0].Length == RightsManagedMessageItem.EmptyDrmLicense[0].Length)
						{
							base.DeleteProperties(new PropertyDefinition[]
							{
								InternalSchema.DRMLicense
							});
						}
					}
					else if (base.IsDraft && base.GetValueOrDefault<byte[][]>(InternalSchema.DRMLicense) == null)
					{
						this[InternalSchema.DRMLicense] = RightsManagedMessageItem.EmptyDrmLicense;
					}
					base.AttachmentCollection.RemoveAll();
					using (StreamAttachment streamAttachment = base.AttachmentCollection.Create(AttachmentType.Stream) as StreamAttachment)
					{
						streamAttachment.FileName = "message.rpmsg";
						streamAttachment.ContentType = "application/x-microsoft-rpmsg-message";
						using (Stream stream = new PooledMemoryStream(131072))
						{
							if (this.serverUseLicense == null || ((this.UsageRights & ContentRight.Owner) == ContentRight.Owner && this.rmsTemplate.RequiresRepublishingWhenRecipientsChange && this.CanRepublish && (base.Recipients.IsDirty || (base.IsPropertyDirty(ItemSchema.Sender) && this.conversationOwner == null))))
							{
								if (this.ConversationOwner == null)
								{
									throw new InvalidOperationException("Conversation owner must be set before protecting the message.");
								}
								this.UnprotectAllAttachments();
								using (MsgToRpMsgConverter msgToRpMsgConverter = new MsgToRpMsgConverter(this, this.ConversationOwner, this.orgId, this.rmsTemplate, this.options))
								{
									msgToRpMsgConverter.Convert(this.decodedItem, stream);
									using (Stream stream2 = base.OpenPropertyStream(MessageItemSchema.DRMServerLicenseCompressed, PropertyOpenMode.Create))
									{
										DrmEmailCompression.CompressUseLicense(msgToRpMsgConverter.ServerUseLicense, stream2);
									}
									if (this.InternalSession != null && this.InternalSession.MailboxOwner.Sid != null)
									{
										ExDateTime useLicenseExpiryTime = RmsClientManagerUtils.GetUseLicenseExpiryTime(msgToRpMsgConverter.ServerUseLicense, this.UsageRights);
										this[MessageItemSchema.DRMRights] = (int)this.UsageRights;
										this[MessageItemSchema.DRMExpiryTime] = useLicenseExpiryTime;
										using (RightsSignatureBuilder rightsSignatureBuilder = new RightsSignatureBuilder(msgToRpMsgConverter.ServerUseLicense, msgToRpMsgConverter.PublishLicense, RmsClientManager.EnvironmentHandle, msgToRpMsgConverter.LicensePair))
										{
											this[MessageItemSchema.DRMPropsSignature] = rightsSignatureBuilder.Sign(this.UsageRights, useLicenseExpiryTime, this.InternalSession.MailboxOwner.Sid);
										}
									}
									goto IL_362;
								}
							}
							using (MsgToRpMsgConverter msgToRpMsgConverter2 = new MsgToRpMsgConverter(this, this.orgId, this.publishLicense, this.serverUseLicense, this.options))
							{
								msgToRpMsgConverter2.Convert(this.decodedItem, stream);
							}
							IL_362:
							using (Stream contentStream = streamAttachment.GetContentStream(PropertyOpenMode.Create))
							{
								stream.Seek(0L, SeekOrigin.Begin);
								Util.StreamHandler.CopyStreamData(stream, contentStream);
							}
						}
						bool flag = false;
						foreach (AttachmentHandle handle in this.decodedItem.AttachmentCollection)
						{
							if (!CoreAttachmentCollection.IsInlineAttachment(handle))
							{
								flag = true;
								break;
							}
						}
						this[InternalSchema.AllAttachmentsHidden] = !flag;
						streamAttachment.Save();
					}
				}
				this.decodedItem.Dispose();
				this.decodedItem = null;
				this.effectiveRights = ContentRight.Owner;
				this.publishLicense = null;
				this.restrictionInfo = null;
				this.rmsTemplate = null;
				this.serverUseLicense = null;
				this.conversationOwner = null;
			}
			base.OnBeforeSave();
		}

		protected override void OnBeforeSend()
		{
			try
			{
				this.isSending = true;
				base.OnBeforeSend();
			}
			finally
			{
				this.isSending = false;
			}
		}

		private static void CopyProtectableData(MessageItem sourceItem, MessageItem targetItem)
		{
			RightsManagedMessageItem rightsManagedMessageItem = targetItem as RightsManagedMessageItem;
			if (rightsManagedMessageItem != null)
			{
				if (!rightsManagedMessageItem.isFullCharsetDetectionEnabled)
				{
					throw new InvalidOperationException();
				}
				rightsManagedMessageItem.isFullCharsetDetectionEnabled = false;
			}
			try
			{
				RightsManagedMessageItem.CopyProtectableData(sourceItem.Body, sourceItem.AttachmentCollection, targetItem);
			}
			finally
			{
				if (rightsManagedMessageItem != null)
				{
					rightsManagedMessageItem.isFullCharsetDetectionEnabled = true;
				}
			}
		}

		private static void CopyProtectableData(RightsManagedMessageItem sourceItem, MessageItem targetItem)
		{
			RightsManagedMessageItem.CopyProtectableData(sourceItem.Body, sourceItem.AttachmentCollection, targetItem);
		}

		private static void CopyProtectableData(Body sourceBody, AttachmentCollection sourceAttachmentCollection, MessageItem targetItem)
		{
			if (sourceBody.IsBodyDefined)
			{
				using (Stream stream = sourceBody.OpenReadStream(new BodyReadConfiguration(sourceBody.Format, sourceBody.RawCharset.Name)))
				{
					using (Stream stream2 = targetItem.Body.OpenWriteStream(new BodyWriteConfiguration(sourceBody.Format, sourceBody.RawCharset)))
					{
						Util.StreamHandler.CopyStreamData(stream, stream2);
					}
					goto IL_6D;
				}
			}
			targetItem.DeleteProperties(Body.BodyProps);
			IL_6D:
			targetItem.AttachmentCollection.RemoveAll();
			foreach (AttachmentHandle handle in sourceAttachmentCollection)
			{
				using (Attachment attachment = sourceAttachmentCollection.Open(handle))
				{
					using (Attachment attachment2 = attachment.CreateCopy(targetItem.AttachmentCollection, new BodyFormat?(targetItem.Body.Format)))
					{
						attachment2.Save();
					}
				}
			}
		}

		private static void CheckSession(StoreSession session)
		{
			if (session == null)
			{
				return;
			}
			MailboxSession mailboxSession = session as MailboxSession;
			if (mailboxSession == null)
			{
				throw new NotSupportedException("RightsManagedMessageItem can only be backed by a mailbox.");
			}
			if (mailboxSession.MailboxOwner.RecipientTypeDetails != RecipientTypeDetails.GroupMailbox && mailboxSession.LogonType == LogonType.Delegated)
			{
				throw new NotSupportedException("RightsManagedMessageItem doesn't support delegate scenario.");
			}
		}

		private void GetCharsetDetectionStringFromProtectedData(StringBuilder stringBuilder)
		{
			this.EnsureIsDecoded();
			this.decodedItem.CoreItem.GetCharsetDetectionData(stringBuilder, CharsetDetectionDataFlags.Complete);
			if (this.isFullCharsetDetectionEnabled)
			{
				using (TextReader textReader = this.ProtectedBody.OpenTextReader(BodyFormat.TextPlain))
				{
					char[] array = new char[32768];
					int charCount = textReader.ReadBlock(array, 0, array.Length);
					stringBuilder.Append(array, 0, charCount);
				}
			}
		}

		private void CheckPermission(ContentRight perms)
		{
			if ((this.effectiveRights & ContentRight.Owner) == ContentRight.Owner)
			{
				return;
			}
			if ((this.effectiveRights & perms) != perms)
			{
				ExTraceGlobals.RightsManagementTracer.TraceError(0L, "Not enough permissions to perform the requested operation.");
				throw new RightsManagementPermanentException(RightsManagementFailureCode.UserRightNotGranted, ServerStrings.NotEnoughPermissionsToPerformOperation);
			}
		}

		private void UpdateEffectiveRights()
		{
			ContentRight usageRightsInternal = this.UsageRightsInternal;
			this.effectiveRights = usageRightsInternal;
			foreach (ContentRight[] array2 in RightsManagedMessageItem.impliedRights)
			{
				if ((usageRightsInternal & array2[0]) == array2[0])
				{
					this.effectiveRights |= array2[1];
				}
			}
		}

		private void CopyLicenseDataFrom(RightsManagedMessageItem source)
		{
			this.rmsTemplate = source.rmsTemplate;
			this.conversationOwner = source.conversationOwner;
			this.publishLicense = source.publishLicense;
			this.serverUseLicense = source.serverUseLicense;
			this.restrictionInfo = source.restrictionInfo;
			this.publishedByExternalRMS = source.publishedByExternalRMS;
			this.UpdateEffectiveRights();
		}

		private RightsManagedMessageItem CreateReplyForwardInternal(MailboxSession session, StoreId parentFolderId, ReplyForwardConfiguration configuration, RightsManagedMessageItem.ReplyForwardCreationCall call)
		{
			Util.ThrowOnNullArgument(session, "session");
			Util.ThrowOnNullArgument(parentFolderId, "parentFolderId");
			Util.ThrowOnNullArgument(configuration, "configuration");
			if ((configuration.ForwardCreationFlags & (ForwardCreationFlags.PreserveSender | ForwardCreationFlags.TreatAsMeetingMessage)) != ForwardCreationFlags.None)
			{
				throw new InvalidOperationException("Invalid forward creation flags used.");
			}
			RightsManagedMessageItem result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				RightsManagedMessageItem rightsManagedMessageItem = RightsManagedMessageItem.Create(session, parentFolderId, this.options);
				disposeGuard.Add<RightsManagedMessageItem>(rightsManagedMessageItem);
				call(this, rightsManagedMessageItem, configuration);
				rightsManagedMessageItem.CopyLicenseDataFrom(this);
				disposeGuard.Success();
				result = rightsManagedMessageItem;
			}
			return result;
		}

		private void InitNewItem(OutboundConversionOptions options)
		{
			this[StoreObjectSchema.ContentClass] = "rpmsg.message";
			base.IconIndex = IconIndex.MailIrm;
			this.decodedItem = MessageItem.CreateInMemory(StoreObjectSchema.ContentConversionProperties);
			this.SetConversionOptions(options);
			this.UpdateEffectiveRights();
		}

		private void SetConversionOptions(OutboundConversionOptions options)
		{
			this.options = options;
			if (this.InternalSession != null)
			{
				this.orgId = this.InternalSession.MailboxOwner.MailboxInfo.OrganizationId;
				return;
			}
			if (options.UserADSession != null)
			{
				this.orgId = options.UserADSession.SessionSettings.CurrentOrganizationId;
				return;
			}
			this.orgId = OrganizationId.ForestWideOrgId;
		}

		private void UnprotectAllAttachments()
		{
			if (!(this.decodedItem.TryGetProperty(MessageItemSchema.DRMServerLicense) is string))
			{
				return;
			}
			foreach (AttachmentHandle handle in this.decodedItem.AttachmentCollection)
			{
				using (Attachment attachment = this.decodedItem.AttachmentCollection.Open(handle))
				{
					Stream stream = null;
					StreamAttachment streamAttachment = attachment as StreamAttachment;
					if (StreamAttachment.TryOpenRestrictedContent(streamAttachment, this.orgId, out stream))
					{
						using (stream)
						{
							using (Stream contentStream = streamAttachment.GetContentStream(PropertyOpenMode.Create))
							{
								Util.StreamHandler.CopyStreamData(stream, contentStream);
							}
						}
						attachment.Save();
					}
				}
			}
		}

		private void EnsureIsDecoded()
		{
			if (this.decodedItem == null)
			{
				if (this.options != null)
				{
					this.Decode(this.options, true);
				}
				if (this.decodedItem == null)
				{
					throw new InvalidOperationException("Message is not decoded yet.");
				}
			}
		}

		private const int InitialBufferCapacityForRpmsgStream = 131072;

		private static readonly byte[][] EmptyDrmLicense = new byte[][]
		{
			Array<byte>.Empty
		};

		private static ContentRight[][] impliedRights = new ContentRight[][]
		{
			new ContentRight[]
			{
				ContentRight.Edit,
				ContentRight.View | ContentRight.DocumentEdit
			},
			new ContentRight[]
			{
				ContentRight.Print,
				ContentRight.View
			},
			new ContentRight[]
			{
				ContentRight.Extract,
				ContentRight.View
			},
			new ContentRight[]
			{
				ContentRight.ObjectModel,
				ContentRight.View
			},
			new ContentRight[]
			{
				ContentRight.ViewRightsData,
				ContentRight.View
			},
			new ContentRight[]
			{
				ContentRight.Forward,
				ContentRight.View
			},
			new ContentRight[]
			{
				ContentRight.Reply,
				ContentRight.View
			},
			new ContentRight[]
			{
				ContentRight.ReplyAll,
				ContentRight.View
			},
			new ContentRight[]
			{
				ContentRight.DocumentEdit,
				ContentRight.View | ContentRight.Edit
			},
			new ContentRight[]
			{
				ContentRight.Export,
				ContentRight.View | ContentRight.Edit | ContentRight.DocumentEdit
			}
		};

		private MessageItem decodedItem;

		private RestrictionInfo restrictionInfo;

		private Participant conversationOwner;

		private ContentRight effectiveRights;

		private RmsTemplate rmsTemplate;

		private string publishLicense;

		private string serverUseLicense;

		private MessageItem originalItem;

		private bool publishedByExternalRMS;

		private OutboundConversionOptions options;

		private OrganizationId orgId;

		private bool isSending;

		private bool isFullCharsetDetectionEnabled = true;

		private RightsManagedMessageDecryptionStatus decryptionStatus = RightsManagedMessageDecryptionStatus.Success;

		private StringBuilder charsetDetectionStringForProtectedData;

		private bool licenseAcquired;

		private UseLicenseAndUsageRights useLicenseValue;

		private delegate void ReplyForwardCreationCall(RightsManagedMessageItem original, RightsManagedMessageItem result, ReplyForwardConfiguration configuration);
	}
}
