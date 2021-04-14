using System;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Threading;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Probes
{
	public class ObserverHeartbeatProbe : ProbeWorkItem
	{
		public static ProbeDefinition CreateDefinition(string probeName, string subject, TimeSpan recurrenceInterval, TimeSpan missingResponderResultLimit, TracingContext context)
		{
			WTFDiagnostics.TraceFunction(ExTraceGlobals.CommonComponentsTracer, context, "Creating Observer Heartbeat Probe definition", null, "CreateDefinition", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HM\\ObserverHeartbeatProbe.cs", 139);
			ProbeDefinition probeDefinition = new ProbeDefinition();
			probeDefinition.AssemblyPath = ObserverHeartbeatProbe.AssemblyPath;
			probeDefinition.TypeName = ObserverHeartbeatProbe.TypeName;
			probeDefinition.Name = probeName;
			probeDefinition.ServiceName = ExchangeComponent.RemoteMonitoring.Name;
			probeDefinition.RecurrenceIntervalSeconds = (int)recurrenceInterval.TotalSeconds;
			probeDefinition.TimeoutSeconds = probeDefinition.RecurrenceIntervalSeconds / 2;
			probeDefinition.MaxRetryAttempts = 0;
			probeDefinition.TargetResource = subject;
			probeDefinition.Attributes.Add("MissingResponderResultLimit", missingResponderResultLimit.ToString());
			return probeDefinition;
		}

		internal static void CheckHeartbeat(IObserver self, ISubject subject, TimeSpan missingResponderResultLimit, TracingContext traceContext, out ObserverHeartbeatResult result, out DateTime? lastResultTimestamp, out IPStatus? pingReplyStatus, out TimeSpan crimsonQueryDuration, out TimeSpan rpcDuration, out TimeSpan pingDuration)
		{
			result = ObserverHeartbeatResult.None;
			lastResultTimestamp = null;
			pingReplyStatus = null;
			crimsonQueryDuration = TimeSpan.Zero;
			rpcDuration = TimeSpan.Zero;
			pingDuration = TimeSpan.Zero;
			if (!self.GetAllSubjects().Any((ISubject element) => MonitoringServerManager.IsSameServer(element.ServerName, subject.ServerName)))
			{
				WTFDiagnostics.TraceInformation<ISubject>(ExTraceGlobals.HeartbeatTracer, traceContext, "We are no longer supposed to observe subject '{0}'. This probe will continue to run, but do nothing, until the next restart of the Health Manager; then it won't be recreated.", subject, null, "CheckHeartbeat", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HM\\ObserverHeartbeatProbe.cs", 196);
				result = ObserverHeartbeatResult.NoLongerObserver;
				return;
			}
			try
			{
				if (subject.IsInMaintenance)
				{
					WTFDiagnostics.TraceWarning<ISubject>(ExTraceGlobals.HeartbeatTracer, traceContext, "Subject '{0}' has an AD entry that indicates monitoring state is Offline.", subject, null, "CheckHeartbeat", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HM\\ObserverHeartbeatProbe.cs", 206);
					result = ObserverHeartbeatResult.MonitoringOffline;
					return;
				}
			}
			catch (ServerNotFoundException)
			{
				WTFDiagnostics.TraceWarning<ISubject>(ExTraceGlobals.HeartbeatTracer, traceContext, "Subject '{0}' could not be found in AD, so removing it from the list of subjects.", subject, null, "CheckHeartbeat", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HM\\ObserverHeartbeatProbe.cs", 213);
				self.RemoveSubject(subject);
				result = ObserverHeartbeatResult.NoLongerObserver;
				return;
			}
			WTFDiagnostics.TraceInformation<ISubject>(ExTraceGlobals.HeartbeatTracer, traceContext, "Sending heartbeat RPC to subject {0}", subject, null, "CheckHeartbeat", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HM\\ObserverHeartbeatProbe.cs", 223);
			ObserverHeartbeatResponse observerHeartbeatResponse;
			Utils.Measure<ObserverHeartbeatResponse>(() => self.SendHeartbeat(subject), out observerHeartbeatResponse, out rpcDuration);
			if (observerHeartbeatResponse == ObserverHeartbeatResponse.UnknownObserver)
			{
				WTFDiagnostics.TraceWarning<ISubject>(ExTraceGlobals.HeartbeatTracer, traceContext, "Subject '{0}' responded with unknown observer, so removing it from the list of subjects.", subject, null, "CheckHeartbeat", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HM\\ObserverHeartbeatProbe.cs", 232);
				self.RemoveSubject(subject);
				result = ObserverHeartbeatResult.NoLongerObserver;
				return;
			}
			if (observerHeartbeatResponse == ObserverHeartbeatResponse.NoResponse)
			{
				WTFDiagnostics.TraceWarning<ISubject>(ExTraceGlobals.HeartbeatTracer, traceContext, "Subject '{0}' did not respond to heartbeat, checking if it is pingable.", subject, null, "CheckHeartbeat", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HM\\ObserverHeartbeatProbe.cs", 243);
				Utils.Measure<IPStatus?>(() => new IPStatus?(subject.Ping(ObserverHeartbeatProbe.DefaultPingTimeout)), out pingReplyStatus, out pingDuration);
				if (pingReplyStatus == IPStatus.Success)
				{
					WTFDiagnostics.TraceInformation<ISubject>(ExTraceGlobals.HeartbeatTracer, traceContext, "Subject '{0}' responding to pings.", subject, null, "CheckHeartbeat", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HM\\ObserverHeartbeatProbe.cs", 249);
					result |= ObserverHeartbeatResult.ServiceNotResponsive;
					return;
				}
				WTFDiagnostics.TraceWarning<ISubject>(ExTraceGlobals.HeartbeatTracer, traceContext, "Subject '{0}' did not respond to ping.", subject, null, "CheckHeartbeat", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HM\\ObserverHeartbeatProbe.cs", 254);
				result |= ObserverHeartbeatResult.MachineNotResponsive;
				return;
			}
			else
			{
				Utils.Measure<DateTime?>(() => subject.GetLastResultTimestamp(), out lastResultTimestamp, out crimsonQueryDuration);
				if (lastResultTimestamp == null)
				{
					WTFDiagnostics.TraceWarning<ISubject>(ExTraceGlobals.HeartbeatTracer, traceContext, "No responder result found on subject {0}.", subject, null, "CheckHeartbeat", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HM\\ObserverHeartbeatProbe.cs", 264);
					result |= ObserverHeartbeatResult.NoResponderResult;
					return;
				}
				if (DateTime.UtcNow - lastResultTimestamp >= missingResponderResultLimit)
				{
					WTFDiagnostics.TraceWarning<ISubject, DateTime?, TimeSpan>(ExTraceGlobals.HeartbeatTracer, traceContext, "Last responder result on subject '{0}' was at '{1}' which is older than the threshold of '{2}'.", subject, lastResultTimestamp, missingResponderResultLimit, null, "CheckHeartbeat", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HM\\ObserverHeartbeatProbe.cs", 269);
					result |= ObserverHeartbeatResult.OldResponderResult;
					return;
				}
				result = ObserverHeartbeatResult.Success;
			}
		}

		protected override void DoWork(CancellationToken cancellationToken)
		{
			WTFDiagnostics.TraceFunction<string>(ExTraceGlobals.HeartbeatTracer, base.TraceContext, "Starting Observer Heartbeat Probe for subject {0}", base.Definition.TargetResource, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HM\\ObserverHeartbeatProbe.cs", 304);
			AttributeHelper attributeHelper = new AttributeHelper(base.Definition);
			TimeSpan timeSpan = attributeHelper.GetTimeSpan("MissingResponderResultLimit", false, ObserverHeartbeatProbe.DefaultMissingResponderResultLimit, null, null);
			WTFDiagnostics.TraceInformation<TimeSpan>(ExTraceGlobals.HeartbeatTracer, base.TraceContext, "Missing Responder Result Limit = '{0}'", timeSpan, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HM\\ObserverHeartbeatProbe.cs", 309);
			ObserverHeartbeatResult observerHeartbeatResult;
			DateTime? dateTime;
			IPStatus? ipstatus;
			TimeSpan timeSpan2;
			TimeSpan timeSpan3;
			TimeSpan timeSpan4;
			ObserverHeartbeatProbe.CheckHeartbeat(MonitoringServerManager.ObserverFactory.CreateObserver(NativeHelpers.GetLocalComputerFqdn(true)), MonitoringServerManager.SubjectFactory.CreateSubject(base.Definition.TargetResource), timeSpan, base.TraceContext, out observerHeartbeatResult, out dateTime, out ipstatus, out timeSpan2, out timeSpan3, out timeSpan4);
			base.Result.StateAttribute1 = observerHeartbeatResult.ToString();
			base.Result.StateAttribute2 = dateTime.ToString();
			base.Result.StateAttribute3 = ipstatus.ToString();
			base.Result.StateAttribute6 = timeSpan2.TotalMilliseconds;
			base.Result.StateAttribute7 = timeSpan3.TotalMilliseconds;
			base.Result.StateAttribute8 = timeSpan4.TotalMilliseconds;
			if (!observerHeartbeatResult.Succeeded())
			{
				throw new Exception(string.Format("Heartbeat failure for subject '{0}' - reason: {1}", base.Definition.TargetResource, observerHeartbeatResult.ToString()));
			}
		}

		internal static readonly TimeSpan DefaultMissingResponderResultLimit = TimeSpan.FromMinutes(5.0);

		internal static readonly TimeSpan DefaultPingTimeout = TimeSpan.FromSeconds(10.0);

		private static readonly string AssemblyPath = Assembly.GetExecutingAssembly().Location;

		private static readonly string TypeName = typeof(ObserverHeartbeatProbe).FullName;
	}
}
