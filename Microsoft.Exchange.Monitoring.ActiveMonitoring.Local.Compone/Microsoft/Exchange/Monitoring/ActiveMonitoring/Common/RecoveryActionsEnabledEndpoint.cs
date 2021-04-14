using System;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Common
{
	internal class RecoveryActionsEnabledEndpoint : IEndpoint
	{
		public bool RestartOnChange
		{
			get
			{
				return true;
			}
		}

		public Exception Exception { get; set; }

		public void Initialize()
		{
			try
			{
				this.cachedIsOnline = ServerComponentStateManager.IsOnline(ServerComponentEnum.RecoveryActionsEnabled);
			}
			catch (Exception ex)
			{
				WTFDiagnostics.TraceError(ExTraceGlobals.RecoveryActionsEnabledEndpointTracer, this.traceContext, string.Format("[Initialize] ServerComponentStateManager.IsOnline failed: {0}", ex.ToString()), null, "Initialize", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\RecoveryActionsEnabledEndpoint.cs", 60);
				throw;
			}
		}

		public bool DetectChange()
		{
			bool result;
			try
			{
				result = (this.cachedIsOnline != ServerComponentStateManager.IsOnline(ServerComponentEnum.RecoveryActionsEnabled));
			}
			catch (Exception ex)
			{
				WTFDiagnostics.TraceError(ExTraceGlobals.RecoveryActionsEnabledEndpointTracer, this.traceContext, string.Format("[DetectChange] ServerComponentStateManager.IsOnline failed: {0}", ex.ToString()), null, "DetectChange", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\RecoveryActionsEnabledEndpoint.cs", 82);
				throw;
			}
			return result;
		}

		private bool cachedIsOnline;

		private TracingContext traceContext = TracingContext.Default;
	}
}
