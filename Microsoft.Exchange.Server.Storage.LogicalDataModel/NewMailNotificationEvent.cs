using System;
using System.Security.Principal;
using System.Text;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	public class NewMailNotificationEvent : ObjectNotificationEvent
	{
		public NewMailNotificationEvent(StoreDatabase database, int mailboxNumber, WindowsIdentity userIdentity, ClientType clientType, EventFlags eventFlags, ExtendedEventFlags extendedEventFlags, ExchangeId fid, ExchangeId mid, int documentId, int? conversationDocumentId, string messageClass, int messageFlags) : base(database, mailboxNumber, EventType.NewMail, userIdentity, clientType, eventFlags, extendedEventFlags, fid, mid, fid, new int?(documentId), conversationDocumentId, messageClass)
		{
			Statistics.NotificationTypes.NewMail.Bump();
			this.messageFlags = messageFlags;
		}

		public int MessageFlags
		{
			get
			{
				return this.messageFlags;
			}
		}

		public string MessageClass
		{
			get
			{
				return base.ObjectClass;
			}
		}

		protected override void AppendClassName(StringBuilder sb)
		{
			sb.Append("NewMailNotificationEvent");
		}

		protected override void AppendFields(StringBuilder sb)
		{
			base.AppendFields(sb);
			sb.Append(" MessageFlags:[");
			sb.Append(this.messageFlags);
			sb.Append("]");
		}

		private int messageFlags;
	}
}
