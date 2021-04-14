using System;
using Microsoft.Exchange.Cluster.ActiveManagerServer;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;
using Microsoft.Exchange.Rpc.ActiveManager;
using Microsoft.Isam.Esent.Interop;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal sealed class SourceReplicaInstance : ReplicaInstance
	{
		public SourceReplicaInstance(ReplayConfiguration replayConfiguration, ReplicaInstance previousReplicaInstance, IPerfmonCounters perfCounters) : base(replayConfiguration, false, previousReplicaInstance, perfCounters)
		{
			ReplicaInstance.DisposeIfActionUnsuccessful(delegate
			{
				this.TraceDebug("object created");
				ExTraceGlobals.PFDTracer.TracePfd<int>((long)this.GetHashCode(), "PFD CRS {0} SourceReplicaInstance is created", 32029);
				base.InitializeSuspendState();
				base.CurrentContext.InitializeForSource();
			}, this);
		}

		internal override bool GetSignatureAndCheckpoint(out JET_SIGNATURE? logfileSignature, out long lowestGenerationRequired, out long highestGenerationRequired, out long lastGenerationBackedUp)
		{
			logfileSignature = base.FileChecker.FileState.LogfileSignature;
			lowestGenerationRequired = 0L;
			highestGenerationRequired = 0L;
			lastGenerationBackedUp = 0L;
			if (logfileSignature == null)
			{
				base.FileChecker.TryUpdateActiveDatabaseLogfileSignature();
				logfileSignature = base.FileChecker.FileState.LogfileSignature;
			}
			return logfileSignature != null;
		}

		internal override AmAcllReturnStatus AttemptCopyLastLogsRcr(AmAcllArgs acllArgs, AcllPerformanceTracker acllPerf)
		{
			throw new AcllInvalidForActiveCopyException(base.Configuration.DisplayName);
		}

		protected override bool ConfigurationCheckerInternal()
		{
			this.TraceDebug("ConfigurationCheckerInternal()");
			base.CheckEdbLogDirectoryUnderMountPoint();
			base.CheckEdbLogDirectoriesIfNeeded();
			base.CheckInstanceAbortRequested();
			FileOperations.RemoveDirectory(base.Configuration.LogInspectorPath);
			string[] directoriesToCreate = new string[]
			{
				base.Configuration.SourceLogPath,
				base.Configuration.LogInspectorPath,
				base.Configuration.E00LogBackupPath
			};
			base.CreateDirectories(directoriesToCreate);
			base.CheckInstanceAbortRequested();
			string[] directoriesToCheck = new string[]
			{
				base.Configuration.SourceLogPath,
				base.Configuration.SourceSystemPath
			};
			base.CheckDirectories(directoriesToCheck);
			base.CheckInstanceAbortRequested();
			base.FileChecker.TryUpdateActiveDatabaseLogfileSignature();
			return true;
		}

		protected override void StartComponents()
		{
			ReplayState replayState = base.Configuration.ReplayState;
			ExTraceGlobals.ReplicaInstanceTracer.TraceDebug<string>((long)this.GetHashCode(), "Starting SourceReplicaInstance: {0}", base.Configuration.Identity);
			ExTraceGlobals.PFDTracer.TracePfd<int, string>((long)this.GetHashCode(), "PFD CRS {0} Starting SourceReplicaInstance: {1}", 23837, base.Configuration.Name);
			ReplayEventLogConstants.Tuple_SourceInstanceStart.LogEvent(null, new object[]
			{
				base.Configuration.ServerName,
				base.Configuration.SourceMachine,
				base.Configuration.Type,
				base.Configuration.Name
			});
			this.TraceDebug("started");
			ExTraceGlobals.PFDTracer.TracePfd<int, ReplayConfigType, string>((long)this.GetHashCode(), "PFD CRS {0} SourceReplicaInstance started {1} {2}", 19741, base.Configuration.Type, base.Configuration.Name);
		}

		protected override void PrepareToStopInternal()
		{
			this.TraceDebug("PrepareToStopInternal()");
		}

		protected override void StopInternal()
		{
			if (base.StartedComponents)
			{
				ReplayEventLogConstants.Tuple_SourceInstanceStop.LogEvent(null, new object[]
				{
					base.Configuration.ServerName,
					base.Configuration.SourceMachine,
					base.Configuration.Type,
					base.Configuration.Name
				});
			}
			this.TraceDebug("StopInternal()");
			ExTraceGlobals.PFDTracer.TracePfd<int, ReplayConfigType, string>((long)this.GetHashCode(), "PFD CRS {0} SourceReplicaInstance is stopped {1} {2}", 29341, base.Configuration.Type, base.Configuration.Name);
		}

		protected override void TraceDebug(string message)
		{
			ExTraceGlobals.ReplicaInstanceTracer.TraceDebug<string, string>((long)this.GetHashCode(), "SourceReplicaInstance {0}: {1}", base.Configuration.Name, message);
		}

		protected override void TraceError(string message)
		{
			ExTraceGlobals.ReplicaInstanceTracer.TraceError<string, string>((long)this.GetHashCode(), "SourceReplicaInstance {0}: {1}", base.Configuration.Name, message);
		}
	}
}
