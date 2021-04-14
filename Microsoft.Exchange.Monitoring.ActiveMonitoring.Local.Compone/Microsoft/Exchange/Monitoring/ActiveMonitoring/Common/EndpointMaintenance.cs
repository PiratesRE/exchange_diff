using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Common
{
	public class EndpointMaintenance<TEndpoint> : MaintenanceWorkItem where TEndpoint : IEndpoint, new()
	{
		public static string GetDefaultName()
		{
			return string.Format("{0}Maintenance", typeof(TEndpoint).Name);
		}

		protected override void DoWork(CancellationToken cancellationToken)
		{
			Type endpointType = typeof(TEndpoint);
			WTFDiagnostics.TraceFunction<string>(ExTraceGlobals.EndpointMaintenanceTracer, base.TraceContext, "Start maintenance for endpoint {0}", endpointType.Name, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\EndpointMaintenance.cs", 45);
			TimeSpan searchWindow = DateTime.UtcNow - LocalDataAccess.StartTime;
			if (searchWindow.TotalHours < 1.0)
			{
				searchWindow = TimeSpan.FromHours(1.0);
			}
			IDataAccessQuery<MaintenanceResult> lastMaintenanceResult = base.Broker.GetLastMaintenanceResult(base.Definition, searchWindow);
			Task<MaintenanceResult> task = lastMaintenanceResult.ExecuteAsync(cancellationToken, base.TraceContext);
			task.Continue(delegate(MaintenanceResult lastResult)
			{
				WTFDiagnostics.TraceInformation<MaintenanceResult>(ExTraceGlobals.EndpointMaintenanceTracer, this.TraceContext, "Found last result {0}", lastResult, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\EndpointMaintenance.cs", 61);
				int num = 0;
				if (lastResult != null && !string.IsNullOrWhiteSpace(lastResult.StateAttribute2))
				{
					WTFDiagnostics.TraceInformation<DateTime>(ExTraceGlobals.EndpointMaintenanceTracer, this.TraceContext, "Found last result at {0}", lastResult.ExecutionEndTime, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\EndpointMaintenance.cs", 70);
					string[] array = lastResult.StateAttribute2.Split(new char[]
					{
						'|'
					});
					DateTime t = DateTime.UtcNow.AddHours(-1.0);
					foreach (string text in array)
					{
						DateTime t2;
						if (DateTime.TryParse(text, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out t2))
						{
							t2 = t2.ToUniversalTime();
							if (t2 > t)
							{
								num++;
								this.AppendRestartTime(text);
							}
						}
					}
				}
				if (!LocalEndpointManager.Instance.IsEndpointInitialized(endpointType))
				{
					WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.EndpointMaintenanceTracer, this.TraceContext, "Initialize endpoint {0}", endpointType.Name, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\EndpointMaintenance.cs", 97);
					this.Result.StateAttribute1 = "Initialization";
					TEndpoint tendpoint = (default(TEndpoint) == null) ? Activator.CreateInstance<TEndpoint>() : default(TEndpoint);
					try
					{
						try
						{
							tendpoint.Initialize();
						}
						catch (Exception exception)
						{
							tendpoint.Exception = exception;
							throw;
						}
						return;
					}
					finally
					{
						LocalEndpointManager.Instance.SetEndpoint(endpointType, tendpoint, this.Definition.TargetExtension != "Test");
					}
				}
				TEndpoint tendpoint2 = (TEndpoint)((object)LocalEndpointManager.Instance.GetEndpoint(endpointType, false));
				this.Result.StateAttribute1 = "Detecting change";
				bool flag = false;
				try
				{
					if (tendpoint2.Exception != null)
					{
						tendpoint2.Initialize();
						flag = true;
					}
					else
					{
						flag = (tendpoint2.DetectChange() && tendpoint2.RestartOnChange);
					}
					tendpoint2.Exception = null;
				}
				catch (Exception exception2)
				{
					tendpoint2.Exception = exception2;
					throw;
				}
				if (flag)
				{
					if (LocalEndpointManager.Instance.RestartThrottlingAllowed && num >= this.Definition.MaxRestartRequestAllowedPerHour)
					{
						this.Result.StateAttribute3 = "Restart required but throttled";
						return;
					}
					this.Result.StateAttribute3 = "Restart requested";
					this.AppendRestartTime(DateTime.UtcNow.ToString());
					this.Broker.RequestRestart(this.Result.ResultName, this.Result.ResultId);
				}
			}, cancellationToken, TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.NotOnFaulted | TaskContinuationOptions.NotOnCanceled);
		}

		private void AppendRestartTime(string restartTimeString)
		{
			if (string.IsNullOrWhiteSpace(base.Result.StateAttribute2))
			{
				base.Result.StateAttribute2 = restartTimeString;
				return;
			}
			base.Result.StateAttribute2 = string.Format("{0}|{1}", base.Result.StateAttribute2, restartTimeString);
		}
	}
}
