using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.HighAvailability.Probes
{
	public abstract class ReplicationHealthChecksProbeBase : ProbeWorkItem
	{
		private string ServerName
		{
			get
			{
				return Environment.MachineName;
			}
		}

		private IADServer ServerObj
		{
			get
			{
				return this.serverObj;
			}
			set
			{
				this.serverObj = value;
			}
		}

		private string MomEventSource
		{
			get
			{
				return "ActiveMonitoringProbe";
			}
		}

		private IEventManager EventManager
		{
			get
			{
				return new ReplicationEventManager();
			}
		}

		protected abstract Type GetCheckType();

		protected override void DoWork(CancellationToken cancellationToken)
		{
			if (HighAvailabilityUtility.CheckCancellationRequested(cancellationToken))
			{
				base.Result.StateAttribute1 = "Cancellation Requested!";
				return;
			}
			this.InitializeEnvironmentForReplicationCheck();
			this.RunReplicationCheck(this.GetCheckType());
		}

		private static bool AreConfigBitsSet(ServerConfig configuration, ServerConfig comparisonBits)
		{
			if (comparisonBits == ServerConfig.Unknown)
			{
				throw new ArgumentException("comparisonBits cannot be Unknown.", "comparisonBits");
			}
			return (configuration & comparisonBits) == comparisonBits;
		}

		private void InitializeEnvironmentForReplicationCheck()
		{
			this.ServerObj = CachedAdReader.Instance.LocalServer;
			ReplicationCheckGlobals.Server = this.ServerObj;
			this.BuildServerConfiguration(ReplicationCheckGlobals.Server);
			ReplicationCheckGlobals.ServerConfiguration = this.serverConfigBitfield;
			ReplicationCheckGlobals.ActiveManagerCheckHasRun = false;
			ReplicationCheckGlobals.ReplayServiceCheckHasRun = false;
			ReplicationCheckGlobals.TasksRpcListenerCheckHasRun = false;
			ReplicationCheckGlobals.TcpListenerCheckHasRun = false;
			ReplicationCheckGlobals.ThirdPartyReplCheckHasRun = false;
			ReplicationCheckGlobals.ServerLocatorServiceCheckHasRun = false;
		}

		private void RunReplicationCheck(Type checkType)
		{
			WTFDiagnostics.TraceDebug<string>(ExTraceGlobals.HighAvailabilityTracer, base.TraceContext, "RhChecksProbeBase:: RunReplicationCheck(): Instantiating Object {0}.", checkType.FullName, null, "RunReplicationCheck", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HighAvailability\\Probes\\ReplicationHealthChecksProbeBase.cs", 181);
			using (ReplicationCheck replicationCheck = (typeof(DagMemberCheck).IsAssignableFrom(checkType) || checkType == typeof(TcpListenerCheck)) ? ((ReplicationCheck)Activator.CreateInstance(checkType, new object[]
			{
				this.ServerName,
				this.EventManager,
				this.MomEventSource,
				CachedAdReader.Instance.LocalDAG
			})) : ((ReplicationCheck)Activator.CreateInstance(checkType, new object[]
			{
				this.ServerName,
				this.EventManager,
				this.MomEventSource
			})))
			{
				Exception ex = null;
				try
				{
					WTFDiagnostics.TraceDebug<string>(ExTraceGlobals.HighAvailabilityTracer, base.TraceContext, "RhChecksProbeBase:: RunReplicationCheck(): Invoke Run() for {0}.", checkType.FullName, null, "RunReplicationCheck", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HighAvailability\\Probes\\ReplicationHealthChecksProbeBase.cs", 207);
					replicationCheck.Run();
					WTFDiagnostics.TraceDebug<string>(ExTraceGlobals.HighAvailabilityTracer, base.TraceContext, "RhChecksProbeBase:: RunReplicationCheck(): Run() for {0} finished without any Exception logged.", checkType.FullName, null, "RunReplicationCheck", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HighAvailability\\Probes\\ReplicationHealthChecksProbeBase.cs", 215);
				}
				catch (Exception ex2)
				{
					WTFDiagnostics.TraceDebug<string, string>(ExTraceGlobals.HighAvailabilityTracer, base.TraceContext, "RhChecksProbeBase:: RunReplicationCheck(): Run() for {0} threw Exception - {1}.", checkType.FullName, ex2.ToString(), null, "RunReplicationCheck", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HighAvailability\\Probes\\ReplicationHealthChecksProbeBase.cs", 223);
					if (!(ex2 is ReplicationCheckHighPriorityFailedException) && !(ex2 is ReplicationCheckFailedException))
					{
						throw ex2;
					}
					ex = ex2;
				}
				bool flag = true;
				StringBuilder stringBuilder = new StringBuilder();
				if (ReplicationHealthChecksProbeBase.DagMemberOnlyChecksTypeNames.Contains(checkType.FullName) && !this.AreConfigBitsSet(ServerConfig.DagMember))
				{
					flag = true;
					stringBuilder.AppendFormat("Suppressed Check '{0}' due to {1} not a DAG Member!{2}", checkType.FullName, this.ServerName, Environment.NewLine);
				}
				else
				{
					if (ex != null)
					{
						flag = false;
						stringBuilder.AppendFormat("Check '{0}' thrown an Exception!{1}Exception - {2}{1}", checkType.FullName, Environment.NewLine, ex.ToString());
					}
					if (!replicationCheck.HasPassed)
					{
						flag = false;
						stringBuilder.AppendFormat("Check '{0}' did not Pass!{1}Detail Message - {2}{1}", checkType.FullName, Environment.NewLine, (replicationCheck.GetCheckOutcome().Error == null) ? "NULL" : replicationCheck.GetCheckOutcome().Error);
					}
					if (!replicationCheck.HasRun)
					{
						flag = false;
						stringBuilder.AppendFormat("Check '{0}' did not Run!{1}Detail Message - {2}{1}", checkType.FullName, Environment.NewLine, (replicationCheck.GetCheckOutcome().Error == null) ? "NULL" : replicationCheck.GetCheckOutcome().Error);
					}
				}
				string text = stringBuilder.ToString();
				base.Result.StateAttribute1 = this.ServerName;
				base.Result.StateAttribute2 = checkType.Name;
				base.Result.StateAttribute3 = (flag ? "Pass" : "Fail");
				base.Result.StateAttribute4 = ((replicationCheck.GetCheckOutcome().Error == null) ? "NULL" : replicationCheck.GetCheckOutcome().Error);
				base.Result.StateAttribute5 = text;
				if (!flag)
				{
					WTFDiagnostics.TraceError<string, string, bool, string>(ExTraceGlobals.HighAvailabilityTracer, base.TraceContext, "RhChecksProbeBase:: RunReplicationCheck: Server={0}, Check={1}, CheckRan={2}, Outcome={3}", this.ServerName, checkType.Name, replicationCheck.HasRun, (replicationCheck.GetCheckOutcome().Error == null) ? "NULL" : replicationCheck.GetCheckOutcome().Error, null, "RunReplicationCheck", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HighAvailability\\Probes\\ReplicationHealthChecksProbeBase.cs", 306);
					throw new Exception(text);
				}
				WTFDiagnostics.TraceInformation<string, string, bool, string>(ExTraceGlobals.HighAvailabilityTracer, base.TraceContext, "RhChecksProbeBase:: RunReplicationCheck: Server={0}, Check={1}, CheckRan={2}, Outcome={3}", this.ServerName, checkType.Name, replicationCheck.HasRun, (replicationCheck.GetCheckOutcome().Error == null) ? "NULL" : replicationCheck.GetCheckOutcome().Error, null, "RunReplicationCheck", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HighAvailability\\Probes\\ReplicationHealthChecksProbeBase.cs", 317);
			}
		}

		private void BuildServerConfiguration(IADServer serverObj)
		{
			this.serverConfigBitfield = ServerConfig.Unknown;
			if (serverObj.DatabaseAvailabilityGroup != null)
			{
				WTFDiagnostics.TraceDebug<string>(ExTraceGlobals.HighAvailabilityTracer, base.TraceContext, "RhChecksProbeBase:: BuildReplayConfiguration(): {0} is a DAG Member.", this.ServerName, null, "BuildServerConfiguration", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HighAvailability\\Probes\\ReplicationHealthChecksProbeBase.cs", 340);
				this.serverConfigBitfield |= ServerConfig.DagMember;
			}
			WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.HighAvailabilityTracer, base.TraceContext, "RhChecksProbeBase:: BuildReplayConfiguration(): The following bits are set on localConfigBitfield: {0}", this.serverConfigBitfield.ToString(), null, "BuildServerConfiguration", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HighAvailability\\Probes\\ReplicationHealthChecksProbeBase.cs", 348);
		}

		private bool AreConfigBitsSet(ServerConfig configBits)
		{
			return ReplicationHealthChecksProbeBase.AreConfigBitsSet(this.serverConfigBitfield, configBits);
		}

		private static readonly List<string> DagMemberOnlyChecksTypeNames = new List<string>(new string[]
		{
			typeof(ClusterRpcCheck).FullName,
			typeof(TcpListenerCheck).FullName,
			typeof(QuorumGroupCheck).FullName,
			typeof(ClusterNetworkCheck).FullName,
			typeof(ServerLocatorProbe).FullName
		});

		private ServerConfig serverConfigBitfield;

		private IADServer serverObj;
	}
}
