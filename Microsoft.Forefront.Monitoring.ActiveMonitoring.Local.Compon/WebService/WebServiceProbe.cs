using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ForefrontActiveMonitoring;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.WebService
{
	public class WebServiceProbe : ProbeWorkItem
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			base.Result.ExecutionContext = "WebServiceProbe started. ";
			ProbeSyncInitialRun probeSyncInitialRun = new ProbeSyncInitialRun(this);
			if (!probeSyncInitialRun.CanRun())
			{
				base.Result.SetCompleted(ResultType.Rejected, new Exception(string.Format("Skipping as the Producer probe '{0}' has not completed", probeSyncInitialRun.ProducerProbeName)));
				return;
			}
			System.Diagnostics.Trace.CorrelationManager.ActivityId = CombGuidGenerator.NewGuid();
			WTFDiagnostics.TraceInformation<Guid>(ExTraceGlobals.WebServiceTracer, new TracingContext(), "New SystemProbe Guid: {0}\r\n", System.Diagnostics.Trace.CorrelationManager.ActivityId, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\WebService\\Probes\\WebServiceProbe.cs", 52);
			WebServiceClient webServiceClient = null;
			if (this.workDefinition == null)
			{
				ProbeResult result = base.Result;
				result.ExecutionContext += "Processing work definition. ";
				this.workDefinition = new WebServiceProbeWorkDefinition(base.Definition.ExtensionAttributes);
				base.Result.StateAttribute1 = Environment.MachineName + ":" + this.workDefinition.MachineMacAddress.ToString();
				ProbeResult result2 = base.Result;
				result2.ExecutionContext += "Work definition processed. ";
			}
			try
			{
				ProbeResult result3 = base.Result;
				result3.ExecutionContext += "Creating web service proxy. ";
				webServiceClient = new WebServiceClient(this.workDefinition.WebServiceConfiguration);
				ProbeResult result4 = base.Result;
				result4.ExecutionContext += "Web service proxy created. ";
				base.Result.SampleValue = 0.0;
				using (List<Operation>.Enumerator enumerator = this.workDefinition.Operations.GetEnumerator())
				{
					IL_394:
					while (enumerator.MoveNext())
					{
						Operation operation = enumerator.Current;
						int num = 0;
						bool flag = false;
						while (!cancellationToken.IsCancellationRequested)
						{
							ProbeResult result5 = base.Result;
							result5.ExecutionContext += string.Format("Calling '{0}'. ", operation.Name);
							bool flag2 = operation.Invoke(webServiceClient, this.workDefinition.Operations, false);
							ProbeResult result6 = base.Result;
							result6.ExecutionContext += string.Format("'{0}' finished, Latency={1}, SLA met={2}.  ", operation.Name, operation.Latency.TotalMilliseconds, flag2);
							if (!flag2)
							{
								if (operation.ValidateResult(webServiceClient, false) && this.workDefinition.WebServiceConfiguration.DumpDiagnosticsInfoOnSuccess)
								{
									ProbeResult result7 = base.Result;
									result7.StateAttribute15 += operation.GetDiagnosticsInfo(webServiceClient);
								}
								if (num == operation.MaxRetryAttempts)
								{
									throw new Exception(string.Format("Operation {0} response time ({1}) exceeded SLA ({2})", operation.Name, operation.Latency, operation.Sla));
								}
							}
							else
							{
								base.Result.SampleValue += operation.Latency.TotalMilliseconds;
								ProbeResult result8 = base.Result;
								result8.ExecutionContext += string.Format("Validating result of '{0}'. ", operation.Name);
								flag = operation.ValidateResult(webServiceClient, num == operation.MaxRetryAttempts);
								ProbeResult result9 = base.Result;
								result9.ExecutionContext += string.Format("Result of '{0}' IsExpectedResult={1}. ", operation.Name, flag);
								if (flag)
								{
									if (this.workDefinition.WebServiceConfiguration.DumpDiagnosticsInfoOnSuccess)
									{
										ProbeResult result10 = base.Result;
										result10.StateAttribute15 += operation.GetDiagnosticsInfo(webServiceClient);
										goto IL_346;
									}
									goto IL_346;
								}
							}
							if (++num <= operation.MaxRetryAttempts)
							{
								continue;
							}
							IL_346:
							if (flag && operation.PauseTime > TimeSpan.Zero)
							{
								ProbeResult result11 = base.Result;
								result11.ExecutionContext += string.Format("Pausing {0}. ", operation.PauseTime);
								this.Pause(operation.PauseTime, cancellationToken);
								goto IL_394;
							}
							goto IL_394;
						}
						throw new OperationCanceledException();
					}
				}
				probeSyncInitialRun.MarkCompleted();
			}
			catch (Exception ex)
			{
				WTFDiagnostics.TraceError<string, string>(ExTraceGlobals.WebServiceTracer, new TracingContext(), "{0}\r\n{1}", ex.Message, this.GetOperationCallHistory(), null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\WebService\\Probes\\WebServiceProbe.cs", 139);
				ProbeResult result12 = base.Result;
				result12.FailureContext += string.Format("Exception: {0} ", ex.Message);
				ProbeResult result13 = base.Result;
				result13.ExecutionContext += "Catched exception";
				throw;
			}
			finally
			{
				if (webServiceClient != null)
				{
					webServiceClient.Close();
				}
			}
			WTFDiagnostics.TraceInformation(ExTraceGlobals.WebServiceTracer, new TracingContext(), "All operations completed and validated.\r\n", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\WebService\\Probes\\WebServiceProbe.cs", 152);
			ProbeResult result14 = base.Result;
			result14.ExecutionContext += "WebServiceProbe finished.";
		}

		private void Pause(TimeSpan duration, CancellationToken cancellationToken)
		{
			TimeSpan timeSpan = duration;
			TimeSpan timeSpan2 = TimeSpan.FromSeconds(1.0);
			while (timeSpan > TimeSpan.Zero && !cancellationToken.IsCancellationRequested)
			{
				if (timeSpan2 < timeSpan)
				{
					timeSpan2 = timeSpan;
				}
				Thread.Sleep(timeSpan2);
				timeSpan -= timeSpan2;
			}
		}

		private string GetOperationCallHistory()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine();
			stringBuilder.AppendLine("Operation Call History:");
			stringBuilder.AppendLine();
			foreach (Operation operation in this.workDefinition.Operations)
			{
				stringBuilder.AppendLine(operation.ToString());
			}
			return stringBuilder.ToString();
		}

		private WebServiceProbeWorkDefinition workDefinition;
	}
}
