using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Conversations;
using Microsoft.Exchange.Data.TextConverters;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.DataConverter;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Services.Core.Conversations.Repositories
{
	internal class XsoConversationRepository<T> : IConversationRepository<T> where T : ICoreConversation
	{
		public XsoConversationRepository(ItemResponseShape itemResponseShape, PropertyDefinition[] propertiesToLoad, IIdConverter idConverter, ICoreConversationFactory<T> conversationFactory, IParticipantResolver participantResolver) : this(itemResponseShape, propertiesToLoad, idConverter, conversationFactory, null, participantResolver)
		{
		}

		public XsoConversationRepository(ItemResponseShape itemResponseShape, PropertyDefinition[] propertiesToLoad, IIdConverter idConverter, ICoreConversationFactory<T> conversationFactory, CallContext callContext, IParticipantResolver participantResolver)
		{
			this.itemResponseShape = itemResponseShape;
			this.propertiesToLoad = propertiesToLoad;
			this.idConverter = idConverter;
			this.conversationFactory = conversationFactory;
			this.PrepareSmimeProperties(callContext);
			this.participantResolver = participantResolver;
		}

		public Dictionary<StoreObjectId, HashSet<PropertyDefinition>> PropertiesLoadedPerItem
		{
			get
			{
				return this.propertiesLoadedPerItem;
			}
		}

		public HashSet<PropertyDefinition> PropertiesLoaded
		{
			get
			{
				return this.propertiesLoaded;
			}
		}

		private void PrepareSmimeProperties(CallContext callContext)
		{
			bool flag = this.VerifySmimeConversationFlightEnabled(callContext);
			bool flag2 = flag && callContext != null && callContext.DefaultDomain != null && callContext.IsOwa;
			this.isSmimeSupported = (flag2 && !callContext.IsSmimeInstalled);
			this.domainName = (this.isSmimeSupported ? callContext.DefaultDomain.ToString() : null);
			this.getInlineImageWhenClientSmimeInstalled = (flag2 && callContext.IsSmimeInstalled);
		}

		private bool VerifySmimeConversationFlightEnabled(CallContext callContext)
		{
			if (callContext == null || callContext.AccessingADUser == null)
			{
				return false;
			}
			VariantConfigurationSnapshot snapshot = VariantConfiguration.GetSnapshot(callContext.AccessingADUser.GetContext(null), null, null);
			return snapshot != null && snapshot.OwaClientServer.SmimeConversation.Enabled;
		}

		private ICollection<PropertyDefinition> GetAdditionalOpportunisticPropertiesToLoad(IStorePropertyBag storePropertyBag)
		{
			List<PropertyDefinition> list = new List<PropertyDefinition>();
			list.AddRange(ConversationLoaderHelper.EntityExtractionPropeties);
			if (IsClutterProperty.GetFlagValueOrDefaultFromStorePropertyBag(storePropertyBag, this.itemResponseShape))
			{
				list.AddRange(ConversationLoaderHelper.InferenceReasonsProperties);
			}
			return list;
		}

		private HashSet<PropertyDefinition> GetAdditionalPropertiesToLoad(IStorePropertyBag storePropertyBag)
		{
			HashSet<PropertyDefinition> hashSet = new HashSet<PropertyDefinition>();
			if (storePropertyBag.GetValueOrDefault<bool>(MessageItemSchema.ReplyToNamesExists, false) || storePropertyBag.GetValueOrDefault<bool>(MessageItemSchema.ReplyToBlobExists, false))
			{
				hashSet.Add(MessageItemSchema.ReplyToNames);
				hashSet.Add(MessageItemSchema.ReplyToBlob);
			}
			if (storePropertyBag.GetValueOrDefault<bool>(ItemSchema.IsClassified, false))
			{
				hashSet.UnionWith(ConversationLoaderHelper.ComplianceProperties);
			}
			string valueOrDefault = storePropertyBag.GetValueOrDefault<string>(StoreObjectSchema.ItemClass, string.Empty);
			if (ObjectClass.IsVoiceMessage(valueOrDefault))
			{
				hashSet.UnionWith(ConversationLoaderHelper.VoiceMailProperties);
			}
			if (ObjectClass.IsOfClass(valueOrDefault, "IPM.Note.Microsoft.Approval.Request"))
			{
				hashSet.UnionWith(ConversationLoaderHelper.ApprovalRequestProperties);
			}
			if (ObjectClass.IsMeetingMessage(valueOrDefault))
			{
				hashSet.UnionWith(ConversationLoaderHelper.MeetingMessageProperties);
				hashSet.UnionWith(ConversationLoaderHelper.SingleRecipientProperties);
			}
			if (ObjectClass.IsMeetingRequest(valueOrDefault))
			{
				hashSet.UnionWith(ConversationLoaderHelper.ChangeHighlightingProperties);
			}
			if (ObjectClass.IsEventReminderMessage(valueOrDefault))
			{
				hashSet.UnionWith(ConversationLoaderHelper.ReminderMessageProperties);
			}
			return hashSet;
		}

		private List<StoreObjectId> CalculateFoldersStoreObjectId(BaseFolderId[] folderIds)
		{
			List<StoreObjectId> list;
			if (folderIds != null)
			{
				list = new List<StoreObjectId>(folderIds.Length);
				foreach (BaseFolderId folderId in folderIds)
				{
					IdAndSession idAndSession = this.idConverter.ConvertFolderIdToIdAndSessionReadOnly(folderId);
					if (!(idAndSession.Session is MailboxSession))
					{
						throw new ServiceInvalidOperationException(CoreResources.IDs.ConversationSupportedOnlyForMailboxSession);
					}
					StoreObjectId asStoreObjectId = idAndSession.GetAsStoreObjectId();
					list.Add(asStoreObjectId);
				}
			}
			else
			{
				list = new List<StoreObjectId>(0);
			}
			return list;
		}

		private void ConversationOnBeforeItemLoad(LoadItemEventArgs eventArgs, params PropertyDefinition[] requestedProperties)
		{
			eventArgs.HtmlStreamOptionCallback = new HtmlStreamOptionCallback(this.GetSafeHtmlCallbacks);
			IStorePropertyBag storePropertyBag = eventArgs.StorePropertyBag;
			HashSet<PropertyDefinition> additionalPropertiesToLoad = this.GetAdditionalPropertiesToLoad(storePropertyBag);
			additionalPropertiesToLoad.UnionWith(requestedProperties);
			ICollection<PropertyDefinition> additionalOpportunisticPropertiesToLoad = this.GetAdditionalOpportunisticPropertiesToLoad(storePropertyBag);
			this.PropertiesLoadedPerItem[eventArgs.StorePropertyBag.GetValueOrDefault<VersionedId>(ItemSchema.Id, null).ObjectId] = new HashSet<PropertyDefinition>(additionalPropertiesToLoad);
			eventArgs.MessagePropertyDefinitions = additionalPropertiesToLoad.ToArray<PropertyDefinition>();
			eventArgs.OpportunisticLoadPropertyDefinitions = additionalOpportunisticPropertiesToLoad.ToArray<PropertyDefinition>();
		}

		private KeyValuePair<HtmlStreamingFlags, HtmlCallbackBase> GetSafeHtmlCallbacks(Item item)
		{
			bool itemBlockStatus = Util.GetItemBlockStatus(this.participantResolver, item, this.itemResponseShape.BlockExternalImages, this.itemResponseShape.BlockExternalImagesIfSenderUntrusted);
			HtmlBodyCallback htmlBodyCallback = new HtmlBodyCallback(item, null, this.getInlineImageWhenClientSmimeInstalled);
			htmlBodyCallback.AddBlankTargetToLinks = this.itemResponseShape.AddBlankTargetToLinks;
			htmlBodyCallback.InlineImageUrlTemplate = this.itemResponseShape.InlineImageUrlTemplate;
			htmlBodyCallback.InlineImageUrlOnLoadTemplate = this.itemResponseShape.InlineImageUrlOnLoadTemplate;
			htmlBodyCallback.InlineImageCustomDataTemplate = this.itemResponseShape.InlineImageCustomDataTemplate;
			htmlBodyCallback.IsBodyFragment = true;
			htmlBodyCallback.BlockExternalImages = itemBlockStatus;
			htmlBodyCallback.CalculateAttachmentInlineProps = this.itemResponseShape.CalculateAttachmentInlineProps;
			htmlBodyCallback.HasBlockedImagesAction = delegate(bool value)
			{
				EWSSettings.ItemHasBlockedImages = new bool?(value);
			};
			htmlBodyCallback.InlineAttachmentIdAction = delegate(string value)
			{
				IDictionary<string, bool> inlineImagesInUniqueBody = EWSSettings.InlineImagesInUniqueBody;
				inlineImagesInUniqueBody[value] = true;
			};
			HtmlBodyCallback value2 = htmlBodyCallback;
			return new KeyValuePair<HtmlStreamingFlags, HtmlCallbackBase>(HtmlStreamingFlags.FilterHtml, value2);
		}

		public T Load(ConversationId conversationId, IMailboxSession mailboxSession, BaseFolderId[] folderIds, bool useFolderIdsAsExclusionList = true, bool additionalPropertiesOnLoadItemParts = true, params PropertyDefinition[] requestedProperties)
		{
			return this.InternalLoad(conversationId, mailboxSession, folderIds, useFolderIdsAsExclusionList, additionalPropertiesOnLoadItemParts, requestedProperties);
		}

		public void PrefetchAndLoadItemParts(IMailboxSession mailboxSession, ICoreConversation conversation, HashSet<IConversationTreeNode> nodesToLoadItemPart, bool isSyncScenario)
		{
			List<StoreObjectId> list;
			if (isSyncScenario)
			{
				list = XsoConversationRepositoryExtensions.GetListStoreObjectIds(nodesToLoadItemPart);
			}
			else
			{
				list = conversation.GetMessageIdsForPreread();
			}
			if (list.Count > 0)
			{
				this.PrefetchItems(mailboxSession, list);
			}
			this.InternalLoadItemParts(conversation, nodesToLoadItemPart, false);
			this.participantResolver.LoadAdDataIfNeeded(conversation.AllParticipants(null));
		}

		public void LoadItemParts(ICoreConversation conversation, ICollection<IConversationTreeNode> changedTreeNodes, bool skipBodySummaries = false)
		{
			this.InternalLoadItemParts(conversation, changedTreeNodes, skipBodySummaries);
		}

		private void InternalLoadItemParts(ICoreConversation conversation, ICollection<IConversationTreeNode> changedTreeNodes, bool skipBodySummaries)
		{
			if (!changedTreeNodes.Any<IConversationTreeNode>())
			{
				return;
			}
			if (!skipBodySummaries)
			{
				conversation.LoadBodySummaries();
			}
			try
			{
				conversation.LoadItemParts(changedTreeNodes);
			}
			catch (TextConvertersException innerException)
			{
				throw new PropertyRequestFailedException(CoreResources.IDs.ErrorItemPropertyRequestFailed, ItemSchema.Body.PropertyPath, innerException);
			}
		}

		protected virtual bool CalculateIsIrmEnabled(IMailboxSession mailboxSession)
		{
			return IrmUtils.IsIrmEnabled(this.itemResponseShape.ClientSupportsIrm, (MailboxSession)mailboxSession);
		}

		protected virtual void PrefetchItems(IMailboxSession mailboxSession, List<StoreObjectId> itemIds)
		{
			((MailboxSession)mailboxSession).PrereadMessages(itemIds.ToArray());
		}

		protected virtual bool IsPermissionError(Exception exception)
		{
			return exception is MapiExceptionNoAccess;
		}

		private T InternalLoad(ConversationId conversationId, IMailboxSession mailboxSession, BaseFolderId[] folderIds, bool useFolderIdsAsExclusionList = true, bool additionalPropertiesOnLoadItemParts = true, params PropertyDefinition[] requestedProperties)
		{
			T result;
			try
			{
				bool isIrmEnabled = this.CalculateIsIrmEnabled(mailboxSession);
				this.propertiesLoadedPerItem = new Dictionary<StoreObjectId, HashSet<PropertyDefinition>>();
				this.propertiesLoaded = new HashSet<PropertyDefinition>();
				T t = this.conversationFactory.CreateConversation(conversationId, this.CalculateFoldersStoreObjectId(folderIds), useFolderIdsAsExclusionList, isIrmEnabled, this.isSmimeSupported, this.domainName, this.propertiesToLoad);
				this.propertiesLoaded = new HashSet<PropertyDefinition>(this.propertiesToLoad);
				if (additionalPropertiesOnLoadItemParts)
				{
					t.OnBeforeItemLoad += delegate(object sender, LoadItemEventArgs args)
					{
						this.ConversationOnBeforeItemLoad(args, requestedProperties);
					};
				}
				result = t;
			}
			catch (StoragePermanentException ex)
			{
				if (!this.IsPermissionError(ex.InnerException))
				{
					ExTraceGlobals.CommonAlgorithmTracer.TraceDebug((long)conversationId.GetHashCode(), "[GetConversationItems::LoadConversation] encountered exception - Class: {0}, Message: {1}.Inner exception was not MapiExceptionConversationNotFound but rather Class: {2}, Message {3}", new object[]
					{
						ex.GetType().FullName,
						ex.Message,
						(ex.InnerException == null) ? "<NULL>" : ex.InnerException.GetType().FullName,
						(ex.InnerException == null) ? "<NULL>" : ex.InnerException.Message
					});
					throw;
				}
				ExTraceGlobals.CommonAlgorithmTracer.TraceDebug((long)conversationId.GetHashCode(), "[GetConversationItems::LoadConversation] encountered exception - Class: {0}, Message: {1}.Inner exception was MapiExceptionNoAccess - Class: {2}, Message {3}", new object[]
				{
					ex.GetType().FullName,
					ex.Message,
					(ex.InnerException == null) ? "<NULL>" : ex.InnerException.GetType().FullName,
					(ex.InnerException == null) ? "<NULL>" : ex.InnerException.Message
				});
				result = default(T);
			}
			return result;
		}

		private readonly PropertyDefinition[] propertiesToLoad;

		private readonly ItemResponseShape itemResponseShape;

		private readonly IIdConverter idConverter;

		private readonly ICoreConversationFactory<T> conversationFactory;

		private bool isSmimeSupported;

		private string domainName;

		private bool getInlineImageWhenClientSmimeInstalled;

		private readonly IParticipantResolver participantResolver;

		private HashSet<PropertyDefinition> propertiesLoaded;

		private Dictionary<StoreObjectId, HashSet<PropertyDefinition>> propertiesLoadedPerItem;
	}
}
