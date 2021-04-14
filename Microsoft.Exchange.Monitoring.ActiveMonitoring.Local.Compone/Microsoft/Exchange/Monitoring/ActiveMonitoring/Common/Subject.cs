using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Common
{
	internal class Subject : ISubject
	{
		public Subject(string serverName)
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

		public IEnumerable<IObserver> GetAllObservers()
		{
			List<Observer> observers = new List<Observer>();
			Array.ForEach<string>(MonitoringServerManager.GetAllObservers(), delegate(string svr)
			{
				observers.Add(new Observer(svr));
			});
			return observers;
		}

		public bool TryAddObserver(IObserver observer)
		{
			return MonitoringServerManager.TryAddObserver(observer.ServerName);
		}

		public void RemoveObserver(IObserver observer)
		{
			MonitoringServerManager.RemoveObserver(observer.ServerName);
		}

		public bool SendRequest(IObserver observer)
		{
			bool result = false;
			try
			{
				RpcRequestObserverImpl.SendRequestObserver(observer.ServerName, out result);
			}
			catch (ActiveMonitoringServerException arg)
			{
				WTFDiagnostics.TraceWarning<string, ActiveMonitoringServerException>(ExTraceGlobals.HeartbeatTracer, TracingContext.Default, "Observer request RPC to server '{0}' failed with exception '{1}'.", observer.ServerName, arg, null, "SendRequest", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\Subject.cs", 112);
			}
			return result;
		}

		public void SendCancel(IObserver observer)
		{
			try
			{
				RpcCancelObserverImpl.SendCancelObserver(observer.ServerName);
			}
			catch (ActiveMonitoringServerException arg)
			{
				WTFDiagnostics.TraceWarning<string, ActiveMonitoringServerException>(ExTraceGlobals.HeartbeatTracer, TracingContext.Default, "Observer cancel RPC to server '{0}' failed with exception '{1}'.", observer.ServerName, arg, null, "SendCancel", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\Subject.cs", 132);
			}
		}

		public DateTime? GetLastResultTimestamp()
		{
			DateTime? result;
			try
			{
				DateTime? dateTime;
				RpcGetCrimsonEventImpl.SendRequest(this.serverName, out dateTime, 30000);
				result = dateTime;
			}
			catch (Exception arg)
			{
				WTFDiagnostics.TraceWarning<string, Exception>(ExTraceGlobals.HeartbeatTracer, TracingContext.Default, "GetLastResultTimestamp failed over RPC event log query for {0} with exception '{1}'", this.serverName, arg, null, "GetLastResultTimestamp", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\Subject.cs", 160);
				if (DirectoryAccessor.Instance.IsRecoveryActionsEnabledOffline(this.serverName))
				{
					result = this.GetLastResultTimestamp<MonitorResult>("HealthManagerHeartbeatMonitor");
				}
				else
				{
					result = this.GetLastResultTimestamp<ResponderResult>("HealthManagerHeartbeatResponder");
				}
			}
			return result;
		}

		public IPStatus Ping(TimeSpan timeout)
		{
			IPStatus status;
			using (Ping ping = new Ping())
			{
				PingReply pingReply = ping.Send(this.serverName, (int)timeout.TotalMilliseconds);
				status = pingReply.Status;
			}
			return status;
		}

		public DateTime? GetLastObserverSelectionTimestamp()
		{
			return MonitoringServerManager.GetLastObserverSelectionTimestamp();
		}

		public DateTime? GetObserverHeartbeat(IObserver observer)
		{
			return MonitoringServerManager.GetObserverHeartbeat(observer.ServerName);
		}

		public override string ToString()
		{
			return this.serverName;
		}

		private DateTime? GetLastResultTimestamp<TResult>(string resultName) where TResult : WorkItemResult, IPersistence, new()
		{
			DateTime? result = null;
			TResult tresult = default(TResult);
			using (CrimsonReader<TResult> crimsonReader = new CrimsonReader<TResult>())
			{
				crimsonReader.ConnectionInfo = new CrimsonConnectionInfo(this.serverName);
				crimsonReader.QueryUserPropertyCondition = string.Format("(ResultName='{0}')", resultName);
				crimsonReader.IsReverseDirection = true;
				tresult = crimsonReader.ReadNext();
			}
			if (tresult != null)
			{
				result = new DateTime?(tresult.ExecutionEndTime);
			}
			return result;
		}

		private readonly string serverName;
	}
}
