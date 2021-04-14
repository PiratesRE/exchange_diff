using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Monitoring
{
	internal abstract class ReplicationEventBaseInfo
	{
		public ReplicationEventType EventType
		{
			get
			{
				return this.m_eventType;
			}
		}

		public LocalizedString? BaseEventMessage
		{
			get
			{
				return this.m_BaseEventMessage;
			}
		}

		public bool ShouldBeRolledUp
		{
			get
			{
				return this.m_ShouldBeRolledUp;
			}
		}

		public ReplicationEventBaseInfo(ReplicationEventType eventType, bool shouldBeRolledUp, LocalizedString? baseEventMessage)
		{
			this.m_eventType = eventType;
			this.m_ShouldBeRolledUp = shouldBeRolledUp;
			this.m_BaseEventMessage = baseEventMessage;
		}

		private readonly ReplicationEventType m_eventType;

		private readonly LocalizedString? m_BaseEventMessage;

		private readonly bool m_ShouldBeRolledUp;
	}
}
