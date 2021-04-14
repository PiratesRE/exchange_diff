using System;
using System.IO;
using Microsoft.Exchange.Connections.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Connections.Pop
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class Pop3Email : DisposeTrackableBase
	{
		internal Pop3Email(ILog log, ExDateTime receivedTime, Stream mimeStream)
		{
			this.log = log;
			this.receivedTime = receivedTime;
			this.mimeStream = mimeStream;
		}

		public ILog Log
		{
			get
			{
				return this.log;
			}
		}

		public bool? IsRead
		{
			get
			{
				return new bool?(false);
			}
		}

		public MessageResponseType? MessageResponseType
		{
			get
			{
				return null;
			}
		}

		public string From
		{
			get
			{
				return null;
			}
		}

		public string Subject
		{
			get
			{
				return null;
			}
		}

		public ExDateTime? ReceivedTime
		{
			get
			{
				return new ExDateTime?(this.receivedTime);
			}
		}

		public string MessageClass
		{
			get
			{
				return null;
			}
		}

		public MessageImportance? MessageImportance
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

		public MessageSensitivity? MessageSensitivity
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
				return null;
			}
		}

		public bool? IsDraft
		{
			get
			{
				return null;
			}
		}

		public string InternetMessageId
		{
			get
			{
				return null;
			}
		}

		public ExDateTime? LastModifiedTime
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

		protected override void InternalDispose(bool disposing)
		{
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<Pop3Email>(this);
		}

		private ILog log;

		private ExDateTime receivedTime;

		private Stream mimeStream;
	}
}
