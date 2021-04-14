using System;

namespace Microsoft.Office.Datacenter.WorkerTaskFramework
{
	public class RestartRequest
	{
		private RestartRequest()
		{
			this.Timestamp = DateTime.UtcNow;
		}

		public RestartRequestReason Reason { get; private set; }

		public Exception Exception { get; private set; }

		public string ResultName { get; private set; }

		public int ResultId { get; private set; }

		public DateTime Timestamp { get; private set; }

		public override string ToString()
		{
			string text = string.IsNullOrEmpty(this.ResultName) ? string.Empty : this.ResultName;
			if (this.Exception == null)
			{
				return string.Format("[RestartRequest at {0}]: ResultName={1}, ResultId={2}, RestartRequestReason={3}", new object[]
				{
					this.Timestamp,
					text,
					this.ResultId,
					this.Reason.ToString()
				});
			}
			return string.Format("[RestartRequest at {0}]: ResultName={1}, ResultId={2}, RestartRequestReason={3}, Exception = {4}", new object[]
			{
				this.Timestamp,
				text,
				this.ResultId,
				this.Reason.ToString(),
				this.Exception
			});
		}

		internal static RestartRequest CreateDataAccessErrorRestartRequest(Exception exception)
		{
			return RestartRequest.CreateExceptionBasedRestartRequest(RestartRequestReason.DataAccessError, exception);
		}

		internal static RestartRequest CreateUnknownRestartRequest(Exception exception)
		{
			return RestartRequest.CreateExceptionBasedRestartRequest(RestartRequestReason.Unknown, exception);
		}

		internal static RestartRequest CreatePoisonResultRestartRequest(string resultName, int resultId)
		{
			return RestartRequest.CreateResultBasedRestartRequest(RestartRequestReason.PoisonResult, resultName, resultId);
		}

		internal static RestartRequest CreateMaintenanceRestartRequest(string resultName, int resultId)
		{
			return RestartRequest.CreateResultBasedRestartRequest(RestartRequestReason.Maintenance, resultName, resultId);
		}

		internal static RestartRequest CreateSelfRecoveryBasedRestartRequest(string recoveryType, string metricName)
		{
			RestartRequest restartRequest = new RestartRequest();
			restartRequest.Reason = RestartRequestReason.SelfHealing;
			restartRequest.ResultName = string.Format("RecoveryType = {0}, MetricName = {1}", recoveryType, metricName);
			WTFDiagnostics.TraceError<DateTime, string, string>(WTFLog.WorkItem, TracingContext.Default, "[RestartRequest at {0}]: RestartRequestReason={1}, {2}", restartRequest.Timestamp, restartRequest.Reason.ToString(), restartRequest.ResultName, null, "CreateSelfRecoveryBasedRestartRequest", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\Core\\RestartRequest.cs", 137);
			return restartRequest;
		}

		private static RestartRequest CreateResultBasedRestartRequest(RestartRequestReason reason, string resultName, int resultId)
		{
			RestartRequest restartRequest = new RestartRequest();
			restartRequest.Reason = reason;
			restartRequest.ResultName = resultName;
			restartRequest.ResultId = resultId;
			WTFDiagnostics.TraceError<DateTime, string, int, string>(WTFLog.WorkItem, TracingContext.Default, "[RestartRequest at {0}]: ResultName={1}, ResultId={2}, RestartRequestReason={3}", restartRequest.Timestamp, resultName, resultId, reason.ToString(), null, "CreateResultBasedRestartRequest", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\Core\\RestartRequest.cs", 156);
			return restartRequest;
		}

		private static RestartRequest CreateExceptionBasedRestartRequest(RestartRequestReason reason, Exception exception)
		{
			RestartRequest restartRequest = new RestartRequest();
			restartRequest.Reason = reason;
			restartRequest.Exception = exception;
			WTFDiagnostics.TraceError<DateTime, string, Exception>(WTFLog.WorkItem, TracingContext.Default, "[RestartRequest at {0}]: RestartRequestReason={1}, Exception={2}", restartRequest.Timestamp, reason.ToString(), exception, null, "CreateExceptionBasedRestartRequest", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\Core\\RestartRequest.cs", 173);
			return restartRequest;
		}
	}
}
