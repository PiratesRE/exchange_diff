using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.SystemConfigurationTasks;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Monitoring;

namespace Microsoft.Exchange.Management.EhfHybridMailflow
{
	[Cmdlet("Get", "HybridMailflowDatacenterIPs")]
	public sealed class GetHybridMailflowDatacenterIPs : Task
	{
		protected override void InternalProcessRecord()
		{
			this.WriteResult(this.GetDatacenterIPs());
		}

		private void WriteResult(object dataObject)
		{
			base.WriteObject(dataObject);
		}

		private HybridMailflowDatacenterIPs GetDatacenterIPs()
		{
			MultiValuedProperty<IPRange> datacenterIPs;
			HybridMailflowDatacenterIPs result;
			if (HygieneDCSettings.GetFfoDCPublicIPAddresses(out datacenterIPs))
			{
				result = new HybridMailflowDatacenterIPs(datacenterIPs);
			}
			else
			{
				LocalizedException exception = new LocalizedException(Strings.HybridMailflowGetFfoDCPublicIPAddressesFailed);
				base.WriteError(exception, ExchangeErrorCategory.ServerOperation, this);
				result = null;
			}
			return result;
		}

		protected override bool IsKnownException(Exception exception)
		{
			return base.IsKnownException(exception) || DataAccessHelper.IsDataAccessKnownException(exception) || MonitoringHelper.IsKnownExceptionForMonitoring(exception);
		}

		private void WriteErrorAndMonitoringEvent(Exception exception, ExchangeErrorCategory errorCategory, object target, int eventId, string eventSource)
		{
			this.monitoringData.Events.Add(new MonitoringEvent(eventSource, eventId, EventTypeEnumeration.Error, exception.Message));
			base.WriteError(exception, (ErrorCategory)errorCategory, target);
		}

		private const string CmdletNoun = "HybridMailflowDatacenterIPs";

		private const string CmdletMonitoringEventSource = "MSExchange Monitoring HybridMailflowDatacenterIPs";

		private MonitoringData monitoringData = new MonitoringData();
	}
}
