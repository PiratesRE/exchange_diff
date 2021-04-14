using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Monitoring
{
	internal sealed class ApplicationEventInfo : ReplicationEventBaseInfo
	{
		public ExEventLog.EventTuple EventTuple
		{
			get
			{
				return this.m_eventTuple;
			}
		}

		public ApplicationEventInfo(ExEventLog.EventTuple eventTuple) : base(ReplicationEventType.AppLog, false, null)
		{
			this.m_eventTuple = eventTuple;
		}

		private ExEventLog.EventTuple m_eventTuple;
	}
}
