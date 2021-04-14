using System;
using System.Globalization;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	[Serializable]
	internal class OwaStoreObjectId : ObjectId
	{
		private OwaStoreObjectId(ConversationId conversationId, StoreObjectId folderId, byte[] instanceKey)
		{
			this.objectIdType = OwaStoreObjectIdType.Conversation;
			this.storeObjectId = conversationId;
			this.containerFolderId = folderId;
			this.instanceKey = instanceKey;
		}

		private OwaStoreObjectId(ConversationId conversationId, StoreObjectId folderId, string mailboxOwnerLegacyDN, byte[] instanceKey)
		{
			this.objectIdType = OwaStoreObjectIdType.ArchiveConversation;
			this.storeObjectId = conversationId;
			this.containerFolderId = folderId;
			this.mailboxOwnerLegacyDN = mailboxOwnerLegacyDN;
			this.instanceKey = instanceKey;
		}

		private OwaStoreObjectId(OwaStoreObjectIdType objectIdType, StoreObjectId storeObjectId) : this(objectIdType, storeObjectId, null, null)
		{
		}

		private OwaStoreObjectId(OwaStoreObjectIdType objectIdType, StoreObjectId storeObjectId, string mailboxOwnerLegacyDN) : this(objectIdType, storeObjectId, null, mailboxOwnerLegacyDN)
		{
		}

		private OwaStoreObjectId(OwaStoreObjectIdType objectIdType, StoreObjectId storeObjectId, StoreObjectId containerFolderId) : this(objectIdType, storeObjectId, containerFolderId, null)
		{
		}

		private OwaStoreObjectId(OwaStoreObjectIdType objectIdType, StoreObjectId storeObjectId, StoreObjectId containerFolderId, string mailboxOwnerLegacyDN)
		{
			this.objectIdType = objectIdType;
			this.storeObjectId = storeObjectId;
			this.containerFolderId = containerFolderId;
			this.mailboxOwnerLegacyDN = mailboxOwnerLegacyDN;
		}

		public static StoreObjectId[] ConvertToStoreObjectIdArray(params OwaStoreObjectId[] owaStoreObjectIds)
		{
			if (owaStoreObjectIds == null)
			{
				throw new ArgumentNullException("owaStoreObjectIds");
			}
			StoreObjectId[] array = new StoreObjectId[owaStoreObjectIds.Length];
			for (int i = 0; i < owaStoreObjectIds.Length; i++)
			{
				array[i] = owaStoreObjectIds[i].StoreObjectId;
			}
			return array;
		}

		public static OwaStoreObjectId CreateFromItemId(StoreObjectId itemStoreObjectId, OwaStoreObjectId containerFolderId)
		{
			if (itemStoreObjectId == null)
			{
				throw new ArgumentNullException("itemStoreObjectId");
			}
			if (containerFolderId == null)
			{
				throw new ArgumentNullException("containerFolderId");
			}
			return OwaStoreObjectId.CreateFromItemId(itemStoreObjectId, containerFolderId.StoreObjectId, containerFolderId.OwaStoreObjectIdType, containerFolderId.MailboxOwnerLegacyDN);
		}

		public static OwaStoreObjectId CreateFromStoreObjectId(StoreObjectId storeObjectId, OwaStoreObjectId relatedStoreObjectId)
		{
			if (storeObjectId == null)
			{
				throw new ArgumentNullException("storeObjectId");
			}
			StoreObjectId storeObjectId2 = null;
			OwaStoreObjectIdType owaStoreObjectIdType = OwaStoreObjectIdType.MailBoxObject;
			if (IdConverter.IsFromPublicStore(storeObjectId))
			{
				if (IdConverter.IsMessageId(storeObjectId))
				{
					owaStoreObjectIdType = OwaStoreObjectIdType.PublicStoreItem;
					storeObjectId2 = IdConverter.GetParentIdFromMessageId(storeObjectId);
				}
				else
				{
					owaStoreObjectIdType = OwaStoreObjectIdType.PublicStoreFolder;
				}
			}
			else if (relatedStoreObjectId != null)
			{
				if (!relatedStoreObjectId.IsConversationId)
				{
					owaStoreObjectIdType = relatedStoreObjectId.OwaStoreObjectIdType;
				}
				else if (relatedStoreObjectId.OwaStoreObjectIdType == OwaStoreObjectIdType.ArchiveConversation)
				{
					owaStoreObjectIdType = OwaStoreObjectIdType.ArchiveMailboxObject;
				}
			}
			return new OwaStoreObjectId(owaStoreObjectIdType, storeObjectId, storeObjectId2, (relatedStoreObjectId == null) ? null : relatedStoreObjectId.MailboxOwnerLegacyDN);
		}

		public static OwaStoreObjectId CreateFromItemId(StoreObjectId itemStoreObjectId, Folder containerFolder)
		{
			if (itemStoreObjectId == null)
			{
				throw new ArgumentNullException("itemStoreObjectId");
			}
			if (containerFolder == null)
			{
				throw new ArgumentNullException("containerFolder");
			}
			OwaStoreObjectIdType owaStoreObjectIdType = OwaStoreObjectIdType.MailBoxObject;
			string legacyDN = null;
			if (Utilities.IsPublic(containerFolder))
			{
				owaStoreObjectIdType = OwaStoreObjectIdType.PublicStoreItem;
			}
			else if (Utilities.IsOtherMailbox(containerFolder))
			{
				owaStoreObjectIdType = OwaStoreObjectIdType.OtherUserMailboxObject;
				legacyDN = Utilities.GetMailboxSessionLegacyDN(containerFolder);
			}
			else if (Utilities.IsInArchiveMailbox(containerFolder))
			{
				owaStoreObjectIdType = OwaStoreObjectIdType.ArchiveMailboxObject;
				legacyDN = Utilities.GetMailboxSessionLegacyDN(containerFolder);
			}
			return OwaStoreObjectId.CreateFromItemId(itemStoreObjectId, (containerFolder.Id == null) ? null : containerFolder.Id.ObjectId, owaStoreObjectIdType, legacyDN);
		}

		public static OwaStoreObjectId CreateFromMailboxItemId(StoreObjectId mailboxItemStoreObjectId)
		{
			if (mailboxItemStoreObjectId == null)
			{
				throw new ArgumentNullException("mailboxItemStoreObjectId");
			}
			return new OwaStoreObjectId(OwaStoreObjectIdType.MailBoxObject, mailboxItemStoreObjectId);
		}

		public static OwaStoreObjectId CreateFromStoreObject(StoreObject storeObject)
		{
			if (storeObject == null)
			{
				throw new ArgumentNullException("storeObject");
			}
			if (storeObject.Id == null)
			{
				throw new ArgumentException("storeObject.Id must not be null");
			}
			OwaStoreObjectIdType owaStoreObjectIdType = OwaStoreObjectIdType.MailBoxObject;
			string legacyDN = null;
			if (Utilities.IsOtherMailbox(storeObject))
			{
				owaStoreObjectIdType = OwaStoreObjectIdType.OtherUserMailboxObject;
				legacyDN = Utilities.GetMailboxSessionLegacyDN(storeObject);
			}
			else if (Utilities.IsInArchiveMailbox(storeObject))
			{
				owaStoreObjectIdType = OwaStoreObjectIdType.ArchiveMailboxObject;
				legacyDN = Utilities.GetMailboxSessionLegacyDN(storeObject);
			}
			if (storeObject is Item)
			{
				if (Utilities.IsPublic(storeObject))
				{
					owaStoreObjectIdType = OwaStoreObjectIdType.PublicStoreItem;
				}
				return OwaStoreObjectId.CreateFromItemId(storeObject.Id.ObjectId, (owaStoreObjectIdType == OwaStoreObjectIdType.OtherUserMailboxObject) ? null : storeObject.ParentId, owaStoreObjectIdType, legacyDN);
			}
			if (storeObject is Folder)
			{
				if (Utilities.IsPublic(storeObject))
				{
					owaStoreObjectIdType = OwaStoreObjectIdType.PublicStoreFolder;
				}
				return OwaStoreObjectId.CreateFromFolderId(storeObject.Id.ObjectId, owaStoreObjectIdType, legacyDN);
			}
			string message = string.Format(CultureInfo.InvariantCulture, "OwaStoreObjectId.CreateOwaStoreObjectId(StoreObject) only support item or folder as input, but the input is an {0}", new object[]
			{
				storeObject.GetType().ToString()
			});
			throw new ArgumentOutOfRangeException("storeObject", message);
		}

		public static OwaStoreObjectId CreateFromSessionFolderId(UserContext userContext, StoreSession session, StoreObjectId folderId)
		{
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			if (folderId == null)
			{
				throw new ArgumentNullException("folderId");
			}
			if (userContext.IsMyMailbox(session))
			{
				return OwaStoreObjectId.CreateFromSessionFolderId(OwaStoreObjectIdType.MailBoxObject, null, folderId);
			}
			if (userContext.IsOtherMailbox(session))
			{
				return OwaStoreObjectId.CreateFromSessionFolderId(OwaStoreObjectIdType.OtherUserMailboxObject, ((MailboxSession)session).MailboxOwner.LegacyDn, folderId);
			}
			if (Utilities.IsArchiveMailbox(session))
			{
				return OwaStoreObjectId.CreateFromSessionFolderId(OwaStoreObjectIdType.ArchiveMailboxObject, ((MailboxSession)session).MailboxOwnerLegacyDN, folderId);
			}
			if (session is PublicFolderSession)
			{
				return OwaStoreObjectId.CreateFromSessionFolderId(OwaStoreObjectIdType.PublicStoreFolder, null, folderId);
			}
			throw new ArgumentException("The type of session is unknown");
		}

		public static OwaStoreObjectId CreateFromSessionFolderId(OwaStoreObjectIdType owaStoreObjectIdType, string legacyDN, StoreObjectId folderId)
		{
			if (folderId == null)
			{
				throw new ArgumentNullException("folderId");
			}
			switch (owaStoreObjectIdType)
			{
			case OwaStoreObjectIdType.MailBoxObject:
				return OwaStoreObjectId.CreateFromMailboxFolderId(folderId);
			case OwaStoreObjectIdType.PublicStoreFolder:
				return OwaStoreObjectId.CreateFromPublicFolderId(folderId);
			case OwaStoreObjectIdType.OtherUserMailboxObject:
				return OwaStoreObjectId.CreateFromOtherUserMailboxFolderId(folderId, legacyDN);
			case OwaStoreObjectIdType.ArchiveMailboxObject:
				return OwaStoreObjectId.CreateFromArchiveMailboxFolderId(folderId, legacyDN);
			}
			throw new ArgumentException("mailbox session type is unknown");
		}

		public static OwaStoreObjectIdType GetOwaStoreObjectIdType(UserContext userContext, StoreSession session, out string mailboxOwnerLegacyDN)
		{
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			mailboxOwnerLegacyDN = string.Empty;
			if (userContext.IsMyMailbox(session))
			{
				return OwaStoreObjectIdType.MailBoxObject;
			}
			if (userContext.IsOtherMailbox(session))
			{
				mailboxOwnerLegacyDN = ((MailboxSession)session).MailboxOwner.LegacyDn;
				return OwaStoreObjectIdType.OtherUserMailboxObject;
			}
			if (Utilities.IsArchiveMailbox(session))
			{
				mailboxOwnerLegacyDN = ((MailboxSession)session).MailboxOwnerLegacyDN;
				return OwaStoreObjectIdType.ArchiveMailboxObject;
			}
			if (session is PublicFolderSession)
			{
				return OwaStoreObjectIdType.PublicStoreFolder;
			}
			throw new ArgumentException("The type of session is unknown");
		}

		public static OwaStoreObjectId CreateFromNavigationNodeFolder(UserContext userContext, NavigationNodeFolder nodeFolder)
		{
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			if (nodeFolder == null)
			{
				throw new ArgumentNullException("nodeFolder");
			}
			if (!nodeFolder.IsValid)
			{
				throw new ArgumentException("Not valid navigation node folder.");
			}
			if (!nodeFolder.IsGSCalendar && nodeFolder.FolderId == null)
			{
				throw new NotSupportedException("Doesn't support this kind of node folder");
			}
			if (nodeFolder.IsFolderInSpecificMailboxSession(userContext.MailboxSession))
			{
				return OwaStoreObjectId.CreateFromMailboxFolderId(nodeFolder.FolderId);
			}
			if (nodeFolder.IsGSCalendar)
			{
				return OwaStoreObjectId.CreateFromGSCalendarLegacyDN(nodeFolder.MailboxLegacyDN);
			}
			ExchangePrincipal exchangePrincipal;
			if (userContext.DelegateSessionManager.TryGetExchangePrincipal(nodeFolder.MailboxLegacyDN, out exchangePrincipal) && exchangePrincipal.MailboxInfo.IsArchive)
			{
				return OwaStoreObjectId.CreateFromArchiveMailboxFolderId(nodeFolder.FolderId, nodeFolder.MailboxLegacyDN);
			}
			return OwaStoreObjectId.CreateFromOtherUserMailboxFolderId(nodeFolder.FolderId, nodeFolder.MailboxLegacyDN);
		}

		public static OwaStoreObjectId CreateFromString(string owaStoreObjectIdString)
		{
			OwaStoreObjectIdType owaStoreObjectIdType = OwaStoreObjectIdType.MailBoxObject;
			if (owaStoreObjectIdString.StartsWith("PSF.", StringComparison.Ordinal))
			{
				owaStoreObjectIdType = OwaStoreObjectIdType.PublicStoreFolder;
			}
			else if (owaStoreObjectIdString.StartsWith("PSI.", StringComparison.Ordinal))
			{
				owaStoreObjectIdType = OwaStoreObjectIdType.PublicStoreItem;
			}
			else if (owaStoreObjectIdString.StartsWith("OUM.", StringComparison.Ordinal))
			{
				owaStoreObjectIdType = OwaStoreObjectIdType.OtherUserMailboxObject;
			}
			else if (owaStoreObjectIdString.StartsWith("CID.", StringComparison.Ordinal))
			{
				owaStoreObjectIdType = OwaStoreObjectIdType.Conversation;
			}
			else if (owaStoreObjectIdString.StartsWith("AMB.", StringComparison.Ordinal))
			{
				owaStoreObjectIdType = OwaStoreObjectIdType.ArchiveMailboxObject;
			}
			else if (owaStoreObjectIdString.StartsWith("ACI.", StringComparison.Ordinal))
			{
				owaStoreObjectIdType = OwaStoreObjectIdType.ArchiveConversation;
			}
			else if (owaStoreObjectIdString.StartsWith("GS.", StringComparison.Ordinal))
			{
				owaStoreObjectIdType = OwaStoreObjectIdType.GSCalendar;
			}
			StoreObjectId folderStoreObjectId = null;
			StoreObjectId folderStoreObjectId2 = null;
			string text = null;
			switch (owaStoreObjectIdType)
			{
			case OwaStoreObjectIdType.PublicStoreFolder:
				folderStoreObjectId = Utilities.CreateStoreObjectId(owaStoreObjectIdString.Substring("PSF".Length + 1));
				OwaStoreObjectId.ValidateFolderId(folderStoreObjectId);
				break;
			case OwaStoreObjectIdType.PublicStoreItem:
			{
				int num = owaStoreObjectIdString.LastIndexOf(".", StringComparison.Ordinal);
				if (num == "PSI".Length)
				{
					throw new OwaInvalidIdFormatException(string.Format("There should be two separator \"{0}\" in the id of the public item. Invalid Id string: {1}", ".", owaStoreObjectIdString));
				}
				folderStoreObjectId2 = Utilities.CreateStoreObjectId(owaStoreObjectIdString.Substring("PSI".Length + 1, num - "PSI".Length - 1));
				folderStoreObjectId = Utilities.CreateStoreObjectId(owaStoreObjectIdString.Substring(num + 1));
				OwaStoreObjectId.ValidateFolderId(folderStoreObjectId2);
				break;
			}
			case OwaStoreObjectIdType.Conversation:
			{
				string[] array = owaStoreObjectIdString.Split(new char[]
				{
					"."[0]
				});
				OwaStoreObjectId owaStoreObjectId;
				if (array.Length == 4)
				{
					owaStoreObjectId = new OwaStoreObjectId(ConversationId.Create(array[1]), string.IsNullOrEmpty(array[2]) ? null : Utilities.CreateStoreObjectId(array[2]), string.IsNullOrEmpty(array[3]) ? null : Utilities.CreateInstanceKey(array[3]));
				}
				else if (array.Length == 3)
				{
					owaStoreObjectId = new OwaStoreObjectId(ConversationId.Create(array[1]), Utilities.CreateStoreObjectId(array[2]), null);
				}
				else
				{
					if (array.Length != 2)
					{
						throw new OwaInvalidRequestException(string.Format("There should be one or two separator \"{0}\" in the id of the conversation item", "."));
					}
					owaStoreObjectId = new OwaStoreObjectId(ConversationId.Create(array[1]), null, null);
				}
				owaStoreObjectId.bufferedIdString = owaStoreObjectIdString;
				return owaStoreObjectId;
			}
			case OwaStoreObjectIdType.OtherUserMailboxObject:
			{
				int num = owaStoreObjectIdString.LastIndexOf(".", StringComparison.Ordinal);
				folderStoreObjectId = OwaStoreObjectId.CreateStoreObjectIdFromString(owaStoreObjectIdString, "OUM", num);
				text = OwaStoreObjectId.ParseLegacyDnBase64String(owaStoreObjectIdString.Substring(num + 1));
				break;
			}
			case OwaStoreObjectIdType.ArchiveMailboxObject:
			{
				int num = owaStoreObjectIdString.LastIndexOf(".", StringComparison.Ordinal);
				folderStoreObjectId = OwaStoreObjectId.CreateStoreObjectIdFromString(owaStoreObjectIdString, "AMB", num);
				text = OwaStoreObjectId.ParseLegacyDnBase64String(owaStoreObjectIdString.Substring(num + 1));
				break;
			}
			case OwaStoreObjectIdType.ArchiveConversation:
			{
				string[] array = owaStoreObjectIdString.Split(new char[]
				{
					"."[0]
				});
				OwaStoreObjectId owaStoreObjectId;
				if (array.Length == 5)
				{
					text = OwaStoreObjectId.ParseLegacyDnBase64String(array[3]);
					owaStoreObjectId = new OwaStoreObjectId(ConversationId.Create(array[1]), string.IsNullOrEmpty(array[2]) ? null : Utilities.CreateStoreObjectId(array[2]), text, string.IsNullOrEmpty(array[4]) ? null : Utilities.CreateInstanceKey(array[4]));
				}
				else if (array.Length == 4)
				{
					text = OwaStoreObjectId.ParseLegacyDnBase64String(array[3]);
					owaStoreObjectId = new OwaStoreObjectId(ConversationId.Create(array[1]), Utilities.CreateStoreObjectId(array[2]), text, null);
				}
				else
				{
					if (array.Length != 3)
					{
						throw new OwaInvalidRequestException(string.Format("There should be two or three separator \"{0}\" in the id of the archive conversation item", "."));
					}
					text = OwaStoreObjectId.ParseLegacyDnBase64String(array[2]);
					owaStoreObjectId = new OwaStoreObjectId(ConversationId.Create(array[1]), null, text, null);
				}
				owaStoreObjectId.bufferedIdString = owaStoreObjectIdString;
				return owaStoreObjectId;
			}
			case OwaStoreObjectIdType.GSCalendar:
			{
				string[] array = owaStoreObjectIdString.Split(new char[]
				{
					"."[0]
				});
				if (array.Length != 2)
				{
					throw new OwaInvalidRequestException(string.Format("There should be two separator \"{0}\" in the id of the GS calendar item", "."));
				}
				text = OwaStoreObjectId.ParseLegacyDnBase64String(array[1]);
				break;
			}
			default:
				return OwaStoreObjectId.CreateFromStoreObjectId(Utilities.CreateStoreObjectId(owaStoreObjectIdString), null);
			}
			return new OwaStoreObjectId(owaStoreObjectIdType, folderStoreObjectId, folderStoreObjectId2, text)
			{
				bufferedIdString = owaStoreObjectIdString
			};
		}

		public static OwaStoreObjectId CreateFromItemId(StoreObjectId itemStoreObjectId, StoreObjectId containerFolderId, OwaStoreObjectIdType objectIdType, string legacyDN)
		{
			if (itemStoreObjectId == null)
			{
				throw new ArgumentNullException("itemStoreObjectId");
			}
			if (objectIdType == OwaStoreObjectIdType.PublicStoreItem && containerFolderId == null)
			{
				throw new ArgumentNullException("containerFolderId");
			}
			if (objectIdType == OwaStoreObjectIdType.OtherUserMailboxObject && string.IsNullOrEmpty(legacyDN))
			{
				throw new ArgumentNullException("legacyDN");
			}
			if (objectIdType == OwaStoreObjectIdType.ArchiveMailboxObject && string.IsNullOrEmpty(legacyDN))
			{
				throw new ArgumentNullException("legacyDN");
			}
			return new OwaStoreObjectId(objectIdType, itemStoreObjectId, containerFolderId, legacyDN);
		}

		public ConversationId ConversationId
		{
			get
			{
				return this.storeObjectId as ConversationId;
			}
		}

		public byte[] InstanceKey
		{
			get
			{
				return this.instanceKey;
			}
		}

		public StoreId StoreId
		{
			get
			{
				return this.storeObjectId;
			}
		}

		public StoreObjectId StoreObjectId
		{
			get
			{
				if (this.IsGSCalendar)
				{
					throw new OwaInvalidOperationException("GS calendar doesn't has a store object id.");
				}
				return this.storeObjectId as StoreObjectId;
			}
		}

		public bool IsStoreObjectId
		{
			get
			{
				return this.storeObjectId is StoreObjectId;
			}
		}

		public bool IsConversationId
		{
			get
			{
				return this.storeObjectId is ConversationId;
			}
		}

		public StoreObjectType StoreObjectType
		{
			get
			{
				StoreObjectId storeObjectId = this.storeObjectId as StoreObjectId;
				if (storeObjectId != null)
				{
					return storeObjectId.ObjectType;
				}
				return StoreObjectType.Unknown;
			}
		}

		public OwaStoreObjectIdType OwaStoreObjectIdType
		{
			get
			{
				return this.objectIdType;
			}
		}

		public string MailboxOwnerLegacyDN
		{
			get
			{
				return this.mailboxOwnerLegacyDN;
			}
		}

		public bool IsPublic
		{
			get
			{
				return this.objectIdType == OwaStoreObjectIdType.PublicStoreFolder || this.objectIdType == OwaStoreObjectIdType.PublicStoreItem;
			}
		}

		public bool IsOtherMailbox
		{
			get
			{
				return this.objectIdType == OwaStoreObjectIdType.OtherUserMailboxObject;
			}
		}

		public bool IsGSCalendar
		{
			get
			{
				return this.objectIdType == OwaStoreObjectIdType.GSCalendar;
			}
		}

		public bool IsArchive
		{
			get
			{
				return this.objectIdType == OwaStoreObjectIdType.ArchiveConversation || this.objectIdType == OwaStoreObjectIdType.ArchiveMailboxObject;
			}
		}

		public OwaStoreObjectId ProviderLevelItemId
		{
			get
			{
				StoreObjectId storeObjectId = StoreObjectId.FromProviderSpecificId(this.StoreObjectId.ProviderLevelItemId);
				return new OwaStoreObjectId(this.objectIdType, storeObjectId, this.containerFolderId, this.mailboxOwnerLegacyDN);
			}
		}

		public StoreObjectId ParentFolderId
		{
			get
			{
				return this.containerFolderId;
			}
		}

		public StoreSession GetSession(UserContext userContext)
		{
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			OwaStoreObjectIdSessionHandle owaStoreObjectIdSessionHandle = new OwaStoreObjectIdSessionHandle(this, userContext);
			StoreSession result;
			try
			{
				StoreSession session = owaStoreObjectIdSessionHandle.Session;
				userContext.AddSessionHandle(owaStoreObjectIdSessionHandle);
				result = session;
			}
			catch (Exception)
			{
				owaStoreObjectIdSessionHandle.Dispose();
				throw;
			}
			return result;
		}

		public StoreSession GetSessionForFolderContent(UserContext userContext)
		{
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			OwaStoreObjectIdSessionHandle owaStoreObjectIdSessionHandle = new OwaStoreObjectIdSessionHandle(this, userContext);
			StoreSession result;
			try
			{
				StoreSession sessionForFolderContent = owaStoreObjectIdSessionHandle.SessionForFolderContent;
				userContext.AddSessionHandle(owaStoreObjectIdSessionHandle);
				result = sessionForFolderContent;
			}
			catch (Exception)
			{
				owaStoreObjectIdSessionHandle.Dispose();
				throw;
			}
			return result;
		}

		public string ToBase64String()
		{
			return this.ToString();
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			OwaStoreObjectId owaStoreObjectId = obj as OwaStoreObjectId;
			if (owaStoreObjectId == null)
			{
				return false;
			}
			if (this.objectIdType != owaStoreObjectId.objectIdType)
			{
				return false;
			}
			if (this.objectIdType == OwaStoreObjectIdType.PublicStoreItem)
			{
				return this.StoreObjectId.Equals(owaStoreObjectId.StoreObjectId) && this.containerFolderId.Equals(owaStoreObjectId.containerFolderId);
			}
			if (this.objectIdType == OwaStoreObjectIdType.OtherUserMailboxObject || this.objectIdType == OwaStoreObjectIdType.ArchiveMailboxObject)
			{
				return this.StoreObjectId.Equals(owaStoreObjectId.StoreObjectId) && this.mailboxOwnerLegacyDN.Equals(owaStoreObjectId.MailboxOwnerLegacyDN, StringComparison.OrdinalIgnoreCase);
			}
			if (this.objectIdType == OwaStoreObjectIdType.GSCalendar)
			{
				return this.mailboxOwnerLegacyDN.Equals(owaStoreObjectId.MailboxOwnerLegacyDN, StringComparison.OrdinalIgnoreCase);
			}
			return this.StoreObjectId.Equals(owaStoreObjectId.StoreObjectId);
		}

		public override int GetHashCode()
		{
			if (this.IsGSCalendar)
			{
				return this.MailboxOwnerLegacyDN.ToLowerInvariant().GetHashCode();
			}
			return this.StoreId.GetHashCode();
		}

		public override byte[] GetBytes()
		{
			if (this.IsGSCalendar)
			{
				return Encoding.Unicode.GetBytes(this.MailboxOwnerLegacyDN);
			}
			return this.StoreId.GetBytes();
		}

		public override string ToString()
		{
			if (this.bufferedIdString == null)
			{
				switch (this.objectIdType)
				{
				case OwaStoreObjectIdType.MailBoxObject:
					this.bufferedIdString = this.storeObjectId.ToBase64String();
					break;
				case OwaStoreObjectIdType.PublicStoreFolder:
					this.bufferedIdString = this.CreatePublicStoreFolderId();
					break;
				case OwaStoreObjectIdType.PublicStoreItem:
					this.bufferedIdString = this.CreatePublicStoreItemId();
					break;
				case OwaStoreObjectIdType.Conversation:
					this.bufferedIdString = this.CreateConversationId();
					break;
				case OwaStoreObjectIdType.OtherUserMailboxObject:
					this.bufferedIdString = this.CreateOtherUserMailboxStoreItemId();
					break;
				case OwaStoreObjectIdType.ArchiveMailboxObject:
					this.bufferedIdString = this.CreateArchiveMailboxStoreObjectId();
					break;
				case OwaStoreObjectIdType.ArchiveConversation:
					this.bufferedIdString = this.CreateArchiveConversationId();
					break;
				case OwaStoreObjectIdType.GSCalendar:
					this.bufferedIdString = this.CreateGSCalendarId();
					break;
				}
			}
			return this.bufferedIdString;
		}

		internal static OwaStoreObjectId CreateFromConversationId(ConversationId conversationId, StoreObject folder)
		{
			return OwaStoreObjectId.CreateFromConversationId(conversationId, folder, null);
		}

		internal static OwaStoreObjectId CreateFromConversationId(ConversationId conversationId, StoreObject folder, byte[] instanceKey)
		{
			if (conversationId == null)
			{
				throw new ArgumentNullException("conversationId");
			}
			if (folder == null)
			{
				throw new ArgumentNullException("folder");
			}
			if (Utilities.IsOtherMailbox(folder))
			{
				throw new NotSupportedException("conversation should not from other user's folder");
			}
			if (Utilities.IsInArchiveMailbox(folder))
			{
				return new OwaStoreObjectId(conversationId, folder.Id.ObjectId, Utilities.GetMailboxSessionLegacyDN(folder), instanceKey);
			}
			return new OwaStoreObjectId(conversationId, folder.Id.ObjectId, instanceKey);
		}

		internal static OwaStoreObjectId CreateFromConversationIdForListViewNotification(ConversationId conversationId, StoreObjectId folderId, byte[] instanceKey)
		{
			if (conversationId == null)
			{
				throw new ArgumentNullException("conversationId");
			}
			if (folderId == null)
			{
				throw new ArgumentNullException("folder");
			}
			return new OwaStoreObjectId(conversationId, folderId, instanceKey);
		}

		internal static OwaStoreObjectId CreateFromPublicFolderId(StoreObjectId publicStoreFolderId)
		{
			if (publicStoreFolderId == null)
			{
				throw new ArgumentNullException("publicStoreFolderId");
			}
			return OwaStoreObjectId.CreateFromFolderId(publicStoreFolderId, OwaStoreObjectIdType.PublicStoreFolder);
		}

		internal static OwaStoreObjectId CreateFromPublicStoreItemId(StoreObjectId publicStoreItemId, StoreObjectId publicStoreFolderId)
		{
			if (publicStoreItemId == null)
			{
				throw new ArgumentNullException("publicStoreItemId");
			}
			if (publicStoreFolderId == null)
			{
				throw new ArgumentNullException("publicStoreFolderId");
			}
			return OwaStoreObjectId.CreateFromItemId(publicStoreItemId, publicStoreFolderId, OwaStoreObjectIdType.PublicStoreItem, null);
		}

		internal static OwaStoreObjectId CreateFromMailboxFolderId(StoreObjectId mailboxFolderStoreObjectId)
		{
			if (mailboxFolderStoreObjectId == null)
			{
				throw new ArgumentNullException("mailboxFolderStoreObjectId");
			}
			return OwaStoreObjectId.CreateFromFolderId(mailboxFolderStoreObjectId, OwaStoreObjectIdType.MailBoxObject);
		}

		internal static OwaStoreObjectId CreateFromOtherUserMailboxFolderId(StoreObjectId otherMailboxFolderStoreObjectId, string legacyDN)
		{
			if (otherMailboxFolderStoreObjectId == null)
			{
				throw new ArgumentNullException("otherMailboxFolderStoreObjectId");
			}
			return OwaStoreObjectId.CreateFromFolderId(otherMailboxFolderStoreObjectId, OwaStoreObjectIdType.OtherUserMailboxObject, legacyDN);
		}

		internal static OwaStoreObjectId CreateFromArchiveMailboxFolderId(StoreObjectId archiveMailboxFolderStoreObjectId, string legacyDN)
		{
			if (archiveMailboxFolderStoreObjectId == null)
			{
				throw new ArgumentNullException("archiveMailboxFolderStoreObjectId");
			}
			if (string.IsNullOrEmpty(legacyDN))
			{
				throw new ArgumentNullException("legacyDN");
			}
			return OwaStoreObjectId.CreateFromFolderId(archiveMailboxFolderStoreObjectId, OwaStoreObjectIdType.ArchiveMailboxObject, legacyDN);
		}

		internal static OwaStoreObjectId CreateFromGSCalendarLegacyDN(string legacyDN)
		{
			if (string.IsNullOrEmpty(legacyDN))
			{
				throw new ArgumentNullException("legacyDN");
			}
			return new OwaStoreObjectId(OwaStoreObjectIdType.GSCalendar, null, legacyDN);
		}

		internal static OwaStoreObjectId CreateFromFolderId(StoreObjectId folderStoreObjectId, OwaStoreObjectIdType objectIdType, string legacyDN)
		{
			if (folderStoreObjectId == null)
			{
				throw new ArgumentNullException("folderStoreObjectId");
			}
			return new OwaStoreObjectId(objectIdType, folderStoreObjectId, legacyDN);
		}

		internal static OwaStoreObjectId CreateFromFolderId(StoreObjectId folderStoreObjectId, OwaStoreObjectIdType objectIdType)
		{
			if (folderStoreObjectId == null)
			{
				throw new ArgumentNullException("folderStoreObjectId");
			}
			if (objectIdType == OwaStoreObjectIdType.OtherUserMailboxObject)
			{
				throw new OwaInvalidOperationException("Mailbox legacy DN is required for other user mailbox");
			}
			return new OwaStoreObjectId(objectIdType, folderStoreObjectId);
		}

		internal static bool IsDummyArchiveFolder(string owaStoreObjectIdString)
		{
			if (!owaStoreObjectIdString.StartsWith("AMB.", StringComparison.Ordinal))
			{
				return false;
			}
			int num = owaStoreObjectIdString.LastIndexOf(".", StringComparison.Ordinal);
			string strA = owaStoreObjectIdString.Substring("AMB".Length + 1, num - "AMB".Length - 1);
			return string.Compare(strA, StoreObjectId.DummyId.ToString(), StringComparison.Ordinal) == 0;
		}

		private static StoreObjectId CreateStoreObjectIdFromString(string owaStoreObjectIdString, string itemPrefix, int indexOfLastSeparatorChar)
		{
			if (indexOfLastSeparatorChar == itemPrefix.Length)
			{
				throw new OwaInvalidIdFormatException(string.Format("There should be two separators \"{0}\" in the id of the public item. Invalid Id: {1}", ".", owaStoreObjectIdString));
			}
			return Utilities.CreateStoreObjectId(owaStoreObjectIdString.Substring(itemPrefix.Length + 1, indexOfLastSeparatorChar - itemPrefix.Length - 1));
		}

		private static void ValidateFolderId(StoreObjectId folderStoreObjectId)
		{
			if (!Folder.IsFolderId(folderStoreObjectId))
			{
				throw new OwaInvalidIdFormatException(string.Format("Invalid folder id: {0}", folderStoreObjectId.ToBase64String()));
			}
		}

		private static string ParseLegacyDnBase64String(string legacyDnBase64String)
		{
			string text = null;
			try
			{
				text = Encoding.UTF8.GetString(Convert.FromBase64String(legacyDnBase64String));
			}
			catch (ArgumentException innerException)
			{
				throw new OwaInvalidIdFormatException(string.Format("Invalid legacyDN. Invalid Id string: {0}", legacyDnBase64String), innerException);
			}
			catch (FormatException innerException2)
			{
				throw new OwaInvalidIdFormatException(string.Format("Invalid legacyDN. Invalid Id string: {0}", legacyDnBase64String), innerException2);
			}
			if (string.IsNullOrEmpty(text) || !Utilities.IsValidLegacyDN(text))
			{
				throw new OwaInvalidIdFormatException(string.Format("Invalid legacy distinguished name: {0}", (text == null) ? "null" : text));
			}
			return text;
		}

		private string NormalizedContainerFolderIdBase64String
		{
			get
			{
				byte[] bytes = this.containerFolderId.GetBytes();
				bytes[bytes.Length - 1] = 1;
				return Convert.ToBase64String(bytes);
			}
		}

		private string NormalizedMailboxOwnerLegacyDnBase64String
		{
			get
			{
				byte[] bytes = Encoding.UTF8.GetBytes(this.mailboxOwnerLegacyDN);
				return Convert.ToBase64String(bytes);
			}
		}

		private string CreatePublicStoreFolderId()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("PSF");
			stringBuilder.Append(".");
			stringBuilder.Append(this.storeObjectId.ToBase64String());
			return stringBuilder.ToString();
		}

		private string CreatePublicStoreItemId()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("PSI");
			stringBuilder.Append(".");
			stringBuilder.Append(this.NormalizedContainerFolderIdBase64String);
			stringBuilder.Append(".");
			stringBuilder.Append(this.storeObjectId.ToBase64String());
			return stringBuilder.ToString();
		}

		private string CreateConversationId()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("CID");
			stringBuilder.Append(".");
			stringBuilder.Append(this.storeObjectId.ToBase64String());
			stringBuilder.Append(".");
			if (this.containerFolderId != null)
			{
				stringBuilder.Append(this.NormalizedContainerFolderIdBase64String);
			}
			stringBuilder.Append(".");
			if (this.instanceKey != null)
			{
				stringBuilder.Append(Convert.ToBase64String(this.instanceKey));
			}
			return stringBuilder.ToString();
		}

		private string CreateOtherUserMailboxStoreItemId()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("OUM");
			stringBuilder.Append(".");
			stringBuilder.Append(this.storeObjectId.ToBase64String());
			stringBuilder.Append(".");
			stringBuilder.Append(this.NormalizedMailboxOwnerLegacyDnBase64String);
			return stringBuilder.ToString();
		}

		private string CreateArchiveMailboxStoreObjectId()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("AMB");
			stringBuilder.Append(".");
			stringBuilder.Append(this.storeObjectId.ToBase64String());
			stringBuilder.Append(".");
			stringBuilder.Append(this.NormalizedMailboxOwnerLegacyDnBase64String);
			return stringBuilder.ToString();
		}

		private string CreateArchiveConversationId()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("ACI");
			stringBuilder.Append(".");
			stringBuilder.Append(this.storeObjectId.ToBase64String());
			stringBuilder.Append(".");
			if (this.containerFolderId != null)
			{
				stringBuilder.Append(this.NormalizedContainerFolderIdBase64String);
			}
			stringBuilder.Append(".");
			stringBuilder.Append(this.NormalizedMailboxOwnerLegacyDnBase64String);
			stringBuilder.Append(".");
			if (this.instanceKey != null)
			{
				stringBuilder.Append(Convert.ToBase64String(this.instanceKey));
			}
			return stringBuilder.ToString();
		}

		private string CreateGSCalendarId()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("GS");
			stringBuilder.Append(".");
			stringBuilder.Append(this.NormalizedMailboxOwnerLegacyDnBase64String);
			return stringBuilder.ToString();
		}

		private const string PublicStoreFolderPrefix = "PSF";

		private const string PublicStoreItemPrefix = "PSI";

		private const string ConversationIdPrefix = "CID";

		private const string OtherUserMailboxObjectPrefix = "OUM";

		private const string ArchiveMailBoxObjectPrefix = "AMB";

		private const string ArchiveConversationIdPrefix = "ACI";

		private const string GSCalendarPrefix = "GS";

		private const string SeparatorChar = ".";

		private OwaStoreObjectIdType objectIdType;

		private StoreId storeObjectId;

		private StoreObjectId containerFolderId;

		private byte[] instanceKey;

		private string bufferedIdString;

		private string mailboxOwnerLegacyDN;
	}
}
