using System;
using System.Security.Principal;
using System.Text;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	public abstract class ObjectNotificationEvent : LogicalModelNotificationEvent
	{
		public ObjectNotificationEvent(StoreDatabase database, int mailboxNumber, EventType eventType, WindowsIdentity userIdentity, ClientType clientType, EventFlags eventFlags, ExchangeId fid, ExchangeId mid, ExchangeId parentFid, int? documentId, int? conversationDocumentId, string objectClass) : this(database, mailboxNumber, eventType, userIdentity, clientType, eventFlags, Microsoft.Exchange.Server.Storage.LogicalDataModel.ExtendedEventFlags.None, fid, mid, parentFid, documentId, conversationDocumentId, objectClass)
		{
		}

		public ObjectNotificationEvent(StoreDatabase database, int mailboxNumber, EventType eventType, WindowsIdentity userIdentity, ClientType clientType, EventFlags eventFlags, ExtendedEventFlags extendedEventFlags, ExchangeId fid, ExchangeId mid, ExchangeId parentFid, int? documentId, int? conversationDocumentId, string objectClass) : this(database, mailboxNumber, eventType, userIdentity, clientType, eventFlags, extendedEventFlags, fid, mid, parentFid, documentId, conversationDocumentId, objectClass, null)
		{
		}

		public ObjectNotificationEvent(StoreDatabase database, int mailboxNumber, EventType eventType, WindowsIdentity userIdentity, ClientType clientType, EventFlags eventFlags, ExtendedEventFlags extendedEventFlags, ExchangeId fid, ExchangeId mid, ExchangeId parentFid, int? documentId, int? conversationDocumentId, string objectClass, Guid? userIdentityContext) : base(database, mailboxNumber, eventType, userIdentity, clientType, eventFlags, userIdentityContext)
		{
			this.fid = fid;
			this.mid = mid;
			this.parentFid = parentFid;
			this.documentId = documentId;
			this.conversationDocumentId = conversationDocumentId;
			this.objectClass = objectClass;
			this.extendedEventFlags = new ExtendedEventFlags?(extendedEventFlags);
		}

		public bool IsMessageEvent
		{
			get
			{
				return this.mid.IsValid;
			}
		}

		public bool IsFolderEvent
		{
			get
			{
				return !this.IsMessageEvent;
			}
		}

		public ExchangeId Fid
		{
			get
			{
				return this.fid;
			}
		}

		public ExchangeId Mid
		{
			get
			{
				return this.mid;
			}
		}

		public ExchangeId ParentFid
		{
			get
			{
				return this.parentFid;
			}
		}

		public int? DocumentId
		{
			get
			{
				return this.documentId;
			}
		}

		public int? ConversationDocumentId
		{
			get
			{
				return this.conversationDocumentId;
			}
		}

		public string ObjectClass
		{
			get
			{
				return this.objectClass;
			}
		}

		public ExtendedEventFlags? ExtendedEventFlags
		{
			get
			{
				return this.extendedEventFlags;
			}
		}

		protected bool IsSameObject(ObjectNotificationEvent otherEvent)
		{
			return this.Mid == otherEvent.Mid && this.Fid == otherEvent.Fid;
		}

		protected override void AppendFields(StringBuilder sb)
		{
			base.AppendFields(sb);
			sb.Append(" Fid:[");
			sb.Append(this.fid);
			sb.Append("] Mid:[");
			sb.Append(this.mid);
			sb.Append("] ParentFid:[");
			sb.Append(this.parentFid);
			sb.Append("] DocumentId:[");
			sb.Append(this.documentId);
			sb.Append("] ConversationDocumentId:[");
			sb.Append(this.conversationDocumentId);
			sb.Append("] ObjectClass:[");
			sb.Append(this.objectClass);
			sb.Append("] ExtendedEventFlags:[");
			sb.Append(this.extendedEventFlags);
			sb.Append("]");
		}

		private readonly ExchangeId fid;

		private readonly ExchangeId mid;

		private readonly ExchangeId parentFid;

		private readonly int? documentId;

		private readonly int? conversationDocumentId;

		private readonly ExtendedEventFlags? extendedEventFlags;

		private readonly string objectClass;
	}
}
