using System;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Common
{
	internal class MonitoringEndpoint : IEndpoint
	{
		public bool IsOnline
		{
			get
			{
				return this.cachedIsOnline;
			}
		}

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
				this.cachedIsOnline = ServerComponentStateManager.IsOnline(ServerComponentEnum.Monitoring);
				if (DirectoryAccessor.Instance.Server != null)
				{
					this.monitoringGroup = DirectoryAccessor.Instance.Server.MonitoringGroup;
				}
				else if (DirectoryAccessor.Instance.Computer != null)
				{
					this.monitoringGroup = DirectoryAccessor.Instance.Computer.MonitoringGroup;
				}
			}
			catch (Exception arg)
			{
				WTFDiagnostics.TraceError<Exception>(ExTraceGlobals.MonitoringEndpointTracer, this.traceContext, "[Initialize] ServerComponentStateManager.IsOnline failed: {0}", arg, null, "Initialize", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\MonitoringEndpoint.cs", 85);
				throw;
			}
		}

		public bool DetectChange()
		{
			bool result;
			try
			{
				if (this.cachedIsOnline != ServerComponentStateManager.IsOnline(ServerComponentEnum.Monitoring))
				{
					WTFDiagnostics.TraceDebug<bool, bool>(ExTraceGlobals.MonitoringEndpointTracer, this.traceContext, "[DetectChange] ServerComponentStateManager.DetectChange: detected monitoring online state changed from {0} to {1}", this.cachedIsOnline, !this.cachedIsOnline, null, "DetectChange", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\MonitoringEndpoint.cs", 106);
					result = true;
				}
				else
				{
					DirectoryAccessor.Instance.RefreshServerOrComputerObject();
					if ((DirectoryAccessor.Instance.Server != null && string.Compare(DirectoryAccessor.Instance.Server.MonitoringGroup, this.monitoringGroup, true) != 0) || (DirectoryAccessor.Instance.Computer != null && string.Compare(DirectoryAccessor.Instance.Computer.MonitoringGroup, this.monitoringGroup, true) != 0))
					{
						WTFDiagnostics.TraceDebug<string, string>(ExTraceGlobals.MonitoringEndpointTracer, this.traceContext, "[DetectChange] ServerComponentStateManager.DetectChange: detected monitoring group changed from {0} to {1}", this.monitoringGroup, (DirectoryAccessor.Instance.Server != null) ? DirectoryAccessor.Instance.Server.MonitoringGroup : DirectoryAccessor.Instance.Computer.MonitoringGroup, null, "DetectChange", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\MonitoringEndpoint.cs", 119);
						result = true;
					}
					else
					{
						result = false;
					}
				}
			}
			catch (Exception arg)
			{
				WTFDiagnostics.TraceError<Exception>(ExTraceGlobals.MonitoringEndpointTracer, this.traceContext, "[DetectChange] ServerComponentStateManager.DetectChange failed: {0}", arg, null, "DetectChange", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\MonitoringEndpoint.cs", 132);
				throw;
			}
			return result;
		}

		private bool cachedIsOnline;

		private string monitoringGroup;

		private TracingContext traceContext = TracingContext.Default;
	}
}
