using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Monitoring
{
	internal sealed class MomEventInfo : ReplicationEventBaseInfo
	{
		public int MomEventId
		{
			get
			{
				return this.m_MomEventId;
			}
		}

		public EventTypeEnumeration MomEventType
		{
			get
			{
				return this.m_MomEventType;
			}
		}

		public MomEventInfo(int momEventId, EventTypeEnumeration momEventType, bool shouldBeRolledUp, LocalizedString? baseEventMessage) : base(ReplicationEventType.MOM, shouldBeRolledUp, baseEventMessage)
		{
			this.m_MomEventId = momEventId;
			this.m_MomEventType = momEventType;
		}

		private readonly int m_MomEventId;

		private readonly EventTypeEnumeration m_MomEventType;
	}
}
