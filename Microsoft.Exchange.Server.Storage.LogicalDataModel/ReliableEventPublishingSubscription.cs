using System;
using System.Text;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	public class ReliableEventPublishingSubscription : NotificationSubscription
	{
		public ReliableEventPublishingSubscription(NotificationCallback callback) : base(SubscriptionKind.PreCommit, null, null, 0, 50592894, callback)
		{
		}

		public EventType EventTypeMask
		{
			get
			{
				return (EventType)base.EventTypeValueMask;
			}
		}

		public override bool IsInterested(NotificationEvent nev)
		{
			return true;
		}

		protected override void AppendClassName(StringBuilder sb)
		{
			sb.Append("ReliableEventPublishingSubscription");
		}

		protected override void AppendFields(StringBuilder sb)
		{
			base.AppendFields(sb);
			sb.Append(" EventTypeMask:[");
			sb.Append(this.EventTypeMask);
			sb.Append("]");
		}
	}
}
