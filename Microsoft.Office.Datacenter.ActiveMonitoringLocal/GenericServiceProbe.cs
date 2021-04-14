using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Office.Datacenter.ActiveMonitoring
{
	public class GenericServiceProbe : ProbeWorkItem
	{
		public override void PopulateDefinition<ProbeDefinition>(ProbeDefinition definition, Dictionary<string, string> propertyBag)
		{
			definition.TargetResource = propertyBag["TargetResource"];
		}

		internal override IEnumerable<PropertyInformation> GetSubstitutePropertyInformation()
		{
			return new List<PropertyInformation>
			{
				new PropertyInformation(GenericServiceProbe.TargetResourcePropertyName, StringsLocal.GenericServiceProbeTargetResource, true)
			};
		}

		protected virtual bool ShouldRun()
		{
			return true;
		}

		protected override void DoWork(CancellationToken cancellationToken)
		{
			if (!this.ShouldRun())
			{
				return;
			}
			string windowsServiceName = this.GetWindowsServiceName();
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.ServiceTracer, base.TraceContext, "Starting Service check against {0}", windowsServiceName, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\LocalDataAccess\\WorkItems\\Probes\\GenericServiceProbe.cs", 81);
			try
			{
				using (ServiceController serviceController = new ServiceController(windowsServiceName))
				{
					if (serviceController.Status != ServiceControllerStatus.Running)
					{
						base.Result.StateAttribute1 = serviceController.Status.ToString();
						string message = string.Format("{0} service is not running", windowsServiceName);
						WTFDiagnostics.TraceError(ExTraceGlobals.ServiceTracer, base.TraceContext, message, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\LocalDataAccess\\WorkItems\\Probes\\GenericServiceProbe.cs", 102);
						throw new Exception(message);
					}
					if (base.Broker != null)
					{
						IDataAccessQuery<ProbeResult> probeResults = base.Broker.GetProbeResults(base.Definition, GenericServiceProbe.systemBootTime);
						Task<ProbeResult> task = probeResults.ExecuteAsync(cancellationToken, base.TraceContext);
						task.Continue(delegate(ProbeResult lastProbeResult)
						{
							if (lastProbeResult == null)
							{
								StartupNotification.InsertStartupNotification(windowsServiceName);
							}
						}, cancellationToken, TaskContinuationOptions.AttachedToParent);
					}
				}
			}
			finally
			{
				stopwatch.Stop();
				base.Result.SampleValue = (double)((int)stopwatch.ElapsedMilliseconds);
			}
		}

		protected string GetWindowsServiceName()
		{
			if (base.Definition.Attributes.ContainsKey("WindowsServiceName"))
			{
				return base.Definition.Attributes["WindowsServiceName"];
			}
			return base.Definition.TargetResource;
		}

		public static readonly string TargetResourcePropertyName = "TargetResource";

		private static DateTime systemBootTime = StartupNotification.GetSystemBootTime(true);

		internal static class AttributeNames
		{
			internal const string WindowsServiceName = "WindowsServiceName";
		}
	}
}
