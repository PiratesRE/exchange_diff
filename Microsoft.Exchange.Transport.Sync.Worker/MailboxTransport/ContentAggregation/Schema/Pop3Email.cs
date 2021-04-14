using System;
using System.IO;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.MailboxTransport.ContentAggregation.Schema
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class Pop3Email : DisposeTrackableBase, ISyncEmail, ISyncObject, IDisposeTrackable, IDisposable
	{
		internal Pop3Email(ISyncSourceSession sourceSession, ExDateTime receivedTime, Stream mimeStream)
		{
			this.sourceSession = sourceSession;
			this.receivedTime = receivedTime;
			this.mimeStream = mimeStream;
		}

		public SchemaType Type
		{
			get
			{
				base.CheckDisposed();
				return SchemaType.Email;
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
				return new bool?(false);
			}
		}

		public SyncMessageResponseType? SyncMessageResponseType
		{
			get
			{
				base.CheckDisposed();
				return null;
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
				return null;
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

		public ExDateTime? LastModifiedTime
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

		protected override void InternalDispose(bool disposing)
		{
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<Pop3Email>(this);
		}

		private ISyncSourceSession sourceSession;

		private ExDateTime receivedTime;

		private Stream mimeStream;
	}
}
