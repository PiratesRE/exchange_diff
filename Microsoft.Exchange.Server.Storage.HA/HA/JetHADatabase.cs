using System;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Common.HA;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.HA;
using Microsoft.Exchange.EseRepl;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Server.Storage.BlockMode;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.PhysicalAccessJet;
using Microsoft.Win32;

namespace Microsoft.Exchange.Server.Storage.HA
{
	[CLSCompliant(false)]
	public class JetHADatabase : JetDatabase
	{
		internal JetHADatabase(Guid dbGuid, string displayName, string logPath, string filePath, string fileName, DatabaseFlags databaseFlags, DatabaseOptions databaseOptions) : base(dbGuid, displayName, logPath, filePath, fileName, databaseFlags, databaseOptions)
		{
			if (JetHADatabase.IsBlockModeEnabled())
			{
				this.collector = new BlockModeCollector(dbGuid, displayName);
			}
		}

		[CLSCompliant(false)]
		public static Trace Tracer
		{
			get
			{
				return ExTraceGlobals.JetHADatabaseTracer;
			}
		}

		public ThrottlingData ThrottlingData
		{
			get
			{
				ThrottlingData throttlingData = null;
				if (this.collector != null)
				{
					throttlingData = this.collector.ThrottlingData;
				}
				if (throttlingData == null)
				{
					throttlingData = this.unhealthyThrottlingData;
					if (throttlingData == null)
					{
						throttlingData = new ThrottlingData();
						throttlingData.MarkFailed();
						this.unhealthyThrottlingData = throttlingData;
					}
				}
				return throttlingData;
			}
		}

		internal LastLogWriter LastLogWriter
		{
			get
			{
				return this.lastLogWriter;
			}
		}

		[CLSCompliant(false)]
		public static Database JetHADatabaseCreator(Guid dbGuid, string displayName, string logPath, string filePath, string fileName, DatabaseFlags databaseFlags, DatabaseOptions databaseOptions)
		{
			return new JetHADatabase(dbGuid, displayName, logPath, filePath, fileName, databaseFlags, databaseOptions);
		}

		[CLSCompliant(false)]
		public static Factory.JetHADatabaseCreator GetCreator()
		{
			return new Factory.JetHADatabaseCreator(JetHADatabase.JetHADatabaseCreator);
		}

		public static bool IsBlockModeEnabled()
		{
			IRegistryReader instance = RegistryReader.Instance;
			return instance.GetValue<int>(Registry.LocalMachine, "Software\\Microsoft\\ExchangeServer\\v15\\Replay\\Parameters", "DisableGranularReplication", 0) == 0;
		}

		[CLSCompliant(false)]
		public ErrorCode StartBlockModeReplicationToPassive(IExecutionDiagnostics context, string passiveName, uint firstGenToSend)
		{
			Exception ex = null;
			try
			{
				if (this.collector == null)
				{
					throw new GranularReplicationInitFailedException("BlockMode is disabled");
				}
				this.collector.StartReplicationToPassive(passiveName, firstGenToSend);
			}
			catch (NetworkTransportException ex2)
			{
				ex = ex2;
				context.OnExceptionCatch(ex);
			}
			catch (NetworkRemoteException ex3)
			{
				ex = ex3;
				context.OnExceptionCatch(ex);
			}
			catch (GranularReplicationInitFailedException ex4)
			{
				ex = ex4;
				context.OnExceptionCatch(ex);
			}
			if (ex != null)
			{
				context.OnExceptionCatch(ex);
				JetHADatabase.Tracer.TraceError<string, string, Exception>((long)this.GetHashCode(), "StartBlockModeReplicationToPassive({0}\\{1}) caught {2}", base.DisplayName, passiveName, ex);
				ReplayCrimsonEvents.ActiveFailedToEnterBlockMode.Log<string, string, string, string>(base.DisplayName, passiveName, ex.Message, ex.ToString());
				return ErrorCode.CreateBlockModeInitFailed((LID)58312U);
			}
			return ErrorCode.NoError;
		}

		internal override void PublishHaFailure(FailureTag failureTag)
		{
			FailureItem.PublishHaFailure(this.DbGuid, base.DisplayName, failureTag);
		}

		protected override void PrepareToMountAsActive()
		{
			if (this.collector != null)
			{
				this.collector.PrepareToMountAsActive(base.JetInstance);
			}
		}

		protected override void PrepareToMountAsPassive()
		{
			if (this.collector != null)
			{
				this.collector.PrepareToMountAsPassive(base.JetInstance);
			}
		}

		protected override void PrepareToTransitionToActive()
		{
			base.PrepareToTransitionToActive();
			if (this.collector != null)
			{
				this.collector.PrepareToTransitionToActive();
			}
		}

		protected override void JetInitComplete()
		{
			this.StartLastLogWriter();
		}

		protected override void DismountBegins()
		{
			this.StopLastLogWriter();
		}

		protected override void DismountComplete()
		{
			if (this.collector != null)
			{
				this.collector.DismountComplete();
			}
		}

		private void StartLastLogWriter()
		{
			int num = LastLogWriter.ReadUpdateInterval();
			if (num > 0)
			{
				this.lastLogWriter = new LastLogWriter(this.DbGuid, base.DisplayName, base.JetInstance, base.DatabaseFile);
				this.lastLogWriter.Start(num);
			}
		}

		private void StopLastLogWriter()
		{
			if (this.lastLogWriter != null)
			{
				this.lastLogWriter.Stop();
				this.lastLogWriter = null;
			}
		}

		private ThrottlingData unhealthyThrottlingData;

		private BlockModeCollector collector;

		private LastLogWriter lastLogWriter;
	}
}
