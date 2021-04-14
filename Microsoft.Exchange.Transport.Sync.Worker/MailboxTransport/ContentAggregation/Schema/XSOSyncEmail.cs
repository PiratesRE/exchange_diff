using System;
using System.IO;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Transport.Sync.Common;
using Microsoft.Exchange.Transport.Sync.Worker.Throttling;

namespace Microsoft.Exchange.MailboxTransport.ContentAggregation.Schema
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class XSOSyncEmail : DisposeTrackableBase, ISyncEmail, ISyncObject, IDisposeTrackable, IDisposable
	{
		internal XSOSyncEmail(XSOSyncStorageProviderState providerState, StoreObjectId nativeId, ChangeType changeType)
		{
			SyncUtilities.ThrowIfArgumentNull("providerState", providerState);
			SyncUtilities.ThrowIfArgumentNull("nativeId", nativeId);
			if (providerState.SyncMailboxSession == null || providerState.SyncMailboxSession.MailboxSession == null)
			{
				throw new ArgumentException("session");
			}
			PropertyDefinition[] properties;
			switch (changeType)
			{
			case ChangeType.Add:
			case ChangeType.Change:
				properties = XSOSyncEmail.ChangeProperties;
				using (Item item = SyncStoreLoadManager.ItemBind(providerState.SyncMailboxSession.MailboxSession, nativeId, StoreObjectSchema.ContentConversionProperties, new EventHandler<RoundtripCompleteEventArgs>(providerState.OnRoundtripComplete)))
				{
					this.mimeStream = XSOSyncContentConversion.ConvertItemToMime(item, providerState.ScopedOutboundConversionOptions);
					goto IL_B1;
				}
				break;
			case (ChangeType)3:
			case ChangeType.Delete:
				goto IL_AF;
			case ChangeType.ReadFlagChange:
				break;
			default:
				goto IL_AF;
			}
			throw new InvalidOperationException("ReadFlagChange should have used the other constructor.");
			IL_AF:
			properties = null;
			IL_B1:
			this.item = SyncStoreLoadManager.ItemBind(providerState.SyncMailboxSession.MailboxSession, nativeId, properties, new EventHandler<RoundtripCompleteEventArgs>(providerState.OnRoundtripComplete));
			this.sourceSession = providerState;
		}

		internal XSOSyncEmail(ISyncSourceSession sourceSession, bool read)
		{
			this.read = new bool?(read);
			this.sourceSession = sourceSession;
		}

		public ISyncSourceSession SourceSession
		{
			get
			{
				base.CheckDisposed();
				return this.sourceSession;
			}
		}

		public bool? IsRead
		{
			get
			{
				base.CheckDisposed();
				if (this.read != null)
				{
					return this.read;
				}
				return new bool?(SyncUtilities.SafeGetProperty<bool>(this.item, MessageItemSchema.IsRead));
			}
		}

		public SyncMessageResponseType? SyncMessageResponseType
		{
			get
			{
				base.CheckDisposed();
				IconIndex iconIndex = SyncUtilities.SafeGetProperty<IconIndex>(this.item, ItemSchema.IconIndex);
				IconIndex iconIndex2 = iconIndex;
				switch (iconIndex2)
				{
				case IconIndex.MailReplied:
					break;
				case IconIndex.MailForwarded:
					goto IL_52;
				default:
					switch (iconIndex2)
					{
					case IconIndex.MailEncryptedReplied:
					case IconIndex.MailSmimeSignedReplied:
						break;
					case IconIndex.MailEncryptedForwarded:
					case IconIndex.MailSmimeSignedForwarded:
						goto IL_52;
					default:
						return new SyncMessageResponseType?(Microsoft.Exchange.MailboxTransport.ContentAggregation.Schema.SyncMessageResponseType.None);
					}
					break;
				}
				return new SyncMessageResponseType?(Microsoft.Exchange.MailboxTransport.ContentAggregation.Schema.SyncMessageResponseType.Replied);
				IL_52:
				return new SyncMessageResponseType?(Microsoft.Exchange.MailboxTransport.ContentAggregation.Schema.SyncMessageResponseType.Forwarded);
			}
		}

		public string From
		{
			get
			{
				base.CheckDisposed();
				Participant participant = SyncUtilities.SafeGetProperty<Participant>(this.item, ItemSchema.From);
				if (null == participant)
				{
					return null;
				}
				if (participant.RoutingType != "SMTP")
				{
					Participant participant2 = Participant.TryConvertTo(participant, "SMTP", true);
					if (null != participant2)
					{
						participant = participant2;
					}
				}
				return participant.EmailAddress;
			}
		}

		public string Subject
		{
			get
			{
				base.CheckDisposed();
				return SyncUtilities.SafeGetProperty<string>(this.item, ItemSchema.Subject);
			}
		}

		public ExDateTime? ReceivedTime
		{
			get
			{
				base.CheckDisposed();
				ExDateTime exDateTime = SyncUtilities.SafeGetProperty<ExDateTime>(this.item, ItemSchema.ReceivedTime);
				if (exDateTime == ExDateTime.MinValue)
				{
					return null;
				}
				return new ExDateTime?(exDateTime);
			}
		}

		public string MessageClass
		{
			get
			{
				base.CheckDisposed();
				return this.item.ClassName;
			}
		}

		public Importance? Importance
		{
			get
			{
				base.CheckDisposed();
				return new Importance?(this.item.Importance);
			}
		}

		public string ConversationTopic
		{
			get
			{
				base.CheckDisposed();
				return SyncUtilities.SafeGetProperty<string>(this.item, ItemSchema.ConversationTopic);
			}
		}

		public string ConversationIndex
		{
			get
			{
				base.CheckDisposed();
				byte[] array = SyncUtilities.SafeGetProperty<byte[]>(this.item, ItemSchema.ConversationIndex);
				if (array == null)
				{
					return null;
				}
				return HexConverter.ByteArrayToHexString(array);
			}
		}

		public Sensitivity? Sensitivity
		{
			get
			{
				base.CheckDisposed();
				return new Sensitivity?(this.item.Sensitivity);
			}
		}

		public int? Size
		{
			get
			{
				base.CheckDisposed();
				return new int?((int)this.item.Size());
			}
		}

		public bool? HasAttachments
		{
			get
			{
				base.CheckDisposed();
				return new bool?(SyncUtilities.SafeGetProperty<bool>(this.item, ItemSchema.HasAttachment));
			}
		}

		public bool? IsDraft
		{
			get
			{
				base.CheckDisposed();
				return new bool?(SyncUtilities.SafeGetProperty<bool>(this.item, MessageItemSchema.IsDraft));
			}
		}

		public string InternetMessageId
		{
			get
			{
				base.CheckDisposed();
				return SyncUtilities.SafeGetProperty<string>(this.item, ItemSchema.InternetMessageId);
			}
		}

		public Stream MimeStream
		{
			get
			{
				base.CheckDisposed();
				return this.mimeStream;
			}
		}

		public virtual SchemaType Type
		{
			get
			{
				base.CheckDisposed();
				return SchemaType.Email;
			}
		}

		public ExDateTime? LastModifiedTime
		{
			get
			{
				base.CheckDisposed();
				return new ExDateTime?(this.item.LastModifiedTime);
			}
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				if (this.mimeStream != null)
				{
					this.mimeStream.Dispose();
					this.mimeStream = null;
				}
				if (this.item != null)
				{
					this.item.Dispose();
					this.item = null;
				}
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<XSOSyncEmail>(this);
		}

		private static readonly PropertyDefinition[] ChangeProperties = new PropertyDefinition[]
		{
			MessageItemSchema.IsRead,
			MessageItemSchema.ReplyForwardStatus,
			ItemSchema.From,
			ItemSchema.Subject,
			ItemSchema.ReceivedTime,
			StoreObjectSchema.ContentClass,
			ItemSchema.Importance,
			ItemSchema.ConversationTopic,
			ItemSchema.ConversationIndex,
			ItemSchema.Sensitivity,
			ItemSchema.Size,
			ItemSchema.HasAttachment,
			MessageItemSchema.IsDraft,
			ItemSchema.InternetMessageId,
			ItemSchema.IconIndex
		};

		private ISyncSourceSession sourceSession;

		private Item item;

		private Stream mimeStream;

		private bool? read;
	}
}
