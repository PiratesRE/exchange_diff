using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Management.Automation;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;
using Microsoft.PowerShell.Commands.GetCounter;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Fips
{
	public class EngineUpdateFailuresProbe : ProbeWorkItem
	{
		private FailedUpdatesDefinition FailureDefinition
		{
			get
			{
				return FipsUtils.LoadFromContext(base.Definition.ExtensionAttributes);
			}
		}

		protected override void DoWork(CancellationToken cancellationToken)
		{
			this.TraceDebug("FailedUpdatesMonitor probe started.", new object[0]);
			List<string> enginesFailed = this.GetEnginesFailed();
			if (enginesFailed.Count >= this.FailureDefinition.NumberOfFailedEngine)
			{
				string text = string.Format("Following engines have failed updates - {0}, Update information - \r\n{1}", string.Join(",", enginesFailed.ToArray()), this.GetEngineUpdateInformation());
				base.Result.Error = text;
				this.TraceError(text, new object[0]);
				throw new ApplicationException(text);
			}
			this.TraceDebug("MonitorFailedUpdates probe finished with success.", new object[0]);
		}

		private string GetEngineUpdateInformation()
		{
			string result;
			try
			{
				Collection<PSObject> collection = FipsUtils.RunFipsCmdlet<object>("Get-EngineUpdateInformation", null);
				string text = string.Empty;
				foreach (PSObject psobject in collection)
				{
					text += string.Format("Engine\t\t\t: {0}\r\nLastChecked\t\t: {1}\r\nLastUpdated\t\t: {2}\r\nEngineVersion\t\t: {3}\r\nSignatureVersion\t: {4}\r\nSignatureDateTime\t: {5}\r\nUpdateVersion\t\t: {6}\r\nUpdateStatus\t\t: {7}\r\n\r\n", new object[]
					{
						psobject.Properties["Engine"].Value,
						psobject.Properties["LastChecked"].Value,
						psobject.Properties["LastUpdated"].Value,
						psobject.Properties["EngineVersion"].Value,
						psobject.Properties["SignatureVersion"].Value,
						psobject.Properties["SignatureDateTime"].Value,
						psobject.Properties["UpdateVersion"].Value,
						psobject.Properties["UpdateStatus"].Value
					});
				}
				result = text;
			}
			catch (Exception ex)
			{
				base.Result.Error = ex.Message;
				this.TraceError(ex.Message, new object[0]);
				result = ex.Message;
			}
			return result;
		}

		private List<string> GetEnginesFailed()
		{
			List<string> list = new List<string>();
			try
			{
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				if (PerformanceCounterCategory.Exists("[MSExchange Hygiene Updates Engine xxx xxx xxx xx]"))
				{
					dictionary.Add("Counter", "\\[msexchange hygiene updates engine xxx xxx xxx xx](*)\\[consecutive failed updates xxx xxx xxx]");
				}
				else
				{
					dictionary.Add("Counter", "\\msexchange hygiene updates engine(*)\\consecutive failed updates");
				}
				Collection<PSObject> collection = FipsUtils.RunFipsCmdlet<object>("Get-Counter", dictionary);
				if (collection != null && collection.Count > 0)
				{
					using (IEnumerator<PSObject> enumerator = collection.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							PSObject psobject = enumerator.Current;
							PerformanceCounterSample[] array = psobject.Properties["CounterSamples"].Value as PerformanceCounterSample[];
							if (array == null || array.Length <= 0)
							{
								throw new ApplicationException(string.Format("Unable to read the performance counter samples for {0}", dictionary["Counter"]));
							}
							foreach (PerformanceCounterSample performanceCounterSample in array)
							{
								if (performanceCounterSample.CookedValue >= (double)this.FailureDefinition.ConsecutiveFailures && !list.Contains(performanceCounterSample.InstanceName))
								{
									list.Add(performanceCounterSample.InstanceName);
								}
							}
						}
						goto IL_135;
					}
					goto IL_11A;
					IL_135:
					return list;
				}
				IL_11A:
				throw new ApplicationException(string.Format("Unable to read the performance counter samples for {0}", dictionary["Counter"]));
			}
			catch (Exception ex)
			{
				base.Result.Error = ex.Message;
				this.TraceError(ex.Message, new object[0]);
				throw new ApplicationException(ex.Message);
			}
			return list;
		}

		private void TraceDebug(string format, params object[] args)
		{
			string text = string.Format(format, args);
			ProbeResult result = base.Result;
			result.ExecutionContext = result.ExecutionContext + text + " ";
			WTFDiagnostics.TraceDebug(ExTraceGlobals.FIPSTracer, base.TraceContext, text, null, "TraceDebug", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\FIPS\\EngineUpdateFailuresProbe.cs", 198);
		}

		private void TraceError(string format, params object[] args)
		{
			string text = string.Format(format, args);
			ProbeResult result = base.Result;
			result.ExecutionContext = result.ExecutionContext + text + " ";
			WTFDiagnostics.TraceError(ExTraceGlobals.FIPSTracer, base.TraceContext, text, null, "TraceError", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\FIPS\\EngineUpdateFailuresProbe.cs", 210);
		}
	}
}
