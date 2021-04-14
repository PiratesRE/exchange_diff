using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Common
{
	internal class Observer : IObserver
	{
		public Observer(string serverName)
		{
			this.serverName = serverName;
		}

		public string ServerName
		{
			get
			{
				return this.serverName;
			}
		}

		public bool IsInMaintenance
		{
			get
			{
				return DirectoryAccessor.Instance.IsMonitoringOffline(this.serverName);
			}
		}

		public IEnumerable<ISubject> GetAllSubjects()
		{
			List<Subject> subjects = new List<Subject>();
			Array.ForEach<string>(MonitoringServerManager.GetAllSubjects(), delegate(string svr)
			{
				subjects.Add(new Subject(svr));
			});
			return subjects;
		}

		public bool TryAddSubject(ISubject subject)
		{
			return MonitoringServerManager.TryAddSubject(subject.ServerName);
		}

		public void RemoveSubject(ISubject subject)
		{
			MonitoringServerManager.RemoveSubject(subject.ServerName);
		}

		public ObserverHeartbeatResponse SendHeartbeat(ISubject subject)
		{
			ObserverHeartbeatResponse result = ObserverHeartbeatResponse.NoResponse;
			try
			{
				RpcObserverHeartbeatImpl.SendObserverHeartbeat(subject.ServerName, out result);
			}
			catch (ActiveMonitoringServerException arg)
			{
				WTFDiagnostics.TraceWarning<string, ActiveMonitoringServerException>(ExTraceGlobals.HeartbeatTracer, TracingContext.Default, "Observer heartbeat RPC to server '{0}' failed with exception '{1}'.", subject.ServerName, arg, null, "SendHeartbeat", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\Observer.cs", 110);
			}
			return result;
		}

		public override string ToString()
		{
			return this.serverName;
		}

		private readonly string serverName;
	}
}
