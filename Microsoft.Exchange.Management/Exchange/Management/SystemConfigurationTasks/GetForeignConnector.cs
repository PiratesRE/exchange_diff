using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Get", "ForeignConnector", DefaultParameterSetName = "Identity")]
	public sealed class GetForeignConnector : GetSystemConfigurationObjectTask<ForeignConnectorIdParameter, ForeignConnector>
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
			ForeignConnector foreignConnector = dataObject as ForeignConnector;
			if (foreignConnector != null && !foreignConnector.IsReadOnly)
			{
				foreignConnector.IsScopedConnector = foreignConnector.GetScopedConnector();
				foreignConnector.ResetChangeTracking();
			}
			base.WriteResult(dataObject);
		}
	}
}
