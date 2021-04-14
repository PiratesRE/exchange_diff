using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.AnchorService.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class AnchorMessageItem : AnchorStoreObject, IAnchorMessageItem, IAnchorStoreObject, IDisposable, IPropertyBag, IReadOnlyPropertyBag, IAnchorAttachmentMessage
	{
		internal AnchorMessageItem(AnchorContext context, MessageItem message)
		{
			base.AnchorContext = context;
			this.Message = message;
		}

		internal AnchorMessageItem(AnchorContext context, MailboxSession mailboxSession, StoreObjectId id)
		{
			base.AnchorContext = context;
			this.Initialize(mailboxSession, id, AnchorStoreObject.IdPropertyDefinition);
		}

		internal AnchorMessageItem(MailboxSession mailboxSession, StoreObjectId id, PropertyDefinition[] propertyDefinitions)
		{
			this.Initialize(mailboxSession, id, propertyDefinitions);
		}

		public override string Name
		{
			get
			{
				return this.Message.ClassName;
			}
		}

		protected override StoreObject StoreObject
		{
			get
			{
				return this.Message;
			}
		}

		private MessageItem Message { get; set; }

		public override void OpenAsReadWrite()
		{
			this.Message.OpenAsReadWrite();
		}

		public override void Save(SaveMode saveMode)
		{
			this.Message.Save(saveMode);
		}

		public AnchorAttachment CreateAttachment(string name)
		{
			base.CheckDisposed();
			return AnchorMessageHelper.CreateAttachment(base.AnchorContext, this.Message, name);
		}

		public AnchorAttachment GetAttachment(string name, PropertyOpenMode openMode)
		{
			base.CheckDisposed();
			return AnchorMessageHelper.GetAttachment(base.AnchorContext, this.Message, name, openMode);
		}

		public void DeleteAttachment(string name)
		{
			base.CheckDisposed();
			AnchorMessageHelper.DeleteAttachment(this.Message, name);
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing && this.Message != null)
			{
				this.Message.Dispose();
				this.Message = null;
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<AnchorMessageItem>(this);
		}

		private void Initialize(MailboxSession mailboxSession, StoreObjectId id, PropertyDefinition[] properties)
		{
			bool flag = false;
			try
			{
				AnchorUtil.ThrowOnNullArgument(mailboxSession, "dataProvider");
				AnchorUtil.ThrowOnNullArgument(id, "id");
				AnchorUtil.ThrowOnNullArgument(properties, "properties");
				this.Message = MessageItem.Bind(mailboxSession, id, properties);
				flag = true;
			}
			catch (ArgumentException ex)
			{
				base.AnchorContext.Logger.Log(MigrationEventType.Error, ex, "Encountered an argument exception when trying to find message with id={0}", new object[]
				{
					id.ToString()
				});
				throw new ObjectNotFoundException(ServerStrings.ExItemNotFound, ex);
			}
			finally
			{
				if (!flag)
				{
					this.Dispose();
				}
			}
		}
	}
}
