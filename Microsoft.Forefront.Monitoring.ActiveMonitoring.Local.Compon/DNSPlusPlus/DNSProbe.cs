using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Exchange.Diagnostics.Components.ForefrontActiveMonitoring;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.DNSPlusPlus
{
	public class DNSProbe : ProbeWorkItem
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			WTFDiagnostics.TraceInformation<DateTime>(ExTraceGlobals.DNSTracer, base.TraceContext, "DNSProbe: DoWork start, currentTime={0}", DateTime.UtcNow, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\DNS\\Probes\\DNSProbe.cs", 33);
			Stopwatch stopwatch = Stopwatch.StartNew();
			StringBuilder stringBuilder = new StringBuilder();
			StringBuilder stringBuilder2 = new StringBuilder();
			DNSProbeWorkDefinition dnsprobeWorkDefinition = null;
			ConcurrentBag<DnsProbeOperation> failedOperations = new ConcurrentBag<DnsProbeOperation>();
			ConcurrentBag<DnsProbeOperation> successOperations = new ConcurrentBag<DnsProbeOperation>();
			ConcurrentBag<DnsProbeOperation> successWithRetryOperations = new ConcurrentBag<DnsProbeOperation>();
			ConcurrentBag<DnsProbeOperation> skippedOperation = new ConcurrentBag<DnsProbeOperation>();
			try
			{
				Stopwatch stopwatch2 = Stopwatch.StartNew();
				dnsprobeWorkDefinition = new DNSProbeWorkDefinition(base.Definition.ExtensionAttributes, base.TraceContext);
				stopwatch2.Stop();
				stringBuilder.AppendLine(string.Format("DnsConfiguration={0}", dnsprobeWorkDefinition.Configuration));
				stringBuilder.AppendFormat("DNSProbeWorkDefinition creation took {0} milliseconds\n", stopwatch2.ElapsedMilliseconds);
				WTFDiagnostics.TraceInformation<List<DnsProbeOperation>>(ExTraceGlobals.DNSTracer, base.TraceContext, "DNSProbe: Executing operations, Count={0}", dnsprobeWorkDefinition.Operations, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\DNS\\Probes\\DNSProbe.cs", 52);
				if (cancellationToken.IsCancellationRequested)
				{
					throw new OperationCanceledException(cancellationToken);
				}
				Parallel.ForEach<DnsProbeOperation>(dnsprobeWorkDefinition.Operations, delegate(DnsProbeOperation operation, ParallelLoopState loopState)
				{
					if (cancellationToken.IsCancellationRequested)
					{
						throw new OperationCanceledException(cancellationToken);
					}
					if (operation.CanInvoke())
					{
						int num = 0;
						try
						{
							while (!operation.Invoke(cancellationToken))
							{
								if (cancellationToken.IsCancellationRequested)
								{
									throw new OperationCanceledException(cancellationToken);
								}
								if (++num > operation.ProbeRetryAttempts)
								{
									break;
								}
							}
						}
						catch
						{
							failedOperations.Add(operation);
							throw;
						}
						if (num > operation.ProbeRetryAttempts)
						{
							failedOperations.Add(operation);
							if (operation.ExitOnFailure)
							{
								loopState.Break();
								return;
							}
						}
						else
						{
							if (num == 0)
							{
								successOperations.Add(operation);
								return;
							}
							successWithRetryOperations.Add(operation);
							return;
						}
					}
					else
					{
						skippedOperation.Add(operation);
					}
				});
				WTFDiagnostics.TraceInformation(ExTraceGlobals.DNSTracer, base.TraceContext, "DNSProbe: Executing operations completed", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\DNS\\Probes\\DNSProbe.cs", 123);
				if (failedOperations.Count > 0)
				{
					throw new DnsMonitorException(string.Format("{0} Failures detected: \r\n{1}", failedOperations.Count, string.Join<DnsProbeOperation>(Environment.NewLine, failedOperations)), null);
				}
			}
			catch (Exception ex)
			{
				if (!(ex is DnsMonitorException))
				{
					WTFDiagnostics.TraceInformation<Exception>(ExTraceGlobals.DNSTracer, base.TraceContext, "Exception occured, details={0}", ex, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\DNS\\Probes\\DNSProbe.cs", 134);
				}
				throw;
			}
			finally
			{
				stopwatch.Stop();
				if (dnsprobeWorkDefinition != null && dnsprobeWorkDefinition.Operations != null)
				{
					stringBuilder.Insert(0, string.Format("Total={0}, Executed={1}, Succeeded={2}, SucceededWithRetry={3} Failed={4}, Skipped={5}\n", new object[]
					{
						dnsprobeWorkDefinition.Operations.Count,
						successOperations.Count + successWithRetryOperations.Count + failedOperations.Count,
						successOperations.Count,
						successWithRetryOperations.Count,
						failedOperations.Count,
						skippedOperation.Count
					}));
					if (skippedOperation.Count > 0)
					{
						stringBuilder.AppendLine("SkippedOperations:");
						stringBuilder.AppendLine(string.Join<DnsProbeOperation>(Environment.NewLine, skippedOperation));
					}
					if (successWithRetryOperations.Count > 0)
					{
						stringBuilder.AppendLine("SuccessWithRetryOperations:");
						stringBuilder.AppendLine(string.Join<DnsProbeOperation>(Environment.NewLine, successWithRetryOperations));
					}
					if (successOperations.Count > 0)
					{
						stringBuilder.AppendLine("SuccessOperations:");
						stringBuilder.AppendLine(string.Join<DnsProbeOperation>(Environment.NewLine, successOperations));
					}
					if (failedOperations.Count > 0)
					{
						stringBuilder2.AppendLine("FailedOperations:");
						stringBuilder2.AppendLine(string.Join<DnsProbeOperation>(Environment.NewLine, failedOperations));
					}
				}
				base.Result.SampleValue = (double)stopwatch.ElapsedMilliseconds;
				base.Result.ExecutionContext = stringBuilder.ToString();
				base.Result.FailureContext = stringBuilder2.ToString();
				WTFDiagnostics.TraceInformation<string, string>(ExTraceGlobals.DNSTracer, base.TraceContext, "DNSProbe: DoWork end, executionContext={0}, failureContext={1}", base.Result.ExecutionContext, base.Result.FailureContext, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\DNS\\Probes\\DNSProbe.cs", 183);
			}
		}
	}
}
