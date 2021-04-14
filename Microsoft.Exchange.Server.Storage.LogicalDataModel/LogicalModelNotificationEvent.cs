using System;
using System.Security.Principal;
using System.Text;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	public abstract class LogicalModelNotificationEvent : NotificationEvent
	{
		public LogicalModelNotificationEvent(StoreDatabase database, int mailboxNumber, EventType eventType, WindowsIdentity userIdentity, ClientType clientType, EventFlags eventFlags, Guid? userIdentityContext) : base(database, mailboxNumber, (int)eventType, userIdentityContext)
		{
			this.userIdentity = userIdentity;
			this.clientType = clientType;
			this.eventFlags = eventFlags;
		}

		public EventType EventType
		{
			get
			{
				return (EventType)base.EventTypeValue;
			}
		}

		public WindowsIdentity UserIdentity
		{
			get
			{
				return this.userIdentity;
			}
		}

		public ClientType ClientType
		{
			get
			{
				return this.clientType;
			}
		}

		public EventFlags EventFlags
		{
			get
			{
				return this.eventFlags;
			}
		}

		protected override void AppendFields(StringBuilder sb)
		{
			base.AppendFields(sb);
			sb.Append(" EventType:[");
			sb.Append(this.EventType);
			sb.Append("] UserIdentity:[");
			sb.Append(this.userIdentity);
			sb.Append("] clientType:[");
			sb.Append(this.clientType);
			sb.Append("] EventFlags:[");
			sb.Append(this.eventFlags);
			sb.Append("]");
		}

		private WindowsIdentity userIdentity;

		private ClientType clientType;

		private EventFlags eventFlags;
	}
}
