using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class PostItem : Item, IStorePropertyBag, IPropertyBag, IReadOnlyPropertyBag, IDisposable
	{
		internal PostItem(ICoreItem coreItem) : base(coreItem, false)
		{
		}

		public Participant Sender
		{
			get
			{
				this.CheckDisposed("Sender::get");
				return base.GetValueOrDefault<Participant>(InternalSchema.Sender);
			}
			set
			{
				this.CheckDisposed("Sender::set");
				base.SetOrDeleteProperty(InternalSchema.Sender, value);
			}
		}

		public Participant From
		{
			get
			{
				this.CheckDisposed("From::get");
				return base.GetValueOrDefault<Participant>(InternalSchema.From);
			}
			set
			{
				this.CheckDisposed("From::set");
				base.SetOrDeleteProperty(InternalSchema.From, value);
			}
		}

		public ExDateTime PostedTime
		{
			get
			{
				this.CheckDisposed("PostedTime::get");
				return base.GetValueOrDefault<ExDateTime>(InternalSchema.CreationTime, ExDateTime.MinValue);
			}
		}

		public string InternetMessageId
		{
			get
			{
				this.CheckDisposed("InternetMessageId::get");
				return base.GetValueOrDefault<string>(InternalSchema.InternetMessageId, string.Empty);
			}
			set
			{
				this.CheckDisposed("InternetMessageId::set");
				base.CheckSetNull("Message::InternetMessageId", "InternetMessageId", value);
				this[InternalSchema.InternetMessageId] = value;
			}
		}

		public byte[] ConversationIndex
		{
			get
			{
				this.CheckDisposed("ConversationIndex::get");
				byte[] array = base.GetValueOrDefault<byte[]>(InternalSchema.ConversationIndex);
				if (array == null)
				{
					array = Microsoft.Exchange.Data.Storage.ConversationIndex.CreateNew().ToByteArray();
					if (base.PropertyBag is AcrPropertyBag && !((AcrPropertyBag)base.PropertyBag).IsReadOnly)
					{
						this[InternalSchema.ConversationIndex] = array;
					}
				}
				return array;
			}
			set
			{
				this.CheckDisposed("ConversationIndex::set");
				base.CheckSetNull("Post::ConversationIndex", "ConversationIndex", value);
				this[InternalSchema.ConversationIndex] = value;
			}
		}

		public string ConversationTopic
		{
			get
			{
				this.CheckDisposed("ConversationTopic::get");
				string text = base.TryGetProperty(InternalSchema.ConversationTopic) as string;
				if (text == null)
				{
					text = base.GetValueOrDefault<string>(InternalSchema.NormalizedSubjectInternal, string.Empty);
					if (base.PropertyBag is AcrPropertyBag && !((AcrPropertyBag)base.PropertyBag).IsReadOnly)
					{
						this[InternalSchema.ConversationTopic] = text;
					}
				}
				return text;
			}
			set
			{
				this.CheckDisposed("ConversationTopic::set");
				base.CheckSetNull("Post::ConversationTopic", "ConversationTopic", value);
				this[InternalSchema.ConversationTopic] = value;
			}
		}

		public string InReplyTo
		{
			get
			{
				this.CheckDisposed("InReplyTo::get");
				return base.GetValueOrDefault<string>(InternalSchema.InReplyTo, string.Empty);
			}
			set
			{
				this.CheckDisposed("InReplyTo::set");
				base.CheckSetNull("Post::InReplyTo", "InReplyTo", value);
				this[InternalSchema.InReplyTo] = value;
			}
		}

		public string References
		{
			get
			{
				this.CheckDisposed("References::get");
				return base.GetValueOrDefault<string>(InternalSchema.InternetReferences, string.Empty);
			}
			set
			{
				this.CheckDisposed("References::set");
				base.CheckSetNull("Post::InternetReferences", "InternetReferences", value);
				this[InternalSchema.InternetReferences] = value;
			}
		}

		public override Schema Schema
		{
			get
			{
				this.CheckDisposed("Schema::get");
				return PostItemSchema.Instance;
			}
		}

		public bool IsRead
		{
			get
			{
				this.CheckDisposed("IsRead::get");
				return base.GetValueOrDefault<bool>(InternalSchema.IsRead);
			}
			set
			{
				this.CheckDisposed("IsRead::set");
				this[InternalSchema.IsRead] = value;
			}
		}

		public static PostItem Create(StoreSession session, StoreId destFolderId)
		{
			if (session == null)
			{
				ExTraceGlobals.StorageTracer.TraceError<string>(0L, "PostItem::Create. {0} should not be null.", "session");
				throw new ArgumentNullException(ServerStrings.ExNullParameter("session", 1));
			}
			if (destFolderId == null)
			{
				ExTraceGlobals.StorageTracer.TraceError<string>(0L, "PostItem::Create. {0} should not be null.", "destFolderId");
				throw new ArgumentNullException(ServerStrings.ExNullParameter("destFolderId", 1));
			}
			PostItem postItem = null;
			bool flag = false;
			PostItem result;
			try
			{
				postItem = ItemBuilder.CreateNewItem<PostItem>(session, destFolderId, ItemCreateInfo.PostInfo);
				postItem[InternalSchema.ItemClass] = "IPM.Post";
				postItem[InternalSchema.IconIndex] = IconIndex.PostItem;
				MailboxSession mailboxSession = session as MailboxSession;
				if (mailboxSession != null)
				{
					postItem.From = new Participant(mailboxSession.MailboxOwner.MailboxInfo.DisplayName, mailboxSession.MailboxOwnerLegacyDN, "EX");
				}
				postItem.IsRead = false;
				flag = true;
				result = postItem;
			}
			finally
			{
				if (!flag && postItem != null)
				{
					postItem.Dispose();
					postItem = null;
				}
			}
			return result;
		}

		public new static PostItem Bind(StoreSession session, StoreId postId)
		{
			return PostItem.Bind(session, postId, null);
		}

		public new static PostItem Bind(StoreSession session, StoreId postId, ICollection<PropertyDefinition> propsToReturn)
		{
			return ItemBuilder.ItemBind<PostItem>(session, postId, PostItemSchema.Instance, propsToReturn);
		}

		public override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<PostItem>(this);
		}

		public void MarkAsRead(bool deferToSave)
		{
			this.CheckDisposed("MarkAsRead");
			if (!deferToSave)
			{
				this.SetReadFlagsInternal(SetReadFlags.None);
			}
			this.IsRead = true;
			if (!deferToSave)
			{
				base.ClearFlagsPropertyForSet(InternalSchema.IsRead);
			}
		}

		public void MarkAsUnread(bool deferToSave)
		{
			this.CheckDisposed("MarkAsUnread");
			if (!deferToSave)
			{
				this.SetReadFlagsInternal(SetReadFlags.ClearRead);
			}
			this.IsRead = false;
			if (!deferToSave)
			{
				base.ClearFlagsPropertyForSet(InternalSchema.IsRead);
			}
		}

		public PostItem ReplyToFolder(StoreId parentFolderId, ReplyForwardConfiguration configuration)
		{
			return this.ReplyToFolder(base.Session, parentFolderId, configuration);
		}

		public PostItem ReplyToFolder(StoreSession session, StoreId parentFolderId, ReplyForwardConfiguration configuration)
		{
			this.CheckDisposed("ReplyToFolder");
			Util.ThrowOnNullArgument(session, "session");
			Util.ThrowOnNullArgument(parentFolderId, "parentFolderId");
			Util.ThrowOnNullArgument(configuration, "configuration");
			PostItem postItem = null;
			bool flag = false;
			PostItem result;
			try
			{
				postItem = PostItem.Create(session, parentFolderId);
				ReplyCreation replyCreation = new ReplyCreation(this, postItem, configuration, false, false, true);
				replyCreation.PopulateProperties();
				postItem[InternalSchema.IconIndex] = IconIndex.PostItem;
				flag = true;
				result = postItem;
			}
			finally
			{
				if (!flag && postItem != null)
				{
					postItem.Dispose();
					postItem = null;
				}
			}
			return result;
		}

		public MessageItem CreateForward(MailboxSession session, StoreId parentFolderId, ReplyForwardConfiguration configuration)
		{
			this.CheckDisposed("CreateForward");
			Util.ThrowOnNullArgument(session, "session");
			Util.ThrowOnNullArgument(parentFolderId, "parentFolderId");
			Util.ThrowOnNullArgument(configuration, "configuration");
			return base.InternalCreateForward(session, parentFolderId, configuration);
		}

		public MessageItem CreateReply(MailboxSession session, StoreId parentFolderId, ReplyForwardConfiguration configuration)
		{
			this.CheckDisposed("CreateReply");
			Util.ThrowOnNullArgument(session, "session");
			Util.ThrowOnNullArgument(parentFolderId, "parentFolderId");
			Util.ThrowOnNullArgument(configuration, "configuration");
			return base.InternalCreateReply(session, parentFolderId, configuration);
		}

		public MessageItem CreateReplyAll(MailboxSession session, StoreId parentFolderId, ReplyForwardConfiguration configuration)
		{
			this.CheckDisposed("CreateReplyAll");
			Util.ThrowOnNullArgument(session, "session");
			Util.ThrowOnNullArgument(parentFolderId, "parentFolderId");
			Util.ThrowOnNullArgument(configuration, "configuration");
			return base.InternalCreateReplyAll(session, parentFolderId, configuration);
		}

		internal static void CoreObjectUpdateDraftFlag(CoreItem coreItem)
		{
			bool? valueAsNullable = coreItem.PropertyBag.GetValueAsNullable<bool>(InternalSchema.IsDraft);
			if (valueAsNullable == null || valueAsNullable.Value)
			{
				coreItem.PropertyBag[InternalSchema.IsDraft] = false;
			}
		}

		internal static void CoreObjectUpdateConversationTopic(CoreItem coreItem)
		{
			ICorePropertyBag propertyBag = coreItem.PropertyBag;
			string valueOrDefault = propertyBag.GetValueOrDefault<string>(InternalSchema.NormalizedSubjectInternal);
			string valueOrDefault2 = propertyBag.GetValueOrDefault<string>(InternalSchema.ConversationTopic);
			byte[] valueOrDefault3 = propertyBag.GetValueOrDefault<byte[]>(InternalSchema.ConversationIndex);
			if (valueOrDefault != null)
			{
				if (valueOrDefault2 == null)
				{
					propertyBag[InternalSchema.ConversationTopic] = valueOrDefault;
				}
				if (valueOrDefault3 == null)
				{
					propertyBag[InternalSchema.ConversationIndex] = Microsoft.Exchange.Data.Storage.ConversationIndex.CreateNew().ToByteArray();
				}
			}
		}

		protected override void InternalDispose(bool disposing)
		{
			base.InternalDispose(disposing);
		}

		private void SetReadFlagsInternal(SetReadFlags flags)
		{
			StoreSession session = base.Session;
			bool flag = false;
			try
			{
				if (session != null)
				{
					session.BeginMapiCall();
					session.BeginServerHealthCall();
					flag = true;
				}
				if (StorageGlobals.MapiTestHookBeforeCall != null)
				{
					StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
				}
				base.MapiMessage.SetReadFlag(flags);
			}
			catch (MapiPermanentException ex)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotSetReadFlags, ex, session, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("PostItem::SetReadFlagsInternal.", new object[0]),
					ex
				});
			}
			catch (MapiRetryableException ex2)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotSetReadFlags, ex2, session, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("PostItem::SetReadFlagsInternal.", new object[0]),
					ex2
				});
			}
			finally
			{
				try
				{
					if (session != null)
					{
						session.EndMapiCall();
						if (flag)
						{
							session.EndServerHealthCall();
						}
					}
				}
				finally
				{
					if (StorageGlobals.MapiTestHookAfterCall != null)
					{
						StorageGlobals.MapiTestHookAfterCall(MethodBase.GetCurrentMethod());
					}
				}
			}
		}
	}
}
