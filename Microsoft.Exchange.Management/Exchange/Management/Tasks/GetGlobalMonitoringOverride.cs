using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Monitoring.ActiveMonitoring;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("Get", "GlobalMonitoringOverride")]
	public sealed class GetGlobalMonitoringOverride : GetSingletonSystemConfigurationObjectTask<MonitoringOverride>
	{
		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			ADObjectId descendantId = base.RootOrgContainerId.GetDescendantId(MonitoringOverride.RdnContainer);
			QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Name, MonitoringOverride.ContainerName);
			Container[] array = this.ConfigurationSession.Find<Container>(descendantId, QueryScope.SubTree, filter, null, 0);
			if (array == null || array.Length == 0)
			{
				TaskLogger.LogExit();
				return;
			}
			foreach (object obj in Enum.GetValues(typeof(MonitoringItemTypeEnum)))
			{
				MonitoringItemTypeEnum monitoringItemTypeEnum = (MonitoringItemTypeEnum)obj;
				string text = monitoringItemTypeEnum.ToString();
				Container childContainer = array[0].GetChildContainer(text);
				MonitoringOverride[] array2 = this.ConfigurationSession.Find<MonitoringOverride>(childContainer.Id, QueryScope.SubTree, null, null, 0);
				foreach (MonitoringOverride monitoringOverride in array2)
				{
					this.WriteResult(new MonitoringOverrideObject(monitoringOverride, text));
				}
			}
			TaskLogger.LogExit();
		}
	}
}
