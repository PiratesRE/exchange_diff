using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Directory.Probes
{
	public class SharedConfigurationTenantProbe : ProbeWorkItem
	{
		public override void PopulateDefinition<ProbeDefinition>(ProbeDefinition pDef, Dictionary<string, string> propertyBag)
		{
		}

		protected override void DoWork(CancellationToken cancellationToken)
		{
			DirectoryUtils.Logger(this, StxLogType.SharedConfigurationTenantMonitor, delegate
			{
				string name = DirectoryAccessor.Instance.Server.Name;
				base.Result.StateAttribute1 = name;
				WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.DirectoryTracer, base.TraceContext, "Starting verification of Shared configuration tenants in server: {0}", name, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Directory\\SharedConfigurationTenantProbe.cs", 52);
				DirectoryUtils.CheckSharedConfigurationTenants();
				string text = string.Format("SCTs are created for all supported versions: {0}.", name);
				WTFDiagnostics.TraceInformation(ExTraceGlobals.DirectoryTracer, base.TraceContext, text, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Directory\\SharedConfigurationTenantProbe.cs", 64);
				base.Result.StateAttribute2 = text;
			});
		}
	}
}
