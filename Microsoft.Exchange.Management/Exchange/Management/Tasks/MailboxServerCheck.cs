using System;
using Microsoft.Exchange.Configuration.Common;
using Microsoft.Exchange.Monitoring;

namespace Microsoft.Exchange.Management.Tasks
{
	internal class MailboxServerCheck : AssistantTroubleshooterBase
	{
		public MailboxServerCheck(PropertyBag fields) : base(fields)
		{
		}

		public override MonitoringData InternalRunCheck()
		{
			MonitoringData monitoringData = new MonitoringData();
			if (!base.ExchangeServer.IsMailboxServer)
			{
				monitoringData.Events.Add(new MonitoringEvent(AssistantTroubleshooterBase.EventSource, 5200, EventTypeEnumeration.Error, Strings.TSNotAMailboxServer(base.ExchangeServer.Name)));
			}
			return monitoringData;
		}

		public override MonitoringData Resolve(MonitoringData monitoringData)
		{
			monitoringData.Events.Add(base.TSResolutionFailed(base.ExchangeServer.Name));
			return monitoringData;
		}
	}
}
