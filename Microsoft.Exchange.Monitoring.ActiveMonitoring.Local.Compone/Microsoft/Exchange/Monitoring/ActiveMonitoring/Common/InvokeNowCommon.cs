using System;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.ActiveMonitoring.Management.Common;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Common
{
	internal class InvokeNowCommon
	{
		internal static WorkDefinition GetWorkDefinition(string monitorIdentity)
		{
			if (!MonitoringItemIdentity.MonitorIdentityId.IsValidFormat(monitorIdentity))
			{
				return null;
			}
			string healthSet = MonitoringItemIdentity.MonitorIdentityId.GetHealthSet(monitorIdentity);
			string monitor = MonitoringItemIdentity.MonitorIdentityId.GetMonitor(monitorIdentity);
			string targetResource = MonitoringItemIdentity.MonitorIdentityId.GetTargetResource(monitorIdentity);
			WorkDefinition definitionFromCrimson = InvokeNowCommon.GetDefinitionFromCrimson<ProbeDefinition>(healthSet, monitor, targetResource);
			if (definitionFromCrimson == null)
			{
				definitionFromCrimson = InvokeNowCommon.GetDefinitionFromCrimson<MonitorDefinition>(healthSet, monitor, targetResource);
			}
			if (definitionFromCrimson == null)
			{
				definitionFromCrimson = InvokeNowCommon.GetDefinitionFromCrimson<ResponderDefinition>(healthSet, monitor, targetResource);
			}
			if (definitionFromCrimson == null)
			{
				definitionFromCrimson = InvokeNowCommon.GetDefinitionFromCrimson<MaintenanceDefinition>(healthSet, monitor, targetResource);
			}
			return definitionFromCrimson;
		}

		internal static WorkDefinition GetDefinitionFromCrimson<T>(string healthSetName, string monitorName, string targetResource) where T : WorkDefinition, IPersistence, new()
		{
			using (CrimsonReader<T> crimsonReader = new CrimsonReader<T>())
			{
				for (WorkDefinition workDefinition = crimsonReader.ReadNext(); workDefinition != null; workDefinition = crimsonReader.ReadNext())
				{
					if (string.Equals(healthSetName, workDefinition.ServiceName, StringComparison.InvariantCultureIgnoreCase) && string.Equals(monitorName, workDefinition.Name, StringComparison.InvariantCultureIgnoreCase))
					{
						if (string.IsNullOrEmpty(targetResource))
						{
							return workDefinition;
						}
						if (string.Equals(targetResource, workDefinition.TargetResource, StringComparison.InvariantCultureIgnoreCase))
						{
							return workDefinition;
						}
					}
				}
			}
			return null;
		}
	}
}
