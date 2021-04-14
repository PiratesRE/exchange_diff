using System;
using System.IO;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Transport.Sync.Common;
using Microsoft.Exchange.Transport.Sync.Common.Logging;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;

namespace Microsoft.Exchange.MailboxTransport.ContentAggregation.Schema
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class SyncErrorNotificationEmail : DisposeTrackableBase, ISyncEmail, ISyncObject, IDisposeTrackable, IDisposable
	{
		public SyncErrorNotificationEmail(ExDateTime receivedTime, Stream mimeStream)
		{
			SyncUtilities.ThrowIfArgumentNull("mimeStream", mimeStream);
			this.receivedTime = receivedTime;
			this.mimeStream = mimeStream;
		}

		public SchemaType Type
		{
			get
			{
				return SchemaType.Email;
			}
		}

		public ExDateTime? LastModifiedTime
		{
			get
			{
				return new ExDateTime?(this.receivedTime);
			}
		}

		public ISyncSourceSession SourceSession
		{
			get
			{
				return SyncErrorNotificationEmail.syncSourceSession;
			}
		}

		public bool? IsRead
		{
			get
			{
				return new bool?(false);
			}
		}

		public string From
		{
			get
			{
				base.CheckDisposed();
				return null;
			}
		}

		public string Subject
		{
			get
			{
				base.CheckDisposed();
				return null;
			}
		}

		public ExDateTime? ReceivedTime
		{
			get
			{
				base.CheckDisposed();
				return new ExDateTime?(this.receivedTime);
			}
		}

		public string MessageClass
		{
			get
			{
				return "IPM.Note";
			}
		}

		public Importance? Importance
		{
			get
			{
				return null;
			}
		}

		public string ConversationTopic
		{
			get
			{
				return null;
			}
		}

		public string ConversationIndex
		{
			get
			{
				return null;
			}
		}

		public Sensitivity? Sensitivity
		{
			get
			{
				return null;
			}
		}

		public int? Size
		{
			get
			{
				return null;
			}
		}

		public bool? HasAttachments
		{
			get
			{
				return new bool?(false);
			}
		}

		public bool? IsDraft
		{
			get
			{
				return new bool?(false);
			}
		}

		public string InternetMessageId
		{
			get
			{
				return null;
			}
		}

		public Stream MimeStream
		{
			get
			{
				return this.mimeStream;
			}
		}

		public SyncMessageResponseType? SyncMessageResponseType
		{
			get
			{
				return null;
			}
		}

		internal MessageItem CreateSyncErrorNotificationMessage(MailboxSession userMailboxSession, StoreId folderId, AggregationSubscription subscription, SyncLogSession syncLogSession)
		{
			MessageItem messageItem = MessageItem.Create(userMailboxSession, folderId);
			InboundConversionOptions scopedInboundConversionOptions = XSOSyncContentConversion.GetScopedInboundConversionOptions(userMailboxSession.GetADRecipientSession(true, ConsistencyMode.IgnoreInvalid));
			XSOSyncContentConversion.ConvertAnyMimeToItem(this.MimeStream, messageItem, scopedInboundConversionOptions);
			if (this.IsRead != null)
			{
				messageItem[MessageItemSchema.IsRead] = this.IsRead.Value;
			}
			messageItem[MessageItemSchema.IsDraft] = false;
			messageItem[ItemSchema.SpamConfidenceLevel] = -1;
			messageItem[ItemSchema.ReceivedTime] = this.receivedTime.ToUtc();
			messageItem[MessageItemSchema.SharingInstanceGuid] = subscription.SubscriptionGuid;
			messageItem[StoreObjectSchema.LastModifiedTime] = this.LastModifiedTime.Value;
			return messageItem;
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing && this.mimeStream != null)
			{
				this.mimeStream.Dispose();
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<SyncErrorNotificationEmail>(this);
		}

		private static readonly ISyncSourceSession syncSourceSession = new SyncErrorNotificationEmail.SyncErrorNotificationEmailSourceSession();

		private readonly ExDateTime receivedTime;

		private readonly Stream mimeStream;

		private class SyncErrorNotificationEmailSourceSession : ISyncSourceSession
		{
			public string Protocol
			{
				get
				{
					return string.Empty;
				}
			}

			public string SessionId
			{
				get
				{
					return string.Empty;
				}
			}

			public string Server
			{
				get
				{
					return string.Empty;
				}
			}
		}
	}
}
