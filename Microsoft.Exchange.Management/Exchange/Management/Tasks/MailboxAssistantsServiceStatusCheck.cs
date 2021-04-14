using System;
using System.ServiceProcess;
using Microsoft.Exchange.Configuration.Common;
using Microsoft.Exchange.Monitoring;

namespace Microsoft.Exchange.Management.Tasks
{
	internal class MailboxAssistantsServiceStatusCheck : AssistantTroubleshooterBase
	{
		public MailboxAssistantsServiceStatusCheck(PropertyBag fields) : base(fields)
		{
		}

		public override MonitoringData InternalRunCheck()
		{
			MonitoringData monitoringData = new MonitoringData();
			using (ServiceController serviceController = new ServiceController("MsExchangeMailboxAssistants", base.ExchangeServer.Fqdn))
			{
				if (serviceController.Status != ServiceControllerStatus.Running)
				{
					monitoringData.Events.Add(new MonitoringEvent(AssistantTroubleshooterBase.EventSource, 5201, EventTypeEnumeration.Error, Strings.MailboxAssistantsServiceNotRunning(base.ExchangeServer.Fqdn, serviceController.Status.ToString())));
				}
			}
			return monitoringData;
		}
	}
}
