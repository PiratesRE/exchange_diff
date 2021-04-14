using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring
{
	public sealed class SubjectMaintenance : MaintenanceWorkItem
	{
		internal static void SelectObservers(ISubject self, List<IObserver> candidateObservers, int maxObservers, TracingContext traceContext, out int requests, out int accepts)
		{
			requests = 0;
			accepts = 0;
			Random random = new Random(self.ServerName.GetHashCode());
			while (candidateObservers.Count > 0 && maxObservers > 0)
			{
				WTFDiagnostics.TraceInformation<int, int>(ExTraceGlobals.HeartbeatTracer, traceContext, "Looking for '{0}' more observers among '{1}' servers.", maxObservers, candidateObservers.Count, null, "SelectObservers", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HM\\SubjectMaintenance.cs", 127);
				int index = random.Next(candidateObservers.Count);
				IObserver observer = candidateObservers[index];
				candidateObservers.RemoveAt(index);
				WTFDiagnostics.TraceInformation<IObserver>(ExTraceGlobals.HeartbeatTracer, traceContext, "Sending request RPC to target '{0}'.", observer, null, "SelectObservers", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HM\\SubjectMaintenance.cs", 136);
				bool flag = self.SendRequest(observer);
				WTFDiagnostics.TraceInformation<IObserver, bool>(ExTraceGlobals.HeartbeatTracer, traceContext, "Target '{0}' responded with isAccepted = '{1}'.", observer, flag, null, "SelectObservers", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HM\\SubjectMaintenance.cs", 141);
				if (flag)
				{
					bool flag2 = self.TryAddObserver(observer);
					if (flag2)
					{
						accepts++;
						maxObservers--;
					}
					else
					{
						WTFDiagnostics.TraceError<IObserver>(ExTraceGlobals.HeartbeatTracer, traceContext, "Unable to add '{0}' to list of observers.", observer, null, "SelectObservers", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HM\\SubjectMaintenance.cs", 161);
					}
				}
				requests++;
				if (requests >= MonitoringServerManager.MaxRequestObservers)
				{
					WTFDiagnostics.TraceWarning<int>(ExTraceGlobals.HeartbeatTracer, traceContext, "Reached maximum allowed number of requests of '{0}', forcibly terminating the observer selection loop.", MonitoringServerManager.MaxRequestObservers, null, "SelectObservers", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HM\\SubjectMaintenance.cs", 169);
					break;
				}
			}
			MonitoringServerManager.UpdateLastObserverSelectionTimestamp();
		}

		internal static void CalculateRequiredObserverCount(ISubject self, TimeSpan maxSelectionInterval, TimeSpan missingObserverHeartbeatLimit, TracingContext traceContext, out int requiredObservers, out ObserverSelectionReason reason, out List<IObserver> observersWithNoHeartbeat, out List<IObserver> observersWithOldHeartbeat)
		{
			requiredObservers = 0;
			reason = ObserverSelectionReason.None;
			observersWithNoHeartbeat = new List<IObserver>();
			observersWithOldHeartbeat = new List<IObserver>();
			if (!self.IsInMaintenance)
			{
				DateTime? lastObserverSelectionTimestamp = self.GetLastObserverSelectionTimestamp();
				if (lastObserverSelectionTimestamp == null)
				{
					WTFDiagnostics.TraceInformation(ExTraceGlobals.CommonComponentsTracer, traceContext, "Observer selection timestamp not found.", null, "CalculateRequiredObserverCount", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HM\\SubjectMaintenance.cs", 214);
					requiredObservers = MonitoringServerManager.MaxObservers;
					reason |= ObserverSelectionReason.NoSelectionTimestamp;
					return;
				}
				if (DateTime.UtcNow - lastObserverSelectionTimestamp.Value >= maxSelectionInterval)
				{
					WTFDiagnostics.TraceInformation<DateTime, double>(ExTraceGlobals.CommonComponentsTracer, traceContext, "Observer selection timestamp = '{0}' which is more than '{1}' minutes old.", lastObserverSelectionTimestamp.Value, maxSelectionInterval.TotalMinutes, null, "CalculateRequiredObserverCount", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HM\\SubjectMaintenance.cs", 221);
					requiredObservers = MonitoringServerManager.MaxObservers;
					reason |= ObserverSelectionReason.OldSelectionTimestamp;
					using (IEnumerator<IObserver> enumerator = self.GetAllObservers().GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							IObserver observer = enumerator.Current;
							self.RemoveObserver(observer);
						}
						return;
					}
				}
				IEnumerable<IObserver> allObservers = self.GetAllObservers();
				if (allObservers.Count<IObserver>() < MonitoringServerManager.MaxObservers)
				{
					WTFDiagnostics.TraceInformation<int, int>(ExTraceGlobals.CommonComponentsTracer, traceContext, "Found '{0}' observers which is less than the max observers = '{1}'.", allObservers.Count<IObserver>(), MonitoringServerManager.MaxObservers, null, "CalculateRequiredObserverCount", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HM\\SubjectMaintenance.cs", 242);
					requiredObservers = MonitoringServerManager.MaxObservers - allObservers.Count<IObserver>();
					reason |= ObserverSelectionReason.NotEnoughObservers;
				}
				using (IEnumerator<IObserver> enumerator2 = allObservers.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						IObserver observer2 = enumerator2.Current;
						DateTime? observerHeartbeat = self.GetObserverHeartbeat(observer2);
						if (observerHeartbeat == null)
						{
							WTFDiagnostics.TraceInformation<IObserver>(ExTraceGlobals.CommonComponentsTracer, traceContext, "Heartbeat timestamp not found for observer '{0}'.", observer2, null, "CalculateRequiredObserverCount", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HM\\SubjectMaintenance.cs", 260);
							self.RemoveObserver(observer2);
							requiredObservers++;
							reason |= ObserverSelectionReason.NoObserverTimestamp;
							observersWithNoHeartbeat.Add(observer2);
							self.SendCancel(observer2);
						}
						else if (DateTime.UtcNow - observerHeartbeat.Value >= missingObserverHeartbeatLimit)
						{
							WTFDiagnostics.TraceInformation<IObserver, DateTime, double>(ExTraceGlobals.CommonComponentsTracer, traceContext, "Heartbeat timestamp for observer '{0}' = '{1}' which is more than '{2}' minutes old.", observer2, observerHeartbeat.Value, missingObserverHeartbeatLimit.TotalMinutes, null, "CalculateRequiredObserverCount", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HM\\SubjectMaintenance.cs", 274);
							self.RemoveObserver(observer2);
							requiredObservers++;
							reason |= ObserverSelectionReason.OldObserverTimestamp;
							observersWithOldHeartbeat.Add(observer2);
							self.SendCancel(observer2);
						}
					}
					return;
				}
			}
			WTFDiagnostics.TraceInformation(ExTraceGlobals.CommonComponentsTracer, traceContext, "Not running selection algorithm because this server is in maintenance.", null, "CalculateRequiredObserverCount", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HM\\SubjectMaintenance.cs", 297);
			reason |= ObserverSelectionReason.NoneInMaintenance;
		}

		protected override void DoWork(CancellationToken cancellationToken)
		{
			WTFDiagnostics.TraceFunction(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, "Start discovery for Subject Maintenance.", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HM\\SubjectMaintenance.cs", 322);
			if (FfoLocalEndpointManager.IsForefrontForOfficeDatacenter)
			{
				WTFDiagnostics.TraceFunction(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, "Exiting Subject Maintenance for FFO DC.", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HM\\SubjectMaintenance.cs", 327);
				return;
			}
			AttributeHelper attributeHelper = new AttributeHelper(base.Definition);
			TimeSpan timeSpan = attributeHelper.GetTimeSpan("SelectionIntervalInMinutes", false, SubjectMaintenance.DefaultSelectionInterval, null, null);
			TimeSpan timeSpan2 = attributeHelper.GetTimeSpan("MissingObserverHeartbeatLimitInMinutes", false, SubjectMaintenance.DefaultMissingObserverHeartbeatLimit, null, null);
			bool @bool = attributeHelper.GetBool("AlertIfNotEnoughObservers", false, true);
			WTFDiagnostics.TraceInformation<TimeSpan, TimeSpan>(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, "Configurable properties are: Maximum Selection Interval = '{0}', Missing Observer Heartbeat Limit = '{1}'.", timeSpan, timeSpan2, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HM\\SubjectMaintenance.cs", 337);
			ISubject self = MonitoringServerManager.SubjectFactory.CreateSubject(NativeHelpers.GetLocalComputerFqdn(true));
			int num;
			ObserverSelectionReason observerSelectionReason;
			List<IObserver> source;
			List<IObserver> source2;
			SubjectMaintenance.CalculateRequiredObserverCount(self, timeSpan, timeSpan2, base.TraceContext, out num, out observerSelectionReason, out source, out source2);
			base.Result.StateAttribute1 = observerSelectionReason.ToString();
			base.Result.StateAttribute2 = string.Join(",", from observer in source
			select observer.ServerName);
			base.Result.StateAttribute3 = string.Join(",", from observer in source2
			select observer.ServerName);
			if (num == 0)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.HeartbeatTracer, base.TraceContext, "We have enough observers at this time.", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HM\\SubjectMaintenance.cs", 368);
				return;
			}
			List<string> list;
			TimeSpan timeSpan3;
			Utils.Measure<List<string>>(() => DirectoryAccessor.Instance.GetCandidateObservers(), out list, out timeSpan3);
			base.Result.StateAttribute6 = timeSpan3.TotalMilliseconds;
			base.Result.StateAttribute7 = (double)list.Count;
			WTFDiagnostics.TraceInformation<int>(ExTraceGlobals.HeartbeatTracer, base.TraceContext, "Found '{0}' machines as observer candidates.", list.Count, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HM\\SubjectMaintenance.cs", 380);
			if (list.Count == 0)
			{
				return;
			}
			List<IObserver> list2 = new List<IObserver>();
			foreach (string serverName in list)
			{
				list2.Add(MonitoringServerManager.ObserverFactory.CreateObserver(serverName));
			}
			int num2 = list2.FindIndex((IObserver server) => MonitoringServerManager.IsSameServer(server.ServerName, self.ServerName));
			if (list2.Count == 1 && num2 != -1)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.HeartbeatTracer, base.TraceContext, "Not removing self from list because the list has only one machine.", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HM\\SubjectMaintenance.cs", 409);
			}
			else
			{
				if (num2 != -1)
				{
					WTFDiagnostics.TraceInformation(ExTraceGlobals.HeartbeatTracer, base.TraceContext, "Removing self from list of candidate observers.", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HM\\SubjectMaintenance.cs", 416);
					list2.RemoveAt(num2);
				}
				IEnumerable<IObserver> allObservers = self.GetAllObservers();
				using (IEnumerator<IObserver> enumerator2 = allObservers.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						IObserver currentObserver = enumerator2.Current;
						num2 = list2.FindIndex((IObserver candidate) => MonitoringServerManager.IsSameServer(currentObserver.ServerName, candidate.ServerName));
						if (num2 != -1)
						{
							WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.HeartbeatTracer, base.TraceContext, "Discarding '{0}' as an observer candidate because it is already observing us.", currentObserver.ServerName, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HM\\SubjectMaintenance.cs", 427);
							list2.RemoveAt(num2);
						}
					}
				}
			}
			int num3;
			int num4;
			SubjectMaintenance.SelectObservers(self, list2, num, base.TraceContext, out num3, out num4);
			base.Result.StateAttribute8 = (double)num3;
			base.Result.StateAttribute9 = (double)num4;
			IEnumerable<IObserver> allObservers2 = self.GetAllObservers();
			int num5 = allObservers2.Count<IObserver>();
			int num6 = Math.Min(MonitoringServerManager.MaxObservers, list2.Count);
			if (num5 < num6 && VariantConfiguration.InvariantNoFlightingSnapshot.ActiveMonitoring.SubjectMaintenance.Enabled && @bool)
			{
				string message = string.Format("The number of machines monitoring this machine is '{0}' which is less than the expected value of '{1}'.", num5, num6);
				WTFDiagnostics.TraceError(ExTraceGlobals.HeartbeatTracer, base.TraceContext, message, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HM\\SubjectMaintenance.cs", 452);
				throw new Exception(message);
			}
		}

		private const string SelectionIntervalName = "SelectionIntervalInMinutes";

		private const string MissingObserverHeartbeatLimitName = "MissingObserverHeartbeatLimitInMinutes";

		private const string AlertIfNotEnoughObserversName = "AlertIfNotEnoughObservers";

		internal static readonly TimeSpan DefaultSelectionInterval = TimeSpan.FromDays(3.0);

		internal static readonly TimeSpan DefaultMissingObserverHeartbeatLimit = TimeSpan.FromMinutes(15.0);
	}
}
