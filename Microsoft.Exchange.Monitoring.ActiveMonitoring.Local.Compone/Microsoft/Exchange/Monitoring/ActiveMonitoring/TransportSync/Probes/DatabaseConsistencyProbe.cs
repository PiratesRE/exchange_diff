using System;
using System.ServiceProcess;
using System.Threading;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.TransportSync.Exceptions;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.TransportSync.Probes
{
	public class DatabaseConsistencyProbe : ProbeWorkItem
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			string targetResource = base.Definition.TargetResource;
			using (ServiceController serviceController = new ServiceController(Configurations.TransportSyncManagerServiceName))
			{
				ServiceControllerStatus status = serviceController.Status;
				if (!status.Equals(ServiceControllerStatus.Running))
				{
					return;
				}
			}
			if (DirectoryAccessor.Instance.IsServerComponentOnline(base.Definition.TargetGroup, ServerComponentEnum.HubTransport))
			{
				LocalEndpointManager instance = LocalEndpointManager.Instance;
				foreach (MailboxDatabaseInfo mailboxDatabaseInfo in instance.MailboxDatabaseEndpoint.MailboxDatabaseInfoCollectionForBackend)
				{
					if (mailboxDatabaseInfo.MailboxDatabaseName.Equals(targetResource, StringComparison.OrdinalIgnoreCase))
					{
						WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.TransportSyncTracer, base.TraceContext, "Check if the status of the database {0} between Store and TransportSync is consistent or not", mailboxDatabaseInfo.MailboxDatabaseName, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\TransportSync\\Probes\\DatabaseConsistencyProbe.cs", 60);
						this.CheckDatabaseConsistencyForTransportSync(mailboxDatabaseInfo);
						break;
					}
				}
				return;
			}
			base.Result.StateAttribute15 = "HubTransport is offline";
		}

		private void CheckDatabaseConsistencyForTransportSync(MailboxDatabaseInfo dbInfo)
		{
			ProbeResult result = base.Result;
			string mailboxDatabaseName = dbInfo.MailboxDatabaseName;
			string text = dbInfo.MailboxDatabaseGuid.ToString();
			WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.TransportSyncTracer, base.TraceContext, "Check if the database {0} is an active copy and mounted or not", mailboxDatabaseName, null, "CheckDatabaseConsistencyForTransportSync", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\TransportSync\\Probes\\DatabaseConsistencyProbe.cs", 95);
			if (DirectoryAccessor.Instance.IsDatabaseCopyActiveOnLocalServer(dbInfo.MailboxDatabaseGuid))
			{
				result.ResultType = ResultType.Failed;
				result.StateAttribute1 = mailboxDatabaseName;
				result.StateAttribute2 = text;
				result.StateAttribute3 = "ActiveCopy=true";
				WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.TransportSyncTracer, base.TraceContext, "Check if the database {0} is loaded by Transport sync manager", mailboxDatabaseName, null, "CheckDatabaseConsistencyForTransportSync", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\TransportSync\\Probes\\DatabaseConsistencyProbe.cs", 109);
				bool flag = false;
				DiagnosticInfo diagnosticInfoInstance = this.GetDiagnosticInfoInstance();
				MdbHealthInfo healthInfoPerMdb = diagnosticInfoInstance.GetHealthInfoPerMdb(dbInfo.MailboxDatabaseGuid);
				if (healthInfoPerMdb != null)
				{
					flag = healthInfoPerMdb.Enabled;
					result.ExecutionContext = healthInfoPerMdb.ToString();
				}
				result.StateAttribute4 = flag.ToString();
				if (!flag)
				{
					throw new DatabaseNotLoadedByTransportSyncException(Strings.DBAvailableButUnloadedByTransportSyncManagerMessage(mailboxDatabaseName, text));
				}
				WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.TransportSyncTracer, base.TraceContext, "Check if subscriptions under the database {0} are out of SLA or not.", mailboxDatabaseName, null, "CheckDatabaseConsistencyForTransportSync", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\TransportSync\\Probes\\DatabaseConsistencyProbe.cs", 134);
				this.CheckTransportSyncSLAForDatabase(dbInfo, diagnosticInfoInstance);
				if (diagnosticInfoInstance.LastDatabaseDiscoveryStartTime != null)
				{
					TimeSpan t = ExDateTime.UtcNow - diagnosticInfoInstance.LastDatabaseDiscoveryStartTime.Value;
					result.StateAttribute8 = t.TotalMinutes;
					if (t > TimeSpan.FromMinutes(30.0))
					{
						throw new DatabaseDiscoveryTimeOutException(Strings.LastDBDiscoveryTimeFailedMessage(t.ToString()));
					}
				}
				result.ResultType = ResultType.Succeeded;
			}
		}

		private void CheckTransportSyncSLAForDatabase(MailboxDatabaseInfo dbInfo, DiagnosticInfo diagnosticInfo)
		{
			SlaDiagnosticInfo slainfoPerMdb = diagnosticInfo.GetSLAInfoPerMdb(dbInfo.MailboxDatabaseGuid);
			if (slainfoPerMdb != null && slainfoPerMdb.IsOutOfSla)
			{
				diagnosticInfo.Refresh(DiagnosticMode.Info);
				base.Result.StateAttribute5 = slainfoPerMdb.OutOfSlaTime.ToString();
				if (slainfoPerMdb.ItemsOutOfSla != null)
				{
					base.Result.StateAttribute6 = (double)slainfoPerMdb.ItemsOutOfSla.Value;
				}
				if (slainfoPerMdb.ItemsOutOfSlaPercent != null)
				{
					base.Result.StateAttribute7 = (double)slainfoPerMdb.ItemsOutOfSlaPercent.Value;
				}
				throw new DatabaseOutOfSlaException(Strings.TransportSyncOutOfSLA(dbInfo.MailboxDatabaseName, dbInfo.MailboxDatabaseGuid.ToString()));
			}
		}

		private DiagnosticInfo GetDiagnosticInfoInstance()
		{
			TimeSpan getExchangeDiagnosticInfoTimeout = Configurations.GetExchangeDiagnosticInfoTimeout;
			DiagnosticInfo diagnosticInfo = null;
			Exception ex = null;
			Action delegateGetDiagnosticInfo = delegate()
			{
				try
				{
					diagnosticInfo = DiagnosticInfo.GetCachedInstance();
				}
				catch (Exception ex)
				{
					ex = ex;
				}
			};
			IAsyncResult asyncResult = delegateGetDiagnosticInfo.BeginInvoke(delegate(IAsyncResult r)
			{
				delegateGetDiagnosticInfo.EndInvoke(r);
			}, null);
			if (!asyncResult.AsyncWaitHandle.WaitOne(getExchangeDiagnosticInfoTimeout))
			{
				throw new System.ServiceProcess.TimeoutException(Strings.GetDiagnosticInfoTimeoutMessage((int)getExchangeDiagnosticInfoTimeout.TotalSeconds));
			}
			if (ex != null)
			{
				throw ex;
			}
			return diagnosticInfo;
		}
	}
}
