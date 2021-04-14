using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.UM.Responders
{
	internal static class UMSipOptionsResponderUtils
	{
		public static void InvokeBaseResponderMethodIfRequired(ResponderWorkItem responder, IEnumerable<string> errorCodes, Action<CancellationToken> baseDoResponderWork, TracingContext traceContext, CancellationToken cancellationToken)
		{
			IResponderWorkBroker broker = (IResponderWorkBroker)responder.Broker;
			IDataAccessQuery<ResponderResult> lastSuccessfulResponderResult = broker.GetLastSuccessfulResponderResult(responder.Definition);
			Task<ResponderResult> task = lastSuccessfulResponderResult.ExecuteAsync(cancellationToken, traceContext);
			task.Continue(delegate(ResponderResult lastResponderResult)
			{
				DateTime startTime = DateTime.MinValue;
				if (lastResponderResult != null)
				{
					startTime = lastResponderResult.ExecutionStartTime;
				}
				IDataAccessQuery<MonitorResult> lastSuccessfulMonitorResult = broker.GetLastSuccessfulMonitorResult(responder.Definition.AlertMask, startTime, responder.Result.ExecutionStartTime);
				Task<MonitorResult> task2 = lastSuccessfulMonitorResult.ExecuteAsync(cancellationToken, traceContext);
				task2.Continue(delegate(MonitorResult lastMonitorResult)
				{
					if (lastMonitorResult != null && lastMonitorResult.IsAlert)
					{
						string stateAttribute = lastMonitorResult.StateAttribute1;
						if (!string.IsNullOrEmpty(stateAttribute))
						{
							foreach (string value in errorCodes)
							{
								if (stateAttribute.IndexOf(value, StringComparison.InvariantCultureIgnoreCase) >= 0)
								{
									baseDoResponderWork(cancellationToken);
									break;
								}
							}
						}
					}
				}, cancellationToken, TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.NotOnFaulted | TaskContinuationOptions.NotOnCanceled);
			}, cancellationToken, TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.NotOnFaulted | TaskContinuationOptions.NotOnCanceled);
		}
	}
}
