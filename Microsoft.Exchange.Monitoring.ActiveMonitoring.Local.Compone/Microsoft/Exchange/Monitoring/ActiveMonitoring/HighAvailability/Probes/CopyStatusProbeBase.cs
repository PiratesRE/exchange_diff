using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Rpc.Cluster;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.HighAvailability.Probes
{
	internal class CopyStatusProbeBase : ProbeWorkItem
	{
		protected CopyStatusProbeBase.CopyStatusEntry CopyStatus
		{
			get
			{
				return this.copyStatusEntry;
			}
		}

		protected static ProbeDefinition CreateDefinition(string name, string className, string serviceName, MailboxDatabaseInfo targetDatabase, int recurrenceInterval, int timeout, int maxRetry)
		{
			ProbeDefinition probeDefinition = new ProbeDefinition();
			probeDefinition.AssemblyPath = Assembly.GetExecutingAssembly().Location;
			probeDefinition.ServiceName = serviceName;
			probeDefinition.TypeName = className;
			probeDefinition.Name = name;
			probeDefinition.RecurrenceIntervalSeconds = recurrenceInterval;
			probeDefinition.TimeoutSeconds = timeout;
			probeDefinition.MaxRetryAttempts = maxRetry;
			probeDefinition.TargetResource = targetDatabase.MailboxDatabaseName;
			probeDefinition.Attributes[CopyStatusProbeBase.DBGuidAttrName] = targetDatabase.MailboxDatabaseGuid.ToString();
			return probeDefinition;
		}

		public override void PopulateDefinition<ProbeDefinition>(ProbeDefinition pDef, Dictionary<string, string> propertyBag)
		{
			if (pDef == null)
			{
				throw new ArgumentException("Please specify a value for probeDefinition");
			}
			if (propertyBag.ContainsKey(CopyStatusProbeBase.DBGuidAttrName))
			{
				pDef.Attributes[CopyStatusProbeBase.DBGuidAttrName] = propertyBag[CopyStatusProbeBase.DBGuidAttrName].ToString().Trim();
				return;
			}
			throw new ArgumentException("Please specify value for" + CopyStatusProbeBase.DBGuidAttrName);
		}

		protected override void DoWork(CancellationToken cancellationToken)
		{
			if (HighAvailabilityUtility.CheckCancellationRequested(cancellationToken))
			{
				base.Result.StateAttribute1 = "Cancellation Requested!";
				return;
			}
			if (!base.Definition.Attributes.ContainsKey("dbGuid") || string.IsNullOrWhiteSpace(base.Definition.Attributes["dbGuid"]))
			{
				throw new HighAvailabilityMAProbeException("Probe Attribute 'dbGuid' is Undefined, Null or Empty");
			}
			Guid databaseGuid = new Guid(base.Definition.Attributes["dbGuid"]);
			this.copyStatusEntry = CopyStatusProbeBase.CopyStatusEntry.ConstructFromMdbGuid(databaseGuid);
			base.Result.StateAttribute1 = base.Definition.Name;
			base.Result.StateAttribute2 = string.Format("{0}\\{1}", this.CopyStatus.DatabaseName, Environment.MachineName);
			base.Result.StateAttribute3 = this.CopyStatus.CopyStatus.ToString();
			base.Result.StateAttribute4 = string.Format("Status Timestamp - {0}", this.CopyStatus.CopyStatusTimestamp.ToString());
			base.Result.StateAttribute5 = string.Format("ReplayLagConfigured - {0}", this.CopyStatus.ReplayLagEnabled);
			base.Result.StateAttribute6 = (double)this.CopyStatus.CopyQueueLength;
			base.Result.StateAttribute7 = (double)this.CopyStatus.ReplayQueueLength;
		}

		private CopyStatusProbeBase.CopyStatusEntry copyStatusEntry;

		public static readonly string DBGuidAttrName = "dbGuid";

		protected class CopyStatusEntry
		{
			private CopyStatusEntry(Guid databaseGuid)
			{
				this.databaseGuid = databaseGuid;
				this.databaseName = CachedAdReader.Instance.GetDatabaseOnLocalServer(this.databaseGuid).Name;
				this.ConstructEntry();
			}

			public CopyStatusEnum CopyStatus
			{
				get
				{
					return this.currentCopyStatus;
				}
			}

			public DateTime CopyStatusTimestamp
			{
				get
				{
					return this.currentCopyStatusTimestamp;
				}
			}

			public string DatabaseName
			{
				get
				{
					return this.databaseName;
				}
			}

			public Guid DatabaseGuid
			{
				get
				{
					return this.databaseGuid;
				}
			}

			public long CopyQueueLength
			{
				get
				{
					return this.currentCopyQueueLength;
				}
			}

			public long ReplayQueueLength
			{
				get
				{
					return this.currentReplayQueueLength;
				}
			}

			public ReplayLagEnabledEnum ReplayLagEnabled
			{
				get
				{
					return this.currentReplayLagEnabled;
				}
			}

			public static CopyStatusProbeBase.CopyStatusEntry ConstructFromMdbGuid(Guid databaseGuid)
			{
				return new CopyStatusProbeBase.CopyStatusEntry(databaseGuid);
			}

			private void ConstructEntry()
			{
				CopyStatusClientCachedEntry dbCopyStatusOnLocalServer = CachedDbStatusReader.Instance.GetDbCopyStatusOnLocalServer(this.databaseGuid);
				DateTime utcNow = DateTime.UtcNow;
				if (dbCopyStatusOnLocalServer == null)
				{
					throw new HighAvailabilityMAProbeException(string.Format("Unable to find copy status for database {0}!", this.databaseName));
				}
				if (dbCopyStatusOnLocalServer.Result != CopyStatusRpcResult.Success)
				{
					throw new HighAvailabilityMAProbeException(string.Format("GetCopyStatus RPC Error! Database {0}, RpcResult={1}, RpcTargetServer={2}", this.databaseName, dbCopyStatusOnLocalServer.Result, dbCopyStatusOnLocalServer.ServerContacted));
				}
				this.currentCopyStatus = dbCopyStatusOnLocalServer.CopyStatus.CopyStatus;
				this.currentCopyQueueLength = dbCopyStatusOnLocalServer.CopyStatus.GetCopyQueueLength();
				this.currentReplayQueueLength = dbCopyStatusOnLocalServer.CopyStatus.GetReplayQueueLength();
				this.currentReplayLagEnabled = dbCopyStatusOnLocalServer.CopyStatus.ReplayLagEnabled;
				this.currentCopyStatusTimestamp = dbCopyStatusOnLocalServer.CopyStatus.LastStatusTransitionTime.ToUniversalTime();
			}

			private readonly Guid databaseGuid;

			private readonly string databaseName;

			private CopyStatusEnum currentCopyStatus;

			private DateTime currentCopyStatusTimestamp;

			private long currentCopyQueueLength;

			private long currentReplayQueueLength;

			private ReplayLagEnabledEnum currentReplayLagEnabled;
		}
	}
}
