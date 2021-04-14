using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Get", "DeliveryAgentConnector", DefaultParameterSetName = "Identity")]
	public sealed class GetDeliveryAgentConnector : GetSystemConfigurationObjectTask<DeliveryAgentConnectorIdParameter, DeliveryAgentConnector>
	{
		protected override bool DeepSearch
		{
			get
			{
				return true;
			}
		}

		protected override ObjectId RootId
		{
			get
			{
				IConfigurationSession configurationSession = (IConfigurationSession)base.DataSession;
				return configurationSession.GetOrgContainerId().GetChildId("Administrative Groups");
			}
		}

		protected override void WriteResult(IConfigurable dataObject)
		{
			DeliveryAgentConnector deliveryAgentConnector = dataObject as DeliveryAgentConnector;
			if (deliveryAgentConnector != null && !deliveryAgentConnector.IsReadOnly)
			{
				deliveryAgentConnector.IsScopedConnector = deliveryAgentConnector.GetScopedConnector();
				deliveryAgentConnector.ResetChangeTracking();
			}
			base.WriteResult(dataObject);
		}
	}
}
