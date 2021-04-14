using System;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.RpcClientAccess.Parser;

namespace Microsoft.Exchange.RpcClientAccess.Handler
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal abstract class SessionServerObject : ServerObject
	{
		protected SessionServerObject()
		{
		}

		protected SessionServerObject(Logon logon) : base(logon)
		{
		}

		public abstract StoreSession Session { get; }

		public Folder OpenFolder(StoreId folderId, OpenMode openMode, out bool hasRules, out ReplicaServerInfo? replicaServerInfo)
		{
			hasRules = false;
			replicaServerInfo = null;
			bool allowSoftDeleted = (byte)(openMode & OpenMode.OpenSoftDeleted) != 0;
			Folder result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				CoreFolder coreFolder = null;
				IdConverter idConverter = this.Session.IdConverter;
				PublicLogon publicLogon = base.LogonObject as PublicLogon;
				try
				{
					coreFolder = CoreFolder.Bind(this.Session, idConverter.CreateFolderId(folderId), allowSoftDeleted, null);
					disposeGuard.Add<CoreFolder>(coreFolder);
				}
				catch (AccessDeniedException ex)
				{
					throw new ObjectNotFoundException(ex.LocalizedString, ex.InnerException);
				}
				hasRules = coreFolder.PropertyBag.GetValueOrDefault<bool>(CoreFolderSchema.HasRules);
				if (publicLogon != null)
				{
					replicaServerInfo = PublicLogon.GetReplicaServerInfo(coreFolder, true);
				}
				Folder folder = new Folder(coreFolder, base.LogonObject);
				disposeGuard.Add<Folder>(folder);
				disposeGuard.Success();
				result = folder;
			}
			return result;
		}

		public Message OpenMessage(ushort codePageId, StoreId folderId, OpenMode openMode, StoreId messageId, Func<MessageHeader, PropertyTag[], Encoding, RecipientCollector> createRecipientCollectorDelegate, out RecipientCollector recipientCollector)
		{
			recipientCollector = null;
			if (openMode < OpenMode.ReadOnly || openMode >= (OpenMode)16)
			{
				throw new RopExecutionException("Invalid OpenMode.", (ErrorCode)2147746050U);
			}
			if ((byte)(openMode & OpenMode.OpenSoftDeleted) == 4)
			{
				Feature.Stubbed(176710, "Support opening SoftDeleted messages");
			}
			if ((byte)(openMode & OpenMode.BestAccess) != 3)
			{
				Feature.Stubbed(33084, "Currently XSO opens all store objects in BestAccess mode.");
			}
			Message result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				PublicLogon publicLogon = base.LogonObject as PublicLogon;
				PropertyDefinition[] propsToReturn = (publicLogon != null) ? SessionServerObject.PublicFolderMessageBindProperties : SessionServerObject.MessageBindProperties;
				CoreItem coreItem = CoreItem.Bind(this.Session, this.Session.IdConverter.CreateMessageId(folderId, messageId), propsToReturn);
				disposeGuard.Add<CoreItem>(coreItem);
				if (openMode != OpenMode.ReadOnly)
				{
					coreItem.OpenAsReadWrite();
				}
				Encoding string8Encoding;
				if (!String8Encodings.TryGetEncoding((int)codePageId, this.String8Encoding, out string8Encoding))
				{
					string message = string.Format("Cannot resolve code page: {0}", codePageId);
					throw new RopExecutionException(message, ErrorCode.UnknownCodepage);
				}
				Message message2 = new Message(coreItem, base.LogonObject, string8Encoding);
				disposeGuard.Add<Message>(message2);
				MessageHeader messageHeader = Message.GetMessageHeader(coreItem);
				RecipientCollector recipientCollector2 = createRecipientCollectorDelegate(messageHeader, message2.ExtraRecipientPropertyTags, String8Encodings.TemporaryDefault);
				disposeGuard.Add<RecipientCollector>(recipientCollector2);
				message2.AddRecipients(recipientCollector2);
				disposeGuard.Success();
				recipientCollector = recipientCollector2;
				result = message2;
			}
			return result;
		}

		private static readonly PropertyDefinition[] MessageBindProperties = new PropertyDefinition[]
		{
			CoreItemSchema.SubjectPrefix,
			CoreItemSchema.NormalizedSubject
		};

		private static readonly PropertyDefinition[] PublicFolderMessageBindProperties = new PropertyDefinition[]
		{
			CoreItemSchema.SubjectPrefix,
			CoreItemSchema.NormalizedSubject,
			CoreObjectSchema.ParentFid
		};
	}
}
