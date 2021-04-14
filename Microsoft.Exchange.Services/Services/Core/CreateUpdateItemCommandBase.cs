using System;
using System.Globalization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Security.RightsManagement;
using Microsoft.Exchange.Services.Core.DataConverter;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal abstract class CreateUpdateItemCommandBase<RequestType, SingleItemType> : MultiStepServiceCommand<RequestType, SingleItemType> where RequestType : BaseRequest
	{
		public CreateUpdateItemCommandBase(CallContext callContext, RequestType request) : base(callContext, request)
		{
		}

		public event Action<MessageItem> BeforeMessageDisposition
		{
			add
			{
				this.beforeMessageDispositionEventHandler = (Action<MessageItem>)Delegate.Combine(this.beforeMessageDispositionEventHandler, value);
			}
			remove
			{
				this.beforeMessageDispositionEventHandler = (Action<MessageItem>)Delegate.Remove(this.beforeMessageDispositionEventHandler, value);
			}
		}

		public event Action<MessageItem> AfterMessageDisposition
		{
			add
			{
				this.afterMessageDispositionEventHandler = (Action<MessageItem>)Delegate.Combine(this.afterMessageDispositionEventHandler, value);
			}
			remove
			{
				this.afterMessageDispositionEventHandler = (Action<MessageItem>)Delegate.Remove(this.afterMessageDispositionEventHandler, value);
			}
		}

		public IdAndSession SaveToFolderIdAndSession
		{
			get
			{
				return this.saveToFolderIdAndSession;
			}
		}

		protected ItemType ExecuteOperation(Item item, ItemResponseShape responseShape, ConflictResolutionType conflictResolutionType, out ConflictResolutionResult conflictResolutionResult)
		{
			MessageItem messageItem = null;
			CalendarItemBase calendarItem = null;
			ItemType itemType;
			if (XsoDataConverter.TryGetStoreObject<MessageItem>(item, out messageItem))
			{
				itemType = this.ExecuteMessageOperation(messageItem, responseShape, conflictResolutionType, out conflictResolutionResult);
			}
			else if (XsoDataConverter.TryGetStoreObject<CalendarItemBase>(item, out calendarItem))
			{
				conflictResolutionResult = this.ExecuteCalendarOperation(calendarItem, conflictResolutionType);
				item.Load();
				itemType = new EwsCalendarItemType();
				base.LoadServiceObject(itemType, item, IdAndSession.CreateFromItem(item), responseShape);
			}
			else
			{
				itemType = this.ExecuteItemOperation(item, conflictResolutionType, responseShape, out conflictResolutionResult);
			}
			return itemType;
		}

		protected ConflictResolutionResult SaveThenSendMeetingMessages(CalendarItemBase calendarItem, bool isToAllAttendees, bool copyToSentItems, ConflictResolutionType conflictResolutionType)
		{
			calendarItem.SetClientIntentBasedOnModifiedProperties(null);
			ConflictResolutionResult result = base.SaveXsoItem(calendarItem, conflictResolutionType);
			calendarItem.SendMeetingMessages(isToAllAttendees, null, false, copyToSentItems, null, null);
			return result;
		}

		protected ConflictResolutionResult SaveWhileSendingMeetingMessages(CalendarItem calendarItem, bool isToAllAttendees, bool copyToSentItems, ConflictResolutionType conflictResolutionType)
		{
			calendarItem.SetClientIntentBasedOnModifiedProperties(null);
			return base.SaveXsoItem(calendarItem, delegate(SaveMode saveModeDelegate)
			{
				calendarItem.SaveModeOnSendMeetingMessages = saveModeDelegate;
				calendarItem.SendMeetingMessages(isToAllAttendees, null, false, copyToSentItems, null, null);
				if (conflictResolutionType == ConflictResolutionType.AutoResolve)
				{
					return new ConflictResolutionResult(SaveResult.SuccessWithConflictResolution, new PropertyConflict[]
					{
						new PropertyConflict(StoreObjectSchema.LastModifiedTime, DateTime.Now.AddSeconds(-2.0), DateTime.Now.AddSeconds(-1.0), DateTime.Now, DateTime.Now, true)
					});
				}
				return new ConflictResolutionResult(SaveResult.Success, null);
			}, conflictResolutionType, null);
		}

		protected ConflictResolutionResult SendMeetingMessageOnUpdate(CalendarItemBase calendarItemBase, bool isToAllAttendees, bool copyToSentItems, ConflictResolutionType conflictResolutionType)
		{
			CalendarItem calendarItem = calendarItemBase as CalendarItem;
			if (calendarItem == null)
			{
				return this.SaveThenSendMeetingMessages(calendarItemBase, isToAllAttendees, copyToSentItems, conflictResolutionType);
			}
			return this.SaveWhileSendingMeetingMessages(calendarItem, isToAllAttendees, copyToSentItems, conflictResolutionType);
		}

		protected virtual ItemType ExecuteItemOperation(Item item, ConflictResolutionType conflictResolutionType, ItemResponseShape responseShape, out ConflictResolutionResult conflictResolutionResult)
		{
			RightsManagedMessageItem rightsManagedMessageItem = item as RightsManagedMessageItem;
			bool flag = rightsManagedMessageItem != null && rightsManagedMessageItem.IsDecoded;
			conflictResolutionResult = base.SaveXsoItem(item, conflictResolutionType);
			if (flag)
			{
				IrmUtils.DecodeIrmMessage(item.Session, item, true);
			}
			StoreObjectId storeObjectId = StoreId.GetStoreObjectId(item.Id);
			ItemType itemType = ItemType.CreateFromStoreObjectType(storeObjectId.ObjectType);
			ToServiceObjectPropertyList toServiceObjectPropertyList = XsoDataConverter.GetToServiceObjectPropertyList(item, responseShape);
			toServiceObjectPropertyList.CharBuffer = this.charBuffer;
			ServiceCommandBase.LoadServiceObject(itemType, item, IdAndSession.CreateFromItem(item), responseShape, toServiceObjectPropertyList);
			return itemType;
		}

		protected ItemType ExecuteMessageOperation(MessageItem messageItem, ItemResponseShape responseShape, ConflictResolutionType conflictResolutionType, out ConflictResolutionResult conflictResolutionResult)
		{
			CreateUpdateItemCommandBase<RequestType, SingleItemType>.<>c__DisplayClass4 CS$<>8__locals1 = new CreateUpdateItemCommandBase<RequestType, SingleItemType>.<>c__DisplayClass4();
			CS$<>8__locals1.messageItem = messageItem;
			CS$<>8__locals1.conflictResolutionType = conflictResolutionType;
			CS$<>8__locals1.<>4__this = this;
			ItemType result = null;
			conflictResolutionResult = new ConflictResolutionResult(SaveResult.Success, null);
			CS$<>8__locals1.mailboxSession = (CS$<>8__locals1.messageItem.Session as MailboxSession);
			this.OnBeforeMessageDisposition(CS$<>8__locals1.messageItem);
			switch (this.messageDisposition.Value)
			{
			case MessageDispositionType.SendOnly:
				this.SendMessageItemWithoutSaving(CS$<>8__locals1.messageItem, CS$<>8__locals1.mailboxSession);
				break;
			case MessageDispositionType.SaveOnly:
				result = this.SaveMessageItem(CS$<>8__locals1.messageItem, responseShape, CS$<>8__locals1.conflictResolutionType, out conflictResolutionResult);
				break;
			case MessageDispositionType.SendAndSaveCopy:
			{
				StoreObjectId sentItemsFolderId = this.GetSentItemsFolderId(CS$<>8__locals1.messageItem);
				try
				{
					CreateItemRequest createItemRequest = base.Request as CreateItemRequest;
					UpdateItemRequest updateItemRequest = base.Request as UpdateItemRequest;
					if ((createItemRequest != null && createItemRequest.ItemShape != null) || (updateItemRequest != null && updateItemRequest.ItemShape != null))
					{
						result = this.LoadItemAfterOperation(CS$<>8__locals1.messageItem, responseShape, delegate
						{
							CS$<>8__locals1.<>4__this.SendMessage(CS$<>8__locals1.messageItem, CS$<>8__locals1.mailboxSession, sentItemsFolderId, CS$<>8__locals1.conflictResolutionType);
							CS$<>8__locals1.messageItem.Load();
						});
					}
					else
					{
						this.SendMessage(CS$<>8__locals1.messageItem, CS$<>8__locals1.mailboxSession, sentItemsFolderId, CS$<>8__locals1.conflictResolutionType);
					}
				}
				catch (AccessDeniedException innerException)
				{
					if (ExchangeVersion.Current.Supports(ExchangeVersion.Exchange2007SP1))
					{
						throw new ServiceAccessDeniedException(CoreResources.IDs.MessageInsufficientPermissionsToSend, innerException);
					}
					throw;
				}
				catch (LocalizedException localizedException)
				{
					this.DeleteDraftCreatedOnSendFailure(localizedException, CS$<>8__locals1.mailboxSession, CS$<>8__locals1.messageItem.Id);
					throw;
				}
				break;
			}
			}
			this.OnAfterMessageDisposition(CS$<>8__locals1.messageItem);
			return result;
		}

		protected IdAndSession GetMessageParentFolderIdAndSession()
		{
			if (base.CallContext.AccessingPrincipal == null && this.messageDisposition != null && this.messageDisposition.Value == MessageDispositionType.SendOnly)
			{
				throw new ServiceInvalidOperationException((CoreResources.IDs)2271901695U);
			}
			return this.saveToFolderIdAndSession ?? this.GetDefaultParentFolderIdAndSession(DefaultFolderType.Drafts);
		}

		protected void SetBodyCharsetOptions(Item item)
		{
			StoreSession session = this.GetMessageParentFolderIdAndSession().Session;
			if (session != null)
			{
				Charset preferredCharset = null;
				string name;
				BodyCharsetFlags bodyCharsetOptions = this.GetBodyCharsetOptions(out name);
				Charset.TryGetCharset(name, out preferredCharset);
				OutboundConversionOptions outboundConversionOptions = new OutboundConversionOptions(base.CallContext.DefaultDomain.DomainName.Domain);
				outboundConversionOptions.LoadPerOrganizationCharsetDetectionOptions(session.OrganizationId);
				outboundConversionOptions.DetectionOptions.PreferredCharset = preferredCharset;
				item.CharsetDetector.DetectionOptions = outboundConversionOptions.DetectionOptions;
				item.CharsetDetector.CharsetFlags = bodyCharsetOptions;
			}
		}

		private BodyCharsetFlags GetBodyCharsetOptions(out string charsetName)
		{
			BodyCharsetFlags bodyCharsetFlags = BodyCharsetFlags.None;
			bool flag = false;
			bool flag2 = false;
			OutboundCharsetOptions outboundCharsetOptions = OutboundCharsetOptions.AutoDetect;
			if (base.Request is CreateItemRequest)
			{
				CreateItemRequest createItemRequest = base.Request as CreateItemRequest;
				flag = createItemRequest.UseGB18030;
				flag2 = createItemRequest.UseISO885915;
				outboundCharsetOptions = createItemRequest.OutboundCharsetOptions;
			}
			else if (base.Request is UpdateItemRequest)
			{
				UpdateItemRequest updateItemRequest = base.Request as UpdateItemRequest;
				flag = updateItemRequest.UseGB18030;
				flag2 = updateItemRequest.UseISO885915;
				outboundCharsetOptions = updateItemRequest.OutboundCharsetOptions;
			}
			if (flag)
			{
				bodyCharsetFlags |= BodyCharsetFlags.PreferGB18030;
			}
			if (flag2)
			{
				bodyCharsetFlags |= BodyCharsetFlags.PreferIso885915;
			}
			if (outboundCharsetOptions == OutboundCharsetOptions.AlwaysUTF8)
			{
				bodyCharsetFlags |= BodyCharsetFlags.DisableCharsetDetection;
				charsetName = "utf-8";
			}
			else
			{
				if (outboundCharsetOptions == OutboundCharsetOptions.UserLanguageChoice)
				{
					bodyCharsetFlags |= BodyCharsetFlags.DisableCharsetDetection;
				}
				else
				{
					bodyCharsetFlags = bodyCharsetFlags;
				}
				CultureInfo clientCulture = base.CallContext.ClientCulture;
				Culture culture = null;
				if (Culture.TryGetCulture(clientCulture.Name, out culture))
				{
					Charset mimeCharset = culture.MimeCharset;
					if (mimeCharset.IsAvailable)
					{
						charsetName = mimeCharset.Name;
						return bodyCharsetFlags;
					}
				}
				charsetName = Culture.Default.MimeCharset.Name;
			}
			return bodyCharsetFlags;
		}

		private void OnBeforeMessageDisposition(MessageItem messageItem)
		{
			if (this.beforeMessageDispositionEventHandler != null)
			{
				this.beforeMessageDispositionEventHandler(messageItem);
			}
		}

		private void OnAfterMessageDisposition(MessageItem messageItem)
		{
			if (this.afterMessageDispositionEventHandler != null)
			{
				this.afterMessageDispositionEventHandler(messageItem);
			}
		}

		private void SendMessage(MessageItem messageItem, MailboxSession mailboxSession, StoreObjectId sentItemsFolderId, ConflictResolutionType conflictResolutionType)
		{
			CreateItemRequest createItemRequest = base.Request as CreateItemRequest;
			if (createItemRequest != null && createItemRequest.IsNonDraft)
			{
				messageItem.MarkAsNonDraft();
			}
			SaveMode saveMode = base.GetSaveMode(conflictResolutionType);
			if (sentItemsFolderId.Equals(messageItem.ParentId))
			{
				messageItem.SendWithoutMovingMessage(sentItemsFolderId, saveMode);
				return;
			}
			messageItem.Send(sentItemsFolderId, saveMode);
		}

		private void SendMessageItemWithoutSaving(MessageItem messageItem, MailboxSession mailboxSession)
		{
			try
			{
				messageItem.SendWithoutSavingMessage();
			}
			catch (LocalizedException localizedException)
			{
				this.DeleteDraftCreatedOnSendFailure(localizedException, mailboxSession, messageItem.Id);
				throw;
			}
		}

		private ItemType SaveMessageItem(MessageItem messageItem, ItemResponseShape responseShape, ConflictResolutionType conflictResolutionType, out ConflictResolutionResult conflictResolutionResult)
		{
			ConflictResolutionResult localResult = null;
			CreateItemRequest createItemRequest = base.Request as CreateItemRequest;
			if (createItemRequest != null && createItemRequest.IsNonDraft)
			{
				messageItem.MarkAsNonDraft();
				if (base.CallContext.IsOwa)
				{
					messageItem[MessageItemSchema.NeedSpecialRecipientProcessing] = true;
				}
			}
			ItemType result = this.LoadItemAfterOperation(messageItem, responseShape, delegate
			{
				localResult = this.SaveXsoItem(messageItem, conflictResolutionType);
			});
			conflictResolutionResult = localResult;
			return result;
		}

		protected Item GetItemForUpdate(IdAndSession idAndSession, bool sendOnNotFoundError)
		{
			Item result;
			try
			{
				result = ServiceCommandBase.GetXsoItemForUpdate(idAndSession, new PropertyDefinition[]
				{
					MessageItemSchema.Flags
				});
			}
			catch (ObjectNotFoundException)
			{
				if (!sendOnNotFoundError || (this.messageDisposition.Value != MessageDispositionType.SendAndSaveCopy && this.messageDisposition.Value != MessageDispositionType.SendOnly))
				{
					throw;
				}
				ExTraceGlobals.CreateItemCallTracer.TraceDebug<StoreId>((long)this.GetHashCode(), "CreateUpdateItemCommandBase.GetItemForUpdate: item to update {0} not found; Creating a new item.", idAndSession.Id);
				result = MessageItem.Create(idAndSession.Session, idAndSession.Session.GetDefaultFolderId(DefaultFolderType.Drafts));
			}
			return result;
		}

		protected IdAndSession GetDefaultParentFolderIdAndSession(DefaultFolderType defaultFolderType)
		{
			return new IdAndSession(base.MailboxIdentityMailboxSession.GetRefreshedDefaultFolderId(defaultFolderType), base.MailboxIdentityMailboxSession);
		}

		private StoreObjectId GetSentItemsFolderId(MessageItem messageItem)
		{
			IdAndSession idAndSession;
			if (this.saveToFolderIdAndSession != null)
			{
				idAndSession = this.saveToFolderIdAndSession;
			}
			else
			{
				MailboxSession mailboxSession = messageItem.Session as MailboxSession;
				idAndSession = base.IdConverter.ConvertDefaultFolderType(DefaultFolderType.SentItems, mailboxSession.MailboxOwner.MailboxInfo.PrimarySmtpAddress.ToString());
			}
			return idAndSession.GetAsStoreObjectId();
		}

		protected abstract ConflictResolutionResult ExecuteCalendarOperation(CalendarItemBase calendarItem, ConflictResolutionType resolutionType);

		protected virtual void DeleteDraftCreatedOnSendFailure(LocalizedException localizedException, MailboxSession mailboxSession, StoreId storeId)
		{
		}

		protected void RequireMessageDisposition()
		{
			if (this.messageDisposition == null)
			{
				throw new MessageDispositionRequiredException();
			}
		}

		protected bool IsSaveToFolderIdSessionPublicFolderSession()
		{
			return this.saveToFolderIdAndSession != null && this.saveToFolderIdAndSession.Session is PublicFolderSession;
		}

		protected DelegateSessionHandleWrapper GetDelegateSessionHandleWrapperAndWorkflowContext(IdAndSession referenceIdAndSession, out IdAndSession adjustedIdAndSession, out string workflowMailboxSmtpAddress)
		{
			return this.GetDelegateSessionHandleWrapperAndWorkflowContext(referenceIdAndSession, false, out adjustedIdAndSession, out workflowMailboxSmtpAddress);
		}

		protected DelegateSessionHandleWrapper GetDelegateSessionHandleWrapperAndWorkflowContext(IdAndSession referenceIdAndSession, bool checkSameAccountForOwnerLogon, out IdAndSession adjustedIdAndSession, out string workflowMailboxSmtpAddress)
		{
			MailboxSession mailboxSession = referenceIdAndSession.Session as MailboxSession;
			if (mailboxSession == null || base.CallContext.AccessingPrincipal == null || (mailboxSession.LogonType == LogonType.Owner && (!checkSameAccountForOwnerLogon || string.Equals(mailboxSession.MailboxOwner.MailboxInfo.PrimarySmtpAddress.ToString(), base.CallContext.AccessingPrincipal.MailboxInfo.PrimarySmtpAddress.ToString(), StringComparison.OrdinalIgnoreCase))) || mailboxSession.LogonType == LogonType.Admin || mailboxSession.LogonType == LogonType.SystemService || !ExchangeVersionDeterminer.MatchesLocalServerVersion(base.CallContext.AccessingPrincipal.MailboxInfo.Location.ServerVersion))
			{
				adjustedIdAndSession = referenceIdAndSession;
				workflowMailboxSmtpAddress = ((MailboxSession)adjustedIdAndSession.Session).MailboxOwner.MailboxInfo.PrimarySmtpAddress.ToString();
				return null;
			}
			this.LogDelegateSession(mailboxSession.MailboxOwner.MailboxInfo.PrimarySmtpAddress.ToString());
			DelegateSessionHandleWrapper delegateSessionHandleWrapper = new DelegateSessionHandleWrapper(base.MailboxIdentityMailboxSession.GetDelegateSessionHandleForEWS(mailboxSession.MailboxOwner));
			adjustedIdAndSession = new IdAndSession(referenceIdAndSession.Id, delegateSessionHandleWrapper.Handle.MailboxSession);
			workflowMailboxSmtpAddress = base.MailboxIdentityMailboxSession.MailboxOwner.MailboxInfo.PrimarySmtpAddress.ToString();
			return delegateSessionHandleWrapper;
		}

		private ItemType LoadItemAfterOperation(MessageItem messageItem, ItemResponseShape responseShape, Action operation)
		{
			RightsManagedMessageItem rightsManagedMessageItem = messageItem as RightsManagedMessageItem;
			bool flag = rightsManagedMessageItem != null && rightsManagedMessageItem.IsDecoded;
			operation();
			if (flag)
			{
				IrmUtils.DecodeIrmMessage(messageItem.Session, messageItem, false);
			}
			ItemType itemType = ItemType.CreateFromStoreObjectType(messageItem.Id.ObjectId.ObjectType);
			ToServiceObjectPropertyList toServiceObjectPropertyList = XsoDataConverter.GetToServiceObjectPropertyList(messageItem, responseShape);
			toServiceObjectPropertyList.CharBuffer = this.charBuffer;
			ServiceCommandBase.LoadServiceObject(itemType, messageItem, IdAndSession.CreateFromItem(messageItem), responseShape, toServiceObjectPropertyList);
			itemType.Conversation = Util.LoadConversationUsingConversationId((ConversationId)PropertyCommand.GetPropertyValueFromStoreObject(messageItem, ItemSchema.ConversationId), responseShape.ConversationShapeName, responseShape.ConversationFolderId, base.IdConverter, this.GetHashCode(), base.CallContext.ProtocolLog);
			return itemType;
		}

		protected MessageDispositionType? messageDisposition = null;

		protected IdAndSession saveToFolderIdAndSession;

		protected char[] charBuffer;

		protected RmsTemplate rmsTemplate;

		private Action<MessageItem> beforeMessageDispositionEventHandler;

		private Action<MessageItem> afterMessageDispositionEventHandler;
	}
}
