using System;
using System.Globalization;
using System.IO;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Net.Protocols.DeltaSync;

namespace Microsoft.Exchange.MailboxTransport.ContentAggregation.Schema
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class DeltaSyncEmail : DisposeTrackableBase, ISyncEmail, ISyncObject, IDisposeTrackable, IDisposable
	{
		internal DeltaSyncEmail(ISyncSourceSession sourceSession, DeltaSyncMail deltaSyncMail)
		{
			this.sourceSession = sourceSession;
			this.deltaSyncMail = deltaSyncMail;
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
				return new bool?(this.deltaSyncMail.Read);
			}
		}

		public SyncMessageResponseType? SyncMessageResponseType
		{
			get
			{
				base.CheckDisposed();
				if (this.deltaSyncMail.ReplyToOrForward == null)
				{
					return null;
				}
				switch (this.deltaSyncMail.ReplyToOrForward.Value)
				{
				case DeltaSyncMail.ReplyToOrForwardState.None:
					return new SyncMessageResponseType?(Microsoft.Exchange.MailboxTransport.ContentAggregation.Schema.SyncMessageResponseType.None);
				case DeltaSyncMail.ReplyToOrForwardState.RepliedTo:
					return new SyncMessageResponseType?(Microsoft.Exchange.MailboxTransport.ContentAggregation.Schema.SyncMessageResponseType.Replied);
				case DeltaSyncMail.ReplyToOrForwardState.Forwarded:
					return new SyncMessageResponseType?(Microsoft.Exchange.MailboxTransport.ContentAggregation.Schema.SyncMessageResponseType.Forwarded);
				default:
					throw new InvalidDataException(string.Format(CultureInfo.InvariantCulture, "Unknown ReplyToOrForwardState: {0}", new object[]
					{
						this.deltaSyncMail.ReplyToOrForward
					}));
				}
			}
		}

		public string From
		{
			get
			{
				base.CheckDisposed();
				return this.deltaSyncMail.From;
			}
		}

		public string Subject
		{
			get
			{
				base.CheckDisposed();
				return this.deltaSyncMail.Subject;
			}
		}

		public ExDateTime? ReceivedTime
		{
			get
			{
				base.CheckDisposed();
				return new ExDateTime?(this.deltaSyncMail.DateReceived);
			}
		}

		public string MessageClass
		{
			get
			{
				base.CheckDisposed();
				return this.deltaSyncMail.MessageClass;
			}
		}

		public Importance? Importance
		{
			get
			{
				base.CheckDisposed();
				switch (this.deltaSyncMail.Importance)
				{
				case DeltaSyncMail.ImportanceLevel.Low:
					return new Importance?(Microsoft.Exchange.Data.Storage.Importance.Low);
				case DeltaSyncMail.ImportanceLevel.Normal:
					return new Importance?(Microsoft.Exchange.Data.Storage.Importance.Normal);
				case DeltaSyncMail.ImportanceLevel.High:
					return new Importance?(Microsoft.Exchange.Data.Storage.Importance.High);
				default:
					throw new InvalidDataException(string.Format(CultureInfo.InvariantCulture, "Unknown Importance: {0}", new object[]
					{
						this.deltaSyncMail.Importance
					}));
				}
			}
		}

		public string ConversationTopic
		{
			get
			{
				base.CheckDisposed();
				return this.deltaSyncMail.ConversationTopic;
			}
		}

		public string ConversationIndex
		{
			get
			{
				base.CheckDisposed();
				return this.deltaSyncMail.ConversationIndex;
			}
		}

		public Sensitivity? Sensitivity
		{
			get
			{
				base.CheckDisposed();
				switch (this.deltaSyncMail.Sensitivity)
				{
				case DeltaSyncMail.SensitivityLevel.Normal:
					return new Sensitivity?(Microsoft.Exchange.Data.Storage.Sensitivity.Normal);
				case DeltaSyncMail.SensitivityLevel.Personal:
					return new Sensitivity?(Microsoft.Exchange.Data.Storage.Sensitivity.Personal);
				case DeltaSyncMail.SensitivityLevel.Private:
					return new Sensitivity?(Microsoft.Exchange.Data.Storage.Sensitivity.Private);
				case DeltaSyncMail.SensitivityLevel.Confidential:
					return new Sensitivity?(Microsoft.Exchange.Data.Storage.Sensitivity.CompanyConfidential);
				default:
					throw new InvalidDataException(string.Format(CultureInfo.InvariantCulture, "Unknown Sensitivity: {0}", new object[]
					{
						this.deltaSyncMail.Sensitivity
					}));
				}
			}
		}

		public int? Size
		{
			get
			{
				base.CheckDisposed();
				return new int?(this.deltaSyncMail.Size);
			}
		}

		public bool? HasAttachments
		{
			get
			{
				base.CheckDisposed();
				return new bool?(this.deltaSyncMail.HasAttachments);
			}
		}

		public bool? IsDraft
		{
			get
			{
				base.CheckDisposed();
				return new bool?(this.deltaSyncMail.IsDraft);
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
				return this.deltaSyncMail.EmailMessage;
			}
		}

		protected override void InternalDispose(bool disposing)
		{
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<DeltaSyncEmail>(this);
		}

		private ISyncSourceSession sourceSession;

		private DeltaSyncMail deltaSyncMail;
	}
}
