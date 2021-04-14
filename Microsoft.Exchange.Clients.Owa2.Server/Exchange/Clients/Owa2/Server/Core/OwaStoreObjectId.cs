using System;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[Serializable]
	public class OwaStoreObjectId : ObjectId
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

		internal ConversationId ConversationId
		{
			get
			{
				return this.storeObjectId as ConversationId;
			}
		}

		internal byte[] InstanceKey
		{
			get
			{
				return this.instanceKey;
			}
		}

		internal StoreId StoreId
		{
			get
			{
				return this.storeObjectId;
			}
		}

		internal StoreObjectId StoreObjectId
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

		internal bool IsStoreObjectId
		{
			get
			{
				return this.storeObjectId is StoreObjectId;
			}
		}

		internal bool IsConversationId
		{
			get
			{
				return this.storeObjectId is ConversationId;
			}
		}

		internal StoreObjectType StoreObjectType
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

		internal OwaStoreObjectIdType OwaStoreObjectIdType
		{
			get
			{
				return this.objectIdType;
			}
		}

		internal string MailboxOwnerLegacyDN
		{
			get
			{
				return this.mailboxOwnerLegacyDN;
			}
		}

		internal bool IsPublic
		{
			get
			{
				return this.objectIdType == OwaStoreObjectIdType.PublicStoreFolder || this.objectIdType == OwaStoreObjectIdType.PublicStoreItem;
			}
		}

		internal bool IsOtherMailbox
		{
			get
			{
				return this.objectIdType == OwaStoreObjectIdType.OtherUserMailboxObject;
			}
		}

		internal bool IsGSCalendar
		{
			get
			{
				return this.objectIdType == OwaStoreObjectIdType.GSCalendar;
			}
		}

		internal bool IsArchive
		{
			get
			{
				return this.objectIdType == OwaStoreObjectIdType.ArchiveConversation || this.objectIdType == OwaStoreObjectIdType.ArchiveMailboxObject;
			}
		}

		internal OwaStoreObjectId ProviderLevelItemId
		{
			get
			{
				StoreObjectId storeObjectId = StoreObjectId.FromProviderSpecificId(this.StoreObjectId.ProviderLevelItemId);
				return new OwaStoreObjectId(this.objectIdType, storeObjectId, this.containerFolderId, this.mailboxOwnerLegacyDN);
			}
		}

		internal StoreObjectId ParentFolderId
		{
			get
			{
				return this.containerFolderId;
			}
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

		public override byte[] GetBytes()
		{
			if (this.IsGSCalendar)
			{
				return Encoding.Unicode.GetBytes(this.MailboxOwnerLegacyDN);
			}
			return this.StoreId.GetBytes();
		}

		internal static StoreObjectId[] ConvertToStoreObjectIdArray(params OwaStoreObjectId[] owaStoreObjectIds)
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

		internal static OwaStoreObjectId CreateFromString(string owaStoreObjectIdString)
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
				folderStoreObjectId = OwaStoreObjectId.CreateStoreObjectId(owaStoreObjectIdString.Substring("PSF".Length + 1));
				OwaStoreObjectId.ValidateFolderId(folderStoreObjectId);
				break;
			case OwaStoreObjectIdType.PublicStoreItem:
			{
				int num = owaStoreObjectIdString.LastIndexOf(".", StringComparison.Ordinal);
				if (num == "PSI".Length)
				{
					throw new OwaInvalidIdFormatException(string.Format("There should be two separator \"{0}\" in the id of the public item. Invalid Id string: {1}", ".", owaStoreObjectIdString));
				}
				folderStoreObjectId2 = OwaStoreObjectId.CreateStoreObjectId(owaStoreObjectIdString.Substring("PSI".Length + 1, num - "PSI".Length - 1));
				folderStoreObjectId = OwaStoreObjectId.CreateStoreObjectId(owaStoreObjectIdString.Substring(num + 1));
				OwaStoreObjectId.ValidateFolderId(folderStoreObjectId2);
				break;
			}
			case OwaStoreObjectIdType.Conversation:
			{
				string[] array = owaStoreObjectIdString.Split(new char[]
				{
					"."[0]
				});
				OwaStoreObjectId result;
				if (array.Length == 4)
				{
					result = new OwaStoreObjectId(ConversationId.Create(array[1]), string.IsNullOrEmpty(array[2]) ? null : OwaStoreObjectId.CreateStoreObjectId(array[2]), string.IsNullOrEmpty(array[3]) ? null : OwaStoreObjectId.CreateInstanceKey(array[3]));
				}
				else if (array.Length == 3)
				{
					result = new OwaStoreObjectId(ConversationId.Create(array[1]), OwaStoreObjectId.CreateStoreObjectId(array[2]), null);
				}
				else
				{
					if (array.Length != 2)
					{
						throw new OwaInvalidRequestException(string.Format("There should be one or two separator \"{0}\" in the id of the conversation item", "."));
					}
					result = new OwaStoreObjectId(ConversationId.Create(array[1]), null, null);
				}
				return result;
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
				OwaStoreObjectId result;
				if (array.Length == 5)
				{
					text = OwaStoreObjectId.ParseLegacyDnBase64String(array[3]);
					result = new OwaStoreObjectId(ConversationId.Create(array[1]), string.IsNullOrEmpty(array[2]) ? null : OwaStoreObjectId.CreateStoreObjectId(array[2]), text, string.IsNullOrEmpty(array[4]) ? null : OwaStoreObjectId.CreateInstanceKey(array[4]));
				}
				else if (array.Length == 4)
				{
					text = OwaStoreObjectId.ParseLegacyDnBase64String(array[3]);
					result = new OwaStoreObjectId(ConversationId.Create(array[1]), OwaStoreObjectId.CreateStoreObjectId(array[2]), text, null);
				}
				else
				{
					if (array.Length != 3)
					{
						throw new OwaInvalidRequestException(string.Format("There should be two or three separator \"{0}\" in the id of the archive conversation item", "."));
					}
					text = OwaStoreObjectId.ParseLegacyDnBase64String(array[2]);
					result = new OwaStoreObjectId(ConversationId.Create(array[1]), null, text, null);
				}
				return result;
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
				return OwaStoreObjectId.CreateFromStoreObjectId(OwaStoreObjectId.CreateStoreObjectId(owaStoreObjectIdString), null);
			}
			return new OwaStoreObjectId(owaStoreObjectIdType, folderStoreObjectId, folderStoreObjectId2, text);
		}

		internal static OwaStoreObjectId CreateFromStoreObjectId(StoreObjectId storeObjectId, OwaStoreObjectId relatedStoreObjectId)
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

		internal string ToBase64String()
		{
			return this.ToString();
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
			if (string.IsNullOrEmpty(text) || !OwaStoreObjectId.IsValidLegacyDN(text))
			{
				throw new OwaInvalidIdFormatException(string.Format("Invalid legacy distinguished name: {0}", (text == null) ? "null" : text));
			}
			return text;
		}

		private static StoreObjectId CreateStoreObjectIdFromString(string owaStoreObjectIdString, string itemPrefix, int indexOfLastSeparatorChar)
		{
			if (indexOfLastSeparatorChar == itemPrefix.Length)
			{
				throw new OwaInvalidIdFormatException(string.Format("There should be two separators \"{0}\" in the id of the public item. Invalid Id: {1}", ".", owaStoreObjectIdString));
			}
			return OwaStoreObjectId.CreateStoreObjectId(owaStoreObjectIdString.Substring(itemPrefix.Length + 1, indexOfLastSeparatorChar - itemPrefix.Length - 1));
		}

		private static StoreObjectId CreateStoreObjectId(string storeObjectIdString)
		{
			if (storeObjectIdString == null)
			{
				throw new ArgumentNullException("storeObjectIdString");
			}
			StoreObjectId result = null;
			try
			{
				result = StoreObjectId.Deserialize(storeObjectIdString);
			}
			catch (ArgumentException innerException)
			{
				OwaStoreObjectId.ThrowInvalidIdFormatException(storeObjectIdString, null, innerException);
			}
			catch (FormatException innerException2)
			{
				OwaStoreObjectId.ThrowInvalidIdFormatException(storeObjectIdString, null, innerException2);
			}
			catch (CorruptDataException innerException3)
			{
				OwaStoreObjectId.ThrowInvalidIdFormatException(storeObjectIdString, null, innerException3);
			}
			return result;
		}

		private static byte[] CreateInstanceKey(string instanceKeyString)
		{
			if (instanceKeyString == null)
			{
				throw new ArgumentNullException("instanceKeyString");
			}
			byte[] result = null;
			try
			{
				result = Convert.FromBase64String(instanceKeyString);
			}
			catch (ArgumentException innerException)
			{
				OwaStoreObjectId.ThrowInvalidIdFormatException(instanceKeyString, null, innerException);
			}
			catch (FormatException innerException2)
			{
				OwaStoreObjectId.ThrowInvalidIdFormatException(instanceKeyString, null, innerException2);
			}
			catch (CorruptDataException innerException3)
			{
				OwaStoreObjectId.ThrowInvalidIdFormatException(instanceKeyString, null, innerException3);
			}
			return result;
		}

		private static bool IsValidLegacyDN(string address)
		{
			if (string.IsNullOrEmpty(address))
			{
				throw new ArgumentNullException("address");
			}
			LegacyDN legacyDN;
			return LegacyDN.TryParse(address, out legacyDN);
		}

		private static void ThrowInvalidIdFormatException(string storeObjectId, string changeKey, Exception innerException)
		{
			throw new OwaInvalidIdFormatException(string.Format("Invalid id format. Store object id: {0}. Change key: {1}", (storeObjectId == null) ? "null" : storeObjectId, (changeKey == null) ? "null" : changeKey), innerException);
		}

		private const string PublicStoreFolderPrefix = "PSF";

		private const string PublicStoreItemPrefix = "PSI";

		private const string ConversationIdPrefix = "CID";

		private const string OtherUserMailboxObjectPrefix = "OUM";

		private const string ArchiveMailBoxObjectPrefix = "AMB";

		private const string ArchiveConversationIdPrefix = "ACI";

		private const string GSCalendarPrefix = "GS";

		private const string SeparatorChar = ".";

		private readonly string mailboxOwnerLegacyDN;

		private OwaStoreObjectIdType objectIdType;

		private StoreId storeObjectId;

		private StoreObjectId containerFolderId;

		private byte[] instanceKey;
	}
}
