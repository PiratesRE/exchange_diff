using System;
using System.Globalization;
using System.Text;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal class ConversationItemsLogger
	{
		public ConversationItemsLogger(Enum conversationItemsMetadata, IdConverter idConverter, MailboxSession mailboxSession)
		{
			this.logger = RequestDetailsLogger.Current;
			this.idConverter = idConverter;
			this.conversationItemsMetadata = conversationItemsMetadata;
			this.mailboxSession = mailboxSession;
			this.bufferCreated = false;
		}

		public void CreateBufferLog()
		{
			if (this.bufferCreated)
			{
				throw new InvalidOperationException("Before creating a new bufferlog, you should save the one already created");
			}
			this.bufferCreated = true;
			this.firstItemLogged = false;
			string value = this.logger.Get(this.conversationItemsMetadata);
			this.itemLogBuilder = new StringBuilder(value);
			if (!string.IsNullOrEmpty(value))
			{
				this.itemLogBuilder.Append("|");
			}
		}

		public void AddToLog(ItemType item)
		{
			if (!this.bufferCreated)
			{
				throw new InvalidOperationException("Adding to log can happen only after creating the buffer");
			}
			if (this.firstItemLogged)
			{
				this.itemLogBuilder.Append("~");
			}
			StoreObjectId folderId = null;
			if (item.ParentFolderId != null)
			{
				IdAndSession idAndSession = this.idConverter.ConvertFolderIdToIdAndSessionReadOnly(item.ParentFolderId);
				folderId = idAndSession.GetAsStoreObjectId();
			}
			this.itemLogBuilder.AppendFormat(CultureInfo.InvariantCulture, "{0}/{1}/{2}", new object[]
			{
				(item.ItemId == null) ? "null" : (item.ItemId.Id + ":" + item.ItemId.ChangeKey),
				ServiceCommandBase.GetFolderLogString(folderId, this.mailboxSession),
				item.InstanceKeyString
			});
			this.firstItemLogged = true;
		}

		public void SaveBufferLog()
		{
			if (!this.bufferCreated)
			{
				throw new InvalidOperationException("You can only save a buffer log that was previoulsy created");
			}
			this.bufferCreated = false;
			RequestDetailsLoggerBase<RequestDetailsLogger>.SafeSetLogger(this.logger, this.conversationItemsMetadata, this.itemLogBuilder.ToString());
		}

		private readonly RequestDetailsLogger logger;

		private readonly Enum conversationItemsMetadata;

		private readonly IdConverter idConverter;

		private readonly MailboxSession mailboxSession;

		private bool firstItemLogged;

		private StringBuilder itemLogBuilder;

		private bool bufferCreated;
	}
}
