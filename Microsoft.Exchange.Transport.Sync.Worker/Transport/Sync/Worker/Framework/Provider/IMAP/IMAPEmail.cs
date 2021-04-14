using System;
using System.IO;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.MailboxTransport.ContentAggregation.Schema;
using Microsoft.Exchange.Transport.Sync.Common;
using Microsoft.Exchange.Transport.Sync.Worker.Framework.Provider.IMAP.Client;

namespace Microsoft.Exchange.Transport.Sync.Worker.Framework.Provider.IMAP
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class IMAPEmail : DisposeTrackableBase, ISyncEmail, ISyncObject, IDisposeTrackable, IDisposable
	{
		internal IMAPEmail(IMAPSyncStorageProviderState state, string folderCloudId, IMAPMailFlags mailFlags, IMAPMailFlags permanentFlags)
		{
			SyncUtilities.ThrowIfArgumentNull("state", state);
			this.sourceSession = state;
			if (state.GetDefaultFolderMapping(DefaultFolderType.Drafts) == folderCloudId)
			{
				mailFlags |= IMAPMailFlags.Draft;
				permanentFlags |= IMAPMailFlags.Draft;
			}
			if ((permanentFlags & IMAPMailFlags.Draft) == IMAPMailFlags.Draft)
			{
				this.draft = new bool?((mailFlags & IMAPMailFlags.Draft) == IMAPMailFlags.Draft);
			}
			if ((permanentFlags & IMAPMailFlags.Seen) == IMAPMailFlags.Seen)
			{
				this.read = new bool?((mailFlags & IMAPMailFlags.Seen) == IMAPMailFlags.Seen);
			}
			if ((permanentFlags & IMAPMailFlags.Answered) == IMAPMailFlags.Answered)
			{
				this.syncMessageResponseType = new SyncMessageResponseType?(((mailFlags & IMAPMailFlags.Answered) == IMAPMailFlags.Answered) ? Microsoft.Exchange.MailboxTransport.ContentAggregation.Schema.SyncMessageResponseType.Replied : Microsoft.Exchange.MailboxTransport.ContentAggregation.Schema.SyncMessageResponseType.None);
			}
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
				return this.read;
			}
		}

		public SyncMessageResponseType? SyncMessageResponseType
		{
			get
			{
				base.CheckDisposed();
				return this.syncMessageResponseType;
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
				return this.receivedTime;
			}
		}

		public string MessageClass
		{
			get
			{
				base.CheckDisposed();
				return null;
			}
		}

		public Importance? Importance
		{
			get
			{
				base.CheckDisposed();
				return null;
			}
		}

		public string ConversationTopic
		{
			get
			{
				base.CheckDisposed();
				return null;
			}
		}

		public string ConversationIndex
		{
			get
			{
				base.CheckDisposed();
				return null;
			}
		}

		public Sensitivity? Sensitivity
		{
			get
			{
				base.CheckDisposed();
				return null;
			}
		}

		public int? Size
		{
			get
			{
				base.CheckDisposed();
				return null;
			}
		}

		public bool? HasAttachments
		{
			get
			{
				base.CheckDisposed();
				return null;
			}
		}

		public bool? IsDraft
		{
			get
			{
				base.CheckDisposed();
				return this.draft;
			}
		}

		public string InternetMessageId
		{
			get
			{
				base.CheckDisposed();
				return null;
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

		public ExDateTime? LastModifiedTime
		{
			get
			{
				base.CheckDisposed();
				return null;
			}
		}

		public SchemaType Type
		{
			get
			{
				base.CheckDisposed();
				return SchemaType.Email;
			}
		}

		internal void SetItemProperties(ISyncSourceSession sourceSession, Stream mimeStream, ExDateTime? internalDate)
		{
			SyncUtilities.ThrowIfArgumentNull("sourceSession", sourceSession);
			SyncUtilities.ThrowIfArgumentNull("mimeStream", mimeStream);
			this.sourceSession = sourceSession;
			this.mimeStream = mimeStream;
			this.receivedTime = ((internalDate != null) ? internalDate : this.FindReceivedTime());
		}

		protected override void InternalDispose(bool disposing)
		{
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<IMAPEmail>(this);
		}

		private ExDateTime? FindReceivedTime()
		{
			if (this.mimeStream == null)
			{
				return null;
			}
			ExDateTime receivedDate = SyncUtilities.GetReceivedDate(this.mimeStream, true);
			if (receivedDate == ExDateTime.MinValue)
			{
				return null;
			}
			return new ExDateTime?(receivedDate);
		}

		private ISyncSourceSession sourceSession;

		private Stream mimeStream;

		private bool? draft;

		private bool? read;

		private ExDateTime? receivedTime;

		private SyncMessageResponseType? syncMessageResponseType;
	}
}
