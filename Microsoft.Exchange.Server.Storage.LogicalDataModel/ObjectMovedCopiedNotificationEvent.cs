using System;
using System.Security.Principal;
using System.Text;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	public abstract class ObjectMovedCopiedNotificationEvent : ObjectNotificationEvent
	{
		public ObjectMovedCopiedNotificationEvent(StoreDatabase database, int mailboxNumber, EventType eventType, WindowsIdentity userIdentity, ClientType clientType, EventFlags eventFlags, ExtendedEventFlags extendedEventFlags, ExchangeId fid, ExchangeId mid, ExchangeId parentFid, int? documentId, int? conversationDocumentId, ExchangeId oldFid, ExchangeId oldMid, ExchangeId oldParentFid, int? oldConversationDocumentId, string objectClass) : base(database, mailboxNumber, eventType, userIdentity, clientType, eventFlags, extendedEventFlags, fid, mid, parentFid, documentId, conversationDocumentId, objectClass)
		{
			this.oldFid = oldFid;
			this.oldMid = oldMid;
			this.oldParentFid = oldParentFid;
			this.oldConversationDocumentId = oldConversationDocumentId;
		}

		public ExchangeId OldFid
		{
			get
			{
				return this.oldFid;
			}
		}

		public ExchangeId OldMid
		{
			get
			{
				return this.oldMid;
			}
		}

		public ExchangeId OldParentFid
		{
			get
			{
				return this.oldParentFid;
			}
		}

		public int? OldConversationDocumentId
		{
			get
			{
				return this.oldConversationDocumentId;
			}
		}

		protected override void AppendFields(StringBuilder sb)
		{
			base.AppendFields(sb);
			sb.Append(" OldFid:[");
			sb.Append(this.oldFid);
			sb.Append("] OldMid:[");
			sb.Append(this.oldMid);
			sb.Append("] OldParentFid:[");
			sb.Append(this.oldParentFid);
			sb.Append("]");
			sb.Append("] OldConversationDocumentId:[");
			sb.Append(this.oldConversationDocumentId);
			sb.Append("]");
		}

		private readonly ExchangeId oldFid;

		private readonly ExchangeId oldMid;

		private readonly ExchangeId oldParentFid;

		private readonly int? oldConversationDocumentId;
	}
}
